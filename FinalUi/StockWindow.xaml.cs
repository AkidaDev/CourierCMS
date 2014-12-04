using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for StockWindow.xaml
    /// </summary>
    public partial class StockWindow : Window
    {
        Stock s;
        List<Employee> emp;
        bool isUpdate;
        CollectionViewSource empView;
        public StockWindow()
        {
            InitializeComponent();
            s = new Stock();
            BillingDataDataContext db = new BillingDataDataContext();
            this.emp = db.Employees.ToList();
            empView = (CollectionViewSource)FindResource("EmpList");
            this.empView.Source = this.emp;
            this.AssignCombo.ItemsSource = this.emp;
            this.AssignCombo.SelectedItem = null;
            AddDate.SelectedDate = DateTime.Today;
            isUpdate = false;
        }
        public StockWindow(StockAssignmentView StockAssignment):this()
        {
           s = new Stock();
            s.ID = StockAssignment.SrlNo;
            s.StockEnd = StockAssignment.EndNumber;
            s.StockStart = StockAssignment.StartNumber;
            Employee selectedEmp = emp.SingleOrDefault(x => x.UserName == StockAssignment.EmployeeName);
            s.UserId = selectedEmp.Id;
            s.Employee = selectedEmp;
            s.BookNo = StockAssignment.BookNo;
            s.cost = StockAssignment.CostPerSlip;
            s.Date = StockAssignment.AddDate;
            s.desc = StockAssignment.Description;
            isUpdate = true;
            filldetails(s);
            
        }
        void filldetails(Stock s)
        {
            this.FromBox.Text = s.StockStart;
            this.ToBox.Text = s.StockEnd;
            this.CostBox.Text = s.cost.ToString();
            this.DescriptionBox.Text = s.desc;
            AssignCombo.SelectedItem = s.Employee;
            BookNumber.Text = s.BookNo.ToString();
            AddDate.SelectedDate = s.Date;
            Add_Filter.Text = "Update Stock";
        }
        bool getdetails()
        {
            var e = new Employee();
            this.s.StockStart = this.FromBox.Text;
            this.s.StockEnd = this.ToBox.Text;
            this.s.desc = this.DescriptionBox.Text;
            if (this.AssignCombo.SelectedItem != null)
            {
                e = (Employee)this.AssignCombo.SelectedItem;
            }
            else
            {
                MessageBox.Show("Select an Employee");
                return false;
            }
            this.s.UserId = e.Id;
            s.BookNo = BookNumber.Text;
            if(AddDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a date.");
                return false;
            }
            else
            {
                s.Date = AddDate.SelectedDate;
            }
            try
            {
                this.s.cost = float.Parse(this.CostBox.Text);
            }
            catch (Exception) { MessageBox.Show("Cost should be number"); return false; }

            return true;
        }
        private void AddstockLabel_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            if (getdetails())
            {
                BillingDataDataContext db = new BillingDataDataContext();
                if (!isUpdate)
                {
                    db.Stocks.InsertOnSubmit(this.s);
                    try
                    {
                        try
                        {
                            db.SubmitChanges();
                            flag = true;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); flag = false; return; }

                    }
                    catch (Exception d) { flag = false; MessageBox.Show(d.Message); return; }
                    if (flag)
                    {
                        MessageBox.Show("Stock added","Info");
                        this.Close();
                    }
                }
                else
                {
                    Stock stockEntry = db.Stocks.SingleOrDefault(x => x.ID == s.ID);
                    if(stockEntry == null)
                    {
                        MessageBox.Show("This stock entry doesn't exists","Error");
                        return;
                    }
                    else
                    {
                        stockEntry.BookNo = s.BookNo;
                        stockEntry.cost = s.cost;
                        stockEntry.Date = s.Date;
                        stockEntry.desc = s.desc;
                        stockEntry.StockEnd = s.StockEnd;
                        stockEntry.StockStart = s.StockStart;
                        stockEntry.UserId = s.UserId;
                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("Stock successfully updated");
                            this.Close();
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Cannot update stock entry. Error: " + ex.Message);
                        }
                    }
                }
            }
        }
        public class ListViewConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                DateTime date = (DateTime)value;
                return date.ToShortDateString();
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string strValue = value as string;
                DateTime resultDateTime;
                if (DateTime.TryParse(strValue, out resultDateTime))
                {
                    return resultDateTime;
                }
                return DependencyProperty.UnsetValue;
            }
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