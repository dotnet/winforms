// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace Microsoft.VisualStudio.Shell;

/// <inheritdoc cref="Interface"/>
internal unsafe struct IVSMDPerPropertyBrowsing : IComIID
{
    internal static Guid Guid { get; } = new(0x7494683C, 0x37A0, 0x11D2, 0xA2, 0x73, 0x00, 0xC0, 0x4F, 0x8E, 0xF4, 0xFF);

    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data =
            [
                0x3c, 0x68, 0x94, 0x74,
                0xa0, 0x37,
                0xd2, 0x11,
                0xa2, 0x73, 0x00, 0xc0, 0x4f, 0x8e, 0xf4, 0xFF
            ];

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    private readonly void** _lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IVSMDPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVSMDPerPropertyBrowsing*, Guid*, void**, HRESULT>)_lpVtbl[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef"/>
    public uint AddRef()
    {
        fixed (IVSMDPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVSMDPerPropertyBrowsing*, uint>)_lpVtbl[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release"/>
    public uint Release()
    {
        fixed (IVSMDPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVSMDPerPropertyBrowsing*, uint>)_lpVtbl[2])(pThis);
    }

    /// <inheritdoc cref="Interface.GetPropertyAttributes(int, uint*, BSTR**, VARIANT**)"/>
    public HRESULT GetPropertyAttributes(
        int dispid,
        uint* pceltAttrs,
        BSTR** ppbstrTypeNames,
        VARIANT** ppvarAttrValues)
    {
        fixed (IVSMDPerPropertyBrowsing* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IVSMDPerPropertyBrowsing*, int, uint*, BSTR**, VARIANT**, HRESULT>)_lpVtbl[3])(
                pThis,
                dispid,
                pceltAttrs,
                ppbstrTypeNames,
                ppvarAttrValues);
    }

    /// <summary>
    ///  This interface allows native COM objects to specify managed attributes on values. It can be used to expose
    ///  the richer functionality of managed objects as COM objects.
    /// </summary>
    [ComImport]
    [Guid("7494683C-37A0-11d2-A273-00C04F8EF4FF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface Interface
    {
        /// <summary>
        ///  Gets the list of attributes for the object.
        /// </summary>
        /// <param name="dispid">The dispid of the property to retrieve attributes for.</param>
        /// <param name="pceltAttrs">The number of attribute type names in <paramref name="ppbstrTypeNames"/>.</param>
        /// <param name="ppbstrTypeNames">
        ///  Attribute type names, such as <see cref="System.ComponentModel.BrowsableAttribute"/>, or
        ///  see <see cref="System.ComponentModel.DescriptionAttribute"/>. This can be the name of any type that
        ///  derives from <see cref="Attribute"/>. The array is callee allocated and will be callee freed using
        ///  CoTaskMemFree. Strings themselves will be freed with SysFreeString. It can also be a static instance name
        ///  such as <see cref="System.ComponentModel.BrowsableAttribute.No"/>, which will cause the initializer value
        ///  to be ignored.
        /// </param>
        /// <param name="ppvarAttrValues">
        ///  An array of variants to be used to initialize the given attributes. If the attributes have a constructor
        ///  that takes a parameter, the given value will be used to iniitalize the attribute. If the initializer is
        ///  NULL, VT_EMPTY or VT_NULL, the default constructor will be called. Variants will be caller freed individually
        ///  using VariantClear then CoTaskMemFree on the array itself.
        /// </param>
        [PreserveSig]
        HRESULT GetPropertyAttributes(
            int dispid,
            uint* pceltAttrs,
            BSTR** ppbstrTypeNames,
            VARIANT** ppvarAttrValues);
    }
}
