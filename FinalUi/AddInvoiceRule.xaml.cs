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
using System.Windows.Shapes;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for AddInvoiceRule.xaml
    /// </summary>
    public partial class AddInvoiceRule : Window
    {
        int currentCanvas = 1;
        Canvas currentCanvasObj;
        Quotation quoation;
        public AddInvoiceRule(Quotation quoation)
        {
            InitializeComponent();
            this.quoation = quoation;
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
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
