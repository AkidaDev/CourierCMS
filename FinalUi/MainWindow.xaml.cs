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
        #region initScripts
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
        BackgroundWorker DeleteSheetWorker;
        BillingDataDataContext db;
        #endregion
        public MainWindow()
        {
            SecurityModule.authenticate("dharmendra", "pass");
            #region setupCode
            PreviewMouseMove += OnPreviewMouseMove;
            #endregion
            #region WindowDimensionsCode
            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height;
            this.Left = 0;
            this.Top = 0;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.WindowState = WindowState.Normal;
            #endregion
            db = new BillingDataDataContext();
            ResourceDictionary dict = this.Resources;
            InitializeComponent();
            CollectionViewSource clientCodeList = (CollectionViewSource)FindResource("ClientCodeList");
            clientCodeList.Source = db.Clients.Select(c => c.CLCODE);

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
            #endregion

            #region loading initial pages
            List<int> sheets = db.RuntimeMetas.Where(y => y.UserName == SecurityModule.currentUserName).Select(x => x.SheetNo).Distinct().ToList();
            foreach (int sheet in sheets)
            {
                List<RuntimeData> runtimeData = (db.RuntimeMetas.Where(x => x.SheetNo == sheet && x.UserName == SecurityModule.currentUserName).Select(y => y.RuntimeData)).OrderBy(x => x.BookingDate).ThenBy(z=>z.ConsignmentNo).ToList(); ;
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
            DeleteSheetWorker = new BackgroundWorker();
            DeleteSheetWorker.DoWork += DeleteWorker_DoWork;
            DeleteSheetWorker.RunWorkerCompleted += DeleteWorker_RunWorkerCompleted;
        }
        #region backGround Worker Functions
        #region LoadWorker
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
                help.insertRuntimeData((List<RuntimeData>)e.Argument, dataGridHelper.currentSheetNumber,isLoadedFromFile,toDate_loadDataWin,fromDate_loadDataWin);
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
        #region Delete Sheet Worker
        void DeleteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBlock.Text = MessageBlock.Text + "\n" + "Delete Operation Completed. " + e.Error;
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
            SanitizingWindow window = new SanitizingWindow(dataGridHelper.getCurrentDataStack, db, dataGridHelper.currentSheetNumber, dataGrid);
            window.ShowDialog();
        }
        #endregion
        #region PowerEntryCommand
        RoutedCommand PowerEntryCommand = new RoutedCommand();
        private void PowerEntryCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            PowerEntry powerWin = new PowerEntry(dataGridHelper.getCurrentDataStack, db.Clients.Select(c => c.CLCODE.ToString()).ToList(), db);
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
        #region printCommand

        private void ExecutePrint(object sender, ExecutedRoutedEventArgs e)
        {

            PrintWindow win = new PrintWindow(dataGridHelper.getCurrentDataStack);
            win.ShowDialog();
        }
        private void CanExecutePrintCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataGridHelper != null)
            {
                if (dataGridHelper.getCurrentDataStack != null)
                {
                    e.CanExecute = true;
                }
                else
                    e.CanExecute = false;
            }
            else
                e.CanExecute = false;
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
        #region deleteCommand
        public RoutedCommand DeleteCommand = new RoutedCommand();
        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DeleteSheetWorker != null && dataGridHelper != null)
            {
                if (DeleteSheetWorker.IsBusy == true || buttonList.Count == 0)
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
                DeleteSheetWorker.RunWorkerAsync(dataGridHelper.currentSheetNumber);
                dataGridHelper.removeSheet(dataGridHelper.currentSheetNumber);
                buttontabcanvaswrap.Children.Remove(activeButton);
                if (buttonList.Count > 0)
                    b = buttonList.Single(x => x.Value == buttonList.Values.Min()).Key;
                else
                    b = null;
                changeSheetButton(activeButton,b);
                buttonList.Remove(activeButton);
                activeButton = b;
                
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
            canvasButton.Style = (Style)FindResource("Sheet_button");
            canvasButton.Width = 90;
            canvasButton.Height = 20;
            canvasButton.Margin = new Thickness(-9, 1, 0, 0);
            canvasButton.Height = 20;
            canvasButton.Width = 90;

            Canvas canvastab = new Canvas();
            canvastab.Width = 90;
            canvastab.Height = 20;

            Path pathsquare = new Path();
            pathsquare.Data = Geometry.Parse(@"F1M2,1.644C2,1.644 2,20 2,20 2,20 77.831,20 77.831,20 77.831,20 91.619,1.644 91.619,1.644 91.619,1.644 2,1.644 2,1.644z");
            pathsquare.Fill = Brushes.RoyalBlue;
            pathsquare.Height = 20;
            pathsquare.Width = 88.5;
            pathsquare.Stretch = Stretch.Fill;
            Button buttonsquare = new Button();
            buttonsquare.Name = "_pathsquare";
            buttonsquare.Style = (Style)FindResource("Sheet_button");
            buttonsquare.Content = pathsquare;


            Path pathkatta = new Path();
            pathkatta.Fill = Brushes.White;
            pathkatta.Stretch = Stretch.Fill;
            pathkatta.Height = 9;
            pathkatta.Width = 9;
            pathkatta.Data = Geometry.Parse(@"F1M14.987,13.789C14.987,13.789 13.622,15.154 13.622,15.154 13.622,15.154 8.16,9.692 8.16,9.692 8.16,
                      9.692 2.699,15.154 2.699,15.154 2.699,15.154 1.333,13.789 1.333,13.789 1.333,13.789 6.795,8.327 6.795,8.327 6.795,8.327 1.333,2.865 1.333,2.865 1.333,
                     2.865 2.699,1.5 2.699,1.5 2.699,1.5 8.16,6.962 8.16,6.962 8.16,6.962 13.622,1.5 13.622,1.5 13.622,1.5 14.987,2.865 14.987,2.865 14.987,2.865 9.526,
                    8.327 9.526,8.327 9.526,8.327 14.987,13.789 14.987,13.789z");

            TextBlock text = new TextBlock();
            text.Name = "Sheettext";
            text.Text = "Sheet- " + key.ToString();
            text.Foreground = Brushes.White;
            text.FontSize = 16;
            Canvas.SetLeft(text, 4);
            Canvas.SetTop(text, -2);

            Button buttonkatta = new Button();
            buttonkatta.Style = (Style)FindResource("smallbutton");
            buttonkatta.Content = pathkatta;
            buttonkatta.Name = "CloseCurrentClick";
            buttonkatta.Command = DeleteCommand;

            Canvas.SetLeft(buttonkatta, 67);
            Canvas.SetTop(buttonkatta, 5);

            canvastab.Children.Add(buttonsquare);
            canvastab.Children.Add(buttonkatta);
            canvastab.Children.Add(text);
            canvasButton.Content = canvastab;
            canvasButton.Click += SheetSelectButton_Click;
            buttontabcanvaswrap.Children.Add(canvasButton);
            buttonList.Add(canvasButton, key);
            if(activeButton != null)
            changeSheetButton(activeButton, canvasButton);
            activeButton = canvasButton;

        }
        bool isLoadedFromFile = true;
        DateTime? toDate_loadDataWin;
        DateTime? fromDate_loadDataWin;
        void loadData_Closed(object sender, EventArgs e)
        {
            this.Effect = null;
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
                isLoadedFromFile = dataWind.isLoadedFromFile;
                if(!isLoadedFromFile)
                {
                    toDate_loadDataWin = dataWind.toDate;
                    fromDate_loadDataWin = dataWind.fromDate;
                }
                LoadWorker.RunWorkerAsync(dataWind.data);
            }
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
                            b.Fill = Brushes.RoyalBlue;
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
            FilterSelectWindow window = new FilterSelectWindow(dataGridHelper.currentConnNos);
            window.Closed += window_Closed;
            window.ShowDialog();
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

        #region Handling Resizing
        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        Path path = new Path();
                        path.Data = Geometry.Parse(@"F1M2.111,7.667C2.111,7.667 2.111,14.958 2.111,14.958 2.111,14.958 9.889,14.958 9.889,14.958 9.889,14.958 9.889,7.667 9.889,
				7.667 9.889,7.667 2.111,7.667 2.111,7.667z M6.222,2.25C6.222,2.25,6.222,4.438,6.222,6.625L8.674,6.625C9.403,6.625 9.889,6.625 9.889,6.625 10.5,6.625 11,
				7.094 11,7.667 11,7.667 11,8.123 11,8.806L11,11 12.071,11C13.575,11 14.778,11 14.778,11 14.778,11 14.778,2.25 14.778,2.25 14.778,2.25 6.222,2.25 6.222,
				2.25z M6.222,1C6.222,1 14.778,1 14.778,1 15.45,1 16,1.562 16,2.25 16,2.25 16,11 16,11 16,11.687 15.45,12.25 14.778,12.25 14.778,12.25 13.575,12.25 12.071,
				12.25L11,12.25 11,13.819C11,14.502 11,14.958 11,14.958 11,15.531 10.5,16 9.889,16 9.889,16 2.111,16 2.111,16 1.5,16 1,15.531 1,14.958 1,14.958 1,7.667 1,
				7.667 1,7.094 1.5,6.625 2.111,6.625 2.111,6.625 2.597,6.625 3.326,6.625L5,6.625C5,4.438 5,2.25 5,2.25 5,1.562 5.55,1 6.222,1z");
                        path.Fill = Brushes.Black;
                        StackPanel panel = new StackPanel();
                        panel.Children.Add(path);
                        this.NormalMaximize.Content = panel;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        this.WindowState = WindowState.Normal;
                        Path path = new Path();
                        path.Data = Geometry.Parse(@"F1M3.222,5L3.222,6.702C3.222,9.071 3.222,11.778 3.222,11.778 3.222,11.778 11.778,11.778 11.778,11.778 11.778,11.778 11.778,9.071 11.778,
						6.702L11.778,5 11.281,5C9.219,5,5.781,5,3.719,5z M3.222,2C3.222,2 11.778,2 11.778,2 12.114,2 12.42,2.138 12.641,2.359L12.908,3 13,3C13,3,13,3.25,13,3.222L13,
						3.5 13,3.59 13,3.844C13,3.938 13,4 13,4 13,4 13,4.25 13,4.5L13,4.559 13,5 13,6.702C13,9.071 13,11.778 13,11.778 13,12.45 12.45,13 11.778,13 11.778,13 3.222,
						13 3.222,13 2.55,13 2,12.45 2,11.778 2,11.778 2,8.436 2,5.929L2,5 2,4.559C2,5,2,4.75,2,4.5L2,4C2,3.757 2,3.222 2,3.222 2,2.55 2.55,2 3.222,2z");
                        path.Fill = Brushes.Black;
                        StackPanel panel = new StackPanel();
                        panel.Children.Add(path);
                        this.NormalMaximize.Content = panel;
                        break;
                    }
            }
        }


        private void NormalMaximize_Click(object sender, RoutedEventArgs e)
        {
            SwitchWindowState();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            //Application.Current.Shutdown();
            this.Close();
        }
        #endregion

        #region custom window resize
        private void Rec_MouseMove(object sender, MouseEventArgs e)
        {
            var rec = (Rectangle)sender;
            switch (rec.Name)
            {
                case "Rec_Top":
                    {
                        Cursor = Cursors.SizeNS;
                        break;
                    } 
				case "Rec2":
                    {
                        Cursor = Cursors.SizeWE;
                        break;
                    }
                case "Rec_Top_Left":
                    {
                        Cursor = Cursors.SizeNWSE;
                        break;
                    }
                case "Rec_Top_Right":
                    {
                        Cursor = Cursors.SizeNESW;
                        break;
                    }
                case "Rec_Bottom":
                    {
                        Cursor = Cursors.SizeNS;
                        break;
                    }
                case "Rec_Bottom_Left":
                    {
                        Cursor = Cursors.SizeNESW;
                        break;
                    }
                case "Rec_Bottom_Right":
                    {
                        Cursor = Cursors.SizeNWSE;
                        break;
                    }
                case "Rec_Left":
                    {
                        Cursor = Cursors.SizeWE;
                        break;
                    }
                case "Rec_Right":
                    {
                        Cursor = Cursors.SizeWE;
                        break;
                    }
            }
        }
        public void resizeWindow()
        {
        }

        #endregion
        protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "Rec_Top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "Rec_Bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "Rec_Left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "Rec_Right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "Rec_Top_Left":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "Rec_Top_Right":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "Rec_Bottom_Left":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "Rec_Bottom_Right":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
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

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #region menuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ManageClient_Click(object sender, RoutedEventArgs e)
        {
            ManageClient window = new ManageClient(); window.ShowDialog();
        }

        private void ManageEmployee_Click(object sender, RoutedEventArgs e)
        {
            ManageEmployee window = new ManageEmployee(); window.ShowDialog();
        }

        private void RateWindowMenu_Click(object sender, RoutedEventArgs e)
        {
            RateWindow window = new RateWindow(); window.ShowDialog();
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
    }
}
