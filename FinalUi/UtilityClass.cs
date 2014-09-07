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
            runtimeDataObj.ConsignmentNo = trans.ConnsignmentNo;
            runtimeDataObj.TransactionId = trans.ID;
            if (trans.AmountCharged != null)
                runtimeDataObj.Amount = (decimal)trans.AmountCharged;
            runtimeDataObj.FrAmount = trans.AmountPayed;
            runtimeDataObj.BookingDate = trans.BookingDate;
            runtimeDataObj.Destination = trans.Destination;
            if (trans.DestinationPin != null)
                runtimeDataObj.DestinationPin = (decimal)trans.DestinationPin;
            runtimeDataObj.Weight = Double.Parse(trans.Weight.ToString());
            if (trans.WeightByFranchize != null)
                runtimeDataObj.FrWeight = double.Parse(trans.WeightByFranchize.ToString());
            if (trans.ClientTransactions.Count != 0)
            {
                runtimeDataObj.CustCode = trans.ClientTransactions.First().Client.Code;
            }
            return runtimeDataObj;


        }
        #endregion
        #region converting runtime data to transaction data
        static public List<Transaction> convertRuntimeListToTransList(List<RuntimeData> transList)
        {
            List<Transaction> data = new List<Transaction>();
            foreach(var transObj in transList)
            {
                Transaction dat = convertRuntimeObjToTransObj(transObj);
                data.Add(dat);
            }
            return data;
        }
        static public Transaction convertRuntimeObjToTransObj(RuntimeData data)
        {
            var transactionData = new Transaction();
            transactionData.AmountCharged = data.FrAmount;
            transactionData.AmountPayed = data.Amount;
            transactionData.ConnsignmentNo = data.ConsignmentNo;
            transactionData.Destination = data.Destination;
            transactionData.DestinationPin = data.DestinationPin;
            Guid id = Guid.NewGuid();
            transactionData.ID = id;
            data.TransactionId = id;
            transactionData.AddDate = System.DateTime.Today;
            transactionData.Weight = (decimal)(data.Weight);
            transactionData.BookingDate = data.BookingDate;
            if (data.FrWeight != null)
                transactionData.WeightByFranchize = (decimal)data.FrWeight;
            return transactionData;

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
