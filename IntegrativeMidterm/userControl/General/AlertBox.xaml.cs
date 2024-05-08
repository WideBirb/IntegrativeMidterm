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
using System.Windows.Shapes;

namespace IntegrativeMidterm.userControl.General
{
	/// <summary>
	/// Interaction logic for AlertBox.xaml
	/// </summary>
	public partial class AlertBox : Window
	{
		public AlertBox(string Message)
		{
			InitializeComponent();
			MessageTextBlock.Text = Message;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
