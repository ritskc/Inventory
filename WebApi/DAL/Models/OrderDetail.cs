using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class OrderDetail
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PartId { get; set; }
        public long BlanketPOId { get; set; }
        public int BlanketPOAdjQty { get; set; }
        public int SrNo { get; set; }
        public string LineNumber { get; set; }
        public int Qty { get; set; }
        public int ShippedQty { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string Note { get; set; }
        public bool IsForceClosed { get; set; }
        public string PackingSlipNo { get; set; }
        public DateTime ShippingDate { get; set; }

        public Part part { get; set; }
    }
    
}
