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
        List<RuntimeCityView> source;
        Microsoft.Reporting.WinForms.ReportDataSource rs;
        public PrintMainWindow(Invoice inv)
        {
            try
            {
                InitializeComponent();
                BillingDataDataContext db = new BillingDataDataContext();
                List<RuntimeData> data = UtilityClass.convertTransListToRuntimeList(db.ExecuteQuery<Transaction>(@"
                SELECT  [ID]
      ,[AmountPayed]
      ,[AmountCharged]
      ,[ConnsignmentNo]
      ,[Weight]
      ,[WeightByFranchize]
      ,[Destination]
      ,[DestinationPin]
      ,[UserId]
      ,[BookingDate]
      ,[AddDate]
      ,[LastModified]
      ,[Type]
      ,[Mode]
      ,[DOX]
      ,[ServiceTax]
      ,[SplDisc]
      ,[InvoiceNo]
      ,[InvoiceDate]
      ,[CustCode]
      ,[TransMF_No]
      ,[BilledWeight]
  FROM [BillingDatabase].[dbo].[InvoiceView]      
where [BillId] = '" + inv.BillId + @"'        
            ").ToList());
                source = UtilityClass.convertToRuntimeVIew(data);
                rs = new ReportDataSource();
                rs.Value = source;
                if (inv.Misc == null)
                    inv.Misc = 0;
                if (inv.PreviousDue == null)
                    inv.PreviousDue = 0;

                Client clc = db.Clients.SingleOrDefault(x => x.CLCODE == inv.ClientCode);
                List<ReportParameter> repParams = new List<ReportParameter>();
                string dateString = inv.Date.ToString();
                repParams.Add(new ReportParameter("DateString", dateString));
                string descriptionString = "";
                descriptionString = "Total Connsignments: " + source.Count;
                repParams.Add(new ReportParameter("DescriptionString", descriptionString));
                string mainAmount = source.Sum(x => x.FrAmount).ToString();
                repParams.Add(new ReportParameter("MainAmountString", mainAmount));
                repParams.Add(new ReportParameter("FuelString", inv.Fuel.ToString()));
                repParams.Add(new ReportParameter("ServiceTaxString", inv.STax.ToString()));
                double tax = inv.Fuel + inv.STax;
                repParams.Add(new ReportParameter("TaxPercentageString", tax.ToString()));
                double taxamount = tax * double.Parse(mainAmount) / 100;
                repParams.Add(new ReportParameter("TaxAmountString", taxamount.ToString()));
                repParams.Add(new ReportParameter("MiscellaneousAmountString", inv.Misc.ToString()));
                double totalAmount = double.Parse(mainAmount) + taxamount + (double)(inv.Misc) + (double)inv.PreviousDue;
                repParams.Add(new ReportParameter("TotalAmountString", totalAmount.ToString()));
                string totalAmountinWordString = UtilityClass.NumbersToWords((int)totalAmount);
                repParams.Add(new ReportParameter("TotalAmountInWordString", totalAmountinWordString));
                repParams.Add(new ReportParameter("PreviousDueString", inv.PreviousDue.ToString()));
                repParams.Add(new ReportParameter("CompanyName", Configs.Default.CompanyName));
                repParams.Add(new ReportParameter("ComapnyPhoneNo", Configs.Default.CompanyPhone));
                repParams.Add(new ReportParameter("CompanyAddress", Configs.Default.CompanyAddress));
                repParams.Add(new ReportParameter("CompanyEmail", Configs.Default.CompanyEmail));
                repParams.Add(new ReportParameter("CompanyFax", Configs.Default.CompanyFax));
                repParams.Add(new ReportParameter("ClientName", clc.CLNAME));
                repParams.Add(new ReportParameter("ClientAddress", clc.ADDRESS));
                repParams.Add(new ReportParameter("ClientPhoneNo", clc.CONTACTNO));
                string invoiceNo = inv.BillId;
                repParams.Add(new ReportParameter("InvoiceNumber", invoiceNo));
                BillViewer.LocalReport.ReportPath = "Report1.rdlc";
                BillViewer.LocalReport.DataSources.Clear();
                rs.Name = "DataSet1";
                BillViewer.LocalReport.DataSources.Add(rs);
                BillViewer.LocalReport.SetParameters(repParams);
                BillViewer.RefreshReport();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error opening file.");
                this.Close();
            }
        }
        void PrintMainWindow_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Dont forget to save the invoice... (Ignore if done already)");
        }
    }
}
