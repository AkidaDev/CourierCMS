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
using System.Xml.Linq;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for RateWindow.xaml
    /// </summary>
    public partial class RateWindow : Window
    {
        CollectionViewSource ZoneList;
        CollectionViewSource ServiceList;
        CollectionViewSource RateList;
        bool isEdited;
        Rate rate;
        public RateWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            isEdited = false;
        }

        private void ComboBoxRate_KeyUp(object sender, KeyEventArgs e)
        {
            string rateCode = ComboBoxRate.Text;
            BillingDataDataContext db = new BillingDataDataContext();
            Assignment assign = db.Assignments.FirstOrDefault(x => x.RateCode == rateCode);
            if (assign != null)
            {
                ComboBoxRate.SelectedValue = assign.RateCode;
                ComboBoxService.SelectedValue = assign.ServiceCode;
            }
        }
        
        void ComboBoxSelectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
