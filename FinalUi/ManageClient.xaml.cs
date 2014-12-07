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
        Client client;
        BillingDataDataContext db;
        
        public ManageClient()
        {
            InitializeComponent();
            db = new BillingDataDataContext();
            clientToEdit = new List<Client>();
            clients = DataSources.ClientCopy;
            viewsource = (CollectionViewSource)FindResource("ClienTable");
            viewsource.Source = clients;
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient window = new AddClient();
            window.Closed += AddClient_close;
            window.Show();
        }

        private void updateClient_Click(object sender, RoutedEventArgs e)
        {
            if(this.mangaclientgrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select 1 client to update");
                return;
            }
           client = (Client)this.mangaclientgrid.SelectedItem;
            if(client == null)
            {
                MessageBox.Show("Please select a client to update");
            }
            AddClient add = new AddClient(client);
            add.Closed += AddClient_close;
            add.ShowDialog();
         }
        private void AddClient_close(object sender, EventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            var clients = DataSources.ClientCopy;
            viewsource = (CollectionViewSource)FindResource("ClienTable");
            viewsource.Source = clients;
        }
        private void showReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteClientGridButton_Click(object sender, RoutedEventArgs e)
        {
            if(mangaclientgrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select 1 client to delete","Error");
                return;
            }
            if(MessageBox.Show("Are you sure you want to delete this client?","Confirm",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
            {
                Client client = (Client)mangaclientgrid.SelectedItem;
                client = db.Clients.SingleOrDefault(x => x.CLCODE == client.CLCODE);
                if(client == null)
                {
                    MessageBox.Show("This client does not exists.", "Error");
                    return;
                }
                if(client.CLCODE == "<NONE>"|| client.CLCODE == "<DTDC>")
                {
                    MessageBox.Show("This client cannot be deleted");
                    return;
                }
                db.Clients.DeleteOnSubmit(client);
                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("Client deleted.", "Success");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Deletion failed with error : " + ex.Message,"Failure");
                }
                DataSources.refreshClientList();
                ReloadClientGridButton_Click(null, null);
            }

        }

        private void ReloadClientGridButton_Click(object sender, RoutedEventArgs e)
        {
            DataSources.refreshClientList();
            clients = DataSources.ClientCopy;
            viewsource = (CollectionViewSource)FindResource("ClienTable");
            viewsource.Source = clients;
 
        }
    }
}