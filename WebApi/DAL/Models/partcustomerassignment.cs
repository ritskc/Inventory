using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PartCustomerAssignment
    {
      public long Id { get; set; }
      public long PartId { get; set; }
      public int CustomerId { get; set; }
      public string MapCode { get; set; }
      public string Description { get; set; }
      public decimal Weight { get; set; }
      public decimal Rate { get; set; }
      public bool SurchargeExist { get; set; }
      public decimal SurchargePerPound { get; set; }      
    }
}
