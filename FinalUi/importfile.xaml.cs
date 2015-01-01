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
    /// Interaction logic for importfile.xaml
    /// </summary>
    public partial class importfile : Window
    {

        BackgroundWorker bg;
        public importfile()
        {
            InitializeComponent();
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
            if(bg.IsBusy == true)
            {
                MessageBox.Show("File loading already in progress..","Information");
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
            if(e.Error != null)
            {
                MessageBox.Show("Error occured during loading file: " + e.Error.Message, "Error");
                return;
            }
            else
            {
                if((string)e.Result != "")
                {
                    MessageBox.Show("Following records cannot be processed:\n" + e.Result, "Error");
                    return;
                }
                else
                {
                    MessageBox.Show("File loaded successfully");
                    this.Close();
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
            List<RuntimeData> data = csvDL.getRuntimeDataFromCSV(fileName, '"', '\'');
            double progress = 5;
            bg.ReportProgress((int)progress);
            double i = 0;
            double count = data.Count;
            BillingDataDataContext db = new BillingDataDataContext();
            int sheetNo;
            string errorMessage = "";
            try
            {
                sheetNo = db.RuntimeDatas.Max(x => x.SheetNo) + 1;
            }
            catch (Exception)
            {
                sheetNo = 0;
            }
            foreach (RuntimeData rData in data)
            {
                try
                {
                    rData.SheetNo = sheetNo;
                    rData.UserId = "System";
                    db.RuntimeDatas.InsertOnSubmit(rData);
                    db.SubmitChanges();
                }
                catch
                {
                    errorMessage += rData.ConsignmentNo + " ";
                }
                i++;
                int prog = (int)(progress + (i / count * 85));
                bg.ReportProgress(prog);
            }
            db.ImportFileData(sheetNo, "System", SecurityModule.currentUserName);
            e.Result = errorMessage;
            bg.ReportProgress(100);
        }
    }
}
