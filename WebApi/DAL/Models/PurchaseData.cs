using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PurchaseData
    {
        //SM.Name SupplierName	,SIM.InvoiceNo ,SIM.InvoiceDate	,SIM.ReceivedDate ,SID.[InvoiceId],SID.[PartId] ,PM.Code ,PM.Description 
        //,Sum(SID.[Qty]) as Qty ,SID.Price,Sum(SID.[Total]) as Total

        public string SupplierName { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public int PartId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
