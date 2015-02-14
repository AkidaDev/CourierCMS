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
         
        }

        private void ClientProfitabilityGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}