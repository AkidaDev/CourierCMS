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
            window.Show();
        }

        private void mainwindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void manageclient_Click(object sender, RoutedEventArgs e)
        {
            ManageClient window = new ManageClient();
            window.Show();
        }

        private void manageemployee_Click(object sender, RoutedEventArgs e)
        {
            ManageEmployee window = new ManageEmployee();
            window.Show();
        }

        private void ratewindow_Click(object sender, RoutedEventArgs e)
        {
            RateWindow winodw = new RateWindow();
            winodw.Show();
        }

        private void stockwindow_Click(object sender, RoutedEventArgs e)
        {
            StockWindow window = new StockWindow();
            window.Show();
        }

        private void rateassignment_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
