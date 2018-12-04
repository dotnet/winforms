// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Design
{
    internal static class NativeMethods
    {
        public static readonly HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public static readonly int PS_SOLID = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TEXTMETRIC
        {
            public readonly int tmAscent = 0;
            public readonly int tmAveCharWidth = 0;
            public readonly char tmBreakChar;
            public readonly byte tmCharSet = 0;
            public readonly char tmDefaultChar;
            public readonly int tmDescent = 0;
            public readonly int tmDigitizedAspectX = 0;
            public readonly int tmDigitizedAspectY = 0;
            public readonly int tmExternalLeading = 0;
            public readonly char tmFirstChar;
            public readonly int tmHeight = 0;
            public readonly int tmInternalLeading = 0;
            public readonly byte tmItalic = 0;
            public readonly char tmLastChar;
            public readonly int tmMaxCharWidth = 0;
            public readonly int tmOverhang = 0;
            public readonly byte tmPitchAndFamily = 0;
            public readonly byte tmStruckOut = 0;
            public readonly byte tmUnderlined = 0;
            public readonly int tmWeight = 0;
        }

        public delegate bool EnumChildrenCallback(IntPtr hwnd, IntPtr lParam);

        // Investigate removing this if the duplicate code in OleDragDropHandler.cs is removed
        public const int HOLLOW_BRUSH = 5;

        public const int WM_USER = 0x0400,
            WM_CLOSE = 0x0010,
            WM_GETDLGCODE = 0x0087,
            WM_MOUSEMOVE = 0x0200,
            WM_NOTIFY = 0x004E,
            DLGC_WANTALLKEYS = 0x0004,
            NM_CLICK = 0 - 0 - 2,
            WM_REFLECT = WM_USER + 0x1C00,
            BM_SETIMAGE = 0x00F7,
            IMAGE_ICON = 1,
            WM_DESTROY = 0x0002,
            BS_ICON = 0x00000040,
            EM_SETMARGINS = 0x00D3,
            EC_LEFTMARGIN = 0x0001,
            EC_RIGHTMARGIN = 0x0002,
            IDOK = 1,
            WM_INITDIALOG = 0x0110;
        public const int VK_PROCESSKEY = 0xE5;

        public const int STGM_READ = 0x00000000,
            STGM_WRITE = 0x00000001,
            STGM_READWRITE = 0x00000002,
            STGM_SHARE_EXCLUSIVE = 0x00000010,
            STGM_CREATE = 0x00001000,
            STGM_TRANSACTED = 0x00010000,
            STGM_CONVERT = 0x00020000,
            WM_COMMAND = 0x0111,
            CC_FULLOPEN = 0x00000002,
            CC_ENABLETEMPLATEHANDLE = 0x00000040,
            STGM_DELETEONRELEASE = 0x04000000;

        public const int RECO_PASTE = 0x00000000; // paste from clipboard
        public const int RECO_DROP = 0x00000001; // drop

        public const int LOGPIXELSX = 88,
            LOGPIXELSY = 90;

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int MultiByteToWideChar(int CodePage, int dwFlags,
            byte[] lpMultiByteStr, int cchMultiByte, char[] lpWideCharStr, int cchWideChar);

        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public extern static IntPtr SendDlgItemMessage(IntPtr hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool EnableWindow(IntPtr hWnd, bool enable);
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetDlgItemInt(IntPtr hWnd, int nIDDlgItem, bool[] err, bool signed);
        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [StructLayout(LayoutKind.Sequential)]
        public class NMHEADER
        {
            public readonly int code = 0;
            public readonly int hwndFrom = 0;
            public readonly int iButton = 0;
            public readonly int idFrom = 0;
            public readonly int iItem = 0;
            public readonly int pItem = 0; // HDITEM*
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public readonly int x;
            public readonly int y;
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public readonly IntPtr hwnd;
            public readonly IntPtr hwndInsertAfter;
            public readonly int x;
            public readonly int y;
            public readonly int cx;
            public readonly int cy;
            public readonly int flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public class TV_ITEM
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
        public class NMHDR
        {
            public readonly int code = 0;
            public readonly int hwndFrom = 0;
            public readonly int idFrom = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NMTREEVIEW
        {
            public readonly int action = 0;
            public readonly TV_ITEM itemNew = null;
            public readonly TV_ITEM itemOld = null;
            public readonly NMHDR nmhdr = null;
            public readonly POINT ptDrag = null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TCHITTESTINFO
        {
            public readonly TabControlHitTest flags;
            public readonly Point pt;
        }

        public const int TCM_HITTEST = 4877;

        [Flags]
        public enum TabControlHitTest
        {
            TCHT_NOWHERE = 0x0001,
            TCHT_ONITEMICON = 0x0002,
            TCHT_ONITEMLABEL = 0x0004
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public class TRACKMOUSEEVENT
        {
            public readonly int cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
            public readonly int dwFlags;
            public readonly int dwHoverTime = 0;
            public readonly IntPtr hwndTrack;
        }

        [ComVisible(false)]
        public enum StructFormat
        {
            Ansi = 1,
            Unicode = 2,
            Auto = 3
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

        public const int
            PM_NOREMOVE = 0x0000,
            PM_REMOVE = 0x0001;

        public const int
            WM_CHAR = 0x0102;

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEHOOKSTRUCT
        {
            // pt was a by-value POINT structure
            public readonly int pt_x;
            public readonly int pt_y;
            public readonly IntPtr hWnd;
            public readonly int wHitTestCode;
            public readonly int dwExtraInfo;
        }

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetClientRect(IntPtr hWnd, [In] [Out] COMRECT rect);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool PeekMessage([In] [Out] ref MSG msg, IntPtr hwnd, int msgMin, int msgMax, int remove);

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;

            public int time;

            // pt was a by-value POINT structure
            public int pt_x;
            public int pt_y;
        }

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

            public COMRECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public sealed class FORMATETC
        {
            [MarshalAs(UnmanagedType.I4)] public readonly int cfFormat = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int dwAspect = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int lindex = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly IntPtr ptd = IntPtr.Zero;

            [MarshalAs(UnmanagedType.I4)] public readonly int tymed = 0;
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public class STGMEDIUM
        {
            public readonly IntPtr pUnkForRelease = IntPtr.Zero;

            [MarshalAs(UnmanagedType.I4)] public readonly int tymed = 0;

            public readonly IntPtr unionmember = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class OLECMD
        {
            [MarshalAs(UnmanagedType.U4)] public readonly int cmdf = 0;

            [MarshalAs(UnmanagedType.U4)] public readonly int cmdID = 0;
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagOIFI
        {
            [MarshalAs(UnmanagedType.U4)] public readonly int cAccelEntries;

            [MarshalAs(UnmanagedType.U4)] public readonly int cb;

            [MarshalAs(UnmanagedType.I4)] public readonly int fMDIApp;

            public readonly IntPtr hAccel;
            public readonly IntPtr hwndFrame;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagSIZE
        {
            [MarshalAs(UnmanagedType.I4)] public readonly int cx = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int cy = 0;
        }

        [ComVisible(true)]
        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagSIZEL
        {
            [MarshalAs(UnmanagedType.I4)] public readonly int cx = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int cy = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagLOGPALETTE
        {
            [MarshalAs(UnmanagedType.U2)] public readonly short palNumEntries = 0;

            [MarshalAs(UnmanagedType.U2)] public readonly short palVersion = 0;

            // UNMAPPABLE: palPalEntry: Cannot be used as a structure field.
            //   /** @com.structmap(UNMAPPABLE palPalEntry) */
            //  public UNMAPPABLE palPalEntry;
        }

        public class DOCHOSTUIDBLCLICK
        {
            public const int DEFAULT = 0x0;
            public const int SHOWPROPERTIES = 0x1;
            public const int SHOWCODE = 0x2;
        }

        public class DOCHOSTUIFLAG
        {
            public const int DIALOG = 0x1;
            public const int DISABLE_HELP_MENU = 0x2;
            public const int NO3DBORDER = 0x4;
            public const int SCROLL_NO = 0x8;
            public const int DISABLE_SCRIPT_INACTIVE = 0x10;
            public const int OPENNEWWIN = 0x20;
            public const int DISABLE_OFFSCREEN = 0x40;
            public const int FLAT_SCROLLBAR = 0x80;
            public const int DIV_BLOCKDEFAULT = 0x100;
            public const int ACTIVATE_CLIENTHIT_ONLY = 0x200;
            public const int DISABLE_COOKIE = 0x400;
        }

        [ComVisible(true)]
        [StructLayout(LayoutKind.Sequential)]
        public class DOCHOSTUIINFO
        {
            [MarshalAs(UnmanagedType.U4)] public readonly int cbSize = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int dwDoubleClick = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int dwFlags = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int dwReserved1 = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int dwReserved2 = 0;
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDocHostUIHandler
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowContextMenu(
                [In] [MarshalAs(UnmanagedType.U4)] int dwID,
                [In] POINT pt,
                [In] [MarshalAs(UnmanagedType.Interface)]
                object pcmdtReserved,
                [In] [MarshalAs(UnmanagedType.Interface)]
                object pdispReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetHostInfo(
                [In] [Out] DOCHOSTUIINFO info);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowUI(
                [In] [MarshalAs(UnmanagedType.I4)] int dwID,
                [In] IOleInPlaceActiveObject activeObject,
                [In] IOleCommandTarget commandTarget,
                [In] IOleInPlaceFrame frame,
                [In] IOleInPlaceUIWindow doc);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int HideUI();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int UpdateUI();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnableModeless(
                [In] [MarshalAs(UnmanagedType.Bool)] bool fEnable);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnDocWindowActivate(
                [In] [MarshalAs(UnmanagedType.Bool)] bool fActivate);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnFrameWindowActivate(
                [In] [MarshalAs(UnmanagedType.Bool)] bool fActivate);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ResizeBorder(
                [In] COMRECT rect,
                [In] IOleInPlaceUIWindow doc,
                bool fFrameWindow);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateAccelerator(
                [In] ref MSG msg,
                [In] ref Guid group,
                [In] [MarshalAs(UnmanagedType.I4)] int nCmdID);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetOptionKeyPath(
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                string[] pbstrKey,
                [In] [MarshalAs(UnmanagedType.U4)] int dw);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetDropTarget(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleDropTarget pDropTarget,
                [Out] [MarshalAs(UnmanagedType.Interface)]
                out IOleDropTarget ppDropTarget);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetExternal(
                [Out] [MarshalAs(UnmanagedType.Interface)]
                out object ppDispatch);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateUrl(
                [In] [MarshalAs(UnmanagedType.U4)] int dwTranslate,
                [In] [MarshalAs(UnmanagedType.LPWStr)] string strURLIn,
                [Out] [MarshalAs(UnmanagedType.LPWStr)]
                out string pstrURLOut);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int FilterDataObject(
                IComDataObject pDO,
                out IComDataObject ppDORet);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("00000122-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleDropTarget
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OleDragEnter(
                IComDataObject pDataObj,
                [In] [MarshalAs(UnmanagedType.U4)] int grfKeyState,
                [In] POINTL pt,
                [In] [Out] [MarshalAs(UnmanagedType.I4)]
                ref int pdwEffect);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OleDragOver(
                [In] [MarshalAs(UnmanagedType.U4)] int grfKeyState,
                [In] POINTL pt,
                [In] [Out] [MarshalAs(UnmanagedType.I4)]
                ref int pdwEffect);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OleDragLeave();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OleDrop(
                IComDataObject pDataObj,
                [In] [MarshalAs(UnmanagedType.U4)] int grfKeyState,
                [In] POINTL pt,
                [In] [Out] [MarshalAs(UnmanagedType.I4)]
                ref int pdwEffect);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleCommandTarget
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryStatus(
                ref Guid pguidCmdGroup,
                int cCmds,
                [In] [Out] OLECMD prgCmds,
                [In] [Out] string pCmdText);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Exec(
                ref Guid pguidCmdGroup,
                int nCmdID,
                int nCmdexecopt,
                // we need to have this an array because callers need to be able to specify NULL or VT_NULL
                [In] [MarshalAs(UnmanagedType.LPArray)]
                object[] pvaIn,
                IntPtr pvaOut);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("00000116-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceFrame
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

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateAccelerator(
                [In] ref MSG lpmsg,
                [In] [MarshalAs(UnmanagedType.U2)] short wID);
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

        [ComVisible(true)]
        [ComImport]
        [Guid("00000117-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceActiveObject
        {
            int GetWindow(out IntPtr hwnd);

            void ContextSensitiveHelp(
                [In] [MarshalAs(UnmanagedType.I4)] int fEnterMode);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateAccelerator(
                [In] ref MSG lpmsg);

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
        [Guid("0000011B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleContainer
        {
            void ParseDisplayName(
                [In] [MarshalAs(UnmanagedType.Interface)]
                object pbc,
                [In] [MarshalAs(UnmanagedType.BStr)] string pszDisplayName,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                int[] pchEaten,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                object[] ppmkOut);

            void EnumObjects(
                [In] [MarshalAs(UnmanagedType.U4)] int grfFlags,
                [Out] [MarshalAs(UnmanagedType.Interface)]
                out object ppenum);

            void LockContainer(
                [In] [MarshalAs(UnmanagedType.I4)] int fLock);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("00000118-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleClientSite
        {
            void SaveObject();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetMoniker(
                [In] [MarshalAs(UnmanagedType.U4)] int dwAssign,
                [In] [MarshalAs(UnmanagedType.U4)] int dwWhichMoniker);

            [PreserveSig]
            int GetContainer(
                [Out] out IOleContainer ppContainer);

            void ShowObject();

            void OnShowWindow(
                [In] [MarshalAs(UnmanagedType.I4)] int fShow);

            void RequestNewObjectLayout();
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("B722BCC7-4E68-101B-A2BC-00AA00404770")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleDocumentSite
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ActivateMe(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleDocumentView pViewToActivate);
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
                IStream pstm);

            void ApplyViewState(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IStream pstm);

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

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Scroll(
                [In] [MarshalAs(UnmanagedType.U4)] tagSIZE scrollExtant);

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

        [ComVisible(true)]
        [ComImport]
        [Guid("0000000C-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IStream
        {
            [return: MarshalAs(UnmanagedType.I4)]
            int Read(
                [In] IntPtr buf,
                [In] [MarshalAs(UnmanagedType.I4)] int len);

            [return: MarshalAs(UnmanagedType.I4)]
            int Write(
                [In] IntPtr buf,
                [In] [MarshalAs(UnmanagedType.I4)] int len);

            [return: MarshalAs(UnmanagedType.I8)]
            long Seek(
                [In] [MarshalAs(UnmanagedType.I8)] long dlibMove,
                [In] [MarshalAs(UnmanagedType.I4)] int dwOrigin);

            void SetSize(
                [In] [MarshalAs(UnmanagedType.I8)] long libNewSize);

            [return: MarshalAs(UnmanagedType.I8)]
            long CopyTo(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IStream pstm,
                [In] [MarshalAs(UnmanagedType.I8)] long cb,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                long[] pcbRead);

            void Commit(
                [In] [MarshalAs(UnmanagedType.I4)] int grfCommitFlags);

            void Revert();

            void LockRegion(
                [In] [MarshalAs(UnmanagedType.I8)] long libOffset,
                [In] [MarshalAs(UnmanagedType.I8)] long cb,
                [In] [MarshalAs(UnmanagedType.I4)] int dwLockType);

            void UnlockRegion(
                [In] [MarshalAs(UnmanagedType.I8)] long libOffset,
                [In] [MarshalAs(UnmanagedType.I8)] long cb,
                [In] [MarshalAs(UnmanagedType.I4)] int dwLockType);

            void Stat(
                [In] IntPtr pStatstg,
                [In] [MarshalAs(UnmanagedType.I4)] int grfStatFlag);

            [return: MarshalAs(UnmanagedType.Interface)]
            IStream Clone();
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("00000112-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleObject
        {
            [PreserveSig]
            int SetClientSite(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleClientSite pClientSite);

            [PreserveSig]
            int GetClientSite(out IOleClientSite site);

            [PreserveSig]
            int SetHostNames(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string szContainerApp,
                [In] [MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);

            [PreserveSig]
            int Close(
                [In] [MarshalAs(UnmanagedType.I4)] int dwSaveOption);

            [PreserveSig]
            int SetMoniker(
                [In] [MarshalAs(UnmanagedType.U4)] int dwWhichMoniker,
                [In] [MarshalAs(UnmanagedType.Interface)]
                object pmk);

            [PreserveSig]
            int GetMoniker(
                [In] [MarshalAs(UnmanagedType.U4)] int dwAssign,
                [In] [MarshalAs(UnmanagedType.U4)] int dwWhichMoniker,
                out object moniker);

            [PreserveSig]
            int InitFromData(
                IComDataObject pDataObject,
                [In] [MarshalAs(UnmanagedType.I4)] int fCreation,
                [In] [MarshalAs(UnmanagedType.U4)] int dwReserved);

            [PreserveSig]
            int GetClipboardData(
                [In] [MarshalAs(UnmanagedType.U4)] int dwReserved,
                out IComDataObject data);

            [PreserveSig]
            int DoVerb(
                [In] [MarshalAs(UnmanagedType.I4)] int iVerb,
                [In] IntPtr lpmsg,
                [In] [MarshalAs(UnmanagedType.Interface)]
                IOleClientSite pActiveSite,
                [In] [MarshalAs(UnmanagedType.I4)] int lindex,
                [In] IntPtr hwndParent,
                [In] COMRECT lprcPosRect);

            [PreserveSig]
            int EnumVerbs(out IEnumOLEVERB e);

            [PreserveSig]
            int OleUpdate();

            [PreserveSig]
            int IsUpToDate();

            [PreserveSig]
            int GetUserClassID(
                [In] [Out] ref Guid pClsid);

            [PreserveSig]
            int GetUserType(
                [In] [MarshalAs(UnmanagedType.U4)] int dwFormOfType,
                [Out] [MarshalAs(UnmanagedType.LPWStr)]
                out string userType);

            [PreserveSig]
            int SetExtent(
                [In] [MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
                [In] tagSIZEL pSizel);

            [PreserveSig]
            int GetExtent(
                [In] [MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
                [Out] tagSIZEL pSizel);

            [PreserveSig]
            int Advise(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IAdviseSink pAdvSink,
                out int cookie);

            [PreserveSig]
            int Unadvise(
                [In] [MarshalAs(UnmanagedType.U4)] int dwConnection);

            [PreserveSig]
            int EnumAdvise(out object e);

            [PreserveSig]
            int GetMiscStatus(
                [In] [MarshalAs(UnmanagedType.U4)] int dwAspect,
                out int misc);

            [PreserveSig]
            int SetColorScheme(
                [In] tagLOGPALETTE pLogpal);
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("0000010F-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAdviseSink
        {
            void OnDataChange(
                [In] FORMATETC pFormatetc,
                [In] STGMEDIUM pStgmed);

            void OnViewChange(
                [In] [MarshalAs(UnmanagedType.U4)] int dwAspect,
                [In] [MarshalAs(UnmanagedType.I4)] int lindex);

            void OnRename(
                [In] [MarshalAs(UnmanagedType.Interface)]
                object pmk);

            void OnSave();

            void OnClose();
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("7FD52380-4E07-101B-AE2D-08002B2EC713")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersistStreamInit
        {
            void GetClassID(
                [In] [Out] ref Guid pClassID);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int IsDirty();

            void Load(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IStream pstm);

            void Save(
                [In] [MarshalAs(UnmanagedType.Interface)]
                IStream pstm,
                [In] [MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

            void GetSizeMax(
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                long pcbSize);

            void InitNew();
        }

        [ComVisible(true)]
        [ComImport]
        [Guid("25336920-03F9-11CF-8FD0-00AA00686F13")]
        public class HTMLDocument
        {
        }

#if LEGACY
        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("626FC520-A41E-11CF-A731-00A0C9082637"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDocument {

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetScript();
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("332C4425-26CB-11D0-B483-00C04FD90119"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDocument2 {

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetScript();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetAll();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement GetBody();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement GetActiveElement();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetImages();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetApplets();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetLinks();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetForms();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetAnchors();

            void SetTitle(
                         [In, MarshalAs(UnmanagedType.BStr)]
                         string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTitle();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetScripts();

            void SetDesignMode(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDesignMode();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetSelection();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetReadyState();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetFrames();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetEmbeds();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetPlugins();

            void SetAlinkColor(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            object GetAlinkColor();

            void SetBgColor(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            object GetBgColor();

            void SetFgColor(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            object GetFgColor();

            void SetLinkColor(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            object GetLinkColor();

            void SetVlinkColor(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetVlinkColor();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetReferrer();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetLocation();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetLastModified();

            void SetURL(
                       [In, MarshalAs(UnmanagedType.BStr)]
                       string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetURL();

            void SetDomain(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDomain();

            void SetCookie(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetCookie();

            void SetExpando(
                           [In, MarshalAs(UnmanagedType.Bool)]
                           bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetExpando();

            void SetCharset(
                           [In, MarshalAs(UnmanagedType.BStr)]
                           string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetCharset();

            void SetDefaultCharset(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDefaultCharset();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetMimeType();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFileSize();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFileCreatedDate();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFileModifiedDate();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFileUpdatedDate();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetSecurity();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetProtocol();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetNameProp();

            void DummyWrite(
                           [In, MarshalAs(UnmanagedType.I4)]
                           int psarray);

            void DummyWriteln(
                             [In, MarshalAs(UnmanagedType.I4)]
                             int psarray);

            [return: MarshalAs(UnmanagedType.Interface)]
            object Open(
                       [In, MarshalAs(UnmanagedType.BStr)]
                       string URL,
                       [In, MarshalAs(UnmanagedType.Struct)]
                       Object name,
                       [In, MarshalAs(UnmanagedType.Struct)]
                       Object features,
                       [In, MarshalAs(UnmanagedType.Struct)]
                       Object replace);

            void Close();

            void Clear();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool QueryCommandSupported(
                                      [In, MarshalAs(UnmanagedType.BStr)]
                                      string cmdID);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool QueryCommandEnabled(
                                    [In, MarshalAs(UnmanagedType.BStr)]
                                    string cmdID);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool QueryCommandState(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string cmdID);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool QueryCommandIndeterm(
                                     [In, MarshalAs(UnmanagedType.BStr)]
                                     string cmdID);

            [return: MarshalAs(UnmanagedType.BStr)]
            string QueryCommandText(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string cmdID);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object QueryCommandValue(
                                    [In, MarshalAs(UnmanagedType.BStr)]
                                    string cmdID);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool ExecCommand(
                            [In, MarshalAs(UnmanagedType.BStr)]
                            string cmdID,
                            [In, MarshalAs(UnmanagedType.Bool)]
                            bool showUI,
                            [In, MarshalAs(UnmanagedType.Struct)]
                            Object value);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool ExecCommandShowHelp(
                                    [In, MarshalAs(UnmanagedType.BStr)]
                                    string cmdID);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement CreateElement(
                                      [In, MarshalAs(UnmanagedType.BStr)]
                                      string eTag);

            void SetOnhelp(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnhelp();

            void SetOnclick(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnclick();

            void SetOndblclick(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndblclick();

            void SetOnkeyup(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeyup();

            void SetOnkeydown(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeydown();

            void SetOnkeypress(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeypress();

            void SetOnmouseup(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmouseup();

            void SetOnmousedown(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmousedown();

            void SetOnmousemove(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmousemove();

            void SetOnmouseout(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmouseout();

            void SetOnmouseover(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmouseover();

            void SetOnreadystatechange(
                                      [In, MarshalAs(UnmanagedType.Struct)]
                                      Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnreadystatechange();

            void SetOnafterupdate(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnafterupdate();

            void SetOnrowexit(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnrowexit();

            void SetOnrowenter(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnrowenter();

            void SetOndragstart(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndragstart();

            void SetOnselectstart(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnselectstart();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement ElementFromPoint(
                                         [In, MarshalAs(UnmanagedType.I4)]
                                         int x,
                                         [In, MarshalAs(UnmanagedType.I4)]
                                         int y);

            [return: MarshalAs(UnmanagedType.Interface)]
            /*IHTMLWindow2*/ object GetParentWindow();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetStyleSheets();

            void SetOnbeforeupdate(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforeupdate();

            void SetOnerrorupdate(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnerrorupdate();

            [return: MarshalAs(UnmanagedType.BStr)]
            string toString();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLStyleSheet CreateStyleSheet(
                                            [In, MarshalAs(UnmanagedType.BStr)]
                                            string bstrHref,
                                            [In, MarshalAs(UnmanagedType.I4)]
                                            int lIndex);
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F1FF-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElement {

            void SetAttribute(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string strAttributeName,
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object AttributeValue,
                             [In, MarshalAs(UnmanagedType.I4)]
                             int lFlags);

            void GetAttribute(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string strAttributeName,
                             [In, MarshalAs(UnmanagedType.I4)]
                             int lFlags,
                             [Out, MarshalAs(UnmanagedType.LPArray)]
                             Object[] pvars);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool RemoveAttribute(
                                [In, MarshalAs(UnmanagedType.BStr)]
                                string strAttributeName,
                                [In, MarshalAs(UnmanagedType.I4)]
                                int lFlags);

            void SetClassName(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetClassName();

            void SetId(
                      [In, MarshalAs(UnmanagedType.BStr)]
                      string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetId();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTagName();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement GetParentElement();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLStyle GetStyle();

            void SetOnhelp(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnhelp();

            void SetOnclick(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnclick();

            void SetOndblclick(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndblclick();

            void SetOnkeydown(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeydown();

            void SetOnkeyup(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeyup();

            void SetOnkeypress(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeypress();

            void SetOnmouseout(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmouseout();

            void SetOnmouseover(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmouseover();

            void SetOnmousemove(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmousemove();

            void SetOnmousedown(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmousedown();

            void SetOnmouseup(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmouseup();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDocument2 GetDocument();

            void SetTitle(
                         [In, MarshalAs(UnmanagedType.BStr)]
                         string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTitle();

            void SetLanguage(
                            [In, MarshalAs(UnmanagedType.BStr)]
                            string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetLanguage();

            void SetOnselectstart(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnselectstart();

            void ScrollIntoView(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object varargStart);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool Contains(
                         [In, MarshalAs(UnmanagedType.Interface)]
                         IHTMLElement pChild);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetSourceIndex();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetRecordNumber();

            void SetLang(
                        [In, MarshalAs(UnmanagedType.BStr)]
                        string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetLang();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetOffsetLeft();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetOffsetTop();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetOffsetWidth();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetOffsetHeight();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement GetOffsetParent();

            void SetInnerHTML(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetInnerHTML();

            void SetInnerText(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetInnerText();

            void SetOuterHTML(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetOuterHTML();

            void SetOuterText(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetOuterText();

            void InsertAdjacentHTML(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string where,
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string html);

            void InsertAdjacentText(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string where,
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string text);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement GetParentTextEdit();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetIsTextEdit();

            void Click();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetFilters();

            void SetOndragstart(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndragstart();

            [return: MarshalAs(UnmanagedType.BStr)]
            string toString();

            void SetOnbeforeupdate(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforeupdate();

            void SetOnafterupdate(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnafterupdate();

            void SetOnerrorupdate(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnerrorupdate();

            void SetOnrowexit(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnrowexit();

            void SetOnrowenter(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnrowenter();

            void SetOndatasetchanged(
                                    [In, MarshalAs(UnmanagedType.Struct)]
                                    Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndatasetchanged();

            void SetOndataavailable(
                                   [In, MarshalAs(UnmanagedType.Struct)]
                                   Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndataavailable();

            void SetOndatasetcomplete(
                                     [In, MarshalAs(UnmanagedType.Struct)]
                                     Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndatasetcomplete();

            void SetOnfilterchange(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnfilterchange();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetChildren();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetAll();
        }

        [System.Runtime.InteropServices.ComImport(), Guid("3050F434-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElement2 {

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetScopeName();

            void SetCapture(
                           [In, MarshalAs(UnmanagedType.Bool)]
                           bool containerCapture);

            void ReleaseCapture();

            void SetOnlosecapture(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnlosecapture();

            [return: MarshalAs(UnmanagedType.BStr)]
            string ComponentFromPoint(
                                     [In, MarshalAs(UnmanagedType.I4)]
                                     int x,
                                     [In, MarshalAs(UnmanagedType.I4)]
                                     int y);

            void DoScroll(
                         [In, MarshalAs(UnmanagedType.Struct)]
                         Object component);

            void SetOnscroll(
                            [In, MarshalAs(UnmanagedType.Struct)]
                            Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnscroll();

            void SetOndrag(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndrag();

            void SetOndragend(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndragend();

            void SetOndragenter(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndragenter();

            void SetOndragover(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndragover();

            void SetOndragleave(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndragleave();

            void SetOndrop(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOndrop();

            void SetOnbeforecut(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforecut();

            void SetOncut(
                         [In, MarshalAs(UnmanagedType.Struct)]
                         Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOncut();

            void SetOnbeforecopy(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforecopy();

            void SetOncopy(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOncopy();

            void SetOnbeforepaste(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforepaste();

            void SetOnpaste(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnpaste();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLCurrentStyle GetCurrentStyle();

            void SetOnpropertychange(
                                    [In, MarshalAs(UnmanagedType.Struct)]
                                    Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnpropertychange();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLRectCollection GetClientRects();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLRect GetBoundingClientRect();

            void SetExpression(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string propname,
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string expression,
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string language);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetExpression(
                                [In, MarshalAs(UnmanagedType.BStr)]
                                Object propname);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool RemoveExpression(
                                 [In, MarshalAs(UnmanagedType.BStr)]
                                 string propname);

            void SetTabIndex(
                            [In, MarshalAs(UnmanagedType.I2)]
                            short p);

            [return: MarshalAs(UnmanagedType.I2)]
            short GetTabIndex();

            void Focus();

            void SetAccessKey(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetAccessKey();

            void SetOnblur(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnblur();

            void SetOnfocus(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnfocus();

            void SetOnresize(
                            [In, MarshalAs(UnmanagedType.Struct)]
                            Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnresize();

            void Blur();

            void AddFilter(
                          [In, MarshalAs(UnmanagedType.Interface)]
                          object pUnk);

            void RemoveFilter(
                             [In, MarshalAs(UnmanagedType.Interface)]
                             object pUnk);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetClientHeight();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetClientWidth();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetClientTop();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetClientLeft();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool AttachEvent(
                            [In, MarshalAs(UnmanagedType.BStr)]
                            string ev,
                            [In, MarshalAs(UnmanagedType.Interface)]
                            object pdisp);

            void DetachEvent(
                            [In, MarshalAs(UnmanagedType.BStr)]
                            string ev,
                            [In, MarshalAs(UnmanagedType.Interface)]
                            object pdisp);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetReadyState();

            void SetOnreadystatechange(
                                      [In, MarshalAs(UnmanagedType.Struct)]
                                      Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnreadystatechange();

            void SetOnrowsdelete(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnrowsdelete();

            void SetOnrowsinserted(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnrowsinserted();

            void SetOncellchange(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOncellchange();

            void SetDir(
                       [In, MarshalAs(UnmanagedType.BStr)]
                       string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDir();

            [return: MarshalAs(UnmanagedType.Interface)]
            object CreateControlRange();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetScrollHeight();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetScrollWidth();

            void SetScrollTop(
                             [In, MarshalAs(UnmanagedType.I4)]
                             int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetScrollTop();

            void SetScrollLeft(
                              [In, MarshalAs(UnmanagedType.I4)]
                              int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetScrollLeft();

            void ClearAttributes();

            void MergeAttributes(
                                [In, MarshalAs(UnmanagedType.Interface)]
                                IHTMLElement mergeThis);

            void SetOncontextmenu(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOncontextmenu();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement InsertAdjacentElement(
                                              [In, MarshalAs(UnmanagedType.BStr)]
                                              string where,
                                              [In, MarshalAs(UnmanagedType.Interface)]
                                              IHTMLElement insertedElement);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement ApplyElement(
                                     [In, MarshalAs(UnmanagedType.Interface)]
                                     IHTMLElement apply,
                                     [In, MarshalAs(UnmanagedType.BStr)]
                                     string where);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetAdjacentText(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string where);

            [return: MarshalAs(UnmanagedType.BStr)]
            string ReplaceAdjacentText(
                                      [In, MarshalAs(UnmanagedType.BStr)]
                                      string where,
                                      [In, MarshalAs(UnmanagedType.BStr)]
                                      string newText);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetCanHaveChildren();

            [return: MarshalAs(UnmanagedType.I4)]
            int AddBehavior(
                           [In, MarshalAs(UnmanagedType.BStr)]
                           string bstrUrl,
                           [In]
                           ref Object pvarFactory);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool RemoveBehavior(
                               [In, MarshalAs(UnmanagedType.I4)]
                               int cookie);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLStyle GetRuntimeStyle();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetBehaviorUrns();

            void SetTagUrn(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTagUrn();

            void SetOnbeforeeditfocus(
                                     [In, MarshalAs(UnmanagedType.Struct)]
                                     Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforeeditfocus();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetReadyStateValue();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElementCollection GetElementsByTagName(
                                                       [In, MarshalAs(UnmanagedType.BStr)]
                                                       string v);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLStyle GetBaseStyle();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLCurrentStyle GetBaseCurrentStyle();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLStyle GetBaseRuntimeStyle();

            void SetOnmousehover(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnmousehover();

            void SetOnkeydownpreview(
                                    [In, MarshalAs(UnmanagedType.Struct)]
                                    Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnkeydownpreview();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetBehavior(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string bstrName,
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string bstrUrn);
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F673-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElement3 {

            void MergeAttributes(
                                [In, MarshalAs(UnmanagedType.Interface)]
                                IHTMLElement mergeThis,
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object pvarFlags);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetIsMultiLine();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetCanHaveHTML();

            void SetOnLayoutComplete(
                                    [In, MarshalAs(UnmanagedType.Struct)]
                                    Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnLayoutComplete();

            void SetOnPage(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnPage();

            void SetInflateBlock(
                                [In, MarshalAs(UnmanagedType.Bool)]
                                bool inflate);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetInflateBlock();

            void SetOnBeforeDeactivate(
                                      [In, MarshalAs(UnmanagedType.Struct)]
                                      Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnBeforeDeactivate();

            void SetActive();

            void SetContentEditable(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetContentEditable();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetIsContentEditable();

            void SetHideFocus(
                             [In, MarshalAs(UnmanagedType.Bool)]
                             bool v);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetHideFocus();

            void SetDisabled(
                            [In, MarshalAs(UnmanagedType.Bool)]
                            bool v);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetDisabled();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetIsDisabled();

            void SetOnMove(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnMove();

            void SetOnControlSelect(
                                   [In, MarshalAs(UnmanagedType.Struct)]
                                   Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnControlSelect();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool FireEvent(
                          [In, MarshalAs(UnmanagedType.BStr)] 
                          string eventName,
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object eventObject);

            void SetOnResizeStart(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnResizeStart();

            void SetOnResizeEnd(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnResizeEnd();

            void SetOnMoveStart(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnMoveStart();

            void SetOnMoveEnd(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnMoveEnd();

            void SetOnMouseEnter(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnMouseEnter();

            void SetOnMouseLeave(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnMouseLeave();

            void SetOnActivate(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnActivate();

            void SetOnDeactivate(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnDeactivate();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool DragDrop();

            [return: MarshalAs(UnmanagedType.I4)]
            int GetGlyphMode();
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F1D8-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLBodyElement {

            void SetBackground(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackground();

            void SetBgProperties(
                                [In, MarshalAs(UnmanagedType.BStr)]
                                string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBgProperties();

            void SetLeftMargin(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLeftMargin();

            void SetTopMargin(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetTopMargin();

            void SetRightMargin(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetRightMargin();

            void SetBottomMargin(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBottomMargin();

            void SetNoWrap(
                          [In, MarshalAs(UnmanagedType.Bool)]
                          bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetNoWrap();

            void SetBgColor(
                           [In, MarshalAs(UnmanagedType.Struct)]
                           Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBgColor();

            void SetText(
                        [In, MarshalAs(UnmanagedType.Struct)]
                        Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetText();

            void SetLink(
                        [In, MarshalAs(UnmanagedType.Struct)]
                        Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLink();

            void SetVLink(
                         [In, MarshalAs(UnmanagedType.Struct)]
                         Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetVLink();

            void SetALink(
                         [In, MarshalAs(UnmanagedType.Struct)]
                         Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetALink();

            void SetOnload(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnload();

            void SetOnunload(
                            [In, MarshalAs(UnmanagedType.Struct)]
                            Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnunload();

            void SetScroll(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetScroll();

            void SetOnselect(
                            [In, MarshalAs(UnmanagedType.Struct)]
                            Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnselect();

            void SetOnbeforeunload(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetOnbeforeunload();

            [return: MarshalAs(UnmanagedType.Interface)]
            object CreateTextRange();
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F2E3-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLStyleSheet {

            void SetTitle(
                         [In, MarshalAs(UnmanagedType.BStr)] 
                         string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTitle();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLStyleSheet GetParentStyleSheet();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement GetOwningElement();

            void SetDisabled(
                            [In, MarshalAs(UnmanagedType.Bool)] 
                            bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetDisabled();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetReadOnly();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetImports();

            void SetHref(
                        [In, MarshalAs(UnmanagedType.BStr)] 
                        string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetHref();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetStyleSheetType();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetId();

            [return: MarshalAs(UnmanagedType.I4)]
            int AddImport(
                         [In, MarshalAs(UnmanagedType.BStr)] 
                         string bstrURL,
                         [In, MarshalAs(UnmanagedType.I4)] 
                         int lIndex);

            [return: MarshalAs(UnmanagedType.I4)]
            int AddRule(
                       [In, MarshalAs(UnmanagedType.BStr)] 
                       string bstrSelector,
                       [In, MarshalAs(UnmanagedType.BStr)] 
                       string bstrStyle,
                       [In, MarshalAs(UnmanagedType.I4)] 
                       int lIndex);

            void RemoveImport(
                             [In, MarshalAs(UnmanagedType.I4)] 
                             int lIndex);

            void RemoveRule(
                           [In, MarshalAs(UnmanagedType.I4)] 
                           int lIndex);

            void SetMedia(
                         [In, MarshalAs(UnmanagedType.BStr)] 
                         string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetMedia();

            void SetCssText(
                           [In, MarshalAs(UnmanagedType.BStr)] 
                           string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetCssText();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetRules();
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F25E-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLStyle {

            void SetFontFamily(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontFamily();

            void SetFontStyle(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontStyle();

            void SetFontObject(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontObject();

            void SetFontWeight(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontWeight();

            void SetFontSize(
                            [In, MarshalAs(UnmanagedType.Struct)]
                            Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetFontSize();

            void SetFont(
                        [In, MarshalAs(UnmanagedType.BStr)]
                        string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFont();

            void SetColor(
                         [In, MarshalAs(UnmanagedType.Struct)]
                         Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetColor();

            void SetBackground(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackground();

            void SetBackgroundColor(
                                   [In, MarshalAs(UnmanagedType.Struct)]
                                   Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBackgroundColor();

            void SetBackgroundImage(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundImage();

            void SetBackgroundRepeat(
                                    [In, MarshalAs(UnmanagedType.BStr)]
                                    string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundRepeat();

            void SetBackgroundAttachment(
                                        [In, MarshalAs(UnmanagedType.BStr)]
                                        string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundAttachment();

            void SetBackgroundPosition(
                                      [In, MarshalAs(UnmanagedType.BStr)]
                                      string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundPosition();

            void SetBackgroundPositionX(
                                       [In, MarshalAs(UnmanagedType.Struct)]
                                       Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBackgroundPositionX();

            void SetBackgroundPositionY(
                                       [In, MarshalAs(UnmanagedType.Struct)]
                                       Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBackgroundPositionY();

            void SetWordSpacing(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetWordSpacing();

            void SetLetterSpacing(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLetterSpacing();

            void SetTextDecoration(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTextDecoration();

            void SetTextDecorationNone(
                                      [In, MarshalAs(UnmanagedType.Bool)]
                                      bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetTextDecorationNone();

            void SetTextDecorationUnderline(
                                           [In, MarshalAs(UnmanagedType.Bool)]
                                           bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetTextDecorationUnderline();

            void SetTextDecorationOverline(
                                          [In, MarshalAs(UnmanagedType.Bool)]
                                          bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetTextDecorationOverline();

            void SetTextDecorationLineThrough(
                                             [In, MarshalAs(UnmanagedType.Bool)]
                                             bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetTextDecorationLineThrough();

            void SetTextDecorationBlink(
                                       [In, MarshalAs(UnmanagedType.Bool)]
                                       bool p);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetTextDecorationBlink();

            void SetVerticalAlign(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetVerticalAlign();

            void SetTextTransform(
                                 [In, MarshalAs(UnmanagedType.BStr)]
                                 string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTextTransform();

            void SetTextAlign(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTextAlign();

            void SetTextIndent(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetTextIndent();

            void SetLineHeight(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLineHeight();

            void SetMarginTop(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginTop();

            void SetMarginRight(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginRight();

            void SetMarginBottom(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginBottom();

            void SetMarginLeft(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginLeft();

            void SetMargin(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetMargin();

            void SetPaddingTop(
                              [In, MarshalAs(UnmanagedType.Struct)]
                              Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingTop();

            void SetPaddingRight(
                                [In, MarshalAs(UnmanagedType.Struct)]
                                Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingRight();

            void SetPaddingBottom(
                                 [In, MarshalAs(UnmanagedType.Struct)]
                                 Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingBottom();

            void SetPaddingLeft(
                               [In, MarshalAs(UnmanagedType.Struct)]
                               Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingLeft();

            void SetPadding(
                           [In, MarshalAs(UnmanagedType.BStr)]
                           string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPadding();

            void SetBorder(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorder();

            void SetBorderTop(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderTop();

            void SetBorderRight(
                               [In, MarshalAs(UnmanagedType.BStr)]
                               string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderRight();

            void SetBorderBottom(
                                [In, MarshalAs(UnmanagedType.BStr)]
                                string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderBottom();

            void SetBorderLeft(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderLeft();

            void SetBorderColor(
                               [In, MarshalAs(UnmanagedType.BStr)]
                               string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderColor();

            void SetBorderTopColor(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderTopColor();

            void SetBorderRightColor(
                                    [In, MarshalAs(UnmanagedType.Struct)]
                                    Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderRightColor();

            void SetBorderBottomColor(
                                     [In, MarshalAs(UnmanagedType.Struct)]
                                     Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderBottomColor();

            void SetBorderLeftColor(
                                   [In, MarshalAs(UnmanagedType.Struct)]
                                   Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderLeftColor();

            void SetBorderWidth(
                               [In, MarshalAs(UnmanagedType.BStr)]
                               string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderWidth();

            void SetBorderTopWidth(
                                  [In, MarshalAs(UnmanagedType.Struct)]
                                  Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderTopWidth();

            void SetBorderRightWidth(
                                    [In, MarshalAs(UnmanagedType.Struct)]
                                    Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderRightWidth();

            void SetBorderBottomWidth(
                                     [In, MarshalAs(UnmanagedType.Struct)]
                                     Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderBottomWidth();

            void SetBorderLeftWidth(
                                   [In, MarshalAs(UnmanagedType.Struct)]
                                   Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderLeftWidth();

            void SetBorderStyle(
                               [In, MarshalAs(UnmanagedType.BStr)]
                               string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderStyle();

            void SetBorderTopStyle(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderTopStyle();

            void SetBorderRightStyle(
                                    [In, MarshalAs(UnmanagedType.BStr)]
                                    string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderRightStyle();

            void SetBorderBottomStyle(
                                     [In, MarshalAs(UnmanagedType.BStr)]
                                     string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderBottomStyle();

            void SetBorderLeftStyle(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderLeftStyle();

            void SetWidth(
                         [In, MarshalAs(UnmanagedType.Struct)]
                         Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetWidth();

            void SetHeight(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetHeight();

            void SetStyleFloat(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetStyleFloat();

            void SetClear(
                         [In, MarshalAs(UnmanagedType.BStr)]
                         string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetClear();

            void SetDisplay(
                           [In, MarshalAs(UnmanagedType.BStr)]
                           string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDisplay();

            void SetVisibility(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetVisibility();

            void SetListStyleType(
                                 [In, MarshalAs(UnmanagedType.BStr)]
                                 string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStyleType();

            void SetListStylePosition(
                                     [In, MarshalAs(UnmanagedType.BStr)]
                                     string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStylePosition();

            void SetListStyleImage(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStyleImage();

            void SetListStyle(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStyle();

            void SetWhiteSpace(
                              [In, MarshalAs(UnmanagedType.BStr)]
                              string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetWhiteSpace();

            void SetTop(
                       [In, MarshalAs(UnmanagedType.Struct)]
                       Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetTop();

            void SetLeft(
                        [In, MarshalAs(UnmanagedType.Struct)]
                        Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLeft();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPosition();

            void SetZIndex(
                          [In, MarshalAs(UnmanagedType.Struct)]
                          Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetZIndex();

            void SetOverflow(
                            [In, MarshalAs(UnmanagedType.BStr)]
                            string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetOverflow();

            void SetPageBreakBefore(
                                   [In, MarshalAs(UnmanagedType.BStr)]
                                   string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPageBreakBefore();

            void SetPageBreakAfter(
                                  [In, MarshalAs(UnmanagedType.BStr)]
                                  string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPageBreakAfter();

            void SetCssText(
                           [In, MarshalAs(UnmanagedType.BStr)]
                           string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetCssText();

            void SetPixelTop(
                            [In, MarshalAs(UnmanagedType.I4)]
                            int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetPixelTop();

            void SetPixelLeft(
                             [In, MarshalAs(UnmanagedType.I4)]
                             int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetPixelLeft();

            void SetPixelWidth(
                              [In, MarshalAs(UnmanagedType.I4)]
                              int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetPixelWidth();

            void SetPixelHeight(
                               [In, MarshalAs(UnmanagedType.I4)]
                               int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetPixelHeight();

            void SetPosTop(
                          [In, MarshalAs(UnmanagedType.R4)]
                          float p);

            [return: MarshalAs(UnmanagedType.R4)]
            float GetPosTop();

            void SetPosLeft(
                           [In, MarshalAs(UnmanagedType.R4)]
                           float p);

            [return: MarshalAs(UnmanagedType.R4)]
            float GetPosLeft();

            void SetPosWidth(
                            [In, MarshalAs(UnmanagedType.R4)]
                            float p);

            [return: MarshalAs(UnmanagedType.R4)]
            float GetPosWidth();

            void SetPosHeight(
                             [In, MarshalAs(UnmanagedType.R4)]
                             float p);

            [return: MarshalAs(UnmanagedType.R4)]
            float GetPosHeight();

            void SetCursor(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetCursor();

            void SetClip(
                        [In, MarshalAs(UnmanagedType.BStr)]
                        string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetClip();

            void SetFilter(
                          [In, MarshalAs(UnmanagedType.BStr)]
                          string p);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFilter();

            void SetAttribute(
                             [In, MarshalAs(UnmanagedType.BStr)]
                             string strAttributeName,
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object AttributeValue,
                             [In, MarshalAs(UnmanagedType.I4)]
                             int lFlags);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetAttribute(
                               [In, MarshalAs(UnmanagedType.BStr)]
                               string strAttributeName,
                               [In, MarshalAs(UnmanagedType.I4)]
                               int lFlags);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool RemoveAttribute(
                                [In, MarshalAs(UnmanagedType.BStr)]
                                string strAttributeName,
                                [In, MarshalAs(UnmanagedType.I4)]
                                int lFlags);

        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F3DB-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLCurrentStyle {

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPosition();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetStyleFloat();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetColor();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBackgroundColor();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontFamily();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontStyle();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetFontObject();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetFontWeight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetFontSize();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundImage();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBackgroundPositionX();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBackgroundPositionY();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundRepeat();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderLeftColor();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderTopColor();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderRightColor();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderBottomColor();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderTopStyle();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderRightStyle();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderBottomStyle();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderLeftStyle();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderTopWidth();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderRightWidth();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderBottomWidth();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBorderLeftWidth();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLeft();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetTop();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetWidth();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetHeight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingLeft();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingTop();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingRight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetPaddingBottom();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTextAlign();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTextDecoration();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDisplay();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetVisibility();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetZIndex();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLetterSpacing();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetLineHeight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetTextIndent();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetVerticalAlign();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBackgroundAttachment();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginTop();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginRight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginBottom();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetMarginLeft();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetClear();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStyleType();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStylePosition();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetListStyleImage();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetClipTop();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetClipRight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetClipBottom();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetClipLeft();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetOverflow();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPageBreakBefore();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPageBreakAfter();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetCursor();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetTableLayout();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBorderCollapse();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetDirection();

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetBehavior();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetAttribute(
                               [In, MarshalAs(UnmanagedType.BStr)]
                               string strAttributeName,
                               [In, MarshalAs(UnmanagedType.I4)]
                               int lFlags);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetUnicodeBidi();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetRight();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetBottom();

        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F21F-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElementCollection {

            [return: MarshalAs(UnmanagedType.BStr)]
            string toString();

            void SetLength(
                          [In, MarshalAs(UnmanagedType.I4)] 
                          int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetLength();

            [return: MarshalAs(UnmanagedType.Interface)]
            object Get_newEnum();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement Item(
                       [In, MarshalAs(UnmanagedType.Struct)] 
                       Object name,
                       [In, MarshalAs(UnmanagedType.Struct)] 
                       Object index);

            [return: MarshalAs(UnmanagedType.Interface)]
            object Tags(
                       [In, MarshalAs(UnmanagedType.Struct)] 
                       Object tagName);
        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F4A3-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLRect {

            void SetLeft(
                        [In, MarshalAs(UnmanagedType.I4)] 
                        int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetLeft();

            void SetTop(
                       [In, MarshalAs(UnmanagedType.I4)] 
                       int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetTop();

            void SetRight(
                         [In, MarshalAs(UnmanagedType.I4)] 
                         int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetRight();

            void SetBottom(
                          [In, MarshalAs(UnmanagedType.I4)] 
                          int p);

            [return: MarshalAs(UnmanagedType.I4)]
            int GetBottom();

        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F4A4-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLRectCollection {

            [return: MarshalAs(UnmanagedType.I4)]
            int GetLength();

            [return: MarshalAs(UnmanagedType.Interface)]
            object Get_newEnum();

            [return: MarshalAs(UnmanagedType.Struct)]
            Object Item(
                       [In]
                       ref Object pvarIndex);

        }

        [System.Runtime.InteropServices.ComVisible(true), System.Runtime.InteropServices.ComImport(), Guid("3050F5DA-98B5-11CF-BB82-00AA00BDCE0B"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDOMNode {

            [return: MarshalAs(UnmanagedType.I4)]
            int GetNodeType();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode GetParentNode();

            [return: MarshalAs(UnmanagedType.Bool)]
            bool HasChildNodes();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetChildNodes();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetAttributes();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode InsertBefore(
                                     [In, MarshalAs(UnmanagedType.Interface)]
                                     IHTMLDOMNode newChild,
                                     [In, MarshalAs(UnmanagedType.Struct)]
                                     Object refChild);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode RemoveChild(
                                    [In, MarshalAs(UnmanagedType.Interface)]
                                    IHTMLDOMNode oldChild);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode ReplaceChild(
                                     [In, MarshalAs(UnmanagedType.Interface)]
                                     IHTMLDOMNode newChild,
                                     [In, MarshalAs(UnmanagedType.Interface)]
                                     IHTMLDOMNode oldChild);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode CloneNode(
                                  [In, MarshalAs(UnmanagedType.Bool)]
                                  bool fDeep);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode RemoveNode(
                                   [In, MarshalAs(UnmanagedType.Bool)]
                                   bool fDeep);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode SwapNode(
                                 [In, MarshalAs(UnmanagedType.Interface)]
                                 IHTMLDOMNode otherNode);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode ReplaceNode(
                                    [In, MarshalAs(UnmanagedType.Interface)]
                                    IHTMLDOMNode replacement);

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode AppendChild(
                                    [In, MarshalAs(UnmanagedType.Interface)]
                                    IHTMLDOMNode newChild);

            [return: MarshalAs(UnmanagedType.BStr)]
            string GetNodeName();

            void SetNodeValue(
                             [In, MarshalAs(UnmanagedType.Struct)]
                             Object p);

            [return: MarshalAs(UnmanagedType.Struct)]
            Object GetNodeValue();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode GetFirstChild();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode GetLastChild();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode GetPreviousSibling();

            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLDOMNode GetNextSibling();
        }
#endif
        public static readonly int WM_MOUSEENTER = Util.RegisterWindowMessage("WinFormsMouseEnter");
        public static readonly int HDN_ENDTRACK = HDN_ENDTRACKW;

        public const int
            DT_CALCRECT = 0x00000400,
            WM_CAPTURECHANGED = 0x0215,
            WM_PARENTNOTIFY = 0x0210,
            WM_CREATE = 0x0001,
            WM_SETREDRAW = 0x000B,
            WM_NCACTIVATE = 0x0086,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_SHOWWINDOW = 0x0018,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_POPUP = unchecked((int)0x80000000),
            WS_BORDER = 0x00800000,
            CS_DROPSHADOW = 0x00020000,
            CS_DBLCLKS = 0x0008,
            NOTSRCCOPY = 0x00330008,
            SRCCOPY = 0x00CC0020,
            LVM_SETCOLUMNWIDTH = 0x1000 + 30,
            LVM_GETHEADER = 0x1000 + 31,
            LVM_CREATEDRAGIMAGE = 0x1000 + 33,
            LVM_GETVIEWRECT = 0x1000 + 34,
            LVM_GETTEXTCOLOR = 0x1000 + 35,
            LVM_SETTEXTCOLOR = 0x1000 + 36,
            LVM_GETTEXTBKCOLOR = 0x1000 + 37,
            LVM_SETTEXTBKCOLOR = 0x1000 + 38,
            LVM_GETTOPINDEX = 0x1000 + 39,
            LVM_GETCOUNTPERPAGE = 0x1000 + 40,
            LVM_GETORIGIN = 0x1000 + 41,
            LVM_UPDATE = 0x1000 + 42,
            LVM_SETITEMSTATE = 0x1000 + 43,
            LVM_GETITEMSTATE = 0x1000 + 44,
            LVM_GETITEMTEXTA = 0x1000 + 45,
            LVM_GETITEMTEXTW = 0x1000 + 115,
            LVM_SETITEMTEXTA = 0x1000 + 46,
            LVM_SETITEMTEXTW = 0x1000 + 116,
            LVSICF_NOINVALIDATEALL = 0x00000001,
            LVSICF_NOSCROLL = 0x00000002,
            LVM_SETITEMCOUNT = 0x1000 + 47,
            LVM_SORTITEMS = 0x1000 + 48,
            LVM_SETITEMPOSITION32 = 0x1000 + 49,
            LVM_GETSELECTEDCOUNT = 0x1000 + 50,
            LVM_GETITEMSPACING = 0x1000 + 51,
            LVM_GETISEARCHSTRINGA = 0x1000 + 52,
            LVM_GETISEARCHSTRINGW = 0x1000 + 117,
            LVM_SETICONSPACING = 0x1000 + 53,
            LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1000 + 54,
            LVM_GETEXTENDEDLISTVIEWSTYLE = 0x1000 + 55,
            LVS_EX_GRIDLINES = 0x00000001,
            HDM_HITTEST = 0x1200 + 6,
            HDM_GETITEMRECT = 0x1200 + 7,
            HDM_SETIMAGELIST = 0x1200 + 8,
            HDM_GETIMAGELIST = 0x1200 + 9,
            HDM_ORDERTOINDEX = 0x1200 + 15,
            HDM_CREATEDRAGIMAGE = 0x1200 + 16,
            HDM_GETORDERARRAY = 0x1200 + 17,
            HDM_SETORDERARRAY = 0x1200 + 18,
            HDM_SETHOTDIVIDER = 0x1200 + 19,
            HDN_ITEMCHANGINGA = 0 - 300 - 0,
            HDN_ITEMCHANGINGW = 0 - 300 - 20,
            HDN_ITEMCHANGEDA = 0 - 300 - 1,
            HDN_ITEMCHANGEDW = 0 - 300 - 21,
            HDN_ITEMCLICKA = 0 - 300 - 2,
            HDN_ITEMCLICKW = 0 - 300 - 22,
            HDN_ITEMDBLCLICKA = 0 - 300 - 3,
            HDN_ITEMDBLCLICKW = 0 - 300 - 23,
            HDN_DIVIDERDBLCLICKA = 0 - 300 - 5,
            HDN_DIVIDERDBLCLICKW = 0 - 300 - 25,
            HDN_BEGINTRACKA = 0 - 300 - 6,
            HDN_BEGINTRACKW = 0 - 300 - 26,
            HDN_ENDTRACKA = 0 - 300 - 7,
            HDN_ENDTRACKW = 0 - 300 - 27,
            HDN_TRACKA = 0 - 300 - 8,
            HDN_TRACKW = 0 - 300 - 28,
            HDN_GETDISPINFOA = 0 - 300 - 9,
            HDN_GETDISPINFOW = 0 - 300 - 29,
            HDN_BEGINDRAG = 0 - 300 - 10,
            HDN_ENDDRAG = 0 - 300 - 11,
            HC_ACTION = 0,
            HIST_BACK = 0,
            HHT_ONHEADER = 0x0002,
            HHT_ONDIVIDER = 0x0004,
            HHT_ONDIVOPEN = 0x0008,
            HHT_ABOVE = 0x0100,
            HHT_BELOW = 0x0200,
            HHT_TORIGHT = 0x0400,
            HHT_TOLEFT = 0x0800,
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2,
            CWP_SKIPINVISIBLE = 0x0001,
            RDW_FRAME = 0x0400,
            WM_KILLFOCUS = 0x0008,
            WM_STYLECHANGED = 0x007D,
            TVM_GETITEMRECT = 0x1100 + 4,
            TVM_GETCOUNT = 0x1100 + 5,
            TVM_GETINDENT = 0x1100 + 6,
            TVM_SETINDENT = 0x1100 + 7,
            TVM_GETIMAGELIST = 0x1100 + 8,
            TVSIL_NORMAL = 0,
            TVSIL_STATE = 2,
            TVM_SETIMAGELIST = 0x1100 + 9,
            TVM_GETNEXTITEM = 0x1100 + 10,
            TVGN_ROOT = 0x0000,
            TV_FIRST = 0x1100,
            TVM_SETEXTENDEDSTYLE = TV_FIRST + 44,
            TVM_GETEXTENDEDSTYLE = TV_FIRST + 45,
            TVS_EX_FADEINOUTEXPANDOS = 0x0040,
            TVS_EX_DOUBLEBUFFER = 0x0004,
            LVS_EX_DOUBLEBUFFER = 0x00010000,
            TVHT_ONITEMICON = 0x0002,
            TVHT_ONITEMLABEL = 0x0004,
            TVHT_ONITEMINDENT = 0x0008,
            TVHT_ONITEMBUTTON = 0x0010,
            TVHT_ONITEMRIGHT = 0x0020,
            TVHT_ONITEMSTATEICON = 0x0040,
            TVHT_ABOVE = 0x0100,
            TVHT_BELOW = 0x0200,
            TVHT_TORIGHT = 0x0400,
            TVHT_TOLEFT = 0x0800,
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_MAX = 5,
            GWL_HWNDPARENT = -8,
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3,
            SB_LINEUP = 0,
            SB_LINELEFT = 0,
            SB_LINEDOWN = 1,
            SB_LINERIGHT = 1,
            SB_PAGEUP = 2,
            SB_PAGELEFT = 2,
            SB_PAGEDOWN = 3,
            SB_PAGERIGHT = 3,
            SB_THUMBPOSITION = 4,
            SB_THUMBTRACK = 5,
            SB_TOP = 6,
            SB_LEFT = 6,
            SB_BOTTOM = 7,
            SB_RIGHT = 7,
            SB_ENDSCROLL = 8,
            MK_LBUTTON = 0x0001,
            TVM_HITTEST = 0x1100 + 17,
            MK_RBUTTON = 0x0002,
            MK_SHIFT = 0x0004,
            MK_CONTROL = 0x0008,
            MK_MBUTTON = 0x0010,
            MK_XBUTTON1 = 0x0020,
            MK_XBUTTON2 = 0x0040,
            LB_ADDSTRING = 0x0180,
            LB_INSERTSTRING = 0x0181,
            LB_DELETESTRING = 0x0182,
            LB_SELITEMRANGEEX = 0x0183,
            LB_RESETCONTENT = 0x0184,
            LB_SETSEL = 0x0185,
            LB_SETCURSEL = 0x0186,
            LB_GETSEL = 0x0187,
            LB_GETCURSEL = 0x0188,
            LB_GETTEXT = 0x0189,
            LB_GETTEXTLEN = 0x018A,
            LB_GETCOUNT = 0x018B,
            LB_SELECTSTRING = 0x018C,
            LB_DIR = 0x018D,
            LB_GETTOPINDEX = 0x018E,
            LB_FINDSTRING = 0x018F,
            LB_GETSELCOUNT = 0x0190,
            LB_GETSELITEMS = 0x0191,
            LB_SETTABSTOPS = 0x0192,
            LB_GETHORIZONTALEXTENT = 0x0193,
            LB_SETHORIZONTALEXTENT = 0x0194,
            LB_SETCOLUMNWIDTH = 0x0195,
            LB_ADDFILE = 0x0196,
            LB_SETTOPINDEX = 0x0197,
            LB_GETITEMRECT = 0x0198,
            LB_GETITEMDATA = 0x0199,
            LB_SETITEMDATA = 0x019A,
            LB_SELITEMRANGE = 0x019B,
            LB_SETANCHORINDEX = 0x019C,
            LB_GETANCHORINDEX = 0x019D,
            LB_SETCARETINDEX = 0x019E,
            LB_GETCARETINDEX = 0x019F,
            LB_SETITEMHEIGHT = 0x01A0,
            LB_GETITEMHEIGHT = 0x01A1,
            LB_FINDSTRINGEXACT = 0x01A2,
            LB_SETLOCALE = 0x01A5,
            LB_GETLOCALE = 0x01A6,
            LB_SETCOUNT = 0x01A7,
            LB_INITSTORAGE = 0x01A8,
            LB_ITEMFROMPOINT = 0x01A9,
            LB_MSGMAX = 0x01B0,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = 4,
            PRF_NONCLIENT = 0x00000002,
            PRF_CLIENT = 0x00000004,
            PRF_ERASEBKGND = 0x00000008,
            PRF_CHILDREN = 0x00000010,
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DRAWFRAME = 0x0020,
            SWP_NOREPOSITION = 0x0200,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000,
            WA_INACTIVE = 0,
            WA_ACTIVE = 1,
            WH_MOUSE = 7,
            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x10E,
            WM_IME_COMPOSITION = 0x010F,
            WM_ACTIVATE = 0x0006,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN = 0x00AB,
            WM_NCXBUTTONUP = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_MOUSEFIRST = 0x0200,
            WM_MOUSEACTIVATE = 0x0021,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_NCMOUSEHOVER = 0x02A0,
            WM_NCMOUSELEAVE = 0x02A2,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSELAST = 0x020A,
            WM_NCHITTEST = 0x0084,
            WM_SETCURSOR = 0x0020,
            WM_GETOBJECT = 0x003D,
            WM_CANCELMODE = 0x001F,
            WM_SETFOCUS = 0x0007,
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_KEYLAST = 0x0108,
            WM_CONTEXTMENU = 0x007B,
            WM_PAINT = 0x000F,
            WM_PRINTCLIENT = 0x0318,
            WM_NCPAINT = 0x0085,
            WM_SIZE = 0x0005,
            WM_TIMER = 0x0113,
            WM_PRINT = 0x0317;

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetCursor();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetCursorPos([In] [Out] POINT pt);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [In] [Out] HDHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [In] [Out] TV_HITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern short GetKeyState(int keyCode);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In] [Out] ref RECT rect, int cPoints);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In] [Out] POINT pt, int cPoints);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool ValidateRect(IntPtr hwnd, IntPtr prect);

        [DllImport(ExternDll.Gdi32, ExactSpelling = true, EntryPoint = "CreateRectRgn", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        private static extern IntPtr IntCreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetUpdateRect(IntPtr hwnd, [In] [Out] ref RECT rc, bool fErase);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetUpdateRgn(IntPtr hwnd, IntPtr hrgn, bool fErase);

        [DllImport(ExternDll.Gdi32, ExactSpelling = true, EntryPoint = "DeleteObject", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool ExternalDeleteObject(HandleRef hObject);

        [DllImport(ExternDll.Gdi32, ExactSpelling = true, EntryPoint = "DeleteObject", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool IntDeleteObject(IntPtr hObject);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool TranslateMessage([In] [Out] ref MSG msg);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int DispatchMessage([In] ref MSG msg);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool GetWindowRect(IntPtr hWnd, [In] [Out] ref RECT rect);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int RevokeDragDrop(IntPtr hwnd);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, int x, int y, int uFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api")]
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetFocus();

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

            public static int MAKELPARAM(int low, int high)
            {
                return (high << 16) | (low & 0xffff);
            }

            public static int HIWORD(int n)
            {
                return (n >> 16) & 0xffff;
            }

            public static int LOWORD(int n)
            {
                return n & 0xffff;
            }

            public static int SignedHIWORD(int n)
            {
                int i = (short)((n >> 16) & 0xffff);

                i = i << 16;
                i = i >> 16;

                return i;
            }

            public static int SignedLOWORD(int n)
            {
                int i = (short)(n & 0xFFFF);

                i = i << 16;
                i = i >> 16;

                return i;
            }

            public static int SignedHIWORD(IntPtr n)
            {
                return SignedHIWORD(unchecked((int)(long)n));
            }

            public static int SignedLOWORD(IntPtr n)
            {
                return SignedLOWORD(unchecked((int)(long)n));
            }

            //Uncalled private code - reported by FxCop
            //[DllImport(ExternDll.Kernel32, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            //private static extern int lstrlen(String s);

            [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
            [ResourceExposure(ResourceScope.None)]
            internal static extern int RegisterWindowMessage(string msg);
        }

        internal class ActiveX
        {
            public const int OCM__BASE = 0x2000;
            public const int DISPID_VALUE = 0x0;
            public const int DISPID_UNKNOWN = unchecked((int)0xFFFFFFFF);
            public const int DISPID_AUTOSIZE = unchecked((int)0xFFFFFE0C);
            public const int DISPID_BACKCOLOR = unchecked((int)0xFFFFFE0B);
            public const int DISPID_BACKSTYLE = unchecked((int)0xFFFFFE0A);
            public const int DISPID_BORDERCOLOR = unchecked((int)0xFFFFFE09);
            public const int DISPID_BORDERSTYLE = unchecked((int)0xFFFFFE08);
            public const int DISPID_BORDERWIDTH = unchecked((int)0xFFFFFE07);
            public const int DISPID_DRAWMODE = unchecked((int)0xFFFFFE05);
            public const int DISPID_DRAWSTYLE = unchecked((int)0xFFFFFE04);
            public const int DISPID_DRAWWIDTH = unchecked((int)0xFFFFFE03);
            public const int DISPID_FILLCOLOR = unchecked((int)0xFFFFFE02);
            public const int DISPID_FILLSTYLE = unchecked((int)0xFFFFFE01);
            public const int DISPID_FONT = unchecked((int)0xFFFFFE00);
            public const int DISPID_FORECOLOR = unchecked((int)0xFFFFFDFF);
            public const int DISPID_ENABLED = unchecked((int)0xFFFFFDFE);
            public const int DISPID_HWND = unchecked((int)0xFFFFFDFD);
            public const int DISPID_TABSTOP = unchecked((int)0xFFFFFDFC);
            public const int DISPID_TEXT = unchecked((int)0xFFFFFDFB);
            public const int DISPID_CAPTION = unchecked((int)0xFFFFFDFA);
            public const int DISPID_BORDERVISIBLE = unchecked((int)0xFFFFFDF9);
            public const int DISPID_APPEARANCE = unchecked((int)0xFFFFFDF8);
            public const int DISPID_MOUSEPOINTER = unchecked((int)0xFFFFFDF7);
            public const int DISPID_MOUSEICON = unchecked((int)0xFFFFFDF6);
            public const int DISPID_PICTURE = unchecked((int)0xFFFFFDF5);
            public const int DISPID_VALID = unchecked((int)0xFFFFFDF4);
            public const int DISPID_READYSTATE = unchecked((int)0xFFFFFDF3);
            public const int DISPID_REFRESH = unchecked((int)0xFFFFFDDA);
            public const int DISPID_DOCLICK = unchecked((int)0xFFFFFDD9);
            public const int DISPID_ABOUTBOX = unchecked((int)0xFFFFFDD8);
            public const int DISPID_CLICK = unchecked((int)0xFFFFFDA8);
            public const int DISPID_DBLCLICK = unchecked((int)0xFFFFFDA7);
            public const int DISPID_KEYDOWN = unchecked((int)0xFFFFFDA6);
            public const int DISPID_KEYPRESS = unchecked((int)0xFFFFFDA5);
            public const int DISPID_KEYUP = unchecked((int)0xFFFFFDA4);
            public const int DISPID_MOUSEDOWN = unchecked((int)0xFFFFFDA3);
            public const int DISPID_MOUSEMOVE = unchecked((int)0xFFFFFDA2);
            public const int DISPID_MOUSEUP = unchecked((int)0xFFFFFDA1);
            public const int DISPID_ERROREVENT = unchecked((int)0xFFFFFDA0);
            public const int DISPID_RIGHTTOLEFT = unchecked((int)0xFFFFFD9D);
            public const int DISPID_READYSTATECHANGE = unchecked((int)0xFFFFFD9F);
            public const int DISPID_AMBIENT_BACKCOLOR = unchecked((int)0xFFFFFD43);
            public const int DISPID_AMBIENT_DISPLAYNAME = unchecked((int)0xFFFFFD42);
            public const int DISPID_AMBIENT_FONT = unchecked((int)0xFFFFFD41);
            public const int DISPID_AMBIENT_FORECOLOR = unchecked((int)0xFFFFFD40);
            public const int DISPID_AMBIENT_LOCALEID = unchecked((int)0xFFFFFD3F);
            public const int DISPID_AMBIENT_MESSAGEREFLECT = unchecked((int)0xFFFFFD3E);
            public const int DISPID_AMBIENT_SCALEUNITS = unchecked((int)0xFFFFFD3D);
            public const int DISPID_AMBIENT_TEXTALIGN = unchecked((int)0xFFFFFD3C);
            public const int DISPID_AMBIENT_USERMODE = unchecked((int)0xFFFFFD3B);
            public const int DISPID_AMBIENT_UIDEAD = unchecked((int)0xFFFFFD3A);
            public const int DISPID_AMBIENT_SHOWGRABHANDLES = unchecked((int)0xFFFFFD39);
            public const int DISPID_AMBIENT_SHOWHATCHING = unchecked((int)0xFFFFFD38);
            public const int DISPID_AMBIENT_DISPLAYASDEFAULT = unchecked((int)0xFFFFFD37);
            public const int DISPID_AMBIENT_SUPPORTSMNEMONICS = unchecked((int)0xFFFFFD36);
            public const int DISPID_AMBIENT_AUTOCLIP = unchecked((int)0xFFFFFD35);
            public const int DISPID_AMBIENT_APPEARANCE = unchecked((int)0xFFFFFD34);
            public const int DISPID_AMBIENT_PALETTE = unchecked((int)0xFFFFFD2A);
            public const int DISPID_AMBIENT_TRANSFERPRIORITY = unchecked((int)0xFFFFFD28);
            public const int DISPID_Name = unchecked((int)0xFFFFFCE0);
            public const int DISPID_Delete = unchecked((int)0xFFFFFCDF);
            public const int DISPID_Object = unchecked((int)0xFFFFFCDE);
            public const int DISPID_Parent = unchecked((int)0xFFFFFCDD);
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

        public static bool Failed(int hr)
        {
            return hr < 0;
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

        [StructLayout(LayoutKind.Sequential)]
        public class CHARRANGE
        {
            public readonly int cpMax = 0;
            public readonly int cpMin = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class STATSTG
        {
            [MarshalAs(UnmanagedType.I8)] public readonly long atime = 0;

            [MarshalAs(UnmanagedType.I8)] public readonly long cbSize = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b0 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b1 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b2 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b3 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b4 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b5 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b6 = 0;

            [MarshalAs(UnmanagedType.U1)] public readonly byte clsid_b7 = 0;

            public readonly int clsid_data1 = 0;

            [MarshalAs(UnmanagedType.I2)] public readonly short clsid_data2 = 0;

            [MarshalAs(UnmanagedType.I2)] public readonly short clsid_data3 = 0;

            [MarshalAs(UnmanagedType.I8)] public readonly long ctime = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int grfLocksSupported = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int grfMode = 0;

            [MarshalAs(UnmanagedType.I4)] public readonly int grfStateBits = 0;

            [MarshalAs(UnmanagedType.I8)] public readonly long mtime = 0;

            [MarshalAs(UnmanagedType.LPWStr)] public readonly string pwcsName = null;

            [MarshalAs(UnmanagedType.I4)] public readonly int reserved = 0;

            public readonly int type = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class FILETIME
        {
            public readonly uint dwHighDateTime = 0;
            public readonly uint dwLowDateTime = 0;
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
        public const string uuid_IEnumVariant = "{00020404-0000-0000-C000-000000000046}";

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern ITypeLib LoadRegTypeLib(ref Guid clsid, short majorVersion, short minorVersion, int lcid);

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern ITypeLib LoadTypeLib([In] [MarshalAs(UnmanagedType.LPWStr)] string typelib);

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.BStr)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern string QueryPathOfRegTypeLib(ref Guid guid, short majorVersion, short minorVersion,
            int lcid);

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
            GWL_STYLE = -16,
            WS_EX_LAYOUTRTL = 0x00400000;

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
        public sealed class CommonHandles
        {
            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Accelerator"]/*' />
            /// <devdoc>
            ///     Handle type for accelerator tables.
            /// </devdoc>
            public static readonly int Accelerator = System.Internal.HandleCollector.RegisterType("Accelerator", 80, 50);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Cursor"]/*' />
            /// <devdoc>
            ///     handle type for cursors.
            /// </devdoc>
            public static readonly int Cursor = System.Internal.HandleCollector.RegisterType("Cursor", 20, 500);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.EMF"]/*' />
            /// <devdoc>
            ///     Handle type for enhanced metafiles.
            /// </devdoc>
            public static readonly int EMF = System.Internal.HandleCollector.RegisterType("EnhancedMetaFile", 20, 500);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Find"]/*' />
            /// <devdoc>
            ///     Handle type for file find handles.
            /// </devdoc>
            public static readonly int Find = System.Internal.HandleCollector.RegisterType("Find", 0, 1000);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.GDI"]/*' />
            /// <devdoc>
            ///     Handle type for GDI objects.
            /// </devdoc>
            public static readonly int GDI = System.Internal.HandleCollector.RegisterType("GDI", 90, 50);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.HDC"]/*' />
            /// <devdoc>
            ///     Handle type for HDC's that count against the Win98 limit of five DC's.  HDC's
            ///     which are not scarce, such as HDC's for bitmaps, are counted as GDIHANDLE's.
            /// </devdoc>
            public static readonly int HDC = System.Internal.HandleCollector.RegisterType("HDC", 100, 2); // wait for 2 dc's before collecting

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Icon"]/*' />
            /// <devdoc>
            ///     Handle type for icons.
            /// </devdoc>
            public static readonly int Icon = System.Internal.HandleCollector.RegisterType("Icon", 20, 500);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Kernel"]/*' />
            /// <devdoc>
            ///     Handle type for kernel objects.
            /// </devdoc>
            public static readonly int Kernel = System.Internal.HandleCollector.RegisterType("Kernel", 0, 1000);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Menu"]/*' />
            /// <devdoc>
            ///     Handle type for files.
            /// </devdoc>
            public static readonly int Menu = System.Internal.HandleCollector.RegisterType("Menu", 30, 1000);

            /// <include file='doc\NativeMethods.uex' path='docs/doc[@for="NativeMethods.CommonHandles.Window"]/*' />
            /// <devdoc>
            ///     Handle type for windows.
            /// </devdoc>
            public static readonly int Window = System.Internal.HandleCollector.RegisterType("Window", 5, 1000);
        }

#if LEGACY
        [StructLayout(LayoutKind.Sequential)]
        public class NONCLIENTMETRICS
        {
            public int cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
            public int iBorderWidth = 0;
            public int iScrollWidth = 0;
            public int iScrollHeight = 0;
            public int iCaptionWidth = 0;
            public int iCaptionHeight = 0;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfCaptionFont = null;
            public int iSmCaptionWidth = 0;
            public int iSmCaptionHeight = 0;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfSmCaptionFont = null;
            public int iMenuWidth = 0;
            public int iMenuHeight = 0;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfMenuFont = null;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfStatusFont = null;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfMessageFont = null;
        }
#endif

        public const int SPI_GETNONCLIENTMETRICS = 41;

        //scoberry Nov 1, 2004: Removed DispatchMessageA, DispatchMessageW, GetClientRect, PeekMessageA
        //PeekMessageW, PostMessage, SendMessage(IntPtr, int, IntPtr, ref RECT), SendMessage(IntPtr, int, ref short, ref short)
        //SendMessage(IntPtr, int, bool, IntPtr), SendMessage(IntPtr, int, IntPtr, ListViewCompareCallback),
        //SendMessageW, SendMessageA, ValidateRect(IntPtr, ref RECT), ValidateRgn(IntPtr, IntPtr)
        //COMRECT.FromXYWH, RECT.FromXYWH
    }
}
