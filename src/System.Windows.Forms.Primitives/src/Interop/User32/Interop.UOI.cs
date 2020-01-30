// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum UOI
        {
            FLAGS = 1,
            NAME = 2,
            TYPE = 3,
            USER_SID = 4,
            HEAPSIZE = 5,
            IO = 6,
            TIMERPROC_EXCEPTION_SUPPRESSION = 7
        }
    }
}
