// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

public class ColumnHeaderConverter : ExpandableObjectConverter
{
    /// <summary>
    ///  Gets a value indicating whether this converter can
    ///  convert an object to the given destination type using the context.
    /// </summary>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }

        return base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    ///  Converts the given object to another type. The most common types to convert
    ///  are to and from a string object. The default implementation will make a call
    ///  to ToString on the object if the object is valid and if the destination
    ///  type is string. If this cannot convert to the destination type, this will
    ///  throw a NotSupportedException.
    /// </summary>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(InstanceDescriptor) && value is ColumnHeader col)
        {
            ConstructorInfo? ctor;

            Type t = TypeDescriptor.GetReflectionType(value);
            InstanceDescriptor? id = null;

            if (col.ImageIndex != -1)
            {
                ctor = t.GetConstructor([typeof(int)]);
                if (ctor is not null)
                {
                    id = new InstanceDescriptor(ctor, new object[] { col.ImageIndex }, false);
                }
            }

            if (id is null && !string.IsNullOrEmpty(col.ImageKey))
            {
                ctor = t.GetConstructor([typeof(string)]);
                if (ctor is not null)
                {
                    id = new InstanceDescriptor(ctor, new object[] { col.ImageKey }, false);
                }
            }

            if (id is null)
            {
                ctor = t.GetConstructor([]);
                if (ctor is not null)
                {
                    return new InstanceDescriptor(ctor, Array.Empty<object>(), false);
                }
                else
                {
                    throw new ArgumentException(string.Format(SR.NoDefaultConstructor, t.FullName));
                }
            }

            return id;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
