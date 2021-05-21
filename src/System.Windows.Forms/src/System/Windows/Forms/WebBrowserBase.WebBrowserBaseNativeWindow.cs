// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class WebBrowserBase
    {
        /// <summary>
        ///  Defines a window that the ActiveX window is attached to so that we can override it's wndproc.
        /// </summary>
        private class WebBrowserBaseNativeWindow : NativeWindow
        {
            private readonly WebBrowserBase WebBrowserBase;

            public WebBrowserBaseNativeWindow(WebBrowserBase ax)
            {
                WebBrowserBase = ax;
            }

            /// <summary>
            ///  Pass messages on to the NotifyIcon object's wndproc handler.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.WINDOWPOSCHANGING:
                        WmWindowPosChanging(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            private unsafe void WmWindowPosChanging(ref Message m)
            {
                User32.WINDOWPOS* wp = (User32.WINDOWPOS*)m.LParam;
                wp->x = 0;
                wp->y = 0;
                Size s = WebBrowserBase.webBrowserBaseChangingSize;
                if (s.Width == -1)
                {   // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
                    wp->cx = WebBrowserBase.Width;
                    wp->cy = WebBrowserBase.Height;
                }
                else
                {
                    wp->cx = s.Width;
                    wp->cy = s.Height;
                }

                m.Result = (IntPtr)0;
            }
        }
    }
}
