using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LPREventRead.Monitor.Controllers
{
    public class MonitorController : ApiController
    {
        [HttpGet]
        public string IdleMinutes()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                InitialCatalog = "LPR",
                UserID = "LPRMonitor",
                Password = "abc123",
            };

            var minutes = "15";
            try
            {
                using (var conn = new SqlConnection(sb.ConnectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand("SELECT Minutes FROM dbo.vMinutesSinceLastRecord", conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        minutes = reader["Minutes"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return minutes;
        }
    }
}
