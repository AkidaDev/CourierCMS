using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalUi;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace UtilityProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName;
            fileName = Console.ReadLine();
            fileName = fileName.Trim('\"');
            PdfReader reader = new PdfReader(fileName);
            List<string> lines = new List<string>();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                ITextExtractionStrategy its = new LocationTextExtractionStrategy();
                string[] pageLines = PdfTextExtractor.GetTextFromPage(reader, i, its).Split('\n');
                lines.AddRange(pageLines.ToList());
            }
            Regex regex = new Regex(@"^\s[\d]{1,}");
            List<string> matchedLines = new List<string>();
            List<RuntimeData> matchedRecs = new List<RuntimeData>();
            BillingDataDataContext db = new BillingDataDataContext();
            List<City> cities = db.Cities.ToList();
            List<string> discrepantRecords = new List<string>();
            lines.ForEach((x) =>
                {
                    if (regex.Match(x).Success)
                    {
                        int index = 0;
                        string[] terms = Regex.Replace(x, @"\s{1,}", " ").Trim().Split(' ');
                        matchedLines.Add(x);
                        RuntimeData runtimeData = new RuntimeData();
                        foreach (string item in terms)
                        {
                            if (!Regex.IsMatch(item, @"\D\d{1,}"))
                                index++;
                            else
                                break;
                        }
                        try
                        {
                            runtimeData.ConsignmentNo = terms[index + 0];
                            runtimeData.TransMF_No = terms[index + 1];
                            runtimeData.BookingDate = DateTime.ParseExact(terms[index + 2].Replace("\'", ""), "dd/MM/yy", new CultureInfo("en-US")).Date;
                            runtimeData.Destination = terms[index + 3];
                            double temp;
                            if (double.TryParse(terms[index + 4], out temp))
                                runtimeData.Weight = temp;

                            else
                            {
                                index++;
                                runtimeData.Destination += terms[index + 3];
                                runtimeData.Weight = double.Parse(terms[index + 4]);
                            }
                            runtimeData.Type = terms[index + 5];
                            runtimeData.Amount = decimal.Parse(terms[index + 6]);
                            matchedRecs.Add(runtimeData);
                        }
                        catch (Exception e)
                        {
                            discrepantRecords.Add(string.Join(",", terms.Where((y, i) => i <= index + 6)));
                        }
                        if (terms.Length > index + 7)
                        {
                            foreach (string item in terms.Skip(index + 7))
                            {
                                if (!Regex.IsMatch(item, @"\D\d{1,}"))
                                    index++;
                                else
                                    break;
                            }
                            City city;
                            double temp;

                            runtimeData = new RuntimeData();
                            try
                            {
                                runtimeData.ConsignmentNo = terms[index + 7];
                                runtimeData.TransMF_No = terms[index + 8];
                                runtimeData.BookingDate = DateTime.ParseExact(terms[index + 9].Replace("\'", ""), "dd/MM/yy", new CultureInfo("en-US")).Date;
                                runtimeData.Destination = terms[index + 10];
                                if (double.TryParse(terms[index + 11], out temp))
                                    runtimeData.Weight = temp;
                                else
                                {
                                    index++;
                                    runtimeData.Destination = runtimeData.Destination + terms[index + 10];
                                    runtimeData.Weight = double.Parse(terms[index + 11]);
                                }
                                runtimeData.Type = terms[index + 12];
                                runtimeData.Amount = decimal.Parse(terms[index + 13].Trim(), NumberStyles.Float);
                                matchedRecs.Add(runtimeData);
                            }
                            catch (Exception)
                            {
                                discrepantRecords.Add(string.Join(",", terms.Where((y, i) => i >= index + 6)));
                            }
                        }
                    }
                });
            Console.WriteLine("Total lines: " + lines.Count.ToString());
            Console.WriteLine("Discrepant Records: ");
            foreach (String record in discrepantRecords)
                Console.WriteLine(record);
            Console.WriteLine("Add number of extra records: ");
            int num = int.Parse(Console.ReadLine());
            for (int i = 0; i < num; i++)
            {
                RuntimeData runData = new RuntimeData();
                Console.WriteLine("Enter connsignment for record {0}", i + 1);
                runData.ConsignmentNo = Console.ReadLine();
                Console.WriteLine("Enter TransMf No for record {0}", i + 1);
                runData.TransMF_No = Console.ReadLine();
                Console.WriteLine("Enter booking date for record {0}", i + 1);
                runData.BookingDate = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yy", new CultureInfo("en-US")).Date;
                Console.WriteLine("Enter destination for record {0}", i + 1);
                string desitination = Console.ReadLine();
                City city;
                if ((city = cities.FirstOrDefault(y => y.CITY_DESC == desitination && y.CITY_STATUS == "A")) != null)
                    runData.Destination = city.CITY_CODE;
                Console.WriteLine("Enter weight for record {0}", i + 1);
                runData.Weight = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter Type for record {0}", i + 1);
                runData.Type = Console.ReadLine();
                Console.WriteLine("Enter amount for record {0}", i + 1);
                runData.Amount = decimal.Parse(Console.ReadLine(), NumberStyles.Float);
                matchedRecs.Add(runData);
            }
            Console.WriteLine("Enter File Path..");
            string writeFilePath = Console.ReadLine();
            writeFilePath = writeFilePath.Trim('\"');
            matchedLines.Clear();
            matchedRecs.ForEach((x) =>
            {
                matchedLines.Add(string.Join(",", x.ConsignmentNo, x.Destination, x.Type, x.Weight, x.Amount));
            });
            File.WriteAllLines(writeFilePath, matchedLines);
            Dictionary<RuntimeData, string> analyzingResults = new Dictionary<RuntimeData, string>();
            DateTime toDate = DateTime.Today;
            DateTime fromDate = new DateTime(2014, 4, 1);
            Console.WriteLine("Analyzing...");
            analyzeData(matchedRecs, analyzingResults, toDate, fromDate);
            Console.WriteLine("Analyzing Done");
            Console.WriteLine("Enter file path: ");
            writeFilePath = Console.ReadLine();
            writeFilePath = writeFilePath.Trim('\"');
            matchedLines.Clear();
            analyzingResults.Keys.ToList().ForEach((x) =>
            {
                matchedLines.Add(string.Join(",", x.ConsignmentNo, x.Destination, x.Type, x.Weight, x.Amount, analyzingResults[x]));
            });
            File.WriteAllLines(writeFilePath, matchedLines);

            Console.ReadLine();

        }
        static private void analyzeData(List<RuntimeData> matchedRecs, Dictionary<RuntimeData, string> analyzingResults, DateTime toDate, DateTime fromDate)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            List<FinalUi.RuntimeData> loadedData = FinalUi.UtilityClass.loadDataFromDatabase(fromDate, toDate);
            /*Basic Checks*/
            List<RuntimeData> dupData = matchedRecs.Where(x => matchedRecs.Where(y => y.ConsignmentNo == x.ConsignmentNo).Count() > 1).ToList();
            dupData.ForEach((x) =>
            {
                analyzingResults.Add(x, "Duplicate Record");
            });
            foreach (RuntimeData data in matchedRecs)
            {
                FinalUi.RuntimeData origData = loadedData.SingleOrDefault(x => x.ConsignmentNo == data.ConsignmentNo);
                if (origData == null)
                {
                    addToAnalyzingResults(analyzingResults, data, "No corresponding record found in original transaction data");
                    continue;
                }
                if (origData.Weight != data.Weight)
                    addToAnalyzingResults(analyzingResults, data, "Weight does not match with original weight");
                if (origData.FrWeight != data.Weight && origData.FrWeight != null)
                    addToAnalyzingResults(analyzingResults, data, "Weight does not match with observed weight");
                if (origData.Amount != data.Amount)
                    addToAnalyzingResults(analyzingResults, data, "Amount does not match with original results");
                if (origData.Type != data.Type)
                    addToAnalyzingResults(analyzingResults, data, "Type does not match with original type");

                City city = db.Cities.FirstOrDefault(x => x.CITY_CODE == origData.Destination && x.CITY_STATUS == "A");
                if (city == null)
                    addToAnalyzingResults(analyzingResults, data, "No such destination found. Original destination is " + origData.Destination);
                else
                {
                    if (city.CITY_DESC.Replace(" ","") != data.Destination)
                        addToAnalyzingResults(analyzingResults, data, "Destination does not match. Original destination is " + origData.Destination);
                }
            }
        }
        static public void addToAnalyzingResults(Dictionary<RuntimeData, string> analyzingResults, RuntimeData data, string value)
        {
            if (analyzingResults.ContainsKey(data))
                analyzingResults[data] = analyzingResults[data] + ". " + value;
            else
                analyzingResults.Add(data, value);
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
