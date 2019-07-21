﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PackingSlip
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public string PackingSlipNo { get; set; }
        public DateTime ShippingDate { get; set; }
        public string ShipVia { get; set; }
        public int Crates { get; set; }
        public int Boxes { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal CustomCharge { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public bool IsInvoiceCreated { get; set; }
        public bool IsPaymentReceived { get; set; }
        public string FOB { get; set; }
        public string Terms { get; set; }
        public int ShipmentInfoId { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public List<PackingSlipDetails> PackingSlipDetails { get; set; }
        public CustomerShippingInfo customerShippingInfo { get; set; }
        public Customer CustomerDetail { get; set; }
        public Company CompanyDetail { get; set; }

    }
}