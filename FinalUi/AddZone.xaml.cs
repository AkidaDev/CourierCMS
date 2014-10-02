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
    /// Interaction logic for AddZone.xaml
    /// </summary>
    public partial class AddZone : Window
    {
        public ZONE zone;
        public AddZone()
        {
            InitializeComponent();
        }

        private void AddZoneButton_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            db.Log = Console.Out;
            ZONE z = new ZONE();
            bool a;
            z.Zone_name = this.ZoneNameTextBox.Text;
            z.zcode = this.Zonecodebox.Text;
            db.ZONEs.InsertOnSubmit(z);
            try
            {
                db.SubmitChanges();
                a = true;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                a = false;
            }
            if (!a) MessageBox.Show("error error");
            else { this.Close(); }
        }
    }
}