using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VityazReports.Helpers {
    public class ColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            System.Windows.Media.Color? color = new System.Windows.Media.Color?();
            color = value as System.Windows.Media.Color?;
            System.Drawing.Color c = new System.Drawing.Color();
            c = System.Drawing.Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B);
            System.Windows.Media.Color converted = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
            return new SolidColorBrush(converted);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
