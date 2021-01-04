using System;
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
        public string ESWBS { get; set; }
        public string PESWBS { get; set; }
        public string Nomanclature { get; set; }
        public string Title { get; set; }
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



 