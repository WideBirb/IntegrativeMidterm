using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class SaleSupplyReceipt
    {
        // FOR INPUT & OUTPUT --------------------------
        public int PurchaseQuantity { get; set; }

        // FOR OUTPUT IN ORDER -------------------------
        public string SupplyName { get; set; }
        public string SupplyType { get; set; }
        public string Species { get; set; }
        public float PricePerItem { get; set; }
        // - PurchaseQuantity
        public float TotalCost { get; set; }
        
        // FOR INPUT ----------------------------------
        public int InTransactionHistoryID { get; set; }
        public int InSupplyID { get; set; }
    }
}
