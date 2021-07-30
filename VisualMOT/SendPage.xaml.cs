using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Org.Reddragonit.Stringtemplate;
using Plugin.Toast;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public partial class SendPage : ContentPage
    {
        public IFileHelper FileHelper = DependencyService.Get<IFileHelper>();
        private string CustomerEmail { get; set; }
        private string CustomerSMS { get; set; }
        private string YourEmail { get; set; }
        private string Comment { get; set; }

        private MOTHistory MOTHistory { get; set; }


        public SendPage(MOTHistory motHistory)
        {
            InitializeComponent();
            MOTHistory = motHistory;
            string sendMethod = null;
            try
            {
                sendMethod = Application.Current.Properties[Constants.SendMethodProperty] as string;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine("Send method not yet chosen");
            }
            if (sendMethod != null && sendMethod == "SMS")
            {
                Tabs.SelectedIndex = 1;
                SendButton.Text = "Send SMS";
            }
            else
            {
                SendButton.Text = "Send Email";
            }
        }

        private void CustomerEmailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            CustomerEmail = e.NewTextValue;
        }
        private void CustomerSMSEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            CustomerSMS = e.NewTextValue;
        }
        private void YourEmailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            YourEmail = e.NewTextValue;
        }
        private void CommentEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Comment = e.NewTextValue;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SfBusyIndicator busyIndicator = new SfBusyIndicator();
            busyIndicator.AnimationType = AnimationTypes.Gear;
            busyIndicator.IsBusy = true;
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
                    bool success = await EmailTask();
                    if (success)
                    {
                        CrossToastPopUp.Current.ShowToastSuccess("The MOT report has been successfully sent. Thank you for using Visual MOT");
                        loadingSpinner.IsBusy = false;
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        loadingSpinner.IsBusy = false;
                        await DisplayAlert("Error", "There was a problem sending the report, please try again.", "OK");
                    }
                });
            }
        }

        private async Task<bool> EmailTask()
        {
            MimeMessage message = new MimeMessage();

            // now create the multipart/mixed container to hold the message text and the
            // image attachment
            var multipart = new Multipart("mixed");

            string fileNamePrefix = MOTHistory.registration + "_" + MOTHistory.LastTest.motTestNumber;
            List<string> imageFiles = new List<string>();

            int i = 1;
            foreach (MOTItem item in MOTHistory.Items.Where(item => item.image != null))
            {
                string fileName = fileNamePrefix + "_" + i + ".jpg";
                string contentId = Guid.NewGuid().ToString();
                string file = Path.Combine(FileSystem.CacheDirectory, contentId);
                File.WriteAllBytes(file, item.image);
                imageFiles.Add(file);

                multipart.Add(new MimePart("image", "jpg")
                {
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
            templator.SetAttribute("comment", Comment);

            List<string> items = new List<string>();
            foreach (MOTItem item in MOTHistory.Items)
            {
                string image = item.imageFileName != null ? "<img src=\"cid:" + item.imageFileName + "\">" : "(no photo supplied)";
                Template itemTemplator = new Template(itemTemplate);
                itemTemplator.SetAttribute("severity", item.type);
                itemTemplator.SetAttribute("description", item.text);
                itemTemplator.SetAttribute("image", image);
                itemTemplator.SetAttribute("comment", item.comment);
                items.Add(itemTemplator.ToString());
            }

            templator.SetAttribute("items", string.Join("\n", items));
            string body = templator.ToString();
            string bodyFileName = Guid.NewGuid().ToString() + ".html";
            string bodyFile = Path.Combine(FileSystem.CacheDirectory, bodyFileName);
            File.WriteAllText(bodyFile, body.Replace("cid:",""));

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
                if (!string.IsNullOrEmpty(CustomerEmail) || !string.IsNullOrEmpty(YourEmail))
                {
                    client.Send(message);
                }
                bool sendSuccess = true;
                if (CustomerSMS != null)
                {
                    sendSuccess = SendViaFTP(bodyFile, bodyFileName, imageFiles);
                }
                if (sendSuccess)
                {
                    Application.Current.Properties[Constants.FreeFlagProperty] = false;
                    Application.Current.SavePropertiesAsync();
                }
                return sendSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private bool SendViaFTP(string bodyFile, string bodyFileName, List<string> imageFiles)
        {
            string bodyResponse = DependencyService.Get<IFtpWebRequest>().Upload(Constants.FTPHost, bodyFile, Constants.FTPUserName, Constants.FTPPassword, Constants.FTPPath);
            Console.WriteLine(bodyResponse);
            foreach (string imageFile in imageFiles)
            {
                string imageResponse = DependencyService.Get<IFtpWebRequest>().Upload(Constants.FTPHost, imageFile, Constants.FTPUserName, Constants.FTPPassword, Constants.FTPPath);
                Console.WriteLine(imageResponse);
            }

            string message =
            "{" +
                "\"sender\": \"" + Constants.SMSSender + "\", " +
                "\"destination\": \"" + CustomerSMS + "\", " +
                "\"content\": \"" + Constants.SMSMessage + bodyFileName + "\" " +
            "}";

            using (HttpClient sendRequest = new HttpClient())
            {
                sendRequest.DefaultRequestHeaders.Add("Authorization", Constants.SMSApiKey);
                HttpResponseMessage sendResponseMessage = sendRequest.PostAsync(Constants.SMSApiURL + "/message/send", new StringContent(message, Encoding.UTF8, "application/json")).Result;
                if (sendResponseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        private void SfTabView_SelectionChanged(object sender, Syncfusion.XForms.TabView.SelectionChangedEventArgs e)
        {
            SendButton.Text = "Send " + e.Name;
            Application.Current.Properties[Constants.SendMethodProperty] = e.Name;
            Application.Current.SavePropertiesAsync();
        }
    }
}