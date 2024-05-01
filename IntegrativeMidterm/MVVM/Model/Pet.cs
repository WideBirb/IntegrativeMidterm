using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class Pet
    {
        // FOR INPUT & OUTPUT IN ORDER
        public string PetName { get; set; }
        public string Gender { get; set; }
        public string Birhdate { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public string Price { get; set; }
        // - Status

        // FOR OUTPUT
        public string Status { get; set; }

        // FOR INPUT ON ADDING TO TRANSACTIONS
        public int InTransactionHistoryID { get; set; }
    }
}
