// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.UI.Shell;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal unsafe struct NOTIFYICONDATAW
{
    public uint cbSize;
    public IntPtr hWnd;
    public uint uID;
    public NOTIFY_ICON_DATA_FLAGS uFlags;
    public uint uCallbackMessage;
    public IntPtr hIcon;
    public fixed char _szTip[128];
    public uint dwState;
    public uint dwStateMask;
    public fixed char _szInfo[256];
    public uint uTimeoutOrVersion;
    public fixed char _szInfoTitle[64];
    public NOTIFY_ICON_INFOTIP_FLAGS dwInfoFlags;

    private Span<char> szTip
    {
        get { fixed (char* c = _szTip) { return new Span<char>(c, 128); } }
    }

    public ReadOnlySpan<char> Tip
    {
        get => szTip.SliceAtFirstNull();
        set => SpanHelpers.CopyAndTerminate(value, szTip);
    }

    private Span<char> szInfo
    {
        get { fixed (char* c = _szInfo) { return new Span<char>(c, 256); } }
    }

    public ReadOnlySpan<char> Info
    {
        get => szInfo.SliceAtFirstNull();
        set => SpanHelpers.CopyAndTerminate(value, szInfo);
    }

    private Span<char> szInfoTitle
    {
        get { fixed (char* c = _szInfoTitle) { return new Span<char>(c, 64); } }
    }

    public ReadOnlySpan<char> InfoTitle
    {
        get => szInfoTitle.SliceAtFirstNull();
        set => SpanHelpers.CopyAndTerminate(value, szInfoTitle);
    }
}
