using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model
{
    // TransactionHistory for DB
    internal class SaleOverview
    {
        // FOR INPUT & OUTPUT
        public DateTime ProcessDate { get; set; }
        public int TotalCost { get; set; }
        public int TotalQuantity { get; set; }

        // FOR OUTPUT IN ORDER -------------------------
        public int TransactionHistoryID { get; set; }
        // - TotalCost
        // - TotalQuantity
        // - TransactionDate
        public string TransactionType { get; set; }
        public string StaffName { get; set; }

        // FOR INPUT ----------------------------------
        public int InCustomerID { get; set; }
        public int InStaffID { get; set; }
        public int InTransactionTypeID { get; set; }
    }
}
