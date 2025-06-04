// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class WebBrowserBase
{
    /// <summary>
    ///  Defines a window that the ActiveX window is attached to so that we can override it's wndproc.
    /// </summary>
    private class WebBrowserBaseNativeWindow : NativeWindow
    {
        private readonly WebBrowserBase _webBrowserBase;

        public WebBrowserBaseNativeWindow(WebBrowserBase ax)
        {
            _webBrowserBase = ax;
        }

        /// <summary>
        ///  Pass messages on to the NotifyIcon object's wndproc handler.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_WINDOWPOSCHANGING:
                    WmWindowPosChanging(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private unsafe void WmWindowPosChanging(ref Message m)
        {
            WINDOWPOS* wp = (WINDOWPOS*)m.LParamInternal;
            wp->x = 0;
            wp->y = 0;
            Size s = _webBrowserBase._webBrowserBaseChangingSize;
            if (s.Width == -1)
            {
                // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
                wp->cx = _webBrowserBase.Width;
                wp->cy = _webBrowserBase.Height;
            }
            else
            {
                wp->cx = s.Width;
                wp->cy = s.Height;
            }

            m.ResultInternal = (LRESULT)0;
        }
    }
}
