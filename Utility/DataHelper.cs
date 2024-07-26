using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;





namespace CallBackUtility.Utility
{
    public static class DataHelper
    {
      

      public static string ToPascalConvention(string textToChange)
        {
            // textToChange = "WARD_VS_VITAL_SIGNS";
            System.Text.StringBuilder resultBuilder = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(textToChange))
            {
                foreach (char c in textToChange)
                {
                    // Replace anything, but letters and digits, with space
                    if (!Char.IsLetterOrDigit(c))
                    {
                        resultBuilder.Append(" ");
                    }
                    else
                    {
                        resultBuilder.Append(c);
                    }
                }
            }
            string result = resultBuilder.ToString();
            // Make result string all lowercase, because ToTitleCase does not change all uppercase correctly
            result = result.ToLower();
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            // result = myTI.ToTitleCase(result).Replace(" ", String.Empty);
            result = myTI.ToTitleCase(result);
            return result;
        }

        internal static string getFileName(string UploadedFileName, out string fileUserId, out string fileUploaded)
        {
            string fileNameToDisplay = string.Empty;
            fileUploaded = "";
            fileUserId = "";
            if (!string.IsNullOrEmpty(UploadedFileName))
            {
                string uploadedfolderName = ConfigurationManager.AppSettings["emailAttachments"].ToString();
                string file = UploadedFileName.Split('&')[1];
                fileUserId = UploadedFileName.Split('&')[0];
                string filename = file.Split('_')[0];
                string fileExtension = System.IO.Path.GetExtension(file);
                fileNameToDisplay = filename + fileExtension;
                fileUploaded = "/" + uploadedfolderName + "/" + fileUserId + "/" + file;
            }
            return fileNameToDisplay;
        }

        internal static void filterDates(string date, out string fromdate, out string todate)
        {
            string dateFiltered = CryptorEngine.ConvertHexToString(date, System.Text.Encoding.Unicode);
            string[] dates = dateFiltered.Split('@');
            fromdate = dates[0] == "none" ? null : dates[0];
            todate = dates[1] == "none" ? null : dates[1];
        }

        //public static IPAddress[] GetIPAddresses()
        //{
        //    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
        //    return ipHostInfo.AddressList;
        //}

        //public static IPAddress GetIPAddressa(int num = 0)
        //{
        //    return GetIPAddresses()[num];
        //}

        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }

        internal static string priorityLevel(string priority)
        {
            priority = priority.ToLower();
            switch (priority)
            {
                case "high":
                    priority = "high";
                    break;

                case "urgent":
                    priority = "high";
                    break;

                case "low":
                    priority = "low";
                    break;
                case "normal":
                    priority = "normal";
                    break;
            }
            return priority;
        }

        //public static string GetIPAddress(int num = 0)
        //{
        //    return GetIPAddresses()[1].ToString();
        //}

        //internal static string convertIntegarArrayToString(int[] productArray)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    for (int i = 0; i < productArray.Count(); i++)
        //    {
        //        builder.Append(productArray[i]);
        //        if ((i + 1) < productArray.Count())
        //        {
        //            builder.Append(',');
        //        }
        //    }
        //    return builder.ToString();
        //}

        //public static string getCurrentUrl(HttpRequestBase Request)
        //{
        //    return "http://" + DataHelper.GetIPAddress() + ":" + Request.Url.Port.ToString();
        //}

        internal static string getReplyEmailTemplate()
        {
            string body = string.Empty;
            string filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ReplyEmailTemplate"].ToString());
            using (var sr = new System.IO.StreamReader(filepath))
            {
                body = sr.ReadToEnd();
            }
            return body;
        }

        internal static string getEmailType(int Id)
        {
            if (Id <= 5)//0--5
            {
                if (Id < 1)//0
                {
                    return "";
                }
                else if (Id > 0 && 2 > Id)//1
                {
                    return "Replied";
                }
                else if (Id > 1 && 3 > Id)//2
                {
                    return "Forwarded";
                }
                else if (Id == 3)//3
                {
                    return "New";
                }
                else if (Id == 4)//4
                { return "Sent"; }
                else { return "Replied"; }//5
            }
            else//6----->
            {
                if (Id > 5 && 7 > Id)//6
                {
                    return "Replied";
                }
                else if (Id > 6 && 8 > Id) //7
                {
                    return "Sent";
                }
                else if (Id > 7 && 9 > Id)//8
                {
                    return "Forwarded";
                }
                else { return ""; }
            }
        }

        internal static string getOriginalFileNameEmail(string UploadedFileName, out string filePath, string fileUserId)
        {
            UploadedFileName = UploadedFileName.Contains("/") ? UploadedFileName.Split('/')[UploadedFileName.Split('/').Count() - 1] : UploadedFileName;
            String[] spearator = { "@uzma$" };
            string fileNameToDisplay = string.Empty;
            string uploadedfolderName = ConfigurationManager.AppSettings["emailAttachments"].ToString();
            string file = UploadedFileName.Split(spearator, StringSplitOptions.None)[1];

            filePath = String.Format("/{0}/{1}/{2}", uploadedfolderName, fileUserId, UploadedFileName);
            return file;
        }

        //internal static string getOriginalFileNameSystem(string UploadedFileName, out string filePath)
        //{
        //    string fileNameToDisplay = string.Empty;
        //    string uploadedfolderName = ConfigurationManager.AppSettings["emailAttachments"].ToString();
        //    string file = UploadedFileName.Split('&')[1];
        //    string fileUserId = UploadedFileName.Split('&')[0];                     
        //    string fileExtension = Path.GetExtension(file);
        //    for (int i = 0; i < file.Split('_').Count(); i++)
        //    {
        //        if (i > 0)
        //        {
        //            fileNameToDisplay += file[i];
        //        }
        //    }
        //    filePath = String.Format("/" + uploadedfolderName + "/{1}/{2}", uploadedfolderName, fileUserId, file);
        //    return fileNameToDisplay;
        //}

        internal static bool CreateFolderIfNeededWithPhysicalPath(string physicalPath)
        {
            bool result = true;
            if (!System.IO.Directory.Exists(physicalPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(physicalPath);
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        internal static string GetDateFormatForFileName()
        {
            return DateTime.Now.ToString().Replace(" ", "").Replace("/", "").Replace(":", "");
        }

        //internal static string webSettings(int name, string text)
        //{
        //    string value = string.Empty;
        //    if (text == "priority")
        //    {
        //        value = ConfigurationManager.AppSettings[DBHandler.GetPriorityTypeById(name).ToLower().ToString()].ToString();
        //    }
        //    return value;
        //}
    }
}