// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if DRAWING_DESIGN_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;   
    using System.Text;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Runtime.Versioning;

    /// <summary>
    ///   This is an extract of the System.Drawing IntNativeMethods in the CommonUI tree.
    ///   This is done to be able to compile the GDI code in both assemblies System.Drawing
    ///   and System.Windows.Forms.
    /// </summary>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    static partial class IntSafeNativeMethods 
    {
        // Brush.

        [DllImport(ExternDll.Gdi32, SetLastError=true, ExactSpelling = true, EntryPoint = "CreateSolidBrush", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        
        private static extern IntPtr IntCreateSolidBrush(int crColor);
        
        
        public static IntPtr CreateSolidBrush(int crColor) 
        {
            IntPtr hBrush = Interop.HandleCollector.Add(IntCreateSolidBrush(crColor), Interop.CommonHandles.GDI);
            DbgUtil.AssertWin32(hBrush != IntPtr.Zero, "IntCreateSolidBrush(color={0}) failed.", crColor);
            return hBrush;
        }

        // Pen.

        [DllImport(ExternDll.Gdi32, SetLastError=true, ExactSpelling = true, EntryPoint = "CreatePen", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        
        private static extern IntPtr IntCreatePen(int fnStyle, int nWidth, int crColor);
        
        
        public static IntPtr CreatePen(int fnStyle, int nWidth, int crColor)
        {
            IntPtr hPen = Interop.HandleCollector.Add(IntCreatePen(fnStyle, nWidth, crColor), Interop.CommonHandles.GDI);
            DbgUtil.AssertWin32(hPen != IntPtr.Zero, "IntCreatePen(style={0}, width={1}, color=[{2}]) failed.", fnStyle, nWidth, crColor);
            return hPen;
        }

        [DllImport(ExternDll.Gdi32, SetLastError=true, ExactSpelling = true, EntryPoint = "ExtCreatePen", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        
        private static extern IntPtr IntExtCreatePen(int fnStyle, int dwWidth, IntNativeMethods.LOGBRUSH lplb, int dwStyleCount, [MarshalAs(UnmanagedType.LPArray)] int[] lpStyle);
        
        
        public static IntPtr ExtCreatePen(int fnStyle, int dwWidth, IntNativeMethods.LOGBRUSH lplb, int dwStyleCount, int[] lpStyle)
        {
            IntPtr hPen = Interop.HandleCollector.Add(IntExtCreatePen(fnStyle, dwWidth, lplb, dwStyleCount, lpStyle), Interop.CommonHandles.GDI);
            DbgUtil.AssertWin32(hPen != IntPtr.Zero, "IntExtCreatePen(style={0}, width={1}, brush={2}, styleCount={3}, styles={4}) failed.", fnStyle, dwWidth, lplb, dwStyleCount, lpStyle);
            return hPen;
        }

        // Region

        [DllImport(ExternDll.Gdi32, SetLastError=true, ExactSpelling=true, EntryPoint="CreateRectRgn", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        
        public static extern IntPtr IntCreateRectRgn(int x1, int y1, int x2, int y2);
        
        
        public static IntPtr CreateRectRgn(int x1, int y1, int x2, int y2) 
        {
            IntPtr hRgn = Interop.HandleCollector.Add(IntCreateRectRgn(x1, y1, x2, y2), Interop.CommonHandles.GDI);
            DbgUtil.AssertWin32(hRgn != IntPtr.Zero, "IntCreateRectRgn([x1={0}, y1={1}, x2={2}, y2={3}]) failed.", x1, y1, x2, y2);
            return hRgn;
        }

        // Misc.
           
        [DllImport(ExternDll.Kernel32, SetLastError=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
        
        public static extern int GetUserDefaultLCID();
        
        [DllImport(ExternDll.Gdi32, SetLastError=true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        
        public static extern bool GdiFlush();
    }
}

