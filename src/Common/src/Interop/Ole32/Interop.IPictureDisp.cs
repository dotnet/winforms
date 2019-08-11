// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport()]
        [Guid("7BF80981-BF32-101A-8BBB-00AA00300CAB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        public interface IPictureDisp
        {
            IntPtr Handle { get; }

            IntPtr hPal { get; }

            PICTYPE Type { [return: MarshalAs(UnmanagedType.I2)] get; }

            int Width { get; }

            int Height { get; }

            void Render(
                IntPtr hDC,
                int x,
                int y,
                int cx,
                int cy,
                long xSrc,
                long ySrc,
                long cxSrc,
                long cySrc,
                ref RECT pRcWBounds);
        }
    }
}
