using ClassLibrary.Common;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ViewModels
{
    public class CageViewModels
    {
        public tbl_Cage tbl_Cage { get; set; }
        public tbl_Country tbl_Country { get; set; }
        public IEnumerable<tbl_Country> _tbl_Country { get; set; }
        public List<tbl_Cage> tbl_Cage_list { get; set; }
        public List<CageViewModels> CageViewModels_list { get; set; }

        public int? cageCount { get; set; }
        public int? cageActiveCount { get; set; }
        public int? CageCountryCount { get; set; }
        public GraphData GraphData { get; set; }
        public List<GraphData> dataList { get; set; }
    }
}
