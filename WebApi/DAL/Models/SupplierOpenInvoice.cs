using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplierOpenInvoice
    {
        //[InvoiceId],[PartId],InvoiceNo ,sum(Qty-AdjustedInvoiceQty) OpenQty
        public int InvoiceId { get; set; }
        public int PartId { get; set; }
        public string InvoiceNo { get; set; }
        public int OpenQty { get; set; }
    }
}
