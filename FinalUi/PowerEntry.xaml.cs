using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public PowerEntry(List<RuntimeData> DataStack, List<String> ClientCodes, BillingDataDataContext db)
            : this()
        {
            DataStack = DataStack.OrderBy(x => x.BookingDate).ThenBy(y => y.ConsignmentNo).ToList();
            List<string> connList = DataStack.Select(c => c.ConsignmentNo).ToList();
            startConnNo.DataContext = connList;
            endConnNo.DataContext = connList;
            clientCode.DataContext = ClientCodes;
            this.DataStack = DataStack;
            this.db = db;
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Debug.WriteLine("Progress is " + e.ProgressPercentage);
            progressbar.Value = ((double)e.ProgressPercentage); 
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          //  MessageBox.Show(e.Error.Message);
            SubmitRecords.IsEnabled = true;
            if(errorNos != "")
            MessageBox.Show("Error calculating records: " + errorNos);
            Debug.WriteLine(e.Error.Message);
            startCOnnNoIndex = -1;
            endConnNoIndex = -1;
            progressbar.Value = 0;
        }
        string errorNos;
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            errorNos = "";
            Debug.WriteLine("inside do work");
            if (startCOnnNoIndex <= endConnNoIndex && startCOnnNoIndex != -1 && endConnNoIndex != -1)
            {
                int total = endConnNoIndex - startCOnnNoIndex;
                var cs = (from m in db.Cities select m).ToList();
                for (int i = startCOnnNoIndex; i <= endConnNoIndex; i++)
                {
                    RuntimeData data = DataStack.ElementAt(i);
                    data.CustCode = clientCodeSelectedValue;
                   
                    data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                    var c = cs.Where(x => x.CITY_CODE == data.Destination && x.CITY_STATUS == "A").FirstOrDefault();
                    if (c == null)
                        c = db.Cities.SingleOrDefault(x => x.CITY_CODE == "DEL");
                    data.CustCode = clientCodeSelectedValue;
                    data.BilledWeight = data.Weight;
                    data.FrAmount = (decimal)UtilityClass.getCost(data.CustCode, (double)data.BilledWeight, data.Destination, data.Type,(char) data.DOX);
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch(Exception)
                    {
                        errorNos = errorNos + "\n " + data.ConsignmentNo;
                    }
                    worker.ReportProgress((((i-startCOnnNoIndex+1)*100)/total));
                }
            }
           
            SubmitRecords.IsEnabled = true;
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
                if (clientCode.Text == "")
                    return;
                clientCodeSelectedValue = clientCode.SelectedValue.ToString(); 
                worker.RunWorkerAsync();
            }
            else
                MessageBox.Show("Process already running.");
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}