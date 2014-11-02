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
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        int currentCanvas = 1;
        Canvas currentCanvasObj;
        
        public Setup()
        {
            InitializeComponent();
        }
        public void SetDefaultSetting()
        { 
        }
        private void Browse_Directory(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                DirectoryPath.Text = (path);
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void update_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            if (this.checkbox_unselected.Visibility == Visibility.Hidden)
            {
                this.checkbox_selected.Visibility = Visibility.Hidden;
                this.checkbox_unselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.checkbox_selected.Visibility = Visibility.Visible;
                this.checkbox_unselected.Visibility = Visibility.Hidden;
            }
        }
        private void Shorcut_selection(object sender, RoutedEventArgs e)
        {
            if (this.unselected.Visibility == Visibility.Hidden)
            {
                this.selected.Visibility = Visibility.Hidden;
                this.unselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.selected.Visibility = Visibility.Visible;
                this.unselected.Visibility = Visibility.Hidden;
            }
        }
        private void Open_Vortex(object sender, RoutedEventArgs e)
        {
            if (this.checkunselected.Visibility == Visibility.Hidden)
            {
                this.checkselected.Visibility = Visibility.Hidden;
                this.checkunselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.checkselected.Visibility = Visibility.Visible;
                this.checkunselected.Visibility = Visibility.Hidden;
            }
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            switch (currentCanvas)
            {
                case 1:
                    currentCanvas = 2;
                    Step1.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 2:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 3:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    currentCanvas = 5;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step5;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
            }
            if (currentCanvas == 5)
            {
                Next.Visibility = Visibility.Collapsed;
                Finish.Visibility = Visibility.Visible;
            }
            else
            {
                Next.Visibility = Visibility.Visible;
                Finish.Visibility = Visibility.Collapsed;
            }
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            switch (currentCanvas)
            {
                case 5:
                    currentCanvas = 4;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step4;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    currentCanvas = 3;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step3;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 3:
                    currentCanvas = 2;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step2;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Visible;
                    break;
                case 2:
                    currentCanvas = 1;
                    currentCanvasObj.Visibility = Visibility.Collapsed;
                    currentCanvasObj = Step1;
                    currentCanvasObj.Visibility = Visibility.Visible;
                    Previous.Visibility = Visibility.Collapsed;
                    break;
            }
        }

    }
}
