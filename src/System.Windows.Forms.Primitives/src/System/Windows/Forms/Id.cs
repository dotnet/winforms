// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
///  Positive <see langword="int"/> enforcing identifier.
/// </summary>
/// <devdoc>
///  Idea here is that doing this makes it less likely we'll slip through cases where
///  we don't check for negative numbers. And also not confuse counts with ids.
/// </devdoc>
internal readonly struct Id : IEquatable<Id>
{
    private readonly int _id;

    private Id(int id) => _id = id.OrThrowIfNegative();

    public static implicit operator int(Id value) => value._id;
    public static implicit operator Id(int value) => new(value);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is Id id && Equals(id)) || (obj is int value && value == _id);

    public bool Equals(Id other) => _id == other._id;

    public override readonly int GetHashCode() => _id.GetHashCode();
    public override readonly string ToString() => _id.ToString();

    public static bool operator ==(Id left, Id right) => left._id == right._id;

    public static bool operator !=(Id left, Id right) => !(left == right);
}
