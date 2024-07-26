using CallBackUtility.Models;
using CallBackUtility.Utility;
using CallBackUtility.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using WebGrease;

namespace CallBackUtility.Controllers
{
    [Authorize(Roles = AppRoles.RecordingRoles)]
    public class RecordingsController : Controller
    {
      
        private ApplicationDbContext db = new ApplicationDbContext();
       
        public async Task<ActionResult> Index()
        {
           // clearUserSpecifiedFilesAndFolders();
            return View();
        }
        
     
        public ActionResult Indexx()
        {
            return View();
        }
       
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + "DecodedAudios/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            { string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
                LogsManager.Logs(LogPath, "error: " + ex.Message);
                LogsManager.Logs(LogPath,    "path: " + path);
                result = false;
            }
            return result;
        }

        private string setFiles(string filename, string TarFileName)
        { 
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            string slash = "/";
            try
            {
                string ffmpegLocation = ConfigurationManager.AppSettings["ProductionffmpegLocation"].ToString();
                string AudioFilesPath = ConfigurationManager.AppSettings["ProductionAudioFilesServerPath"].ToString();
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()))
                {
                    ffmpegLocation = ConfigurationManager.AppSettings["LabffmpegLocation"].ToString();
                    AudioFilesPath = ConfigurationManager.AppSettings["LabAudioFilesServerPath"].ToString();
                }
                if (AudioFilesPath.Contains(@"\"))
                {
                    slash = @"\";
                }
                filename = filename.Replace(".xml", ".wav");
                string userid = User.Identity.GetUserId();
                string UserLocation = AudioFilesPath + userid + slash;
                string clientPlayPath = UserLocation + "DecodedAudios" + slash;
                string UserDecodedWavPath = Server.MapPath(clientPlayPath);
                string serverEncodedWavPathwFile = Server.MapPath(UserLocation + filename);
                UserLocation = Server.MapPath(UserLocation);
                if (this.CreateFolderIfNeeded(UserLocation))
                {
                    DirectoryInfo folder_path = new DirectoryInfo(UserDecodedWavPath);
                    if (!folder_path.GetFiles(filename).Any())
                    {
                        AudioFormatter.DecodeAudio(LogPath, TarFileName, filename, serverEncodedWavPathwFile, UserLocation, UserDecodedWavPath, ffmpegLocation);
                        if (!folder_path.GetFiles(filename).Any())
                        {
                            LogsManager.Notify(LogPath, UserDecodedWavPath + filename + " file does not exist.");
                        }
                    }
                }
                return (clientPlayPath + filename);
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath, "SetFile: error : " + ex.Message);
            }
            return "";
        }

        private string _setFiles(string filename, string TarFileName)
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            string slash = "/";
            try
            {
                string AudioFilesPhysicalPath = ConfigurationManager.AppSettings["ProductionAudioFilesPhysicalPath"].ToString();
                string ffmpegLocation = ConfigurationManager.AppSettings["ProductionffmpegLocation"].ToString();
                string AudioFilesServerPath = ConfigurationManager.AppSettings["ProductionAudioFilesServerPath"].ToString();
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()))
                {
                    ffmpegLocation = ConfigurationManager.AppSettings["LabffmpegLocation"].ToString();
                    AudioFilesPhysicalPath = ConfigurationManager.AppSettings["LabAudioFilesPhysicalPath"].ToString();
                    AudioFilesServerPath = ConfigurationManager.AppSettings["LabAudioFilesServerPath"].ToString();
                }
                if (AudioFilesPhysicalPath.Contains(@"\"))
                {
                    slash = @"\";
                }
                filename = filename.Replace(".xml", ".wav");
                string userid = User.Identity.GetUserId();
                AudioFilesServerPath = AudioFilesServerPath + userid;
                string UserLocation = AudioFilesPhysicalPath + userid + slash;
                string clientPlayPath = UserLocation + "DecodedAudios" + slash;
                string serverEncodedWavPathwFile = UserLocation + filename;
                if (this.CreateFolderIfNeeded(UserLocation))
                {
                    DirectoryInfo folder_path = new DirectoryInfo(clientPlayPath);
                    if (!folder_path.GetFiles(filename).Any())
                    {
                        AudioFormatter.DecodeAudio(LogPath, TarFileName, filename, serverEncodedWavPathwFile, UserLocation, clientPlayPath, ffmpegLocation);
                        if (!folder_path.GetFiles(filename).Any())
                        {
                            LogsManager.Logs(LogPath, clientPlayPath + filename + " file does not exist.");
                        }
                    }
                }
                return (AudioFilesServerPath + "/DecodedAudios/" + filename);
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath,"SetFile: error : " + ex.Message);
            }
            return "";
        }

        [HttpPost]
        public ActionResult getChildRecording(int tarid,string inum)//parent call
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
           
            string fileNamePath = _setTarFiles(tarid.ToString(), inum);
            return Json(new { filepath = fileNamePath });
        }

        private string _setTarFiles(string tarfile,string inum)
        {
            tarfile = tarfile + ".tar";
            string filename = inum + ".wav";
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            string slash = "/";
            try
            {
                string ffmpegLocation = ConfigurationManager.AppSettings["ProductionffmpegLocation"].ToString();
                string AudioFilesPath = ConfigurationManager.AppSettings["ProductionAudioFilesServerPath"].ToString();
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()))
                {
                    ffmpegLocation = ConfigurationManager.AppSettings["LabffmpegLocation"].ToString();
                    AudioFilesPath = ConfigurationManager.AppSettings["LabAudioFilesServerPath"].ToString();
                }
                if (AudioFilesPath.Contains(@"\"))
                {
                    slash = @"\";
                }
              
                string userid = User.Identity.GetUserId();
                string UserLocation = AudioFilesPath + userid + slash;
                string clientPlayPath = UserLocation + "DecodedAudios" + slash;
                string UserDecodedWavPath = Server.MapPath(clientPlayPath);
                string serverEncodedWavPathwFile = Server.MapPath(UserLocation + filename);
                UserLocation = Server.MapPath(UserLocation);
                if (this.CreateFolderIfNeeded(UserLocation))
                {
                    DirectoryInfo folder_path = new DirectoryInfo(UserDecodedWavPath);
                    if (!folder_path.GetFiles(filename).Any())
                    {
                        AudioFormatter.DecodeAudio(LogPath, tarfile, filename, serverEncodedWavPathwFile, UserLocation, UserDecodedWavPath, ffmpegLocation);
                        if (!folder_path.GetFiles(filename).Any())
                        {
                            LogsManager.Notify(LogPath, UserDecodedWavPath + filename + " file does not exist.");
                        }
                    }
                }
                return (clientPlayPath + filename);
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath, "SetFile: error : " + ex.Message);
            }
            return "";
        }

        [HttpPost]
        public ActionResult getRecordings(string callid, string year)//parent calls
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            
            List<Recording> recordings = new List<Recording>();
            
            string query = "select inum,tarid from Recordings_"+year+" where callid='" + callid + "'  and audiochans = 1 order by segmentnum ";
            recordings = DBHandler._executeQueryForList(LogPath, query);
           
            var fileName = new List<string>();
            foreach (Recording _recording in recordings)
            {
                fileName.Add(_setTarFiles(_recording.tarid.ToString(), _recording.inum));
            }
            return Json(new { filepath = fileName });
        }

        [HttpPost]
        public ActionResult getChildRecordings(Int64 recordingId,string callid,string year)
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            List<Recording> recordings = new List<Recording>(); 
            string query = "select startdayname,duration,_startdate,calledparty,inum,tarid,callingparty,agentname,callid,skill,services,dirn from Recordings_" + year + " where  callid='" + callid + "'  and  audiochans=1 order by segmentnum";        
            recordings = DBHandler._executeQueryForList(LogPath, query);         
            return Json(new { count = recordings.Count, recording = recordings });
        }

        public JsonResult loadDDL()
        {          
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            List<DDL> ddls = new List<DDL>();           
          JsonResult result = null;
            string startDateTime = Request.QueryString["startDateTime"];
            string endDateTime = Request.QueryString["endDateTime"];
           // string search = Request.QueryString["searched"];
            Int64 page = int.Parse(Request.QueryString["page"]);
            Int64 pagesize = int.Parse(Request.QueryString["pagesize"]);
            string columnName = Request.QueryString["columnName"].ToString().ToLower().Replace("inums","inum").Replace("callingnos", "core_callingparty").Replace("dailednos", "core_calledparty");
            Int64 totalCount = 0;
            Int64 offset =(page - 1) * pagesize ;
            string condition = "    segmentnum=1 and ";
            //  search = string.IsNullOrEmpty(search)?"": columnName+" like '%" +search+"%' and ";
            // string totalCountQuery = "select Convert(nvarchar(50),count(distinct "+ columnName + ")) as count from  Recordings where " + search+ " _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     "+condition;
            string totalCountQuery = "select Convert(nvarchar(50),count(distinct " + columnName + ")) as count from  Recordings where "+condition+" _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     " ;
            //  string query = "select "+ columnName + " as id from  Recordings where  " + search + " _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     "+ condition + " group by "+ columnName + " order by "+ columnName + "  offset " + offset + "  rows fetch next " + pagesize + " rows only";
            string query = "select " + columnName + " as id from  Recordings where  " + condition + "  _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     group by " + columnName + " order by " + columnName + "  offset " + offset + "  rows fetch next " + pagesize + " rows only";
            ddls = DBHandler._executeQueryForDDLs(LogPath,query, totalCountQuery, out totalCount);
            ddls.ForEach(x=>x.text=x.id);
            //result = this.Json(new
            //{
            //    page= page,
            //    data = ddls,
            //    totalCount = totalCount
            //}, JsonRequestBehavior.AllowGet);
            result = this.Json(new
            {
                page = page,
                data = ddls,
                totalCount = totalCount
            }, JsonRequestBehavior.AllowGet);
            //return jsSerializer.Serialize(new
            //{
            //    page = page,
            //    data = ddls,
            //    totalCount = totalCount
            //});
            return result;
        }

        //[HttpGet]
        //public string loadDailedNosDDL()
        //{
        //    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //    string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
        //    List<DDL> ddls = new List<DDL>();
        //   // string tableName = "Recordings";
        //    JsonResult result = null;
        //    string startDateTime = Request.QueryString["startDateTime"];
        //    string endDateTime = Request.QueryString["endDateTime"];
        //    string search = Request.QueryString["searched"];
        //    int page = int.Parse(Request.QueryString["page"]);
        //    int pagesize = int.Parse(Request.QueryString["pagesize"]);
        //    Int64 totalCount = 0;
        //    int offset = (page - 1) * pagesize;
        //    search = string.IsNullOrEmpty(search) ? "" : "columnName like '%" + search + "%' and ";
        //    string totalCountQuery = "select Convert(nvarchar(50),count(distinct columnName)) as count from  Recordings  where " + search+"  _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     condition ";
           
        //    string query = "select columnName as id from  Recordings where  " + search + " _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     condition group by columnName order by columnName  offset " + offset + "  rows fetch next " + pagesize + " rows only";
        //    ddls = DBHandler._executeQueryForExtensionsDDLs(LogPath, query, totalCountQuery, out totalCount);
        //    //result = this.Json(new
        //    //{
        //    //    data = ddls,
        //    //    totalCount = totalCount
        //    //}, JsonRequestBehavior.AllowGet);
         
        //    return jsSerializer.Serialize(new
        //    {
        //        page = page,
        //        data = ddls,
        //        totalCount = totalCount
        //    });
        //}
        //[HttpGet]
        //public string loadCallingNosDDL()
        //{
        //    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //    string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
        //    List<DDL> ddls = new List<DDL>();
        //    string tableName = "Recordings";
        //    //JsonResult result = null;
        //    string startDateTime = Request.QueryString["startDateTime"];
        //    string endDateTime = Request.QueryString["endDateTime"];
        //    string search = Request.QueryString["searched"];
        //    int page = int.Parse(Request.QueryString["page"]);
        //    int pagesize = int.Parse(Request.QueryString["pagesize"]);
        //    Int64 totalCount = 0;
        //    int offset = (page - 1) * pagesize;
        //    search = string.IsNullOrEmpty(search) ? "" : "columnName like '%" + search + "%' and ";
        //    string totalCountQuery = "select Convert(nvarchar(50),count(distinct columnName)) as  count from  " + tableName + "  where " + search + "  _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME)     condition ";
        //    string query = "select columnName as id from  Recordings where  " + search + " _starttime>=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + startDateTime + "', 103), 121) AS DATETIME) and _endTime<=CAST(CONVERT(VARCHAR(24), CONVERT(DATETIME,'" + endDateTime + "', 103), 121) AS DATETIME) condition group by columnName order by columnName  offset " + offset + "  rows fetch next " + pagesize + " rows only";
        //    ddls = DBHandler._executeQueryForCallingNosDDLs(LogPath, query, totalCountQuery, out totalCount);         
        //    return jsSerializer.Serialize(new
        //    {
        //        page = page,
        //        data = ddls,
        //        totalCount = totalCount
        //    });
        //}

        protected override void Dispose(bool disposing)
        {
            if (User.Identity.IsAuthenticated)
            {

                RedirectToAction("LogOff", "Account");

            }
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
