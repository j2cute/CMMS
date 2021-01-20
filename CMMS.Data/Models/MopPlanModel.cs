using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public partial class MopPlanModel
    {
        public MopPlanModel()
        {
            Actions = new Dictionary<string, string>();
        }
        public int SiteId { get; set; }
        public string MOP_No { get; set; }
        public string PMS_No { get; set; }
        public string ESWBS { get; set; }
        public Nullable<System.DateTime> SelectedDate { get; set; }
        public Nullable<System.DateTime> DoneDate { get; set; }
        public Nullable<System.DateTime> NextDueDate { get; set; }
        public string DoneBy { get; set; }

        public string Status { get; set; }
        public Dictionary<string, string> Actions { get; set; }
    }

    [MetadataType(typeof(MopPlanModel))]
    public partial class M_MOP_PLAN
    {
    
        public M_MOP_PLAN()
        {
            Actions = new Dictionary<string, string>();
        }
        public Nullable<System.DateTime> SelectedDate { get; set; }

        public string Status { get; set; }
        public Dictionary<string, string> Actions { get; set; }

    }
}