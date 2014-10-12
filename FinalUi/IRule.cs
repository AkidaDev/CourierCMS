using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    interface IRule
    {
         void applyRule(Object obj);
         void encodeString();
        void decodeString();
    }
}
