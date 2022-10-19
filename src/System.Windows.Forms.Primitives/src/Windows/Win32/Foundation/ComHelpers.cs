// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation
{
    internal static unsafe class ComHelpers
    {
        internal static bool TryQueryInterface<T>(object obj, out T* pInterface) where T : unmanaged, INativeGuid
            => QueryInterface(obj, out pInterface).Succeeded;

        internal static HRESULT QueryInterface<T>(object obj, out T* pInterface) where T : unmanaged, INativeGuid
        {
            pInterface = null;
            using ComScope<IUnknown> unknown = new((IUnknown*)Marshal.GetIUnknownForObject(obj));
            void* ppvObject;
            HRESULT result = unknown.Value->QueryInterface(T.NativeGuid, &ppvObject);
            if (result.Succeeded)
            {
                pInterface = (T*)ppvObject;
            }

            return result;
        }

        public static void Release<T>(T* release) where T : unmanaged, IUnknown.Interface
        {
            if (release is not null)
            {
                ((IUnknown*)release)->Release();
            }
        }
    }
}
