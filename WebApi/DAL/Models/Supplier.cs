using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string ContactPersonName { get; set; }
        public string PhoneNo { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZIPCode { get; set; }
        public string FAXNo { get; set; }
        public string DateFormat { get; set; }
        public int noofstages { get; set; }
        public int CompanyProfileID { get; set; }
        public int PoLetterHead { get; set; }

        public List<SupplierTerms> Terms { get; set; }
    }
}
