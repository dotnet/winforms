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
using static Interop;

namespace System.Windows.Forms
{
    internal static class UnsafeNativeMethods
    {
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

        [DllImport(ExternDll.Kernel32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetLocaleInfo(uint Locale, int LCType, StringBuilder lpLCData, int cchData);

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
                    if (capacity < Kernel32.MAX_UNICODESTRING_LEN)
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

        [DllImport(ExternDll.Shell32, CharSet = CharSet.Auto)]
        public static extern int Shell_NotifyIcon(int message, NativeMethods.NOTIFYICONDATA pnid);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static bool InsertMenuItem(HandleRef hMenu, int uItem, bool fByPosition, NativeMethods.MENUITEMINFO_T lpmii);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr GetMenu(HandleRef hWnd);

        [DllImport(Libraries.User32, CharSet = CharSet.Auto)]
        public static extern BOOL GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, [In, Out] NativeMethods.MENUITEMINFO_T lpmii);

        public static BOOL GetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, NativeMethods.MENUITEMINFO_T lpmii)
        {
            BOOL result = GetMenuItemInfo(hMenu.Handle, uItem, fByPosition, lpmii);
            GC.KeepAlive(hMenu.Wrapper);
            return result;
        }

        [DllImport(Libraries.User32, CharSet = CharSet.Auto)]
        public static extern BOOL GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, [In, Out] NativeMethods.MENUITEMINFO_T_RW lpmii);

        public static BOOL GetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, NativeMethods.MENUITEMINFO_T_RW lpmii)
        {
            BOOL result = GetMenuItemInfo(hMenu.Handle, uItem, fByPosition, lpmii);
            GC.KeepAlive(hMenu.Wrapper);
            return result;
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static bool SetMenuItemInfo(HandleRef hMenu, int uItem, bool fByPosition, NativeMethods.MENUITEMINFO_T lpmii);

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

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);

        public static StringBuilder GetModuleFileNameLongPath(HandleRef hModule)
        {
            StringBuilder buffer = new StringBuilder(Kernel32.MAX_PATH);
            int noOfTimes = 1;
            int length = 0;
            // Iterating by allocating chunk of memory each time we find the length is not sufficient.
            // Performance should not be an issue for current MAX_PATH length due to this change.
            while (((length = GetModuleFileName(hModule, buffer, buffer.Capacity)) == buffer.Capacity)
                && Marshal.GetLastWin32Error() == NativeMethods.ERROR_INSUFFICIENT_BUFFER
                && buffer.Capacity < Kernel32.MAX_UNICODESTRING_LEN)
            {
                noOfTimes += 2; // Increasing buffer size by 520 in each iteration.
                int capacity = noOfTimes * Kernel32.MAX_PATH < Kernel32.MAX_UNICODESTRING_LEN ? noOfTimes * Kernel32.MAX_PATH : Kernel32.MAX_UNICODESTRING_LEN;
                buffer.EnsureCapacity(capacity);
            }
            buffer.Length = length;
            return buffer;
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendDlgItemMessage(HandleRef hDlg, int nIDDlgItem, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, Point pt, int uFlags);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern int SetWindowRgn(HandleRef hwnd, HandleRef hrgn, bool fRedraw);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern void GetTempFileName(string tempDirName, string prefixName, int unique, StringBuilder sb);

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
        public static extern IntPtr GetAncestor(HandleRef hWnd, int flags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsZoomed(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowName);

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
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVBKIMAGE lParam);

        [DllImport(ExternDll.User32, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int SendMessage(HandleRef hWnd, int msg, int wParam, ref NativeMethods.LVHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.TCITEM_T lParam);

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
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref Richedit.CHARRANGE lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.FINDTEXT lParam);

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
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVHITTESTINFO lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVCOLUMN_T lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out] ref NativeMethods.LVITEM lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.LVCOLUMN lParam);

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
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, NativeMethods.SYSTEMTIMEARRAY lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, ref RECT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, ref short wParam, ref short lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, [In, Out, MarshalAs(UnmanagedType.Bool)] ref bool wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, int wParam, ref RECT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, NativeMethods.ListViewCompareCallback pfnCompare);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool GetWindowRect(HandleRef hWnd, ref RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(HandleRef hWnd, int nIDDlgItem);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr DefMDIChildProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport(ExternDll.User32, SetLastError = true)]
        public static extern bool GetUserObjectInformation(HandleRef hObj, int nIndex, ref NativeMethods.USEROBJECTFLAGS pvBuffer, int nLength, ref int lpnLengthNeeded);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, HandleRef pAcc);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, IntPtr pAcc);

        [DllImport(ExternDll.Oleacc, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int CreateStdAccessibleObject(HandleRef hWnd, int objID, ref Guid refiid, [In, Out, MarshalAs(UnmanagedType.Interface)] ref object pAcc);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr GetWindowDC(HandleRef hWnd);

        [DllImport(ExternDll.User32, EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public extern static IntPtr SendCallbackMessage(HandleRef hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.Shell32, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void DragAcceptFiles(HandleRef hWnd, bool fAccept);

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

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr DefFrameProc(IntPtr hWnd, IntPtr hWndClient, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(HandleRef hwnd, int crKey, byte bAlpha, int dwFlags);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        public static extern void GetStartupInfo([In, Out] NativeMethods.STARTUPINFO_I startupinfo_i);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetActiveWindow(HandleRef hWnd);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateIC(string lpszDriverName, string lpszDeviceName, string lpszOutput, HandleRef /*DEVMODE*/ lpInitData);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindow(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(HandleRef hwnd, int msg, int wparam, int lparam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(HandleRef hwnd, int msg, int wparam, IntPtr lparam);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        // This method is not available until Windows 8.1
        [DllImport(ExternDll.User32, ExactSpelling = true, SetLastError = true)]
        public static extern uint GetDpiForWindow(HandleRef hWnd);

        //for RegionData
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetRegionData(HandleRef hRgn, int size, IntPtr lpRgnData);

        public unsafe static RECT[] GetRectsFromRegion(IntPtr hRgn)
        {
            RECT[] regionRects = null;
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
                            regionRects = new RECT[pRgnDataHeader->nCount];

                            Debug.Assert(regionDataSize == pRgnDataHeader->cbSizeOfStruct + pRgnDataHeader->nCount * pRgnDataHeader->nRgnSize);
                            Debug.Assert(Marshal.SizeOf<RECT>() == pRgnDataHeader->nRgnSize || pRgnDataHeader->nRgnSize == 0);

                            // use the header size as the offset, and cast each rect in.
                            int rectStart = pRgnDataHeader->cbSizeOfStruct;
                            for (int i = 0; i < pRgnDataHeader->nCount; i++)
                            {
                                // use some fancy pointer math to just copy the rect bits directly into the array.
                                regionRects[i] = *((RECT*)((byte*)pBytes + rectStart + (Marshal.SizeOf<RECT>() * i)));
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

        // Used to control the webbrowser appearance and provide DTE to script via window.external
        [ComImport]
        [ComVisible(true)]
        [Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IDocHostUIHandler
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

            [PreserveSig]
            HRESULT ShowUI(
                uint dwID,
                Ole32.IOleInPlaceActiveObject activeObject,
                Ole32.IOleCommandTarget commandTarget,
                Ole32.IOleInPlaceFrame frame,
                Ole32.IOleInPlaceUIWindow doc);

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

            [PreserveSig]
            HRESULT OnDocWindowActivate(
                BOOL fActivate);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnFrameWindowActivate(
                [In, MarshalAs(UnmanagedType.Bool)]
                bool fActivate);

            [PreserveSig]
            HRESULT ResizeBorder(
                RECT* rect,
                Ole32.IOleInPlaceUIWindow doc,
                BOOL fFrameWindow);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                User32.MSG* lpMsg,
                Guid* pguidCmdGroup,
                uint nCmdID);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetOptionKeyPath(
                [Out, MarshalAs(UnmanagedType.LPArray)]
                string[] pbstrKey,
                [In, MarshalAs(UnmanagedType.U4)]
                int dw);

            [PreserveSig]
            HRESULT GetDropTarget(
                Ole32.IDropTarget pDropTarget,
                out Ole32.IDropTarget ppDropTarget);

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
            [DispId(501)]
            Ole32.OLECMDF QueryStatusWB(
                Ole32.OLECMDID cmdID);

            [DispId(502)]
            [PreserveSig]
            HRESULT ExecWB(
                Ole32.OLECMDID cmdID,
                Ole32.OLECMDEXECOPT cmdexecopt,
                IntPtr pvaIn,
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
        }

        [ComImport]
        [Guid("00000112-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleObject
        {
            [PreserveSig]
            HRESULT SetClientSite(
                Ole32.IOleClientSite pClientSite);

            [PreserveSig]
            HRESULT GetClientSite(
                out Ole32.IOleClientSite ppClientSite);

            [PreserveSig]
            int SetHostNames(
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string szContainerApp,
                   [In, MarshalAs(UnmanagedType.LPWStr)]
                      string szContainerObj);

            [PreserveSig]
            HRESULT Close(
                Ole32.OLECLOSE dwSaveOption);

            [PreserveSig]
            HRESULT SetMoniker(
                Ole32.OLEWHICHMK dwWhichMoniker,
                [MarshalAs(UnmanagedType.Interface)] object pmk);

            [PreserveSig]
            HRESULT GetMoniker(
                Ole32.OLEGETMONIKER dwAssign,
                Ole32.OLEWHICHMK dwWhichMoniker,
                IntPtr* ppmk);

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
            HRESULT DoVerb(
                Ole32.OLEIVERB iVerb,
                User32.MSG* lpmsg,
                Ole32.IOleClientSite pActiveSite,
                int lindex,
                IntPtr hwndParent,
                RECT* lprcPosRect);

            [PreserveSig]
            HRESULT EnumVerbs(
                out IEnumOLEVERB e);

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
            HRESULT SetExtent(
                Ole32.DVASPECT dwDrawAspect,
                Size* pSizel);

            [PreserveSig]
            HRESULT GetExtent(
                Ole32.DVASPECT dwDrawAspect,
                Size* pSizel);

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
            HRESULT GetMiscStatus(
                Ole32.DVASPECT dwAspect,
                Ole32.OLEMISC* pdwStatus);

            [PreserveSig]
            HRESULT SetColorScheme(
                Gdi32.LOGPALETTE* pLogpal);
        }

        [ComImport]
        [Guid("1C2056CC-5EF4-101B-8BC8-00AA003E3B29")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceObjectWindowless
        {
            [PreserveSig]
            HRESULT SetClientSite(
                Ole32.IOleClientSite pClientSite);

            [PreserveSig]
            HRESULT GetClientSite(
                out Ole32.IOleClientSite ppClientSite);

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
            HRESULT SetMoniker(
                Ole32.OLEWHICHMK dwWhichMoniker,
                [MarshalAs(UnmanagedType.Interface)] object pmk);

            [PreserveSig]
            HRESULT GetMoniker(
                Ole32.OLEGETMONIKER dwAssign,
                Ole32.OLEWHICHMK dwWhichMoniker,
                IntPtr* ppmk);

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
            HRESULT DoVerb(
                int iVerb,
                User32.MSG* lpmsg,
                Ole32.IOleClientSite pActiveSite,
                int lindex,
                IntPtr hwndParent,
                RECT* lprcPosRect);

            [PreserveSig]
            HRESULT EnumVerbs(
                out IEnumOLEVERB e);

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
            HRESULT SetExtent(
                Ole32.DVASPECT dwDrawAspect, 
                Size* pSizel);

            [PreserveSig]
            HRESULT GetExtent(
                Ole32.DVASPECT dwDrawAspect,
                Size* pSizel);

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
            HRESULT GetMiscStatus(
                Ole32.DVASPECT dwAspect,
                Ole32.OLEMISC* pdwStatus);

            [PreserveSig]
            HRESULT SetColorScheme(
                Gdi32.LOGPALETTE* pLogpal);

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

        [ComImport]
        [Guid("0000010d-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IViewObject
        {
            [PreserveSig]
            HRESULT Draw(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                Ole32.DVTARGETDEVICE* ptd,
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
            HRESULT GetColorSet(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                Ole32.DVTARGETDEVICE* ptd,
                IntPtr hicTargetDev,
                Gdi32.LOGPALETTE* ppColorSet);

            [PreserveSig]
            int Freeze(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [Out]
                IntPtr pdwFreeze);

            [PreserveSig]
            int Unfreeze(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwFreeze);

            [PreserveSig]
            HRESULT SetAdvise(
                Ole32.DVASPECT aspects,
                Ole32.ADVF advf,
                IAdviseSink pAdvSink);

            [PreserveSig]
            HRESULT GetAdvise(
                Ole32.DVASPECT* pAspects,
                Ole32.ADVF* pAdvf,
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                IAdviseSink[] ppAdvSink);
        }

        [ComImport]
        [Guid("00000127-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IViewObject2 /* : IViewObject */
        {
            void Draw(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                Ole32.DVTARGETDEVICE* ptd,
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
            HRESULT GetColorSet(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                Ole32.DVTARGETDEVICE* ptd,
                IntPtr hicTargetDev,
                Gdi32.LOGPALETTE* ppColorSet);

            [PreserveSig]
            int Freeze(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                [Out]
                IntPtr pdwFreeze);

            [PreserveSig]
            int Unfreeze(
                [In, MarshalAs(UnmanagedType.U4)]
                int dwFreeze);

            [PreserveSig]
            HRESULT SetAdvise(
                Ole32.DVASPECT aspects,
                Ole32.ADVF advf,
                IAdviseSink pAdvSink);

            [PreserveSig]
            HRESULT GetAdvise(
                Ole32.DVASPECT* pAspects,
                Ole32.ADVF* pAdvf,
                // These can be NULL if caller doesn't want them
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                IAdviseSink[] ppAdvSink);

            [PreserveSig]
            HRESULT GetExtent(
                Ole32.DVASPECT dwDrawAspect,
                int lindex,
                Ole32.DVTARGETDEVICE* ptd,
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

            void Save(IPropertyBag pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties);
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
            HRESULT SetContentExtent(Size* pSizel);

            [PreserveSig]
            HRESULT GetContentExtent(Size* pSizel);
        }

        [ComImport]
        [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyBag
        {
            [PreserveSig]
            HRESULT Read(
                [MarshalAs(UnmanagedType.LPWStr)]
                string pszPropName,
                ref object pVar,
                IErrorLog pErrorLog);

            [PreserveSig]
            HRESULT Write(
                [MarshalAs(UnmanagedType.LPWStr)]
                string pszPropName,
                ref object pVar);
        }

        [ComImport]
        [Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IErrorLog
        {
            [PreserveSig]
            HRESULT AddError(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                Ole32.EXCEPINFO* pExcepInfo);
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

        [ComImport]
        [Guid("00000104-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IEnumOLEVERB
        {
            [PreserveSig]
            HRESULT Next(
                uint celt,
                Ole32.OLEVERB rgelt,
                uint* pceltFetched);

            [PreserveSig]
            HRESULT Skip(
                uint celt);

            [PreserveSig]
            HRESULT Reset();

            [PreserveSig]
            HRESULT Clone(
               out IEnumOLEVERB ppenum);
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

        [ComImport]
        [Guid("00020400-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IDispatch
        {
            [PreserveSig]
            HRESULT GetTypeInfoCount(
                uint* pctinfo);

            [PreserveSig]
            HRESULT GetTypeInfo(
                uint iTInfo,
                uint lcid,
                out ITypeInfo ppTInfo);

            [PreserveSig]
            HRESULT GetIDsOfNames(
                Guid* riid,
                [MarshalAs(UnmanagedType.LPArray)] string[] rgszNames,
                uint cNames,
                uint lcid,
                Ole32.DispatchID* rgDispId);

            [PreserveSig]
            HRESULT Invoke(
                Ole32.DispatchID dispIdMember,
                Guid* riid,
                uint lcid,
                uint dwFlags,
                Ole32.DISPPARAMS* pDispParams,
                [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult,
                Ole32.EXCEPINFO* pExcepInfo,
                IntPtr* pArgErr);
        }

        [ComImport]
        [Guid("00020401-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface ITypeInfo
        {
            [PreserveSig]
            HRESULT GetTypeAttr(ref IntPtr pTypeAttr);

            /// <remarks>
            /// This method is unused so we do not define the interface ITypeComp
            /// and its dependencies to avoid maintenance costs and code size.
            /// </remarks>
            [PreserveSig]
            HRESULT GetTypeComp(
                IntPtr* ppTComp);

            [PreserveSig]
            HRESULT GetFuncDesc(
                [MarshalAs(UnmanagedType.U4)] int index, ref IntPtr pFuncDesc);

            [PreserveSig]
            HRESULT GetVarDesc(
                [MarshalAs(UnmanagedType.U4)] int index,
                ref IntPtr pVarDesc);

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
            HRESULT GetImplTypeFlags(
                uint index,
                Ole32.IMPLTYPEFLAG* pImplTypeFlags);

            [PreserveSig]
            int GetIDsOfNames(IntPtr rgszNames, int cNames, IntPtr pMemId);

            [PreserveSig]
            int Invoke();

            [PreserveSig]
            HRESULT GetDocumentation(
                Ole32.DispatchID memid,
                ref string pBstrName,
                ref string pBstrDocString,
                [Out, MarshalAs(UnmanagedType.LPArray)] int[] pdwHelpContext,
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrHelpFile);

            [PreserveSig]
            HRESULT GetDllEntry(
                Ole32.DispatchID memid,
                INVOKEKIND invkind,
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrDllName,
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pBstrName,
                short* pwOrdinal);

            [PreserveSig]
            HRESULT GetRefTypeInfo(
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

            /// <remarks>
            /// This method is unused so we do not define the interface ITypeLib
            /// and its dependencies to avoid maintenance costs and code size.
            /// </remarks>
            [PreserveSig]
            HRESULT GetContainingTypeLib(
                IntPtr* ppTLib,
                uint* pIndex);

            [PreserveSig]
            void ReleaseTypeAttr(IntPtr typeAttr);

            [PreserveSig]
            void ReleaseFuncDesc(IntPtr funcDesc);

            [PreserveSig]
            void ReleaseVarDesc(IntPtr varDesc);
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagQACONTAINER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize = Marshal.SizeOf<tagQACONTAINER>();

            public Ole32.IOleClientSite pClientSite;

            [MarshalAs(UnmanagedType.Interface)]
            public object pAdviseSink = null;

            public Ole32.IPropertyNotifySink pPropertyNotifySink;

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
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagQACONTROL
        {
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=0, cbSize)*/]
            public int cbSize = Marshal.SizeOf<tagQACONTROL>();

            public Ole32.OLEMISC dwMiscStatus = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=8, dwViewStatus)*/]
            public int dwViewStatus = 0;

            public uint dwEventCookie = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=16, dwPropNotifyCookie)*/]
            public int dwPropNotifyCookie = 0;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=20, dwPointerActivationPolicy)*/]
            public int dwPointerActivationPolicy = 0;
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
    }
}
