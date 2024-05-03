using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class SuppliesInventoryViewModel : ViewModelBase
    {

        public ObservableCollection<PetSupply> SupplyItems { get; set; }

        public RelayCommand AddCommand => new RelayCommand(parameter => AddItem(parameter));

        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => SelectedItem != null);
        //can only execute when selecteditem is not null
        public SuppliesInventoryViewModel()
        {

            SupplyItems = new ObservableCollection<PetSupply>();

        }

        private PetSupply selectedItem;
        public PetSupply SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }

        Random random = new Random();
        private void AddItem(object parameter)
        {
            var item = (SuppliesInventoryView)parameter;

            if ((item.NameTextBox.Text == null) ||
                    (item.QuantityTextBox.Text == null) ||
                    (item.PriceTextBox.Text == null) ||
                    (item.PetTypeComboBox.SelectedItem == null))
            {
                MessageBox.Show("Please Fill out all the forms");
            }
            else
            {
                SupplyItems.Add(new PetSupply
                {
                    //ItemID = random.Next().ToString(),
                    //Name = item.NameTextBox.Text,
                    //Quantity = int.Parse(item.QuantityTextBox.Text),
                    //Price = int.Parse(item.PriceTextBox.Text),
                    //PetType = item.PetTypeComboBox.Text,
                    //ImagePath = item.ImagePathTextBox.Text
                }); ;
            }

        }

        private void DeleteItem()
        {
            SupplyItems.Remove(selectedItem);
        }
    }
}
