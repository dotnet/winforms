// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class LockBytesWrapper
        {
            private IntPtr _wrappedInstance;

            public LockBytesWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public void Stat(out Ole32.STATSTG statstg, Ole32.STATFLAG grfStatFlag)
            {
                fixed (Ole32.STATSTG* pstatstg = &statstg)
                {
                    ((delegate* unmanaged<IntPtr, Ole32.STATSTG*, Ole32.STATFLAG, HRESULT>)(*(*(void***)_wrappedInstance + 9 /* ILockBytes.Stat */)))
                        (_wrappedInstance, pstatstg, grfStatFlag).ThrowOnFailure();
                }
            }
        }
    }
}
