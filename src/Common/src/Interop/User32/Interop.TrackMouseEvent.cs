// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public struct TRACKMOUSEEVENT
        {
            public uint cbSize;
            public TME dwFlags;
            public IntPtr hwndTrack;
            public uint dwHoverTime;

            public bool IsDefault()
            {
                return cbSize == 0 && dwFlags == 0 && hwndTrack == IntPtr.Zero && dwHoverTime == 0;
            }
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);
    }
}
