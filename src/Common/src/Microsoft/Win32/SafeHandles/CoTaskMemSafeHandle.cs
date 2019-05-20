// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class CoTaskMemSafeHandle : SafeHandle
    {
        internal CoTaskMemSafeHandle() : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => IsClosed || handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(handle);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
