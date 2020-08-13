// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("7BF80981-BF32-101A-8BBB-00AA00300CAB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        internal unsafe interface IPictureDisp
        {
            /// <remarks>
            ///  IPicture handles are sign extended on 64 bit devices. To preserve
            ///  the sign, handles should be represented as ints, not uints.
            /// </remarks>
            int Handle { get; }

            /// <remarks>
            ///  HPALETTE handles are sign extended on 64 bit devices. To preserve
            ///  the sign, handles should be represented as ints, not uints.
            /// </remarks>
            int hPal { get; }

            /// <remarks>
            ///  This is actually <see cref="PICTYPE"/>, but we can't describe it as such.
            ///  We want <see cref="PICTYPE"/> to be <see langword="uint"/> so that
            ///  <see cref="PICTDESC"/> is blittable.
            /// </remarks>
            short Type { get; }

            int Width { get; }

            int Height { get; }

            // There are other methods, but we don't explicitly use them. See IPicture.
        }
    }
}
