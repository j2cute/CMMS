using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class ChildPartsModel
    {
        public Nullable<int> ParentPartId { get; set; }
        public int ChildPartId { get; set; }
        public string APLNumber { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Status { get; set; }
        public string ChildPartName { get; set; }
        public string ChildPartNo { get; set; }
    }
    [MetadataType(typeof(ChildPartsModel))]
    public partial class tbl_ChildParts
    {
        public string ChildPartName { get; set; }
        public string ChildPartNo { get; set; }
    }
}
