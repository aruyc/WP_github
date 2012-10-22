using System;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
//using Windows.UI.Xaml.Ink;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using System.ComponentModel;
using System.Linq;
using WinRTCalendarControl;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Controls.Primitives;

namespace WinRTCalendarDemo
{
    public class MainViewModel :INotifyPropertyChanged, IDateToBrushConverter
    {
        Popup popup;
        public MainViewModel()
        {   
            // CalDates Contains list of all events
            CalDates = new ObservableCollection<ISupportCalendarItem>();

            // SelectedDateEventList contains list of events for the selected date
            SelectedDateEventList = new ObservableCollection<ISupportCalendarItem>();
        }
        
        
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { selectedDate = value; NotifyPropertyChanged("SelectedDate"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<ISupportCalendarItem> dates;

        public ObservableCollection<ISupportCalendarItem> CalDates
        {
            get { return dates; }
            set { dates = value; NotifyPropertyChanged("CalDates"); }
        }

        private ObservableCollection<ISupportCalendarItem> selectedDateEventList;

        public ObservableCollection<ISupportCalendarItem> SelectedDateEventList
        {
            get { return selectedDateEventList; }
            set { selectedDateEventList = value; NotifyPropertyChanged("SelectedDateEventList"); }
        }

        // Implement IDateToBrushConverter.
        // This method is used to change Forground & Background color of CalendarItem
        public Brush Convert(DateTime dateTime, bool isSelected, Brush defaultValue, BrushType brushType)
        {
            if (brushType == BrushType.Background)
            {
                if (CalDates != null && CalDates.Where(one => one.CalendarItemDate.Date == dateTime.Date).Any() && !isSelected)
                {
                    return new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                if (CalDates != null && CalDates.Where(one => one.CalendarItemDate.Date == dateTime.Date).Any() && !isSelected)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        // Open a Popup to Add a New Event
        internal void addEvent()
        {
            AddCalendarEvent addPopup = new AddCalendarEvent(SelectedDate);
            addPopup.addEventRequested += OnaddEventRequested;

            popup = new Popup();
            popup.IsLightDismissEnabled = true;
            popup.Child = addPopup;
            popup.HorizontalOffset = (Window.Current.Bounds.Width / 2) - 200;
            popup.VerticalOffset = (Window.Current.Bounds.Height / 2) - 300;
            popup.IsOpen = true;
        }

        private void OnaddEventRequested(object sender, DateInfo e)
        {
            CalDates.Add(e);
            popup.IsOpen = false;
            getEventList();
        }


        // Get Event List for selected date
        public void getEventList()
        {
            SelectedDateEventList.Clear();
            foreach (DateInfo d in CalDates)
            {
                if (d.CalendarItemDate.Date == SelectedDate.Date)
                    selectedDateEventList.Add(d);
            }
        }
    } 
}
