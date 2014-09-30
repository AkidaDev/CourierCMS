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
            this.ClientDetailAddUpdate.Content = "Update";
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
            db.SubmitChanges();
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

                this.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

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
    }
}
