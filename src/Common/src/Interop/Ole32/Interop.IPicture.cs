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
        [Guid("7BF80980-BF32-101A-8BBB-00AA00300CAB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPicture
        {
            IntPtr Handle { get; }

            IntPtr hPal { get; }

            /// <remarks>
            ///  This is actually <see cref="PICTYPE"/>, but we can't describe it as such.
            ///  We want <see cref="PICTYPE"/> to be <see langword="uint"/> so that
            ///  <see cref="PICTDESC"/> is blittable.
            /// </remarks>
            short Type { get; }

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

            void SetHPal(IntPtr hPal);

            IntPtr CurDC { get; }

            IntPtr SelectPicture(IntPtr hDC, out IntPtr phDCOut);

            BOOL KeepOriginalFormat { get; set; }

            void PictureChanged();

            int SaveAsFile(
                /* opt IStream */ IntPtr pStream,
                BOOL fSaveMemCopy);

            uint Attributes { get; }
        }
    }
}
