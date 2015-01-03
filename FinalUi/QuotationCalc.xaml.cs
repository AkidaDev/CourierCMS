using System;
using System.Collections.Generic;
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
    /// Interaction logic for QuotationCalc.xaml
    /// </summary>
    public partial class QuotationCalc : Window
    {
        CollectionViewSource ServiceTable;
        CollectionViewSource clientViewSource;
        CollectionViewSource cityViewSource;
        public QuotationCalc()
        {
            InitializeComponent();
            clientViewSource = (CollectionViewSource)FindResource("ClienTable");
            clientViewSource.Source = DataSources.ClientCopy;
            cityViewSource = (CollectionViewSource)FindResource("CityTable");
            cityViewSource.Source = DataSources.CityCopy;
            ServiceTable = (CollectionViewSource)FindResource("ServiceTable");
            ServiceTable.Source = DataSources.ServicesCopy;
        }

        private void WeightRuleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetRateButton_Click(null, null);
            }
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
    }
}
