﻿#pragma checksum "..\..\AddingTask.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A146BF4FAEA3012BBB8855CBFFC87915242B5AD537CAD3F8BFF47E5DB58BFA3D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
using WPFScheduler;


namespace WPFScheduler {
    
    
    /// <summary>
    /// AddingTask
    /// </summary>
    public partial class AddingTask : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 43 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox nameTextBox;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox prioTextBox;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox coreNumTextBox;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox finishDateTextBox;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox timeLimitTextBox;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox taskTypeComboBox;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridResourceHolder;
        
        #line default
        #line hidden
        
        
        #line 168 "..\..\AddingTask.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button addButton;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFScheduler;component/addingtask.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AddingTask.xaml"
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
            this.nameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.prioTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.coreNumTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.finishDateTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.timeLimitTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.taskTypeComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 143 "..\..\AddingTask.xaml"
            this.taskTypeComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.taskTypeComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.gridResourceHolder = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            this.addButton = ((System.Windows.Controls.Button)(target));
            
            #line 175 "..\..\AddingTask.xaml"
            this.addButton.Click += new System.Windows.RoutedEventHandler(this.addButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

