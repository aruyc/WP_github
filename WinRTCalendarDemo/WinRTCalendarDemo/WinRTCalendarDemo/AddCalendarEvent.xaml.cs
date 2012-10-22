using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WinRTCalendarDemo
{
    public sealed partial class AddCalendarEvent : UserControl
    {
        DateTime selectedDate;
        public AddCalendarEvent(DateTime selectedDate)
        {
            this.InitializeComponent();
            selectedDateTb.Text = selectedDate.Date.ToString("dd-MM-yyyy");
            this.selectedDate = selectedDate;
        }

        public event EventHandler<DateInfo> addEventRequested;

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            DateInfo info = new DateInfo();
            info.EventTitle = titleTb.Text;
            info.EventDescription = descriptionTb.Text;

            int hour=int.Parse(hourComboBox.SelectionBoxItem.ToString());
            int minute=int.Parse(minComboBox.SelectionBoxItem.ToString());
            String amPm=amPmComboBox.SelectionBoxItem.ToString();

            if(hour==12)
            {
                if(amPm=="AM")
                    hour=0;
            }
            else if(amPm=="PM")
                hour+=12;

            info.CalendarItemDate = new DateTime(selectedDate.Date.Year, selectedDate.Date.Month, selectedDate.Date.Day, hour, minute, 00);
            
            if(this.addEventRequested!=null)
            {
                addEventRequested(this, info);
            }            
        }
    }
}
