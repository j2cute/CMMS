using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class RequestModel
    {
        public int RequestId { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string Part_No { get; set; }
        [RegularExpression("^[a-zA-Z0-9]{5}$", ErrorMessage = "Max length should be 5, Alphanumeric Characters")]
        public string CageCode { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string ManufacturerName { get; set; }
        [MaxLength(100, ErrorMessage = "Maximum Length Should be Equal to 100 Characters")]
        public string Equipment_Name { get; set; }
        public string NSN { get; set; }
        public Nullable<decimal> UNIT_PRICE { get; set; }
        public string CurrencyID { get; set; }
        public Nullable<decimal> LENGTH { get; set; }
        public Nullable<decimal> WIDTH { get; set; }
        public Nullable<decimal> HEIGHT { get; set; }
        public Nullable<decimal> WEIGHT { get; set; }
        public Nullable<decimal> DIAMETER { get; set; }
        public Nullable<int> PART_MEC { get; set; }
        public Nullable<decimal> MTBF { get; set; }
        public Nullable<decimal> MTTR { get; set; }
        public string SMR { get; set; }
        public Nullable<decimal> BRF { get; set; }
        public string PART_CHARACTERISTIC { get; set; }
        public string PICTURE_FILE_NAME { get; set; }
        public string File_Path { get; set; }
        public string FileExtension { get; set; }
        public string Country { get; set; }
        public Nullable<System.DateTime> DateOfInstallation { get; set; }
        public Nullable<int> Qty { get; set; }
        [RegularExpression("^[0-9]{1}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "format:1-1111-1111")]
        public string PMS_No { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string SERIAL_NO { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string JUSTIFICATION { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string DETAIL_REMARKS { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string LOCATIONS { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string NAME { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string DESIGNATION { get; set; }
        public Nullable<int> CONTACT { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<System.DateTime> ModifiedOnDate { get; set; }
        public string CreatedByUser { get; set; }
        public string ModifiedByUser { get; set; }
        public Nullable<int> Status { get; set; }
    }
    [MetadataType(typeof(RequestModel))]
    public partial class tbl_Request
    {

    }
}
