// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class EnumFORMATETCWrapper : IEnumFORMATETC
        {
            private IntPtr _wrappedInstance;

            public EnumFORMATETCWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            ~EnumFORMATETCWrapper()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public int Next(int celt, FORMATETC[] rgelt, int[]? pceltFetched)
            {
                fixed (FORMATETC* rgeltPtr = &MemoryMarshal.GetArrayDataReference(rgelt))
                {
                    if (pceltFetched is null)
                    {
                        return ((delegate* unmanaged<IntPtr, int, FORMATETC*, int*, int>)(*(*(void***)_wrappedInstance + 3)))
                            (_wrappedInstance, celt, rgeltPtr, null);
                    }
                    else
                    {
                        fixed (int* pceltFetchedPtr = &MemoryMarshal.GetArrayDataReference(pceltFetched))
                        {
                            return ((delegate* unmanaged<IntPtr, int, FORMATETC*, int*, int>)(*(*(void***)_wrappedInstance + 3)))
                                (_wrappedInstance, celt, rgeltPtr, pceltFetchedPtr);
                        }
                    }
                }
            }

            public int Skip(int celt)
            {
                return ((delegate* unmanaged<IntPtr, int, int>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, celt);
            }

            public int Reset()
            {
                return ((delegate* unmanaged<IntPtr, int>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance);
            }

            public void Clone(out IEnumFORMATETC newEnum)
            {
                IntPtr resultPtr;
                ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, &resultPtr).ThrowOnFailure();
                newEnum = (IEnumFORMATETC)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(resultPtr, CreateObjectFlags.Unwrap);
            }
        }
    }
}
