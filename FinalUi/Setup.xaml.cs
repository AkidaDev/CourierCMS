using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Sql;
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
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        int currentCanvas = 1;
        Canvas currentCanvasObj;
        System.Data.DataTable table;
        System.Data.SqlClient.SqlConnectionStringBuilder constring;
        Employee emp;
        public Setup()
        {
            InitializeComponent();
            table = SqlDataSourceEnumerator.Instance.GetDataSources();
            constring = new System.Data.SqlClient.SqlConnectionStringBuilder();
            CollectionViewSource instanceSource;
            instanceSource = (CollectionViewSource)FindResource("InstanceList");
            instanceSource.Source = table;
            emp = new Employee();
        }
        public void SetDefaultSetting()
        { 
        }
        private void Browse_Directory(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                DirectoryPath.Text = (path);
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            Configs.Default.IsFirst = false;
            Configs.Default.Save();
            Login win = new Login();
            win.Show();
            this.Close();
        }
        private void update_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            if (this.checkbox_unselected.Visibility == Visibility.Hidden)
            {
                this.checkbox_selected.Visibility = Visibility.Hidden;
                this.checkbox_unselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.checkbox_selected.Visibility = Visibility.Visible;
                this.checkbox_unselected.Visibility = Visibility.Hidden;
            }
        }
        private void Shorcut_selection(object sender, RoutedEventArgs e)
        {
            if (this.unselected.Visibility == Visibility.Hidden)
            {
                this.selected.Visibility = Visibility.Hidden;
                this.unselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.selected.Visibility = Visibility.Visible;
                this.unselected.Visibility = Visibility.Hidden;
            }
        }
        private void Open_Vortex(object sender, RoutedEventArgs e)
        {
            if (this.checkunselected.Visibility == Visibility.Hidden)
            {
                this.checkselected.Visibility = Visibility.Hidden;
                this.checkunselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.checkselected.Visibility = Visibility.Visible;
                this.checkunselected.Visibility = Visibility.Hidden;
            }
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentCanvas == 2)
            { //Data Source=SYSTEM;Initial Catalog=BillingDatabase;User ID=sa;Password=Alver!22
                constring.DataSource = this.ServerNameBox.Text;
                constring.InitialCatalog = this.databaseBox.Text;
                constring.UserID = this.UserNameBox.Text;
                constring.Password = this.PasswordBox.Password;
                string provider = "System.Data.SqlClient"; // for example
                DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
                using (DbConnection conn = factory.CreateConnection())
                {
                    conn.ConnectionString = constring.ConnectionString;
                    try
                    {
                        conn.Open();
                        isconnected = true;
                        Configs.Default.BillingDatabaseConnectionString = constring.ConnectionString;
                        Configs.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to connect to server"); isconnected = false; return;
                    }
                }
                MessageBox.Show("Connected successfully");
            }
            if (currentCanvas == 3)
            {
                if (this.UserPasswordBox.Password == this.UserCPasswordBox.Password)
                {
                    BillingDataDataContext db = new BillingDataDataContext(constring.ConnectionString);

                    this.emp.UserName = this.EUserNameBox.Text;
                    this.emp.Password = this.UserPasswordBox.Password;
                    this.emp.EMPCode = "Super";
                    this.emp.Id = Guid.NewGuid();
                    this.emp.Gender = 'M';
                    this.emp.Status = 'A';
                    this.emp.Name = "<none>";
                    Configs.Default.CompanyName = CompanyNameBox.Text;
                    Configs.Default.Save();
                    Employee emp = db.Employees.Single(x => x.UserName == this.emp.UserName);
                    if (emp != null)
                    {
                        if (!(MessageBox.Show("This employee already exists. Do you want to make this employee super on this system?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                        {
                            MessageBox.Show("Please enter different employee name","Error");
                            return;
                        }
                    }
                    else
                    {
                        db.Employees.InsertOnSubmit(emp);
                    }
                    try {
                        Configs.Default.SuperUser = this.emp.UserName;
                        Configs.Default.Save();
                        db.SubmitChanges();
                    }
                    catch (Exception ex){ MessageBox.Show(ex.Message); return;}
                }
                else { MessageBox.Show("Password do not match","Error"); return; }
            }
            switch (currentCanvas)
            {
                case 1:
                    currentCanvas = 2;
                    Step1.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 2:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 3:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    currentCanvas = 5;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step5;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
            }
            if (currentCanvas == 5)
            {
                Next.Visibility = Visibility.Collapsed;
                Finish.Visibility = Visibility.Visible;
            }
            else
            {
                Next.Visibility = Visibility.Visible;
                Finish.Visibility = Visibility.Collapsed;
            }
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            switch (currentCanvas)
            {
                case 5:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 3:
                    currentCanvas = 2;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 2:
                    currentCanvas = 1;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step1;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        public void saveDbConnection()
        {
            Configs.Default.BillingDatabaseConnectionString = constring.ConnectionString;
            Configs.Default.Save();
        }

        public bool isconnected { get; set; }
    }
}
