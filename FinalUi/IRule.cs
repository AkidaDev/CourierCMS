using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    interface IRule
    {
        virtual void applyRule(Object obj);
        public void encodeString();
        public void decodeString();
    }
}
