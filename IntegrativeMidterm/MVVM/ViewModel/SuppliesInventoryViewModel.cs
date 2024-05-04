using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            InitializeData();

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
                    (item.ImageURLTextBox.Text == null))
            {
                MessageBox.Show("Please Fill out all the forms");
            }
            else
            {

				SupplyItems.Add(new PetSupply
				{
					PetSupplyName = item.NameTextBox.Text,
					Quantity = int.TryParse(item.QuantityTextBox.Text, out int quantity) ? quantity : 0,
					Price = float.TryParse(item.PriceTextBox.Text, out float price) ? price : 0.0f,
					PetSupplyID = random.Next(),//int.Parse(item.ItemIDTextBox.Text),
					Status = "Available",
					SupplyType = "Grooming",
					Species = item.PetTypeTextBox.Text.ToLower(),
					InStatusID = 1,
					InSupplyTypeID = 1,
					InPetTypeID = 1,
					ImagePath = "C:\\Users\\Brid G\\Source\\Repos\\IntegrativeMidterm\\IntegrativeMidterm\\Themes\\Images\\MyImage.jpg"

				});

                clearTextbox(item.TextBoxContainer);
                MessageBox.Show("Data Added Successfully");
			}

        }

        private void InitializeData()
        {
            ISingleResult<spGetAllPetSuppliesResult> retrievedData = PetshopDB.spGetAllPetSupplies(null, null, null);

            foreach (spGetAllPetSuppliesResult item in retrievedData)
            {
                SupplyItems.Add(new PetSupply
                {
                    PetSupplyName = item.Product_name,
                    Quantity = item.Quantity,
                    Price = (float)item.Price,
                    PetSupplyID = item.ID,
                    Status = item.Status,
                    SupplyType = item.Species,
                    Species = item.Species,
                    InStatusID = 1,
                    InSupplyTypeID = 1,
                    InPetTypeID = 1,
                    ImagePath = "C:\\Users\\Brid G\\Source\\Repos\\IntegrativeMidterm\\IntegrativeMidterm\\Themes\\Images\\MyImage.jpg"

                });
            }

		}
        private void clearTextbox(StackPanel container)
        {
			foreach (var child in container.Children)
				if (child is TextBox textBox)
                    textBox.Text = "";
		}

        private void DeleteItem()
        {
            SupplyItems.Remove(selectedItem);
			MessageBox.Show("Data Deleted Successfully");
		}
    }
}
