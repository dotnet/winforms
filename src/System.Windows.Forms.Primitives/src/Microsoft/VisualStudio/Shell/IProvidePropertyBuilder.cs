// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace Microsoft.VisualStudio.Shell;

internal unsafe struct IProvidePropertyBuilder : IComIID
{
    internal static Guid Guid { get; } = new(0x33C0C1D8, 0x33CF, 0x11d3, 0xBF, 0xF2, 0x00, 0xC0, 0x4F, 0x99, 0x02, 0x35);

    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data =
            [
                0xd8, 0xc1, 0xc0, 0x33,
                0xcf, 0x33,
                0xd3, 0x11,
                0xbf, 0xf2, 0x00, 0xc0, 0x4f, 0x99, 0x02, 0x35
            ];

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    private readonly void** _lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IProvidePropertyBuilder* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IProvidePropertyBuilder*, Guid*, void**, HRESULT>)_lpVtbl[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef"/>
    public uint AddRef()
    {
        fixed (IProvidePropertyBuilder* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IProvidePropertyBuilder*, uint>)_lpVtbl[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release"/>
    public uint Release()
    {
        fixed (IProvidePropertyBuilder* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IProvidePropertyBuilder*, uint>)_lpVtbl[2])(pThis);
    }

    public HRESULT MapPropertyToBuilder(
        int dispid,
        CTLBLDTYPE* pdwCtlBldType,
        BSTR* pbstrGuidBldr,
        VARIANT_BOOL* builderAvailable)
    {
        fixed (IProvidePropertyBuilder* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IProvidePropertyBuilder*, int, CTLBLDTYPE*, BSTR*, VARIANT_BOOL*, HRESULT>)_lpVtbl[3])(
                pThis,
                dispid,
                pdwCtlBldType,
                pbstrGuidBldr,
                builderAvailable);
    }

    public HRESULT ExecuteBuilder(
        int dispid,
        BSTR* bstrGuidBldr,
        IDispatch* pdispApp,
        HWND hwndBldrOwner,
        VARIANT* pvarValue,
        VARIANT_BOOL* pbActionCommitted)
    {
        fixed (IProvidePropertyBuilder* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IProvidePropertyBuilder*, int, BSTR*, IDispatch*, HWND, VARIANT*, VARIANT_BOOL*, HRESULT>)_lpVtbl[4])(
                pThis,
                dispid,
                bstrGuidBldr,
                pdispApp,
                hwndBldrOwner,
                pvarValue,
                pbActionCommitted);
    }

    /// <summary>
    ///  The <see cref="IProvidePropertyBuilder"/> interface, when implemented, allows objects to specify property builder
    ///  objects for properties. Builders are invoked by an ellipsis button ([...]) on the Microsoft Visual Studio property
    ///  browser and are invoked through <see cref="ExecuteBuilder(int, BSTR*, IDispatch*, HWND, VARIANT*, VARIANT_BOOL*)"/>
    ///  when the button is pressed. To supply a builder for a given property, return a GUID for the property builder that
    ///  should be invoked for the current property from <see cref="MapPropertyToBuilder(int, CTLBLDTYPE*, BSTR*, VARIANT_BOOL*)"/>
    ///  Builders are generally implemented through modal dialog boxes.
    /// </summary>
    [ComImport]
    [Guid("33C0C1D8-33CF-11d3-BFF2-00C04F990235")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface Interface
    {
        /// <summary>
        ///  Checks whether a builder should be associated with a particular property.
        /// </summary>
        /// <param name="dispid">The DISPID of the property in question.</param>
        /// <param name="pdwCtlBldType">The builder to be mapped.</param>
        /// <param name="pbstrGuidBldr">The GUID that identifies the builder for this property.</param>
        /// <param name="builderAvailable">
        ///  This parameter is <see cref="VARIANT_BOOL.VARIANT_TRUE"/> if this property currently supports a builder.
        /// </param>
        [PreserveSig]
        HRESULT MapPropertyToBuilder(
            int dispid,
            CTLBLDTYPE* pdwCtlBldType,
            BSTR* pbstrGuidBldr,
            VARIANT_BOOL* builderAvailable);

        /// <summary>
        ///  Notifies an object that it should display its builder for the specified property.
        /// </summary>
        /// <param name="dispid">The DISPID of the property for which the builder displays.</param>
        /// <param name="bstrGuidBldr">
        ///  The <see cref="BSTR"/> of the builder GUID to invoke. This is returned from
        ///  <see cref="MapPropertyToBuilder(int, CTLBLDTYPE*, BSTR*, VARIANT_BOOL*)"/>.
        /// </param>
        /// <param name="pdispApp">Set to NULL.</param>
        /// <param name="hwndBldrOwner">A handle to the parent pop-up builder window.</param>
        /// <param name="pvarValue">
        ///  The current value of the property. This value can be modified by the object and changes to the new value
        ///  if <paramref name="pbActionCommitted "/> is <see cref="VARIANT_BOOL.VARIANT_TRUE"/>.
        /// </param>
        /// <param name="pbActionCommitted ">
        ///  A value that indicates whether the builder performed an action on the object. Can be used when a user
        ///  modifies something, then presses OK on the pop-up builder dialog box.
        /// </param>
        [PreserveSig]
        HRESULT ExecuteBuilder(
            int dispid,
            BSTR* bstrGuidBldr,
            IDispatch* pdispApp,
            HWND hwndBldrOwner,
            VARIANT* pvarValue,
            VARIANT_BOOL* pbActionCommitted);
    }
}
