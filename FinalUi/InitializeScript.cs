using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    class InitializeScript
    {
        public string intializeDatabase(string userName, string passWord, string dataSource)
        {
            try
            {

                System.Data.Common.DbConnectionStringBuilder stringBuilder = new System.Data.Common.DbConnectionStringBuilder();
                string insName = @"\" + dataSource;
                if (insName != "")
                {
                    stringBuilder.Add("Data Source", Environment.MachineName + insName);
                }
                else
                    stringBuilder.Add("Data Source", Environment.MachineName);
                stringBuilder.Add("User ID", userName);
                stringBuilder.Add("Password", passWord);
                Configs.Default.ConnString = stringBuilder.ConnectionString;
                Configs.Default.Save();
                BillingDataDataContext db = new BillingDataDataContext();
                if (db.DatabaseExists())
                {
                    db.DeleteDatabase();
                }
                db.CreateDatabase();
                Employee emp = new Employee();
                emp.Name = "Dharmendra";
                Guid empId = Guid.NewGuid();
                emp.Id = empId;
                emp.UserName = "dharmendra";
                emp.Gender = 'M';
                emp.EMPCode = "DMD";
                emp.Password = "pass";
                db.Employees.InsertOnSubmit(emp);
                Role role = new Role();
                role.Name = "SuperUser";
                Guid roleId = new Guid();
                role.Id = roleId;
                db.Roles.InsertOnSubmit(role);
                User_Role user_role = new User_Role();
                user_role.Id = Guid.NewGuid();
                user_role.EmployeeId = empId;
                user_role.RoleId = roleId;
                db.User_Roles.InsertOnSubmit(user_role);
                for (int i = 0; i < 5; i++)
                {
                    Client client = new Client();
                    client.CLNAME = "Client" + i.ToString();
                    client.ADDRESS = "Address" + i.ToString();
                    client.EMAILID = "Email" + i.ToString();
                    client.CLCODE = "CLT" + i.ToString();
                    db.Clients.InsertOnSubmit(client);
                }
                    db.SubmitChanges();
                stringBuilder.Add("Initial Catalog", "BillingDatabase");
                Configs.Default.ConnString = stringBuilder.ConnectionString;
                Configs.Default.Save();

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
