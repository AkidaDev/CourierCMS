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
        private static List<Permission> permisstionList;
        private static List<Permission> userpermissionList;
        static SecurityModule()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            permisstionList = db.Permissions.ToList();
            userpermissionList = new List<Permission>();
        }
        public static bool hasPermission(Guid id, string permission)
        {
            var p = userpermissionList.Where(x => x.Per == permission);
            if (p != null)
                return true;
            return false;
        }
        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        static string _currentUser;
        public static bool authenticate(string userName, string Password)
        {
            BillingDataDataContext db = new BillingDataDataContext();

            if (db.Employees.Where(x => x.UserName == userName && x.Password == Password).Count() == 1)
            {
                _currentUser = userName;
                var emp = db.Employees.Where(x => x.UserName == userName).FirstOrDefault();
                userpermissionList = emp.User_permissions.Select(x => x.Permission).ToList();
                return true;
            }
            else
                return false;
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
