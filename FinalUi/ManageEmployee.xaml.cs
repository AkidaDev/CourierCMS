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
        public ManageEmployee()
        {
            InitializeComponent();
            employeeToEdit = new List<Employee>();
            BillingDataDataContext db = new BillingDataDataContext();
            employees = (from employee in db.Employees
                         select employee).ToList();
            CollectionViewSource view = (CollectionViewSource)FindResource("EmployeeTable");
            view.Source = employees;
        }

        private void mangaEmployeegrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            employeeToEdit.Add((Employee)e.Row.DataContext);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();

            foreach (var employee in employeeToEdit)
            {
                var data = db.Employees.Single(x => x.Id == employee.Id);
                data.Name = employee.Name;
                data.Gender = employee.Gender;
                db.SubmitChanges();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee window = new AddEmployee();
            window.Show();
        }

        private void mangaEmployeegrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

        }
    }
}
