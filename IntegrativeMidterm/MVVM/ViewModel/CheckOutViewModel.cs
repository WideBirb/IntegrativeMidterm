using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.View;
using IntegrativeMidterm.userControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IntegrativeMidterm.MVVM.ViewModel
{
	class CheckOutViewModel : ViewModelBase
	{
        private float totalPrice { get; set; }

        public float TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PetSupply> ShoppingCart { get; set; }
		public ObservableCollection<PetSupply> PetSupplyItems { get; set; }

		public RelayCommand AddItemCommand { get; set; }
        public RelayCommand ConfirmPurchaseCommand { get; set; }
        public RelayCommand CancelPurchaseCommand { get; set; }
        public RelayCommand RemoveItemCommand { get; set; }
        public RelayCommand increaseQuantityCommand { get; set; }
        public RelayCommand decreaseQuantityCommand { get; set; }
        public RelayCommand filterItemsCommand { get; set; }


        public RelayCommand AB => new RelayCommand(parameter => { MessageBox.Show("AB! AB! AB!"); });

		private void increaseQuantity(object parameter)
		{
            PetSupply newItem = new PetSupply();
			CheckoutItems checkoutItems = (CheckoutItems)parameter;

			TextBlock supplyIDTextblock = checkoutItems.QuantityTextBlock;
            string PetSupplyID = checkoutItems.PetSupplyIDTextBox.Text;
			TextBlock quantityTextblock = checkoutItems.QuantityTextBlock;

            foreach (PetSupply item in PetSupplyItems)
                if (item.PetSupplyID == int.Parse(PetSupplyID))
                    newItem = item;

			newItem.Quantity++;
			quantityTextblock.Text = newItem.Quantity.ToString();
			updateTotalCost();
		}

		private void decreaseQuantity(object parameter)
		{
			PetSupply newItem = new PetSupply();
			CheckoutItems checkoutItems = (CheckoutItems)parameter;

			TextBlock supplyIDTextblock = checkoutItems.QuantityTextBlock;
			string PetSupplyID = checkoutItems.PetSupplyIDTextBox.Text;
			TextBlock quantityTextblock = checkoutItems.QuantityTextBlock;

			foreach (PetSupply item in PetSupplyItems)
				if (item.PetSupplyID == int.Parse(PetSupplyID))
					newItem = item;

			if (newItem.Quantity < 2)
				return;
			else
			{
                newItem.Quantity--;
                quantityTextblock.Text = newItem.Quantity.ToString();
                updateTotalCost();
            }

        }

		private void updateTotalCost()
		{
			TotalPrice = 0;
			foreach (PetSupply item in ShoppingCart)
                TotalPrice += item.Price * item.Quantity;
		}
		private void ConfirmPurchase(object parameter)
		{
            ShoppingCart.Clear();
            updateTotalCost();
        }

		private void CancelPurchase(object parameter)
		{

			ShoppingCart.Clear();
            updateTotalCost();
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
			updateTotalCost();

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
			updateTotalCost();
		}


        public CheckOutViewModel()
        {

			AddItemCommand = new RelayCommand(parameter => AddItem(parameter));
			ConfirmPurchaseCommand = new RelayCommand(parameter => ConfirmPurchase(parameter));
			CancelPurchaseCommand = new RelayCommand(parameter => CancelPurchase(parameter));
			RemoveItemCommand = new RelayCommand(parameter => RemoveItem(parameter));
			increaseQuantityCommand = new RelayCommand(parameter => increaseQuantity(parameter));
			decreaseQuantityCommand = new RelayCommand(parameter => decreaseQuantity(parameter));

			string baseDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString();

			ShoppingCart = new ObservableCollection<PetSupply>();
			PetSupplyItems = new ObservableCollection<PetSupply>();

			//MessageBox.Show(Path.Combine(baseDirectory, "Themes", "Images", "MyImage.jpg"));

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
				ImagePath = Path.Combine(baseDirectory, "Themes", "Images", "MyImage.jpg")

			});

			PetSupplyItems.Add(new PetSupply
			{
				PetSupplyName = "Bleach",
				Quantity = 50,
				Price = 299.99f,
				PetSupplyID = 53498230,
				Status = "Available",
				SupplyType = "Food",
				Species = "Dog",
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
				Species = "Bird",
				InStatusID = 1,
				InSupplyTypeID = 1,
				InPetTypeID = 1,
				ImagePath = "C:\\Users\\Brid G\\Source\\Repos\\IntegrativeMidterm\\IntegrativeMidterm\\Themes\\Images\\MyImage.jpg"

			});

		}

    }
}
