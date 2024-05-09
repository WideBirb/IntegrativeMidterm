using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class TransactionHistoryViewModel : ViewModelBase
    {
        public RelayCommand ResultSelectCommand => new RelayCommand(parameter => SetResultSelection(parameter));
        public RelayCommand EndScrollCommand => new RelayCommand(parameter => LoadMoreItems(parameter));

        public ObservableCollection<TransactionHistory> DisplayedTransactions { get; set; }
        public ObservableCollection<ItemDetails> TransactionDetails { get; set; }

        private object _contentScroller = null;
        int _displayLimit = 20;
        int _retrieveIndex = 0;

        private string _transactionID;
        private DateTime _transactionDate;
        private int _purchaseQuantity;
        private string _totalPrice;

        public string TransactionID
        {
            get { return _transactionID; }
            set { _transactionID = value; OnPropertyChanged(); }
        }
        public DateTime TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; OnPropertyChanged(); }
        }
        public int PurchaseQuantity
        {
            get { return _purchaseQuantity; }
            set { _purchaseQuantity = value; OnPropertyChanged(); }
        }
        public string TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        RadioButton _activeResultButton = null;

        public TransactionHistoryViewModel()
        {
            DisplayedTransactions = new ObservableCollection<TransactionHistory>();
            TransactionDetails = new ObservableCollection<ItemDetails>();

            UpdateSearchResult();
        }

        private void SetResultSelection(object sender)
        {
            RadioButton button = sender as RadioButton;

            if (button == _activeResultButton)
            {
                _activeResultButton.IsChecked = false;

                TransactionID = null;
                TransactionDate = DateTime.MinValue;
                PurchaseQuantity = 0;
                TotalPrice = null;

                _activeResultButton = null;
                TransactionDetails.Clear();
                return;
            }

            if (_activeResultButton != null)
                _activeResultButton.IsChecked = false;
            _activeResultButton = button;

            TransactionDetails.Clear();
            var transactionSummary = PetshopDB.spGetSupplyTransactionSummary((int?)button.Tag);
            var transaction = PetshopDB.spGetAllTransactions(null, null).FirstOrDefault(t => t.Transaction_ID == (int?)button.Tag);

            if (transaction != null)
            {
                TransactionID = transaction.Transaction_ID.ToString();
                TransactionDate = transaction.Process_Date;
                PurchaseQuantity = (int)transaction.Quantity_Sold;
                TotalPrice = Regex.Replace(Math.Round((double)transaction.Total_Cost, 2).ToString(), @"(\d)(?=(\d{3})+$)", "$1,");
            }

            if (transaction.Transaction_Type_ID == 1)
            {
                foreach (var product in transactionSummary)
                {
                    TransactionDetails.Add(new ItemDetails
                    {
                        SupplyName = product.Supply_name,
                        QuantityAndPrice = product.Purchase_Quantity.ToString() + "x " + Math.Round(product.Price_per_item,2).ToString(),
                        TotalItemPrice = Math.Round(product.Purchase_Quantity * product.Price_per_item, 2).ToString()
                    });
                }
            }
            else
            {
                TransactionDetails.Add(new ItemDetails
                {
                    SupplyName = "PET PURCHASE",
                    QuantityAndPrice = transaction.Total_Cost.ToString(),
                    TotalItemPrice = Regex.Replace(transaction.Total_Cost.ToString(), @"(\d)(?=(\d{3})+$)", "$1,")
                });
            }
        }

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private void ResetAndUpdateResults()
        {
            ResetResultsAndRestrictions();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => GetSearchResults(_cancellationTokenSource.Token));
        }
        private void UpdateSearchResult()
        {
            if (DisplayedTransactions == null) { return; }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => GetSearchResults(_cancellationTokenSource.Token));
        }

        private async void GetSearchResults(CancellationToken cancellationToken)
        {
            var data = PetshopDB.spGetAllTransactions(null, null).Reverse();

            int counter = 0;
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var transaction in data)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    if (counter++ < _retrieveIndex)
                        continue;
                    if (counter > _displayLimit)
                        continue;
                    if (DisplayedTransactions.Any(item => item.ID == transaction.Transaction_ID))
                        return;

                    if (transaction.Total_Cost == null)
                    {
                        --counter;
                        continue;
                    }
                    
                    DisplayedTransactions.Add(new TransactionHistory
                    {
                        ID = transaction.Transaction_ID,
                        TotalCost = transaction.Total_Cost == null ? 0 : Math.Round((double)transaction.Total_Cost, 2),
                        Quantity = transaction.Quantity_Sold == null ? 0 : (int)transaction.Quantity_Sold,
                        ProcessDate = transaction.Process_Date,
                        Staff = transaction.Staff,
                        TransactionType = transaction.Transaction_Type,
                        TransactionTypeID = transaction.Transaction_Type_ID,
                        TransactionTypeColor = transaction.Transaction_Type_ID == 1 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Yellow)
                    });
                }
            }));
        }
        private void LoadMoreItems(object sender)
        {
            _contentScroller = sender;

            if (DisplayedTransactions.Count == _displayLimit)
            {
                _retrieveIndex = _displayLimit;
                _displayLimit += 10;
                UpdateSearchResult();
            }
        }
        private void ResetResultsAndRestrictions()
        {
            _retrieveIndex = 0;
            _displayLimit = 15;
            DisplayedTransactions.Clear();

            if (_contentScroller != null)
                ((ScrollViewer)_contentScroller).ScrollToTop();
        }

        public class ItemDetails
        {
            public string SupplyName { get; set; }
            public string QuantityAndPrice { get; set; }
            public string TotalItemPrice { get; set; }
        }
    }
}
