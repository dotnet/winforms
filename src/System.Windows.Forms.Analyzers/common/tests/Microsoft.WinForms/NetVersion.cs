// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;

[Flags]
internal enum NetVersion
{
    Net6_0 = 0x00000006,
    Net7_0 = 0x00000007,
    Net8_0 = 0x00000008,
    Net9_0 = 0x00000009,
    Net10_0 = 0x0000000A,
    Net11_0 = 0x0000000B,
    Net12_0 = 0x0000000C,

    /// <summary>
    ///  If this is selected, we're taking WinForms runtime build from 
    ///  this repo and the .NET version, the runtime is build against.
    /// </summary>
    WinFormsBuild = 0x01000000,

    /// <summary>
    ///  If this is OR'ed in, we're taking the specified runtime version,
    ///  and the WinForms runtime for this repo.
    /// </summary>
    BuildOutput = 0x10000000
}
