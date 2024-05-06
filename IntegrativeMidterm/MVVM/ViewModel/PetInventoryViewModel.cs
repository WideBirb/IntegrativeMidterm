using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.Model.Filters;
using IntegrativeMidterm.userControl.General;
using IntegrativeMidterm.userControl.PetInventory;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class PetInventoryViewModel : ViewModelBase
    {
        public ObservableCollection<AvailabilityIndicatorData> AvailabilityIndicators { get; set; }
        public ObservableCollection<PetSpecies> PetSpeciesFilters { get; set; }
        public ObservableCollection<Pet> DisplayedPets { get; set; }
        
        public RelayCommand ConfirmCommand => new RelayCommand(execute => ManageInformation());
        public RelayCommand SearchCommand => new RelayCommand(parameter => UpdateSearchResult(parameter));
        public RelayCommand UploadImageCommand => new RelayCommand(parameter => UploadImage());
        public RelayCommand SaveChangesCommand => new RelayCommand(parameter => SaveChanges());
        public RelayCommand DiscardChangesCommand => new RelayCommand(parameter => DiscardChanges());

        public RelayCommand FilterCommand => new RelayCommand(parameter => SetSpeciesFilter(parameter));
        public RelayCommand AvailabilityCommand => new RelayCommand(parameter => SetAvailabilityFilter(parameter));
        public RelayCommand ResultSelectCommand => new RelayCommand(parameter => SetResultSelection(parameter));

        private string _searchBarInput = string.Empty;
        private string _searchBarPlaceholderText = string.Empty;
        private bool _profileClosedStatus = true;
        private Visibility _profileOptionsVisibility = Visibility.Collapsed;
        private Visibility _profileVisibility = Visibility.Collapsed;

        private int? _speciesFilter = null;
        private int? _availabilityFilter = null;

        RadioButton _activeFilterButton = null;
        RadioButton _activeAvailabilityButton = null;
        RadioButton _activeResultButton = null;
        
        private string _petName = string.Empty;
        private string _birthdate = string.Empty;
        private string _petStatus = string.Empty;
        private string _price = string.Empty;
        private string _customer = string.Empty;
        private string _species = string.Empty;
        private string _breed = string.Empty;
        private string _vaccinationDate = string.Empty;
        private string _dewormDate = string.Empty;
        private bool _isGenderMale = true;
        private bool _isGenderFemale = false;
        private string _profileImagePath = string.Empty;

        private Pet _selectedPet { get; set; }

        public Visibility ProfileOptionsVisibility
        {
            get { return _profileOptionsVisibility;; }
            set { _profileOptionsVisibility = value; OnPropertyChanged(); }
        }
        public bool ProfileClosedStatus
        {
            get { return _profileClosedStatus; }
            set { _profileClosedStatus = value; OnPropertyChanged(); }
        }
        public Visibility ProfileVisibility
        {
            get { return _profileVisibility; }
            set { _profileVisibility = value; OnPropertyChanged(); }
        }
        //-----------------------------------------------------------------//

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
        public string PetStatus
        {
            get { return _petStatus; }
            set { _petStatus = value; OnPropertyChanged(); }
        }
        public string Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }
        public string Customer
        {
            get { return _customer; }
            set { _customer = value; OnPropertyChanged(); }
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
        public string DewormDate
        {
            get { return _dewormDate; }
            set { _dewormDate = value; OnPropertyChanged(); }
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
        public string ProfileImagePath
        {
            get { return _profileImagePath; }
            set { _profileImagePath = value; OnPropertyChanged(); }
        }


        //-----------------------------------------------------------------//

        public PetInventoryViewModel()
        {
            AvailabilityIndicators = new ObservableCollection<AvailabilityIndicatorData>();
            PetSpeciesFilters = new ObservableCollection<PetSpecies>();
            DisplayedPets = new ObservableCollection<Pet>();

            SearchBarPlaceholderText = "Search...";
            SearchBarInput = string.Empty;

            InitializeData();
        }

        //-----------------------------------------------------------------//

        private void UpdateSearchResult(object parameter)
        {
            if (DisplayedPets == null) { return; }

            DisplayedPets.Clear();
            GetSearchResults(parameter as string);
        }
        private void GetSearchResults(string filter = null)
        {
            DateTime currentDateTime;
            DateTime previousDateTime;
            float price;
            int age;

            ISingleResult<spGetAllPetsResult> retrievedData = PetshopDB.spGetAllPets(
                _speciesFilter,
                null,
                null,
                _availabilityFilter);

            if (retrievedData == null) { return; }
            ResetAvailabilityCount();

            foreach (var petData in retrievedData)
            {
                currentDateTime = DateTime.Now;
                previousDateTime = petData.Birthdate;
                age = ((currentDateTime.Year - previousDateTime.Year) * 12) + currentDateTime.Month - previousDateTime.Month;
                price = (float)Math.Round(petData.Price, 2);
                UpdateAvailabilityCount(petData.Status_ID);

                if (filter != null)
                {
                    if (!petData.Name.ToLower().Contains(filter.ToLower()))
                        continue;

                    DisplayedPets.Add(new Pet
                    {
                        ID = petData.ID,
                        PetName = petData.Name,
                        Breed = petData.Breed,
                        Species = petData.Species,
                        Gender = petData.Gender,
                        Birthdate = petData.Birthdate,
                        Price = price,
                        Status = petData.Status,
                        StatusID = petData.Status_ID,
                        BreedID = petData.Breed_ID,
                        SpeciesID = petData.Species_ID,
                        Age = age.ToString() + "mo.",
                        StatusColor = GetStatusColor(petData.Status_ID),
                        ImagePath = petData.Image_path
                    });
                    continue;
                }

                DisplayedPets.Add(new Pet
                {
                    ID = petData.ID,
                    PetName = petData.Name,
                    Breed = petData.Breed,
                    Species = petData.Species,
                    Gender = petData.Gender,
                    Birthdate = petData.Birthdate,
                    Price = price,
                    Status = petData.Status,
                    StatusID = petData.Status_ID,
                    BreedID = petData.Breed_ID,
                    SpeciesID = petData.Species_ID,
                    Age = age.ToString() + "mo.",
                    StatusColor = GetStatusColor(petData.Status_ID),
                    ImagePath = petData.Image_path
                });
            }
        }
        private void UpdateAvailabilityCount(int id)
        {
            AvailabilityIndicators[id - 1].Count++;
        }
        private void ResetAvailabilityCount()
        {
            for (int i = 0; i < AvailabilityIndicators.Count; ++i)
            {
                AvailabilityIndicators[i].Count = 0;
            }
        }

        //-----------------------------------------------------------------//

        private async void InitializeData()
        {
            ISingleResult<spGetPetStatusResult> availabilityStatus = null;
            do
            {
                availabilityStatus = PetshopDB.spGetPetStatus();
                await Task.Delay(0);
            } while (availabilityStatus == null);

            foreach (var status in availabilityStatus)
            {
                AvailabilityIndicators.Add(new AvailabilityIndicatorData
                {
                    ID = status.pet_status_id,
                    Description = status.description,
                    IconColor = GetStatusColor(status.pet_status_id)
                });
            }

            ISingleResult<spGetPetTypesResult> petSpecies = null;
            do
            {
                petSpecies = PetshopDB.spGetPetTypes();
                await Task.Delay(0);
            } while (petSpecies == null);

            foreach (var species in petSpecies)
            {
                PetSpeciesFilters.Add(new PetSpecies
                {
                    ID = species.pet_type_id,
                    Description = species.description
                });
            }

            while (PetshopDB.spGetAllPets(null, null, null, null) == null)
            {
                await Task.Delay(100);
            }
            GetSearchResults();
        }
        private SolidColorBrush GetStatusColor(int status)
        {
            switch (status)
            {
                case 1:
                    return Brushes.Green;
                case 2:
                    return Brushes.Yellow;
                case 3:
                    return Brushes.Red;
                case 4:
                    return Brushes.Black;
                default:
                    return Brushes.Transparent;
            }
        }
        private void ManageInformation()
        {
            ProfileClosedStatus = false;
            ProfileVisibility = Visibility.Visible;
        }

        //-----------------------------------------------------------------//

        private void SetSpeciesFilter(object sender)
        {
            RadioButton button = sender as RadioButton;

            if (_activeFilterButton == button)
            {
                _activeFilterButton.IsChecked = false;

                _activeFilterButton = null;
                _speciesFilter = null;
            }
            else
            {
                if (_activeFilterButton != null)
                    _activeFilterButton.IsChecked = false;

                int? filter = (int?)button.Tag;
                _activeFilterButton = button;
                _speciesFilter = filter;
            }

            UpdateSearchResult(SearchBarInput);
        }
        private void SetAvailabilityFilter(object sender)
        {
            RadioButton button = sender as RadioButton;

            if (_activeAvailabilityButton == button)
            {
                _activeAvailabilityButton.IsChecked = false;

                _activeAvailabilityButton = null;
                _availabilityFilter = null;
            }
            else
            {
                if (_activeAvailabilityButton != null)
                    _activeAvailabilityButton.IsChecked = false;

                int? availability = (int?)button.Tag;
                _activeAvailabilityButton = button;
                _availabilityFilter = availability;
            }

            UpdateSearchResult(SearchBarInput);
        }
        private void SetResultSelection(object sender)
        {
            RadioButton button = sender as RadioButton;

            if (button == _activeResultButton)
            {
                _activeResultButton.IsChecked = false;

                PetName = null;
                Birthdate = null;
                PetStatus = null;
                Price = null;
                Customer = null;
                Species = null;
                Breed = null;
                VaccinationDate = null;
                DewormDate = null;
                IsGenderMale = true;
                IsGenderFemale = false;

                ProfileOptionsVisibility = Visibility.Collapsed;
                _activeResultButton = null;
                return;
            }

            if (_activeResultButton != null)
                _activeResultButton.IsChecked = false;
            _activeResultButton = button;

            var chosenPet = DisplayedPets.FirstOrDefault(item => item.ID == (int)button.Tag);
            if (chosenPet == null)
                return;
            _selectedPet = chosenPet;

            PetName = chosenPet.PetName;
            Birthdate = chosenPet.Birthdate.ToString("MM/dd/yyyy");
            PetStatus = chosenPet.Status;
            Price = Math.Round(chosenPet.Price, 2).ToString("#,##0.00");
            ProfileOptionsVisibility = Visibility.Visible;
            Species = chosenPet.Species;
            Breed = chosenPet.Breed;
            VaccinationDate = "N/A";
            DewormDate = "N/A";
            ProfileImagePath = chosenPet.ImagePath;

            if (chosenPet.Gender == "M")
            {
                IsGenderMale = true; IsGenderFemale = false;
            }
            else
            {
                IsGenderMale = false; IsGenderFemale = true;
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
            if (!CheckDateFormat(DewormDate, out deworm) && DewormDate != "N/A")
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

            PetshopDB.spUpdatePetData(_selectedPet.ID, PetName, gender, birthday, species_ID, breed_ID, Double.Parse(Price), ProfileImagePath);

            ProfileClosedStatus = true;
            ProfileVisibility = Visibility.Collapsed;
            ProfileOptionsVisibility = Visibility.Collapsed;
            UpdateSearchResult(SearchBarInput);
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
        private void DiscardChanges()
        {
            ProfileClosedStatus = true;
            ProfileVisibility = Visibility.Collapsed;
        }
    }
}
