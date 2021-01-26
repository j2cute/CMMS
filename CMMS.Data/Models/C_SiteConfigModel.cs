deusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class C_SiteConfigModel
    {
        public int SiteId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(25, ErrorMessage = "Maximum Length Should be Equal to 25 Characters")]

        public string ESWBS { get; set; }
        public string PESWBS { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Nomanclature { get; set; }
        public string Title { get; set; }
        //[Required(ErrorMessage = "This field is required.")]
        [Range(1, 1000, ErrorMessage = "The field {0} must be 1 or greater than {1}.")]
        public Nullable<int> Qty { get; set; }
        public string PMS_No { get; set; }
        public string SMR_Code { get; set; }
        public Nullable<int> PartId { get; set; }
        public Nullable<int> CageId { get; set; }

        public Nullable<int> EditSelectedPartId { get; set; }
        public Nullable<int> EditSelectedCageId { get; set; }
    }
    [MetadataType(typeof(C_SiteConfigModel))]
    public partial class C_Site_Config
    {
        public Nullable<int> EditSelectedPartId { get; set; }
        public Nullable<int> EditSelectedCageId { get; set; }
    }
}



 