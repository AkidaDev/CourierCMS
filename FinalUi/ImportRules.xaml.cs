using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
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
        Client clientO;
        List<Client> clientList;
        CollectionViewSource clientViewSource;
        CollectionViewSource CostingRulesSource;
        CollectionViewSource serviceRulesView;
        List<Rule> ruleList;
        Quotation qutObj;
        ImportRules()
        {
            InitializeComponent();
            this.ClientComboBox.SelectionChanged -= ComboBox_SelectionChanged;
        }
        public ImportRules(Client clientO)
            : this()
        {
            this.clientO = clientO;
            clientViewSource = (CollectionViewSource)FindResource("ClinetViewList");
            clientList = DataSources.ClientCopy.ToList();
            clientViewSource.Source = clientList;
            client = clientList.Single(x => x.CLCODE == "<NONE>");
            this.ClientBox.Text = clientO.NameAndCode;
            this.ClientComboBox.Text = this.client.NameAndCode;
            this.ClientComboBox.SelectionChanged += ComboBox_SelectionChanged;
        LoadClientRules(this.client);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CostingRuleGrid.SelectedItems.Count + ServiceRuleGrid.SelectedItems.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("You have selected " + CostingRuleGrid.SelectedItems.Count + " Costing Rules and " + ServiceRuleGrid.SelectedItems.Count + " Service Rules. Selection will be lost if you change client. Continue? ", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    return;
            }
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
            CostingRuleGrid.SelectedItems.Clear();
            ServiceRuleGrid.SelectedItems.Clear();
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

        private void ServiceRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (ServiceRuleGrid != null && CostingRuleGrid != null)
            {
                if (ServiceRadio.IsChecked == true)
                {
                    ServiceRuleGrid.Visibility = Visibility.Visible;
                    CostingRuleGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ServiceRuleGrid.Visibility = Visibility.Collapsed;
                    CostingRuleGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private void ImportRulesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("You have selected " + CostingRuleGrid.SelectedItems.Count + " Costing Rules and " + ServiceRuleGrid.SelectedItems.Count + " Service Rules. Are you sure you want to import them? ", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                List<CostingRule> costingRules = CostingRuleGrid.SelectedItems.Cast<CostingRule>().ToList();
                List<ServiceRule> serviceRules = ServiceRuleGrid.SelectedItems.Cast<ServiceRule>().ToList();
                BillingDataDataContext db = new BillingDataDataContext();
                Quotation quotation = db.Quotations.Single(x => x.CLCODE == clientO.CLCODE);
                foreach (CostingRule CRule in costingRules)
                {
                    int id;
                    id = Convert.ToInt32(db.ExecuteQuery<decimal>("SELECT IDENT_CURRENT('Rule') +1;").FirstOrDefault());
                    CRule.Id = id;
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string serialized = js.Serialize(CRule);
                    Rule r = new Rule();
                    r.Type = 1;
                    r.Properties = serialized;
                    r.QID = quotation.Id;
                    r.Remark = "Imported rule from " + client.CLNAME;
                    db.Rules.InsertOnSubmit(r);
                    db.SubmitChanges();
                }
                foreach (ServiceRule SRule in serviceRules)
                {
                    int id;
                    id = Convert.ToInt32(db.ExecuteQuery<decimal>("SELECT IDENT_CURRENT('Rule') +1;").FirstOrDefault());
                    SRule.Id = id;

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string serialized = js.Serialize(SRule);
                    Rule r = new Rule();
                    r.Type = 2;
                    r.Properties = serialized;
                    r.QID = quotation.Id;
                    r.Remark = "Imported rule from " + client.CLNAME;
                    db.Rules.InsertOnSubmit(r);
                    db.SubmitChanges();
                }
                MessageBox.Show("Rules imported.");
                this.Close();
            }
        }
    }
}
