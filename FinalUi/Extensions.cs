using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class Extensions
    {
        public Rate rate { get; set; }
        public List<RateDetail> rateDetails {get; set;}
        public Client client { get; set; }
      
    }
    public partial class Client
    {
        public string NameAndCode
        {
            get
            {
                return "<" + this.CLCODE + ">" + this.CLNAME; 
            }
        }
    }
    public partial class Rule
    {
       
    }
}
