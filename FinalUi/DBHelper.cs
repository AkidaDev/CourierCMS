using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    class DBHelper
    {
        public void insertRuntimeData(List<RuntimeData> data, int sheetNo)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            List<RuntimeMeta> MetaData = new List<RuntimeMeta>();
            foreach(var runData in data)
            {
                RuntimeMeta metaData = new RuntimeMeta();
                metaData.Id = Guid.NewGuid();
                metaData.RuntimeDataID = runData.Id;
                metaData.SheetNo = sheetNo;
                metaData.UserName = SecurityModule.currentUserName;
                MetaData.Add(metaData);
            }
            db.RuntimeDatas.InsertAllOnSubmit(data);
            db.RuntimeMetas.InsertAllOnSubmit(MetaData);
            db.SubmitChanges();
        }
        public void deleteRuntimeData(int sheetNo)
        {
            try
            {
                BillingDataDataContext db = new BillingDataDataContext();
                IQueryable<RuntimeMeta> metaDataList = db.RuntimeMetas.Where(x => x.UserName == SecurityModule.currentUserName && x.SheetNo == sheetNo);
                IQueryable<RuntimeData> data = metaDataList.Select(x => x.RuntimeData);
                db.RuntimeMetas.DeleteAllOnSubmit(metaDataList);
                db.RuntimeDatas.DeleteAllOnSubmit(data);
                db.SubmitChanges();
            }
            catch(ChangeConflictException e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
