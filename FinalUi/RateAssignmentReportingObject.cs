using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    public class RateAssignmentReportingObject
    {
        public Rate rate { get; set; }
        public List<RateDetail> rateDetails {get; set;}
        public Client client { get; set; }
        public RateAssignmentReportingObject()
        { }
    }
}
