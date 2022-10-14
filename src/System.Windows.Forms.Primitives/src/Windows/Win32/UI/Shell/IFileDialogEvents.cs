// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;
using Windows.Win32.System.Com;

namespace Windows.Win32.UI.Shell;

internal unsafe partial struct IFileDialogEvents : INativeGuid, IPopulateVTable<IFileDialogEvents.Vtbl>, IUnknown.Interface
{
    static Guid* INativeGuid.NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));

    public static void PopulateVTable(Vtbl* vtable)
    {
        vtable->OnFileOk_4 = &OnFileOk;
        vtable->OnFolderChanging_5 = &OnFolderChanging;
        vtable->OnFolderChange_6 = &OnFolderChange;
        vtable->OnSelectionChange_7 = &OnSelectionChange;
        vtable->OnShareViolation_8 = &OnShareViolation;
        vtable->OnTypeChange_9 = &OnTypeChange;
        vtable->OnOverwrite_10 = &OnOverwrite;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnFileOk(IFileDialogEvents* @this, IFileDialog* pfd)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnFileOk(pfd));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnFolderChanging(IFileDialogEvents* @this, IFileDialog* pfd, IShellItem* psiFolder)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnFolderChanging(pfd, psiFolder));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnFolderChange(IFileDialogEvents* @this, IFileDialog* pfd)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnFolderChange(pfd));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnSelectionChange(IFileDialogEvents* @this, IFileDialog* pfd)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnSelectionChange(pfd));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnShareViolation(IFileDialogEvents* @this, IFileDialog* pfd, IShellItem* psi, FDE_SHAREVIOLATION_RESPONSE* pResponse)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnShareViolation(pfd, psi, pResponse));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnTypeChange(IFileDialogEvents* @this, IFileDialog* pfd)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnTypeChange(pfd));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static HRESULT OnOverwrite(IFileDialogEvents* @this, IFileDialog* pfd, IShellItem* psi, FDE_OVERWRITE_RESPONSE* pResponse)
        => ComWrappers.UnwrapAndInvoke<IFileDialogEvents, Interface>(@this, o => o.OnOverwrite(pfd, psi, pResponse));
}
