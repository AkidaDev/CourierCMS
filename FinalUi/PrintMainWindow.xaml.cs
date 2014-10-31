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
                DateTime FromDate = data.Min(x => x.BookingDate);
                DateTime ToDate = data.Max(x => x.BookingDate);
                string dateString = FromDate.ToString("dd/MM/yyyy") + " to " + ToDate.ToString("dd/MM/yyyy");
                repParams.Add(new ReportParameter("DateString", dateString));
                string descriptionString = "Total Connsignments: " + source.Count;
                repParams.Add(new ReportParameter("DescriptionString", descriptionString));
                double mainAmountValue = inv.Basic;
                repParams.Add(new ReportParameter("MainAmountString", mainAmountValue.ToString()));
                repParams.Add(new ReportParameter("FuelString",inv.Fuel.ToString()));
                double fuelAmount = inv.Fuel * mainAmountValue / 100;
                repParams.Add(new ReportParameter("FuelAmount", fuelAmount.ToString()));
                repParams.Add(new ReportParameter("ServiceTaxString", inv.STax.ToString()));
                double tax = inv.STax * mainAmountValue / 100;
                repParams.Add(new ReportParameter("ServiceTaxAmount", tax.ToString()));
                double discount = (inv.Discount??0) * mainAmountValue / 100;
                repParams.Add(new ReportParameter("DiscountPString",inv.Discount.ToString()));
                repParams.Add(new ReportParameter("DiscountAmountString", discount.ToString()));
                repParams.Add(new ReportParameter("MiscellaneousAmountString", inv.Misc.ToString()));
                repParams.Add(new ReportParameter("TNC", Configs.Default.TNC));
                double taxamount = tax + fuelAmount;
                double totalAmount = mainAmountValue + taxamount + (double)(inv.Misc??0) + (double)(inv.PreviousDue??0) - discount;
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
              
                repParams.Add(new ReportParameter("InvoiceNumber", inv.BillId));
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
