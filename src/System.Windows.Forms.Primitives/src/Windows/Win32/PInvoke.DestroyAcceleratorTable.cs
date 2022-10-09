// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL DestroyAcceleratorTable<T>(T hAccel)
             where T : IHandle<HACCEL>
        {
            BOOL result = DestroyAcceleratorTable(hAccel.Handle);
            GC.KeepAlive(hAccel.Wrapper);
            return result;
        }
    }
}
