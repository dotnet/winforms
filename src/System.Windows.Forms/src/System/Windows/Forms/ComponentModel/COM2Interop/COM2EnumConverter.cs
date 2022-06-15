// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2EnumConverter : TypeConverter
    {
        internal readonly Com2Enum _com2Enum;
        private StandardValuesCollection? _values;

        public Com2EnumConverter(Com2Enum enumObj)
        {
            _com2Enum = enumObj;
        }

        /// <summary>
        ///  Determines if this converter can convert an object in the given source
        ///  type to the native type of the converter.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destType)
        {
            if (base.CanConvertTo(context, destType))
            {
                return true;
            }

            return destType is not null && destType.IsEnum;
        }

        /// <summary>
        ///  Converts the given object to the converter's native type.
        /// </summary>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string valueAsString)
            {
                return _com2Enum.FromString(valueAsString);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///  Converts the given object to another type.  The most common types to convert
        ///  are to and from a string object.  The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string.  If this cannot convert to the destination type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            ArgumentNullException.ThrowIfNull(destinationType);

            if (destinationType == typeof(string))
            {
                if (value is not null)
                {
                    string str = _com2Enum.ToString(value);
                    return str ?? string.Empty;
                }
            }

            if (value is not null && destinationType.IsEnum)
            {
                return Enum.ToObject(destinationType, value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///  Retrieves a collection containing a set of standard values
        ///  for the data type this validator is designed for.  This
        ///  will return null if the data type does not support a
        ///  standard set of values.
        /// </summary>
        public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
        {
            if (_values is null)
            {
                object[] objValues = _com2Enum.Values;
                if (objValues is not null)
                {
                    _values = new StandardValuesCollection(objValues);
                }
            }

            return _values;
        }

        /// <summary>
        ///  Determines if the list of standard values returned from
        ///  GetStandardValues is an exclusive list.  If the list
        ///  is exclusive, then no other values are valid, such as
        ///  in an enum data type.  If the list is not exclusive,
        ///  then there are other valid values besides the list of
        ///  standard values GetStandardValues provides.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

        /// <summary>
        ///  Determines if this object supports a standard set of values
        ///  that can be picked from a list.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        /// <summary>
        ///  Determines if the given object value is valid for this type.
        /// </summary>
        public override bool IsValid(ITypeDescriptorContext? context, object? value)
        {
            string strValue = _com2Enum.ToString(value);
            return strValue is not null && strValue.Length > 0;
        }

        public void RefreshValues()
        {
            _values = null;
        }
    }
}
