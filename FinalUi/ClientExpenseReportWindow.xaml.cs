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
    /// Interaction logic for ClientExpenseReportWindow.xaml
    /// </summary>
    public partial class ClientExpenseReportWindow : Window
    {
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        CollectionViewSource ClientListSource;
        List<ClientReportView> invoice;
        public ClientExpenseReportWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            List<Client> clientList = DataSources.ClientCopy;
            ClientListSource.Source = clientList.OrderBy(x => x.NameAndCode).ToList();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "ClientDataSet";
            AccountStatementViewer.LocalReport.ReportPath = "ClientAnalysisReport.rdlc";
        }

        private void ClientListCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            {
                MessageBox.Show("Please select from date and to date correctly..");
                return;
            }
            Client selectedClient = ClientListCombo.SelectedItem as Client;
            BillingDataDataContext db = new BillingDataDataContext();
            var source = db.ClientReportViews.Where(x=>x.BookingDate >= FromDate.SelectedDate && x.BookingDate <= ToDate.SelectedDate);
            if (ShowSelectedClientCheck.IsChecked == true)
                source = source.Where(x => x.CustCode == selectedClient.CLCODE);
            List<ClientReportView> reportSource = source.ToList();
            rs.Value = reportSource;
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            AccountStatementViewer.ShowExportButton = true;
            AccountStatementViewer.RefreshReport();
        }
    }
}
