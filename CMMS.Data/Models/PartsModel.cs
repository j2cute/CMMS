using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class PartsModel
    {
        public int PartId { get; set; }
        [MaxLength(50, ErrorMessage = "Maximum Length Should be Equal to 50 Characters")]
        public string Part_No { get; set; }
        //[RegularExpression("^[a-zA-Z0-9]{5}$", ErrorMessage = "Max length should be 5, Alphanumeric Characters")]
        public string CageCode { get; set; }
        [MaxLength(100, ErrorMessage = "Maximum Length Should be Equal to 100 Characters")]
        public string PART_NAME { get; set; }
        [RegularExpression("^[a-zA-Z0-9]{11}$", ErrorMessage = "Max length should be 11, Alphanumeric Characters")]
        public string NSN { get; set; }
        public Nullable<decimal> UNIT_PRICE { get; set; }
        public string CurrencyID { get; set; }
        public string MCAT_ID { get; set; }
        public string PartTypeID { get; set; }
        public Nullable<decimal> LENGTH { get; set; }
        public Nullable<decimal> WIDTH { get; set; }
        public Nullable<decimal> HEIGHT { get; set; }
        public Nullable<decimal> WEIGHT { get; set; }
        public Nullable<decimal> DIAMETER { get; set; }
        public Nullable<int> PART_MEC { get; set; }
        public Nullable<decimal> MTBF { get; set; }
        public Nullable<decimal> MTTR { get; set; }
        [MaxLength(6, ErrorMessage = "Alphabets only"), MinLength(5, ErrorMessage = "Alphabets only")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Please Enter Alphabets only")]
        public string SMR { get; set; }
        public Nullable<decimal> BRF { get; set; }
        public string PART_CHARACTERISTIC { get; set; }
        public string PICTURE_FILE_NAME { get; set; }
        public string File_Path { get; set; }
        public string FileExtension { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<System.DateTime> ModifiedOnDate { get; set; }
        public string CreatedByUser { get; set; }
        public string ModifiedByUser { get; set; }
        public string Status { get; set; }
        public string base64Pic { get; set; }
        public string PartTypeName { get; set; }
    }
    [MetadataType(typeof(PartsModel))]
    public partial class tbl_Parts
    {
        public string base64Pic { get; set; }
        public string PartTypeName { get; set; }
    }
}
