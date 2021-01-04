using ClassLibrary.Common;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ViewModels
{
    public class PartsViewModels
    {
        public tbl_Parts tbl_Parts { get; set; }
        public tbl_Cage tbl_Cage { get; set; }
        public tbl_PartType tbl_PartType { get; set; }
        public tbl_MCAT tbl_MCAT { get; set; }
        public tbl_Currency tbl_Currency { get; set; }
        public FileHelper FileHelper { get; set; }
        public IEnumerable<tbl_Cage> _tbl_Cage { get; set; }
        public IEnumerable<tbl_PartType> _tbl_PartType { get; set; }
        public IEnumerable<tbl_MCAT> _tbl_MCAT { get; set; }
        public IEnumerable<tbl_Currency> _tbl_Currency { get; set; }
        public List<tbl_Parts> tbl_Parts_list { get; set; }
        public List<PartsViewModels> PartsViewModels_list { get; set; }

        public int partsCount { get; set; }
        public int cageActiveCount { get; set; }
        public int partTypeCount { get; set; }
        public GraphData GraphData { get; set; }
        public List<GraphData> dataList { get; set; }
        public tbl_ChildParts tbl_ChildParts { get; set; }
        public List<tbl_ChildParts> tbl_ChildParts_list { get; set; }
        public ChildPartsModel ChildPartsModel { get; set; }
        public List<ChildPartsModel> ChildPartsModel_list { get; set; }
    }
}
