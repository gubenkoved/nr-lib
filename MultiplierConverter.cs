using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace NRLib
{
    public class MultiplierConverter : DependencyObject, IValueConverter
    {
        public double Multiplier
        {
            get { return (double)GetValue(MultiplierProperty); }
            set { SetValue(MultiplierProperty, value); }
        }
        public static readonly DependencyProperty MultiplierProperty = DependencyProperty.Register("Multiplier", typeof(double), typeof(MultiplierConverter), new UIPropertyMetadata(1.0));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Multiplier * (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
