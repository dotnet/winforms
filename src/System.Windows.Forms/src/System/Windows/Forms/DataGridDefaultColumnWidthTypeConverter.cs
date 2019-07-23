// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    public class DataGridPreferredColumnWidthTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(int))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value.GetType() == typeof(int))
                {
                    int pulica = (int)value;
                    if (pulica == -1)
                    {
                        return "AutoColumnResize (-1)";
                    }
                    else
                    {
                        return pulica.ToString(CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                    return base.ConvertTo(context, culture, value, destinationType);
                }
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string text = value.ToString();
                if (text.Equals("AutoColumnResize (-1)"))
                {
                    return -1;
                }
                else
                {
                    return int.Parse(text, CultureInfo.CurrentCulture);
                }
            }
            else if (value.GetType() == typeof(int))
            {
                return (int)value;
            }
            else
            {
                throw GetConvertFromException(value);
            }
        }
    }
}
