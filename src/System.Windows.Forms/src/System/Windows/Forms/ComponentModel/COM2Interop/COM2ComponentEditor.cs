// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal class Com2ComponentEditor : WindowsFormsComponentEditor
{
    public static unsafe bool NeedsComponentEditor(object comObject)
    {
        if (comObject is IPerPropertyBrowsing.Interface perPropertyBrowsing)
        {
            // Check for a property page.
            Guid guid = Guid.Empty;
            HRESULT hr = perPropertyBrowsing.MapPropertyToPage(PInvoke.MEMBERID_NIL, &guid);
            if (hr.Succeeded && !guid.Equals(Guid.Empty))
            {
                return true;
            }
        }

        if (comObject is ISpecifyPropertyPages.Interface ispp)
        {
            CAUUID uuids = default;
            try
            {
                HRESULT hr = ispp.GetPages(&uuids);
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
        if (obj is IPerPropertyBrowsing.Interface perPropertyBrowsing)
        {
            // Check for a property page.
            Guid guid = Guid.Empty;
            HRESULT hr = perPropertyBrowsing.MapPropertyToPage(PInvoke.MEMBERID_NIL, &guid);
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
                        PInvoke.GetThreadLocale(),
                        0,
                        null);
                }

                return true;
            }
        }

        if (obj is ISpecifyPropertyPages.Interface ispp)
        {
            try
            {
                var uuids = default(CAUUID);
                HRESULT hr = ispp.GetPages(&uuids);
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
                            PInvoke.GetThreadLocale(),
                            0,
                            null);

                        return true;
                    }
                }
                finally
                {
                    if (uuids.pElems is not null)
                    {
                        Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                    }
                }
            }
            catch (Exception ex)
            {
                string errString = SR.ErrorPropertyPageFailed;

                IUIService? uiSvc = (IUIService?)context?.GetService(typeof(IUIService));

                if (uiSvc is null)
                {
                    RTLAwareMessageBox.Show(
                        null,
                        errString,
                        SR.PropertyGridTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
                else if (ex is not null)
                {
                    uiSvc.ShowError(ex, errString);
                }
                else
                {
                    uiSvc.ShowError(errString);
                }
            }
        }

        return false;
    }
}
