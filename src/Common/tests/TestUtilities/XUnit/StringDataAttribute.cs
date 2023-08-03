// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data with <see cref="string.Empty"/> and
///  a sample <see langword="string"/>.
/// </summary>
public class StringDataAttribute : CommonMemberDataAttribute
{
    public StringDataAttribute() : base(typeof(StringDataAttribute)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(string.Empty, "teststring");
}
