using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Org.Reddragonit.Stringtemplate;
using Plugin.Toast;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                    busyIndicator.IsBusy = true;

                    MimeMessage message = new MimeMessage();

                    // now create the multipart/mixed container to hold the message text and the
                    // image attachment
                    var multipart = new Multipart("mixed");

                    foreach (MOTItem item in MOTHistory.Items.Where(item => item.image != null))
                    {
                        string fileName = Path.GetTempFileName();
                        string file = Path.Combine(FileSystem.CacheDirectory, fileName);
                        File.WriteAllBytes(file, item.image);

                        string contentId = Guid.NewGuid().ToString();
                        multipart.Add(new MimePart("image", "jpg") {
                            ContentObject = new ContentObject(File.OpenRead(file), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            ContentId = contentId,
                            FileName = fileName,
                        }); ;
                        item.imageFileName = contentId;
                    }

                    //To embed image in email
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
                    foreach (MOTItem item in MOTHistory.Items)
                    {
                        Template itemTemplator = new Template(itemTemplate);
                        itemTemplator.SetAttribute("severity", item.type);
                        itemTemplator.SetAttribute("description", item.text);
                        itemTemplator.SetAttribute("image", item.imageFileName);
                        items.Add(itemTemplator.ToString());
                    }

                    templator.SetAttribute("items", string.Join("\n", items));
                    string body = templator.ToString();


                    int port = System.Int32.Parse(Constants.MailPort);
                    bool ssl = bool.Parse(Constants.MailSSL);

                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    SmtpClient client = new SmtpClient();
                    client.Connect(Constants.MailHost, port, SecureSocketOptions.StartTls);
                    client.Authenticate(Constants.MailUsername, Constants.MailPassword);
                                      
                    multipart.Add(new TextPart("html") { Text = body });
                    message.Body = multipart;

                    MailboxAddress fromAddress = new MailboxAddress(Constants.MailFrom, Constants.MailFromAddress);
                    message.Subject = Constants.MailSubject;
                    message.From.Add(fromAddress);

                    if (!string.IsNullOrEmpty(CustomerEmail))
                    {
                        message.To.Add(new MailboxAddress(CustomerEmail));
                    }
                    if (!string.IsNullOrEmpty(YourEmail))
                    {
                        message.To.Add(new MailboxAddress(YourEmail));
                    }

                    try
                    {
                        client.Send(message);
                        CrossToastPopUp.Current.ShowToastSuccess("The MOT report has been successfully sent. Thank you for using Visual MOT");
                        loadingSpinner.IsBusy = false;
                        Navigation.PopAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        DisplayAlert("Error", "There was a problem sending the report, please try again.", "OK");
                    }
                    loadingSpinner.IsBusy = false;
                });
            }
        }
    }
}