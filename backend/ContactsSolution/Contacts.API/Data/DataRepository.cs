using Contacts.API.Data.Helpers;
using Contacts.API.Data.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;


namespace Contacts.API.Data
{
    public class DataRepository : IDataRepository
    {

        #region Fields
        private readonly string _connectionString;
        #endregion

        #region Ctor
        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        #endregion

        #region Read Data

        public async Task<ContactModel> GetContact(int contactID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (GridReader results = await connection.QueryMultipleAsync(@"EXEC dbo.Contact_Get @ContactID = @ContactID; EXEC dbo.PhoneNumbers_GetList_ByContactID @ContactID = @ContactID", new { ContactID = contactID }))
                {
                    var contact = results.Read<ContactModel>().FirstOrDefault();
                    if (contact != null)
                    {
                        contact.PhoneNumbers = results.Read<PhoneNumbersListModel>().ToList();
                    }
                    return contact;
                }
            }
        }

        public async Task<IEnumerable<ContactsListModel>> GetContacts()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ContactsListModel>("dbo.Contact_GetList", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<ContactsListModel>> GetContactsDeleted()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ContactsListModel>("dbo.Contact_Deleted_GetList", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<ContactsListModel>> GetContactsFavorited()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ContactsListModel>("dbo.Contact_Favorited_GetList", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<ContactsListModel>> GetContactsSearch(string searchText)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@SearchText", searchText == null ? null : searchText.Trim());

                return await connection.QueryAsync<ContactsListModel>("dbo.Contact_Search", queryParameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<ContactsListModel>> GetContactsSearchByDetails(string firstname, string lastname, DateTime dob, string address, string phoneNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Firstname", firstname == null ? null : firstname.Trim());
                queryParameters.Add("@Lastname", lastname == null ? null : lastname.Trim());
                queryParameters.Add("@DOB", dob == DateTime.MinValue ? null : dob);
                queryParameters.Add("@Address", address == null ? null : address.Trim());
                queryParameters.Add("@MobilePhone", phoneNumber == null ? null : phoneNumber.Trim());

                return await connection.QueryAsync<ContactsListModel>("dbo.Contact_Search_Detailed", queryParameters, commandType: CommandType.StoredProcedure);

            }
        }

        //Dropdown
        public async Task<IEnumerable<CitiesListModel>> GetCities()
        {
            IEnumerable<CitiesListModel> cityList = new List<CitiesListModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                cityList = await connection.QueryAsync<CitiesListModel>("dbo.City_GetList", commandType: CommandType.StoredProcedure);
            }
            return cityList.ToList();
        }

        //Dropdown
        public async Task<IEnumerable<GenderListModel>> GetGender()
        {
            IEnumerable<GenderListModel> genderList = new List<GenderListModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                genderList = await connection.QueryAsync<GenderListModel>("dbo.Gender_GetList", commandType: CommandType.StoredProcedure);
            }
            return genderList.ToList();
        }

        //Dropdown
        public async Task<IEnumerable<PhoneNumberTypesListModel>> GetPhoneNumberTypes()
        {
            IEnumerable<PhoneNumberTypesListModel> phoneNumberTypeList = new List<PhoneNumberTypesListModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                phoneNumberTypeList = await connection.QueryAsync<PhoneNumberTypesListModel>("dbo.PhoneNumberType_GetList", commandType: CommandType.StoredProcedure);
            }
            return phoneNumberTypeList.ToList();
        }

        public async Task<bool> CityExists(string cityName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@CityName", cityName.Trim());

                return await connection.ExecuteScalarAsync<bool>("dbo.City_Exists", queryParameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> PhoneNumberTypeExists(string typeName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@TypeName", typeName.Trim());

                return await connection.ExecuteScalarAsync<bool>("dbo.PhoneNumberType_Exists", queryParameters, commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region Write/Update Data

        public async Task<ContactModel> PostContact(ContactModel contactModel)
        {
            List<PhoneNumbersListPostModel> phoneNumbers = contactModel.PhoneNumbersPost.ToList();
            DataTable dt = Helpers.ConvertMethods.ListToDataTable(phoneNumbers);

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {

                    await connection.OpenAsync();

                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("@Firstname", contactModel.Firstname);
                    queryParameters.Add("@Lastname", contactModel.Lastname);
                    queryParameters.Add("@DOB", contactModel.DOB);
                    queryParameters.Add("@GenderID", contactModel.GenderID);
                    queryParameters.Add("@CityID", contactModel.CityID);
                    queryParameters.Add("@Address", contactModel.Address);
                    queryParameters.Add("@AdditionalDetails", contactModel.AdditionalDetails);
                    queryParameters.Add("@IsFavorite", contactModel.IsFavorite);
                    queryParameters.Add("@ContactPhoto", contactModel.ContactPhoto, DbType.Binary);
                    queryParameters.Add("@CityActionID", contactModel.CityActionID);
                    queryParameters.Add("@CityName", contactModel.CityName);
                    queryParameters.Add("@PhoneNumberTypeActionID", contactModel.PhoneNumberTypeActionID);
                    queryParameters.Add("@PhoneNumberTypeName", contactModel.PhoneNumberTypeName);
                    queryParameters.Add("@OtherPhoneNumber", contactModel.OtherPhoneNumber);
                    queryParameters.Add("@PhoneNumbersList", dt.AsTableValuedParameter("dbo.PhoneNumberListPost"));

                    var contactID = await connection.ExecuteScalarAsync<int>("dbo.Contact_Post", queryParameters, commandType: CommandType.StoredProcedure);

                    return await GetContact(contactID);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                return null;
            }
        }

        public async Task<ContactModel> PutContact(ContactModel contactModel, int contactID)
        {
            List<PhoneNumbersListModel> phoneNumbers = contactModel.PhoneNumbers.ToList();
            var dt = ConvertMethods.ListToDataTable(phoneNumbers);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ContactID", contactModel.ContactID);
                queryParameters.Add("@Firstname", contactModel.Firstname);
                queryParameters.Add("@Lastname", contactModel.Lastname);
                queryParameters.Add("@DOB", contactModel.DOB);
                queryParameters.Add("@GenderID", contactModel.GenderID);
                queryParameters.Add("@CityID", contactModel.CityID);
                queryParameters.Add("@Address", contactModel.Address);
                queryParameters.Add("@AdditionalDetails", contactModel.AdditionalDetails);
                queryParameters.Add("@ContactPhoto", contactModel.ContactPhoto, DbType.Binary);
                queryParameters.Add("@IsFavorite", contactModel.IsFavorite);
                queryParameters.Add("@CityActionID", contactModel.CityActionID);
                queryParameters.Add("@CityName", contactModel.CityName);
                queryParameters.Add("@PhoneNumberTypeActionID", contactModel.PhoneNumberTypeActionID);
                queryParameters.Add("@PhoneNumberTypeName", contactModel.PhoneNumberTypeName);
                queryParameters.Add("@OtherPhoneNumber", contactModel.OtherPhoneNumber);
                queryParameters.Add("@PhoneNumbersList", dt.AsTableValuedParameter("dbo.PhoneNumberList"));

                await connection.ExecuteAsync("dbo.Contact_Put", queryParameters, commandType: CommandType.StoredProcedure);

                return await GetContact(contactID);
            }
        }

        public async Task DeleteContacts(int contactID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ContactID", contactID);

                await connection.ExecuteAsync("dbo.Contact_Delete", queryParameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task ContactsAddFavorite(int contactID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ContactID", contactID);

                await connection.ExecuteAsync("dbo.Contact_AddFavorite", queryParameters, commandType: CommandType.StoredProcedure);
            }
        }

        #endregion
    }
}
