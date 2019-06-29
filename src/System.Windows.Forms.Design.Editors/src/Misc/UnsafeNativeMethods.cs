// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Design
{
    internal class UnsafeNativeMethods
    {
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(HandleRef hWnd, [In] [Out] NativeMethods.POINT pt);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ScreenToClient(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern void NotifyWinEvent(int winEvent, HandleRef hwnd, int objType, int objID);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        [DllImport(ExternDll.Ole32)]
        public static extern int ReadClassStg(HandleRef pStg, [In] [Out] ref Guid pclsid);

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "GetDC", CharSet = CharSet.Auto)]
        private static extern IntPtr IntGetDC(HandleRef hWnd);

        public static IntPtr GetDC(HandleRef hWnd)
        {
            return Interop.HandleCollector.Add(IntGetDC(hWnd), Interop.CommonHandles.HDC);
        }

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "ReleaseDC", CharSet = CharSet.Auto)]
        private static extern int IntReleaseDC(HandleRef hWnd, HandleRef hDC);

        public static int ReleaseDC(HandleRef hWnd, HandleRef hDC)
        {
            Interop.HandleCollector.Remove((IntPtr)hDC, Interop.CommonHandles.HDC);
            return IntReleaseDC(hWnd, hDC);
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceCaps(HandleRef hDC, int nIndex);

        [DllImport(ExternDll.Ole32, PreserveSig = false)]
        public static extern ILockBytes CreateILockBytesOnHGlobal(HandleRef hGlobal, bool fDeleteOnRelease);

        [DllImport(ExternDll.Ole32, PreserveSig = false)]
        public static extern IStorage StgCreateDocfileOnILockBytes(ILockBytes iLockBytes, int grfMode, int reserved);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

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
