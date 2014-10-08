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
        public void insertRuntimeData(List<RuntimeData> data, int sheetNo, bool isLoadedFromFile, DateTime? toDate = null, DateTime? fromDate = null)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            if(isLoadedFromFile == false)
            {
                db.sp_LoadToRuntimeFromDate(SecurityModule.currentUserName, sheetNo, toDate, fromDate);
                return;
            }
           
            List<RuntimeMeta> MetaData = new List<RuntimeMeta>();
            foreach(var runData in data)
            {
                RuntimeMeta metaData = new RuntimeMeta();
                metaData.Id = Guid.NewGuid();
                metaData.RuntimeDataID = runData.Id;
                metaData.SheetNo = sheetNo;
                metaData.UserName = SecurityModule.currentUserName;
                db.RuntimeDatas.InsertOnSubmit(runData);
                db.SubmitChanges();
                db.RuntimeMetas.InsertOnSubmit(metaData);
                db.SubmitChanges();
            }
            db.SubmitChanges();
        }
        public void deleteRuntimeData(int sheetNo)
        {
            try
            {
                BillingDataDataContext db = new BillingDataDataContext();
                db.sp_deleteSheetFromRuntime(SecurityModule.currentUserName, sheetNo);
            }
            catch(ChangeConflictException e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
