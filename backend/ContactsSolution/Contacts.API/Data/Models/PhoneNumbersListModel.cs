using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.API.Data.Models
{
    public class PhoneNumbersListModel
    {
        public int ID { get; set; }
        public int PhoneNumberTypeID { get; set; }
        public string PhoneNumber { get; set; }
    }
}
