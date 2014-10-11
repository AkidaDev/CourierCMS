﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FinalUi
{
    public class UtilityClass
    {
        #region Convert runtime data to runtime view data
        public static List<RuntimeCityView> convertToRuntimeVIew(List<RuntimeData> data)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            List<City> cities = db.Cities.ToList();
            List<RuntimeCityView> rcvd = new List<RuntimeCityView>();
            foreach (var dataObj in data)
            {
                RuntimeCityView cityObj = new RuntimeCityView();
                cityObj.Amount = dataObj.Amount;
                cityObj.BilledWeight = dataObj.BilledWeight;
                cityObj.BookingDate = dataObj.BookingDate;
                cityObj.CITY_DESC = cities.SingleOrDefault(x => x.CITY_CODE == dataObj.Destination).CITY_DESC;
                cityObj.ConsignmentNo = dataObj.ConsignmentNo;
                cityObj.CustCode = dataObj.CustCode;
                cityObj.Destination = dataObj.Destination;
                cityObj.DestinationPin = dataObj.DestinationPin;
                cityObj.DOX = dataObj.DOX;
                cityObj.EmpId = dataObj.EmpId;
                cityObj.FrAmount = dataObj.FrAmount;
                cityObj.FrWeight = dataObj.FrWeight;
                cityObj.Id = dataObj.Id;
                cityObj.InvoiceDate = dataObj.InvoiceDate;
                cityObj.InvoiceNo = dataObj.InvoiceNo;
                cityObj.Mode = dataObj.Mode;
                cityObj.ServiceTax = dataObj.ServiceTax;
                cityObj.SplDisc = dataObj.SplDisc;
                cityObj.TransactionId = dataObj.TransactionId;
                cityObj.TransMF_No = dataObj.TransMF_No;
                cityObj.Type = dataObj.Type;
                cityObj.Weight = dataObj.Weight;
                rcvd.Add(cityObj);
            }
            return rcvd;

        }
        #endregion
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
            runtimeDataObj.ConsignmentNo = trans.ConnsignmentNo.Trim();
            runtimeDataObj.Destination = trans.Destination.Trim();
            if (trans.DestinationPin != null)
                runtimeDataObj.DestinationPin = (decimal)trans.DestinationPin;
            runtimeDataObj.DOX = trans.DOX;
            runtimeDataObj.FrAmount = trans.AmountCharged;
            runtimeDataObj.FrWeight = trans.WeightByFranchize;
            runtimeDataObj.Id = Guid.NewGuid();
            if (trans.InvoiceDate != null)
                runtimeDataObj.InvoiceDate = (DateTime)trans.InvoiceDate;
            if (trans.InvoiceNo != null)
                runtimeDataObj.InvoiceNo = trans.InvoiceNo.Trim();
            runtimeDataObj.Mode = trans.Mode.Trim();
            runtimeDataObj.ServiceTax = trans.ServiceTax;
            runtimeDataObj.SplDisc = trans.SplDisc;
            runtimeDataObj.CustCode = trans.CustCode;
            runtimeDataObj.TransactionId = trans.ID;
            if (trans.Type != null)
                runtimeDataObj.Type = trans.Type.Trim();
            return runtimeDataObj;
        }
        static public List<RuntimeData> loadDataFromDatabase(DateTime startDate, DateTime endDate)
        {

            BillingDataDataContext db = new BillingDataDataContext();
            List<Transaction> transData = db.Transactions.Where(x => startDate <= x.BookingDate && endDate >= x.BookingDate).ToList();
            return convertTransListToRuntimeList(transData).OrderBy(x => x.BookingDate).ThenBy(y => y.ConsignmentNo).ToList();

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
            transactionData.DOX = (char)data.DOX;
            if (data.EmpId != null)
                transactionData.Employee = db.Employees.Single(x => x.Id == data.EmpId);
            transactionData.InvoiceDate = data.InvoiceDate;
            transactionData.InvoiceNo = data.InvoiceNo;
            transactionData.Mode = data.Mode;
            transactionData.ServiceTax = (Double)data.ServiceTax;
            transactionData.SplDisc = (double)data.SplDisc;
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
        public static double getPriceFromRateCode(string rateCode, double weight, char isDox)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            Rate rate = db.Rates.SingleOrDefault(x => x.RateCode == rateCode);
            if (rateCode != null)
            {
                List<RateDetail> rateDetails = rate.RateDetails.OrderBy(x => x.Type.ToString() + x.Weight.ToString()).ToList();
                double lastRangeWeight = 0;
                double price = 0;
                int i = 0;
                double nextLimit = 99999;
                foreach (RateDetail rateD in rateDetails)
                {
                    if (rateDetails.ElementAtOrDefault(i + 1) != null)
                        nextLimit = rateDetails.ElementAt(i + 1).Weight;
                    else
                        nextLimit = 99999;
                    if (rateD.Type == 1)
                    {
                        if (rateD.Weight > weight)
                            return (double)(Char.ToUpper(isDox) == 'D' ? rateD.DoxRate : rateD.NonDoxRate);
                        else
                        {
                            price = (double)(Char.ToUpper(isDox) == 'D' ? rateD.DoxRate : rateD.NonDoxRate);
                            lastRangeWeight = (double)rateD.Weight;
                        }
                    }
                    if (rateD.Type == 2 || rateD.Type == 3)
                    {
                        int icurrentW, iWeight, iStepWeight;
                        iWeight = (int)(weight * 1000);
                        iStepWeight = (int)(rateD.StepWeight * 1000);
                        int inextLimit;
                        double currentW = rateD.Weight;
                        icurrentW = (int)(rateD.Weight * 1000);
                        if (weight <= currentW)
                            return price;
                        else
                        {
                            nextLimit = rateDetails.ElementAtOrDefault(i + 1) != null ? rateDetails.ElementAtOrDefault(i + 1).Weight : 999;
                            inextLimit = (int)(nextLimit * 1000);
                            while (icurrentW < inextLimit && icurrentW < iWeight)
                            {
                                price = price + (double)(Char.ToUpper(isDox) == 'D' ? rateD.DoxRate : rateD.NonDoxRate);
                                icurrentW = icurrentW + iStepWeight;
                            }
                        }
                    }
                    i++;
                }
                return price;
            }
            return -1;
        }
        public static double getCost(string clientCode, string destinationCode, double wieght, string zoneCode, string serviceCode, char dox)
        {
            Assignment ab;
            BillingDataDataContext db = new BillingDataDataContext();
            ab = db.Assignments.FirstOrDefault(x => x.ServiceCode == serviceCode && x.ClientCode == clientCode && x.ZoneCode == zoneCode);
            if (ab == null)
            {
                ab = db.Assignments.FirstOrDefault(x => x.ServiceCode == "DEFAULT" && x.ClientCode == clientCode && x.ZoneCode == zoneCode);
                if (ab == null)
                {
                    ab = db.Assignments.FirstOrDefault(x => x.ServiceCode == "DEFAULT" && x.ClientCode == clientCode && x.ZoneCode == "DEF");
                    if (ab == null)
                    {
                        ab = db.Assignments.FirstOrDefault(x => x.ServiceCode == "DEFAULT" && x.ClientCode == "DEF" && x.ZoneCode == "DEF");
                    }
                }
            }
            return getPriceFromRateCode(ab.RateCode, wieght, dox);
        }
        #endregion
        #region Miscellaneous Utilities
        public static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }
        public static string getZoneCodeFromCityCode()
        {
            return "";
        }
        #endregion

        internal static State getStateFromCity(string p)
        {
            throw new NotImplementedException();
        }

        internal static ZONE getZoneFromCityCode(string p)
        {
            throw new NotImplementedException();
        }
    }
}