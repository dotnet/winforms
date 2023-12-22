// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data to test normalization of <see langword="null"/> to
///  <see cref="string.Empty"/>.
/// </summary>
public class NormalizedStringDataAttribute : CommonMemberDataAttribute
{
    public NormalizedStringDataAttribute() : base(typeof(NormalizedStringDataAttribute)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(new TheoryData<string?, string>()
    {
        { null, string.Empty },
        { string.Empty, string.Empty },
        { "teststring", "teststring" }
    });
}
