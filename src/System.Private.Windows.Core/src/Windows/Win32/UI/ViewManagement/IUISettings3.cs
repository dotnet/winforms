// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.ViewManagement;

/// <summary>
///  WinRT ABI for <c>Windows.UI.ViewManagement.IUISettings3</c>.
/// </summary>
/// <remarks>
///  <para>
///   Manually defined as the type lives in WinRT metadata, not Win32 metadata,
///   and we do not want a CsWinRT projection dependency. Slots 3-5 are the
///   <c>IInspectable</c> methods.
///  </para>
/// </remarks>
internal unsafe struct IUISettings3 : IComIID
{
    private readonly void** _vtbl;

    // {03021BE4-5254-4781-8194-5168F7D06D7B}
    public static Guid IID_Guid { get; } = new(0x03021be4, 0x5254, 0x4781, 0x81, 0x94, 0x51, 0x68, 0xf7, 0xd0, 0x6d, 0x7b);

    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data =
            [
                // 0x03021be4, 0x5254, 0x4781, 0x81, 0x94, 0x51, 0x68, 0xf7, 0xd0, 0x6d, 0x7b
                0xe4, 0x1b, 0x02, 0x03, 0x54, 0x52, 0x81, 0x47, 0x81, 0x94, 0x51, 0x68, 0xf7, 0xd0, 0x6d, 0x7b
            ];

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IUISettings3* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IUISettings3*, Guid*, void**, HRESULT>)_vtbl[0])(pThis, riid, ppvObject);
    }

    public uint AddRef()
    {
        fixed (IUISettings3* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IUISettings3*, uint>)_vtbl[1])(pThis);
    }

    public uint Release()
    {
        fixed (IUISettings3* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IUISettings3*, uint>)_vtbl[2])(pThis);
    }

    // Slots 3-5: IInspectable::GetIids, GetRuntimeClassName, GetTrustLevel (unused).

    public HRESULT GetColorValue(UIColorType desiredColor, UIColor* value)
    {
        fixed (IUISettings3* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IUISettings3*, UIColorType, UIColor*, HRESULT>)_vtbl[6])(pThis, desiredColor, value);
    }
}
