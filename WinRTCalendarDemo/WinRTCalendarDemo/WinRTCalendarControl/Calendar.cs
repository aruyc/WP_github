﻿using System;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.UI.Xaml.Data;
using System.ComponentModel;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;


//using Microsoft.Xna.Framework.Input.Touch;

namespace WinRTCalendarControl
{
    /// <summary>
    /// Calendar control for Windows RT
    /// </summary>
    public class Calendar : Control
    {
        #region Constructor
        
        /// <summary>
        /// Create new instance of a calendar
        /// </summary>
        public Calendar()
        {
            DefaultStyleKey = typeof(Calendar);
            DatesAssigned = new List<DateTime>();
            var binding = new Binding();
            Loaded += CalendarLoaded;
            SetBinding(PrivateDataContextPropertyProperty, binding);
            WireUpDataSource(DataContext, DataContext);
            _dateTimeFormatInfo = !CultureInfo.CurrentCulture.IsNeutralCulture ?
                CultureInfo.CurrentCulture.DateTimeFormat :
                (new CultureInfo("en-US")).DateTimeFormat;
            SetupDaysOfWeekLabels();            
        }

        void CalendarLoaded(object sender, RoutedEventArgs e)
        {            
            if (EnableGestures)
            {
                EnableGesturesSupport();
            }            
        }
        
        
        #endregion

        #region Gestures

        private double cumulativeDeltaY, cumulativeDeltaX, linearVelocityX, linearVelocityY;
        private const double HorizontalDeltaX = 75.0;
        private const double HorizontalDeltaY = 200.0;
        private const double VerticalDeltaX = 200.0;
        private const double VerticalDeltaY = 75.0;
        private const double LinearVelocity = 0.05;

        private void EnableGesturesSupport()
        {
            DisableGesturesSupport();
            //TouchPanel.EnabledGestures = GestureType.Flick;

            ManipulationMode = ManipulationModes.All;
            ManipulationStarting += Calendar_ManipulationStarting;
            ManipulationDelta += Calendar_ManipulationDelta;
            ManipulationCompleted += Calendar_ManipulationCompleted;
        }

        void Calendar_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //store values of horizontal & vertical cumulative translation
            cumulativeDeltaX = e.Cumulative.Translation.X;
            cumulativeDeltaY = e.Cumulative.Translation.Y;

            //store value of linear velocity into horizontal direction 
            linearVelocityX = e.Velocities.Linear.X;

            //store value of linear velocity into vertical direction 
            linearVelocityY = e.Velocities.Linear.Y;
        }

        void Calendar_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            e.Container = this;
            e.Handled = true;
        }

        private void Calendar_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            bool isRightToLeftSwipe = false; //to determine horizontal swipe direction
            bool isDownToUpSwipe = false; //to determine Vertical swipe direction

            if (cumulativeDeltaX < 0)
            {
                //moving from right to left
                isRightToLeftSwipe = true;
            }

            if (cumulativeDeltaY < 0)
            {
                //moving from down to up
                isDownToUpSwipe = true;
            }

            //check if this is a horizontal swipe gesture
            if (isHorizontalSwipeGesture(cumulativeDeltaX, cumulativeDeltaY, linearVelocityX))
            {
                if (isRightToLeftSwipe)
                {
                    IncrementMonth();
                }
                else
                {
                    DecrementMonth();
                }
            }

            //check if this is a vertical swipe gesture
            if (isVerticalSwipeGesture(cumulativeDeltaX, cumulativeDeltaY, linearVelocityY))
            {
                if (isDownToUpSwipe)
                {
                    IncrementYear();
                }
                else
                {
                    DecrementYear();
                }
            }
        }

        private bool isHorizontalSwipeGesture(double deltaX, double deltaY, double linearVelocity)
        {
            bool result = false;
            if (Math.Abs(deltaY) <= HorizontalDeltaX && Math.Abs(deltaX) >= HorizontalDeltaY && Math.Abs(linearVelocity) >= LinearVelocity)
                result = true;

            return result;
        }

        private bool isVerticalSwipeGesture(double deltaX, double deltaY, double linearVelocity)
        {
            bool result = false;
            if (Math.Abs(deltaY) >= VerticalDeltaX && Math.Abs(deltaX) <= VerticalDeltaY && Math.Abs(linearVelocity) >= LinearVelocity)
                result = true;

            return result;
        }

        private void DisableGesturesSupport()
        {
            //TouchPanel.EnabledGestures = GestureType.None;
            ManipulationStarting -= Calendar_ManipulationStarting;
            ManipulationDelta -= Calendar_ManipulationDelta;
            ManipulationCompleted -= Calendar_ManipulationCompleted;            
        }
        #endregion

        #region Memebers

        private Grid _itemsGrid;
        CalendarItem _lastItem;
        private bool _addedItems;
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;
        internal List<DateTime> DatesAssigned;

        #endregion

        #region Events

        /// <summary>
        /// Event that occurs before month/year combination is changed
        /// </summary>
        public event EventHandler<MonthChangedEventArgs> MonthChanging;

        /// <summary>
        /// Event that occurs after month/year combination is changed
        /// </summary>
        public event EventHandler<MonthChangedEventArgs> MonthChanged;

        /// <summary>
        /// Event that occurs after a date is selected on the calendar
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Event that occurs after a date is clicked on
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> DateClicked;

        /// <summary>
        /// Raises MonthChanging event
        /// </summary>
        /// <param name="year">Year for event arguments</param>
        /// <param name="month">Month for event arguments</param>
        protected void OnMonthChanging(int year, int month)
        {
            if (MonthChanging != null)
            {
                MonthChanging(this, new MonthChangedEventArgs(year, month));
            }
        }

        /// <summary>
        /// Raises MonthChanged event
        /// </summary>
        /// <param name="year">Year for event arguments</param>
        /// <param name="month">Month for event arguments</param>
        protected void OnMonthChanged(int year, int month)
        {
            if (MonthChanged != null)
            {
                MonthChanged(this, new MonthChangedEventArgs(year, month));
            }
        }

        /// <summary>
        /// Raises SelectedChanged event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnSelectionChanged(DateTime dateTime)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new SelectionChangedEventArgs(dateTime));
            }
        }

        /// <summary>
        /// Raises DateClicked event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnDateClicked(DateTime dateTime)
        {
            if (DateClicked != null)
            {
                DateClicked(this, new SelectionChangedEventArgs(dateTime));
            }
        }

        #endregion

        #region Constants

        private const short RowCount = 6;
        private const short ColumnCount = 8;

        #endregion

        #region Properties



        internal object PrivateDataContextProperty
        {
            get { return GetValue(PrivateDataContextPropertyProperty); }
            set { SetValue(PrivateDataContextPropertyProperty, value); }
        }

        internal static readonly DependencyProperty PrivateDataContextPropertyProperty =
           DependencyProperty.Register("PrivateDataContextProperty", typeof(object), typeof(Calendar), new PropertyMetadata(null, new PropertyChangedCallback(OnPrivateDataContextChanged)));
                
        private static void OnPrivateDataContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                calendar.WireUpDataSource(e.OldValue, e.NewValue);
                calendar.Refresh();
            }
        }

        private void WireUpDataSource(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                var source = newValue as INotifyPropertyChanged;
                if (source != null)
                {
                    source.PropertyChanged += SourcePropertyChanged;
                }
            }
            if (oldValue != null)
            {
                var source = newValue as INotifyPropertyChanged;
                if (source != null)
                {
                    source.PropertyChanged -= SourcePropertyChanged;
                }
            }
        }

        private const short Factor = 1000;

        /// <summary>
        /// Explicitly refresh the calendar
        /// </summary>
        public void Refresh()
        {
            BuildDates();
            BuildItems();
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            /*var expression = GetBindingExpression(DatesSourceProperty);
            if (expression != null)
            {
                if (expression.ParentBinding.Path.Path.EndsWith(e.PropertyName))
                {
                    Refresh();
                }
            }*/
            if(e.PropertyName.ToLower().Contains("date"))
                Refresh();
        }


        /// <summary>
        /// Collection of objects containing dates
        /// </summary>
        public IEnumerable<ISupportCalendarItem> DatesSource
        {
            get { return (IEnumerable<ISupportCalendarItem>)GetValue(DatesSourceProperty); }
            set { SetValue(DatesSourceProperty, value); }
        }

        /// <summary>
        /// Collection of objects containing dates
        /// </summary>
        public static readonly DependencyProperty DatesSourceProperty =
            DependencyProperty.Register("DatesSource", typeof(object), typeof(Calendar), new PropertyMetadata(null, OnDatesSourceChanged));

        private static void OnDatesSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                calendar.BuildDates();
                calendar.BuildItems();
                if (e.OldValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged) e.NewValue).CollectionChanged -= calendar.DatesSourceChanged;
                }
                if (e.NewValue is INotifyCollectionChanged)
                {
                    (e.NewValue as INotifyCollectionChanged).CollectionChanged += calendar.DatesSourceChanged;
                }
            }
        }


        private void DatesSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Property name for each object in DatesSource that contains the date to be evaluating 
        /// when building a calendar
        /// </summary>
        public static readonly DependencyProperty DatePropertyNameForDatesSourceProperty =
            DependencyProperty.Register("DatePropertyNameForDatesSource", typeof(string), typeof(Calendar), new PropertyMetadata(string.Empty, OnDatesSourceChanged));



        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public Style CalendarItemStyle
        {
            get { return (Style)GetValue(CalendarItemStyleProperty); }
            set { SetValue(CalendarItemStyleProperty, value); }
        }

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public static readonly DependencyProperty CalendarItemStyleProperty =
            DependencyProperty.Register("CalendarItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public Style CalendarWeekItemStyle
        {
            get { return (Style)GetValue(CalendarWeekItemStyleStyleProperty); }
            set { SetValue(CalendarWeekItemStyleStyleProperty, value); }
        }

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public static readonly DependencyProperty CalendarWeekItemStyleStyleProperty =
            DependencyProperty.Register("CalendarWeekItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));

        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public string YearMonthLabel
        {
            get { return (string)GetValue(YearMonthLabelProperty); }
            internal set { SetValue(YearMonthLabelProperty, value); }
        }

        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public static readonly DependencyProperty YearMonthLabelProperty =
            DependencyProperty.Register("YearMonthLabel", typeof(string), typeof(Calendar), new PropertyMetadata(""));

        /// <summary>
        /// This value currently selected date on the calendar
        /// This property can be bound to
        /// </summary>
        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// This value currently selected date on the calendar
        /// This property can be bound to
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.MinValue, OnSelectedDateChanged));

        private static void OnSelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                var newValue = (DateTime) e.NewValue;
                if (calendar._itemsGrid != null)
                {
                    var query = from oneChild in calendar._itemsGrid.Children
                                where
                                    oneChild is CalendarItem && ((CalendarItem) oneChild).IsSelected &&
                                    ((CalendarItem) oneChild).ItemDate != newValue
                                select (CalendarItem) oneChild;
                    //query.ToList().ForEach(one => one.IsSelected = false);
                    foreach (var one in query.ToList())
                    {
                        one.IsSelected = false;
                    }
                }
                calendar.OnSelectionChanged(newValue);
            }
        }


        /// <summary>
        /// This converter is used to dynamically color the background or day number of a calendar cell
        /// based on date and the fact that a date is selected and type of conversion
        /// </summary>
        public IDateToBrushConverter ColorConverter
        {
            get { return (IDateToBrushConverter)GetValue(ColorConverterProperty); }
            set { SetValue(ColorConverterProperty, value); }
        }

        /// <summary>
        /// This converter is used to dynamically color the background of a calendar cell
        /// based on date and the fact that a date is selected
        /// </summary>
        public static readonly DependencyProperty ColorConverterProperty =
            DependencyProperty.Register("ColorConverter", typeof(object), typeof(Calendar), new PropertyMetadata(null));



        /// <summary>
        /// Currently selected year
        /// </summary>
        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        /// <summary>
        /// Currently selected year
        /// </summary>
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Year, OnSelectedYearMonthChanged));

        private static void OnSelectedYearMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null && (calendar._year != calendar.SelectedYear || calendar._month != calendar.SelectedMonth))
            {
                if (!calendar._ignoreMonthChange)
                {
                    calendar._year = calendar.SelectedYear;
                    calendar._month = calendar.SelectedMonth;
                    calendar.SetYearMonthLabel();
                }
            }
        }


        /// <summary>
        /// Currently selected month
        /// </summary>
        public int SelectedMonth
        {
            get { return (int)GetValue(SelectedMonthProperty); }
            set { SetValue(SelectedMonthProperty, value); }
        }

        /// <summary>
        /// Currently selected month
        /// </summary>
        public static readonly DependencyProperty SelectedMonthProperty =
            DependencyProperty.Register("SelectedMonth", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Month, OnSelectedYearMonthChanged));


        /// <summary>
        /// If true, previous and next month buttons are shown
        /// </summary>
        public bool ShowNavigationButtons
        {
            get { return (bool)GetValue(ShowNavigationButtonsProperty); }
            set { SetValue(ShowNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// If true, previous and next month buttons are shown
        /// </summary>
        public static readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register("ShowNavigationButtons", typeof(bool), typeof(Calendar), new PropertyMetadata(true));

        /// <summary>
        /// If true, gesture support is enabled
        /// </summary>
        public bool EnableGestures
        {
            get { return (bool)GetValue(EnableGesturesProperty); }
            set { SetValue(EnableGesturesProperty, value); }
        }

        /// <summary>
        /// If true, gesture support is enabled
        /// </summary>
        public static readonly DependencyProperty EnableGesturesProperty =
            DependencyProperty.Register("EnableGestures", typeof(bool), typeof(Calendar), new PropertyMetadata(false, OnEnableGesturesChanged));

        /// <summary>
        /// Handle changes to gesture support
        /// </summary>
        /// <param name="sender">Calendar control</param>
        /// <param name="e">Event arguments</param>
        public static void OnEnableGesturesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = (Calendar)sender;
            if (target.EnableGestures)
            {
                target.EnableGesturesSupport();
            }
            else
            {
                target.DisableGesturesSupport();
            }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public bool ShowSelectedDate
        {
            get { return (bool)GetValue(ShowSelectedDateProperty); }
            set { SetValue(ShowSelectedDateProperty, value); }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public static readonly DependencyProperty ShowSelectedDateProperty =
            DependencyProperty.Register("ShowSelectedDate", typeof(bool), typeof(Calendar), new PropertyMetadata(true));


        /// <summary>
        /// Sets an option of how to display week number
        /// </summary>
        public WeekNumberDisplayOption WeekNumberDisplay
        {
            get { return (WeekNumberDisplayOption)GetValue(WeekNumberDisplayProperty); }
            set { SetValue(WeekNumberDisplayProperty, value); }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public static readonly DependencyProperty WeekNumberDisplayProperty =
            DependencyProperty.Register("WeekNumberDisplay", typeof(WeekNumberDisplayOption), typeof(Calendar),
            new PropertyMetadata(WeekNumberDisplayOption.None, OnWeekNumberDisplayChanged));

        /// <summary>
        /// Update calendar display when display option changes
        /// </summary>
        /// <param name="sender">Calendar control</param>
        /// <param name="e">Event arguments</param>
        public static void OnWeekNumberDisplayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)sender).BuildItems();
        }



        /// <summary>
        /// Gets or sets the label for Sunday
        /// </summary>
        /// <value>
        /// The label for Sunday
        /// </value>
        public string Sunday
        {
            get { return (string)GetValue(SundayProperty); }
            set { SetValue(SundayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Sunday
        /// </summary>
        /// <value>
        /// The label for Sunday
        /// </value>
        public static readonly DependencyProperty SundayProperty =
            DependencyProperty.Register("Sunday", typeof(string), typeof(Calendar), new PropertyMetadata("Su"));


        /// <summary>
        /// Gets or sets the label for Monday
        /// </summary>
        /// <value>
        /// The label for Monday
        /// </value>
        public string Monday
        {
            get { return (string)GetValue(MondayProperty); }
            set { SetValue(MondayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Monday
        /// </summary>
        /// <value>
        /// The label for Monday
        /// </value>
        public static readonly DependencyProperty MondayProperty =
            DependencyProperty.Register("Monday", typeof(string), typeof(Calendar), new PropertyMetadata("Mo"));



        /// <summary>
        /// Gets or sets the label for Tuesday
        /// </summary>
        /// <value>
        /// The label for Tuesday
        /// </value>
        public string Tuesday
        {
            get { return (string)GetValue(TuesdayProperty); }
            set { SetValue(TuesdayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Tuesday
        /// </summary>
        /// <value>
        /// The label for Tuesday
        /// </value>
        public static readonly DependencyProperty TuesdayProperty =
            DependencyProperty.Register("Tuesday", typeof(string), typeof(Calendar), new PropertyMetadata("Tu"));


        /// <summary>
        /// Gets or sets the label for Wednesday
        /// </summary>
        /// <value>
        /// The label for Wednesday
        /// </value>
        public string Wednesday
        {
            get { return (string)GetValue(WednesdayProperty); }
            set { SetValue(WednesdayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Wednesday
        /// </summary>
        /// <value>
        /// The label for Wednesday
        /// </value>
        public static readonly DependencyProperty WednesdayProperty =
            DependencyProperty.Register("Wednesday", typeof(string), typeof(Calendar), new PropertyMetadata("We"));


        /// <summary>
        /// Gets or sets the label for Thursday
        /// </summary>
        /// <value>
        /// The label for Thursday
        /// </value>
        public string Thursday
        {
            get { return (string)GetValue(ThursdayProperty); }
            set { SetValue(ThursdayProperty, value); }
        }
        /// <summary>
        /// Gets or sets the label for Thursday
        /// </summary>
        /// <value>
        /// The label for Thursday
        /// </value>
        public static readonly DependencyProperty ThursdayProperty =
            DependencyProperty.Register("Thursday", typeof(string), typeof(Calendar), new PropertyMetadata("Th"));


        /// <summary>
        /// Gets or sets the label for Friday
        /// </summary>
        /// <value>
        /// The label for Friday
        /// </value>
        public string Friday
        {
            get { return (string)GetValue(FridayProperty); }
            set { SetValue(FridayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Friday
        /// </summary>
        /// <value>
        /// The label for Friday
        /// </value>
        public static readonly DependencyProperty FridayProperty =
            DependencyProperty.Register("Friday", typeof(string), typeof(Calendar), new PropertyMetadata("Fr"));



        /// <summary>
        /// Gets or sets the label for Saturday
        /// </summary>
        /// <value>
        /// The label for Saturday
        /// </value>
        public string Saturday
        {
            get { return (string)GetValue(SaturdayProperty); }
            set { SetValue(SaturdayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Saturday
        /// </summary>
        /// <value>
        /// The label for Saturday
        /// </value>
        public static readonly DependencyProperty SaturdayProperty =
            DependencyProperty.Register("Saturday", typeof(string), typeof(Calendar), new PropertyMetadata("Sa"));



        /// <summary>
        /// Minimum Date that calendar navigation supports
        /// </summary>
        public DateTime MinimumDate
        {
            get { return (DateTime)GetValue(MinimumDateProperty); }
            set { SetValue(MinimumDateProperty, value); }
        }
        /// <summary>
        /// Minimum Date that calendar navigation supports
        /// </summary>
        public static readonly DependencyProperty MinimumDateProperty =
            DependencyProperty.Register("MinimumDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(new DateTime(1753, 1, 1)));


        /// <summary>
        /// Maximum Date that calendar navigation supports
        /// </summary>
        public DateTime MaximumDate
        {
            get { return (DateTime)GetValue(MaximumDateProperty); }
            set { SetValue(MaximumDateProperty, value); }
        }

        /// <summary>
        /// Maximum Date that calendar navigation supports
        /// </summary>
        public static readonly DependencyProperty MaximumDateProperty =
            DependencyProperty.Register("MaximumDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(new DateTime(2499, 12, 31)));



        #endregion

        #region Template

        /// <summary>
        /// Apply default template and perform initialization
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var previousButton = GetTemplateChild("PreviousMonthButton") as Button;
            if (previousButton != null) previousButton.Click += PreviousButtonClick;
            var nextButton = GetTemplateChild("NextMonthButton") as Button;
            if (nextButton != null) nextButton.Click += NextButtonClick;
            _itemsGrid = GetTemplateChild("ItemsGrid") as Grid;
            BuildDates();
            SetYearMonthLabel();
        }

        #endregion

        #region Event handling

        void NextButtonClick(object sender, RoutedEventArgs e)
        {
            IncrementMonth();
        }

        private void IncrementMonth()
        {
            if (CanMoveToMonthYear(_year, _month + 1))
            {
                _month += 1;
                if (_month == 13)
                {
                    _month = 1;
                    _year += 1;
                }
                SetYearMonthLabel();
            }
        }

        void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            DecrementMonth();
        }

        private void DecrementMonth()
        {
            if (CanMoveToMonthYear(_year, _month - 1))
            {
                _month -= 1;
                if (_month == 0)
                {
                    _month = 12;
                    _year -= 1;
                }
                SetYearMonthLabel();
            }
        }

        private void IncrementYear()
        {
            if (CanMoveToMonthYear(_year + 1, _month))
            {
                _year += 1;
                SetYearMonthLabel();
            }
        }

        private void DecrementYear()
        {
            if (CanMoveToMonthYear(_year - 1, _month))
            {
                _year -= 1;
                SetYearMonthLabel();
            }
        }

        private bool CanMoveToMonthYear(int year, int month)
        {
            var returnValue = false;
            if (month == 0)
            {
                year = year - 1;
                month = 12;
            }
            else if (month == 13)
            {
                month = 1;
                year = year + 1;
            }
            var testDate = new DateTime(year, month, 1);
            if (testDate >= MinimumDate && testDate <= MaximumDate)
            {
                returnValue = true;
            }
            return returnValue;
        }

        private void ItemClick(object sender, RoutedEventArgs e)
        {
            if (_lastItem != null)
            {
                _lastItem.IsSelected = false;
            }
            _lastItem = (sender as CalendarItem);
            if (_lastItem != null)
            {
                if (ShowSelectedDate)
                    _lastItem.IsSelected = true;
                SelectedDate = _lastItem.ItemDate;
                OnDateClicked(_lastItem.ItemDate);
            }
        }

        #endregion

        #region Methods
        private bool _ignoreMonthChange;
        private void SetYearMonthLabel()
        {
            OnMonthChanging(_year, _month);
            YearMonthLabel = string.Concat(GetMonthName(), " ", _year.ToString());
            _ignoreMonthChange = true;
            SelectedMonth = _month;
            SelectedYear = _year;
            _ignoreMonthChange = false;
            BuildItems();
            OnMonthChanged(_year, _month);
        }

        private readonly DateTimeFormatInfo _dateTimeFormatInfo;

        private string GetMonthName()
        {
            string returnValue = _dateTimeFormatInfo.MonthNames[_month - 1];
            return returnValue;
        }

        private void SetupDaysOfWeekLabels()
        {
            Sunday = _dateTimeFormatInfo.AbbreviatedDayNames[0];
            Monday = _dateTimeFormatInfo.AbbreviatedDayNames[1];
            Tuesday = _dateTimeFormatInfo.AbbreviatedDayNames[2];
            Wednesday = _dateTimeFormatInfo.AbbreviatedDayNames[3];
            Thursday = _dateTimeFormatInfo.AbbreviatedDayNames[4];
            Friday = _dateTimeFormatInfo.AbbreviatedDayNames[5];
            Saturday = _dateTimeFormatInfo.AbbreviatedDayNames[6];
        }

        private void BuildItems()
        {
            if (_itemsGrid != null)
            {
                AddDefaultItems();
                var startOfMonth = new DateTime(_year, _month, 1);
                DayOfWeek dayOfWeek = startOfMonth.DayOfWeek;
                var daysInMonth = (int)Math.Floor(startOfMonth.AddMonths(1).Subtract(startOfMonth).TotalDays);
                var addedDays = 0;
                int lastWeekNumber = 0;
                for (int rowCount = 1; rowCount <= RowCount; rowCount++)
                {
                    for (var columnCount = 1; columnCount < ColumnCount; columnCount++)
                    {
                        var item = (CalendarItem)(from oneChild in _itemsGrid.Children
                                                  where oneChild is CalendarItem &&
                                                  ((CalendarItem)oneChild).Tag.ToString() == string.Concat(rowCount.ToString(), ":", columnCount.ToString())
                                                  select oneChild).First();
                        if (rowCount == 1 && columnCount < (int)dayOfWeek + 1)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                        else if (addedDays < daysInMonth)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            item.Visibility = Visibility.Collapsed;
                        }

                        var weekItem = (CalendarWeekItem)(from oneChild in _itemsGrid.Children
                                                          where oneChild is CalendarWeekItem &&
                                                          ((CalendarWeekItem)oneChild).Tag.ToString() == string.Concat(rowCount.ToString(), ":0")
                                                          select oneChild).FirstOrDefault();

                        if (item.Visibility == Visibility.Visible)
                        {
                            item.ItemDate = startOfMonth.AddDays(addedDays);
                            if (SelectedDate == DateTime.MinValue && item.ItemDate == DateTime.Today)
                            {
                                SelectedDate = item.ItemDate;
                                if (ShowSelectedDate)
                                    item.IsSelected = true;
                                _lastItem = item;
                            }
                            else
                            {
                                if (item.ItemDate == SelectedDate)
                                {
                                    if (ShowSelectedDate)
                                        item.IsSelected = true;
                                }
                                else
                                {
                                    item.IsSelected = false;
                                }
                            }
                            addedDays += 1;
                            item.DayNumber = addedDays;
                            item.SetBackcolor();
                            item.SetForecolor();


                            if (WeekNumberDisplay != WeekNumberDisplayOption.None)
                            {
                                int weekNumber;

                                if (WeekNumberDisplay == WeekNumberDisplayOption.WeekOfYear)
                                {
                                    var systemCalendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
                                    weekNumber = systemCalendar.GetWeekOfYear(
                                        item.ItemDate,
                                        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                                        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
                                }
                                else
                                {
                                    weekNumber = rowCount;
                                }
                                if (weekItem != null)
                                {
                                    weekItem.WeekNumber = weekNumber;
                                    lastWeekNumber = weekNumber;
                                    weekItem.Visibility = Visibility.Visible;
                                }
                            }
                        }
                        else
                        {
                            if (WeekNumberDisplay != WeekNumberDisplayOption.None && weekItem != null && weekItem.WeekNumber != lastWeekNumber)
                            {
                                weekItem.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }

        private void AddDefaultItems()
        {
            if (!_addedItems && _itemsGrid != null)
            {
                for (int rowCount = 1; rowCount <= RowCount; rowCount++)
                {
                    for (int columnCount = 1; columnCount < ColumnCount; columnCount++)
                    {
                        var item = new CalendarItem(this);
                        item.SetValue(Grid.RowProperty, rowCount);
                        item.SetValue(Grid.ColumnProperty, columnCount);
                        item.Visibility = Visibility.Collapsed;
                        item.Tag = string.Concat(rowCount.ToString(), ":", columnCount.ToString());
                        item.Click += ItemClick;
                        if (CalendarItemStyle != null)
                        {
                            item.Style = CalendarItemStyle;
                        }
                        _itemsGrid.Children.Add(item);
                    }
                    if (WeekNumberDisplay != WeekNumberDisplayOption.None)
                    {
                        const int columnCount = 0;
                        var item = new CalendarWeekItem();
                        item.SetValue(Grid.RowProperty, rowCount);
                        item.SetValue(Grid.ColumnProperty, columnCount);
                        item.Visibility = Visibility.Collapsed;
                        item.Tag = string.Concat(rowCount.ToString(), ":", columnCount.ToString());
                        if (CalendarWeekItemStyle != null)
                        {
                            item.Style = CalendarWeekItemStyle;
                        }
                        _itemsGrid.Children.Add(item);
                    }
                }
                _addedItems = true;
            }
        }

        private void BuildDates()
        {
            if (DatesSource != null)
            {
                DatesAssigned.Clear();
                //DatesSource.ToList().ForEach(one => DatesAssigned.Add(one.CalendarItemDate));
                foreach (var one in DatesSource.ToList())
                {
                    DatesAssigned.Add(one.CalendarItemDate);
                }
            }
        }
        
        #endregion


        
    }
}
