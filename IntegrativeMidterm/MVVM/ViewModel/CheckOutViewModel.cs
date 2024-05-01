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

        public ObservableCollection<PetSupply> ShoppingCart { get; set; }
        public ObservableCollection<PetSupply> SupplyItems { get; set; }
        public CheckOutViewModel()
        {
            ShoppingCart = new ObservableCollection<PetSupply>();

            SupplyItems = new ObservableCollection<PetSupply>();

        }

    }
}
