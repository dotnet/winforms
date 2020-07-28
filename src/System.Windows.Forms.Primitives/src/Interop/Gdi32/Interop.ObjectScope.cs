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
#if DEBUG
        internal class ObjectScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct ObjectScope
#endif
        {
            public HGDIOBJ Object { get; }

            /// <param name="object">The object to be deleted when the scope closes.</param>
            public ObjectScope(HGDIOBJ @object)
            {
                Object = @object;
            }

            public static implicit operator HGDIOBJ(in ObjectScope objectScope) => objectScope.Object;

            public void Dispose()
            {
                if (Object.Handle != IntPtr.Zero)
                {
                    DeleteObject(Object);
                }

                DisposalTracking.SuppressFinalize(this!);
            }
        }
    }
}
