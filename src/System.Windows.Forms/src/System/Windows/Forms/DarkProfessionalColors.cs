// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static System.Windows.Forms.Control;

namespace System.Windows.Forms
{
#pragma warning disable WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal class DarkProfessionalColors : ProfessionalColorTable
    {
        public override Color MenuItemPressedGradientBegin
            => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

        public override Color MenuItemPressedGradientMiddle
            => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

        public override Color MenuItemPressedGradientEnd
            => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

        public override Color MenuItemSelected
            => ControlSystemColors.Current.ControlText;

        public override Color MenuItemSelectedGradientBegin
            => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);

        public override Color MenuItemSelectedGradientEnd
            => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);

        public override Color MenuStripGradientBegin
            => ControlSystemColors.Current.Control;

        public override Color MenuStripGradientEnd
            => ControlSystemColors.Current.Control;

        public override Color StatusStripGradientBegin
            => ControlSystemColors.Current.Control;

        public override Color StatusStripGradientEnd
            => ControlSystemColors.Current.Control;

        public override Color ToolStripDropDownBackground
            => ControlSystemColors.Current.Control;

        public override Color ImageMarginGradientBegin
            => ControlSystemColors.Current.Control;

        public override Color ImageMarginGradientMiddle
            => ControlSystemColors.Current.Control;

        public override Color ImageMarginGradientEnd
            => ControlSystemColors.Current.Control;
    }
#pragma warning restore WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
