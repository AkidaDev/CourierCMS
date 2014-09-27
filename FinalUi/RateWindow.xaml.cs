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
            editWin.ShowDialog();
        }
        private void newDetObj(int type)
        {
            RateDetail retD = new RateDetail();
            retD.ID = Guid.NewGuid();
            retD.Type = type;
            retD.RateCode = ComboBoxRate.Text;
            EditRateDetailsWindow win = new EditRateDetailsWindow(retD);
            win.Closed += win_Closed;
            win.ShowDialog();
        }
        void win_Closed(object sender, EventArgs e)
        {
            EditRateDetailsWindow win = (EditRateDetailsWindow)sender;
            if (win.isRateAdded)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                db.RateDetails.InsertOnSubmit(win.retD);
                db.SubmitChanges();
                if (win.retD.Type == 1)
                    ((ListCollectionView)Type1DG.ItemsSource).AddNewItem(win.retD);
                else
                    ((ListCollectionView)Type2DG.ItemsSource).AddNewItem(win.retD);
            }
        }
        private void Type1DGNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            newDetObj(1);
        }

        private void Type2DGDeleteSelectedRowButton_Click(object sender, RoutedEventArgs e)
        {
            Type2DG.CancelEdit();
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
            addRateWindow.ShowDialog();
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
        private void editAssignDetails(Assignment assign)
        {
            RateAssignment assignWin = new RateAssignment(assign);
            assignWin.Closed += assignWin_Closed;
            assignWin.ShowDialog();
        }

        void assignWin_Closed(object sender, EventArgs e)
        {
            RateAssignment window = (RateAssignment)sender;
            if (window.isEdited)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                Assignment bdAssign = db.Assignments.SingleOrDefault(x => x.Id == window.assignment.Id);
                bool isNew = false;
                if (bdAssign == null)
                {
                    bdAssign = new Assignment();
                    bdAssign.Id = Guid.NewGuid();
                    isNew = true;
                }
                bdAssign.RateCode = window.assignment.RateCode;
                bdAssign.ServiceCode = window.assignment.ServiceCode;
                bdAssign.ClientCode = window.assignment.ClientCode;
                bdAssign.ZoneCode = window.assignment.ZoneCode;
                if (isNew)
                {
                    db.Assignments.InsertOnSubmit(bdAssign);
                    ((ListCollectionView)DGRateAssignment.ItemsSource).AddNewItem(bdAssign);
                }
                db.SubmitChanges();
            }
        }
        private void AddNewAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            Assignment assignment = new Assignment();
            assignment.Id = Guid.NewGuid();
            assignment.RateCode = ComboBoxRate.Text;
            editAssignDetails(assignment);
        }

        private void DGRateAssignment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Assignment assignment = (Assignment)DGRateAssignment.SelectedItem;
            editAssignDetails(assignment);
        }

        private void DeleteAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            Assignment assignment = (Assignment)DGRateAssignment.SelectedItem;
            BillingDataDataContext db = new BillingDataDataContext();
            Assignment dbAssignment = db.Assignments.Single(x => x.Id == assignment.Id);
            db.Assignments.DeleteOnSubmit(dbAssignment);
            ListCollectionView view = (ListCollectionView)DGRateAssignment.ItemsSource;
            view.Remove(assignment);
            db.SubmitChanges();
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
