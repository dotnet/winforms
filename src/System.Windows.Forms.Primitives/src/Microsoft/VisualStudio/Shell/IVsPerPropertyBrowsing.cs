// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Microsoft.VisualStudio.Shell;

/// <inheritdoc cref="Interface"/>
internal unsafe struct IVsPerPropertyBrowsing : IComIID
{
    public static Guid Guid { get; } = new(0x0FF510A3, 0x5FA5, 0x49F1, 0x8C, 0xCC, 0x19, 0x0D, 0x71, 0x08, 0x3F, 0x3E);

    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data =
            [
                0xa3, 0x10, 0xf5, 0x0f,
                0xa5, 0x5f,
                0xf1, 0x49,
                0x8c, 0xcc, 0x19, 0x0d, 0x71, 0x08, 0x3f, 0x3e
            ];

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    private readonly void** _lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, Guid*, void**, HRESULT>)_lpVtbl[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef"/>
    public uint AddRef()
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, uint>)_lpVtbl[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release"/>
    public uint Release()
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, uint>)_lpVtbl[2])(pThis);
    }

    /// <inheritdoc cref="Interface.HideProperty(int, BOOL*)"/>
    public HRESULT HideProperty(int dispid, BOOL* pfHide)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, BOOL*, HRESULT>)_lpVtbl[3])(pThis, dispid, pfHide);
    }

    /// <inheritdoc cref="Interface.DisplayChildProperties(int, BOOL*)"/>
    public HRESULT DisplayChildProperties(int dispid, BOOL* pfDisplay)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, BOOL*, HRESULT>)_lpVtbl[4])(pThis, dispid, pfDisplay);
    }

    /// <inheritdoc cref="Interface.GetLocalizedPropertyInfo(int, uint, BSTR*, BSTR*)"/>
    public HRESULT GetLocalizedPropertyInfo(
        int dispid,
        uint localeID,
        BSTR* pbstrLocalizedName,
        BSTR* pbstrLocalizeDescription)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, uint, BSTR*, BSTR*, HRESULT>)_lpVtbl[5])(
                pThis,
                dispid,
                localeID,
                pbstrLocalizedName,
                pbstrLocalizeDescription);
    }

    /// <inheritdoc cref="Interface.HasDefaultValue(int, BOOL*)"/>
    public HRESULT HasDefaultValue(
        int dispid,
        BOOL* fDefault)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, BOOL*, HRESULT>)_lpVtbl[6])(pThis, dispid, fDefault);
    }

    /// <inheritdoc cref="Interface.IsPropertyReadOnly(int, BOOL*)"/>
    public HRESULT IsPropertyReadOnly(
        int dispid,
        BOOL* fReadOnly)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, BOOL*, HRESULT>)_lpVtbl[7])(pThis, dispid, fReadOnly);
    }

    /// <inheritdoc cref="Interface.GetClassName(BSTR*)"/>
    public HRESULT GetClassName(
        BSTR* pbstrClassName)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, BSTR*, HRESULT>)_lpVtbl[8])(pThis, pbstrClassName);
    }

    /// <inheritdoc cref="Interface.CanResetPropertyValue(int, BOOL*)"/>
    public HRESULT CanResetPropertyValue(
        int dispid,
        BOOL* pfCanReset)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, BOOL*, HRESULT>)_lpVtbl[9])(pThis, dispid, pfCanReset);
    }

    /// <inheritdoc cref="Interface.ResetPropertyValue(int)"/>
    public HRESULT ResetPropertyValue(
        int dispid)
    {
        fixed (IVsPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVsPerPropertyBrowsing*, int, HRESULT>)_lpVtbl[9])(pThis, dispid);
    }

    [ComImport]
    [Guid("0FF510A3-5FA5-49F1-8CCC-190D71083F3E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface Interface
    {
        /// <summary>
        ///  Hides the property at the given <paramref name="dispid"/> from the properties window. Implementers should
        ///  return E_NOTIMPL to show all properties that are otherwise browsable.
        /// </summary>
        [PreserveSig]
        HRESULT HideProperty(
            int dispid,
            BOOL* pfHide);

        /// <summary>
        ///  Will have the "+" expandable glyph next to it and can be expanded or collapsed by the user. Returning any
        ///  result other than <see cref="HRESULT.S_OK"/> or false for <paramref name="pfDisplay"/> will suppress this feature.
        /// </summary>
        [PreserveSig]
        HRESULT DisplayChildProperties(
            int dispid,
            BOOL* pfDisplay);

        /// <summary>
        ///  Retrieves the localized name and description for a property. Returning any result other than
        ///  <see cref="HRESULT.S_OK"/> return code will display the default values.
        /// </summary>
        [PreserveSig]
        HRESULT GetLocalizedPropertyInfo(
            int dispid,
            uint localeID,
            BSTR* pbstrLocalizedName,
            BSTR* pbstrLocalizeDescription);

        /// <summary>
        ///  Determines if the given (usually current) value for a property is the default. If it is not default,
        ///  the property will be shown as bold in the browser to indicate that it has been modified from the default.
        /// </summary>
        [PreserveSig]
        HRESULT HasDefaultValue(
            int dispid,
            BOOL* fDefault);

        /// <summary>
        ///  Determines if a property should be made read only. This only applies to properties that are writeable.
        /// </summary>
        [PreserveSig]
        HRESULT IsPropertyReadOnly(
            int dispid,
            BOOL* fReadOnly);

        /// <summary>
        ///  Returns the class name for this object. The class name is the non-bolded text that appears in the properties
        ///  window selection combo. Returning any result other than <see cref="HRESULT.S_OK"/> will cause the default to
        ///  be used. The default is the name string from a call to
        ///  <see cref="ITypeInfo.GetDocumentation(int, BSTR*, BSTR*, out uint, BSTR*)"/> with <see cref="PInvoke.MEMBERID_NIL"/>.
        /// </summary>
        [PreserveSig]
        HRESULT GetClassName(
            BSTR* pbstrClassName);

        /// <summary>
        ///  Checks whether the given property can be reset to some default value. Returning any result other than
        ///  <see cref="HRESULT.S_OK"/> or false for <paramref name="pfCanReset"/> will not allow the value to be reset.
        /// </summary>
        [PreserveSig]
        HRESULT CanResetPropertyValue(
            int dispid,
            BOOL* pfCanReset);

        /// <summary>
        ///  If the return value is <see cref="HRESULT.S_OK"/>, the property's value will then be refreshed to the
        ///  new default values.
        /// </summary>
        [PreserveSig]
        HRESULT ResetPropertyValue(
            int dispid);
    }
}
