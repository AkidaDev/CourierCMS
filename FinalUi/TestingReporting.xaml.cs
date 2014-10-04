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
using Microsoft.Reporting.Common;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for TestingReporting.xaml
    /// </summary>
    public partial class TestingReporting : Window
    {
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        CollectionViewSource ClientListSource;
        CollectionViewSource DataGridSource;
        List<Client> clientList;
        List<Invoice> invoice;
        public TestingReporting()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            ClientListSource.Source = db.Clients.ToList();
            rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            rs.Name = "InvoiceDataSet";
            AccountStatementViewer.LocalReport.ReportPath = "AccountStatementReport.rdlc";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            CreateObj();
        }

        private void CreateObj()
        {

            BillingDataDataContext db = new BillingDataDataContext();
            var c  =  (Client) this.ClientListCombo.SelectedItem;
            invoice = db.Invoices.Where(x=> x.ClientCode == c.CLCODE).ToList();
            rs.Value = invoice;
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.LocalReport.DataSources.Add(rs);
            List<ReportParameter> repParams = new List<ReportParameter>();
            repParams.Add(new ReportParameter("CompanyName", Configs.Default.CompanyName));
            repParams.Add(new ReportParameter("ComapnyPhoneNo", Configs.Default.CompanyPhone));
            repParams.Add(new ReportParameter("CompanyAddress", Configs.Default.CompanyAddress));
            repParams.Add(new ReportParameter("CompanyEmail", Configs.Default.CompanyEmail));
            repParams.Add(new ReportParameter("CompanyFax", Configs.Default.CompanyFax));
            string basicsum = this.invoice.Select(y => y.Basic).Sum().ToString() ?? "";
            string totalsum = this.invoice.Select(y => y.TotalAmount).Sum().ToString() ?? "";
            repParams.Add(new ReportParameter("BasicTotal", basicsum));
            repParams.Add(new ReportParameter("TotalSum", basicsum));
            repParams.Add(new ReportParameter("ClientName", c.CLNAME));
            repParams.Add(new ReportParameter("ClientAddress", c.ADDRESS));
            repParams.Add(new ReportParameter("ClientPhoneNo", c.CONTACTNO));
            AccountStatementViewer.LocalReport.SetParameters(repParams);
            AccountStatementViewer.ShowExportButton = true;
            AccountStatementViewer.RefreshReport();
        }

        private void ClientListCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AccountStatementViewer.LocalReport.DataSources.Clear();
            AccountStatementViewer.RefreshReport();
        }
    }
}