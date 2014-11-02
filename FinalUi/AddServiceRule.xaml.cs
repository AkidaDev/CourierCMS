using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddServiceRule.xamleee
    /// </summary>
    public partial class AddServiceRule : Window
    {
        int currentCanvas = 1;
        Canvas currentCanvasObj;
        ServiceRule RuleSR;
        Quotation quotation;
        ServiceRule SRule;
        Rule rule;
        public bool isUpdate { get; set; }
        public bool isInitialized { get; set; }
        public AddServiceRule()
        {
            InitializeComponent();
            currentCanvasObj = Step1Canvas;
            currentCanvasObj.Visibility = Visibility.Visible;
            this.WholeRadio.Checked += WholeRadio_Checked;
            this.StepRadio.Checked += RadioButton_Checked_1;
            PerRadio.Checked += PerRadio_Checked;
            AmountRadio.Checked += AmountRadio_Checked;
        }
        public AddServiceRule(Quotation quoation)
            : this()
        {
            isdone = false;
            this.quotation = quoation;
            ServiceTwinBox.AllListSource = (DataSources.ServicesCopy);
            ServiceTwinBox.SelectedListSource = new List<Service>();
            ServiceTwinBox.DisplayValuePath = "NameAndCode";
            ZoneTwinBox.AllListSource = (DataSources.ZoneCopy);
            ZoneTwinBox.SelectedListSource = new List<ZONE>();
            ZoneTwinBox.DisplayValuePath = "NameAndCode";
            StateTwinBox.AllListSource = DataSources.StateCopy;
            StateTwinBox.SelectedListSource = new List<State>();
            StateTwinBox.DisplayValuePath = "NameAndCode";
            CitiesTwinBox.AllListSource = DataSources.CityCopy;
            CitiesTwinBox.SelectedListSource = new List<City>();
            CitiesTwinBox.DisplayValuePath = "NameAndCode";
            PerRadio.IsChecked = true;
            PercentageWrap.Visibility = Visibility.Visible;
        }
        public AddServiceRule(int ruleId)
            : this()
        {
            this.isUpdate = true;
            BillingDataDataContext db = new BillingDataDataContext();
            db = new BillingDataDataContext();
            rule = db.Rules.SingleOrDefault(x => x.ID == ruleId);
            if (rule == null)
            {
                MessageBox.Show("Invalid Rule.");
                return;
            }
            isInitialized = true;
            SRule = RuleSR = (new JavaScriptSerializer()).Deserialize<ServiceRule>(rule.Properties);
            this.FromWeightBox.Text = SRule.startW.ToString();
            this.ToWeightBox.Text = SRule.endW.ToString();
            if (SRule.type == 'W')
            {
                this.WholeRadio.IsChecked = true;
                this.StepRadio.IsChecked = false;
            }
            else
            {
                this.WholeRadio.IsChecked = false;
                this.StepRadio.IsChecked = true;
                this.StepBox.Text = SRule.stepweight.ToString();
            }
            if (SRule.change == 'I')
                this.IncRadio.IsChecked = true;
            else
                this.DecRadio.IsChecked = true;
            if (SRule.mode == 'P')
            {
                this.PerRadio.IsChecked = true;
                this.ValueBox.Text = SRule.per.ToString();
            }
            else
            {
                this.AmountRadio.IsChecked = true;
                this.AmountBox.Text = SRule.per.ToString();
            }
            if (SRule.applicable == 'O')
                this.OriginalAmountRadio.IsChecked = true;
            else
                this.CompoundRadio.IsChecked = true;
            AddRule.setFormList(SRule,this.ServiceTwinBox,this.ZoneTwinBox,this.StateTwinBox,this.CitiesTwinBox);
            this.RemarkBox.Text = rule.Remark;
            this.AddRuleButtonBox.Text = "Update Rule";
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void GetFilter_Click(object sender, RoutedEventArgs e)
        {
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
        }
        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            Rule r;
            BillingDataDataContext db = new BillingDataDataContext();
            double startW = 0, endW = 0;
            string errorMsg = "";
            double temp;
            if (double.TryParse(FromWeightBox.Text, out temp))
            {
                startW = temp;
            }
            else
                errorMsg = errorMsg + "Enter From Weight Properly \n";
            if (double.TryParse(ToWeightBox.Text, out temp))
                endW = temp;
            else
                errorMsg = errorMsg + "Enter To Weight Properly \n";
            if (startW > endW)
                errorMsg += "Starting weight cannot be greater than ending weight. \n";
            double stepweight = 0;
            if (!double.TryParse(StepBox.Text, out stepweight) && StepRadio.IsChecked == true)
                errorMsg += "Enter Step Weight Properly \n";
            char type;
            double step = 0;
            if (WholeRadio.IsChecked == true)
            {
                type = 'W';
            }
            else
            {
                type = 'S';
                if (!double.TryParse(StepBox.Text, out step) && StepRadio.IsChecked == true)
                    errorMsg += "Enter Step Weight Properly \n";
            }
            char change;
            if (IncRadio.IsChecked == true)
            {
                change = 'I';
            }
            else { change = 'D'; }
            char mode;
            float per = 0;
            if (PerRadio.IsChecked == true)
            {
                mode = 'P';
                if (!float.TryParse(ValueBox.Text, out per))
                {
                    errorMsg += "Enter Percentage Properly\n";
                }

            }
            else { 
                mode = 'A';
                if (!float.TryParse(AmountBox.Text, out per))
                {
                    errorMsg += "Enter Percentage Properly\n";
                }
            }
            char applicable;
            if (CompoundRadio.IsChecked == true)
            {
                applicable = 'C';
            }
            else
            {
                applicable = 'O';
            }
            
            
            if (errorMsg != "")
            {
                MessageBox.Show("Please correct following errors: " + errorMsg);
                return;
            }
            List<string> selectedServiceList = ServiceTwinBox.SelectedListSource.Cast<Service>().Select(x => x.SER_CODE).ToList();
            List<string> selectedZoneList = ZoneTwinBox.SelectedListSource.Cast<ZONE>().Select(x => x.zcode).ToList();
            List<String> selectedCityList = CitiesTwinBox.SelectedListSource.Cast<City>().Select(x => x.CITY_CODE).ToList();
            List<string> selectedStateList = StateTwinBox.SelectedListSource.Cast<State>().Select(x => x.STATE_CODE).ToList();
            if (RuleSR == null || rule == null)
            {
                r = new Rule();
                RuleSR = new ServiceRule();
                int id;
                id = Convert.ToInt32(db.ExecuteQuery<decimal>("SELECT IDENT_CURRENT('Rule') +1;").FirstOrDefault());
                RuleSR.Id = id;
            }
            else
            {
                r = db.Rules.SingleOrDefault(x => x.ID == rule.ID);
            }
            RuleSR.ServiceList = selectedServiceList;
            RuleSR.ZoneList = selectedZoneList;
            RuleSR.CityList = selectedCityList;
            RuleSR.StateList = selectedStateList;
            RuleSR.startW = startW;
            RuleSR.endW = endW;
            RuleSR.type = type;
            RuleSR.change = change;
            RuleSR.mode = mode;
            RuleSR.per = per;
            RuleSR.step = step;
            RuleSR.stepweight = stepweight;
            RuleSR.applicable = applicable;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string serialized = js.Serialize(RuleSR);
            r.Type = 2;
            r.Properties = serialized;
            if(!isUpdate)
            r.QID = quotation.Id;
            r.Remark = this.RemarkBox.Text;
            if (!isUpdate)
            db.Rules.InsertOnSubmit(r);
            if (validate())
            {
                try
                {
                    db.SubmitChanges();
                    isdone = true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                if (isdone)
                {
                    MessageBox.Show("New Service Rule Added");
                    this.Close();
                }
            }
        }
        public bool isdone;
        private bool validate()
        {
            if (this.StateTwinBox.SelectedListR.Items.Count > 0 || this.ZoneTwinBox.SelectedListR.Items.Count > 0 || this.CitiesTwinBox.SelectedListR.Items.Count > 0 && this.ServiceTwinBox.SelectedListR.Items.Count > 0)
                return true;
            else
            {
                MessageBox.Show("Must add at least one service and at least one from any zone or city or state ");
            }
            return false;
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            switch (currentCanvas)
            {
                case 1:
                    currentCanvas = 2;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 2:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 3:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 4:
                    currentCanvas = 5;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step5Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 5:
                    currentCanvas = 6;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step6Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
            }
            if (currentCanvas == 6)
            {
                Next.Visibility = Visibility.Collapsed;
                AddRuleButton.Visibility = Visibility.Visible;
            }
            else
            {
                Next.Visibility = Visibility.Visible;
                AddRuleButton.Visibility = Visibility.Collapsed;
            }
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            switch (currentCanvas)
            {
                case 6:
                    currentCanvas = 5;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step5Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 5:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 4:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 3:
                    currentCanvas = 2;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
                case 2:
                    currentCanvas = 1;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step1Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 6";
                    break;
            }
            if (currentCanvas == 6)
            {
                Next.Visibility = Visibility.Collapsed;
                AddRuleButton.Visibility = Visibility.Visible;
            }
            else
            {
                Next.Visibility = Visibility.Visible;
                AddRuleButton.Visibility = Visibility.Collapsed;
            }
        }
        private void FromWeightBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }
        private void WholeRadio_Checked(object sender, RoutedEventArgs e)
        {
            StepBox.Visibility = Visibility.Collapsed;
        }
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            StepBox.Visibility = Visibility.Visible;
        }

        private void PerRadio_Checked(object sender, RoutedEventArgs e)
        {
            AmountWrap.Visibility = Visibility.Collapsed;
            PercentageWrap.Visibility = Visibility.Visible;
        }

        private void AmountRadio_Checked(object sender, RoutedEventArgs e)
        {
            AmountWrap.Visibility = Visibility.Visible;
            PercentageWrap.Visibility = Visibility.Collapsed;
        }
    }
}
