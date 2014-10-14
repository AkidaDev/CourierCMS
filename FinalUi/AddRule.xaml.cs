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
        int currentCanvas = 1;
        Canvas currentCanvasObj;
        Quotation quoation;
        CostingRule RuleCR;
        BillingDataDataContext db;
        public AddRule(int ruleId)
        {
            db = new BillingDataDataContext();
            Rule rule = db.Rules.SingleOrDefault(x => x.ID == ruleId);
            if(rule == null)
            {
                MessageBox.Show("Invalid Rule.");
                return;
            }
            InitializeComponent();
            RuleCR = (new JavaScriptSerializer()).Deserialize<CostingRule>(rule.Properties);
            List<Service> serviceListr =  DataSources.ServicesCopy.Where(x => !RuleCR.ServiceList.Contains(x.SER_CODE)).ToList();
            ServiceTwinBox.AllListSource = serviceListr;
            ServiceTwinBox.SelectedListSource = DataSources.ServicesCopy.Except<Service>(serviceListr).ToList();
            List<City> cityList = DataSources.CityCopy.Where(x => !RuleCR.CityList.Contains(x.CITY_CODE)).ToList();
            CitiesTwinBox.AllListSource = cityList;
            CitiesTwinBox.SelectedListSource = DataSources.CityCopy.Except<City>(cityList).ToList();
            List<State> stateList = DataSources.StateCopy.Where(x => !RuleCR.StateList.Contains(x.STATE_CODE)).ToList();
            StateTwinBox.AllListSource = stateList;
            StateTwinBox.SelectedListSource = DataSources.StateCopy.Except<State>(stateList).ToList() ;
            List<ZONE> zoneList = DataSources.ZoneCopy.Where(x => !RuleCR.ZoneList.Contains(x.zcode)).ToList();
            ZoneTwinBox.AllListSource = zoneList;
            ZoneTwinBox.SelectedListSource = DataSources.ZoneCopy.Except<ZONE>(zoneList).ToList();
            FromWeightBox.Text = RuleCR.startW.ToString();
            ToWeightBox.Text = RuleCR.endW.ToString();
            DOXAmountBox.Text = RuleCR.doxAmount.ToString();
            NDoxAmountBox.Text = RuleCR.ndoxAmount.ToString();
            if (RuleCR.type == 'R')
            {
                RangeTypeRadio.IsChecked = true;
                StepTypeRadio.IsChecked = false;
            }
            else
            {
                RangeTypeRadio.IsChecked = false;
                StepTypeRadio.IsChecked = true;
            }
            StepBlockBox.Text = RuleCR.stepWeight.ToString() ;
            DoxStartValueBox.Text = RuleCR.dStartValue.ToString();
            NDoxStartValueBox.Text = RuleCR.ndStartValue.ToString();


        }
        public AddRule(Quotation quoation)
        {
            this.quoation = quoation;
            RuleCR = new CostingRule();
            db = new BillingDataDataContext();
            InitializeComponent();
            currentCanvasObj = Step1Canvas;
            currentCanvasObj.Visibility = Visibility.Visible;
            ServiceTwinBox.AllListSource = (DataSources.ServicesCopy);
            ServiceTwinBox.SelectedListSource = new List<Service>();
            ServiceTwinBox.DisplayValuePath = "NameAndCOde";
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
            char type;
            if (RangeTypeRadio.IsChecked == true)
            {
                type = 'R';
            }
            else
                type = 'S';
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
            RuleCR.endW = endW;
            RuleCR.type = type;
            RuleCR.doxAmount = doxAmount;
            RuleCR.ndoxAmount = ndoxAmount;
            RuleCR.stepWeight = stepweight;
            RuleCR.dStartValue = doxStartValue;
            RuleCR.ndStartValue = ndoxStartValue;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string serialized = js.Serialize(RuleCR);
            Rule r = new Rule();
            r.Type = 1;
            r.Properties = serialized;
            r.QID = quoation.Id;
            r.Remark = "hello";
            db.Rules.InsertOnSubmit(r);
            bool isdone = false;
            if (validate())
            {
                try
                {
                    db.SubmitChanges();
                    isdone = true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                if(isdone)
                {
                    MessageBox.Show("Rule Added Now Party");
                    this.Close();
                }
            }
        }
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
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
                case 2:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
                case 3:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
                case 4:
                    currentCanvas = 5;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step5Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
            }
            if (currentCanvas == 5)
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
                case 5:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
                case 4:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
                case 3:
                    currentCanvas = 2;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
                case 2:
                    currentCanvas = 1;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step1Canvas;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    StepBlock.Text = "Step " + currentCanvas.ToString() + " of 5";
                    break;
            }
            if (currentCanvas == 5)
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
            try
            {

                StepWeightBlock.Visibility = Visibility.Visible;
                StepBlockBox.Visibility = Visibility.Visible;
                NDoxStartValueBox.Visibility = Visibility.Visible;
                DoxStartValue.Visibility = Visibility.Visible;
                DoxStartValueBox.Visibility = Visibility.Visible;
                NDoxStartValue.Visibility = Visibility.Visible;
            }
            catch(NullReferenceException ex)
            {
                
            }
        }

        private void RangeTypeRadio_Checked(object sender, RoutedEventArgs e)
        {

            StepWeightBlock.Visibility = Visibility.Collapsed;
            StepBlockBox.Visibility = Visibility.Collapsed;
            NDoxStartValueBox.Visibility = Visibility.Collapsed;
            DoxStartValue.Visibility = Visibility.Collapsed;
            DoxStartValueBox.Visibility = Visibility.Collapsed;
            NDoxStartValue.Visibility = Visibility.Collapsed;
        }
    }
}
