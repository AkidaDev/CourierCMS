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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddRule : Window
    {
        int currentGrid = 1;
        Grid currentGridObj;
        Rule rule;
        Quotation quoation;
        CostingRule RuleCR;
        BillingDataDataContext db;
        bool isInitialized = false;
        public bool isUpdate { get; set; }
        public AddRule()
        {
            InitializeComponent();
            isInitialized = true;
            currentGridObj = Step1Grid;
        }
        public AddRule(int ruleId)
            : this()
        {
            this.isUpdate = true;
            db = new BillingDataDataContext();
            rule = db.Rules.SingleOrDefault(x => x.ID == ruleId);
            if (rule == null)
            {
                MessageBox.Show("Invalid Rule.");
                return;
            }
            InitializeComponent();
            isInitialized = true;
            TitleBox.Text = "Update Rule " + ruleId.ToString();
            RuleCR = (new JavaScriptSerializer()).Deserialize<CostingRule>(rule.Properties);
            FromWeightBox.Text = RuleCR.startW.ToString();
            ToWeightBox.Text = RuleCR.endW.ToString();
            DOXAmountBox.Text = RuleCR.doxAmount.ToString();
            NDoxAmountBox.Text = RuleCR.ndoxAmount.ToString();
            AddRule.setFormList(RuleCR, ServiceTwinBox, ZoneTwinBox, StateTwinBox, CitiesTwinBox);
            if (RuleCR.type == 'R')
            {
                RangeTypeRadio.IsChecked = true;
                StepTypeRadio.IsChecked = false;
                MultiplierTypeRadio.IsChecked = false;
            }
            if (RuleCR.type == 'S')
            {
                RangeTypeRadio.IsChecked = false;
                StepTypeRadio.IsChecked = true;
                MultiplierTypeRadio.IsChecked = false;
            }
            if (RuleCR.type == 'M')
            {
                RangeTypeRadio.IsChecked = false;
                StepTypeRadio.IsChecked = false;
                MultiplierTypeRadio.IsChecked = true;

            }
            StepBlockBox.Text = RuleCR.stepWeight.ToString();
            DoxStartValueBox.Text = RuleCR.dStartValue.ToString();
            NDoxStartValueBox.Text = RuleCR.ndStartValue.ToString();
        }
        public AddRule(Quotation quoation)
            : this()
        {
            this.quoation = quoation;
            RuleCR = new CostingRule();
            db = new BillingDataDataContext();
            currentGridObj.Visibility = Visibility.Visible;
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
        }
        public static void setFormList(IRule crule, CustomControls.TwinListBox ServiceTwinBox, CustomControls.TwinListBox ZoneTwinBox, CustomControls.TwinListBox StateTwinBox, CustomControls.TwinListBox CitiesTwinBox)
        {
            if (crule == null)
            {
                throw new Exception("Rule is Null");

            }
            if (crule.ServiceList.Count > 0)
            {
                ServiceTwinBox.AllListSource = DataSources.ServicesCopy.Where(x => !crule.ServiceList.Contains(x.SER_CODE)).ToList();
                ServiceTwinBox.SelectedListSource = DataSources.ServicesCopy.Where(x => crule.ServiceList.Contains(x.SER_CODE)).ToList();
            }
            else
            {
                ServiceTwinBox.AllListSource = (DataSources.ServicesCopy);
                ServiceTwinBox.SelectedListSource = new List<Service>();
            }
            ServiceTwinBox.DisplayValuePath = "NameAndCode";
            if (crule.ZoneList.Count > 0)
            {
                ZoneTwinBox.AllListSource = DataSources.ZoneCopy.Where(x => !crule.ZoneList.Contains(x.zcode)).ToList();
                ZoneTwinBox.SelectedListSource = DataSources.ZoneCopy.Where(x => crule.ZoneList.Contains(x.zcode)).ToList();
            }
            else
            {
                ZoneTwinBox.AllListSource = (DataSources.ZoneCopy);
                ZoneTwinBox.SelectedListSource = new List<ZONE>();
            }
            ZoneTwinBox.DisplayValuePath = "NameAndCode";
            if (crule.StateList.Count > 0)
            {
                StateTwinBox.AllListSource = DataSources.StateCopy.Where(x => !crule.StateList.Contains(x.STATE_CODE)).ToList();
                StateTwinBox.SelectedListSource = DataSources.StateCopy.Where(x => crule.StateList.Contains(x.STATE_CODE)).ToList();
            }
            else
            {
                StateTwinBox.AllListSource = DataSources.StateCopy;
                StateTwinBox.SelectedListSource = new List<State>();
            }
            StateTwinBox.DisplayValuePath = "NameAndCode";
            if (crule.CityList.Count > 0)
            {
                CitiesTwinBox.AllListSource = DataSources.CityCopy.Where(x => !crule.CityList.Contains(x.CITY_CODE)).ToList();
                CitiesTwinBox.SelectedListSource = DataSources.CityCopy.Where(x => crule.CityList.Contains(x.CITY_CODE)).ToList();
            }
            else
            {
                CitiesTwinBox.AllListSource = DataSources.CityCopy;
                CitiesTwinBox.SelectedListSource = new List<City>();
            }
            CitiesTwinBox.DisplayValuePath = "NameAndCode";
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
        { }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        { }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        { }
        private void UpdateRule()
        { }
        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            Rule r;
            BillingDataDataContext db = new BillingDataDataContext();
            if (!isUpdate)
            {
                r = new Rule();
            }
            else
            {
                if (rule != null)
                    r = rule;
                else
                {
                    MessageBox.Show("Some Error Occured");
                    r = new Rule();
                    this.Close();
                }
            }
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
            char type = 'U';
            if (RangeTypeRadio.IsChecked == true)
                type = 'R';
            if (StepTypeRadio.IsChecked == true)
                type = 'S';
            if (MultiplierTypeRadio.IsChecked == true)
                type = 'M';
            double doxAmount = 0, ndoxAmount = 0;
            if (!double.TryParse(DOXAmountBox.Text, out doxAmount))
                errorMsg += "Enter dox amount properly \n";
            if (!double.TryParse(NDoxAmountBox.Text, out ndoxAmount))
                errorMsg += "Enter non dox amount properly \n";
            double doxStartValue = 0, ndoxStartValue = 0;
            if (!double.TryParse(DoxStartValueBox.Text, out doxStartValue) && StepTypeRadio.IsChecked == true)
                errorMsg += "Enter Dox start value properly \n";
            if (!Double.TryParse(NDoxStartValueBox.Text, out ndoxStartValue) && StepTypeRadio.IsChecked == true)
                errorMsg += "Enter Non Dox start value properly \n";
            double stepweight = 0;
            if (!double.TryParse(StepBlockBox.Text, out stepweight) && StepTypeRadio.IsChecked == true)
                errorMsg += "Enter Step Weight Properly \n";
            if (errorMsg != "")
            {
                MessageBox.Show("Please correct following errors: " + errorMsg);
                return;
            }
            List<string> selectedServiceList = ServiceTwinBox.SelectedListSource.Cast<Service>().Select(x => x.SER_CODE).ToList();
            List<string> selectedZoneList = ZoneTwinBox.SelectedListSource.Cast<ZONE>().Select(x => x.zcode).ToList();
            List<String> selectedCityList = CitiesTwinBox.SelectedListSource.Cast<City>().Select(x => x.CITY_CODE).ToList();
            List<string> selectedStateList = StateTwinBox.SelectedListSource.Cast<State>().Select(x => x.STATE_CODE).ToList();
            RuleCR.ServiceList = selectedServiceList;
            RuleCR.ZoneList = selectedZoneList;
            RuleCR.CityList = selectedCityList;
            RuleCR.StateList = selectedStateList;
            
            RuleCR.startW = startW;
            int id;
            id = Convert.ToInt32(db.ExecuteQuery<decimal>("SELECT IDENT_CURRENT('Rule') +1;").FirstOrDefault());
            if (!isUpdate)
            {
                RuleCR.Id = id;
                r.QID = quoation.Id;
            }
            RuleCR.endW = endW;
            RuleCR.type = type;
            RuleCR.doxAmount = doxAmount;
            RuleCR.ndoxAmount = ndoxAmount;
            RuleCR.stepWeight = stepweight;
            RuleCR.dStartValue = doxStartValue;
            RuleCR.ndStartValue = ndoxStartValue;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string serialized = js.Serialize(RuleCR);
            r.Type = 1;
            r.Properties = serialized;
            r.Remark = this.RemarkBox.Text ?? " ";
            if (!isUpdate)
                db.Rules.InsertOnSubmit(r);
            else
            {
                Rule ruledb = db.Rules.Where(x => x.ID == r.ID).FirstOrDefault();
                if (ruledb == null)
                {
                    MessageBox.Show("Some Error Occured");
                    return;
                }
                else
                    setvalue(ruledb, r);
            }
            bool isdone = false;
            if (validate())
            {
                try
                {
                    db.SubmitChanges();
                    isdone = true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                if (!isUpdate)
                {
                    if (isdone)
                    {
                        if (MessageBox.Show("Do you want to add a new rule for this configuration of service and destination", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            FromWeightBox.Text = (RuleCR.endW + 0.0001).ToString();
                            ToWeightBox.Text = "";
                            currentGrid = 6;
                            currentGridObj.Visibility = Visibility.Collapsed;
                            currentGridObj = Step6Grid;
                            currentGridObj.Visibility = Visibility.Visible;
                            StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                        }
                        else
                            this.Close();
                    }
                }
                else
                {
                    if(isdone)
                    {
                        MessageBox.Show("Rule updated successfully","Information");
                        this.Close();
                    }
                }
            }
        }
        public void setvalue(Rule r, Rule rule)
        {
            r.ID = rule.ID;
            r.QID = rule.QID;
            r.Remark = rule.Remark;
            r.Properties = rule.Properties;
            r.Type = rule.Type;
        }
        private bool validate()
        {
            if (this.StateTwinBox.SelectedListR.Items.Count > 0 || this.ZoneTwinBox.SelectedListR.Items.Count > 0 || this.CitiesTwinBox.SelectedListR.Items.Count > 0 && this.ServiceTwinBox.SelectedListR.Items.Count > 0)
                return true;
            else
            {
                MessageBox.Show("Must add at least one service and at least one from any zone or city or state ");
                return false;
            }
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            switch (currentGrid)
            {
                case 1:
                    currentGrid = 2;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step2Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 2:
                    currentGrid = 3;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step3Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 3:
                    currentGrid = 4;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step4Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 4:
                    currentGrid = 5;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step5Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 5:
                    currentGrid = 6;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step6Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 6:
                    currentGrid = 7;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step7Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
            }
            if (currentGrid == 7)
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
            switch (currentGrid)
            {
                case 7:
                    currentGrid = 6;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step6Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 6:
                    currentGrid = 5;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step5Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 5:
                    currentGrid = 4;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step4Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 4:
                    currentGrid = 3;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step3Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 3:
                    currentGrid = 2;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step2Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
                case 2:
                    currentGrid = 1;
                    currentGridObj.Visibility = Visibility.Collapsed;
                    currentGridObj = Step1Grid;
                    currentGridObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentGrid.ToString() + " of 7";
                    break;
            }
            if (currentGrid == 7)
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

        private void StepTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                try
                {
                    StepWeightBlock.Visibility = Visibility.Visible;
                    StepBlockBox.Visibility = Visibility.Visible;
                    NDoxStartValueBox.Visibility = Visibility.Visible;
                    DoxStartValue.Visibility = Visibility.Visible;
                    DoxStartValueBox.Visibility = Visibility.Visible;
                    NDoxStartValue.Visibility = Visibility.Visible;
                    vertical_line.Visibility = Visibility.Visible;
                }
                catch (NullReferenceException ex)
                {

                }
            }
        }
        private void RangeTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                vertical_line.Visibility = Visibility.Collapsed;
                StepWeightBlock.Visibility = Visibility.Collapsed;
                StepBlockBox.Visibility = Visibility.Collapsed;
                NDoxStartValueBox.Visibility = Visibility.Collapsed;
                DoxStartValue.Visibility = Visibility.Collapsed;
                DoxStartValueBox.Visibility = Visibility.Collapsed;
                NDoxStartValue.Visibility = Visibility.Collapsed;
            }
        }

        private void MultiplierTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {

                StepWeightBlock.Visibility = Visibility.Visible;
                StepBlockBox.Visibility = Visibility.Visible;
                vertical_line.Visibility = Visibility.Collapsed;
                NDoxStartValueBox.Visibility = Visibility.Collapsed;
                DoxStartValue.Visibility = Visibility.Collapsed;
                DoxStartValueBox.Visibility = Visibility.Collapsed;
                NDoxStartValue.Visibility = Visibility.Collapsed;
            }
        }
    }
}