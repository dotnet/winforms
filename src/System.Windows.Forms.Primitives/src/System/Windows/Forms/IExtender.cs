// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

using static WinFormsComWrappers;

namespace System.Windows.Forms;

// It appears that WFC originally defined this interface under this GUID. Interestingly it appears to
// have been defined as IDispatch there. There is no reference to this guid in Windows sources outside of
// code that was copied from Windows Forms in the first place. It appears that this was inspired by
// IExtender in VB for their Extender Control, which used the same IExtender interface name with similar
// properties, but under a completely different GUID and DISPIDs.
//
// There don't seem to be any references to this interface on the web. No references were found for this
// interface in OLE specifications from the mid 90s. Looks to be purely our own projection and it might be
// unused in the wild.
//
// ExtenderProxy, which implements this, gets it's IDispatch view of Control via IReflect.
//
// WFC - Windows Foundation Classes, the basis of Windows Forms

/// <inheritdoc cref="Interface"/>
internal unsafe struct IExtender : IComIID, IVTable<IExtender, IExtender.Vtbl>
{
    private readonly void** _vtable;

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    public unsafe HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IExtender* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IExtender*, Guid*, void**, HRESULT>)_vtable[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef()"/>
    public uint AddRef()
    {
        fixed (IExtender* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IExtender*, uint>)_vtable[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release()"/>
    public uint Release()
    {
        fixed (IExtender* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IExtender*, uint>)_vtable[2])(pThis);
    }

    public unsafe struct Vtbl
    {
#pragma warning disable IDE1006 // Naming Styles
        internal delegate* unmanaged[Stdcall]<IExtender*, Guid*, void**, HRESULT> QueryInterface_1;
        internal delegate* unmanaged[Stdcall]<IExtender*, uint> AddRef_2;
        internal delegate* unmanaged[Stdcall]<IExtender*, uint> Release_3;
        internal delegate* unmanaged[Stdcall]<IExtender*, int*, HRESULT> get_Align_4;
        internal delegate* unmanaged[Stdcall]<IExtender*, int, HRESULT> set_Align_5;
        internal delegate* unmanaged[Stdcall]<IExtender*, BOOL*, HRESULT> get_Enabled_6;
        internal delegate* unmanaged[Stdcall]<IExtender*, BOOL, HRESULT> set_Enabled_7;
        internal delegate* unmanaged[Stdcall]<IExtender*, int*, HRESULT> get_Height_8;
        internal delegate* unmanaged[Stdcall]<IExtender*, int, HRESULT> set_Height_9;
        internal delegate* unmanaged[Stdcall]<IExtender*, int*, HRESULT> get_Left_10;
        internal delegate* unmanaged[Stdcall]<IExtender*, int, HRESULT> set_Left_11;
        internal delegate* unmanaged[Stdcall]<IExtender*, BOOL*, HRESULT> get_TabStop_12;
        internal delegate* unmanaged[Stdcall]<IExtender*, BOOL, HRESULT> set_TabStop_13;
        internal delegate* unmanaged[Stdcall]<IExtender*, int*, HRESULT> get_Top_14;
        internal delegate* unmanaged[Stdcall]<IExtender*, int, HRESULT> set_Top_15;
        internal delegate* unmanaged[Stdcall]<IExtender*, BOOL*, HRESULT> get_Visible_16;
        internal delegate* unmanaged[Stdcall]<IExtender*, BOOL, HRESULT> set_Visible_17;
        internal delegate* unmanaged[Stdcall]<IExtender*, int*, HRESULT> get_Width_18;
        internal delegate* unmanaged[Stdcall]<IExtender*, int, HRESULT> set_Width_19;
        internal delegate* unmanaged[Stdcall]<IExtender*, BSTR*, HRESULT> get_Name_20;
        internal delegate* unmanaged[Stdcall]<IExtender*, IUnknown**, HRESULT> get_Parent_21;
        internal delegate* unmanaged[Stdcall]<IExtender*, HWND*, HRESULT> get_Hwnd_22;
        internal delegate* unmanaged[Stdcall]<IExtender*, IUnknown**, HRESULT> get_Container_23;
        internal delegate* unmanaged[Stdcall]<IExtender*, void*, void*, void*, void*, HRESULT> Move_24;
#pragma warning restore IDE1006
    }

    // 39088D7E-B71E-11D1-8F39-00C04FD946D0
    /// <summary>The IID guid for this interface.</summary>
    public static Guid IID_Guid { get; } = new(0x39088D7E, 0xB71E, 0x11D1, 0x8F, 0x39, 0x00, 0xC0, 0x4F, 0xD9, 0x46, 0xD0);

    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data =
            [
                0x7e, 0x8d, 0x08, 0x39,
                0x1e, 0xb7,
                0xd1, 0x11,
                0x8f, 0x39, 0x00, 0xc0, 0x4f, 0xd9, 0x46, 0xd0
            ];

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public static void PopulateVTable(Vtbl* vtable)
    {
        vtable->get_Align_4 = &get_Align;
        vtable->set_Align_5 = &set_Align;
        vtable->get_Enabled_6 = &get_Enabled;
        vtable->set_Enabled_7 = &set_Enabled;
        vtable->get_Height_8 = &get_Height;
        vtable->set_Height_9 = &set_Height;
        vtable->get_Left_10 = &get_Left;
        vtable->set_Left_11 = &set_Left;
        vtable->get_TabStop_12 = &get_TabStop;
        vtable->set_TabStop_13 = &set_TabStop;
        vtable->get_Top_14 = &get_Top;
        vtable->set_Top_15 = &set_Top;
        vtable->get_Visible_16 = &get_Visible;
        vtable->set_Visible_17 = &set_Visible;
        vtable->get_Width_18 = &get_Width;
        vtable->set_Width_19 = &set_Width;
        vtable->get_Name_20 = &get_Name;
        vtable->get_Parent_21 = &get_Parent;
        vtable->get_Hwnd_22 = &get_Hwnd;
        vtable->get_Container_23 = &get_Container;
        vtable->Move_24 = &Move;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Align(IExtender* @this, int* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Align);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Align(IExtender* @this, int value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Align = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Enabled(IExtender* @this, BOOL* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Enabled);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Enabled(IExtender* @this, BOOL value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Enabled = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Height(IExtender* @this, int* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Height);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Height(IExtender* @this, int value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Height = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Left(IExtender* @this, int* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Left);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Left(IExtender* @this, int value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Left = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_TabStop(IExtender* @this, BOOL* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.TabStop);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_TabStop(IExtender* @this, BOOL value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.TabStop = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Top(IExtender* @this, int* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Top);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Top(IExtender* @this, int value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Top = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Visible(IExtender* @this, BOOL* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Visible);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Visible(IExtender* @this, BOOL value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Visible = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Width(IExtender* @this, int* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Width);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT set_Width(IExtender* @this, int value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => o.Width = value);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Name(IExtender* @this, BSTR* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Name);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Parent(IExtender* @this, IUnknown** value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Parent);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Hwnd(IExtender* @this, HWND* value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Hwnd);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT get_Container(IExtender* @this, IUnknown** value)
        => UnwrapAndInvoke<IExtender, Interface>(@this, o => *value = o.Container);

    // This one isn't actually used.
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT Move(IExtender* @this, void* left, void* top, void* width, void* height) => HRESULT.S_OK;

    [ComImport]
    [Guid("39088D7E-B71E-11D1-8F39-00C04FD946D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface Interface
    {
        int Align { get; set; }

        BOOL Enabled { get; set; }

        int Height { get; set; }

        int Left { get; set; }

        BOOL TabStop { get; set; }

        int Top { get; set; }

        BOOL Visible { get; set; }

        int Width { get; set; }

        BSTR Name { get; }

        IUnknown* Parent { get; }

        HWND Hwnd { get; }

        IUnknown* Container { get; }

        // This isn't even utilized
        void Move(
            void* left,
            void* top,
            void* width,
            void* height);
    }
}
