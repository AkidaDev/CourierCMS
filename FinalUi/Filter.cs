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
            fromDate = DateTime.MinValue;
            showBilled = null;
            this.startConnNo = "";
            this.endConnNo = "";
        }
        public List<RuntimeData> applyFilter(List<RuntimeData> data, int sheetNo)
        {
            SecurityModule.authenticate("dharmendra", "pass");
            IEnumerable<RuntimeData> fData = data;
            BillingDataDataContext db = new BillingDataDataContext();
            List<RuntimeData> qData = db.ExecuteQuery<RuntimeData>(@"
                select 
	                RuntimeData.Id,
	                RuntimeData.ConsignmentNo,
	                RuntimeData.Weight,
	                RuntimeData.Type,
	                RuntimeData.Destination,
	                RuntimeData.Mode,
	                RuntimeData.DestinationPin,
	                RuntimeData.BookingDate,
	                RuntimeData.Amount,
	                RuntimeData.DOX,
	                RuntimeData.ServiceTax,
	                RuntimeData.SplDisc,
	                RuntimeData.InvoiceNo,
	                RuntimeData.InvoiceDate,
	                RuntimeData.EmpId,
	                RuntimeData.FrAmount,
	                RuntimeData.FrWeight,
	                RuntimeData.TransactionId,
	                RuntimeData.CustCode,
	                RuntimeData.TransMF_No,
	                RuntimeData.BilledWeight
                from
	                RuntimeData join RuntimeMeta
                on
	                RuntimeData.Id = RuntimeMeta.RuntimeDataID
	                join InvoiceAssignment
                on
	                RuntimeData.TransactionId = InvoiceAssignment.TransactionId
                where
	                RuntimeMeta.UserName = '{0}' 
	                and
	                RuntimeMeta.SheetNo = {1};

                ", "dharmendra", sheetNo).ToList();

            if (showBilled != null)
            {
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
