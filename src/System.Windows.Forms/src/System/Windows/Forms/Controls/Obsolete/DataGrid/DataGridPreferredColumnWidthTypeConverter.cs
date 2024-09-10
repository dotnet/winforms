// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016
// Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridPreferredColumnWidthTypeConverterMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridPreferredColumnWidthTypeConverterDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class DataGridPreferredColumnWidthTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => throw new PlatformNotSupportedException();

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
        throw new PlatformNotSupportedException();

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => throw new PlatformNotSupportedException();
}
