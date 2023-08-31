// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for all defined values for the specified enum type.
/// </summary>
public class EnumDataAttribute<TEnum> : CommonMemberDataAttribute where TEnum : struct, Enum
{
    public EnumDataAttribute() : base(typeof(EnumDataAttribute<TEnum>)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(Enum.GetValues<TEnum>());
}
