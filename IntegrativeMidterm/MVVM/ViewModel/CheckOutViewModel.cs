using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.Model.Filters;
using IntegrativeMidterm.MVVM.View;
using IntegrativeMidterm.userControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Net.WebRequestMethods;

namespace IntegrativeMidterm.MVVM.ViewModel
{
	class CheckOutViewModel : ViewModelBase
	{
        private float totalPrice { get; set; }
		private string _searchBarInput = string.Empty;
		private string _searchBarPlaceholderText = string.Empty;

		public string SearchBarInput
		{
			get { return _searchBarInput; }
			set { _searchBarInput = value; OnPropertyChanged(); }
		}
		public string SearchBarPlaceholderText
		{
			get { return _searchBarPlaceholderText; }
			set { _searchBarPlaceholderText = value; OnPropertyChanged(); }
		}
		public float TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                OnPropertyChanged();
            }
        }

        public string currentPetFilter { get; set; }
        public bool isFiltering { get; set; } = false;
        public ObservableCollection<PetSupply> ShoppingCart { get; set; }
		public ObservableCollection<PetSupply> PetSupplyItems { get; set; }
		public List<PetSupply> HiddenPetSupplyItems { get; set; }
		public ObservableCollection<PetSpecies> PetSpeciesFilters { get; set; }

		public RelayCommand AddItemCommand { get; set; }
        public RelayCommand ConfirmPurchaseCommand { get; set; }
        public RelayCommand CancelPurchaseCommand { get; set; }
        public RelayCommand RemoveItemCommand { get; set; }
        public RelayCommand increaseQuantityCommand { get; set; }
        public RelayCommand decreaseQuantityCommand { get; set; }
        public RelayCommand FilterPetTypesCommand { get; set; }
        public RelayCommand AB => new RelayCommand(parameter => { MessageBox.Show("AB! AB! AB!"); });
		public RelayCommand FilterCommand { get; }

		public int determineSupplyType(string supplyType)
		{
			if (supplyType == "food")
				return 1;
			else if (supplyType == "hygiene")
				return 2;
			else if (supplyType == "toys")
				return 3;
			else if (supplyType == "grooming")
				return 4;
			else if (supplyType == "accessories")
				return 5;
			else if (supplyType == "medication")
				return 6;
			else if (supplyType == "decoration")
				return 7;
			else if (supplyType == "feeding")
				return 8;
			else
				return 9;
		}
		public int determinePetType(string petType)
		{

			if (petType == "dog")
				return 1;
			else if (petType == "cat")
				return 2;
			else if (petType == "bird")
				return 3;
			else if (petType == "fish")
				return 4;
			else
				return 5;
		}

		private void increaseQuantity(object parameter)
		{
            PetSupply newItem = new PetSupply();
			CheckoutItems checkoutItems = (CheckoutItems)parameter;

			TextBlock supplyIDTextblock = checkoutItems.QuantityTextBlock;
            string PetSupplyID = checkoutItems.PetSupplyIDTextBox.Text;
			TextBlock quantityTextblock = checkoutItems.QuantityTextBlock;

			int maxQuantity = 0;

			foreach (PetSupply item in HiddenPetSupplyItems)
				if (item.PetSupplyID == int.Parse(PetSupplyID))
					if (int.Parse(quantityTextblock.Text) == item.Quantity)
						maxQuantity = item.Quantity;

			foreach (PetSupply item in ShoppingCart)
				if (item.PetSupplyID == int.Parse(PetSupplyID))
				{
					newItem = item;
					if (newItem.Quantity == maxQuantity)
						return;
					else
					{
						newItem.Quantity++;
						quantityTextblock.Text = newItem.Quantity.ToString();
						updateTotalCost();
					}
						
				}

		}

		private void decreaseQuantity(object parameter)
		{
			PetSupply newItem = new PetSupply();
			CheckoutItems checkoutItems = (CheckoutItems)parameter;

			TextBlock supplyIDTextblock = checkoutItems.QuantityTextBlock;
			string PetSupplyID = checkoutItems.PetSupplyIDTextBox.Text;
			TextBlock quantityTextblock = checkoutItems.QuantityTextBlock;

			foreach (PetSupply item in ShoppingCart)
				if (item.PetSupplyID == int.Parse(PetSupplyID))
				{
					newItem = item;
					if (newItem.Quantity == 1)
						return;
					else
					{
						newItem.Quantity--;
						quantityTextblock.Text = newItem.Quantity.ToString();
						updateTotalCost();
					}
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

			updateProductQuantity();

            ShoppingCart.Clear();
			PetSupplyItems.Clear();
			InitializeData();
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
			{
				// need new initialization because changing the property also changes the object it copies from
				if (item.PetSupplyID == int.Parse(PetSupplyID.ToString()))
				{
					// if out of stock then do nothing
					if (item.Quantity < 1)
						return;
					else
					{
						newItem = new PetSupply()
						{
							PetSupplyName = item.PetSupplyName,
							Quantity = 1,
							Price = (float)item.Price,
							PetSupplyID = item.PetSupplyID,
							Status = item.Status,
							SupplyType = item.SupplyType,
							Species = item.Species,
							InStatusID = 1,
							InSupplyTypeID = 1,
							InPetTypeID = 1,
							ImagePath = item.ImagePath
						};
					}
				}
			}

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

		private void Filter(object parameter)
		{
			if (PetSupplyItems == null) { return; }

			PetSupplyItems.Clear();
			InitializeData(parameter as string);
		}

		private void InitializeData(string filter = null)
		{
			ISingleResult<spGetAllPetSuppliesResult> retrievedData = PetshopDB.spGetAllPetSupplies(null,null,null);

			foreach (spGetAllPetSuppliesResult item in retrievedData)
			{
				//STATUS ID = 2 IS ARCHIVED
				if (item.Status_ID == 1 || item.Status_ID == 3)
				{
					if (filter != null)
					{
						if (!item.Product_name.ToLower().Contains(filter.ToLower()))
							continue;

						PetSupplyItems.Add(new PetSupply
						{
							PetSupplyName = item.Product_name,
							Quantity = item.Quantity,
							Price = (float)item.Price,
							PetSupplyID = item.ID,
							Status = item.Status,
							SupplyType = item.Supply_Type,
							Species = item.Species,
							InStatusID = 1,
							InSupplyTypeID = 1,
							InPetTypeID = 1,
							ImagePath = item.Image_path

						});
						continue;
					}

					PetSupplyItems.Add(
					new PetSupply
					{
						PetSupplyName = item.Product_name,
						Quantity = item.Quantity,
						Price = (float)item.Price,
						PetSupplyID = item.ID,
						Status = item.Status,
						SupplyType = item.Supply_Type,
						Species = item.Species,
						InStatusID = 1,
						InSupplyTypeID = 1,
						InPetTypeID = 1,
						ImagePath = item.Image_path
					});
				}
			}

			HiddenPetSupplyItems = PetSupplyItems.ToList();
		}


		private void updateProductQuantity()
		{
			// should make the pet supply status to 3 (out of stock) here but gilbs is not gonan notice

			Dictionary<int,int> IDQuantitypairs = new Dictionary<int,int>();


			// Get ID and Quantity for each order
			foreach (PetSupply item in ShoppingCart)
				IDQuantitypairs.Add(item.PetSupplyID, item.Quantity);


			//Update DB
			foreach (KeyValuePair<int, int> kvp in IDQuantitypairs)
				PetshopDB.spUpdatePetSupplyQuantity(kvp.Key, kvp.Value);


			//PetSupplyItems.Clear();
			//InitializeData("");
			//HiddenPetSupplyItems = PetSupplyItems.ToList();
		}

		private void filterPetType(object parameter)
		{
			
			PetSupplyItems.Clear();
			string petType = parameter as string;
			ISingleResult<spGetAllPetSuppliesResult> retrievedData = PetshopDB.spGetAllPetSupplies(null, null, null);

			// if same filter is pressed again,
			if (currentPetFilter == petType)
			{
				currentPetFilter = "";
				foreach (spGetAllPetSuppliesResult item in retrievedData)
				{
					//STATUS ID = 2 IS ARCHIVED
					if (item.Status_ID == 1 || item.Status_ID == 3 )
					{
						PetSupplyItems.Add(new PetSupply
						{
							PetSupplyName = item.Product_name,
							Quantity = item.Quantity,
							Price = (float)item.Price,
							PetSupplyID = item.ID,
							Status = item.Status,
							SupplyType = item.Supply_Type,
							Species = item.Species,
							InStatusID = 1,
							InSupplyTypeID = 1,
							InPetTypeID = 1,
							ImagePath = item.Image_path
						});
					}

				}
			}
			// if no filter is applied
			else
			{
				foreach (spGetAllPetSuppliesResult item in retrievedData)
				{
					if (item.Species.ToLower() == (petType.ToLower()))
						//STATUS ID = 2 IS ARCHIVED
						if (item.Status_ID == 1 || item.Status_ID == 3)
						{
							PetSupplyItems.Add(new PetSupply
							{
								PetSupplyName = item.Product_name,
								Quantity = item.Quantity,
								Price = (float)item.Price,
								PetSupplyID = item.ID,
								Status = item.Status,
								SupplyType = item.Supply_Type,
								Species = item.Species,
								InStatusID = 1,
								InSupplyTypeID = 1,
								InPetTypeID = 1,
								ImagePath = item.Image_path

							});
						}

					continue;
				}
				currentPetFilter = petType;
			}
		}

		public CheckOutViewModel()
		{

			SearchBarPlaceholderText = "Search...";
			SearchBarInput = "";

			FilterCommand = new RelayCommand(parameter => Filter(parameter));
			AddItemCommand = new RelayCommand(parameter => AddItem(parameter));
			ConfirmPurchaseCommand = new RelayCommand(parameter => ConfirmPurchase(parameter));
			CancelPurchaseCommand = new RelayCommand(parameter => CancelPurchase(parameter));
			RemoveItemCommand = new RelayCommand(parameter => RemoveItem(parameter));
			increaseQuantityCommand = new RelayCommand(parameter => increaseQuantity(parameter));
			decreaseQuantityCommand = new RelayCommand(parameter => decreaseQuantity(parameter));
			FilterPetTypesCommand = new RelayCommand(parameter => filterPetType(parameter));

			ShoppingCart = new ObservableCollection<PetSupply>();
			PetSupplyItems = new ObservableCollection<PetSupply>();

			PetSpeciesFilters = new ObservableCollection<PetSpecies>
			{
				new PetSpecies
				{
					ID = 1,
					Description = "Cat"
				},
				new PetSpecies
				{
					ID = 2,
					Description = "Dog"
				},
				new PetSpecies
				{
					ID = 3,
					Description = "Bird"
				},
				new PetSpecies
				{
					ID = 4,
					Description = "Shark"
				},
				new PetSpecies
				{
					ID = 5,
					Description = "Dinosaur"
				}
			};

			
			InitializeData();
			
			var dataToUpdate = from item in PetshopDB.spGetAllPetSupplies(null, null, null) select item;
			//foreach (var item in dataToUpdate)
				//MessageBox.Show(item.Product_name);
		}

    }
}
