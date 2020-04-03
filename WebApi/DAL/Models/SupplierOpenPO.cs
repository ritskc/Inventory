using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplierOpenPO
    {
        //S.SupplierName ,[PoId] ,[PartId] ,p.code ,p.Description   ,[ReferenceNo]  ,[UnitPrice],PD.[DueDate],[Note],[AckQty] - (pd.[InTransitQty] +  pd.[ReceivedQty]) as OpenQty ,[SrNo]
        public string SupplierName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ReferenceNo { get; set; }
        public float UnitPrice { get; set; }
        public DateTime DueDate { get; set; }
        public int OpenQty { get; set; }
        public string Note { get; set; }
        public int SrNo { get; set; }
        public string PoNo { get; set; }
    }
}
