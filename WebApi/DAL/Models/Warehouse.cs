using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }        
        public string Name { get; set; }        
    }
}
