using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.Core
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DataClassDataContext _petshopDB = null;
        public DataClassDataContext PetshopDB
        {
            get { return _petshopDB; }
            private set { _petshopDB = value; }
        }

        public ViewModelBase()
        {
            PetshopDB = new DataClassDataContext(Properties.Settings.Default.PetShopConnectionString);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
