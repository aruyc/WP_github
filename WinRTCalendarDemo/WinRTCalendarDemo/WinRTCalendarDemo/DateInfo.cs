using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinRTCalendarControl;

namespace WinRTCalendarDemo
{
    public class DateInfo : ISupportCalendarItem
    {
        public DateTime CalendarItemDate
        {
            get;
            set;
        }

        public String EventTitle
        {
            get;
            set;
        }

        public String EventDescription
        {
            get;
            set;
        }
    }
}
