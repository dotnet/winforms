// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

internal abstract partial class ButtonBaseAdapter
{
    internal class LayoutData
    {
        public Rectangle Client;
        public Rectangle Face;
        public Rectangle CheckArea;
        public Rectangle CheckBounds;
        public Rectangle TextBounds;
        public Rectangle Field;
        public Rectangle Focus;
        public Rectangle ImageBounds;

        // For .NET Framework 1.1 compatibility
        public Point ImageStart;
        public LayoutOptions Options;

        internal LayoutData(LayoutOptions options)
        {
            Debug.Assert(options is not null, "must have options");
            Options = options;
        }
    }
}
