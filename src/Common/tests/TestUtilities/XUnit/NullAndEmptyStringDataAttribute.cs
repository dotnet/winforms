// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for <see langword="null"/> and <see cref="string.Empty"/>.
/// </summary>
public class NullAndEmptyStringDataAttribute : CommonMemberDataAttribute
{
    public NullAndEmptyStringDataAttribute() : base(typeof(NullAndEmptyStringDataAttribute)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(null, string.Empty);
}
