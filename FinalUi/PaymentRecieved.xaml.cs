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
    /// Interaction logic for PaymentRecieved.xaml
    /// </summary>
    public partial class PaymentRecieved : Window
    {
        List<Client> ClientList;
        public PaymentRecieved()
        {
            InitializeComponent();
            PaymentRefNoBox.Text = (DateTime.Now.ToString("yyyyMMddhhmmss", System.Globalization.CultureInfo.GetCultureInfo("en-US")));
            PaymentDatePicker.SelectedDate = DateTime.Today;
            ChequeRadio.Checked += CashRadio_Unchecked;
            CashRadio.Checked += CashRadio_Checked;
            DebitNoteBox.Text = "0";
            TDSBox.Text = "0";
            CollectionViewSource clientListSource = (CollectionViewSource)(FindResource("ClientList"));
            clientListSource.Source = DataSources.ClientCopy;
            CollectionViewSource invoiceListSource = (CollectionViewSource)FindResource("InvoiceList");
            BillingDataDataContext db = new BillingDataDataContext();
            invoiceListSource.Source = (from invoice in db.Invoices
                                        where !(from payment in db.PaymentEntries
                                                select payment.InvoiceNumber)
                                                .Contains(invoice.BillId)
                                        select invoice).ToList();
        }
        private void CashRadio_Checked(object sender, RoutedEventArgs e)
        {
            ChequeNumberPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void CashRadio_Unchecked(object sender, RoutedEventArgs e)
        {
            ChequeNumberPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            bool isdone = false;
            PaymentEntry paymentEntry = new PaymentEntry();
            string errorMessage = "";
            paymentEntry.Id = PaymentRefNoBox.Text;
            double tempStorage;
            if (!double.TryParse(AmountTextBox.Text, out tempStorage))
                errorMessage += "Amount is not in correct format. \n";
            else
                paymentEntry.RecievedAmount = tempStorage;
            if (InvoiceSelectRadio.IsChecked == true)
            {
                Invoice invoice = (Invoice)InvoiceComboBox.SelectedItem;
                paymentEntry.InvoiceNumber = invoice.BillId;
                paymentEntry.ClientCode = invoice.ClientCode;
            }
            else
            {
                Client client = (Client)ClientComboBox.SelectedItem;
                paymentEntry.ClientCode = client.CLCODE;
            }
            if (PaymentDatePicker.SelectedDate != null)
                paymentEntry.Date = (DateTime)PaymentDatePicker.SelectedDate;
            else
                errorMessage += "Payment Entry Date is not Selected. \n";
            if ((bool)ChequeRadio.IsChecked)
            {
                paymentEntry.Type = "Cheque";
                if (ChequeNumberBox.Text == "")
                    errorMessage += "Enter the cheque number. \n";
                else
                {
                    paymentEntry.ChequeNumber = ChequeNumberBox.Text;
                }
                if (ChequeBankName.Text == "")
                    errorMessage += "Enter bank name: \n";
                else
                    paymentEntry.BankName = ChequeBankName.Text;

            }
            else
                paymentEntry.Type = "Cash";
            paymentEntry.Remarks = RemarkBox.Text;
            if (double.TryParse(DebitNoteBox.Text, out tempStorage))
                paymentEntry.DebitNote = tempStorage;
            else
                errorMessage += "Enter debit note properly \n";
            if (double.TryParse(TDSBox.Text, out tempStorage))
                paymentEntry.TDS = tempStorage;
            else
                errorMessage += "Enter TDS properly \n";

            if (errorMessage == "")
            {
                BillingDataDataContext db = new BillingDataDataContext();
                db.PaymentEntries.InsertOnSubmit(paymentEntry);
                try
                {
                    db.SubmitChanges();
                    isdone = true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); isdone = false; return; }
            }
            else
            {
                MessageBox.Show("Please correct the following errors: \n" + errorMessage);
            }
            if (isdone)
            {
                MessageBox.Show("Payment Recived\nReference no is " + paymentEntry.Id);
                this.Close();
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                if (InvoiceSelectRadio.IsChecked == true)
                {
                    InvoiceComboBox.Visibility = Visibility.Visible;
                    ClientComboBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    InvoiceComboBox.Visibility = System.Windows.Visibility.Collapsed;
                    ClientComboBox.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(InvoiceSelectRadio.IsChecked == true)
            {
                Invoice tempInv = InvoiceComboBox.SelectedItem as Invoice;
                if(tempInv != null)
                {
                    double tempStorage;
                    if(double.TryParse(AmountTextBox.Text, out tempStorage))
                    {
                        DebitNoteBox.Text = (tempInv.totalAmount - tempStorage).ToString();
                    }
                }
            }
        }

        private void DebitNoteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(InvoiceSelectRadio.IsChecked == true)
            {
                Invoice tempInv = InvoiceComboBox.SelectedItem as Invoice;
                if(tempInv != null)
                {
                    double tempStorage1, tempStorage2;
                    if(double.TryParse(AmountTextBox.Text, out tempStorage1) && double.TryParse(DebitNoteBox.Text,out tempStorage2))
                    {
                        TDSBox.Text = (tempInv.totalAmount - tempStorage1 - tempStorage2).ToString();
                    }
                }
            }
        }
    }
}
