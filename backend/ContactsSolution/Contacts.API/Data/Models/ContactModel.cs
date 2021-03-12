using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.API.Data.Models
{
    public class ContactModel
    {
        public int? ContactID { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is required.")]
        public int GenderID { get; set; }

        public int CityID { get; set; }

        [Display(Name = "Firstname")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Length betwwen 3 and 100")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        [Required(ErrorMessage = "Firstname is required.")]
        public string Firstname { get; set; }

        [Display(Name = "Lastname")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Length betwwen 3 and 100")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        [Required(ErrorMessage = "Lastname is required.")]
        public string Lastname { get; set; }


        [Display(Name = "Birth Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime DOB { get; set; }

        [StringLength(500, ErrorMessage = "Length less then 500")]
        public string Address { get; set; }

        public byte[] ContactPhoto { get; set; }

        public bool IsFavorite { get; set; }

        [StringLength(500, ErrorMessage = "Length less then 500")]
        public string AdditionalDetails { get; set; }

        public bool CityActionID { get; set; }

        [StringLength(200, ErrorMessage = "Length less then 200")]
        public string CityName { get; set; }

        public bool PhoneNumberTypeActionID { get; set; }

        [StringLength(100, ErrorMessage = "Length less then 100")]
        public string PhoneNumberTypeName { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "Between 6-20")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed")]
        [DataType(DataType.PhoneNumber)]
        public string OtherPhoneNumber { get; set; }

        public IEnumerable<PhoneNumbersListModel> PhoneNumbers { get; set; }

        public IEnumerable<PhoneNumbersListPostModel> PhoneNumbersPost { get; set; }

    }
}
