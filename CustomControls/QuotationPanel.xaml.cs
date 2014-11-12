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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for QuotationPanel.xaml
    /// </summary>
    public partial class QuotationPanel : UserControl
    {
        public QuotationPanel()
        {
            InitializeComponent();
        }

        private void ImportRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddCostingRuleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddServiceRuleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddInvoiceRuleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetRateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WeightRuleTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
        }
        private void ClientCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               // e.AddedItems.Cast<Client>().Count();
            }
            catch (Exception ex)
            {
                return;
            }

            LoadClientRules();
        }
        private void LoadClientRules()
        {
            //if (isInitialized == true)
            //{
            //    if (CostingRulesSource == null)
            //    {
            //        CostingRulesSource = (CollectionViewSource)FindResource("CostingRuleList");
            //    }
            //    if (serviceRulesView == null)
            //    {
            //        serviceRulesView = (CollectionViewSource)FindResource("ServiceRuleList");
            //    }
            //    BillingDataDataContext db = new BillingDataDataContext();
            //    qutObj = db.Quotations.SingleOrDefault(x => x.CLCODE == ((Client)ClientCombo.SelectedItem).CLCODE);
            //    if (qutObj == null)
            //    {
            //        unLoadQuotation();
            //    }
            //    else
            //    {
            //        loadQuotation(qutObj);
            //    }
            //}
        }
        void unLoadQuotation()
        {
        //    CostingRulesSource.Source = null;
        //    serviceRulesView.Source = null;
        //    CostingRuleGrid.Items.Refresh();
        //    ServiceRuleGrid.Items.Refresh();
        }
        void loadQuotation()
        {
            //CostingRulesSource.Source = qutObj.CostingRules;
            //serviceRulesView.Source = qutObj.ServiceRules;
        }
        private void cloakAllGrid()
        {
            //if (CostingRuleGrid != null && InvoiceRuleGrid != null && ServiceRuleGrid != null)
            //{
            //    CostingRuleGrid.Visibility = Visibility.Collapsed;
            //    InvoiceRuleGrid.Visibility = Visibility.Collapsed;
            //    ServiceRuleGrid.Visibility = Visibility.Collapsed;
            //}
        }
        private void CostingRuleRadio_Checked_1(object sender, RoutedEventArgs e)
        {
            //if (isInitialized == true)
            //{
            //    try
            //    {
            //        cloakAllGrid();
            //        CostingRuleGrid.Visibility = Visibility.Visible;

            //        currentaddrulebutton.Visibility = Visibility.Collapsed;
            //        AddCostingRuleButton.Visibility = Visibility.Visible;
            //        currentaddrulebutton = AddCostingRuleButton;
            //    }
            //    catch (NullReferenceException ex)
            //    { }
            //}
        }

        private void ServiceRuleRadio_Checked(object sender, RoutedEventArgs e)
        {
            //cloakAllGrid();
            //ServiceRuleGrid.Visibility = Visibility.Visible;
            //currentaddrulebutton.Visibility = Visibility.Collapsed;
            //AddServiceRuleButton.Visibility = Visibility.Visible;
            //currentaddrulebutton = AddServiceRuleButton;
        }

        private void InvoiceRuleRadio_Checked(object sender, RoutedEventArgs e)
        {
            //cloakAllGrid();
            //InvoiceRuleGrid.Visibility = Visibility.Visible;
            //currentaddrulebutton.Visibility = Visibility.Collapsed;
            //AddInvoiceRuleButton.Visibility = Visibility.Visible;
            //currentaddrulebutton = AddInvoiceRuleButton;
        }
    }
}
