// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.TestUtilities;

/// <summary>
///  Specifies what type of test data to include in the <see cref="TheoryData"/>.
/// </summary>
[Flags]
public enum TestIncludeType
{
    All,
    NoPositives,
    NoNegatives
}
