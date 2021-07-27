using Plugin.Toast;
using SkiaSharp;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualMOT.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VisualMOT
{
    [DesignTimeVisible(false)]
    public partial class AddCommentPage : ContentPage
    {
        public MOTItem MOTItem { get; set; }
        public string Comment { get; set; }
        public string Text { get; set; }

        private MOTHistoryPage MOTHistoryPage { get; set; }

        public AddCommentPage(MOTItem motItem, MOTHistoryPage motHistoryPage)
        {
            MOTItem = motItem;
            Text = motItem.text;
            this.MOTHistoryPage = motHistoryPage;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                Navigation.PopAsync();
            }
            base.OnDisappearing();
        }
        private async void Save_Clicked(object sender, EventArgs e)
        {
            MOTItem.comment = Comment;
            MOTHistoryPage.Refresh();
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}