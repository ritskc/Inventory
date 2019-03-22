using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class CustomerShippingInfo
    {
      public int Id { get; set; }      
      public string Name { get; set; }
      public string ContactPersonName { get; set; }
      public string AddressLine1 { get; set; }
      public string City { get; set; }
      public string State { get; set; }
      public string ZIPCode { get; set; }
      public bool IsDefault { get; set; }
    }
}
