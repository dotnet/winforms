// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Internal
{
    internal partial class IntNativeMethods
    {
        public const int
        /* FONT WEIGHT (BOLD) VALUES */
        FW_NORMAL = 400,
        FW_BOLD = 700,
        // some others...

        /* Font OutPrecision */
        OUT_DEFAULT_PRECIS = 0,
        OUT_TT_PRECIS = 4,
        OUT_TT_ONLY_PRECIS = 7,
        // some others...

        /* Font Quality */
        DEFAULT_QUALITY = 0,
        DRAFT_QUALITY = 1,
        PROOF_QUALITY = 2,
        NONANTIALIASED_QUALITY = 3,
        ANTIALIASED_QUALITY = 4,
        CLEARTYPE_QUALITY = 5,
        CLEARTYPE_NATURAL_QUALITY = 6,

        // Brush styles
        BS_SOLID = 0;
    }
}
