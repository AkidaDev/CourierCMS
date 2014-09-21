﻿using System;
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
            return roles;
        }
        
        static List<String> getAllPermissions(string role)
        {
            List<String> permissions = new List<String>();
            BillingDataDataContext db = new BillingDataDataContext();
            return permissions;
        }
        static string _currentUser;
        public static bool authenticate(string userName, string Password)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            
            if (db.Employees.Where(x=> x.UserName == userName && x.Password == Password).Count() == 1)
            {
                _currentUser = userName;
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
//Database Interactin Method Ends
    }
}
