// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <remarks>
        /// NMHDR is defined in winuser.h and richedit.h. idFrom is a pointer in winuser.h but is an int
        /// in richedit.h. The definition in richedit.h is inactive and we use the definition in winuser.h.
        /// </remarks>
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }
    }
}
