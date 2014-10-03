using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalUi
{
    public class  Filter
    {
        public List<string> selectedClientList {get; set;}
        DateTime toDate { get; set; }
        DateTime fromDate { get; set; }
        bool? showBilled { get; set; }
        string startConnNo { get; set; }
        string endConnNo { get; set; }
        /// <summary>
        /// Creates a Filter Object
        /// </summary>
        /// <param name="startConnNo">
        /// Starting Connsignment Number
        /// </param>
        /// <param name="endConnNo">
        /// Ending Connsignment Number
        /// </param>
        public Filter(string startConnNo, string endConnNo)
        {
        //    selectedClientList = new List<string>();
        //    toDate = DateTime.Now;
        //    fromDate = DateTime.MinValue;
        //    showBilled = null;
        //    this.startConnNo = startConnNo;
        //    this.endConnNo = endConnNo;
        }
    }
}
