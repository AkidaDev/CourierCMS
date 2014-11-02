using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
   public class InvoiceRule :IRule
    {
        public int Id { get; set; }
        public List<string> ServiceList { get; set; }
        public  List<string> ZoneList { get; set; }
        public List<string> StateList { get; set; }
        public List<string> CityList { get; set; }
        public  void applyRule(object obj)
        {
        }
        public void encodeString() { }
        public void decodeString()
        { }

    }
}
