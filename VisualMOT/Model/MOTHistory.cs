using System;
using System.Collections.Generic;
using System.Text;

namespace VisualMOT.Model
{
    public class MOTHistory
    {
        public string registration { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public List<MOTTest> motTests { get; set; }
        
        public List<MOTItem> Items { get; set; }
        public MOTTest LastTest { get; set; }
        public string LastTestDisplayText { get; set; }
        public string LastTestExpiryDate { get; set; }
    }
}
