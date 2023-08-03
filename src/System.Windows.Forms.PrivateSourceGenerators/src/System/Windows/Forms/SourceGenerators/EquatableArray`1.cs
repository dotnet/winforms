// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;

namespace System.Windows.Forms.PrivateSourceGenerators;

internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>
    where T : IEquatable<T>
{
    public ImmutableArray<T> Values { get; }

    public EquatableArray(ImmutableArray<T> values)
    {
        Values = values;
    }

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !(left == right);
    }

    public ImmutableArray<T>.Enumerator GetEnumerator()
        => Values.GetEnumerator();

    public bool Equals(EquatableArray<T> other)
    {
        return Values.SequenceEqual(other.Values);
    }

    public override bool Equals(object obj)
    {
        return obj is EquatableArray<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        int hashCode = 644276958;
        foreach (var value in Values)
        {
            hashCode = hashCode * -1521134295 + (value?.GetHashCode() ?? 0);
        }

        return hashCode;
    }
}
