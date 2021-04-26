using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.API.Data;
using Contacts.API.Data.Models;
using Contacts.API.Data.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Contacts.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        #region Fields
        private readonly IDataRepository _dataRepository;
        private readonly IContactCache _cache;
        private readonly IConfiguration _configuration;

        private int _statusCode;
        private string _errorText;
        #endregion

        #region Ctor
        public ContactsController(IDataRepository dataRepository, IContactCache contactCache, IConfiguration configuration)
        {
            _dataRepository = dataRepository;
            _cache = contactCache;
            _configuration = configuration;
        }
        #endregion

        #region GET

        [HttpGet("{contactID}")]
        public async Task<ActionResult<ContactModel>> GetContact(int contactID)
        {
            var contact = _cache.Get(contactID);
            if (contact == null)
            {
                contact = await _dataRepository.GetContact(contactID);
                if (contact == null)
                {
                    return NotFound();
                }
                _cache.Set(contact);
            }
            return contact;
        }

        [HttpGet]
        public async Task<IEnumerable<ContactsListModel>> GetContacts(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await _dataRepository.GetContacts();
            }
            else
            {
                return await _dataRepository.GetContactsSearch(searchText);
            }
        }

        [HttpGet("deletedContacts")]
        public async Task<IEnumerable<ContactsListModel>> GetContactsDeleted()
        {
            return await _dataRepository.GetContactsDeleted();
        }

        [HttpGet("favoritedContacts")]
        public async Task<IEnumerable<ContactsListModel>> GetContactsFavorited()
        {
            return await _dataRepository.GetContactsFavorited();
        }

        [HttpGet("searchByDetails")]
        public async Task<IEnumerable<ContactsListModel>> GetContactsSearchByDetails(string firstname, string lastname, DateTime dob, string address, string phoneNumber)
        {
            return await _dataRepository.GetContactsSearchByDetails(firstname, lastname, dob, address, phoneNumber);
        }

        [HttpGet("city")]
        public async Task<IEnumerable<CitiesListModel>> GetCities()
        {
            var cityList = await _dataRepository.GetCities();
            if (cityList == null)
            {
                return (IEnumerable<CitiesListModel>)NotFound();
            }
            return cityList;
        }

        [HttpGet("gender")]
        public async Task<IEnumerable<GenderListModel>> GetGender()
        {
            var genderList = await _dataRepository.GetGender();
            if (genderList == null)
            {
                return (IEnumerable<GenderListModel>)NotFound();
            }
            return genderList;
        }

        [HttpGet("phoneNumberTypes")]
        public async Task<IEnumerable<PhoneNumberTypesListModel>> GetPhoneNumberTypes()
        {
            var typesList = await _dataRepository.GetPhoneNumberTypes();
            if (typesList == null)
            {
                return (IEnumerable<PhoneNumberTypesListModel>)NotFound();
            }
            return typesList;
        }

        #endregion

        #region Post/Put

        [HttpPost]
        public async Task<ActionResult> PostContact(ContactModel contactModel)
        {
            var validation = await Validate(contactModel, false);
            if (!validation)
            {
                return StatusCode(_statusCode, _errorText);
            }
            else
            {
                var savedContact = await _dataRepository.PostContact(contactModel);
                return CreatedAtAction(nameof(GetContact), new { contactID = savedContact.ContactID }, savedContact);
            }
        }

        [HttpPut("{contactID}")]
        public async Task<ActionResult<ContactModel>> PutContact(int contactID, ContactModel contactModel)
        {
            if (contactID == 0)
            {
                return StatusCode(400, "Please pass Contact ID");
            }

            var contact = await _dataRepository.GetContact(contactID);
            if (contact == null)
            {
                return NotFound();
            }

            var validation = await Validate(contactModel, true);
            if (!validation)
            {
                return StatusCode(_statusCode, _errorText);
            }
            else
            {
                contactModel.Firstname = string.IsNullOrEmpty(contactModel.Firstname) ? contact.Firstname : contactModel.Firstname;
                contactModel.Lastname = string.IsNullOrEmpty(contactModel.Lastname) ? contact.Lastname : contactModel.Lastname;
                contactModel.DOB = contactModel.DOB == DateTime.MinValue ? contact.DOB : contactModel.DOB;
                contactModel.ContactPhoto = contactModel.ContactPhoto == null ? contact.ContactPhoto : contactModel.ContactPhoto;
                contactModel.CityID = contactModel.CityID == 0 ? contact.CityID : contactModel.CityID;
                contactModel.GenderID = contactModel.GenderID == 0 ? contact.GenderID : contactModel.GenderID;
                contactModel.Address = string.IsNullOrEmpty(contactModel.Address) ? contact.Address : contactModel.Address;
                contactModel.AdditionalDetails = string.IsNullOrEmpty(contactModel.AdditionalDetails) ? contact.AdditionalDetails : contactModel.AdditionalDetails;
                contactModel.CityName = string.IsNullOrEmpty(contactModel.CityName) ? contact.CityName : contactModel.CityName;

                var savedContact = await _dataRepository.PutContact(contactModel, contactID);
                _cache.Remove(savedContact.ContactID);

                return savedContact;
            }
        }

        [HttpDelete("{contactID}")]
        public async Task<ActionResult> DeleteContact(int contactID)
        {
            var contact = await _dataRepository.GetContact(contactID);
            if (contact == null)
            {
                return NotFound();
            }
            await _dataRepository.DeleteContacts(contactID);
            _cache.Remove(contactID);

            return NoContent();
        }

        [HttpPut("addFavorite/{contactID}")]
        public async Task<ActionResult> ContactsAddFavorite(int contactID)
        {
            var contact = await _dataRepository.GetContact(contactID);
            if (contact == null)
            {
                return NotFound();
            }

            await _dataRepository.ContactsAddFavorite(contactID);
            _cache.Remove(contactID);

            return NoContent();
        }

        #endregion

        #region Validation

        public async Task<bool> Validate(ContactModel contactModel, bool isUpdate)
        {
            if (CheckMultiLingual(contactModel.Firstname + contactModel.Lastname))
            {
                _statusCode = 400;
                _errorText = "Please use only English, or only Georgian letters for Firstname and Lastname.";
                return false;
            }

            if (contactModel.CityActionID == true && string.IsNullOrWhiteSpace(contactModel.CityName))
            {
                _statusCode = 400;
                _errorText = "City Name must be filled in.";
                return false;
            }
            else if (contactModel.CityID > 0 && contactModel.CityActionID == true)
            {
                _statusCode = 400;
                _errorText = "You can not add another City, if it's already choosen.";
                return false;
            }
            else if (contactModel.CityActionID == true)
            {
                if (contactModel.CityName.Any(e => IsGeorgian(e)))
                {
                    _statusCode = 400;
                    _errorText = "Please enter only English letters for city name.";
                    return false;
                }
                var cityExist = await _dataRepository.CityExists(contactModel.CityName);
                if (cityExist)
                {
                    _statusCode = 409;
                    _errorText = $"City name: '{contactModel.CityName}' already exists.";
                    return false;
                }
            }


            if (contactModel.PhoneNumberTypeActionID == true && string.IsNullOrWhiteSpace(contactModel.PhoneNumberTypeName))
            {
                _statusCode = 400;
                _errorText = "Phone number type Name must be filled in.";
                return false;
            }
            else if (contactModel.PhoneNumberTypeActionID == true && string.IsNullOrWhiteSpace(contactModel.OtherPhoneNumber))
            {
                _statusCode = 400;
                _errorText = "Phone number must be filled.";
                return false;
            }
            else if (contactModel.PhoneNumberTypeActionID == true)
            {
                var phoneNumberTypeNameExist = await _dataRepository.PhoneNumberTypeExists(contactModel.PhoneNumberTypeName); ;
                if (phoneNumberTypeNameExist)
                {
                    _statusCode = 409;
                    _errorText = $"Phone number type name: '{contactModel.PhoneNumberTypeName}' already exists.";
                    return false;
                }
            }

            if (!PhoneNumbersIsValid(contactModel, isUpdate))
            {
                return false;
            }

            return true;
        }

        public bool PhoneNumbersIsValid(ContactModel contactModel, bool isUpdate)
        {
            if (isUpdate)
            {
                if (contactModel.PhoneNumbers == null)
                {
                    _statusCode = 400;
                    _errorText = "Please add at least one Phone Number.";
                    return false;
                }
                else if (contactModel.PhoneNumbers.Any(pn => pn.PhoneNumberTypeID == 0 | String.IsNullOrWhiteSpace(pn.PhoneNumber)))
                {
                    _statusCode = 400;
                    _errorText = "Please select Phone Type, or add the Phone Number.";
                    return false;
                }
            }
            else
            {
                if (contactModel.PhoneNumbersPost == null)
                {
                    _statusCode = 400;
                    _errorText = "Please add at least one Phone Number.";
                    return false;
                }
                else if (contactModel.PhoneNumbersPost.Any(pn => pn.PhoneNumberTypeID == 0 | String.IsNullOrWhiteSpace(pn.PhoneNumber)))
                {
                    _statusCode = 400;
                    _errorText = "Please select Phone Type, or add the Phone Number.";
                    return false;
                }
            }
            return true;
        }

        public static bool CheckMultiLingual(string text)
        {
            return text.Any(e => IsEnglish(e)) && text.Any(e => IsGeorgian(e));
        }

        public static bool IsEnglish(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public static bool IsGeorgian(char c)
        {
            return c >= 'ა' && c <= 'ჰ';
        }

        #endregion

        [AllowAnonymous]
        [HttpGet]
        [Route("GenerateToken")]
        public string GenerateToken()
        {
            var claims = new[]
            {
                        //new Claim("PersonalId", userLoginFromDb.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("UserID", "Dvali")
            };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Token:Issuer"],
                audience: _configuration["Token:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), //TODO: უნდა შემცირდეს 60 წუთზე შემდეგ
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(_configuration["Token:Key"])),
                        SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
