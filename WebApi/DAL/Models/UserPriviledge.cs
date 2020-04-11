using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserPriviledge
    {        
        public int Id { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }

        public List<UserPriviledgeDetail> UserPriviledgeDetails { get; set; }

        public List<UserMenu> UserMenus { get; set; }
    }
}
