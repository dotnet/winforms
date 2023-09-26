// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com;

/// <summary>
///  Helper to scope lifetime of a <see cref="SAFEARRAY"/> created via <see cref="PInvoke.SafeArrayCreate(VARENUM, uint, in SAFEARRAYBOUND)"/>
///  Destroys the <see cref="SAFEARRAY"/> (if any) when disposed. Note that this scope currently only works for a one dimensional <see cref="SAFEARRAY"/>.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement to ensure the <see cref="SAFEARRAY"/> gets disposed.
///   Use <see cref="Value"/> to pass the <see cref="SAFEARRAY"/> to APIs that populate it.
///  </para>
/// </remarks>
internal unsafe ref struct SafeArrayScope<T>
{
    public SAFEARRAY* Value { get; private set; }

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

        Value = PInvoke.SafeArrayCreate(vt, 1, &saBound);
    }

    public readonly T? this[int i]
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

            // Noop. We should never get here as we would've thrown in the constructor
            // for an unknown type.
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
        }
    }

    public readonly bool IsNull => Value is null;

    public void Dispose()
    {
        if (!IsNull)
        {
            PInvoke.SafeArrayDestroy(Value).ThrowOnFailure();
            Value = null;
        }
    }
}
