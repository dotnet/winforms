// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static Interop.Ole32;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        public unsafe struct SAFEARRAY
        {
            public ushort cDims;
            public FADF fFeatures;
            public uint cbElements;
            public uint cLocks;
            public void* pvData;
            public SAFEARRAYBOUND _rgsabound;

            public ReadOnlySpan<SAFEARRAYBOUND> rgsabound => TrailingArray<SAFEARRAYBOUND>.GetBuffer(ref _rgsabound, cDims);

            public VARENUM VarType
            {
                get
                {
                    // Match CLR behaviour.
                    FADF hardwiredType = fFeatures & (FADF.BSTR | FADF.UNKNOWN | FADF.DISPATCH | FADF.VARIANT);
                    if (hardwiredType == FADF.BSTR && cbElements == sizeof(char*))
                    {
                        return VARENUM.BSTR;
                    }
                    else if (hardwiredType == FADF.UNKNOWN && cbElements == sizeof(IntPtr))
                    {
                        return VARENUM.UNKNOWN;
                    }
                    else if (hardwiredType == FADF.DISPATCH && cbElements == sizeof(IntPtr))
                    {
                        return VARENUM.DISPATCH;
                    }
                    else if (hardwiredType == FADF.VARIANT && cbElements == sizeof(VARIANT))
                    {
                        return VARENUM.VARIANT;
                    }

                    // Call native API.
                    VARENUM vt = VARENUM.EMPTY;
                    fixed (SAFEARRAY* pThis = &this)
                    {
                        SafeArrayGetVartype(pThis, &vt);
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

                ReadOnlySpan<SAFEARRAYBOUND> bounds = rgsabound;
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
}
