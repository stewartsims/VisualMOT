using System;
using System.Collections.Generic;
using System.Text;

namespace VisualMOT
{
    class Constants
    {
        public static string MOTHistoryURL = "https://beta.check-mot.service.gov.uk/trade/vehicles/mot-tests?registration=";
        public static string MOTHistoryURLMakeSuffix = "&make=";
        public static string APIKey = "YOUR_DVSA_API_KEY";

        public static int FINAL_IMAGE_WIDTH_IN_PIXELS = 384;

        public static string EmailSubject = "Visual MOT Test Results";

        public static string MailPort = "587";
        public static string MailSSL = "true";
        public static string MailHost = "YOUR_MAIL_HOST";
        public static string MailUsername = "YOUR_EMAIL_ADDRESS";
        public static string MailPassword = "YOUR_PASSWORD";
        public static string MailFrom = "Visual MOT";
        public static string MailFromAddress = "YOUR_MAIL_FROM_ADDRESS";

        public static string MailSubject = "MOT report from Visual MOT";

        public static string FTPHost = "YOUR_FTP_HOST";
        public static string FTPUserName = "YOUR_FTP_USERNAME";
        public static string FTPPassword = "YOUR_FTP_PASSWORD";
        public static string FTPPath = "";

        public static string SMSSender = "VisualMOT";
        public static string SMSMessage = "Your MOT is complete and the tester has sent you the following report: https://visualmot.co.uk/visualmot/";
        public static string SMSApiURL = "https://api.thesmsworks.co.uk/v1";
        public static string SMSApiKey = "JWT YOUR_SMS_WORKS_API_KEY_JWT";

        public static string SendMethodProperty = "SendMethod";
        public static string RequiresPurchaseFlag = "FreeFlag";
        public static string SavedEmailProperty = "SavedEmailEmail";

        public static string EmailBillingProductId = "visual_mot_email";
        public static string SMSBillingProductId = "visual_mot_sms";

        public static string PriceGoogle = "50p";
        public static string PriceApple = "49p";

    }
}
