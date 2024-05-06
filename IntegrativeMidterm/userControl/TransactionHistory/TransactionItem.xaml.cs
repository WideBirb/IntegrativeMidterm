using IntegrativeMidterm.userControl.PetInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrativeMidterm.userControl.TransactionHistory
{
    /// <summary>
    /// Interaction logic for TransactionItem.xaml
    /// </summary>
    public partial class TransactionItem : UserControl
    {
        public static readonly DependencyProperty SelectedCommandProperty =
            DependencyProperty.Register("SelectedCommand", typeof(ICommand), typeof(TransactionItem));

        public ICommand SelectedCommand
        {
            get { return (ICommand)GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }

        public TransactionItem()
        {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedCommand?.Execute(sender);
        }
    }
}
