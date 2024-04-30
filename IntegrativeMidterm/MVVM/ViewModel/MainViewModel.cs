using IntegrativeMidterm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        public RelayCommand DashboardViewCommand { get; set; }
        public RelayCommand CheckOutViewCommand { get; set; }
        public RelayCommand SuppliesViewCommand { get; set; }
        public RelayCommand TransactionHistoryViewCommand { get; set; }


        public DashboardViewModel DashboardVM { get; set; }
        public CheckOutViewModel CheckOutVM { get; set; }
        public SuppliesInventoryViewModel SuppliesInventoryVM { get; set; }
        public TransactionHistoryViewModel TransactionHistoryVM { get; set; }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private string _windowNameText { get; set; }

        public string WindowNameText
        {
            get { return _windowNameText; }
            set
            {
                _windowNameText = value;
                OnPropertyChanged();
            }
        }



        public MainViewModel()
        {
            DashboardVM = new DashboardViewModel();
            CheckOutVM = new CheckOutViewModel();
            SuppliesInventoryVM = new SuppliesInventoryViewModel();
            TransactionHistoryVM = new TransactionHistoryViewModel();

            CurrentView = DashboardVM;
            WindowNameText = "DASHBOARD";

            DashboardViewCommand = new RelayCommand(o =>
            {
                CurrentView = DashboardVM;
                WindowNameText = "DASHBOARD";
            });
            CheckOutViewCommand = new RelayCommand(o =>
            {
                CurrentView = CheckOutVM;
                WindowNameText = "SUPPLIES SHOP";
            });
            SuppliesViewCommand = new RelayCommand(o =>
            {
                CurrentView = SuppliesInventoryVM;
                WindowNameText = "SUPPLIES INVENTORY";
            });
            TransactionHistoryViewCommand = new RelayCommand(o =>
            {
                CurrentView = TransactionHistoryVM;
                WindowNameText = "TRANSACTION HISTORY";
            });

        }

    }
}
