﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        private static extern HRESULT RegisterDragDrop(IntPtr hwnd, IntPtr pDropTarget);

        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT RegisterDragDrop(IntPtr hwnd, IDropTarget pDropTarget);

        public static HRESULT RegisterDragDrop(IHandle hwnd, IDropTarget pDropTarget)
        {
            IntPtr dropTargetPtr = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(pDropTarget, CreateComInterfaceFlags.None);
            HRESULT result = RegisterDragDrop(hwnd.Handle, dropTargetPtr);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
