using System;
using System.Text.RegularExpressions;
namespace FinalUi
{
    public partial class InvoiceAnalyzeResult
    {
        decimal _AmountDiff;
        decimal _WeightDiff;
        public decimal AmountDiff
        {
            get
            {
                return Math.Round(_AmountDiff, 2, MidpointRounding.AwayFromZero);
            }
            set
            {
                _AmountDiff = value;
            }
        }
        public decimal WeightDif
        {
            get
            {
                return Math.Round(_WeightDiff, 3, MidpointRounding.AwayFromZero);
            }
            set
            {
                _WeightDiff = value;
            }
        }
       
    }
    
    public partial class BalanceView
    {
        public string ClientName
        {
            get
            {
                return DataSources.ClientNameFromCode(this.PC);
            }
        }
    }
    partial class Invoice
    {
        public string ClientName
        {
            get
            {
                return DataSources.ClientNameFromCode(this.ClientCode);
            }
        }
        public double totalAmount
        {
            get
            {
                
                double total = (Basic + fuelAmount + taxAmount - discountAmount) ;
                if (PreviousDue != null)
                    total = total + (double)PreviousDue;
                if (Misc != null)
                    total = total + (double)Misc;
                total = Math.Round(total);
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
                double msc = 0;
                if(Misc != null)
                    msc = (double)Misc;
                return (Basic - discountAmount + fuelAmount + msc) * 0.01 * STax;
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