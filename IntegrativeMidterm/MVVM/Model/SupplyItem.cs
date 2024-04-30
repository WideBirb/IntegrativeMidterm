using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class SupplyItem
    {
        public string ItemID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string PetType { get; set; }
        public string ImagePath { get; set; }
    }
}
