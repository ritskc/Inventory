using DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DAL.Settings
{
    public class ConnectionSettings
    {
        private const string ConnectionFile = "connection.json";
        //public static string ConnectionString = "Data Source=MISLW1AD899191;Initial Catalog=Inventory;Persist Security Info=True;UID=sa;PWD=Tom&Jerry1;MultipleActiveResultSets=False;Packet Size=4096;Connection Timeout=1800";
        //public static string ConnectionString = "Data Source=WEBSERVER\\SQLEXPRESS;Initial Catalog=Inventory;Persist Security Info=True;UID=sa;PWD=HiP2318;MultipleActiveResultSets=False;Packet Size=4096;Connection Timeout=1800";
        private static string cacheConnection = string.Empty;
        public static string ConnectionString
        {
            get
            {
                if (cacheConnection != string.Empty)
                    return cacheConnection;
               
                if (File.Exists(ConnectionFile))
                {
                    var content = File.ReadAllText(ConnectionFile);
                    if (!string.IsNullOrEmpty(content))
                    {
                        var connectionInfo = JsonConvert.DeserializeObject<ConnectionInfo>(content);
                        cacheConnection = connectionInfo.ConnectionString; ;
                        return connectionInfo.ConnectionString;
                    }
                }
                else
                {
                    var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConnectionFile);
                    if (File.Exists(fullPath))
                    {
                        var content = File.ReadAllText(fullPath);
                        if (!string.IsNullOrEmpty(content))
                        {
                            var connectionInfo = JsonConvert.DeserializeObject<ConnectionInfo>(content);
                            cacheConnection = connectionInfo.ConnectionString; ;
                            return connectionInfo.ConnectionString;
                        }
                    }
                }
                return null;
            }
        }
    }

}
