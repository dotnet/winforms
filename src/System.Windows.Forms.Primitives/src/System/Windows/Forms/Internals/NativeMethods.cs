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

        public const int LANG_NEUTRAL = 0x00,
                         LOCALE_IFIRSTDAYOFWEEK = 0x0000100C;   /* first day of week specifier */

        public const int LOCALE_IMEASURE = 0x0000000D;   // 0 = metric, 1 = US

        public static readonly uint LOCALE_USER_DEFAULT = MAKELCID(LANG_USER_DEFAULT);
        public static readonly uint LANG_USER_DEFAULT = MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT);

        public static uint MAKELANGID(uint primary, uint sub)
        {
            return unchecked((uint)((((ushort)(sub)) << 10) | (ushort)(primary)));
        }

        /// <summary>
        ///  Creates an LCID from a LangId
        /// </summary>
        public static uint MAKELCID(uint lgid)
        {
            return MAKELCID(lgid, SORT_DEFAULT);
        }

        /// <summary>
        ///  Creates an LCID from a LangId
        /// </summary>
        public static uint MAKELCID(uint lgid, uint sort)
        {
            return ((0xFFFF & lgid) | (((0x000f) & sort) << 16));
        }

        public const int
        MDIS_ALLCHILDSTYLES = 0x0001;

        public const int
        PATCOPY = 0x00F00021,
        PATINVERT = 0x005A0049;

        public const int
        SORT_DEFAULT = 0x0,
        SUBLANG_DEFAULT = 0x01;

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_AKLINK
        {
            internal int cbStruct = Marshal.SizeOf<HH_AKLINK>();
            internal bool fReserved = false;
            internal string pszKeywords = null;
            internal string pszUrl = null;
            internal string pszMsgText = null;
            internal string pszMsgTitle = null;
            internal string pszWindow = null;
            internal bool fIndexOnFail = false;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_POPUP
        {
            internal int cbStruct = Marshal.SizeOf<HH_POPUP>();
            internal IntPtr hinst = IntPtr.Zero;
            internal int idString = 0;
            internal IntPtr pszText;
            internal Point pt;
            internal int clrForeground = -1;
            internal int clrBackground = -1;
            internal RECT rcMargins = new RECT(-1, -1, -1, -1);     // amount of space between edges of window and text, -1 for each member to ignore
            internal string pszFont = null;
        }

        public const int HH_FTS_DEFAULT_PROXIMITY = -1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_FTS_QUERY
        {
            internal int cbStruct = Marshal.SizeOf<HH_FTS_QUERY>();
            internal bool fUniCodeStrings = false;
            [MarshalAs(UnmanagedType.LPStr)]
            internal string pszSearchQuery = null;
            internal int iProximity = NativeMethods.HH_FTS_DEFAULT_PROXIMITY;
            internal bool fStemmedSearch = false;
            internal bool fTitleOnly = false;
            internal bool fExecute = true;
            [MarshalAs(UnmanagedType.LPStr)]
            internal string pszWindow = null;
        }

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
            public string lpPrintTemplateName;

            public WndProc lpCallback = null;

            public int nPropertyPages;

            public IntPtr lphPropertyPages;

            public int nStartPage;
            public Comdlg32.PD_RESULT dwResultAction;
        }

        // x86 requires EXPLICIT packing of 1.
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
        public class PRINTPAGERANGE
        {
            public int nFromPage = 0;
            public int nToPage = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OPENFILENAME_I
        {
            public int lStructSize = Marshal.SizeOf<OPENFILENAME_I>(); //ndirect.DllLib.sizeOf(this);
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;   // use embedded nulls to separate filters
            public IntPtr lpstrCustomFilter = IntPtr.Zero;
            public int nMaxCustFilter = 0;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile = Kernel32.MAX_PATH;
            public IntPtr lpstrFileTitle = IntPtr.Zero;
            public int nMaxFileTitle = Kernel32.MAX_PATH;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset = 0;
            public short nFileExtension = 0;
            public string lpstrDefExt;
            public IntPtr lCustData = IntPtr.Zero;
            public WndProc lpfnHook;
            public string lpTemplateName = null;
            public IntPtr pvReserved = IntPtr.Zero;
            public int dwReserved = 0;
            public int FlagsEx;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class COMRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public COMRECT()
            {
            }

            public COMRECT(Rectangle r)
            {
                left = r.X;
                top = r.Y;
                right = r.Right;
                bottom = r.Bottom;
            }

            public override string ToString()
            {
                return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CLIENTCREATESTRUCT
        {
            public IntPtr hWindowMenu;
            public int idFirstChild;

            public CLIENTCREATESTRUCT(IntPtr hmenu, int idFirst)
            {
                hWindowMenu = hmenu;
                idFirstChild = idFirst;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENLINK
        {
            public User32.NMHDR nmhdr;
            public int msg = 0;
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
