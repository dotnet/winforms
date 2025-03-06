// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

internal class FlatButtonAppearanceConverter : ExpandableObjectConverter
{
    // Don't let the property grid display the full type name in the value cell
    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
        destinationType == typeof(string) ? "" : base.ConvertTo(context, culture, value, destinationType)!;

    // Don't let the property grid display the CheckedBackColor property for Button controls
    [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
    {
        if (context is not null && attributes is not null && context.Instance is Button)
        {
            Attribute[] attributes2 = new Attribute[attributes.Length + 1];
            attributes.CopyTo(attributes2, 0);
            attributes2[attributes.Length] = new ApplicableToButtonAttribute();
            attributes = attributes2;
        }

        return TypeDescriptor.GetProperties(value, attributes);
    }
}
