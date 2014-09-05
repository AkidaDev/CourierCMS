using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class CSVDataLoader
    {
        public List<RuntimeData> getRuntimeDataFromCSV(string filePath, char seperator, char quotes)
        {
            List<RuntimeData> data = new List<RuntimeData>();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.ElementAt(0) != '\'')
                {
                    continue;
                }

                string[] lineData = line.Split(seperator);

                RuntimeData rowData = new RuntimeData();
                rowData.Id = Guid.NewGuid();
                rowData.BranchCode = lineData[0].Trim('\'');
                rowData.ConsignmentNo = lineData[1].Trim('\'');
                rowData.BookedBy = lineData[2].Trim('\'');
                rowData.CustomerCode = lineData[3].Trim('\'');
                rowData.Weight = Double.Parse(lineData[4]);
                rowData.Type = lineData[5].Trim('\'');
                rowData.Destiniation = lineData[6].Trim('\'');
                rowData.Mode = lineData[7].Trim('\'');
                rowData.Pieces = int.Parse(lineData[8]);
                rowData.DestinationPin = Decimal.Parse(lineData[9]);
                rowData.BookingDate = DateTime.ParseExact(lineData[10].Replace("\'", ""), "dd-MM-yyyy", new CultureInfo("en-US")).Date;
                rowData.Amount = Decimal.Parse(lineData[11]);
                rowData.Status = lineData[12].Trim('\'');
                rowData.Pod_RecD = lineData[13].Trim('\'');
                rowData.TransMF_No = lineData[14].Trim('\'');
                TimeSpan time = new TimeSpan();
                if (TimeSpan.TryParseExact(lineData[15].Replace("'", ""), "hh:mm:ss", new CultureInfo("en-US"), out time))
                {
                    rowData.BookingTime = time;
                }
                rowData.DOX = lineData[16].ToCharArray()[0];
                Double doubledata;
                if (Double.TryParse(lineData[17], out doubledata))
                {
                    rowData.ServiceTax = doubledata;
                }
                if (Double.TryParse(lineData[18], out doubledata))
                {
                    rowData.SplDisc = doubledata;
                }
                rowData.Contents = lineData[19].Trim('\'');
                rowData.Remarks = lineData[20].Trim('\'');
                Decimal decimalData;
                if (Decimal.TryParse(lineData[21], out decimalData))
                {
                    rowData.Value = decimalData;
                }
                rowData.InvoiceNo = lineData[22].Trim('\'');
                DateTime dateTimeObj;
                if (DateTime.TryParse(lineData[23], out dateTimeObj))
                {
                    rowData.InvoiceDate = dateTimeObj;
                }
                if (DateTime.TryParse(lineData[24], out dateTimeObj))
                {
                    rowData.Mod_Date = dateTimeObj;
                }
                rowData.Office_Type = lineData[25].Trim('\'');
                rowData.Office_Code = lineData[26].Trim('\'');
                rowData.RefNo = lineData[27].Trim('\'');
                if (TimeSpan.TryParse(lineData[28], out time))
                {
                    //  rowData.Mod_Time = null;// time;
                }
                rowData.NodeId = lineData[29].Trim('\'');
                rowData.UserId = lineData[30].Trim('\'');
                rowData.Trans_Status = lineData[31].Trim('\'');
                rowData.Act_Cust_Code = lineData[32].Trim('\'');
                if (Decimal.TryParse(lineData[32], out decimalData))
                {
                    rowData.Mobile = decimalData;
                }
                rowData.email = lineData[34].Trim('\'');
                rowData.NDX_Paper = lineData[35].Trim('\'');
                if (TimeSpan.TryParse(lineData[36], out time))
                {
                    // rowData.PickupTime = time;
                }
                rowData.VolumeWieght = lineData[37].Trim('\'');
                rowData.CapturedWieght = lineData[38].Trim('\'');
                data.Add(rowData);

            }
            return data;
        }
    }
}
