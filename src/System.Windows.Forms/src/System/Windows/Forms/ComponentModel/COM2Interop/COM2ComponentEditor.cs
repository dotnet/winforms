﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2ComponentEditor : WindowsFormsComponentEditor
    {
        public unsafe static bool NeedsComponentEditor(object obj)
        {
            if (obj is Ole32.IPerPropertyBrowsing)
            {
                // check for a property page
                Guid guid = Guid.Empty;
                HRESULT hr = ((Ole32.IPerPropertyBrowsing)obj).MapPropertyToPage(Ole32.DispatchID.MEMBERID_NIL, &guid);
                if ((hr == HRESULT.S_OK) && !guid.Equals(Guid.Empty))
                {
                    return true;
                }
            }

            if (obj is Ole32.ISpecifyPropertyPages ispp)
            {
                var uuids = new Ole32.CAUUID();
                try
                {
                    HRESULT hr = ispp.GetPages(&uuids);
                    if (!hr.Succeeded())
                    {
                        return false;
                    }

                    return uuids.cElems > 0;
                }
                finally
                {
                    if (uuids.pElems != null)
                    {
                        Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                    }
                }
            }

            return false;
        }

        public unsafe override bool EditComponent(ITypeDescriptorContext context, object obj, IWin32Window parent)
        {
            IntPtr handle = (parent == null ? IntPtr.Zero : parent.Handle);

            // try to get the page guid
            if (obj is Ole32.IPerPropertyBrowsing)
            {
                // check for a property page
                Guid guid = Guid.Empty;
                HRESULT hr = ((Ole32.IPerPropertyBrowsing)obj).MapPropertyToPage(Ole32.DispatchID.MEMBERID_NIL, &guid);
                if (hr == HRESULT.S_OK & !guid.Equals(Guid.Empty))
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
                            (uint)Application.CurrentCulture.LCID,
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

            if (obj is Ole32.ISpecifyPropertyPages ispp)
            {
                try
                {
                    var uuids = new Ole32.CAUUID();
                    HRESULT hr = ispp.GetPages(&uuids);
                    if (!hr.Succeeded() || uuids.cElems == 0)
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
                            (uint)Application.CurrentCulture.LCID,
                            0,
                            IntPtr.Zero);
                        return true;
                    }
                    finally
                    {
                        Marshal.Release(pUnk);
                        if (uuids.pElems != null)
                        {
                            Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errString = SR.ErrorPropertyPageFailed;

                    IUIService uiSvc = (context != null) ? ((IUIService)context.GetService(typeof(IUIService))) : null;

                    if (uiSvc == null)
                    {
                        RTLAwareMessageBox.Show(null, errString, SR.PropertyGridTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1, 0);
                    }
                    else if (ex != null)
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
