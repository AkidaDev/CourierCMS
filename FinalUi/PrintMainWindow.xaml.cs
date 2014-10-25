using Microsoft.Reporting.WinForms;
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
    /// Interaction logic for PrintMainWindow.xaml
    /// </summary>
    public partial class PrintMainWindow : Window
    {
        public PrintMainWindow(ReportDataSource rs,List<ReportParameter> repParams)
        {

            InitializeComponent();
            BillViewer.LocalReport.ReportPath = "Report1.rdlc";
            BillViewer.LocalReport.DataSources.Clear();
            BillViewer.LocalReport.DataSources.Add(rs);
            BillViewer.LocalReport.SetParameters(repParams);
            BillViewer.RefreshReport();
            this.Closed += PrintMainWindow_Closed;
        }

        void PrintMainWindow_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Dont forget to save the invoice after this window is closed... (Ignore if done already)");
        }
    }
}
