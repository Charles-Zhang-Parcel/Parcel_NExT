﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Parcel.Neo.Converters
{
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;
        public bool Negate { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value?.ToString();
            if (bool.TryParse(stringValue, out var b))
            {
                return (Negate ? !b : b) ? Visibility.Visible : FalseVisibility;
            }
            else if (double.TryParse(stringValue, out var d))
            {
                return (Negate ? (d <= 0) : (d > 0)) ? Visibility.Visible : FalseVisibility;
            }
            else if (value is string s)
            {
                bool empty = string.IsNullOrWhiteSpace(s);
                return (Negate ? !empty : empty) ? FalseVisibility : Visibility.Visible;
            }

            bool hasResult = value != null;
            return (Negate ? !hasResult : hasResult) ? Visibility.Visible: FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility v && v == Visibility.Visible;

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
