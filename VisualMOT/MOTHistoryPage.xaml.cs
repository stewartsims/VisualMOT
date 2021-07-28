using Newtonsoft.Json;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualMOT.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Syncfusion.DataSource.Extensions;

namespace VisualMOT
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MOTHistoryPage : ContentPage
    {
        private MOTHistory MOTHistory { get; set; }

        private ObservableCollection<MOTItem> motItems;

        public ObservableCollection<MOTItem> MOTItems
        {
            get { return motItems; }
            set { this.motItems = value; }
        }

        public MOTHistoryPage(MOTHistory motHistory)
        {
            this.MOTHistory = motHistory;
            this.MOTItems = new ObservableCollection<MOTItem>(motHistory.Items);
            InitializeComponent();
            BindingContext = this.MOTHistory;
            ItemsListView.ItemsSource = MOTHistory.Items;
        }

        protected override void OnAppearing()
        {
            this.Refresh();
            base.OnAppearing();
        }

        public void Refresh()
        {
            this.MOTItems = new ObservableCollection<MOTItem>(MOTHistory.Items);
            this.MOTItems.ForEach(item =>
            {
                if (item.image != null)
                {
                    item.ImageSource = ImageSource.FromStream(() => new MemoryStream(item.image));
                }
                else
                {
                    item.ImageSource = ImageSource.FromFile("image_placeholder.png");
                }
                item.HasComment = item.comment != null;
                item.NoComment = item.comment == null;
            });
            OnPropertyChanged("MOTItems");
            OnPropertyChanged();
        }

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new SendPage(MOTHistory)));
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            MOTItem item = button.CommandParameter as MOTItem;
            if (item.image != null)
            {

            }
            else
            {
                Navigation.PushModalAsync(new NavigationPage(new UploadImagePage(item, this)));
            }
        }

        public Command ImageCommand
        {
            get
            {
                return new Command(async (parameter) =>
                {
                    ClickableImage image = parameter as ClickableImage;
                    MOTItem item = image.ClickParameter as MOTItem;
                    if (item.image != null)
                    {

                    }
                    else
                    {
                        Navigation.PushModalAsync(new NavigationPage(new UploadImagePage(item, this)));
                    }
                });
            }
        }

        private void CommentButton_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            MOTItem item = button.CommandParameter as MOTItem;
            Navigation.PushModalAsync(new NavigationPage(new AddCommentPage(item, this)));
        }
    }
}
