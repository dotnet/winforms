// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for true/false.
/// </summary>
public class BoolDataAttribute : CommonMemberDataAttribute
{
    public BoolDataAttribute() : base(typeof(BoolDataAttribute), nameof(TheoryData)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(true, false);
}
