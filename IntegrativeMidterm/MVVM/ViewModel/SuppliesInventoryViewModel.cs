using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using IntegrativeMidterm.userControl.General;
using IntegrativeMidterm.MVVM.Model.Filters;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class SuppliesInventoryViewModel : ViewModelBase
    {
		public ObservableCollection<PetSupply> SupplyItems { get; set; }
		public ObservableCollection<PetSpecies> PetSpeciesSelection { get; set; }
		public ObservableCollection<PetSupplyType> SupplyTypeSelection { get; set; }

        public RelayCommand CopyCommand => new RelayCommand(parameter => copySelectedItems(parameter), canExecute => SelectedItem != null);
		public RelayCommand ClearCommand => new RelayCommand(parameter => clearTextbox((StackPanel)parameter));
		public RelayCommand AddCommand => new RelayCommand(parameter => AddItem(parameter));
		public RelayCommand UpdateCommand => new RelayCommand(execute => UpdateItem(execute), canExecute => SelectedItem != null);
		public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => SelectedItem != null);
		public RelayCommand UploadImageCommand => new RelayCommand(parameter => UploadImage());
		//can only execute when selecteditem is not null

		private PetSpecies _chosenSpecies = new PetSpecies();
		public string ChosenSpecies
		{
			get
			{
				return _chosenSpecies.Description;
			}
			set
			{
				_chosenSpecies.Description = value;
				_chosenSpecies.ID = DeterminePetType(value);
				OnPropertyChanged();
			}
		}

        private PetSupplyType _chosenSupplyType = new PetSupplyType();
        public string ChosenSupplyType
        {
            get
            {
                return _chosenSupplyType.Description;
            }
            set
            {
                _chosenSupplyType.Description = value;
                _chosenSupplyType.ID = DetermineSupplyType(value);
                OnPropertyChanged();
            }
        }

        // SPTransactionCreate , staff = 1, custmer id = null
        // Kunin mo yung id ng ginawa mo
        public SuppliesInventoryViewModel()
        {
            SupplyItems = new ObservableCollection<PetSupply>();
			PetSpeciesSelection = new ObservableCollection<PetSpecies>();
			SupplyTypeSelection = new ObservableCollection<PetSupplyType>();

            var species = PetshopDB.spGetPetTypes();

			foreach ( var type in species )
			{
				PetSpeciesSelection.Add(new PetSpecies{
					ID = type.pet_type_id,
					Description = type.description
				});
			}

            var supplyTypes = PetshopDB.spGetPetSupplyTypes();

            foreach (var type in supplyTypes)
            {
                SupplyTypeSelection.Add(new PetSupplyType
                {
                    ID = type.pet_supply_type_id,
                    Description = type.description
                });
            }
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

		private string _profileImagePath = string.Empty;
		public string ProfileImagePath
		{
			get { return _profileImagePath; }
			set { _profileImagePath = value; OnPropertyChanged(); }
		}

		//Random random = new Random();

		public int DetermineSupplyType(string supplyType)
		{
            foreach (var type in SupplyTypeSelection)
            {
                if (type.Description == supplyType)
                {
                    return type.ID;
                }
            }
            return -1;
            //if (supplyType == "food")
            //	return 1;
            //else if (supplyType == "hygiene")
            //	return 2;
            //else if (supplyType == "toys")
            //	return 3;
            //else if (supplyType == "grooming")
            //	return 4;
            //else if (supplyType == "accessories")
            //	return 5;
            //else if (supplyType == "medication")
            //	return 6;
            //else if (supplyType == "decoration")
            //	return 7;
            //else if (supplyType == "feeding")
            //	return 8;
            //else
            //	return 9;
		}

		public int DeterminePetType(string petType)
		{
			foreach (var species in PetSpeciesSelection)
			{
				if (species.Description == petType)
				{
					return species.ID;
				}
			}
			return -1;
			//if (petType == "Dog")
			//	return 1;
			//else if (petType == "Cat")
			//	return 2;
			//else if (petType == "Bird")
			//	return 3;
			//else if (petType == "Fish")
			//	return 4;
			//else
			//	return 5;
		}

		private void InitializeData()
		{
			ISingleResult<spGetAllPetSuppliesResult> retrievedData = PetshopDB.spGetAllPetSupplies(null, null, null);

			foreach (spGetAllPetSuppliesResult item in retrievedData)
			{
				//STATUS ID = 2 IS ARCHIVED
				if (item.Status_ID == 2)
					continue;
				else
				{
					SupplyItems.Add(new PetSupply
					{
						PetSupplyName = item.Product_name,
						Quantity = item.Quantity,
						Price = (float)item.Price,
						PetSupplyID = item.ID,
						Status = item.Status,
						SupplyType = item.Supply_Type,
						Species = item.Species,
						InStatusID = item.Status_ID,
						InSupplyTypeID = item.ID,
						InPetTypeID = item.Species_ID,
						ImagePath = item.Image_path

					});
				}

			}

		}

		private void UploadImage()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif|All Files (*.*)|*.*";

			if (openFileDialog.ShowDialog() == true)
			{
				string selectedFilePath = openFileDialog.FileName;

				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(selectedFilePath);

				string destinationDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PetImages");
				Directory.CreateDirectory(destinationDirectory);

				string destinationFilePath = Path.Combine(destinationDirectory, fileName);
				File.Copy(selectedFilePath, destinationFilePath);

				ProfileImagePath = (new BitmapImage(new Uri(destinationFilePath))).ToString();
			}
		}

		private void copySelectedItems(object parameter)
		{ 
			SuppliesInventoryView view = (SuppliesInventoryView)parameter;		
			view.NameTextBox.InputText = selectedItem.PetSupplyName;
			view.QuantityTextBox.InputText = selectedItem.Quantity.ToString();
			view.PriceTextBox.InputText = selectedItem.Price.ToString();
			view.PetTypeComboBox.InputText = selectedItem.Species.ToString();
			view.SupplyTypeComboBox.InputText = selectedItem.SupplyType.ToString();
			ProfileImagePath = selectedItem.ImagePath;
			//foreach (var item in view.PetTypeComboBox.Items)
			//	if ((item as ComboBoxItem).Content.ToString() == selectedItem.Species.ToString())
			//	{
			//		view.PetTypeComboBox.SelectedItem = item;
			//		break; // Exit the loop once the item is found
			//	}

			//view.SupplyTypeTextBox.Text = selectedItem.SupplyType;
			//view.ImageURLTextBox.Text = selectedItem.ImagePath;
		}

		private void clearTextbox(StackPanel container)
		{
			foreach (var child in container.Children)
			{
				if (child is InputBar textBox)
					textBox.InputText = "";
				else if (child is DropdownSelection combo)
				{ 
					combo.InputText = string.Empty;
				}
			}
		}

		private void AddItem(object parameter)
        {
            var item = (SuppliesInventoryView)parameter;

			if (item.NameTextBox.InputText == string.Empty ||
				item.QuantityTextBox.InputText == string.Empty ||
				item.PriceTextBox.InputText == string.Empty ||
				ChosenSupplyType == string.Empty ||
				ChosenSpecies == string.Empty)
				//item.SupplyTypeTextBox.Text == string.Empty ||
				//item.ImageURLTextBox.Text == string.Empty)
			{

				new AlertBox("Please fill out all the forms!", 18).ShowDialog();
			}
            else
            {
				if (_chosenSupplyType.ID == -1 || _chosenSpecies.ID == -1)
				{
                    new AlertBox("Invalid pet supply or species selection!", 18).ShowDialog();
                    ProfileImagePath = string.Empty;
                    return;
                }

                //Add to DB
                //ComboBoxItem selectedComboBox = new ComboBoxItem();
                //foreach (ComboBoxItem items in item.PetTypeComboBox.Items)
                //	if ((items as ComboBoxItem) == item.PetTypeComboBox.SelectedItem)
                //	{
                //		selectedComboBox = items;
                //		break; // Exit the loop once the item is found
                //	}

                PetshopDB.spAddPetSupply
					(
						item.NameTextBox.InputText,
						_chosenSupplyType.ID,
						_chosenSpecies.ID,
						float.TryParse(item.PriceTextBox.InputText, out float price2) ? price2 : 999.99f,
						int.TryParse(item.QuantityTextBox.InputText, out int quantity2) ? quantity2 : 0,
						ProfileImagePath //item.ImageURLTextBox.Text
					);
				int maxID = PetshopDB.vwPetSupplies.Max(t => t.ID);

				// ADD TO MEMORY
				SupplyItems.Add(new PetSupply
				{
					PetSupplyName = item.NameTextBox.InputText,
					Quantity = int.TryParse(item.QuantityTextBox.InputText, out int quantity) ? quantity : 0,
					Price = float.TryParse(item.PriceTextBox.InputText, out float price) ? price : 0.0f,
					PetSupplyID = maxID,//int.Parse(item.ItemIDTextBox.Text),
					SupplyType = _chosenSupplyType.Description, /*item.SupplyTypeTextBox.Text.ToLower(),*/
					Species = _chosenSpecies.Description,
					InSupplyTypeID = _chosenSupplyType.ID, /*determineSupplyType(item.SupplyTypeTextBox.Text.ToLower()),*/
					InPetTypeID = +_chosenSpecies.ID,
					ImagePath = ProfileImagePath, //item.ImageURLTextBox.Text,

					Status = "Available",
					InStatusID = 2

				});

				new AlertBox("Data Added Successfully!", 20).ShowDialog();
				clearTextbox(item.TextBoxContainer);

			}
			ProfileImagePath = string.Empty;
		}

        private void DeleteItem()

        {
			//STATUS ID = 2 IS ARCHIVED
			PetshopDB.spSetPetSupplyStatus(selectedItem.PetSupplyID, 2);
			SupplyItems.Remove(selectedItem);
			new AlertBox("Data Deleted Successfully!", 20).ShowDialog();
		}

		private void UpdateItem(object parameter)
		{
			var item = (SuppliesInventoryView)parameter;

			//ComboBoxItem selectedComboBox = new ComboBoxItem();
			//foreach (ComboBoxItem items in item.PetTypeComboBox.Items)
			//	if ((items as ComboBoxItem) == item.PetTypeComboBox.SelectedItem)
			//	{
			//		selectedComboBox = items;
			//		break; // Exit the loop once the item is found
			//	}


			if (item.NameTextBox.InputText == string.Empty ||
				item.QuantityTextBox.InputText == string.Empty ||
				item.PriceTextBox.InputText == string.Empty ||
                ChosenSpecies == string.Empty ||
				ChosenSupplyType == string.Empty)
				//item.SupplyTypeTextBox.Text == string.Empty ||
				//item.ImageURLTextBox.Text == string.Empty)
			{
				new AlertBox("Please fill out all the forms!", 18).ShowDialog();
			}
			else
			{
                if (_chosenSupplyType.ID == -1 || _chosenSpecies.ID == -1)
                {
                    new AlertBox("Invalid pet supply or species selection!", 18).ShowDialog();
                    ProfileImagePath = string.Empty;
                    return;
                }

                selectedItem.PetSupplyName = item.NameTextBox.InputText;
				selectedItem.Quantity = int.Parse(item.QuantityTextBox.InputText);
				selectedItem.Price = float.Parse(item.PriceTextBox.InputText);
				selectedItem.Species = _chosenSpecies.Description;
				selectedItem.SupplyType = _chosenSupplyType.Description;
                selectedItem.ImagePath = ProfileImagePath;
				//selectedItem.SupplyType = item.SupplyTypeTextBox.Text;
				//selectedItem.ImagePath = item.ImageURLTextBox.Text;

				PetshopDB.spUpdateSupplyData
					(
						selectedItem.PetSupplyID,
						selectedItem.PetSupplyName,
						_chosenSupplyType.ID,
                        _chosenSpecies.ID, //determinePetType(selectedItem.Species),
						selectedItem.Price,
						selectedItem.Quantity,
						selectedItem.ImagePath
					);

				item.myDataGrid.ItemsSource = null;
				item.myDataGrid.ItemsSource = SupplyItems;
				new AlertBox("Data Updated Successfully!", 20).ShowDialog();
				clearTextbox(item.TextBoxContainer);
			}
			ProfileImagePath = string.Empty;	
		}
	}
}
