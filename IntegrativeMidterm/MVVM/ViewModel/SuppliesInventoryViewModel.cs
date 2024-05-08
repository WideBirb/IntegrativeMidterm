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
		public ObservableCollection<PetSpecies> PetSpecies { get; set; }

		public RelayCommand CopyCommand => new RelayCommand(parameter => copySelectedItems(parameter), canExecute => SelectedItem != null);
		public RelayCommand ClearCommand => new RelayCommand(parameter => clearTextbox((StackPanel)parameter));
		public RelayCommand AddCommand => new RelayCommand(parameter => AddItem(parameter));
		public RelayCommand UpdateCommand => new RelayCommand(execute => UpdateItem(execute), canExecute => SelectedItem != null);
		public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => SelectedItem != null);
		public RelayCommand UploadImageCommand => new RelayCommand(parameter => UploadImage());
		//can only execute when selecteditem is not null


		// SPTransactionCreate , staff = 1, custmer id = null
		// Kunin mo yung id ng ginawa mo
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
		private string _profileImagePath = string.Empty;
		public string ProfileImagePath
		{
			get { return _profileImagePath; }
			set { _profileImagePath = value; OnPropertyChanged(); }
		}

		Random random = new Random();

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

			if (petType == "Dog")
				return 1;
			else if (petType == "Cat")
				return 2;
			else if (petType == "Bird")
				return 3;
			else if (petType == "Fish")
				return 4;
			else
				return 5;
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
			view.NameTextBox.Text = selectedItem.PetSupplyName;
			view.QuantityTextBox.Text = selectedItem.Quantity.ToString();
			view.PriceTextBox.Text = selectedItem.Price.ToString();
			foreach (var item in view.PetTypeComboBox.Items)
				if ((item as ComboBoxItem).Content.ToString() == selectedItem.Species.ToString())
				{
					view.PetTypeComboBox.SelectedItem = item;
					break; // Exit the loop once the item is found
				}

			//view.SupplyTypeTextBox.Text = selectedItem.SupplyType;
			//view.ImageURLTextBox.Text = selectedItem.ImagePath;
		}

		private void clearTextbox(StackPanel container)
		{
			foreach (var child in container.Children)
			{
				if (child is TextBox textBox)
					textBox.Text = "";
				else if (child is ComboBox combo)
				{ 
					combo.SelectedItem = null;
				}
			}


		}

		private void AddItem(object parameter)
        {
            var item = (SuppliesInventoryView)parameter;

			if (item.NameTextBox.Text == string.Empty ||
				item.QuantityTextBox.Text == string.Empty ||
				item.PriceTextBox.Text == string.Empty ||
				item.PetTypeComboBox.SelectedItem == null)
				//item.SupplyTypeTextBox.Text == string.Empty ||
				//item.ImageURLTextBox.Text == string.Empty)
			{

				new AlertBox("Please fill out all the forms!", 18).ShowDialog();
			}
            else
            {
				//Add to DB
				ComboBoxItem selectedComboBox = new ComboBoxItem();
				foreach (ComboBoxItem items in item.PetTypeComboBox.Items)
					if ((items as ComboBoxItem) == item.PetTypeComboBox.SelectedItem)
					{
						selectedComboBox = items;
						break; // Exit the loop once the item is found
					}

				PetshopDB.spAddPetSupply
					(
						item.NameTextBox.Text,
						1,//determineSupplyType(item.SupplyTypeTextBox.Text),
						determinePetType(selectedComboBox.Content.ToString()),
						float.TryParse(item.PriceTextBox.Text, out float price2) ? price2 : 999.99f,
						int.TryParse(item.QuantityTextBox.Text, out int quantity2) ? quantity2 : 0,
						ProfileImagePath //item.ImageURLTextBox.Text
					);


				int maxID = PetshopDB.vwPetSupplies.Max(t => t.ID);

				// ADD TO MEMORY
				SupplyItems.Add(new PetSupply
				{
					PetSupplyName = item.NameTextBox.Text,
					Quantity = int.TryParse(item.QuantityTextBox.Text, out int quantity) ? quantity : 0,
					Price = float.TryParse(item.PriceTextBox.Text, out float price) ? price : 0.0f,
					PetSupplyID = maxID,//int.Parse(item.ItemIDTextBox.Text),
					SupplyType = "1", /*item.SupplyTypeTextBox.Text.ToLower(),*/
					Species = selectedComboBox.Content.ToString().ToLower(),
					InSupplyTypeID = 1, /*determineSupplyType(item.SupplyTypeTextBox.Text.ToLower()),*/
					InPetTypeID = determinePetType(item.PetTypeComboBox.SelectedItem.ToString().ToLower()),
					ImagePath = ProfileImagePath, //item.ImageURLTextBox.Text,

					Status = "Available",
					InStatusID = 2

				});

				new AlertBox("Data Added Successfully!", 20).ShowDialog();
				clearTextbox(item.TextBoxContainer);

			}
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

			ComboBoxItem selectedComboBox = new ComboBoxItem();
			foreach (ComboBoxItem items in item.PetTypeComboBox.Items)
				if ((items as ComboBoxItem) == item.PetTypeComboBox.SelectedItem)
				{
					selectedComboBox = items;
					break; // Exit the loop once the item is found
				}


			if (item.NameTextBox.Text == string.Empty ||
				item.QuantityTextBox.Text == string.Empty ||
				item.PriceTextBox.Text == string.Empty ||
				item.PetTypeComboBox.SelectedItem.ToString() == string.Empty)
				//item.SupplyTypeTextBox.Text == string.Empty ||
				//item.ImageURLTextBox.Text == string.Empty)
			{

				new AlertBox("Please fill out all the forms!", 18).ShowDialog();
			}
			else
			{
				selectedItem.PetSupplyName = item.NameTextBox.Text;
				selectedItem.Quantity = int.Parse(item.QuantityTextBox.Text);
				selectedItem.Price = float.Parse(item.PriceTextBox.Text);
				selectedItem.Species = selectedComboBox.Content.ToString();
				selectedItem.ImagePath = ProfileImagePath;
				//selectedItem.SupplyType = item.SupplyTypeTextBox.Text;
				//selectedItem.ImagePath = item.ImageURLTextBox.Text;

				PetshopDB.spUpdateSupplyData
					(
						selectedItem.PetSupplyID,
						selectedItem.PetSupplyName,
						determineSupplyType(selectedItem.SupplyType),
						determinePetType(selectedComboBox.Content.ToString().ToLower()), //determinePetType(selectedItem.Species),
						selectedItem.Price,
						selectedItem.Quantity,
						selectedItem.ImagePath
					);

				item.myDataGrid.ItemsSource = null;
				item.myDataGrid.ItemsSource = SupplyItems;
				new AlertBox("Data Updated Successfully!", 20).ShowDialog();
				clearTextbox(item.TextBoxContainer);
			}

		}
	}
}
