using Org.Reddragonit.Stringtemplate;
using Plugin.Toast;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendPage : ContentPage
    {
        public IFileHelper FileHelper = DependencyService.Get<IFileHelper>();
        private string CustomerEmail { get; set; }
        private string YourEmail { get; set; }

        private MOTHistory MOTHistory { get; set; }

        public SendPage(MOTHistory motHistory)
        {
            InitializeComponent();
            MOTHistory = motHistory;
        }

        private void CustomerEmailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            CustomerEmail = e.NewTextValue;
        }
        private void YourEmailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            YourEmail = e.NewTextValue;
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

                    List<EmailAttachment> attachments = new List<EmailAttachment>();
                    foreach (MOTItem item in MOTHistory.Items.Where(item => item.image != null))
                    {
                        string fileName = Path.GetTempFileName();
                        string file = Path.Combine(FileSystem.CacheDirectory, fileName);
                        File.WriteAllBytes(file, item.image);
                        EmailAttachment attachment = new EmailAttachment(file);
                        item.imageFileName = file;
                    }
                    /*
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
                    */

                    string template;
                    if (MOTHistory.LastTest.testResult == "PASSED")
                    {
                        template = new StreamReader(FileHelper.GetSharedFile("TemplatePass.html")).ReadToEnd();
                    }
                    else
                    {
                        template = new StreamReader(FileHelper.GetSharedFile("TemplateFail.html")).ReadToEnd();
                    }
                    string itemTemplate = new StreamReader(FileHelper.GetSharedFile("ItemTemplate.html")).ReadToEnd();

                    Template templator = new Template(template);
                    templator.SetAttribute("make", MOTHistory.make);
                    templator.SetAttribute("model", MOTHistory.model);
                    templator.SetAttribute("registration", MOTHistory.registration);
                    templator.SetAttribute("number", MOTHistory.LastTest.motTestNumber);
                    templator.SetAttribute("mileage", MOTHistory.LastTest.odometerValue + MOTHistory.LastTest.odometerUnit);
                    templator.SetAttribute("expiry", MOTHistory.LastTestExpiryDate);

                    List<string> items = new List<string>();
                    foreach(MOTItem item in MOTHistory.Items)
                    {
                        Template itemTemplator = new Template(itemTemplate);
                        itemTemplator.SetAttribute("severity", item.type);
                        itemTemplator.SetAttribute("description", item.text);
                        itemTemplator.SetAttribute("image", item.imageFileName);
                        items.Add(itemTemplate.ToString());
                    }

                    templator.SetAttribute("items", string.Join("\n", items));
                    string body = templator.ToString();

                    try
                    {
                        var message = new EmailMessage
                        {
                            Subject = Constants.EmailSubject,
                            Body = body,
                            To = new List<string>() { CustomerEmail, YourEmail },
                            Attachments = attachments
                        };
                        await Email.ComposeAsync(message);
                        Navigation.PopModalAsync();
                        CrossToastPopUp.Current.ShowToastSuccess("The MOT report has been successfully sent. Thank you for using Visual MOT");
                    } 
                    catch (Exception e)
                    {
                        await DisplayAlert("Error", "Failed to send, please try again", "OK");
                    }
                    

                    loadingSpinner.IsBusy = false;
                });
            }
        }
    }
}