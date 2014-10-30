using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
            viewSource.Source = DataSources.ClientCopy.ToList();
            cityList = (CollectionViewSource)FindResource("CityList");
            cityList.Source = DataSources.CityCopy;
            ServiceListSource = (CollectionViewSource)FindResource("ServiceList");
            ServiceListSource.Source = DataSources.ServicesCopy;
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
            try
            {
                SaveData();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show("Data will not be saved. Error: " + ex.Message + ". Continue?", "Error", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
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
            if (Destination.Text == "" && Destination.Text == null)
            {
                MessageBox.Show("City cannot be empty");
            }
            data = dataContext.SingleOrDefault(x => x.ConsignmentNo == ConnsignmentNumber.Text);
            if (data == null)
            {
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
            if (Cost.Text == "" || !double.TryParse(Cost.Text, out tmpD))
                Cost.Text = "0";
            data.Amount = Decimal.Parse(Cost.Text);
            var c1 = DataSources.CityCopy.Where(x => x.NameAndCode == Destination.Text).Select(y => y.CITY_CODE).FirstOrDefault();
            data.Destination = c1;
            decimal tempDecimal;
            if (decimal.TryParse(DestinationPin.Text, out tempDecimal))
                data.DestinationPin = tempDecimal;
            data.CustCode = DataSources.ClientCopy.Where(x => x.NameAndCode == CustomerSelected.Text).Select(y => y.CLCODE).FirstOrDefault();
            if (MODE.Text != "")
                data.Mode = MODE.Text;
            data.Type = DataSources.ServicesCopy.Where(x => x.NameAndCode == TypeComboBox.Text).Select(y => y.SER_CODE).FirstOrDefault();
            data.BookingDate = (DateTime)InsertionDate.SelectedDate;
            data.FrAmount = Decimal.Parse(BilledAmount.Text);
            if (DoxCombobox.Text == "")
                DoxCombobox.Text = "Dox";
            data.DOX = DoxCombobox.Text.ElementAt(0);
            float tempValue;
            if (float.TryParse(BilledWeightTextBox.Text, out tempValue))
                data.BilledWeight = double.Parse(BilledWeightTextBox.Text, CultureInfo.InvariantCulture);
            else
                BilledWeightTextBox.Text = "";
            if (data.Destination == null)
            {
                MessageBoxResult rsltMessageBox = MessageBox.Show("No city with this code is entered. Data will not be entered if city is not present. Do you want to enter it now?", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                if (MessageBoxResult.Yes == rsltMessageBox)
                {
                    AddCity window = new AddCity();
                    window.Show();
                }
                else
                    return null;
            }
            data.CustCode = DataSources.ClientCopy.Where(x => x.NameAndCode == CustomerSelected.Text).Select(y => y.CLCODE).FirstOrDefault();
            if (this.BilledWeightTextBox.Text == "" || this.BilledWeightTextBox.Text == null)
            {
                if (this.WeightAccToFranchize.Text == "" || this.WeightAccToFranchize == null)
                    data.BilledWeight = 0;
                else
                    data.BilledWeight = data.FrWeight;
            }
            else
            {
                if (float.TryParse(BilledWeightTextBox.Text, out tempValue))
                    data.BilledWeight = double.Parse(BilledWeightTextBox.Text, CultureInfo.InvariantCulture);
            }

            return data;
        }
        public void dupliData(RuntimeData sData, RuntimeData dData)
        {
            dData.Amount = sData.Amount;
            dData.BilledWeight = sData.BilledWeight;
            dData.BookingDate = sData.BookingDate;
            dData.City_Desc = sData.City_Desc;
            dData.Client_Desc = sData.Client_Desc;
            dData.ConsignmentNo = sData.ConsignmentNo;
            dData.CustCode = sData.CustCode;
            dData.Destination = sData.Destination;
            dData.DestinationPin = sData.DestinationPin;
            dData.DOX = sData.DOX;
            dData.EmpId = sData.EmpId;
            dData.FrAmount = sData.FrAmount;
            dData.FrWeight = sData.FrWeight;
            dData.InvoiceDate = sData.InvoiceDate;
            dData.InvoiceNo = sData.InvoiceNo;
            dData.Mode = sData.Mode;
            dData.Service_Desc = sData.Service_Desc;
            dData.ServiceTax = sData.ServiceTax;
            dData.SheetNo = sData.SheetNo;
            dData.SplDisc = sData.SplDisc;
            dData.TransactionId = sData.TransactionId;
            dData.TransMF_No = sData.TransMF_No;
            dData.Type = sData.Type;
            dData.UserId = sData.UserId;
            dData.Weight = sData.Weight;

        }
        public void SaveData()
        {

            BillingDataDataContext db = new BillingDataDataContext();
            RuntimeData data = null;
            data = fillData(data);
            if (data == null)
                throw new Exception("Details not present");
            data.SheetNo = sheetNo;
            data.UserId = SecurityModule.currentUserName;

            if (dataContext.Where(x => x.ConsignmentNo == data.ConsignmentNo).Count() > 0)
            {
                RuntimeData origData = db.RuntimeDatas.SingleOrDefault(x => x.Id == data.Id);
                dupliData(data, origData);
            }
            else
            {
                db.RuntimeDatas.InsertOnSubmit(data);
                dataContext.Add(data);
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
            ConnsignmentNumber.Focus();
        }
        public void setdata(RuntimeData dbdata, RuntimeData data)
        {
        }
        public void setPreviousData()
        {
            int index = ConnsignmentNumber.SelectedIndex;
            if (index == 0 || index == -1)
            {
            }
            else
            {
                if (index - 1 > ConnsignmentNumber.Items.Count - 1)
                    index = 1;
                ConnsignmentNumber.Text = (string)ConnsignmentNumber.Items.GetItemAt(index - 1);
                fillAllElements(ConnsignmentNumber.Text);
            }
            ConnsignmentNumber.Focus();
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
                if (backDataGrid != null)
                {
                    if (backDataGrid.Items.Contains(data))
                    {
                        backDataGrid.SelectedItem = data;
                        DataGridRow row = (DataGridRow)backDataGrid.ItemContainerGenerator.ContainerFromItem(data);
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        this.Focus();
                    }
                    else
                    {
                        backDataGrid.SelectedItems.Clear();
                    }
                }
                fillDetails(data);
            }
            else
            {
                if (backDataGrid != null)
                    backDataGrid.SelectedItems.Clear();
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
            Destination.Text = db.Cities.Where(x => x.CITY_CODE == data.Destination).Select(y => y.NameAndCode).FirstOrDefault();
            if (data.CustCode != "" && data.CustCode != null)
                CustomerSelected.Text = DataSources.ClientCopy.Where(x => x.CLCODE == data.CustCode).Select(y => y.NameAndCode).FirstOrDefault();
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
                TypeComboBox.Text = DataSources.ServicesCopy.Where(x => x.SER_CODE == data.Type.Trim()).Select(y => y.NameAndCode).FirstOrDefault();
            if (data.Mode != "" && data.Mode != null)
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
            try
            {
                SaveData();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show("Data will not be saved. Error: " + ex.Message + ". Continue?", "Error", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
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
            if (Destination.Text == "" || Destination.Text == null)
            {
                return;
            }
            if (this.BilledWeightTextBox.Text != null && this.BilledWeightTextBox.Text != "")
            {
                RuntimeData data = null;
                data = fillData(data);
                if(data.Destination == null)
                {
                    return;
                }
                var c = db.Cities.Where(x => x.CITY_CODE == data.Destination && x.CITY_STATUS == "A").FirstOrDefault();
                if (c == null)
                    c = db.Cities.SingleOrDefault(x => x.CITY_CODE == "DEL");
                var d = (City)this.Destination.SelectedItem;
                double cost;
                if (d != null)
                {
                    cost = UtilityClass.getCost(data.CustCode, (double)data.BilledWeight, data.Destination, data.Type, (char)data.DOX);
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