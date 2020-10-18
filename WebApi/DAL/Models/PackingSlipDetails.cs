using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PackingSlipDetails
    {
        public int Id { get; set; }
        public int PackingSlipId { get; set; }
        public bool IsBlankOrder { get; set; }
        public string OrderNo { get; set; }
        public int OrderId { get; set; }
        public int? OrderDetailId { get; set; }
        public long SupplierInvoiceId { get; set; }
        public int SupplierInvoiceDetailId { get; set; }
        public int SrNo { get; set; }
        public int PartId { get; set; }
        public int Qty { get; set; }
        public int Boxes { get; set; }
        public int SupplierInvoiceOpenQty { get; set; }
        public bool InBasket { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal Surcharge { get; set; }
        public decimal SurchargePerPound { get; set; }
        public decimal SurchargePerUnit { get; set; }
        public decimal TotalSurcharge { get; set; }
        public string LineNumber { get; set; }
        public int ExcessQty { get; set; }
        public bool IsRepackage { get; set; }
        public int WarehouseId { get; set; }
        public bool DefaultWarehouse { get; set; }

        public Part PartDetail { get; set; }

        public List<PackingSlipBoxDetails> PackingSlipBoxDetails { get; set; }
    }
}
