// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal class Com2EnumConverter : TypeConverter
{
    internal readonly Com2Enum _com2Enum;
    private StandardValuesCollection? _values;

    public Com2EnumConverter(Com2Enum enumObj) => _com2Enum = enumObj;

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destType)
        => base.CanConvertTo(context, destType) || (destType is not null && destType.IsEnum);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string valueAsString ? _com2Enum.FromString(valueAsString) : base.ConvertFrom(context, culture, value);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string) && value is not null)
        {
            return _com2Enum.ToString(value) ?? string.Empty;
        }

        return value is not null && destinationType.IsEnum
            ? Enum.ToObject(destinationType, value)
            : base.ConvertTo(context, culture, value, destinationType);
    }

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

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

    public override bool IsValid(ITypeDescriptorContext? context, object? value)
        => !string.IsNullOrEmpty(_com2Enum.ToString(value));

    public void RefreshValues() => _values = null;
}
