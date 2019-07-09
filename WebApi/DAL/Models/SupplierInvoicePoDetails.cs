using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplierInvoicePoDetails
    {
        public long Id { get; set; }
        public long PartId { get; set; }
        public long InvoiceId { get; set; }
        public long InvoiceDetailId { get; set; }
        public long PoId { get; set; }
        public long PODetailId { get; set; }
        public string PONo { get; set; }
        public int Qty { get; set; }        
    }
}
