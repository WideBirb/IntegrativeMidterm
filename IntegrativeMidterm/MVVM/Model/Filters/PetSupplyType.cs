using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeMidterm.MVVM.Model.Filters
{
    internal class PetSupplyType
    {
        public int ID { get; set; }
        public string Description { get; set; }

		public static implicit operator PetSupplyType(PetSupply v)
		{
			throw new NotImplementedException();
		}
	}
}
