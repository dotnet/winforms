// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using static Windows.Win32.System.Ole.FDEX_PROP_FLAGS;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IDispatch
{
    /// <summary>
    ///  Get the specified <paramref name="dispId"/> property.
    /// </summary>
    internal HRESULT TryGetProperty(
        uint dispId,
        VARIANT* pVar,
        uint lcid = 0) => TryGetProperty((int)dispId, pVar, lcid);

    /// <summary>
    ///  Get the specified <paramref name="dispId"/> property.
    /// </summary>
    internal HRESULT TryGetProperty(
        int dispId,
        VARIANT* pVar,
        uint lcid = 0)
    {
        fixed (IDispatch* dispatch = &this)
        {
            Guid riid = Guid.Empty;
            DISPPARAMS disparams = default;
            EXCEPINFO pExcepInfo = default;
            HRESULT hr = dispatch->Invoke(
                dispId,
                &riid,
                lcid,
                DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                &disparams,
                pVar,
                &pExcepInfo,
                puArgErr: null);

            if (hr == HRESULT.DISP_E_EXCEPTION)
            {
                if (pExcepInfo.scode != 0)
                {
                    hr = (HRESULT)pExcepInfo.scode;
                }

                ClearStrings(ref pExcepInfo);
            }

            return hr;
        }
    }

    private static void ClearStrings(ref EXCEPINFO exceptionInfo)
    {
#if DEBUG
        Debug.WriteLine($"""
            Exception on property access.
                Description: {exceptionInfo.bstrDescription.ToStringAndFree()}
                Source: {exceptionInfo.bstrSource.ToStringAndFree()}
                Help file: {exceptionInfo.bstrHelpFile.ToStringAndFree()}
            """);
#else
        exceptionInfo.bstrDescription.Dispose();
        exceptionInfo.bstrSource.Dispose();
        exceptionInfo.bstrHelpFile.Dispose();
#endif
    }

    /// <summary>
    ///  Get the specified <paramref name="dispId"/> property.
    /// </summary>
    internal VARIANT GetProperty(
        uint dispId,
        uint lcid = 0) => GetProperty((int)dispId, lcid);

    /// <summary>
    ///  Get the specified <paramref name="dispId"/> property.
    /// </summary>
    internal VARIANT GetProperty(
        int dispId,
        uint lcid = 0)
    {
        VARIANT variant = default;
        TryGetProperty(dispId, &variant, lcid).ThrowOnFailure();
        return variant;
    }

    public HRESULT SetPropertyValue(int dispatchId, VARIANT value, out string? errorText)
    {
        Guid guid = Guid.Empty;
        EXCEPINFO pExcepInfo = default;
        int putDispatchID = PInvokeCore.DISPID_PROPERTYPUT;
        errorText = null;

        DISPPARAMS dispParams = new()
        {
            cArgs = 1,
            cNamedArgs = 1,
            // You HAVE to name the put argument or you'll get DISP_E_PARAMNOTFOUND
            rgdispidNamedArgs = &putDispatchID,
            rgvarg = &value
        };

        uint argumentError;

        HRESULT hr = Invoke(
            dispatchId,
            &guid,
            PInvokeCore.GetThreadLocale(),
            DISPATCH_FLAGS.DISPATCH_PROPERTYPUT,
            &dispParams,
            null,
            &pExcepInfo,
            &argumentError);

        if (hr == HRESULT.DISP_E_EXCEPTION)
        {
            if (pExcepInfo.scode != 0)
            {
                hr = (HRESULT)pExcepInfo.scode;
            }

            errorText = pExcepInfo.bstrDescription.ToString();
            ClearStrings(ref pExcepInfo);
        }

        return hr;
    }

    /// <inheritdoc cref="GetIDsOfNames(Guid*, PWSTR*, uint, uint, int*)"/>
    internal HRESULT GetIDOfName(string name, out int dispId)
    {
        int id = 0;

        fixed (char* n = name)
        {
            HRESULT result = GetIDsOfNames(IID.NULL(), (PWSTR*)&n, 1u, PInvokeCore.GetThreadLocale(), &id);
            dispId = id;
            return result;
        }
    }

    // Prop flags for fields, methods, and properties follow the same
    // values as defined in https://github.com/dotnet/runtime/blob/main/src/coreclr/vm/stdinterfaces.cpp#L1780

    // private static FDEX_PROP_FLAGS GetFieldFlags()
    // {
    //     return
    //         fdexPropCanGet
    //         | fdexPropCanPut
    //         | fdexPropCannotPutRef
    //         | fdexPropCannotCall
    //         | fdexPropCannotConstruct
    //         | fdexPropCannotSourceEvents;
    // }

    internal static FDEX_PROP_FLAGS GetMethodFlags()
        => fdexPropCannotGet
            | fdexPropCannotPut
            | fdexPropCannotPutRef
            | fdexPropCanCall
            | fdexPropCannotConstruct
            | fdexPropCannotSourceEvents;

    internal static FDEX_PROP_FLAGS GetPropertyFlags(bool canRead, bool canWrite)
        => (canRead ? fdexPropCanGet : fdexPropCannotGet)
            | (canWrite ? fdexPropCanPut : fdexPropCannotPut)
            | fdexPropCannotPutRef
            | fdexPropCannotCall
            | fdexPropCannotConstruct
            | fdexPropCannotSourceEvents;
}
