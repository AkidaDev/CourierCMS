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
using Microsoft.Reporting.Common;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for TestingReporting.xaml
    /// </summary>
    public partial class AccountStatementReportingWindow : Window
    {
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        CollectionViewSource ClientListSource;
        List<AccountStatement> invoice;
        public AccountStatementReportingWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            ClientListSource.Source = db.Clients.ToList();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "InvoiceDataSet1";
            AccountStatementViewer.LocalReport.ReportPath = "AccountStatementReport.rdlc";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            CreateObj();
        }

        private void CreateObj()
        {

            BillingDataDataContext db = new BillingDataDataContext();
            var c  =  (Client) this.ClientListCombo.SelectedItem;
            invoice = db.AccountStatements.Where(x=> x.ClientCode == c.CLCODE).OrderBy(y => y.TransactionDate).ToList();
            rs.Value = invoice;
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            List<ReportParameter> repParams = new List<ReportParameter>();
            repParams.Add(new ReportParameter("CompanyName", Configs.Default.CompanyName));
            repParams.Add(new ReportParameter("ComapnyPhoneNo", Configs.Default.CompanyPhone));
            repParams.Add(new ReportParameter("CompanyAddress", Configs.Default.CompanyAddress));
            repParams.Add(new ReportParameter("CompanyEmail", Configs.Default.CompanyEmail));
            repParams.Add(new ReportParameter("CompanyFax", Configs.Default.CompanyFax));
            double? billedamountsum =  this.invoice.Select(y => y.PayAmount).Sum();
            double? amountRecivedsum = this.invoice.Select(y => y.TotalRecievedAmount).Sum();
            double? TotalSum = billedamountsum - amountRecivedsum;
            string sumS = String.Format("{0:F2}", TotalSum??0);
            repParams.Add(new ReportParameter("TotalSum", sumS));
            repParams.Add(new ReportParameter("ClientName", c.CLNAME));
            repParams.Add(new ReportParameter("ClientAddress", c.ADDRESS));
            repParams.Add(new ReportParameter("ClientPhoneNo", c.CONTACTNO));
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
