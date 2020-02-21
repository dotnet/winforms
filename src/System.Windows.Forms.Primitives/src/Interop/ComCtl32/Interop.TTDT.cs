// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TTDT : uint
        {
            AUTOMATIC = 0,
            RESHOW = 1,
            AUTOPOP = 2,
            INITIAL = 3,
        }
    }
}
