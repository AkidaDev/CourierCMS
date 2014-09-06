using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    class UtilityClass
    {
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
            runtimeDataObj.BookingDate = trans.Date;
            runtimeDataObj.Destiniation = trans.Destination;
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
    }
}
