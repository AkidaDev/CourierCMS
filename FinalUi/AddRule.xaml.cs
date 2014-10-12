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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddRule : Window
    {
        int currentCanvas = 1;
        Canvas currentCanvasObj;
        public AddRule()
        {
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
            double startW = 0,endW = 0;
            string errorMsg ="";
            double temp;
            if(double.TryParse(FromWeightBox.Text,out temp))
            {
                startW = temp;
            }
            else
                errorMsg = errorMsg + "Enter From Weight Properly \n";
            if(double.TryParse(ToWeightBox.Text,out temp))
                endW = temp;
            else
                errorMsg = errorMsg + "Enter To Weight Properly \n";
            if(startW > endW)
                errorMsg += "Starting weight cannot be greater than ending weight. \n";
            char type;
            if(RangeTypeRadio.IsChecked == true)
            {
                type = 'R';
            }
            else
                type = 'S';
            double doxAmount = 0, ndoxAmount = 0;
            if(!double.TryParse(DOXAmountBox.Text, out doxAmount))
                errorMsg += "Enter dox amount properly \n";
            if(!double.TryParse(NDoxAmountBox.Text, out ndoxAmount))
                errorMsg += "Enter non dox amount properly \n";
            double doxStartValue = 0, ndoxStartValue = 0;
            if(!double.TryParse(DoxStartValueBox.Text,out doxStartValue) && StepTypeRadio.IsChecked==true)
                errorMsg += "Enter Dox start value properly \n";
            if(!Double.TryParse(NDoxStartValueBox.Text,out ndoxStartValue) && StepTypeRadio.IsChecked==true)
                errorMsg += "Enter Non Dox start value properly \n";
            double stepweight = 0;
            if(!double.TryParse(StepBlockBox.Text,out stepweight) && StepTypeRadio.IsChecked==true)
                errorMsg += "Enter Step Weight Properly \n"; 
            if(errorMsg != "")
            {
                MessageBox.Show("Please correct following errors: " + errorMsg);
                return;
            }
            List<string> selectedServiceList = ServiceTwinBox.SelectedListSource.Cast<Service>().Select(x=>x.SER_CODE).ToList();
            List<string> selectedZoneList = ZoneTwinBox.SelectedListSource.Cast<ZONE>().Select(x=>x.zcode).ToList();
            List<String> selectedCityList = CitiesTwinBox.SelectedListSource.Cast<City>().Select(x=>x.CITY_CODE).ToList();
            List<string> selectedStateList = StateTwinBox.SelectedListSource.Cast<State>().Select(x=>x.STATE_CODE).ToList();
            CostingRule RuleCR = new CostingRule(selectedServiceList, selectedZoneList, selectedCityList, selectedStateList, startW, endW, type, doxAmount, ndoxAmount,stepweight,ndoxStartValue,doxStartValue);
            JavaScriptSerializer js = new JavaScriptSerializer();
            string serialized = js.Serialize(RuleCR);
            MessageBox.Show(serialized);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
              switch (currentCanvas)
                { 
                    case 1 :
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
    }
}
