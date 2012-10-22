"Calendar.cs > SourcePropertyChanged()"  is used to check if there is any change occured in ViewModel. If DatesSource binded with calendar is changed, then we have to refresh the Calendar.

As GetBindingExpression() is not available in Windows RT, we have to hardcode the property name which is bind with DatesSource.