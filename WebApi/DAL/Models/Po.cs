using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Po
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public int SupplierId { get; set; }
        public string PoNo { get; set; }
        public DateTime PoDate { get; set; }       
        public string EmailIds { get; set; }
        public string Remarks { get; set; }
        public bool IsClosed { get; set; }        
        public DateTime? ClosingDate { get; set; }
        public string IsAcknowledged { get; set; }
        public DateTime? AcknowledgementDate { get; set; }
        public string PaymentTerms { get; set; }      
        public string DeliveryTerms { get; set; }

        public List<PoDetail> poDetails { get; set; }
        public List<PoTerm> poTerms { get; set; }
    }
}
