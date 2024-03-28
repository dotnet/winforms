// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms;

internal class TableLayoutPanelCellPositionTypeConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }

        return base.CanConvertTo(context, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }

        return base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            ReadOnlySpan<char> text = stringValue.AsSpan().Trim();
            if (text.IsEmpty)
            {
                return null;
            }

            // Parse 2 integer values.
            culture ??= CultureInfo.CurrentCulture;

            Span<int> values = stackalloc int[2];

            if (!TypeConverterHelper.TryParseAsSpan(context, culture, text, values))
            {
                throw new ArgumentException(
                    string.Format(
                        SR.TextParseFailedFormat,
                        stringValue,
                        "column, row"),
                    nameof(value));
            }

            return new TableLayoutPanelCellPosition(values[0], values[1]);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor) && value is TableLayoutPanelCellPosition cellPosition)
        {
            return new InstanceDescriptor(
                typeof(TableLayoutPanelCellPosition).GetConstructor([typeof(int), typeof(int)]),
                new object[] { cellPosition.Column, cellPosition.Row });
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object? CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
    {
        ArgumentNullException.ThrowIfNull(propertyValues);

        try
        {
            return new TableLayoutPanelCellPosition(
                (int)propertyValues[nameof(TableLayoutPanelCellPosition.Column)]!,
                (int)propertyValues[nameof(TableLayoutPanelCellPosition.Row)]!);
        }
        catch (InvalidCastException invalidCast)
        {
            throw new ArgumentException(SR.PropertyValueInvalidEntry, nameof(propertyValues), invalidCast);
        }
        catch (NullReferenceException nullRef)
        {
            throw new ArgumentException(SR.PropertyValueInvalidEntry, nameof(propertyValues), nullRef);
        }
    }

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context) => true;

    [RequiresUnreferencedCode(TrimmingConstants.TypeOrValueNotDiscoverableMessage)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields, typeof(BrowsableAttribute))]
    public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
    {
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TableLayoutPanelCellPosition), attributes);
        return props.Sort([nameof(TableLayoutPanelCellPosition.Column), nameof(TableLayoutPanelCellPosition.Row)]);
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;
}
