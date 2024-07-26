using CallBackUtility.Models;
using CallBackUtility.ViewModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CallBackUtility.Utility
{
    public static class DBHandler
    {
        internal static List<DDL> _executeQueryForDDLs(string LogPath, string sql, string totalCountQuery, out Int64 totalCount)
        {
            string count = "";
            ApplicationDbContext db = new ApplicationDbContext();
            List<DDL> ddlList = new List<DDL>();
            try
            {
            // LogsManager.Notify(LogPath, "DDLs totalCountQuery: " + totalCountQuery);
                count = db.Database.SqlQuery<string>(totalCountQuery).First();
                
             //   LogsManager.Notify(LogPath, "DDLs query: " + sql);
                ddlList = db.Database.SqlQuery<DDL>(sql).ToList<DDL>();
            }
            catch (Exception ex)
            {
                count = "";
                LogsManager.Logs(LogPath, "DDLs query: " + sql);
                LogsManager.Logs(LogPath, "DDLs Load Error: " + ex.Message);
            }
            totalCount = string.IsNullOrEmpty(count) ? 0 : Int64.Parse(count);
            return ddlList;
        }

        internal static int executeQueryForCount(string LogPath, string sql)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //Int64 count = 0;
            
            int count = 0;

            try
            {
                /*LogsManager.Notify(LogPath, "Count Table Query: " + sql);*/
                count = db.Database.SqlQuery<int>(sql).First();
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath, "Count Query Error: " + sql);
                LogsManager.Logs(LogPath, ex.Message);
            }
            return count;
        }

       internal static List<Recording> _executeQueryForList(string LogPath, string sql)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Recording> recordings = new List<Recording>();
            try
            {
                /*LogsManager.Notify(LogPath, "List Query: " + sql);*/
                recordings = db.Database.SqlQuery<Recording>(sql).ToList();               
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath, "List Query Error: " +sql);
                LogsManager.Logs(LogPath, ex.Message);
            }           
            return recordings;
        }
    }
}