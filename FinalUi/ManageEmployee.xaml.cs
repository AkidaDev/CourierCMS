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
        private CollectionViewSource employeeview;
        public ManageEmployee()
        {
            InitializeComponent();
            employeeToEdit = new List<Employee>();
            BillingDataDataContext db = new BillingDataDataContext();
            employees = DataSources.EmployeeCopy.ToList();
            employeeview = (CollectionViewSource)FindResource("EmployeeTable");
            employeeview.Source = employees;
        }
        private void mangaEmployeegrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            employeeToEdit.Add((Employee)e.Row.DataContext);
        }
        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if ((Employee)this.mangaEmployeegrid.SelectedItem != null)
            {
                AddEmployee window = new AddEmployee((Employee)this.mangaEmployeegrid.SelectedItem);
                window.Closed += reloadgrid;
                window.Show();
                window.Owner = this;
            }
            else {
                MessageBox.Show("No Employees selected");
            }
        }
        private void reloadgrid(object sender, EventArgs e)
        {
            DataSources.refreshEmployeeList();
            this.employees = DataSources.EmployeeCopy.ToList();
            employeeview.Source = this.employees;
            this.mangaEmployeegrid.Items.Refresh();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee window = new AddEmployee();
            window.Closed += reloadgrid;
            window.ShowDialog();
        }
        private void DeleteEmployee_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if (mangaEmployeegrid.SelectedItem != null)
            {
                var emp = db.Employees.Where(x => x.Id == ((Employee)mangaEmployeegrid.SelectedItem).Id).FirstOrDefault();
                emp.Status = 'D';
                try
                {
                    db.SubmitChanges();
                    reloadgrid(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }
    }
}
