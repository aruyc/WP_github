using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinRTCalendarDemo
{
	// *********************************  //
    public sealed partial class MainPage : Page
    {
        MainViewModel m;
        public MainPage()
        {
            m = new MainViewModel();
            this.DataContext = m;
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {   
            
        }
        
        private void Cal_MonthChanged(object sender, WinRTCalendarControl.MonthChangedEventArgs e)
        {
            //MessageBox.Show("Cal_MonthChanged fired.  New year is " + e.Year.ToString() + " new month is " + e.Month.ToString());
        }

        private void Cal_MonthChanging(object sender, WinRTCalendarControl.MonthChangedEventArgs e)
        {
            //MessageBox.Show("Cal_MonthChanging fired.  New year is " + e.Year.ToString() + " new month is " + e.Month.ToString());
        }

        private void Cal_SelectionChanged(object sender, WinRTCalendarControl.SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Cal_SelectionChanged fired.  New date is " + e.SelectedDate.ToString());
        }

        private void Cal_DateClicked(object sender, WinRTCalendarControl.SelectionChangedEventArgs e)
        {
            m.getEventList();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            m.addEvent();
			//hello
			//edited by me
            //edited from my a/c
        }
                        
    }
}
