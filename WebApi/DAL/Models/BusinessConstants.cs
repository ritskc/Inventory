using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public static class BusinessConstants
    {
        public const string ENTITY_TRACKER_PO = "PO";
        public const string ENTITY_TRACKER_SUPP_INVOICE = "SUPP_INVOICE";
        public const string ENTITY_TRACKER_ORDER = "ORDER";
        public const string ENTITY_TRACKER_PACKING_SLIP = "PACKING_SLIP";
        public const string ENTITY_TRACKER_MASTER_PACKING_SLIP = "MASTER_PACKING_SLIP";

        public enum INVENTORY_TYPE
        {
            INTRANSIT_QTY = 1,
            QTY_IN_HAND = 2
        }

        public enum TRANSACTION_TYPE
        {
            UPLOAD_SUPPLIER_INVOICE = 1,
            RECEIVE_SUPPLIER_INVOICE = 2,
            CUSTOMER_PACKINGSLIP = 3,
            ADJUSTMENT_PLUS = 4,
            ADJUSTMENT_MINUS = 5,
            REVERT_UPLOAD_SUPPLIER_INVOICE = 6,
            REVERT_CUSTOMER_PACKINGSLIP = 7,
        }

        public enum DIRECTION
        {
            IN = 1,
            OUT = 2
        }
    }
}
