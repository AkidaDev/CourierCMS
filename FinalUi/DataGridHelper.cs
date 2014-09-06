using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace FinalUi
{
    class FilterManager<T>
    {
        List<Func<T, bool>> _filters;
        List<Func<T,bool>> filters
        {
            get
            {
                return _filters;
            }
        }
        public FilterManager()
        {
            _filters = new List<Func<T,bool>>();
        }
        public void addFilter(Func<T,bool> filter)
        {
            _filters.Add(filter);
        }
        public List<T> applyFilter(List<T> dataStack)
        {
            IEnumerable<T> dataSource = dataStack;
            foreach(var filter in filters)
            {
                dataSource = dataSource.Where(filter);
            }
            return dataSource.ToList();
        }
        public void removeFilter(Func<T,bool> filter)
        {
            _filters.Remove(filter);
        }
    }

    class DataSheet<T>
    {
        public List<T> dataStack
        {
            get
            {
                return filterManager.applyFilter(_dataStack);
            }
            set
            {
                _dataStack = value;
            }
        }
        public List<T> _dataStack { get; set; }
        public string name { get; set; }
        public int currentPageNo;
        public int rowsPerPage;
        public FilterManager<T> filterManager;

        public DataSheet(List<T> value, string name)
        {
            currentPageNo = 1;
            rowsPerPage = 100;
            dataStack = value;
            filterManager = new FilterManager<T>();
            this.name = name;
        }
        public void addFilter(Func<T,bool> filter)
        {
            filterManager.addFilter(filter);
        }
        public void addData(List<T> value)
        {
            if (dataStack == null)
            {
                dataStack = value;
            }
            else
            {
                dataStack.AddRange(value);
            }
        }
    }





    class DataSheetmanager<T>
    {
        CollectionViewSource dataGridSource;
        Dictionary<int, DataSheet<T>> sheets;
        DataSheet<T> _currentDataSheet;
        public DataSheet<T> currentDataSheet
        {
            get
            {
                return _currentDataSheet;
            }
        }
        public int maxIndex
        {
            get
            {
                return sheets.Keys.Max();
            }
        }
        int _currentSheet;
        public int totalSheets
        {
            get
            {
                return sheets.Count;
            }
        }
        public int currentSheet
        {
            get
            {
                return _currentSheet;
            }
        }

        public DataSheetmanager(CollectionViewSource dataGridSource)
        {
            sheets = new Dictionary<int, DataSheet<T>>();
            this.dataGridSource = dataGridSource;
        }
        public int addNewSheet(List<T> data, string name)
        {
            if (name == "")
            {
                name = "Sheet " + totalSheets.ToString();
            }

            DataSheet<T> sheet = new DataSheet<T>(data, name);
            if (sheets.ContainsKey(totalSheets))
            {
                sheets[totalSheets] = sheet;
            }
            else
                sheets.Add(totalSheets, sheet);
            setActiveSheet(totalSheets - 1);
            return totalSheets - 1;
        }
        public void addDataToCurrentSheet(List<T> data)
        {
            _currentDataSheet.addData(data);
        }
        public void setActiveSheet(int index)
        {
            if (!sheets.ContainsKey(index))
            {
                throw new IndexOutOfRangeException("Sheet selected does not exist. (Indexing is 0 based)");
            }
            _currentSheet = index;
            _currentDataSheet = sheets[index];
        }

    }



    class DataGridHelper : INotifyPropertyChanged
    {
        DataSheetmanager<RuntimeData> dataSheetManager;
        CollectionViewSource dataGrid;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<RuntimeData> getCurrentDataStack
        {
            get
            {
                if (dataSheetManager.currentDataSheet == null)
                {
                    return null;
                }
                return dataSheetManager.currentDataSheet.dataStack;
            }
        }
        private void notifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public List<string> currentConnNos
        {
            get
            {
                if (dataSheetManager.currentDataSheet != null)
                    return dataSheetManager.currentDataSheet.dataStack.Select(c => c.ConsignmentNo).ToList();
                else
                    return null;

            }
        }
        public int rowsPerPage
        {
            get
            {
                if (dataSheetManager.currentDataSheet != null)
                    return dataSheetManager.currentDataSheet.rowsPerPage;
                else
                    return 0;
            }
            set
            {
                dataSheetManager.currentDataSheet.rowsPerPage = value;
                this.notifyPropertyChanged("rowsPerPage");
                refreshCurrentPage();

            }
        }
        public int currentPageNo
        {
            get
            {
                if (dataSheetManager.currentDataSheet != null)
                    return dataSheetManager.currentDataSheet.currentPageNo;
                else
                    return 0;
            }
            set
            {
                dataSheetManager.currentDataSheet.currentPageNo = value;

                this.notifyPropertyChanged("currentPageNo");
                refreshCurrentPage();
            }
        }
        int totalPageNo
        {
            get
            {
                if (totalRecords == 0)
                    return 1;
                else
                    return ((totalRecords - 1) / rowsPerPage) + 1;
            }
        }
        int totalRecords
        {
            get
            {
                try
                {
                    return dataSheetManager.currentDataSheet.dataStack.Count;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return 0;
                }
            }
        }
        public DataGridHelper(CollectionViewSource dataGrid)
        {
            dataSheetManager = new DataSheetmanager<RuntimeData>(dataGrid);
            this.dataGrid = dataGrid;

        }
        public int addNewSheet(List<RuntimeData> data, string name)
        {

            int val = dataSheetManager.addNewSheet(data, name);
            this.notifyPropertyChanged("currentPageNo");
            this.notifyPropertyChanged("rowsPerPage");
            return val;
        }
        public void addDataToCurrentSheet(List<RuntimeData> data)
        {

            dataSheetManager.addDataToCurrentSheet(data);
            this.notifyPropertyChanged("currentPageNo");
            this.notifyPropertyChanged("rowsPerPage");
            refreshCurrentPage();
        }

        #region pageNavigationMethods
        public List<RuntimeData> getDataForPage(int pageNo, int rows)
        {
            return dataSheetManager.currentDataSheet.dataStack.Skip((pageNo - 1) * rowsPerPage).Take(rows).ToList();
        }
        public void getPrevPage()
        {
            if (currentPageNo > 1)
            {
                currentPageNo = currentPageNo - 1;
            }
        }
        public void refreshCurrentPage()
        {
            dataGrid.Source = getDataForPage(currentPageNo, rowsPerPage);
        }
        public void getNextPage()
        {
            if (totalPageNo == currentPageNo)
            {

            }
            else
            {
                currentPageNo = currentPageNo + 1;
            }
        }
        public void getLastPage()
        {
            currentPageNo = totalPageNo;
        }
        public void getFirstPage()
        {
            currentPageNo = 1;
        }
        #endregion pageNavigationMethodsEnds

        #region sheetManagmentMethods
        public void setActiveSheet(int key)
        {
            dataSheetManager.setActiveSheet(key);
            this.notifyPropertyChanged("currentPageNo");
            this.notifyPropertyChanged("rowsPerPage");
        }
        #endregion sheetManagmentMethodsEnds
    }
}
