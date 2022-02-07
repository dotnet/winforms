// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IFileDialogEventsVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IFileDialogEventsVtbl), IntPtr.Size * 10);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, int>)&OnFileOk;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, int>)&OnFolderChanging;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, int>)&OnFolderChange;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, int>)&OnSelectionChange;
                vtblRaw[7] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, Shell32.FDESVR*, int>)&OnShareViolation;
                vtblRaw[8] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, int>)&OnTypeChange;
                vtblRaw[9] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, Shell32.FDEOR*, int>)&OnOverwrite;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            public static int OnFileOk(IntPtr thisPtr, IntPtr pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    return (int)inst.OnFileOk(fd);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            public static int OnFolderChanging(IntPtr thisPtr, IntPtr pfd, IntPtr psiFolder)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    Shell32.IShellItem siFolder = (Shell32.IShellItem)Instance.GetOrCreateObjectForComInstance(psiFolder, CreateObjectFlags.Unwrap);
                    return (int)inst.OnFolderChanging(fd, siFolder);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            public static int OnFolderChange(IntPtr thisPtr, IntPtr pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    return (int)inst.OnFolderChange(fd);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            public static int OnSelectionChange(IntPtr thisPtr, IntPtr pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    return (int)inst.OnSelectionChange(fd);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            public static int OnShareViolation(IntPtr thisPtr, IntPtr pfd, IntPtr psi, Shell32.FDESVR* pResponse)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    Shell32.IShellItem si = (Shell32.IShellItem)Instance.GetOrCreateObjectForComInstance(psi, CreateObjectFlags.Unwrap);
                    return (int)inst.OnShareViolation(fd, si, pResponse);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            public static int OnTypeChange(IntPtr thisPtr, IntPtr pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    return (int)inst.OnTypeChange(fd);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            public static int OnOverwrite(IntPtr thisPtr, IntPtr pfd, IntPtr psi, Shell32.FDEOR* pResponse)
            {
                try
                {
                    Shell32.IFileDialogEvents inst = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)thisPtr);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance(pfd, CreateObjectFlags.Unwrap);
                    Shell32.IShellItem si = (Shell32.IShellItem)Instance.GetOrCreateObjectForComInstance(psi, CreateObjectFlags.Unwrap);
                    return (int)inst.OnOverwrite(fd, si, pResponse);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }
        }
    }
}
