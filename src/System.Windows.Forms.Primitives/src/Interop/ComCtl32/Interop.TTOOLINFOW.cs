// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct TTOOLINFOW
        {
            public uint cbSize;
            public TOOLTIP_FLAGS uFlags;
            public IntPtr hwnd;
            public IntPtr uId;
            public RECT rect;
            public IntPtr hinst;
            public char* lpszText;
            public IntPtr lParam;
            // void *lpReserved;
        }

        public struct ToolInfoWrapper<T>
            where T : IHandle<HWND>
        {
            public TTOOLINFOW Info;
            public string? Text { get; set; }
            [MaybeNull]
            private readonly T _handle;

            public unsafe ToolInfoWrapper(T handle, TOOLTIP_FLAGS flags = default, string? text = null)
            {
                Info = new TTOOLINFOW
                {
                    hwnd = handle.Handle,
                    uId = handle.Handle,
                    uFlags = flags | TOOLTIP_FLAGS.TTF_IDISHWND
                };
                Text = text;
                _handle = handle;
            }

            public unsafe ToolInfoWrapper(T handle, IntPtr id, TOOLTIP_FLAGS flags = default, string? text = null, RECT rect = default)
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

            public unsafe LRESULT SendMessage(IHandle<HWND> sender, MessageId message, bool state = false)
            {
                Info.cbSize = (uint)sizeof(TTOOLINFOW);
                fixed (char* c = Text)
                fixed (void* i = &Info)
                {
                    if (Text is not null)
                    {
                        Info.lpszText = c;
                    }

                    LRESULT result = PInvoke.SendMessage(sender, message, (WPARAM)(BOOL)state, (LPARAM)i);
                    GC.KeepAlive(_handle);
                    return result;
                }
            }
        }
    }
}
