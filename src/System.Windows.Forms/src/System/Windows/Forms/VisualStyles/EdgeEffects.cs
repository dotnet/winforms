// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    [Flags]
    public enum EdgeEffects
    {
        None = 0,
        FillInterior = (int)User32.BF.MIDDLE,
        Flat = (int)User32.BF.FLAT,
        Soft = (int)User32.BF.SOFT,
        Mono = (int)User32.BF.MONO,
    }
}
