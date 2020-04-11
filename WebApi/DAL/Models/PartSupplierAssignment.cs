using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PartSupplierAssignment
    {
        public long Id { get; set; }
        public long PartID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string MapCode { get; set; }
        public string Description { get; set; }
        public int QtyInHand { get; set; }
        public int QtyInTransit { get; set; }
        public int TotalQty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
