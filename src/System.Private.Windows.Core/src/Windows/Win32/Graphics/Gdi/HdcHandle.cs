// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Used when you must keep a handle to an <see cref="HDC"/> in a field. Avoid keeping HDC handles in fields
///  when possible.
/// </summary>
internal sealed class HdcHandle : IDisposable, IHandle<HDC>
{
    /// <summary>
    ///  Take ownership from a <see cref="CreateDcScope"/>.
    /// </summary>
    public HdcHandle(CreateDcScope hdc)
    {
#if DEBUG
        // Don't want to track the CreateDcScope.
        GC.SuppressFinalize(hdc);
#endif
        Handle = hdc;
    }

    public HDC Handle { get; private set; }

    public static implicit operator HDC(in HdcHandle handle) => handle.Handle;
    public static implicit operator nint(in HdcHandle handle) => handle.Handle;

    public void Dispose()
    {
        if (!Handle.IsNull)
        {
            PInvokeCore.DeleteDC(Handle);
            Handle = default;
        }

        GC.SuppressFinalize(this);
    }

    ~HdcHandle() => Dispose();
}
