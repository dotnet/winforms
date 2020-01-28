// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct TTOOLINFOW
        {
            public uint cbSize;
            public TTF uFlags;
            public IntPtr hwnd;
            public IntPtr uId;
            public RECT rect;
            public IntPtr hinst;
            public char* lpszText;
            public IntPtr lParam;
            // void *lpReserved;
        }

        public struct ToolInfoWrapper<T>
            where T : IHandle
        {
            public TTOOLINFOW Info;
            public string? Text { get; set; }
            [MaybeNull]
            private readonly T _handle;

            public unsafe ToolInfoWrapper(T handle, TTF flags = default, string? text = null)
            {
                Info = new TTOOLINFOW
                {
                    hwnd = handle.Handle,
                    uId = handle.Handle,
                    uFlags = flags | TTF.IDISHWND
                };
                Text = text;
                _handle = handle;
            }

            public unsafe ToolInfoWrapper(T handle, IntPtr id, TTF flags = default, string? text = null, RECT rect = default)
            {
                Info = new TTOOLINFOW
                {
                    hwnd = handle.Handle,
                    uId = id,
                    uFlags = flags,
                    rect = rect
                };
                Text = text;
                _handle = handle;
            }

            public unsafe IntPtr SendMessage(IHandle sender, User32.WM message, BOOL state = BOOL.FALSE)
            {
                Info.cbSize = (uint)sizeof(TTOOLINFOW);
                fixed (char* c = Text)
                fixed (void* i = &Info)
                {
                    if (Text != null)
                    {
                        Info.lpszText = c;
                    }
                    IntPtr result = User32.SendMessageW(sender, message, (IntPtr)state, (IntPtr)i);
                    GC.KeepAlive(_handle);
                    return result;
                }
            }
        }
    }
}
