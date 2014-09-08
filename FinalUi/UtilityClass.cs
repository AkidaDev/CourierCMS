using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    class UtilityClass
    {
        #region converting transaction to runtime data
        static public IEnumerable<RuntimeData> convertTransListToRuntimeList(IEnumerable<Transaction> transIEnumerable)
        {
            List<RuntimeData> runIEnumerable = new List<RuntimeData>();
            foreach (var trans in transIEnumerable)
            {
                RuntimeData runtimeDataObj = convertTransObjToRunObj(trans);
                runIEnumerable.Add(runtimeDataObj);
            }
            return runIEnumerable;
        }
        static public RuntimeData convertTransObjToRunObj(Transaction trans)
        {
            RuntimeData runtimeDataObj = new RuntimeData();
            runtimeDataObj.Amount = trans.AmountPayed;
            runtimeDataObj.BookingDate = trans.BookingDate;

            runtimeDataObj.ConsignmentNo = trans.ConnsignmentNo;
            if (trans.ClientTransactions.Count > 0)
                runtimeDataObj.CustCode = trans.ClientTransactions.Where(x => x.TransactionID == trans.ID).Single().Client.Code;
            runtimeDataObj.Destination = trans.Destination;
            runtimeDataObj.DestinationPin = (decimal)trans.DestinationPin;
            runtimeDataObj.DOX = trans.DOX;
            runtimeDataObj.EmpId = trans.Employee.Id;
            runtimeDataObj.FrAmount = trans.AmountCharged;
            runtimeDataObj.FrWeight = trans.WeightByFranchize;
            runtimeDataObj.Id = Guid.NewGuid();
            runtimeDataObj.InvoiceDate = (DateTime)trans.InvoiceDate;
            runtimeDataObj.InvoiceNo = trans.InvoiceNo;
            runtimeDataObj.Mode = trans.Mode;
            runtimeDataObj.ServiceTax = trans.ServiceTax;
            runtimeDataObj.SplDisc = trans.SplDisc;

            return runtimeDataObj;


        }
        #endregion
        #region converting runtime data to transaction data
        static public IEnumerable<Transaction> convertRuntimeIEnumerableToTransIEnumerable(IEnumerable<RuntimeData> transIEnumerable)
        {
            List<Transaction> data = new List<Transaction>();
            foreach (var transObj in transIEnumerable)
            {
                Transaction dat = convertRuntimeObjToTransObj(transObj);
                data.Add(dat);
            }
            return data;
        }
        static public Transaction convertRuntimeObjToTransObj(RuntimeData data)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            Transaction transactionData;
            if (data.TransactionId != null)
            {
                transactionData = db.Transactions.Single(x => x.ID == data.TransactionId);
                
            }
            else
            {
                transactionData = new Transaction();
                transactionData.AddDate = DateTime.Today;
                transactionData.ID = Guid.NewGuid();
            }
            transactionData.AmountCharged = data.FrAmount;
            transactionData.AmountPayed = data.Amount;
            transactionData.BookingDate = data.BookingDate;
            transactionData.ConnsignmentNo = data.ConsignmentNo;
            transactionData.Destination = data.Destination;
            transactionData.DestinationPin = data.DestinationPin;
            transactionData.DOX = data.DOX;
            if (data.EmpId != null)
                transactionData.Employee = db.Employees.Single(x => x.Id == data.EmpId);
            transactionData.InvoiceDate = data.InvoiceDate;
            transactionData.InvoiceNo = data.InvoiceNo;
            transactionData.Mode = data.Mode;
            transactionData.ServiceTax = data.ServiceTax;
            transactionData.SplDisc = data.SplDisc;
            transactionData.Type = data.Type;
            transactionData.UserId = data.EmpId;
            transactionData.Weight = data.Weight;
            transactionData.WeightByFranchize = data.FrWeight;
            transactionData.LastModified = DateTime.Today;
            return transactionData;

        }
        #endregion
        #region save runtime into transaction and save
        public static string saveRuntimeAsTransaction(IEnumerable<RuntimeData> runIEnumerable)
        {
            try
            {
                IEnumerable<RuntimeData> oldData = runIEnumerable.Where(x => x.TransactionId != null);
                IEnumerable<RuntimeData> newData = runIEnumerable.Where(x => x.TransactionId == null);
                IEnumerable<Transaction> oldTransData = convertRuntimeIEnumerableToTransIEnumerable(oldData);

                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        #region Cost calculating
        public static double getCost(string clientCode, string destinationCode, decimal destinationPin, double wieght)
        {
            return 100;
        }
        #endregion
    }
}
