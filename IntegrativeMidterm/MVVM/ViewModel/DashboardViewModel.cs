using System;
using System.Collections.Generic;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace IntegrativeMidterm.MVVM.ViewModel
{
    internal class DashboardViewModel
    {
        public SeriesCollection seriesCollection  { get; set; }
		public string[] Labels { get; set; }
		public Func<double, string> YFormatter { get; set; }
		public string Title { get; }

        public ChartValues<double> Values1  { get; set; }
		public ChartValues<double> Values2 { get; set; }

		public ChartValues<double> Values3 { get; set; }
		public DashboardViewModel() 
        {
			seriesCollection = new SeriesCollection();

			Values1 = new ChartValues<double> { 63, 43, 52, 21, 45, 51, 75 };
			Values2 = new ChartValues<double> { 73, 13, 22, 41, 95, 73, 92 };
			Values3 = new ChartValues<double> { 23, 83, 62, 41, 53, 36, 64 };

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
