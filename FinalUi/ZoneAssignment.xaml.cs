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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ZoneAssignment : Window
    {
        CollectionViewSource ZoneTableSource;
        public ZoneAssignment()
		{
			this.InitializeComponent();
            ZoneTableSource = (CollectionViewSource)FindResource("zoneTable");
            BillingDataDataContext db = new BillingDataDataContext();
            ZoneTableSource.Source = db.ZONEs;
			
		}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddZone window = new AddZone();
            window.Closed += window_Closed;
            window.ShowDialog();
        }

        void window_Closed(object sender, EventArgs e)
        {
            ZoneDataReload();
        }

        private void UpdateZoneButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Updating zone is not allowed as of now.");
            return;
            /*
            if(ZoneDataGrid.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Select a zone to update.");
                return;
            }
            AddZone addZoneWindow = new AddZone((ZONE)ZoneDataGrid.SelectedItem);
            addZoneWindow.Closed +=window_Closed;
            addZoneWindow.ShowDialog();*/
        }

        private void DeleteZoneButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deleting zone is not allowed as of now");
        }
        private void ZoneDataReload()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            ZoneTableSource.Source = db.ZONEs;
			
        }
    }
}