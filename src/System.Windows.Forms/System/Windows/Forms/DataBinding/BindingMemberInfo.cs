// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public readonly struct BindingMemberInfo : IEquatable<BindingMemberInfo>
{
    private readonly string? _dataList;
    private readonly string? _dataField;

    public BindingMemberInfo(string? dataMember)
    {
        dataMember ??= string.Empty;

        int lastDot = dataMember.LastIndexOf('.');
        if (lastDot != -1)
        {
            _dataList = dataMember[..lastDot];
            _dataField = dataMember[(lastDot + 1)..];
        }
        else
        {
            _dataList = string.Empty;
            _dataField = dataMember;
        }
    }

    public string BindingPath => _dataList ?? string.Empty;

    public string BindingField => _dataField ?? string.Empty;

    public string BindingMember
        => BindingPath.Length > 0
            ? $"{BindingPath}.{BindingField}"
            : BindingField;

    public override bool Equals(object? otherObject)
    {
        if (otherObject is not BindingMemberInfo otherMember)
        {
            return false;
        }

        return Equals(otherMember);
    }

    public bool Equals(BindingMemberInfo other)
        => string.Equals(BindingMember, other.BindingMember, StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(BindingMemberInfo a, BindingMemberInfo b) => a.Equals(b);

    public static bool operator !=(BindingMemberInfo a, BindingMemberInfo b) => !a.Equals(b);

    public override int GetHashCode() => base.GetHashCode();
}
