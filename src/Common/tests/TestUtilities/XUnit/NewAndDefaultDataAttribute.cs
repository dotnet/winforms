// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for <see langword="default"/> and the default constructor
///  for the given class type.
/// </summary>
public class NewAndDefaultDataAttribute<TClass> : CommonMemberDataAttribute where TClass : new()
{
    private static readonly TheoryData<TClass?> _data = new();

    public NewAndDefaultDataAttribute()
        : base(typeof(NewAndDefaultDataAttribute<TClass>), nameof(GetTheoryData))
    {
        _data.Add(new TClass());
        _data.Add(default);
    }

    public static TheoryData<TClass?> GetTheoryData() => _data;
}
