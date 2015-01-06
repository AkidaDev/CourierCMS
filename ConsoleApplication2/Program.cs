using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalUi;
using System.Web;
using System.Web.Script.Serialization;
namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Trying to load all services...");
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Service> AllServices = db.Services.ToList();
            Console.WriteLine("Services loaded.. Inserting..");
            foreach (Service service in AllServices)
            {
                ServiceGroupAssignment Assignment = new ServiceGroupAssignment();
                Assignment.AssignId = Guid.NewGuid();
                Assignment.ServiceName = service.SER_CODE;
                switch (Assignment.ServiceName.Trim().ToUpper())
                {
                    case "COP":
                    case "CPL":
                    case "DAR":
                    case "DC2":
                    case "DCD":
                    case "DCP":
                    case "DCS":
                    case "DM2":
                    case "DMD":
                    case "DMS":
                    case "DN2":
                    case "DND":
                    case "DNP":
                    case "DNS":
                    case "DSF":
                    case "DSM":
                    case "DSZ":
                    case "DZ2":
                    case "DZD":
                    case "DZS":
                    case "MOP":
                    case "NOP":
                    case "NPC":
                    case "PEC":
                    case "PZP":
                    case "ROP":
                    case "RPC":
                    case "RPL":
                    case "SP2":
                    case "SP3":
                    case "SZP":
                    case "ZOP":
                    case "ZP2":
                    case "ZP3":
                    case "ZPC":
                        Assignment.GroupName = "PLUS";
                        break;
                    case "CP1":
                    case "D10":
                    case "D1Z":
                        Assignment.GroupName = "PTP 10:30";
                        break;
                    case "CP2":
                    case "D12":
                    case "D2Z":
                    case "R12":
                        Assignment.GroupName = "PTP 12:00";
                        break;
                    case "C14":
                    case "M14":
                    case "R14":
                    case "Z14":
                        Assignment.GroupName = "PTP 14:00";
                        break;
                    case "PAR":
                    case "PEX":
                    case "PSF":
                    case "Y01":
                    case "Y02":
                    case "Y03":
                    case "Y04":
                    case "Y05":
                    case "Y06":
                    case "Y07":
                    case "Y08":
                    case "Y09":
                    case "Y10":
                    case "Y11":
                    case "Y12":
                    case "Y13":
                    case "Y14":
                    case "Y15":
                    case "Y16":
                    case "Y17":
                    case "Y18":
                    case "Y19":
                    case "Y20":
                    case "Y22":
                    case "Y23":
                    case "Y24":
                    case "Y25":
                    case "Y26":
                    case "Y27":
                    case "Y28":
                    case "Y29":
                    case "Y30":
                    case "Y31":
                    case "Y32":
                    case "Y33":
                    case "Y34":
                    case "Y35":
                    case "Y36":
                    case "Y37":
                    case "Y38":
                    case "Y39":
                        Assignment.GroupName = "PRIORITY";
                        break;
                    case "SF1":
                    case "AC1":
                    case "AR1":
                        Assignment.GroupName = "LITE";
                        break;
                    default:
                        Console.WriteLine("Cannot find for service... " + service.SER_DESC);
                        continue;
                }
                db.ServiceGroupAssignments.InsertOnSubmit(Assignment);
            }
            Console.WriteLine("Total assignments to be made: " + db.GetChangeSet().Inserts.Count);
            Console.WriteLine("Attempting to make assignments...");
            db.SubmitChanges();
            Console.WriteLine("Changes made successfully...");
            Console.ReadLine();
        }
        static void funcToHandleAllRecords()
        {
            DataClasses1DataContext d1 = new DataClasses1DataContext();
            DataClasses2DataContext d2 = new DataClasses2DataContext();
            Console.WriteLine("Attempting to load data from mi database....");
            List<DATAENTRY> dataEntries = d2.ExecuteQuery<DATAENTRY>(@"
                SELECT [AWBNO]
      ,[BKDATE]
      ,[DEST]
      ,[CLCODE]
      ,[CONSIGNEE]
      ,[CNEADDRESS]
      ,d.[TYPE]
      ,[BMODE]
      ,[MMODE]
      ,[PKGS]
      ,[BWT]
      ,[MWT]
      ,[PMODE]
      ,[AMOUNT]
      ,[POD_DATE]
      ,[POD_TIME]
      ,[STATUS]
      ,[REMARKS]
      ,[USERNAME]
      ,[BILLNO]
      ,[network]
      ,d.[destination]
      ,[clname]
      ,[tdesc]
      ,[tamt]
      ,[ntdesc]
      ,[ntamt]
      ,[fname]
      ,[famt]
      ,[fcode]
      ,[adsns]
      ,[state]
      ,[NETNO]
      ,[dlvboy]
      ,[mainbilno]
      ,[BRANCH]
      ,[MFNO]
      ,[MFDATE]
      ,[MFWD]
      ,[Actualdate]
      ,[upload]
      ,[Shpchrg]
      ,[dtdamt]
      ,[zone]
  FROM [mi].[dbo].[DATAENTRY] d 
  join [BillingDatabase].dbo.[Transaction] t 
  on t.ConnsignmentNo COLLATE DATABASE_DEFAULT = d.AWBNO COLLATE DATABASE_DEFAULT

            ").ToList();
            Console.WriteLine("Total records loaded : {0}", dataEntries.Count);
            Console.WriteLine("Attempting to load data from BillingDatabase...");
            List<Transaction> transactions = d1.ExecuteQuery<Transaction>(@"
    SELECT [ID]
      ,[AmountPayed]
      ,[AmountCharged]
      ,[ConnsignmentNo]
      ,[Weight]
      ,[WeightByFranchize]
      ,t.[Destination]
      ,[DestinationPin]
      ,[UserId]
      ,[BookingDate]
      ,[AddDate]
      ,[LastModified]
      ,t.[Type]
      ,[Mode]
      ,[DOX]
      ,[ServiceTax]
      ,[SplDisc]
      ,[InvoiceNo]
      ,[InvoiceDate]
      ,[CustCode]
      ,[TransMF_No]
      ,[BilledWeight]
      ,[ConsigneeName]
      ,[ConsigneeAddress]
      ,[ConsignerName]
      ,[ConsignerAddress]
      ,[SubClient]
      ,[DeliveryStatus]
  FROM [BillingDatabase].[dbo].[Transaction] t
  join mi.dbo.DATAENTRY d
  on
  d.AWBNO COLLATE DATABASE_DEFAULT = t.ConnsignmentNo COLLATE DATABASE_DEFAULT
").ToList();
            Console.WriteLine("Total records loaded from BillingDatabase: {0} ", transactions.Count);
            List<String> clients = d1.Clients.Select(x => x.CLCODE).ToList();
            Console.WriteLine("Attempting to modify data..... ");
            int i = 0;
            foreach (Transaction transaction in transactions)
            {
                DATAENTRY dataentry = dataEntries.SingleOrDefault(x => x.AWBNO == transaction.ConnsignmentNo);
                if (dataentry != null)
                {
                    if (transaction.Destination != null && transaction.Destination != "")
                    {
                        try
                        {
                            switch (dataentry.CLCODE.Trim())
                            {
                                case "REKR":
                                    transaction.CustCode = "MVHS";
                                    break;
                                case "SUNY":
                                    transaction.CustCode = "suny";
                                    break;
                                case "DLFC":
                                    transaction.CustCode = "DISHL";
                                    break;
                                case "TVLS":
                                    transaction.CustCode = "TRVL";
                                    break;
                                case "OFCE":
                                    transaction.CustCode = "<NONE>";
                                    break;
                                case "HYMI":
                                    transaction.CustCode = "HONY";
                                    break;
                                case "TREF":
                                    transaction.CustCode = "TREF ";
                                    break;
                                case "MANJ":
                                    transaction.CustCode = "mnoj";
                                    break;
                                default:
                                    if (clients.Contains(dataentry.CLCODE))
                                        transaction.CustCode = dataentry.CLCODE;
                                    else
                                        throw new Exception("No client found.. " + dataentry.CLCODE);
                                    break;
                            }

                            transaction.BilledWeight = dataentry.BWT;
                            transaction.AmountCharged = (decimal)UtilityClass.getCost(transaction.CustCode, transaction.BilledWeight ?? 0.0, transaction.Destination, transaction.Type, transaction.DOX);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(transaction.ConnsignmentNo + " : " + e.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine(transaction.ConnsignmentNo + " : " + "Cannot find destination ");
                    }
                }

                else
                    Console.WriteLine(transaction.ConnsignmentNo + " : " + "Cannot find this transaction in database");
                i++;
                if (i % 250 == 0)
                    Console.WriteLine(">>" + i.ToString() + "Records done....");
            }
            Console.WriteLine("Total records to be affected: " + d1.GetChangeSet().Updates.Count);
            Console.WriteLine("Attempting to save records.... ");
            d1.SubmitChanges();
            Console.WriteLine("Changes done.....");

        }

        static void mayUseThisFuncLater() //func to change multiplier to step
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Rule> AllRules = db.Rules.Where(x => x.Type == 1).ToList();
            JavaScriptSerializer jsS = new JavaScriptSerializer();
            int i = 0;
            Console.WriteLine("Total record to be processed: {0}", AllRules.Count);
            foreach (Rule rule in AllRules)
            {
                i++;
                CostingRule cRule = jsS.Deserialize<CostingRule>(rule.Properties);
                if (cRule.type == 'S')
                {
                    if (cRule.startW > 0.49 && cRule.startW < 0.51)
                    {
                        cRule.ndStartValue = 0;
                        cRule.dStartValue = 0;

                    }
                    rule.Properties = jsS.Serialize(cRule);
                }
                if (i % 250 == 0)
                    Console.WriteLine("Processed records: {0}", i);

            }
            Console.WriteLine("Trying to submit changes ..{0}", db.GetChangeSet().Updates.Count);
            db.SubmitChanges();
        }
    }
}
