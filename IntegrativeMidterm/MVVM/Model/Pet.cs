using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class Pet
    {
        // FOR INPUT & OUTPUT IN ORDER
        public string PetName { get; set; }
        public string Gender { get; set; }
        public DateTime Birhdate { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public float Price { get; set; }
        // - Status

        // FOR OUTPUT
        public int Status { get; set; }
        public int Age { get; set; }
        public SolidColorBrush StatusColor { get; set; }

        // FOR INPUT ON ADDING TO TRANSACTIONS
        public int InTransactionHistoryID { get; set; }

        // OTHER OUTPUT
        public string ImagePath { get; set; }
    }
}
