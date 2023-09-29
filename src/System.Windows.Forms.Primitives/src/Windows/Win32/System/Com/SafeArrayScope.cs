// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com;

/// <summary>
///  Helper to scope lifetime of a <see cref="SAFEARRAY"/> created via <see cref="PInvoke.SafeArrayCreate(VARENUM, uint, SAFEARRAYBOUND*)"/>
///  Destroys the <see cref="SAFEARRAY"/> (if any) when disposed. Note that this scope currently only works for a one dimensional <see cref="SAFEARRAY"/>.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement to ensure the <see cref="SAFEARRAY"/> gets disposed.
///  </para>
///  <para>
///   If the <see cref="SAFEARRAY"/> you are intending to scope the lifetime of has type <see cref="VARENUM.VT_UNKNOWN"/>,
///   use <see cref="ComSafeArrayScope{T}"/> for better usability.
///  </para>
/// </remarks>
internal readonly unsafe ref struct SafeArrayScope<T>
{
    private readonly nint _value;

    public SAFEARRAY* Value => (SAFEARRAY*)_value;

    public SafeArrayScope(SAFEARRAY* value)
    {
        _value = (nint)value;
    }

    public SafeArrayScope(uint size)
    {
        VARENUM vt = VARENUM.VT_EMPTY;
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
        else if (typeof(T) == typeof(nint))
        {
            throw new ArgumentException("Use ComSafeArrayScope instead");
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

        _value = (nint)PInvoke.SafeArrayCreate(vt, 1, &saBound);
    }

    /// <remarks>
    ///  <para>
    ///   A copy will be made of anything that is put into the <see cref="SAFEARRAY"/>
    ///   and anything the <see cref="SAFEARRAY"/> gives out. Be sure to dispose of the
    ///   items that are given to/from the <see cref="SAFEARRAY"/> if necessary.
    ///  </para>
    /// </remarks>
    public T? this[int i]
    {
        get
        {
            Span<int> indices = stackalloc int[] { i };

            if (typeof(T) == typeof(string))
            {
                using BSTR result = default;
                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayGetElement(Value, pIndices, &result).ThrowOnFailure();
                }

                string resultString = result.ToString();
                return (T)(object)resultString;
            }
            else if (typeof(T) == typeof(int))
            {
                int result = default;
                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayGetElement(Value, pIndices, &result).ThrowOnFailure();
                }

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(double))
            {
                double result = default;

                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayGetElement(Value, pIndices, &result).ThrowOnFailure();
                }

                return (T)(object)result;
            }
            else if (typeof(T) == typeof(nint))
            {
                nint result;

                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayGetElement(Value, pIndices, &result).ThrowOnFailure();
                }

                return (T)(object)result;
            }

            // Noop. This is an unknown type. We should fill this method out to to do the right
            // thing as we run into new types.
            return default;
        }
        set
        {
            Span<int> indices = stackalloc int[] { i };
            if (value is string s)
            {
                using BSTR bstrText = new(s);
                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayPutElement(Value, pIndices, bstrText).ThrowOnFailure();
                }
            }
            else if (value is int @int)
            {
                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayPutElement(Value, pIndices, &@int).ThrowOnFailure();
                }
            }
            else if (value is double dbl)
            {
                fixed (int* pIndices = indices)
                {
                    PInvoke.SafeArrayPutElement(Value, pIndices, &dbl).ThrowOnFailure();
                }
            }
        }
    }

    public int Length => (int)Value->GetBounds().cElements;

    public bool IsNull => _value == 0;

    public bool IsEmpty => Length == 0;

    public void Dispose()
    {
        if (IsNull)
        {
            return;
        }

        SAFEARRAY* safeArray = (SAFEARRAY*)_value;

        // Really want _value to be null after disposal to avoid double destroy, but we also want
        // to maintain the readonly state of the struct to allow passing as `in` without creating implicit
        // copies (which would break the T** and void** operators).
        *(void**)_value = null;

        if (safeArray is not null)
        {
            PInvoke.SafeArrayDestroy(safeArray).ThrowOnFailure();
        }
    }

    public static implicit operator SAFEARRAY*(in SafeArrayScope<T> scope) => (SAFEARRAY*)scope._value;

    public static implicit operator nint(in SafeArrayScope<T> scope) => scope._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SAFEARRAY**(in SafeArrayScope<T> scope) => (SAFEARRAY**)Unsafe.AsPointer(ref Unsafe.AsRef(in scope._value));
}
