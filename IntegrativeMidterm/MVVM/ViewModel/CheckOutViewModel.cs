using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    class CheckOutViewModel : ViewModelBase
    {

        public ObservableCollection<SupplyItem> ShoppingCart { get; set; }
        public ObservableCollection<SupplyItem> SupplyItems { get; set; }
        public CheckOutViewModel()
        {
            ShoppingCart = new ObservableCollection<SupplyItem>();

            SupplyItems = new ObservableCollection<SupplyItem>();

        }

    }
}
