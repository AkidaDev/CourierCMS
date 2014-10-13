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
        public SanitizingWindow(List<RuntimeData> dataContext, BillingDataDataContext db, int sheetNo, DataGrid dg, RuntimeData selectedRec = null)
            : this()
        {
            this.backDataGrid = dg;
            this.sheetNo = sheetNo;
            if (dataContext != null)
                this.dataContext = dataContext;
            if (dg.ItemsSource != null)
                dataListContext = (ListCollectionView)dg.ItemsSource;
            conssNumbers = (CollectionViewSource)FindResource("ConsignmentNumbers");
            conssNumbers.Source = (from id in dataContext
                                   orderby id.BookingDate, id.ConsignmentNo
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
        public RuntimeData fillData(RuntimeData data)
        {
            if (CustomerSelected.Text == "")
                CustomerSelected.Text = "<NONE>";
            if (WeightAccToDTDC.Text == "")
                WeightAccToDTDC.Text = "0";
            if (WeightAccToFranchize.Text == "")
                WeightAccToFranchize.Text = WeightAccToDTDC.Text;
            if (BilledWeightTextBox.Text == "")
                BilledWeightTextBox.Text = WeightAccToFranchize.Text;
            if (BilledAmount.Text == "")
            {
                BilledAmount.Text = "0";
            }
            bool isDataInContext = true;
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
            double tmpD;
            if (Cost.Text == "" || !double.TryParse(Cost.Text,out tmpD))
                Cost.Text = "0";
            data.Amount = Decimal.Parse(Cost.Text);
            var c1 = db.Cities.Where(x => x.CITY_DESC == Destination.Text).Select(y => y.CITY_CODE).FirstOrDefault();
            data.Destination = c1;
            DestinationPin.Text = DestinationPin.Text ?? "";
            data.CustCode = CustomerSelected.Text;
            if(MODE.Text == "")
            data.Mode = MODE.Text;
            data.Type = TypeComboBox.Text;
            data.BookingDate = (DateTime)InsertionDate.SelectedDate;
            data.FrAmount = Decimal.Parse(BilledAmount.Text);
            if (DoxCombobox.Text == "")
                DoxCombobox.Text = "Dox";
            data.DOX = DoxCombobox.Text.ElementAt(0);
            float tempValue;
            if (float.TryParse(BilledWeightTextBox.Text, out tempValue))
                data.BilledWeight = tempValue;
            else
                BilledWeightTextBox.Text = "";
            if (isDataInContext)
            {
                data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                data.Weight = Double.Parse(WeightAccToDTDC.Text);
                data.FrWeight = Double.Parse(WeightAccToFranchize.Text);
                data.Amount = Decimal.Parse(Cost.Text);
                data.FrAmount = Decimal.Parse(BilledAmount.Text);
                data.Destination = db.Cities.Where(x => x.CITY_DESC == Destination.Text).Select(y => y.CITY_CODE).FirstOrDefault();
                if (data.Destination == null)
                {
                    MessageBoxResult rsltMessageBox = MessageBox.Show("No city with this code is entered. Do you want to enter it now?", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (MessageBoxResult.Yes == rsltMessageBox)
                    {
                        AddCity window = new AddCity();
                        window.Show();
                    }
                }
                if (DestinationPin.Text == "" || DestinationPin.Text == null)
                    data.DestinationPin = null;
                else
                    data.DestinationPin = Decimal.Parse(DestinationPin.Text);
                data.CustCode = CustomerSelected.Text;
                data.BilledWeight = float.Parse(this.BilledWeightTextBox.Text);
            }
            return data;

        }
        public void SaveData()
        {
            RuntimeData data = null;
            data = fillData(data);
            if (dataContext.Where(x => x.ConsignmentNo == data.ConsignmentNo).Count() > 0)
            {
            }

            else
            {
                
                data.SheetNo = sheetNo;
                data.UserId = SecurityModule.currentUserName;
                db.RuntimeDatas.InsertOnSubmit(data);
                dataListContext.AddNewItem(data);
            }

            if (data.FrAmount == null)
            {
                var c = db.Cities.Where(x => x.CITY_CODE == data.Destination && x.CITY_STATUS == "A").FirstOrDefault();
                if (c == null)
                    c = db.Cities.SingleOrDefault(x => x.CITY_CODE == "DEL");
                data.FrAmount = (decimal)UtilityClass.getCost(data.CustCode, (double)data.BilledWeight, data.Destination, data.Type, (char)data.DOX);
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
            if (index == 0)
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
                }
                else
                {
                    clearDetails();
                }
            }
        }
        void clearDetails()
        {
            this.WeightAccToDTDC.Text = "";
            this.Cost.Text = "";
            this.Destination.Text = "";
            this.DestinationPin.Text = "";
            this.WeightAccToFranchize.Text = "";
            this.BilledAmount.Text = "";
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
            if (data.CustCode != "" && data.CustCode != null)
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
            if (data.BilledWeight != null)
                this.BilledWeightTextBox.Text = data.BilledWeight.ToString();
            else
                this.BilledWeightTextBox.Text = "";
            if (data.Type != "" && data.Type != null)
                TypeComboBox.Text = data.Type.Trim();
            if (data.Mode != "")
                MODE.Text = data.Mode.Trim();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
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
        private void BilledWeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.GetRateButton.Visibility = Visibility.Visible;
        }
        private void GetRateButton_Click(object sender, RoutedEventArgs e)
        {
            getrate();
        }
        private void getrate()
        {
            if (this.BilledWeightTextBox.Text != null && this.BilledWeightTextBox.Text != "")
            {
                RuntimeData data = null;
                data = fillData(data);
                var c = db.Cities.Where(x => x.CITY_CODE == data.Destination && x.CITY_STATUS == "A").FirstOrDefault();
                if (c == null)
                    c = db.Cities.SingleOrDefault(x => x.CITY_CODE == "DEL");
                var d = (City)this.Destination.SelectedItem;
                double cost;
                if (d != null)
                {
                    cost =  UtilityClass.getCost(data.CustCode, (double)data.BilledWeight, data.Destination, data.Type, (char)data.DOX);
                    this.BilledAmount.Text = cost.ToString();
                }
            }
            else
            {
                this.BilledAmount.Text = "0";
            }
        }
        private void Validate_Form()
        {
        }
        private void BilledWeightTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.BilledAmount.Text == "" || this.BilledAmount.Text == null)
            {
                getrate();
            }
        }

        private void CustomerSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MODE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}