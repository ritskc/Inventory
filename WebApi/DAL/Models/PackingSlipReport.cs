using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PackingSlipReport
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNo { get; set; }
        public string CompanyFaxNo { get; set; }
        public string CompanyEMail { get; set; }
        public string CompanyContactPersonName { get; set; }
        public string CompanyWHName { get; set; }
        public string CompanyWHAddress { get; set; }
        public string CompanyWHPhoneNo { get; set; }
        public string CompanyWHEmail { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerZIPCode { get; set; }
        public string CustomerContactPersonName { get; set; }
        public string CustomerTelephoneNumber { get; set; }
        public string CustomerFaxNumber { get; set; }
        public string CustomerEmailAddress { get; set; }
        public string CustomerTruckType { get; set; }
        public string CustomerCollectFreight { get; set; }
        public string CustomerComments { get; set; }
        public decimal CustomerSurcharge { get; set; }
        public string CustomerFOB { get; set; }
        public string CustomerTerms { get; set; }
        public decimal CustomerRePackingCharge { get; set; }
        public string CustomerShipVia { get; set; }
        public int CustomerInvoicingtypeid { get; set; }
        public string CustomerEndCustomerName { get; set; }
        public string Billing { get; set; }

        public string ShipmentName { get; set; }
        public string ShipmentContactPersonName { get; set; }
        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentCity { get; set; }
        public string ShipmentState { get; set; }
        public string ShipmentZIPCode { get; set; }
        public bool ShipmentIsDefault { get; set; }

        public string MasterPackingSlipNo { get; set; }
        public string PackingSlipNo { get; set; }
        public string RePackingSlipNo { get; set; }
        public DateTime ShippingDate { get; set; }
        public string ShipVia { get; set; }
        public int Crates { get; set; }
        public int Boxes { get; set; }
        public int TotalBoxes { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal CustomCharge { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public bool IsInvoiceCreated { get; set; }
        public bool IsPaymentReceived { get; set; }
        public string FOB { get; set; }
        public string Terms { get; set; }
        public int ShipmentInfoId { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public string OrderNo { get; set; }   
        public int SrNo { get; set; }
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public string PartDescription { get; set; }
        public int Qty { get; set; }        
        public bool InBasket { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal TotalRePackingCharge { get; set; }
        public decimal SumRePackingCharge { get; set; }
        public decimal Surcharge { get; set; }
        public decimal SurchargePerPound { get; set; }
        public decimal SurchargePerUnit { get; set; }
        public decimal TotalSurcharge { get; set; }
        public int ExcessQty { get; set; }
        public string LineNumber { get; set; }

    }
}
