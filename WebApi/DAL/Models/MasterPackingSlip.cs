using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MasterPackingSlip
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public string MasterPackingSlipNo { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Comment { get; set; }        
        public bool IsPOSUploaded { get; set; }
        public string POSPath { get; set; }
        public string TrakingNumber { get; set; }

        public List<PackingSlip> PackingSlips { get; set; }

        public List<PackingSlipBoxDetails> PackingSlipBoxDetails { get; set; }

    }
}
