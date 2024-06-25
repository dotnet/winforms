// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms
{
    internal class DarkProfessionalColors : ProfessionalColorTable
    {
        public override Color MenuItemPressedGradientBegin
            => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

        public override Color MenuItemPressedGradientMiddle
            => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

        public override Color MenuItemPressedGradientEnd
            => Color.FromArgb(0xFF, 0x60, 0x60, 0x60);

        public override Color MenuItemSelected
            => Application.ApplicationColors.ControlText;

        public override Color MenuItemSelectedGradientBegin
            => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);

        public override Color MenuItemSelectedGradientEnd
            => Color.FromArgb(0xFF, 0x40, 0x40, 0x40);

        public override Color MenuStripGradientBegin
            => Application.ApplicationColors.Control;

        public override Color MenuStripGradientEnd
            => Application.ApplicationColors.Control;

        public override Color StatusStripGradientBegin
            => Application.ApplicationColors.Control;

        public override Color StatusStripGradientEnd
            => Application.ApplicationColors.Control;

        public override Color ToolStripDropDownBackground
            => Application.ApplicationColors.Control;

        public override Color ImageMarginGradientBegin
            => Application.ApplicationColors.Control;

        public override Color ImageMarginGradientMiddle
            => Application.ApplicationColors.Control;

        public override Color ImageMarginGradientEnd
            => Application.ApplicationColors.Control;
    }
}
