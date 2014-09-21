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
using Microsoft.Reporting.WebForms;
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
        public PrintWindow(List<RuntimeData> data)
        {
            InitializeComponent();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            DataGridSource = (CollectionViewSource)FindResource("DataGridDataSource");
            dataGridSource = data;
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource.Source = db.Clients.Select(x => x.CLCODE);
            Microsoft.Reporting.WinForms.ReportDataSource rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "DataSet1";
            rs.Value = dataGridSource;
            BillViewer.LocalReport.ReportPath = "Report1.rdlc";
            //PageSettings pg = BillViewer.GetPageSettings() ;
            //pg.Margins = new Margins(6,6,6,6);
            //BillViewer.SetPageSettings(pg);
            BillViewer.LocalReport.DataSources.Add(rs);
            BillViewer.ShowExportButton = true;
            BillViewer.RefreshReport();
        }

        void BillViewer_RenderingComplete(object sender, Microsoft.Reporting.WinForms.RenderingCompleteEventArgs e)
        {
            BillViewer.RefreshReport();
        }
        public void RefreshDataGridSource()
        {
            if (ClientList.SelectedValue != null && ToDate.SelectedDate != null && FromDate.SelectedDate != null)
            {

                DataGridSource.Source = dataGridSource.Where(x => x.CustCode == (string)ClientList.SelectedValue && x.BookingDate <= ToDate.SelectedDate && x.BookingDate >= FromDate.SelectedDate).ToList() ;
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
        private void printObj()
        {
            
            BillViewer.RefreshReport();
          }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            printObj();
        }
    }
}
