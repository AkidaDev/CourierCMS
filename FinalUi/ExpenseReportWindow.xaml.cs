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
    /// Interaction logic for ExpenseReportWindow.xaml
    /// </summary>
    public partial class ExpenseReportWindow : Window
    {
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        public ExpenseReportWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "ExpenseDataSet";
            AccountStatementViewer.LocalReport.ReportPath = "ExpenseAnalysisReport.rdlc";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
            {
                MessageBox.Show("Please select from date and to date correctly..");
                return;
            }
            BillingDataDataContext db = new BillingDataDataContext();
            var source = db.Expenses.Where(x => x.Date>= FromDate.SelectedDate && x.Date<= ToDate.SelectedDate);
            List<Expense> reportSource = source.ToList();
            rs.Value = reportSource;
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            AccountStatementViewer.ShowExportButton = true;
            AccountStatementViewer.RefreshReport();
        }
    }
}
