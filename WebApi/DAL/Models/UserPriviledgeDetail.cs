using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserPriviledgeDetail
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Menu { get; set; }
        public int ActionId { get; set; }
        public string Action { get; set; }
        public bool IsApplicable { get; set; }
        public bool IsPermitted { get; set; }
        public int UserPriviledgeId { get; set; }
        public int UserMenuActionId { get; set; }
    }
}
