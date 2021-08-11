using System;
using System.IO;
using Xamarin.Forms;
using VisualMOT.iOS;

[assembly: Dependency(typeof(FileHelper))]
namespace VisualMOT.iOS
{
    public class FileHelper : IFileHelper
    {
        public void ForceBackButton()
        {

        }

        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }

        public byte[] GetResourceByName(string filename)
        {
            return File.ReadAllBytes(filename);
            /*using (var image = UIImage.FromFile(filename))
            using (NSData imageData = image.AsPNG())
            {
                var byteArray = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, byteArray, 0, Convert.ToInt32(imageData.Length));
                return byteArray;
            }*/
        }

        public void OpenFileInDefaultApp(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(documentsPath, filename);
            var PreviewController = UIKit.UIDocumentInteractionController.FromUrl(Foundation.NSUrl.FromFilename(filePath));
            PreviewController.Delegate = new UIKit.UIDocumentInteractionControllerDelegate();
            Device.BeginInvokeOnMainThread(() =>
            {
                PreviewController.PresentPreview(true);
            });
        }

        public Stream GetSharedFile(string fileResourceName)
        {
            Type type = typeof(IFileHelper);
            return type.Assembly.GetManifestResourceStream("VisualMOT."+fileResourceName);
        }
    }
}