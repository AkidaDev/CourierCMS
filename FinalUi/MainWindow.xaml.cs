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
        public void initDb()
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
                client.CLCODE = "CLT" + i.ToString();
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
        #region Global Objects
        DataGridHelper dataGridHelper;
        CollectionViewSource dataGridSource;
        Dictionary<Button, int> buttonList;
        Button activeButton;
        BackgroundWorker LoadWorker;
        BackgroundWorker SaveWorker;
        #endregion
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
            clientCodeList.Source = (new BillingDataDataContext()).Clients.Select(c => c.CLCODE);

            #region DataGrid Code Lines
            dataGridSource = (CollectionViewSource)FindResource("DataGridDataContext");
            dataGridHelper = new DataGridHelper(dataGridSource);
            buttonList = new Dictionary<Button, int>();
            DataGridPageNum.DataContext = dataGridHelper;
            DataGridNumOfRows.DataContext = dataGridHelper;
            #endregion

            #region Command Bindings
            CommandBinding SanitizingCommandBinding = new CommandBinding(SanitizingCommand, ExecuteSanitizingCommand, CanExecuteIsDataGridNotNull);
            this.CommandBindings.Add(SanitizingCommandBinding);
            SanitizingButton.Command = SanitizingCommand;

            CommandBinding PowerEntryCommandBinding = new CommandBinding(PowerEntryCommand, PowerEntryCommandExecuted, CanExecuteIsDataGridNotNull);
            this.CommandBindings.Add(PowerEntryCommandBinding);
            PowerEntryButton.Command = PowerEntryCommand;

            CommandBinding SaveCommandBinding = new CommandBinding(ApplicationCommands.Save, ExecuteSaveCommand, CanExecuteSaveCommand);
            this.CommandBindings.Add(SaveCommandBinding);
            SaveButton.Command = ApplicationCommands.Save;
            #endregion


            #region loading initial pages
            BillingDataDataContext db = new BillingDataDataContext();
            List<int> sheets = db.RuntimeMetas.Where(y => y.UserName == SecurityModule.currentUserName).Select(x => x.SheetNo).Distinct().ToList();
            foreach (int sheet in sheets)
            {
                List<RuntimeData> runtimeData = db.RuntimeMetas.Where(x => x.SheetNo == sheet && x.UserName == SecurityModule.currentUserName).Select(y => y.RuntimeData).ToList();
                dataGridHelper.addNewSheet(runtimeData, "sheet " + sheet.ToString());
                addingNewPage(sheet);
            }
            #endregion

            SaveWorker = new BackgroundWorker();
            SaveWorker.DoWork += SaveWorker_DoWork;
            SaveWorker.ProgressChanged += SaveWorker_ProgressChanged;
            SaveWorker.RunWorkerCompleted += SaveWorker_RunWorkerCompleted;
            LoadWorker = new BackgroundWorker();
            LoadWorker.DoWork += LoadWorker_DoWork;
            LoadWorker.ProgressChanged += LoadWorker_ProgressChanged;
            LoadWorker.RunWorkerCompleted += LoadWorker_RunWorkerCompleted;
        }
        #region backGround Worker Functions

        void LoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBlock.Text = MessageBlock.Text + "\n" + e.Result;
        }

        void LoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void LoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DBHelper help = new DBHelper();
            string response;
            try
            {
                help.insertRuntimeData((List<RuntimeData>)e.Argument, dataGridHelper.currentSheetNumber);
                response = "Data Loading Successful";
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            e.Result = response;
        }

        #region Save Worker
        void SaveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBlock.Text = MessageBlock.Text + "\n " + e.Result;
        }

        void SaveWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }


        void SaveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string response;
            try
            {
                response = UtilityClass.saveRuntimeAsTransaction(dataGridHelper.getCurrentDataStack);
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            e.Result = response;
        }

        #endregion
        #endregion
        #region CustomCommands
        private void CanExecuteIsDataGridNotNull(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataGridHelper.getCurrentDataStack != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
        #region SanitizingCommand
        public RoutedCommand SanitizingCommand = new RoutedCommand();

        private void ExecuteSanitizingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SanitizingWindow window = new SanitizingWindow(dataGridHelper.getCurrentDataStack);
            window.Show();
        }
        #endregion
        #region PowerEntryCommand
        RoutedCommand PowerEntryCommand = new RoutedCommand();
        private void PowerEntryCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            BillingDataDataContext db = new BillingDataDataContext();
            PowerEntry powerWin = new PowerEntry(dataGridHelper.getCurrentDataStack, db.Clients.Select(c => c.CLCODE.ToString()).ToList());
            powerWin.Show();

        }
        #endregion
        #region SaveCommand
        private void CanExecuteSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Debug.WriteLine("Here In");
            if (dataGridHelper != null)
            {
                if (dataGridHelper.getCurrentDataStack == null || SaveWorker.IsBusy == true)
                {
                    e.CanExecute = false;
                }
                else
                    e.CanExecute = true;
            }
            else
                e.CanExecute = false;
            Debug.WriteLine("Here Out");
        }
        private void ExecuteSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBlock.Text = MessageBlock.Text + "\n Saving operation started...";
            SaveWorker.RunWorkerAsync();
        }
        #endregion
        #region LoadCommand
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Debug.WriteLine("Here in open..");
            if (LoadWorker != null)
            {
                if (LoadWorker.IsBusy == true)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            else
                e.CanExecute = false;
            Debug.WriteLine("Here in open out");
        }
        #endregion
        #endregion

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



        private void CloseCurrentClick_Click(object sender, RoutedEventArgs e)
        {

            if (buttonList.Count > 0)
            {
                DBHelper help = new DBHelper();
                help.deleteRuntimeData(dataGridHelper.currentSheetNumber);
                dataGridHelper.removeSheet(dataGridHelper.currentSheetNumber);
                DataGridSheetPanel.Children.Remove(activeButton);
                buttonList.Remove(activeButton);
                activeButton = buttonList.Single(x => x.Value == buttonList.Values.Min()).Key;

            }
        }
        #endregion
        #region DataGrid Methods
        void addingNewPage(int key)
        {
            dataGridHelper.getFirstPage();
            Button button = new Button();
            button.Style = (Style)FindResource("all");
            button.Background = Brushes.Transparent;
            StackPanel panel = new StackPanel();
            Path path = new Path();
            path.Data = Geometry.Parse(@"F1M3.905,27.953C3.905,27.953 2,2.147 16.096,2.074 30.193,2 55.109,2.074 55.109,2.074 55.109,2.074 61.586,2.221 77.054,
			27.953 77.054,27.953 3.905,27.953 3.905,27.953z");
            path.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A000000"));
            path.Height = 29;
            path.Width = 79;
            path.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5B5B5B"));
            panel.Children.Add(path);
            TextBlock text = new TextBlock();
            text.Text = "Sheet " + key.ToString();
            text.Margin = new Thickness(0, -21, 12, 0);
            text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFffffff"));
            text.FontSize = 12;
            text.Background = Brushes.Transparent;
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panel.Children.Add(text);
            button.Content = panel;
            button.Click += SheetSelectButton_Click;
            DataGridSheetPanel.Children.Add(button);
            buttonList.Add(button, key);
            activeButton = button;

        }
        void loadData_Closed(object sender, EventArgs e)
        {
            LoadData dataWind = (LoadData)sender;
            //TODO: Get Name 
            string name = "";
            if (dataWind.dataLoaded)
            {
                if (dataWind.isNewSheet)
                {
                    int key = dataGridHelper.addNewSheet(dataWind.data, name);
                    addingNewPage(key);
                }
                else
                {
                    dataGridHelper.addDataToCurrentSheet(dataWind.data);
                    dataGridHelper.refreshCurrentPage();
                }
                LoadWorker.RunWorkerAsync(dataWind.data);


            }
        }

        private void SheetSelectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            dataGridHelper.setActiveSheet(buttonList[button]);
            dataGridHelper.refreshCurrentPage();
            activeButton = button;
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
            foreach (var filter in filterWindow.filters)
            {
                dataGridHelper.currentDataSheet.addFilter(filter);
            }
        }
        #endregion

        





    }

}
