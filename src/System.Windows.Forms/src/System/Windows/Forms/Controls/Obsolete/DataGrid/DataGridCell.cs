﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridCellMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridCellDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public struct DataGridCell : IEquatable<DataGridCell>
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning restore CA1815 // Override equals and operator equals on value types
#pragma warning restore CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
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

    public DataGridCell(int r, int c) => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public readonly bool Equals(object obj) => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public readonly int GetHashCode() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public readonly string ToString() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

    public readonly bool Equals(DataGridCell other) => throw new PlatformNotSupportedException();

    public static bool operator == (DataGridCell left, DataGridCell right) => throw new PlatformNotSupportedException();

    public static bool operator != (DataGridCell left, DataGridCell right) => throw new PlatformNotSupportedException();
}
