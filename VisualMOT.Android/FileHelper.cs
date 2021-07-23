using System;
using System.IO;
using VisualMOT.Droid;
using Android.Content;
using Android.Widget;
using Android.App;
using Android.Graphics.Drawables;
using Android.Graphics;
using Xamarin.Forms;
using System.Reflection;

[assembly: Dependency(typeof(FileHelper))]
namespace VisualMOT.Droid
{
    public class FileHelper : IFileHelper
    {
        public void ForceBackButton()
        {

        }

        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(path, filename);
        }

        public byte[] GetResourceByName(string filename)
        {
            using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualMOT.Droid." + filename))
            {
                var stream = new MemoryStream();
                resource.CopyTo(stream);
                return stream.ToArray();
            }
        }

        public void OpenFileInDefaultApp(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);

            //Copy the private file's data to the EXTERNAL PUBLIC location
            string externalStorageState = global::Android.OS.Environment.ExternalStorageState;
            string application = "";

            string extension = System.IO.Path.GetExtension(filePath);

            switch (extension.ToLower())
            {
                case ".doc":
                case ".docx":
                    application = "application/msword";
                    break;
                case ".pdf":
                    application = "application/pdf";
                    break;
                case ".xls":
                case ".xlsx":
                    application = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    application = "image/jpeg";
                    break;
                default:
                    application = "*/*";
                    break;
            }
            var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/report" + extension;
            File.WriteAllBytes(externalPath, bytes);

            Java.IO.File file = new Java.IO.File(externalPath);
            file.SetReadable(true);
            //Android.Net.Uri uri = Android.Net.Uri.Parse("file://" + filePath);
            Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, application);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {
                Xamarin.Forms.Forms.Context.StartActivity(intent);
            }
            catch (Exception)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, "No Application Available to View File", ToastLength.Short).Show();
            }
        }

        public Stream GetSharedFile(string fileResourceName)
        {
            Type type = typeof(IFileHelper);
            return type.Assembly.GetManifestResourceStream("VisualMOT."+fileResourceName);
        }
    }
}