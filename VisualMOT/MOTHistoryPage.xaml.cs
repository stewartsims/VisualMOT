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
            this.MOTItems.ForEach(item => item.imageSource = ImageSource.FromStream(() => new MemoryStream(item.image)));
            InitializeComponent();
            BindingContext = this.MOTHistory;
            ItemsListView.ItemsSource = MOTHistory.Items;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SfBusyIndicator busyIndicator = new SfBusyIndicator();
            busyIndicator.AnimationType = AnimationTypes.Gear;
            Container.Children.Add(busyIndicator);
            EmailCommand.Execute(busyIndicator);
        }

        public ICommand EmailCommand
        {
            get
            {
                return new Command(async (parameter) =>
                {
                    SfBusyIndicator loadingSpinner = (SfBusyIndicator)parameter;
                    loadingSpinner.IsBusy = true;
                    /*
                    List<EmailAttachment> attachments = new List<EmailAttachment>();
                    foreach (MOTItem item in MOTHistory.Items.Where(item => item.image != null))
                    {
                        string file = Path.Combine(FileSystem.CacheDirectory, Path.GetTempFileName());
                        File.WriteAllBytes(file, item.image);
                        EmailAttachment attachment = new EmailAttachment(file);
                    }

                    msg.IsBodyHtml = true;
                    Attachment inlineLogo = new Attachment(@"C:\Desktop\Image.jpg");
                    msg.Attachments.Add(inlineLogo);
                    string contentID = "Image";
                    inlineLogo.ContentId = contentID;

                    //To make the image display as inline and not as attachment

                    inlineLogo.ContentDisposition.Inline = true;
                    inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

                    //To embed image in email

                    msg.Body = "<htm><body> <img src=\"cid:" + contentID + "\"> </body></html>";

                    try
                    {
                        var message = new EmailMessage
                        {
                            Subject = Constants.EmailSubject,
                            Body = body,
                            To = recipients,
                            Attachments = attachments
                        };
                        await Email.ComposeAsync(message);
                    }
                    */

                    loadingSpinner.IsBusy = false;
                });
            }
        }

        private void ItemsListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is MOTItem)
                {
                    MOTItem item = e.AddedItems[0] as MOTItem;
                    if (item.image != null)
                    {

                    }
                    else
                    {
                        Navigation.PushAsync(new NavigationPage(new UploadImagePage(item)));
                    }
                } else
                {
                    DisplayAlert("Oops", "not what i was expecting", "ok");
                }
            }
        }
    }
}
