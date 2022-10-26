// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Windows.Win32.System.Ole;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2ComponentEditor : WindowsFormsComponentEditor
    {
        public static unsafe bool NeedsComponentEditor(object comObject)
        {
            if (comObject is IPerPropertyBrowsing.Interface perPropertyBrowsing)
            {
                // Check for a property page.
                Guid guid = Guid.Empty;
                HRESULT hr = perPropertyBrowsing.MapPropertyToPage((int)DispatchID.MEMBERID_NIL, &guid);
                if (hr.Succeeded && !guid.Equals(Guid.Empty))
                {
                    return true;
                }
            }

            if (comObject is ISpecifyPropertyPages ispp)
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
            IntPtr handle = (parent is null ? IntPtr.Zero : parent.Handle);

            // Try to get the page guid
            if (obj is IPerPropertyBrowsing.Interface perPropertyBrowsing)
            {
                // Check for a property page.
                Guid guid = Guid.Empty;
                HRESULT hr = perPropertyBrowsing.MapPropertyToPage((int)DispatchID.MEMBERID_NIL, &guid);
                if (hr.Succeeded & !guid.Equals(Guid.Empty))
                {
                    IntPtr pUnk = Marshal.GetIUnknownForObject(obj);
                    try
                    {
                        Oleaut32.OleCreatePropertyFrame(
                            new HandleRef(parent, handle),
                            0,
                            0,
                            "PropertyPages",
                            1,
                            &pUnk,
                            1,
                            &guid,
                            PInvoke.GetThreadLocale(),
                            0,
                            IntPtr.Zero);
                        return true;
                    }
                    finally
                    {
                        Marshal.Release(pUnk);
                    }
                }
            }

            if (obj is ISpecifyPropertyPages ispp)
            {
                try
                {
                    var uuids = default(CAUUID);
                    HRESULT hr = ispp.GetPages(&uuids);
                    if (!hr.Succeeded || uuids.cElems == 0)
                    {
                        return false;
                    }

                    IntPtr pUnk = Marshal.GetIUnknownForObject(obj);
                    try
                    {
                        Oleaut32.OleCreatePropertyFrame(
                            new HandleRef(parent, handle),
                            0,
                            0,
                            "PropertyPages",
                            1,
                            &pUnk,
                            uuids.cElems,
                            uuids.pElems,
                            PInvoke.GetThreadLocale(),
                            0,
                            IntPtr.Zero);
                        return true;
                    }
                    finally
                    {
                        Marshal.Release(pUnk);
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
                        RTLAwareMessageBox.Show(null, errString, SR.PropertyGridTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1, 0);
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
}
