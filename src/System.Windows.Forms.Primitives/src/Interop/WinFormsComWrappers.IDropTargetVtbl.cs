// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IDropTargetVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDropTargetVtbl), IntPtr.Size * 7);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, uint, Point, uint*, int>)&DragEnter;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, uint, Point, uint*, int>)&DragOver;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, int>)&DragLeave;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, uint, Point, uint*, int>)&Drop;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static int DragEnter(IntPtr thisPtr, IntPtr pDataObj, uint grfKeyState, Point pt, uint* pdwEffect)
            {
                Ole32.IDropTarget inst = ComInterfaceDispatch.GetInstance<Ole32.IDropTarget>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.DragEnter(Marshal.GetObjectForIUnknown(pDataObj), grfKeyState, pt, ref *pdwEffect);
            }

            [UnmanagedCallersOnly]
            private static int DragOver(IntPtr thisPtr, uint grfKeyState, Point pt, uint* pdwEffect)
            {
                Ole32.IDropTarget inst = ComInterfaceDispatch.GetInstance<Ole32.IDropTarget>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.DragOver(grfKeyState, pt, ref *pdwEffect);
            }

            [UnmanagedCallersOnly]
            private static int DragLeave(IntPtr thisPtr)
            {
                Ole32.IDropTarget inst = ComInterfaceDispatch.GetInstance<Ole32.IDropTarget>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.DragLeave();
            }

            [UnmanagedCallersOnly]
            private static int Drop(IntPtr thisPtr, IntPtr pDataObj, uint grfKeyState, Point pt, uint* pdwEffect)
            {
                Ole32.IDropTarget inst = ComInterfaceDispatch.GetInstance<Ole32.IDropTarget>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.Drop(Marshal.GetObjectForIUnknown(pDataObj), grfKeyState, pt, ref *pdwEffect);
            }
        }
    }
}
