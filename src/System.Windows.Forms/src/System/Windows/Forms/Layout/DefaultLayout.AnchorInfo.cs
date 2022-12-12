// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Layout
{
    internal partial class DefaultLayout
    {
        internal sealed class AnchorInfo
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            // In PermonV2 mode applications, when moved from one monitor to the other, Form/Container is scaled with scale
            // factor computed with respect to new DPI on the monitor. However, scaling ratio between non-client and client
            // area is not linear. This is causing few pixels variance when anchors are also scaled with same scale factor.
            // Anchors are relative to display rectangle.Hence, making change to computing anchor scale factor with respect
            // to change in DisplayRect instead of change in the bounds (a.k.a: scale factor computed with respect to new DPI).
            public Rectangle DisplayRect;
        }
    }
}
