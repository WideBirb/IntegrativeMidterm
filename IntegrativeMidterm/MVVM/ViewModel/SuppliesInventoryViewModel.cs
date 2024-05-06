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

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class SuppliesInventoryViewModel : ViewModelBase
    {
        
		public ObservableCollection<PetSupply> SupplyItems { get; set; }

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
			view.PetTypeTextBox.Text = selectedItem.Species;
			//view.SupplyTypeTextBox.Text = selectedItem.SupplyType;
			//view.ImageURLTextBox.Text = selectedItem.ImagePath;
		}

		private void clearTextbox(StackPanel container)
		{
			foreach (var child in container.Children)
				if (child is TextBox textBox)
					textBox.Text = "";
		}

		private void AddItem(object parameter)
        {
            var item = (SuppliesInventoryView)parameter;

			if (item.NameTextBox.Text == string.Empty ||
				item.QuantityTextBox.Text == string.Empty ||
				item.PriceTextBox.Text == string.Empty ||
				item.PetTypeTextBox.Text == string.Empty)
				//item.SupplyTypeTextBox.Text == string.Empty ||
				//item.ImageURLTextBox.Text == string.Empty)
			{

				MessageBox.Show("Please fill up all the forms");
			}
            else
            {
				//Add to DB
				PetshopDB.spAddPetSupply
					(
						item.NameTextBox.Text,
						1,//determineSupplyType(item.SupplyTypeTextBox.Text),
						determinePetType(item.PetTypeTextBox.Text),
						float.TryParse(item.PriceTextBox.Text, out float price2) ? price2 : 999.99f,
						int.TryParse(item.QuantityTextBox.Text, out int quantity2) ? quantity2 : 0,
						ProfileImagePath //item.ImageURLTextBox.Text
					);

				// ADD TO MEMORY
				SupplyItems.Add(new PetSupply
				{
					PetSupplyName = item.NameTextBox.Text,
					Quantity = int.TryParse(item.QuantityTextBox.Text, out int quantity) ? quantity : 0,
					Price = float.TryParse(item.PriceTextBox.Text, out float price) ? price : 0.0f,
					PetSupplyID = random.Next(),//int.Parse(item.ItemIDTextBox.Text),
					SupplyType = "cat", /*item.SupplyTypeTextBox.Text.ToLower(),*/
					Species = item.PetTypeTextBox.Text.ToLower(),
					InSupplyTypeID = 1, /*determineSupplyType(item.SupplyTypeTextBox.Text.ToLower()),*/
					InPetTypeID = determinePetType(item.PetTypeTextBox.Text.ToLower()),
					ImagePath = ProfileImagePath, //item.ImageURLTextBox.Text,

					Status = "Available",
					InStatusID = 2

				});

				clearTextbox(item.TextBoxContainer);
				item.myDataGrid.ItemsSource = null;
				item.myDataGrid.ItemsSource = SupplyItems;
				MessageBox.Show("Data Added Successfully");

			}

		}

        private void DeleteItem()

        {
			//STATUS ID = 2 IS ARCHIVED
			PetshopDB.spSetPetSupplyStatus(selectedItem.PetSupplyID, 2);
			SupplyItems.Remove(selectedItem);
			MessageBox.Show("Data Deleted Successfully");
		}



		private void UpdateItem(object parameter)
		{
			var item = (SuppliesInventoryView)parameter;


			if (item.NameTextBox.Text == string.Empty ||
				item.QuantityTextBox.Text == string.Empty ||
				item.PriceTextBox.Text == string.Empty ||
				item.PetTypeTextBox.Text == string.Empty)
				//item.SupplyTypeTextBox.Text == string.Empty ||
				//item.ImageURLTextBox.Text == string.Empty)
			{

				MessageBox.Show("Please fill up all the forms");
			}
			else
			{
				selectedItem.PetSupplyName = item.NameTextBox.Text;
				selectedItem.Quantity = int.Parse(item.QuantityTextBox.Text);
				selectedItem.Price = float.Parse(item.PriceTextBox.Text);
				selectedItem.Species = item.PetTypeTextBox.Text;
				selectedItem.ImagePath = ProfileImagePath;
				//selectedItem.SupplyType = item.SupplyTypeTextBox.Text;
				//selectedItem.ImagePath = item.ImageURLTextBox.Text;

				PetshopDB.spUpdateSupplyData
					(
						selectedItem.PetSupplyID,
						selectedItem.PetSupplyName,
						determineSupplyType(selectedItem.SupplyType),
						determinePetType(selectedItem.Species),
						selectedItem.Price,
						selectedItem.Quantity,
						selectedItem.ImagePath
					);

				item.myDataGrid.ItemsSource = null;
				item.myDataGrid.ItemsSource = SupplyItems;
				MessageBox.Show("Data Updated Successfully");
				clearTextbox(item.TextBoxContainer);
			}

		}
	}
}
