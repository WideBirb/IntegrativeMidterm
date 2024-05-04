using IntegrativeMidterm.Core;
using IntegrativeMidterm.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace IntegrativeMidterm.MVVM.View
{
	/// <summary>
	/// Interaction logic for SuppliesInventoryView.xaml
	/// </summary>
	/// 
	public partial class SuppliesInventoryView : UserControl
    {
		private DataClassDataContext _petshopDB = null;
		public DataClassDataContext PetshopDB
		{
			get { return _petshopDB; }
			private set { _petshopDB = value; }
		}
		public SuppliesInventoryView()
        {
			PetshopDB = new DataClassDataContext(Properties.Settings.Default.PetShopConnectionString);
			InitializeComponent();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            if (e.Text.Length == 0) { return; }
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            if (e.Text.Length == 0) { return; }
        }

		private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			
		}
    }
}
