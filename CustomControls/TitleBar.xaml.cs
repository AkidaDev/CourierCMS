using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public WindowState windowState {get;set;}
        public Window window
        {
            get;
            set;
        }
        public TitleBar()
        {
            InitializeComponent();
        }
        private void NormalMaximize_Click(object sender, RoutedEventArgs e)
        {
            SwitchWindowState();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.windowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            window.Close();
        }

        private void SwitchWindowState()
        {
            switch (windowState)
            {
                case WindowState.Normal:
                    {
                        windowState = WindowState.Maximized;
                        Path path = new Path();
                        path.Data = Geometry.Parse(@"F1M2.111,7.667C2.111,7.667 2.111,14.958 2.111,14.958 2.111,14.958 9.889,14.958 9.889,14.958 9.889,14.958 9.889,7.667 9.889,
				7.667 9.889,7.667 2.111,7.667 2.111,7.667z M6.222,2.25C6.222,2.25,6.222,4.438,6.222,6.625L8.674,6.625C9.403,6.625 9.889,6.625 9.889,6.625 10.5,6.625 11,
				7.094 11,7.667 11,7.667 11,8.123 11,8.806L11,11 12.071,11C13.575,11 14.778,11 14.778,11 14.778,11 14.778,2.25 14.778,2.25 14.778,2.25 6.222,2.25 6.222,
				2.25z M6.222,1C6.222,1 14.778,1 14.778,1 15.45,1 16,1.562 16,2.25 16,2.25 16,11 16,11 16,11.687 15.45,12.25 14.778,12.25 14.778,12.25 13.575,12.25 12.071,
				12.25L11,12.25 11,13.819C11,14.502 11,14.958 11,14.958 11,15.531 10.5,16 9.889,16 9.889,16 2.111,16 2.111,16 1.5,16 1,15.531 1,14.958 1,14.958 1,7.667 1,
				7.667 1,7.094 1.5,6.625 2.111,6.625 2.111,6.625 2.597,6.625 3.326,6.625L5,6.625C5,4.438 5,2.25 5,2.25 5,1.562 5.55,1 6.222,1z");
                        path.Fill = Brushes.Black;
                        StackPanel panel = new StackPanel();
                        panel.Children.Add(path);
                        this.minimax.Margin = new Thickness(0, 6, 6, 0);
                        this.NormalMaximize.Content = panel;
                        this.NormalMaximize.ToolTip = "Restore Down";
                        break;
                    }
                case WindowState.Maximized:
                    {
                        windowState = WindowState.Normal;

                        this.windowState = WindowState.Normal;
                        Path path = new Path();
                        path.Data = Geometry.Parse(@"F1M3.222,5L3.222,6.702C3.222,9.071 3.222,11.778 3.222,11.778 3.222,11.778 11.778,11.778 11.778,11.778 11.778,11.778 11.778,9.071 11.778,
						6.702L11.778,5 11.281,5C9.219,5,5.781,5,3.719,5z M3.222,2C3.222,2 11.778,2 11.778,2 12.114,2 12.42,2.138 12.641,2.359L12.908,3 13,3C13,3,13,3.25,13,3.222L13,
						3.5 13,3.59 13,3.844C13,3.938 13,4 13,4 13,4 13,4.25 13,4.5L13,4.559 13,5 13,6.702C13,9.071 13,11.778 13,11.778 13,12.45 12.45,13 11.778,13 11.778,13 3.222,
						13 3.222,13 2.55,13 2,12.45 2,11.778 2,11.778 2,8.436 2,5.929L2,5 2,4.559C2,5,2,4.75,2,4.5L2,4C2,3.757 2,3.222 2,3.222 2,2.55 2.55,2 3.222,2z");
                        path.Fill = Brushes.Black;
                        StackPanel panel = new StackPanel();
                        panel.Children.Add(path);
                        this.minimax.Margin = new Thickness(0, 0, 6, 0);
                        this.NormalMaximize.Content = panel;
                        this.NormalMaximize.ToolTip = "Maximize";
                        break;
                    }
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            window.DragMove();
        }
        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "Rec_Top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "Rec_Bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "Rec_Left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "Rec_Right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "Rec_Top_Left":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "Rec_Top_Right":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "Rec_Bottom_Left":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "Rec_Bottom_Right":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }
         [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }
        private HwndSource _hwndSource;
        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        private void Rec_MouseMove(object sender, MouseEventArgs e)
        {
            var rec = (Rectangle)sender;
            switch (rec.Name)
            {
                case "Rec_Top":
                    {
                        Cursor = Cursors.SizeNS;
                        break;
                    }
                case "Rec2":
                    {
                        Cursor = Cursors.SizeWE;
                        break;
                    }
                case "Rec_Top_Left":
                    {
                        Cursor = Cursors.SizeNWSE;
                        break;
                    }
                case "Rec_Top_Right":
                    {
                        Cursor = Cursors.SizeNESW;
                        break;
                    }
                case "Rec_Bottom":
                    {
                        Cursor = Cursors.SizeNS;
                        break;
                    }
                case "Rec_Bottom_Left":
                    {
                        Cursor = Cursors.SizeNESW;
                        break;
                    }
                case "Rec_Bottom_Right":
                    {
                        Cursor = Cursors.SizeNWSE;
                        break;
                    }
                case "Rec_Left":
                    {
                        Cursor = Cursors.SizeWE;
                        break;
                    }
                case "Rec_Right":
                    {
                        Cursor = Cursors.SizeWE;
                        break;
                    }
            }
        }
    }
}
