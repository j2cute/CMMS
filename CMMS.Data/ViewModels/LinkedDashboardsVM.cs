using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ViewModels
{
    public class LinkedDashboardsVM
    {
        public string UserName { get; set; }
        public List<LinkedDashboards> Dashboards { get; set; }
    }

    public class LinkedDashboards
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertionDateTime { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
        public string IsDefault { get; set; }
        public string IsPrefered { get; set; }
        public string DashboardName { get; set; }
        public string DashboardId { get; set; }
    }
}
