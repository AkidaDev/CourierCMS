using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for SanitizingWindow.xaml
    /// </summary>
    public partial class SanitizingWindow : Window
    {
        public SanitizingWindow()
        {
            InitializeComponent();
        }
        CollectionViewSource viewSource;
        CollectionViewSource conssNumbers;
        List<RuntimeData> dataContext;
        BillingDataDataContext db;
        public SanitizingWindow(List<RuntimeData> helperObj)
        {
            db = new BillingDataDataContext();
            dataContext = helperObj.ToList();
            InitializeComponent();
            viewSource = (CollectionViewSource)FindResource("CustomerNameList");
            viewSource.Source = from client in db.Clients
                                select client.CLCODE;
            conssNumbers = (CollectionViewSource)FindResource("ConsignmentNumbers");
            conssNumbers.Source = (from id in dataContext
                                   orderby id.ConsignmentNo
                                   select id.ConsignmentNo).ToList();
            InsertionDate.SelectedDate = DateTime.Today;
            fillAllElements(ConnsignmentNumber.Text);
        }

        private void SubmitSanitizingDetails_Click(object sender, RoutedEventArgs e)
        {
            var data = dataContext.Single(x => x.ConsignmentNo == ConnsignmentNumber.Text);
            data.Weight = Double.Parse(WeightAccToDTDC.Text);
            data.FrWeight = Double.Parse(WeightAccToFranchize.Text);
            data.Amount = Decimal.Parse(Cost.Text);
            data.FrAmount = Decimal.Parse(BilledAmount.Text);
            data.Destination = Destination.Text;
            data.DestinationPin = Decimal.Parse(DestinationPin.Text);
            data.CustCode = CustomerSelected.Text;
            setNextData();
        }
        public void setNextData()
        {
            int index = ConnsignmentNumber.SelectedIndex;
            if (ConnsignmentNumber.Items.Count - 1 == index)
            {

            }
            else
            {
                ConnsignmentNumber.Text = (string)ConnsignmentNumber.Items.GetItemAt(index + 1);
                fillAllElements(ConnsignmentNumber.Text);
            }
        }

        private void ConnsignmentNumber_KeyUp(object sender, KeyEventArgs e)
        {

            fillAllElements(ConnsignmentNumber.Text);
        }
        void fillAllElements(string connsignmentNo)
        {
            RuntimeData data;
            try
            {
                data = dataContext.Single(x => x.ConsignmentNo == connsignmentNo);
                WeightAccToDTDC.Text = data.Weight.ToString();
                Cost.Text = data.Amount.ToString();
                Destination.Text = data.Destination;
                DestinationPin.Text = data.DestinationPin.ToString();
                WeightAccToFranchize.Text = data.Weight.ToString();
                BilledAmount.Text = "";
            }
            catch (Exception e)
            {
                Debug.WriteLine("No data for connsignment: " + connsignmentNo);
                Debug.WriteLine(e.Message);
            }

        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save this data into database? ", "Save Box", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Yay");
            }
            else
            {
                MessageBox.Show("Nay");
            }
        }
    }
}
