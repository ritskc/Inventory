using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PackingSlipBoxDetails
    {
        public int Id { get; set; }
        public int PackingSlipId { get; set; }
        public int PackingSlipDetailId { get; set; }
        public int PartId { get; set; }
        public int Qty { get; set; }
        public int BoxeNo { get; set; }
        public string Barcode { get; set; }
        public bool IsScanned { get; set; }
    }

    public class PackingSlipScanBoxeStatus
    {
        public int Id { get; set; }
        public int PackingSlipId { get; set; }
        public string PackingSlipNo { get; set; }
        public int PackingSlipDetailId { get; set; }
        public int LineNumber { get; set; }
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public int Qty { get; set; }
        public int TotalBox { get; set; }
        public int BoxeNo { get; set; }
        public string Barcode { get; set; }
        public bool IsScanned { get; set; }
    }
}
