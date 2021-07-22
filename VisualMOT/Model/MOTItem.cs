using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VisualMOT.Model
{
    public class MOTItem
    {
        public string text { get; set; }
        public string type { get; set; }
        public string dangerous { get; set; }

        public byte[] image { get; set; }
        public ImageSource imageSource { get; set; }
    }
}
