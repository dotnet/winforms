// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope selecting a GDI object into an HDC. Restores the original
        ///  object into the HDC when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double selection.
        /// </remarks>
        internal ref struct SelectObjectScope
        {
            private readonly HDC _hdc;
            public HGDIOBJ PreviousObject;

            /// <summary>
            ///  Selects <paramref name="object"/> into the given <paramref name="hdc"/>.
            /// </summary>
            public SelectObjectScope(HDC hdc, HGDIOBJ @object)
            {
                _hdc = hdc;
                PreviousObject = SelectObject(hdc, @object);
            }

            public void Dispose()
            {
                SelectObject(_hdc, PreviousObject);
            }
        }
    }
}
