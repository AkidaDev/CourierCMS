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
        static public List<RuntimeData> convertTransListToRuntimeList(List<Transaction> transList)
        {
            List<RuntimeData> runList = new List<RuntimeData>();
            foreach (var trans in transList)
            {
                RuntimeData runtimeDataObj = convertTransObjToRunObj(trans);
                runList.Add(runtimeDataObj);
            }
            return runList;
        }
        static public RuntimeData convertTransObjToRunObj(Transaction trans)
        {
            RuntimeData runtimeDataObj = new RuntimeData();
            runtimeDataObj.Amount = trans.AmountPayed;
            runtimeDataObj.BookingDate = trans.BookingDate;
            runtimeDataObj.Weight = trans.Weight;
            runtimeDataObj.ConsignmentNo = trans.ConnsignmentNo;
            runtimeDataObj.Destination = trans.Destination;
            runtimeDataObj.DestinationPin = (decimal)trans.DestinationPin;
            runtimeDataObj.DOX = trans.DOX;
            runtimeDataObj.FrAmount = trans.AmountCharged;
            runtimeDataObj.FrWeight = trans.WeightByFranchize;
            runtimeDataObj.Id = Guid.NewGuid();
            runtimeDataObj.InvoiceDate = (DateTime)trans.InvoiceDate;
            runtimeDataObj.InvoiceNo = trans.InvoiceNo;
            runtimeDataObj.Mode = trans.Mode;
            runtimeDataObj.ServiceTax = trans.ServiceTax;
            runtimeDataObj.SplDisc = trans.SplDisc;
            runtimeDataObj.CustCode = trans.CustCode;
           
            return runtimeDataObj;
        }
        static public List<RuntimeData> loadDataFromDatabase(DateTime startDate, DateTime endDate)
        {
           
            BillingDataDataContext db = new BillingDataDataContext();
            List<Transaction> transData = db.Transactions.Where(x => startDate <= x.BookingDate && endDate >= x.BookingDate).ToList();
            return convertTransListToRuntimeList(transData);
           
        }
        #endregion
        #region converting runtime data to transaction data
        static public List<Transaction> convertRuntimeListToTransList(List<RuntimeData> transList, BillingDataDataContext db)
        {
            List<Transaction> data = new List<Transaction>();
            foreach (var transObj in transList)
            {
                Transaction dat = convertRuntimeObjToTransObj(transObj, db);
                data.Add(dat);
            }
            return data;
        }
        static public Transaction convertRuntimeObjToTransObj(RuntimeData data, BillingDataDataContext db)
        {

            Transaction transactionData;
            if (Guid.Empty != data.TransactionId && data.TransactionId != null)
            {
                transactionData = db.Transactions.Single(x => x.ID == data.TransactionId);
            }
            else
            {
                transactionData = new Transaction();
                transactionData.AddDate = DateTime.Today;
                transactionData.ID = Guid.NewGuid();
                data.TransactionId = transactionData.ID;
            }
            transactionData.AmountCharged = data.FrAmount;
            transactionData.AmountPayed = data.Amount;
            transactionData.BookingDate = data.BookingDate;
            transactionData.ConnsignmentNo = data.ConsignmentNo;
            if (data.CustCode != null)
                transactionData.CustCode = db.Clients.Single(x => x.CLCODE == data.CustCode).CLCODE;
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
        public static string saveRuntimeAsTransaction(List<RuntimeData> runList)
        {
            try
            {

                BillingDataDataContext db = new BillingDataDataContext();
                List<RuntimeData> oldData = runList.Where(x => x.TransactionId != Guid.Empty && x.TransactionId != null).ToList();
                List<RuntimeData> newData = runList.Where(x => x.TransactionId == null || x.TransactionId == Guid.Empty).ToList();
                List<Transaction> oldTransData = convertRuntimeListToTransList(oldData, db).ToList();
                List<RuntimeData> duplicateData = new List<RuntimeData>();
                List<RuntimeData> removeList = new List<RuntimeData>();
                foreach (var ele in newData)
                {
                    if (db.Transactions.Count(x => x.ConnsignmentNo == ele.ConsignmentNo) > 0)
                    {
                        removeList.Add(ele);
                        ele.TransactionId = db.Transactions.Single(x => x.ConnsignmentNo == ele.ConsignmentNo).ID;
                    }
                }
                removeList.ForEach(ele =>
                {
                    duplicateData.Add(ele);
                    newData.Remove(ele);
                });
                List<Transaction> newTransData = convertRuntimeListToTransList(newData, db);
                List<RuntimeData> discrepentData = duplicateData.Where(x =>
                {
                    bool flag = false;
                    Transaction transData = db.Transactions.Single(y => y.ID == x.TransactionId);
                    if (transData.BookingDate != x.BookingDate || transData.Weight != x.Weight || transData.AmountPayed != x.Amount)
                        flag = true;
                    return flag;
                }).ToList();
                duplicateData = duplicateData.Where(x => !discrepentData.Select(y => y.Id).Contains(x.Id)).ToList();
                duplicateData.ForEach(ele =>
                {
                    ele.TransactionId = db.Transactions.Single(x => ele.ConsignmentNo == x.ConnsignmentNo).ID;
                });
                List<Transaction> dupliTransData = convertRuntimeListToTransList(duplicateData, db);
                db.Transactions.InsertAllOnSubmit(newTransData);
                db.SubmitChanges();
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