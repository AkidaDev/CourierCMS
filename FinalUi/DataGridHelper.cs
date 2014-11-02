using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;

namespace FinalUi
{

    class DataSheet
    {
        List<RuntimeData> filteredData;
        public List<RuntimeData> dataStack
        {
            get
            {
                return filteredData;
            }
            set
            {
                _dataStack = value;
            }
        }
        
        public List<RuntimeData> _dataStack { get; set; }
        public string name { get; set; }
        public int currentPageNo;
        public int rowsPerPage;
        public Filter filterObj;
        public DataSheet(List<RuntimeData> value, string name)
        {
            currentPageNo = 1;
            rowsPerPage = 100;
            dataStack = value;
            filterObj = new Filter();
            this.name = name;
            if (filteredData == null)
                applyFilter();
        }
        public void applyFilter()
        {
            filteredData = filterObj.applyFilter(_dataStack, int.Parse(name.Last().ToString()));
        }
        public void addData(List<RuntimeData> value)
        {
            if (_dataStack == null)
            {
                _dataStack = value;
            }
            else
            {
                _dataStack.AddRange(value);
            }
        }
    }





    class DataSheetmanager
    {
        CollectionViewSource dataGridSource;
        Dictionary<int, DataSheet> sheets;
        DataSheet _currentDataSheet;

        public DataSheet currentDataSheet
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
                if (sheets != null && sheets.Count > 0)
                {
                    return sheets.Keys.Max();
                }
                else
                {
                    return -1;
                }
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
            sheets = new Dictionary<int, DataSheet>();
            this.dataGridSource = dataGridSource;
        }
        public void removeSheet(int sheetKey)
        {
            if (sheets.ContainsKey(sheetKey))
            {
                sheets.Remove(sheetKey);
                if (sheets.Count > 0)
                {
                    int newActiveSheet = sheets.Keys.Min();
                    setActiveSheet(newActiveSheet);
                }
                else
                {
                    _currentSheet = -1;
                    _currentDataSheet = null;
                }
            }
        }
        public int addNewSheet(List<RuntimeData> data, string name)
        {
            int key = maxIndex + 1;
            if (name == "")
            {
                key = maxIndex + 1;
                name = "Sheet " + (key).ToString();
            }
            else
            {
                key = int.Parse(name.Split(' ').Last());
            }


            DataSheet sheet = new DataSheet(data, name);
            sheets.Add(key, sheet);
            setActiveSheet(key);
            return key;
        }
        public int currentSheetCount
        {
            get
            {
                if (sheets == null)
                    return 0;
                return sheets.Count;
            }
        }
        public void addDataToCurrentSheet(List<RuntimeData> data)
        {
            try
            {
                _currentDataSheet.addData(data);
            }
            catch (Exception)
            {
                MessageBox.Show("booga booga");
            }
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
        public Filter CurrentSheetFilterObject
        {
            get
            {
                return currentDataSheet.filterObj;
            }
            set
            {
                currentDataSheet.filterObj = value;
            }
        }

    }



    class DataGridHelper : INotifyPropertyChanged
    {
        DataSheetmanager dataSheetManager;
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
                return dataSheetManager.currentDataSheet.dataStack.OrderBy(x => x.ConsignmentNo).ToList();
            }
            set
            {
                dataSheetManager.currentDataSheet.dataStack = value;
            }
        }
        public int currentMaxSheetNumber
        {
            get
            {
                return dataSheetManager.maxIndex;
            }
        }
        public int currentSheetNumber
        {
            get
            {
                return dataSheetManager.currentSheet;
            }
        }
        public DataSheet currentDataSheet
        {
            get
            {
                return dataSheetManager.currentDataSheet;
            }
        }
        private void notifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public List<string> currentConnNosNoFilter
        {
            get
            {
                if (dataSheetManager.currentDataSheet != null)
                    return dataSheetManager.currentDataSheet._dataStack.Select(c => c.ConsignmentNo).ToList();
                else
                    return null;

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
                if (dataSheetManager.currentDataSheet != null)
                {
                    dataSheetManager.currentDataSheet.rowsPerPage = value;
                    this.notifyPropertyChanged("rowsPerPage");
                    refreshCurrentPage();
                }

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
                if (dataSheetManager.currentDataSheet != null)
                {
                    dataSheetManager.currentDataSheet.currentPageNo = value;

                    this.notifyPropertyChanged("currentPageNo");
                    refreshCurrentPage();
                }
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
            dataSheetManager = new DataSheetmanager(dataGrid);
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
        public int CurrentNumberOfSheets
        {
            get
            {
                return dataSheetManager.currentSheetCount;
            }
        }
        #region pageNavigationMethods
        public List<RuntimeData> getDataForPage(int pageNo, int rows)
        {
            if (dataSheetManager.currentDataSheet != null)
                return dataSheetManager.currentDataSheet.dataStack.Skip((pageNo - 1) * rowsPerPage).Take(rows).ToList();
            else
                return null;
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
        public void removeSheet(int sheetKey)
        {
            dataSheetManager.removeSheet(sheetKey);
            if (dataSheetManager.totalSheets == 0)
                dataGrid.Source = null;
            this.notifyPropertyChanged("currentPageNo");
            this.notifyPropertyChanged("rowsPerPage");
            if (dataSheetManager.totalSheets != 0)
                refreshCurrentPage();
        }
        public Filter CurrentSheetFilterObject
        {
            get
            {
                return currentDataSheet.filterObj;
            }
            set
            {
                currentDataSheet.filterObj = value;
            }
        }
        public bool areSheetsPresent
        {
            get
            {
                if (dataSheetManager.totalSheets > 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion sheetManagmentMethodsEnds
    }
}
