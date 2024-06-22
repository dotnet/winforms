// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a temporary list of the light color palette that is used when the system is in "normal light mode".
    ///  It's purely for testing purposes and will most likely be removed once the system colors are available.
    /// </summary>
    internal class ForcedLightThemedSystemColors : ThemedSystemColors
    {
        private static ForcedLightThemedSystemColors? s_instance;

        public static ForcedLightThemedSystemColors DefaultInstance => s_instance ??= new ForcedLightThemedSystemColors();

        public override Color ActiveBorder => Color.FromArgb(255, 180, 180, 180);
        public override Color ActiveCaption => Color.FromArgb(255, 153, 180, 209);
        public override Color ActiveCaptionText => Color.FromArgb(255, 0, 0, 0);
        public override Color AppWorkspace => Color.FromArgb(255, 171, 171, 171);
        public override Color ButtonFace => Color.FromArgb(255, 240, 240, 240);
        public override Color ButtonHighlight => Color.FromArgb(255, 255, 255, 255);
        public override Color ButtonShadow => Color.FromArgb(255, 160, 160, 160);
        public override Color Control => Color.FromArgb(255, 240, 240, 240);
        public override Color ControlDark => Color.FromArgb(255, 160, 160, 160);
        public override Color ControlDarkDark => Color.FromArgb(255, 105, 105, 105);
        public override Color ControlLight => Color.FromArgb(255, 227, 227, 227);
        public override Color ControlLightLight => Color.FromArgb(255, 255, 255, 255);
        public override Color ControlText => Color.FromArgb(255, 0, 0, 0);
        public override Color Desktop => Color.FromArgb(255, 0, 0, 0);
        public override Color GradientActiveCaption => Color.FromArgb(255, 185, 209, 234);
        public override Color GradientInactiveCaption => Color.FromArgb(255, 215, 228, 242);
        public override Color GrayText => Color.FromArgb(255, 109, 109, 109);
        public override Color Highlight => Color.FromArgb(255, 0, 120, 215);
        public override Color HighlightText => Color.FromArgb(255, 255, 255, 255);
        public override Color HotTrack => Color.FromArgb(255, 0, 102, 204);
        public override Color InactiveBorder => Color.FromArgb(255, 244, 247, 252);
        public override Color InactiveCaption => Color.FromArgb(255, 191, 205, 219);
        public override Color InactiveCaptionText => Color.FromArgb(255, 0, 0, 0);
        public override Color Info => Color.FromArgb(255, 255, 255, 225);
        public override Color InfoText => Color.FromArgb(255, 0, 0, 0);
        public override Color Menu => Color.FromArgb(255, 240, 240, 240);
        public override Color MenuBar => Color.FromArgb(255, 240, 240, 240);
        public override Color MenuHighlight => Color.FromArgb(255, 51, 153, 255);
        public override Color MenuText => Color.FromArgb(255, 0, 0, 0);
        public override Color ScrollBar => Color.FromArgb(255, 200, 200, 200);
        public override Color Window => Color.FromArgb(255, 255, 255, 255);
        public override Color WindowFrame => Color.FromArgb(255, 100, 100, 100);
        public override Color WindowText => Color.FromArgb(255, 0, 0, 0);
    }
}
