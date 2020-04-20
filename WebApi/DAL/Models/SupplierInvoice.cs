using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplierInvoice
    {
        public long Id { get; set; }        
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? ETA { get; set; }
        public bool IsAirShipment { get; set; }
        public string PoNo { get; set; }
        public string ReferenceNo { get; set; }
        public string Email { get; set; }
        public bool ByCourier { get; set; }
        public bool IsInvoiceUploaded { get; set; }
        public bool IsPackingSlipUploaded { get; set; }
        public bool IsTenPlusUploaded { get; set; }
        public bool IsBLUploaded { get; set; }
        public bool IsTCUploaded { get; set; }
        public string InvoicePath { get; set; }
        public string PackingSlipPath { get; set; }
        public string TenPlusPath { get; set; }
        public string BLPath { get; set; }
        public bool IsInvoiceReceived { get; set; }
        public bool DontImpactPO { get; set; }
        public bool IsOpen { get; set; }
        public string Barcode { get; set; }
        public DateTime UploadedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public Company CompanyDetail { get; set; }
        public Supplier SupplierDetail { get;set; }

        public List<SupplierInvoiceDetail> supplierInvoiceDetails { get; set; }
        //public List<SupplierInvoicePoDetails> supplierInvoicePoDetails { get; set; }
    }
}
