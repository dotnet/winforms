// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Variant;
using static Windows.Win32.System.Com.ADVANCED_FEATURE_FLAGS;
using static Windows.Win32.System.Variant.VARENUM;

namespace Windows.Win32.System.Com;

internal unsafe partial struct SAFEARRAY
{
    /// <summary>
    ///  Gets the <see cref="SAFEARRAYBOUND"/> of the <paramref name="dimension"/>.
    /// </summary>
    public SAFEARRAYBOUND GetBounds(int dimension = 0)
    {
        fixed (void* b = &rgsabound)
        {
            return new ReadOnlySpan<SAFEARRAYBOUND>(b, cDims)[dimension];
        }
    }

    /// <summary>
    ///  Creates an empty one-dimensional SAFEARRAY of type <paramref name="arrayType"/>.
    /// </summary>
    public static SAFEARRAY* CreateEmpty(VARENUM arrayType)
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = 0,
            lLbound = 0
        };

        return PInvokeCore.SafeArrayCreate(arrayType, 1, &saBound);
    }

    public VARENUM VarType
    {
        get
        {
            // Match CLR behavior.
            ADVANCED_FEATURE_FLAGS hardwiredType = fFeatures & (FADF_BSTR | FADF_UNKNOWN | FADF_DISPATCH | FADF_VARIANT);
            if (hardwiredType == FADF_BSTR && cbElements == sizeof(char*))
            {
                return VT_BSTR;
            }
            else if (hardwiredType == FADF_UNKNOWN && cbElements == sizeof(IntPtr))
            {
                return VT_UNKNOWN;
            }
            else if (hardwiredType == FADF_DISPATCH && cbElements == sizeof(IntPtr))
            {
                return VT_DISPATCH;
            }
            else if (hardwiredType == FADF_VARIANT && cbElements == sizeof(VARIANT))
            {
                return VT_VARIANT;
            }

            // Call native API.
            VARENUM vt = VT_EMPTY;
            fixed (SAFEARRAY* pThis = &this)
            {
                PInvokeCore.SafeArrayGetVartype(pThis, &vt).ThrowOnFailure();
                return vt;
            }
        }
    }

    public T GetValue<T>(Span<int> indices)
    {
        // SAFEARRAY is laid out in column-major order.
        // See https://docs.microsoft.com/en-us/previous-versions/windows/desktop/automat/array-manipulation-functions
        int indicesIndex = 0;
        int c1 = indices[indicesIndex++];
        uint dimensionSize = 1;

        fixed (void* b = &rgsabound)
        {
            ReadOnlySpan<SAFEARRAYBOUND> bounds = new(b, cDims);

            int boundIndex = cDims - 1;

            uint cell = 0;
            for (ushort dim = 1; dim < cDims; dim++)
            {
                dimensionSize *= bounds[boundIndex--].cElements;

                int diff = (indices[indicesIndex++] - bounds[boundIndex].lLbound);
                cell += (uint)diff * dimensionSize;
            }

            cell += (uint)(c1 - bounds[cDims - 1].lLbound);

            void* v = Unsafe.Add<T>(pvData, (int)cell);
            return Unsafe.AsRef<T>(v);
        }
    }
}
