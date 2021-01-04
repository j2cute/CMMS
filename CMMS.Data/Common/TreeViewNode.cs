using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Common
{
    public class TreeViewNode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string Name { get; set; }
        public List<string> parents { get; set; }
        public State state { get; set; }
    }
    public class State
    {
        public bool selected { get; set; }
    }
}
