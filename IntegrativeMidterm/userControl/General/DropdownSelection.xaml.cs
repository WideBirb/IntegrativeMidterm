using IntegrativeMidterm.MVVM.Model.Filters;
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
    public partial class DropdownSelection : UserControl
    {
        public DropdownSelection()
        {
            InitializeComponent();
            PlaceholderText = string.Empty;
            EntryLabel = string.Empty;
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(DropdownSelection));
        
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(DropdownSelection));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(DropdownSelection));

        public static readonly DependencyProperty EntryLabelProperty =
            DependencyProperty.Register("EntryLabel", typeof(string), typeof(DropdownSelection));

        public static readonly DependencyProperty DropdownItemSourceProperty =
            DependencyProperty.Register("DropdownItemSource", typeof(object), typeof(DropdownSelection));

        public static readonly DependencyProperty SelectionChangedProperty =
            DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(DropdownSelection));

        public ICommand SelectionChangedCommand
        {
            get { return (ICommand)GetValue(SelectionChangedProperty); }
            set { SetValue(SelectionChangedProperty, value); }
        }

        public object DropdownItemSource
        {
            get { return GetValue(DropdownItemSourceProperty); }
            set { SetValue(DropdownItemSourceProperty, value); }
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
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedCommand?.Execute(((ComboBox)sender).SelectedItem);
        }
    }
}

