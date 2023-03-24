// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for a set of floating point number values.
/// </summary>
public class FloatingPointDataAttribute<TNumber> : CommonMemberDataAttribute where TNumber : IBinaryFloatingPointIeee754<TNumber>, IMinMaxValue<TNumber>
{
    private static readonly TheoryData<TNumber> _data = new();

    public FloatingPointDataAttribute()
        : base(typeof(FloatingPointDataAttribute<TNumber>), nameof(GetTheoryData))
    {
        _data.Add(TNumber.MinValue);
        _data.Add(TNumber.MaxValue);
        _data.Add(TNumber.One);
        _data.Add(TNumber.Zero);
        _data.Add(TNumber.NegativeOne);
        _data.Add(TNumber.MaxValue / (TNumber.One + TNumber.One));
        _data.Add(TNumber.NaN);
        _data.Add(TNumber.NegativeInfinity);
        _data.Add(TNumber.Epsilon);
        _data.Add(TNumber.Epsilon * TNumber.NegativeOne);
    }

    public static TheoryData<TNumber> GetTheoryData() => _data;
}
