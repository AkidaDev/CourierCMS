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

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for EditRateDetailsWindow.xaml
    /// </summary>
    public partial class EditRateDetailsWindow : Window
    {
        public RateDetail retD;
        public bool isRateAdded = false;
        public EditRateDetailsWindow(RateDetail retD)
        {
            this.retD = retD;
            InitializeComponent();
            if (retD.Type == 1)
            {
                MainPanel.Children.Remove(StepWeightWrap);
            }
            else
            {
                StepWeightTextBox.Text = retD.StepWeight.ToString();
            }
            WeightTextBox.Text = retD.Weight.ToString();
            NonDoxRateBox.Text = retD.NonDoxRate.ToString();
            DoxRateTextBox.Text = retD.DoxRate.ToString();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            retD.DoxRate = double.Parse(DoxRateTextBox.Text);
            retD.Weight = double.Parse(WeightTextBox.Text);
            if (NonDoxRateBox.Text == "")
                retD.NonDoxRate = retD.DoxRate;
            else
                retD.NonDoxRate = double.Parse(NonDoxRateBox.Text);
            if (retD.Type != 1)
                retD.StepWeight = double.Parse(StepWeightTextBox.Text);
            isRateAdded = true;
            this.Close();
        }
		private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
