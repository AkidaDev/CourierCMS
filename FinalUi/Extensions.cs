using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class Extensions
    {
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
}
