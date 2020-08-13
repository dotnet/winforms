// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum OLECONTF : uint
        {
            EMBEDDINGS = 0x01,
            LINKS = 0x02,
            OTHERS = 0x04,
            ONLYUSER = 0x08,
            ONLYIFRUNNING = 0x10
        }
    }
}
