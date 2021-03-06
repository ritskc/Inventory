﻿using System;
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
        public bool IsRepackage { get; set; }
        public string Location { get; set; }
        public int IntransitQty { get; set; }
        public int QtyInHand { get; set; }
        public int OpenOrderQty { get; set; }
        public int SupplierOpenPoQty { get; set; }
        public int MonthlyForecastQty { get; set; }
        public decimal SupplierPrice { get; set; }
        public decimal CustomerPrice { get; set; }
        public string SupplierCode { get; set; }

        public int MonthlyOpeningQty { get; set; }
        public int ShippedQty { get; set; }
        public int InvoiceQty { get; set; }        
        public int MonthlyExcessQty { get; set; }
        public int MonthlyReturnQty { get; set; }
        public int MonthlyRejectQty { get; set; }
        public int MonthlyClosingQty { get; set; }

        public bool IsDoublePricingAllowed { get; set; }
        public decimal FuturePrice { get; set; }
        public int CurrentPricingInEffectQty { get; set; }

        public bool DefaultWarehouse { get; set; }
        public int WarehouseId { get; set; }

        public List<PartSupplierAssignment> partSupplierAssignments { get; set; }
        public List<PartCustomerAssignment> partCustomerAssignments { get; set; }

        public List<StockPrice> stockPrices { get; set; }
    }
}
