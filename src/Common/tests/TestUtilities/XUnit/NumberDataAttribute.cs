// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for a representative set of number values.
/// </summary>
public class NumberDataAttribute<TNumber> : CommonMemberDataAttribute where TNumber : IBinaryNumber<TNumber>, IMinMaxValue<TNumber>
{
    private static readonly TheoryData<TNumber> _data = new();

    public NumberDataAttribute()
        : base(typeof(NumberDataAttribute<TNumber>), nameof(GetTheoryData))
    {
        _data.Add(TNumber.MinValue);
        _data.Add(TNumber.MaxValue);
        _data.Add(TNumber.One);
        _data.Add(TNumber.MaxValue / (TNumber.One + TNumber.One));

        if (!TNumber.MinValue.Equals(TNumber.Zero))
        {
            _data.Add(TNumber.Zero);
        }

        if (!TNumber.MaxValue.Equals(TNumber.Zero - TNumber.One))
        {
            _data.Add(TNumber.Zero - TNumber.One);
        }
    }

    public static TheoryData<TNumber> GetTheoryData() => _data;
}
