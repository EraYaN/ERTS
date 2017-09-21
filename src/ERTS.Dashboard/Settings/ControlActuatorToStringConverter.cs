using ERTS.Dashboard.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ERTS.Dashboard.Configuration
{
    public class ControlActuatorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                object parameter, CultureInfo culture)
        {
            ControlActuator ca = (ControlActuator)value;
            return String.Format("{0}", ca.ControlDisplayName);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
