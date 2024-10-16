// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls;

internal unsafe struct ToolInfoWrapper<T>
    where T : IHandle<HWND>
{
    public TTTOOLINFOW Info;
    public string? Text { get; set; }
    private readonly T _handle;

    // The size of the TTTOOLINFOW struct in version 4.7. We use this version to maintain compatibility.
    private static uint TTTOOLINFO_V2_Size => IntPtr.Size == 4 ? 44u : 64u;

    public ToolInfoWrapper(T handle, TOOLTIP_FLAGS flags = default, string? text = null)
        : this(handle, handle.Handle, flags | TOOLTIP_FLAGS.TTF_IDISHWND, text)
    {
    }

    public ToolInfoWrapper(T handle, nint id, TOOLTIP_FLAGS flags = default, string? text = null, RECT rect = default)
    {
        Info = new TTTOOLINFOW
        {
            cbSize = TTTOOLINFO_V2_Size,
            hwnd = handle.Handle,
            uId = (nuint)id,
            uFlags = flags,
            rect = rect
        };

        Text = text;
        _handle = handle;
    }

    public LRESULT SendMessage(IHandle<HWND> sender, MessageId message, bool state = false)
    {
        Info.cbSize = TTTOOLINFO_V2_Size;
        fixed (char* c = Text)
        fixed (void* i = &Info)
        {
            if (Text is not null)
            {
                Info.lpszText = c;
            }

            LRESULT result = PInvokeCore.SendMessage(sender, message, (WPARAM)(BOOL)state, (LPARAM)i);
            GC.KeepAlive(_handle);
            return result;
        }
    }
}
