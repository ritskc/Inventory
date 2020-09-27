using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SalesData
    {
        //CM.Name as CustomerName ,PM.Code,PM.Description,PSM.PackingSlipNo,PSM.ShippingDate ,PSD.[PackingSlipId],PSD.[PartId] ,[Qty],PSD.[UnitPrice],PSD.[Price] 

        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PackingSlipNo { get; set; }
        public DateTime ShippingDate { get; set; }
        public int Qty { get; set; }
        public int PartId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalPurchasePrice { get; set; }
    }
}
