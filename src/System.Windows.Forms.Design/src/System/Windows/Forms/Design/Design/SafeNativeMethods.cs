//------------------------------------------------------------------------------
// <copyright file="SafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Design {
    using System.Runtime.InteropServices;
    using System;
    using System.Security.Permissions;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Runtime.Versioning;

    [
    System.Security.SuppressUnmanagedCodeSecurityAttribute()
    ]
    internal static class SafeNativeMethods {

        [DllImport(ExternDll.Gdi32, SetLastError=true, ExactSpelling=true, EntryPoint="DeleteObject", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool DeleteObject(HandleRef hObject);

        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int GetMessagePos();

        [DllImport(ExternDll.User32, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int RegisterWindowMessage(string msg);

        [DllImport(ExternDll.Gdi32, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetTextMetrics(HandleRef hdc, NativeMethods.TEXTMETRIC tm);

        [DllImport(ExternDll.Gdi32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool BitBlt(IntPtr hDC,int x,int y,int nWidth,int nHeight,IntPtr hSrcDC,int xSrc,int ySrc,int dwRop);

        [DllImport(ExternDll.Gdi32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport(ExternDll.User32, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int GetWindowTextLength(HandleRef hWnd);

        [DllImport(ExternDll.Kernel32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int GetTickCount();
        
        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool RedrawWindow(IntPtr hwnd, NativeMethods.COMRECT rcUpdate, IntPtr hrgnUpdate, int flags);
        
        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                               int x, int y, int cx, int cy, int flags);
        
        [DllImport(ExternDll.User32, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int DrawText(HandleRef hDC, string lpszString, int nCount, ref NativeMethods.RECT lpRect, int nFormat);
        
        [DllImport(ExternDll.Gdi32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SelectObject(HandleRef hDC, HandleRef hObject);
        
        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public extern static bool IsChild(HandleRef parent, HandleRef child);
     

        // this is a wrapper that comctl exposes for the NT function since it doesn't exist natively on 95.
        [DllImport(ExternDll.Comctl32, ExactSpelling=true)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool _TrackMouseEvent(NativeMethods.TRACKMOUSEEVENT tme);
        public static bool TrackMouseEvent(NativeMethods.TRACKMOUSEEVENT tme) {
            // only on NT - not on 95 - comctl32 has a wrapper for 95 and NT.
            return _TrackMouseEvent(tme);
        }

        [DllImport(ExternDll.Kernel32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        public static extern int GetCurrentProcessId();

        [DllImport(ExternDll.Gdi32, ExactSpelling=true, CharSet=CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool RoundRect(HandleRef hDC, int left, int top, int right, int bottom, int width, int height);        

        [DllImport(ExternDll.Gdi32, ExactSpelling=true, CharSet=CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool Rectangle(HandleRef hdc, int left, int top, int right, int bottom);

        [DllImport(ExternDll.Gdi32, ExactSpelling=true, EntryPoint="CreatePen", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        private static extern IntPtr IntCreatePen(int nStyle, int nWidth, int crColor);
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static IntPtr CreatePen(int nStyle, int nWidth, int crColor) {
            return System.Internal.HandleCollector.Add(IntCreatePen(nStyle, nWidth, crColor), NativeMethods.CommonHandles.GDI);
        }

        //--------Added because of VSWhidbey 581670------------------------------------------------------
        // Investigate removing this if the duplicate code in OleDragDropHandler.cs is removed
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int SetROP2(HandleRef hDC, int nDrawMode);
        
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int SetBkColor(HandleRef hDC, int clr);
        //--------Added because of VSWhidbey 581670------------------------------------------------------

        //scoberry Nov 1, 2004: Removed 
	/*

        [DllImport(ExternDll.Gdi32, ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool Ellipse(HandleRef hDC, int left, int top, int right, int bottom);
        

        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr GetSysColorBrush(int nIndex);

        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool MessageBeep(int type);

        [DllImport(ExternDll.User32, ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool RedrawWindow(IntPtr hwnd, ref NativeMethods.RECT rcUpdate, IntPtr hrgnUpdate, int flags);
        

	*/

        [ResourceExposure(ResourceScope.None)]
        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public extern static int SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);
    }
}
