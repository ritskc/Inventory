using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserMenuReport
    {
        public int Id { get; set; }
        public string Report { get; set; }
        public int PriviledgeId { get; set; }

        public List<UserReport> UserReports { get; set; }
    }
}
