// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        private static extern HRESULT RegisterDragDrop(IntPtr hwnd, IntPtr pDropTarget);

        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT RegisterDragDrop(IntPtr hwnd, IDropTarget pDropTarget);

        public static unsafe HRESULT RegisterDragDrop(IHandle hwnd, IDropTarget pDropTarget)
        {
            if (!WinFormsComWrappers.Instance.TryGetComPointer(pDropTarget, IID.IDropTarget, out IUnknown* dropTargetPtr))
            {
                return HRESULT.E_NOINTERFACE;
            }

            HRESULT result = RegisterDragDrop(hwnd.Handle, (nint)(void*)dropTargetPtr);
            Marshal.Release((nint)(void*)dropTargetPtr);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
