// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal static class NativeMethods
    {
        public static IntPtr InvalidIntPtr = (IntPtr)(-1);
        public static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public const int CW_USEDEFAULT = (unchecked((int)0x80000000));

        public const int
        GDI_ERROR = (unchecked((int)0xFFFFFFFF));

        public const int
        HCF_HIGHCONTRASTON = 0x00000001;

        public const int HLP_FILE = 1,
        HLP_KEYWORD = 2,
        HLP_NAVIGATOR = 3,
        HLP_OBJECT = 4;

        public const int
        TB_LINEUP = 0,
        TB_LINEDOWN = 1,
        TB_PAGEUP = 2,
        TB_PAGEDOWN = 3,
        TB_THUMBPOSITION = 4,
        TB_THUMBTRACK = 5,
        TB_TOP = 6,
        TB_BOTTOM = 7,
        TB_ENDTRACK = 8;

        public const int WHEEL_DELTA = 120;

        public static int START_PAGE_GENERAL = unchecked((int)0xffffffff);

        public const int XBUTTON1 = 0x0001;
        public const int XBUTTON2 = 0x0002;

        public const int CHILDID_SELF = 0;

        public const int UiaRootObjectId = -25;
        public const int UiaAppendRuntimeId = 3;

        public const string uuid_IAccessible = "{618736E0-3C3D-11CF-810C-00AA00389B71}";

        public const string WinFormFrameworkId = "WinForm";

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public delegate int ListViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

        public delegate int TreeViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class PRINTDLGEX
        {
            public int lStructSize;

            public IntPtr hwndOwner;
            public IntPtr hDevMode;
            public IntPtr hDevNames;
            public IntPtr hDC;

            public Comdlg32.PD Flags;
            public int Flags2;

            public int ExclusionFlags;

            public int nPageRanges;
            public int nMaxPageRanges;

            public IntPtr pageRanges;

            public int nMinPage;
            public int nMaxPage;
            public int nCopies;

            public IntPtr hInstance;
            [MarshalAs(UnmanagedType.LPStr)]
            public string? lpPrintTemplateName;

            public WndProc? lpCallback;

            public int nPropertyPages;

            public IntPtr lphPropertyPages;

            public int nStartPage;
            public Comdlg32.PD_RESULT dwResultAction;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OPENFILENAME_I
        {
            public int lStructSize = Marshal.SizeOf<OPENFILENAME_I>(); //ndirect.DllLib.sizeOf(this);
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string? lpstrFilter;   // use embedded nulls to separate filters
            public IntPtr lpstrCustomFilter = IntPtr.Zero;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile = Kernel32.MAX_PATH;
            public IntPtr lpstrFileTitle = IntPtr.Zero;
            public int nMaxFileTitle = Kernel32.MAX_PATH;
            public string? lpstrInitialDir;
            public string? lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string? lpstrDefExt;
            public IntPtr lCustData = IntPtr.Zero;
            public WndProc? lpfnHook;
            public string? lpTemplateName;
            public IntPtr pvReserved = IntPtr.Zero;
            public int dwReserved;
            public int FlagsEx;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENLINK
        {
            public User32.NMHDR nmhdr;
            public int msg;
            public IntPtr wParam = IntPtr.Zero;
            public IntPtr lParam = IntPtr.Zero;
            public Richedit.CHARRANGE charrange;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENLINK64
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
            public byte[] contents = new byte[56];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class ENPROTECTED
        {
            public User32.NMHDR nmhdr;
            public int msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public Richedit.CHARRANGE chrg;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENPROTECTED64
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
            public byte[] contents = new byte[56];
        }

        public class ActiveX
        {
            public const int ALIGN_MIN = 0x0;
            public const int ALIGN_NO_CHANGE = 0x0;
            public const int ALIGN_TOP = 0x1;
            public const int ALIGN_BOTTOM = 0x2;
            public const int ALIGN_LEFT = 0x3;
            public const int ALIGN_RIGHT = 0x4;
            public const int ALIGN_MAX = 0x4;

            public static Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

            private ActiveX()
            {
            }
        }

        /// <summary>
        ///  This method takes a file URL and converts it to a local path.  The trick here is that
        ///  if there is a '#' in the path, everything after this is treated as a fragment.  So
        ///  we need to append the fragment to the end of the path.
        /// </summary>
        internal static string GetLocalPath(string fileName)
        {
            System.Diagnostics.Debug.Assert(fileName != null && fileName.Length > 0, "Cannot get local path, fileName is not valid");

            Uri uri = new Uri(fileName);
            return uri.LocalPath + uri.Fragment;
        }

        // Threading stuff
        public const uint STILL_ACTIVE = 259;
    }
}
