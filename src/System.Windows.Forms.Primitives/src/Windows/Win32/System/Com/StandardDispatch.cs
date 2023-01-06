// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Windows.Win32.System.Ole;

namespace Windows.Win32.System.Com;

/// <summary>
///  Base class for providing <see cref="IDispatch"/> services through a standard dispatch implementation
///  generated from a type library.
/// </summary>
internal abstract unsafe class StandardDispatch : IDispatch.Interface, IDispatchEx.Interface, IWrapperInitialize, IDisposable
{
    private readonly Guid _typeLibrary;
    private readonly ushort _majorVersion;
    private readonly ushort _minorVersion;
    private readonly Guid _interfaceId;
    private AgileComPointer<IDispatch>? _standardDispatch;

    // StdOle32.tlb
    private static readonly Guid s_stdole = new("00020430-0000-0000-C000-000000000046");

    /// <summary>
    ///  Construct an <see cref="IUnknown"/> instance. This is useful as a replacement for types exposed as
    ///  <see cref="IDispatch"/> and <see cref="IDispatchEx"/> purely through <see cref="IReflect"/>.
    /// </summary>
    public StandardDispatch() : this(s_stdole, 2, 0, IUnknown.IID_Guid)
    {
    }

    /// <summary>
    ///  Construct a new instance from a registered type library.
    /// </summary>
    /// <param name="typeLibrary"><see cref="Guid"/> for the registered type library.</param>
    /// <param name="majorVersion">Type library major version.</param>
    /// <param name="minorVersion">Type library minor version.</param>
    /// <param name="interfaceId">The <see cref="Guid"/> for the interface the derived class presents.</param>
    public StandardDispatch(
        Guid typeLibrary,
        ushort majorVersion,
        ushort minorVersion,
        Guid interfaceId)
    {
        _typeLibrary = typeLibrary;
        _majorVersion = majorVersion;
        _minorVersion = minorVersion;
        _interfaceId = interfaceId;
    }

    void IWrapperInitialize.OnInitialized(IUnknown* unknown)
    {
        if (_standardDispatch is not null)
        {
            // Already configured.
            return;
        }

        // Load the registered type library and get the relevant ITypeInfo for the specified interface.
        using ComScope<ITypeLib> typelib = new(null);
        HRESULT hr = PInvoke.LoadRegTypeLib(_typeLibrary, _majorVersion, _minorVersion, 0, typelib);
        hr.ThrowOnFailure();

        using ComScope<ITypeInfo> typeinfo = new(null);
        typelib.Value->GetTypeInfoOfGuid(_interfaceId, typeinfo);

        // The unknown we get is a wrapper unknown.
        unknown->QueryInterface(_interfaceId, out void* instance).ThrowOnFailure();
        IUnknown* standard = default;
        PInvoke.CreateStdDispatch(
            unknown,
            instance,
            typeinfo.Value,
            &standard).ThrowOnFailure();

        _standardDispatch = new AgileComPointer<IDispatch>((IDispatch*)standard, takeOwnership: true);
    }

    private ComScope<IDispatch> Dispatch =>
        _standardDispatch is not { } standardDispatch
            ? throw new InvalidOperationException()
            : standardDispatch.GetInterface();

    HRESULT IDispatch.Interface.GetTypeInfoCount(uint* pctinfo)
    {
        using var dispatch = Dispatch;
        dispatch.Value->GetTypeInfoCount(pctinfo);
        return HRESULT.S_OK;
    }

    HRESULT IDispatch.Interface.GetTypeInfo(uint iTInfo, uint lcid, ITypeInfo** ppTInfo)
    {
        using var dispatch = Dispatch;
        dispatch.Value->GetTypeInfo(iTInfo, lcid, ppTInfo);
        return HRESULT.S_OK;
    }

    HRESULT IDispatch.Interface.GetIDsOfNames(Guid* riid, PWSTR* rgszNames, uint cNames, uint lcid, int* rgDispId)
    {
        using var dispatch = Dispatch;
        dispatch.Value->GetIDsOfNames(riid, rgszNames, cNames, lcid, rgDispId);
        return HRESULT.S_OK;
    }

    HRESULT IDispatch.Interface.Invoke(
        int dispIdMember,
        Guid* riid,
        uint lcid,
        DISPATCH_FLAGS wFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        uint* pArgErr)
    {
        HRESULT hr = MapDotNetHRESULTs(Invoke(
            dispIdMember,
            lcid,
            wFlags,
            pDispParams,
            pVarResult,
            pExcepInfo,
            pArgErr));

        if (hr != HRESULT.DISP_E_MEMBERNOTFOUND)
        {
            return hr;
        }

        // The override couldn't find it, pass it along to the standard dispatch.
        using var dispatch = Dispatch;
        hr = dispatch.Value->Invoke(dispIdMember, riid, lcid, wFlags, pDispParams, pVarResult, pExcepInfo, pArgErr);
        return hr;
    }

    HRESULT IDispatchEx.Interface.GetDispID(BSTR bstrName, uint grfdex, int* pid)
    => bstrName.IsNull || pid is null ? HRESULT.E_POINTER : GetDispID(bstrName, grfdex, pid);

    protected virtual HRESULT GetDispID(BSTR bstrName, uint grfdex, int* pid) => HRESULT.E_NOTIMPL;

    HRESULT IDispatchEx.Interface.GetMemberName(int id, BSTR* pbstrName)
        => pbstrName is null ? HRESULT.E_POINTER : GetMemberName(id, pbstrName);

    protected virtual HRESULT GetMemberName(int id, BSTR* pbstrName) => HRESULT.E_NOTIMPL;

    HRESULT IDispatchEx.Interface.GetNextDispID(uint grfdex, int id, int* pid)
    {
        if (pid is null)
        {
            return HRESULT.E_POINTER;
        }

        *pid = PInvoke.DISPID_UNKNOWN;

        return GetNextDispID(grfdex, id, pid);
    }

    protected virtual HRESULT GetNextDispID(uint grfdex, int id, int* pid) => HRESULT.E_NOTIMPL;

    HRESULT IDispatchEx.Interface.InvokeEx(
        int id,
        uint lcid,
        ushort wFlags,
        DISPPARAMS* pdp,
        VARIANT* pvarRes,
        EXCEPINFO* pei,
        IServiceProvider* pspCaller)
    {
        HRESULT hr = MapDotNetHRESULTs(Invoke(
            id,
            lcid,
            (DISPATCH_FLAGS)wFlags,
            pdp,
            pvarRes,
            pei,
            argumentError: null));

        if (hr != HRESULT.DISP_E_MEMBERNOTFOUND)
        {
            return hr;
        }

        // The override couldn't find it, pass it along to the standard dispatch.
        using var dispatch = Dispatch;
        hr = dispatch.Value->Invoke(id, IID.NULL(), lcid, (DISPATCH_FLAGS)wFlags, pdp, pvarRes, pei, puArgErr: null);
        return hr;
    }

    protected virtual HRESULT Invoke(
        int dispId,
        uint lcid,
        DISPATCH_FLAGS flags,
        DISPPARAMS* parameters,
        VARIANT* result,
        EXCEPINFO* exceptionInfo,
        uint* argumentError)
        => HRESULT.DISP_E_MEMBERNOTFOUND;

    HRESULT IDispatchEx.Interface.GetMemberProperties(int id, uint grfdexFetch, FDEX_PROP_FLAGS* pgrfdex)
    {
        if (pgrfdex is null)
        {
            return HRESULT.E_POINTER;
        }

        if (id == PInvoke.DISPID_UNKNOWN)
        {
            return HRESULT.E_INVALIDARG;
        }

        *pgrfdex = default;

        HRESULT hr = GetMemberProperties(id, out FDEX_PROP_FLAGS properties);
        if (hr.Succeeded)
        {
            // Filter to the requested properties
            *pgrfdex = properties & (FDEX_PROP_FLAGS)grfdexFetch;
        }
        else
        {
            *pgrfdex = default;
        }

        return hr;
    }

    protected virtual HRESULT GetMemberProperties(int dispId, out FDEX_PROP_FLAGS properties)
    {
        properties = default;
        return HRESULT.E_NOTIMPL;
    }

    // .NET COM Interop returns E_NOTIMPL for these three.

    HRESULT IDispatchEx.Interface.DeleteMemberByName(BSTR bstrName, uint grfdex) => HRESULT.E_NOTIMPL;
    HRESULT IDispatchEx.Interface.DeleteMemberByDispID(int id) => HRESULT.E_NOTIMPL;

    HRESULT IDispatchEx.Interface.GetNameSpaceParent(IUnknown** ppunk)
    {
        if (ppunk is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppunk = null;
        return HRESULT.E_NOTIMPL;
    }

    private static FDEX_PROP_FLAGS GetFuncDescProperties(FUNCDESC* funcdesc)
    {
        FDEX_PROP_FLAGS flags = default;

        INVOKEKIND invokekind = funcdesc->invkind;
        flags |= invokekind.HasFlag(INVOKEKIND.INVOKE_PROPERTYPUT)
            ? FDEX_PROP_FLAGS.fdexPropCanPut
            : FDEX_PROP_FLAGS.fdexPropCannotPut;

        flags |= invokekind.HasFlag(INVOKEKIND.INVOKE_PROPERTYPUTREF)
            ? FDEX_PROP_FLAGS.fdexPropCanPutRef
            : FDEX_PROP_FLAGS.fdexPropCannotPutRef;

        flags |= invokekind.HasFlag(INVOKEKIND.INVOKE_PROPERTYGET)
            ? FDEX_PROP_FLAGS.fdexPropCanGet
            : FDEX_PROP_FLAGS.fdexPropCannotGet;

        flags |= invokekind.HasFlag(INVOKEKIND.INVOKE_FUNC)
            ? FDEX_PROP_FLAGS.fdexPropCanCall
            : FDEX_PROP_FLAGS.fdexPropCannotCall;

        flags |= FDEX_PROP_FLAGS.fdexPropCannotConstruct | FDEX_PROP_FLAGS.fdexPropCannotSourceEvents;

        return flags;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _standardDispatch?.Dispose();
            _standardDispatch = null;
        }
    }

    private static HRESULT MapDotNetHRESULTs(HRESULT hr)
    {
        if (hr == HRESULT.COR_E_OVERFLOW)
        {
            return HRESULT.DISP_E_OVERFLOW;
        }
        else if (hr == HRESULT.COR_E_INVALIDOLEVARIANTTYPE)
        {
            return HRESULT.DISP_E_BADVARTYPE;
        }
        else if (hr == HRESULT.COR_E_ARGUMENT)
        {
            return HRESULT.E_INVALIDARG;
        }
        else if (hr == HRESULT.COR_E_SAFEARRAYTYPEMISMATCH)
        {
            return HRESULT.DISP_E_TYPEMISMATCH;
        }
        else if (hr == HRESULT.COR_E_MISSINGMEMBER || hr == HRESULT.COR_E_MISSINGMETHOD)
        {
            return HRESULT.DISP_E_MEMBERNOTFOUND;
        }

        // .NET maps this, we would need to populate EXCEPINFO to do the same
        //
        // else if (hr == HRESULT.COR_E_TARGETINVOCATION)
        // {
        //     return HRESULT.DISP_E_EXCEPTION;
        // }

        return hr;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
