// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum ID : int
        {
            OK = 1,
            CANCEL = 2,
            ABORT = 3,
            RETRY = 4,
            IGNORE = 5,
            YES = 6,
            NO = 7,
            CLOSE = 8,
            HELP = 9,
            TRYAGAIN = 10,
            CONTINUE = 11,
            TIMEOUT = 32000
        }
    }
}
