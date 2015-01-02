using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ManageCity.xaml
    /// </summary>
    public partial class ManageCity : Window
    {
        CollectionViewSource cityViewSource;
        BillingDataDataContext db = new BillingDataDataContext();
        public ManageCity()
        {
            InitializeComponent();
            cityViewSource = (CollectionViewSource)FindResource("CityTable");
            cityViewSource.Source = DataSources.CityCopy;
        }
        private void ReloadCityButton_Click(object sender, RoutedEventArgs e)
        {
            DataSources.refreshCityList();
            cityViewSource.Source = DataSources.CityCopy;
            CityDataGrid.Items.Refresh();
        }
        void addCityWin_Closed(object sender, EventArgs e)
        {
            ReloadCityButton_Click(null, null);
        }
        private void EditCityButton_Click(object sender, RoutedEventArgs e)
        {
            if (CityDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select 1 city to edit");
                return;
            }
            AddCity addCityWin = new AddCity(((City)CityDataGrid.SelectedItem).CITY_CODE);
            addCityWin.Closed += addCityWin_Closed;
            addCityWin.ShowDialog();
        }
        private void AddCityButton_Click(object sender, RoutedEventArgs e)
        {
            AddCity addCityWin = new AddCity();
            addCityWin.Closed += addCityWin_Closed;
            addCityWin.ShowDialog();
        }
        private void DeleteCityButton_Click(object sender, RoutedEventArgs e)
        {
            if (CityDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select 1 city to delete");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this city?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (db == null)
                    db = new BillingDataDataContext();
                City city = db.Cities.SingleOrDefault(x => x.CITY_CODE == ((City)CityDataGrid.SelectedItem).CITY_CODE);
                if (city == null)
                {
                    MessageBox.Show("No such city exists.");
                    return;
                }
                db.Cities.DeleteOnSubmit(city);
                db.SubmitChanges();
            }
            ReloadCityButton_Click(null, null);
        }
    }
}
