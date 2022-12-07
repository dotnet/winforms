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

            // Display rectangle of the parent/container used in computing child control's anchors.
            public Rectangle DisplayRect;
        }
    }
}
