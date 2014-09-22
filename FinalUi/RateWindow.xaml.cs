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
        #region Variable to be nulled after ratecode changed
        List<Assignment> ToDeleteFromDatabase;
        List<Assignment> ToAddToDatabase;
        List<Assignment> ModifyFromDatabase;
        #endregion

        CollectionViewSource ZoneList;
        CollectionViewSource ServiceList;
        CollectionViewSource RateList;
        CollectionViewSource Type1DGSource;
        CollectionViewSource Type2DGSource;
        CollectionViewSource RateAssignmentDGSource;
        CollectionViewSource ClientCodeSource;
        bool isEdited;
        Rate rate;
        public RateWindow()
        {
            ToDeleteFromDatabase = new List<Assignment>();
            ToAddToDatabase = new List<Assignment>();
            ModifyFromDatabase = new List<Assignment>();
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            isEdited = false;
            #region Defining Data Sources
            RateAssignmentDGSource = (CollectionViewSource)FindResource("RateAssignmentDGSource");
            Type1DGSource = (CollectionViewSource)FindResource("Type1DGSource");
            Type2DGSource = (CollectionViewSource)FindResource("Type2DGSource");
            RateList = (CollectionViewSource)FindResource("RateCodes");
            RateList.Source = db.Rates;
            ServiceList = (CollectionViewSource)FindResource("ServiceCodes");
            ServiceList.Source = db.Services;
            ZoneList = (CollectionViewSource)FindResource("ZoneCodes");
            ZoneList.Source = db.ZONEs;
            ClientCodeSource = (CollectionViewSource)FindResource("ClientCodes");
            ClientCodeSource.Source = db.Clients;
            #endregion
            #region EventHandlers
            #endregion
        }

        void Type2DG_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            throw new NotImplementedException();
        }

        void Type1DG_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            throw new NotImplementedException();
        }
        #region ComboBoxRate Event Handlers
        private void ComboBoxRate_GeneralEventHandler()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            string rateCode = ComboBoxRate.Text;
            Rate rate = db.Rates.SingleOrDefault(x => rateCode == x.RateCode);
            if (rate != null)
            {
                List<RateDetail> rateDetails = rate.RateDetails.ToList();
                Type1DGSource.Source = rateDetails.Where(x => x.Type == 1).OrderBy(y=>y.Weight).ToList();
                Type2DGSource.Source = rateDetails.Where(x => x.Type == 2 || x.Type == 3).OrderBy(y=>y.Weight).ToList() ;
                List<Assignment> assignDetails = rate.Assignments.ToList();
                RateAssignmentDGSource.Source = assignDetails;
            }
            else
            {
                Type1DGSource.Source = new List<RateDetail>();
                Type2DGSource.Source = new List<RateDetail>();
            }

        }
        private void ComboBoxRate_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBoxRate_GeneralEventHandler();
        }


        private void ComboBoxRate_Selected(object sender, RoutedEventArgs e)
        {
            ComboBoxRate_GeneralEventHandler();
        }

        private void ComboBoxRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxRate_GeneralEventHandler();
        }
        #endregion
        public void refreshTestPrice()
        {
            char dox = 'd';
            if(TestDox.SelectedItem!= null)
            {
                if (TestDox.SelectedItem.ToString() != "Dox")
                    dox = 'n';
            }
            try
            {
                TestPrice.Text = UtilityClass.getPriceFromRateCode(ComboBoxRate.Text, Double.Parse(TestWeight.Text), dox).ToString();
            }
            catch (Exception)
            { }
            
        }
        private void TestWeight_KeyUp(object sender, KeyEventArgs e)
        {
            refreshTestPrice();
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
             
        }
        private void assignRoutine(Rate rateCode, ZONE ZoneCode, Service ServiceCode, Client ClientCode)
        { 
        }
        private void saveRateDetailsRoutine(string rateCode, List<RateDetail> details)
        {

        }
        private void SaveRateDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
