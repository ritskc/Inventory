using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Settings
{
    public class ConnectionSettings
    {
        public static string ConnectionString = "Data Source=MISLW1AD899191;Initial Catalog=Inventory;Persist Security Info=True;UID=sa;PWD=Tom&Jerry1;MultipleActiveResultSets=False;Packet Size=4096;Connection Timeout=1800";
        //public static string ConnectionString = "Data Source=WEBSERVER\\SQLEXPRESS;Initial Catalog=Inventory;Persist Security Info=True;UID=sa;PWD=HiP2318;MultipleActiveResultSets=False;Packet Size=4096;Connection Timeout=1800";
    }
}
