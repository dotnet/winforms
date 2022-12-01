// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation
{
    internal static unsafe class ComHelpers
    {
        // Note that ComScope<T> needs to be the return value to faciliate using in a `using`.
        //
        //  using var stream = GetComScope<IStream>(obj, out bool success);

        /// <summary>
        ///  Attempts to get a pointer for the specified <typeparamref name="T"/> for the given <paramref name="obj"/>.
        /// </summary>
        internal static ComScope<T> GetComScope<T>(object obj, out HRESULT hr) where T : unmanaged, IComIID
        {
            hr = GetComPointer(obj, out T* pInterface);
            return new(pInterface);
        }

        /// <summary>
        ///  Attempts to get a pointer for the specified <typeparamref name="T"/> for the given <paramref name="obj"/>.
        /// </summary>
        internal static ComScope<T> GetComScope<T>(object obj, out bool success) where T : unmanaged, IComIID
        {
            success = TryGetComPointer(obj, out T* pInterface);
            return new(pInterface);
        }

        /// <summary>
        ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="obj"/>.
        /// </summary>
        internal static HRESULT GetComPointer<T>(object? obj, out T* ppvObject) where T : unmanaged, IComIID
        {
            ppvObject = null;

            if (obj is null)
            {
                return HRESULT.E_POINTER;
            }

            IUnknown* ccw = null;
            if (obj is IManagedWrapper)
            {
                ccw = (IUnknown*)Interop.WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None);
            }
            else
            {
                // We haven't converted this, fall back to COM interop.
                ccw = (IUnknown*)Marshal.GetIUnknownForObject(obj);
            }

            if (ccw is null)
            {
                return HRESULT.E_NOINTERFACE;
            }

            fixed (T** unknown = &ppvObject)
            {
                HRESULT result = ccw->QueryInterface(IID.Get<T>(), (void**)unknown);
                ccw->Release();

                return result;
            }
        }

        /// <summary>
        ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="obj"/>.
        /// </summary>
        internal static bool TryGetComPointer<T>(object? obj, out T* ppvObject) where T : unmanaged, IComIID
            => GetComPointer(obj, out ppvObject).Succeeded;
    }
}
