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
           
            foreach(var runData in data)
            {
                runData.UserId = SecurityModule.currentUserName;
                runData.SheetNo = sheetNo;
                db.RuntimeDatas.InsertOnSubmit(runData);
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
