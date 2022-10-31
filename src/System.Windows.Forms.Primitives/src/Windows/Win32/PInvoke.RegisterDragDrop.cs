// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Ole = Interop.Ole32;
using Windows.Win32.System.Ole;
using System.Diagnostics;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe HRESULT RegisterDragDrop<T>(T hwnd, Ole.IDropTarget pDropTarget)
            where T : IHandle<HWND>
        {
            if (!ComHelpers.TryGetComPointer(pDropTarget, Interop.IID.IDropTarget, out IDropTarget* dropTarget))
            {
                return HRESULT.E_NOINTERFACE;
            }

            // RegisterDragDrop calls AddRef()
            HRESULT result = RegisterDragDrop(hwnd.Handle, dropTarget);
            uint count = dropTarget->Release();
            Debug.Assert(count > 0);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
