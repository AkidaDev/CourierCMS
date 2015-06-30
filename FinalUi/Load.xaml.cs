using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.IO;
namespace FinalUi
{
    /// <summary>
    /// Interaction logic for Load.xaml
    /// </summary>
    public partial class Load : Window
    {
        public Load()
        {
            #region Adding Events to be followed in all datagrid
            //Code for selecting values in textbox
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
          new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
                new RoutedEventHandler(SelectAllText));
            try
            {
                using (StreamReader sr = new StreamReader("version.txt"))
                {
                    Configs.Default.ver = sr.ReadToEnd();
                    Configs.Default.Save();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Version Error");
                Application.Current.Shutdown();
            }
            #endregion
            if (Configs.Default.IsFirst)
            {
                Setup win = new Setup();
                win.ContentRendered += win_Loaded;
                win.Show();
            }
            else
            {
                Login win = new Login();
                win.ContentRendered += win_Loaded;
                win.Show();
            }
        }
        private void win_Loaded(object sender, EventArgs e)
        {
            this.Close();
        }
        void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);
            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focused, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }
        void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }
    }
}