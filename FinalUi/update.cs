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
        [DataMember(Name = "ver", IsRequired = true)]
        public  string ver;
    }
    class Update
    {
        Version ver;
        Version vers;
        WebRequest request;
        WebResponse response;
        StreamReader stream;
        string json;
        public Update()
        {
            ver = new Version(Configs.Default.ver);   
        }
        public int checkVer()
        {
            try
            {
                request = WebRequest.Create("http://testapi.sltintegrity.com/ver.json");
                response = request.GetResponse();
                stream = new StreamReader(response.GetResponseStream());
                json = stream.ReadToEnd();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Vortexversion));
                MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(json));
                vers = new Version(((Vortexversion)ser.ReadObject(ms)).ver);
                int i = ver.CompareTo(vers);
                return i;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return 0;
        }
    }
}