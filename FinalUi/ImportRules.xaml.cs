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
    /// Interaction logic for ImportRules.xaml
    /// </summary>
    public partial class ImportRules : Window
    {
        Client client;
        List<Client> clientList;
        CollectionViewSource clientViewSource;
        List<CostingRule> costingRuleList;
        List<ServiceRule> serviceRuleList;
        List<Rule> ruleList;
        public ImportRules()
        {
            InitializeComponent();
        }
        public ImportRules(Client client)
            : this()
        {
            this.client = client;
            clientViewSource = (CollectionViewSource)FindResource("ClinetViewList");
            clientList = DataSources.ClientCopy.ToList();
            clientViewSource.Source = clientList;
            this.ClientBox.Text = client.NameAndCode;
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private List<Rule> getRuleList(Client client)
        {
            return DataSources
        }
        private void reload()
        { 
            
        }
    }
}
