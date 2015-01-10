using System;
using System.Text.RegularExpressions;
namespace FinalUi
{
    partial class Invoice
    {
        public double totalAmount
        {
            get
            {
                
                double total = Math.Round(Basic + fuelAmount + taxAmount - discountAmount + PreviousDue??0 + Misc??0) ;
                return total;
            }
        }
        public double fuelAmount
        {
            get
            {
                return (Basic - discountAmount) * 0.01 * Fuel;
            }
        }
        public double taxAmount
        {
            get
            {
                return (Basic - discountAmount + fuelAmount) * 0.01 * STax;
            }
        }
        public double discountAmount
        {
            get
            {
                return Basic * 0.01 * Discount??0;
            }
        }
    }
    partial class RuntimeCityView
    {
        public string SubClient
        {
            get;
            set;
        }
    }
    partial class StockAssignmentView
    {
        public int? AssignedCount
        {
            get
            {
                string startNo = Regex.Match(StartNumber, @"\d+").Value;
                string endNo = Regex.Match(EndNumber, @"\d+").Value;
                int iStartNo, iEndNo;
                if (int.TryParse(startNo, out iStartNo) && int.TryParse(endNo, out iEndNo))
                    return iEndNo - iStartNo + 1;
                else
                    return -1;

            }
        }
        public int SlipsRemaining
        {
            get
            {
                return (AssignedCount??0) - (int)SlipsUsed;
            }
        }
    }
    partial class BillingDataDataContext
    {
        public BillingDataDataContext() :
            base(LoadResources.getConString(), mappingSource)
        {
            OnCreated();
        }
        string verifyClient(Client instance)
        {
            string msg = "";
            if (instance.CLCODE == "" || instance.CLCODE == null || instance.CLCODE.Length < 3 || instance.CLCODE.Length > 5)
                msg += "Client Code must be between 3 - 5 charaters \n";
            if (instance.CONTACTNO == "" || instance.CONTACTNO == null || instance.CONTACTNO.Length < 8 || instance.CONTACTNO.Length > 50)
                msg += "Contact number must be between 8 to 50 characters \n";
            if (instance.CLNAME == "" || instance.CLNAME == null)
                msg += "Client name cannot be empty \n";
            if ((instance.CLNAME ?? "").Length > 50)
                msg += "Client name must be less than 50 characters \n";
            if ((instance.EMAILID ?? "").Length > 50)
                msg += "Email address must be less than 50 characters \n";
            if (instance.ADDRESS == "" || instance.ADDRESS == null)
                msg += "Client address cannot be empty \n";
            if ((instance.ADDRESS ?? "").Length > 100)
                msg += "Address must be less than 100 characters \n";
            
            return msg;
        }
        partial void UpdateClient(Client instance)
        {
            string msg = "";
            msg = verifyClient(instance);
            if (msg != "")
            {
                throw new System.Exception(msg);
            }
            this.ExecuteDynamicUpdate(instance);
        }
        partial void InsertClient(Client instance)
        {
            string msg = "";
            msg = verifyClient(instance);
            if (DataSources.ClientCopy.FindAll(x => x.CLCODE == instance.CLCODE).Count > 0)
                msg += "A code with this client already exists + \n";
            if (msg != "")
                throw new System.Exception(msg);
            this.ExecuteDynamicInsert(instance);
        }
        string verifyEmployee(Employee instance)
        {
            string msg = "";
            if (instance.Password == "" || instance.Password == null || instance.Password.Length < 6)
                msg += "Password should be at least six Characters \n";
            if (instance.UserName == "" || instance.UserName == null)
                msg += "UserName must be present \n";
            if ((instance.UserName ?? "").Length > 50)
                msg += "Username lenght should be less than 50 characters \n";
            if (instance.Name == "" || instance.Name == null)
                msg += "Name can not Be Empty \n";
            if (instance.EMPCode == "" || instance.EMPCode == null || instance.EMPCode.Length < 3 || instance.EMPCode.Length > 50)
                msg += "Employee Code should be between  characters \n";
            if (instance.UserName.Length > 50)
                msg += "Username should be lesser than 50 characters \n";
            if (instance.EMPCode.Length > 50)
                msg += "Employee code should be less than 50 characters \n";
            if ((instance.Address ?? "").Length > 150)
                msg += "Address should be less than 150 characters \n";
            if ((instance.ContactNo ?? "").Length > 50)
                msg += "Contacts should be atmost 50 characters \n";
            if ((instance.Other ?? "").Length > 50)
                msg += "Other details should be less than 50 characters \n";
            
            return msg;
        }
        partial void InsertEmployee(Employee instance)
        {
            string msg = verifyEmployee(instance);
            DataSources.refreshEmployeeList();
            if ((DataSources.EmployeeCopy.FindAll(x => x.EMPCode == instance.EMPCode || x.UserName == instance.UserName)).Count > 0)
                msg += "Username and employee code must be unique \n";
            if (msg != "")
            {
                throw new System.Exception(msg);
            }
            this.ExecuteDynamicInsert(instance);
        }
        partial void UpdateEmployee(Employee instance)
        {
            string msg = verifyEmployee(instance);
            if (msg != "")
            {
                throw new System.Exception(msg);
            }
            this.ExecuteDynamicUpdate(instance);
        }
        partial void InsertCity(FinalUi.City instance)
        {
            this.ExecuteDynamicInsert(instance);
        }
    }
}