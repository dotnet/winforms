// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

internal class DataGridViewColumnConverter : ExpandableObjectConverter
{
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

        if (destinationType == typeof(InstanceDescriptor) && value is DataGridViewColumn dataGridViewColumn)
        {
            ConstructorInfo? ctor;

            // public DataGridViewColumn(Type cellType)
            //
            if (dataGridViewColumn.CellType is not null)
            {
                ctor = dataGridViewColumn.GetType().GetConstructor([typeof(Type)]);
                if (ctor is not null)
                {
                    return new InstanceDescriptor(ctor, new object[] { dataGridViewColumn.CellType }, false);
                }
            }

            // public DataGridViewColumn()
            //
            ctor = dataGridViewColumn.GetType().GetConstructor([]);
            if (ctor is not null)
            {
                return new InstanceDescriptor(ctor, Array.Empty<object>(), false);
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
