using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class StockPrice
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public decimal SupplierPrice { get; set; }
        public decimal CustomerPrice { get; set; }
        public int Qty { get; set; }
    }
}
