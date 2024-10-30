// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.TestUtilities;

/// <summary>
///  Specifies what type of test data to include in the <see cref="TheoryData"/>. This is
///  used in <see cref="CommonMemberDataAttribute"/>
/// </summary>
[Flags]
public enum TestIncludeType
{
    All,
    NoPositives,
    NoNegatives
}
