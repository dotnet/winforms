// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.System.Com;

/// <summary>
///  Struct that handles managed object COM projection lifetime management.
/// </summary>
/// <typeparam name="TVTable">
///  Struct that repesents the VTable for a COM pointer.
/// </typeparam>
/// <typeparam name="TObject">
///  The type of object being projected.
/// </typeparam>
internal unsafe struct Lifetime<TVTable, TObject> where TVTable : unmanaged
{
    private TVTable* _vtable;
    private void* _handle;
    private uint _refCount;

    public static unsafe uint AddRef(void* @this) =>
        Interlocked.Increment(ref ((Lifetime<TVTable, TObject>*)@this)->_refCount);

    public static unsafe uint Release(void* @this)
    {
        var lifetime = (Lifetime<TVTable, TObject>*)@this;
        Debug.Assert(lifetime->_refCount > 0);
        uint count = Interlocked.Decrement(ref lifetime->_refCount);
        if (count == 0)
        {
            GCHandle.FromIntPtr((nint)lifetime->_handle).Free();
            Marshal.FreeCoTaskMem((nint)lifetime);
        }

        return count;
    }

    /// <summary>
    ///  Allocate a lifetime wrapper for the given <paramref name="object"/> with the given
    ///  <paramref name="vtable"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This creates a <see cref="GCHandle"/> to root the <paramref name="object"/> until ref
    ///   counting has gone to zero.
    ///  </para>
    ///  <para>
    ///   The <paramref name="vtable"/> should be fixed, typically as a static. Com calls always
    ///   include the "this" pointer as the first argument.
    ///  </para>
    /// </remarks>
    public static unsafe Lifetime<TVTable, TObject>* Allocate(TObject @object, TVTable* vtable)
    {
        var wrapper = (Lifetime<TVTable, TObject>*)Marshal.AllocCoTaskMem(sizeof(Lifetime<TVTable, TObject>));

        // Create the wrapper instance.
        wrapper->_vtable = vtable;
        wrapper->_handle = (void*)GCHandle.ToIntPtr(GCHandle.Alloc(@object));
        wrapper->_refCount = 1;

        return wrapper;
    }

    /// <summary>
    ///  Get the object associated with this lifetime.
    /// </summary>
    /// <param name="this">
    ///  The passed back "this" pointer that originally came from <see cref="Allocate(TObject, TVTable*)"/>.
    /// </param>
    /// <returns>The object associated with this lifetime, if any.</returns>
    /// <exception cref="InvalidOperationException">The handle was freed.</exception>
    public static TObject? GetObject(void* @this)
    {
        var lifetime = (Lifetime<TVTable, TObject>*)@this;
        return (TObject?)GCHandle.FromIntPtr((IntPtr)lifetime->_handle).Target;
    }
}
