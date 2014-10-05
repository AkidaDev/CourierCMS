﻿using System;
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
    /// Interaction logic for AddClient.xaml
    /// </summary>
    public partial class AddClient : Window
    {
        Client client;
        bool isupdate = false;
        public AddClient()
        {
            InitializeComponent();
            client = new Client();
        }
        public AddClient(Client client)
            : this()
        {
            this.client = client;
            FillField();
            isupdate = true; 
            this.AddFilter.Data = Geometry.Parse(@"F1M2,12.942C2,12.942 10.226,15.241 10.226,15.241 10.226,15.241 8.275,17.071 8.275,17.071 9.288,17.922 10.917,18.786 12.32,18.786 15.074,18.786 17.386,16.824 18.039,14.171 18.039,14.171 21.987,15.222 21.987,15.222 20.891,19.693 16.996,23 12.357,23 9.771,23 7.076,21.618 5.308,19.934 5.308,19.934 3.454,21.671 3.454,21.671 3.454,21.671 2,12.942 2,12.942z M11.643,2C14.229,2 16.924,3.382 18.692,5.066 18.692,5.066 20.546,3.329 20.546,3.329 20.546,3.329 22,12.058 22,12.058 22,12.058 13.774,9.759 13.774,9.759 13.774,9.759 15.725,7.929 15.725,7.929 14.712,7.078 13.083,6.214 11.68,6.214 8.926,6.214 6.614,8.176 5.961,10.829 5.961,10.829 2.013,9.778 2.013,9.778 3.109,5.307 7.004,2 11.643,2z");
            this.AddUpdateTitle.Text = "Update Client";
            this.Add_Filter.Text = "Update";
        }

        private void ClientDetailSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (isupdate) update(); else add();
        }
        public void add()
        {
            client.CLNAME = ClientName.Text;
            client.ADDRESS = ClientAddress.Text;
            client.CONTACTNO = ClientPhoneNo.Text;
            client.EMAILID = CLientEmailAddress.Text;
            client.CLCODE = ClientCode.Text;
            client.FUEL = float.Parse(ClientFuel.Text);
            client.INTRODATE = DateTime.Now;
            BillingDataDataContext db = new BillingDataDataContext();
            db.Clients.InsertOnSubmit(client);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            if (MessageBox.Show("Do you Want Assgin Rate to this client right now ?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                RateWindow window = new RateWindow();
                window.ShowDialog();
            }
            this.Close();
        }
        public void update()
        {
            var db = new BillingDataDataContext();
            var data = db.Clients.Single(x => x.CLCODE == client.CLCODE);
            data.CLCODE = ClientCode.Text;
            data.CLNAME = ClientName.Text;
            data.ADDRESS = ClientAddress.Text;
            data.EMAILID = CLientEmailAddress.Text;
            data.CONTACTNO = ClientPhoneNo.Text;
            data.FUEL = double.Parse(ClientFuel.Text);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }
        public void FillField()
        {
            this.ClientCode.Text = client.CLCODE;
            this.ClientName.Text = client.CLNAME;
            this.ClientAddress.Text = client.ADDRESS;
            this.ClientPhoneNo.Text = client.CONTACTNO;
            this.CLientEmailAddress.Text = client.EMAILID;
            this.ClientFuel.Text = client.FUEL.ToString();
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
