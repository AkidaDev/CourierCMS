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
        public AddServiceRule(Quotation quoation)
        {
            InitializeComponent();
            isdone = false;
            this.quotation = quoation;
            this.WholeRadio.Checked += WholeRadio_Checked;
            this.StepRadio.Checked += RadioButton_Checked_1;
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
            PerRadio.IsChecked = true;
             PercentageWrap.Visibility = Visibility.Visible;
            PerRadio.Checked += PerRadio_Checked;
            AmountRadio.Checked += AmountRadio_Checked;
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
            if (PerRadio.IsChecked == true)
            {
                mode = 'P';
            }
            else mode = 'A';
            char applicable;
            if (CompoundRadio.IsChecked == true)
            {
                applicable = 'C';
            }
            else
            {
                applicable = 'O';
            }
            float per = 0;
             if (!float.TryParse(PercentageBox.Text, out per))
                {
                    errorMsg += "Enter Percentage Properly\n";
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
            RuleSR = new ServiceRule();
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
            Rule r = new Rule();
            r.Type = 2;
            r.Properties = serialized;
            r.QID = quotation.Id;
            r.Remark = this.RemarkBox.Text;
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
                    MessageBox.Show("Rule Added Now Party");
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
