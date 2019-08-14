// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Accessibility;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    internal static class UnsafeNativeMethods
    {
        [DllImport(ExternDll.Shlwapi, CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern uint SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetMessageTime();

        [DllImport(ExternDll.User32)]
        public static extern int GetClassName(HandleRef hwnd, StringBuilder lpClassName, int nMaxCount);

        //SetClassLong won't work correctly for 64-bit: we should use SetClassLongPtr instead.  On
        //32-bit, SetClassLongPtr is just #defined as SetClassLong.  SetClassLong really should
        //take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        //it'll be OK.
        public static IntPtr SetClassLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetClassLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetClassLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto, EntryPoint = "SetClassLong")]
        public static extern IntPtr SetClassLongPtr32(HandleRef hwnd, int nIndex, IntPtr dwNewLong);

        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto, EntryPoint = "SetClassLongPtr")]
        public static extern IntPtr SetClassLongPtr64(HandleRef hwnd, int nIndex, IntPtr dwNewLong);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, PreserveSig = false)]
        public static extern IClassFactory2 CoGetClassObject(
            [In]
            ref Guid clsid,
            int dwContext,
            int serverInfo,
            [In]
            ref Guid refiid);

        [return: MarshalAs(UnmanagedType.Interface)]
        [DllImport(ExternDll.Ole32, ExactSpelling = true, PreserveSig = false)]
        public static extern object CoCreateInstance(
            [In]
            ref Guid clsid,
            [MarshalAs(UnmanagedType.Interface)]
            object punkOuter,
            int context,
            [In]
            ref Guid iid);

        [DllImport(ExternDll.Kernel32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetLocaleInfo(int Locale, int LCType, StringBuilder lpLCData, int cchData);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PageSetupDlg([In, Out] NativeMethods.PAGESETUPDLG lppsd);

        [DllImport(ExternDll.Comdlg32, EntryPoint = "PrintDlg", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PrintDlg_32([In, Out] NativeMethods.PRINTDLG_32 lppd);

        [DllImport(ExternDll.Comdlg32, EntryPoint = "PrintDlg", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PrintDlg_64([In, Out] NativeMethods.PRINTDLG_64 lppd);

        public static bool PrintDlg([In, Out] NativeMethods.PRINTDLG lppd)
        {
            if (IntPtr.Size == 4)
            {
                if (!(lppd is NativeMethods.PRINTDLG_32 lppd_32))
                {
                    throw new NullReferenceException("PRINTDLG data is null");
                }
                return PrintDlg_32(lppd_32);
            }
            else
            {
                if (!(lppd is NativeMethods.PRINTDLG_64 lppd_64))
                {
                    throw new NullReferenceException("PRINTDLG data is null");
                }
                return PrintDlg_64(lppd_64);
            }
        }

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int PrintDlgEx([In, Out] NativeMethods.PRINTDLGEX lppdex);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int OleGetClipboard(ref IComDataObject data);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int OleSetClipboard(IComDataObject pDataObj);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int OleFlushClipboard();

        [DllImport(ExternDll.Oleaut32, ExactSpelling = true)]
        public static extern void OleCreatePropertyFrameIndirect(NativeMethods.OCPFIPARAMS p);

        [DllImport(ExternDll.Oleaut32, EntryPoint = "OleCreateFontIndirect", ExactSpelling = true, PreserveSig = false)]
        public static extern IFont OleCreateIFontIndirect(NativeMethods.FONTDESC fd, ref Guid iid);

        [DllImport(ExternDll.Oleaut32, EntryPoint = "OleCreatePictureIndirect", ExactSpelling = true, PreserveSig = false)]
        public static extern IPicture OleCreateIPictureIndirect([MarshalAs(UnmanagedType.AsAny)]object pictdesc, ref Guid iid, bool fOwn);

        [DllImport(ExternDll.Oleaut32, EntryPoint = "OleCreatePictureIndirect", ExactSpelling = true, PreserveSig = false)]
        public static extern IPictureDisp OleCreateIPictureDispIndirect([MarshalAs(UnmanagedType.AsAny)] object pictdesc, ref Guid iid, bool fOwn);

        // cpb: #8309 -- next two methods, refiid arg must be IPicture.iid
        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        public static extern IPicture OleCreatePictureIndirect(NativeMethods.PICTDESC pictdesc, [In]ref Guid refiid, bool fOwn);

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        public static extern IFont OleCreateFontIndirect(NativeMethods.tagFONTDESC fontdesc, [In]ref Guid refiid);

        [DllImport(ExternDll.Oleaut32, ExactSpelling = true)]
        public static extern int VarFormat(ref object pvarIn, HandleRef pstrFormat, int iFirstDay, int iFirstWeek, uint dwFlags, [In, Out]ref IntPtr pbstr);

        [DllImport(ExternDll.Shell32, CharSet = CharSet.Auto)]
        public static extern int DragQueryFile(HandleRef hDrop, int iFile, StringBuilder lpszFile, int cch);

        public static int DragQueryFileLongPath(HandleRef hDrop, int iFile, StringBuilder lpszFile)
        {
            if (null != lpszFile && 0 != lpszFile.Capacity && iFile != unchecked((int)0xFFFFFFFF))
            {
                int resultValue = 0;

                // iterating by allocating chunk of memory each time we find the length is not sufficient.
                // Performance should not be an issue for current MAX_PATH length due to this
                if ((resultValue = DragQueryFile(hDrop, iFile, lpszFile, lpszFile.Capacity)) == lpszFile.Capacity)
                {
                    // passing null for buffer will return actual number of charectors in the file name.
                    // So, one extra call would be suffice to avoid while loop in case of long path.
                    int capacity = DragQueryFile(hDrop, iFile, null, 0);
                    if (capacity < Interop.Kernel32.MAX_UNICODESTRING_LEN)
                    {
                        lpszFile.EnsureCapacity(capacity);
                        resultValue = DragQueryFile(hDrop, iFile, lpszFile, capacity);
                    }
                    else
                    {
                        resultValue = 0;
                    }
                }

                lpszFile.Length = resultValue;
                return resultValue;  // what ever the result.
            }
            else
            {
                return DragQueryFile(hDrop, iFile, lpszFile, lpszFile.Capacity);
            }
        }

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool EnumChildWindows(HandleRef hwndParent, NativeMethods.EnumChildrenCallback lpEnumFunc, HandleRef lParam);

        [DllImport(ExternDll.Shell32, CharSet = CharSet.Auto, EntryPoint = "ShellExecute", BestFitMapping = false)]
        public static extern IntPtr ShellExecute_NoBFM(HandleRef hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetScrollPos(HandleRef hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnableScrollBar(HandleRef hWnd, int nBar, int value);

        [DllImport(ExternDll.Shell32, CharSet = CharSet.Auto)]
        public static extern int Shell_NotifyIcon(int message, NativeMethods.NOTIFYICONDATA pnid);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static bool InsertMenuItem(HandleRef hMenu, int uItem, bool fByPosition, NativeMethods.MENUITEMINFO_T lpmii);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr GetMenu(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool GetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, [In, Out] NativeMethods.MENUITEMINFO_T lpmii);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool GetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, [In, Out] NativeMethods.MENUITEMINFO_T_RW lpmii);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static bool SetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, NativeMethods.MENUITEMINFO_T lpmii);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr CreateMenu();

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool EndDialog(HandleRef hWnd, IntPtr result);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int WideCharToMultiByte(int codePage, int flags, [MarshalAs(UnmanagedType.LPWStr)]string wideStr, int chars, [In, Out]byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed);

        [DllImport(ExternDll.Kernel32, SetLastError = true, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Auto)]
        public static extern void CopyMemory(HandleRef destData, HandleRef srcData, int size);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
        public static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
        public static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr DuplicateHandle(HandleRef processSource, HandleRef handleSource, HandleRef processTarget, ref IntPtr handleTarget, int desiredAccess, bool inheritHandle, int options);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int hookid, NativeMethods.HookProc pfnhook, HandleRef hinst, int threadid);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetKeyboardState(byte[] keystate);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int SetKeyboardState(byte[] keystate);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(HandleRef hhook);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(int vkey);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CallNextHookEx(HandleRef hhook, int code, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ScreenToClient(HandleRef hWnd, ref Point pt);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);

        public static StringBuilder GetModuleFileNameLongPath(HandleRef hModule)
        {
            StringBuilder buffer = new StringBuilder(Interop.Kernel32.MAX_PATH);
            int noOfTimes = 1;
            int length = 0;
            // Iterating by allocating chunk of memory each time we find the length is not sufficient.
            // Performance should not be an issue for current MAX_PATH length due to this change.
            while (((length = GetModuleFileName(hModule, buffer, buffer.Capacity)) == buffer.Capacity)
                && Marshal.GetLastWin32Error() == NativeMethods.ERROR_INSUFFICIENT_BUFFER
                && buffer.Capacity < Interop.Kernel32.MAX_UNICODESTRING_LEN)
            {
                noOfTimes += 2; // Increasing buffer size by 520 in each iteration.
                int capacity = noOfTimes * Interop.Kernel32.MAX_PATH < Interop.Kernel32.MAX_UNICODESTRING_LEN ? noOfTimes * Interop.Kernel32.MAX_PATH : Interop.Kernel32.MAX_UNICODESTRING_LEN;
                buffer.EnsureCapacity(capacity);
            }
            buffer.Length = length;
            return buffer;
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        public static extern bool IsDialogMessage(HandleRef hWndDlg, [In, Out] ref NativeMethods.MSG msg);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool TranslateMessage([In, Out] ref NativeMethods.MSG msg);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr DispatchMessageA([In] ref NativeMethods.MSG msg);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr DispatchMessageW([In] ref NativeMethods.MSG msg);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int PostThreadMessage(int id, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.Ole32, ExactSpelling = true)]
        public static extern int CoRegisterMessageFilter(HandleRef newFilter, ref IntPtr oldMsgFilter);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, SetLastError = true)]
        public static extern int OleInitialize(int val = 0);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public extern static bool EnumThreadWindows(int dwThreadId, NativeMethods.EnumThreadWindowsCallback lpfn, HandleRef lParam);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        public extern static bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendDlgItemMessage(HandleRef hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int OleUninitialize();

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, Point pt, int uFlags);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseHandle(HandleRef handle);

        #region SendKeys SendInput functionality

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        [DllImport(ExternDll.User32, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint SendInput(uint nInputs, NativeMethods.INPUT[] pInputs, int cbSize);

        #endregion

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr GetDCEx(HandleRef hWnd, HandleRef hrgnClip, int flags);

        // GetObject stuff
        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(HandleRef hObject, int nSize, [In, Out] NativeMethods.BITMAP bm);

        //HPALETTE
        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetObject(HandleRef hObject, int nSize, ref int nEntries);

        [DllImport(ExternDll.User32)]
        public static extern IntPtr CreateAcceleratorTable(/*ACCEL*/ HandleRef pentries, int cCount);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool DestroyAcceleratorTable(HandleRef hAccel);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern short VkKeyScan(char key);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetCapture();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetCapture(HandleRef hwnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int keyCode);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern int SetWindowRgn(HandleRef hwnd, HandleRef hrgn, bool fRedraw);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern void GetTempFileName(string tempDirName, string prefixName, int unique, StringBuilder sb);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GlobalReAlloc(HandleRef handle, int bytes, int flags);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GlobalLock(HandleRef handle);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GlobalUnlock(HandleRef handle);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GlobalFree(HandleRef handle);

        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GlobalSize(HandleRef handle);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern bool ImmSetConversionStatus(HandleRef hIMC, int conversion, int sentence);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern bool ImmGetConversionStatus(HandleRef hIMC, ref int conversion, ref int sentence);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern IntPtr ImmGetContext(HandleRef hWnd);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern bool ImmReleaseContext(HandleRef hWnd, HandleRef hIMC);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern IntPtr ImmAssociateContext(HandleRef hWnd, HandleRef hIMC);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern IntPtr ImmCreateContext();

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern bool ImmSetOpenStatus(HandleRef hIMC, bool open);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern bool ImmGetOpenStatus(HandleRef hIMC);

        [DllImport(ExternDll.Imm32, CharSet = CharSet.Auto)]
        public static extern bool ImmNotifyIME(HandleRef hIMC, int dwAction, int dwIndex, int dwValue);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetAncestor(HandleRef hWnd, int flags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsChild(HandleRef hWndParent, HandleRef hwnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsZoomed(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, ref Interop.RECT rect, int cPoints);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, ref Point pt, uint cPoints);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, bool wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int[] lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int[] wParam, int[] lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, ref int wParam, ref int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, StringBuilder lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.TBBUTTON lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.TBBUTTONINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.TV_ITEM lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.TV_INSERTSTRUCT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.TV_HITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVBKIMAGE lParam);

        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.LVHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.TCITEM_T lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.HDLAYOUT hdlayout);

        //for Tooltips
        //
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, HandleRef wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, HandleRef lParam);

        // For RichTextBox
        //
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out, MarshalAs(UnmanagedType.LPStruct)] NativeMethods.PARAFORMAT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out, MarshalAs(UnmanagedType.LPStruct)] NativeMethods.CHARFORMATA lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out, MarshalAs(UnmanagedType.LPStruct)] NativeMethods.CHARFORMAT2A lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.CHARFORMATW lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int SendMessage(HandleRef hWnd, int msg, int wParam, [Out, MarshalAs(UnmanagedType.IUnknown)]out object editOle);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref Interop.Richedit.CHARRANGE lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.FINDTEXT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.TEXTRANGE lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref Point lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, ref Point wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.EDITSTREAM lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.EDITSTREAM64 lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, NativeMethods.GETTEXTLENGTHEX wParam, int lParam);

        // For Button

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref Size lParam);

        // For ListView

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out] ref NativeMethods.LVFINDINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVCOLUMN_T lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out] ref NativeMethods.LVITEM lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVCOLUMN lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVGROUP lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, ref Point wParam, [In, Out] NativeMethods.LVINSERTMARK lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVINSERTMARK lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out] NativeMethods.LVTILEVIEWINFO lParam);

        // For MonthCalendar
        //
        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.MCHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.SYSTEMTIME lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.SYSTEMTIMEARRAY lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.LOGFONTW lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.MSG lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, ref Interop.RECT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, ref short wParam, ref short lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, [In, Out, MarshalAs(UnmanagedType.Bool)] ref bool wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, int wParam, ref Interop.RECT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, int wParam, [In, Out] ref Rectangle lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, NativeMethods.ListViewCompareCallback pfnCompare);

        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam, int flags, int timeout, out IntPtr pdwResult);

        public const int SMTO_ABORTIFHUNG = 0x0002;

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool GetWindowRect(HandleRef hWnd, ref Interop.RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(HandleRef hWnd, int nIDDlgItem);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr DefMDIChildProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, int msg,
                                                IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool GetClassInfoW(HandleRef hInstance, string lpClassName, ref NativeMethods.WNDCLASS lpWndClass);

        [DllImport(ExternDll.Gdi32, CharSet = CharSet.Auto)]
        public static extern bool GetTextMetrics(HandleRef hdc, NativeMethods.TEXTMETRIC tm);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private unsafe static extern bool SystemParametersInfoW(uint uiAction, uint uiParam, void* pvParam, uint fWinIni);

        public unsafe static bool SystemParametersInfoW(uint uiAction, ref Interop.RECT rect)
        {
            fixed (void* p = &rect)
            {
                return SystemParametersInfoW(uiAction, 0, p, 0);
            }
        }

        public unsafe static bool SystemParametersInfoW(uint uiAction, ref int value)
        {
            fixed (void* p = &value)
            {
                return SystemParametersInfoW(uiAction, 0, p, 0);
            }
        }

        public unsafe static int SystemParametersInfoInt(uint uiAction)
        {
            int value = 0;
            SystemParametersInfoW(uiAction, ref value);
            return value;
        }

        public unsafe static bool SystemParametersInfoW(uint uiAction, ref bool value)
        {
            Interop.BOOL nativeBool = value ? Interop.BOOL.TRUE : Interop.BOOL.FALSE;
            bool result = SystemParametersInfoW(uiAction, 0, &nativeBool, 0);
            value = nativeBool != Interop.BOOL.FALSE;
            return result;
        }

        public unsafe static bool SystemParametersInfoBool(uint uiAction)
        {
            bool value = false;
            SystemParametersInfoW(uiAction, ref value);
            return value;
        }

        public unsafe static bool SystemParametersInfoW(ref NativeMethods.HIGHCONTRASTW highContrast)
        {
            fixed (void* p = &highContrast)
            {
                highContrast.cbSize = (uint)sizeof(NativeMethods.HIGHCONTRASTW);
                return SystemParametersInfoW(
                    NativeMethods.SPI_GETHIGHCONTRAST,
                    highContrast.cbSize,
                    p,
                    0); // This has no meaning when getting values
            }
        }

        public unsafe static bool SystemParametersInfoW(ref NativeMethods.NONCLIENTMETRICSW metrics)
        {
            fixed (void* p = &metrics)
            {
                metrics.cbSize = (uint)sizeof(NativeMethods.NONCLIENTMETRICSW);
                return SystemParametersInfoW(
                    NativeMethods.SPI_GETNONCLIENTMETRICS,
                    metrics.cbSize,
                    p,
                    0); // This has no meaning when getting values
            }
        }

        // This API is available starting Windows 10 Anniversary Update (Redstone 1 / 1607 / 14393).
        // Unlike SystemParametersInfo, there is no "A/W" variance in this api.
        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool SystemParametersInfoForDpi(
            uint uiAction,
            uint uiParam,
            ref NativeMethods.NONCLIENTMETRICSW pvParam,
            uint fWinIni,
            uint dpi);

        /// <summary>
        ///  Tries to get system parameter info for the dpi. dpi is ignored if "SystemParametersInfoForDpi()" API is not available on the OS that this application is running.
        /// </summary>
        public unsafe static bool TrySystemParametersInfoForDpi(ref NativeMethods.NONCLIENTMETRICSW metrics, uint dpi)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                metrics.cbSize = (uint)sizeof(NativeMethods.NONCLIENTMETRICSW);
                return SystemParametersInfoForDpi(
                    NativeMethods.SPI_GETNONCLIENTMETRICS,
                    metrics.cbSize,
                    ref metrics,
                    0,
                    dpi);
            }
            else
            {
                return SystemParametersInfoW(ref metrics);
            }
        }

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern bool GetComputerName(StringBuilder lpBuffer, int[] nSize);

        [DllImport(ExternDll.Advapi32, CharSet = CharSet.Auto)]
        public static extern bool GetUserName(StringBuilder lpBuffer, int[] nSize);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport(ExternDll.User32, SetLastError = true)]
        public static extern bool GetUserObjectInformation(HandleRef hObj, int nIndex, ref NativeMethods.USEROBJECTFLAGS pvBuffer, int nLength, ref int lpnLengthNeeded);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(HandleRef hWnd, ref Point lpPoint);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int MsgWaitForMultipleObjectsEx(int nCount, IntPtr pHandles, int dwMilliseconds, int dwWakeMask, int dwFlags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool PeekMessage([In, Out] ref NativeMethods.MSG msg, HandleRef hwnd, int msgMin, int msgMax, int remove);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]

        public static extern bool PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, HandleRef pAcc);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, IntPtr pAcc);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int CreateStdAccessibleObject(HandleRef hWnd, int objID, ref Guid refiid, [In, Out, MarshalAs(UnmanagedType.Interface)] ref object pAcc);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern void NotifyWinEvent(int winEvent, HandleRef hwnd, int objType, int objID);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetMenuItemID(HandleRef hMenu, int nPos);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetSubMenu(HandleRef hwnd, int index);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetMenuItemCount(HandleRef hMenu);

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        public static extern void GetErrorInfo(int reserved, [In, Out] ref IErrorInfo errorInfo);

        [DllImport(ExternDll.User32, ExactSpelling = true, EntryPoint = "BeginPaint", CharSet = CharSet.Auto)]
        private static extern IntPtr IntBeginPaint(HandleRef hWnd, [In, Out] ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr BeginPaint(HandleRef hWnd, ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool EndPaint(HandleRef hWnd, ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool EndPaint(IntPtr hWnd, ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr GetWindowDC(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, [In, Out] IntPtr[] rc, int nUpdate);

        [DllImport(ExternDll.User32, EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public extern static IntPtr SendCallbackMessage(HandleRef hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.Shell32, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void DragAcceptFiles(HandleRef hWnd, bool fAccept);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetScrollInfo(HandleRef hWnd, int fnBar, NativeMethods.SCROLLINFO si);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int SetScrollInfo(HandleRef hWnd, int fnBar, NativeMethods.SCROLLINFO si, bool redraw);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();

        //GetWindowLong won't work correctly for 64-bit: we should use GetWindowLongPtr instead.  On
        //32-bit, GetWindowLongPtr is just #defined as GetWindowLong.  GetWindowLong really should
        //take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        //it'll be OK.
        public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLong32(HandleRef hWnd, int nIndex);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, int nIndex);

        //SetWindowLong won't work correctly for 64-bit: we should use SetWindowLongPtr instead.  On
        //32-bit, SetWindowLongPtr is just #defined as SetWindowLong.  SetWindowLong really should
        //take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        //it'll be OK.
        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, HandleRef dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, NativeMethods.WndProc wndproc)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, wndproc);
            }
            return SetWindowLongPtr64(hWnd, nIndex, wndproc);
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, NativeMethods.WndProc wndproc);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, NativeMethods.WndProc wndproc);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr CreatePopupMenu();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool RemoveMenu(HandleRef hMenu, int uPosition, int uFlags);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool DestroyMenu(HandleRef hMenu);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetSystemMenu(HandleRef hWnd, bool bRevert);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr DefFrameProc(IntPtr hWnd, IntPtr hWndClient, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool TranslateMDISysAccel(IntPtr hWndClient, [In, Out] ref NativeMethods.MSG msg);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(HandleRef hwnd, int crKey, byte bAlpha, int dwFlags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public extern static bool SetMenu(HandleRef hWnd, HandleRef hMenu);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowPlacement(HandleRef hWnd, ref NativeMethods.WINDOWPLACEMENT placement);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern void GetStartupInfo([In, Out] NativeMethods.STARTUPINFO_I startupinfo_i);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetMenuDefaultItem(HandleRef hwnd, int nIndex, bool pos);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool EnableMenuItem(HandleRef hMenu, int UIDEnabledItem, int uEnable);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetActiveWindow(HandleRef hWnd);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateIC(string lpszDriverName, string lpszDeviceName, string lpszOutput, HandleRef /*DEVMODE*/ lpInitData);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ClipCursor(ref Interop.RECT rcClip);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ClipCursor(NativeMethods.COMRECT rcClip);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetCursor(HandleRef hcursor);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public extern static int ShowCursor(bool bShow);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool DestroyCursor(HandleRef hCurs);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindow(HandleRef hWnd);

        public const int LAYOUT_RTL = 0x00000001;
        public const int LAYOUT_BITMAPORIENTATIONPRESERVED = 0x00000008;

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern bool GetMessageA([In, Out] ref NativeMethods.MSG msg, HandleRef hWnd, int uMsgFilterMin, int uMsgFilterMax);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern bool GetMessageW([In, Out] ref NativeMethods.MSG msg, HandleRef hWnd, int uMsgFilterMin, int uMsgFilterMax);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(HandleRef hwnd, int msg, int wparam, int lparam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(HandleRef hwnd, int msg, int wparam, IntPtr lparam);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool GetClientRect(HandleRef hWnd, ref Interop.RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetClientRect(HandleRef hWnd, IntPtr rect);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            HandleRef hWndParent,
            HandleRef hMenu,
            HandleRef hInst,
            [MarshalAs(UnmanagedType.AsAny)] object lpParam);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool DestroyWindow(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern ushort RegisterClassW(ref NativeMethods.WNDCLASS lpWndClass);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern void WaitMessage();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowPlacement(HandleRef hWnd, [In] ref NativeMethods.WINDOWPLACEMENT placement);

        // This method is not available until Windows 8.1
        [DllImport(ExternDll.User32, ExactSpelling = true, SetLastError = true)]
        public static extern uint GetDpiForWindow(HandleRef hWnd);

        // For system power status
        //
        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetSystemPowerStatus([In, Out] ref NativeMethods.SYSTEM_POWER_STATUS systemPowerStatus);

        [DllImport(ExternDll.Powrprof, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        //for RegionData
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetRegionData(HandleRef hRgn, int size, IntPtr lpRgnData);

        public unsafe static Interop.RECT[] GetRectsFromRegion(IntPtr hRgn)
        {
            Interop.RECT[] regionRects = null;
            IntPtr pBytes = IntPtr.Zero;
            try
            {
                // see how much memory we need to allocate
                int regionDataSize = GetRegionData(new HandleRef(null, hRgn), 0, IntPtr.Zero);
                if (regionDataSize != 0)
                {
                    pBytes = Marshal.AllocCoTaskMem(regionDataSize);
                    // get the data
                    int ret = GetRegionData(new HandleRef(null, hRgn), regionDataSize, pBytes);
                    if (ret == regionDataSize)
                    {
                        // cast to the structure
                        NativeMethods.RGNDATAHEADER* pRgnDataHeader = (NativeMethods.RGNDATAHEADER*)pBytes;
                        if (pRgnDataHeader->iType == 1)
                        {    // expecting RDH_RECTANGLES
                            regionRects = new Interop.RECT[pRgnDataHeader->nCount];

                            Debug.Assert(regionDataSize == pRgnDataHeader->cbSizeOfStruct + pRgnDataHeader->nCount * pRgnDataHeader->nRgnSize);
                            Debug.Assert(Marshal.SizeOf<Interop.RECT>() == pRgnDataHeader->nRgnSize || pRgnDataHeader->nRgnSize == 0);

                            // use the header size as the offset, and cast each rect in.
                            int rectStart = pRgnDataHeader->cbSizeOfStruct;
                            for (int i = 0; i < pRgnDataHeader->nCount; i++)
                            {
                                // use some fancy pointer math to just copy the rect bits directly into the array.
                                regionRects[i] = *((Interop.RECT*)((byte*)pBytes + rectStart + (Marshal.SizeOf<Interop.RECT>() * i)));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (pBytes != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pBytes);
                }
            }
            return regionRects;
        }

        public static ref T PtrToRef<T>(IntPtr ptr)
            where T : unmanaged
        {
            unsafe
            {
                return ref Unsafe.AsRef<T>(ptr.ToPointer());
            }
        }

        [ComImport(), Guid("00000121-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleDropSource
        {
            [PreserveSig]
            int OleQueryContinueDrag(
                int fEscapePressed,
                [In, MarshalAs(UnmanagedType.U4)]
                int grfKeyState);

            [PreserveSig]
            int OleGiveFeedback(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwEffect);
        }

        [ComImport()]
        [Guid("B196B289-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleControlSite
        {
            [PreserveSig]
            int OnControlInfoChanged();

            [PreserveSig]
            int LockInPlaceActive(int fLock);

            [PreserveSig]
            int GetExtendedControl(
                [Out, MarshalAs(UnmanagedType.IDispatch)]
                out object ppDisp);

            [PreserveSig]
            Interop.HRESULT TransformCoords(
                Point *pPtlHimetric,
                PointF *pPtfContainer,
                uint dwFlags);

            [PreserveSig]
            int TranslateAccelerator(
                [In]
                ref NativeMethods.MSG pMsg,
                [In, MarshalAs(UnmanagedType.U4)]
                int grfModifiers);

            [PreserveSig]
            int OnFocus(int fGotFocus);

            [PreserveSig]
            int ShowPropertyFrame();

        }

        [ComImport(), Guid("00000118-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleClientSite
        {
            [PreserveSig]
            int SaveObject();

            [PreserveSig]
            int GetMoniker(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwAssign,
                [In, MarshalAs(UnmanagedType.U4)]
                int dwWhichMoniker,
                [Out, MarshalAs(UnmanagedType.Interface)]
                out object moniker);

            [PreserveSig]
            int GetContainer(out IOleContainer container);

            [PreserveSig]
            int ShowObject();

            [PreserveSig]
            int OnShowWindow(int fShow);

            [PreserveSig]
            int RequestNewObjectLayout();
        }

        [ComImport(), Guid("00000119-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceSite
        {
            IntPtr GetWindow();

            [PreserveSig]
            int ContextSensitiveHelp(int fEnterMode);

            [PreserveSig]
            int CanInPlaceActivate();

            [PreserveSig]
            int OnInPlaceActivate();

            [PreserveSig]
            int OnUIActivate();

            [PreserveSig]
            int GetWindowContext(
                [Out, MarshalAs(UnmanagedType.Interface)]
                out IOleInPlaceFrame ppFrame,
                [Out, MarshalAs(UnmanagedType.Interface)]
                out IOleInPlaceUIWindow ppDoc,
                [Out]
                NativeMethods.COMRECT lprcPosRect,
                [Out]
                NativeMethods.COMRECT lprcClipRect,
                [In, Out]
                NativeMethods.tagOIFI lpFrameInfo);

            [PreserveSig]
            Interop.HRESULT Scroll(
                Size scrollExtant);

            [PreserveSig]
            int OnUIDeactivate(
                int fUndoable);

            [PreserveSig]
            int OnInPlaceDeactivate();

            [PreserveSig]
            int DiscardUndoState();

            [PreserveSig]
            int DeactivateAndUndo();

            [PreserveSig]
            int OnPosRectChange(
                [In]
                NativeMethods.COMRECT lprcPosRect);
        }

        [ComImport(), Guid("742B0E01-14E6-101B-914E-00AA00300CAB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISimpleFrameSite
        {
            [PreserveSig]
            int PreMessageFilter(
                IntPtr hwnd,
                [In, MarshalAs(UnmanagedType.U4)]
                int msg,
                IntPtr wp,
                IntPtr lp,
                [In, Out]
                ref IntPtr plResult,
                [In, Out, MarshalAs(UnmanagedType.U4)]
                ref int pdwCookie);

            [PreserveSig]
            int PostMessageFilter(
                IntPtr hwnd,
                [In, MarshalAs(UnmanagedType.U4)]
                int msg,
                IntPtr wp,
                IntPtr lp,
                [In, Out]
                ref IntPtr plResult,
                [In, MarshalAs(UnmanagedType.U4)]
                int dwCookie);
        }

        [ComImport(), Guid("40A050A0-3C31-101B-A82E-08002B2B2337"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IVBGetControl
        {
            [PreserveSig]
            int EnumControls(
                int dwOleContF,
                int dwWhich,
                [Out]
                out IEnumUnknown ppenum);
        }

        [ComImport(), Guid("91733A60-3F4C-101B-A3F6-00AA0034E4E9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGetVBAObject
        {
            [PreserveSig]
            int GetObject(
                 [In]
                ref Guid riid,
                 [Out, MarshalAs(UnmanagedType.LPArray)]
                IVBFormat[] rval,
                 int dwReserved);
        }

        [ComImport(), Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyNotifySink
        {
            void OnChanged(int dispID);

            [PreserveSig]
            int OnRequestEdit(int dispID);
        }

        [ComImport(), Guid("9849FD60-3768-101B-8D72-AE6164FFE3CF"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IVBFormat
        {
            [PreserveSig]
            int Format(
                [In]
                ref object var,
                IntPtr pszFormat,
                IntPtr lpBuffer,
                short cpBuffer,
                int lcid,
                short firstD,
                short firstW,
                [Out, MarshalAs(UnmanagedType.LPArray)]
                short[] result);
        }

        [ComImport(), Guid("00000100-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumUnknown
        {
            [PreserveSig]
            int Next(
                [In, MarshalAs(UnmanagedType.U4)]
                int celt,
                [Out]
                IntPtr rgelt,
                IntPtr pceltFetched);

            [PreserveSig]
            int Skip(
                [In, MarshalAs(UnmanagedType.U4)]
                int celt);

            void Reset();

            void Clone(
                [Out]
                out IEnumUnknown ppenum);
        }

        [ComImport(), Guid("0000011B-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleContainer
        {
            [PreserveSig]
            int ParseDisplayName(
                [In, MarshalAs(UnmanagedType.Interface)]
                object pbc,
                [In, MarshalAs(UnmanagedType.BStr)]
                string pszDisplayName,
                [Out, MarshalAs(UnmanagedType.LPArray)]
                int[] pchEaten,
                [Out, MarshalAs(UnmanagedType.LPArray)]
                object[] ppmkOut);

            [PreserveSig]
            int EnumObjects(
                [In, MarshalAs(UnmanagedType.U4)]
                int grfFlags,
                [Out]
                out IEnumUnknown ppenum);

            [PreserveSig]
            int LockContainer(
                bool fLock);
        }

        [ComImport(), Guid("00000116-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceFrame
        {
            IntPtr GetWindow();

            [PreserveSig]
            int ContextSensitiveHelp(int fEnterMode);

            [PreserveSig]
            int GetBorder(
                [Out]
                NativeMethods.COMRECT lprectBorder);

            [PreserveSig]
            int RequestBorderSpace(
                [In]
                NativeMethods.COMRECT pborderwidths);

            [PreserveSig]
            int SetBorderSpace(
                [In]
                NativeMethods.COMRECT pborderwidths);

            [PreserveSig]
            int SetActiveObject(
                [In, MarshalAs(UnmanagedType.Interface)]
                IOleInPlaceActiveObject pActiveObject,
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string pszObjName);

            [PreserveSig]
            int InsertMenus(
                [In]
                IntPtr hmenuShared,
                [In, Out]
                NativeMethods.tagOleMenuGroupWidths lpMenuWidths);

            [PreserveSig]
            int SetMenu(
                [In]
                IntPtr hmenuShared,
                [In]
                IntPtr holemenu,
                [In]
                IntPtr hwndActiveObject);

            [PreserveSig]
            int RemoveMenus(
                [In]
                IntPtr hmenuShared);

            [PreserveSig]
            int SetStatusText(
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string pszStatusText);

            [PreserveSig]
            int EnableModeless(
                bool fEnable);

            [PreserveSig]
            int TranslateAccelerator(
                [In]
                ref NativeMethods.MSG lpmsg,
                [In, MarshalAs(UnmanagedType.U2)]
                short wID);
        }

        // Used to control the webbrowser appearance and provide DTE to script via window.external
        [ComVisible(true), ComImport(), Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDocHostUIHandler
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowContextMenu(
                uint dwID,
                ref Point pt,
                [In, MarshalAs(UnmanagedType.Interface)]
                object pcmdtReserved,
                [In, MarshalAs(UnmanagedType.Interface)]
                object pdispReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetHostInfo(
                [In, Out]
                NativeMethods.DOCHOSTUIINFO info);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowUI(
                [In, MarshalAs(UnmanagedType.I4)]
                int dwID,
                [In]
                IOleInPlaceActiveObject activeObject,
                [In]
                NativeMethods.IOleCommandTarget commandTarget,
                [In]
                IOleInPlaceFrame frame,
                [In]
                IOleInPlaceUIWindow doc);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int HideUI();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int UpdateUI();

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnableModeless(
                [In, MarshalAs(UnmanagedType.Bool)]
                bool fEnable);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnDocWindowActivate(
                [In, MarshalAs(UnmanagedType.Bool)]
                bool fActivate);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnFrameWindowActivate(
                [In, MarshalAs(UnmanagedType.Bool)]
                bool fActivate);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ResizeBorder(
                [In]
                NativeMethods.COMRECT rect,
                [In]
                IOleInPlaceUIWindow doc,
                bool fFrameWindow);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateAccelerator(
                [In]
                ref NativeMethods.MSG msg,
                [In]
                ref Guid group,
                [In, MarshalAs(UnmanagedType.I4)]
                int nCmdID);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetOptionKeyPath(
                [Out, MarshalAs(UnmanagedType.LPArray)]
                string[] pbstrKey,
                [In, MarshalAs(UnmanagedType.U4)]
                int dw);

            [PreserveSig]
            Interop.HRESULT GetDropTarget(
                Interop.Ole32.IDropTarget pDropTarget,
                out Interop.Ole32.IDropTarget ppDropTarget);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetExternal(
                [Out, MarshalAs(UnmanagedType.Interface)]
                out object ppDispatch);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateUrl(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwTranslate,
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string strURLIn,
                [Out, MarshalAs(UnmanagedType.LPWStr)]
                out string pstrURLOut);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int FilterDataObject(
                IComDataObject pDO,
                out IComDataObject ppDORet);
        }

        [ComImport(), Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E"),
        TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FOleAutomation)]
        public interface IWebBrowser2
        {
            //
            // IWebBrowser members
            [DispId(100)] void GoBack();
            [DispId(101)] void GoForward();
            [DispId(102)] void GoHome();
            [DispId(103)] void GoSearch();
            [DispId(104)]
            void Navigate([In] string Url, [In] ref object flags,
                            [In] ref object targetFrameName, [In] ref object postData,
                            [In] ref object headers);
            [DispId(-550)] void Refresh();
            [DispId(105)] void Refresh2([In] ref object level);
            [DispId(106)] void Stop();
            [DispId(200)] object Application { [return: MarshalAs(UnmanagedType.IDispatch)]get; }
            [DispId(201)] object Parent { [return: MarshalAs(UnmanagedType.IDispatch)]get; }
            [DispId(202)] object Container { [return: MarshalAs(UnmanagedType.IDispatch)]get; }
            [DispId(203)] object Document { [return: MarshalAs(UnmanagedType.IDispatch)]get; }
            [DispId(204)] bool TopLevelContainer { get; }
            [DispId(205)] string Type { get; }
            [DispId(206)] int Left { get; set; }
            [DispId(207)] int Top { get; set; }
            [DispId(208)] int Width { get; set; }
            [DispId(209)] int Height { get; set; }
            [DispId(210)] string LocationName { get; }
            [DispId(211)] string LocationURL { get; }
            [DispId(212)] bool Busy { get; }
            //
            // IWebBrowserApp members
            [DispId(300)] void Quit();
            [DispId(301)] void ClientToWindow([Out]out int pcx, [Out]out int pcy);
            [DispId(302)] void PutProperty([In] string property, [In] object vtValue);
            [DispId(303)] object GetProperty([In] string property);
            [DispId(0)] string Name { get; }
            [DispId(-515)] int HWND { get; }
            [DispId(400)] string FullName { get; }
            [DispId(401)] string Path { get; }
            [DispId(402)] bool Visible { get; set; }
            [DispId(403)] bool StatusBar { get; set; }
            [DispId(404)] string StatusText { get; set; }
            [DispId(405)] int ToolBar { get; set; }
            [DispId(406)] bool MenuBar { get; set; }
            [DispId(407)] bool FullScreen { get; set; }
            //
            // IWebBrowser2 members
            [DispId(500)]
            void Navigate2([In] ref object URL, [In] ref object flags,
                            [In] ref object targetFrameName, [In] ref object postData,
                            [In] ref object headers);
            [DispId(501)] NativeMethods.OLECMDF QueryStatusWB([In] NativeMethods.OLECMDID cmdID);
            [DispId(502)]
            void ExecWB([In] NativeMethods.OLECMDID cmdID,
                    [In] NativeMethods.OLECMDEXECOPT cmdexecopt,
                    ref object pvaIn,
                    IntPtr pvaOut);
            [DispId(503)]
            void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow,
                    [In] ref object pvarSize);
            [DispId(-525)] WebBrowserReadyState ReadyState { get; }
            [DispId(550)] bool Offline { get; set; }
            [DispId(551)] bool Silent { get; set; }
            [DispId(552)] bool RegisterAsBrowser { get; set; }
            [DispId(553)] bool RegisterAsDropTarget { get; set; }
            [DispId(554)] bool TheaterMode { get; set; }
            [DispId(555)] bool AddressBar { get; set; }
            [DispId(556)] bool Resizable { get; set; }
        }

        [ComImport(), Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
        InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
        TypeLibType(TypeLibTypeFlags.FHidden)]
        public interface DWebBrowserEvents2
        {
            [DispId(102)] void StatusTextChange([In] string text);
            [DispId(108)] void ProgressChange([In] int progress, [In] int progressMax);
            [DispId(105)] void CommandStateChange([In] long command, [In] bool enable);
            [DispId(106)] void DownloadBegin();
            [DispId(104)] void DownloadComplete();
            [DispId(113)] void TitleChange([In] string text);
            [DispId(112)] void PropertyChange([In] string szProperty);
            [DispId(250)]
            void BeforeNavigate2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                                 [In] ref object URL, [In] ref object flags,
                                 [In] ref object targetFrameName, [In] ref object postData,
                                 [In] ref object headers, [In, Out] ref bool cancel);
            [DispId(251)]
            void NewWindow2([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object pDisp,
                                [In, Out] ref bool cancel);
            [DispId(252)]
            void NavigateComplete2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                                [In] ref object URL);
            [DispId(259)]
            void DocumentComplete([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                                [In] ref object URL);
            [DispId(253)] void OnQuit();
            [DispId(254)] void OnVisible([In] bool visible);
            [DispId(255)] void OnToolBar([In] bool toolBar);
            [DispId(256)] void OnMenuBar([In] bool menuBar);
            [DispId(257)] void OnStatusBar([In] bool statusBar);
            [DispId(258)] void OnFullScreen([In] bool fullScreen);
            [DispId(260)] void OnTheaterMode([In] bool theaterMode);
            [DispId(262)] void WindowSetResizable([In] bool resizable);
            [DispId(264)] void WindowSetLeft([In] int left);
            [DispId(265)] void WindowSetTop([In] int top);
            [DispId(266)] void WindowSetWidth([In] int width);
            [DispId(267)] void WindowSetHeight([In] int height);
            [DispId(263)] void WindowClosing([In] bool isChildWindow, [In, Out] ref bool cancel);
            [DispId(268)] void ClientToHostWindow([In, Out] ref long cx, [In, Out] ref long cy);
            [DispId(269)] void SetSecureLockIcon([In] int secureLockIcon);
            [DispId(270)] void FileDownload([In, Out] ref bool cancel);
            [DispId(271)]
            void NavigateError([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                    [In] ref object URL, [In] ref object frame, [In] ref object statusCode, [In, Out] ref bool cancel);
            [DispId(225)] void PrintTemplateInstantiation([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
            [DispId(226)] void PrintTemplateTeardown([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
            [DispId(227)]
            void UpdatePageStatus([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                    [In] ref object nPage, [In] ref object fDone);
            [DispId(272)] void PrivacyImpactedStateChange([In] bool bImpacted);
        }

        [ComImport(),
         Guid("39088D7E-B71E-11D1-8F39-00C04FD946D0"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IExtender
        {
            int Align { get; set; }

            bool Enabled { get; set; }

            int Height { get; set; }

            int Left { get; set; }

            bool TabStop { get; set; }

            int Top { get; set; }

            bool Visible { get; set; }

            int Width { get; set; }

            string Name { [return: MarshalAs(UnmanagedType.BStr)]get; }

            object Parent { [return: MarshalAs(UnmanagedType.Interface)]get; }

            IntPtr Hwnd { get; }

            object Container { [return: MarshalAs(UnmanagedType.Interface)]get; }

            void Move(
                [In, MarshalAs(UnmanagedType.Interface)]
                object left,
                [In, MarshalAs(UnmanagedType.Interface)]
                object top,
                [In, MarshalAs(UnmanagedType.Interface)]
                object width,
                [In, MarshalAs(UnmanagedType.Interface)]
                object height);
        }

        [ComImport(),
         Guid("8A701DA0-4FEB-101B-A82E-08002B2B2337"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IGetOleObject
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetOleObject(ref Guid riid);
        }

        [
            ComImport(),
            Guid("CB2F6722-AB3A-11d2-9C40-00C04FA30A3E"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        internal interface ICorRuntimeHost
        {
            [PreserveSig()] int CreateLogicalThreadState();
            [PreserveSig()] int DeleteLogicalThreadState();
            [PreserveSig()]
            int SwitchInLogicalThreadState(
             [In] ref uint pFiberCookie);                 // [in] Cookie that indicates the fiber to use.

            [PreserveSig()]
            int SwitchOutLogicalThreadState(
             out uint FiberCookie);               // [out] Cookie that indicates the fiber being switched out.

            [PreserveSig()]
            int LocksHeldByLogicalThread(           // Return code.
             out uint pCount                        // [out] Number of locks that the current thread holds.
                );

            [PreserveSig()]
            int MapFile(
             IntPtr hFile,          // [in]  HANDLE for file
             out IntPtr hMapAddress);   // [out] HINSTANCE for mapped file

            //=================================================================
            //
            // New hosting methods
            //
            // Returns an object for configuring the runtime prior to
            // it starting. If the runtime has been initialized this
            // routine returns an error. See ICorConfiguration.
            [PreserveSig()] int GetConfiguration([MarshalAs(UnmanagedType.IUnknown)] out object pConfiguration);

            // Starts the runtime. This is equivalent to CoInitializeCor();
            [PreserveSig()] int Start();

            // Terminates the runtime, This is equivalent CoUninitializeCor();
            [PreserveSig()] int Stop();

            // Creates a domain in the runtime. The identity array is
            // a pointer to an array TYPE containing IIdentity objects defining
            // the security identity.
            [PreserveSig()]
            int CreateDomain(string pwzFriendlyName,
                                 [MarshalAs(UnmanagedType.IUnknown)] object pIdentityArray, // Optional
                                 [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

            // Returns the default domain.
            [PreserveSig()] int GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

            // Enumerate currently existing domains.
            [PreserveSig()] int EnumDomains(out IntPtr hEnum);

            // Returns S_FALSE when there are no more domains. A domain
            // is passed out only when S_OK is returned.
            [PreserveSig()]
            int NextDomain(IntPtr hEnum,
                               [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

            // Close the enumeration, releasing resources
            [PreserveSig()] int CloseEnum(IntPtr hEnum);

            [PreserveSig()]
            int CreateDomainEx(string pwzFriendlyName, // Optional
                                   [MarshalAs(UnmanagedType.IUnknown)] object pSetup,        // Optional
                                   [MarshalAs(UnmanagedType.IUnknown)] object pEvidence,     // Optional
                                   [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

            [PreserveSig()] int CreateDomainSetup([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomainSetup);

            [PreserveSig()] int CreateEvidence([MarshalAs(UnmanagedType.IUnknown)] out object pEvidence);

            [PreserveSig()] int UnloadDomain([MarshalAs(UnmanagedType.IUnknown)] object pAppDomain);

            // Returns the thread's domain.
            [PreserveSig()] int CurrentDomain([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
        }

        [
            ComImport(),
            Guid("CB2F6723-AB3A-11d2-9C40-00C04FA30A3E")
        ]
        internal class CorRuntimeHost
        {
        }

        [ComImport(),
        Guid("000C0601-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IMsoComponentManager
        {
            /// <summary>
            ///  Return in *ppvObj an implementation of interface iid for service
            ///  guidService (same as IServiceProvider::QueryService).
            ///  Return NOERROR if the requested service is supported, otherwise return
            ///  NULL in *ppvObj and an appropriate error (eg E_FAIL, E_NOINTERFACE).
            /// </summary>
            [PreserveSig]
            int QueryService(
                ref Guid guidService,
                ref Guid iid,
                [MarshalAs(UnmanagedType.Interface)]
            out object ppvObj);

            /// <summary>
            ///  Standard FDebugMessage method.
            ///  Since IMsoComponentManager is a reference counted interface,
            ///  MsoDWGetChkMemCounter should be used when processing the
            ///  msodmWriteBe message.
            /// </summary>
            [PreserveSig]
            bool FDebugMessage(
                IntPtr hInst,
                int msg,
                IntPtr wParam,
                IntPtr lParam);

            /// <summary>
            ///  Register component piComponent and its registration info pcrinfo with
            ///  this component manager.  Return in *pdwComponentID a cookie which will
            ///  identify the component when it calls other IMsoComponentManager
            ///  methods.
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            [PreserveSig]
            bool FRegisterComponent(
                IMsoComponent component,
                NativeMethods.MSOCRINFOSTRUCT pcrinfo,
                out IntPtr dwComponentID);

            /// <summary>
            ///  Undo the registration of the component identified by dwComponentID
            ///  (the cookie returned from the FRegisterComponent method).
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            [PreserveSig]
            bool FRevokeComponent(IntPtr dwComponentID);

            /// <summary>
            ///  Update the registration info of the component identified by
            ///  dwComponentID (the cookie returned from FRegisterComponent) with the
            ///  new registration information pcrinfo.
            ///  Typically this is used to update the idle time registration data, but
            ///  can be used to update other registration data as well.
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            [PreserveSig]
            bool FUpdateComponentRegistration(IntPtr dwComponentID, NativeMethods.MSOCRINFOSTRUCT pcrinfo);

            /// <summary>
            ///  Notify component manager that component identified by dwComponentID
            ///  (cookie returned from FRegisterComponent) has been activated.
            ///  The active component gets the  chance to process messages before they
            ///  are dispatched (via IMsoComponent::FPreTranslateMessage) and typically
            ///  gets first chance at idle time after the host.
            ///  This method fails if another component is already Exclusively Active.
            ///  In this case, FALSE is returned and SetLastError is set to
            ///  msoerrACompIsXActive (comp usually need not take any special action
            ///  in this case).
            ///  Return TRUE if successful.
            /// </summary>
            [PreserveSig]
            bool FOnComponentActivate(IntPtr dwComponentID);

            /// <summary>
            ///  Called to inform component manager that  component identified by
            ///  dwComponentID (cookie returned from FRegisterComponent) wishes
            ///  to perform a tracking operation (such as mouse tracking).
            ///  The component calls this method with fTrack == TRUE to begin the
            ///  tracking operation and with fTrack == FALSE to end the operation.
            ///  During the tracking operation the component manager routes messages
            ///  to the tracking component (via IMsoComponent::FPreTranslateMessage)
            ///  rather than to the active component.  When the tracking operation ends,
            ///  the component manager should resume routing messages to the active
            ///  component.
            ///  Note: component manager should perform no idle time processing during a
            ///    tracking operation other than give the tracking component idle
            ///    time via IMsoComponent::FDoIdle.
            ///  Note: there can only be one tracking component at a time.
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            [PreserveSig]
            bool FSetTrackingComponent(IntPtr dwComponentID, [In, MarshalAs(UnmanagedType.Bool)] bool fTrack);

            /// <summary>
            ///  Notify component manager that component identified by dwComponentID
            ///  (cookie returned from FRegisterComponent) is entering the state
            ///  identified by uStateID (msocstateXXX value).  (For convenience when
            ///  dealing with sub CompMgrs, the host can call this method passing 0 for
            ///  dwComponentID.)
            ///  Component manager should notify all other interested components within
            ///  the state context indicated by uContext (a msoccontextXXX value),
            ///  excluding those within the state context of a CompMgr in rgpicmExclude,
            ///  via IMsoComponent::OnEnterState (see "Comments on State Contexts",
            ///  above).
            ///  Component Manager should also take appropriate action depending on the
            ///  value of uStateID (see msocstate comments, above).
            ///  dwReserved is reserved for future use and should be zero.
            ///
            ///  rgpicmExclude (can be NULL) is an array of cpicmExclude CompMgrs (can
            ///  include root CompMgr and/or sub CompMgrs); components within the state
            ///  context of a CompMgr appearing in this array should NOT be notified of
            ///  the state change (note: if uContext    is msoccontextMine, the only
            ///  CompMgrs in rgpicmExclude that are checked for exclusion are those that
            ///  are sub CompMgrs of this Component Manager, since all other CompMgrs
            ///  are outside of this Component Manager's state context anyway.)
            ///
            ///  Note: Calls to this method are symmetric with calls to
            ///  FOnComponentExitState.
            ///  That is, if n OnComponentEnterState calls are made, the component is
            ///  considered to be in the state until n FOnComponentExitState calls are
            ///  made.  Before revoking its registration a component must make a
            ///  sufficient number of FOnComponentExitState calls to offset any
            ///  outstanding OnComponentEnterState calls it has made.
            ///
            ///  Note: inplace objects should not call this method with
            ///  uStateID == msocstateModal when entering modal state. Such objects
            ///  should call IOleInPlaceFrame::EnableModeless instead.
            /// </summary>
            [PreserveSig]
            void OnComponentEnterState(IntPtr dwComponentID, int uStateID, int uContext, int cpicmExclude,/* IMsoComponentManger** */ int rgpicmExclude, int dwReserved);

            /// <summary>
            ///  Notify component manager that component identified by dwComponentID
            ///  (cookie returned from FRegisterComponent) is exiting the state
            ///  identified by uStateID (a msocstateXXX value).  (For convenience when
            ///  dealing with sub CompMgrs, the host can call this method passing 0 for
            ///  dwComponentID.)
            ///  uContext, cpicmExclude, and rgpicmExclude are as they are in
            ///  OnComponentEnterState.
            ///  Component manager      should notify all appropriate interested components
            ///  (taking into account uContext, cpicmExclude, rgpicmExclude) via
            ///  IMsoComponent::OnEnterState (see "Comments on State Contexts", above).
            ///  Component Manager should also take appropriate action depending on
            ///  the value of uStateID (see msocstate comments, above).
            ///  Return TRUE if, at the end of this call, the state is still in effect
            ///  at the root of this component manager's state context
            ///  (because the host or some other component is still in the state),
            ///  otherwise return FALSE (ie. return what FInState would return).
            ///  Caller can normally ignore the return value.
            ///
            ///  Note: n calls to this method are symmetric with n calls to
            ///  OnComponentEnterState (see OnComponentEnterState comments, above).
            /// </summary>
            [PreserveSig]
            bool FOnComponentExitState(
                IntPtr dwComponentID,
                int uStateID,
                int uContext,
                int cpicmExclude,
                /* IMsoComponentManager** */ int rgpicmExclude);

            /// <summary>
            ///  Return TRUE if the state identified by uStateID (a msocstateXXX value)
            ///  is in effect at the root of this component manager's state context,
            ///  FALSE otherwise (see "Comments on State Contexts", above).
            ///  pvoid is reserved for future use and should be NULL.
            /// </summary>
            [PreserveSig]
            bool FInState(int uStateID,/* PVOID */ IntPtr pvoid);

            /// <summary>
            ///  Called periodically by a component during IMsoComponent::FDoIdle.
            ///  Return TRUE if component can continue its idle time processing,
            ///  FALSE if not (in which case component returns from FDoIdle.)
            /// </summary>
            [PreserveSig]
            bool FContinueIdle();

            /// <summary>
            ///  Component identified by dwComponentID (cookie returned from
            ///  FRegisterComponent) wishes to push a message loop for reason uReason.
            ///  uReason is one the values from the msoloop enumeration (above).
            ///  pvLoopData is data private to the component.
            ///  The component manager should push its message loop,
            ///  calling IMsoComponent::FContinueMessageLoop(uReason, pvLoopData)
            ///  during each loop iteration (see IMsoComponent::FContinueMessageLoop
            ///  comments).  When IMsoComponent::FContinueMessageLoop returns FALSE, the
            ///  component manager terminates the loop.
            ///  Returns TRUE if component manager terminates loop because component
            ///  told it to (by returning FALSE from IMsoComponent::FContinueMessageLoop),
            ///  FALSE if it had to terminate the loop for some other reason.  In the
            ///  latter case, component should perform any necessary action (such as
            ///  cleanup).
            /// </summary>
            [PreserveSig]
            bool FPushMessageLoop(IntPtr dwComponentID, int uReason,/* PVOID */ int pvLoopData);

            /// <summary>
            ///  Cause the component manager to create a "sub" component manager, which
            ///  will be one of its children in the hierarchical tree of component
            ///  managers used to maintiain state contexts (see "Comments on State
            ///  Contexts", above).
            ///  piunkOuter is the controlling unknown (can be NULL), riid is the
            ///  desired IID, and *ppvObj returns       the created sub component manager.
            ///  piunkServProv (can be NULL) is a ptr to an object supporting
            ///  IServiceProvider interface to which the created sub component manager
            ///  will delegate its IMsoComponentManager::QueryService calls.
            ///  (see objext.h or docobj.h for definition of IServiceProvider).
            ///  Returns TRUE if successful.
            /// </summary>
            [PreserveSig]
            bool FCreateSubComponentManager(
                [MarshalAs(UnmanagedType.Interface)]
            object punkOuter,
                [MarshalAs(UnmanagedType.Interface)]
            object punkServProv,
                ref Guid riid,
                out IntPtr ppvObj);

            /// <summary>
            ///  Return in *ppicm an AddRef'ed ptr to this component manager's parent
            ///  in the hierarchical tree of component managers used to maintain state
            ///  contexts (see "Comments on State       Contexts", above).
            ///  Returns TRUE if the parent is returned, FALSE if no parent exists or
            ///  some error occurred.
            /// </summary>
            [PreserveSig]
            bool FGetParentComponentManager(
                out IMsoComponentManager ppicm);

            /// <summary>
            ///  Return in *ppic an AddRef'ed ptr to the current active or tracking
            ///  component (as indicated by dwgac (a msogacXXX value)), and
            ///  its registration information in *pcrinfo.  ppic and/or pcrinfo can be
            ///  NULL if caller is not interested these values.  If pcrinfo is not NULL,
            ///  caller should set pcrinfo->cbSize before calling this method.
            ///  Returns TRUE if the component indicated by dwgac exists, FALSE if no
            ///  such component exists or some error occurred.
            ///  dwReserved is reserved for future use and should be zero.
            /// </summary>
            [PreserveSig]
            bool FGetActiveComponent(
            int dwgac,
                [Out, MarshalAs(UnmanagedType.LPArray)]
            IMsoComponent[] ppic,
                NativeMethods.MSOCRINFOSTRUCT pcrinfo,
                int dwReserved);
        }

        [ComImport(),
        Guid("000C0600-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IMsoComponent
        {
            /// <summary>
            ///  Standard FDebugMessage method.
            ///  Since IMsoComponentManager is a reference counted interface,
            ///  MsoDWGetChkMemCounter should be used when processing the
            ///  msodmWriteBe message.
            /// </summary>
            [PreserveSig]
            bool FDebugMessage(
                IntPtr hInst,
                int msg,
                IntPtr wParam,
                IntPtr lParam);

            /// <summary>
            ///  Give component a chance to process the message pMsg before it is
            ///  translated and dispatched. Component can do TranslateAccelerator
            ///  do IsDialogMessage, modify pMsg, or take some other action.
            ///  Return TRUE if the message is consumed, FALSE otherwise.
            /// </summary>
            [PreserveSig]
            bool FPreTranslateMessage(ref NativeMethods.MSG msg);

            /// <summary>
            ///  Notify component when app enters or exits (as indicated by fEnter)
            ///  the state identified by uStateID (a value from olecstate enumeration).
            ///  Component should take action depending on value of uStateID
            ///  (see olecstate comments, above).
            ///
            ///  Note: If n calls are made with TRUE fEnter, component should consider
            ///  the state to be in effect until n calls are made with FALSE fEnter.
            ///
            ///  Note: Components should be aware that it is possible for this method to
            ///  be called with FALSE fEnter more    times than it was called with TRUE
            ///  fEnter (so, for example, if component is maintaining a state counter
            ///  (incremented when this method is called with TRUE fEnter, decremented
            ///  when called with FALSE fEnter), the counter should not be decremented
            ///  for FALSE fEnter if it is already at zero.)
            /// </summary>
            [PreserveSig]
            void OnEnterState(
                int uStateID,
                bool fEnter);

            /// <summary>
            ///  Notify component when the host application gains or loses activation.
            ///  If fActive is TRUE, the host app is being activated and dwOtherThreadID
            ///  is the ID of the thread owning the window being deactivated.
            ///  If fActive is FALSE, the host app is being deactivated and
            ///  dwOtherThreadID is the ID of the thread owning the window being
            ///  activated.
            ///  Note: this method is not called when both the window being activated
            ///  and the one being deactivated belong to the host app.
            /// </summary>
            [PreserveSig]
            void OnAppActivate(
                bool fActive,
                int dwOtherThreadID);

            /// <summary>
            ///  Notify the active component that it has lost its active status because
            ///  the host or another component has become active.
            /// </summary>
            [PreserveSig]
            void OnLoseActivation();

            /// <summary>
            ///  Notify component when a new object is being activated.
            ///  If pic is non-NULL, then it is the component that is being activated.
            ///  In this case, fSameComponent is TRUE if pic is the same component as
            ///  the callee of this method, and pcrinfo is the reg info of pic.
            ///  If pic is NULL and fHostIsActivating is TRUE, then the host is the
            ///  object being activated, and pchostinfo is its host info.
            ///  If pic is NULL and fHostIsActivating is FALSE, then there is no current
            ///  active object.
            ///
            ///  If pic is being activated and pcrinfo->grf has the
            ///  olecrfExclusiveBorderSpace bit set, component should hide its border
            ///  space tools (toolbars, status bars, etc.);
            ///  component should also do this if host is activating and
            ///  pchostinfo->grfchostf has the olechostfExclusiveBorderSpace bit set.
            ///  In either of these cases, component should unhide its border space
            ///  tools the next time it is activated.
            ///
            ///  if pic is being activated and pcrinfo->grf has the
            ///  olecrfExclusiveActivation bit is set, then pic is being activated in
            ///  "ExclusiveActive" mode.
            ///  Component should retrieve the top frame window that is hosting pic
            ///  (via pic->HwndGetWindow(olecWindowFrameToplevel, 0)).
            ///  If this window is different from component's own top frame window,
            ///  component should disable its windows and do other things it would do
            ///  when receiving OnEnterState(olecstateModal, TRUE) notification.
            ///  Otherwise, if component is top-level,
            ///  it should refuse to have its window activated by appropriately
            ///  processing WM_MOUSEACTIVATE (but see WM_MOUSEACTIVATE NOTE, above).
            ///  Component should remain in one of these states until the
            ///  ExclusiveActive mode ends, indicated by a future call to
            ///  OnActivationChange with ExclusiveActivation bit not set or with NULL
            ///  pcrinfo.
            /// </summary>
            [PreserveSig]
            void OnActivationChange(
                IMsoComponent component,
                bool fSameComponent,
                int pcrinfo,
                bool fHostIsActivating,
                int pchostinfo,
                int dwReserved);

            /// <summary>
            ///  Give component a chance to do idle time tasks.  grfidlef is a group of
            ///  bit flags taken from the enumeration of oleidlef values (above),
            ///  indicating the type of idle tasks to perform.
            ///  Component may periodically call IOleComponentManager::FContinueIdle;
            ///  if this method returns FALSE, component should terminate its idle
            ///  time processing and return.
            ///  Return TRUE if more time is needed to perform the idle time tasks,
            ///  FALSE otherwise.
            ///  Note: If a component reaches a point where it has no idle tasks
            ///  and does not need FDoIdle calls, it should remove its idle task
            ///  registration via IOleComponentManager::FUpdateComponentRegistration.
            ///  Note: If this method is called on while component is performing a
            ///  tracking operation, component should only perform idle time tasks that
            ///  it deems are appropriate to perform during tracking.
            /// </summary>
            [PreserveSig]
            bool FDoIdle(
                int grfidlef);

            /// <summary>
            ///  Called during each iteration of a message loop that the component
            ///  pushed. uReason and pvLoopData are the reason and the component private
            ///  data that were passed to IOleComponentManager::FPushMessageLoop.
            ///  This method is called after peeking the next message in the queue
            ///  (via PeekMessage) but before the message is removed from the queue.
            ///  The peeked message is passed in the pMsgPeeked param (NULL if no
            ///  message is in the queue).  This method may be additionally called when
            ///  the next message has already been removed from the queue, in which case
            ///  pMsgPeeked is passed as NULL.
            ///  Return TRUE if the message loop should continue, FALSE otherwise.
            ///  If FALSE is returned, the component manager terminates the loop without
            ///  removing pMsgPeeked from the queue.
            /// </summary>
            [PreserveSig]
            bool FContinueMessageLoop(
                int uReason,
                int pvLoopData,
                [MarshalAs(UnmanagedType.LPArray)] NativeMethods.MSG[] pMsgPeeked);

            /// <summary>
            ///  Called when component manager wishes to know if the component is in a
            ///  state in which it can terminate.  If fPromptUser is FALSE, component
            ///  should simply return TRUE if it can terminate, FALSE otherwise.
            ///  If fPromptUser is TRUE, component should return TRUE if it can
            ///  terminate without prompting the user; otherwise it should prompt the
            ///  user, either 1.) asking user if it can terminate and returning TRUE
            ///  or FALSE appropriately, or 2.) giving an indication as to why it
            ///  cannot terminate and returning FALSE.
            /// </summary>
            [PreserveSig]
            bool FQueryTerminate(
                bool fPromptUser);

            /// <summary>
            ///  Called when component manager wishes to terminate the component's
            ///  registration.  Component should revoke its registration with component
            ///  manager, release references to component manager and perform any
            ///  necessary cleanup.
            /// </summary>
            [PreserveSig]
            void Terminate();

            /// <summary>
            ///  Called to retrieve a window associated with the component, as specified
            ///  by dwWhich, a olecWindowXXX value (see olecWindow, above).
            ///  dwReserved is reserved for future use and should be zero.
            ///  Component should return the desired window or NULL if no such window
            ///  exists.
            /// </summary>
            [PreserveSig]
            IntPtr HwndGetWindow(
                int dwWhich,
                int dwReserved);
        }

        [ComVisible(true), Guid("8CC497C0-A1DF-11ce-8098-00AA0047BE5D"),
        InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface ITextDocument
        {
            string GetName();
            object GetSelection();
            int GetStoryCount();
            object GetStoryRanges();
            int GetSaved();
            void SetSaved(int value);
            object GetDefaultTabStop();
            void SetDefaultTabStop(object value);
            void New();
            void Open(object pVar, int flags, int codePage);
            void Save(object pVar, int flags, int codePage);
            int Freeze();
            int Unfreeze();
            void BeginEditCollection();
            void EndEditCollection();
            int Undo(int count);
            int Redo(int count);
            [return: MarshalAs(UnmanagedType.Interface)] ITextRange Range(int cp1, int cp2);
            [return: MarshalAs(UnmanagedType.Interface)] ITextRange RangeFromPoint(int x, int y);
        };

        [ComVisible(true), Guid("8CC497C2-A1DF-11ce-8098-00AA0047BE5D"),
        InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface ITextRange
        {
            string GetText();
            void SetText(string text);
            object GetChar();
            void SetChar(object ch);
            [return: MarshalAs(UnmanagedType.Interface)] ITextRange GetDuplicate();
            [return: MarshalAs(UnmanagedType.Interface)] ITextRange GetFormattedText();
            void SetFormattedText([In, MarshalAs(UnmanagedType.Interface)] ITextRange range);
            int GetStart();
            void SetStart(int cpFirst);
            int GetEnd();
            void SetEnd(int cpLim);
            object GetFont();
            void SetFont(object font);
            object GetPara();
            void SetPara(object para);
            int GetStoryLength();
            int GetStoryType();
            void Collapse(int start);
            int Expand(int unit);
            int GetIndex(int unit);
            void SetIndex(int unit, int index, int extend);
            void SetRange(int cpActive, int cpOther);
            int InRange([In, MarshalAs(UnmanagedType.Interface)] ITextRange range);
            int InStory([In, MarshalAs(UnmanagedType.Interface)] ITextRange range);
            int IsEqual([In, MarshalAs(UnmanagedType.Interface)] ITextRange range);
            void Select();
            int StartOf(int unit, int extend);
            int EndOf(int unit, int extend);
            int Move(int unit, int count);
            int MoveStart(int unit, int count);
            int MoveEnd(int unit, int count);
            int MoveWhile(object cset, int count);
            int MoveStartWhile(object cset, int count);
            int MoveEndWhile(object cset, int count);
            int MoveUntil(object cset, int count);
            int MoveStartUntil(object cset, int count);
            int MoveEndUntil(object cset, int count);
            int FindText(string text, int cch, int flags);
            int FindTextStart(string text, int cch, int flags);
            int FindTextEnd(string text, int cch, int flags);
            int Delete(int unit, int count);
            void Cut([Out] out object pVar);
            void Copy([Out] out object pVar);
            void Paste(object pVar, int format);
            int CanPaste(object pVar, int format);
            int CanEdit();
            void ChangeCase(int type);
            void GetPoint(int type, [Out] out int x, [Out] out int y);
            void SetPoint(int x, int y, int type, int extend);
            void ScrollIntoView(int value);
            object GetEmbeddedObject();
        };

        [ComImport(), Guid("00000115-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceUIWindow
        {
            IntPtr GetWindow();

            [PreserveSig]
            int ContextSensitiveHelp(
                    int fEnterMode);

            [PreserveSig]
            int GetBorder(
                   [Out]
                      NativeMethods.COMRECT lprectBorder);

            [PreserveSig]
            int RequestBorderSpace(
                   [In]
                      NativeMethods.COMRECT pborderwidths);

            [PreserveSig]
            int SetBorderSpace(
                   [In]
                      NativeMethods.COMRECT pborderwidths);

            void SetActiveObject(
                   [In, MarshalAs(UnmanagedType.Interface)]
                      IOleInPlaceActiveObject pActiveObject,
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string pszObjName);
        }

        [ComImport(),
        Guid("00000117-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceActiveObject
        {
            [PreserveSig]
            int GetWindow(out IntPtr hwnd);

            void ContextSensitiveHelp(
                    int fEnterMode);

            [PreserveSig]
            int TranslateAccelerator(
                   [In]
                      ref NativeMethods.MSG lpmsg);

            void OnFrameWindowActivate(
                    bool fActivate);

            void OnDocWindowActivate(
                    int fActivate);

            void ResizeBorder(
                   [In]
                      NativeMethods.COMRECT prcBorder,
                   [In]
                      IOleInPlaceUIWindow pUIWindow,
                    bool fFrameWindow);

            void EnableModeless(
                    int fEnable);
        }

        [ComImport(), Guid("00000114-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleWindow
        {
            [PreserveSig]
            int GetWindow([Out]out IntPtr hwnd);

            void ContextSensitiveHelp(
                    int fEnterMode);
        }

        [ComImport(),
        Guid("00000113-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceObject
        {
            [PreserveSig]
            int GetWindow([Out]out IntPtr hwnd);

            void ContextSensitiveHelp(
                    int fEnterMode);

            void InPlaceDeactivate();

            [PreserveSig]
            int UIDeactivate();

            void SetObjectRects(
                   [In]
                      NativeMethods.COMRECT lprcPosRect,
                   [In]
                      NativeMethods.COMRECT lprcClipRect);

            void ReactivateAndUndo();
        }

        [ComImport()]
        [Guid("00000112-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleObject
        {
            [PreserveSig]
            int SetClientSite(
                   [In, MarshalAs(UnmanagedType.Interface)]
                      IOleClientSite pClientSite);

            IOleClientSite GetClientSite();

            [PreserveSig]
            int SetHostNames(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string szContainerApp,
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string szContainerObj);

            [PreserveSig]
            int Close(
                    int dwSaveOption);

            [PreserveSig]
            int SetMoniker(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwWhichMoniker,
                   [In, MarshalAs(UnmanagedType.Interface)]
                     object pmk);

            [PreserveSig]
            int GetMoniker(
                  [In, MarshalAs(UnmanagedType.U4)]
                     int dwAssign,
                  [In, MarshalAs(UnmanagedType.U4)]
                     int dwWhichMoniker,
                  [Out, MarshalAs(UnmanagedType.Interface)]
                     out object moniker);

            [PreserveSig]
            int InitFromData(
                   [In, MarshalAs(UnmanagedType.Interface)]
                     IComDataObject pDataObject,
                    int fCreation,
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwReserved);

            [PreserveSig]
            int GetClipboardData(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwReserved,
                    out IComDataObject data);

            [PreserveSig]
            int DoVerb(
                    int iVerb,
                   [In]
                     IntPtr lpmsg,
                   [In, MarshalAs(UnmanagedType.Interface)]
                      IOleClientSite pActiveSite,
                    int lindex,
                    IntPtr hwndParent,
                   [In]
                     NativeMethods.COMRECT lprcPosRect);

            [PreserveSig]
            int EnumVerbs(out IEnumOLEVERB e);

            [PreserveSig]
            int OleUpdate();

            [PreserveSig]
            int IsUpToDate();

            [PreserveSig]
            int GetUserClassID(
                   [In, Out]
                      ref Guid pClsid);

            [PreserveSig]
            int GetUserType(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwFormOfType,
                   [Out, MarshalAs(UnmanagedType.LPWStr)]
                     out string userType);

            [PreserveSig]
            Interop.HRESULT SetExtent(uint dwDrawAspect, Size* pSizel);

            [PreserveSig]
            Interop.HRESULT GetExtent(uint dwDrawAspect, Size* pSizel);

            [PreserveSig]
            int Advise(
                    IAdviseSink pAdvSink,
                    out int cookie);

            [PreserveSig]
            int Unadvise(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwConnection);

            [PreserveSig]
            int EnumAdvise(out IEnumSTATDATA e);

            [PreserveSig]
            int GetMiscStatus(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwAspect,
                    out int misc);

            [PreserveSig]
            int SetColorScheme(
                   [In]
                      NativeMethods.tagLOGPALETTE pLogpal);
        }

        [ComImport]
        [Guid("1C2056CC-5EF4-101B-8BC8-00AA003E3B29")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceObjectWindowless
        {
            [PreserveSig]
            int SetClientSite(
                   [In, MarshalAs(UnmanagedType.Interface)]
                      IOleClientSite pClientSite);

            [PreserveSig]
            int GetClientSite(out IOleClientSite site);

            [PreserveSig]
            int SetHostNames(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string szContainerApp,
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string szContainerObj);

            [PreserveSig]
            int Close(
                    int dwSaveOption);

            [PreserveSig]
            int SetMoniker(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwWhichMoniker,
                   [In, MarshalAs(UnmanagedType.Interface)]
                     object pmk);

            [PreserveSig]
            int GetMoniker(
                  [In, MarshalAs(UnmanagedType.U4)]
                     int dwAssign,
                  [In, MarshalAs(UnmanagedType.U4)]
                     int dwWhichMoniker,
                  [Out, MarshalAs(UnmanagedType.Interface)]
                     out object moniker);

            [PreserveSig]
            int InitFromData(
                   [In, MarshalAs(UnmanagedType.Interface)]
                     IComDataObject pDataObject,
                    int fCreation,
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwReserved);

            [PreserveSig]
            int GetClipboardData(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwReserved,
                    out IComDataObject data);

            [PreserveSig]
            int DoVerb(
                    int iVerb,
                   [In]
                     IntPtr lpmsg,
                   [In, MarshalAs(UnmanagedType.Interface)]
                      IOleClientSite pActiveSite,
                    int lindex,
                    IntPtr hwndParent,
                   [In]
                     NativeMethods.COMRECT lprcPosRect);

            [PreserveSig]
            int EnumVerbs(out IEnumOLEVERB e);

            [PreserveSig]
            int OleUpdate();

            [PreserveSig]
            int IsUpToDate();

            [PreserveSig]
            int GetUserClassID(
                   [In, Out]
                      ref Guid pClsid);

            [PreserveSig]
            int GetUserType(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwFormOfType,
                   [Out, MarshalAs(UnmanagedType.LPWStr)]
                     out string userType);

            [PreserveSig]
            Interop.HRESULT SetExtent(uint dwDrawAspect, Size* pSizel);

            [PreserveSig]
            Interop.HRESULT GetExtent(uint dwDrawAspect, Size* pSizel);

            [PreserveSig]
            int Advise(
                   [In, MarshalAs(UnmanagedType.Interface)]
                     IAdviseSink pAdvSink,
                    out int cookie);

            [PreserveSig]
            int Unadvise(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwConnection);

            [PreserveSig]
            int EnumAdvise(out IEnumSTATDATA e);

            [PreserveSig]
            int GetMiscStatus(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int dwAspect,
                    out int misc);

            [PreserveSig]
            int SetColorScheme(
                   [In]
                      NativeMethods.tagLOGPALETTE pLogpal);

            [PreserveSig]
            int OnWindowMessage(
               [In, MarshalAs(UnmanagedType.U4)]  int msg,
               [In, MarshalAs(UnmanagedType.U4)]  int wParam,
               [In, MarshalAs(UnmanagedType.U4)]  int lParam,
               [Out, MarshalAs(UnmanagedType.U4)] int plResult);

            [PreserveSig]
            int GetDropTarget(
               [Out, MarshalAs(UnmanagedType.Interface)] object ppDropTarget);
        };

        [ComImport(),
        Guid("B196B288-BAB4-101A-B69C-00AA00341D07"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleControl
        {
            [PreserveSig]
            int GetControlInfo(
                   [Out]
                      NativeMethods.tagCONTROLINFO pCI);

            [PreserveSig]
            int OnMnemonic(
                   [In]
                      ref NativeMethods.MSG pMsg);

            [PreserveSig]
            int OnAmbientPropertyChange(
                    int dispID);

            [PreserveSig]
            int FreezeEvents(
                    int bFreeze);
        }

        [ComImport(), Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService(
                 [In]
                      ref Guid guidService,
                 [In]
                  ref Guid riid,
                 out IntPtr ppvObject);
        }

        [ComImport(), Guid("0000010d-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IViewObject
        {
            [PreserveSig]
            int Draw(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [In]
                NativeMethods.tagDVTARGETDEVICE ptd,
                IntPtr hdcTargetDev,
                IntPtr hdcDraw,
                [In]
                NativeMethods.COMRECT lprcBounds,
                [In]
                NativeMethods.COMRECT lprcWBounds,
                IntPtr pfnContinue,
                [In]
                int dwContinue);

            [PreserveSig]
            int GetColorSet(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [In]
                NativeMethods.tagDVTARGETDEVICE ptd,
                IntPtr hicTargetDev,
                [Out]
                NativeMethods.tagLOGPALETTE ppColorSet);

            [PreserveSig]
            int Freeze(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [Out]
                IntPtr pdwFreeze);

            [PreserveSig]
            int Unfreeze(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwFreeze);

            void SetAdvise(
                [In, MarshalAs(UnmanagedType.U4)]
                int aspects,
                [In, MarshalAs(UnmanagedType.U4)]
                int advf,
                [In, MarshalAs(UnmanagedType.Interface)]
                IAdviseSink pAdvSink);

            void GetAdvise(
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                int[] paspects,
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                int[] advf,
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                IAdviseSink[] pAdvSink);
        }

        [ComImport]
        [Guid("00000127-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IViewObject2 /* : IViewObject */
        {
            void Draw(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [In]
                NativeMethods.tagDVTARGETDEVICE ptd,
                IntPtr hdcTargetDev,
                IntPtr hdcDraw,
                [In]
                NativeMethods.COMRECT lprcBounds,
                [In]
                NativeMethods.COMRECT lprcWBounds,
                IntPtr pfnContinue,
                [In]
                int dwContinue);

            [PreserveSig]
            int GetColorSet(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [In]
                NativeMethods.tagDVTARGETDEVICE ptd,
                IntPtr hicTargetDev,
                [Out]
                NativeMethods.tagLOGPALETTE ppColorSet);

            [PreserveSig]
            int Freeze(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [Out]
                IntPtr pdwFreeze);

            [PreserveSig]
            int Unfreeze(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwFreeze);

            void SetAdvise(
                [In, MarshalAs(UnmanagedType.U4)]
                int aspects,
                [In, MarshalAs(UnmanagedType.U4)]
                int advf,
                [In, MarshalAs(UnmanagedType.Interface)]
                IAdviseSink pAdvSink);

            void GetAdvise(
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                int[] paspects,
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                int[] advf,
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                IAdviseSink[] pAdvSink);

            [PreserveSig]
            Interop.HRESULT GetExtent(
                uint dwDrawAspect,
                int lindex,
                NativeMethods.tagDVTARGETDEVICE ptd,
                Size *lpsizel);
        }

        [ComImport(), Guid("0000010C-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersist
        {
            void GetClassID(
                           [Out]
                           out Guid pClassID);
        }

        [ComImport]
        [Guid("37D84F60-42CB-11CE-8135-00AA004BB851")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersistPropertyBag /* : IPersist */
        {
            void GetClassID(out Guid pClassID);

            void InitNew();

            void Load(IPropertyBag pPropBag, IErrorLog pErrorLog);

            void Save(IPropertyBag pPropBag, Interop.BOOL fClearDirty, Interop.BOOL fSaveAllProperties);
        }

        [ComImport()]
        [Guid("CF51ED10-62FE-11CF-BF86-00A0C9034836")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IQuickActivate
        {
            void QuickActivate(
                              tagQACONTAINER pQaContainer,
                              [Out]
                              tagQACONTROL pQaControl);

            [PreserveSig]
            Interop.HRESULT SetContentExtent(Size* pSizel);

            [PreserveSig]
            Interop.HRESULT GetContentExtent(Size* pSizel);
        }

        [ComImport(), Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IPropertyBag
        {
            [PreserveSig]
            int Read(
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string pszPropName,
                [In, Out]
                ref object pVar,
                [In]
                IErrorLog pErrorLog);

            [PreserveSig]
            int Write(
                [In, MarshalAs(UnmanagedType.LPWStr)]
                string pszPropName,
                [In]
                ref object pVar);
        }

        [ComImport(), Guid("3127CA40-446E-11CE-8135-00AA004BB851"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IErrorLog
        {
            void AddError(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                             string pszPropName_p0,
                   [In, MarshalAs(UnmanagedType.Struct)]
                              NativeMethods.tagEXCEPINFO pExcepInfo_p1);
        }

        [ComImport(),
        Guid("B196B286-BAB4-101A-B69C-00AA00341D07"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IConnectionPoint
        {
            [PreserveSig]
            int GetConnectionInterface(out Guid iid);

            [PreserveSig]
            int GetConnectionPointContainer(
                [MarshalAs(UnmanagedType.Interface)]
            ref IConnectionPointContainer pContainer);

            [PreserveSig]
            int Advise(
                   [In, MarshalAs(UnmanagedType.Interface)]
                  object pUnkSink,
                 ref int cookie);

            [PreserveSig]
            int Unadvise(
                     int cookie);

            [PreserveSig]
            int EnumConnections(out object pEnum);
        }

        [ComImport(), Guid("00020404-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumVariant
        {
            [PreserveSig]
            int Next(
                   [In, MarshalAs(UnmanagedType.U4)]
                 int celt,
                   [In, Out]
                 IntPtr rgvar,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 int[] pceltFetched);

            void Skip(
                   [In, MarshalAs(UnmanagedType.U4)]
                 int celt);

            void Reset();

            void Clone(
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   IEnumVariant[] ppenum);
        }

        [ComImport(), Guid("00000104-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumOLEVERB
        {
            [PreserveSig]
            int Next(
                   [MarshalAs(UnmanagedType.U4)]
                int celt,
                   [Out]
                NativeMethods.tagOLEVERB rgelt,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                int[] pceltFetched);

            [PreserveSig]
            int Skip(
                   [In, MarshalAs(UnmanagedType.U4)]
                 int celt);

            void Reset();

            void Clone(
               out IEnumOLEVERB ppenum);
        }

        [ComImport(), Guid("EAC04BC0-3791-11d2-BB95-0060977B464C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAutoComplete2
        {
            int Init(
                    [In] HandleRef hwndEdit,          // hwnd of editbox or editbox deriviative.
                    [In] IEnumString punkACL,          // Pointer to object containing string to complete from. (IEnumString *)
                    [In] string pwszRegKeyPath,       //
                    [In] string pwszQuickComplete
                    );

            void Enable([In] bool fEnable);            // Is it enabled?

            int SetOptions([In] int dwFlag);

            void GetOptions([Out] IntPtr pdwFlag);
        }

        public abstract class CharBuffer
        {
            public static CharBuffer CreateBuffer(int size)
            {
                return new UnicodeCharBuffer(size);
            }

            public abstract IntPtr AllocCoTaskMem();
            public abstract string GetString();
            public abstract void PutCoTaskMem(IntPtr ptr);
            public abstract void PutString(string s);
        }

        public class UnicodeCharBuffer : CharBuffer
        {
            internal char[] buffer;
            internal int offset;

            public UnicodeCharBuffer(int size)
            {
                buffer = new char[size];
            }

            public override IntPtr AllocCoTaskMem()
            {
                IntPtr result = Marshal.AllocCoTaskMem(buffer.Length * 2);
                Marshal.Copy(buffer, 0, result, buffer.Length);
                return result;
            }

            public override string GetString()
            {
                int i = offset;
                while (i < buffer.Length && buffer[i] != 0)
                {
                    i++;
                }

                string result = new string(buffer, offset, i - offset);
                if (i < buffer.Length)
                {
                    i++;
                }

                offset = i;
                return result;
            }

            public override void PutCoTaskMem(IntPtr ptr)
            {
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
                offset = 0;
            }

            public override void PutString(string s)
            {
                int count = Math.Min(s.Length, buffer.Length - offset);
                s.CopyTo(0, buffer, offset, count);
                offset += count;
                if (offset < buffer.Length)
                {
                    buffer[offset++] = (char)0;
                }
            }
        }

        [ComImport(), Guid("B196B28F-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IClassFactory2
        {
            void CreateInstance(
                   [In, MarshalAs(UnmanagedType.Interface)]
                  object unused,
                           [In]
                  ref Guid refiid,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  object[] ppunk);

            void LockServer(
                    int fLock);

            void GetLicInfo(
                   [Out]
                  NativeMethods.tagLICINFO licInfo);

            void RequestLicKey(
                   [In, MarshalAs(UnmanagedType.U4)]
                 int dwReserved,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   string[] pBstrKey);

            void CreateInstanceLic(
                   [In, MarshalAs(UnmanagedType.Interface)]
                  object pUnkOuter,
                   [In, MarshalAs(UnmanagedType.Interface)]
                  object pUnkReserved,
                           [In]
                  ref Guid riid,
                   [In, MarshalAs(UnmanagedType.BStr)]
                  string bstrKey,
                   [Out, MarshalAs(UnmanagedType.Interface)]
                  out object ppVal);
        }

        [ComImport(),
        Guid("B196B284-BAB4-101A-B69C-00AA00341D07"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IConnectionPointContainer
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object EnumConnectionPoints();

            [PreserveSig]
            int FindConnectionPoint([In] ref Guid guid, [Out, MarshalAs(UnmanagedType.Interface)]out IConnectionPoint ppCP);
        }

        [ComImport(), Guid("B196B285-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumConnectionPoints
        {
            [PreserveSig]
            int Next(int cConnections, out IConnectionPoint pCp, out int pcFetched);

            [PreserveSig]
            int Skip(int cSkip);

            void Reset();

            IEnumConnectionPoints Clone();
        }

        [ComImport(), Guid("00020400-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDispatch
        {
            int GetTypeInfoCount();

            [return: MarshalAs(UnmanagedType.Interface)]
            ITypeInfo GetTypeInfo(
                    [In, MarshalAs(UnmanagedType.U4)]
                 int iTInfo,
                    [In, MarshalAs(UnmanagedType.U4)]
                 int lcid);

            [PreserveSig]
            int GetIDsOfNames(
                   [In]
                 ref Guid riid,
                   [In, MarshalAs(UnmanagedType.LPArray)]
                 string[] rgszNames,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int cNames,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int lcid,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 int[] rgDispId);

            [PreserveSig]
            int Invoke(
                    int dispIdMember,
                   [In]
                 ref Guid riid,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int lcid,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int dwFlags,
                   [Out, In]
                  NativeMethods.tagDISPPARAMS pDispParams,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  object[] pVarResult,
                   [Out, In]
                  NativeMethods.tagEXCEPINFO pExcepInfo,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  IntPtr [] pArgErr);
        }

        [ComImport(), Guid("00020401-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITypeInfo
        {
            [PreserveSig]
            int GetTypeAttr(ref IntPtr pTypeAttr);

            [PreserveSig]
            int GetTypeComp(
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                       ITypeComp[] ppTComp);

            [PreserveSig]
            int GetFuncDesc(
                    [In, MarshalAs(UnmanagedType.U4)]
                     int index, ref IntPtr pFuncDesc);

            [PreserveSig]
            int GetVarDesc(
                   [In, MarshalAs(UnmanagedType.U4)]
                     int index, ref IntPtr pVarDesc);

            [PreserveSig]
            int GetNames(
                    int memid,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                      string[] rgBstrNames,
                   [In, MarshalAs(UnmanagedType.U4)]
                     int cMaxNames,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                      int[] pcNames);

            [PreserveSig]
            int GetRefTypeOfImplType(
                    [In, MarshalAs(UnmanagedType.U4)]
                     int index,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      int[] pRefType);

            [PreserveSig]
            int GetImplTypeFlags(
                    [In, MarshalAs(UnmanagedType.U4)]
                     int index,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      int[] pImplTypeFlags);

            [PreserveSig]
            int GetIDsOfNames(IntPtr rgszNames, int cNames, IntPtr pMemId);

            [PreserveSig]
            int Invoke();

            [PreserveSig]
            int GetDocumentation(
                     int memid,
                      ref string pBstrName,
                      ref string pBstrDocString,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      int[] pdwHelpContext,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      string[] pBstrHelpFile);

            [PreserveSig]
            int GetDllEntry(
                     int memid,
                      NativeMethods.tagINVOKEKIND invkind,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      string[] pBstrDllName,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      string[] pBstrName,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      short[] pwOrdinal);

            [PreserveSig]
            int GetRefTypeInfo(
                    IntPtr hreftype,
                    ref ITypeInfo pTypeInfo);

            [PreserveSig]
            int AddressOfMember();

            [PreserveSig]
            int CreateInstance(
                    [In]
                      ref Guid riid,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      object[] ppvObj);

            [PreserveSig]
            int GetMops(
                    int memid,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                     string[] pBstrMops);

            [PreserveSig]
            int GetContainingTypeLib(
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                       ITypeLib[] ppTLib,
                    [Out, MarshalAs(UnmanagedType.LPArray)]
                      int[] pIndex);

            [PreserveSig]
            void ReleaseTypeAttr(IntPtr typeAttr);

            [PreserveSig]
            void ReleaseFuncDesc(IntPtr funcDesc);

            [PreserveSig]
            void ReleaseVarDesc(IntPtr varDesc);
        }

        [ComImport(), Guid("00020403-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITypeComp
        {
            unsafe void RemoteBind(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                 string szName,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int lHashVal,
                   [In, MarshalAs(UnmanagedType.U2)]
                 short wFlags,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   ITypeInfo[] ppTInfo,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  NativeMethods.tagDESCKIND[] pDescKind,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   NativeMethods.tagFUNCDESC*[] ppFuncDesc,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   NativeMethods.tagVARDESC*[] ppVarDesc,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   ITypeComp[] ppTypeComp,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  int[] pDummy);

            void RemoteBindType(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                 string szName,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int lHashVal,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   ITypeInfo[] ppTInfo);
        }

        [ComImport(), Guid("00020402-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITypeLib
        {
            void RemoteGetTypeInfoCount(
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  int[] pcTInfo);

            void GetTypeInfo(
                   [In, MarshalAs(UnmanagedType.U4)]
                 int index,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   ITypeInfo[] ppTInfo);

            void GetTypeInfoType(
                   [In, MarshalAs(UnmanagedType.U4)]
                 int index,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  NativeMethods.tagTYPEKIND[] pTKind);

            void GetTypeInfoOfGuid(
                   [In]
                  ref Guid guid,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   ITypeInfo[] ppTInfo);

            void RemoteGetLibAttr(
                   IntPtr ppTLibAttr,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                  int[] pDummy);

            void GetTypeComp(
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                   ITypeComp[] ppTComp);

            void RemoteGetDocumentation(
                    int index,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int refPtrFlags,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 string[] pBstrName,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 string[] pBstrDocString,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 int[] pdwHelpContext,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 string[] pBstrHelpFile);

            void RemoteIsName(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                 string szNameBuf,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int lHashVal,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 IntPtr [] pfName,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 string[] pBstrLibName);

            void RemoteFindName(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                 string szNameBuf,
                   [In, MarshalAs(UnmanagedType.U4)]
                 int lHashVal,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 ITypeInfo[] ppTInfo,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 int[] rgMemId,
                   [In, Out, MarshalAs(UnmanagedType.LPArray)]
                 short[] pcFound,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                 string[] pBstrLibName);

            void LocalReleaseTLibAttr();
        }

        [ComImport(),
         Guid("DF0B3D60-548F-101B-8E65-08002B2BD119"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISupportErrorInfo
        {
            int InterfaceSupportsErrorInfo(
                    [In] ref Guid riid);
        }

        [ComImport(),
         Guid("1CF2B120-547D-101B-8E65-08002B2BD119"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IErrorInfo
        {
            [PreserveSig]
            int GetGUID(
                       [Out]
                   out Guid pguid);

            [PreserveSig]
            int GetSource(
                         [In, Out, MarshalAs(UnmanagedType.BStr)]
                     ref string pBstrSource);

            [PreserveSig]
            int GetDescription(
                              [In, Out, MarshalAs(UnmanagedType.BStr)]
                          ref string pBstrDescription);

            [PreserveSig]
            int GetHelpFile(
                           [In, Out, MarshalAs(UnmanagedType.BStr)]
                       ref string pBstrHelpFile);

            [PreserveSig]
            int GetHelpContext(
                              [In, Out, MarshalAs(UnmanagedType.U4)]
                          ref int pdwHelpContext);
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagQACONTAINER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize = Marshal.SizeOf<tagQACONTAINER>();

            public IOleClientSite pClientSite;

            [MarshalAs(UnmanagedType.Interface)]
            public object pAdviseSink = null;

            public IPropertyNotifySink pPropertyNotifySink;

            [MarshalAs(UnmanagedType.Interface)]
            public object pUnkEventSink = null;

            [MarshalAs(UnmanagedType.U4)]
            public int dwAmbientFlags;

            [MarshalAs(UnmanagedType.U4)]
            public uint colorFore;

            [MarshalAs(UnmanagedType.U4)]
            public uint colorBack;

            [MarshalAs(UnmanagedType.Interface)]
            public object pFont;

            [MarshalAs(UnmanagedType.Interface)]
            public object pUndoMgr = null;

            [MarshalAs(UnmanagedType.U4)]
            public int dwAppearance;

            public int lcid;

            public IntPtr hpal = IntPtr.Zero;

            [MarshalAs(UnmanagedType.Interface)]
            public object pBindHost = null;

            // visual basic6 uses a old version of the struct that is missing these two fields.
            // So, ActiveX sourcing does not work, with the EE trying to read off the
            // end of the stack to get to these variables. If I do not define these,
            // Office or any of the other hosts will hopefully get nulls, otherwise they
            // will crash.
            //
            //public UnsafeNativeMethods.IOleControlSite pControlSite;

            //public UnsafeNativeMethods.IOleServiceProvider pServiceProvider;
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagQACONTROL
        {
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=0, cbSize)*/]
            public int cbSize = Marshal.SizeOf<tagQACONTROL>();

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=4, dwMiscStatus)*/]
            public int dwMiscStatus = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=8, dwViewStatus)*/]
            public int dwViewStatus = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=12, dwEventCookie)*/]
            public int dwEventCookie = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=16, dwPropNotifyCookie)*/]
            public int dwPropNotifyCookie = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=20, dwPointerActivationPolicy)*/]
            public int dwPointerActivationPolicy = 0;
        }

        [ComImport()]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("E44C3566-915D-4070-99C6-047BFF5A08F5")]
        [ComVisible(true)]
        public interface ILegacyIAccessibleProvider
        {
            void Select(int flagsSelect);

            void DoDefaultAction();

            void SetValue([MarshalAs(UnmanagedType.LPWStr)] string szValue);

            [return: MarshalAs(UnmanagedType.Interface)]
            IAccessible GetIAccessible();

            int ChildId { get; }

            string Name { get; }

            string Value { get; }

            string Description { get; }

            uint Role { get; }

            uint State { get; }

            string Help { get; }

            string KeyboardShortcut { get; }

            object[] /* IRawElementProviderSimple[] */ GetSelection();

            string DefaultAction { get; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class OFNOTIFY
        {
            // hdr was a by-value NMHDR structure
            public IntPtr hdr_hwndFrom = IntPtr.Zero;
            public IntPtr hdr_idFrom = IntPtr.Zero;
            public int hdr_code = 0;

            public IntPtr lpOFN = IntPtr.Zero;
            public IntPtr pszFile = IntPtr.Zero;
        }

        internal class Shell32
        {
            [DllImport(ExternDll.Shell32, PreserveSig = true)]
            public static extern int SHCreateShellItem(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out FileDialogNative.IShellItem ppsi);

            [DllImport(ExternDll.Shell32, PreserveSig = true)]
            public static extern int SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)]string pszPath, out IntPtr ppIdl, ref uint rgflnOut);
        }

        [ComVisible(true), Guid("B722BCC6-4E68-101B-A2BC-00AA00404770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleDocumentView
        {
            void SetInPlaceSite(
                 [In, MarshalAs(UnmanagedType.Interface)]
                    IOleInPlaceSite pIPSite);

            [return: MarshalAs(UnmanagedType.Interface)]
            IOleInPlaceSite GetInPlaceSite();

            [return: MarshalAs(UnmanagedType.Interface)]
            object GetDocument();

            void SetRect(
                 [In]
                    ref Interop.RECT prcView);

            void GetRect(
                 [In, Out]
                    ref Interop.RECT prcView);

            void SetRectComplex(
                 [In]
                    Interop.RECT prcView,
                 [In]
                    Interop.RECT prcHScroll,
                 [In]
                    Interop.RECT prcVScroll,
                 [In]
                    Interop.RECT prcSizeBox);

            void Show(bool fShow);

            [PreserveSig]
            int UIActivate(bool fUIActivate);

            void Open();

            [PreserveSig]
            int Close(
                 [In, MarshalAs(UnmanagedType.U4)]
                    int dwReserved);

            void SaveViewState(
                 [In, MarshalAs(UnmanagedType.Interface)]
                    IStream pstm);

            void ApplyViewState(
                 [In, MarshalAs(UnmanagedType.Interface)]
                    IStream pstm);

            void Clone(
                 [In, MarshalAs(UnmanagedType.Interface)]
                    IOleInPlaceSite pIPSiteNew,
                 [Out, MarshalAs(UnmanagedType.LPArray)]
                    IOleDocumentView[] ppViewNew);
        }

        [ComImport,
         TypeLibType(0x1050),
         Guid("618736E0-3C3D-11CF-810C-00AA00389B71"),
        ]
        public interface IAccessibleInternal
        {
            [return: MarshalAs(UnmanagedType.IDispatch)]
            [DispId(unchecked((int)0xFFFFEC78))]
            [TypeLibFunc(0x0040)]
            object get_accParent();

            [DispId(unchecked((int)0xFFFFEC77))]
            [TypeLibFunc(0x0040)]
            int get_accChildCount();

            [return: MarshalAs(UnmanagedType.IDispatch)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC76))]
            object get_accChild([In][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.BStr)]
            [DispId(unchecked((int)0xFFFFEC75))]
            [TypeLibFunc(0x0040)]
            string get_accName([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.BStr)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC74))]
            string get_accValue([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.BStr)]
            [DispId(unchecked((int)0xFFFFEC73))]
            [TypeLibFunc(0x0040)]
            string get_accDescription([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [DispId(unchecked((int)0xFFFFEC72))]
            [TypeLibFunc(0x0040)]
            object get_accRole([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC71))]
            object get_accState([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.BStr)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC70))]
            string get_accHelp([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC6F))]
            [TypeLibFunc(0x0040)]
            int get_accHelpTopic([Out][MarshalAs(UnmanagedType.BStr)] out string pszHelpFile,
                                                        [In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.BStr)]
            [DispId(unchecked((int)0xFFFFEC6E))]
            [TypeLibFunc(0x0040)]
            string get_accKeyboardShortcut([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [DispId(unchecked((int)0xFFFFEC6D))]
            [TypeLibFunc(0x0040)]
            object get_accFocus();

            [return: MarshalAs(UnmanagedType.Struct)]
            [DispId(unchecked((int)0xFFFFEC6C))]
            [TypeLibFunc(0x0040)]
            object get_accSelection();

            [return: MarshalAs(UnmanagedType.BStr)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC6B))]
            string get_accDefaultAction([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC6A))]
            [TypeLibFunc(0x0040)]
            void accSelect([In] int flagsSelect,
                           [In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC69))]
            [TypeLibFunc(0x0040)]
            void accLocation([Out] out int pxLeft,
                             [Out] out int pyTop,
                             [Out] out int pcxWidth,
                             [Out] out int pcyHeight,
                             [In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC68))]
            object accNavigate([In] int navDir,
                               [In][Optional][MarshalAs(UnmanagedType.Struct)] object varStart);

            [return: MarshalAs(UnmanagedType.Struct)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC67))]
            object accHitTest([In] int xLeft,
                              [In] int yTop);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC66))]
            void accDoDefaultAction([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC75))]
            void set_accName([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild,
                                 [In][MarshalAs(UnmanagedType.BStr)] string pszName);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC74))]
            void set_accValue([In][Optional][MarshalAs(UnmanagedType.Struct)] object varChild,
                              [In][MarshalAs(UnmanagedType.BStr)] string pszValue);
        }

        [
        ComImport(),
        Guid("BEF6E002-A874-101A-8BBA-00AA00300CAB"),
        InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFont
        {
            [return: MarshalAs(UnmanagedType.BStr)]
            string GetName();

            void SetName(
                   [In, MarshalAs(UnmanagedType.BStr)]
                      string pname);

            [return: MarshalAs(UnmanagedType.U8)]
            long GetSize();

            void SetSize(
                   [In, MarshalAs(UnmanagedType.U8)]
                     long psize);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetBold();

            void SetBold(
                   [In, MarshalAs(UnmanagedType.Bool)]
                     bool pbold);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetItalic();

            void SetItalic(
                   [In, MarshalAs(UnmanagedType.Bool)]
                     bool pitalic);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetUnderline();

            void SetUnderline(
                   [In, MarshalAs(UnmanagedType.Bool)]
                     bool punderline);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetStrikethrough();

            void SetStrikethrough(
                   [In, MarshalAs(UnmanagedType.Bool)]
                     bool pstrikethrough);

            [return: MarshalAs(UnmanagedType.I2)]
            short GetWeight();

            void SetWeight(
                   [In, MarshalAs(UnmanagedType.I2)]
                     short pweight);

            [return: MarshalAs(UnmanagedType.I2)]
            short GetCharset();

            void SetCharset(
                   [In, MarshalAs(UnmanagedType.I2)]
                     short pcharset);

            IntPtr GetHFont();

            void Clone(
                      out IFont ppfont);

            [PreserveSig]
            int IsEqual(
                   [In, MarshalAs(UnmanagedType.Interface)]
                      IFont pfontOther);

            void SetRatio(
                    int cyLogical,
                    int cyHimetric);

            void QueryTextMetrics(out IntPtr ptm);

            void AddRefHfont(
                    IntPtr hFont);

            void ReleaseHfont(
                    IntPtr hFont);

            void SetHdc(
                    IntPtr hdc);
        }

        [ComImport(), Guid("7BF80980-BF32-101A-8BBB-00AA00300CAB"), InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPicture
        {
            IntPtr GetHandle();

            IntPtr GetHPal();

            [return: MarshalAs(UnmanagedType.I2)]
            short GetPictureType();

            int GetWidth();

            int GetHeight();

            void Render(
               IntPtr hDC,
               int x,
               int y,
               int cx,
               int cy,
               int xSrc,
               int ySrc,
               int cxSrc,
               int cySrc,
               IntPtr rcBounds
               );

            void SetHPal(
                    IntPtr phpal);

            IntPtr GetCurDC();

            void SelectPicture(
                    IntPtr hdcIn,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                     IntPtr[] phdcOut,
                   [Out, MarshalAs(UnmanagedType.LPArray)]
                     IntPtr[] phbmpOut);

            [return: MarshalAs(UnmanagedType.Bool)]
            bool GetKeepOriginalFormat();

            void SetKeepOriginalFormat(
                   [In, MarshalAs(UnmanagedType.Bool)]
                     bool pfkeep);

            void PictureChanged();

            [PreserveSig]
            int SaveAsFile(
                   [In, MarshalAs(UnmanagedType.Interface)]
                     IStream pstm,

                    int fSaveMemCopy,
                   [Out]
                     out int pcbSize);

            int GetAttributes();
        }

        [ComImport(), Guid("7BF80981-BF32-101A-8BBB-00AA00300CAB"), InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIDispatch)]
        public interface IPictureDisp
        {
            IntPtr Handle { get; }

            IntPtr HPal { get; }

            short PictureType { get; }

            int Width { get; }

            int Height { get; }

            void Render(
                    IntPtr hdc,
                    int x,
                    int y,
                    int cx,
                    int cy,
                    int xSrc,
                    int ySrc,
                    int cxSrc,
                    int cySrc);
        }

        /// <summary>
        ///  This class provides static methods to create, activate and deactivate the theming scope.
        /// </summary>
        internal class ThemingScope
        {
            private static ACTCTX enableThemingActivationContext;
            private static IntPtr hActCtx;
            private static bool contextCreationSucceeded;

            /// <summary>
            ///  We now use explicitactivate everywhere and use this method to determine if we
            ///  really need to activate the activationcontext.  This should be pretty fast.
            /// </summary>
            private static bool IsContextActive()
            {
                IntPtr current = IntPtr.Zero;

                if (contextCreationSucceeded && GetCurrentActCtx(out current))
                {
                    return current == hActCtx;
                }
                return false;
            }

            /// <summary>
            ///  Activate() does nothing if a theming context is already active on the current thread, which is good
            ///  for perf reasons. However, in some cases, like in the Timer callback, we need to put another context
            ///  on the stack even if one is already present. In such cases, this method helps - you get to manage
            ///  the cookie yourself though.
            /// </summary>
            public static IntPtr Activate()
            {
                IntPtr userCookie = IntPtr.Zero;

                if (Application.UseVisualStyles && contextCreationSucceeded && OSFeature.Feature.IsPresent(OSFeature.Themes))
                {
                    if (!IsContextActive())
                    {
                        if (!ActivateActCtx(hActCtx, out userCookie))
                        {
                            // Be sure cookie always zero if activation failed
                            userCookie = IntPtr.Zero;
                        }
                    }
                }

                return userCookie;
            }

            /// <summary>
            ///  Use this to deactivate a context activated by calling ExplicitActivate.
            /// </summary>
            public static IntPtr Deactivate(IntPtr userCookie)
            {
                if (userCookie != IntPtr.Zero && OSFeature.Feature.IsPresent(OSFeature.Themes))
                {
                    if (DeactivateActCtx(0, userCookie))
                    {
                        // deactivation succeeded...
                        userCookie = IntPtr.Zero;
                    }
                }

                return userCookie;
            }

            public static bool CreateActivationContext(string dllPath, int nativeResourceManifestID)
            {
                lock (typeof(ThemingScope))
                {
                    if (!contextCreationSucceeded && OSFeature.Feature.IsPresent(OSFeature.Themes))
                    {

                        enableThemingActivationContext = new ACTCTX
                        {
                            cbSize = Marshal.SizeOf<ACTCTX>(),
                            lpSource = dllPath,
                            lpResourceName = (IntPtr)nativeResourceManifestID,
                            dwFlags = ACTCTX_FLAG_RESOURCE_NAME_VALID
                        };

                        hActCtx = CreateActCtx(ref enableThemingActivationContext);
                        contextCreationSucceeded = (hActCtx != new IntPtr(-1));
                    }

                    return contextCreationSucceeded;
                }
            }

            // All the pinvoke goo...
            [DllImport(ExternDll.Kernel32)]

            private extern static IntPtr CreateActCtx(ref ACTCTX actctx);
            [DllImport(ExternDll.Kernel32)]

            private extern static bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);
            [DllImport(ExternDll.Kernel32)]

            private extern static bool DeactivateActCtx(int dwFlags, IntPtr lpCookie);
            [DllImport(ExternDll.Kernel32)]

            private extern static bool GetCurrentActCtx(out IntPtr handle);

            private const int ACTCTX_FLAG_RESOURCE_NAME_VALID = 0x008;

            private struct ACTCTX
            {
                public int cbSize;
                public uint dwFlags;
                public string lpSource;
                public ushort wProcessorArchitecture;
                public ushort wLangId;
                public string lpAssemblyDirectory;
                public IntPtr lpResourceName;
                public string lpApplicationName;
            }
        }

        // UIAutomationCore methods

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        internal static extern int UiaHostProviderFromHwnd(HandleRef hwnd, out IRawElementProviderSimple provider);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        internal static extern IntPtr UiaReturnRawElementProvider(HandleRef hwnd, IntPtr wParam, IntPtr lParam, IRawElementProviderSimple el);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        internal static extern bool UiaClientsAreListening();

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int UiaRaiseAutomationEvent(IRawElementProviderSimple provider, int id);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int UiaRaiseAutomationPropertyChangedEvent(IRawElementProviderSimple provider, int id, object oldValue, object newValue);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int UiaRaiseNotificationEvent(
            IRawElementProviderSimple provider,
            Automation.AutomationNotificationKind notificationKind,
            Automation.AutomationNotificationProcessing notificationProcessing,
            string notificationText,
            string activityId);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        internal static extern int UiaRaiseStructureChangedEvent(IRawElementProviderSimple provider, StructureChangeType structureChangeType, int[] runtimeId, int runtimeIdLen);

        // UIAutomation interfaces and enums
        // obtained from UIAutomation source code

        /// <summary>
        ///  Logical structure change flags
        /// </summary>
        [ComVisible(true)]
        [Guid("e4cfef41-071d-472c-a65c-c14f59ea81eb")]
        public enum StructureChangeType
        {
            /// <summary>Logical child added</summary>
            ChildAdded,
            /// <summary>Logical child removed</summary>
            ChildRemoved,
            /// <summary>Logical children invalidated</summary>
            ChildrenInvalidated,
            /// <summary>Logical children were bulk added</summary>
            ChildrenBulkAdded,
            /// <summary>Logical children were bulk removed</summary>
            ChildrenBulkRemoved,
            /// <summary>The order of the children below their parent has changed.</summary>
            ChildrenReordered,
        }

        [ComVisible(true)]
        [Guid("76d12d7e-b227-4417-9ce2-42642ffa896a")]
        public enum ExpandCollapseState
        {
            /// <summary>No children are showing</summary>
            Collapsed,
            /// <summary>All children are showing</summary>
            Expanded,
            /// <summary>Not all children are showing</summary>
            PartiallyExpanded,
            /// <summary>Does not expand or collapse</summary>
            LeafNode
        }

        [Flags]
        public enum ProviderOptions
        {
            /// <summary>Indicates that this is a client-side provider</summary>
            ClientSideProvider = 0x0001,
            /// <summary>Indicates that this is a server-side provider</summary>
            ServerSideProvider = 0x0002,
            /// <summary>Indicates that this is a non-client-area provider</summary>
            NonClientAreaProvider = 0x0004,
            /// <summary>Indicates that this is an override provider</summary>
            OverrideProvider = 0x0008,

            /// <summary>Indicates that this provider handles its own focus, and does not want
            ///  UIA to set focus to the nearest HWND on its behalf when AutomationElement.SetFocus
            ///  is used. This option is typically used by providers for HWNDs that appear to take
            ///  focus without actually receiving actual Win32 focus, such as menus and dropdowns</summary>
            ProviderOwnsSetFocus = 0x0010,

            /// <summary>Indicates that this provider expects to be called according to COM threading rules:
            ///  if the provider is in a Single-Threaded Apartment, it will be called only on the apartment
            ///  thread. Only Server-side providers can use this option.</summary>
            UseComThreading = 0x0020
        }

        public static readonly Guid guid_IAccessibleEx = new Guid("{F8B80ADA-2C44-48D0-89BE-5FF23C9CD875}");

        /// <summary>
        ///  The interface representing containers that manage selection.
        /// </summary>
        /// <remarks>
        ///  Client code uses this public interface; server implementers implent the
        ///  ISelectionProvider public interface instead.
        /// </remarks>
        [ComImport()]
        [ComVisible(true)]
        [Guid("fb8b03af-3bdf-48d4-bd36-1a65793be168")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISelectionProvider
        {
            /// <summary>
            ///  Get the currently selected elements
            /// </summary>
            /// <returns>An AutomationElement array containing the currently selected elements</returns>
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            /* IRawElementProviderSimple */
            object[] GetSelection();

            /// <summary>
            ///  Indicates whether the control allows more than one element to be selected
            /// </summary>
            /// <returns>Boolean indicating whether the control allows more than one element to be selected</returns>
            /// <remarks>If this is false, then the control is a single-select ccntrol</remarks>
            bool CanSelectMultiple
            {
                [return: MarshalAs(UnmanagedType.Bool)]
                get;
            }

            /// <summary>
            ///  Indicates whether the control requires at least one element to be selected
            /// </summary>
            /// <returns>Boolean indicating whether the control requires at least one element to be selected</returns>
            /// <remarks>If this is false, then the control allows all elements to be unselected</remarks>
            bool IsSelectionRequired
            {
                [return: MarshalAs(UnmanagedType.Bool)]
                get;
            }
        }

        /// <summary>
        ///  Define a Selectable Item (only supported on logical elements that are a
        ///  child of an Element that supports SelectionPattern and is itself selectable).
        ///  This allows for manipulation of Selection from the element itself.
        /// </summary>
        [ComImport()]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        [Guid("2acad808-b2d4-452d-a407-91ff1ad167b2")]
        public interface ISelectionItemProvider
        {
            /// <summary>
            ///  Sets the current element as the selection
            ///  This clears the selection from other elements in the container.
            /// </summary>
            void Select();

            /// <summary>
            ///  Adds current element to selection.
            /// </summary>
            void AddToSelection();

            /// <summary>
            ///  Removes current element from selection.
            /// </summary>
            void RemoveFromSelection();

            /// <summary>
            ///  Check whether an element is selected.
            /// </summary>
            /// <returns>Returns true if the element is selected.</returns>
            bool IsSelected { [return: MarshalAs(UnmanagedType.Bool)] get; }

            /// <summary>
            ///  The logical element that supports the SelectionPattern for this Item.
            /// </summary>
            /// <returns>Returns a IRawElementProviderSimple.</returns>
            IRawElementProviderSimple SelectionContainer { [return: MarshalAs(UnmanagedType.Interface)] get; }
        }

        /// <summary>
        ///  Implemented by providers which want to provide information about or want to
        ///  reposition contained HWND-based elements.
        /// </summary>
        [ComVisible(true)]
        [Guid("1d5df27c-8947-4425-b8d9-79787bb460b8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderHwndOverride : IRawElementProviderSimple
        {
            /// <summary>
            ///  Request a provider for the specified component. The returned provider can supply additional
            ///  properties or override properties of the specified component.
            /// </summary>
            /// <param name="hwnd">The window handle of the component.</param>
            /// <returns>Return the provider for the specified component, or null if the component is not being overridden.</returns>
            [return: MarshalAs(UnmanagedType.Interface)]
            IRawElementProviderSimple GetOverrideProviderForHwnd(IntPtr hwnd);
        }

        [ComImport()]
        [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IServiceProvider
        {
            [PreserveSig]
            int QueryService(ref Guid service, ref Guid riid, out IntPtr ppvObj);
        }

        [ComVisible(true)]
        [ComImport()]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("F8B80ADA-2C44-48D0-89BE-5FF23C9CD875")]
        internal interface IAccessibleEx
        {
            // Returns the IAccessibleEx for specified child. Returns
            // S_OK/NULL if this implementation does not use child ids,
            // or does not have an IAccessibleEx for the specified child,
            // or already represents a child element.
            // idChild must be normalized; ie. client must have previously
            // used get_accChild to check whether it actually has its own
            // IAccessible. Only idChild values that do not have a corresponding
            // IAccessible can be used here.

            [return: MarshalAs(UnmanagedType.IUnknown)]
            object GetObjectForChild(int idChild);

            // Returns an IAccessible and idChild pair for this IAccessibleEx.
            // Implementation must return fully normalized idChild values: ie.
            // it is not required to call get_accChild on the resulting pair.
            //
            // For IAccessible implementations that do not use child ids, this
            // just returns the corresponding IAccessible and CHILDID_SELF.
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetIAccessiblePair(
            [Out, MarshalAs(UnmanagedType.Interface)]
            out object /*UnsafeNativeMethods.IAccessible*/ ppAcc,
            [Out] out int pidChild);

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
            int[] GetRuntimeId();

            // Some wrapper-based implementations (notably UIABridge) can't reasonably wrap all
            // IRawElementProviderSimple elements returned as property values or patterns, so
            // these elements won't QI to IAccessibleEx. Where this is the case, the original
            // IAccessibleEx that the property was retreived from must implement this method
            // so that the client can get an IAccessibleEx.
            //
            // Usage for a client is as follows:
            // When an IRawElementProviderSimple is obtained as a property value,
            // - first try to QI to IAccessibleEx
            // - if that fails, call this method on the source IAccessibleEx
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ConvertReturnedElement(
            [In, MarshalAs(UnmanagedType.Interface)]
            object /*UnsafeNativeMethods.IRawElementProviderSimple*/ pIn,
            [Out, MarshalAs(UnmanagedType.Interface)]
            out object /*UnsafeNativeMethods.IAccessibleEx*/ ppRetValOut);
        }

        [ComVisible(true)]
        [ComImport()]
        [Guid("d847d3a5-cab0-4a98-8c32-ecb45c59ad24")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IExpandCollapseProvider
        {
            /// <summary>
            ///  Blocking method that returns after the element has been expanded.
            /// </summary>
            void Expand();

            /// <summary>
            ///  Blocking method that returns after the element has been collapsed.
            /// </summary>
            void Collapse();

            ///<summary>indicates an element's current Collapsed or Expanded state</summary>
            ExpandCollapseState ExpandCollapseState
            {
                get;
            }
        }

        [ComVisible(true)]
        [ComImport()]
        [Guid("c7935180-6fb3-4201-b174-7df73adbf64a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IValueProvider
        {
            /// <summary>
            ///  Request to set the value that this UI element is representing
            /// </summary>
            /// <param name="value">Value to set the UI to</param>
            void SetValue([MarshalAs(UnmanagedType.LPWStr)] string value);

            ///<summary>Value of a value control, as a a string.</summary>
            string Value
            {
                get;
            }

            ///<summary>Indicates that the value can only be read, not modified.
            ///returns True if the control is read-only</summary>
            bool IsReadOnly
            {
                [return: MarshalAs(UnmanagedType.Bool)] // CLR
                get;
            }
        }

        [ComImport()]
        [ComVisible(true)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("36dc7aef-33e6-4691-afe1-2be7274b3d33")]
        public interface IRangeValueProvider
        {
            void SetValue(double value);

            double Value { get; }

            bool IsReadOnly { [return: MarshalAs(UnmanagedType.Bool)] get; }

            double Maximum { get; }

            double Minimum { get; }

            double LargeChange { get; }

            double SmallChange { get; }
        }

        [ComImport()]
        [ComVisible(true)]
        [Guid("D6DD68D1-86FD-4332-8666-9ABEDEA2D24C")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderSimple
        {
            /// <summary>
            ///  Indicates the type of provider this is, for example, whether it is a client-side
            ///  or server-side provider.
            /// </summary>
            /// <remarks>
            ///  Providers must specify at least either one of ProviderOptions.ClientSideProvider
            ///  or ProviderOptions.ServerSideProvider.
            ///
            ///  UIAutomation treats different types of providers
            ///  differently - for example, events from server-side provider are broadcast to all listening
            ///  clients, whereas events from client-side providers remain in that client.
            /// </remarks>
            ProviderOptions ProviderOptions
            {
                get;
            }

            /// <summary>
            ///  Get a pattern interface from this object
            /// </summary>
            /// <param name="patternId">Identifier indicating the interface to return</param>
            /// <returns>Returns the interface as an object, if supported; otherwise returns null/</returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object GetPatternProvider(int patternId);

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyId">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            object GetPropertyValue(int propertyId);

            // Only native impl roots need to return something for this,
            // proxies always return null (cause we already know their HWNDs)
            // If proxies create themselves when handling winvents events, then they
            // also need to implement this so we can determine the HWND. Still only
            // lives on a root, however.
            /// <summary>
            ///  Returns a base provider for this element.
            ///
            ///  Typically only used by elements that correspond directly to a Win32 Window Handle,
            ///  in which case the implementation returns AutomationInteropProvider.BaseElementFromHandle( hwnd ).
            /// </summary>
            IRawElementProviderSimple HostRawElementProvider
            {
                get;
            }
        }

        /// <summary>
        ///  Directions for navigation the UIAutomation tree
        /// </summary>
        [ComVisible(true)]
        [Guid("670c3006-bf4c-428b-8534-e1848f645122")]
        public enum NavigateDirection
        {
            /// <summary>Navigate to parent</summary>
            Parent,
            /// <summary>Navigate to next sibling</summary>
            NextSibling,
            /// <summary>Navigate to previous sibling</summary>
            PreviousSibling,
            /// <summary>Navigate to first child</summary>
            FirstChild,
            /// <summary>Navigate to last child</summary>
            LastChild,
        }

        /// <summary>
        ///  Implemented by providers to expose elements that are part of
        ///  a structure more than one level deep. For simple one-level
        ///  structures which have no children, IRawElementProviderSimple
        ///  can be used instead.
        ///
        ///  The root node of the fragment must support the IRawElementProviderFragmentRoot
        ///  interface, which is derived from this, and has some additional methods.
        /// </summary>
        [ComVisible(true)]
        [Guid("f7063da8-8359-439c-9297-bbc5299a7d87")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport()]
        public interface IRawElementProviderFragment : IRawElementProviderSimple
        {
            /// <summary>
            ///  Request to return the element in the specified direction
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate</param>
            /// <returns>Returns the element in the specified direction</returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object /*IRawElementProviderFragment*/ Navigate(NavigateDirection direction);

            /// <summary>
            ///  Gets the runtime ID of an elemenent. This should be unique
            ///  among elements on a desktop.
            /// </summary>
            /// <remarks>
            ///  Proxy implementations should return null for the top-level proxy which
            ///  correpsonds to the HWND; and should return an array which starts
            ///  with AutomationInteropProvider.AppendRuntimeId, followed by values
            ///  which are then unique within that proxy's HWNDs.
            /// </remarks>
            int[] GetRuntimeId();

            /// <summary>
            ///  Return a bounding rectangle of this element
            /// </summary>
            NativeMethods.UiaRect BoundingRectangle
            {
                get;
            }

            /// <summary>
            ///  If this UI is capable of hosting other UI that also supports UIAutomation, and
            ///  the subtree rooted at this element contains such hosted UI fragments, this should return
            ///  an array of those fragments.
            ///
            ///  If this UI does not host other UI, it may return null.
            /// </summary>
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[] /*IRawElementProviderSimple[]*/ GetEmbeddedFragmentRoots();

            /// <summary>
            ///  Request that focus is set to this item.
            ///  The UIAutomation framework will ensure that the UI hosting this fragment is already
            ///  focused before calling this method, so this method should only update its internal
            ///  focus state; it should not attempt to give its own HWND the focus, for example.
            /// </summary>
            void SetFocus();

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            IRawElementProviderFragmentRoot FragmentRoot
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
            }
        }

        /// <summary>
        ///  The root element in a fragment of UI must support this interface. Other
        ///  elements in the same fragment need to support the IRawElementProviderFragment
        ///  interface.
        /// </summary>
        [ComVisible(true)]
        [Guid("620ce2a5-ab8f-40a9-86cb-de3c75599b58")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport()]
        public interface IRawElementProviderFragmentRoot : IRawElementProviderFragment
        {
            /// <summary>
            ///  Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </summary>
            /// <param name="x">x coordinate of point to check</param>
            /// <param name="y">y coordinate of point to check</param>
            /// <returns>Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object /*IRawElementProviderFragment*/ ElementProviderFromPoint(double x, double y);

            /// <summary>
            ///  Return the element in this fragment which has the keyboard focus,
            /// </summary>
            /// <returns>Return the element in this fragment which has the keyboard focus,
            ///  if any; otherwise return null.</returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object /*IRawElementProviderFragment*/ GetFocus();
        }

#pragma warning disable CA1712 // Don't prefix enum values with enum type
        [Flags]
        public enum ToggleState
        {
            ToggleState_Off = 0,
            ToggleState_On = 1,
            ToggleState_Indeterminate = 2
        }
#pragma warning restore CA1712

        [ComImport()]
        [ComVisible(true)]
        [Guid("56D00BD0-C4F4-433C-A836-1A52A57E0892")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IToggleProvider
        {
            void Toggle();

            ///<summary>indicates an element's current on or off state</summary>
            ToggleState ToggleState
            {
                get;
            }
        }

#pragma warning disable CA1712 // Don't prefix enum values with enum type
        [Flags]
        public enum RowOrColumnMajor
        {
            RowOrColumnMajor_RowMajor = 0,
            RowOrColumnMajor_ColumnMajor = 1,
            RowOrColumnMajor_Indeterminate = 2
        }
#pragma warning restore CA1712

        [ComImport()]
        [ComVisible(true)]
        [Guid("9c860395-97b3-490a-b52a-858cc22af166")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITableProvider
        {
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[] /*IRawElementProviderSimple[]*/ GetRowHeaders();

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[] /*IRawElementProviderSimple[]*/ GetColumnHeaders();

            RowOrColumnMajor RowOrColumnMajor
            {
                get;
            }
        }

        [ComImport()]
        [ComVisible(true)]
        [Guid("b9734fa6-771f-4d78-9c90-2517999349cd")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITableItemProvider
        {
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[] /*IRawElementProviderSimple[]*/ GetRowHeaderItems();

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[] /*IRawElementProviderSimple[]*/ GetColumnHeaderItems();
        }

        [ComImport()]
        [ComVisible(true)]
        [Guid("b17d6187-0907-464b-a168-0ef17a1572b1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGridProvider
        {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object /*IRawElementProviderSimple*/ GetItem(int row, int column);

            int RowCount
            {
                [return: MarshalAs(UnmanagedType.I4)]
                get;
            }

            int ColumnCount
            {
                [return: MarshalAs(UnmanagedType.I4)]
                get;
            }
        }

        [ComImport()]
        [ComVisible(true)]
        [Guid("d02541f1-fb81-4d64-ae32-f520f8a6dbd1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGridItemProvider
        {
            int Row
            {
                [return: MarshalAs(UnmanagedType.I4)]
                get;
            }

            int Column
            {
                [return: MarshalAs(UnmanagedType.I4)]
                get;
            }

            int RowSpan
            {
                [return: MarshalAs(UnmanagedType.I4)]
                get;
            }

            int ColumnSpan
            {
                [return: MarshalAs(UnmanagedType.I4)]
                get;
            }

            IRawElementProviderSimple ContainingGrid
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                get;
            }
        }

        /// <summary>
        ///  Implemented by objects that have a single, unambiguous, action associated with them.
        ///  These objects are usually stateless, and invoking them does not change their own state,
        ///  but causes something to happen in the larger context of the app the control is in.
        ///
        ///  Examples of UI that implments this includes:
        ///  Push buttons
        ///  Hyperlinks
        ///  Menu items
        /// </summary>
        [ComImport()]
        [ComVisible(true)]
        [Guid("54fcb24b-e18e-47a2-b4d3-eccbe77599a2")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInvokeProvider
        {
            /// <summary>
            ///  Request that the control initiate its action.
            ///  Should return immediately without blocking.
            ///  There is no way to determine what happened, when it happend, or whether
            ///  anything happened at all.
            /// </summary>
            void Invoke();
        }

        /// <summary>
        ///  Implemented by objects in a known Scrollable context, such as ListItems, ListViewItems, TreeViewItems, and Tabs.
        ///  This allows them to be scrolled into view using known API's based on the control in question.
        /// </summary>
        [ComImport()]
        [ComVisible(true)]
        [Guid("2360c714-4bf1-4b26-ba65-9b21316127eb")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IScrollItemProvider
        {
            /// <summary>
            ///  Scrolls the windows containing this automation element to make this element visible.
            ///  InvalidOperationException should be thrown if item becomes unable to be scrolled. Makes
            ///  no guarantees about where the item will be in the scrolled window.
            /// </summary>
            void ScrollIntoView();
        }
    }
}
