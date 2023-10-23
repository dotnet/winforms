// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls;

internal struct ToolInfoWrapper<T>
            where T : IHandle<HWND>
{
    public TTTOOLINFOW Info;
    public string? Text { get; set; }
    [MaybeNull]
    private readonly T _handle;

    public unsafe ToolInfoWrapper(T handle, TOOLTIP_FLAGS flags = default, string? text = null)
    {
        Info = new TTTOOLINFOW
        {
            hwnd = handle.Handle,
            uId = (nuint)(IntPtr)handle.Handle,
            uFlags = flags | TOOLTIP_FLAGS.TTF_IDISHWND
        };
        Text = text;
        _handle = handle;
    }

    public unsafe ToolInfoWrapper(T handle, IntPtr id, TOOLTIP_FLAGS flags = default, string? text = null, RECT rect = default)
    {
        Info = new TTTOOLINFOW
        {
            hwnd = handle.Handle,
            uId = (nuint)id,
            uFlags = flags,
            rect = rect
        };
        Text = text;
        _handle = handle;
    }

    public unsafe LRESULT SendMessage(IHandle<HWND> sender, MessageId message, bool state = false)
    {
        Info.cbSize = (uint)sizeof(TTTOOLINFOW);
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
