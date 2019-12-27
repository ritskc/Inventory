using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplierIntransitInvoice
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ETA { get; set; }
        public bool IsAirShipment { get; set; }
        public string PoNo { get; set; }
        public string ReferenceNo { get; set; }        
        public bool ByCourier { get; set; }       
        public DateTime UploadedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        
        public long InvoiceDetailId { get; set; }
        public int SrNo { get; set; }
        public long PartId { get; set; }
        public string PartCode { get; set; }
        public int Qty { get; set; }        
        public int BoxNo { get; set; }   
    }
}
