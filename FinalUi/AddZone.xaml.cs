﻿using System;
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
        public AddZone(string code)
            : this()
        {

            Zonecodebox.Text = code;
            Zonecodebox.IsReadOnly = true;
            this.AddFilter.Data = Geometry.Parse(@"F1M2,12.942C2,12.942 10.226,15.241 10.226,15.241 10.226,15.241 8.275,17.071 8.275,17.071 9.288,17.922 10.917,18.786 12.32,18.786 15.074,18.786 17.386,16.824 18.039,14.171 18.039,14.171 21.987,15.222 21.987,15.222 20.891,19.693 16.996,23 12.357,23 9.771,23 7.076,21.618 5.308,19.934 5.308,19.934 3.454,21.671 3.454,21.671 3.454,21.671 2,12.942 2,12.942z M11.643,2C14.229,2 16.924,3.382 18.692,5.066 18.692,5.066 20.546,3.329 20.546,3.329 20.546,3.329 22,12.058 22,12.058 22,12.058 13.774,9.759 13.774,9.759 13.774,9.759 15.725,7.929 15.725,7.929 14.712,7.078 13.083,6.214 11.68,6.214 8.926,6.214 6.614,8.176 5.961,10.829 5.961,10.829 2.013,9.778 2.013,9.778 3.109,5.307 7.004,2 11.643,2z");
            this.AddFilter.Height = 18;
            this.AddFilter.Width = 18;
            this.Title = "Update Zone";
            this.Add_Filter.Text = " Update";
        }
        private void AddZoneButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = "";
            BillingDataDataContext db = new BillingDataDataContext();
            
            if(Zonecodebox.Text == "")
            {
                errorMsg = errorMsg + "Please enter a proper zone code. \n";
            }
            else
            {
                List<ZONE> zones = db.ZONEs.Where(x => x.zcode == Zonecodebox.Text).ToList();
                if (zones.Count > 0)
                    errorMsg = errorMsg + "A zone with this code already exists. \n";
            }
            if(ZoneNameTextBox.Text == "")
            {
                errorMsg = errorMsg + "Please enter a proper zone name. \n";
            }
            if(errorMsg != "")
            {
                MessageBox.Show("Please correct the following errors: \n" + errorMsg);
                return;
            }
            ZONE z = new ZONE();
          
            z.Id = Guid.NewGuid();
            z.Mode = 'A';
            z.Zone_name = this.ZoneNameTextBox.Text;
            z.zcode = this.Zonecodebox.Text;
            db.ZONEs.InsertOnSubmit(z);
            try
            {
                db.SubmitChanges();
               
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
               
            }
            this.Close();
        }
    }
}