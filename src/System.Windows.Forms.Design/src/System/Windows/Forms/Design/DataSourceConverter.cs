// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.Design;

internal class DataSourceConverter : ReferenceConverter
{
    public DataSourceConverter() : base(typeof(IListSource))
    {
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is null)
        {
            return SR.None_lc.ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
