using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
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
        string filePath;
        List<string> unreadableData;
        IQueryable<InvoiceAnalyzeResult> Results;
        public InvoiceReport()
        {
            InitializeComponent();
        }
        private void initializeReport(string lines)
        {
            List<string> recordString = new List<string>();
            foreach (string line in lines.Split('\n'))
            {
            }
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
        public void AnalyzeResults(string filePath)
        {
            unreadableData = new List<string>();
            Guid Id = Guid.NewGuid();
            BillingDataDataContext db = new BillingDataDataContext();
            MatchCollection matches;

            try
            {
                PdfReader reader = new PdfReader(filePath);
                StringBuilder text = new StringBuilder();
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
                Regex reg = new Regex(@"(\d+)\s+([A-Za-z]\d+)\s+\d+\s+(\d{1,2}/){2}\d{2}\s+([^\d]*)(\d+\.\d*)\s+(\w{3})\s+([^\d]*\d+\.\d*)");
                matches = reg.Matches(text.ToString());
                MessageBox.Show("Regex Parsing Done");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Unable to parse invoice");
            }
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
            }
            MessageBox.Show("Insertion of data done");
            //    TestTextBox.AppendText(text.ToString());
            Results = db.InvoiceAnalyzeResults.Where(x => x.AnalyzeId == Id);
            List<TransactionCityView> Transactions = (from transaction in db.TransactionCityViews
                                                      join result in Results
                                                      on transaction.ConnsignmentNo equals result.ConnNo
                                                      select transaction).ToList();
            MessageBox.Show("Selection of data done");
            foreach (InvoiceAnalyzeResult result in Results)
            {
                TransactionCityView trans = Transactions.SingleOrDefault(x => x.ConnsignmentNo == result.ConnNo);
                result.MisMatchDesc = "";
                if (trans == null)
                {
                    result.hasError = true;
                    result.MisMatchDesc = "Transaction not found";
                    continue;
                }

                if (trans.WeightByFranchize != result.Weight)
                {
                    result.hasError = true;
                    result.MisMatchDesc = "Weight should be " + trans.WeightByFranchize;
                }
                if (trans.CITY_DESC != result.Destination)
                {
                    result.hasError = true;
                    result.MisMatchDesc = result.MisMatchDesc + ", Destination should be " + trans.CITY_DESC;
                }
                if (trans.Type != result.serviceCode)
                {
                    result.hasError = true;
                    result.MisMatchDesc = result.MisMatchDesc + ", Service should be " + trans.Type;
                }
                trans.AmountCharged = (decimal)UtilityClass.getCost("<DTDC>", (double)trans.WeightByFranchize, trans.Destination, trans.Type, trans.DOX);
                if (trans.AmountCharged != (decimal)result.Amount)
                {
                    result.hasError = true;
                    result.MisMatchDesc = result.MisMatchDesc + ", Amount should be " + trans.AmountCharged;
                }
            }
            MessageBox.Show("Analysis done");
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(FilePathBlock.Text))
            {
                MessageBox.Show("Unable to open file", "Error");
                return;
            }
            AnalyzeResults(FilePathBlock.Text);
            AnalysisDone();
        }
        private void AnalysisDone()
        {
            CollectionViewSource ResultCollection = (CollectionViewSource)FindResource("ResultData");
            ResultCollection.Source = Results;
        }
    }
}
