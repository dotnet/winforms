// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.System.Com;

internal unsafe partial struct IDispatch
{
    /// <summary>
    ///  Get the specified <paramref name="dispId"/> property.
    /// </summary>
    internal HRESULT TryGetProperty(
        uint dispId,
        VARIANT* pVar,
        uint lcid = 0)
    {
        fixed (IDispatch* dispatch = &this)
        {
            Guid riid = Guid.Empty;
            DISPPARAMS disparams = default;
            return dispatch->Invoke(
                (int)dispId,
                &riid,
                lcid,
                DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                &disparams,
                pVar,
                pExcepInfo: null,
                puArgErr: null);
        }
    }

    /// <summary>
    ///  Get the specified <paramref name="dispId"/> property.
    /// </summary>
    internal VARIANT GetProperty(
        uint dispId,
        uint lcid = 0)
    {
        VARIANT variant = default;
        TryGetProperty(dispId, &variant, lcid).ThrowOnFailure();
        return variant;
    }

    public HRESULT SetPropertyValue(int dispatchId, VARIANT value)
    {
        Guid guid = Guid.Empty;
        EXCEPINFO pExcepInfo = default;
        VARIANT* arg = &value;
        int putDispatchID = PInvoke.DISPID_PROPERTYPUT;

        DISPPARAMS dispParams = new()
        {
            cArgs = 1,
            cNamedArgs = 1,
            // You HAVE to name the put argument or you'll get DISP_E_PARAMNOTFOUND
            rgdispidNamedArgs = &putDispatchID,
            rgvarg = arg
        };

        uint argumentError;

        HRESULT hr = Invoke(
            dispatchId,
            &guid,
            PInvoke.GetThreadLocale(),
            DISPATCH_FLAGS.DISPATCH_PROPERTYPUT,
            &dispParams,
            null,
            &pExcepInfo,
            &argumentError);

        return hr;
    }
}
