using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FinalUi
{
   public interface IRule
    {
        int Id { get; set; }
        List<string> ServiceList { get; set; }
        List<string> ZoneList { get; set; }
        List<string> StateList { get; set; }
        List<string> CityList { get; set; }
        bool applyRule(object obj, double billedWeight);
   }
}
