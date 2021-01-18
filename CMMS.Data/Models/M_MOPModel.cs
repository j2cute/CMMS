using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class M_MOPModel
    {
        public int SiteId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^[0-9]{1}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "format:1-1111-1111")]
        public string PMS_No { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "Fix 3 Digits only")]
        public string MOP_No { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(250, ErrorMessage = "Maximum Length Should be Equal to 250 Characters")]

        public string MOP_Desc { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(10, ErrorMessage = "Maximum Length Should be Equal to 10 Characters")]

        public string By_Whom { get; set; }
        public Nullable<int> Periodicity { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Period { get; set; }
        [MaxLength(1, ErrorMessage = "Only single alphanumeric character can be entered")]
        public string Doc { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Task_Procedure { get; set; }
        public string Safety_Precautions { get; set; }
        public string PeriodMonth { get; set; }
        public string mmsDoc { get; set; }
    }
    [MetadataType(typeof(M_MOPModel))]
    public partial class M_MOP
    {
        public string PeriodMonth { get; set; }
        public string mmsDoc { get; set; }
    }
}
