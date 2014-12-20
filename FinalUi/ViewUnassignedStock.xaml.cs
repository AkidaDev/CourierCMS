using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for ViewUnassignedStock.xaml
    /// </summary>
    public partial class ViewUnassignedStock : Window
    {
        public ViewUnassignedStock(StockAssignmentView stock)
        {
            InitializeComponent();
            BillingDataDataContext db = new BillingDataDataContext();
            List<string> UsedConnsignmentNo = db.Transactions.Where(x=>String.Compare(stock.StartNumber,x.ConnsignmentNo) <= 0 && String.Compare(stock.EndNumber,x.ConnsignmentNo)>=0)
                .OrderBy(y=>y.ConnsignmentNo)
                .Select(x=>x.ConnsignmentNo).ToList();
            string seriesCommon = new string((stock.StartNumber.ToCharArray().Where(x => char.IsLetter(x) == true)).ToArray());
            int seriesStart = int.Parse(stock.StartNumber.Substring(seriesCommon.Length));
            int seriesEnd = int.Parse(stock.EndNumber.Substring(seriesCommon.Length));
            List<int> AvailableConn = Enumerable.Range(seriesStart, seriesEnd - seriesStart + 1).ToList();
            List<int> UsedConn = UsedConnsignmentNo.Select(x => int.Parse(new string(x.ToCharArray().Where(y => char.IsDigit(y)).ToArray()))).ToList();
            List<string> UnusedConn = AvailableConn.Except(UsedConn).Select(x => seriesCommon + x.ToString()).ToList() ;
            CollectionViewSource viewSource = (CollectionViewSource)FindResource("ListSource");
            viewSource.Source = UnusedConn;
        }
    }
}
