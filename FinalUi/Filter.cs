using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace FinalUi
{
    public class Filter
    {
        public List<Client> selectedClientList { get; set; }
        public DateTime toDate { get; set; }
        public DateTime fromDate { get; set; }
        public bool? showBilled { get; set; }
        public string startConnNo { get; set; }
        public string endConnNo { get; set; }
        /// <summary>
        /// Creates a Filter Object
        /// </summary>
        /// <param name="startConnNo">
        /// Starting Connsignment Number
        /// </param>
        /// <param name="endConnNo">
        /// Ending Connsignment Number
        /// </param>
        public Filter()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            selectedClientList = db.Clients.ToList();
            toDate = DateTime.Now;
            fromDate = new DateTime(2009,5,1);
            showBilled = null;
            this.startConnNo = "";
            this.endConnNo = "";
        }
        public List<RuntimeData> applyFilter(List<RuntimeData> data, int sheetNo)
        {
            IEnumerable<RuntimeData> fData = data;
            BillingDataDataContext db = new BillingDataDataContext();
            if (showBilled != null)
            {
                List<RuntimeData> qData = (from rdata in db.RuntimeDatas.Where(x => x.UserId == SecurityModule.currentUserName && x.SheetNo == sheetNo)
                                           join qdata in db.InvoiceAssignments
                                           on rdata.TransactionId equals qdata.TransactionId
                                           select rdata).ToList();
          
                if (showBilled == true)
                {
                    fData = from fdata in fData
                            join qdata in qData
                            on fdata.TransactionId equals qdata.TransactionId
                            select fdata;
                }
                else
                {
                    fData = fData.Where(x => !qData.Select(y => y.ConsignmentNo).Contains(x.ConsignmentNo));
                }
            }
            bool areNullAllowed = selectedClientList.Select(x => x.CLCODE).Contains("<NONE>");
            fData = fData.Where(x => selectedClientList.Select(y => y.CLCODE).Contains(x.CustCode) || (x.CustCode == null && areNullAllowed));
            fData = fData.Where(x => x.BookingDate <= toDate && x.BookingDate >= fromDate);
            if (startConnNo != "" && endConnNo != "")
                fData = fData.Where(x => x.ConsignmentNo.CompareTo(startConnNo) >= 0 && x.ConsignmentNo.CompareTo(endConnNo) <= 0);

            return fData.ToList();
        }
    }
}
