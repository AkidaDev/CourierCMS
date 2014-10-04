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
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        CollectionViewSource ClientListSource;
        CollectionViewSource DataGridSource;
        List<RuntimeData> dataGridSource;
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        public PrintWindow(List<RuntimeData> data, Client client, DateTime toDate, DateTime fromDate, double tax, double previousDue)
            : this(data)
        {
            ToDate.SelectedDate = toDate.Date;
            FromDate.SelectedDate = fromDate.Date;
            ToDate.IsEnabled = false;
            FromDate.IsEnabled = false;
            ClientList.SelectedItem = client.CLCODE;
            ClientList.IsEnabled = false;
            TaxBox.Text = tax.ToString();
            TaxBox.IsEnabled = false;
            PreviousDueTextBox.Text = previousDue.ToString();
            PreviousDueTextBox.IsEnabled = false;
            MiscBox.Text = "0";
            printObj();
        }
        public PrintWindow(List<RuntimeData> data)
        {
            InitializeComponent();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            DataGridSource = (CollectionViewSource)FindResource("DataGridDataSource");
            dataGridSource = data;
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource.Source = db.Clients.Select(x => x.CLCODE);
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "DataSet1";
            rs.Value = dataGridSource;
            BillViewer.LocalReport.ReportPath = "Report1.rdlc";

        }

        void BillViewer_RenderingComplete(object sender, Microsoft.Reporting.WinForms.RenderingCompleteEventArgs e)
        {
            BillViewer.RefreshReport();
        }
        public void RefreshDataGridSource()
        {
            if (ClientList.SelectedValue != null && ToDate.SelectedDate != null && FromDate.SelectedDate != null)
            {

                DataGridSource.Source = dataGridSource.Where(x => x.CustCode == (string)ClientList.SelectedValue && x.BookingDate <= ToDate.SelectedDate && x.BookingDate >= FromDate.SelectedDate).ToList();
            }
        }
        private void ToDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            RefreshDataGridSource();
        }

        private void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDataGridSource();
        }
        private void printObj(Client client = null)
        {

            BillingDataDataContext db = new BillingDataDataContext();
            List<RuntimeData> source = dataGridSource.Where(x => x.CustCode == ClientList.Text && x.BookingDate <= ToDate.SelectedDate && x.BookingDate >= FromDate.SelectedDate).ToList();
            rs.Value = source;
            Client clc;
            if (client == null)
                clc = db.Clients.SingleOrDefault(x => x.CLCODE == ClientList.Text);
            else
                clc = client;
            BillViewer.LocalReport.DataSources.Clear();
            BillViewer.LocalReport.DataSources.Add(rs);
            List<ReportParameter> repParams = new List<ReportParameter>();
            BillViewer.LocalReport.SetParameters(repParams);
            string dateString = FromDate.DisplayDate.ToString() + " to " + ToDate.DisplayDate.ToString();
            repParams.Add(new ReportParameter("DateString", dateString));
            string descriptionString = "";
            descriptionString = "Total Connsignments: " + source.Count;
            repParams.Add(new ReportParameter("DescriptionString", descriptionString));
            string mainAmount = source.Sum(x => x.FrAmount).ToString();
            repParams.Add(new ReportParameter("MainAmountString", mainAmount));
            repParams.Add(new ReportParameter("TaxPercentageString", TaxBox.Text));
            double taxamount = double.Parse(TaxBox.Text) * double.Parse(mainAmount) / 100 ;
            repParams.Add(new ReportParameter("TaxAmountString", taxamount.ToString()));
            repParams.Add(new ReportParameter("MiscellaneousAmountString", MiscBox.Text));
            double totalAmount = double.Parse(mainAmount) + taxamount + double.Parse(MiscBox.Text) + double.Parse(PreviousDueTextBox.Text);
            repParams.Add(new ReportParameter("TotalAmountString", totalAmount.ToString()));
            string totalAmountinWordString = UtilityClass.NumbersToWords((int)totalAmount);
            repParams.Add(new ReportParameter("TotalAmountInWordString", totalAmountinWordString));
            repParams.Add(new ReportParameter("PreviousDueString", PreviousDueTextBox.Text));
            repParams.Add(new ReportParameter("CompanyName", Configs.Default.CompanyName));
            repParams.Add(new ReportParameter("ComapnyPhoneNo", Configs.Default.CompanyPhone));
            repParams.Add(new ReportParameter("CompanyAddress", Configs.Default.CompanyAddress));
            repParams.Add(new ReportParameter("CompanyEmail", Configs.Default.CompanyEmail));
            repParams.Add(new ReportParameter("CompanyFax", Configs.Default.CompanyFax));
            repParams.Add(new ReportParameter("ClientName", clc.CLNAME));
            repParams.Add(new ReportParameter("ClientAddress", clc.ADDRESS));
            repParams.Add(new ReportParameter("ClientPhoneNo", clc.CONTACTNO));
            repParams.Add(new ReportParameter("InvoiceNumber", Guid.NewGuid().ToString()));
            BillViewer.LocalReport.SetParameters(repParams);
            BillViewer.ShowExportButton = true;

            BillViewer.RefreshReport();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            printObj();
        }
    }
}
