// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGridCell has been deprecated.")]
public struct DataGridCell : IEquatable<DataGridCell>
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int ColumnNumber
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int RowNumber
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public DataGridCell(int r, int c)
    {
        throw new PlatformNotSupportedException();
    }

    public override bool Equals(object o)
    {
        throw new PlatformNotSupportedException();
    }

    public override int GetHashCode()
    {
        throw new PlatformNotSupportedException();
    }

    public override string ToString()
    {
        throw new PlatformNotSupportedException();
    }

    public bool Equals(DataGridCell other)
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(DataGridCell left, DataGridCell right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DataGridCell left, DataGridCell right)
    {
        return !(left == right);
    }
}
