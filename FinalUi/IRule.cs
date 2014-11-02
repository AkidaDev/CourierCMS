using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    interface IRule
    {
        int Id { get; set; } 
        void encodeString();
        void decodeString();
   }
}
