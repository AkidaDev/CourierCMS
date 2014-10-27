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
            clients = (from client in db.Clients
                       select client).ToList();
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
            var clients = (from client in db.Clients
                       select client).ToList();
            viewsource = (CollectionViewSource)FindResource("ClienTable");
            viewsource.Source = clients;
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void showReport_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}