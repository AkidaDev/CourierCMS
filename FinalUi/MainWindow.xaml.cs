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
    /// 
    public partial class MainWindow : Window
    {
        #region initScripts
        void initDb()
        {
            string dataSource = "";
            System.Data.Common.DbConnectionStringBuilder stringBuilder = new System.Data.Common.DbConnectionStringBuilder();
            string insName = @"\" + dataSource;
            if (insName != "")
            {
                stringBuilder.Add("Data Source", Environment.MachineName + insName);
            }
            else
                stringBuilder.Add("Data Source", Environment.MachineName);
            string passWord = "";
            stringBuilder.Add("User ID", "sa");
            stringBuilder.Add("Password", passWord);
            Configs.Default.ConnString = stringBuilder.ConnectionString;
            Configs.Default.Save();
            BillingDataDataContext db = new BillingDataDataContext();
            if (db.DatabaseExists())
            {
                db.DeleteDatabase();
            }
            db.CreateDatabase();
            Employee emp = new Employee();
            emp.Name = "Dharmendra";
            Guid empId = Guid.NewGuid();
            emp.Id = empId;
            emp.UserName = "dharmendra";
            emp.Gender = 'M';
            emp.EMPCode = "DMD";
            emp.Password = "pass";
            db.Employees.InsertOnSubmit(emp);
            Role role = new Role();
            role.Name = "SuperUser";
            Guid roleId = new Guid();
            role.Id = roleId;
            db.Roles.InsertOnSubmit(role);
            User_Role user_role = new User_Role();
            user_role.Id = Guid.NewGuid();
            user_role.EmployeeId = empId;
            user_role.RoleId = roleId;
            db.User_Roles.InsertOnSubmit(user_role);
            for (int i = 0; i < 5; i++)
            {
                Client client = new Client();
                client.Name = "Client" + i.ToString();
                client.Address = "Address" + i.ToString();
                client.EmailAddress = "Email" + i.ToString();
                client.Code = "CLT" + i.ToString();
                client.PhoneNo = i;
                client.Id = Guid.NewGuid();
                db.Clients.InsertOnSubmit(client);
            }
            db.SubmitChanges();
            stringBuilder.Add("Initial Catalog", "BillingDatabase");
            Configs.Default.ConnString = stringBuilder.ConnectionString;
            Configs.Default.Save();


        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            initDb();
        }
        void RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else
                MessageBox.Show("Done");
        }

        #endregion

        DataGridHelper dataGridHelper;
        CollectionViewSource dataGridSource;
        Dictionary<Button, int> buttonList;
        public MainWindow()
        {
            #region setupCode
            BackgroundWorker worker2 = new BackgroundWorker();
            worker2.DoWork += worker_DoWork;
            worker2.RunWorkerCompleted += RunCompleted;
            //   worker2.RunWorkerAsync();
            #endregion
            #region WindowDimensionsCode
            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height;
            this.Left = 0;
            this.Top = 0;
            this.WindowState = WindowState.Normal;
            #endregion
            ResourceDictionary dict = this.Resources;
            InitializeComponent();
            CollectionViewSource clientCodeList = (CollectionViewSource)FindResource("ClientCodeList");
            clientCodeList.Source = (new BillingDataDataContext()).Clients.Select(c => c.Code);

            #region DataGrid Code Lines
            dataGridSource = (CollectionViewSource)FindResource("DataGridDataContext");
            dataGridHelper = new DataGridHelper(dataGridSource);
            buttonList = new Dictionary<Button, int>();
            DataGridPageNum.DataContext = dataGridHelper;
            DataGridNumOfRows.DataContext = dataGridHelper;
            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.DoWork += insertData_DoWork;
            #endregion

        }



        #region CommandFunctions
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
            if (dataGridSource != null)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                PowerEntry powerWin = new PowerEntry(dataGridHelper.currentConnNos, db.Clients.Select(c => c.Code).ToList());
                powerWin.Show();
            }
        }
        #endregion
        BackgroundWorker worker;
        #region DataGrid Methods 
        void loadData_Closed(object sender, EventArgs e)
        {
            LoadData dataWind = (LoadData)sender;
            //TODO: Get Name 
            string name = "";
            if (dataWind.dataLoaded)
            {
                if (dataWind.isNewSheet)
                {
                    worker.WorkerReportsProgress = true;
                    worker.ProgressChanged += worker_ProgressChanged;
                    int key = dataGridHelper.addNewSheet(dataWind.data, name);
                    worker.RunWorkerAsync(dataWind.data);

                    dataGridHelper.getFirstPage();
                    Button button = new Button();
                    button.Content = "Sheet " + key.ToString();
                    button.Style = (Style)Application.Current.FindResource("ButtonStyle");
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
            MessageTextBox.Text = "in progress... " + e.ProgressPercentage.ToString();
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

        #region filter functions
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterSelectWindow window = new FilterSelectWindow(dataGridHelper.currentConnNos);
            window.Closed += window_Closed;
            window.Show();
        }

        void window_Closed(object sender, EventArgs e)
        {
            FilterSelectWindow filterWindow = (FilterSelectWindow)sender;
            foreach(var filter in filterWindow.filters)
            {
                dataGridHelper.currentDataSheet.addFilter(filter);
            }
        }
        #endregion

        private void SanitizingButton_Click(object sender, RoutedEventArgs e)
        {
            SanitizingWindow window = new SanitizingWindow(dataGridHelper.getCurrentDataStack);
            window.Show();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Started");
            BillingDataDataContext db = new BillingDataDataContext();
            IEnumerable<RuntimeData> newData = dataGridHelper.getCurrentDataStack.Where(x => x.TransactionId == null);
            List<RuntimeData> oldData = dataGridHelper.getCurrentDataStack.Where(x => x.TransactionId != null).ToList();
          
            foreach (var data in newData)
            {
               
                var transactionData = UtilityClass.convertRuntimeObjToTransObj(data);
                if (data.CustCode != null)
                {
                    ClientTransaction clientData = new ClientTransaction();
                    clientData.TransactionID = transactionData.ID;
                    clientData.ClientID = db.Clients.Where(x => x.Code == data.CustCode).Single().Id;
                    clientData.ID = Guid.NewGuid();
                    db.ClientTransactions.InsertOnSubmit(clientData);
                }
                db.Transactions.InsertOnSubmit(transactionData);

            }
            foreach (var data in oldData)
            {
                var transactionData = db.Transactions.Single(x => x.ID == data.TransactionId);
                transactionData.AmountCharged = data.FrAmount;
                transactionData.AmountPayed = data.Amount;
                transactionData.ConnsignmentNo = data.ConsignmentNo;
                transactionData.Destination = data.Destination;
                transactionData.DestinationPin = data.DestinationPin;
                transactionData.Weight = (decimal)(data.Weight);
                transactionData.BookingDate = data.BookingDate;
                transactionData.LastModified = System.DateTime.Today;
                if (data.FrWeight != null)
                    transactionData.WeightByFranchize = (decimal)data.FrWeight;
                if (data.CustCode != null)
                {
                    ClientTransaction cdata = db.ClientTransactions.Single(x => x.TransactionID == transactionData.ID);
                    cdata.ClientID = db.Clients.Single(x => x.Code == data.CustCode).Id;
                }
            }
            db.SubmitChanges();
            MessageBox.Show("Done");
        
        }


    }

}
