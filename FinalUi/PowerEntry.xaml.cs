using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
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
    /// Interaction logic for PowerEntry.xaml
    /// </summary>
    public partial class PowerEntry : Window
    {
        BillingDataDataContext db;
        List<RuntimeData> DataStack;
        BackgroundWorker worker;
        int startCOnnNoIndex;
        int endConnNoIndex;
        string clientCodeSelectedValue;
        string consigner;
        string consignee;
        double weight;
        string subClient;
        bool? setWeightCheck;
        bool? consignerCheck;
        bool? consigneeCheck;
        bool? subClientCheck;
        bool? calcRateCheck;
        DataGrid datagrid;
        public PowerEntry(List<RuntimeData> DataStack, DataGrid datagrid)
            : this()
        {
            this.datagrid = datagrid;
            CollectionViewSource source = (CollectionViewSource)FindResource("ClientList");
            source.Source = DataSources.ClientCopy;
            DataStack = DataStack.OrderBy(y => y.ConsignmentNo).ToList();
            List<string> connList = DataStack.Select(c => c.ConsignmentNo).ToList();
            startConnNo.DataContext = connList;
            endConnNo.DataContext = connList;
            this.DataStack = DataStack;
            this.db = new BillingDataDataContext();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
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
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Debug.WriteLine("Progress is " + e.ProgressPercentage);
            progressbar.Value = ((double)e.ProgressPercentage);
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                Debug.WriteLine(e.Error.Message);
            }
            SubmitRecords.IsEnabled = true;
            if (errorNos != "")
                MessageBox.Show("Error calculating records: " + errorNos);

            startCOnnNoIndex = -1;
            endConnNoIndex = -1;
            progressbar.Value = 0;
            if (datagrid != null)
                datagrid.Items.Refresh();
        }
        string errorNos;
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Client client = DataSources.ClientCopy.FirstOrDefault(x => x.CLCODE == clientCodeSelectedValue);
            errorNos = "";
            Debug.WriteLine("inside do work");
            if (startCOnnNoIndex <= endConnNoIndex && startCOnnNoIndex != -1 && endConnNoIndex != -1)
            {
                int total = endConnNoIndex - startCOnnNoIndex;
                var cs = (from m in db.Cities select m).ToList();
                for (int i = startCOnnNoIndex; i <= endConnNoIndex; i++)
                {
                    RuntimeData data = DataStack.ElementAt(i);

                    data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                    var c = cs.Where(x => x.CITY_CODE == data.Destination).FirstOrDefault();
                    if (c == null)
                        c = db.Cities.SingleOrDefault(x => x.CITY_CODE == "DEL");
                    data.CustCode = clientCodeSelectedValue;
                    if (client != null)
                        data.Client_Desc = client.CLNAME;
                    if (data.FrWeight == null)
                        data.FrWeight = data.Weight;
                    if (data.BilledWeight == null)
                        data.BilledWeight = data.Weight;
                    if(setWeightCheck == true)
                    {
                        data.BilledWeight = weight;
                    }
                    if (subClientCheck== true)
                    {
                        data.SubClient = subClient;
                    }
                    if (consignerCheck== true)
                        data.ConsignerName = consigner;
                    if (consigneeCheck== true)
                        data.ConsigneeName = consignee;
                    if (calcRateCheck == true)
                    {
                        try
                        {
                            data.FrAmount = (decimal)UtilityClass.getCost(data.CustCode, (double)data.BilledWeight, data.Destination, data.Type, (char)data.DOX);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message + ": Occured in " + data.ConsignmentNo);
                            data.FrAmount = -1;
                        }
                    }
                    try
                    {
                        RuntimeData ndata = DataStack.ElementAt(i);
                        dupliData(data, ndata);
                        db.SubmitChanges();

                    }
                    catch (Exception)
                    {
                        errorNos = errorNos + "\n " + data.ConsignmentNo;
                    }
                    worker.ReportProgress((((i - startCOnnNoIndex + 1) * 100) / total));
                }
            }
        }
        PowerEntry()
        {
            InitializeComponent();
        }
        private void SubmitRecords_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                SubmitRecords.IsEnabled = false;
                startCOnnNoIndex = startConnNo.SelectedIndex;
                endConnNoIndex = endConnNo.SelectedIndex;
                Client client = DataSources.ClientCopy.SingleOrDefault(x => x.NameAndCode == ((Client)clientCode.SelectedItem).NameAndCode);
                consigneeCheck = Consignee.Checked;
                consignerCheck = ConsignerCheck.Checked;
                calcRateCheck = CalcRateCheck.Checked;
                setWeightCheck = SetWeightCheck.Checked;
                subClientCheck = SubClientCheck.Checked;
                consignee = ConsigneeTextBox.Text;
                consigner = ConsignerTextBox.Text;
                subClient = SubClientTextBox.Text;
                if(!double.TryParse(WeightTextBox.Text,out weight))
                {
                    if (setWeightCheck == true)
                    {
                        MessageBox.Show("Invalid weight", "Error!");
                        SubmitRecords.IsEnabled = true;
                        return;
                    }
                    else
                        weight = 0;
                }
                if (client == null)
                {
                    MessageBox.Show("No such client exists..!!","Error");
                    SubmitRecords.IsEnabled = true;
                    return;
                }
                clientCodeSelectedValue = client.CLCODE;
                worker.RunWorkerAsync();
                //worker_DoWork(null, null);
            }
            else
                MessageBox.Show("Process already running.");
        }
        private void SubClientCheck_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(SubClientCheck.Checked.ToString());
        }
    }
}