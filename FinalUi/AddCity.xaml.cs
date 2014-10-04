using System;
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
	/// Interaction logic for AddCity.xaml
	/// </summary>
	public partial class AddCity : Window
	{
		public AddCity()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
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