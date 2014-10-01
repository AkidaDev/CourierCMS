using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for TestingMainWindow.xaml
    /// </summary>
    public partial class TestingMainWindow : Window
    {
        public TestingMainWindow()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Login window = new Login();
            window.ShowDialog();
        }

        private void mainwindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void manageclient_Click(object sender, RoutedEventArgs e)
        {
            ManageClient window = new ManageClient();
            window.ShowDialog();
        }

        private void manageemployee_Click(object sender, RoutedEventArgs e)
        {
            ManageEmployee window = new ManageEmployee();
            window.ShowDialog();
        }

        private void ratewindow_Click(object sender, RoutedEventArgs e)
        {
            RateWindow winodw = new RateWindow();
            winodw.ShowDialog();
        }

        private void stockwindow_Click(object sender, RoutedEventArgs e)
        {
            StockWindow window = new StockWindow();
            window.ShowDialog();
        }


        private void ServiceWindow_Click(object sender, RoutedEventArgs e)
        {
            ServiceFinder window = new ServiceFinder();
            window.Show();
        }

        private void AnalyzeInvoicebutton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileD = new Microsoft.Win32.OpenFileDialog();
            fileD.DefaultExt = "*.pdf";
            Nullable<bool> result = fileD.ShowDialog();
            if(result == true)
            {
                //InvoiceReport win = new InvoiceReport(fileD.FileName);
                //win.Show();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           testtheme win = new testtheme();
            win.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BillReportWindow win = new BillReportWindow();
            win.Show();
        }

        private void PreferenceWindow_Click(object sender, RoutedEventArgs e)
        {
            PreferenceWindow window = new PreferenceWindow(); window.ShowDialog();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PaymentRecieved window = new PaymentRecieved();
            window.Show();
        }
    }
}
