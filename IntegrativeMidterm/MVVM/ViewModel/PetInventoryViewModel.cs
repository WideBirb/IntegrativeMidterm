using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using IntegrativeMidterm.MVVM.Model.Filters;
using IntegrativeMidterm.userControl.General;
using IntegrativeMidterm.userControl.PetInventory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class PetInventoryViewModel : ViewModelBase
    {
        public ObservableCollection<AvailabilityIndicatorData> AvailabilityIndicators { get; set; }
        public ObservableCollection<Pet> PetsData { get; set; }
        public ObservableCollection<PetSpecies> PetSpeciesFilters { get; set; }

        public RelayCommand ConfirmCommand => new RelayCommand(execute => ManageInformation());
        public RelayCommand SearchCommand => new RelayCommand(parameter => UpdateSearchResult(parameter));

        public RelayCommand FilterCommand => new RelayCommand(parameter => SetSpeciesFilter(parameter));
        public RelayCommand AvailabilityCommand => new RelayCommand(parameter => SetAvailabilityFilter(parameter));
        public RelayCommand ResultSelectCommand => new RelayCommand(parameter => SetResultSelection(parameter));

        private string _searchBarInput = string.Empty;
        private string _searchBarPlaceholderText = string.Empty;

        private int? _speciesFilter = null;
        private int? _availabilityFilter = null;

        RadioButton _activeFilterButton = null;
        RadioButton _activeAvailabilityButton = null;
        RadioButton _activeResultButton = null;
        
        private string _petName;
        private string _birthdate;
        private string _petStatus;
        private string _price;
        private string _customer;

        private string _birthdayLabel = string.Empty;
        private Visibility _profileOptionsVisibility = Visibility.Collapsed;
        
        public Visibility ProfileOptionsVisibility
        {
            get { return _profileOptionsVisibility;; }
            set { _profileOptionsVisibility = value; OnPropertyChanged(); }
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

        //-----------------------------------------------------------------//

        public string BirthdayLabel
        {
            get { return _birthdayLabel; }
            set { _birthdayLabel = value; OnPropertyChanged(); }
        }

        //-----------------------------------------------------------------//

        public PetInventoryViewModel()
        {
            SearchBarPlaceholderText = "Search...";
            SearchBarInput = string.Empty;

            AvailabilityIndicators = new ObservableCollection<AvailabilityIndicatorData>();
            PetSpeciesFilters = new ObservableCollection<PetSpecies>();
            PetsData = new ObservableCollection<Pet>();

            InitializeData();
        }

        //-----------------------------------------------------------------//

        private void UpdateSearchResult(object parameter)
        {
            if (PetsData == null) { return; }

            PetsData.Clear();
            GetSearchResults(parameter as string);
        }
        private void GetSearchResults(string filter = null)
        {
            int statusID = 0;
            DateTime currentDateTime = DateTime.Now;
            DateTime previousDateTime = DateTime.Now;
            int age = 0;
            float price = 0;

            ISingleResult<spGetAllPetsResult> retrievedData =PetshopDB.spGetAllPets(
                _speciesFilter,
                null,
                null,
                _availabilityFilter);

            if (retrievedData == null) { return; }
            ResetAvailabilityCount();

            foreach (spGetAllPetsResult item in retrievedData)
            {
                statusID = GetStatusID(item.Status);
                currentDateTime = DateTime.Now;
                previousDateTime = item.Birthdate;
                age = ((currentDateTime.Year - previousDateTime.Year) * 12) + currentDateTime.Month - previousDateTime.Month;
                price = (float)Math.Round(item.Price, 2);

                if (filter != null)
                {
                    if (!item.Name.ToLower().Contains(filter.ToLower()))
                        continue;

                    PetsData.Add(new Pet
                    {
                        ID = item.ID,
                        PetName = item.Name,
                        Breed = item.Breed,
                        Gender = item.Gender,
                        Age = age.ToString() + "mo.",
                        Price = price,
                        Birhdate = item.Birthdate,
                        Status = item.Status,
                        StatusColor = GetStatusColor(statusID)
                    });
                    UpdateAvailabilityCount(statusID);
                    continue;
                }

                PetsData.Add(new Pet
                {
                    ID = item.ID,
                    PetName = item.Name,
                    Breed = item.Breed,
                    Gender = item.Gender,
                    Age = age.ToString() + "mo.",
                    Price = price,
                    Birhdate = item.Birthdate,
                    Status = item.Status,
                    StatusColor = GetStatusColor(statusID)
                });
                UpdateAvailabilityCount(statusID);
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
        private int GetStatusID(string status)
        {
            foreach (var item in AvailabilityIndicators)
            {
                if (item.Description.ToLower() == status.ToLower())
                {
                    return item.ID;
                }
            }
            return -1;
        }
        private void ManageInformation()
        {

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
                BirthdayLabel = null;

                ProfileOptionsVisibility = Visibility.Collapsed;
                _activeResultButton = null;
                return;
            }

            if (_activeResultButton != null)
                _activeResultButton.IsChecked = false;
            _activeResultButton = button;

            var chosenPet = PetsData.FirstOrDefault(item => item.ID == (int)button.Tag);
            if (chosenPet == null)
                return;

            PetName = chosenPet.PetName;
            BirthdayLabel = "Birthday:";
            Birthdate = chosenPet.Birhdate.ToString("MMMM d, yyyy");
            PetStatus = chosenPet.Status;
            Price = "Php " + Math.Round(chosenPet.Price, 2).ToString();

            ProfileOptionsVisibility = Visibility.Visible;
        }
    }
}
