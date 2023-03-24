// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for <see langword="null"/> and <see cref="string.Empty"/>.
/// </summary>
public class NullAndEmptyStringDataAttribute : CommonMemberDataAttribute
{
    private static readonly TheoryData<string?> _data = new();

    public NullAndEmptyStringDataAttribute()
        : base(typeof(NullAndEmptyStringDataAttribute), nameof(GetTheoryData))
    {
        _data.Add(null);
        _data.Add(string.Empty);
    }

    public static TheoryData<string?> GetTheoryData() => _data;
}
