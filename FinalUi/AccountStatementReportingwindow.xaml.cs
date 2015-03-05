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
            List<Client> clientList = DataSources.ClientCopy;
            ClientListSource.Source = clientList.OrderBy(x => x.NameAndCode).ToList();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "InvoiceDataSet1";
            AccountStatementViewer.LocalReport.ReportPath = "AccountStatementReport.rdlc";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateObj();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to generate statement. Error: " + ex.Message);
            }
        }

        private void CreateObj()
        {
            if(FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            {
                MessageBox.Show("Please select from date and to date correctly..");
                return;
            }
            BillingDataDataContext db = new BillingDataDataContext();
            var c  =  (Client) this.ClientListCombo.SelectedItem;
            invoice = db.AccountStatements.Where(x=> x.ClientCode == c.CLCODE && x.TransactionDate <= ToDate.SelectedDate && x.TransactionDate >= FromDate.SelectedDate).OrderBy(y => y.TransactionDate).ToList();
            AccountStatement carryOverDueRecord = new AccountStatement();
            carryOverDueRecord.Id = "";
            carryOverDueRecord.TypeOfRecord = "Carry";
            carryOverDueRecord.Remark = "PreviousDue/CarryOverDue";
            carryOverDueRecord.TransactionDate = FromDate.SelectedDate??DateTime.Today;
            carryOverDueRecord.PayAmount = db.CarryOverDue(FromDate.SelectedDate, c.CLCODE);
            carryOverDueRecord.TotalRecievedAmount = 0;
            invoice.Add(carryOverDueRecord);
            rs.Value = invoice;
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            List<ReportParameter> repParams = new List<ReportParameter>();
            repParams.Add(new ReportParameter("CompanyName", Configs.Default.CompanyName));
            repParams.Add(new ReportParameter("ComapnyPhoneNo", Configs.Default.CompanyPhone));
            repParams.Add(new ReportParameter("CompanyAddress", Configs.Default.CompanyAddress));
            repParams.Add(new ReportParameter("CompanyEmail", Configs.Default.CompanyEmail));
            repParams.Add(new ReportParameter("CompanyFax", Configs.Default.CompanyFax));
            repParams.Add(new ReportParameter("ToDate", ((DateTime)ToDate.SelectedDate).ToString("dd-MMM-yyyy")));
            repParams.Add(new ReportParameter("FromDate", ((DateTime)FromDate.SelectedDate).ToString("dd-MMM-yyyy")));
            double? billedamountsum =  this.invoice.Select(y => y.PayAmount).Sum();
            double? amountRecivedsum = this.invoice.Select(y => y.TotalRecievedAmount).Sum();
            double? TotalSum = billedamountsum - amountRecivedsum;
            List<PaymentEntry> entries = db.PaymentEntries.Where(x => x.ClientCode == c.CLCODE && x.DebitNote != null && x.Date <= (ToDate.SelectedDate ?? DateTime.Today) && x.Date >= (FromDate.SelectedDate ?? DateTime.Today)).ToList();
            double totalTDS = 0;
            double totalDNote = 0;
            if(entries.Count > 0)
            {
                totalTDS = entries.Select(x=>x.TDS).Sum();
                totalDNote = entries.Select(x=>x.DebitNote).Sum();
            }
            repParams.Add(new ReportParameter("TotalTDS", String.Format("{0:F2}",totalTDS)));
            repParams.Add(new ReportParameter("TotalDiscount", String.Format("{0:F2}",totalDNote)));
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
