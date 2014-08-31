using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{

    class DataSheet<T>
    {
        class DataStack<T>
        {
            List<T> dataStack {get; set;}
            public DataStack()
            {
                
            }
        }

        DataStack<T> dataStack;
        public DataSheet(T value)
        {
            dataStack = new DataStack<T>();
        }
    }
    class DataSheetmanager<T>
    {
        List<DataSheet<T>> sheets;
        public DataSheetmanager()
        {
            sheets = new List<DataSheet<T>>();
        }
    }
    class DataGridHelper
    {
        public DataGridHelper()
        {

        }
    }
}
