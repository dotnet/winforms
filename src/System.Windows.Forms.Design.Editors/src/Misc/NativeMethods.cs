// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Design
{
    internal static partial class NativeMethods
    {
        public const int DLGC_WANTALLKEYS = 0x0004;
        public const int NM_CLICK = 0 - 0 - 2;
        public const int EC_LEFTMARGIN = 0x0001;
        public const int EC_RIGHTMARGIN = 0x0002;
        public const int IDOK = 1;

        public const int VK_PROCESSKEY = 0xE5;

        public const int CC_FULLOPEN = 0x00000002;
        public const int CC_ENABLETEMPLATEHANDLE = 0x00000040;
        public const int STGM_DELETEONRELEASE = 0x04000000;

        public const int RECO_PASTE = 0x00000000; // paste from clipboard

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendDlgItemMessage(IntPtr hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(IntPtr hWnd, bool enable);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetDlgItemInt(IntPtr hWnd, int nIDDlgItem, bool[] err, bool signed);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public class TVITEMW
        {
            public readonly int cChildren = 0;
            public readonly int cchTextMax = 0;
            public readonly int hItem = 0;
            public readonly int iImage = 0;
            public readonly int iSelectedImage = 0;
            public readonly int lParam = 0;
            public readonly int mask = 0;

            public readonly int /* LPTSTR */
                pszText = 0;

            public readonly int state = 0;
            public readonly int stateMask = 0;
        }

        public const int TV_FIRST = 0x1100;
        public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;
        public const int TVS_EX_FADEINOUTEXPANDOS = 0x0040;
        public const int TVS_EX_DOUBLEBUFFER = 0x0004;
        public const int SWP_HIDEWINDOW = 0x0080;

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [In] [Out] TV_HITTESTINFO lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public class TV_HITTESTINFO
        {
            public int flags = 0;
            public int hItem = 0;
            public int pt_x = 0;
            public int pt_y = 0;
        }

        internal class Util
        {
            public static int MAKELONG(int low, int high)
            {
                return (high << 16) | (low & 0xffff);
            }

            public static int LOWORD(int n)
            {
                return n & 0xffff;
            }
        }

        public const int CHILDID_SELF = 0;
        public const int OBJID_WINDOW = 0x00000000;
        public const int OBJID_CLIENT = unchecked((int)0xFFFFFFFC);
    }
}
