namespace FinalUi
{
    partial class BillingDataDataContext
    {
        
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
            if (msg != "")
            {
                throw new System.Exception(msg);
            }
            this.ExecuteDynamicInsert(instance);
        }
        partial void UpdateEmployee(Employee instance)
        {
            string msg = "";
            if (instance.Password == "" || instance.Password == null || instance.Password.Length <6)
                msg += "Password should be at least six Characters \n";
            if (instance.UserName == "" || instance.UserName == null || instance.UserName.Length < 4)
                msg += "UserName should be at least 4 Characters \n";
            if (instance.Name == "" || instance.Name == null)
                msg += "Name can not Be Empty \n";
            if (instance.EMPCode == "" || instance.EMPCode == null || instance.EMPCode.Length < 3)
                msg += "Employee Code should be at least 3 characters \n";
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