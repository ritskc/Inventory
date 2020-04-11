using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class POReport
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string SupplierName { get; set; }
        public int PoLetterHead { get; set; }
        public string ContactPersonName { get; set; }
        public string PoNo { get; set; }
        public DateTime PoDate { get; set; }
        public string EmailIds { get; set; }
        public string Remarks { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosingDate { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime? AcknowledgementDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string AccessId { get; set; }
        public decimal TotalAmount { get; set; }

        public long PoId { get; set; }
        public long PartId { get; set; }
        public string PartCode { get; set; }
        public string PartDescription { get; set; }
        public string ReferenceNo { get; set; }
        public int SrNo { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public DateTime PartDueDate { get; set; }
        public string Note { get; set; }
        public int AckQty { get; set; }
        public int InTransitQty { get; set; }
        public int ReceivedQty { get; set; }
        public string Status { get; set; }

        public string SupplierContactPersonName { get; set; }
        public string SupplierPhoneNo { get; set; }
        public string SupplierEmailID { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierState { get; set; }
        public string SupplierCountry { get; set; }
        public string SupplierZIPCode { get; set; }
        public string SupplierFAXNo { get; set; }
        
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNo { get; set; }
        public string CompanyFaxNo { get; set; }
        public string CompanyEMail { get; set; }
        public string CompanyContactPersonName { get; set; }
        public string CompanyWHName { get; set; }
        public string CompanyWHAddress { get; set; }
        public string CompanyWHPhoneNo { get; set; }
        public string CompanyWHEmail { get; set; }

        public string TermsConditions { get; set; }


    }
}
