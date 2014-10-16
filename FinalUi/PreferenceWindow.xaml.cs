using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
namespace FinalUi
{
    /// <summary>
    /// Interaction logic for PreferenceWindow.xaml
    /// </summary>
    public partial class PreferenceWindow : Window
    {
        BackgroundWorker sqlInstanceGetVersion;
        DataTable table;
        public PreferenceWindow()
        {
            InitializeComponent();
            ThemeColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Configs.Default.Background);
            ConnectionStringCombo.Items.Add("Still loading please wait...");
            sqlInstanceGetVersion = new BackgroundWorker();
            sqlInstanceGetVersion.DoWork += sqlInstanceGetVersion_DoWork;
            sqlInstanceGetVersion.RunWorkerCompleted += sqlInstanceGetVersion_RunWorkerCompleted;
            sqlInstanceGetVersion.RunWorkerAsync();
        }

        void sqlInstanceGetVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ConnectionStringCombo.Items.Clear();
            foreach(DataRow row in table.Rows)
            {
                ConnectionStringCombo.Items.Add(row[0]);
            }
        }

        void sqlInstanceGetVersion_DoWork(object sender, DoWorkEventArgs e)
        {
           table = SqlDataSourceEnumerator.Instance.GetDataSources();
 
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnectionStringBuilder bd = new SqlConnectionStringBuilder();
            bd.DataSource = ConnectionStringCombo.Text;
            bd.UserID = "sa";
            bd.Password = "Alver!22";
            Configs.Default.BillingDatabaseConnectionString = bd.ConnectionString;
            Configs.Default.Background = ThemeColorPicker.SelectedColorText;
            Configs.Default.Save();
            this.Close();
        }
        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
     /*       Configs.Default.Reset();
            FillDetails();
      * */
        }
    }
}