// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

internal class ListViewSubItemConverter : ExpandableObjectConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }

        return base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem.ListViewSubItem item)
        {
            ConstructorInfo? ctor;

            // Subitem has custom style
            if (item.CustomStyle)
            {
                ctor = typeof(ListViewItem.ListViewSubItem).GetConstructor(
                [
                    typeof(ListViewItem),
                    typeof(string),
                    typeof(Color),
                    typeof(Color),
                    typeof(Font)
                ]);
                Debug.Assert(ctor is not null, "Expected the constructor to exist.");
                return new InstanceDescriptor(ctor, new object?[]
                {
                    null,
                    item.Text,
                    item.ForeColor,
                    item.BackColor,
                    item.Font
                }, true);
            }

            // Otherwise, just use the text constructor
            ctor = typeof(ListViewItem.ListViewSubItem).GetConstructor([typeof(ListViewItem), typeof(string)]);
            Debug.Assert(ctor is not null, "Expected the constructor to exist.");
            return new InstanceDescriptor(ctor, new object?[] { null, item.Text }, true);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
