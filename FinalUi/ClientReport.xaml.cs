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
using Microsoft.Reporting.Common;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for ClientReport.xaml
    /// </summary>
    public partial class ClientReport : Window
    {
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        CollectionViewSource ClientListSource;
        public ClientReport()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            ClientListSource.Source = db.Clients.ToList();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "ClientTariff";
            AccountStatementViewer.LocalReport.ReportPath = "ClientReport.rdlc";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            CreateObj();
        }

        private void CreateObj()
        {

            BillingDataDataContext db = new BillingDataDataContext();
            var c = (Client)this.ClientListCombo.SelectedItem;
            var source = db.Assignments.Where(x => x.ClientCode == c.CLCODE);
            rs.Value = source;
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            List<ReportParameter> repParams = new List<ReportParameter>();
            repParams.Add(new ReportParameter("ClientName", c.CLNAME ?? " "));
            repParams.Add(new ReportParameter("ClientAddress", c.ADDRESS ?? " " ));
            repParams.Add(new ReportParameter("ClientPhoneNo", c.CONTACTNO ?? ""));
            AccountStatementViewer.LocalReport.SetParameters(repParams);
            AccountStatementViewer.ShowExportButton = true;
            AccountStatementViewer.RefreshReport();
        }
        private void ClientListCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.RefreshReport();
        }
    }
}
