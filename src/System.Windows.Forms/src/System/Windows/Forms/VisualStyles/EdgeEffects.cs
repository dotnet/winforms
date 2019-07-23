// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    [Flags]
    public enum EdgeEffects
    {
        None = 0,
        FillInterior = 0x0800,
        Flat = 0x1000,
        Soft = 0x4000,
        Mono = 0x8000,
        //	#define BF_SOFT         0x1000  /* For softer buttons */
        //	#define BF_FLAT         0x4000  /* For flat rather than 3D borders */
        //	#define BF_MONO         0x8000  /* For monochrome borders */
    }
}
