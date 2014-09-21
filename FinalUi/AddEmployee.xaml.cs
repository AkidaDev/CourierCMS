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
        List<Roles_Permission> permission;
        List<Roles_Permission> userPermission;
        protected Employee emp;
        public AddEmployee()
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            permission = (from per in db.Roles_Permissions select per).ToList();
        }
        
        public AddEmployee(Employee emp): this()
        {
            this.AddUpdateEmployee.Content = "update";
            BillingDataDataContext db = new BillingDataDataContext();
            permission = (from Rpermissions in db.Roles_Permissions
                          select Rpermissions).ToList();
            setFieldsFromEmp();
        }

        private void AddNewEmployee()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            db.Employees.InsertOnSubmit(emp);
            db.SubmitChanges();
        }
        private void updateEmployee()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            var data = db.Employees.Single(x => x.Id == emp.Id);
            db.SubmitChanges();
        }

        public string PermisstionsToString(List<Roles_Permission> per)
        {
            return String.Join(",",per.Select(x => x.Id.ToString()).ToArray());
        }
        public void setDataFromEmp(Employee data)
        {
            data.Id = emp.Id;
            data.Name = emp.Name;
            data.EMPCode = emp.EMPCode;
            data.Password = emp.Password;
            data.permisstions = emp.permisstions;
        }
        public void setFieldsFromEmp()
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
        public void setEmpFromFields()
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
                setEmpFromFields();
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
