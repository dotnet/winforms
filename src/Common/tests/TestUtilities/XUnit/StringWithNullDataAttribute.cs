// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for <see langword="null"/>, <see cref="string.Empty"/>, and
///  a sample <see langword="string"/>.
/// </summary>
public class StringWithNullDataAttribute : CommonMemberDataAttribute
{
    private static readonly TheoryData<string?> _data = new();

    public StringWithNullDataAttribute()
        : base(typeof(StringWithNullDataAttribute), nameof(GetTheoryData))
    {
        _data.Add(null);
        _data.Add(string.Empty);
        _data.Add("teststring");
    }

    public static TheoryData<string?> GetTheoryData() => _data;
}
