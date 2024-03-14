// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal class DesignBinding
{
    public static DesignBinding Null { get; } = new(null, null);

    public DesignBinding(object? dataSource, string? dataMember)
    {
        DataSource = dataSource;
        DataMember = dataMember;
    }

    public bool IsNull => DataSource is null;

    public object? DataSource { get; }

    public string? DataMember { get; }

    public string DataField
    {
        get
        {
            if (string.IsNullOrEmpty(DataMember))
            {
                return string.Empty;
            }

            int lastDot = DataMember.LastIndexOf('.');
            if (lastDot == -1)
            {
                return DataMember;
            }
            else
            {
                return DataMember[(lastDot + 1)..];
            }
        }
    }

    public bool Equals(object? dataSource, string dataMember) => dataSource == DataSource && string.Equals(dataMember, DataMember, StringComparison.OrdinalIgnoreCase);
}
