using System;
using System.Collections.Generic;
using System.Text;

namespace VisualMOT.Model
{
    public class MOTTest
    {
        public string completedDate { get; set; }
        public string motTestNumber { get; set; }
        public string testResult { get; set; }
        public List<MOTItem> rfrAndComments { get; set; }
    }
}
