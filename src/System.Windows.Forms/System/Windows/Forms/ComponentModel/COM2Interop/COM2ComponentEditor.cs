// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal sealed class Com2ComponentEditor : WindowsFormsComponentEditor
{
    public static unsafe bool NeedsComponentEditor(object comObject)
    {
        using var propertyBrowsing = ComHelpers.TryGetComScope<IPerPropertyBrowsing>(comObject, out HRESULT hr);
        if (hr.Succeeded)
        {
            // Check for a property page.
            Guid guid = Guid.Empty;
            hr = propertyBrowsing.Value->MapPropertyToPage(PInvoke.MEMBERID_NIL, &guid);
            if (hr.Succeeded && !guid.Equals(Guid.Empty))
            {
                return true;
            }
        }

        using var propertyPages = ComHelpers.TryGetComScope<ISpecifyPropertyPages>(comObject, out hr);
        if (hr.Succeeded)
        {
            CAUUID uuids = default;
            try
            {
                hr = propertyPages.Value->GetPages(&uuids);
                return hr.Succeeded && uuids.cElems > 0;
            }
            finally
            {
                if (uuids.pElems is not null)
                {
                    Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                }
            }
        }

        return false;
    }

    public override unsafe bool EditComponent(ITypeDescriptorContext? context, object obj, IWin32Window? parent)
    {
        HWND handle = parent is null ? HWND.Null : (HWND)parent.Handle;

        // Try to get the page guid
        using var propertyBrowsing = ComHelpers.TryGetComScope<IPerPropertyBrowsing>(obj, out HRESULT hr);
        if (hr.Succeeded)
        {
            // Check for a property page.
            Guid guid = Guid.Empty;
            hr = propertyBrowsing.Value->MapPropertyToPage(PInvoke.MEMBERID_NIL, &guid);
            if (hr.Succeeded & !guid.Equals(Guid.Empty))
            {
                using var unknown = ComHelpers.GetComScope<IUnknown>(obj);

                fixed (char* c = "PropertyPages")
                {
                    PInvoke.OleCreatePropertyFrame(
                        handle,
                        0,
                        0,
                        (PCWSTR)c,
                        1,
                        unknown,
                        1,
                        &guid,
                        PInvokeCore.GetThreadLocale(),
                        0,
                        null).ThrowOnFailure();
                }

                return true;
            }
        }

        using var propertyPages = ComHelpers.TryGetComScope<ISpecifyPropertyPages>(obj, out hr);
        if (hr.Succeeded)
        {
            try
            {
                CAUUID uuids = default;
                hr = propertyPages.Value->GetPages(&uuids);
                if (!hr.Succeeded || uuids.cElems == 0)
                {
                    return false;
                }

                using var unknown = ComHelpers.GetComScope<IUnknown>(obj);
                try
                {
                    fixed (char* c = "PropertyPages")
                    {
                        PInvoke.OleCreatePropertyFrame(
                            handle,
                            0,
                            0,
                            (PCWSTR)c,
                            1,
                            unknown,
                            uuids.cElems,
                            uuids.pElems,
                            PInvokeCore.GetThreadLocale(),
                            0,
                            null).ThrowOnFailure();

                        return true;
                    }
                }
                finally
                {
                    if (uuids.pElems is not null)
                    {
                        Marshal.FreeCoTaskMem((nint)uuids.pElems);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!context.TryGetService(out IUIService? uiService))
                {
                    RTLAwareMessageBox.Show(
                        null,
                        SR.ErrorPropertyPageFailed,
                        SR.PropertyGridTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
                else
                {
                    uiService.ShowError(ex, SR.ErrorPropertyPageFailed);
                }
            }
        }

        return false;
    }
}
