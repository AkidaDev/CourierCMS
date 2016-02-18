using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace FinalUi
{
    [DataContract]
    class Vortexversion
    {
       
      
    }
    class Update
    {
        Version ver;
        public Version vers;
        WebRequest request;
        WebResponse response;
        StreamReader stream;
        string json;
        public Update()
        {
            ver = new Version(Configs.Default.ver);   
        }
        public void getLatestVer()
        {
            try
            {
                request = WebRequest.Create("http://api.vortex.sltintegrity.com/beta/ver.json");
                response = request.GetResponse();
                stream = new StreamReader(response.GetResponseStream());
                json = stream.ReadToEnd();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Vortexversion));
                MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(json));
            }
            catch (Exception) {  }
        }
        public int checkUpdate()
        {
            getLatestVer();
            return  ver.CompareTo(vers);
        }
    }
}