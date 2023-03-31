// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for all defined values for the specified enum type.
/// </summary>
public class EnumDataAttribute<TEnum> : CommonMemberDataAttribute where TEnum : struct, Enum
{
    private static readonly TheoryData<TEnum> _data = new();

    public EnumDataAttribute()
        : base(typeof(EnumDataAttribute<TEnum>), nameof(GetTheoryData))
    {
        foreach (TEnum item in Enum.GetValues<TEnum>())
        {
            _data.Add(item);
        }
    }

    public static TheoryData<TEnum> GetTheoryData() => _data;
}
