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
            : this(data, toDate, fromDate)
        {

            ToDate.IsEnabled = false;
            FromDate.IsEnabled = false;
            ClientList.SelectedItem = client.CLCODE;
            ClientList.IsEnabled = false;
            TaxBox.Text = tax.ToString();
            TaxBox.IsEnabled = false;
            PreviousDueTextBox.Text = previousDue.ToString();
            PreviousDueTextBox.IsEnabled = false;
            MiscBox.Text = "0";
            printObj(client);
        }
        public PrintWindow(List<RuntimeData> data, DateTime toDate, DateTime fromDate)
        {
            InitializeComponent();
            ToDate.SelectedDate = toDate.Date;
            FromDate.SelectedDate = fromDate.Date;
            TaxBox.Text = Configs.Default.ServiceTax;

            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            DataGridSource = (CollectionViewSource)FindResource("DataGridDataSource");
            dataGridSource = data;
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource.Source = db.Clients;
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "DataSet1";
            rs.Value = dataGridSource;

        }


        public void RefreshDataGridSource()
        {
            if (ClientList.SelectedValue != null && ToDate.SelectedDate != null && FromDate.SelectedDate != null)
            {

                DataGridSource.Source = dataGridSource.Where(x => x.CustCode == ((Client)ClientList.SelectedValue).CLCODE && x.BookingDate <= ToDate.SelectedDate && x.BookingDate >= FromDate.SelectedDate).ToList();
            }
        }
        private void ToDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            RefreshDataGridSource();
        }

        private void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Client selectedClient = ((Client)ClientList.SelectedItem);
            TaxBox.Text = ((Client)ClientList.SelectedItem).FUEL.ToString();
            ServiceTaxBox.Text = selectedClient.STAX.ToString();
            DiscountBox.Text = selectedClient.AMTDISC.ToString();
            BillingDataDataContext db = new BillingDataDataContext();
            double prevdue = db.GetPreviousDue(selectedClient.CLCODE) ?? 0;
            PreviousDueTextBox.Text = prevdue.ToString();
            RefreshDataGridSource();
        }
        string dateString;
        Client clc;
        string descriptionString;
        double tax;
        double mainAmountValue;
        double taxamount;
        double totalAmount;
        string totalAmountinWordString;
        string invoiceNo;
        List<RuntimeCityView> source;
        private void printObj(Client client = null)
        {
            #region temp
            string errorMsg = "";
            if (ToDate.SelectedDate == null || FromDate.SelectedDate == null || ToDate.SelectedDate < FromDate.SelectedDate)
                errorMsg += "Please set the date properly. \n";
            if (ServiceTaxBox.Text == "")
                ServiceTaxBox.Text = "0";
            double temp;
            if (TaxBox.Text == "")
                TaxBox.Text = "0";
            if (MiscBox.Text == "")
                MiscBox.Text = "0";
            if (PreviousDueTextBox.Text == "")
                PreviousDueTextBox.Text = "0";
            if (double.TryParse(TaxBox.Text, out temp) == false)
                errorMsg += "Please enter fuel surcharge correctly. \n";
            if (double.TryParse(ServiceTaxBox.Text, out temp) == false)
                errorMsg += "Please enter service tax correctly. \n";
            if (double.TryParse(MiscBox.Text, out temp) == false)
                errorMsg += "Please enter miscellenaeous charge correctly. \n";
            if (double.TryParse(PreviousDueTextBox.Text, out temp) == false)
                errorMsg += "Please enter previous charge correctly. \n";
            if (double.TryParse(DiscountBox.Text, out temp) == false)
                errorMsg += "Enter customer discount properly. \n";
            if (errorMsg != "")
            {
                MessageBox.Show(errorMsg);
                return;
            }
            #endregion
            BillingDataDataContext db = new BillingDataDataContext();
            source = UtilityClass.convertToRuntimeVIew(dataGridSource).Where(x => x.CustCode == ((Client)ClientList.SelectedItem).CLCODE && x.BookingDate <= ToDate.SelectedDate && x.BookingDate >= FromDate.SelectedDate).ToList();
            rs.Value = source;
            if (client == null)
                clc = db.Clients.SingleOrDefault(x => x.CLCODE == ((Client)ClientList.SelectedItem).CLCODE);
            else
                clc = client;
            List<ReportParameter> repParams = new List<ReportParameter>();
            DateTime toDate = ToDate.SelectedDate??DateTime.Today;
            DateTime fromDate = FromDate.SelectedDate??DateTime.Today;
            dateString = fromDate.ToString("dd/MM/yyyy") + " to " + toDate.ToString("dd/MM/yyyy");
            repParams.Add(new ReportParameter("DateString", dateString));
            descriptionString = "Total Connsignments: " + source.Count;
            repParams.Add(new ReportParameter("DescriptionString", descriptionString));
            mainAmountValue = (double)(source.Sum(x => x.FrAmount) ?? 0);
            repParams.Add(new ReportParameter("MainAmountString", mainAmountValue.ToString()));
            repParams.Add(new ReportParameter("FuelString", TaxBox.Text));
            double fuelAmount = double.Parse(TaxBox.Text) * mainAmountValue / 100;
            repParams.Add(new ReportParameter("FuelAmount", fuelAmount.ToString()));
            repParams.Add(new ReportParameter("ServiceTaxString", ServiceTaxBox.Text));
            tax = double.Parse(ServiceTaxBox.Text) * mainAmountValue / 100;
            repParams.Add(new ReportParameter("ServiceTaxAmount", tax.ToString()));
            double discount = double.Parse(DiscountBox.Text) * mainAmountValue / 100;
            repParams.Add(new ReportParameter("DiscountPString", DiscountBox.Text));
            repParams.Add(new ReportParameter("DiscountAmountString", discount.ToString()));
            repParams.Add(new ReportParameter("MiscellaneousAmountString", MiscBox.Text));
            taxamount = tax + fuelAmount;
            totalAmount = mainAmountValue + taxamount + double.Parse(MiscBox.Text) + double.Parse(PreviousDueTextBox.Text) - discount;
            repParams.Add(new ReportParameter("TotalAmountString", totalAmount.ToString()));
            totalAmountinWordString = UtilityClass.NumbersToWords((int)totalAmount);
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
            repParams.Add(new ReportParameter("TNC", Configs.Default.TNC));
            invoiceNo = DateTime.Now.ToString("yyyyMMddhhmm");
            repParams.Add(new ReportParameter("InvoiceNumber", invoiceNo));
            PrintMainWindow win = new PrintMainWindow(rs, repParams);
            win.Show();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            printObj();
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SaveInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (source == null || source.Count == 0)
                return;
            MessageBoxResult result = MessageBox.Show("Do you want to save this invoice? ", "Confirm", MessageBoxButton.YesNo);
            int count = source.Where(x => x.TransactionId == null || x.TransactionId == Guid.Empty).Count();
            if (count > 0)
            {
                MessageBox.Show("Selected transactions set contains records that are not yet saved in the database. This bill cannot be saved.");
                return;
            }
            if (result == MessageBoxResult.Yes)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                Invoice invoice = new Invoice();
                try
                {
                    invoice.Basic = mainAmountValue;
                    invoice.BillId = invoiceNo;
                    invoice.ClientCode = clc.CLCODE;
                    invoice.Date = DateTime.Today;
                    invoice.Fuel = double.Parse(TaxBox.Text);
                    invoice.PreviousDue = double.Parse(PreviousDueTextBox.Text);
                    invoice.Remarks = RemarkBox.Text;
                    invoice.STax = double.Parse(ServiceTaxBox.Text);
                    invoice.TotalAmount = totalAmount;
                    invoice.Discount = double.Parse(DiscountBox.Text);
                    invoice.Misc = double.Parse(MiscBox.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Enter the fields properly...");
                    return;
                }
                try
                {
                    db.Invoices.InsertOnSubmit(invoice);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
                foreach (var item in source)
                {
                    InvoiceAssignment assign = new InvoiceAssignment();
                    assign.BillId = invoice.BillId;
                    assign.Id = Guid.NewGuid();
                    assign.TransactionId = (Guid)item.TransactionId;
                    assign.BilledAmount = (double)item.FrAmount;
                    assign.BilledWeight = (double)item.BilledWeight;
                    assign.Destination = item.Destination;
                    assign.DestinationDesc = item.CITY_DESC;
                    db.InvoiceAssignments.InsertOnSubmit(assign);
                }
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
