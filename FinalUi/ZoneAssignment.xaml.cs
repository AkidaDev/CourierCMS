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
    /// Interaction logic for ZoneAssignment.xaml
    /// </summary>
    public partial class ZoneAssignment : Window
    {
        List<ZONE> zoneList;
        List<City> cityList;
        CollectionViewSource viewZone;
        CollectionViewSource viewCity;
        bool zonefound = false;
        public ZoneAssignment()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            
            viewZone = (CollectionViewSource)FindResource("zoneTable");
            zoneList = db.ZONEs.ToList();
            viewZone.Source = zoneList;

            viewCity = (CollectionViewSource)FindResource("CityTable");
            cityList = db.Cities.ToList();
            viewCity.Source = cityList;
            this.ZoneCombo.SelectedItem = null;
            getAllCity();
        }
        private void Assign_Zone_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            var sc = (City)this.CityCombo.SelectedItem;
            City c = db.Cities.Where(x => x.CITY_CODE == sc.CITY_CODE).First();
            ZONE z = (ZONE)this.ZoneCombo.SelectedItem;
            c.ZONE = z.zcode;
            try {
                db.SubmitChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }
        void getAllCity()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if (this.ZoneCombo.SelectedItem != null)
            {
                ZONE z = (ZONE)this.ZoneCombo.SelectedItem;
                this.cityList = db.Cities.Where(x => x.ZONE == z.zcode).ToList();
            }
            else { this.cityList = null; }
            this.CityCombo.Items.Refresh();
        }
        private void addzone_close(object sender, EventArgs e)
        {
            AddZone w = (AddZone)sender;
            BillingDataDataContext db =new BillingDataDataContext();
            this.zoneList = db.ZONEs.ToList();
            this.ZoneCombo.Items.Refresh();
            this.ZoneCombo.SelectedItem = w.zone;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddZone window = new AddZone(); window.ShowDialog();
            window.Closed += addzone_close;
        }
        private void ZoneCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getAllCity();
        }

    }
}
