// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Shell = Windows.Win32.UI.Shell;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IFileDialogEventsVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IFileDialogEvents.Vtbl* vtblRaw = (IFileDialogEvents.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IFileDialogEventsVtbl), sizeof(IFileDialogEvents.Vtbl));
                vtblRaw->QueryInterface_1 = (delegate* unmanaged[Stdcall]<IFileDialogEvents*, Guid*, void**, HRESULT>)fpQueryInterface;
                vtblRaw->AddRef_2 = (delegate* unmanaged[Stdcall]<IFileDialogEvents*, uint>)fpAddRef;
                vtblRaw->Release_3 = (delegate* unmanaged[Stdcall]<IFileDialogEvents*, uint>)fpRelease;
                vtblRaw->OnFileOk_4 = &OnFileOk;
                vtblRaw->OnFolderChanging_5 = &OnFolderChanging;
                vtblRaw->OnFolderChange_6 = &OnFolderChange;
                vtblRaw->OnSelectionChange_7 = &OnSelectionChange;
                vtblRaw->OnShareViolation_8 = &OnShareViolation;
                vtblRaw->OnTypeChange_9 = &OnTypeChange;
                vtblRaw->OnOverwrite_10 = &OnOverwrite;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnFileOk(IFileDialogEvents* @this, IFileDialog* pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    return instance.OnFileOk(fd);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnFolderChanging(IFileDialogEvents* @this, IFileDialog* pfd, Shell.IShellItem* psiFolder)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    Shell32.IShellItem siFolder = (Shell32.IShellItem)Instance.GetOrCreateObjectForComInstance((nint)psiFolder, CreateObjectFlags.Unwrap);
                    return instance.OnFolderChanging(fd, siFolder);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnFolderChange(IFileDialogEvents* @this, IFileDialog* pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    return instance.OnFolderChange(fd);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnSelectionChange(IFileDialogEvents* @this, IFileDialog* pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    return instance.OnSelectionChange(fd);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnShareViolation(IFileDialogEvents* @this, IFileDialog* pfd, Shell.IShellItem* psi, FDE_SHAREVIOLATION_RESPONSE* pResponse)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    Shell32.IShellItem si = (Shell32.IShellItem)Instance.GetOrCreateObjectForComInstance((nint)psi, CreateObjectFlags.Unwrap);
                    return instance.OnShareViolation(fd, si, (Shell32.FDESVR*)pResponse);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnTypeChange(IFileDialogEvents* @this, IFileDialog* pfd)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    return instance.OnTypeChange(fd);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            public static HRESULT OnOverwrite(IFileDialogEvents* @this, IFileDialog* pfd, Shell.IShellItem* psi, FDE_OVERWRITE_RESPONSE* pResponse)
            {
                try
                {
                    Shell32.IFileDialogEvents instance = ComInterfaceDispatch.GetInstance<Shell32.IFileDialogEvents>((ComInterfaceDispatch*)@this);
                    Shell32.IFileDialog fd = (Shell32.IFileDialog)Instance.GetOrCreateObjectForComInstance((nint)pfd, CreateObjectFlags.Unwrap);
                    Shell32.IShellItem si = (Shell32.IShellItem)Instance.GetOrCreateObjectForComInstance((nint)psi, CreateObjectFlags.Unwrap);
                    return instance.OnOverwrite(fd, si, (Shell32.FDEOR*)pResponse);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
