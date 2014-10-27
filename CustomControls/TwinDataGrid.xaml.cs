using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for TwinDataGrid.xaml
    /// </summary>
    public partial class TwinDataGrid : UserControl
    {
          CollectionViewSource _AllDataGridSource;
        CollectionViewSource _SelectedDataGridSource;
        #region Exposing Properties
        public DataGrid SelectedDataGridR
        {
            get
            {
                return SelectedDataGrid;
            }
        }
        public DataGrid AllDataGridR
        {
            get
            {
                return AllDataGridR;
            }
        }
        public IList AllDataGridSource
        {
            get
            {
                return (IList)_AllDataGridSource.Source;
            }
            set
            {
                _AllDataGridSource.Source = value;
            }
        }
        public IList SelectedDataGridSource
        {
            get
            {
                return (IList)_SelectedDataGridSource.Source;
            }
            set
            {
                _SelectedDataGridSource.Source = value;
            }
        }
        public string DisplayValuePath
        {
            get
            {
                return SelectedDataGrid.DisplayMemberPath;
            }
            set
            {
                SelectedDataGrid.DisplayMemberPath = value;
                AllDataGrid.DisplayMemberPath = value;
            }
        }
        public TwinDataGrid()
        {
            InitializeComponent();
            _AllDataGridSource = (CollectionViewSource)FindResource("NetDataGrid");
            _SelectedDataGridSource = (CollectionViewSource)FindResource("SelectedDataGrid");
            
        }
        #endregion

        #region DataGridOperationOperations
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            IList AllObjects = ((IList)_AllDataGridSource.Source);
            IList SelectedObjects = (IList)_SelectedDataGridSource.Source;
            foreach (var obj in AllObjects)
            {
                ((IList)_SelectedDataGridSource.Source).Add(obj);
            }
            ((IList)_AllDataGridSource.Source).Clear();
            AllDataGrid.Items.Refresh();
            SelectedDataGrid.Items.Refresh();
        }

        private void SelectSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            IList AllObjects = AllDataGrid.SelectedItems;
            foreach (var obj in AllObjects)
            {
                ((IList)_AllDataGridSource.Source).Remove(obj);
                ((IList)_SelectedDataGridSource.Source).Add(obj);
            }
            AllDataGrid.Items.Refresh();
            SelectedDataGrid.Items.Refresh();
        }

        private void DeSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            IList SelectedObjects = (IList)_SelectedDataGridSource.Source;
            foreach (var obj in SelectedObjects)
            {
                ((IList)_AllDataGridSource.Source).Add(obj);
            }
            ((IList)_SelectedDataGridSource.Source).Clear();
            AllDataGrid.Items.Refresh();
            SelectedDataGrid.Items.Refresh();
        }

        private void DeSelectSelectededButton_Click(object sender, RoutedEventArgs e)
        {
            IList SelectedObjects = (IList)SelectedDataGrid.SelectedItems;
            foreach (var obj in SelectedObjects)
            {
                ((IList)_AllDataGridSource.Source).Add(obj);
                ((IList)_SelectedDataGridSource.Source).Remove(obj);
            }
            AllDataGrid.Items.Refresh();
            SelectedDataGrid.Items.Refresh();
        }
        #endregion
    }
}
