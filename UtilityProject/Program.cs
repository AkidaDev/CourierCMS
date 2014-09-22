using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalUi;

namespace UtilityProject
{
    class Program
    {
        static void Main(string[] args)
        {
            FinalUi.BillingDataDataContext db = new FinalUi.BillingDataDataContext();
            db.DeleteDatabase();
            db.CreateDatabase();
        }
        static public void assigningRateData()
        {
            MicroDeepDataContext mDb = new MicroDeepDataContext();
            BillingDataDataContext bDb = new BillingDataDataContext();
            Dictionary<string, List<string>> fwdCodeWithClient = new Dictionary<string, List<string>>();
            Console.WriteLine("Fetching Forward Codes");
            List<string> fwdCodes = mDb.clienttariffs.Select(x => x.FWDCD).Distinct().ToList();
            foreach (string fwdCode in fwdCodes)
            {
                List<string> clientList = mDb.clienttariffs.Where(y => y.FWDCD == fwdCode).Select(x => x.CLNAME).Distinct().ToList();
                fwdCodeWithClient.Add(fwdCode, clientList);
            }
            int i = 0;
            Console.WriteLine("Assigning clients to fowarder rate");
            foreach (string fwdCode in fwdCodeWithClient.Keys)
            {
                i++;
                Console.WriteLine("Processing object {0} of {1}", i, fwdCodeWithClient.Count);
                List<Rate> ratesForForwarder = bDb.Rates.Where(x => x.RateCode.StartsWith(fwdCode.Trim())).ToList();
                if (ratesForForwarder.Count == 0)
                    continue;
                List<string> clcCodes = new List<string>();
                foreach (string clName in fwdCodeWithClient[fwdCode])
                {
                    Client client = bDb.Clients.FirstOrDefault(x => x.CLNAME == clName);
                    if (client != null)
                    {
                        clcCodes.Add(client.CLCODE);
                    }
                }
                foreach (Rate rate in ratesForForwarder)
                {
                    foreach (string clcCode in clcCodes)
                    {
                        Assignment assignment = new Assignment();
                        assignment.Id = Guid.NewGuid();
                        assignment.RateCode = rate.RateCode;
                        string zoneCode = rate.RateCode.Trim().Substring(rate.RateCode.Length - 3);
                        if (bDb.ZONEs.Where(x => x.zcode == zoneCode).Count() > 0)
                            assignment.ZoneCode = zoneCode;
                        assignment.ServiceCode = "Default";
                        assignment.ClientCode = clcCode;
                        bDb.Assignments.InsertOnSubmit(assignment);
                    }
                }
            }
            bDb.SubmitChanges();

        }
        static public void insertingRateDataFromFWd()
        {
            BillingDataDataContext bDb = new BillingDataDataContext();
            MicroDeepDataContext mDb = new MicroDeepDataContext();
            List<String> ClientCombiList = mDb.FWDTARIFFs.Select(x => x.fcombi).Distinct().ToList();
            int i = 0;
            foreach (string obj in ClientCombiList)
            {
                i++;
                Console.WriteLine("Processing Object {0} of {1}", i, ClientCombiList.Count);
                List<FWDTARIFF> tarrifList = mDb.FWDTARIFFs.Where(x => x.fcombi == obj).ToList();
                Rate rate = new Rate();
                rate.RateCode = obj;
                rate.Description = "Rate entry for forwarder: " + obj;
                bDb.Rates.InsertOnSubmit(rate);

                foreach (var tariffObj in tarrifList)
                {

                    RateDetail rateDetail = new RateDetail();
                    rateDetail.ID = Guid.NewGuid();
                    rateDetail.RateCode = rate.RateCode;
                    rateDetail.Weight = (double)tariffObj.WT;
                    rateDetail.NonDoxRate = tariffObj.ndox;
                    rateDetail.DoxRate = (double)tariffObj.dox;
                    rateDetail.Type = int.Parse(tariffObj.ratetype);
                    if (rateDetail.Type != 1)
                    {
                        if (rateDetail.Type == 2)
                        {
                            rateDetail.StepWeight = 0.1;
                        }
                        else
                            rateDetail.StepWeight = 1;
                    }
                    bDb.RateDetails.InsertOnSubmit(rateDetail);

                }
                bDb.SubmitChanges();

            }
        }
        static public void insertingRateData()
        {
            BillingDataDataContext bDb = new BillingDataDataContext();
            MicroDeepDataContext mDb = new MicroDeepDataContext();
            List<String> ClientCombiList = mDb.clienttariffs.Select(x => x.clientcombi).Distinct().ToList();
            int i = 0;
            foreach (string obj in ClientCombiList)
            {
                i++;
                Console.WriteLine("Processing Object {0} of {1}", i, ClientCombiList.Count);
                List<clienttariff> tarrifList = mDb.clienttariffs.Where(x => x.clientcombi == obj).ToList();
                Rate rate = new Rate();
                rate.RateCode = obj;
                Client tCl = bDb.Clients.SingleOrDefault(x => x.CLCODE == obj.Substring(0, obj.Length - 3));
                string clName = "";
                if (tCl != null)
                    clName = tCl.CLNAME;
                rate.Description = "Client: " + clName + "  Zone: " + obj.Substring(obj.Length - 3);
                bDb.Rates.InsertOnSubmit(rate);

                foreach (var tariffObj in tarrifList)
                {

                    RateDetail rateDetail = new RateDetail();
                    rateDetail.ID = Guid.NewGuid();
                    rateDetail.RateCode = rate.RateCode;
                    rateDetail.Weight = (double)tariffObj.wt;
                    rateDetail.NonDoxRate = tariffObj.ndox;
                    rateDetail.DoxRate = (double)tariffObj.dox;
                    rateDetail.Type = (int)tariffObj.ratetype;
                    if (rateDetail.Type != 1)
                    {
                        if (rateDetail.Type == 2)
                        {
                            rateDetail.StepWeight = 0.1;
                        }
                        else
                            rateDetail.StepWeight = 1;
                    }
                    bDb.RateDetails.InsertOnSubmit(rateDetail);

                }
                bDb.SubmitChanges();

            }
        }
    }
}
