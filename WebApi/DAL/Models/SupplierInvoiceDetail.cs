﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SupplierInvoiceDetail
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public int SrNo { get; set; }
        public long PartId { get; set; }
        public string PartCode { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public int AdjustedPOQty { get; set; }
        public int ExcessQty { get; set; }
        public int BoxNo { get; set; }

        public Part PartDetail { get; set; }

        public List<SupplierInvoicePoDetails> supplierInvoicePoDetails { get; set; }
    }
}
