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

namespace IntegrativeMidterm.userControl.General
{
    public partial class SearchBar : UserControl
    {
        private bool _hasInput = false;
        public SearchBar()
        {
            InitializeComponent();
            PlaceholderText = "";
            InputText = "";
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(SearchBar));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(SearchBar));

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            tbPlaceholder.Visibility = Visibility.Hidden;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length == 0)
                _hasInput = false;
            else
                _hasInput = true;
        }

        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_hasInput)
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }
}
