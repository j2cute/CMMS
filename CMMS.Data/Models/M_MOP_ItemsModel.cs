using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class M_MOP_ItemsModel
    {
        public int MOP_ItemsId { get; set; }
        public Nullable<int> SiteId { get; set; }
        [Range(1, 1000, ErrorMessage = "The field {0} must be 1 or greater than {1}.")]
        public Nullable<int> SR_Qty { get; set; }

        public Nullable<int> UR_Qty { get; set; }
        public string PMS_No { get; set; }
        public string MOP_No { get; set; }
        public string Part_No { get; set; }
        public string NewSelectedPart_No { get; set; }
    }
    [MetadataType(typeof(M_MOP_ItemsModel))]
    public partial class M_MOP_ITEMS
    {
        public string NewSelectedPart_No { get; set; }
    }
}
