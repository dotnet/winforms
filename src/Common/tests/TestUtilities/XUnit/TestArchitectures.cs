// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Xunit;

[Flags]
public enum TestArchitectures
{
    X86 = 1,
    X64 = 2,
    Any = ~0
}
