﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class DashboardViewModel
    {
        public int UsersAdmin_Count { get; set; }
        public int UsersOther_Count { get; set; }
        public int Personnel_Count { get; set; }
        public int CriminalRecord_Count { get; set; }
        public int LA_Count { get; set; }
        public int PFA_Count { get; set; }
        public int Pets_Count { get; set; }
        public int NDP_Count { get; set; }
        public int VOH_Count { get; set; }
        public int WOA_Count { get; set; }
        public int RTA_Count { get; set; }
        public int CMT_Count { get; set; }
        public int RTI_Count { get; set; }
        public int DP_Count { get; set; }
        public string datetime { get; set; }
    }


    public class PermissionViewModel
    {
        public string PermissionId { get; set; }
        public string Level { get; set; }
        public string ParentId { get; set; }
        public string URL { get; set; }

        public string DisplayName { get; set; }


    }
}


