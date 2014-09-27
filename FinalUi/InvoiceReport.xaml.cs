using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
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
    /// Interaction logic for InvoiceReport.xaml
    /// </summary>
    public partial class InvoiceReport : Window
    {
        string filePath;
        public InvoiceReport(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
            string Lines;
            PdfReader reader = new PdfReader(filePath);
            StringBuilder text = new StringBuilder();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            }
            Lines = text.ToString();
            initializeReport(Lines);
        }
        private void initializeReport(string lines)
        {
            List<string> recordString = new List<string>();
            foreach (string line in lines.Split('\n'))
            {
            }
        }
    }
}
