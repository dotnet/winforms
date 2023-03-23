// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data to test normalization of <see langword="null"/> to
///  <see cref="string.Empty"/>.
/// </summary>
public class NormalizedStringDataAttribute : CommonMemberDataAttribute
{
    private static readonly TheoryData<string?, string> _data = new();

    public NormalizedStringDataAttribute()
        : base(typeof(StringDataAttribute), nameof(GetTheoryData))
    {
        _data.Add(null, string.Empty);
        _data.Add(string.Empty, string.Empty);
        _data.Add("teststring", "teststring");
    }

    public static TheoryData<string?, string> GetTheoryData() => _data;
}

