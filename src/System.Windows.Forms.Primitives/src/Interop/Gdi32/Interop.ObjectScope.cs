// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope lifetime of a GDI object. Deletes the given object (if any) when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double deletion.
        /// </remarks>
        internal ref struct ObjectScope
        {
            public HGDIOBJ Object { get; }

            /// <param name="object">The object to be deleted when the scope closes.</param>
            public ObjectScope(IntPtr @object)
            {
                Object = new HGDIOBJ(@object);
            }

            public static implicit operator IntPtr(ObjectScope objectScope) => objectScope.Object.Handle;

            public void Dispose()
            {
                if (Object.Handle != IntPtr.Zero)
                {
                    DeleteObject(Object);
                }
            }
        }
    }
}
