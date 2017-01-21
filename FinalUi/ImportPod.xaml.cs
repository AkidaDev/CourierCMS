using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ImportPod.xaml
    /// </summary>
    public partial class ImportPod : Window
    {

        BackgroundWorker bg;
        List<RuntimeData> notFound;
        public ImportPod()
        {
            InitializeComponent();
            notFound = new List<RuntimeData>();
            bg = new BackgroundWorker();
            bg.DoWork += bg_DoWork;
            bg.WorkerReportsProgress = true;
            bg.ProgressChanged += bg_ProgressChanged;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (bg.IsBusy == true)
            {
                MessageBox.Show("File loading already in progress..", "Information");
                return;
            }
            OpenFileDialog fd = new OpenFileDialog();
            fd.DefaultExt = ".txt";
            fd.Filter = "(.txt)|*.txt";
            Nullable<bool> result = fd.ShowDialog();
            if (result == true)
            {
                FileNameTextBox.Text = fd.FileName;
            }
        }
        private void StartLoadingButton_Click(object sender, RoutedEventArgs e)
        {
            if (bg.IsBusy == true)
            {
                MessageBox.Show("File loading already in progress..", "Information");
                return;
            }
            Analyzeprogress.Visibility = System.Windows.Visibility.Visible;
            if (File.Exists(FileNameTextBox.Text))
            {
                string fileName = FileNameTextBox.Text;
                bg.RunWorkerAsync(fileName);
            }
            else
            {
                MessageBox.Show("Error: File not found", "Error");
            }
        }
        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error occured during loading file: " + e.Error.Message, "Error");
                return;
            }
            else
            {
                if ((string)e.Result != "")
                {
                    MessageBox.Show("Following records cannot be processed:\n" + e.Result, "Error");
                    return;
                }
                else
                {
                    if(notFound.Count > 0)
                    {
                        foreach(var rdata in notFound)
                        {
                            Log.Text += '\n' + rdata.ConsignmentNo;
                        }
                    }else
                    {
                        MessageBox.Show("File loaded successfully");
                        this.Close();
                    }
                }
            }
            Analyzeprogress.Visibility = Visibility.Hidden;
        }
        void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Analyzeprogress.Value = e.ProgressPercentage;
            if (Analyzeprogress.Value == 100 || Analyzeprogress.Value == 0)
            {
                Analyzeprogress.Visibility = Visibility.Hidden;
            }
            else
            {
                Analyzeprogress.Visibility = Visibility.Visible;
            }
        }
        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;
            CSVDataLoader csvDL = new CSVDataLoader();
            List<RuntimeData> data = csvDL.getRuntimeDataFromPodCSV(fileName, '"', '\'');
            double progress = 5;
            bg.ReportProgress((int)progress);
            double i = 0;
            double count = data.Count;
            BillingDataDataContext db = new BillingDataDataContext();
            string errorMessage = "";
           
            foreach (RuntimeData rData in data)
            {
                try
                {
                    Transaction trans = db.Transactions.SingleOrDefault(x => x.ConnsignmentNo == rData.ConsignmentNo);
                    if( trans != null)
                    {
                        trans.DeliveryDate = rData.DeliveryDate;
                        if(rData.DeliveryDate != null)
                        {
                            trans.DeliveryStatus = "Delivered";
                        }
                        trans.DeliveryTime = rData.DeliveryTime;
                        trans.ReceivedBy = rData.ReceivedBy;
                        trans.Remarks = trans.Remarks;
                        db.Transactions.InsertOnSubmit(trans);
                        db.SubmitChanges();
                    }
                    else
                    {
                        notFound.Add(rData);
                    }
                }
                catch
                {
                    errorMessage += rData.ConsignmentNo + " ";
                }
                i++;
                int prog = (int)(progress + (i / count * 85));
                bg.ReportProgress(prog);
            }

            e.Result = errorMessage;
            bg.ReportProgress(100);
        }
    }
}
