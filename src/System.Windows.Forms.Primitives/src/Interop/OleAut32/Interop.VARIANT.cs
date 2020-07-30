// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms.Primitives.Resources;
using static Interop.Ole32;

internal static partial class Interop
{
    internal static partial class Oleaut32
    {
        /// <remarks>
        ///  This implementation supports both VARIANT and VARIANTARG semantics.
        ///  See https://devblogs.microsoft.com/oldnewthing/20171221-00/?p=97625
        /// </remarks>
        public unsafe struct VARIANT : IDisposable
        {
            public VARENUM vt;
            public short reserved1;
            public short reserved2;
            public short reserved3;
            public VARIANTUnion data;

            public VARENUM Type => (vt & VARENUM.TYPEMASK);

            public bool Byref => (vt & VARENUM.BYREF) != 0;

            public void Clear()
            {
                PropVariantClear(ref this);
                vt = VARENUM.EMPTY;
                data = new VARIANTUnion();
            }

            public void Dispose() => Clear();

            public object? ToObject()
            {
                fixed (VARIANT* thisVariant = &this)
                {
                    void* data = &thisVariant->data;
                    if (Byref)
                    {
                        data = *((void**)data);
                        // CLR allows VT_EMPTY/NULL | VT_BYREF to have no data.
                        // In other cases, the variant is invalid.
                        if (data is null && !(Type == VARENUM.EMPTY || Type == VARENUM.NULL))
                        {
                            throw new ArgumentException("Invalid Variant");
                        }
                    }
                    else if (vt == VARENUM.DECIMAL)
                    {
                        data = thisVariant;
                    }

                    // Note that the following check also covers VT_ILLEGAL.
                    if ((vt & ~(VARENUM.BYREF | VARENUM.ARRAY | VARENUM.VECTOR)) >= (VARENUM)0x80)
                    {
                        throw new InvalidOleVariantTypeException();
                    }

                    if ((vt & VARENUM.VECTOR) != 0)
                    {
                        return ToVector(thisVariant->data.ca, vt);
                    }
                    if ((vt & VARENUM.ARRAY) != 0)
                    {
                        return ToArray(*(SAFEARRAY**)data, vt);
                    }

                    return ToObject(Type, Byref, data);
                }
            }

            private static object? ToObject(VARENUM type, bool byRef, void* data)
            {
                switch (type)
                {
                    case VARENUM.EMPTY:
                        if (byRef)
                        {
                            // CLR returns VT_EMPTY | VT_BYREF data as nuint.
                            if (IntPtr.Size == 8)
                            {
                                return (ulong)data;
                            }

                            return (uint)data;
                        }

                        return null;
                    case VARENUM.NULL:
                        return Convert.DBNull;
                    case VARENUM.I1:
                        return *((sbyte*)data);
                    case VARENUM.UI1:
                        return *((byte*)data);
                    case VARENUM.I2:
                        return *((short*)data);
                    case VARENUM.UI2:
                        return *((ushort*)data);
                    case VARENUM.I4:
                    case VARENUM.INT:
                    case VARENUM.ERROR:
                    case VARENUM.HRESULT:
                        return *((int*)data);
                    case VARENUM.UI4:
                    case VARENUM.UINT:
                        return *((uint*)data);
                    case VARENUM.I8:
                        return *((long*)data);
                    case VARENUM.UI8:
                        return *((ulong*)data);
                    case VARENUM.R4:
                        return *((float*)data);
                    case VARENUM.R8:
                        return *((double*)data);
                    case VARENUM.CY:
                        long cyVal = *((long*)data);
                        return decimal.FromOACurrency(cyVal);
                    case VARENUM.DATE:
                        double date = *((double*)data);
                        return DateTime.FromOADate(date);
                    case VARENUM.BSTR:
                    case VARENUM.LPWSTR:
                        return Marshal.PtrToStringUni(*(IntPtr*)data);
                    case VARENUM.LPSTR:
                        return Marshal.PtrToStringAnsi(*(IntPtr*)data);
                    case VARENUM.DISPATCH:
                    case VARENUM.UNKNOWN:
                        IntPtr pInterface = *(IntPtr*)data;
                        if (pInterface == IntPtr.Zero)
                        {
                            return null;
                        }

                        return Marshal.GetObjectForIUnknown(pInterface);
                    case VARENUM.DECIMAL:
                        return ((DECIMAL*)data)->ToDecimal();
                    case VARENUM.BOOL:
                        return (*(VARIANT_BOOL*)data) != VARIANT_BOOL.FALSE;
                    case VARENUM.VARIANT:
                        // We only support VT_VARIANT | VT_BYREF.
                        if (!byRef)
                        {
                            break;
                        }

                        // BYREF VARIANTS are not allowed to be nested.
                        VARIANT* pVariant = (VARIANT*)data;
                        if (pVariant->Byref)
                        {
                            throw new InvalidOleVariantTypeException();
                        }

                        return pVariant->ToObject();
                    case VARENUM.CLSID:
                        // We only support VT_CLSID.
                        // This is the type of InitPropVariantFromCLSID.
                        if (byRef)
                        {
                            break;
                        }

                        return **((Guid**)data);
                    case VARENUM.FILETIME:
                        // We only support VT_FILETIME.
                        // This is the type of InitPropVariantFromFILETIME.
                        if (byRef)
                        {
                            break;
                        }

                        return (*(Kernel32.FILETIME*)data).ToDateTime();
                    case VARENUM.VOID:
                        return null;
                    case VARENUM.RECORD:
                    {
                        VARIANTRecord* record = (VARIANTRecord*)data;
                        if (record->pRecInfo == IntPtr.Zero)
                        {
                            throw new ArgumentException("Specified OLE variant is invalid.");
                        }
                        if (record->pvRecord is null)
                        {
                            return null;
                        }

                        // TODO: cast IntPtr to IRecordInfo. Not that much of a concern
                        // as .NET Core doesn't support records anyway.
                        // Type recordType = GetRecordElementType(record->pvRecord);
                        throw new ArgumentException("Record marshalling doesn't actually work in .NET Core. Matching that behaviour.");
                    }
                }

                throw new ArgumentException(string.Format(SR.COM2UnhandledVT, type));
            }

            private static Type GetRecordElementType(IRecordInfo record)
            {
                Guid guid;
                HRESULT hr = record.GetGuid(&guid);
                if (!hr.Succeeded())
                {
                    throw Marshal.GetExceptionForHR((int)hr)!;
                }

                Type? t = System.Type.GetTypeFromCLSID(guid);
                if (t is null || !t.IsValueType)
                {
                    throw new ArgumentException("The specified record cannot be mapped to a managed value class.");
                }

                return t;
            }

            private static object? ToArray(SAFEARRAY* psa, VARENUM vt)
            {
                if (psa is null)
                {
                    return null;
                }

                VARENUM arrayType = vt & ~VARENUM.ARRAY;
                Array array = CreateArrayFromSafeArray(psa, arrayType);

                HRESULT hr = SafeArrayLock(psa);
                Debug.Assert(hr == HRESULT.S_OK);

                try
                {
                    if (array.Rank == 1)
                    {
                        switch (arrayType)
                        {
                            case VARENUM.I1:
                                new Span<sbyte>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<sbyte>(array));
                                break;
                            case VARENUM.UI1:
                                new Span<byte>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<byte>(array));
                                break;
                            case VARENUM.I2:
                                new Span<short>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<short>(array));
                                break;
                            case VARENUM.UI2:
                                new Span<ushort>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<ushort>(array));
                                break;
                            case VARENUM.I4:
                            case VARENUM.INT:
                                new Span<int>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<int>(array));
                                break;
                            case VARENUM.UI4:
                            case VARENUM.UINT:
                            case VARENUM.ERROR: // Not explicitly mentioned in the docs but trivial to implement.
                                new Span<uint>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<uint>(array));
                                break;
                            case VARENUM.I8:
                                new Span<long>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<long>(array));
                                break;
                            case VARENUM.UI8:
                                new Span<ulong>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<ulong>(array));
                                break;
                            case VARENUM.R4:
                                new Span<float>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<float>(array));
                                break;
                            case VARENUM.R8:
                                new Span<double>(psa->pvData, array.Length)
                                    .CopyTo(GetSpan<double>(array));
                                break;
                            case VARENUM.BOOL:
                            {
                                var data = new Span<VARIANT_BOOL>(psa->pvData, array.Length);
                                var result = GetSpan<bool>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    result[i] = data[i] != VARIANT_BOOL.FALSE;
                                }
                                break;
                            }
                            case VARENUM.DECIMAL:
                            {
                                var data = new Span<DECIMAL>(psa->pvData, array.Length);
                                var result = GetSpan<decimal>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    result[i] = data[i].ToDecimal();
                                }
                                break;
                            }
                            case VARENUM.CY:
                            {
                                var data = new Span<long>(psa->pvData, array.Length);
                                var result = GetSpan<decimal>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    result[i] = decimal.FromOACurrency(data[i]);
                                }
                                break;
                            }
                            case VARENUM.DATE:
                            {
                                var data = new Span<double>(psa->pvData, array.Length);
                                var result = GetSpan<DateTime>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    result[i] = DateTime.FromOADate(data[i]);
                                }
                                break;
                            }
                            case VARENUM.BSTR:
                            {
                                var data = new Span<IntPtr>(psa->pvData, array.Length);
                                var result = GetSpan<string?>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    result[i] = Marshal.PtrToStringUni(data[i]);
                                }
                                break;
                            }
                            case VARENUM.DISPATCH:
                            case VARENUM.UNKNOWN:
                            {
                                var data = new Span<IntPtr>(psa->pvData, array.Length);
                                var result = GetSpan<object?>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (data[i] == IntPtr.Zero)
                                    {
                                        result[i] = null;
                                    }
                                    else
                                    {
                                        result[i] = Marshal.GetObjectForIUnknown(data[i]);
                                    }
                                }
                                break;
                            }
                            case VARENUM.VARIANT:
                            {
                                var data = new Span<VARIANT>(psa->pvData, array.Length);
                                var result = GetSpan<object?>(array);
                                for (int i = 0; i < data.Length; i++)
                                {
                                    result[i] = data[i].ToObject();
                                }
                                break;
                            }
                            case VARENUM.RECORD:
                                throw new NotImplementedException();
                            default:
                                throw new ArgumentException(string.Format(SR.COM2UnhandledVT, vt));
                        }
                    }
                    else if (array.Length != 0)
                    {
                        // CLR arrays are laid out in row-major order.
                        // See CLI 8.9.1: https://www.ecma-international.org/publications/files/ECMA-ST/ECMA-335.pdf
                        // However, SAFEARRAYs are laid out in column-major order.
                        // See https://docs.microsoft.com/en-us/previous-versions/windows/desktop/automat/array-manipulation-functions
                        // Therefore, we need to transpose data.
                        TransposeArray(psa, array, arrayType);
                    }
                }
                finally
                {
                    hr = SafeArrayUnlock(psa);
                    Debug.Assert(hr == HRESULT.S_OK);
                }

                return array;
            }

            private static void TransposeArray(SAFEARRAY* psa, Array array, VARENUM arrayType)
            {
                if (array.Rank <= 32)
                {
                    StackTransposeArray(psa, array, arrayType);
                }
                else
                {
                    Debug.Fail("The CLR should not support arrays with more than 32 dimensions.");
                    HeapTransposeArray(psa, array, arrayType);
                }

                static void StackTransposeArray(SAFEARRAY* psa, Array array, VARENUM arrayType)
                {
                    Span<int> indices = stackalloc int[array.Rank];
                    Span<int> lower = stackalloc int[array.Rank];
                    Span<int> upper = stackalloc int[array.Rank];
                    InternalTransposeArray(psa, array, arrayType, indices, lower, upper);
                }

                static void HeapTransposeArray(SAFEARRAY* psa, Array array, VARENUM arrayType)
                {
                    var indices = new int[array.Rank];
                    var lower = new int[array.Rank];
                    var upper = new int[array.Rank];
                    InternalTransposeArray(psa, array, arrayType, indices, lower, upper);
                }

                static void InternalTransposeArray(SAFEARRAY* psa, Array array, VARENUM arrayType, Span<int> indices, Span<int> lower, Span<int> upper)
                {
                    int lastIndex = array.Rank - 1;
                    int i;
                    for (i = 0; i < array.Rank; i++)
                    {
                        indices[i] = lower[i] = array.GetLowerBound(i);
                        upper[i] = array.GetUpperBound(i);
                    }

                    // Loop through all the indices.
                    while (true)
                    {
                        BeginMainLoop:

                        SetArrayValue(psa, array, indices, lower, arrayType);

                        for (i = lastIndex; i > 0;)
                        {
                            if (++indices[i] <= upper[i])
                            {
                                goto BeginMainLoop;
                            }

                            indices[i] = lower[i];
                            --i;
                        }

                        // Special case for the first index, it must be enumerated only once
                        if (++indices[0] > upper[0])
                        {
                            break;
                        }
                    }
                }
            }

            private static void SetArrayValue(SAFEARRAY* psa, Array array, Span<int> indices, Span<int> lowerBounds, VARENUM arrayType)
            {
                static void SetValue<T>(Array array, T value, Span<int> indices, Span<int> lowerBounds)
                {
                    // CLR arrays are laid out in row-major order.
                    // See CLI 8.9.1: https://www.ecma-international.org/publications/files/ECMA-ST/ECMA-335.pdf
                    var span = GetSpan<T>(array);
                    int offset = 0;
                    int multiplier = 1;
                    for (int i = array.Rank; i >= 1; i--)
                    {
                        int diff = indices[i - 1] - lowerBounds[i - 1];
                        offset += diff * multiplier;
                        multiplier *= array.GetLength(i - 1);
                    }

                    span[offset] = value;
                }

                switch (arrayType)
                {
                    case VARENUM.I1:
                        SetValue(array, psa->GetValue<sbyte>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.UI1:
                        SetValue(array, psa->GetValue<byte>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.I2:
                        SetValue(array, psa->GetValue<short>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.UI2:
                        SetValue(array, psa->GetValue<ushort>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.I4:
                    case VARENUM.INT:
                        SetValue(array, psa->GetValue<int>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.UI4:
                    case VARENUM.UINT:
                    case VARENUM.ERROR: // Not explicitly mentioned in the docs but trivial to implement.
                        SetValue(array, psa->GetValue<uint>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.I8:
                        SetValue(array, psa->GetValue<long>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.UI8:
                        SetValue(array, psa->GetValue<ulong>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.R4:
                        SetValue(array, psa->GetValue<float>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.R8:
                        SetValue(array, psa->GetValue<double>(indices), indices, lowerBounds);
                        break;
                    case VARENUM.BOOL:
                    {
                        VARIANT_BOOL data = psa->GetValue<VARIANT_BOOL>(indices);
                        SetValue(array, data != VARIANT_BOOL.FALSE, indices, lowerBounds);
                        break;
                    }
                    case VARENUM.DECIMAL:
                    {
                        DECIMAL data = psa->GetValue<DECIMAL>(indices);
                        SetValue(array, data.ToDecimal(), indices, lowerBounds);
                        break;
                    }
                    case VARENUM.CY:
                    {
                        long data = psa->GetValue<long>(indices);
                        SetValue(array, decimal.FromOACurrency(data), indices, lowerBounds);
                        break;
                    }
                    case VARENUM.DATE:
                    {
                        double data = psa->GetValue<double>(indices);
                        SetValue(array, DateTime.FromOADate(data), indices, lowerBounds);
                        break;
                    }
                    case VARENUM.BSTR:
                    {
                        IntPtr data = psa->GetValue<IntPtr>(indices);
                        SetValue(array, Marshal.PtrToStringUni(data), indices, lowerBounds);
                        break;
                    }
                    case VARENUM.DISPATCH:
                    case VARENUM.UNKNOWN:
                    {
                        IntPtr data = psa->GetValue<IntPtr>(indices);
                        if (data == IntPtr.Zero)
                        {
                            SetValue<object?>(array, null, indices, lowerBounds);
                        }
                        else
                        {
                            SetValue(array, Marshal.GetObjectForIUnknown(data), indices, lowerBounds);
                        }
                        break;
                    }
                    case VARENUM.VARIANT:
                    {
                        VARIANT data = psa->GetValue<VARIANT>(indices);
                        SetValue(array, data.ToObject(), indices, lowerBounds);
                        break;
                    }
                    case VARENUM.RECORD:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, arrayType));
                }
            }

            private static Array CreateArrayFromSafeArray(SAFEARRAY* psa, VARENUM vt)
            {
                Type elementType;
                if (vt == VARENUM.EMPTY)
                {
                    throw new InvalidOleVariantTypeException();
                }
                if (vt == VARENUM.RECORD)
                {
                    HRESULT hr = SafeArrayGetRecordInfo(psa, out IRecordInfo record);
                    if (!hr.Succeeded())
                    {
                        throw Marshal.GetExceptionForHR((int)hr)!;
                    }

                    elementType = GetRecordElementType(record);
                }

                VARENUM arrayVarType = psa->VarType;
                if (arrayVarType == VARENUM.EMPTY)
                {
                    if (psa->cbElements != GetElementSizeForVarType(vt))
                    {
                        throw new SafeArrayTypeMismatchException();
                    }
                }
                // Allow limited conversion between arrays of different but related types.
                else if (arrayVarType != vt
                    && !(vt == VARENUM.INT && arrayVarType == VARENUM.I4)
                    && !(vt == VARENUM.UINT && arrayVarType == VARENUM.UI4)
                    && !(vt == VARENUM.I4 && arrayVarType == VARENUM.INT)
                    && !(vt == VARENUM.UI4 && arrayVarType == VARENUM.UINT)
                    && !(vt == VARENUM.UNKNOWN && arrayVarType == VARENUM.DISPATCH)
                    && !(arrayVarType == VARENUM.RECORD))
                {
                    // To match CLR behaviour.
                    throw new SafeArrayTypeMismatchException();
                }

                switch (vt)
                {
                    case VARENUM.I1:
                        elementType = typeof(sbyte);
                        break;
                    case VARENUM.UI1:
                        elementType = typeof(byte);
                        break;
                    case VARENUM.I2:
                        elementType = typeof(short);
                        break;
                    case VARENUM.UI2:
                        elementType = typeof(ushort);
                        break;
                    case VARENUM.I4:
                    case VARENUM.INT:
                        elementType = typeof(int);
                        break;
                    case VARENUM.I8:
                        elementType = typeof(long);
                        break;
                    case VARENUM.UI8:
                        elementType = typeof(ulong);
                        break;
                    case VARENUM.UI4:
                    case VARENUM.UINT:
                    case VARENUM.ERROR:
                        elementType = typeof(uint);
                        break;
                    case VARENUM.R4:
                        elementType = typeof(float);
                        break;
                    case VARENUM.R8:
                        elementType = typeof(double);
                        break;
                    case VARENUM.BOOL:
                        elementType = typeof(bool);
                        break;
                    case VARENUM.DECIMAL:
                    case VARENUM.CY:
                        elementType = typeof(decimal);
                        break;
                    case VARENUM.DATE:
                        elementType = typeof(DateTime);
                        break;
                    case VARENUM.BSTR:
                        elementType = typeof(string);
                        break;
                    case VARENUM.DISPATCH:
                    case VARENUM.UNKNOWN:
                    case VARENUM.VARIANT:
                        elementType = typeof(object);
                        break;
                    default:
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, vt));
                }

                if (psa->cDims == 1 && psa->rgsabound[0].lLbound == 0)
                {
                    // SZArray.
                    return Array.CreateInstance(elementType, (int)psa->rgsabound[0].cElements);
                }

                var lengths = new int[psa->cDims];
                var bounds = new int[psa->cDims];
                int counter = 0;
                // Copy the lower bounds and count of elements for the dimensions. These
                // need to copied in reverse order.
                for (int i = psa->cDims - 1; i >= 0; i--)
                {
                    lengths[counter] = (int)psa->rgsabound[i].cElements;
                    bounds[counter] = (int)psa->rgsabound[i].lLbound;
                    counter++;
                }

                return Array.CreateInstance(elementType, lengths, bounds);
            }

            private static uint GetElementSizeForVarType(VARENUM vt)
            {
                switch (vt)
                {
                    case VARENUM.EMPTY:
                    case VARENUM.NULL:
                    case VARENUM.VOID:
                        return 0;
                    case VARENUM.I1:
                    case VARENUM.UI1:
                        return 1;
                    case VARENUM.I2:
                    case VARENUM.UI2:
                    case VARENUM.BOOL:
                        return 2;
                    case VARENUM.I4:
                    case VARENUM.UI4:
                    case VARENUM.INT:
                    case VARENUM.UINT:
                    case VARENUM.R4:
                    case VARENUM.HRESULT:
                    case VARENUM.ERROR:
                        return 4;
                    case VARENUM.I8:
                    case VARENUM.UI8:
                    case VARENUM.CY:
                    case VARENUM.R8:
                    case VARENUM.DATE:
                        return 8;
                    case VARENUM.DECIMAL:
                        return (uint)sizeof(DECIMAL);
                    case VARENUM.VARIANT:
                        return (uint)sizeof(VARIANT);
                    case VARENUM.BSTR:
                    case VARENUM.LPSTR:
                    case VARENUM.LPWSTR:
                    case VARENUM.UNKNOWN:
                    case VARENUM.DISPATCH:
                    case VARENUM.USERDEFINED:
                    case VARENUM.CARRAY:
                    case VARENUM.SAFEARRAY:
                    case VARENUM.PTR:
                        return (uint)IntPtr.Size;
                    default:
                        if ((vt & VARENUM.ARRAY) != 0)
                        {
                            return (uint)sizeof(SAFEARRAY*);
                        }

                        return 0;
                }
            }

            private static object ToVector(in CA ca, VARENUM vectorType)
            {
                VARENUM vt = vectorType & ~VARENUM.VECTOR;
                switch (vt)
                {
                    case VARENUM.I1:
                        return new Span<sbyte>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.UI1:
                        return new Span<byte>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.I2:
                        return new Span<short>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.UI2:
                        return new Span<ushort>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.BOOL:
                    {
                        var data = new Span<VARIANT_BOOL>(ca.pElems, (int)ca.cElems);
                        var result = new bool[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = data[i] != VARIANT_BOOL.FALSE;
                        }
                        return result;
                    }
                    case VARENUM.I4:
                    case VARENUM.INT: // Not explicitly mentioned in the docs but trivial to implement.
                        return new Span<int>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.UI4:
                    case VARENUM.ERROR:
                    case VARENUM.UINT: // Not explicitly mentioned in the docs but trivial to implement.
                        return new Span<uint>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.I8:
                        return new Span<long>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.UI8:
                        return new Span<ulong>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.R4:
                        return new Span<float>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.R8:
                        return new Span<double>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.CY:
                    {
                        var data = new Span<long>(ca.pElems, (int)ca.cElems);
                        var result = new decimal[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = decimal.FromOACurrency(data[i]);
                        }
                        return result;
                    }
                    case VARENUM.DATE:
                    {
                        var data = new Span<double>(ca.pElems, (int)ca.cElems);
                        var result = new DateTime[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = DateTime.FromOADate(data[i]);
                        }
                        return result;
                    }
                    case VARENUM.FILETIME:
                    {
                        var data = new Span<Kernel32.FILETIME>(ca.pElems, (int)ca.cElems);
                        var result = new DateTime[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = data[i].ToDateTime();
                        }
                        return result;
                    }
                    case VARENUM.CLSID:
                        return new Span<Guid>(ca.pElems, (int)ca.cElems).ToArray();
                    case VARENUM.BSTR:
                    case VARENUM.LPWSTR:
                    {
                        var data = new Span<IntPtr>(ca.pElems, (int)ca.cElems);
                        var result = new string?[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = Marshal.PtrToStringUni(data[i]);
                        }
                        return result;
                    }
                    case VARENUM.LPSTR:
                    {
                        var data = new Span<IntPtr>(ca.pElems, (int)ca.cElems);
                        var result = new string?[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = Marshal.PtrToStringAnsi(data[i]);
                        }
                        return result;
                    }
                    case VARENUM.VARIANT:
                    {
                        var data = new Span<VARIANT>(ca.pElems, (int)ca.cElems);
                        var result = new object?[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            result[i] = data[i].ToObject();
                        }
                        return result;
                    }
                    case VARENUM.CF: // Not implemented.
                    case VARENUM.BSTR_BLOB: // System use only.
                    default: // Documentation does not specify any other types that are supported.
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, vt));
                }
            }

            private static Span<T> GetSpan<T>(Array arr)
                => MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0).ToPointer()), arr.Length);

            [StructLayout(LayoutKind.Explicit)]
            public struct VARIANTUnion
            {
                [FieldOffset(0)]
                public long llVal;

                [FieldOffset(0)]
                public int lVal;

                [FieldOffset(0)]
                public byte bVal;

                [FieldOffset(0)]
                public short iVal;

                [FieldOffset(0)]
                public float fltVal;

                [FieldOffset(0)]
                public double dblVal;

                [FieldOffset(0)]
                public short boolVal;

                [FieldOffset(0)]
                public int scode;

                [FieldOffset(0)]
                public long cyVal;

                [FieldOffset(0)]
                public double date;

                [FieldOffset(0)]
                public IntPtr bstrVal;

                [FieldOffset(0)]
                public IntPtr punkVal;

                [FieldOffset(0)]
                public IntPtr pdispVal;

                [FieldOffset(0)]
                public SAFEARRAY* parray;

                [FieldOffset(0)]
                public byte* pbVal;

                [FieldOffset(0)]
                public short* piVal;

                [FieldOffset(0)]
                public int* plVal;

                [FieldOffset(0)]
                public long* pllVal;

                [FieldOffset(0)]
                public float* pfltVal;

                [FieldOffset(0)]
                public double* pdblVal;

                [FieldOffset(0)]
                public short* pboolVal;

                [FieldOffset(0)]
                public int* pscode;

                [FieldOffset(0)]
                public long* pcyVal;

                [FieldOffset(0)]
                public double* pdate;

                [FieldOffset(0)]
                public IntPtr* pbstrVal;

                [FieldOffset(0)]
                public IntPtr* ppunkVal;

                [FieldOffset(0)]
                public IntPtr* ppdispVal;

                [FieldOffset(0)]
                public SAFEARRAY** pparray;

                [FieldOffset(0)]
                public VARIANT* pvarVal;

                [FieldOffset(0)]
                public void* Byref;

                [FieldOffset(0)]
                public sbyte cVal;

                [FieldOffset(0)]
                public ushort uiVal;

                [FieldOffset(0)]
                public uint ulVal;

                [FieldOffset(0)]
                public ulong ullVal;

                [FieldOffset(0)]
                public DECIMAL* pdecVal;

                [FieldOffset(0)]
                public sbyte* pcVal;

                [FieldOffset(0)]
                public ushort* puiVal;

                [FieldOffset(0)]
                public uint* pulVal;

                [FieldOffset(0)]
                public ulong* pullVal;

                [FieldOffset(0)]
                public int* pintVal;

                [FieldOffset(0)]
                public uint* puintVal;

                [FieldOffset(0)]
                public VARIANTRecord recordVal;

                [FieldOffset(0)]
                public Guid* puuid;

                [FieldOffset(0)]
                public Kernel32.FILETIME filetime;

                [FieldOffset(0)]
                public CA ca;
            }

            public struct VARIANTRecord
            {
                public void* pvRecord;
                public IntPtr pRecInfo;
            }
        }
    }
}
