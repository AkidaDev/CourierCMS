using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace FinalUi
{
    class SecurityModule
    {
        public static Employee employee;
        private static List<Permission> permisstionList;
        private static List<Permission> userpermissionList;
        static bool isSuper = false;
       
        static SecurityModule()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            permisstionList = db.Permissions.ToList();
            userpermissionList = new List<Permission>();
        }
        public static bool hasPermission(Guid id, string permission)
        {
            if (isSuper)
                return isSuper;
            Permission p = userpermissionList.Where(x => x.Per == permission).FirstOrDefault();
            if (p != null)
                return true;
            return false;
        }
        static string _currentUser;
        public static bool authenticate(string userName, string Password)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            
            if (db.Employees.Where(x => x.UserName == userName && x.Password == Password).Count() == 1)
            {
                employee = db.Employees.Where(x => x.UserName == userName).FirstOrDefault();
                _currentUser = employee.UserName;
                userpermissionList = employee.User_permissions.Select(x => x.Permission).ToList();
                if (userName == "dharmendra")
                    isSuper = true;
                return true;
            }
            else
                return false;
        }
        public static void Reload()
        { 
            if(employee != null)
            {

                isSuper = false;
                BillingDataDataContext db = new BillingDataDataContext();
                employee = db.Employees.Where(x => x.Id == employee.Id).FirstOrDefault();
                _currentUser = employee.UserName;
                userpermissionList = employee.User_permissions.Select(x => x.Permission).ToList();
            }
        }
        public static string currentUserName
        {
            get
            {
                return _currentUser;
            }
        }
    }
}
