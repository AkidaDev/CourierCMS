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
    /// Interaction logic for FilterSelectWindow.xaml
    /// </summary>
    public partial class FilterSelectWindow : Window
    {
        CollectionViewSource clientListToSet;
        CollectionViewSource clientListToAdd;
        CollectionViewSource StartConnNoList;
        CollectionViewSource EndConnoList;
        Filter filterObj;

        public List<Func<RuntimeData, int, bool>> filters;
        /// <summary>
        /// Creates a new Filter Selection Window
        /// </summary>
        /// <param name="filterObj">Filter Object of current context</param>
        /// <param name="ConnNo">Connsignment Number List of current object</param>
        public FilterSelectWindow(Filter filterObj, IEnumerable<string> ConnNo)
        {
            setDataSource(filterObj, ConnNo);
            //  List<WrapPanel> panels = (List<WrapPanel>)SelectClient.ItemsSource;
        }
        public void setDataSource(Filter filterObj, IEnumerable<string> ConnNo)
        {
            InitializeComponent();
            this.filterObj = filterObj;
            BillingDataDataContext db = new BillingDataDataContext();

            StartConnNoList = (CollectionViewSource)FindResource("StartConnNoList");
            StartConnNoList.Source = ConnNo;
            EndConnoList = (CollectionViewSource)FindResource("EndConnoList");
            EndConnoList.Source = ConnNo;
            clientListToAdd = (CollectionViewSource)FindResource("ClientToAdd");
            clientListToSet = (CollectionViewSource)FindResource("ClientToSet");
            List<Client> fClientList = db.Clients.ToList();
            clientListToSet.Source = fClientList.Where(x => !filterObj.selectedClientList.Select(y => y.CLCODE).Contains(x.CLCODE)).ToList();
            clientListToAdd.Source = filterObj.selectedClientList;
            switch (filterObj.showBilled)
            {
                case true:
                    {
                        BilledRadio.IsChecked = true;
                        break;
                    }
                case false:
                    {
                        UnBilledRadio.IsChecked = true;
                        break;
                    }
                case null:
                    {
                        AllRadio.IsChecked = true;
                        break;
                    }

            }
            if (filterObj.startConnNo != "" && ConnNo.Contains(filterObj.startConnNo))
                StartConnNo.Text = filterObj.startConnNo;
            else
                StartConnNo.Text = ConnNo.FirstOrDefault() ?? "";
            if (filterObj.endConnNo != "" && ConnNo.Contains(filterObj.endConnNo))
                EndConnNo.Text = filterObj.endConnNo;
            else
                EndConnNo.Text = ConnNo.LastOrDefault() ?? "";
            ToDate.SelectedDate = filterObj.toDate;
            FromDate.SelectedDate = filterObj.fromDate;
        }

        private void GetFilter_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = "";
            if (!((List<string>)EndConnoList.Source).Contains(EndConnNo.Text) && !((List<string>)StartConnNoList.Source).Contains(StartConnNo.Text))
                errorMsg += "Enter the connsignments correctly \n";
            if (ToDate.SelectedDate < FromDate.SelectedDate)
                errorMsg += "Enter the date correctly \n";
            if (errorMsg != "")
                MessageBox.Show("Please correct the following errors: \n" + errorMsg);
            else
            {
                filterObj.endConnNo = EndConnNo.Text;
                filterObj.fromDate = (DateTime)FromDate.SelectedDate;
                filterObj.selectedClientList = ((List<Client>)clientListToAdd.Source);
                if (BilledRadio.IsChecked == true)
                    filterObj.showBilled = true;
                if (UnBilledRadio.IsChecked == true)
                    filterObj.showBilled = false;
                if (AllRadio.IsChecked == true)
                    filterObj.showBilled = null;
                filterObj.startConnNo = StartConnNo.Text;
                filterObj.toDate = (DateTime)ToDate.SelectedDate;

            }
            this.Close();
        }


        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        private void RemoveClient_Click(object sender, RoutedEventArgs e)
        {

            if (this.ClientsToAdd.SelectedItem != null)
            {
                foreach (Client item in ClientsToAdd.SelectedItems)
                {
                    ((List<Client>)clientListToAdd.Source).Remove(item);
                    ((List<Client>)clientListToSet.Source).Add(item);
                }
                ClientsToSet.SelectedItem = null;
                ClientsToAdd.SelectedItem = null;
                ClientsToSet.Items.Refresh();
                ClientsToAdd.Items.Refresh();
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            if (this.ClientsToSet.SelectedItem != null)
            {
                foreach (Client item in ClientsToSet.SelectedItems)
                {
                    ((List<Client>)clientListToSet.Source).Remove(item);
                    ((List<Client>)clientListToAdd.Source).Add(item);
                }
                ClientsToSet.SelectedItem = null;
                ClientsToAdd.SelectedItem = null;
                ClientsToSet.Items.Refresh();
                ClientsToAdd.Items.Refresh();
            }
        }

        private void AddAllButton_Click(object sender, RoutedEventArgs e)
        {
            List<Client> clients = new List<Client>();
            ((List<Client>)clientListToSet.Source).RemoveAll(
                (x) =>
                {
                    clients.Add(x);
                    return true;
                }
            );
            ((List<Client>)clientListToAdd.Source).AddRange(clients);
            ClientsToSet.Items.Refresh();
            ClientsToAdd.Items.Refresh();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            List<Client> clients = new List<Client>();
            ((List<Client>)clientListToAdd.Source).RemoveAll(
                (x) =>
                {
                    clients.Add(x);
                    return true;
                }
            );
            ((List<Client>)clientListToSet.Source).AddRange(clients);
            ClientsToSet.Items.Refresh();
            ClientsToAdd.Items.Refresh();

        }

    }
}
