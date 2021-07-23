using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace VisualMOT.Model
{
    public class MOTItem : INotifyPropertyChanged
    {
        public string text { get; set; }
        public string type { get; set; }
        public string dangerous { get; set; }

        public byte[] image { get; set; }
        public string imageFileName { get; set; }


        private ImageSource imageSource = null;

        public ImageSource ImageSource
        {
            get => imageSource;
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
 
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
