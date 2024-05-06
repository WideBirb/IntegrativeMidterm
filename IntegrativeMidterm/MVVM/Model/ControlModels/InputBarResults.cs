using IntegrativeMidterm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.Model.ControlModels
{
    internal class InputBarResults : ViewModelBase
    {
        private int _id;
        private string _description;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }
    }
}
