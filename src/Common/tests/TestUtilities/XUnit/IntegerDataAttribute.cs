// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for a representative set of number values.
/// </summary>
public class IntegerDataAttribute<TNumber> : CommonMemberDataAttribute
    where TNumber : struct, IBinaryInteger<TNumber>, IMinMaxValue<TNumber>
{
    public IntegerDataAttribute() : base(typeof(IntegerDataAttribute<TNumber>)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(TestData.GetIntegerData<TNumber>());
}
