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
        List<Permission> permission;
        List<Permission> userPermission;
        List<Permission> todelete;
        CollectionViewSource viewsourcePermission;
        CollectionViewSource viewsourceUserPermission;
        protected Employee emp;
        bool isupdate;
        public AddEmployee()
        {
            InitializeComponent();
            emp = new Employee();
            emp.Id = Guid.NewGuid();
            BillingDataDataContext db = new BillingDataDataContext();
            permission = (from per in db.Permissions select per).ToList();
            userPermission = emp.User_permissions.Select(x => x.Permission).ToList();
            viewsourcePermission = (CollectionViewSource)FindResource("Permisstion");
            viewsourcePermission.Source = permission;
            viewsourceUserPermission = (CollectionViewSource)FindResource("UserCurrentPermisstionToSet");
            viewsourceUserPermission.Source = userPermission;
            todelete = new List<Permission>();
            this.PermisstionToset.SelectedItem = null;
            this.UserPermisstionToset.SelectedItem = null;
        }

        public AddEmployee(Employee emp)
            : this()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            this.emp = emp;
            isupdate = true;
            userPermission = emp.User_permissions.Select(x => x.Permission).ToList();
            this.AddUpdateEmployee.Content = "update";
            permission.RemoveAll(x => userPermission.Select(y => y.id).Contains(x.id));
            setFieldsFromEmp();
            this.AddUpdateEmployee.Content = "update";
            viewsourceUserPermission.Source = userPermission;
            viewsourcePermission.Source = permission;
        }

        private void AddNewEmployee()
        {
            if (Password.Password == ConfirmPass.Password)
            {
                setEmpFromFields();
                BillingDataDataContext db = new BillingDataDataContext();
                db.Employees.InsertOnSubmit(emp);
                Debug.WriteLine("Type is" + UserPermisstionToset.ItemsSource.GetType().ToString());
                db.User_permissions.InsertAllOnSubmit(returnUserPermissionList((List<Permission>)(viewsourceUserPermission.Source)));
                if (Validator.EmployeeV(emp))
                {
                    db.SubmitChanges();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Password Invalid");
            }
        }
        private void updateEmployee()
        {
            setEmpFromFields();
            BillingDataDataContext db = new BillingDataDataContext();
            Employee data = db.Employees.Single(x => x.Id == emp.Id);
            removeduplicatePermission();
            db.User_permissions.InsertAllOnSubmit(returnUserPermissionList((List<Permission>)(viewsourceUserPermission.Source)));
            setDataFromEmp(data);
            if (Validator.EmployeeV(data))
            {
                db.SubmitChanges();
                this.Close();
            }
        }
        private void removeduplicatePermission()
        {
            var db = new BillingDataDataContext();
            List<User_permission> temp = (from a in db.User_permissions select a).Where(x => x.emp_id == emp.Id).ToList();
            db.User_permissions.DeleteAllOnSubmit(temp);
            db.SubmitChanges();
        }
        private void setDataFromEmp(Employee data)
        {
            data.Id = emp.Id;
            data.Name = emp.Name;
            data.UserName = emp.UserName;
            data.EMPCode = emp.EMPCode;
            data.Password = emp.Password;
            data.Gender = emp.Gender;
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
            if (!isupdate)
                emp.Id = Guid.NewGuid();
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
            if (isupdate)
                updateEmployee();
            else
                AddNewEmployee();
        }

        private void AddPermisstion_Click(object sender, RoutedEventArgs e)
        {
            if (this.PermisstionToset.SelectedItem != null)
            {
                permission.Remove((Permission)this.PermisstionToset.SelectedItem);
                userPermission.Add((Permission)this.PermisstionToset.SelectedItem);
                this.PermisstionToset.Items.Refresh();
                this.UserPermisstionToset.Items.Refresh();
            }
        }

        private void RemovePermisstion_Click(object sender, RoutedEventArgs e)
        {

            if (this.UserPermisstionToset.SelectedItem != null)
            {
                permission.Add((Permission)this.UserPermisstionToset.SelectedItem);
                userPermission.Remove((Permission)this.UserPermisstionToset.SelectedItem);
                this.PermisstionToset.Items.Refresh();
                this.UserPermisstionToset.Items.Refresh();
            }
        }
        public List<User_permission> returnUserPermissionList(List<Permission> per)
        {
            var temp = new List<User_permission>();
            foreach (Permission t in per)
            {
                var p = new User_permission();
                p.per_id = t.id;
                p.emp_id = emp.Id;
                temp.Add(p);
            }
            return temp;
        }
    }
}

