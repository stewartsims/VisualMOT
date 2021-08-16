using Syncfusion.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using VisualMOT.Model;

namespace VisualMOT
{
    public class SeverityItemComparer : IComparer<MOTItem>
    {
        public int Compare(MOTItem x, MOTItem y)
        {
            if (x.type == "MAJOR")
            {
                //GroupResult y is stacked into top of the group i.e., Ascending.
                //GroupResult x is stacked at the bottom of the group i.e., Descending.
                return -1;
            }
            else if (x.type == "MINOR")
            {
                //GroupResult x is stacked into top of the group i.e., Ascending.
                //GroupResult y is stacked at the bottom of the group i.e., Descending.
                return 0;
            }

            return 1;
        }
    }
}
