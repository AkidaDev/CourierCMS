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
            
            TextBox t = (TextBox) sender;
            BillingDataDataContext db =new BillingDataDataContext();
            cities = (from c in db.Cities select c).Where(x => x.CITY_DESC.StartsWith(t.Text)).ToList();
            cityView.Source = cities;
            this.CityListBox.Items.Refresh();
        }
    }
}
