// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class Com2PropertyDescriptor
{
    /// <summary>
    ///  We wrap all value editors in this one so we can intercept the GetTextFromValue calls for objects that
    ///  we would like to modify the display name.
    /// </summary>
    private class Com2PropDescMainConverter : Com2ExtendedTypeConverter
    {
        private readonly Com2PropertyDescriptor _propertyDescriptor;

        private const int CheckSubprops = 0;
        private const int AllowSubprops = 1;
        private const int SuppressSubprops = 2;

        private int _subprops = CheckSubprops;

        public Com2PropDescMainConverter(
            Com2PropertyDescriptor propertyDescriptor,
            TypeConverter baseConverter) : base(baseConverter)
        {
            _propertyDescriptor = propertyDescriptor;
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            object? baseConversion = base.ConvertTo(context, culture, value, destinationType);

            // If this is our current value, ask if it should be changed for display,
            // otherwise we'll ask for our enum drop downs, which we don't want to do.
            if (destinationType == typeof(string)
                && _propertyDescriptor.IsLastKnownValue(value)
                && !(_propertyDescriptor.PropertyType?.IsEnum ?? false))
            {
                return GetWrappedConverter(typeof(Com2EnumConverter)) is Com2EnumConverter baseConverter
                    ? baseConverter.ConvertTo(value, destinationType)
                    : _propertyDescriptor.GetDisplayValue((string?)baseConversion);
            }

            return baseConversion;
        }

        [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
        public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);

            if (properties is not null && properties.Count > 0)
            {
                // Return sorted read-only collection (can't sort original because its read-only)
                properties = properties.Sort();
                PropertyDescriptor[] descriptors = new PropertyDescriptor[properties.Count];
                properties.CopyTo(descriptors, 0);
                properties = new PropertyDescriptorCollection(descriptors, true);
            }

            return properties;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
        {
            if (_subprops == CheckSubprops)
            {
                if (!base.GetPropertiesSupported(context))
                {
                    _subprops = SuppressSubprops;
                }
                else
                {
                    // Special case the font converter here.
                    if ((_propertyDescriptor._valueConverter is { } converter && converter.AllowExpand)
                        || Com2IVsPerPropertyBrowsingHandler.AllowChildProperties(_propertyDescriptor))
                    {
                        _subprops = AllowSubprops;
                    }
                }
            }

            return _subprops == AllowSubprops;
        }
    }
}
