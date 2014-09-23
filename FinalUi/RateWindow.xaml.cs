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
        CollectionViewSource Type1DGSource;
        CollectionViewSource Type2DGSource;
        CollectionViewSource RateAssignmentDGSource;
        CollectionViewSource ClientCodeSource;
        public RateWindow()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();

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
            ComboBoxRate_GeneralEventHandler();
        }
        public void refreshDataSources()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            RateList.Source = db.Rates;
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
                Type1DGSource.Source = rateDetails.Where(x => x.Type == 1).OrderBy(y => y.Weight).ToList();
                Type2DGSource.Source = rateDetails.Where(x => x.Type == 2 || x.Type == 3).OrderBy(y => y.Weight).ToList();
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
            if (TestDox.SelectedItem != null)
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
        private void Type1DG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid sourceGrid = (DataGrid)e.Source;
            RateDetail detObj = (RateDetail)sourceGrid.SelectedItem;
            EditRateDetailsWindow editWin = new EditRateDetailsWindow(detObj);
            editWin.Show();
        }
        private void newDetObj(int type)
        {
            RateDetail retD = new RateDetail();
            retD.ID = Guid.NewGuid();
            retD.Type = type;
            retD.RateCode = ComboBoxRate.Text;
            ListCollectionView view;
            if (type == 1)
                view = (ListCollectionView)Type1DG.ItemsSource;
            else
                view = (ListCollectionView)Type2DG.ItemsSource;
            view.AddNewItem(retD);
            EditRateDetailsWindow win = new EditRateDetailsWindow(retD);
            win.Closed += win_Closed;
            win.Show();
        }
        void win_Closed(object sender, EventArgs e)
        {
            EditRateDetailsWindow win = (EditRateDetailsWindow)sender;
            if (!win.isRateAdded)
            {
                ListCollectionView view;
                if (win.retD.Type == 1)
                    view = (ListCollectionView)Type1DG.ItemsSource;
                else
                    view = (ListCollectionView)Type2DG.ItemsSource;
                view.Remove(win.retD);
            }
            else
            {
                BillingDataDataContext db = new BillingDataDataContext();
                db.RateDetails.InsertOnSubmit(win.retD);
                db.SubmitChanges();
            }
        }
        private void Type1DGNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            newDetObj(1);
        }

        private void Type2DGDeleteSelectedRowButton_Click(object sender, RoutedEventArgs e)
        {
            RateDetail retD = (RateDetail)Type2DG.SelectedItem;
            ListCollectionView view = (ListCollectionView)Type2DG.ItemsSource;
            view.Remove(retD);
            removeDGItemFromDB(retD);
        }
        private void removeDGItemFromDB(RateDetail retD)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            retD = db.RateDetails.SingleOrDefault(x => x.ID == retD.ID);
            if (retD != null)
            {
                db.RateDetails.DeleteOnSubmit(retD);
                db.SubmitChanges();
            }
        }
        private void Type2DGNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            newDetObj(2);
        }

        private void Type1DGDeleteSelectedRowButton_Click(object sender, RoutedEventArgs e)
        {
            RateDetail retD = (RateDetail)Type1DG.SelectedItem;
            ListCollectionView view = (ListCollectionView)Type1DG.ItemsSource;
            view.Remove(retD);
            removeDGItemFromDB(retD);
        }


        private void AssignRateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddNewRateButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewRateWindow addRateWindow = new AddNewRateWindow();
            addRateWindow.Closed += addRateWindow_Closed;
            addRateWindow.Show();
        }

        void addRateWindow_Closed(object sender, EventArgs e)
        {
            AddNewRateWindow addNewRateWindow = (AddNewRateWindow)sender;
            if (addNewRateWindow.isEntered)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                db.Rates.InsertOnSubmit(addNewRateWindow.rate);
                db.SubmitChanges();
                refreshDataSources();
            }
        }

        private void AddNewAssignmentButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
