// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class DataGridPreferredColumnWidthTypeConverter : TypeConverter
{
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => throw new PlatformNotSupportedException();

    public object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
        throw new PlatformNotSupportedException();

    public object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => throw new PlatformNotSupportedException();
#pragma warning restore CS0114
}
