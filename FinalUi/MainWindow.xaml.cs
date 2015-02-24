using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Web.Script.Serialization;
namespace FinalUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private List<CostingRule> costingRules;
        private CollectionViewSource serviceRulesView;
        private CollectionViewSource ServiceTable;
        private Quotation qutObj;
        // Employee listing data import procedure
        // Client listing data import procedure
        List<Client> clientToEdit;
        CollectionViewSource clientViewSource;
        CollectionViewSource cityViewSource;
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
        CollectionViewSource CostingRulesSource;

        #endregion
        bool isInitialized = false;

        public MainWindow()
        {
            InitializeComponent();
            isInitialized = true;
            // Client listing data import procedure
            clientToEdit = new List<Client>();
            clientViewSource = (CollectionViewSource)FindResource("ClienTable");
            clientViewSource.Source = DataSources.ClientCopy;
            cityViewSource = (CollectionViewSource)FindResource("CityTable");
            cityViewSource.Source = DataSources.CityCopy;
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
            if(Configs.Default.Background == null || Configs.Default.Background == "")
            {
                Configs.Default.Background = "LightSeaGreen";
                Configs.Default.Save();
            }
            this.MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Configs.Default.Background));
            db = new BillingDataDataContext();
            ResourceDictionary dict = this.Resources;
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
            Update up = new Update();
            if (up.checkUpdate() < 0)
            {
                this.UpdateMenuButton.Visibility = Visibility.Visible;
            }
            else { this.UpdateMenuButton.Visibility = Visibility.Collapsed; }
            setUiFromPermissions();
        }
        private void setUiFromPermissions()
        {
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "Management"))
            {
                this.Managment.Visibility = Visibility.Collapsed;
                this.DeleteConnMenuItem.Visibility = System.Windows.Visibility.Collapsed;
                this.UpdateMenuButton.Visibility = Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "AccountStatement"))
            {
                this.AccountStatementMenuItem.Visibility = Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "ManageQuotation"))
            {
                this.Quotation.Visibility = Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "CreateInvoice"))
            {
                this.PrintButton.Visibility = this.PrintMenuItem.Visibility = this.AfterPrint.Visibility = Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "Analysis"))
            {
                this.InvoiceAnalysis.Visibility = this.MISReportMenuItem.Visibility = this.ClientReport.Visibility = Visibility.Collapsed;
            }
            if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "PaymentEntry"))
            {
                this.PaymentEntryMenuItem.Visibility = Visibility.Collapsed;
            }
            //if (!SecurityModule.hasPermission(SecurityModule.employee.Id, "ManageInvoice"))
            //{
            //    this.PrintButton.Visibility = this.PrintMenuItem.Visibility = this.AfterPrint.Visibility = Visibility.Collapsed;
            //}

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
            BillingDataDataContext db = new BillingDataDataContext();
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
            BillingDataDataContext db = new BillingDataDataContext();
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
            clients.ForEach(x =>
            {
                List<string> subClients = data.Where(y => y.CustCode == x).Select(z => z.SubClient).Distinct().ToList();
                SubClientList.Add(x, subClients);
            });
            List<string> ConsigneeList = data.Select(x => x.ConsigneeName).Distinct().ToList();
            List<string> ConsignerList = data.Select(x => x.ConsignerName).Distinct().ToList();
            window = new SanitizingWindow(dataGridHelper.getCurrentDataStack, db, dataGridHelper.currentSheetNumber, dataGrid, dataGridHelper, SubClientList, ConsigneeList, ConsignerList, dataToSend);

            window.Closed += SanitizingWindow_Closed;
            try
            {
                window.Show();
            }
            catch (Exception ex) { }
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
            try
            {
                powerWin.Show();
            }
            catch (Exception ex) { }
        }

        void powerWin_Closed(object sender, EventArgs e)
        {
            GC.Collect();
        }
        #endregion
        #region SaveCommand
        private void CanExecuteSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
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
             int count ;
            if((count = cData.Count(x=>x.FrAmount == 0)) > 0)
            {
                if (MessageBoxResult.No == MessageBox.Show("There are " + count.ToString() + " records whose billed amount is 0. Are you sure you want to continue?"))
                    return;
            }
            count= cData.Count(x => x.FrAmount == null);
            if (count > 0)
            {
                if (MessageBox.Show("There are " + count.ToString() + " records whose billed amount is not set. Are you sure you want to continue? If you continue then amount billed for those those records will be equal to 0.", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            PrintWindow win = new PrintWindow(cData, cData.Select(x => x.BookingDate).Max(), cData.Select(x => x.BookingDate).Min());
            try
            {
                win.Show();
            }
            catch (Exception ex) { }
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
                    try
                    {
                        result = MessageBox.Show("Do you want to save this sheet", "Information", MessageBoxButton.YesNoCancel);
                    }
                    catch (Exception ex) { }
                if (result == MessageBoxResult.No)
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
                else if (result == MessageBoxResult.Yes)
                {
                    ExecuteSaveCommand(null, null);
                    RunWorkerCompletedEventHandler workerCompleted = null;
                    workerCompleted = (obj, senderP) =>
                        {
                            if (senderP.Error == null && senderP.Cancelled == false)
                            {
                                DeleteSheetWorker.RunWorkerAsync(dataGridHelper.currentSheetNumber);
                                SaveWorker.RunWorkerCompleted -= workerCompleted;
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
                            else
                            {
                                MessageBox.Show("Save operation unsuccessfull...", "Error");
                            }
                            SaveWorker.RunWorkerCompleted -= workerCompleted;
                        };
                    SaveWorker.RunWorkerCompleted += workerCompleted;
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

                    stockStart = dataWind.startConnNo;
                    stockEnd = dataWind.endConnNo;
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
                    dataWind.data = UtilityClass.loadDataFromDatabase(fromDate_loadDataWin ?? DateTime.Now, toDate_loadDataWin ?? DateTime.Now, dataGridHelper.currentMaxSheetNumber + 1);
                else
                {
                    dataWind.data = UtilityClass.loadDataFromBook(dataGridHelper.currentMaxSheetNumber + 1, stockStart, stockEnd);
                }

                int key = dataGridHelper.addNewSheet(dataWind.data, name);
                addingNewPage(key);
            }
            else
            {
                if (dataWind.isLoadedFromBook == false)
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

        #endregion DataGrid Methods Ends
        #region filter functions
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridHelper.areSheetsPresent)
            {
                FilterSelectWindow window = new FilterSelectWindow(dataGridHelper.currentDataSheet.filterObj, dataGridHelper.currentConnNosNoFilter);
                window.Closed += window_Closed;
                try
                {
                    window.Show();
                }
                catch (Exception ex) { }
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
            try
            {
                window.Show();
            }
            catch (Exception ex) { }
        }
        private void ManageEmployee_Click(object sender, RoutedEventArgs e)
        {
            ManageEmployee window = new ManageEmployee();
            try
            {
                window.Show();
            }
            catch (Exception ex) { }
        }
        private void RateWindowMenu_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ViewHelp_Click(object sender, RoutedEventArgs e)
        {
            viewhelp window = new viewhelp();
            try
            {
                window.ShowDialog();
            }
            catch (Exception ex) { }
        }
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            About window = new About(); try
            {
                window.ShowDialog();
            }
            catch (Exception ex) { }
        }
        #endregion
        private void PaymentEntry_Click(object sender, RoutedEventArgs e)
        {
            PaymentDetailsWindow window = new PaymentDetailsWindow(); try
            {
                window.Show();
            }
            catch (Exception ex) { }
        }
        private void PaymentRecieved_Click(object sender, RoutedEventArgs e)
        {
            PaymentRecieved window = new PaymentRecieved();
            try
            {
                window.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }
        private void ClientReport_Click(object sender, RoutedEventArgs e)
        {
            ClientReportWindow window = new ClientReportWindow(); try
            {
                window.ShowDialog();
            }
            catch (Exception ex) { }
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ManageCity window = new ManageCity();
            try
            {
                window.Show();
            }
            catch (Exception ex) { }
            }

        private void ManageServices_Click(object sender, RoutedEventArgs e)
        {
            ManageService window = new ManageService();
            try
            {
                window.Show();
            }
            catch (Exception ex) { }
        }
        private void ManageZone_Click(object sender, RoutedEventArgs e)
        {
            ZoneAssignment zone = new ZoneAssignment();
            try
            {
                zone.ShowDialog();
            }
            catch (Exception ex) { }
        }
        private void ManageStock_Click(object sender, RoutedEventArgs e)
        {
            StockManagmentWindow window = new StockManagmentWindow();
            try
            {
                window.Show();
            } catch (Exception ex) { }
        }
        private void AccountStatementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AccountStatementReportingWindow window = new AccountStatementReportingWindow(); window.WindowState = WindowState.Maximized; try
            {
                window.Show();
            }
            catch (Exception ex) { }
        }
        private void AddClient_close(object sender, EventArgs e)
        {
            DataSources.refreshClientList();
            clientViewSource.Source = DataSources.ClientCopy;
            ClientCombo.Items.Refresh();
        }
        #region sidepanel
        private void cloakAll()
        {
            this.DataDockPanel.Visibility = Visibility.Collapsed;
            this.QuotationoptionPanel.Visibility = Visibility.Collapsed;
            this.HideAllDatagrid.Visibility = Visibility.Collapsed;
            //this.FilterQuotation.Visibility = Visibility.Collapsed;

        }
        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (CostingRuleRadio.IsChecked == true)
            {
                AddRule window = new AddRule(new BillingDataDataContext().Quotations.Where(x => x.CLCODE == ((Client)this.ClientCombo.SelectedItem).CLCODE).FirstOrDefault());
                window.Closed += addRulwWindow_Closed;
                window.Show();
            }
            if (ServiceRuleRadio.IsChecked == true)
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
            //FilterQuotation.Visibility = Visibility.Visible;
        }
        private void DataDockPanelTreeView_Selected(object sender, RoutedEventArgs e)
        {
            cloakAll();
            DataDockPanel.Visibility = Visibility.Visible;
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
            ObservableCollection<CostingRule> cRules = new ObservableCollection<CostingRule>(qutObj.CostingRules);
            ObservableCollection<ServiceRule> sRule = new ObservableCollection<ServiceRule>(qutObj.ServiceRules);
            ICollectionView cRuleView = CollectionViewSource.GetDefaultView(cRules);
            ICollectionView sRuleView = CollectionViewSource.GetDefaultView(sRule);
            cRuleView.GroupDescriptions.Add(new PropertyGroupDescription("serviceGroupReporting"));
            sRuleView.GroupDescriptions.Add(new PropertyGroupDescription("serviceGroupReporting"));
            cRuleView.GroupDescriptions.Add(new PropertyGroupDescription("zoneListReporting"));
            sRuleView.GroupDescriptions.Add(new PropertyGroupDescription("zoneListReporting"));
            CostingRuleGrid.DataContext = cRuleView;
            ServiceRuleGrid.DataContext = sRuleView;
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
        #region Config Load
        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MainGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Configs.Default.Background));
        }
        #endregion
        private void Peferences_Click(object sender, RoutedEventArgs e)
        {
            PreferenceWindow window = new PreferenceWindow();
            try
            {
                window.Show();
            }
            catch (Exception ex) { }
        }
        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if (CostingRuleGrid.Visibility == Visibility.Visible && CostingRuleGrid.SelectedItems != null)
            {
                try {
                    if (MessageBox.Show("Do you want delete " + CostingRuleGrid.SelectedItems.Count.ToString() + " Rule", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        List<CostingRule> dcr = CostingRuleGrid.SelectedItems.Cast<CostingRule>().ToList();
                        CostingRuleGrid.SelectedItem = null;
                        List<int> Ids = dcr.Select(x => x.Id).ToList();
                        List<Rule> dr = db.Rules.Where(x => Ids.Contains(x.ID)).ToList();
                        db.Rules.DeleteAllOnSubmit(dr);
                    } }
                catch (Exception ex) {}
                

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
        private void EditRule_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if (CostingRuleGrid.Visibility == Visibility.Visible && CostingRuleGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Do you Want edit this Rule", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    try
                    {
                        CostingRule dcr = (CostingRule)CostingRuleGrid.SelectedItem;
                        AddRule win = new AddRule(dcr.Id);
                        win.Closed += win_Closed;
                        win.Show();
                        CostingRuleGrid.SelectedItem = null;
                    }
                    catch (Exception ex) { }
                }
            }
            if (ServiceRuleGrid.Visibility == Visibility.Visible && ServiceRuleGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Do you Want edit this Rule", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    try
                    {
                        ServiceRule dcr = (ServiceRule)ServiceRuleGrid.SelectedItem;
                        AddServiceRule win = new AddServiceRule(dcr.Id);
                        win.Closed += win_Closed;

                        win.Show();
                        ServiceRuleGrid.SelectedItem = null;
                    }
                    catch (Exception ex) { }
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
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            RecalculatePriceWindow win = new RecalculatePriceWindow();
            try
            {
                win.ShowDialog();
            }
            catch (Exception ex) { }
        }
        private void Quotation_Calc_click(object sender, RoutedEventArgs e)
        {
            QuotationCalc win = new QuotationCalc();
            try
            {
                win.Show();
            }
            catch (Exception ex) { }
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            int rowNumOffset = (dataGridHelper.currentPageNo - 1) * (dataGridHelper.rowsPerPage) + 1;
            e.Row.Header = (e.Row.GetIndex() + rowNumOffset).ToString();
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.EditAction == DataGridEditAction.Commit)
            {
                RuntimeData rData = ((RuntimeData)e.Row.Item);
                double weight;
                if (double.TryParse((e.EditingElement as TextBox).Text, out weight))
                {
                    if (e.Column.Header.ToString() == "Billed Weight")
                        rData.BilledWeight = weight;
                    else if (e.Column.Header.ToString() == "Billed Amount")
                        rData.FrAmount = (decimal)weight;
                }
                else
                {
                    MessageBox.Show("Unable to edit record!! Please check if input does not contain any invalid characters.", "Error");
                    return;
                }
                if (rData == null)
                {
                    MessageBox.Show("Unable to edit record....");
                    return;
                }
                RuntimeData sData = dataGridHelper.getCurrentDataStack.SingleOrDefault(x => x.ConsignmentNo == rData.ConsignmentNo);
                if (sData == null)
                {
                    MessageBox.Show("Unable to edit transaction. Please reload the data and try again..", "Error");
                    return;
                }
                BillingDataDataContext db = new BillingDataDataContext();
                RuntimeData dData = db.RuntimeDatas.SingleOrDefault(x => x.ConsignmentNo == rData.ConsignmentNo && x.UserId == SecurityModule.currentUserName && dataGridHelper.currentSheetNumber == x.SheetNo);
                if (dData == null)
                {
                    MessageBox.Show("Unable to edit transaction. Please check if data exists and try again..", "Error");
                    return;
                }
                if (e.Column.Header.ToString() == "Billed Weight")
                {
                    dData.BilledWeight = rData.BilledWeight;
                    dData.FrAmount = (decimal)UtilityClass.getCost(rData.CustCode, weight, rData.Destination, rData.Type.Trim(), rData.DOX ?? 'N');
                    db.SubmitChanges();
                    rData.FrAmount = dData.FrAmount;
                    sData.BilledWeight = dData.BilledWeight;
                    sData.FrAmount = dData.FrAmount;
                }
                if(e.Column.Header.ToString() == "Billed Amount")
                {
                    dData.FrAmount = rData.FrAmount;
                    db.SubmitChanges();
                    sData.FrAmount = rData.FrAmount;
                }

            }
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.IsReadOnly = false;
        }
        private void MISReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintWindow win = new PrintWindow(null, DateTime.Today, DateTime.Today, false);
            try
            {
                win.Show();
            }
            catch (Exception ex) { }
        }
        private void UpdateMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Please save your work before Continuing", "Update", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                About up = new About();
                try
                {
                    up.Show();
                }catch(Exception ex){ }
            }
        }
        private void DeleteConnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteConnsignment win = new DeleteConnsignment();
            try
            {


                win.ShowDialog();
            }
            catch (Exception ex) { }
        }

        private void CostingRuleGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                double rate;
                if (double.TryParse((e.EditingElement as TextBox).Text, out rate))
                {
                    try
                    {
                        BillingDataDataContext db = new BillingDataDataContext();
                        CostingRule origRule = e.Row.Item as CostingRule;
                        Rule newRule = db.Rules.SingleOrDefault(x => x.ID == origRule.Id);
                        if (newRule == null)
                        {
                            MessageBox.Show("Unable to find this rule in database....", "Error");
                            e.Cancel = true;
                            return;
                        }
                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        CostingRule crr = jss.Deserialize<CostingRule>(newRule.Properties);
                        if (e.Column.Header.ToString() == "Non-Dox (Rs)")
                        {
                            crr.ndoxAmount = rate;
                        }
                        if (e.Column.Header.ToString() == "Dox (Rs)")
                        {
                            crr.doxAmount = rate;
                        }
                        newRule.Properties = jss.Serialize(crr);
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to save data....", "Error");
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Input.. ", "Error");
                    e.Cancel = true;
                }
            }
        }

        private void ExpenseEntryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExpenseEntry win = new ExpenseEntry();
            try
            {
                win.Show();
            }
            catch (Exception ex) { }
        }

        private void ClientReport_Click_1(object sender, RoutedEventArgs e)
        {
            ClientExpenseReportWindow win = new ClientExpenseReportWindow();
            try {
            win.Show();
            }
            catch (Exception ex) { }
        }

        private void ExpenseReportWin_Click(object sender, RoutedEventArgs e)
        {
            ExpenseReportWindow win = new ExpenseReportWindow();
            try
            {
                win.Show();
            }
            catch (Exception ex) { }
            
        }
    }
}