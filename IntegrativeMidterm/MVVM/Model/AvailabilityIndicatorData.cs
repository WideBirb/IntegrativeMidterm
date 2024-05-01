using IntegrativeMidterm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class AvailabilityIndicatorData : ViewModelBase
    {
        private SolidColorBrush _iconColor;
        private string _description;
        private int _count;

        public SolidColorBrush IconColor
        {
            get { return _iconColor; }
            set { _iconColor = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        public int Count
        {
            get { return _count; }
            set
            { _count = value; OnPropertyChanged(); }
        }
    }
}
