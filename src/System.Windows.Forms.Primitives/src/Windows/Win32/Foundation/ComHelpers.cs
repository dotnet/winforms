// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation
{
    internal static unsafe class ComHelpers
    {
        /// <summary>
        ///  Attempts to get the specified <paramref name="iid"/> interface for the given <paramref name="obj"/>.
        /// </summary>
        internal static bool TryGetComPointer<T>(object? obj, in Guid iid, out T* ppvObject) where T : unmanaged
            => GetComPointer(obj, in iid, out ppvObject).Succeeded;

        internal static ComScope<T> GetComScope<T>(object obj, out HRESULT hr) where T : unmanaged, INativeGuid
        {
            hr = GetComPointer(obj, T.NativeGuid, out T* pInterface);
            return new(pInterface);
        }

        internal static ComScope<T> GetComScope<T>(object obj, out bool success) where T : unmanaged, INativeGuid
        {
            success = TryGetComPointer(obj, out T* pInterface);
            return new(pInterface);
        }

        internal static HRESULT GetComPointer<T>(object? obj, in Guid iid, out T* ppvObject) where T : unmanaged
        {
            fixed (Guid* pGuid = &iid)
            {
                return GetComPointer(obj, pGuid, out ppvObject);
            }
        }

        internal static HRESULT GetComPointer<T>(object? obj, Guid* iid, out T* ppvObject) where T : unmanaged
        {
            ppvObject = null;

            if (obj is null)
            {
                return HRESULT.E_POINTER;
            }

            IUnknown* ccw = (IUnknown*)Interop.WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None);
            if (ccw is null)
            {
                // We haven't converted this, fall back to COM interop.
                ccw = (IUnknown*)Marshal.GetIUnknownForObject(obj);
                if (ccw is null)
                {
                    return HRESULT.E_NOINTERFACE;
                }
            }

            fixed (T** unknown = &ppvObject)
            {
                HRESULT result = ccw->QueryInterface(iid, (void**)unknown);
                ccw->Release();

                return result;
            }
        }

        /// <summary>
        ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="obj"/>.
        /// </summary>
        internal static bool TryGetComPointer<T>(object? obj, out T* ppvObject) where T : unmanaged, INativeGuid
            => GetComPointer(obj, T.NativeGuid, out ppvObject).Succeeded;

        internal static HRESULT GetDispatchProperty(
            IDispatch* dispatch,
            uint dispId,
            VARIANT* pVar,
            uint lcid = 0)
        {
            Guid riid = Guid.Empty;
            DISPPARAMS disparams = default;
            return dispatch->Invoke((int)dispId, &riid, lcid, DISPATCH_FLAGS.DISPATCH_PROPERTYGET, &disparams, pVar, null, null);
        }
    }
}
