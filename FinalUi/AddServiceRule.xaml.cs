﻿using System;
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
        int currentGrid = 1;
        Grid currentGridObj;
        ServiceRule RuleSR;
        Quotation quotation;
        ServiceRule SRule;
        Rule rule;
        public bool isUpdate { get; set; }
        public bool isInitialized { get; set; }
        public AddServiceRule()
        {
            InitializeComponent();
            currentGridObj = Step1Grid;
            currentGridObj.Visibility = Visibility.Visible;
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
            ServiceGroupTwinBox.AllListSource = DataSources.ServiceGroupCopy;
            ServiceGroupTwinBox.DisplayValuePath = "GroupName";
            ServiceGroupTwinBox.SelectedListSource = new List<ServiceGroup>();
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
            AddRule.setFormList(SRule, this.ServiceTwinBox, this.ZoneTwinBox, this.StateTwinBox, this.CitiesTwinBox, ServiceGroupTwinBox);
            this.RemarkBox.Text = rule.Remark;
            this.AddFilter.Data = Geometry.Parse(@"F1M2,12.942C2,12.942 10.226,15.241 10.226,15.241 10.226,15.241 8.275,17.071 8.275,17.071 9.288,17.922 10.917,18.786 12.32,18.786 15.074,18.786 17.386,16.824 18.039,14.171 18.039,14.171 21.987,15.222 21.987,15.222 20.891,19.693 16.996,23 12.357,23 9.771,23 7.076,21.618 5.308,19.934 5.308,19.934 3.454,21.671 3.454,21.671 3.454,21.671 2,12.942 2,12.942z M11.643,2C14.229,2 16.924,3.382 18.692,5.066 18.692,5.066 20.546,3.329 20.546,3.329 20.546,3.329 22,12.058 22,12.058 22,12.058 13.774,9.759 13.774,9.759 13.774,9.759 15.725,7.929 15.725,7.929 14.712,7.078 13.083,6.214 11.68,6.214 8.926,6.214 6.614,8.176 5.961,10.829 5.961,10.829 2.013,9.778 2.013,9.778 3.109,5.307 7.004,2 11.643,2z");
            this.AddFilter.Height = 18;
            this.AddFilter.Width = 18;
            this.Title = "Update Rule " + ruleId.ToString();
            this.AddRuleButtonBox.Text = " Update Rule";
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
            else
            {
                mode = 'A';
                if (!float.TryParse(AmountBox.Text, out per))
                {
                    errorMsg += "Enter Percentage Properly\n";
                }
            }
            char applicable;
            applicable = 'O';


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
            RuleSR.ServiceGroupList = selectedServiceGroupList;
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
            if (!isUpdate)
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
                    if (!isUpdate)
                        MessageBox.Show("New Service Rule Added");
                    else
                        MessageBox.Show("Service Rule updated");
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
        private void FromWeightBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }
        private void WholeRadio_Checked(object sender, RoutedEventArgs e)
        {
            StepBox.Visibility = Visibility.Collapsed;
            this.StepRadio.Content = "Step";
        }
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            StepBox.Visibility = Visibility.Visible;
            this.StepRadio.Content = "Step .: ";
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

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            viewhelp window = new viewhelp();
            window.Show();
        }
    }
}
