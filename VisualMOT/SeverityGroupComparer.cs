using Syncfusion.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualMOT
{
    public class SeverityGroupComparer : IComparer<GroupResult>
    {
        public int Compare(GroupResult x, GroupResult y)
        {
            if (x.Key == "MAJOR")
            {
                //GroupResult y is stacked into top of the group i.e., Ascending.
                //GroupResult x is stacked at the bottom of the group i.e., Descending.
                return -1;
            }
            else if (x.Key == "MINOR")
            {
                //GroupResult x is stacked into top of the group i.e., Ascending.
                //GroupResult y is stacked at the bottom of the group i.e., Descending.
                return 0;
            }

            return 1;
        }
    }
}
