using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class CostingRule:Rule
    {
        #region When to apply 
        List<Service> ServiceList;
        List<ZONE> ZoneList;
        List<City> CityList;
        double startW;
        double endW;
        #endregion
        
        public CostingRule(string propertyString):base()
        {
            this.Properties = propertyString;
        }
        public CostingRule(List<Service> ServiceList, List<ZONE> Zonelist, List<City> CityList, double startW, double endW)
        {
            this.ServiceList = ServiceList;
            this.ZoneList = ZoneList;
            this.CityList = CityList;
            this.startW = startW;
            this.endW = endW;
        }
    }
}
