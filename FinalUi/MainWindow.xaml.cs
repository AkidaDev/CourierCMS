﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
        // Zone listing data import procedure
        CollectionViewSource ZoneTableSource;
        // Zone listing data import procedure
        // Employee listing data import procedure
        public List<Employee> employeeToEdit;
        public List<Employee> employees;
        private CollectionViewSource employeeview;
        private List<CostingRule> costingRules;
        private CollectionViewSource serviceRulesView;
        private List<ServiceRule> serviceRules;
        private CollectionViewSource ServiceTable;
        private Quotation qutObj;
        // Employee listing data import procedure
        // Client listing data import procedure
        CollectionViewSource viewsource;
        List<Client> clientToEdit;
        Client client;
        CollectionViewSource clientViewSource;
        CollectionViewSource cityViewSource;
        Button currentaddrulebutton;
        // Client listing data import procedure
        #region initScripts

        CollectionViewSource dueDataGridSource;
        CollectionViewSource profitDataGridSource;
        #endregion
        #region Global Objects
        DataGridHelper dataGridHelper;
        CollectionViewSource dataGridSource;
        Dictionary<Button, int> buttonList;
        Button activeButton;
        BackgroundWorker LoadWorker;
        BackgroundWorker SaveWorker;
        BackgroundWorker DeleteSheetWorker;
        BillingDataDataContext db;
        CollectionViewSource CostingRulesSource;

        #endregion
        bool isInitialized = false;
        public MainWindow()
        {
            InitializeComponent();
            isInitialized = true;
            // Zone listing data import procedure
            ZoneTableSource = (CollectionViewSource)FindResource("zoneTable");
            ZoneTableSource.Source = DataSources.ZoneCopy;
            // Zone listing data import procedure
            // Client listing data import procedure
            clientToEdit = new List<Client>();
            clientViewSource = (CollectionViewSource)FindResource("ClienTable");
            clientViewSource.Source = DataSources.ClientCopy;
            cityViewSource = (CollectionViewSource)FindResource("CityTable");
            cityViewSource.Source = DataSources.CityCopy;
            // Employee listing data import procedure
            employeeToEdit = new List<Employee>();
            this.employees = DataSources.EmployeeCopy;
            employeeview = (CollectionViewSource)FindResource("EmployeeTable");
            employeeview.Source = this.employees;
            // Employee listing data import procedure
            dueDataGridSource = (CollectionViewSource)FindResource("DueGridDataSource");
            profitDataGridSource = (CollectionViewSource)FindResource("UnpaidInvoiceGridSource");
            BillingDataDataContext db = new BillingDataDataContext();
            dueDataGridSource.Source = db.BalanceViews;
            profitDataGridSource.Source = (from invoice in db.Invoices
                                           where !(from payment in db.PaymentEntries
                                                   select payment.InvoiceNumber)
                                                   .Contains(invoice.BillId)
                                           select invoice).ToList();
            ServiceTable = (CollectionViewSource)FindResource("ServiceTable");
            ServiceTable.Source = DataSources.ServicesCopy;
            #region setupCode
            PreviewMouseMove += OnPreviewMouseMove;
            #endregion
            this.MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Configs.Default.Background));
            db = new BillingDataDataContext();
            ResourceDictionary dict = this.Resources;
            InitializeComponent();
            CollectionViewSource clientCodeList = (CollectionViewSource)FindResource("ClientCodeList");
            clientCodeList.Source = DataSources.ClientCopy;
            #region DataGrid Code Lines
            dataGridSource = (CollectionViewSource)FindResource("DataGridDataContext");
            dataGridHelper = new DataGridHelper(dataGridSource);
            buttonList = new Dictionary<Button, int>();
            DataGridPageNum.DataContext = dataGridHelper;
            DataGridNumOfRows.DataContext = dataGridHelper;
            #endregion
            #region Command Bindings
            CommandBinding SanitizingCommandBinding = new CommandBinding(SanitizingCommand, ExecuteSanitizingCommand, CanExecuteSanitizingCommand);
            this.CommandBindings.Add(SanitizingCommandBinding);
            SanitizingButton.Command = SanitizingCommand;

            CommandBinding PowerEntryCommandBinding = new CommandBinding(PowerEntryCommand, PowerEntryCommandExecuted, CanExecuteIsDataGridNotNull);
            this.CommandBindings.Add(PowerEntryCommandBinding);
            PowerEntryButton.Command = PowerEntryCommand;

            CommandBinding SaveCommandBinding = new CommandBinding(ApplicationCommands.Save, ExecuteSaveCommand, CanExecuteSaveCommand);
            this.CommandBindings.Add(SaveCommandBinding);
            SaveButton.Command = ApplicationCommands.Save;

            CommandBinding DeleteCommandBinding = new CommandBinding(DeleteCommand, DeleteCommandExecuted, DeleteCommand_CanExecute);
            this.CommandBindings.Add(DeleteCommandBinding);
            CommandBinding NewSheetCommandBinding = new CommandBinding(NewSheetCommand, NewSheetCommandExecuted, NewSheetCommand_CanExecute);
            NewSheetCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            NewSheetButton.Command = NewSheetMenuItem.Command = NewSheetCommand;
            this.CommandBindings.Add(NewSheetCommandBinding);
            #endregion
            #region loading initial pages
            List<int> sheets = db.RuntimeDatas.Where(y => y.UserId == SecurityModule.currentUserName).Select(x => x.SheetNo).Distinct().ToList();
            foreach (int sheet in sheets)
            {
                List<RuntimeData> runtimeData = db.RuntimeDatas.Where(x => x.UserId == SecurityModule.currentUserName && x.SheetNo == sheet).OrderBy(z => z.ConsignmentNo).ToList(); ;
                dataGridHelper.addNewSheet(runtimeData, "sheet " + sheet.ToString());
                addingNewPage(sheet);
            }
            #endregion
            #region LoadConfigs
            Configs.Default.PropertyChanged += Default_PropertyChanged;
            #endregion
            SaveWorker = new BackgroundWorker();
            SaveWorker.DoWork += SaveWorker_DoWork;
            SaveWorker.ProgressChanged += SaveWorker_ProgressChanged;
            SaveWorker.RunWorkerCompleted += SaveWorker_RunWorkerCompleted;
            LoadWorker = new BackgroundWorker();
            LoadWorker.DoWork += LoadWorker_DoWork;
            LoadWorker.ProgressChanged += LoadWorker_ProgressChanged;
            LoadWorker.RunWorkerCompleted += LoadWorker_RunWorkerCompleted;
            DeleteSheetWorker = new BackgroundWorker();
            DeleteSheetWorker.DoWork += DeleteWorker_DoWork;
            DeleteSheetWorker.RunWorkerCompleted += DeleteWorker_RunWorkerCompleted;
            costingRules = new List<CostingRule>();
            setUiFromPermissions();
        }

        private void setUiFromPermissions()
        {
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "ManageEmployee"))
            {
                this.ManageEmployeeMenuItem.Visibility = Visibility.Collapsed;
                //TreeViewEmployee.Visibility = Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "ManageClient"))
            {
              //  this.ManageClient.Visibility = Visibility.Collapsed;
               // TreeViewClient.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "CreateInvoice"))
            {
                this.PrintButton.Visibility = this.PrintMenuItem.Visibility = this.AfterPrint.Visibility = Visibility.Collapsed;
            }
        }
        #region DataEntrySection
        #region backGround Worker Functions
        #region LoadWorker
        void LoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBlock.Text = DateTime.Now.ToShortTimeString() + ": " + e.Result + "\n" + MessageBlock.Text;
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
                help.insertRuntimeData((List<RuntimeData>)e.Argument, dataGridHelper.currentSheetNumber, isLoadedFromBook, toDate_loadDataWin, fromDate_loadDataWin);
                response = "Data Loading Successful";
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            e.Result = response;
        }
        #endregion
        #region Save Worker
        void SaveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveDialogue.Visibility = System.Windows.Visibility.Collapsed;
            if (e.Error != null)
                MessageBlock.Text = DateTime.Now.ToShortTimeString() + ": " + "Error in saving: " + e.Error.Message + "\n" + MessageBlock.Text;
            else
                MessageBlock.Text = DateTime.Now.ToShortTimeString() + ": " + "Save Completed" + e.Result + "\n" + MessageBlock.Text;
        }

        void SaveWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }


        void SaveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string message = UtilityClass.saveRuntimeAsTransaction(dataGridHelper.currentSheetNumber, SecurityModule.currentUserName);
            if (message != "")
            {
                throw new Exception(message);
            }
            try
            {
                dataGridHelper.currentDataSheet.dataStack = db.RuntimeDatas.Where(x => x.SheetNo == dataGridHelper.currentSheetNumber && x.UserId == SecurityModule.currentUserName).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion
        #region Delete Sheet Worker
        void DeleteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBlock.Text = DateTime.Now.ToShortTimeString() + ": " + "Delete Operation Completed. " + e.Error + "\n" + MessageBlock.Text;
        }
        void DeleteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DBHelper help = new DBHelper();
            help.deleteRuntimeData((int)e.Argument);

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
        #region NewSheetCommands
        RoutedCommand NewSheetCommand = new RoutedCommand();
        private void NewSheetCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            int key = this.dataGridHelper.addNewSheet(new List<RuntimeData>(), "");
            addingNewPage(key);
            cloakAll();
            DataDockPanel.Visibility = Visibility.Visible;
            buttontabcanvaswrap.Visibility = Visibility.Visible;

        }
        private void NewSheetCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.buttonList.Count >= 6)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

        }
        #endregion
        #region SanitizingCommand
        public RoutedCommand SanitizingCommand = new RoutedCommand();
        private void CanExecuteSanitizingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ExecuteSanitizingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (dataGridHelper.currentDataSheet == null)
            {
                int key = dataGridHelper.addNewSheet(new List<RuntimeData>(), "");
                addingNewPage(key);
            }
            SanitizingWindow window;
            RuntimeData dataToSend = null;
            if (dataGrid.SelectedItem != null)
                dataToSend = (RuntimeData)dataGrid.SelectedItem;
           
            concatinateAllRecordsInOnePage();
            Dictionary<string, List<string>> SubClientList = new Dictionary<string, List<string>>();
            List<RuntimeData> data = dataGridHelper.getCurrentDataStack;
            List<string> clients = data.Select(x => x.CustCode).Distinct().ToList();
            clients.ForEach(x => {
                List<string> subClients = data.Where(y => y.CustCode == x).Select(z => z.SubClient).Distinct().ToList();
                SubClientList.Add(x, subClients);
            });
            List<string> ConsigneeList = data.Select(x => x.ConsigneeName).Distinct().ToList();
            List<string> ConsignerList = data.Select(x => x.ConsignerName).Distinct().ToList();
            window = new SanitizingWindow(dataGridHelper.getCurrentDataStack, db, dataGridHelper.currentSheetNumber, dataGrid, dataGridHelper, SubClientList, ConsigneeList, ConsignerList,dataToSend);
        
               window.Closed += SanitizingWindow_Closed;
            window.Show();
        }
        int currentRowsPerPage;
        private void concatinateAllRecordsInOnePage()
        {
            if (int.TryParse(DataGridNumOfRows.Text, out currentRowsPerPage))
                dataGridHelper.rowsPerPage = dataGridHelper.currentConnNos.Count;
            else
                currentRowsPerPage = 100;
        }

        private void SanitizingWindow_Closed(object sender, EventArgs e)
        {
            distributeAllRecords();
            /* BillingDataDataContext db = new BillingDataDataContext();
             dataGridHelper.currentDataSheet.dataStack = db.RuntimeDatas.Where(x => x.UserId == SecurityModule.currentUserName && x.SheetNo == dataGridHelper.currentSheetNumber).ToList();
        */
        }

        private void distributeAllRecords()
        {
            dataGridHelper.rowsPerPage = currentRowsPerPage;
        }
        #endregion
        #region PowerEntryCommand
        RoutedCommand PowerEntryCommand = new RoutedCommand();
        private void PowerEntryCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PowerEntry powerWin = new PowerEntry(dataGridHelper.getCurrentDataStack, dataGrid);
            powerWin.Closed += powerWin_Closed;
            powerWin.Show();
        }

        void powerWin_Closed(object sender, EventArgs e)
        {
            GC.Collect();
        }
        #endregion
        #region SaveCommand
        private void CanExecuteSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Debug.WriteLine("Here In");
            if (dataGridHelper != null)
            {
                if (dataGridHelper.getCurrentDataStack == null || SaveWorker.IsBusy == true || LoadWorker.IsBusy == true)
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
            MessageBlock.Text = DateTime.Now.ToShortTimeString() + ": " + "Saving operation started..." + "\n" + MessageBlock.Text;
            SaveDialogue.Visibility = Visibility.Visible;
            SaveWorker.RunWorkerAsync();
            //SaveWorker_DoWork(null, null);
        }
        #endregion
        #region printCommand

        private void ExecutePrint(object sender, ExecutedRoutedEventArgs e)
        {

            List<RuntimeData> cData = dataGridHelper.getCurrentDataStack;
            if (cData.Count(x => x.TransactionId == Guid.Empty || x.TransactionId == null) > 0)
            {
                MessageBox.Show("This data contains records that are loaded directly from a file. To print out an invoice data must loaded properly from database. ", "Error");
                return;
            }
            int count = cData.Count(x => x.FrAmount == null);
            if (count > 0)
            {
                if (MessageBox.Show("There are " + count.ToString() + " records whose billed amount is not set. Are you sure you want to continue? If you continue then amount billed for those those records will be equal to 0.", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            PrintWindow win = new PrintWindow(cData, cData.Select(x => x.BookingDate).Max(), cData.Select(x => x.BookingDate).Min());
            win.Show();
        }
        private void CanExecutePrintCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataGridHelper != null && dataGridHelper.areSheetsPresent && dataGridHelper.getCurrentDataStack.Count > 0 && SecurityModule.hasPermission(SecurityModule.employee.Id, "CreateInvoice"))
            {
                e.CanExecute = true;
            }
        }
        #endregion
        #region LoadCommand
        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SecurityModule.hasPermission(SecurityModule.employee.Id, "LoadData"))
            {
                if (LoadWorker != null)
                {
                    if (LoadWorker.IsBusy == true)
                        e.CanExecute = false;
                    else
                        e.CanExecute = true;
                }
                else
                    e.CanExecute = false;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        #endregion
        #region deleteCommand
        public RoutedCommand DeleteCommand = new RoutedCommand();
        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DeleteSheetWorker != null && dataGridHelper != null)
            {
                if (DeleteSheetWorker.IsBusy == true || buttonList.Count == 0 || SaveWorker.IsBusy == true || LoadWorker.IsBusy == true)
                {
                    e.CanExecute = false;
                }
                else
                    e.CanExecute = true;
            }
            else
                e.CanExecute = false;
        }
        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (buttonList.Count > 0)
            {
                Button b;
                MessageBoxResult result = MessageBoxResult.No;
                if (dataGridHelper.currentConnNos.Count < 1)
                    result = MessageBoxResult.Yes;
                else
                    result = MessageBox.Show("If you continue then you will lose all the unsaved data for this sheet. Please make sure that you have saved before closing the sheet. Click Yes to continue or click No to go back and save the sheet data", "Information", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteSheetWorker.RunWorkerAsync(dataGridHelper.currentSheetNumber);
                    dataGridHelper.removeSheet(dataGridHelper.currentSheetNumber);
                    buttontabcanvaswrap.Children.Remove(activeButton);
                    if (buttonList.Count > 0)
                        b = buttonList.Single(x => x.Value == buttonList.Values.Min()).Key;
                    else
                        b = null;
                    changeSheetButton(activeButton, b);
                    buttonList.Remove(activeButton);
                    activeButton = b;
                }
            }
        }
        #endregion
        #endregion
        #region CommandFunctions
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadData loadData = new LoadData();
            loadData.Closed += loadData_Closed;
            var blur = new BlurEffect();
            var current = this.Background;
            blur.Radius = 5;
            this.Effect = blur;
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




        #endregion
        #region DataGrid Methods
        void addingNewPage(int key)
        {

            dataGridHelper.getFirstPage();

            Button canvasButton = new Button();
            canvasButton.Style = (Style)FindResource("window");
            canvasButton.Margin = new Thickness(-9, 1, 0, 0);
            canvasButton.Height = 20;
            canvasButton.Width = 92;

            Canvas canvastab = new Canvas();
            canvastab.Width = 92;
            canvastab.Height = 20;

            Path pathsquare = new Path();
            pathsquare.Data = Geometry.Parse(@"F1M2,1.644C2,1.644 2,20 2,20 2,20 77.831,20 77.831,20 77.831,20 91.619,1.644 91.619,1.644 91.619,1.644 2,1.644 2,1.644z");
            pathsquare.Fill = Brushes.Black;
            pathsquare.Height = 20;
            pathsquare.Width = 92;
            pathsquare.Stretch = Stretch.Fill;
            Button buttonsquare = new Button();
            buttonsquare.Name = "_pathsquare";
            buttonsquare.Style = (Style)FindResource("window");
            buttonsquare.Content = pathsquare;


            TextBlock text = new TextBlock();
            text.Name = "Sheettext";
            text.Text = "Sheet- " + key.ToString();
            text.Foreground = Brushes.White;
            text.FontSize = 16;
            Canvas.SetLeft(text, 4);
            Canvas.SetTop(text, -2);

            Button buttonkatta = new Button();
            buttonkatta.Style = (Style)FindResource("CloseButton");
            buttonkatta.Height = 10;
            buttonkatta.Width = 10;
            buttonkatta.Name = "CloseCurrentClick";
            buttonkatta.Command = DeleteCommand;
            Canvas.SetLeft(buttonkatta, 68.5);
            Canvas.SetTop(buttonkatta, 5);

            canvastab.Children.Add(buttonsquare);
            canvastab.Children.Add(buttonkatta);
            canvastab.Children.Add(text);
            canvasButton.Content = canvastab;
            canvasButton.Click += SheetSelectButton_Click;
            buttontabcanvaswrap.Children.Add(canvasButton);
            buttonList.Add(canvasButton, key);
            if (activeButton != null)
                changeSheetButton(activeButton, canvasButton);
            activeButton = canvasButton;
        }
        bool isLoadedFromBook = true;
        DateTime? toDate_loadDataWin;
        DateTime? fromDate_loadDataWin;
        void loadData_Closed(object sender, EventArgs e)
        {
            this.Effect = null;
            LoadData dataWind = (LoadData)sender;
            if (dataWind.dataLoaded == false)
                return;
            //TODO: Get Name 
            string name = "";
            string stockStart = "", stockEnd = "";
            if (dataWind.isLoadedFromBook == true)
            {
                try
                {
                    BillingDataDataContext db = new BillingDataDataContext();
                    Stock stock = db.Stocks.Single(x => x.BookNo == dataWind.BookNo);
                    stockStart = stock.StockStart;
                    stockEnd = stock.StockEnd;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message, "Error");
                    return;
                }
            }
            if (dataWind.dataLoaded)
            {
                isLoadedFromBook = dataWind.isLoadedFromBook;
                toDate_loadDataWin = dataWind.toDate;
                fromDate_loadDataWin = dataWind.fromDate;
            }
            
            if (dataWind.isNewSheet || dataGridHelper.CurrentNumberOfSheets <= 0)
            {
                if (dataWind.isLoadedFromBook == false)
                    dataWind.data = UtilityClass.loadDataFromDatabase(toDate_loadDataWin ?? DateTime.Now, fromDate_loadDataWin ?? DateTime.Now, dataGridHelper.currentMaxSheetNumber + 1);
                else
                {
                    dataWind.data = UtilityClass.loadDataFromBook(dataGridHelper.currentMaxSheetNumber + 1,stockStart,stockEnd);
                }

                int key = dataGridHelper.addNewSheet(dataWind.data, name);
                addingNewPage(key);
            }
            else
            {
                if (dataWind.isLoadedFromBook== false)
                    dataWind.data = UtilityClass.loadDataFromDatabase(toDate_loadDataWin ?? DateTime.Now, fromDate_loadDataWin ?? DateTime.Now, dataGridHelper.currentSheetNumber);
                else
                    dataWind.data = UtilityClass.loadDataFromBook(dataGridHelper.currentSheetNumber, stockStart, stockEnd);
                dataGridHelper.addDataToCurrentSheet(dataWind.data);
                dataGridHelper.refreshCurrentPage();
            }

            MessageBlock.Text = DateTime.Now.ToShortTimeString() + ": " + "Data loading successfull" + "\n" + MessageBlock.Text;
        }


        void changeSheetButton(Button current_active, Button button)
        {
            if (!current_active.Equals(button))
            {
                Canvas c = (Canvas)button.Content;
                foreach (UIElement u in c.Children)
                {
                    if (u is Button)
                    {
                        var p = (Button)u;

                        if (p.Name == "_pathsquare")
                        {
                            var b = (Path)p.Content;
                            b.Fill = Brushes.Black;
                        }
                    }
                    if (u is TextBlock)
                    {
                        var p = (TextBlock)u;
                        if (p.Name == "Sheettext")
                        {
                            p.Foreground = Brushes.White;
                        }
                    }
                }
                c = (Canvas)current_active.Content;
                foreach (UIElement u in c.Children)
                {
                    if (u is Button)
                    {
                        var p = (Button)u;

                        if (p.Name == "_pathsquare")
                        {
                            var b = (Path)p.Content;
                            b.Fill = Brushes.AliceBlue;
                        }
                    }
                    if (u is TextBlock)
                    {
                        var p = (TextBlock)u;
                        if (p.Name == "Sheettext")
                        {
                            p.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }
        private void SheetSelectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            dataGridHelper.setActiveSheet(buttonList[button]);
            dataGridHelper.refreshCurrentPage();
            changeSheetButton(activeButton, button);
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
            if (dataGridHelper.areSheetsPresent)
            {
                FilterSelectWindow window = new FilterSelectWindow(dataGridHelper.currentDataSheet.filterObj, dataGridHelper.currentConnNosNoFilter);
                window.Closed += window_Closed;
                window.Show();
            }
        }
        void window_Closed(object sender, EventArgs e)
        {
            dataGridHelper.currentDataSheet.applyFilter();
            dataGridHelper.refreshCurrentPage();
        }
        #endregion
        #endregion*-
        #region WindowResizingMenuAndUtilities
        #region Handling Resizing

        #endregion
        #region custom window resize
        protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }

        private HwndSource _hwndSource;
        protected override void OnInitialized(EventArgs e)
        {
            SourceInitialized += OnSourceInitialized;
            base.OnInitialized(e);
        }
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }

        #endregion
        #region menuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            Login window = new Login(); window.Show(); this.Close();
            //Application.Current.Shutdown();
        }
        private void ManageClient_Click(object sender, RoutedEventArgs e)
        {
            ManageClient window = new ManageClient();
            window.Closed += AddClient_close;
            window.Show();
        }
        private void ManageEmployee_Click(object sender, RoutedEventArgs e)
        {
            ManageEmployee window = new ManageEmployee(); window.Show();
        }
        private void RateWindowMenu_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ViewHelp_Click(object sender, RoutedEventArgs e)
        {
            viewhelp window = new viewhelp(); window.ShowDialog();
        }
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            About window = new About(); window.ShowDialog();
        }
        #endregion
        private void BillAnalysis_Click(object sender, RoutedEventArgs e)
        {
            BillReportWindow win = new BillReportWindow();
            win.ShowDialog();
        }
        private void PaymentEntry_Click(object sender, RoutedEventArgs e)
        {
            PaymentDetailsWindow window = new PaymentDetailsWindow(); window.Show();
        }
        private void PaymentRecieved_Click(object sender, RoutedEventArgs e)
        {
            PaymentRecieved window = new PaymentRecieved(); window.ShowDialog();
        }
        private void StockEntry_Click(object sender, RoutedEventArgs e)
        {
            StockWindow window = new StockWindow(); window.Show();
        }
        private void ClientReport_Click(object sender, RoutedEventArgs e)
        {
            ClientReportWindow window = new ClientReportWindow(); window.ShowDialog();
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            AddCity window = new AddCity();
            window.Show();
        }
        private void ManageServices_Click(object sender, RoutedEventArgs e)
        {
            ManageService window = new ManageService();
            window.Show();
        }
        private void ManageZone_Click(object sender, RoutedEventArgs e)
        {
            ZoneAssignment zone = new ZoneAssignment();
            zone.ShowDialog();
        }
        private void ManageStock_Click(object sender, RoutedEventArgs e)
        {
            StockManagmentWindow window = new StockManagmentWindow();
            window.Show();
        }
        private void AccountStatementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AccountStatementReportingWindow window = new AccountStatementReportingWindow(); window.WindowState = WindowState.Maximized; window.Show();
        }
        private void mangaEmployeegrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            employeeToEdit.Add((Employee)e.Row.DataContext);
        }
        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient window = new AddClient();
            window.Closed += AddClient_close;
            window.Show();
        }
        private void updateClient_Click(object sender, RoutedEventArgs e)
        {
            client = (Client)this.mangaclientgrid.SelectedItem;
            AddClient add = new AddClient(client);
            add.Closed += AddClient_close;
            add.ShowDialog();
        }
        private void AddClient_close(object sender, EventArgs e)
        {
            DataSources.refreshClientList();
            clientViewSource.Source = DataSources.ClientCopy;
            mangaclientgrid.Items.Refresh();
            ClientCombo.Items.Refresh();

        }
        #region sidepanel
        private void cloakAll()
        {
            this.DataDockPanel.Visibility = Visibility.Collapsed;
            this.ClientReportOptionPanel.Visibility = Visibility.Collapsed;
            this.QuotationoptionPanel.Visibility = Visibility.Collapsed;
            this.ClientOptionPanel.Visibility = Visibility.Collapsed;
            this.EmployeeOptionPanel.Visibility = Visibility.Collapsed;
            this.CityOptionPanel.Visibility = Visibility.Collapsed;
            this.HideAllDatagrid.Visibility = Visibility.Collapsed;
            this.FilterQuotation.Visibility = Visibility.Collapsed;

        }
        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (CostingRuleRadio.IsChecked==true)
            {
                AddRule window = new AddRule(new BillingDataDataContext().Quotations.Where(x => x.CLCODE == ((Client)this.ClientCombo.SelectedItem).CLCODE).FirstOrDefault());
                window.Closed += addRulwWindow_Closed;
                window.Show();
            }
            if(ServiceRuleRadio.IsChecked==true)
            {
                AddServiceRule window = new AddServiceRule(new BillingDataDataContext().Quotations.Where(x => x.CLCODE == ((Client)this.ClientCombo.SelectedItem).CLCODE).FirstOrDefault());
                window.Closed += ServiceRuleWindowClosed;
                window.Show();
            }
            
        }

        private void addRulwWindow_Closed(object sender, EventArgs e)
        {
            LoadClientRules();
            this.CostingRuleGrid.Items.Refresh();
        }
        private void ServiceRuleWindowClosed(object sender, EventArgs e)
        {
            LoadClientRules();
            this.ServiceRuleGrid.Items.Refresh();
        }

        private void DataGrid_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
        }
        private void ClientQuotationTreeView_Selected(object sender, RoutedEventArgs e)
        {
            cloakAll();
            QuotationoptionPanel.Visibility = Visibility.Visible;
            FilterQuotation.Visibility = Visibility.Visible;
        }
        private void ClientsReportTreeView_Selected(object sender, RoutedEventArgs e)
        {
            cloakAll();
            ClientReportOptionPanel.Visibility = Visibility.Visible;
        }
        private void DataDockPanelTreeView_Selected(object sender, RoutedEventArgs e)
        {
            cloakAll();
            DataDockPanel.Visibility = Visibility.Visible;
        }
        private void clienttreeviewitembutton_Click(object sender, RoutedEventArgs e)
        {
            cloakAll();
            ClientOptionPanel.Visibility = Visibility.Visible;
        }
        private void Employeetreeviewitembutton_Click(object sender, RoutedEventArgs e)
        {
            cloakAll();
            EmployeeOptionPanel.Visibility = Visibility.Visible;
        }
        private void citytreeviewitembutton_Click(object sender, RoutedEventArgs e)
        {
            cloakAll();
            CityOptionPanel.Visibility = Visibility.Visible;
        }
        private void CloseTableTreeView_Selected(object sender, RoutedEventArgs e)
        {
            cloakAll();
            HideAllDatagrid.Visibility = Visibility.Visible;
        }
        #endregion
        #endregion
        #region QuotationFunctions
        private void ClientCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                e.AddedItems.Cast<Client>().Count();
            }
            catch (Exception ex)
            {
                return;
            }

            LoadClientRules();
        }
        private void LoadClientRules()
        {
            if (isInitialized == true)
            {
                if (CostingRulesSource == null)
                {
                    CostingRulesSource = (CollectionViewSource)FindResource("CostingRuleList");
                }
                if (serviceRulesView == null)
                {
                    serviceRulesView = (CollectionViewSource)FindResource("ServiceRuleList");
                }
                BillingDataDataContext db = new BillingDataDataContext();
                qutObj = db.Quotations.SingleOrDefault(x => x.CLCODE == ((Client)ClientCombo.SelectedItem).CLCODE);
                if (qutObj == null)
                {
                    unLoadQuotation();
                }
                else
                {
                    loadQuotation(qutObj);
                }
            }
        }
        void unLoadQuotation()
        {
            CostingRulesSource.Source = null;
            serviceRulesView.Source = null;
            CostingRuleGrid.Items.Refresh();
            ServiceRuleGrid.Items.Refresh();
        }
        void loadQuotation(Quotation qutObj)
        {
            CostingRulesSource.Source = qutObj.CostingRules;
            serviceRulesView.Source = qutObj.ServiceRules;
        }
        private void cloakAllGrid()
        {
            if (CostingRuleGrid != null && ServiceRuleGrid != null)
            {
                CostingRuleGrid.Visibility = Visibility.Collapsed;
                ServiceRuleGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void CostingRuleRadio_Checked_1(object sender, RoutedEventArgs e)
        {
            if (isInitialized == true)
            {
                try
                {
                    cloakAllGrid();
                    CostingRuleGrid.Visibility = Visibility.Visible;
                }
                catch (NullReferenceException ex)
                { }
            }
        }

        private void ServiceRuleRadio_Checked(object sender, RoutedEventArgs e)
        {
            cloakAllGrid();
            ServiceRuleGrid.Visibility = Visibility.Visible;
        }
        #endregion
        #region Employee Grid Buttons
        private void ReloadEmployeeGrid_Click(object sender, RoutedEventArgs e)
        {
            DataSources.refreshEmployeeList();
            this.employees = DataSources.EmployeeCopy;
            employeeview.Source = this.employees;
            mangaEmployeegrid.Items.Refresh();
        }
        private void EditEmployeeGrid_Click(object sender, RoutedEventArgs e)
        {
            if (mangaEmployeegrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one employee to edit");
                return;
            }
            AddEmployee addEmpWin = new AddEmployee((Employee)mangaEmployeegrid.SelectedItem);
            addEmpWin.Closed += addEmpWin_Closed;
            addEmpWin.ShowDialog();
        }

        void addEmpWin_Closed(object sender, EventArgs e)
        {
            ReloadEmployeeGrid_Click(null, null);
        }
        private void AddEmployeeGrid_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmpWin = new AddEmployee();
            addEmpWin.Closed += addEmpWin_Closed;
            addEmpWin.ShowDialog();
        }
        private void DeleteEmployeeGrid_Click(object sender, RoutedEventArgs e)
        {
            if (mangaEmployeegrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select 1 employee to delete");
                return;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this employee?", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    BillingDataDataContext db = new BillingDataDataContext();
                    Employee emp = db.Employees.SingleOrDefault(x => x.EMPCode == ((Employee)mangaEmployeegrid.SelectedItem).EMPCode);
                    db.Employees.DeleteOnSubmit(emp);
                    db.SubmitChanges();
                    ReloadEmployeeGrid_Click(null, null);
                }
            }
        }
        #endregion
        #region Client Grid Buttons
        private void ReloadClientGridButton_Click(object sender, RoutedEventArgs e)
        {
            DataSources.refreshClientList();
            clientViewSource.Source = DataSources.ClientCopy;
            mangaclientgrid.Items.Refresh();
        }
        private void UpdateClientGridButton_Click(object sender, RoutedEventArgs e)
        {
            if (mangaclientgrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select 1 client to edit.");
                return;
            }
            AddClient addClientWin = new AddClient((Client)mangaclientgrid.SelectedItem);
            addClientWin.Closed += addClientWin_Closed;
            addClientWin.ShowDialog();
        }
        void addClientWin_Closed(object sender, EventArgs e)
        {
            ReloadClientGridButton_Click(null, null);
        }
        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddClient addClientWin = new AddClient();
            addClientWin.Closed += addClientWin_Closed;
            addClientWin.ShowDialog();
        }
        private void DeleteClientGridButton_Click(object sender, RoutedEventArgs e)
        {
            if (mangaclientgrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select 1 client to delete");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure want to delete this client?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (db == null)
                    db = new BillingDataDataContext();
                Client client = db.Clients.SingleOrDefault(x => x.CLCODE == ((Client)mangaclientgrid.SelectedItem).CLCODE);
                if (client == null)
                {
                    MessageBox.Show("No such client");
                    return;
                }
                if (client.CLCODE == "<NONE>" || client.CLCODE == "<DTDC>")
                {
                    MessageBox.Show("This client cannot be deleted");
                    return;
                }

                db.Clients.DeleteOnSubmit(client);
                db.SubmitChanges();
            }
            ReloadClientGridButton_Click(null, null);
        }
        #endregion
        private void ReloadCityButton_Click(object sender, RoutedEventArgs e)
        {
            DataSources.refreshCityList();
            cityViewSource.Source = DataSources.CityCopy;
            CityDataGrid.Items.Refresh();
        }
        private void EditCityButton_Click(object sender, RoutedEventArgs e)
        {
            if (CityDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select 1 city to edit");
                return;
            }
            AddCity addCityWin = new AddCity(((City)CityDataGrid.SelectedItem).CITY_CODE);
            addCityWin.Closed += addCityWin_Closed;
            addCityWin.ShowDialog();
        }
        void addCityWin_Closed(object sender, EventArgs e)
        {
            ReloadCityButton_Click(null, null);
        }
        private void AddCityButton_Click(object sender, RoutedEventArgs e)
        {
            AddCity addCityWin = new AddCity();
            addCityWin.Closed += addCityWin_Closed;
            addCityWin.ShowDialog();
        }
        private void DeleteCityButton_Click(object sender, RoutedEventArgs e)
        {
            if (CityDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select 1 city to delete");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this city?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (db == null)
                    db = new BillingDataDataContext();
                City city = db.Cities.SingleOrDefault(x => x.CITY_CODE == ((City)CityDataGrid.SelectedItem).CITY_CODE);
                if (city == null)
                {
                    MessageBox.Show("No such city exists.");
                    return;
                }
                db.Cities.DeleteOnSubmit(city);
                db.SubmitChanges();
            }
            ReloadCityButton_Click(null, null);
        }
        #region City Grid Button

        #endregion
        #region Config Load
        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Configs.Default.Background));
        }
        #endregion
        private void Peferences_Click(object sender, RoutedEventArgs e)
        {
            PreferenceWindow window = new PreferenceWindow();
            window.Show();
        }
        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if (CostingRuleGrid.Visibility == Visibility.Visible && CostingRuleGrid.SelectedItems != null)
            {
                if (MessageBox.Show("Do you want delete " + CostingRuleGrid.SelectedItems.Count.ToString() +" Rule", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    List<CostingRule> dcr = CostingRuleGrid.SelectedItems.Cast<CostingRule>().ToList();
                    CostingRuleGrid.SelectedItem = null;
                    List<int> Ids = dcr.Select(x=>x.Id).ToList();
                    List<Rule> dr = db.Rules.Where(x => Ids.Contains(x.ID)).ToList();
                    db.Rules.DeleteAllOnSubmit(dr);
                }
            }
            if (ServiceRuleGrid.Visibility == Visibility.Visible && ServiceRuleGrid.SelectedItems != null)
            {
                if (MessageBox.Show("Do you want delete " + ServiceRuleGrid.SelectedItems.Count.ToString() + " Rule", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    List<ServiceRule> dcr = ServiceRuleGrid.SelectedItems.Cast<ServiceRule>().ToList();
                    ServiceRuleGrid.SelectedItem = null;
                    List<int> Ids = dcr.Select(x => x.Id).ToList();
                    List<Rule> dr = db.Rules.Where(x => Ids.Contains(x.ID)).ToList();
                    db.Rules.DeleteAllOnSubmit(dr);
                }
            }
            db.SubmitChanges();
            LoadClientRules();
            this.CostingRuleGrid.Items.Refresh();
            this.ServiceRuleGrid.Items.Refresh();
        }
        private void Calculator_Click(object sender, RoutedEventArgs e)
        {
            if (Calculator.Visibility != Visibility.Collapsed)
            {
                Calculator.Visibility = Visibility.Collapsed;
            }
            else
            {
                Calculator.Visibility = Visibility.Visible;
            }
        }
        private void ImportRule_Click(object sender, RoutedEventArgs e)
        {
            Client c = (Client)Client_Combo.SelectedItem;
            ImportRules importRulesWindow = new ImportRules(c);
            importRulesWindow.Closed += importRulesWindow_Closed;
            importRulesWindow.Show();
        }

        void importRulesWindow_Closed(object sender, EventArgs e)
        {
            LoadClientRules();
            this.CostingRuleGrid.Items.Refresh();
            this.ServiceRuleGrid.Items.Refresh();
        }

        private void GetRateButton_Click(object sender, RoutedEventArgs e)
        {
            Client client = (Client)Client_Combo.SelectedItem;
            Service service = (Service)Service_Combo.SelectedItem;
            char dox = Dox_Combo.Text == "Non-Dox" ? 'N' : 'D';
            City city = (City)City_Combo.SelectedItem;
            double weight;
            if (!double.TryParse(WeightRuleTextBox.Text, out weight))
            {
                return;
            }
            if (client == null || service == null || city == null)
                return;
            RateRuleTextBox.Text = UtilityClass.getCost(client.CLCODE, weight, city.CITY_CODE, service.SER_CODE, dox).ToString();
        }
        private void ClientReportReloadButton_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            dueDataGridSource.Source = db.BalanceViews;
            profitDataGridSource.Source = db.PROFITVIEWs;
            ClientDueGrid.Items.Refresh();
            ClientProfitabilityGrid.Items.Refresh();
        }
        private void EditRule_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if (CostingRuleGrid.Visibility == Visibility.Visible && CostingRuleGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Do you Want edit this Rule", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    CostingRule dcr = (CostingRule)CostingRuleGrid.SelectedItem;
                    AddRule win = new AddRule(dcr.Id);
                    win.Closed += win_Closed;
                    win.Show();
                    CostingRuleGrid.SelectedItem = null;
                }
            }
            if (ServiceRuleGrid.Visibility == Visibility.Visible && ServiceRuleGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Do you Want edit this Rule", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ServiceRule dcr = (ServiceRule)ServiceRuleGrid.SelectedItem;
                    AddServiceRule win = new AddServiceRule(dcr.Id);
                    win.Closed += win_Closed;
                    win.Show();
                    ServiceRuleGrid.SelectedItem = null;
                }
            }
            db.SubmitChanges();
        }
        private void win_Closed(object sender, EventArgs e)
        {
            LoadClientRules();
            this.CostingRuleGrid.Items.Refresh();
            this.ServiceRuleGrid.Items.Refresh();
        }

        private void InvoiceAnalysis_Click(object sender, RoutedEventArgs e)
        {
            InvoiceReport invoiceReportWindow = new InvoiceReport();
            invoiceReportWindow.Show();
        }

        private void WeightRuleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetRateButton_Click(null, null);
            }
        }

        private void DataImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            importfile importFileWindow = new importfile();
            importFileWindow.Show();
        }
    }
}