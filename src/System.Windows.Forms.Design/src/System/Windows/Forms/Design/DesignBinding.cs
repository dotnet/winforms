// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal class DesignBinding
{
    private object? dataSource;
    private string? dataMember;

    public static DesignBinding Null = new(null, null);

    public DesignBinding(object? dataSource, string? dataMember)
    {
        this.dataSource = dataSource;
        this.dataMember = dataMember;
    }

    public bool IsNull
    {
        get
        {
            return (dataSource is null);
        }
    }

    public object? DataSource
    {
        get
        {
            return dataSource;
        }
    }

    public string? DataMember
    {
        get
        {
            return dataMember;
        }
    }

    public string DataField
    {
        get
        {
            if (string.IsNullOrEmpty(dataMember))
            {
                return string.Empty;
            }

            int lastDot = dataMember.LastIndexOf(".");
            if (lastDot == -1)
            {
                return dataMember;
            }
            else
            {
                return dataMember.Substring(lastDot + 1);
            }
        }
    }

    public bool Equals(object? dataSource, string dataMember)
    {
        return dataSource == this.dataSource && string.Equals(dataMember, this.dataMember, StringComparison.OrdinalIgnoreCase);
    }
}
