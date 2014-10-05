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
        CollectionViewSource profitDataGridSource;
        public ClientReportWindow()
        {
            InitializeComponent();
            dueDataGridSource = (CollectionViewSource)FindResource("DueGridDataSource");
            profitDataGridSource = (CollectionViewSource)FindResource("ProfitabilityGridDataSource");
            BillingDataDataContext db = new BillingDataDataContext();
            dueDataGridSource.Source = db.BalanceViews;
            profitDataGridSource.Source = db.PROFITVIEWs;
        }
    }
}
