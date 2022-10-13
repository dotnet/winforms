// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation
{
    internal readonly unsafe ref struct ComScope<T> where T : unmanaged
    {
        // Keeping internal as nint allows us to use Unsafe methods to get significantly better generated code.
        private readonly nint _value;
        public T* Value => (T*)_value;

        public ComScope(T* value) => _value = (nint)value;

        public static implicit operator T*(in ComScope<T> scope) => (T*)scope._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T**(in ComScope<T> scope) => (T**)Unsafe.AsPointer(ref Unsafe.AsRef(scope._value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void**(in ComScope<T> scope) => (void**)Unsafe.AsPointer(ref Unsafe.AsRef(scope._value));

        public bool IsNull => _value == 0;

        public void Dispose()
        {
            IUnknown* unknown = (IUnknown*)_value;
            if (unknown is not null)
            {
                unknown->Release();
            }
        }
    }
}
