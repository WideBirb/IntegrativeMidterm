﻿using System;
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
	/// Interaction logic for ConfirmationBox.xaml
	/// </summary>
	public partial class ConfirmationBox : Window
	{
		public MessageBoxResult Result { get; private set; }
		public ConfirmationBox(string Message)
		{
			InitializeComponent();
			DataContext = this;
			MessageTextBlock.Text = Message;

		}

		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Yes;
			DialogResult = true;
			Close();
		}

		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.No;
			DialogResult = false;
			Close();
		}

	}
}
