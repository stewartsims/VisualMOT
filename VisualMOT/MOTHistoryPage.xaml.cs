using Newtonsoft.Json;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualMOT.Model;
using Xamarin.Forms;

namespace VisualMOT
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MOTHistoryPage : ContentPage
    {
        private MOTHistory MOTHistory { get; set; }

        public MOTHistoryPage(MOTHistory motHistory)
        {
            this.MOTHistory = motHistory;
            InitializeComponent();
            BindingContext = this.MOTHistory;
            ItemsListView.ItemsSource = MOTHistory.Items;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        public ICommand EmailCommand
        {
            get
            {
                return new Command(async (parameter) =>
                {
                    
                });
            }
        }
    }
}
