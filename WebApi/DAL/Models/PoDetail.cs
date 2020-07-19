using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PoDetail
    {
        public long Id { get; set; }
        public long PoId { get; set; }
        public long PartId { get; set; }
        public string ReferenceNo { get; set; }
        public int SrNo { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime DueDate { get; set; }
        public string Note { get; set; }
        public int AckQty { get; set; }
        public int InTransitQty { get; set; }
        public int ReceivedQty { get; set; }
        public bool IsClosed { get; set; }
        public bool IsForceClosed { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? PartAcknowledgementDate { get; set; }
        public string InvoiceNo { get; set; }

        public Part part { get; set; }       
    }
}
