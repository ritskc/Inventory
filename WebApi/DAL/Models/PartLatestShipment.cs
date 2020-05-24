using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PartLatestShipment
    {
        //NAME as CustomerName ,[code],[Description],[PackingSlipNo],[ShippingDate],[QTY]
        public string CustomerName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PackingSlipNo { get; set; }
        public DateTime ShippingDate { get; set; }
        public int Qty { get; set; }
    }

    public class PartTotalShipment
    {
        //NAME as CustomerName ,[code],[Description],[PackingSlipNo],[ShippingDate],[QTY]
        public int ShippedQty { get; set; }
        public int MonthlyExcessQty { get; set; }
    }

    public class PartTotalInvoiceQty
    {
        //NAME as CustomerName ,[code],[Description],[PackingSlipNo],[ShippingDate],[QTY]
        public int InvoiceQty { get; set; }        
    }
}
