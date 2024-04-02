// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridCellMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridCellDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public struct DataGridCell : IEquatable<DataGridCell>
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ColumnNumber
    {
        readonly get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int RowNumber
    {
        readonly get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public DataGridCell(int r, int c)
         => throw new PlatformNotSupportedException();

    public override readonly bool Equals(object obj)
         => throw new PlatformNotSupportedException();

    public override readonly int GetHashCode()
         => throw new PlatformNotSupportedException();

    public override readonly string ToString()
         => throw new PlatformNotSupportedException();

    public readonly bool Equals(DataGridCell other)
         => throw new PlatformNotSupportedException();

    public static bool operator ==(DataGridCell left, DataGridCell right)
         => throw new PlatformNotSupportedException();

    public static bool operator !=(DataGridCell left, DataGridCell right)
         => throw new PlatformNotSupportedException();
}
