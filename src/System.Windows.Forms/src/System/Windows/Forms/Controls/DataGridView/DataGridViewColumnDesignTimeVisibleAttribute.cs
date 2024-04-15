// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DataGridViewColumnDesignTimeVisibleAttribute : Attribute
{
    public DataGridViewColumnDesignTimeVisibleAttribute()
    {
    }

    public DataGridViewColumnDesignTimeVisibleAttribute(bool visible) => Visible = visible;

    public bool Visible { get; }

    public static readonly DataGridViewColumnDesignTimeVisibleAttribute Yes = new(true);

    public static readonly DataGridViewColumnDesignTimeVisibleAttribute No = new(false);

    public static readonly DataGridViewColumnDesignTimeVisibleAttribute Default = Yes;

    public override bool Equals(object? obj) =>
        obj == this || (obj is DataGridViewColumnDesignTimeVisibleAttribute other && other.Visible == Visible);

    public override int GetHashCode() => HashCode.Combine(typeof(DataGridViewColumnDesignTimeVisibleAttribute), Visible);

    public override bool IsDefaultAttribute() => Visible == Default.Visible;
}
