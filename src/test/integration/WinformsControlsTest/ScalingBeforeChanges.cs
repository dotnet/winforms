// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ScalingBeforeChanges : Form
{
    public ScalingBeforeChanges()
    {
        InitializeComponent();
    }

    [DllImport("user32", ExactSpelling = true)]
    internal static extern bool EnableNonClientDpiScaling(HandleRef hWnd);

    [DllImport("user32", EntryPoint = "#2704")]
    internal static extern bool EnableChildWindowDpiMessage(HandleRef hWnd, bool fEnable);

    internal const double LogicalDpi = 96.0;
    internal const int LOGPIXELSX = 88;
    internal const int LOGPIXELSY = 90;

    internal static void GetDevicePixels(HWND hwnd, out double x, out double y)
    {
        x = LogicalDpi;
        y = LogicalDpi;

        using GetDcScope dc = new(hwnd);
        if (!dc.IsNull)
        {
            x = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            y = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
        }
    }

    private double _deviceDpiX, _deviceDpiY;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        GetDevicePixels((HWND)Handle, out _deviceDpiX, out _deviceDpiY);
        EnableNonClientDpiScaling(new HandleRef(this, Handle));
        EnableChildWindowDpiMessage(new HandleRef(this, Handle), true);
        GC.KeepAlive(this);
    }

    internal static int LOWORD(IntPtr param)
    {
        return (short)(unchecked((int)(long)param) & 0xFFFF);
    }

    internal static int HIWORD(IntPtr param)
    {
        return (short)((unchecked((int)(long)param) >> 16) & 0xFFFF);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_DPICHANGED:
                int x = LOWORD(m.WParam);
                int y = HIWORD(m.WParam);
                if (x != _deviceDpiX || y != _deviceDpiY)
                {
                    RECT suggestedRect = Marshal.PtrToStructure<RECT>(m.LParam);

                    PInvoke.SetWindowPos(
                        this,
                        HWND.Null,
                        suggestedRect.left,
                        suggestedRect.top,
                        suggestedRect.right - suggestedRect.left,
                        suggestedRect.bottom - suggestedRect.top,
                        SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

                    float factorX = (float)(x / _deviceDpiX);
                    float factorY = (float)(y / _deviceDpiY);
                    _deviceDpiY = y;
                    _deviceDpiX = x;

                    Font = new Font(Font.FontFamily, Font.Size * factorY, Font.Style);

                    checkBox1.Scale(new SizeF(factorX, factorY));
                }

                m.Result = IntPtr.Zero;
                break;
        }
    }
}

public class MyCheckBox : CheckBox
{
    public MyCheckBox() : base()
    {
    }

    protected override void WndProc(ref Message m)
    {
        uint dpi;
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_DPICHANGED_BEFOREPARENT:
                dpi = PInvoke.GetDpiForWindow(this);
                Debug.WriteLine($"WM_DPICHANGED_BEFOREPARENT  {dpi}");

                m.Result = 1;
                break;
            case PInvokeCore.WM_DPICHANGED_AFTERPARENT:
                dpi = PInvoke.GetDpiForWindow(this);
                Debug.WriteLine($"WM_DPICHANGED_AFTERPARENT {dpi}");
                m.Result = 1;
                break;
        }

        base.WndProc(ref m);
    }
}
