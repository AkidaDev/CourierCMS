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
    /// Interaction logic for LoadData.xaml
    /// </summary>
    public partial class LoadData : Window
    {
        public LoadData()
        {
            isNewSheet = true;
            InitializeComponent();
            dataLoaded = false;
            isLoadedFromBook = false;
        }
        public bool dataLoaded { get; set; }
        public string filename1 { get; set; }
        public bool isNewSheet { get; set; }
        public string BookNo { get; set; }
        public bool isLoadedFromBook { get; set; }
        public DateTime toDate;
        public DateTime fromDate;
        public List<RuntimeData> data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        List<RuntimeData> _data;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dataLoaded = true;
            isLoadedFromBook = true;
            string bookNumber;
            bookNumber = BookNoTextBox.Text;
            BillingDataDataContext db = new BillingDataDataContext();
            try
            {
                Stock stock = db.Stocks.SingleOrDefault(x => x.BookNo == bookNumber);
                if (stock == null)
                {
                    MessageBox.Show("No book found", "Error");
                    return;
                }
                else
                {
                    BookNo = bookNumber;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
                return;
            }
            this.Close();
        }
        private void addData_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            if (this.checkbox_selected.Visibility == Visibility.Hidden)
            {
                this.checkbox_selected.Visibility = Visibility.Visible;
                this.checkbox_unselected.Visibility = Visibility.Hidden;
                isNewSheet = false;
            }
            else
            {
                this.checkbox_selected.Visibility = Visibility.Hidden;
                this.checkbox_unselected.Visibility = Visibility.Visible;
                isNewSheet = true;
            }
        }
        public void loaddata(object sender, RoutedEventArgs e)
        {
            if (DateRadio.IsChecked == true)
            {
                Button_Click_2(sender, e);
            }
            else
            {
                if (BookRadio.IsChecked == true)
                    Button_Click_1(sender, e);
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (StartLoadDate.SelectedDate != null && EndLoadDate.SelectedDate != null)
            {
                if (StartLoadDate.SelectedDate <= EndLoadDate.SelectedDate)
                {
                    dataLoaded = true;
                    isLoadedFromBook = false;
                    toDate = (DateTime)EndLoadDate.SelectedDate;
                    fromDate = (DateTime)StartLoadDate.SelectedDate;
                    this.Close();
                }
            }
        }
        private void DataBrowserRadio_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            if (bt.Name == "DataBaseRadioButton")
                DateRadio.IsChecked = true;
            else
                BookRadio.IsChecked = true;
        }
    }
}
