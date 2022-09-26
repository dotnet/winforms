﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static RegionType IntersectClipRect(IHandle hdc, int left, int top, int right, int bottom)
        {
            RegionType result = (RegionType)IntersectClipRect((HDC)hdc.Handle, left, top, right, bottom);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
