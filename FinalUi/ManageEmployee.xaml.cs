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
    /// Interaction logic for ManageEmployee.xaml
    /// </summary>
    public partial class ManageEmployee : Window
    {
        public List<Employee> employeeToEdit;
        public List<Employee> employees;
        private CollectionViewSource view;
        public ManageEmployee()
        {
            InitializeComponent();
            employeeToEdit = new List<Employee>();
            BillingDataDataContext db = new BillingDataDataContext();
            employees = (from employee in db.Employees
                         select employee).ToList();
            view = (CollectionViewSource)FindResource("EmployeeTable");
            view.Source = employees;
        }
        private void mangaEmployeegrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            employeeToEdit.Add((Employee)e.Row.DataContext);
        }
        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee window = new AddEmployee((Employee)this.mangaEmployeegrid.SelectedItem);
            window.Show();
            window.Closed += reloadgrid;
            window.Owner = this;
        }
        private void reloadgrid(object sender, EventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            this.employees = db.Employees.ToList();
            this.mangaEmployeegrid.Items.Refresh();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee window = new AddEmployee();
            window.ShowDialog();
            window.Closed +=  reloadgrid;
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
