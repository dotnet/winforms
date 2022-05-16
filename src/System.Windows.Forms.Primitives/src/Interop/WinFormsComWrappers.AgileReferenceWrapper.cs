// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class AgileReferenceWrapper
        {
            private IntPtr _wrappedInstance;

            public AgileReferenceWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            ~AgileReferenceWrapper()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public unsafe IntPtr Resolve(Guid iid)
            {
                IntPtr result = IntPtr.Zero;
                ((delegate* unmanaged<IntPtr, Guid*, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                    (_wrappedInstance, &iid, &result).ThrowIfFailed();
                return result;
            }
        }
    }
}
