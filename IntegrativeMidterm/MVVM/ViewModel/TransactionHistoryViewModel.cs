using IntegrativeMidterm.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class TransactionHistoryViewModel
    {
        public ObservableCollection<Sales> Sales { get; set; }
        public TransactionHistoryViewModel()
        {
            Sales = new ObservableCollection<Sales>();
        }
    }
}
