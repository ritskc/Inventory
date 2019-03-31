using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Part
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public decimal WeightInKg { get; set; }
        public decimal WeightInLb { get; set; }
        public int MinQty { get; set; }
        public int MaxQty { get; set; }
        public int OpeningQty { get; set; }
        public int SafeQty { get; set; }
        public string DrawingNo { get; set; }
        public bool DrawingUploaded { get; set; }
        public string DrawingFileName { get; set; }
        public bool IsActive { get; set; }
        public bool IsSample { get; set; }
        public string Location { get; set; }

        public List<PartSupplierAssignment> partSupplierAssignments { get; set; }
        public List<PartCustomerAssignment> partCustomerAssignments { get; set; }
    }
}
