// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation
{
    /// <summary>
    ///  Finalizable wrapper for COM pointers that gives agile access to the specified interface.
    /// </summary>
    internal sealed unsafe class AgileComPointer<TInterface> : IDisposable
        where TInterface : unmanaged, INativeGuid
    {
        private readonly uint _cookie;

        public AgileComPointer(TInterface* @interface)
        {
            _cookie = GlobalInterfaceTable.RegisterInterface(@interface);

            // We let the GlobalInterfaceTable maintain the ref count here
            uint count = ((IUnknown*)@interface)->Release();
            Debug.Assert(count == 1);
        }

        public ComScope<TInterface> GetInterface()
        {
            var scope = GlobalInterfaceTable.GetInterface<TInterface>(_cookie, out HRESULT hr);
            Debug.Assert(hr.Succeeded);
            return scope;
        }

        ~AgileComPointer()
        {
            Debug.Fail("Did not dispose AgileComPointer");
            Dispose();
        }

        public void Dispose()
        {
            HRESULT hr = GlobalInterfaceTable.RevokeInterface(_cookie);
            Debug.Assert(hr.Succeeded);
            GC.SuppressFinalize(this);
        }
    }
}
