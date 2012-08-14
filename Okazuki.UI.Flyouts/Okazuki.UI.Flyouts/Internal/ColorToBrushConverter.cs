using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Okazuki.UI.Flyouts.Internal
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is Color))
            {
                return DependencyProperty.UnsetValue;
            }

            if (!targetType.GetTypeInfo().IsAssignableFrom(typeof(SolidColorBrush).GetTypeInfo()))
            {
                return DependencyProperty.UnsetValue;
            }

            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
