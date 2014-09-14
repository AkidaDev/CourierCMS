using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ManageClient.xaml
    /// </summary>
    public partial class ManageClient : Window
    {
        List<Client> clients;
        CollectionViewSource viewsource;
        List<Client> clientToEdit;
        public ManageClient()
        {
            InitializeComponent();
            clientToEdit = new List<Client>();
            BillingDataDataContext db = new BillingDataDataContext();
            clients = (from client in db.Clients
                       select client).ToList();
            viewsource = (CollectionViewSource)FindResource("ClienTable");
            viewsource.Source = clients;

        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient window = new AddClient();
            window.Show();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                clientToEdit.Add((Client)e.Row.Item);
            }
        }


        private void update_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            
            foreach (var client in clientToEdit)
            {
               var data = db.Clients.Single(x => x.Id == client.Id);
               data.Name = client.Name;
               data.PhoneNo = client.PhoneNo;
               data.EmailAddress = client.EmailAddress;
               data.Address = data.Address;
               db.SubmitChanges();     
            }
         }

    }
}
