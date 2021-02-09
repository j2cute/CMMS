using ILS.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ClassLibrary.ViewModels
{
    public class UserDashboardMappingVM
    {
        public tbl_User UserIds { get; set; }
        public tbl_Role RoleIds { get; set; }
        public List<string> Dashboards { get; set; }
    }
}
