// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Microsoft.VisualStudio.Shell;

/// <inheritdoc cref="Interface"/>
internal unsafe struct ICategorizeProperties : IComIID
{
    internal static Guid Guid { get; } = new(0x4D07FC10, 0xF931, 0x11CE, 0xB0, 0x01, 0x00, 0xAA, 0x00, 0x68, 0x84, 0xE5);

    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data =
            [
                0x10, 0xfc, 0x07, 0x4d,
                0x31, 0xf9,
                0xce, 0x11,
                0xb0, 0x01, 0x00, 0xaa, 0x00, 0x68, 0x84, 0xe5
            ];

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    private readonly void** _lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (ICategorizeProperties* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<ICategorizeProperties*, Guid*, void**, HRESULT>)_lpVtbl[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef"/>
    public uint AddRef()
    {
        fixed (ICategorizeProperties* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<ICategorizeProperties*, uint>)_lpVtbl[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release"/>
    public uint Release()
    {
        fixed (ICategorizeProperties* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<ICategorizeProperties*, uint>)_lpVtbl[2])(pThis);
    }

    internal HRESULT MapPropertyToCategory(
        int dispid,
        PROPCAT* ppropcat)
    {
        fixed (ICategorizeProperties* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<ICategorizeProperties*, int, PROPCAT*, HRESULT>)_lpVtbl[2])(pThis, dispid, ppropcat);
    }

    internal HRESULT GetCategoryName(
        PROPCAT propcat,
        int lcid,
        BSTR* pbstrName)
    {
        fixed (ICategorizeProperties* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<ICategorizeProperties*, PROPCAT, int, BSTR*, HRESULT>)_lpVtbl[2])(pThis, propcat, lcid, pbstrName);
    }

    /// <summary>
    ///  Provides category names and maps categories to properties for display in the Properties window.
    /// </summary>
    [ComImport]
    [Guid("4D07FC10-F931-11CE-B001-00AA006884E5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface Interface
    {
        /// <summary>
        ///  Returns the property category value for the specified property.
        /// </summary>
        /// <param name="dispid">Specifies the dispatch ID of the property to be displayed.</param>
        /// <param name="ppropcat">Specifies a pointer to the property category.</param>
        HRESULT MapPropertyToCategory(
            int dispid,
            PROPCAT* ppropcat);

        /// <summary>
        ///  Returns a <see cref="BSTR"/> containing the category name.
        /// </summary>
        /// <param name="propcat">Specifies the property category.</param>
        /// <param name="lcid">Locale identifier.</param>
        /// <param name="pbstrName">Pointer to a string containing the category name.</param>
        HRESULT GetCategoryName(
            PROPCAT propcat,
            int lcid,
            BSTR* pbstrName);
    }
}
