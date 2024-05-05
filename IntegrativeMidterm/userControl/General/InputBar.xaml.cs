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
    public partial class InputBar : UserControl
    {
        private bool _hasInput = false;
        public InputBar()
        {
            InitializeComponent();
            PlaceholderText = string.Empty;
            InputText = string.Empty;
            EntryLabel = string.Empty;
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(InputBar));

        public static readonly DependencyProperty EntryLabelProperty =
            DependencyProperty.Register("EntryLabel", typeof(string), typeof(InputBar));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(InputBar));

        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.Register("TextChangedCommand", typeof(ICommand), typeof(InputBar));

        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        public string EntryLabel
        {
            get { return (string)GetValue(EntryLabelProperty); }
            set { SetValue(EntryLabelProperty, value); }
        }
        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        private void InputBar_GotFocus(object sender, RoutedEventArgs e)
        {
            tbPlaceholder.Visibility = Visibility.Hidden;
        }

        private void InputBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChangedCommand?.Execute(((TextBox)sender).Text);

            if (((TextBox)sender).Text.Length == 0)
                _hasInput = false;
            else
                _hasInput = true;
        }
        
        private void InputBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_hasInput)
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }
}
