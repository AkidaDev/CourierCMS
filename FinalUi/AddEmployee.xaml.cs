using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        public AddEmployee()
        {
            InitializeComponent();
        }
        
        public AddEmployee(Employee emp)
        {
            InitializeComponent();
            setFieldsFromEmp(emp);
        }
        public void setFieldsFromEmp(Employee emp)
        {

            FullName.Text = emp.Name;
            UserName.Text = emp.UserName;
            Password.Password = "";
            if (emp.Gender == 'M')
            {
                Gender.SelectedIndex = 0;
            }
            else
                Gender.SelectedIndex = 1;
            EmployeeCode.Text = emp.EMPCode;
            ConfirmPass.Password = "";
        }
        public void setEmpFromFields(Employee emp)
        {

            emp.Name = FullName.Text;
            emp.Password = Password.Password;
            if ((Gender.SelectedIndex == 0))
            {

                emp.Gender = 'M';
            }
            else
            {
                emp.Gender = 'F';
                Debug.WriteLine(Gender.SelectedValue.ToString());
            }
            emp.EMPCode = EmployeeCode.Text;
            emp.UserName = UserName.Text;
        }

        private void CreateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password == ConfirmPass.Password)
            {
                Employee emp = new Employee();
                emp.Id = Guid.NewGuid();
                setEmpFromFields(emp);
                addEmployeeINDatabase(emp);
                this.Close();
            }
            else
            {
                MessageBox.Show("Password Invalid");
            }
        }
        bool addEmployeeINDatabase(Employee emp)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            db.Employees.InsertOnSubmit(emp);
            db.SubmitChanges();
            return true;
        }
    }
}
