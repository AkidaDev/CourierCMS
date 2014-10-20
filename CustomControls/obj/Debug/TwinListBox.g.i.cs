﻿#pragma checksum "..\..\TwinListBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CF0766D2B4C3C6E49291AD6EF5B5DCD8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CustomControls {
    
    
    /// <summary>
    /// TwinListBox
    /// </summary>
    public partial class TwinListBox : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 2 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CustomControls.TwinListBox TwinLostBox;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox AllListBox;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SelectAllButton;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SelectSelectedButton;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DeSelectSelectededButton;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DeSelectAllButton;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\TwinListBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox SelectedList;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CustomControls;component/twinlistbox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\TwinListBox.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TwinLostBox = ((CustomControls.TwinListBox)(target));
            return;
            case 2:
            this.AllListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 3:
            this.SelectAllButton = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\TwinListBox.xaml"
            this.SelectAllButton.Click += new System.Windows.RoutedEventHandler(this.SelectAllButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.SelectSelectedButton = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\TwinListBox.xaml"
            this.SelectSelectedButton.Click += new System.Windows.RoutedEventHandler(this.SelectSelectedButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DeSelectSelectededButton = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\TwinListBox.xaml"
            this.DeSelectSelectededButton.Click += new System.Windows.RoutedEventHandler(this.DeSelectSelectededButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.DeSelectAllButton = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\TwinListBox.xaml"
            this.DeSelectAllButton.Click += new System.Windows.RoutedEventHandler(this.DeSelectAllButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.SelectedList = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

