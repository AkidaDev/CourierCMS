using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TwinListBox : UserControl
    {
        CollectionViewSource _AllListSource;
        CollectionViewSource _SelectedListSource;
        #region Exposing Properties
        public ListBox SelectedListR
        {
            get
            {
                return SelectedList;
            }
        }
        public ListBox AllListR
        {
            get
            {
                return AllListR;
            }
        }
        public IList AllListSource
        {
            get
            {
                return (IList)_AllListSource.Source;
            }
            set
            {
                _AllListSource.Source = value;
            }
        }
        public IList SelectedListSource
        {
            get
            {
                return (IList)_SelectedListSource.Source;
            }
            set
            {
                _SelectedListSource.Source = value;
            }
        }
        public string DisplayValuePath
        {
            get
            {
                return SelectedList.DisplayMemberPath;
            }
            set
            {
                SelectedList.DisplayMemberPath = value;
                AllListBox.DisplayMemberPath = value;
            }
        }
        public TwinListBox()
        {
            InitializeComponent();
            _AllListSource = (CollectionViewSource)FindResource("NetList");
            _SelectedListSource = (CollectionViewSource)FindResource("SelectedList");
            
        }
        #endregion

        #region ListOperationOperations
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            IList AllObjects = ((IList)_AllListSource.Source);
            IList SelectedObjects = (IList)_SelectedListSource.Source;
            if (AllObjects != null)
            {
                foreach (var obj in AllObjects)
                {
                    ((IList)_SelectedListSource.Source).Add(obj);
                }
                ((IList)_AllListSource.Source).Clear();
                AllListBox.Items.Refresh();
                SelectedList.Items.Refresh();
            }
        }

        private void SelectSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            IList AllObjects = AllListBox.SelectedItems;
            if (AllObjects != null)
            {
                foreach (var obj in AllObjects)
                {
                    ((IList)_AllListSource.Source).Remove(obj);
                    ((IList)_SelectedListSource.Source).Add(obj);
                }
                AllListBox.Items.Refresh();
                SelectedList.Items.Refresh();
            }
        }

        private void DeSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            IList SelectedObjects = (IList)_SelectedListSource.Source;
            if (SelectedObjects != null)
            {
                foreach (var obj in SelectedObjects)
                {
                    ((IList)_AllListSource.Source).Add(obj);
                }
                ((IList)_SelectedListSource.Source).Clear();
                AllListBox.Items.Refresh();
                SelectedList.Items.Refresh();
            }
        }

        private void DeSelectSelectededButton_Click(object sender, RoutedEventArgs e)
        {
            IList SelectedObjects = (IList)SelectedList.SelectedItems;
            if (SelectedObjects != null)
            {
                foreach (var obj in SelectedObjects)
                {
                    ((IList)_AllListSource.Source).Add(obj);
                    ((IList)_SelectedListSource.Source).Remove(obj);
                }
                AllListBox.Items.Refresh();
                SelectedList.Items.Refresh();
            }
        }
        #endregion
    }
}

