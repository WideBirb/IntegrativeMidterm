using IntegrativeMidterm.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using IntegrativeMidterm.MVVM.Model;
using System.Collections.ObjectModel;
using IntegrativeMidterm.MVVM.Model.Filters;
using System.Data.Common;
using System.Windows.Controls;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class PetProfileViewModel : ViewModelBase
    {
        public ObservableCollection<PetSpecies> PetSpeciesList { get; set; }
        public ObservableCollection<PetBreed> PetBreedsList {  get; set; }

        public EventHandler CloseView;
        public readonly Pet SelectedPet = null;

        public bool ChangesSaved { get; private set; }

        //-----------------------------------------------------------------//

        private string _petName = string.Empty;
        private string _birthdate = string.Empty;
        private string _petStatus = string.Empty;
        private string _price = string.Empty;
        private string _customer = string.Empty;
        private string _species = string.Empty;
        private string _breed = string.Empty;
        private string _vaccinationDate = string.Empty;
        private string _checkupDate = string.Empty;
        private string _profileImagePath = string.Empty;
        private bool _isGenderMale = true;
        private bool _isGenderFemale = false;

        //-----------------------------------------------------------------//

        public string PetName
        {
            get { return _petName; }
            set { _petName = value; OnPropertyChanged(); }
        }
        public string Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; OnPropertyChanged(); }
        }
        public string Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }
        public string Species
        {
            get { return _species; }
            set { _species = value; OnPropertyChanged(); }
        }
        public string Breed
        {
            get { return _breed; }
            set { _breed = value; OnPropertyChanged(); }
        }
        public string VaccinationDate
        {
            get { return _vaccinationDate; }
            set { _vaccinationDate = value; OnPropertyChanged(); }
        }
        public string CheckupDate
        {
            get { return _checkupDate; }
            set { _checkupDate = value; OnPropertyChanged(); }
        }
        public bool IsGenderMale
        {
            get { return _isGenderMale; }
            set { _isGenderMale = value; OnPropertyChanged(); }
        }
        public bool IsGenderFemale
        {
            get { return _isGenderFemale; }
            set { _isGenderFemale = value; OnPropertyChanged(); }
        }
        public string PetStatus
        {
            get { return _petStatus; }
            set { _petStatus = value; OnPropertyChanged(); }
        }
        public string ProfileImagePath
        {
            get { return _profileImagePath; }
            set { _profileImagePath = value; OnPropertyChanged(); }
        }
        public string Customer
        {
            get { return _customer; }
            set { _customer = value; OnPropertyChanged(); }
        }
        public Visibility PlaceholderVisibility { get; set; } = Visibility.Visible;

        //-----------------------------------------------------------------//

        public RelayCommand UploadImageCommand => new RelayCommand(execute => UploadImage());
        public RelayCommand SaveChangesCommand => new RelayCommand(execute => SaveChanges());
        public RelayCommand DiscardChangesCommand => new RelayCommand(execute => DiscardChanges());
        public RelayCommand SelectionChanged => new RelayCommand(parameter => SetSelection(parameter));
        
        //-----------------------------------------------------------------//

        public PetProfileViewModel(Pet chosenPet = null)
        {
            PetSpeciesList = new ObservableCollection<PetSpecies>();
            PetBreedsList = new ObservableCollection<PetBreed>();

            SelectedPet = chosenPet;

            if (chosenPet != null)
            {
                PlaceholderVisibility = Visibility.Hidden;

                PetName = chosenPet.PetName;
                Birthdate = chosenPet.Birthdate.ToString("MM/dd/yyyy");
                PetStatus = chosenPet.Status;
                Price = Math.Round(chosenPet.Price, 2).ToString("#,##0.00");
                Species = chosenPet.Species;
                Breed = chosenPet.Breed;
                VaccinationDate = "N/A";
                CheckupDate = "N/A";
                ProfileImagePath = chosenPet.ImagePath;
                if (chosenPet.Gender == "M")
                {
                    IsGenderMale = true; IsGenderFemale = false;
                }
                else
                {
                    IsGenderMale = false; IsGenderFemale = true;
                }

                var species = PetshopDB.spGetPetTypes();
                foreach (var type in species)
                {
                    PetSpeciesList.Add(new PetSpecies
                    {
                        ID = type.pet_type_id,
                        Description = type.description
                    });
                }

                UpdateBreeds(chosenPet.SpeciesID);
            }
        }

        //-----------------------------------------------------------------//

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
        private void SaveChanges()
        {
            DateTime birthday;
            DateTime vaccination;
            DateTime deworm;
            string gender = string.Empty;

            if (!CheckDateFormat(Birthdate, out birthday))
            {
                MessageBox.Show("Date entries must be written in MM/DD/YYYY format!", "Invalid Birthday Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!CheckDateFormat(VaccinationDate, out vaccination) && VaccinationDate != "N/A")
            {
                MessageBox.Show("Date entries must be written in MM/DD/YYYY format!", "Invalid Vaccination Date Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!CheckDateFormat(CheckupDate, out deworm) && CheckupDate != "N/A")
            {
                MessageBox.Show("Date entries must be written in MM/DD/YYYY format!", "Invalid Deworm Date Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!CheckSpecies(out int species_ID))
            {
                MessageBox.Show("Species not found or registered in database!", "Invalid Species Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!CheckBreed(species_ID, out int breed_ID))
            {
                MessageBox.Show("Breed not found or registered in " + Species + " species!", "Invalid Breed Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!CheckPriceFormat(Price))
            {
                MessageBox.Show("Price entry must be in proper monetary format, 2 decimal places max!", "Invalid Price Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!float.TryParse(Price, out float price) && price < 0)
            {
                MessageBox.Show("Price entry can not become negative!", "Invalid Price Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsGenderMale)
                gender = "M";
            else
                gender = "F";
            if (!CheckEmptyEntries())
            {
                MessageBox.Show("Please fill out all entries!", "Incomplete Entries", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PetshopDB.spUpdatePetData(SelectedPet.ID, PetName, gender, birthday, species_ID, breed_ID, Double.Parse(Price), ProfileImagePath);

            ChangesSaved = true;
            CloseView?.Invoke(this, EventArgs.Empty);
        }
        private void DiscardChanges()
        {
            ChangesSaved = false;
            CloseView?.Invoke(this, EventArgs.Empty);
        }
        private void SetSelection(object sender)
        {
            if (sender is PetSpecies species)
            {
                Species = species.Description;
                UpdateBreeds(species.ID);
                return;
            }

            if (sender is PetBreed breed)
            {
                Breed = breed.Description;
                return;
            }
        }
        private void UpdateBreeds(int ID)
        {
            PetBreedsList.Clear();

            var breeds = PetshopDB.spGetPetBreeds(ID);
            foreach (var breed in breeds)
            {
                PetBreedsList.Add(new PetBreed
                {
                    ID = breed.pet_breed_id,
                    Description = breed.description
                });
            }
        }

        //-----------------------------------------------------------------//

        private bool CheckEmptyEntries()
        {
            if (
                PetName == string.Empty ||
                Birthdate == string.Empty ||
                Price == string.Empty ||
                Species == string.Empty ||
                Breed == string.Empty ||
                VaccinationDate == string.Empty ||
                CheckupDate == string.Empty
                )
                return false;

            return true;
        }
        private bool CheckDateFormat(string date, out DateTime result)
        {
            return DateTime.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out result);
        }
        private bool CheckSpecies(out int ID)
        {
            var species = PetshopDB.spGetPetTypes().FirstOrDefault(pet => pet.description.ToLower() == Species.ToLower());
            if (species != null)
                ID = species.pet_type_id;
            else
                ID = -1;

            return species != null;
        }
        private bool CheckBreed(int species_ID, out int ID)
        {
            var breed = PetshopDB.spGetPetBreeds(species_ID).FirstOrDefault(pet => pet.description.ToLower() == Breed.ToLower());
            if (breed != null)
                ID = breed.pet_breed_id;
            else
                ID = -1;

            return breed != null;
        }
        private bool CheckPriceFormat(string price)
        {
            string pattern = @"^\d{1,3}(,\d{3})*(\.\d{1,2})?$";

            return Regex.IsMatch(price, pattern);
        }
    }
}
