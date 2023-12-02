// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com;

/// <summary>
///  Helper to scope lifetime of a <see cref="SAFEARRAY"/> created via <see cref="PInvoke.SafeArrayCreate(VARENUM, uint, SAFEARRAYBOUND*)"/>
///  that holds COM pointers.
///  Destroys the <see cref="SAFEARRAY"/> (if any) when disposed. Note that this scope currently only works for a one dimensional <see cref="SAFEARRAY"/>
///  of type <see cref="VARENUM.VT_UNKNOWN"/>
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement to ensure the <see cref="SAFEARRAY"/> gets disposed.
///  </para>
/// </remarks>
internal readonly unsafe ref struct ComSafeArrayScope<T> where T : unmanaged, IComIID
{
    private readonly SafeArrayScope<nint> _value;

    public ComSafeArrayScope(SAFEARRAY* value)
    {
        if (value is not null && value->VarType != VARENUM.VT_UNKNOWN)
        {
            throw new ArgumentException("Must pass in a SafeArray of type VARENUM.VT_UNKNOWN");
        }

        _value = new(value);
    }

    public ComSafeArrayScope(uint size)
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = size,
            lLbound = 0
        };

        _value = new(PInvoke.SafeArrayCreate(VARENUM.VT_UNKNOWN, 1, &saBound));
    }

    /// <summary>
    ///  Creates a <see cref="ComSafeArrayScope{T}"/> from an array of COM interfaces.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <typeparamref name="T"/> must implement <see cref="IComInterface{T}"/> where T is <typeparamref name="TComInterface"/>.
    ///   Otherwise an <see cref="ArgumentException"/> will be thrown.
    ///  </para>
    /// </remarks>
    public static ComSafeArrayScope<T> CreateFromInterfaceArray<TComInterface>(TComInterface[] array)
    {
        if (!typeof(T).IsAssignableTo(typeof(IComInterface<TComInterface>)))
        {
            throw new ArgumentException($"{typeof(T).Name} must implement {nameof(IComInterface<TComInterface>)}<{typeof(TComInterface).Name}>");
        }

        uint length = (uint)array.Length;
        ComSafeArrayScope<T> scope = new(length);
        for (int i = 0; i < length; i++)
        {
            scope[i] = ComHelpers.GetComPointer<T>(array[i]);
        }

        return scope;
    }

    /// <remarks>
    ///  <para>
    ///   The <see cref="SAFEARRAY"/> will ref count the COM pointer on get/put.
    ///   Be sure to release of COM pointers that are given to/from the <see cref="SAFEARRAY"/>.
    ///  </para>
    /// </remarks>
    public T* this[int i]
    {
        get
        {
            using ComScope<IUnknown> unknown = new((IUnknown*)_value[i]);
            void* result;
            unknown.Value->QueryInterface(IID.Get<T>(), &result).ThrowOnFailure();
            return (T*)result;
        }
        set => _value[i] = (nint)value;
    }

    public int Length => _value.Length;

    public bool IsNull => _value.IsNull;

    public bool IsEmpty => _value.IsEmpty;

    public void Dispose() => _value.Dispose();

    public static implicit operator SAFEARRAY**(in ComSafeArrayScope<T> scope) => scope._value;

    public static implicit operator SAFEARRAY*(in ComSafeArrayScope<T> scope) => scope._value;
}
