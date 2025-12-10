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

    /// <summary>
    /// 十六进制颜色字符串到画刷的转换器
    /// </summary>
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom(colorString)!;
                }
                catch
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 枚举转布尔值的转换器（用于RadioButton绑定）
    /// </summary>
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string enumValue = value.ToString() ?? string.Empty;
            string targetValue = parameter.ToString() ?? string.Empty;
            
            return enumValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter != null)
            {
                return Enum.Parse(targetType, parameter.ToString()!);
            }
            return Binding.DoNothing;
        }
    }
}