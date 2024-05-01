using IntegrativeMidterm.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class PetInventoryViewModel
    {
        public ObservableCollection<AvailabilityIndicatorData> AvailabilityIndicators { get; set; }

        public PetInventoryViewModel()
        {
            AvailabilityIndicators = new ObservableCollection<AvailabilityIndicatorData>
            {
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Green,
                    Description = "Available",
                    Count = 0
                },
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Red,
                    Description = "Reserved",
                    Count = 0
                },
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Yellow,
                    Description = "Adopted",
                    Count = 0
                },
                new AvailabilityIndicatorData
                {
                    IconColor = Brushes.Black,
                    Description = "Deceased",
                    Count = 0
                }
            };
        }
    }

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
