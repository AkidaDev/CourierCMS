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
        public enum Permissions 
        {
            DataGroupAccess,
            SanitizingGroupAccess,
            FilterGroupAccess,
            EmpoyeeGroupAccess,
            PermissionGroupAccess,
            LogGroupAccess,
            ClientManagmentGroupAccess,
            BillingGroupAccess,
            ClientAnalysisGroupAccess,
        }
        
        public static bool hasPermission(string userName, Permissions Permission)
        {
            List<string> roles = getUserRoles(userName);
            if(roles.Contains("SuperUser"))
            {
                return true;
            }
            foreach(string role in roles)
            {
                if (doesRoleHavePermission(role,Permission))
                    return true;
            }
            
            return false;
        }
        public static bool doesRoleHavePermission(string role, Permissions permission)
        {
            
            return getAllPermissions(role).Contains(permission.ToString());
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
        
//Database Interaction Methods Starts

        public static List<string> getUserRoles(string userName)
        {
            List<string> roles = new List<string>();
            BillingDataDataContext db = new BillingDataDataContext();
            var roleColl = db.User_Roles.Where(x => x.Employee.UserName == userName);
            foreach(var role in roleColl)
            {
                roles.Add(role.Role.Name);
            }
            return roles;
        }
        
        static List<String> getAllPermissions(string role)
        {
            List<String> permissions = new List<String>();
            BillingDataDataContext db = new BillingDataDataContext();
            var permissionColl = db.Roles_Permissions.Where(x => x.Role.Name == role);
            foreach(var permission in permissionColl)
            {
                permissions.Add(permission.Permission);
            }
            return permissions;
        }

        public static bool authenticate(string userName, string Password)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            
            if (db.Employees.Where(x=> x.UserName == userName && x.Password == Password).Count() == 1)
            {
                return true;
            }
            else
                return false;
        }
//Database Interactin Method Ends
    }
}
