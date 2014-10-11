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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for TwinListBox.xaml
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
        public ListCollectionView AllListSource
        {
            get
            {
                return (ListCollectionView)_AllListSource.Source;
            }
            set
            {
                _AllListSource.Source = value;
            }
        }
        public ListCollectionView SelectedListSource
        {
            get
            {
                return (ListCollectionView)_SelectedListSource.Source;
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
            List<object> AllObjects = (List<object>)_AllListSource.Source;
            List<object> SelectedObjects = (List<object>)_SelectedListSource.Source;
            foreach(object obj in AllObjects)
            {
                AllObjects.Remove(obj);
                SelectedObjects.Add(obj);
            }
            AllListBox.Items.Refresh();
            SelectedList.Items.Refresh();
        }

        private void SelectSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            List<object> AllObjects = (List<object>)AllListBox.SelectedItems;
            List<object> SelectedObjects = (List<object>)_SelectedListSource.Source;
            foreach (object obj in AllObjects)
            {
                AllObjects.Remove(obj);
                SelectedObjects.Add(obj);
            }
            AllListBox.Items.Refresh();
            SelectedList.Items.Refresh();
        }

        private void DeSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            List<object> AllObjects = (List<object>)_AllListSource.Source;
            List<object> SelectedObjects = (List<object>)_SelectedListSource.Source;
            foreach (object obj in SelectedObjects)
            {
                AllObjects.Add(obj);
                SelectedObjects.Remove(obj);
            }
            AllListBox.Items.Refresh();
            SelectedList.Items.Refresh();
        }

        private void DeSelectSelectededButton_Click(object sender, RoutedEventArgs e)
        {
            List<object> AllObjects = (List<object>)_AllListSource.Source;
            List<object> SelectedObjects = (List<object>)SelectedList.SelectedItems;
            foreach (object obj in SelectedObjects)
            {
                AllObjects.Add(obj);
                SelectedObjects.Remove(obj);
            }
            AllListBox.Items.Refresh();
            SelectedList.Items.Refresh();
        }
        #endregion
    }
}
