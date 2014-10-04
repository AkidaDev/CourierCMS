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
    /// Interaction logic for AddCity.xaml
    /// </summary>
    public partial class AddCity : Window
    {
        City city;
        List<State> stateList;
        List<Country> countryList;
        List<City> cityList;
        List<ZONE> zoneList;

        CollectionViewSource stateview;
        CollectionViewSource countryview;
        CollectionViewSource cityview;
        CollectionViewSource zoneview;
        public AddCity()
        {
            this.InitializeComponent();
            city = new City();
            BillingDataDataContext db = new BillingDataDataContext();
            // get all data
            zoneList = db.ZONEs.ToList();
            cityList = db.Cities.ToList();
            stateList = db.States.ToList();
            countryList = db.Countries.ToList();
            //find all resource
            zoneview = (CollectionViewSource)FindResource("ZoneList");
            cityview = (CollectionViewSource)FindResource("CitiesList");
            stateview = (CollectionViewSource)FindResource("StateList");
            countryview = (CollectionViewSource)FindResource("CountryList");

            zoneview.Source = zoneList;
            cityview.Source = cityList;
            stateview.Source = stateList;
            countryview.Source = countryList;
        }
        public AddCity(string code)
            : this()
        {
            this.city.CITY_CODE = code;
        }
        private void getfield()
        {
            this.city.CITY_CODE = this.CityCodeBox.Text;
            var s = (State)this.StateCombo.SelectedItem;
            this.city.CITY_STATE = s.STATE_CODE;
            var z = (ZONE)this.ZoneCombo.SelectedItem;
            this.city.ZONE = z.zcode;
            this.city.CITY_DESC = this.CityDscBox.Text;
        }
        private void fillfield()
        { }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddUpdate_Click(object sender, RoutedEventArgs e)
        {
            getfield();
            BillingDataDataContext db = new BillingDataDataContext();
            db.Cities.InsertOnSubmit(this.city);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }
    }
}