// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Shell32
    {
        public enum CDCS : uint
        {
            INACTIVE = 0,
            ENABLED = 0x1,
            VISIBLE = 0x2,
            ENABLEDVISIBLE = 0x3
        }
    }
}
