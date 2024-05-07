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
using System.Data.Common;
using System.Threading;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class PetInventoryViewModel : ViewModelBase
    {
        public ObservableCollection<AvailabilityIndicatorData> AvailabilityIndicators { get; set; }
        public ObservableCollection<PetSpecies> PetSpeciesFilters { get; set; }
        public ObservableCollection<Pet> DisplayedPets { get; set; }
        
        public RelayCommand ConfirmCommand => new RelayCommand(execute => ManageInformation());
        public RelayCommand SearchCommand => new RelayCommand(parameter => UpdateSearchInput(parameter));

        public RelayCommand FilterCommand => new RelayCommand(parameter => SetSpeciesFilter(parameter));
        public RelayCommand AvailabilityCommand => new RelayCommand(parameter => SetAvailabilityFilter(parameter));
        public RelayCommand ResultSelectCommand => new RelayCommand(parameter => SetResultSelection(parameter));
        public RelayCommand EndScrollCommand => new RelayCommand(parameter => LoadMoreItems(parameter));

        private string _searchBarInput = string.Empty;
        private string _searchBarPlaceholderText = string.Empty;
        private bool _profileClosedStatus = true;
        private Visibility _profileOptionsVisibility = Visibility.Collapsed;

        private int? _speciesFilter = null;
        private int? _availabilityFilter = null;

        RadioButton _activeFilterButton = null;
        RadioButton _activeAvailabilityButton = null;
        RadioButton _activeResultButton = null;
        private string _previousSearchInput = string.Empty;
        
        private string _petName = string.Empty;
        private string _birthdate = string.Empty;
        private string _petStatus = string.Empty;
        private string _price = string.Empty;
        private string _customer = string.Empty;
        private string _species = string.Empty;
        private string _breed = string.Empty;

        int _displayLimit = 20;
        int _retrieveIndex = 0;
        
        private Pet _selectedPet { get; set; }
        private object _profileView = null;
        private object _contentScroller = null;
        
        public object ProfileView
        {
            get { return _profileView; }
            set
            {
                _profileView = value;
                OnPropertyChanged();
            }
        }

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
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private void UpdateSearchInput(object parameter)
        {
            ResetResultsAndRestrictions();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => GetSearchResults(_cancellationTokenSource.Token, parameter as string));
        }
        private void UpdateSearchResult(object parameter)
        {
            if (DisplayedPets == null) { return; }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => GetSearchResults(_cancellationTokenSource.Token, parameter as string));
        }

        private async void GetSearchResults(CancellationToken cancellationToken, string filter = null)
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

            int counter = 0;

            if (filter != null)
            {
                await Application.Current.Dispatcher.BeginInvoke(new Action( async() =>
                {
                    foreach (var petData in retrievedData)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        if (counter++ < _retrieveIndex)
                            continue;
                        if (counter > _displayLimit)
                            return;
                        if (DisplayedPets.Any(item => item.ID == petData.ID))
                            return;

                        currentDateTime = DateTime.Now;
                        previousDateTime = petData.Birthdate;
                        age = ((currentDateTime.Year - previousDateTime.Year) * 12) + currentDateTime.Month - previousDateTime.Month;
                        price = (float)Math.Round(petData.Price, 2);
                        UpdateAvailabilityCount(petData.Status_ID);

                        if (!petData.Name.ToLower().Contains(filter.ToLower()))
                        {
                            --counter;
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
                        await Task.Delay(1);
                    }
                }));
                return;
            }

            await Application.Current.Dispatcher.BeginInvoke(new Action( async() =>
            {
                foreach (var petData in retrievedData)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (counter++ < _retrieveIndex)
                        continue;
                    if (counter > _displayLimit)
                        return;
                    if (DisplayedPets.Any(item => item.ID == petData.ID))
                        return;

                    currentDateTime = DateTime.Now;
                    previousDateTime = petData.Birthdate;
                    age = ((currentDateTime.Year - previousDateTime.Year) * 12) + currentDateTime.Month - previousDateTime.Month;
                    price = (float)Math.Round(petData.Price, 2);
                    UpdateAvailabilityCount(petData.Status_ID);

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

                    await Task.Delay(1);
                }
            }));
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
        private void LoadMoreItems(object sender)
        {
            _contentScroller = sender;

            if (DisplayedPets.Count == _displayLimit)
            {
                _retrieveIndex = _displayLimit;
                _displayLimit += 10;
                UpdateSearchResult(SearchBarInput);
            }
        }
        private void ResetResultsAndRestrictions()
        {
            _retrieveIndex = 0;
            _displayLimit = 15;
            ResetAvailabilityCount();
            DisplayedPets.Clear();

            if (_contentScroller != null)
            ((ScrollViewer)_contentScroller).ScrollToTop();
        }

        //-----------------------------------------------------------------//

        private void InitializeData()
        {
            ISingleResult<spGetPetStatusResult> availabilityStatus = PetshopDB.spGetPetStatus();
            foreach (var status in availabilityStatus)
            {
                AvailabilityIndicators.Add(new AvailabilityIndicatorData
                {
                    ID = status.pet_status_id,
                    Description = status.description,
                    IconColor = GetStatusColor(status.pet_status_id)
                });
            }

            ISingleResult<spGetPetTypesResult> petSpecies = PetshopDB.spGetPetTypes();
            foreach (var species in petSpecies)
            {
                PetSpeciesFilters.Add(new PetSpecies
                {
                    ID = species.pet_type_id,
                    Description = species.description
                });
            }
            UpdateSearchResult(null);
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
            ProfileView = new PetProfileViewModel(_selectedPet);
            ((PetProfileViewModel)ProfileView).CloseView += ClosePetProfile;
        }
        private void ClosePetProfile(object sender, EventArgs e)
        {
            PetProfileViewModel ProfileVM = sender as PetProfileViewModel;
            ProfileClosedStatus = true;

            if (ProfileVM.ChangesSaved)
            {
                ProfileOptionsVisibility = Visibility.Collapsed;
                UpdateSearchInput(SearchBarInput);
            }
            ProfileView = null;
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

            ResetResultsAndRestrictions();
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

                _activeResultButton = null;
                ProfileOptionsVisibility = Visibility.Collapsed;
                return;
            }

            if (_activeResultButton != null)
                _activeResultButton.IsChecked = false;
            _activeResultButton = button;

            var chosenPet = DisplayedPets.FirstOrDefault(item => item.ID == (int)button.Tag);

            _selectedPet = chosenPet;
            ShowPetDetails(chosenPet);

        }
        private void ShowPetDetails(Pet chosenPet)
        {
            if (chosenPet == null)
            {
                ProfileOptionsVisibility = Visibility.Collapsed;
                return;
            }

            PetName = chosenPet.PetName;
            Birthdate = chosenPet.Birthdate.ToString("MM/dd/yyyy");
            PetStatus = chosenPet.Status;
            Price = Math.Round(chosenPet.Price, 2).ToString("#,##0.00");
            Species = chosenPet.Species;
            Breed = chosenPet.Breed;

            ProfileOptionsVisibility = Visibility.Visible;
        }

        //-----------------------------------------------------------------//
    }
}
