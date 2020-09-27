using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public long PartId { get; set; }
        public int Qty { get; set; }
        public BusinessConstants.TRANSACTION_TYPE TransactionTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public BusinessConstants.DIRECTION DirectionId { get; set; } 
        public BusinessConstants.INVENTORY_TYPE InventoryType { get; set; }
        public string ReferenceNo { get; set; }
        public string DirectionType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
