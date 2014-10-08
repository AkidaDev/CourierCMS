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
            BillingDataDataContext db = new BillingDataDataContext();
            ClientList = db.Clients.ToList();
            CollectionViewSource clientListSource = (CollectionViewSource)(FindResource("ClientList"));
            clientListSource.Source = ClientList;
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
            paymentEntry.ClientCode = ((Client)ClientComboBox.SelectedItem).CLCODE;
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
                    paymentEntry.ChequeNumber = ChequeNumberBox.Text;
            }
            else
                paymentEntry.Type = "Cash";
            paymentEntry.Remarks = RemarkBox.Text;
            if(errorMessage == "")
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
                MessageBox.Show("Payment Recived\n Reference no is "+paymentEntry.Id);
                this.Close();
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
