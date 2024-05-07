using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class TransactionHistoryViewModel : ViewModelBase
    {
        //public RelayCommand EndScrollCommand => new RelayCommand(parameter => LoadMoreItems(parameter));


        public ObservableCollection<TransactionHistory> AllTransactions { get; set; }


        public TransactionHistoryViewModel()
        {
            AllTransactions = new ObservableCollection<TransactionHistory>();

            foreach (var transaction in PetshopDB.spGetAllTransactions(null, null))
            {
                if (transaction.Total_Cost == null)
                    continue;

                AllTransactions.Add(new TransactionHistory
                {
                    ID = transaction.Transaction_ID,
                    TotalCost = transaction.Total_Cost == null ? 0 : (double)transaction.Total_Cost,
                    Quantity = transaction.Quantity_Sold == null ? 0 : (int)transaction.Quantity_Sold,
                    ProcessDate = transaction.Process_Date,
                    Staff = transaction.Staff,
                    TransactionTypeID = transaction.Transaction_Type_ID,
                    TransactionTypeColor = transaction.Transaction_Type_ID == 1 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Yellow)
                });
            }
        }

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        //private void UpdateSearchInput(object parameter)
        //{
        //    ResetResultsAndRestrictions();

        //    _cancellationTokenSource.Cancel();
        //    _cancellationTokenSource = new CancellationTokenSource();

        //    Task.Run(() => GetSearchResults(_cancellationTokenSource.Token, parameter as string));
        //}
        //private void UpdateSearchResult(object parameter)
        //{
        //    if (DisplayedPets == null) { return; }

        //    _cancellationTokenSource.Cancel();
        //    _cancellationTokenSource = new CancellationTokenSource();

        //    Task.Run(() => GetSearchResults(_cancellationTokenSource.Token, parameter as string));
        //}

        //private async void GetSearchResults(CancellationToken cancellationToken, string filter = null)
        //{
        //    DateTime currentDateTime;
        //    DateTime previousDateTime;
        //    float price;
        //    int age;

        //    ISingleResult<spGetAllPetsResult> retrievedData = PetshopDB.spGetAllPets(
        //        _speciesFilter,
        //        null,
        //        null,
        //        _availabilityFilter);

        //    int counter = 0;

        //    if (filter != null)
        //    {
        //        await Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
        //        {
        //            foreach (var petData in retrievedData)
        //            {
        //                if (cancellationToken.IsCancellationRequested)
        //                    return;

        //                if (counter++ < _retrieveIndex)
        //                    continue;
        //                if (counter > _displayLimit)
        //                    return;
        //                if (DisplayedPets.Any(item => item.ID == petData.ID))
        //                    return;

        //                currentDateTime = DateTime.Now;
        //                previousDateTime = petData.Birthdate;
        //                age = ((currentDateTime.Year - previousDateTime.Year) * 12) + currentDateTime.Month - previousDateTime.Month;
        //                price = (float)Math.Round(petData.Price, 2);
        //                UpdateAvailabilityCount(petData.Status_ID);

        //                if (!petData.Name.ToLower().Contains(filter.ToLower()))
        //                {
        //                    --counter;
        //                    continue;
        //                }


        //                DisplayedPets.Add(new Pet
        //                {
        //                    ID = petData.ID,
        //                    PetName = petData.Name,
        //                    Breed = petData.Breed,
        //                    Species = petData.Species,
        //                    Gender = petData.Gender,
        //                    Birthdate = petData.Birthdate,
        //                    Price = price,
        //                    Status = petData.Status,
        //                    StatusID = petData.Status_ID,
        //                    BreedID = petData.Breed_ID,
        //                    SpeciesID = petData.Species_ID,
        //                    Age = age.ToString() + "mo.",
        //                    StatusColor = GetStatusColor(petData.Status_ID),
        //                    ImagePath = petData.Image_path
        //                });
        //                await Task.Delay(1);
        //            }
        //        }));
        //        return;
        //    }

        //    await Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
        //    {
        //        foreach (var petData in retrievedData)
        //        {
        //            if (cancellationToken.IsCancellationRequested)
        //                return;

        //            if (counter++ < _retrieveIndex)
        //                continue;
        //            if (counter > _displayLimit)
        //                return;
        //            if (DisplayedPets.Any(item => item.ID == petData.ID))
        //                return;

        //            currentDateTime = DateTime.Now;
        //            previousDateTime = petData.Birthdate;
        //            age = ((currentDateTime.Year - previousDateTime.Year) * 12) + currentDateTime.Month - previousDateTime.Month;
        //            price = (float)Math.Round(petData.Price, 2);
        //            UpdateAvailabilityCount(petData.Status_ID);

        //            DisplayedPets.Add(new Pet
        //            {
        //                ID = petData.ID,
        //                PetName = petData.Name,
        //                Breed = petData.Breed,
        //                Species = petData.Species,
        //                Gender = petData.Gender,
        //                Birthdate = petData.Birthdate,
        //                Price = price,
        //                Status = petData.Status,
        //                StatusID = petData.Status_ID,
        //                BreedID = petData.Breed_ID,
        //                SpeciesID = petData.Species_ID,
        //                Age = age.ToString() + "mo.",
        //                StatusColor = GetStatusColor(petData.Status_ID),
        //                ImagePath = petData.Image_path
        //            });

        //            await Task.Delay(1);
        //        }
        //    }));
        //}
        //private void UpdateAvailabilityCount(int id)
        //{
        //    AvailabilityIndicators[id - 1].Count++;
        //}
        //private void ResetAvailabilityCount()
        //{
        //    for (int i = 0; i < AvailabilityIndicators.Count; ++i)
        //    {
        //        AvailabilityIndicators[i].Count = 0;
        //    }
        //}
        //private void LoadMoreItems(object sender)
        //{
        //    _contentScroller = sender;

        //    if (DisplayedPets.Count == _displayLimit)
        //    {
        //        _retrieveIndex = _displayLimit;
        //        _displayLimit += 10;
        //        UpdateSearchResult(SearchBarInput);
        //    }
        //}
        //private void ResetResultsAndRestrictions()
        //{
        //    _retrieveIndex = 0;
        //    _displayLimit = 15;
        //    ResetAvailabilityCount();
        //    DisplayedPets.Clear();

        //    if (_contentScroller != null)
        //        ((ScrollViewer)_contentScroller).ScrollToTop();
        //}
    }
}
