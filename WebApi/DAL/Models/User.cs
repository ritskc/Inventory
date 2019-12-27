using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int PriviledgeId { get; set; }
        public string PriviledgeName { get; set; }
        public int UserTypeId { get; set; } //1 = company, 2= Customer, 3 = Supplier
        public string UserType { get; set; }
        public int MapId { get; set; } // supplierid / customer id incase of UserType not 1
        public string Token { get; set; }
        public DateTime? TokenExpires { get; set; }

        public List<Report> Reports { get; set; }
        public List<SSRSReport> SSRSReports { get; set; }

}
}
