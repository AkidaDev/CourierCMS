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
        public AddClient()
        {
            InitializeComponent();
        }

        private void ClientDetailSubmit_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
           // client.Name = ClientName.Text;
           // client.Id = Guid.NewGuid();
            //client.Address = ClientAddress.Text;
            //client.PhoneNo = decimal.Parse(ClientPhoneNo.Text);
           // client.EmailAddress = CLientEmailAddress.Text;
           // client.Code = ClientCode.Text;
            BillingDataDataContext db = new BillingDataDataContext();
            db.Clients.InsertOnSubmit(client);
            db.SubmitChanges();
            this.Close();
        }
    }
}
