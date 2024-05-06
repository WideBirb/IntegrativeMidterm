using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IntegrativeMidterm.MVVM.Model
{
    internal class Pet
    {
        public int ID { get; set; }

        // FOR INPUT & OUTPUT IN ORDER
        public string PetName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public string Status { get; set; }
        public float Price { get; set; }
        // - Status

        // FOR OUTPUT
        public int StatusID { get; set; }
        public string Age { get; set; }
        public SolidColorBrush StatusColor { get; set; }

        // FOR INPUT ON ADDING TO TRANSACTIONS
        public int InTransactionHistoryID { get; set; }
        public int SpeciesID { get; set; }
        public int BreedID { get; set; }

        // OTHER OUTPUT
        public string ImagePath { get; set; }
    }
}
