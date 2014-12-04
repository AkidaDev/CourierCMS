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
    /// Interaction logic for StockManagmentWindow.xaml
    /// </summary>
    public partial class StockManagmentWindow : Window
    {
        public StockManagmentWindow()
        {
            InitializeComponent();
            FromDatePicker.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ToDatePicker.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            RefreshDataButton_Click(null, null);
        }


        private void AddStockButton_Click(object sender, RoutedEventArgs e)
        {
            StockWindow window = new StockWindow();
            window.Show();
        }

        private void RefreshDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToDatePicker.SelectedDate == null || FromDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select dates properly");
                return;
            }
            BillingDataDataContext db = new BillingDataDataContext();
            CollectionViewSource AssignmentListSource = (CollectionViewSource)FindResource("StockAssignmentList");
            AssignmentListSource.Source = db.StockAssignmentViews.Where(x => x.AddDate >= FromDatePicker.SelectedDate && x.AddDate <= ToDatePicker.SelectedDate).ToList();
        }

        private void UpdateStockButton_Click(object sender, RoutedEventArgs e)
        {
            if(StockAssignmentDatagrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select 1 Entry to edit.", "Info");
                return;
            }
            StockAssignmentView AssignmentEntry = (StockAssignmentView)StockAssignmentDatagrid.SelectedItem;
            StockWindow window = new StockWindow(AssignmentEntry);
            window.Closed += window_Closed;
            window.Show();
        }

        void window_Closed(object sender, EventArgs e)
        {
            RefreshDataButton_Click(null, null);
        }

        private void DeleteStockButton_Click(object sender, RoutedEventArgs e)
        {
            if (StockAssignmentDatagrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select delete Entry to edit.", "Info");
                return;
            }
            if(MessageBox.Show("Are you sure want to delete this entry. This cant be undone!!","Confirm",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                StockAssignmentView AssignmentEntry = (StockAssignmentView)StockAssignmentDatagrid.SelectedItem;
                BillingDataDataContext db = new BillingDataDataContext();
                Stock selectedStock = db.Stocks.SingleOrDefault(x => x.ID == AssignmentEntry.SrlNo);
                if(selectedStock == null)
                {
                    MessageBox.Show("Selected stock doesn't exists..", "Error");
                    return;
                }

                db.Stocks.DeleteOnSubmit(selectedStock);
                db.SubmitChanges();
            }
        }
    }
}
