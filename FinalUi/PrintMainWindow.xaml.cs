using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        }
        public PrintMainWindow(ReportDataSource rs, List<ReportParameter> repParams, bool isMis)
        {
            InitializeComponent();
            BillViewer.LocalReport.ReportPath = "Report2.rdlc";
            BillViewer.LocalReport.DataSources.Clear();
            BillViewer.LocalReport.DataSources.Add(rs);
            BillViewer.LocalReport.SetParameters(repParams);
            BillViewer.RefreshReport();
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
                source = UtilityClass.convertToRuntimeVIew(data.OrderBy(x=>x.BookingDate).ThenBy(z=>z.ConsignmentNo).ToList());
                rs = new ReportDataSource();
                rs.Value = source;
                if (inv.Misc == null)
                    inv.Misc = 0;
                if (inv.PreviousDue == null)
                    inv.PreviousDue = 0;

                Client clc = DataSources.ClientCopy.SingleOrDefault(x => x.CLCODE == inv.ClientCode);
                List<ReportParameter> repParams = new List<ReportParameter>();
                DateTime FromDate = data.Min(x => x.BookingDate);
                DateTime ToDate = data.Max(x => x.BookingDate);
                string dateString = FromDate.ToString("dd/MM/yyyy") + " to " + ToDate.ToString("dd/MM/yyyy");
                repParams.Add(new ReportParameter("DateString", dateString));
                string descriptionString = "Total Connsignments: " + source.Count;
                repParams.Add(new ReportParameter("DescriptionString", descriptionString));
                repParams.Add(new ReportParameter("MainAmountString", String.Format("{0:0.00}",inv.Basic.ToString())));
                repParams.Add(new ReportParameter("DiscountPString", String.Format("{0:0.00}",inv.Discount)));
                repParams.Add(new ReportParameter("FuelString",String.Format("{0:0.00}",inv.Fuel)));
                repParams.Add(new ReportParameter("FuelAmount", String.Format("{0:0.00}",inv.fuelAmount.ToString())));
                repParams.Add(new ReportParameter("ServiceTaxString", String.Format("{0:0.00}",inv.STax)));
                repParams.Add(new ReportParameter("ServiceTaxAmount",  String.Format("{0:0.00}",inv.taxAmount.ToString())));
                repParams.Add(new ReportParameter("DiscountAmountString",  String.Format("{0:0.00}",inv.discountAmount.ToString())));
                repParams.Add(new ReportParameter("MiscellaneousAmountString",  String.Format("{0:0.00}",inv.Misc)));
                repParams.Add(new ReportParameter("TNC", Configs.Default.TNC));
                repParams.Add(new ReportParameter("TotalAmountString",  String.Format("{0:0.00}",inv.totalAmount.ToString())));
                repParams.Add(new ReportParameter("TotalAmountInWordString", UtilityClass.NumbersToWords((int)Math.Round(inv.totalAmount))));
                if (inv.PreviousDue == 0 || inv.PreviousDue == null)
                {
                    repParams.Add(new ReportParameter("PreviousDueString", ""));
                }
                else
                {
                    repParams.Add(new ReportParameter("PreviousDueString", String.Format("{0:0.00}", inv.PreviousDue)));
                    repParams.Add(new ReportParameter("PreviousDueCheck", "Previous Due .:"));
                }
                repParams.Add(new ReportParameter("CompanyName", Configs.Default.CompanyName));
                repParams.Add(new ReportParameter("ComapnyPhoneNo", Configs.Default.CompanyPhone));
                repParams.Add(new ReportParameter("CompanyAddress", Configs.Default.CompanyAddress));
                repParams.Add(new ReportParameter("CompanyEmail", Configs.Default.CompanyEmail));
                repParams.Add(new ReportParameter("CompanyFax", Configs.Default.CompanyFax));
                repParams.Add(new ReportParameter("TinNumber", Configs.Default.Tin ?? ""));
                repParams.Add(new ReportParameter("ClientName", clc.CLNAME));
                repParams.Add(new ReportParameter("ClientAddress", clc.ADDRESS));
                repParams.Add(new ReportParameter("ClientPhoneNo", clc.CONTACTNO));
                repParams.Add(new ReportParameter("ServiceTaxNumber", Configs.Default.ServiceTaxno??""));
           
               // repParams.Add(new ReportParameter("Tinnumber", Configs.Default.Tin));
                DateTime invDateTime;
                if (inv.BillId.Length < 14)
                    invDateTime = DateTime.ParseExact(inv.BillId, "yyyyMMddhhmm", CultureInfo.InvariantCulture);
                else
                    invDateTime = DateTime.ParseExact(inv.BillId, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
                repParams.Add(new ReportParameter("InvoiceDate", invDateTime.ToString("dd-MMM-yyyy")));
            
                repParams.Add(new ReportParameter("InvoiceNumber", inv.BillId));
                BillViewer.LocalReport.ReportPath = "Report1.rdlc";
                BillViewer.LocalReport.DataSources.Clear();
                rs.Name = "DataSet1";
                BillViewer.LocalReport.DataSources.Add(rs);
                BillViewer.LocalReport.SetParameters(repParams);
                BillViewer.LocalReport.DisplayName = inv.ClientCode + "-" + inv.BillId;
                BillViewer.RefreshReport();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error opening file.");
                this.Close();
            }
        }
        
    }
}
