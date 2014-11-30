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
    /// Interaction logic for ClientReportWindow.xaml
    /// </summary>
    public partial class ClientReportWindow : Window
    {
        CollectionViewSource dueDataGridSource;
        CollectionViewSource UnpaidInvoicesList;
        public ClientReportWindow()
        {
            InitializeComponent();
            dueDataGridSource = (CollectionViewSource)FindResource("DueGridDataSource");
            UnpaidInvoicesList = (CollectionViewSource)FindResource("UnpaidInvoiceGridSource");
            BillingDataDataContext db = new BillingDataDataContext();
            dueDataGridSource.Source = db.BalanceViews;
            UnpaidInvoicesList.Source = (from invoice in db.Invoices
                                        where !(from payment in db.PaymentEntries
                                                    select payment.InvoiceNumber)
                                                .Contains(invoice.BillId)
                                         select invoice).ToList();
            Invoice inv = new Invoice();
            
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
