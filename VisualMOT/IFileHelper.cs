using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualMOT
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        void OpenFileInDefaultApp(string filePath);
        byte[] GetResourceByName(string filename);
        void ForceBackButton();
        Stream GetSharedFile(string fileResourceName);
    }
}