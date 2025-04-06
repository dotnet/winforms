// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms;

public sealed partial class TableLayoutSettings
{
    internal class StyleConverter : TypeConverter
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
            if (destinationType == typeof(InstanceDescriptor) && value is TableLayoutStyle style)
            {
                switch (style.SizeType)
                {
                    case SizeType.AutoSize:
                        return new InstanceDescriptor(
                            style.GetTypeWithConstructor().GetConstructor([]),
                            Array.Empty<object>());
                    case SizeType.Absolute:
                    case SizeType.Percent:
                        return new InstanceDescriptor(
                            style.GetTypeWithConstructor().GetConstructor([typeof(SizeType), typeof(int)]),
                            new object[] { style.SizeType, style.Size });
                    default:
                        break;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
