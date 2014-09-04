using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataGridHelper dataGridHelper;
        CollectionViewSource dataGridSource;
        Dictionary<Button, int> buttonList;
        public MainWindow()
        {
            
            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height;
            this.Left = 0;
            this.Top = 0;
            this.WindowState = WindowState.Normal;
            InitializeComponent();
            dataGridSource = (CollectionViewSource)FindResource("DataGridDataContext");
            dataGridHelper = new DataGridHelper(dataGridSource);
            buttonList = new Dictionary<Button, int>();
            DataGridPageNum.DataContext = dataGridHelper;
            DataGridNumOfRows.DataContext = dataGridHelper;
            worker = new BackgroundWorker();
        }

        
        public MainWindow(string userName)
        {

        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadData loadData = new LoadData();
            loadData.Closed += loadData_Closed;
            loadData.Show();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void PowerEntry_Click(object sender, RoutedEventArgs e)
        {
            if(dataGridSource != null)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                PowerEntry powerWin = new PowerEntry(dataGridHelper.currentConnNos,db.Clients.Select(c=>c.Code).ToList());
                powerWin.Show();
            }
        }
        BackgroundWorker worker;
        #region DataGrid Methods Starts:
        void loadData_Closed(object sender, EventArgs e)
        {
            LoadData dataWind = (LoadData)sender;
            //TODO: Get Name 
            string name = "";
            if (dataWind.dataLoaded)
            {
                if (dataWind.isNewSheet)
                {
                    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                    worker.DoWork += insertData_DoWork;
                    worker.WorkerReportsProgress = true;
                    worker.ProgressChanged += worker_ProgressChanged;
                    int key = dataGridHelper.addNewSheet(dataWind.data, name);
                    worker.RunWorkerAsync(dataWind.data);
                    
                    dataGridHelper.getFirstPage();
                    Button button = new Button();
                    button.Content = "Sheet " + key.ToString();
                    button.Click += SheetSelectButton_Click;
                    DataGridSheetPanel.Children.Add(button);
                    buttonList.Add(button, key);
                }
                else
                {
                    dataGridHelper.addDataToCurrentSheet(dataWind.data);
                }
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show("Done... " + e.Error.Message);
            else
                MessageBox.Show("Done");
            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MessageTextBox.Text = "in progress... " +e.ProgressPercentage.ToString();
        }

        void insertData_DoWork(object sender, DoWorkEventArgs e)
        {
            
            ((BackgroundWorker)sender).ReportProgress(10);
            List<RuntimeData> data = (List<RuntimeData>)e.Argument;
            
            DBHelper help = new DBHelper();
            help.insertRuntimeData(data);
            ((BackgroundWorker)sender).ReportProgress(100);
        }

        private void SheetSelectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            dataGridHelper.setActiveSheet(buttonList[button]);
            dataGridHelper.refreshCurrentPage();
        }
        #region DataGrid Navigation Method
        private void DataGridPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            dataGridHelper.getPrevPage();
        }

        private void DataGridCurrentPage_Click(object sender, RoutedEventArgs e)
        {
            dataGridHelper.refreshCurrentPage();
        }

        private void DataGridNextPage_Click(object sender, RoutedEventArgs e)
        {
            dataGridHelper.getNextPage();
        }

        private void DataGridLastPage_Click(object sender, RoutedEventArgs e)
        {
            dataGridHelper.getLastPage();
        }

        private void DataGridFirstPage_Click(object sender, RoutedEventArgs e)
        {
            dataGridHelper.getFirstPage();
        }
        #endregion DataGrid Navigation ends

        


        #region Datagrid Sheet Methods Start

        #endregion DataGrid Sheet Methods Ends
        #endregion DataGrid Methods Ends


    }

}
