using System;
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
        BackgroundWorker DeleteSheetWorker;
        BillingDataDataContext db;
        #endregion
        public MainWindow()
        {
            SecurityModule.authenticate("dharmendra", "pass");
            #region setupCode
            this.SourceInitialized += Window_SourceInitialized;
            #endregion
            #region WindowDimensionsCode
            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height;
            this.Left = 0;
            this.Top = 0;
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
            CloseCurrentClick.Command = DeleteCommand;
            #endregion


            #region loading initial pages
            List<int> sheets = db.RuntimeMetas.Where(y => y.UserName == SecurityModule.currentUserName).Select(x => x.SheetNo).Distinct().ToList();
            foreach (int sheet in sheets)
            {
                List<RuntimeData> runtimeData = db.RuntimeMetas.Where(x => x.SheetNo == sheet && x.UserName == SecurityModule.currentUserName).Select(y => y.RuntimeData).OrderBy(x => x.ConsignmentNo).ToList(); ;
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
                help.insertRuntimeData((List<RuntimeData>)e.Argument, dataGridHelper.currentSheetNumber);
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
            MessageBlock.Text = MessageBlock.Text + "\n" + "Delete Opoeration Completed. " + e.Error;
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
            if(dataGridHelper.currentDataSheet == null)
            {
                int key = dataGridHelper.addNewSheet(new List<RuntimeData>(), "");
                addingNewPage(key);
            }
            SanitizingWindow window = new SanitizingWindow(dataGridHelper.getCurrentDataStack, db,dataGridHelper.currentSheetNumber);
            window.Show();
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
            win.Show();
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
                DeleteSheetWorker.RunWorkerAsync(dataGridHelper.currentSheetNumber);
                dataGridHelper.removeSheet(dataGridHelper.currentSheetNumber);
                DataGridSheetPanel.Children.Remove(activeButton);
                buttonList.Remove(activeButton);
                if (buttonList.Count > 0)
                    activeButton = buttonList.Single(x => x.Value == buttonList.Values.Min()).Key;
                else
                    activeButton = null;

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
            Button button = new Button();
            button.Style = (Style)FindResource("Sheet_button");
            button.Background = Brushes.Transparent;
            StackPanel panel = new StackPanel();

            WrapPanel panel1 = new WrapPanel();
        //    panel.Children.Add(panel1);
            Path path = new Path();
            path.Data = Geometry.Parse(@"F1M2,1.644C2,1.644 2,20 2,20 2,20 77.831,20 77.831,20 77.831,20 91.619,1.644 91.619,1.644 91.619,1.644 2,1.644 2,1.644z");
            path.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
            path.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("RoyalBlue"));
            panel.Children.Add(path);
            TextBlock text = new TextBlock();
            text.Text = "Sheet- " + key.ToString();
            text.Margin = new Thickness(0, -22, 20, 0);
            text.FontFamily = new FontFamily("Segoe UI");
            text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("RoyalBlue"));
            text.FontSize = 16;
            text.Background = Brushes.Transparent;
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

           
           // panel.Children.Add(path1);

            panel.Children.Add(text);
            button.Content = panel;
            button.Click += SheetSelectButton_Click;
            Path path1 = new Path();
           // image of the katta 
            path1.Data = Geometry.Parse(@"F1M14.987,13.789C14.987,13.789 13.622,15.154 13.622,15.154 13.622,15.154 8.16,9.692 8.16,9.692 8.16,
				9.692 2.699,15.154 2.699,15.154 2.699,15.154 1.333,13.789 1.333,13.789 1.333,13.789 6.795,8.327 6.795,8.327 6.795,8.327 1.333,2.865 1.333,2.865 1.333,
				2.865 2.699,1.5 2.699,1.5 2.699,1.5 8.16,6.962 8.16,6.962 8.16,6.962 13.622,1.5 13.622,1.5 13.622,1.5 14.987,2.865 14.987,2.865 14.987,2.865 9.526,
				8.327 9.526,8.327 9.526,8.327 14.987,13.789 14.987,13.789z");
            path1.Fill = Brushes.Black;
            DataGridSheetPanel.Children.Add(button);
            buttonList.Add(button, key);
            activeButton = button;

        }
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
        #region Handling Resizing
        private bool mRestoreIfMove = false;


        void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }


        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }


        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }


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


        
        private void rctHeader_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreIfMove = false;
        }


        private void rctHeader_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mRestoreIfMove)
            {
                mRestoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                WindowState = WindowState.Normal;

                POINT lMousePosition;
                GetCursorPos(out lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                DragMove();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }


        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
        private void NormalMaximize_Click(object sender, RoutedEventArgs e)
        {
            SwitchWindowState();
        }
        #endregion
    }
}
