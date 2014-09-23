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
    /// Interaction logic for AddNewRateWindow.xaml
    /// </summary>
    public partial class AddNewRateWindow : Window
    {
        public Rate rate;
        public bool isEntered = false;
        public AddNewRateWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            rate = new Rate();
            rate.RateCode = RateCodeTextBox.Text;
            rate.Description = RateDescriptionTextBox.Text;
            BillingDataDataContext db = new BillingDataDataContext();
            if(db.Rates.Count(x=>x.RateCode == rate.RateCode) > 0)
            {
                MessageBox.Show("This rate code already exists. Please enter different code");
            }
            else
            {
                isEntered = true;
                this.Close();
            }
           

        }
    }
}
