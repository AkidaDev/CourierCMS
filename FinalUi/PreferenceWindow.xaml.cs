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
            FillDetails();
            
            ConnectionStringCombo.Items.Add("Still loading please wait...");
            sqlInstanceGetVersion = new BackgroundWorker();
            sqlInstanceGetVersion.DoWork += sqlInstanceGetVersion_DoWork;
            sqlInstanceGetVersion.RunWorkerCompleted += sqlInstanceGetVersion_RunWorkerCompleted;
            sqlInstanceGetVersion.RunWorkerAsync();
        }

        void sqlInstanceGetVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ConnectionStringCombo.Items.Clear();
            foreach (DataRow row in table.Rows)
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
            Configs.Default.Background = ThemeColorPicker.SelectedColorText;
            Configs.Default.Tin = Pannumber.Text;
            Configs.Default.TNC = TNCBox.Text;
            Configs.Default.CompanyAddress = CompanyAddressBox.Text;
            Configs.Default.CompanyEmail = CompanyEmailBox.Text;
            Configs.Default.CompanyName = CompanyNameBox.Text;
            Configs.Default.CompanyOwner = CompanyOwnerBox.Text;
            Configs.Default.CompanyPhone = CompanyContactBox.Text;
            Configs.Default.Save();
            MessageBox.Show("Settings Saved");
        }
        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you realy want to reset setting to default", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if(MessageBox.Show(" Application will restart for Rest to Factory settings \n Unsaved Changes will be lost","",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Configs.Default.Reset();
                    System.Windows.Forms.Application.Restart();
                    Application.Current.Shutdown();
                }
            }
            else { return; }
        }
        private void FillDetails()
        {
            ThemeColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(Configs.Default.Background);
            CompanyAddressBox.Text = Configs.Default.CompanyAddress;
            CompanyContactBox.Text = Configs.Default.CompanyPhone;
            CompanyEmailBox.Text = Configs.Default.CompanyEmail;
            CompanyNameBox.Text = Configs.Default.CompanyName;
            CompanyOwnerBox.Text = Configs.Default.CompanyOwner;
            TNCBox.Text = Configs.Default.TNC;
            Pannumber.Text = Configs.Default.Tin;
            this.ServiceTax.Text = Configs.Default.ServiceTax;
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void cloakAll()
        {
            this.BackgroundSettingGrid.Visibility = Visibility.Collapsed;
            this.CompanyDetails.Visibility = Visibility.Collapsed;
            this.InvoicePanel.Visibility = Visibility.Collapsed;
        }
        private void settingtreeview_Click(object sender, RoutedEventArgs e)
        {
            cloakAll();
            this.BackgroundSettingGrid.Visibility = Visibility.Visible;
        }
        private void DetailsEntry_Click(object sender, RoutedEventArgs e)
        {
            cloakAll();
            this.CompanyDetails.Visibility = Visibility.Visible;
        }
        private void InvoiceEntry_Click(object sender, RoutedEventArgs e)
        {
            cloakAll();
            this.InvoicePanel.Visibility = Visibility.Visible;
        }
    }
}