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
        bool isupdate = false;
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
            isupdate = true;
            BillingDataDataContext db = new BillingDataDataContext();
            this.city = db.Cities.Where(x => x.CITY_CODE == code).FirstOrDefault();
            if (city != null)
            {
                fillfield();
            }
            CityCodeBox.IsReadOnly = true;
            this.AddFilter.Data = Geometry.Parse(@"F1M2,12.942C2,12.942 10.226,15.241 10.226,15.241 10.226,15.241 8.275,17.071 8.275,17.071 9.288,17.922 10.917,18.786 12.32,18.786 15.074,18.786 17.386,16.824 18.039,14.171 18.039,14.171 21.987,15.222 21.987,15.222 20.891,19.693 16.996,23 12.357,23 9.771,23 7.076,21.618 5.308,19.934 5.308,19.934 3.454,21.671 3.454,21.671 3.454,21.671 2,12.942 2,12.942z M11.643,2C14.229,2 16.924,3.382 18.692,5.066 18.692,5.066 20.546,3.329 20.546,3.329 20.546,3.329 22,12.058 22,12.058 22,12.058 13.774,9.759 13.774,9.759 13.774,9.759 15.725,7.929 15.725,7.929 14.712,7.078 13.083,6.214 11.68,6.214 8.926,6.214 6.614,8.176 5.961,10.829 5.961,10.829 2.013,9.778 2.013,9.778 3.109,5.307 7.004,2 11.643,2z");
            this.Title = "Update City";
            this.Add_Filter.Text = "Update";
        }
        private void getfield()
        {
            this.city.CITY_CODE = this.CityCodeBox.Text;
            var s = stateList.SingleOrDefault(x => x.STATE_DESC == StateCombo.Text);
            if (s != null)
                this.city.CITY_STATE = s.STATE_CODE;
            var z = zoneList.SingleOrDefault(x => x.Zone_name == ZoneCombo.Text);
            if (z != null)
                this.city.ZONE = z.zcode;
            this.city.CITY_DESC = this.CityDscBox.Text;
        }
        private void fillfield()
        {
            CityCodeBox.Text = city.CITY_CODE;
            StateCombo.Text = city.CITY_STATE;
            ZoneCombo.Text = city.ZONE;
            CityDscBox.Text = city.CITY_DESC;
        }
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
            bool isdone = false;
            getfield();
            BillingDataDataContext db = new BillingDataDataContext();
            City lcity = db.Cities.SingleOrDefault(x => x.CITY_CODE == this.city.CITY_CODE);
            if (isupdate)
            {
                 if (city == null)
                {
                    isupdate = false;
                }
                else
                {
                    lcity.CITY_DESC = this.city.CITY_DESC;
                    lcity.CITY_STATE = this.city.CITY_STATE;
                    lcity.ZONE = this.city.ZONE;
                    
                }
            }
            if (!isupdate)
            {
                city.Status = 'A';
                db.Cities.InsertOnSubmit(this.city);
            }
            try
            {
                
                db.SubmitChanges();
                isdone = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); isdone = false; return; }
            if (isdone)
            {
                MessageBox.Show("City Added");
                this.Close();
            }
        }
    }
}