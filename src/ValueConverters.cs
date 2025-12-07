using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AdvancedClock
{
    /// <summary>
    /// 布尔值到强提醒文本的转换器
    /// </summary>
    public class BoolToStrongAlertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isStrongAlert)
            {
                return isStrongAlert ? "强提醒" : "弱提醒";
            }
            return "弱提醒";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 布尔值到颜色的转换器
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isStrongAlert)
            {
                return isStrongAlert ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}