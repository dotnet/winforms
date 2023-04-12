// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for a set of floating point number values.
/// </summary>
public class FloatingPointDataAttribute<TNumber> : CommonMemberDataAttribute
    where TNumber : struct, IBinaryFloatingPointIeee754<TNumber>, IMinMaxValue<TNumber>
{
    public FloatingPointDataAttribute() : base(typeof(FloatingPointDataAttribute<TNumber>)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(TestData.GetFloatingPointData<TNumber>());
}
