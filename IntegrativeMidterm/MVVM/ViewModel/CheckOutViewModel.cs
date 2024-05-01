using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrativeMidterm.MVVM.ViewModel
{
	class CheckOutViewModel : ViewModelBase
	{
        public int TotalPrice { get; set; }
        public ObservableCollection<PetSupply> ShoppingCart { get; set; }
		public ObservableCollection<PetSupply> PetSupplyItems { get; set; }

		public RelayCommand AddItemCommand => new RelayCommand(parameter => AddItem(parameter));

		public RelayCommand ConfirmPurchaseCommand => new RelayCommand(parameter => ConfirmPurchase(parameter));

		public RelayCommand CancelPurchaseCommand => new RelayCommand(parameter => CancelPurchase(parameter));

		public RelayCommand RemoveItemCommand => new RelayCommand(parameter => RemoveItem(parameter));

		public RelayCommand increaseQuantityCommand => new RelayCommand(parameter => increaseQuantity(parameter));
		public RelayCommand decreaseQuantityCommand => new RelayCommand(parameter => decreaseQuantity(parameter));


		public RelayCommand AB => new RelayCommand(parameter => { MessageBox.Show("AB! AB! AB!"); });

		private void increaseQuantity(object parameter)
		{ 
		
		}

		private void decreaseQuantity(object parameter)
		{

		}
		private void updateTotalCost()
		{

		}
		private void ConfirmPurchase(object parameter)
		{ 

		}

		private void CancelPurchase(object parameter)
		{

		}

		private void AddItem(object parameter)
		{
			PetSupply newItem = new PetSupply();
			string PetSupplyID = parameter.ToString();

			//check if item is already in cart
			bool alreadyExists = ShoppingCart.Any(x => x.PetSupplyID.ToString() == PetSupplyID);
			if (alreadyExists)
			{
				MessageBox.Show("Item is already in the cart");
				return;
			}

			// Find the PetSupply object from the Petsupplyitems and put it in the shopping cart
			foreach (PetSupply item in PetSupplyItems)
				if (item.PetSupplyID == int.Parse(PetSupplyID.ToString()))
					newItem = item;

			// Change the Quantity to default
			newItem.Quantity = 1;
			ShoppingCart.Add(newItem);

		}
		
		private void RemoveItem(object parameter)
		{

			PetSupply itemToRemove = new PetSupply();
			string PetSupplyID = parameter.ToString();

			//Find Item in Cart
			foreach (PetSupply item in ShoppingCart)
				if (item.PetSupplyID == int.Parse(PetSupplyID.ToString()))
					itemToRemove = item;

			// Remove Item in Cart
			ShoppingCart.Remove(itemToRemove);
		}
        public CheckOutViewModel()
        {
            ShoppingCart = new ObservableCollection<PetSupply>();

			PetSupplyItems = new ObservableCollection<PetSupply>();

            PetSupplyItems.Add(new PetSupply
            { 
                PetSupplyName = "Soap",
                Quantity = 50,
                Price = 199.99f,
                PetSupplyID = 16598230,
                Status = "Available",
                SupplyType = "Food",
                Species = "Cat",
                InStatusID = 1,
                InSupplyTypeID = 1,
                InPetTypeID = 1,
                ImagePath = "C:\\Users\\Brid G\\Source\\Repos\\IntegrativeMidterm\\IntegrativeMidterm\\Themes\\Images\\MyImage.jpg"

			});

			PetSupplyItems.Add(new PetSupply
			{
				PetSupplyName = "Bleach",
				Quantity = 50,
				Price = 299.99f,
				PetSupplyID = 53498230,
				Status = "Available",
				SupplyType = "Food",
				Species = "Cat",
				InStatusID = 1,
				InSupplyTypeID = 1,
				InPetTypeID = 1,
				ImagePath = "C:\\Users\\Brid G\\Source\\Repos\\IntegrativeMidterm\\IntegrativeMidterm\\Themes\\Images\\MyImage.jpg"

			});

			PetSupplyItems.Add(new PetSupply
			{
				PetSupplyName = "Leash",
				Quantity = 50,
				Price = 399.99f,
				PetSupplyID = 36598230,
				Status = "Available",
				SupplyType = "Food",
				Species = "Cat",
				InStatusID = 1,
				InSupplyTypeID = 1,
				InPetTypeID = 1,
				ImagePath = "C:\\Users\\Brid G\\Source\\Repos\\IntegrativeMidterm\\IntegrativeMidterm\\Themes\\Images\\MyImage.jpg"

			});

		}

    }
}
