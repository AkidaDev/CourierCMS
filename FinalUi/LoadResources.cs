using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace FinalUi
{
    class LoadResources
    {
        public  void DynamicLoadStyles()
        {
            string fileName =   "Blue.xaml";
                    if (File.Exists(fileName))
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Open))
                        {
                            // Read in ResourceDictionary File
                            var asq = (ResourceDictionary)XamlReader.Load(fs); 
                              ResourceDictionary dic = asq;
                            // Clear any previous dictionaries loaded
                            Application.Current.Resources.MergedDictionaries.Clear();
                            // Add in newly loaded Resource Dictionary
                            Application.Current.Resources.MergedDictionaries.Add(asq);
                        }
                    }
                    else
                        MessageBox.Show("File: " + fileName + " does not exist. Please re-enter the name.");
                
        }
    }
}
