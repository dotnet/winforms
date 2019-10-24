// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Design
{
    internal class UnsafeNativeMethods
    {
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(HandleRef hWnd, ref Point lpPoint);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ScreenToClient(HandleRef hWnd, ref Point lpPoint);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern void NotifyWinEvent(int winEvent, HandleRef hwnd, int objType, int objID);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        [DllImport(ExternDll.Ole32)]
        public static extern int ReadClassStg(HandleRef pStg, ref Guid pclsid);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public const int OBJID_CLIENT = unchecked((int)0xFFFFFFFC);

        [Flags]
        public enum ProviderOptions
        {
            ClientSideProvider = 0x0001,

            ServerSideProvider = 0x0002,

            NonClientAreaProvider = 0x0004,

            OverrideProvider = 0x0008,

            ProviderOwnsSetFocus = 0x0010,

            UseComThreading = 0x0020
        }

        [ComImport()]
        [ComVisible(true)]
        [Guid("D6DD68D1-86FD-4332-8666-9ABEDEA2D24C")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderSimple
        {
            /// </remarks>
            ProviderOptions ProviderOptions
            {
                get;
            }

            [return: MarshalAs(UnmanagedType.IUnknown)]
            object GetPatternProvider(int patternId);

            object GetPropertyValue(int propertyId);

            IRawElementProviderSimple HostRawElementProvider
            {
                get;
            }
        }

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        public static extern IntPtr UiaReturnRawElementProvider(HandleRef hwnd, IntPtr wParam, IntPtr lParam, IRawElementProviderSimple el);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        public static extern int UiaHostProviderFromHwnd(HandleRef hwnd, out IRawElementProviderSimple provider);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode)]
        public static extern bool UiaClientsAreListening();

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int UiaRaiseAutomationEvent(IRawElementProviderSimple provider, int id);

        [DllImport(ExternDll.UiaCore, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int UiaRaiseNotificationEvent(
            IRawElementProviderSimple provider,
            NativeMethods.AutomationNotificationKind notificationKind,
            NativeMethods.AutomationNotificationProcessing notificationProcessing,
            string notificationText,
            string activityId);
    }
}
