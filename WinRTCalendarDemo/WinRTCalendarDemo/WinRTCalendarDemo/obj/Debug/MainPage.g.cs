﻿

#pragma checksum "C:\Users\chintan.prajapati\Documents\Visual Studio 2012\Projects\WinRTCalendarDemo\WinRTCalendarDemo\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "35E50952E523A66482F932CA84744047"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WinRTCalendarDemo
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 23 "..\..\MainPage.xaml"
                ((global::WinRTCalendarControl.Calendar)(target)).MonthChanged += this.Cal_MonthChanged;
                 #line default
                 #line hidden
                #line 24 "..\..\MainPage.xaml"
                ((global::WinRTCalendarControl.Calendar)(target)).MonthChanging += this.Cal_MonthChanging;
                 #line default
                 #line hidden
                #line 25 "..\..\MainPage.xaml"
                ((global::WinRTCalendarControl.Calendar)(target)).SelectionChanged += this.Cal_SelectionChanged;
                 #line default
                 #line hidden
                #line 26 "..\..\MainPage.xaml"
                ((global::WinRTCalendarControl.Calendar)(target)).DateClicked += this.Cal_DateClicked;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 62 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Add_Button_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


