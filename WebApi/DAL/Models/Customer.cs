using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Customer
    {
       public int Id { get; set; }
       public int CompanyId { get; set; }
       public string Name { get; set; }
       public string AddressLine1 { get; set; }
       public string City { get; set; }
       public string State { get; set; }
       public string ZIPCode { get; set; }
       public string ContactPersonName { get; set; }
       public string TelephoneNumber { get; set; }
       public string FaxNumber { get; set; }
       public string EmailAddress { get; set; }
       public string TruckType { get; set; }
       public string CollectFreight { get; set; }
       public string Comments { get; set; }
       public decimal Surcharge { get; set; }
       public string FOB { get; set; }
       public string Terms { get; set; }
       public decimal RePackingCharge { get; set; }
       public string ShipVia { get; set; }
       public int Invoicingtypeid { get; set; }
       public string EndCustomerName { get; set; }
       public bool DisplayLineNo { get; set; }
       public string Billing { get; set; }

        public List<CustomerShippingInfo> ShippingInfos { get; set; }
    }
}
