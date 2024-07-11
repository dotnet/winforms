// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms
{
    internal class DarkThemedApplicationColors : ApplicationColors
    {
        private static DarkThemedApplicationColors? s_instance;

        public static DarkThemedApplicationColors DefaultInstance => s_instance ??= new DarkThemedApplicationColors();

        public override Color ActiveBorder => Color.FromArgb(255, 70, 70, 70);
        public override Color ActiveCaption => Color.FromArgb(255, 60, 95, 120);
        public override Color ActiveCaptionText => Color.FromArgb(255, 190, 190, 190);
        public override Color AppWorkspace => Color.FromArgb(255, 60, 60, 60);
        public override Color ButtonFace => Color.FromArgb(255, 55, 55, 55);
        public override Color ButtonHighlight => Color.FromArgb(255, 105, 105, 105);
        public override Color ButtonShadow => Color.FromArgb(255, 70, 70, 70);
        public override Color Control => Color.FromArgb(255, 32, 32, 32);
        public override Color ControlDark => Color.FromArgb(255, 70, 70, 70);
        public override Color ControlDarkDark => Color.FromArgb(255, 45, 45, 45);
        public override Color ControlLight => Color.FromArgb(255, 75, 75, 75);
        public override Color ControlLightLight => Color.FromArgb(255, 95, 95, 95);
        public override Color ControlText => Color.FromArgb(255, 190, 190, 190);
        public override Color Desktop => Color.FromArgb(255, 30, 30, 30);
        public override Color GradientActiveCaption => Color.FromArgb(255, 65, 100, 130);
        public override Color GradientInactiveCaption => Color.FromArgb(255, 85, 115, 150);
        public override Color GrayText => Color.FromArgb(255, 140, 140, 140);
        public override Color Highlight => Color.FromArgb(255, 40, 100, 180);
        public override Color HighlightText => Color.FromArgb(255, 255, 255, 255);
        public override Color HotTrack => Color.FromArgb(255, 45, 95, 175);
        public override Color InactiveBorder => Color.FromArgb(255, 60, 63, 65);
        public override Color InactiveCaption => Color.FromArgb(255, 55, 75, 90);
        public override Color InactiveCaptionText => Color.FromArgb(255, 190, 190, 190);
        public override Color Info => Color.FromArgb(255, 80, 80, 60);
        public override Color InfoText => Color.FromArgb(255, 190, 190, 190);
        public override Color Menu => Color.FromArgb(255, 55, 55, 55);
        public override Color MenuBar => Color.FromArgb(255, 55, 55, 55);
        public override Color MenuHighlight => Color.FromArgb(255, 42, 128, 210);
        public override Color MenuText => Color.FromArgb(255, 240, 240, 240);
        public override Color ScrollBar => Color.FromArgb(255, 80, 80, 80);
        public override Color Window => Color.FromArgb(255, 50, 50, 50);
        public override Color WindowFrame => Color.FromArgb(255, 40, 40, 40);
        public override Color WindowText => Color.FromArgb(255, 240, 240, 240);
    }
}
