// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for a representative set of positive number values.
/// </summary>
public class PositiveIntegerDataAttribute<TNumber>
    : CommonMemberDataAttribute where TNumber : struct, IBinaryInteger<TNumber>, IMinMaxValue<TNumber>
{
    public PositiveIntegerDataAttribute() : base(typeof(PositiveIntegerDataAttribute<TNumber>)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(TestData.GetPositiveIntegerData<TNumber>());
}
