using Newtonsoft.Json;
using Syncfusion.SfBusyIndicator.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using VisualMOT.Model;
using Xamarin.Forms;

namespace VisualMOT
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EntryPage : ContentPage
    {
        public string VehicleRegistration { get; set; }
        public string VehicleMake { get; set; }

        public EntryPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SfBusyIndicator busyIndicator = new SfBusyIndicator();
            busyIndicator.AnimationType = AnimationTypes.Gear;
            Container.Children.Add(busyIndicator);
            MOTHistoryCommand.Execute(busyIndicator);
        }

        public ICommand MOTHistoryCommand
        {
            get
            {
                return new Command(async (parameter) =>
                {
                    SfBusyIndicator loadingSpinner = (SfBusyIndicator)parameter;
                    try
                    {
                        MOTHistory motHistory = await MOTHistoryTask();
                        motHistory.LastTest = motHistory.motTests[0];
                        DateTime lastTestDate = DateTime.ParseExact(motHistory.LastTest.completedDate.Substring(0, 10), "yyyy.MM.dd", CultureInfo.InvariantCulture);
                        motHistory.LastTestDisplayText = motHistory.LastTest.motTestNumber + " " + lastTestDate.ToString("dd/MM/yyyy");
                        motHistory.Items = motHistory.LastTest.rfrAndComments;
                        MOTHistoryPage motHistoryPage = new MOTHistoryPage(motHistory);
                        await Navigation.PushAsync(new NavigationPage(motHistoryPage));
                        loadingSpinner.IsBusy = false;
                    }
                    catch (Exception e)
                    {
                        await App.Current.MainPage.DisplayAlert("Login error", "An error occured: \n\n" + e.Message, "Close");
                        loadingSpinner.IsBusy = false;
                        return;
                    }
                });
            }
        }

        private async Task<MOTHistory> MOTHistoryTask()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string VehicleMakeItem = VehicleMakeComboBox.SelectedItem as string;
                string url = Constants.MOTHistoryURL + VehicleRegistration + Constants.MOTHistoryURLMakeSuffix + VehicleMake;
                var motHistoryRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                };
                motHistoryRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+v6"));
                motHistoryRequest.Headers.Add("x-api-key", Constants.APIKey);

                httpClient.Timeout = TimeSpan.FromSeconds(10);

                HttpResponseMessage response = await httpClient.SendAsync(motHistoryRequest);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    //DisplayAlert("json", json, "ok");
                    return JsonConvert.DeserializeObject<List<MOTHistory>>(json)[0];
                }
                else
                {
                    throw new Exception("Request error.\n" + response.StatusCode + ":" + response.ReasonPhrase + "\n\n");
                }
            }
        }

        private void VehicleMakeComboBox_SelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            VehicleMake = e.Value as string;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as Entry).Text = e.NewTextValue.ToUpperInvariant();
            VehicleRegistration = (sender as Entry).Text;
        }
    }
}
