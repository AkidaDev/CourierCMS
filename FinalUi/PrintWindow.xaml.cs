using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    class PrintAborted : Exception
    {
        public PrintAborted()
            : base()
        {

        }
        public PrintAborted(string message)
            : base(message)
        { }
    }
    public class UIPrinter
    {
        #region Properties

        public Int32 VerticalOffset { get; set; }
        public Int32 HorizontalOffset { get; set; }
        public String Title { get; set; }
        public UIElement Content { get; set; }

        #endregion

        #region Initialization

        public UIPrinter()
        {
            HorizontalOffset = 20;
            VerticalOffset = 20;
            Title = "Print " + DateTime.Now.ToString();
        }

        #endregion

        #region Methods
       
        public Int32 Print()
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                //---FIRST PAGE---//
                // Size the Grid.
                Content.Measure(new Size(Double.PositiveInfinity,
                                         Double.PositiveInfinity));

                Size sizeGrid = Content.DesiredSize;

                //check the width
                if (sizeGrid.Width > dlg.PrintableAreaWidth)
                {
                    MessageBoxResult result = MessageBox.Show("Ambitious width. Continue?", "Print", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                        throw new PrintAborted("Printing aborted. Width Problem");
                }

                // Position of the grid 
                var ptGrid = new Point(HorizontalOffset, VerticalOffset);

                // Layout of the grid
                Content.Arrange(new Rect(ptGrid, sizeGrid));

                //print
                dlg.PrintVisual(Content, Title);

                //---MULTIPLE PAGES---//
                double diff;
                int i = 1;
                while ((diff = sizeGrid.Height - (dlg.PrintableAreaHeight - VerticalOffset * i) * i) > 0)
                {
                    //Position of the grid 
                    var ptSecondGrid = new Point(HorizontalOffset, -sizeGrid.Height + diff + VerticalOffset);

                    // Layout of the grid
                    Content.Arrange(new Rect(ptSecondGrid, sizeGrid));

                    //print
                    int k = i + 1;
                    dlg.PrintVisual(Content, Title + " (Page " + k + ")");

                    i++;
                }

                return i;
            }

            throw new PrintAborted("Print aborted");
        }

        #endregion
    }



    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        CollectionViewSource ClientListSource;
        CollectionViewSource DataGridSource;
        List<RuntimeData> dataGridSource;
        public PrintWindow(List<RuntimeData> data)
        {
            InitializeComponent();
            ClientListSource = (CollectionViewSource)FindResource("ClientList");
            DataGridSource = (CollectionViewSource)FindResource("DataGridDataSource");
            dataGridSource = data;
            BillingDataDataContext db = new BillingDataDataContext();
            ClientListSource.Source = db.Clients.Select(x => x.CLCODE);
        }
        public void RefreshDataGridSource()
        {
            if (ClientList.SelectedValue != null && ToDate.SelectedDate != null && FromDate.SelectedDate != null)
            {

                DataGridSource.Source = dataGridSource.Where(x => x.CustCode == (string)ClientList.SelectedValue && x.BookingDate <= ToDate.SelectedDate && x.BookingDate >= FromDate.SelectedDate).ToList() ;
            }
        }
        private void ToDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            RefreshDataGridSource();
        }

        private void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDataGridSource();
        }
        private void printObj()
        {
            MessageBoxResult result = MessageBox.Show("Continue Printing? ", "Print", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var border = VisualTreeHelper.GetChild(DataGridPrint, 0) as Decorator;
                    if (border != null)
                    {
                        var scrollViewer = border.Child as ScrollViewer;
                        if (scrollViewer != null)
                        {
                            scrollViewer.ScrollToTop();
                            scrollViewer.ScrollToLeftEnd();
                        }
                    }

                    Title = ClientList.SelectedValue.ToString() + " - " + "Bill";

                    var myPrinter = new UIPrinter { Title = Title, Content = DataGridPrint };
                    int nbrOfPages = myPrinter.Print();

                    Title = ClientList.SelectedValue.ToString() + " - " + "BillPrinting Done" + " (" + nbrOfPages + " Pages)";
                }
                catch (PrintAborted ex)
                {
                    Title = ClientList.SelectedValue.ToString() + " - " + ex.Message;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            printObj();
        }
    }
}
