using Contacts.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.API.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<ContactsListModel>> GetContacts();
        Task<IEnumerable<ContactsListModel>> GetContactsFavorited();
        Task<IEnumerable<ContactsListModel>> GetContactsDeleted();
        Task<IEnumerable<ContactsListModel>> GetContactsSearch(string searchText);
        Task<IEnumerable<ContactsListModel>> GetContactsSearchByDetails(string firstname, string lastname, DateTime dob, string address, string phoneNumber);
        Task<IEnumerable<GenderListModel>> GetGender();
        Task<IEnumerable<CitiesListModel>> GetCities();
        Task<IEnumerable<PhoneNumberTypesListModel>> GetPhoneNumberTypes();
        Task<ContactModel> GetContact(int contactID);
        Task<ContactModel> PostContact(ContactModel contactModel);
        Task<ContactModel> PutContact(ContactModel contactModel, int contactID);
        Task DeleteContacts(int contactID);
        Task ContactsAddFavorite(int contactID);
        Task<bool> PhoneNumberTypeExists(string typeName);
        Task<bool> CityExists(string cityName);
    }
}
