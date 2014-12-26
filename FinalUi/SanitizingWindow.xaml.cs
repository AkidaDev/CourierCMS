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
        CollectionViewSource ConsigneeListSource;
        CollectionViewSource ConsignerListSource;
        CollectionViewSource SubClientListSource;
        DataGrid backDataGrid;
        DataGridHelper helper;
        Dictionary<string, List<string>> SubClientList;
        List<string> ConsigneeList;
        List<string> ConsignerList;
        int sheetNo;
        public SanitizingWindow(List<RuntimeData> dataContext, BillingDataDataContext db, int sheetNo, DataGrid dg, DataGridHelper helper, Dictionary<string, List<string>> SubClientList, List<string> ConsigneeList, List<string> ConsignerList, RuntimeData selectedRec = null)
            : this()
        {
            ConsigneeListSource = (CollectionViewSource)FindResource("ConsigneeListSource");
            ConsignerListSource = (CollectionViewSource)FindResource("ConsignerListSource");
            SubClientListSource = (CollectionViewSource)FindResource("SubClientListSource");
            this.SubClientList = SubClientList;
            this.ConsigneeList = ConsigneeList;
            this.ConsignerList = ConsignerList;
            ConsignerListSource.Source = this.ConsignerList;
            ConsigneeListSource.Source = this.ConsigneeList;
            string SelectedClientCode = ((Client)CustomerSelected.SelectedItem).CLCODE;
            SubClientListSource.Source = SubClientList.ContainsKey(SelectedClientCode) ? SubClientList[SelectedClientCode] : null;
            this.helper = helper;
            this.backDataGrid = dg;
            this.sheetNo = sheetNo;
            if (dataContext != null)
                this.dataContext = dataContext;
            if (dg.ItemsSource != null)
                dataListContext = (ListCollectionView)dg.ItemsSource;
            conssNumbers = (CollectionViewSource)FindResource("ConsignmentNumbers");
            conssNumbers.Source = (from id in dataContext
                                   orderby id.ConsignmentNo
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
            List<Client> clientList = DataSources.ClientCopy;
            List<City> cityList = DataSources.CityCopy;
            List<Service> serviceList = DataSources.ServicesCopy;
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
            var c1 = cityList.Where(x => x.NameAndCode == Destination.Text).FirstOrDefault();
            if (c1 != null)
            {
                data.Destination = c1.CITY_CODE;
                data.City_Desc = c1.CITY_DESC;
            }
            decimal tempDecimal;
            if (decimal.TryParse(DestinationPin.Text, out tempDecimal))
                data.DestinationPin = tempDecimal;
            data.CustCode = DataSources.ClientCopy.Where(x => x.NameAndCode == CustomerSelected.Text).Select(y => y.CLCODE).FirstOrDefault();
            if (MODE.Text != "")
                data.Mode = MODE.Text;
            Service service = serviceList.Where(x => x.NameAndCode == TypeComboBox.Text).FirstOrDefault();
            data.Type = service.SER_CODE;
            data.Service_Desc = service.SER_DESC;
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
            data.CustCode = clientList.Where(x => x.NameAndCode == CustomerSelected.Text).Select(y => y.CLCODE).FirstOrDefault();
            if (data.CustCode != null)
            {
                data.Client_Desc = clientList.Where(x => x.NameAndCode == CustomerSelected.Text).Select(y => y.CLNAME).FirstOrDefault();
            }
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
            data.ConsigneeName = ConsgineeName.Text;
            data.ConsigneeAddress = ConsigneeAddress.Text;
            data.ConsignerName = ConsignerName.Text;
            data.ConsignerAddress = ConsignerAddress.Text;
            data.SubClient = SubClientComboBox.Text;
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
            dData.Client_Desc = sData.Client_Desc;
            dData.ConsigneeName = sData.ConsigneeName;
            dData.ConsignerAddress = sData.ConsignerAddress;
            dData.ConsignerName = sData.ConsignerName;
            dData.ConsigneeAddress = sData.ConsigneeAddress;
            dData.SubClient = sData.SubClient;
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
            if (!SubClientList.ContainsKey(data.CustCode))
            {
                SubClientList.Add(data.CustCode, new List<string>() { data.SubClient });
            }
            else
            {
                if (!SubClientList[data.CustCode].Contains(data.SubClient))
                {
                    SubClientList[data.CustCode].Add(data.SubClient);
                }
            }
            if (!ConsignerList.Contains(data.ConsignerName))
                ConsignerList.Add(data.ConsignerName);
            if (!ConsigneeList.Contains(data.ConsigneeName))
                ConsigneeList.Add(data.ConsigneeName);
            if (dataContext.Where(x => x.ConsignmentNo == data.ConsignmentNo).Count() > 0)
            {
                RuntimeData origData = db.RuntimeDatas.SingleOrDefault(x => x.Id == data.Id);
                dupliData(data, origData);
            }
            else
            {
                db.RuntimeDatas.InsertOnSubmit(data);
                dataContext.Add(data);
                helper.addRecordToCurrentSheet(data);
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
            int connsigmentNo = int.Parse(new string(ConnsignmentNumber.Text.ToCharArray().Where(x => char.IsDigit(x)).ToArray()));
            connsigmentNo++;
            string start = new string(ConnsignmentNumber.Text.ToCharArray().Where(x => char.IsLetter(x)).ToArray());
            start = start + connsigmentNo.ToString();
            ConnsignmentNumber.Text = start;
            fillAllElements(ConnsignmentNumber.Text);
            ConnsignmentNumber.Focus();
        }
        public void setdata(RuntimeData dbdata, RuntimeData data)
        {
        }
        public void setPreviousData()
        {
            int index = ConnsignmentNumber.SelectedIndex;
            int connsigmentNo = int.Parse(new string(ConnsignmentNumber.Text.ToCharArray().Where(x => char.IsDigit(x)).ToArray()));
            connsigmentNo--;
            string start = new string(ConnsignmentNumber.Text.ToCharArray().Where(x => char.IsLetter(x)).ToArray());
            start = start + connsigmentNo.ToString();
            ConnsignmentNumber.Text = start;
            fillAllElements(ConnsignmentNumber.Text);

            ConnsignmentNumber.Focus();
        
        }
        private void ConnsignmentNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
                        DataGridRow row;
                        try
                        {
                            row = (DataGridRow)backDataGrid.ItemContainerGenerator.ContainerFromItem(data);
                            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error....");
                        }

                    }
                    else
                    {
                        backDataGrid.SelectedItems.Clear();
                    }
                }
                this.Focus();
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
            Destination.Text = DataSources.CityCopy.Where(x => x.CITY_CODE == data.Destination).Select(y => y.NameAndCode).FirstOrDefault();
            if (data.CustCode != "" && data.CustCode != null && data.CustCode != "<NONE>")
                CustomerSelected.Text = DataSources.ClientCopy.Where(x => x.CLCODE == data.CustCode).Select(y => y.NameAndCode).FirstOrDefault();
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
                this.BilledWeightTextBox.Text = data.Weight.ToString();
            if (data.Type != "" && data.Type != null)
                TypeComboBox.Text = DataSources.ServicesCopy.Where(x => x.SER_CODE == data.Type.Trim()).Select(y => y.NameAndCode).FirstOrDefault();
            if (data.Mode != "" && data.Mode != null)
                MODE.Text = data.Mode.Trim();
            string SelectedClientCode = ((Client)CustomerSelected.SelectedItem).CLCODE;
            SubClientListSource.Source = SubClientList.ContainsKey(SelectedClientCode) ? SubClientList[SelectedClientCode] : new List<string>();
            ConsigneeListSource.Source = ConsigneeList;
            ConsignerListSource.Source = ConsignerList;
            ConsigneeAddress.Text = data.ConsigneeAddress ?? "";
            ConsgineeName.Text = data.ConsigneeName ?? "";
            ConsignerAddress.Text = data.ConsignerAddress ?? "";
            ConsignerName.Text = data.ConsignerName ?? "";
            if (data.SubClient != "" && data.SubClient != null)
            {
                SubClientComboBox.Text = data.SubClient;
            }
            SlipCost.Text = data.Stock;
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
                if (data.Destination == null)
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
        private void BilledWeightTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.BilledAmount.Text == "" || this.BilledAmount.Text == null)
            {
                getrate();
            }
        }
        private void CustomerSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubClientList != null)
            {
                string SelectedClientCode = ((Client)CustomerSelected.SelectedItem).CLCODE;
                SubClientListSource.Source = SubClientList.ContainsKey(SelectedClientCode) ? SubClientList[SelectedClientCode] : null;
            }
        }
        private void MODE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }



        private void ConnsignmentNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            fillAllElements(ConnsignmentNumber.Text);
        }

        private void Button_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                SubmitSanitizingDetails_Click(null, null);
        }
    }
}