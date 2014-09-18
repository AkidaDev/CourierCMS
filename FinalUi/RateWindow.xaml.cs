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
using System.Xml.Linq;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for RateWindow.xaml
    /// </summary>
    public partial class RateWindow : Window
    {
        CollectionViewSource ZoneList;
        CollectionViewSource ServiceList;
        CollectionViewSource RateList;
        bool isEdited;
        public RateWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            ZoneList.Source = db.ZONEs.Select(x => x.zcode);
            ServiceList.Source = db.Services.Select(x => x.SER_CODE);
            RateList.Source = db.ZONEs.Select(x => x.zcode);
            isEdited = false;
        }

        private void ComboBoxRate_KeyUp(object sender, KeyEventArgs e)
        {
            string rateCode = ComboBoxRate.Text;
            BillingDataDataContext db = new BillingDataDataContext();
            Assignment assign = db.Assignments.FirstOrDefault(x => x.RateCode == rateCode);
            if (assign != null)
            {
                ComboBoxRate.SelectedValue = assign.RateCode;
                ComboBoxService.SelectedValue = assign.ServiceCode;
            }
        }
        private void executeFillNewRatePanel()
        {
        }
        private void fillPanel(XElement rate)
        {

            RatePanel.Children.RemoveRange(0, RatePanel.Children.Count - 1);
            foreach (var item in rate.Elements())
            {
                WrapPanel rateNodeTypePanel = new WrapPanel();
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "Select Type: ";
                rateNodeTypePanel.Children.Add(textBlock);
                ComboBox ComboBoxSelectType = new ComboBox();
                ComboBoxSelectType.Items.Add("Range");
                ComboBoxSelectType.Items.Add("Step");
                ComboBoxSelectType.SelectionChanged += ComboBoxSelectType_SelectionChanged;
                rateNodeTypePanel.Children.Add(ComboBoxSelectType);
                TextBlock weightTextBlock = new TextBlock();
                weightTextBlock.Text = "Enter TillWeight: ";
                rateNodeTypePanel.Children.Add(weightTextBlock);
                TextBox weightTextBox = new TextBox();
                rateNodeTypePanel.Children.Add(weightTextBox);
                weightTextBox.Tag = "Weight";
                TextBlock priceTextBlock = new TextBlock();
                priceTextBlock.Text = "Enter Price: ";
                rateNodeTypePanel.Children.Add(priceTextBlock);
                TextBox priceTextBox = new TextBox();
                priceTextBox.Tag = "Price";
                rateNodeTypePanel.Children.Add(priceTextBox);
                if (item.Attribute("type").Value == "step")
                {
                    TextBlock stepTextBlock = new TextBlock();
                    stepTextBlock.Text = "Enter StepWeight: ";
                    rateNodeTypePanel.Children.Add(stepTextBlock);
                    TextBox stepTextBox = new TextBox(); 
                    stepTextBox.Tag = "StepWeight";
                    rateNodeTypePanel.Children.Add(stepTextBox);
                }
            }
        }

        void ComboBoxSelectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
