// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  Base class for value editors that extend basic functionality. Calls will be delegated to the "base value editor".
/// </summary>
internal class Com2ExtendedTypeConverter : TypeConverter
{
    private readonly TypeConverter? _innerConverter;

    public Com2ExtendedTypeConverter(TypeConverter? innerConverter)
    {
        _innerConverter = innerConverter;
    }

    public Com2ExtendedTypeConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type baseType)
    {
        _innerConverter = TypeDescriptor.GetConverter(baseType);
    }

    public TypeConverter? InnerConverter => _innerConverter;

    public TypeConverter? GetWrappedConverter(Type t)
    {
        TypeConverter? converter = _innerConverter;

        while (converter is not null)
        {
            if (t.IsInstanceOfType(converter))
            {
                return converter;
            }

            if (converter is Com2ExtendedTypeConverter com2ExtendedTypeConverter)
            {
                converter = com2ExtendedTypeConverter.InnerConverter;
            }
            else
            {
                break;
            }
        }

        return null;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => _innerConverter is not null
            ? _innerConverter.CanConvertFrom(context, sourceType)
            : base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => _innerConverter is not null
            ? _innerConverter.CanConvertTo(context, destinationType)
            : base.CanConvertTo(context, destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => _innerConverter is not null
            ? _innerConverter.ConvertFrom(context, culture, value)
            : base.ConvertFrom(context, culture, value);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        => _innerConverter is not null
            ? _innerConverter.ConvertTo(context, culture, value, destinationType)
            : base.ConvertTo(context, culture, value, destinationType);

    public override object? CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
        => _innerConverter is not null
            ? _innerConverter.CreateInstance(context, propertyValues)
            : base.CreateInstance(context, propertyValues);

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context)
        => _innerConverter is not null
            ? _innerConverter.GetCreateInstanceSupported(context)
            : base.GetCreateInstanceSupported(context);

    [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
    public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        => _innerConverter is not null
            ? _innerConverter.GetProperties(context, value, attributes)
            : base.GetProperties(context, value, attributes);

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
        => _innerConverter is not null
            ? _innerConverter.GetPropertiesSupported(context)
            : base.GetPropertiesSupported(context);

    public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
        => _innerConverter is not null
            ? _innerConverter.GetStandardValues(context)
            : base.GetStandardValues(context);

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        => _innerConverter is not null
            ? _innerConverter.GetStandardValuesExclusive(context)
            : base.GetStandardValuesExclusive(context);

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        => _innerConverter is not null
            ? _innerConverter.GetStandardValuesSupported(context)
            : base.GetStandardValuesSupported(context);

    public override bool IsValid(ITypeDescriptorContext? context, object? value)
        => _innerConverter is not null
            ? _innerConverter.IsValid(context, value)
            : base.IsValid(context, value);
}
