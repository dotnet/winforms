// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for true/false.
/// </summary>
public class BoolDataAttribute : CommonMemberDataAttribute
{
    private static readonly TheoryData<bool> _data = new();

    public BoolDataAttribute()
        : base(typeof(BoolDataAttribute), nameof(GetTheoryData))
    {
        _data.Add(true);
        _data.Add(false);
    }

    public static TheoryData<bool> GetTheoryData() => _data;
}
