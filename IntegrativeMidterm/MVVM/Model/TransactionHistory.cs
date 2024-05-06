using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class TransactionHistory
    {
        public int ID { get; set; }
        public double TotalCost { get; set; }
        public DateTime ProcessDate { get; set; }
        public int Quantity { get; set; }
        public string Staff { get; set; }
        public int TransactionTypeID { get; set; }
        public SolidColorBrush TransactionTypeColor { get; set; }
    }
}
