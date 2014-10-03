using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            db = new BillingDataDataContext();
            viewSource = (CollectionViewSource)FindResource("CustomerNameList");
            viewSource.Source = from client in db.Clients
                                select client.CLCODE;
            cityList = (CollectionViewSource)FindResource("CityList");
            cityList.Source = db.Cities.ToList();
            ServiceListSource = (CollectionViewSource)FindResource("ServiceList");
            ServiceListSource.Source = db.Services.ToList();
        }
        CollectionViewSource viewSource;
        CollectionViewSource conssNumbers;
        List<RuntimeData> dataContext;
        CollectionViewSource cityList;
        ListCollectionView dataListContext;
        CollectionViewSource ServiceListSource;
        BillingDataDataContext db;
        DataGrid backDataGrid;
        int sheetNo;
        public SanitizingWindow(List<RuntimeData> dataContext, BillingDataDataContext db, int sheetNo, DataGrid dg, RuntimeData selectedRec = null) :this()
        {
            this.backDataGrid = dg;
            this.sheetNo = sheetNo;
            if (dataContext != null)
                this.dataContext = dataContext;
            if (dg.ItemsSource != null)
                dataListContext = (ListCollectionView)dg.ItemsSource;
            
           
            conssNumbers = (CollectionViewSource)FindResource("ConsignmentNumbers");
            conssNumbers.Source = (from id in dataContext
                                   orderby id.BookingDate,id.ConsignmentNo
                                   select id.ConsignmentNo).ToList();
            InsertionDate.SelectedDate = DateTime.Today;
            if (selectedRec != null)
                ConnsignmentNumber.Text = selectedRec.ConsignmentNo;
            fillAllElements(ConnsignmentNumber.Text);
        }
        private void SubmitSanitizingDetails_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            setNextData();
        }
        public void SaveData()
        {
            if (BilledAmount.Text == "" || CustomerSelected.Text == "<NONE>")
                return;
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
            data.Mode = MODE.Text;
            data.Type = TypeComboBox.Text;
            data.BookingDate = (DateTime)InsertionDate.SelectedDate;
            data.DOX = DoxCombobox.Text.ElementAt(0);
            if (isDataInContext)
            {
                data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                data.Weight = Double.Parse(WeightAccToDTDC.Text);
                data.FrWeight = Double.Parse(WeightAccToFranchize.Text);
                data.Amount = Decimal.Parse(Cost.Text);
                data.FrAmount = Decimal.Parse(BilledAmount.Text);
                data.Destination = db.Cities.Where(x => x.CITY_DESC == Destination.Text).Select(y => y.CITY_CODE).FirstOrDefault();
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
                metaData.UserName = SecurityModule.currentUserName;
                db.RuntimeMetas.InsertOnSubmit(metaData);
                dataListContext.AddNewItem(data);
            }
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
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
        public void setPreviousData()
        {
            int index = ConnsignmentNumber.SelectedIndex;
            if ( index == 0)
            {
            }
            else
            {
                ConnsignmentNumber.Text = (string)ConnsignmentNumber.Items.GetItemAt(index - 1);
                fillAllElements(ConnsignmentNumber.Text);
            }
        }
        private void ConnsignmentNumber_KeyUp(object sender, KeyEventArgs e)
        {
            fillAllElements(ConnsignmentNumber.Text);
        }
        void fillAllElements(string connsignmentNo)
        {
 //           StatusTextBox.Text = "";
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
        //            StatusTextBox.Text = "This record will be added to current sheet";
                }
                else
                {
      //              StatusTextBox.Text = "New Record. This will be added in current sheet and database.";
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
            BillingDataDataContext db = new BillingDataDataContext();
            WeightAccToDTDC.Text = data.Weight.ToString();
            Cost.Text = data.Amount.ToString();
            MODE.Text = data.Mode;
            if (data.DOX == 'N')
                DoxCombobox.Text = "Non Dox";
            else
                DoxCombobox.Text = "Dox";
            if (data.BookingDate != null)
                InsertionDate.SelectedDate = data.BookingDate;
            Destination.Text = db.Cities.Where(x => x.CITY_CODE == data.Destination).Select(y => y.CITY_DESC).FirstOrDefault();
            if (data.CustCode != "")
                CustomerSelected.Text = data.CustCode;
            else
                CustomerSelected.Text = "<NONE>";
            DestinationPin.Text = data.DestinationPin.ToString();
            if (data.FrWeight != null)
                WeightAccToFranchize.Text = data.FrWeight.ToString();
            else
                WeightAccToFranchize.Text = data.Weight.ToString();
            if (data.FrAmount != null)
                BilledAmount.Text = data.FrAmount.ToString();
            else
                BilledAmount.Text = "";
            if (data.Type != "")
                TypeComboBox.Text = data.Type.Trim();
            if (data.Mode != "")
                MODE.Text = data.Mode.Trim();
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
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            setPreviousData();
        }

    }
}