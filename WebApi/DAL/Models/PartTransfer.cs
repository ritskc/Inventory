using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PartTransfer
    {
        public long PartId { get; set; }
        public int CompanyId { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public int Qty { get; set; }
        public DateTime ActionTime { get; set; }
    }

    public class WarehouseInventory
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public long PartId { get; set; }
        public int WarehouseId { get; set; }
        public int QtyInHand { get; set; }
        public string Warehousename { get; set; }
    }
}
