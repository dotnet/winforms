// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

[RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses ComNativeDescriptor which is not trim-compatible.")]
internal unsafe class Com2IDispatchConverter : Com2ExtendedTypeConverter
{
    private readonly bool _allowExpand;

    public Com2IDispatchConverter(bool allowExpand, TypeConverter? baseConverter)
        : base(baseConverter)
    {
        _allowExpand = allowExpand;
    }

    public Com2IDispatchConverter(Com2PropertyDescriptor propertyDescriptor, bool allowExpand)
        // This will throw in the base.
        : base(propertyDescriptor.PropertyType!)
    {
        _allowExpand = allowExpand;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => false;

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        if (value is null)
        {
            return SR.toStringNone;
        }

        string? text = null;
        using var dispatch = ComHelpers.TryGetComScope<IDispatch>(value, out HRESULT hr);
        if (hr.Succeeded)
        {
            text = ComNativeDescriptor.GetName(dispatch);
        }

        if (string.IsNullOrEmpty(text))
        {
            text = ComNativeDescriptor.GetClassName(value);
        }

        return text is null ? "(Object)" : (object)text;
    }

    [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        => TypeDescriptor.GetProperties(value, attributes);

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => _allowExpand;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        // No dropdown, please!
        => false;
}
