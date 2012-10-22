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

namespace WinRTCalendarControl
{
    /// <summary>
    /// Class representing week number cell
    /// </summary>
    public class CalendarWeekItem : Control
    {
        #region Constructor

        /// <summary>
        /// Create new instance of a calendar week number cell
        /// </summary>
        public CalendarWeekItem()
        {
            DefaultStyleKey = typeof(CalendarWeekItem);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public int? WeekNumber
        {
            get { return (int)GetValue(WeekNumberProperty); }
            internal set { SetValue(WeekNumberProperty, value); }
        }

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public static readonly DependencyProperty WeekNumberProperty =
            DependencyProperty.Register("WeekNumber", typeof(int), typeof(CalendarWeekItem), new PropertyMetadata(null));

        #endregion
    }
}
