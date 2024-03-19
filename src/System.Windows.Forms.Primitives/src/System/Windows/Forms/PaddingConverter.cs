// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Windows.Forms.Primitives.Resources;

namespace System.Windows.Forms;

public class PaddingConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string stringValue)
        {
            return base.ConvertFrom(context, culture, value);
        }

        ReadOnlySpan<char> text = stringValue.AsSpan().Trim();

        if (text.IsEmpty)
        {
            return null;
        }

        // Parse 4 integer values.
        culture ??= CultureInfo.CurrentCulture;
        Span<int> values = stackalloc int[4];

        return TypeConverterHelper.TryParseAsSpan(context, culture, text, values)
            ? (object)new Padding(values[0], values[1], values[2], values[3])
            : throw new ArgumentException(string.Format(SR.TextParseFailedFormat, stringValue, "left, top, right, bottom"), nameof(value));
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Padding padding)
        {
            if (destinationType == typeof(string))
            {
                culture ??= CultureInfo.CurrentCulture;
                return string.Join(
                    $"{culture.TextInfo.ListSeparator} ",
                    new object[] { padding.Left, padding.Top, padding.Right, padding.Bottom });
            }
            else if (destinationType == typeof(InstanceDescriptor))
            {
                return padding.ShouldSerializeAll()
                    ? new InstanceDescriptor(
                        typeof(Padding).GetConstructor([typeof(int)]),
                        new object[] { padding.All })
                    : new InstanceDescriptor(
                        typeof(Padding).GetConstructor([typeof(int), typeof(int), typeof(int), typeof(int)]),
                        new object[] { padding.Left, padding.Top, padding.Right, padding.Bottom });
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
    {
        ArgumentNullException.ThrowIfNull(propertyValues);

        object? original = context?.PropertyDescriptor?.GetValue(context.Instance);
        try
        {
            // When incrementally changing an existing Padding instance e.g. through a PropertyGrid
            // the expected behavior is that a change of Padding.All will override the now outdated
            // other properties of the original padding.
            //
            // If we are not incrementally changing an existing instance (i.e. have no original value)
            // then we can just select the individual components passed in the full set of properties
            // and the Padding constructor will determine whether they are all the same or not.

            if (original is Padding originalPadding)
            {
                int all = (int)propertyValues[nameof(Padding.All)]!;
                if (originalPadding.All != all)
                {
                    return new Padding(all);
                }
            }

            return new Padding(
                (int)propertyValues[nameof(Padding.Left)]!,
                (int)propertyValues[nameof(Padding.Top)]!,
                (int)propertyValues[nameof(Padding.Right)]!,
                (int)propertyValues[nameof(Padding.Bottom)]!);
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

    [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Padding), attributes);
        return properties.Sort([nameof(Padding.All), nameof(Padding.Left), nameof(Padding.Top), nameof(Padding.Right), nameof(Padding.Bottom)]);
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;
}
