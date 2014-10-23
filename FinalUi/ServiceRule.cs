using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    public class ServiceRule:IRule
    {
        public int Id;
        public List<string> ServiceList;
        public List<string> ZoneList;
        public List<string> CityList;
        public List<string> StateList;
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
        public void applyRule(object obj)
        { 
        }
        public void applyRule(Object obj, double origAmount)
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
                if(mode == 'P')
                {

                }

            }
        }
        public void encodeString()
        {}
        public void decodeString()
        {}
    }
}