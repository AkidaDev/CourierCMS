﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalUi
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class ZoneAssignment : Window
	{
		public ZoneAssignment()
		{
			this.InitializeComponent();
			
		}
		private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddZone window = new AddZone(); window.ShowDialog();
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
	}
}