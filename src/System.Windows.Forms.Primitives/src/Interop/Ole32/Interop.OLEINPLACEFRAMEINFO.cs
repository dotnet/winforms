// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public struct OLEINPLACEFRAMEINFO
        {
            public uint cb;
            public BOOL fMDIApp;
            public IntPtr hwndFrame;
            public IntPtr hAccel;
            public uint cAccelEntries;
        }
    }
}
