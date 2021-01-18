using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ViewModels
{
    public class MopViewModels
    {
        public M_MOP M_MOP { get; set; }
        public List<M_MOP> M_MOP_List { get; set; }
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
        public M_MOPModel M_MOPModel { get; set; }
        public List<M_MOPModel> M_MOPModel_List { get; set; }
        public M_MOP_ITEMS M_MOP_ITEMS { get; set; }
        public List<M_MOP_ITEMS> M_MOP_ITEMS_list { get; set; }
        public M_PMS M_PMS { get; set; }
        public IEnumerable<M_PMS> _M_PMS { get; set; }
        public M_MOP_ItemsModel M_MOP_ItemsModel { get; set; }

        public List<M_MOP_ItemsModel> M_MOP_ItemsModelList { get; set; }
        public string pmsNo { get; set; }
    }
}
