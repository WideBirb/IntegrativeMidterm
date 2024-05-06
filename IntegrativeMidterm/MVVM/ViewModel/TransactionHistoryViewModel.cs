using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class TransactionHistoryViewModel : ViewModelBase
    {
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
    }
}
