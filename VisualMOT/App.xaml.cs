using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VisualMOT
{
    public partial class App : Application
    {
        private static string SyncFusionLicenseKey = "MzY3NTQ2QDMxMzgyZTM0MmUzMEVna3BVYll1cHcrVFVGcHBiOXBnaXh4bDJSc0ZZR2hDUXJIWWpiQStFa3c9;MzY3NTQ3QDMxMzgyZTM0MmUzMGJYNnZmMW1DZ3Nua2ZZcUlxcW13a01BTWNKVk9ZTXEyRnZ1VEZ5QkpFeEk9;MzY3NTQ4QDMxMzgyZTM0MmUzMG55TzdFdzhkSVBVNVBid29GdU1CSWFwRXhkTy9oZFlqaVM4ZlNkZjZ0N2M9";
        
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncFusionLicenseKey);
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
