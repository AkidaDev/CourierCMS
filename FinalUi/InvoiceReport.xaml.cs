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
        public InvoiceReport()
        {
            InitializeComponent();
        /*    PdfReader reader = new PdfReader(filePath);
            StringBuilder text = new StringBuilder();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            }
            Lines = text.ToString();
            initializeReport(Lines);
         * */
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

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if(!File.Exists(FilePathBlock.Text))
            {
                MessageBox.Show("Unable to open file", "Error");
                return;
            }
            try
            {
                PdfReader reader = new PdfReader(FilePathBlock.Text);
                StringBuilder text = new StringBuilder();
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
                Regex reg = new Regex(@"\d+\s+([A-Za-z]\d+)\s+\d+\s+(\d{1,2}/){2}\d{2}\s+([^\d]*)(\d+\.\d*)\s+(\w{3})\s+([^\d]*\d+\.\d*)");
                var matches = reg.Matches(text.ToString());
                
                foreach(Match match in matches)
                {
                    
                    TestTextBox.AppendText("Conn No. " + match.Groups[1] + "  Dest: " + match.Groups[3] + "  Weight: " + match.Groups[4] + " Service:" + match.Groups[5] + " Amount:" + match.Groups[6] + "\n");
                }
            //    TestTextBox.AppendText(text.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error processing file. Please check if you have selected the correct file.","Error");
                Debug.WriteLine(ex.Message);
                return;
            }
        }
    }
}
