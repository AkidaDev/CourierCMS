using Microsoft.Reporting.WinForms;
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
    public partial class AccountStatementFullWindow : Window
    {
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        public AccountStatementFullWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "AccountStatementDatasetFull";
            AccountStatementViewer.LocalReport.ReportPath = "AccountStatementFullReport.rdlc";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            {
                MessageBox.Show("Please select from date and to date correctly..");
                return;
            }
            BillingDataDataContext db = new BillingDataDataContext();
            string type;
            if (InvoiceRadio.IsChecked == true)
                type = "Invoice";
            else
                type = "Payment";
            var source = db.AccountStatementFulls.Where(x => x.TransactionDate >= FromDate.SelectedDate && x.TransactionDate <= ToDate.SelectedDate && x.TypeOfRecord == type).OrderBy(y=>y.TransactionDate);
            List<AccountStatementFull> reportSource = source.ToList();
            if(type == "Invoice")
            {
                List<Invoice> invoiceList = db.Invoices.Where(x => x.Date >= FromDate.SelectedDate && x.Date <= ToDate.SelectedDate).ToList();
                List<AccountStatementFull> AccSTats = source.ToList();
                AccSTats.ForEach(x =>
                {
                    Invoice inv = invoiceList.SingleOrDefault(y => y.BillId == x.Id);
                    if (inv != null)
                        x.PayAmount = x.PayAmount + inv.PreviousDue;
                });
                reportSource = AccSTats;
            }
            rs.Value = reportSource;
            List<ReportParameter> repParams = new List<ReportParameter>();
            repParams.Add(new ReportParameter("ToDate", ((DateTime)ToDate.SelectedDate).ToString("dd-MMM-yyyy")));
            repParams.Add(new ReportParameter("FromDate", ((DateTime)FromDate.SelectedDate).ToString("dd-MMM-yyyy")));
            repParams.Add(new ReportParameter("Type", type));
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            AccountStatementViewer.LocalReport.SetParameters(repParams);
            AccountStatementViewer.ShowExportButton = true;
            AccountStatementViewer.RefreshReport();
        }
    }
}