using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string EMail { get; set; }
        public string ContactPersonName { get; set; }
        public string WHName { get; set; }
        public string WHAddress { get; set; }
        public string WHPhoneNo { get; set; }
        public string WHEmail { get; set; }

        public List<Warehouse> Warehouses { get; set; }
    }
}
