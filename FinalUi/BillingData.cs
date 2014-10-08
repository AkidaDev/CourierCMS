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
        partial void InsertEmployee(Employee instance)
        {
            string msg = "";
            if (instance.Password == "" && instance.Password == null)
                msg += "Password can not Be Empty";
            if (instance.UserName == "" && instance.UserName  == null)
                msg +="UserName can not Be Empty";
            if (instance.Name == "" && instance.Name  == null)
                msg +="Name can not Be Empty";
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
            if (instance.Password == "" && instance.Password == null)
                msg += "Password can not Be Empty";
            if (instance.UserName == "" && instance.UserName == null)
                msg += "UserName can not Be Empty";
            if (instance.Name == "" && instance.Name == null)
                msg += "Name can not Be Empty";
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
