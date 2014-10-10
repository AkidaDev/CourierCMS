using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for BillReportWindow.xaml
    /// </summary>
    public partial class BillReportWindow : Window
    {
        public BillReportWindow()
        {
            InitializeComponent();
        }
        string InvoiceNumber = "";
        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? toDate = null;
            if (ToDate.SelectedDate != null)
                toDate = (DateTime)ToDate.SelectedDate;
            DateTime? fromDate = null;
            if (FromDate.SelectedDate != null)
                fromDate = (DateTime)FromDate.SelectedDate;
            if (toDate != null && fromDate != null)
            {

                InvoiceNumber = InvoiceNumberTextBox.Text;
                if (InvoiceNumber == "")
                {
                    MessageBox.Show("Invoice Number not entered");
                    return;
                }
                if (!File.Exists(FilePathTextBox.Text))
                {
                    MessageBox.Show("Selected file doesn't exists.");
                    return;
                }
                if (toDate >= fromDate)
                {
                    analyzeBill();
                }
            }
        }
        private void analyzeBill()
        {
            string fileName;
            fileName = FilePathTextBox.Text;
            fileName = fileName.Trim('\"');
            PdfReader reader = new PdfReader(fileName);
            List<string> lines = new List<string>();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                ITextExtractionStrategy its = new LocationTextExtractionStrategy();
                string[] pageLines = PdfTextExtractor.GetTextFromPage(reader, i, its).Split('\n');
                lines.AddRange(pageLines.ToList());
            }
            Regex regex = new Regex(@"^\s[\d]{1,}");
            List<string> matchedLines = new List<string>();
            List<RuntimeData> matchedRecs = new List<RuntimeData>();
            BillingDataDataContext db = new BillingDataDataContext();
            List<City> cities = db.Cities.ToList();
            List<string> discrepantRecords = new List<string>();
            lines.ForEach((x) =>
            {
                if (regex.Match(x).Success)
                {
                    int index = 0;
                    string[] terms = Regex.Replace(x, @"\s{1,}", " ").Trim().Split(' ');
                    matchedLines.Add(x);
                    RuntimeData runtimeData = new RuntimeData();
                    foreach (string item in terms)
                    {
                        if (!Regex.IsMatch(item, @"\D\d{1,}"))
                            index++;
                        else
                            break;
                    }
                    try
                    {
                        runtimeData.ConsignmentNo = terms[index + 0];
                        runtimeData.TransMF_No = terms[index + 1];
                        runtimeData.BookingDate = DateTime.ParseExact(terms[index + 2].Replace("\'", ""), "dd/MM/yy", new CultureInfo("en-US")).Date;
                        runtimeData.Destination = terms[index + 3];
                        double temp;
                        if (double.TryParse(terms[index + 4], out temp))
                            runtimeData.Weight = temp;

                        else
                        {
                            index++;
                            runtimeData.Destination += terms[index + 3];
                            runtimeData.Weight = double.Parse(terms[index + 4]);
                        }
                        runtimeData.Type = terms[index + 5];
                        runtimeData.Amount = decimal.Parse(terms[index + 6]);
                        matchedRecs.Add(runtimeData);
                    }
                    catch (Exception )
                    {
                        discrepantRecords.Add(string.Join(",", terms.Where((y, i) => i <= index + 6)));
                    }
                    if (terms.Length > index + 7)
                    {
                        foreach (string item in terms.Skip(index + 7))
                        {
                            if (!Regex.IsMatch(item, @"\D\d{1,}"))
                                index++;
                            else
                                break;
                        }
                    
                        double temp;

                        runtimeData = new RuntimeData();
                        try
                        {
                            runtimeData.ConsignmentNo = terms[index + 7];
                            runtimeData.TransMF_No = terms[index + 8];
                            runtimeData.BookingDate = DateTime.ParseExact(terms[index + 9].Replace("\'", ""), "dd/MM/yy", new CultureInfo("en-US")).Date;
                            runtimeData.Destination = terms[index + 10];
                            if (double.TryParse(terms[index + 11], out temp))
                                runtimeData.Weight = temp;
                            else
                            {
                                index++;
                                runtimeData.Destination = runtimeData.Destination + terms[index + 10];
                                runtimeData.Weight = double.Parse(terms[index + 11]);
                            }
                            runtimeData.Type = terms[index + 12];
                            runtimeData.Amount = decimal.Parse(terms[index + 13].Trim(), NumberStyles.Float);
                            matchedRecs.Add(runtimeData);
                        }
                        catch (Exception)
                        {
                            discrepantRecords.Add(string.Join(",", terms.Where((y, i) => i >= index + 6)));
                        }
                    }
                }
            });
            MessageBox.Show("Total lines: " + lines.Count.ToString());
            MessageBox.Show("Discrepant Records: " + discrepantRecords.Count);
            string writeFilePath = @"D:\test.txt";
            writeFilePath = writeFilePath.Trim('\"');
            matchedLines.Clear();
            matchedRecs.ForEach((x) =>
            {
                matchedLines.Add(string.Join(",", x.ConsignmentNo, x.Destination, x.Type, x.Weight, x.Amount));
            });
            File.WriteAllLines(writeFilePath, matchedLines);
            Dictionary<RuntimeData, string> analyzingResults = new Dictionary<RuntimeData, string>();
            DateTime toDate = DateTime.Today;
            DateTime fromDate = new DateTime(2014, 4, 1);
            MessageBox.Show("Analyzing...");
            analyzeData(matchedRecs, analyzingResults, toDate, fromDate);
            MessageBox.Show("Analyzing Done");

            matchedLines.Clear();
            analyzingResults.Keys.ToList().ForEach((x) =>
            {
                matchedLines.Add(string.Join(",", x.ConsignmentNo, x.Destination, x.Type, x.Weight, x.Amount, analyzingResults[x]));
            });
            foreach (string line in matchedLines)
            {
                ReportBox.AppendText("\n" + line);
            }
        }
        private void analyzeData(List<RuntimeData> matchedRecs, Dictionary<RuntimeData, string> analyzingResults, DateTime toDate, DateTime fromDate)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            List<FinalUi.RuntimeData> loadedData = FinalUi.UtilityClass.loadDataFromDatabase(fromDate, toDate);
            /*Basic Checks*/
            List<RuntimeData> dupData = matchedRecs.Where(x => matchedRecs.Where(y => y.ConsignmentNo == x.ConsignmentNo).Count() > 1).ToList();
            dupData.ForEach((x) =>
            {
                analyzingResults.Add(x, "Duplicate Record");
            });
            foreach (RuntimeData data in matchedRecs)
            {
                FinalUi.RuntimeData origData = loadedData.SingleOrDefault(x => x.ConsignmentNo == data.ConsignmentNo);
                if (origData == null)
                {
                    addToAnalyzingResults(analyzingResults, data, "No corresponding record found in original transaction data");
                    continue;
                }
                if (origData.InvoiceNo != InvoiceNumber)
                    addToAnalyzingResults(analyzingResults, data, "Invoice Number doesn't match the entered invoice number");
                if (origData.Weight != data.Weight)
                    addToAnalyzingResults(analyzingResults, data, "Weight does not match with original weight");
                if (origData.FrWeight != data.Weight && origData.FrWeight != null)
                    addToAnalyzingResults(analyzingResults, data, "Weight does not match with observed weight");
                if (origData.Amount != data.Amount)
                    addToAnalyzingResults(analyzingResults, data, "Amount does not match with original results");
                if (origData.Type.Trim() != data.Type.Trim())
                    addToAnalyzingResults(analyzingResults, data, "Type does not match with original type");

                City city = db.Cities.FirstOrDefault(x => x.CITY_CODE == origData.Destination && x.CITY_STATUS == "A");
                if (city == null)
                    addToAnalyzingResults(analyzingResults, data, "No such destination found. Original destination is " + origData.Destination);
                else
                {
                    if (city.CITY_DESC.Replace(" ", "") != data.Destination)
                        addToAnalyzingResults(analyzingResults, data, "Destination does not match. Original destination is " + origData.Destination);
                }
            }
        }
        public void addToAnalyzingResults(Dictionary<RuntimeData, string> analyzingResults, RuntimeData data, string value)
        {
            if (analyzingResults.ContainsKey(data))
                analyzingResults[data] = analyzingResults[data] + ". " + value;
            else
                analyzingResults.Add(data, value);
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".pdf";
            dialog.Filter = "(.pdf)|*.pdf";
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                FilePathTextBox.Text = dialog.FileName;
            }
        }
    }
}
