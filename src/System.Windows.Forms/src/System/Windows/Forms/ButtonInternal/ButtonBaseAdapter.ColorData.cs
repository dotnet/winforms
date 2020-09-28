// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal
{
    internal abstract partial class ButtonBaseAdapter
    {
        internal class ColorData
        {
            internal Color ButtonFace { get; set; }
            internal Color ButtonShadow { get; set; }
            internal Color ButtonShadowDark { get; set; }
            internal Color ConstrastButtonShadow { get; set; }
            internal Color WindowText { get; set; }
            internal Color WindowDisabled { get; set; }
            internal Color Highlight { get; set; }
            internal Color LowHighlight { get; set; }
            internal Color LowButtonFace { get; set; }
            internal Color WindowFrame { get; set; }

            internal ColorOptions Options { get; set; }

            internal ColorData(ColorOptions options)
            {
                Options = options;
            }
        }
    }
}
