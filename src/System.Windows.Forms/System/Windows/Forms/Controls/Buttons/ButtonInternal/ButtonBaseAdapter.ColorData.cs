// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ButtonInternal;

internal abstract partial class ButtonBaseAdapter
{
    internal class ColorData(ColorOptions options)
    {
        public Color ButtonFace { get; set; }
        public Color ButtonShadow { get; set; }
        public Color ButtonShadowDark { get; set; }
        public Color ContrastButtonShadow { get; set; }
        public Color WindowText { get; set; }
        public Color WindowDisabled { get; set; }
        public Color Highlight { get; set; }
        public Color LowHighlight { get; set; }
        public Color LowButtonFace { get; set; }
        public Color WindowFrame { get; set; }
        public ColorOptions Options { get; set; } = options;
    }
}
