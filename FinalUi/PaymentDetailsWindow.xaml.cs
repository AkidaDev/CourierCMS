using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for PaymentDetailsWindow.xaml
    /// </summary>
    public partial class PaymentDetailsWindow : Window
    {
        List<Client> ClientList;
        List<PaymentEntry> EntryList;
        List<Invoice> InvoiceLists;
        public PaymentDetailsWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            ClientList = db.Clients.ToList();
            CollectionViewSource clientListSource = (CollectionViewSource)(FindResource("ClientList"));
            clientListSource.Source = ClientList;
        }

        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = "";
            CollectionViewSource dataGridSource = (CollectionViewSource)FindResource("PaymentDetailsSourceGrid");
            CollectionViewSource invoiceSource = (CollectionViewSource)FindResource("InvoiceDetailsSourceGrid");
            BillingDataDataContext db = new BillingDataDataContext();
            if (FromDatepicker.SelectedDate == null)
                errorMsg += "Select From Date. \n";
            if (ToDatePicker.SelectedDate == null)
                errorMsg += "Select To Date. \n";
            if (errorMsg == "")
            {
                EntryList = db.PaymentEntries.Where(x => x.Date >= (DateTime)FromDatepicker.SelectedDate && x.Date <= (DateTime)ToDatePicker.SelectedDate
                    && x.ClientCode == ((Client)ClientComboBox.SelectedItem).CLCODE).ToList();
                dataGridSource.Source = EntryList;
                InvoiceLists = db.Invoices.Where(x =>
                    x.Date >= (DateTime)FromDatepicker.SelectedDate && x.Date <= (DateTime)ToDatePicker.SelectedDate &&
                    x.ClientCode == ((Client)ClientComboBox.SelectedItem).CLCODE).ToList();
                invoiceSource.Source = InvoiceLists;
            }
            else
            {
                MessageBox.Show("Please correct the following errors: \n" + errorMsg);
            }

        }

        private void FetchInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (InvoiceDatagrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select one row to view the invoice details.");
                return;
            }
            Invoice inv = (Invoice)InvoiceDatagrid.SelectedItem;
            BillingDataDataContext db = new BillingDataDataContext();
            /*List<Transaction> transactions = db.InvoiceAssignments.Where(x => x.BillId == inv.BillId).Select(y => y.Transaction).ToList();
            Double tax = (((inv.TotalAmount - (inv.Basic + inv.Fuel)) / (inv.TotalAmount)) * 100);
            if (inv.PreviousDue == null)
                inv.PreviousDue = 0;
             * */
            PrintMainWindow window = new PrintMainWindow(inv);
            try
            {
                window.Show();
            }catch(InvalidOperationException)
            {
                MessageBox.Show("Unable to open invoice"); 
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

        private void DeleteInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if(InvoiceDatagrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select 1 invoice to delete", "Message");
                return;
            }
            if(MessageBox.Show("Are you sure you want to delete this invoice?","Confirm",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                Invoice invoice = db.Invoices.SingleOrDefault(x => x.BillId == ((Invoice)InvoiceDatagrid.SelectedItem).BillId);
                if(invoice == null)
                {
                    MessageBox.Show("This invoice doesn't exists or has already been deleted..", "Error");
                    return;
                }
                db.Invoices.DeleteOnSubmit(invoice);
                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("Invoice successfully deleted. Click on fetch button to view the changes.","Success");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error in deleting invoice. Error Message: " + ex.Message, "Error");
                }
            }
        }
    }
}
