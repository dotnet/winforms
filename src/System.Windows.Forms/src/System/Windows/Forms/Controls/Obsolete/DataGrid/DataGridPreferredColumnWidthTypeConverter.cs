// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete("DataGridPreferredColumnWidthTypeConverter has been deprecated.")]
#nullable disable
public class DataGridPreferredColumnWidthTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => throw new PlatformNotSupportedException();

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => throw new PlatformNotSupportedException();

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => throw new PlatformNotSupportedException();
}
