using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class CageModel
    {
        public int CageId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^[a-zA-Z0-9]{5}$", ErrorMessage = "Max length should be 5, Alphanumeric Characters")]
        public string CageCode { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string CageName { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string Address { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string City { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string Country { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string PostalCode { get; set; }
        public string Status { get; set; }

    }
    [MetadataType(typeof(CageModel))]
    public partial class tbl_Cage
    {


    }
}
