// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

// Investigate removing this if the duplicate code in OleDragDropHandler.cs is removed
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "System.Design.UnsafeNativeMethods.GetStockObject(System.Int32):System.IntPtr")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "System.Design.UnsafeNativeMethods.IntReleaseDC(System.Runtime.InteropServices.HandleRef,System.Runtime.InteropServices.HandleRef):System.Int32")]

namespace System.Windows.Forms.Design
{
    [SuppressUnmanagedCodeSecurity]
    internal class UnsafeNativeMethods
    {
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int ClientToScreen(HandleRef hWnd, [In] [Out] NativeMethods.POINT pt);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ScreenToClient(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetActiveWindow();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int GetMessageTime();

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SetActiveWindow(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern void NotifyWinEvent(int winEvent, HandleRef hwnd, int objType, int objID);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetFocus();

        //Can't use IsMdiChild: IsChild's not being used for MDI-specific or Form-specific stuff.
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api")]
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool IsChild(HandleRef hWndParent, HandleRef hwnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int MsgWaitForMultipleObjectsEx(int nCount, IntPtr pHandles, int dwMilliseconds,
            int dwWakeMask, int dwFlags);

        [DllImport(ExternDll.Ole32)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int ReadClassStg(HandleRef pStg, [In] [Out] ref Guid pclsid);

        // Investigate removing this if the duplicate code in OleDragDropHandler.cs is removed
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        public static extern IntPtr GetStockObject(int nIndex);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, IntPtr pAcc);

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "BeginPaint", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr BeginPaint(IntPtr hWnd, [In] [Out] ref PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "EndPaint", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "GetDC", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        private static extern IntPtr IntGetDC(HandleRef hWnd);

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static IntPtr GetDC(HandleRef hWnd)
        {
            return System.Internal.HandleCollector.Add(IntGetDC(hWnd), NativeMethods.CommonHandles.HDC);
        }

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "ReleaseDC", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern int IntReleaseDC(HandleRef hWnd, HandleRef hDC);

        public static int ReleaseDC(HandleRef hWnd, HandleRef hDC)
        {
            System.Internal.HandleCollector.Remove((IntPtr)hDC, NativeMethods.CommonHandles.HDC);
            return IntReleaseDC(hWnd, hDC);
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int GetDeviceCaps(HandleRef hDC, int nIndex);

        [DllImport(ExternDll.Shell32)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern IntPtr ExtractIcon(HandleRef hMod, string exeName, int index);

        [DllImport(ExternDll.User32)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool DestroyIcon(HandleRef hIcon);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SetWindowsHookEx(int hookid, HookProc pfnhook, HandleRef hinst, int threadid);

        //GetWindowLong won't work correctly for 64-bit: we should use GetWindowLongPtr instead.  On
        //32-bit, GetWindowLongPtr is just #defined as GetWindowLong.  GetWindowLong really should 
        //take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        //it'll be OK.
        public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
        {
            if (IntPtr.Size == 4) return GetWindowLong32(hWnd, nIndex);
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetWindowLong32(HandleRef hWnd, int nIndex);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, int nIndex);

        //This is called, and needs to be owned (unmanaged SetWindowLong) vs. parented (managed 
        //Control.Parent).
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
        //SetWindowLong won't work correctly for 64-bit: we should use SetWindowLongPtr instead.  On
        //32-bit, SetWindowLongPtr is just #defined as SetWindowLong.  SetWindowLong really should 
        //take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        //it'll be OK.
        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, HandleRef dwNewLong)
        {
            if (IntPtr.Size == 4) return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern bool UnhookWindowsHookEx(HandleRef hhook);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        public static extern int GetWindowThreadProcessId(HandleRef hWnd, out int lpdwProcessId);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr CallNextHookEx(HandleRef hhook, int code, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.Ole32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        public static extern ILockBytes CreateILockBytesOnHGlobal(HandleRef hGlobal, bool fDeleteOnRelease);

        [DllImport(ExternDll.Ole32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IStorage StgCreateDocfileOnILockBytes(ILockBytes iLockBytes, int grfMode, int reserved);

        [Flags]
        public enum BrowseInfos
        {
            // Browsing for directory.
            ReturnOnlyFSDirs = 0x0001, // For finding a folder to start document searching
            DontGoBelowDomain = 0x0002, // For starting the Find Computer
            StatusText = 0x0004, // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if

            // this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
            // rest of the text.  This is not used with USENEWUI and BROWSEINFO.lpszTitle gets
            // all three lines of text.
            ReturnFSAncestors = 0x0008,
            EditBox = 0x0010, // Add an editbox to the dialog
            Validate = 0x0020, // insist on valid result (or CANCEL)

            NewDialogStyle = 0x0040, // Use the new dialog layout with the ability to resize
            // Caller needs to call OleInitialize() before using this API

            UseNewUI = NewDialogStyle | EditBox,

            AllowUrls = 0x0080, // Allow URLs to be displayed or entered. (Requires USENEWUI)

            BrowseForComputer = 0x1000, // Browsing for Computers.
            BrowseForPrinter = 0x2000, // Browsing for Printers
            BrowseForEverything = 0x4000, // Browsing for Everything
            ShowShares = 0x8000 // sharable resources displayed (remote shares, requires USENEWUI)
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class BROWSEINFO
        {
            public IntPtr hwndOwner; //HWND        hwndOwner;    // HWND of the owner for the dialog

            public int
                iImage; //int          iImage;                      // output var: where to return the Image index.

            public IntPtr
                lParam; //LPARAM       lParam;                      // extra info that's passed back in callbacks

            public IntPtr lpfn; //BFFCALLBACK  lpfn;            // Call back pointer

            public string lpszTitle; //LPCWSTR      lpszTitle;           // text to go in the banner over the tree.
            public IntPtr pidlRoot; //LPCITEMIDLIST pidlRoot;   // Root ITEMIDLIST

            // For interop purposes, send over a buffer of MAX_PATH size. 
            public IntPtr pszDisplayName; //LPWSTR       pszDisplayName;      // Return display name of item selected.
            public int ulFlags; //UINT         ulFlags;                     // Flags that control the return stuff
        }

        public class Shell32
        {
            [DllImport(ExternDll.Shell32)]
            [ResourceExposure(ResourceScope.None)]
            public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);
            //SHSTDAPI SHGetSpecialFolderLocation(HWND hwnd, int csidl, LPITEMIDLIST *ppidl);

            [DllImport(ExternDll.Shell32, CharSet = CharSet.Auto)]
            [ResourceExposure(ResourceScope.None)]
            public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);
            //SHSTDAPI_(BOOL) SHGetPathFromIDListW(LPCITEMIDLIST pidl, LPWSTR pszPath);

            [DllImport(ExternDll.Shell32, CharSet = CharSet.Auto)]
            [ResourceExposure(ResourceScope.None)]
            public static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);
            //SHSTDAPI_(LPITEMIDLIST) SHBrowseForFolderW(LPBROWSEINFOW lpbi);

            [DllImport(ExternDll.Shell32)]
            [ResourceExposure(ResourceScope.None)]
            public static extern int SHGetMalloc([Out] [MarshalAs(UnmanagedType.LPArray)]
                IMalloc[] ppMalloc);

            //SHSTDAPI SHGetMalloc(LPMALLOC * ppMalloc);
        }

        [ComImport]
        [Guid("00000002-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMalloc
        {
            [PreserveSig]
            IntPtr Alloc(int cb);

            [PreserveSig]
            IntPtr Realloc(IntPtr pv, int cb);

            [PreserveSig]
            void Free(IntPtr pv);

            [PreserveSig]
            int GetSize(IntPtr pv);

            [PreserveSig]
            int DidAlloc(IntPtr pv);

            [PreserveSig]
            void HeapMinimize();
        }

        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;

            public bool fErase;

            // rcPaint was a by-value RECT structure
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [ComImport]
        [Guid("00020D03-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRichEditOleCallback
        {
        }

        [ComImport]
        [Guid("00020D03-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRichTextBoxOleCallback
        {
            [PreserveSig]
            int GetNewStorage(out IStorage ret);

            [PreserveSig]
            int GetInPlaceContext(IntPtr lplpFrame, IntPtr lplpDoc, IntPtr lpFrameInfo);

            [PreserveSig]
            int ShowContainerUI(int fShow);

            [PreserveSig]
            int QueryInsertObject(ref Guid lpclsid, IntPtr lpstg, int cp);

            [PreserveSig]
            int DeleteObject(IntPtr lpoleobj);

            [PreserveSig]
            int QueryAcceptData(IComDataObject lpdataobj, /* CLIPFORMAT* */ IntPtr lpcfFormat, int reco, int fReally,
                IntPtr hMetaPict);

            [PreserveSig]
            int ContextSensitiveHelp(int fEnterMode);

            [PreserveSig]
            int GetClipboardData(NativeMethods.CHARRANGE lpchrg, int reco, IntPtr lplpdataobj);

            [PreserveSig]
            int GetDragDropEffect(bool fDrag, int grfKeyState, ref int pdwEffect);

            [PreserveSig]
            int GetContextMenu(short seltype, IntPtr lpoleobj, NativeMethods.CHARRANGE lpchrg, out IntPtr hmenu);
        }

        [ComImport]
        [Guid("0000000B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IStorage
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            IStream CreateStream(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName,
                [In] [MarshalAs(UnmanagedType.U4)] int grfMode,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved1,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved2);

            [return: MarshalAs(UnmanagedType.Interface)]
            IStream OpenStream(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName,
                IntPtr reserved1,
                [In] [MarshalAs(UnmanagedType.U4)] int grfMode,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved2);

            [return: MarshalAs(UnmanagedType.Interface)]
            IStorage CreateStorage(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName,
                [In] [MarshalAs(UnmanagedType.U4)] int grfMode,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved1,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved2);

            [return: MarshalAs(UnmanagedType.Interface)]
            IStorage OpenStorage(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName,
                IntPtr pstgPriority, // must be null
                [In] [MarshalAs(UnmanagedType.U4)] int grfMode,
                IntPtr snbExclude,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved);

            void CopyTo(
                int ciidExclude,
                [In] [MarshalAs(UnmanagedType.LPArray)]
                Guid[] pIIDExclude,
                IntPtr snbExclude,
                [In] [MarshalAs(UnmanagedType.Interface)]
                IStorage stgDest);

            void MoveElementTo(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName,
                [In] [MarshalAs(UnmanagedType.Interface)]
                IStorage stgDest,
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsNewName,
                [In] [MarshalAs(UnmanagedType.U4)] int grfFlags);

            void Commit(
                int grfCommitFlags);

            void Revert();

            void EnumElements(
                [In] [MarshalAs(UnmanagedType.U4)] int reserved1,
                // void *
                IntPtr reserved2,
                [In] [MarshalAs(UnmanagedType.U4)] int reserved3,
                [Out] [MarshalAs(UnmanagedType.Interface)]
                out object ppVal); // IEnumSTATSTG

            void DestroyElement(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName);

            void RenameElement(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsOldName,
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

            void SetElementTimes(
                [In] [MarshalAs(UnmanagedType.BStr)] string pwcsName,
                [In] NativeMethods.FILETIME pctime,
                [In] NativeMethods.FILETIME patime,
                [In] NativeMethods.FILETIME pmtime);

            void SetClass(
                [In] ref Guid clsid);

            void SetStateBits(
                int grfStateBits,
                int grfMask);

            void Stat(
                [Out] NativeMethods.STATSTG pStatStg,
                int grfStatFlag);
        }

        [SuppressUnmanagedCodeSecurity]
        [ComImport]
        [Guid("0000000C-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IStream
        {
            int Read(
                IntPtr buf,
                int len);

            int Write(
                IntPtr buf,
                int len);

            [return: MarshalAs(UnmanagedType.I8)]
            long Seek(
                [In] [MarshalAs(UnmanagedType.I8)] long dlibMove,
                int dwOrigin);

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
                int grfCommitFlags);

            void Revert();

            void LockRegion(
                [In] [MarshalAs(UnmanagedType.I8)] long libOffset,
                [In] [MarshalAs(UnmanagedType.I8)] long cb,
                int dwLockType);

            void UnlockRegion(
                [In] [MarshalAs(UnmanagedType.I8)] long libOffset,
                [In] [MarshalAs(UnmanagedType.I8)] long cb,
                int dwLockType);

            void Stat(
                [Out] NativeMethods.STATSTG pStatstg,
                int grfStatFlag);

            [return: MarshalAs(UnmanagedType.Interface)]
            IStream Clone();
        }

        [ComImport]
        [Guid("0000000A-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ILockBytes
        {
            void ReadAt(
                [In] [MarshalAs(UnmanagedType.U8)] long ulOffset,
                [Out] IntPtr pv,
                [In] [MarshalAs(UnmanagedType.U4)] int cb,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                int[] pcbRead);

            void WriteAt(
                [In] [MarshalAs(UnmanagedType.U8)] long ulOffset,
                IntPtr pv,
                [In] [MarshalAs(UnmanagedType.U4)] int cb,
                [Out] [MarshalAs(UnmanagedType.LPArray)]
                int[] pcbWritten);

            void Flush();

            void SetSize(
                [In] [MarshalAs(UnmanagedType.U8)] long cb);

            void LockRegion(
                [In] [MarshalAs(UnmanagedType.U8)] long libOffset,
                [In] [MarshalAs(UnmanagedType.U8)] long cb,
                [In] [MarshalAs(UnmanagedType.U4)] int dwLockType);

            void UnlockRegion(
                [In] [MarshalAs(UnmanagedType.U8)] long libOffset,
                [In] [MarshalAs(UnmanagedType.U8)] long cb,
                [In] [MarshalAs(UnmanagedType.U4)] int dwLockType);

            void Stat(
                [Out] NativeMethods.STATSTG pstatstg,
                [In] [MarshalAs(UnmanagedType.U4)] int grfStatFlag);
        }

        public const int OBJID_CLIENT = unchecked((int)0xFFFFFFFC);
    }
}
