using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    class DBHelper
    {
        public void insertRuntimeData(List<RuntimeData> data)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            db.RuntimeDatas.InsertAllOnSubmit(data);
            db.SubmitChanges();
        }
    }
}
