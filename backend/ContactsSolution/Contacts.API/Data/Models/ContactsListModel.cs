using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.API.Data.Models
{
    public class ContactsListModel
    {
        public int ContactID { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string CityName { get; set; }
        public string GenderName { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public bool IsFavorite { get; set; }
        public byte[] ContactPhoto { get; set; }
        public string AdditionalDetails { get; set; }
    }
}
