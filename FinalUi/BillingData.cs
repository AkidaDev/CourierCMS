using System.Text.RegularExpressions;
namespace FinalUi
{
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
        partial void UpdateClient(Client instance)
        {
            string msg = "";
            if (instance.CLCODE == "" || instance.CLCODE == null || instance.CLCODE.Length < 3)
                msg += "Client Code must be at least three charaters \n";
            if (instance.CONTACTNO == "" || instance.CONTACTNO == null || instance.CONTACTNO.Length < 8)
                msg += "Enter correct contact number \n";
            if (instance.CLNAME == "" || instance.CLNAME == null)
                msg += "Client name cannot be empty";
            if (instance.ADDRESS == "" || instance.ADDRESS == null)
                msg += "Client address cannot be empty";
            if (msg != "")
            {
                throw new System.Exception(msg);
            }
            this.ExecuteDynamicUpdate(instance);
        }
        partial void InsertEmployee(Employee instance)
        {
            string msg = "";
            if (instance.Password == "" || instance.Password == null || instance.Password.Length < 6)
                msg += "Password should be at least six Characters \n";
            if (instance.UserName == "" || instance.UserName == null || instance.UserName.Length < 4)
                msg += "UserName should be at least 4 Characters \n";
            if (instance.Name == "" || instance.Name == null)
                msg += "Name can not Be Empty \n";
            if (instance.EMPCode == "" || instance.EMPCode == null || instance.EMPCode.Length < 3)
                msg += "Employee Code should be at least 3 characters \n";
            
            BillingDataDataContext db = new BillingDataDataContext();
            DataSources.refreshEmployeeList();
            if ((DataSources.EmployeeCopy.FindAll(x => x.EMPCode == instance.EMPCode || x.UserName == instance.UserName)).Count > 0)
                msg += "Username and employee code must be unique";
            if (msg != "")
            {
                throw new System.Exception(msg);
            }
            this.ExecuteDynamicInsert(instance);
        }
        partial void UpdateEmployee(Employee instance)
        {
            string msg = "";
            if (instance.Password == "" || instance.Password == null || instance.Password.Length < 6)
                msg += "Password should be at least six Characters \n";
            if (instance.UserName == "" || instance.UserName == null || instance.UserName.Length < 4)
                msg += "UserName should be at least 4 Characters \n";
            if (instance.Name == "" || instance.Name == null)
                msg += "Name can not Be Empty \n";
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