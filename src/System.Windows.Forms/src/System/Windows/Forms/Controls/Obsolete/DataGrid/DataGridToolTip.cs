// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridToolTipMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridToolTipDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
internal class DataGridToolTip : MarshalByRefObject
{
    public DataGridToolTip(DataGrid dataGrid)
        => throw new PlatformNotSupportedException();

    public static void CreateToolTipHandle()
        => throw new PlatformNotSupportedException();

    public static void AddToolTip(string toolTipString, IntPtr toolTipId, Rectangle iconBounds)
        => throw new PlatformNotSupportedException();

    public static void RemoveToolTip(IntPtr toolTipId)
        => throw new PlatformNotSupportedException();

    public static void Destroy()
        => throw new PlatformNotSupportedException();
}
