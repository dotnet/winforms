// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for <see langword="default"/> and the default constructor
///  for the given class type.
/// </summary>
public class NewAndDefaultDataAttribute<TClass> : CommonMemberDataAttribute where TClass : new()
{
    public NewAndDefaultDataAttribute() : base(typeof(NewAndDefaultDataAttribute<TClass>)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = new(new TClass(), default);
}
