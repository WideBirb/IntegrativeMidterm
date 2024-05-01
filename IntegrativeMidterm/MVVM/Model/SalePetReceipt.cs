using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class SalePetReceipt
    {
        // FOR INPUT & OUTPUT --------------------------
        public int InTransactionHistoryID { get; set; }

        // FOR OUTPUT IN ORDER -------------------------
        // - TransactionHistoryID
        public string PetName { get; set; }
        public string CustomerName { get; set; }
        public int TotalCost { get; set; }
        public DateTime ProcessDate { get; set; }
        public string StaffName { get; set; }
        
        // FOR INPUT ----------------------------------
        public int InPetID { get; set; }
    }
}
