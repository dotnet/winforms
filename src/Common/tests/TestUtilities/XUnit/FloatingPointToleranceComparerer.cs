// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;

namespace System.XUnit;

/// <summary>
///  Equality comparer for numbers that checks whether two values are within a specifed tolerance.
/// </summary>
public class FloatingPointToleranceComparerer<T> : IEqualityComparer<T>
    where T : struct, IFloatingPoint<T>
{
    private readonly T _tolerance;

    /// <param name="tolerance">The difference must be less than or equal to this number.</param>
    public FloatingPointToleranceComparerer(T tolerance) => _tolerance = T.Abs(tolerance);

    public bool Equals(T x, T y) => ComparisonHelpers.EqualsFloating(x, y, _tolerance);

    public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
}
