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
            Add_Filter.Text = " Update Stock"; 
            this.AddFilter.Data = Geometry.Parse(@"F1M2,12.942C2,12.942 10.226,15.241 10.226,15.241 10.226,15.241 8.275,17.071 8.275,17.071 9.288,17.922 10.917,18.786 12.32,18.786 15.074,18.786 17.386,16.824 18.039,14.171 18.039,14.171 21.987,15.222 21.987,15.222 20.891,19.693 16.996,23 12.357,23 9.771,23 7.076,21.618 5.308,19.934 5.308,19.934 3.454,21.671 3.454,21.671 3.454,21.671 2,12.942 2,12.942z M11.643,2C14.229,2 16.924,3.382 18.692,5.066 18.692,5.066 20.546,3.329 20.546,3.329 20.546,3.329 22,12.058 22,12.058 22,12.058 13.774,9.759 13.774,9.759 13.774,9.759 15.725,7.929 15.725,7.929 14.712,7.078 13.083,6.214 11.68,6.214 8.926,6.214 6.614,8.176 5.961,10.829 5.961,10.829 2.013,9.778 2.013,9.778 3.109,5.307 7.004,2 11.643,2z");
            this.AddFilter.Width=18;
            this.AddFilter.Height = 18;
            this.AddFilter.Stretch = Stretch.Fill;
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
                    if(db.Stocks.Where(x=>x.BookNo == s.BookNo).Count() > 0)
                    {
                        MessageBox.Show("This book is already entered..","Error");
                        return;
                    }
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