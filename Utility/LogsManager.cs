using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;

namespace CallBackUtility.Utility
{
    public static class LogsManager
    {
       internal static void Logs( string logFilePath=null,string logMessage = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {  
                    string filepath = logFilePath + "Web_Error_Logs_" + (DateTime.Now.Date.ToString().Replace(" ", "").Replace(":", "").Replace("/", "") + ".txt");
                    if (!Directory.Exists(logFilePath))
                    {
                        Directory.CreateDirectory(logFilePath);
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    using (StreamWriter w = System.IO.File.AppendText(filepath))
                    {
                        w.WriteLine("Error: " + logMessage);
                        w.WriteLine("Method : " + memberName);
                        w.WriteLine("File: " + sourceFilePath);
                        w.WriteLine("Line: " + sourceLineNumber);
                        w.WriteLine(DateTime.Now.ToString() + "-------------------------------");
                        w.Dispose();
                    }
                
            }
            catch (Exception ex)
            {
                 Notify(logFilePath, ex.Message);
                throw;
            }
        }

        internal static void Notify(string logFilePath, string notification)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["enableNotifications"].ToString()))
            {
                //String logFilePath = ConfigurationManager.AppSettings["logFilesPath"].ToString();
                string filepath = logFilePath + "Web_Notification_" + (DateTime.Now.Date.ToString().Replace(" ", "").Replace(":", "").Replace("/", "") + ".txt");
                if (!Directory.Exists(logFilePath))
                {
                    Directory.CreateDirectory(logFilePath);
                }
                using (StreamWriter w = System.IO.File.AppendText(filepath))
                {
                    w.WriteLine("{0} ", DateTime.Now.ToString() + ":" + notification);
                    w.WriteLine("-------------------------------");
                    w.Dispose();
                }
            }
        }
    }
}