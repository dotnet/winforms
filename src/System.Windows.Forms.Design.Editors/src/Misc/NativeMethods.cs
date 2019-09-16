﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Design
{
    internal static partial class NativeMethods
    {
        public static readonly HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public static readonly int PS_SOLID = 0;

        public delegate bool EnumChildrenCallback(IntPtr hwnd, IntPtr lParam);

        public const int DLGC_WANTALLKEYS = 0x0004;
        public const int NM_CLICK = 0 - 0 - 2;
        public const int BM_SETIMAGE = 0x00F7;
        public const int IMAGE_ICON = 1;
        public const int BS_ICON = 0x00000040;
        public const int EC_LEFTMARGIN = 0x0001;
        public const int EC_RIGHTMARGIN = 0x0002;
        public const int IDOK = 1;

        public const int VK_PROCESSKEY = 0xE5;

        public const int CC_FULLOPEN = 0x00000002;
        public const int CC_ENABLETEMPLATEHANDLE = 0x00000040;
        public const int STGM_DELETEONRELEASE = 0x04000000;

        public const int RECO_PASTE = 0x00000000; // paste from clipboard
        public const int RECO_DROP = 0x00000001; // drop

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

        [StructLayout(LayoutKind.Sequential)]
        public class TRACKMOUSEEVENT
        {
            public readonly int cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>();
            public readonly int dwFlags;
            public readonly int dwHoverTime = 0;
            public readonly IntPtr hwndTrack;
        }

        public static readonly IntPtr InvalidIntPtr = (IntPtr)(-1);

        public const int S_OK = 0x00000000;
        public const int S_FALSE = 0x00000001;
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);
        public const int E_INVALIDARG = unchecked((int)0x80070057);
        public const int E_FAIL = unchecked((int)0x80004005);

        public const int WS_EX_STATICEDGE = 0x00020000;
        public static readonly int TME_HOVER = 0x00000001;

        public const int
            OLEIVERB_PRIMARY = 0,
            OLEIVERB_SHOW = -1,
            OLEIVERB_OPEN = -2,
            OLEIVERB_HIDE = -3,
            OLEIVERB_UIACTIVATE = -4,
            OLEIVERB_INPLACEACTIVATE = -5,
            OLEIVERB_DISCARDUNDOSTATE = -6,
            OLEIVERB_PROPERTIES = -7;

        public const int
            OLECLOSE_SAVEIFDIRTY = 0,
            OLECLOSE_NOSAVE = 1,
            OLECLOSE_PROMPTSAVE = 2;

        [StructLayout(LayoutKind.Sequential)]
        public class COMRECT
        {
            public int bottom;
            public int left;
            public int right;
            public int top;

            public COMRECT()
            {
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class FORMATETC
        {
            [MarshalAs(UnmanagedType.I4)] public readonly int cfFormat = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int dwAspect = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int lindex = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly IntPtr ptd = IntPtr.Zero;

            [MarshalAs(UnmanagedType.I4)] public readonly int tymed = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagOIFI
        {
            [MarshalAs(UnmanagedType.U4)] public readonly int cAccelEntries;

            [MarshalAs(UnmanagedType.U4)] public readonly int cb;

            [MarshalAs(UnmanagedType.I4)] public readonly int fMDIApp;

            public readonly IntPtr hAccel;
            public readonly IntPtr hwndFrame;
        }

        [ComImport]
        [ComVisible(true)]
        [Guid("00000116-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceFrame
        {
            IntPtr GetWindow();

            void ContextSensitiveHelp(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnterMode);

            void GetBorder(
                [Out] COMRECT lprectBorder);

            void RequestBorderSpace(
                [In] COMRECT pborderwidths);

            void SetBorderSpace(
                [In] COMRECT pborderwidths);

            void SetActiveObject(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleInPlaceActiveObject pActiveObject,
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszObjName);

            void InsertMenus(
                [In] IntPtr hmenuShared,
                [In] [Out] object lpMenuWidths);

            void SetMenu(
                [In] IntPtr hmenuShared,
                [In] IntPtr holemenu,
                [In] IntPtr hwndActiveObject);

            void RemoveMenus(
                [In] IntPtr hmenuShared);

            void SetStatusText(
                [In] [MarshalAs(UnmanagedType.BStr)] string pszStatusText);

            void EnableModeless(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnable);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                User32.MSG* lpmsg,
                ushort wID);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("00000115-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceUIWindow
        {
            IntPtr GetWindow();

            void ContextSensitiveHelp(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnterMode);

            void GetBorder(
                [Out] COMRECT lprectBorder);

            void RequestBorderSpace(
                [In] COMRECT pborderwidths);

            void SetBorderSpace(
                [In] COMRECT pborderwidths);

            void SetActiveObject(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleInPlaceActiveObject pActiveObject,
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszObjName);
        }

        [ComImport]
        [ComVisible(true)]
        [Guid("00000117-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceActiveObject
        {
            int GetWindow(out IntPtr hwnd);

            void ContextSensitiveHelp(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnterMode);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                User32.MSG* lpmsg);

            void OnFrameWindowActivate(
                [In] [MarshalAs(UnmanagedType.I4)] int fActivate);

            void OnDocWindowActivate(
                [In] [MarshalAs(UnmanagedType.I4)] int fActivate);

            void ResizeBorder(
                [In] COMRECT prcBorder,
                [In] IOleInPlaceUIWindow pUIWindow,
                [In] [MarshalAs(UnmanagedType.I4)] int fFrameWindow);

            void EnableModeless(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnable);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("B722BCC6-4E68-101B-A2BC-00AA00404770")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleDocumentView
        {
            void SetInPlaceSite(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleInPlaceSite pIPSite);

            [return: MarshalAs(UnmanagedType.Interface)]
            IOleInPlaceSite GetInPlaceSite();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetDocument();

            void SetRect(
                [In] COMRECT prcView);

            void GetRect(
                [Out] COMRECT prcView);

            void SetRectComplex(
                [In] COMRECT prcView,
                [In] COMRECT prcHScroll,
                [In] COMRECT prcVScroll,
                [In] COMRECT prcSizeBox);

            void Show(
                [In] [MarshalAs(UnmanagedType.I4)] int fShow);

            void UIActivate(
                [In] [MarshalAs(UnmanagedType.I4)] int fUIActivate);

            void Open();

            void CloseView(
                [In] [MarshalAs(UnmanagedType.U4)] int dwReserved);

            void SaveViewState(
                [In] [MarshalAs(UnmanagedType.Interface)]
                Ole32.IStream pstm);

            void ApplyViewState(
                [In] [MarshalAs(UnmanagedType.Interface)]
                Ole32.IStream pstm);

            void Clone(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleInPlaceSite pIPSiteNew,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                IOleDocumentView[] ppViewNew);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("00000119-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceSite
        {
            IntPtr GetWindow();

            void ContextSensitiveHelp(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnterMode);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int CanInPlaceActivate();

            void OnInPlaceActivate();

            void OnUIActivate();

            void GetWindowContext(
                [Out] out IOleInPlaceFrame ppFrame,
                [Out] out IOleInPlaceUIWindow ppDoc,
                [Out] COMRECT lprcPosRect,
                [Out] COMRECT lprcClipRect,
                [In] [Out] tagOIFI lpFrameInfo);

            [PreserveSig]
            HRESULT Scroll(Size scrollExtant);

            void OnUIDeactivate(
                [In] [MarshalAs(UnmanagedType.I4)] int fUndoable);

            void OnInPlaceDeactivate();

            void DiscardUndoState();

            void DeactivateAndUndo();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnPosRectChange(
                [In] COMRECT lprcPosRect);
        }

        private static uint wmMouseEnterMessage = uint.MaxValue;

        public static User32.WindowMessage WM_MOUSEENTER
        {
            get
            {
                if (wmMouseEnterMessage == uint.MaxValue)
                {
                    wmMouseEnterMessage = (uint)User32.RegisterWindowMessageW("WinFormsMouseEnter");
                }

                return (User32.WindowMessage)wmMouseEnterMessage;
            }
        }
        
        public static readonly int HDN_ENDTRACK = HDN_ENDTRACKW;

        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_BORDER = 0x00800000;
        public const int CS_DROPSHADOW = 0x00020000;
        public const int CS_DBLCLKS = 0x0008;
        public const int SRCCOPY = 0x00CC0020;
        public const int LVM_SETCOLUMNWIDTH = 0x1000 + 30;
        public const int LVM_GETHEADER = 0x1000 + 31;
        public const int LVM_CREATEDRAGIMAGE = 0x1000 + 33;
        public const int LVM_GETVIEWRECT = 0x1000 + 34;
        public const int LVM_GETTEXTCOLOR = 0x1000 + 35;
        public const int LVM_SETTEXTCOLOR = 0x1000 + 36;
        public const int LVM_GETTEXTBKCOLOR = 0x1000 + 37;
        public const int LVM_SETTEXTBKCOLOR = 0x1000 + 38;
        public const int LVM_GETTOPINDEX = 0x1000 + 39;
        public const int LVM_GETCOUNTPERPAGE = 0x1000 + 40;
        public const int LVM_GETORIGIN = 0x1000 + 41;
        public const int LVM_UPDATE = 0x1000 + 42;
        public const int LVM_SETITEMSTATE = 0x1000 + 43;
        public const int LVM_GETITEMSTATE = 0x1000 + 44;
        public const int LVM_GETITEMTEXTA = 0x1000 + 45;
        public const int LVM_GETITEMTEXTW = 0x1000 + 115;
        public const int LVM_SETITEMTEXTA = 0x1000 + 46;
        public const int LVM_SETITEMTEXTW = 0x1000 + 116;
        public const int LVSICF_NOINVALIDATEALL = 0x00000001;
        public const int LVSICF_NOSCROLL = 0x00000002;
        public const int LVM_SETITEMCOUNT = 0x1000 + 47;
        public const int LVM_SORTITEMS = 0x1000 + 48;
        public const int LVM_SETITEMPOSITION32 = 0x1000 + 49;
        public const int LVM_GETSELECTEDCOUNT = 0x1000 + 50;
        public const int LVM_GETITEMSPACING = 0x1000 + 51;
        public const int LVM_GETISEARCHSTRINGA = 0x1000 + 52;
        public const int LVM_GETISEARCHSTRINGW = 0x1000 + 117;
        public const int LVM_SETICONSPACING = 0x1000 + 53;
        public const int LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1000 + 54;
        public const int LVM_GETEXTENDEDLISTVIEWSTYLE = 0x1000 + 55;
        public const int LVS_EX_GRIDLINES = 0x00000001;
        public const int HDM_HITTEST = 0x1200 + 6;
        public const int HDM_GETITEMRECT = 0x1200 + 7;
        public const int HDM_SETIMAGELIST = 0x1200 + 8;
        public const int HDM_GETIMAGELIST = 0x1200 + 9;
        public const int HDM_ORDERTOINDEX = 0x1200 + 15;
        public const int HDM_CREATEDRAGIMAGE = 0x1200 + 16;
        public const int HDM_GETORDERARRAY = 0x1200 + 17;
        public const int HDM_SETORDERARRAY = 0x1200 + 18;
        public const int HDM_SETHOTDIVIDER = 0x1200 + 19;
        public const int HDN_ITEMCHANGINGA = 0 - 300 - 0;
        public const int HDN_ITEMCHANGINGW = 0 - 300 - 20;
        public const int HDN_ITEMCHANGEDA = 0 - 300 - 1;
        public const int HDN_ITEMCHANGEDW = 0 - 300 - 21;
        public const int HDN_ITEMCLICKA = 0 - 300 - 2;
        public const int HDN_ITEMCLICKW = 0 - 300 - 22;
        public const int HDN_ITEMDBLCLICKA = 0 - 300 - 3;
        public const int HDN_ITEMDBLCLICKW = 0 - 300 - 23;
        public const int HDN_DIVIDERDBLCLICKA = 0 - 300 - 5;
        public const int HDN_DIVIDERDBLCLICKW = 0 - 300 - 25;
        public const int HDN_BEGINTRACKA = 0 - 300 - 6;
        public const int HDN_BEGINTRACKW = 0 - 300 - 26;
        public const int HDN_ENDTRACKA = 0 - 300 - 7;
        public const int HDN_ENDTRACKW = 0 - 300 - 27;
        public const int HDN_TRACKA = 0 - 300 - 8;
        public const int HDN_TRACKW = 0 - 300 - 28;
        public const int HDN_GETDISPINFOA = 0 - 300 - 9;
        public const int HDN_GETDISPINFOW = 0 - 300 - 29;
        public const int HDN_BEGINDRAG = 0 - 300 - 10;
        public const int HDN_ENDDRAG = 0 - 300 - 11;
        public const int HC_ACTION = 0;
        public const int HIST_BACK = 0;
        public const int HHT_ONHEADER = 0x0002;
        public const int HHT_ONDIVIDER = 0x0004;
        public const int HHT_ONDIVOPEN = 0x0008;
        public const int HHT_ABOVE = 0x0100;
        public const int HHT_BELOW = 0x0200;
        public const int HHT_TORIGHT = 0x0400;
        public const int HHT_TOLEFT = 0x0800;
        public const int CWP_SKIPINVISIBLE = 0x0001;
        public const int TVM_GETITEMRECT = 0x1100 + 4;
        public const int TVM_GETCOUNT = 0x1100 + 5;
        public const int TVM_GETINDENT = 0x1100 + 6;
        public const int TVM_SETINDENT = 0x1100 + 7;
        public const int TVM_GETIMAGELIST = 0x1100 + 8;
        public const int TVSIL_NORMAL = 0;
        public const int TVSIL_STATE = 2;
        public const int TVM_SETIMAGELIST = 0x1100 + 9;
        public const int TVM_GETNEXTITEM = 0x1100 + 10;
        public const int TVGN_ROOT = 0x0000;
        public const int TV_FIRST = 0x1100;
        public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;
        public const int TVS_EX_FADEINOUTEXPANDOS = 0x0040;
        public const int TVS_EX_DOUBLEBUFFER = 0x0004;
        public const int LVS_EX_DOUBLEBUFFER = 0x00010000;
        public const int TVHT_ONITEMICON = 0x0002;
        public const int TVHT_ONITEMLABEL = 0x0004;
        public const int TVHT_ONITEMINDENT = 0x0008;
        public const int TVHT_ONITEMBUTTON = 0x0010;
        public const int TVHT_ONITEMRIGHT = 0x0020;
        public const int TVHT_ONITEMSTATEICON = 0x0040;
        public const int TVHT_ABOVE = 0x0100;
        public const int TVHT_BELOW = 0x0200;
        public const int TVHT_TORIGHT = 0x0400;
        public const int TVHT_TOLEFT = 0x0800;
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        public const int GW_MAX = 5;
        public const int GWL_HWNDPARENT = -8;
        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_CTL = 2;
        public const int SB_BOTH = 3;
        public const int SB_LINEUP = 0;
        public const int SB_LINELEFT = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINERIGHT = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGELEFT = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGERIGHT = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_LEFT = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_RIGHT = 7;
        public const int SB_ENDSCROLL = 8;
        public const int TVM_HITTEST = 0x1100 + 17;
        public const int LB_ADDSTRING = 0x0180;
        public const int LB_INSERTSTRING = 0x0181;
        public const int LB_DELETESTRING = 0x0182;
        public const int LB_SELITEMRANGEEX = 0x0183;
        public const int LB_RESETCONTENT = 0x0184;
        public const int LB_SETSEL = 0x0185;
        public const int LB_SETCURSEL = 0x0186;
        public const int LB_GETSEL = 0x0187;
        public const int LB_GETCURSEL = 0x0188;
        public const int LB_GETTEXT = 0x0189;
        public const int LB_GETTEXTLEN = 0x018A;
        public const int LB_GETCOUNT = 0x018B;
        public const int LB_SELECTSTRING = 0x018C;
        public const int LB_DIR = 0x018D;
        public const int LB_GETTOPINDEX = 0x018E;
        public const int LB_FINDSTRING = 0x018F;
        public const int LB_GETSELCOUNT = 0x0190;
        public const int LB_GETSELITEMS = 0x0191;
        public const int LB_SETTABSTOPS = 0x0192;
        public const int LB_GETHORIZONTALEXTENT = 0x0193;
        public const int LB_SETHORIZONTALEXTENT = 0x0194;
        public const int LB_SETCOLUMNWIDTH = 0x0195;
        public const int LB_ADDFILE = 0x0196;
        public const int LB_SETTOPINDEX = 0x0197;
        public const int LB_GETITEMRECT = 0x0198;
        public const int LB_GETITEMDATA = 0x0199;
        public const int LB_SETITEMDATA = 0x019A;
        public const int LB_SELITEMRANGE = 0x019B;
        public const int LB_SETANCHORINDEX = 0x019C;
        public const int LB_GETANCHORINDEX = 0x019D;
        public const int LB_SETCARETINDEX = 0x019E;
        public const int LB_GETCARETINDEX = 0x019F;
        public const int LB_SETITEMHEIGHT = 0x01A0;
        public const int LB_GETITEMHEIGHT = 0x01A1;
        public const int LB_FINDSTRINGEXACT = 0x01A2;
        public const int LB_SETLOCALE = 0x01A5;
        public const int LB_GETLOCALE = 0x01A6;
        public const int LB_SETCOUNT = 0x01A7;
        public const int LB_INITSTORAGE = 0x01A8;
        public const int LB_ITEMFROMPOINT = 0x01A9;
        public const int LB_MSGMAX = 0x01B0;
        public const int HTHSCROLL = 6;
        public const int HTVSCROLL = 7;
        public const int HTERROR = -2;
        public const int HTTRANSPARENT = -1;
        public const int HTNOWHERE = 0;
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const int HTSYSMENU = 3;
        public const int HTGROWBOX = 4;
        public const int HTSIZE = 4;
        public const int PRF_NONCLIENT = 0x00000002;
        public const int PRF_CLIENT = 0x00000004;
        public const int PRF_ERASEBKGND = 0x00000008;
        public const int PRF_CHILDREN = 0x00000010;

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [In] [Out] HDHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [In] [Out] TV_HITTESTINFO lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class HDHITTESTINFO
        {
            public int flags = 0;
            public int iItem = 0;
            public int pt_x = 0;
            public int pt_y = 0;
        }

        [StructLayout(LayoutKind.Sequential) /*leftover(noAutoOffset)*/]
        public sealed class tagOLEVERB
        {
            [MarshalAs(UnmanagedType.U4) /*leftover(offset=8, fuFlags)*/]
            public readonly int fuFlags = 0;

            [MarshalAs(UnmanagedType.U4) /*leftover(offset=12, grfAttribs)*/]
            public readonly int grfAttribs = 0;

            [MarshalAs(UnmanagedType.LPWStr) /*leftover(offset=4, customMarshal="UniStringMarshaller", lpszVerbName)*/]
            public readonly string lpszVerbName;

            [MarshalAs(UnmanagedType.I4) /*leftover(offset=0, lVerb)*/]
            public readonly int lVerb = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public class TV_HITTESTINFO
        {
            public int flags = 0;
            public int hItem = 0;
            public int pt_x = 0;
            public int pt_y = 0;
        }

        public delegate int ListViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

        public delegate void TimerProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

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

        internal class ActiveX
        {
            public const int OCM__BASE = 0x2000;
            public const int DVASPECT_CONTENT = 0x1;
            public const int DVASPECT_THUMBNAIL = 0x2;
            public const int DVASPECT_ICON = 0x4;
            public const int DVASPECT_DOCPRINT = 0x8;
            public const int OLEMISC_RECOMPOSEONRESIZE = 0x1;
            public const int OLEMISC_ONLYICONIC = 0x2;
            public const int OLEMISC_INSERTNOTREPLACE = 0x4;
            public const int OLEMISC_STATIC = 0x8;
            public const int OLEMISC_CANTLINKINSIDE = 0x10;
            public const int OLEMISC_CANLINKBYOLE1 = 0x20;
            public const int OLEMISC_ISLINKOBJECT = 0x40;
            public const int OLEMISC_INSIDEOUT = 0x80;
            public const int OLEMISC_ACTIVATEWHENVISIBLE = 0x100;
            public const int OLEMISC_RENDERINGISDEVICEINDEPENDENT = 0x200;
            public const int OLEMISC_INVISIBLEATRUNTIME = 0x400;
            public const int OLEMISC_ALWAYSRUN = 0x800;
            public const int OLEMISC_ACTSLIKEBUTTON = 0x1000;
            public const int OLEMISC_ACTSLIKELABEL = 0x2000;
            public const int OLEMISC_NOUIACTIVATE = 0x4000;
            public const int OLEMISC_ALIGNABLE = 0x8000;
            public const int OLEMISC_SIMPLEFRAME = 0x10000;
            public const int OLEMISC_SETCLIENTSITEFIRST = 0x20000;
            public const int OLEMISC_IMEMODE = 0x40000;
            public const int OLEMISC_IGNOREACTIVATEWHENVISIBLE = 0x80000;
            public const int OLEMISC_WANTSTOMENUMERGE = 0x100000;
            public const int OLEMISC_SUPPORTSMULTILEVELUNDO = 0x200000;
            public const int QACONTAINER_SHOWHATCHING = 0x1;
            public const int QACONTAINER_SHOWGRABHANDLES = 0x2;
            public const int QACONTAINER_USERMODE = 0x4;
            public const int QACONTAINER_DISPLAYASDEFAULT = 0x8;
            public const int QACONTAINER_UIDEAD = 0x10;
            public const int QACONTAINER_AUTOCLIP = 0x20;
            public const int QACONTAINER_MESSAGEREFLECT = 0x40;
            public const int QACONTAINER_SUPPORTSMNEMONICS = 0x80;
            public const int XFORMCOORDS_POSITION = 0x1;
            public const int XFORMCOORDS_SIZE = 0x2;
            public const int XFORMCOORDS_HIMETRICTOCONTAINER = 0x4;
            public const int XFORMCOORDS_CONTAINERTOHIMETRIC = 0x8;
            public const int PROPCAT_Nil = unchecked((int)0xFFFFFFFF);
            public const int PROPCAT_Misc = unchecked((int)0xFFFFFFFE);
            public const int PROPCAT_Font = unchecked((int)0xFFFFFFFD);
            public const int PROPCAT_Position = unchecked((int)0xFFFFFFFC);
            public const int PROPCAT_Appearance = unchecked((int)0xFFFFFFFB);
            public const int PROPCAT_Behavior = unchecked((int)0xFFFFFFFA);
            public const int PROPCAT_Data = unchecked((int)0xFFFFFFF9);
            public const int PROPCAT_List = unchecked((int)0xFFFFFFF8);
            public const int PROPCAT_Text = unchecked((int)0xFFFFFFF7);
            public const int PROPCAT_Scale = unchecked((int)0xFFFFFFF6);
            public const int PROPCAT_DDE = unchecked((int)0xFFFFFFF5);
            public const int GC_WCH_SIBLING = 0x1;
            public const int GC_WCH_CONTAINER = 0x2;
            public const int GC_WCH_CONTAINED = 0x3;
            public const int GC_WCH_ALL = 0x4;
            public const int GC_WCH_FREVERSEDIR = 0x8000000;
            public const int GC_WCH_FONLYNEXT = 0x10000000;
            public const int GC_WCH_FONLYPREV = 0x20000000;
            public const int GC_WCH_FSELECTED = 0x40000000;
            public const int OLECONTF_EMBEDDINGS = 0x1;
            public const int OLECONTF_LINKS = 0x2;
            public const int OLECONTF_OTHERS = 0x4;
            public const int OLECONTF_ONLYUSER = 0x8;
            public const int OLECONTF_ONLYIFRUNNING = 0x10;
            public const int ALIGN_MIN = 0x0;
            public const int ALIGN_NO_CHANGE = 0x0;
            public const int ALIGN_TOP = 0x1;
            public const int ALIGN_BOTTOM = 0x2;
            public const int ALIGN_LEFT = 0x3;
            public const int ALIGN_RIGHT = 0x4;
            public const int ALIGN_MAX = 0x4;
            public const int OLEVERBATTRIB_NEVERDIRTIES = 0x1;
            public const int OLEVERBATTRIB_ONCONTAINERMENU = 0x2;

            public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
        }

        public static bool Succeeded(int hr)
        {
            return hr >= 0;
        }

        [ComImport]
        [Guid("00000104-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumOLEVERB
        {
            [PreserveSig]
            int Next(
                [MarshalAs(UnmanagedType.U4)] int celt,
                [In]
                [Out]
                // VSW#409990: Add an "In" so that CLR will initialize the structure.  Otherwise, we crash when marshalling.
                tagOLEVERB rgelt,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                int[] pceltFetched);

            [PreserveSig]
            int Skip(
                [In] [MarshalAs(UnmanagedType.U4)] int celt);

            void Reset();

            void Clone(
                out IEnumOLEVERB ppenum);
        }

        [ComImport]
        [Guid("00000105-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumSTATDATA
        {
            void Next(
                [In] [MarshalAs(UnmanagedType.U4)] int celt,
                [Out] STATDATA rgelt,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                int[] pceltFetched);

            void Skip(
                [In] [MarshalAs(UnmanagedType.U4)] int celt);

            void Reset();

            void Clone(
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                IEnumSTATDATA[] ppenum);
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class STATDATA
        {
            [MarshalAs(UnmanagedType.U4)] public readonly int advf = 0;

            [MarshalAs(UnmanagedType.U4)] public readonly int dwConnection = 0;
        }

        [ComImport]
        [Guid("00000103-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumFORMATETC
        {
            [PreserveSig]
            int Next(
                [In] [MarshalAs(UnmanagedType.U4)] int celt,
                [Out] FORMATETC rgelt,
                [In] [Out] [MarshalAs(UnmanagedType.LPArray)]
                int[] pceltFetched);

            [PreserveSig]
            int Skip(
                [In] [MarshalAs(UnmanagedType.U4)] int celt);

            [PreserveSig]
            int Reset();

            [PreserveSig]
            int Clone(
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                IEnumFORMATETC[] ppenum);
        }

        public const int CHILDID_SELF = 0;
        public const int OBJID_WINDOW = 0x00000000;
        public const int OBJID_CLIENT = unchecked((int)0xFFFFFFFC);
        public const string uuid_IAccessible = "{618736E0-3C3D-11CF-810C-00AA00389B71}";

        public const int QS_KEY = 0x0001,
            QS_MOUSEMOVE = 0x0002,
            QS_MOUSEBUTTON = 0x0004,
            QS_POSTMESSAGE = 0x0008,
            QS_TIMER = 0x0010,
            QS_PAINT = 0x0020,
            QS_SENDMESSAGE = 0x0040,
            QS_HOTKEY = 0x0080,
            QS_ALLPOSTMESSAGE = 0x0100,
            QS_MOUSE = QS_MOUSEMOVE | QS_MOUSEBUTTON,
            QS_INPUT = QS_MOUSE | QS_KEY,
            QS_ALLEVENTS = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY,
            QS_ALLINPUT = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE;

        public const int MWMO_INPUTAVAILABLE = 0x0004; // don't use MWMO_WAITALL, see ddb#176342

        public const int GWL_EXSTYLE = -20,
            GWL_STYLE = -16;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LOGFONT
        {
            public readonly byte lfCharSet;
            public readonly byte lfClipPrecision;
            public readonly int lfEscapement;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly string lfFaceName;

            public readonly int lfHeight;
            public readonly byte lfItalic;
            public readonly int lfOrientation;
            public readonly byte lfOutPrecision;
            public readonly byte lfPitchAndFamily;
            public readonly byte lfQuality;
            public readonly byte lfStrikeOut;
            public readonly byte lfUnderline;
            public readonly int lfWeight;
            public readonly int lfWidth;

            public LOGFONT()
            {
            }

            public LOGFONT(LOGFONT lf)
            {
                lfHeight = lf.lfHeight;
                lfWidth = lf.lfWidth;
                lfEscapement = lf.lfEscapement;
                lfOrientation = lf.lfOrientation;
                lfWeight = lf.lfWeight;
                lfItalic = lf.lfItalic;
                lfUnderline = lf.lfUnderline;
                lfStrikeOut = lf.lfStrikeOut;
                lfCharSet = lf.lfCharSet;
                lfOutPrecision = lf.lfOutPrecision;
                lfClipPrecision = lf.lfClipPrecision;
                lfQuality = lf.lfQuality;
                lfPitchAndFamily = lf.lfPitchAndFamily;
                lfFaceName = lf.lfFaceName;
            }

            public override string ToString()
            {
                return
                    "lfHeight=" + lfHeight + ", " +
                    "lfWidth=" + lfWidth + ", " +
                    "lfEscapement=" + lfEscapement + ", " +
                    "lfOrientation=" + lfOrientation + ", " +
                    "lfWeight=" + lfWeight + ", " +
                    "lfItalic=" + lfItalic + ", " +
                    "lfUnderline=" + lfUnderline + ", " +
                    "lfStrikeOut=" + lfStrikeOut + ", " +
                    "lfCharSet=" + lfCharSet + ", " +
                    "lfOutPrecision=" + lfOutPrecision + ", " +
                    "lfClipPrecision=" + lfClipPrecision + ", " +
                    "lfQuality=" + lfQuality + ", " +
                    "lfPitchAndFamily=" + lfPitchAndFamily + ", " +
                    "lfFaceName=" + lfFaceName;
            }
        }

        public const int SPI_GETNONCLIENTMETRICS = 41;

        //scoberry Nov 1, 2004: Removed DispatchMessageA, DispatchMessageW, GetClientRect, PeekMessageA
        //PeekMessageW, PostMessage, SendMessage(IntPtr, int, IntPtr, ref RECT), SendMessage(IntPtr, int, ref short, ref short)
        //SendMessage(IntPtr, int, bool, IntPtr), SendMessage(IntPtr, int, IntPtr, ListViewCompareCallback),
        //SendMessageW, SendMessageA, ValidateRect(IntPtr, ref RECT), ValidateRgn(IntPtr, IntPtr)
        //COMRECT.FromXYWH, RECT.FromXYWH

        public const int MAX_PATH = 260;
    }
}
