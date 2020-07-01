// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL DrawFrameControl(Gdi32.HDC hdc, ref RECT rect, DFC type, DFCS state);

        public static BOOL DrawFrameControl(IHandle hdc, ref RECT rect, DFC type, DFCS state)
        {
            BOOL result = DrawFrameControl((Gdi32.HDC)hdc.Handle, ref rect, type, state);
            GC.KeepAlive(hdc);
            return result;
        }

        public static BOOL DrawFrameControl(HandleRef hdc, ref RECT rect, DFC type, DFCS state)
        {
            BOOL result = DrawFrameControl((Gdi32.HDC)hdc.Handle, ref rect, type, state);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
