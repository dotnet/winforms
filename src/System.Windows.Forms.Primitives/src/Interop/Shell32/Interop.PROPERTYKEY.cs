// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Shell32
    {
        public struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
        }
    }
}
