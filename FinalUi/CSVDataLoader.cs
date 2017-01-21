using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FinalUi
{
    class CSVDataLoader
    {
        public List<RuntimeData> getRuntimeDataFromPodCSV( string filePath,char seperator, char quotes)
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
                rowData.ConsignmentNo = lineData[0].Trim('\'');
                rowData.DeliveryDate = lineData[1].Trim('\'');
                string time;
                lineData[2] = lineData[2].Trim('\'').Trim(' ');
                try
                {

                    if (lineData[2] != null && lineData[2]!= "" && lineData[2] != "NULL" && lineData[2].Length >= 4)
                    {
                        rowData.DeliveryTime = DateTime.ParseExact(lineData[2], "HHmm",null).ToString("HH:mm");
                    }else
                    {
                        rowData.DeliveryTime = null;
                    }

                }
                catch (Exception ex)
                {
                    rowData.DeliveryTime = null;
                }
                rowData.DeliveryTime = lineData[2].Trim('\'');
                rowData.ReceivedBy = lineData[3].Trim('\'');
                rowData.Remarks = lineData[4].Trim('\'');
                data.Add(rowData);
            }
            return data;
        }
        public List<RuntimeData> getRuntimeDataFromCSV(string filePath, char seperator, char quotes)
        {
            List<City> cityList = DataSources.CityCopy;
            List<Service> serviceList = DataSources.ServicesCopy;
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
                rowData.ConsignmentNo = lineData[1].Trim('\'');
                rowData.Weight = Double.Parse(lineData[4]);
                rowData.FrWeight = rowData.Weight;
                rowData.BilledWeight = rowData.Weight;
                rowData.Type = lineData[5].Trim('\'');
                rowData.Destination = lineData[6].Trim('\'');
                rowData.Mode = lineData[7].Trim('\'');
                rowData.DestinationPin = Decimal.Parse(lineData[9]);
                rowData.BookingDate = DateTime.ParseExact(lineData[10].Replace("\'", ""), "dd-MM-yyyy", new CultureInfo("en-US")).Date;
                rowData.Amount = Decimal.Parse(lineData[11]);
                rowData.TransMF_No = lineData[14].Trim('\'');
                rowData.DOX = lineData[16].Trim('\'').ToCharArray()[0];
                Double doubledata;
                if (Double.TryParse(lineData[17], out doubledata))
                {
                    rowData.ServiceTax = doubledata;
                }
                if (Double.TryParse(lineData[18], out doubledata))
                {
                    rowData.SplDisc = doubledata;
                }
                rowData.InvoiceNo = lineData[22].Trim('\'');
                DateTime dateTimeObj;
                if (DateTime.TryParse(lineData[23], out dateTimeObj))
                {
                    rowData.InvoiceDate = dateTimeObj;
                }
                rowData.CustCode = "<NONE>";
                Service service = serviceList.SingleOrDefault(x => x.SER_CODE == rowData.Type);
                if (service != null)
                    rowData.Service_Desc = service.SER_DESC;
                City city = cityList.SingleOrDefault(x => x.CITY_CODE == rowData.Destination);
                if (city != null)
                    rowData.City_Desc = city.CITY_DESC;
                rowData.Client_Desc = DataSources.ClientCopy.SingleOrDefault(x => x.CLCODE == "<NONE>").CLNAME;
                data.Add(rowData);

            }
            return data;

        }
    }
}
