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
        public readonly Pet CurrentProfile = null;

        public bool ChangesSaved { get; private set; }

        //-----------------------------------------------------------------//

        public bool EditMode { get; private set; }

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

        private string _profileTitle = string.Empty;
        private string _saveButtonContent = string.Empty;
        private string _discardButtonContent = string.Empty;

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

        //-----------------------------------------------------------------//

        public string ProfileTitle
        {
            get { return _profileTitle; }
            set { _profileTitle = value; OnPropertyChanged(); }
        }
        public string SaveButtonContent
        {
            get { return _saveButtonContent; }
            set { _saveButtonContent = value; OnPropertyChanged(); }
        }
        public string DiscardButtonContent
        {
            get { return _discardButtonContent; }
            set { _discardButtonContent = value; OnPropertyChanged(); }
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

            CurrentProfile = chosenPet;

            var species = PetshopDB.spGetPetTypes();
            foreach (var type in species)
            {
                PetSpeciesList.Add(new PetSpecies
                {
                    ID = type.pet_type_id,
                    Description = type.description
                });
            }

            if (chosenPet != null)
            {
                EditMode = true;

                ProfileTitle = "PET PROFILE";
                SaveButtonContent = "SAVE CHANGES";
                DiscardButtonContent = "DISCARD CHANGES";
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

                UpdateBreeds(chosenPet.SpeciesID);
                return;
            }

            EditMode = false; // ADD PET MODE
            CurrentProfile = new Pet();

            IsGenderMale = false;
            ProfileTitle = "REGISTER PET";
            SaveButtonContent = "ADD NEW PROFILE";
            DiscardButtonContent = "CANCEL";
            PlaceholderVisibility = Visibility.Visible;
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
            if (!EntriesAreValid(out ProfileDetails profile))
                return;

            PetName = PetName.Trim();
            PetName = Regex.Replace(PetName, @"\s+", " ");

            if (EditMode)
            {
                PetshopDB.spUpdatePetData
                    (
                        CurrentProfile.ID,
                        PetName,
                        profile.Gender,
                        profile.Birthdate,
                        profile.Species_ID,
                        profile.Breed_ID,
                        Double.Parse(Price),
                        ProfileImagePath
                    );
            }
            else
            {
                PetshopDB.spRegisterPet
                    (
                        PetName,
                        profile.Gender,
                        profile.Birthdate,
                        profile.Species_ID,
                        profile.Breed_ID,
                        Double.Parse(Price),
                        ProfileImagePath
                    );
            }

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

        private bool EntriesAreValid(out ProfileDetails profile)
        {
            profile = null;

            if (!CheckEmptyEntries())
            {
                MessageBox.Show("Please fill out all entries!", "Incomplete Entries", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            DateTime birthday = DateTime.MinValue;
            DateTime vaccination = DateTime.MinValue;
            DateTime checkup = DateTime.MinValue;
            string gender = null;
            int species_ID = 0;
            int breed_ID = 0;

            if (String.IsNullOrWhiteSpace(PetName))
            {
                MessageBox.Show("Please enter pet name!", "Invalid Name Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!Regex.IsMatch(PetName, "^[a-zA-Z\\s]+$"))
            {
                MessageBox.Show("Name entry must not include numerical or special characters!", "Invalid Name Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!CheckDateFormat(Birthdate, out birthday))
            {
                MessageBox.Show("Date entries must be written in MM/DD/YYYY format!", "Invalid Birthday Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!CheckDateFormat(VaccinationDate, out vaccination) && VaccinationDate != "N/A")
            {
                MessageBox.Show("Date entries must be written in MM/DD/YYYY format!", "Invalid Vaccination Date Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!CheckDateFormat(CheckupDate, out checkup) && CheckupDate != "N/A")
            {
                MessageBox.Show("Date entries must be written in MM/DD/YYYY format!", "Invalid Checkup Date Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!CheckSpecies(out species_ID))
            {
                MessageBox.Show("Species not found or registered in database!", "Invalid Species Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!CheckBreed(species_ID, out breed_ID))
            {
                MessageBox.Show("Breed not found or registered in " + Species + " species!", "Invalid Breed Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!CheckPriceFormat(Price))
            {
                MessageBox.Show("Price entry must be in proper monetary format, 2 decimal places max!", "Invalid Price Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!float.TryParse(Price, out float price) && price < 0)
            {
                MessageBox.Show("Price entry can not become negative!", "Invalid Price Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!IsGenderMale && !IsGenderFemale)
            {
                MessageBox.Show("Please select pet gender!", "Invalid Gender Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (IsGenderMale)
                gender = "M";
            else
                gender = "F";

            profile = new ProfileDetails()
            {
                Birthdate = birthday,
                Vaccination = vaccination,
                Checkup = checkup,
                Gender = gender,
                Species_ID = species_ID,
                Breed_ID = breed_ID
            };
            return true;
        }
        private bool CheckEmptyEntries()
        {
            if (
                PetName == string.Empty ||
                Birthdate == string.Empty ||
                Price == string.Empty ||
                Species == string.Empty ||
                Breed == string.Empty ||
                VaccinationDate == string.Empty ||
                CheckupDate == string.Empty ||
                (!IsGenderFemale && !IsGenderMale)
                )
                return false;

            return true;
        }
        private bool CheckDateFormat(string date, out DateTime result)
        {
            date = Regex.Replace(date, @"\s+", "");
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
            string pattern = @"^\d{1,3}(,\d{3})*(\.\d{1,2})?$|^(\d+)(\.\d{1,2})?$";

            return Regex.IsMatch(price, pattern);
        }

        private class ProfileDetails
        {
            public DateTime Birthdate { get; set; }
            public DateTime Vaccination { get; set; }
            public DateTime Checkup { get; set; }
            public string Gender { get; set; }
            public int Species_ID { get; set; }
            public int Breed_ID { get; set; }
        }
    }
}
