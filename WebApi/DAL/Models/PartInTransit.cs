using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PartInTransit
    {
        //[Code] ,[Description] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[PoNo],[SrNo] ,[PartId],[Qty] 
        public string Code { get; set; }
        public string Description { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ETA { get; set; }
        public string PoNo { get; set; }
        public int SrNo { get; set; }
        public int PartId { get; set; }
        public int Qty { get; set; }        
    }
}
