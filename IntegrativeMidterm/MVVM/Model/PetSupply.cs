using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class PetSupply
    {
        // FOR INPUT & OUTPUT
        public string PetSupplyName { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }

        // FOR OUTPUT IN ORDER -------------------------
        public int PetSupplyID { get; set; }
        // - PetSupplyName
        // - Quantity
        // - Price
        public string Status { get; set; }
        public string SupplyType { get; set; }
        public string Species { get; set; }
        
        // FOR INPUT ----------------------------------
        public int InStatusID { get; set; }
        public int InSupplyTypeID { get; set; }
        public int InPetTypeID { get; set; }
    }
}
