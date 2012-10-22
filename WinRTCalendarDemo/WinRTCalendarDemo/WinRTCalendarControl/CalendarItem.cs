using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WinRTCalendarControl
{
    /// <summary>
    /// This class corresponds to a calendar item / cell
    /// </summary>
    public class CalendarItem : Button
    {
        #region Members

        readonly Calendar _owningCalendar;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new instance of a calendar cell
        /// </summary>
        [Obsolete("Internal use only")]
        public CalendarItem()
        {
            DefaultStyleKey = typeof(CalendarItem);
        }

        /// <summary>
        /// Create new instance of a calendar cell
        /// </summary>
        /// <param name="owner">Calenda control that a cell belongs to</param>
        public CalendarItem(Calendar owner)
        {
            DefaultStyleKey = typeof(CalendarItem);
            _owningCalendar = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public int DayNumber
        {
            get { return (int)GetValue(DayNumberProperty); }
            internal set { SetValue(DayNumberProperty, value); }
        }

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public static readonly DependencyProperty DayNumberProperty =
            DependencyProperty.Register("DayNumber", typeof(int), typeof(CalendarItem), new PropertyMetadata(0, OnDayNumberChanged));

        private static void OnDayNumberChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
            if (item != null)
            {
                item.SetForecolor();
                item.SetBackcolor();
            }
        }


        internal bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        internal static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CalendarItem), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
            if (item != null)
            {
                item.SetBackcolor();
                item.SetForecolor();
            }
        }

        /// <summary>
        /// Date for the calendar item
        /// </summary>
        public DateTime ItemDate
        {
            get { return (DateTime)GetValue(ItemDateProperty); }
            internal set { SetValue(ItemDateProperty, value); }
        }

        /// <summary>
        /// Date for the calendar item
        /// </summary>
        internal static readonly DependencyProperty ItemDateProperty =
            DependencyProperty.Register("ItemDate", typeof(DateTime), typeof(CalendarItem), new PropertyMetadata(null));

        #endregion

        #region Template

        /// <summary>
        /// Apply default template and perform initialization
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Background = new SolidColorBrush(Colors.Transparent);
            Foreground = Application.Current.Resources["ApplicationForegroundThemeBrush"] as Brush;
            SetBackcolor();
            SetForecolor();
        }

        private bool IsConverterNeeded()
        {
            //bool returnValue = true;
            //if (_owningCalendar.DatesSource != null)
            //{
            //    if (!_owningCalendar.DatesAssigned.Contains(ItemDate))
            //    {
            //        returnValue = false;
            //    }
            //}
            //return returnValue;
            foreach (DateTime d in _owningCalendar.DatesAssigned)
            {
                if (d.Date == ItemDate.Date)
                    return true;
            }
            return false;
        }

        // Set Background Brush Required for CalendarItem
        internal void SetBackcolor()
        {
            //SolidColorBrush brush = new SolidColorBrush(Colors.White);
            //var defaultBrush = brush as Brush;
            var defaultBrush = Application.Current.Resources["FocusVisualWhiteStrokeThemeBrush"] as Brush;
            if (_owningCalendar.ColorConverter != null && IsConverterNeeded())
            {
                Background = _owningCalendar.ColorConverter.Convert(ItemDate, IsSelected, IsSelected ?
                defaultBrush :
                new SolidColorBrush(Colors.Transparent), BrushType.Background);
            }
            else
            {
                Background = IsSelected ? defaultBrush : new SolidColorBrush(Colors.Transparent);
            }
        }

        // Set Forground Brush Required for CalendarItem
        internal void SetForecolor()
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Orange);
            var defaultBrush = brush as Brush;
            //var defaultBrush = Application.Current.Resources["ApplicationForegroundThemeBrush"] as Brush;
            if (_owningCalendar.ColorConverter != null && IsConverterNeeded())
            {
                Foreground = _owningCalendar.ColorConverter.Convert(ItemDate, IsSelected, defaultBrush, BrushType.Foreground);
            }
            else
            {
                Foreground = defaultBrush;
            }
        }

        #endregion




    }
}
