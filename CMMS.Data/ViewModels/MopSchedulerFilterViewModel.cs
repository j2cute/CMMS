using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ClassLibrary.ViewModels
{
    public class MopSchedulerFilterViewModel
    {
        [Display(Name="Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Display(Name = "Site Id")]
        public string SiteId { get; set; }

        [Display(Name = "MOP No")]
        public string MopNo { get; set; }

        [Display(Name = "PMS No")]
        public string PmsNo { get; set; }
    }
}