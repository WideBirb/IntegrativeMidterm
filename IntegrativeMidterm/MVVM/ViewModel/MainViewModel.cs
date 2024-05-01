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
        public RelayCommand PetCheckOutViewCommand { get; set; }
        public RelayCommand PetInventoryViewCommand { get; set; }
        public RelayCommand PetProfileViewCommand { get; set; }
        

        public DashboardViewModel DashboardVM { get; set; }
        public CheckOutViewModel CheckOutVM { get; set; }
        public SuppliesInventoryViewModel SuppliesInventoryVM { get; set; }
        public TransactionHistoryViewModel TransactionHistoryVM { get; set; }
        public PetCheckOutViewModel PetCheckOutVM { get; set; }
        public PetInventoryViewModel PetInventoryVM { get; set; }
        public PetProfileViewModel PetProfileVM { get; set; }


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

            PetCheckOutVM = new PetCheckOutViewModel();
            PetInventoryVM = new PetInventoryViewModel();
            PetProfileVM = new PetProfileViewModel();

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

            PetCheckOutViewCommand = new RelayCommand(o =>
            {
                CurrentView = PetCheckOutVM;
                WindowNameText = "PET SHOP";
            });
            PetInventoryViewCommand = new RelayCommand(o =>
            {
                CurrentView = PetInventoryVM;
                WindowNameText = "PET INVENTORY";
            });
            PetProfileViewCommand = new RelayCommand(o =>
            {
                CurrentView = PetProfileVM;
                WindowNameText = "PET PROFILE";
            });
        }

    }
}
