// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Specifies the layout of a device context.
    /// </summary>
    [Flags]
    internal enum DeviceContextBinaryRasterOperationFlags
    {
        Black = 1,          //  0
        NotMergePen = 2,    // dpon
        MaskNotPen = 3,     // dpna
        NotCopyPen = 4,     // pn
        MaskPenNot = 5,     // pdna
        Not = 6,            // dn
        XorPen = 7,         // dpx
        NotMaskPen = 8,     // dpan
        MaskPen = 9,        // dpa
        NotXorPen = 10,     // dpxn
        Nop = 11,           // d
        MergeNotPen = 12,   // dpno
        CopyPen = 13,       // p
        MergePenNot = 14,   // pdno
        MergePen = 15,      // dpo
        White = 16,         //  1

        // Binary raster operations.
        /*
        R2_BLACK            = 1,  //  0
        R2_NOTMERGEPEN      = 2,  // DPon
        R2_MASKNOTPEN       = 3,  // DPna
        R2_NOTCOPYPEN       = 4,  // PN
        R2_MASKPENNOT       = 5,  // PDna
        R2_NOT              = 6,  // Dn
        R2_XORPEN           = 7,  // DPx
        R2_NOTMASKPEN       = 8,  // DPan
        R2_MASKPEN          = 9,  // DPa
        R2_NOTXORPEN        = 10, // DPxn
        R2_NOP              = 11, // D
        R2_MERGENOTPEN      = 12, // DPno
        R2_COPYPEN          = 13, // P
        R2_MERGEPENNOT      = 14, // PDno
        R2_MERGEPEN         = 15, // DPo
        R2_WHITE            = 16, //  1
        */
    }
}
