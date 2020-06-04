using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserActivityReport
    {        
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime ActionTime { get; set; }        
    }
}
