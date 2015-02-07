using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
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
    /// Interaction logic for ExpenseEntry.xaml
    /// </summary>
    public partial class ExpenseEntry : Window
    {
        ObservableCollection<Expense> DataSourceCollection;
        CollectionViewSource ExpenseTypesSource;
        List<String> ExpenseTypes;
        BillingDataDataContext db;
        public ExpenseEntry()
        {
            InitializeComponent();
            SelectedDate.SelectedDate = DateTime.Today;
            db = new BillingDataDataContext();
            refreshData();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SubmitChanges();
                MessageBox.Show("Data saved..", "Information");
                TitleTextBox.Text = " Manage Expense";
            }
            catch (Exception exe)
            {
                MessageBox.Show("Unable to add data... Error: " + exe.Message, "Error");
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<Expense> expenseListToDelete = ExpenseDataGrid.SelectedItems.Cast<Expense>().ToList();
            if (expenseListToDelete != null)
            {
                if (expenseListToDelete.Count == 0)
                {
                    MessageBox.Show("Please select atleast one row to delete..", "Error");
                    return;
                }
                if (MessageBoxResult.Yes == MessageBox.Show("Are you sure you want to delete these " + expenseListToDelete.Count + " rows?", "Confirm", MessageBoxButton.YesNo))
                {
                    db.Expenses.DeleteAllOnSubmit(expenseListToDelete);
                    db.SubmitChanges();
                    refreshData();
                }
            }
        }
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text == " Manage Expense *")
            {
                if (MessageBoxResult.No == MessageBox.Show("Changes will be lost..", "Continue", MessageBoxButton.YesNo))
                    return;
            }
            refreshData();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void ExpenseDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TitleTextBox.Text = " Manage Expense *";
        }
        void refreshData()
        {
            TitleTextBox.Text = " Manage Expense";
            ExpenseTypeComboBox.Text = "";
            AmountBox.Text = "";
            RemarkTextBox.Text = "";
            ExpenseTypes = db.Expenses.Select(x => x.ExpenseType).Distinct().ToList();
            ExpenseTypesSource = (CollectionViewSource)FindResource("ExpenseTypesDataContext");
            ExpenseTypesSource.Source = ExpenseTypes;
            DataSourceCollection = new ObservableCollection<Expense>(db.Expenses.Where(x => x.Date == SelectedDate.SelectedDate).ToList());
            ExpenseDataGrid.DataContext = DataSourceCollection;
            ExpenseDataGrid.IsReadOnly = false;
            ExpenseDataGrid.CanUserDeleteRows = false;
            ExpenseDataGrid.CanUserAddRows = false;
        }
        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            Expense expense = new Expense();
            decimal amount;
            if (!decimal.TryParse(AmountBox.Text, out amount))
            {
                MessageBox.Show("Please enter amount correctly...", "Error");
                return;
            }
            if (ExpenseTypeComboBox.Text == "")
            {
                MessageBox.Show("Please enter an expense type...");
                return;
            }
            int id = Convert.ToInt32(db.ExecuteQuery<decimal>("SELECT IDENT_CURRENT('Expense') +1;").FirstOrDefault());
            expense.Id = id;
            expense.Amount = amount;
            expense.Remarks = RemarkTextBox.Text;
            expense.Date = SelectedDate.SelectedDate ?? DateTime.Today;
            expense.ExpenseType = ExpenseTypeComboBox.Text;
            db.Expenses.InsertOnSubmit(expense);
            db.SubmitChanges();
            MessageBox.Show("Data Added...", "Information");
            TitleTextBox.Text = " Manage Expense";
            refreshData();

        }
    }
}
