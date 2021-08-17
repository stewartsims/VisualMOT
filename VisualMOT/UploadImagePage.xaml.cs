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
    public partial class UploadImagePage : ContentPage
    {
        public MOTItem MOTItem { get; set; }
        public FileResult ChosenImage { get; set; }
        public string ImageText { get; set; }

        private MOTHistoryPage MOTHistoryPage { get; set; }

        public UploadImagePage(MOTItem motItem, MOTHistoryPage motHistoryPage)
        {
            MOTItem = motItem;
            ImageText = motItem.text;
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

        private async void PickImage_Clicked(object sender, EventArgs e)
        {
            ChosenImage = await MediaPicker.PickPhotoAsync();
            if (ChosenImage != null)
            {
                ImageText = "Image attached: " + ChosenImage.FullPath;
                OnPropertyChanged("ImageText");
            }
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                ChosenImage = await MediaPicker.CapturePhotoAsync();
            }
            catch (NotSupportedException ex)
            {
                await App.Current.MainPage.DisplayAlert("Camera not available", "Camera not available on this device.", "Close");
            }
            catch (PermissionException pEx)
            {
                await App.Current.MainPage.DisplayAlert("Permission not granted", "Please grant the app permission to use the camera to proceed.", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }

            if (ChosenImage != null)
            {
                ImageText += "\n\n[ new photo attached ]";
                OnPropertyChanged("ImageText");
            }
        }

        private bool ChosenImageValid()
        {
            // since the file is resized on the device before uploading to the server - we don't
            // actually need to check file size here. This is kept in case any other form of image
            // validation is required later
            return true;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (!ChosenImageValid())
            {
                CrossToastPopUp.Current.ShowToastMessage("Chosen image is too large, please select or take a new image (less than 10Mb in size)");
            }

            SfBusyIndicator sfBusyIndicator = new SfBusyIndicator();
            sfBusyIndicator.AnimationType = AnimationTypes.DoubleCircle;
            Container.Children.Add(sfBusyIndicator);
            SaveCommand.Execute(sfBusyIndicator);
            MOTHistoryPage.Refresh();
        }

        public ICommand SaveCommand
        {
            get
            {
                return new Command(async (parameter) =>
                {
                    try
                    {
                        await SaveTask();
                    }
                    catch (Exception e)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "An error occured: \n\n" + e.Message, "Close");
                        return;
                    }
                });
            }
        }

        private async Task SaveTask()
        {
            if (ChosenImage != null)
            {
                using (Stream stream = ChosenImage.OpenReadAsync().Result)
                {

                    // Resize the image
                    byte[] thumbnailBytes = null;
                    SKBitmap image = SKBitmap.Decode(stream);
                    double percentageReduction = (double)Constants.FINAL_IMAGE_WIDTH_IN_PIXELS / (double)image.Width;
                    int width = (int)Math.Round(image.Width * percentageReduction);
                    int height = (int)Math.Round(image.Height * percentageReduction);
                    SKBitmap scaledBitmap = image.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                    SKData imageData;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        SKBitmap rotatedBitmap = Rotate(scaledBitmap, 180);
                        imageData = rotatedBitmap.Encode(SKEncodedImageFormat.Png, 100);
                    }
                    else
                    {
                        imageData = scaledBitmap.Encode(SKEncodedImageFormat.Png, 100);
                    }
                    thumbnailBytes = imageData.ToArray();
                    MOTItem.image = thumbnailBytes;
                    await Navigation.PopModalAsync();
                    MOTHistoryPage.Refresh();
                }
            }
        }

        public static SKBitmap Rotate(SKBitmap bitmap, double angle)
        {
            double radians = Math.PI * angle / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees((float)angle);
                surface.Translate(-originalWidth / 2, -originalHeight / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            }
            return rotatedBitmap;
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}