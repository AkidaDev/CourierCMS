﻿using System;
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
            BillingDataDataContext db = new BillingDataDataContext();
            SecurityModule.authenticate("purushottam", "1234");
          //  MessageBox.Show(LoadResources.getConString());
            LoadResources.hey = 3;
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
            window.ShowDialog();
        }

        private void manageemployee_Click(object sender, RoutedEventArgs e)
        {
            ManageEmployee window = new ManageEmployee();
            window.ShowDialog();
        }

        private void zonewindow_Click(object sender, RoutedEventArgs e)
        {
            ZoneAssignment winodw = new ZoneAssignment();
            winodw.ShowDialog();
        }

        private void stockwindow_Click(object sender, RoutedEventArgs e)
        {
            ManageCity window = new ManageCity();
            window.Show();
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
         //  testtheme win = new testtheme();
         //   win.Show();
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
            CountryAssignment window = new CountryAssignment();
            window.Show();
        }

        private void AccountStatement_Click(object sender, RoutedEventArgs e)
        {
           // AccountStatementReportingWindow window = new AccountStatementReportingWindow(); window.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ClientReport window = new ClientReport(); window.Show();
        }

        private void TestingPermisstion_Click(object sender, RoutedEventArgs e)
        {
            TestingPermisstion window = new TestingPermisstion();
            window.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddServiceRule window = new AddServiceRule(new BillingDataDataContext().Quotations.Where(x => x.CLCODE == "123").FirstOrDefault()); window.Show();
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AddInvoiceRule window = new AddInvoiceRule(); window.Show();
        }

    }
}
