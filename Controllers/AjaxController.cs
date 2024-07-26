using Accord.Math.Geometry;
using CallBackUtility.Models;
using CallBackUtility.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CallBackUtility.Controllers
{
    public class AjaxController : Controller
    {
        public JsonResult _Index(string ringstarttime, string ringendtime, string gcallId, string dailedNos, string[] calltypes, string callingNos)
        {
            JsonResult result = null;
            List<Recording> data = new List<Recording>();
            string search = Request.Form.GetValues("search[value]")[0];
            string draw = Request.Form.GetValues("draw")[0];
            string order = Request.Form.GetValues("order[0][column]")[0];
            string orderByColumnName = Request.Form.GetValues(string.Format("columns[{0}][data]", order))[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            Int64 startRec = Convert.ToInt64(Request.Form.GetValues("start")[0]);
            Int64 pageSize = Convert.ToInt64(Request.Form.GetValues("length")[0]);
            Int32 total_records_count = 0;
            data = GetRecordings(out total_records_count, formatDateTime(ringstarttime), formatDateTime(ringendtime), gcallId, dailedNos, calltypes, callingNos, search, orderByColumnName, orderDir, startRec, pageSize).ToList();
            result = this.Json(new
            {
                draw = Convert.ToInt64(draw),
                recordsTotal = total_records_count,
                recordsFiltered = total_records_count,
                data = data
            }, JsonRequestBehavior.AllowGet);

            return result;
        }

        private string formatDateTime(string ringstarttime)
        {
            //  string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());           
            DateTime date = DateTime.ParseExact(ringstarttime, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            date = date.ToString().ToLower().Contains("pm") ? date.AddSeconds(59) : date;
            //  string formattedDate = date.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            string _ringstarttime = date.ToString("s");
            return _ringstarttime;
        }

        private IList<Recording> GetRecordings(out Int32 totalCountBeforePaging, string ringstarttime = null, string ringendtime = null, string gcallId = null, string dailedNos = null, string[] calltypes = null, string callingNos = null, string searched = null, string orderByColumnName = null, string orderDirection = null, Int64 start_range = 0, Int64 pageSize = 0)
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            searched = searched.Length > 0 ? searched.TrimStart(' ').TrimEnd(' ').Replace("\t", "") : searched;
            string inner_query = string.Empty;
            string count_query = string.Empty;
            totalCountBeforePaging = 0;
            StringBuilder filters = new StringBuilder();
            List<Recording> recordings = new List<Recording>();
            try
            {
                int date_started_year = Convert.ToDateTime(ringstarttime).Year;
                int date_end_year = Convert.ToDateTime(ringendtime).Year;
                bool isSingleTable = date_started_year == date_end_year;
                Int64 from = start_range + 1;
                Int64 to = start_range + pageSize;
                // string count_filters = "";
                orderByColumnName = resetColumnName(orderByColumnName);
                if (isSingleTable)
                {
                    filters.Append(" from  recordings_" + date_started_year + "  where  startedat>='" + ringstarttime + "' and startedat<='" + ringendtime + "' and  audiochans=1 and min_segment=1");
                }
                else
                {
                    filters.Append("from  recordings_" + date_started_year + " a full outer join recordings_" + date_end_year + " b   on 1=2  where (a.startedat>='" + ringstarttime + "' or  b.startedat<='" + ringendtime + "')  and ( a.audiochans=1 or b.audiochans=1)  and ( a.min_segment=1 or b.min_segment=1) ");
                }

                if (!string.IsNullOrEmpty(gcallId) && gcallId != "None selected" && (gcallId.Split(',').Count() > 0))
                {
                    gcallId = gcallId.Replace(",", "','").Replace(" ", "");
                    filters.Append(isSingleTable ? " and callid in ('" + gcallId + "') " : " and (a.callid in ('" + gcallId + "') or b.callid in ('" + gcallId + "')) ");
                }
                if (!string.IsNullOrEmpty(dailedNos) && dailedNos != "None selected" && (dailedNos.Split(',').Count() > 0))
                {
                    dailedNos = dailedNos.Replace(",", "','").Replace(" ", "");
                    filters.Append(isSingleTable ? " and calledparty in ('" + dailedNos + "')" : " and (a.calledparty in ('" + dailedNos + "') or b.calledparty in ('" + dailedNos + "')");
                }
                if (calltypes.Count() > 0)
                {
                    filters.Append(isSingleTable ? "and dirn in (" + string.Join(",", calltypes).Replace("'", "") + ")" : "and (a.dirn in (" + string.Join(",", calltypes).Replace("'", "") + ") or b.dirn in (" + string.Join(",", calltypes).Replace("'", "") + "))");
                }
                if (!string.IsNullOrEmpty(callingNos) && callingNos != "None selected" && (callingNos.Split(',').Count() > 0))
                {
                    callingNos = callingNos.Replace(",", "','").Replace(" ", "");
                    filters.Append(isSingleTable ? " and callingparty in ('" + callingNos + "')" : " and (a.callingparty in ('" + callingNos + "') or b.callingparty in ('" + callingNos + "'))");
                }
                if (!string.IsNullOrEmpty(searched))
                {
                    searched = searched.TrimStart(' ').TrimEnd(' ');
                    string[] dayNames = DateTimeFormatInfo.CurrentInfo.DayNames;
                    if (dayNames.Any(day => searched.IndexOf(day, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        filters.Append(isSingleTable ? " or startdayname like '%" + searched + "%'  " : "   or a.startdayname like '%" + searched + "%' or b.startdayname like '%" + searched + "%' ");
                    }
                    else if (searched.Split(':').Length == 3)
                    {
                        filters.Append(isSingleTable ? " and t_duration=" + TimeSpan.Parse(searched).TotalSeconds : " and (a.t_duration=" + TimeSpan.Parse(searched).TotalSeconds + " or b.t_duration=" + TimeSpan.Parse(searched).TotalSeconds + ")");
                    }
                    else if (searched.Split('/').Length == 3)
                    {
                        filters.Append(isSingleTable ? "or _startdate like '%" + searched + "%'" : "  or a._startdate like '%" + searched + "%' or b._startdate like '%" + searched + "%'");
                    }
                    else if ("inbound".Contains(searched.ToLower()) || "outbound".Contains(searched.ToLower()) || "undefined".Contains(searched.ToLower()))
                    {
                        filters.Append(callDirectionValue(searched, isSingleTable));
                    }
                    else
                    {
                        filters.Append(isSingleTable ?
                            " and ( callingparty like '%" + searched + "%'" + " or agentname like '%" + searched + "%'  or callid like '%" + searched + "%' or skill like '%" + searched + "%' or services like '%" + searched + "%')" :
                            " and ( a.callingparty like '%" + searched + "%' or a.callingparty like '%" + searched + "%' or a.agentname like '%" + searched + "%' or b.agentname like '%" + searched + "%'   or a.callid like '%" + searched + "%' or b.callid like '%" + searched + "%' or a.skill like '%" + searched + "%' or b.skill like '%" + searched + "%' or a.services like '%" + searched + "%' or a.services like '%" + searched + "%')");
                    }
                }
                if (isSingleTable)
                {
                    count_query = "WITH records AS ( select RecordId FILTERS ) SELECT count(RecordId) as count FROM records";
                    inner_query = "WITH records AS ( select RecordId,startedat,_year,startdayname,t_duration,_startdate," + "agentname,inum,tarid,callingparty,calledparty,callid,dirn,sessionsCount ,skill,services FILTERS ) SELECT * FROM records ";
                }
                else
                {
                    count_query = " select count(coalesce(a.inum,b.inum)) as count  FILTERS  ";
                    inner_query = "select coalesce(a.RecordId,b.RecordId)RecordId, coalesce(a.startedat,b.startedat)startedat, coalesce(a.callid,b.callid)callid, coalesce(a.t_duration,b.t_duration)t_duration, coalesce(a._year,b._year)year, coalesce(a.startdayname,b.startdayname)startdayname, coalesce(a._startdate,b._startdate)_startdate, coalesce(a.agentname,b.agentname)agentname, coalesce(a.inum,b.inum)inum, coalesce(a.tarid,b.tarid)tarid, coalesce(a.callingparty,b.callingparty)callingparty, coalesce(a.calledparty,b.calledparty)calledparty, coalesce(a.dirn,b.dirn)dirn, coalesce(a.sessionsCount,b.sessionsCount)sessionsCount, coalesce(a.skill,b.skill)skill, coalesce(a.services,b.services)services FILTERS ";
                }
                string pagging = "  order by " + orderByColumnName + " " + orderDirection + " offset " + start_range + " rows fetch next " + pageSize + " rows only";
                string final_query = inner_query.Replace("FILTERS", filters.ToString()) + pagging;
                string ddls_query = count_query.Replace("FILTERS", filters.ToString());
                totalCountBeforePaging = DBHandler.executeQueryForCount(LogPath, ddls_query);
                recordings = DBHandler._executeQueryForList(LogPath, final_query).ToList();
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath, "error: " + ex.Message);
            }
            return recordings;
        }

        private string callDirectionValue(string directionText, bool isSingleTable)
        {
            string Value = isSingleTable ? " or dirn =1" : " or (a.dirn =1 or b.dirn=1)";
            switch (directionText.ToLower())
            {//\"or (a.dirn = " + (searched.ToLower() == "inbound" ? 2 : (searched.ToLower() == "outbound" ? 3 : 1)) + " or b.dirn = " + (searched.ToLower() == "inbound" ? 2 : (searched.ToLower() == "outbound" ? 3 : 1)) + ") 
                case "inbound":
                    Value = isSingleTable ? " or dirn =2" : " or (a.dirn =2 or b.dirn=2)";
                    break;
                case "outbound":
                    Value = isSingleTable ? " or dirn =3" : " or (a.dirn =3 or b.dirn=3)";
                    break;
                default:
                    Value = "";
                    break;
            }
            return Value;
        }

        private string parseDatetimeIntoDBFormat(string inputDate)
        {
            DateTime date = DateTime.Parse(inputDate);
            // Set time to end of day (23:59:59)
            DateTime endDate = date.Date.AddDays(1).AddSeconds(-1);
            string formattedDate = endDate.ToString("yyyy-dd-MM HH:mm:ss.fff");
            return formattedDate;
        }

        private string resetColumnName(string orderByColumnName)
        {
            orderByColumnName = string.IsNullOrEmpty(orderByColumnName) ? "startedat" : orderByColumnName;
            switch (orderByColumnName)
            {
                case ("t_duration"):
                    return "t_duration";
                case ("startdayname"):
                    return "startdayname";
                case ("_startdate"):
                    return "startedat";
                case ("calledparty"):
                    return "calledparty";
                case ("callingparty"):
                    return "callingparty";

                case ("agentname"):
                    return "agentname";
                case ("callid"):
                    return "callid";
                case ("dirn"):
                    return "dirn";
                case ("services"):
                    return "services";
                case ("skill"):
                    return "skill";
                default:
                    return "startedat";
            }
        }
    }
}