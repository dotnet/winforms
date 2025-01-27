// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com;

/// <summary>
///  Helper to scope lifetime of a <see cref="SAFEARRAY"/> created via
///  <see cref="PInvokeCore.SafeArrayCreate(VARENUM, uint, SAFEARRAYBOUND*)"/>
///  Destroys the <see cref="SAFEARRAY"/> (if any) when disposed. Note that this scope currently only works for a
///  one dimensional <see cref="SAFEARRAY"/>.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement to ensure the <see cref="SAFEARRAY"/> gets disposed.
///  </para>
///  <para>
///   If the <see cref="SAFEARRAY"/> you are intending to scope the lifetime of has type <see cref="VARENUM.VT_UNKNOWN"/>,
///   use <see cref="T:Windows.Win32.System.Com.ComSafeArrayScope`1"/> for better usability.
///  </para>
/// </remarks>
internal readonly unsafe ref struct SafeArrayScope<T>
{
    private readonly nint _value;

    public SAFEARRAY* Value => (SAFEARRAY*)_value;

    public SafeArrayScope(SAFEARRAY* value)
    {
        if (value is null)
        {
            // This SafeArrayScope is meant to receive a SAFEARRAY* from COM.
            _value = (nint)value;
            return;
        }

        if (typeof(T) == typeof(string))
        {
            if (value->VarType is not VARENUM.VT_BSTR)
            {
                throw new ArgumentException($"Wanted SafeArrayScope<{typeof(T)}> but got SAFEARRAY with VarType={value->VarType}");
            }
        }
        else if (typeof(T) == typeof(int))
        {
            if (value->VarType is not VARENUM.VT_I4 or VARENUM.VT_INT)
            {
                throw new ArgumentException($"Wanted SafeArrayScope<{typeof(T)}> but got SAFEARRAY with VarType={value->VarType}");
            }
        }
        else if (typeof(T) == typeof(double))
        {
            if (value->VarType is not VARENUM.VT_R8)
            {
                throw new ArgumentException($"Wanted SafeArrayScope<{typeof(T)}> but got SAFEARRAY with VarType={value->VarType}");
            }
        }
        else if (typeof(T) == typeof(nint))
        {
            if (value->VarType is not VARENUM.VT_UNKNOWN)
            {
                throw new ArgumentException($"Wanted SafeArrayScope<{typeof(T)}> but got SAFEARRAY with VarType={value->VarType}");
            }
        }
        else if (typeof(T).IsAssignableTo(typeof(IComIID)))
        {
            throw new ArgumentException("Use ComSafeArrayScope instead");
        }
        else if (typeof(T) == typeof(object))
        {
            if (value->VarType is not VARENUM.VT_VARIANT)
            {
                throw new ArgumentException($"Wanted SafeArrayScope<{typeof(T)}> but got SAFEARRAY with VarType={value->VarType}");
            }
        }
        else
        {
            // The type has not been accounted for yet in the SafeArrayScope
            // If the type was intentional, this SafeArrayScope needs to be updated
            // to behave appropriately with this type.
            throw new ArgumentException("Unknown type");
        }

        _value = (nint)value;
    }

    public SafeArrayScope(uint size)
    {
        VARENUM vt;

        if (typeof(T) == typeof(string))
        {
            vt = VARENUM.VT_BSTR;
        }
        else if (typeof(T) == typeof(int))
        {
            vt = VARENUM.VT_I4;
        }
        else if (typeof(T) == typeof(double))
        {
            vt = VARENUM.VT_R8;
        }
        else if (typeof(T) == typeof(nint) || typeof(T).IsAssignableTo(typeof(IComIID)))
        {
            throw new ArgumentException("Use ComSafeArrayScope instead");
        }
        else if (typeof(T) == typeof(object))
        {
            vt = VARENUM.VT_VARIANT;
        }
        else
        {
            // The type has not been accounted for yet in the SafeArrayScope
            // If the type was intentional, this SafeArrayScope needs to be updated
            // to behave appropriately with this type.
            throw new ArgumentException("Unknown type");
        }

        SAFEARRAYBOUND saBound = new()
        {
            cElements = size,
            lLbound = 0
        };

        _value = (nint)PInvokeCore.SafeArrayCreate(vt, 1, &saBound);
        if (_value == 0)
        {
            throw new InvalidOperationException("Unable to create SAFEARRAY");
        }
    }

    public SafeArrayScope(T[] array) : this((uint)array.Length)
    {
        for (int i = 0; i < array.Length; i++)
        {
            this[i] = array[i];
        }
    }

    /// <remarks>
    ///  <para>
    ///   A copy will be made of anything that is put into the <see cref="SAFEARRAY"/>
    ///   and anything the <see cref="SAFEARRAY"/> gives out is a copy and has been add ref appropriately if applicable.
    ///   Be sure to dispose of items that are given to the <see cref="SAFEARRAY"/> if necessary. All
    ///   items given out by the <see cref="SAFEARRAY"/> should be disposed.
    ///  </para>
    /// </remarks>
    public T? this[int i]
    {
        get
        {
            if (typeof(T) == typeof(string))
            {
                using BSTR result = GetElement<BSTR>(i);
                return (T)(object)result.ToString();
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)GetElement<int>(i);
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)(object)GetElement<double>(i);
            }
            else if (typeof(T) == typeof(nint))
            {
                return (T)(object)GetElement<nint>(i);
            }
            else if (typeof(T) == typeof(object))
            {
                using VARIANT result = GetElement<VARIANT>(i);
                return (T?)result.ToObject();
            }

            // Noop. This is an unknown type. We should fill this method out to to do the right
            // thing as we run into new types.
            return default;
        }
        set
        {
            if (Value->VarType == VARENUM.VT_VARIANT)
            {
                using VARIANT variant = VARIANT.FromObject(value);
                PutElement(i, &variant);
            }
            else if (value is string s)
            {
                using BSTR bstrText = new(s);
                PutElement(i, bstrText);
            }
            else if (value is int @int)
            {
                PutElement(i, &@int);
            }
            else if (value is double dbl)
            {
                PutElement(i, &dbl);
            }
            else if (value is nint @nint)
            {
                PutElement(i, (void*)@nint);
            }
        }
    }

    private TReturn GetElement<TReturn>(int index) where TReturn : unmanaged
    {
        Span<int> indices = [index];
        TReturn result;
        fixed (int* pIndices = indices)
        {
            PInvokeCore.SafeArrayGetElement(Value, pIndices, &result).ThrowOnFailure();
        }

        return result;
    }

    private void PutElement(int index, void* value)
    {
        Span<int> indices = [index];
        fixed (int* pIndices = indices)
        {
            PInvokeCore.SafeArrayPutElement((SAFEARRAY*)_value, pIndices, value).ThrowOnFailure();
        }
    }

    public int Length => (int)Value->GetBounds().cElements;

    public bool IsNull => _value == 0;

    public bool IsEmpty => Length == 0;

    public void Dispose()
    {
        SAFEARRAY* safeArray = (SAFEARRAY*)_value;

        // Really want this to be null after disposal to avoid double destroy, but we also want
        // to maintain the readonly state of the struct to allow passing as `in` without creating implicit
        // copies (which would break the T** and void** operators).
        *(void**)this = null;

        if (safeArray is not null)
        {
            PInvokeCore.SafeArrayDestroy(safeArray).ThrowOnFailure();
        }
    }

    public static explicit operator VARIANT(in SafeArrayScope<T> scope) => new() { vt = VARENUM.VT_ARRAY | scope.Value->VarType, data = new() { parray = (SAFEARRAY*)scope._value } };

    public static implicit operator SAFEARRAY*(in SafeArrayScope<T> scope) => (SAFEARRAY*)scope._value;

    public static implicit operator nint(in SafeArrayScope<T> scope) => scope._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SAFEARRAY**(in SafeArrayScope<T> scope) => (SAFEARRAY**)Unsafe.AsPointer(ref Unsafe.AsRef(in scope._value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator void**(in SafeArrayScope<T> scope) => (void**)Unsafe.AsPointer(ref Unsafe.AsRef(in scope._value));
}
