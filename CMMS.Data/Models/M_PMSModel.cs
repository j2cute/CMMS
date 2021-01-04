using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class M_PMSModel
    {
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("^[0-9]{1}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "format:1-1111-1111")] 
        public string PMS_NO { get; set; }
        public string PMS_DESC { get; set; }
    }
    [MetadataType(typeof(M_PMSModel))]
    public partial class M_PMS
    {


    }
}
