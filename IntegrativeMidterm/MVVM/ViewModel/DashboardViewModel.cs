using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Xml.Linq;
using IntegrativeMidterm.Core;
using LiveCharts;
using LiveCharts.Wpf;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class DashboardViewModel: ViewModelBase
    {
        public SeriesCollection seriesCollection  { get; set; }
		public string[] Labels { get; set; }
		public Func<double, string> YFormatter { get; set; }
		public string Title { get; }

        public ChartValues<double> SupplyAmountSold  { get; set; }
		public ChartValues<double> SupplySales { get; set; }
		public ChartValues<double> PetSales { get; set; }

		private float lifetimeProfit { get; set; }
		public float LifetimeProfit
		{
			get { return lifetimeProfit; }
			set
			{
				lifetimeProfit = value;
				OnPropertyChanged();
			}
		}

		private double recentSaleCost { get; set; }
		public double RecentSaleCost
		{
			get { return recentSaleCost; }
			set
			{
				recentSaleCost = value;
				OnPropertyChanged();
			}
		}

		private string recentSaleDate { get; set; }
		public string RecentSaleDate
		{
			get { return recentSaleDate; }
			set
			{
				recentSaleDate = value;
				OnPropertyChanged();
			}
		}


		public ChartValues<double> calculateSupplyWeekStatistics  (Func<spGetAllPetSupplyTransactionsResult, double> propertySelector)
		{
			ChartValues<double> WeekList = new ChartValues<double>();
			double totalcost = 0.0;
			for (int days = 7; days > 0; days--)
			{
				totalcost = 0.0;
				ISingleResult<spGetAllPetSupplyTransactionsResult> day =
					PetshopDB.spGetAllPetSupplyTransactions(DateTime.Now.AddDays(-days), DateTime.Now.AddDays(-days + 1));


				foreach (var item in day)
				{
					if (item.Total_Cost == null || item.Quantity_Sold == null)
						continue;
					else
						totalcost += propertySelector(item);
				}
				if (totalcost > 10000)
					totalcost /= 1000;
				WeekList.Add(totalcost);

			}
			return WeekList;
		}

		public ChartValues<double> calculatePetWeekStatistics(Func<spGetAllPetTransactionsResult, double> propertySelector)
		{
			ChartValues<double> WeekList = new ChartValues<double>();
			double totalcost = 0.0;
			for (int days = 7; days > 0; days--)
			{
				totalcost = 0.0;
				ISingleResult<spGetAllPetTransactionsResult> day =
					PetshopDB.spGetAllPetTransactions(DateTime.Now.AddDays(-days), DateTime.Now.AddDays(-days + 1));

				foreach (var item in day)
				{
					if (item.Cost.ToString() == null )
						continue;
					else
						totalcost += propertySelector(item);
				}
				if (totalcost > 10000)
					totalcost /= 1000;
				WeekList.Add(totalcost);

			}
			return WeekList;
		}

		public DashboardViewModel() 
        {

			//Lifetime Profit
			ISingleResult<spGetAllTransactionsResult> allTransactions = PetshopDB.spGetAllTransactions(null, null);
            foreach (var transaction in allTransactions)
			{
				if (transaction.Total_Cost == null)
					continue;
				LifetimeProfit += (float)transaction.Total_Cost;
            }
			spGetAllTransactionsResult recentTransaction = PetshopDB.spGetAllTransactions(null, null).Last();
			if (recentTransaction.Total_Cost != null)
				RecentSaleCost = (double)recentTransaction.Total_Cost;

            RecentSaleDate = recentTransaction.Process_Date.ToString();

            seriesCollection = new SeriesCollection();
			SupplyAmountSold = calculateSupplyWeekStatistics(item => (double)item.Quantity_Sold); ; // new ChartValues<double> { 63, 43, 52, 21, 45, 51, 75 };
			SupplySales = calculateSupplyWeekStatistics(item => (double)item.Total_Cost);
			PetSales = calculatePetWeekStatistics(item => (double)item.Cost);

			// Loop to populate the array with formatted days
			Labels = new string[7];
			for (int i = 0; i < 7; i++)
			{
				// Get the current day's name and format it to three letters
				DateTime currentDateTime = DateTime.Now.AddDays(-i);
				Labels[6-i] = currentDateTime.Month.ToString() + "/" + currentDateTime.Day.ToString();
			};

		}

    }
}
