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
        int sheetNo;
        public SanitizingWindow(List<RuntimeData> helperObj, BillingDataDataContext db, int sheetNo)
        {
            this.sheetNo = sheetNo;
            this.db = db;
            if (helperObj != null)
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
            bool isDataInContext = true;
          
            RuntimeData data;
            data = dataContext.SingleOrDefault(x => x.ConsignmentNo == ConnsignmentNumber.Text);
            if (data == null)
            {
                isDataInContext = false;
                var TData = db.Transactions.SingleOrDefault(x => x.ConnsignmentNo == ConnsignmentNumber.Text);
                if (TData == null)
                {
                    data = new RuntimeData();
                    data.Id = Guid.NewGuid();
                    data.ConsignmentNo = ConnsignmentNumber.Text;
                }
                else
                {
                    data = UtilityClass.convertTransObjToRunObj(TData);
                }
            }
            data.Weight = Double.Parse(WeightAccToDTDC.Text);
            data.FrWeight = Double.Parse(WeightAccToFranchize.Text);
            data.Amount = Decimal.Parse(Cost.Text);
            data.FrAmount = Decimal.Parse(BilledAmount.Text);
            data.Destination = Destination.Text;
            data.DestinationPin = Decimal.Parse(DestinationPin.Text);
            data.CustCode = CustomerSelected.Text;
            if (isDataInContext)
            {
                data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                data.Weight = Double.Parse(WeightAccToDTDC.Text);
                data.FrWeight = Double.Parse(WeightAccToFranchize.Text);
                data.Amount = Decimal.Parse(Cost.Text);
                data.FrAmount = Decimal.Parse(BilledAmount.Text);
                data.Destination = Destination.Text;
                data.DestinationPin = Decimal.Parse(DestinationPin.Text);
                data.CustCode = CustomerSelected.Text;
            }
            else
            {
                db.RuntimeDatas.InsertOnSubmit(data);
                RuntimeMeta metaData = new RuntimeMeta();
                metaData.Id = Guid.NewGuid();
                metaData.RuntimeDataID = data.Id;
                metaData.SheetNo = sheetNo;
                metaData.UserName  = SecurityModule.currentUserName;
                db.RuntimeMetas.InsertOnSubmit(metaData);
                dataContext.Add(data);
            }
            db.SubmitChanges();
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
            StatusTextBox.Text = "";
            RuntimeData data;
            data = dataContext.SingleOrDefault(x => x.ConsignmentNo == connsignmentNo);
            if (data != null)
            {
                fillDetails(data);
            }
            else
            {
                var TData = db.Transactions.SingleOrDefault(x => x.ConnsignmentNo == connsignmentNo);
                if (TData != null)
                {
                    fillDetails(UtilityClass.convertTransObjToRunObj(TData));

                    StatusTextBox.Text = "This record will be added to current sheet";
                }
                else
                {
                    StatusTextBox.Text = "New Record. This will be added in current sheet and database.";
                    clearDetails();
                }

            }
        }
        void clearDetails()
        {
            WeightAccToDTDC.Text = "";
            Cost.Text = "";
            Destination.Text = "";
            DestinationPin.Text = "";
            WeightAccToFranchize.Text = "";
            BilledAmount.Text = "";
        }
        public void fillDetails(RuntimeData data)
        {
            WeightAccToDTDC.Text = data.Weight.ToString();
            Cost.Text = data.Amount.ToString();
            Destination.Text = data.Destination;
            DestinationPin.Text = data.DestinationPin.ToString();
            if (data.FrWeight != null)
                WeightAccToFranchize.Text = data.FrWeight.ToString();
            else
                WeightAccToFranchize.Text = data.Weight.ToString();
            if (data.FrAmount != null)
                BilledAmount.Text = data.FrAmount.ToString();
            else
                BilledAmount.Text = "";

        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            //  this.Owner.Effect = null;
            this.Close();
        }

        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
