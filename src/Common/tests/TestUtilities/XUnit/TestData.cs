// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Numerics;
namespace Xunit;

/// <summary>
///  Raw, cached test data.
/// </summary>
public static class TestData
{
    /// <summary>
    ///  Returns a variety of floating point test cases.
    /// </summary>
    public static ImmutableArray<T> GetFloatingPointData<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
        => FloatingPointData<T>.Data;

    /// <summary>
    ///  Returns a variety of integer test cases.
    /// </summary>
    public static ImmutableArray<T> GetIntegerData<T>()
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
        => IntegerData<T>.Data;

    /// <summary>
    ///  Returns a variety of positive integer test cases.
    /// </summary>
    public static ImmutableArray<T> GetPositiveIntegerData<T>()
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
        => PositiveIntegerData<T>.Data;

    private static class FloatingPointData<T>
        where T : struct, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
    {
        public static ImmutableArray<T> Data { get; } =
        [
            T.MinValue,
            T.MaxValue,
            T.One,
            T.Zero,
            T.NegativeOne,
            T.MaxValue / (T.One + T.One),
            T.NaN,
            T.NegativeInfinity,
            T.Epsilon,
            T.Epsilon * T.NegativeOne
        ];
    }

    private static class IntegerData<T>
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        public static ImmutableArray<T> Data { get; }
            = ImmutableArray.Create(T.MinValue == T.Zero
                ?
                    [
                        T.MinValue,
                        T.MaxValue,
                        T.One,
                        T.MaxValue / (T.One + T.One)
                    ]
                : new T[]
                    {
                        T.MinValue,
                        T.MaxValue,
                        T.One,
                        T.Zero,
                        T.Zero - T.One,
                        T.MaxValue / (T.One + T.One)
                    });
    }

    private static class PositiveIntegerData<T>
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        public static ImmutableArray<T> Data { get; } =
        [
            T.Zero, T.MaxValue, T.One, T.MaxValue / (T.One + T.One)
        ];
    }
}
