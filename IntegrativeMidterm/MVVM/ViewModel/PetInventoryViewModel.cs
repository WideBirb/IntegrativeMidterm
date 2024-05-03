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
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class PetInventoryViewModel : ViewModelBase
    {
        public ObservableCollection<AvailabilityIndicatorData> AvailabilityIndicators { get; set; }
        public ObservableCollection<Pet> PetsData { get; set; }
        public ObservableCollection<PetSpecies> PetSpeciesFilters { get; set; }

        public RelayCommand ConfirmCommand => new RelayCommand(execute => ManageInformation());
        public RelayCommand FilterCommand { get; }

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

        public PetInventoryViewModel()
        {
            SearchBarPlaceholderText = "Search...";
            SearchBarInput = string.Empty;

            FilterCommand = new RelayCommand(UpdateSearchResult);

            AvailabilityIndicators = new ObservableCollection<AvailabilityIndicatorData>
            {
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Green,
                    Description = "Available",
                    Count = 0
                },
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Red,
                    Description = "Reserved",
                    Count = 0
                },
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Yellow,
                    Description = "Adopted",
                    Count = 0
                },
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Black,
                    Description = "Deceased",
                    Count = 0
                }
            };
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
            PetsData = new ObservableCollection<Pet>();

            InitializeData();
        }

        private void UpdateSearchResult(object parameter)
        {
            if (PetsData == null) { return; }

            PetsData.Clear();
            GetSearchResults(parameter as string);
        }

        private void GetSearchResults(string filter = null)
        {
            ISingleResult<spGetAllPetsResult> retrievedData = PetshopDB.spGetAllPets(null, null, null, null);

            if (retrievedData == null) { return; }

            foreach (spGetAllPetsResult item in retrievedData)
            {
                if (filter != null)
                {
                    if (!item.Name.ToLower().Contains(filter.ToLower()))
                        continue;

                    PetsData.Add(
                    new Pet
                    {
                        PetName = item.Name,
                        Breed = item.Breed,
                        Gender = item.Gender,
                        Age = item.Birthdate.ToShortDateString(),
                        StatusColor = Brushes.Green,
                    });
                    continue;
                }

                PetsData.Add(
                new Pet
                {
                    PetName = item.Name,
                    Breed = item.Breed,
                    Gender = item.Gender,
                    Age = item.Birthdate.ToShortDateString(),
                    StatusColor = Brushes.Green,
                });
            }
        }

        private async void InitializeData()
        {
            while (PetshopDB.spGetAllPets(null, null, null, null) == null)
            {
                await Task.Delay(100);
            }
            GetSearchResults();
        }

        private void ManageInformation()
        {

        }
    }
}
