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
        public bool isUpdate { get; set; }
        public AddRule()
        {
            InitializeComponent();
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
            this.Title = "Update Rule " + ruleId.ToString();
            this.AddFilter.Data = Geometry.Parse(@"F1M2,12.942C2,12.942 10.226,15.241 10.226,15.241 10.226,15.241 8.275,17.071 8.275,17.071 9.288,17.922 10.917,18.786 12.32,18.786 15.074,18.786 17.386,16.824 18.039,14.171 18.039,14.171 21.987,15.222 21.987,15.222 20.891,19.693 16.996,23 12.357,23 9.771,23 7.076,21.618 5.308,19.934 5.308,19.934 3.454,21.671 3.454,21.671 3.454,21.671 2,12.942 2,12.942z M11.643,2C14.229,2 16.924,3.382 18.692,5.066 18.692,5.066 20.546,3.329 20.546,3.329 20.546,3.329 22,12.058 22,12.058 22,12.058 13.774,9.759 13.774,9.759 13.774,9.759 15.725,7.929 15.725,7.929 14.712,7.078 13.083,6.214 11.68,6.214 8.926,6.214 6.614,8.176 5.961,10.829 5.961,10.829 2.013,9.778 2.013,9.778 3.109,5.307 7.004,2 11.643,2z");
            this.AddFilter.Height = 18;
            this.AddFilter.Width = 18;
            this.AddRuleButtonBox.Text = " Update Rule";
            RuleCR = (new JavaScriptSerializer()).Deserialize<CostingRule>(rule.Properties);
            FromWeightBox.Text = RuleCR.startW.ToString();
            ToWeightBox.Text = RuleCR.endW.ToString();
            DOXAmountBox.Text = RuleCR.doxAmount.ToString();
            NDoxAmountBox.Text = RuleCR.ndoxAmount.ToString();
            AddRule.setFormList(RuleCR, ServiceTwinBox, ZoneTwinBox, StateTwinBox, CitiesTwinBox, ServiceGroupTwinBox);
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
            ServiceGroupTwinBox.AllListSource = DataSources.ServiceGroupCopy;
            ServiceGroupTwinBox.DisplayValuePath = "GroupName";
            ServiceGroupTwinBox.SelectedListSource = new List<ServiceGroup>();
        }
        public static void setFormList(IRule crule, CustomControls.TwinListBox ServiceTwinBox, CustomControls.TwinListBox ZoneTwinBox, CustomControls.TwinListBox StateTwinBox, CustomControls.TwinListBox CitiesTwinBox, CustomControls.TwinListBox ServiceGroupTwinBox)
        {
            if (crule == null)
            {
                throw new Exception("Rule is Null");
            }
            if (crule.ServiceGroupList != null)
            {
                ServiceGroupTwinBox.AllListSource = DataSources.ServiceGroupCopy.Where(x => !crule.ServiceGroupList.Contains(x.GroupName)).ToList();
                ServiceGroupTwinBox.SelectedListSource = DataSources.ServiceGroupCopy.Where(x => crule.ServiceGroupList.Contains(x.GroupName)).ToList();
            }
            else
            {
                ServiceGroupTwinBox.AllListSource = DataSources.ServiceGroupStatic;
                ServiceGroupTwinBox.SelectedListSource = new List<ServiceGroup>();
            }
            ServiceGroupTwinBox.DisplayValuePath = "GroupName";
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
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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
            double stepweight = 0;
            if (!double.TryParse(StepBlockBox.Text, out stepweight) && StepTypeRadio.IsChecked == true)
                errorMsg += "Enter Step Weight Properly \n";
            if (errorMsg != "")
            {
                MessageBox.Show("Please correct following errors: " + errorMsg);
                return;
            }
            List<string> selectedServiceList = ((ServiceTwinBox.SelectedListSource) ?? new List<Service>()).Cast<Service>().Select(x => x.SER_CODE).ToList();
            List<string> selectedZoneList = ((ZoneTwinBox.SelectedListSource) ?? new List<ZONE>()).Cast<ZONE>().Select(x => x.zcode).ToList();
            List<String> selectedCityList = ((CitiesTwinBox.SelectedListSource) ?? new List<City>()).Cast<City>().Select(x => x.CITY_CODE).ToList();
            List<string> selectedStateList = ((StateTwinBox.SelectedListSource) ?? new List<State>()).Cast<State>().Select(x => x.STATE_CODE).ToList();
            List<string> selectedServiceGroupList = ((ServiceGroupTwinBox.SelectedListSource) ?? new List<ServiceGroup>()).Cast<ServiceGroup>().Select(x => x.GroupName).ToList();
            RuleCR.ServiceList = selectedServiceList;
            RuleCR.ZoneList = selectedZoneList;
            RuleCR.CityList = selectedCityList;
            RuleCR.StateList = selectedStateList;
            RuleCR.ServiceGroupList = selectedServiceGroupList;
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
                    if (isdone)
                    {
                        MessageBox.Show("Rule updated successfully", "Information");
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
            if ((this.StateTwinBox.SelectedListR.Items.Count > 0 || this.ZoneTwinBox.SelectedListR.Items.Count > 0 || this.CitiesTwinBox.SelectedListR.Items.Count > 0 )&&( this.ServiceTwinBox.SelectedListR.Items.Count > 0 || this.ServiceGroupTwinBox.SelectedListR.Items.Count > 0))
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
                }
                catch (NullReferenceException)
                {

                }
            }
        }
        private void RangeTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                StepWeightBlock.Visibility = Visibility.Collapsed;
                StepBlockBox.Visibility = Visibility.Collapsed;
            }
        }

        private void MultiplierTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {

                StepWeightBlock.Visibility = Visibility.Visible;
                StepBlockBox.Visibility = Visibility.Visible;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            viewhelp window = new viewhelp();
            window.Show();
        }
    }
}