using System;
using System.Collections.Generic;
using System.Text;

namespace VisualMOT
{
    public interface IFtpWebRequest
    {
        string Upload(string FtpUrl, string fileName, string userName, string password, string UploadDirectory);
    }
}
