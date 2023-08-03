// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for <see langword="null"/>, <see cref="string.Empty"/>, and
///  a sample <see langword="string"/>.
/// </summary>
public class StringWithNullDataAttribute : CommonMemberDataAttribute
{
    public StringWithNullDataAttribute() : base(typeof(StringWithNullDataAttribute)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(null, string.Empty, "teststring");
}
