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
        CollectionViewSource CostingRulesSource;
        CollectionViewSource serviceRulesView;
        List<CostingRule> costingRuleList;
        List<ServiceRule> serviceRuleList;
        List<Rule> ruleList;
        Quotation qutObj;
        public ImportRules()
        {
            InitializeComponent();
            this.ClientComboBox.SelectionChanged -= ComboBox_SelectionChanged;
        }
        public ImportRules(Client client)
            : this()
        {
            if (client != null)
            {
                this.client = client;
                clientViewSource = (CollectionViewSource)FindResource("ClinetViewList");
                clientList = DataSources.ClientCopy.ToList();
                clientViewSource.Source = clientList;
                this.ClientBox.Text = client.NameAndCode;
                this.ClientComboBox.Text = this.client.NameAndCode;
                this.ClientComboBox.SelectionChanged += ComboBox_SelectionChanged;
            }
            LoadClientRules(this.client);
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
            Client client = (Client)ClientComboBox.SelectedItem;
            if (client != null)
            {
                this.client = client;
                LoadClientRules(client);
            }
        }
        private void LoadClientRules(Client client)
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
            qutObj = db.Quotations.SingleOrDefault(x => x.CLCODE == client.CLCODE);
            if (qutObj == null)
            {
                unLoadQuotation();
            }
            else
            {
                loadQuotation(qutObj);
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
    }
}
