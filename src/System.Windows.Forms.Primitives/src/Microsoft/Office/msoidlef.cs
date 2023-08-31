// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Office;

[Flags]
internal enum msoidlef : uint
{
    Periodic = 1,
    NonPeriodic = 2,
    Priority = 4,
    All = 0xFFFFFFFF
}
