using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserMenu
    {        
        public int MenuId { get; set; }
        public string Menu { get; set; }
        public bool IsViewPermitted { get; set; }
        public bool IsReport { get; set; }

        public List<UserAction> UserActions { get; set; }
        public List<UserReport> UserReports { get; set; }
    }
}
