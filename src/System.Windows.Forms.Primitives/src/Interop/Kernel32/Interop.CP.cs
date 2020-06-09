// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Kernel32
    {
        public enum CP : uint
        {
            ACP = 0,
            OEMCP = 1,
            MACCP = 2,
            THREAD_ACP = 3,
            SYMBOL = 42,
            UTF7 = 65000,
            UTF8 = 65001,
        }
    }
}
