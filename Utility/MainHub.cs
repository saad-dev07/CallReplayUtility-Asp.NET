//using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CallBackUtility.Utility
{
    public class MainHub : Hub
    {
        public string CS = ConfigurationManager.ConnectionStrings["CS"].ConnectionString.ToString();
        public void SendCallsRefreshNotification()
        {
            using (SqlConnection connection = new SqlConnection(CS))
            {
                connection.Open();
                string query = "SELECT [RecordingId],[core_callingparty],[core_calledparty],[inum],[agentname],[starttime],[endtime],[filename],[core_callid],[core_globalcallid],[core_calldirection],[isInComing],[segmentnum],[sessionsCount],[TarFilePath],[contactId],[inums],[parentinum] FROM [dbo].[Recordings]";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Notification = null;
                    SqlDependency sqlDependency = new SqlDependency(command);
                    sqlDependency.OnChange += RecordingsDependency_OnChange;
                    //  sqlDependency.OnChange += Dashboarddependency_OnChange;
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    var reader = command.ExecuteReader();
                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MainHub>();
                    context.Clients.All.recieveCallsData();//this 
                }
            }
        }
       
            
        private void RecordingsDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MainHub mainHub = new MainHub();
                mainHub.SendCallsRefreshNotification();
            }
        }
    }
}