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
namespace FinalUi
{
    /// <summary>
    /// Interaction logic for PreferenceWindow.xaml
    /// </summary>
    public partial class PreferenceWindow : Window
    {
        enum themes
        {
            Blue,
            Gray,
        };
        enum dataFormat
        {
        };
        public PreferenceWindow()
        {
            InitializeComponent();
            FillDetails();
        }
        public void FillDetails()
        {
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Configs.Default.BillingDatabaseConnectionString = this.ConnectionStringBox.Text;
            Configs.Default.Save();
            this.Close();
        }
        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Configs.Default.Reset();
            FillDetails();
        }
    }
}