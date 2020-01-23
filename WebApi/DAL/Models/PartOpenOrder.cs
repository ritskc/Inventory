using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PartOpenOrder
    {
        //[Name],[Code],[Description],[PONo] ,[PODate] ,OD.[DueDate], Qty - ShippedQty as openqty 
        public string CustomerName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; }       
        public int OpenQty { get; set; }
    }
}
