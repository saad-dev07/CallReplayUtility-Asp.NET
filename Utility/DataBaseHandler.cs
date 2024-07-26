using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using WebGrease;
using System.Threading.Tasks;
using CallBackUtility.Models;
using System.Data.Entity.Infrastructure;

namespace CallBackUtility.Utility
{
    public class DataBaseHandler
    {
        public static SqlConnection objSqlConnection = null;
        private static string strConnectionString = "";
        private static bool isProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString());
        public static void connect()
        {

           
                strConnectionString = ConfigurationManager.ConnectionStrings["CS"].ConnectionString.ToString();
                objSqlConnection = new SqlConnection(strConnectionString);
                objSqlConnection.Open();
           
        }

        public static void disconnect()
        {
            objSqlConnection.Close();
        }

        public static int executeQuery(string sql)
        {
            int inserted = 0;
            try
            {
                connect();
                SqlCommand objSqlCommand = new SqlCommand(sql, objSqlConnection);
                inserted = objSqlCommand.ExecuteNonQuery();
                objSqlCommand.Dispose();
            }
            catch (Exception ex)
            {
                LogsManager.Logs(sql, ConfigurationManager.AppSettings["isProduction"].ToString());
            }
            return inserted;

        }

        public static int executeScalerValue(string sql)
        {
            connect();
            SqlCommand objSqlCommand = new SqlCommand(sql, objSqlConnection);
            int Id = Convert.ToInt32(objSqlCommand.ExecuteScalar());

            objSqlCommand.Dispose();
            return Id;
        }

        public static string executeQueryForSingleValue(string sql)
        {
            string result = "";
            connect();
            SqlCommand objSqlCommand = new SqlCommand(sql, objSqlConnection);
            SqlDataReader sdr = objSqlCommand.ExecuteReader();
            if (sdr != null && sdr.Read())
            {
                result = sdr[0].ToString();
            }
            sdr.Close();
            objSqlCommand.Dispose();
            return result;
        }
        internal async Task<Int64> GetStringAsync(string sqlQuery)
        {
           // string result = string.Empty;
            try
            {
                strConnectionString = ConfigurationManager.ConnectionStrings["CS"].ConnectionString.ToString();
                using (var connection = new SqlConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                       
                           
                            //command.Parameters.AddWithValue("@TableName", tableName);

                            // ExecuteScalarAsync retrieves the first column of the first row in the result set
                            // This will be the count of fields
                            var count = await command.ExecuteScalarAsync();

                            // Convert the count to int
                            return Convert.ToInt64(count);
                      

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Return a default value or throw the exception further
                throw;
            }
          //  return result;
        }
        public static DataTable executeQueryForDataTable(string sql)
        {
            DataTable objDataTable = new DataTable();
            SqlCommand objSqlCommand = new SqlCommand(sql, objSqlConnection);
            objDataTable.Load(objSqlCommand.ExecuteReader());
            objSqlCommand.Dispose();
            return objDataTable;
        }

        internal async Task<DataTable> GetTableAsync(string sqlQuery)
        {
            var dataTable = new DataTable();
            strConnectionString = ConfigurationManager.ConnectionStrings["CS"].ConnectionString.ToString();
            using (var connection = new SqlConnection(strConnectionString))
            {
                await connection.OpenAsync();

                //   var sqlQuery = "SELECT Id, Name FROM YourTable"; // Adjust the query accordingly
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }
    }
}