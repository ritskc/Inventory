using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class OrderMaster
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public bool IsBlanketPO { get; set; }
        public string PONo { get; set; }
        public DateTime PoDate { get; set; }        
        public DateTime DueDate { get; set; }
        public string Remarks { get; set; }        

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
