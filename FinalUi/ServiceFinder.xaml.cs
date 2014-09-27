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

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for ServiceFinder.xaml
    /// </summary>
    public partial class ServiceFinder : Window
    {
        List<City> cities;
        CollectionViewSource cityView;
        public ServiceFinder()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            cityView = (CollectionViewSource)FindResource("CityList");
            cities = null;
            cityView.Source = cities;
        }
        private void City_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text != "" && t.Text != null)
            {
                this.CityListBox.SelectionChanged -= CityListBox_SelectionChanged;
                this.CityListBox.Visibility = Visibility.Visible;
                BillingDataDataContext db = new BillingDataDataContext();
                cities = (from c in db.Cities select c).Where(x => x.CITY_DESC.StartsWith(t.Text)).ToList();
                cities = cities.GroupBy(x => x.CITY_DESC).Select(y => y.First()).ToList();
                cityView.Source = cities;
                this.CityListBox.Height = cities.Count * 25;
                this.CityListBox.Items.Refresh();
                this.CityListBox.SelectedItem = null;
                this.CityListBox.SelectionChanged += CityListBox_SelectionChanged;
            }
            else { this.CityListBox.Visibility = Visibility.Hidden;}
        }
        private void CityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cityView.Source != null && this.City.Text != "")
            {
                ListBox l = (ListBox)sender;
                var c = (City)l.SelectedItem;
                status(c);
                this.City.Text = c.CITY_DESC;
                this.CityListBox.Visibility = Visibility.Collapsed;
            }
        }
        private void status(City c)
        {
            if (c != null)
            {
                this.StatusTextBlock.Visibility = Visibility.Visible;
                if (c.CITY_STATUS == "A")
                {
                    this.StatusTextBlock.Text = "Service Available";
                    this.StatusTextBlock.Foreground = Brushes.Blue;
                }
                else
                {
                    this.StatusTextBlock.Foreground = Brushes.Red;
                    this.StatusTextBlock.Text = "Service Unavailable";
                }
            }
        }

        private void CityListBox_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           
        }
    }
}
