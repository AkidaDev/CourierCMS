using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    public class ServiceRule:IRule
    {
        public int Id {get; set;}
        public List<string> ServiceGroupList { get; set; }
       
        public List<string> ServiceList { get; set; }
        public List<string> ZoneList { get; set; }
        public List<string> CityList { get; set; }
        public List<string> StateList { get; set; }
        public char type { get; set; }
        public char mode { get; set; }
        public char change { get; set; }
        public char applicable { get; set; }
        public double step { get; set; }
        public float per { get; set; }
        public double startW { get; set; }
        public double endW { get; set; }
        public double stepweight { get; set; }
        public string RangeWeight
        {
            get
            {
                return startW.ToString() + " to " + endW.ToString();
            }
        }
        public int servCount
        {
            get
            {
                if (ServiceList != null)
                {
                    return ServiceList.Count;
                }
                else
                    return 0;
            }
        }
        public int zoneCount
        {
            get
            {
                if (ZoneList != null)
                {
                    return ZoneList.Count;
                }
                else
                    return 0;
            }
        }
        public int cityCount
        {
            get
            {
                if (CityList != null)
                {
                    return CityList.Count;
                }
                else
                    return 0;
            }
        }
        public int stateCount
        {
            get
            {
                if (StateList != null)
                    return StateList.Count;
                else
                    return 0;
            }
        }
        public void applyRule(object obj)
        { 
        }
        public bool applyRule(Object obj, double origAmount)
        {
            RuntimeData runData = (RuntimeData)obj;
            double amount;
            if (applicable == 'O')
                amount = origAmount;
            else
                amount = (double)runData.FrAmount;
            if(type == 'W')
            {
                if (mode == 'P')
                {
                    amount = amount * per / 100;
                }
                else
                    amount = per;
            }
            else
            {
                int steps = (int)((runData.BilledWeight - startW) / step);
                steps++;
                per = per * steps;
                if (mode == 'P')
                {
                    amount = amount * per / 100;
                }
                else
                    amount = per;

            }
            if (change == 'I')
            {
                runData.FrAmount = (decimal)((double)runData.FrAmount + amount);
            }
            else
                runData.FrAmount = (decimal)((double)runData.FrAmount - amount);
            return true;
        }
        public void encodeString()
        {}
        public void decodeString()
        {}
    }
}