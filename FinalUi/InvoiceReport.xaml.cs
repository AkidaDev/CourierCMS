using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for InvoiceReport.xaml
    /// </summary>
    public partial class InvoiceReport : Window
    {
        BackgroundWorker bgWorker;
        List<string> unreadableData;
        IQueryable<InvoiceAnalyzeResult> Results;
        public InvoiceReport()
        {
            InitializeComponent();
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CollectionViewSource ResultCollection = (CollectionViewSource)FindResource("ResultData");
            ResultCollection.Source = Results.ToList();
            MessageBox.Show("Analysis done", "Info");
            try
            {
                BillingDataDataContext db = new BillingDataDataContext();
                db.ExecuteCommand("delete from invoiceanalyzeresult");
            }
            catch(Exception)
            { }
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Analyzeprogress.Value = e.ProgressPercentage;
            if (Analyzeprogress.Value == 100 || Analyzeprogress.Value == 0)
            {
                Analyzeprogress.Visibility = Visibility.Hidden;
            }
           else
           {
               Analyzeprogress.Visibility = Visibility.Visible;
           }
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            unreadableData = new List<string>();
            Guid Id = Guid.NewGuid();
            BillingDataDataContext db = new BillingDataDataContext();
            MatchCollection matches;
            double progress = 0;
            try
            {
                PdfReader reader = new PdfReader((string)e.Argument);
                StringBuilder text = new StringBuilder();
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
                Regex reg = new Regex(@"(\d+)\s+([A-Za-z]\d+)\s+\d+\s+(\d{1,2}/){2}\d{2}\s+([^\d]*)(\d+\.\d*)\s+(\w{3})\s+([^\d]*\d+\.\d*)");
                matches = reg.Matches(text.ToString());
                progress = 5;
                bgWorker.ReportProgress((int)progress);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Unable to parse invoice");
            }
            double count = matches.Count, ctr = 0;
            foreach (Match match in matches)
            {
                InvoiceAnalyzeResult data = new InvoiceAnalyzeResult();
                double temp;
                bool isValid = true;
                data.AnalyzeId = Id;
                data.Id = Guid.NewGuid();
                data.SrlNo = match.Groups[1].Value;
                data.ConnNo = match.Groups[2].Value;
                data.Destination = match.Groups[4].Value;
                if (double.TryParse(match.Groups[5].Value, out temp) == true)
                    data.Weight = temp;
                else
                    isValid = false;
                data.serviceCode = match.Groups[6].Value;
                if (double.TryParse(match.Groups[7].Value, out temp) == true)
                    data.Amount = temp;
                else
                    isValid = false;
                if (isValid)
                {
                    db.InvoiceAnalyzeResults.InsertOnSubmit(data);
                    db.SubmitChanges();
                }
                else
                    unreadableData.Add(match.Groups[1].Value);
                ctr++;
                bgWorker.ReportProgress((int)(((ctr / count) * 50) + progress));
            }
            Results = db.InvoiceAnalyzeResults.Where(x => x.AnalyzeId == Id);
            List<TransactionCityView> Transactions = (from transaction in db.TransactionCityViews
                                                      join result in Results
                                                      on transaction.ConnsignmentNo equals result.ConnNo
                                                      select transaction).ToList();
            progress = 60;
            bgWorker.ReportProgress((int)progress);
            count = Results.Count();
            ctr = 0;
            db.UpdateBillingAmount();
            foreach (InvoiceAnalyzeResult result in Results)
            {
                TransactionCityView trans = Transactions.SingleOrDefault(x => x.ConnsignmentNo == result.ConnNo);
                result.MisMatchDesc = "";
                try
                {
                    if (trans == null)
                    {
                        result.hasError = true;
                        result.MisMatchDesc = "Transaction not found";
                        continue;
                    }

                    if (trans.WeightByFranchize != result.Weight)
                    {
                        result.MisMatchDesc = "Weight should be " + trans.WeightByFranchize;
                        result.WeightDif = (decimal)((result.Weight ?? 0) - (trans.WeightByFranchize ?? 0));
                    }
                    if (trans.CITY_DESC.Trim() != result.Destination.Trim())
                    {
                        result.MisMatchDesc = result.MisMatchDesc + " Destination should be " + trans.CITY_DESC;
                    }
                    if (trans.Type.Trim() != result.serviceCode.Trim())
                    {
                        result.MisMatchDesc = result.MisMatchDesc + " Service should be " + trans.Type.Trim();
                    }
                    trans.AmountCharged = (decimal)UtilityClass.getCost("<DTDC>", (double)trans.WeightByFranchize, trans.Destination.Trim(), trans.Type.Trim(), trans.DOX);
                    if (Math.Abs((trans.AmountCharged - (decimal)result.Amount)??0) > 2)
                    {
                        result.hasError = true;
                        result.MisMatchDesc = result.MisMatchDesc + " Amount should be " + Math.Round((trans.AmountCharged??0),2);
                        result.AmountDiff = (decimal)(result.Amount??0) - (decimal)(trans.AmountCharged??0);
                    }
                }
                catch(Exception ex)
                {
                    result.hasError = true;
                    result.MisMatchDesc = result.MisMatchDesc + " Unable to process record:  " + ex.Message;
                }
                ctr++;
                bgWorker.ReportProgress((int)(((ctr / count) * 30) + progress));
            }
            bgWorker.ReportProgress(100);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".pdf";
            dialog.Filter = "(.pdf)|*.pdf";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                FilePathBlock.Text = dialog.FileName;
            }
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(FilePathBlock.Text))
            {
                MessageBox.Show("Unable to open file", "Error");
                return;
            }
            if (bgWorker.IsBusy)
                MessageBox.Show("An analysis is already in progress");
            else
                bgWorker.RunWorkerAsync(FilePathBlock.Text);
        }
    }
}
