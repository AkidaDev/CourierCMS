﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class ServiceRule:Rule,IRule
    {
        public int Id;
        public List<string> ServiceList;
        public List<string> ZoneList;
        public List<string> CityList;
        public List<string> StateList;
        public char type;
        public char mode;
        public char change;
        public char applicable;
        public double amount;
        public float per;
        public double startW;
        public double endW;
        public double stepweight;
        public void applyRule(Object obj)
        {
        }
        public void encodeString()
        {}
        public void decodeString()
        {}
    }
}