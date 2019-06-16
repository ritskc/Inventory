using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class EntityTracker
    {
        public int CompanyId { get; set; }
        public string FinYear { get; set; }
        public EntityType Entity { get; set; }
        public string EntityInString { get; set; }
        public int AvailableNo { get; set; } 
    }

    public enum EntityType
    {
        PO,
        SUPP_INVOICE,
        ORDER,
        PACKING_SLIP
    }
}
