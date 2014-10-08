namespace FinalUi
{
    partial class BillingDataDataContext
    {
        public BillingDataDataContext() :
            base(LoadResources.getConString(), mappingSource)
        {
            OnCreated();
        }
        partial void DeleteAssignment(Assignment instance)
        {
            throw new System.NotImplementedException();
        }
        partial void InsertRuntimeData(RuntimeData instance)
        {
            string msg = "";
        }
        partial void UpdateRuntimeData(RuntimeData instance)
        {
            string msg = "";
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
        }
        partial void InsertStock(Stock instance)
        {
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
        }
        partial void InsertCity(City instance)
        {
            throw new System.NotImplementedException();
        }
        partial void UpdateRuntimeData(RuntimeData instance)
        {
        }
        partial void InsertZONE(ZONE instance)
        {
        }
    }
}
