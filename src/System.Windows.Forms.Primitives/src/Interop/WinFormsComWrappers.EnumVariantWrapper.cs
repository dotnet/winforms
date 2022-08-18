// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class EnumVariantWrapper : Oleaut32.IEnumVariant
        {
            private IntPtr _wrappedInstance;

            public EnumVariantWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            ~EnumVariantWrapper()
            {
                DisposeInternal();
            }

            public void Dispose()
            {
                DisposeInternal();
                GC.SuppressFinalize(this);
            }

            private void DisposeInternal()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public HRESULT Next(uint celt, IntPtr rgVar, uint* pCeltFetched)
            {
                return ((delegate* unmanaged<IntPtr, uint, IntPtr, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                    (_wrappedInstance, celt, rgVar, pCeltFetched);
            }

            public HRESULT Skip(uint celt)
            {
                return ((delegate* unmanaged<IntPtr, uint, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, celt);
            }

            public HRESULT Reset()
            {
                return ((delegate* unmanaged<IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance);
            }

            public HRESULT Clone(Oleaut32.IEnumVariant[]? ppEnum)
            {
                if (ppEnum == null || ppEnum.Length == 0)
                {
                    return HRESULT.Values.E_POINTER;
                }

                IntPtr resultPtr;
                var result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, &resultPtr);
                if (result.Failed)
                {
                    ppEnum = null;
                    return result;
                }

                ppEnum[0] = (Oleaut32.IEnumVariant)WinFormsComWrappers.Instance
                    .GetOrCreateObjectForComInstance(resultPtr, CreateObjectFlags.Unwrap);
                return result;
            }
        }
    }
}
