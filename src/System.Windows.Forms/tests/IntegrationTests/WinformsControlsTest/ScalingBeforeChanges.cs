// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WinformsControlsTest
{
    public partial class ScalingBeforeChanges : Form
    {
        public ScalingBeforeChanges()
        {
            InitializeComponent();
        }
        [DllImport("gdi32", ExactSpelling = true)]
        internal static extern int GetDeviceCaps(HandleRef hDC, int index);

        [DllImport("user32", ExactSpelling = true)]
        internal static extern IntPtr GetDC(HandleRef hWnd);

        [DllImport("user32", ExactSpelling = true)]
        internal static extern int ReleaseDC(HandleRef hWnd, HandleRef hDC);

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter,
                                               int x, int y, int cx, int cy, int flags);



        [DllImport("user32", ExactSpelling = true)]
        internal static extern bool EnableNonClientDpiScaling(HandleRef hWnd);

        [DllImport("user32", EntryPoint = "#2704")]
        internal static extern bool EnableChildWindowDpiMessage(HandleRef hWnd, bool fEnable);

        internal const int WM_DPICHANGED = 0x02E0;

        internal const double LogicalDpi = 96.0;
        internal const int LOGPIXELSX = 88;
        internal const int LOGPIXELSY = 90;


        internal static void GetDevicePixels(HandleRef handleRef, out double x, out double y)
        {
            x = LogicalDpi;
            y = LogicalDpi;
            IntPtr hDC = GetDC(handleRef);
            if (hDC != IntPtr.Zero)
            {
                x = GetDeviceCaps(new HandleRef(null, hDC), LOGPIXELSX);
                y = GetDeviceCaps(new HandleRef(null, hDC), LOGPIXELSY);

                ReleaseDC(handleRef, new HandleRef(null, hDC));
            }
        }

        private double deviceDpiX, deviceDpiY;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GetDevicePixels(new HandleRef(this, this.Handle), out deviceDpiX, out deviceDpiY);
            EnableNonClientDpiScaling(new HandleRef(this, Handle));
            EnableChildWindowDpiMessage(new HandleRef(this, Handle), true);
        }

        internal static int LOWORD(IntPtr param)
        {
            return (int)(short)(unchecked((int)(long)param) & 0xFFFF);
        }

        internal static int HIWORD(IntPtr param)
        {
            return (int)(short)((unchecked((int)(long)param) >> 16) & 0xFFFF);
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
            switch (m.Msg)
            {
                case WM_DPICHANGED:
                    int x = LOWORD(m.WParam);
                    int y = HIWORD(m.WParam);
                    if (x != deviceDpiX || y != deviceDpiY)
                    {
                        RECT suggestedRect = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

                        SetWindowPos(new HandleRef(this, Handle), new HandleRef(null, IntPtr.Zero), suggestedRect.left, suggestedRect.top,
                            suggestedRect.right - suggestedRect.left, suggestedRect.bottom - suggestedRect.top, 0x4 | 0x10);

                        float factorX = (float)(x / deviceDpiX);
                        float factorY = (float)(y / deviceDpiY);
                        deviceDpiY = y;
                        deviceDpiX = x;

                        Font = new Font(this.Font.FontFamily, this.Font.Size * factorY, this.Font.Style);

                        checkBox1.Scale(new SizeF(factorX, factorY));
                    }
                    m.Result = IntPtr.Zero;
                    break;

            }
        }
    }

    public class MyCheckBox : CheckBox
    {
        internal const int WM_DPICHANGED_BEFOREPARENT = 0x02E2;
        internal const int WM_DPICHANGED_AFTERPARENT = 0x02E3;

        [DllImport("User32", ExactSpelling = true, SetLastError = true)]
        public static extern uint GetDpiForWindow(HandleRef hWnd);

        public MyCheckBox() : base()
        {
        }
        protected override void WndProc(ref Message m)
        {

            uint dpi;
            switch (m.Msg)
            {
                case WM_DPICHANGED_BEFOREPARENT:
                    dpi = GetDpiForWindow(new HandleRef(this, Handle));
                    Debug.WriteLine($"WM_DPICHANGED_BEFOREPARENT  {dpi}");

                    m.Result = (IntPtr)1;
                    break;
                case WM_DPICHANGED_AFTERPARENT:
                    dpi = GetDpiForWindow(new HandleRef(this, Handle));
                    Debug.WriteLine($"WM_DPICHANGED_AFTERPARENT {dpi}");
                    m.Result = (IntPtr)1;
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
