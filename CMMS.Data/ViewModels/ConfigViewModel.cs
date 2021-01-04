using ClassLibrary.Common;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ViewModels
{
    public class ConfigViewModel
    {
        public C_Site_Config C_Site_Config { get; set; }
        public C_Config_Master C_Config_Master { get; set; }
       // public tbl_Unit tbl_Unit { get; set; }
        public IEnumerable<tbl_Unit> _tbl_Unit { get; set; }
        public List<ConfigViewModel> ConfigViewModel_list { get; set; }
        public string JsonMasterConfig { get; set; }
        public string JsonSiteConfig { get; set; }
        public string selectedData { get; set; }
        public tbl_Parts tbl_Parts { get; set; }
        public List<tbl_Parts> tbl_Parts_list { get; set; }
        public C_SiteConfigModel C_SiteConfigModel { get; set; }
        public PartsModel PartsModel { get; set; }
        public CageModel CageModel { get; set; }
        public FileHelper FileHelper { get; set; }
        public tbl_Request tbl_Request { get; set; }
        public RequestModel RequestModel { get; set; }
        public List<tbl_Request> tbl_Request_list { get; set; }
        public tbl_Currency tbl_Currency { get; set; }
        public IEnumerable<tbl_Currency> _tbl_Currency { get; set; }
        public tbl_Country tbl_Country { get; set; }
        public IEnumerable<tbl_Country> _tbl_Country { get; set; }
        public List<M_PMS> _M_PMS { get; set; }
    }
}
