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
    /// Interaction logic for addRole.xaml
    /// </summary>
    public partial class AddRole : Window
    {
        public AddRole()
        {
          //  DbHelper DbHelper = new DbHelper();
            var permissionList = Enum.GetNames(typeof(SecurityModule.Permissions));
            InitializeComponent();
            foreach (var permission in permissionList)
            {
                WrapPanel panel = new WrapPanel();
                Label permissionLabel = new Label();
                permissionLabel.Content = permission;
                CheckBox permissionCheckbox = new CheckBox();
                permissionCheckbox.Name = permission;
                panel.Children.Add(permissionLabel);
                panel.Children.Add(permissionCheckbox);
                PermissionPanel.Children.Add(panel);
                RegisterName(permission, permissionCheckbox);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            Role role = new Role();
            Guid roleId = Guid.NewGuid();
            role.Id = roleId;
            role.Name = RoleName.Text;
            db.Roles.InsertOnSubmit(role);

            foreach(var permission in Enum.GetNames(typeof(SecurityModule.Permissions)))
            {
                CheckBox check = (CheckBox)PermissionPanel.FindName(permission);
                if(check.IsChecked == true)
                {
                    Roles_Permission rp = new Roles_Permission();
                    rp.Id = Guid.NewGuid();
                    rp.Role_Id = roleId;
                    rp.Permission = permission;
                    db.Roles_Permissions.InsertOnSubmit(rp);
                }
            }
            db.SubmitChanges();
            this.Close();
        }

    }
}
