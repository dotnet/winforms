// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal abstract class ThemedSystemColors
    {
        public virtual Color ActiveBorder { get; } = SystemColors.ActiveBorder;
        public virtual Color Window { get; } = SystemColors.Window;
        public virtual Color ScrollBar { get; } = SystemColors.ScrollBar;
        public virtual Color MenuText { get; } = SystemColors.MenuText;
        public virtual Color MenuHighlight { get; } = SystemColors.MenuHighlight;
        public virtual Color MenuBar { get; } = SystemColors.MenuBar;
        public virtual Color Menu { get; } = SystemColors.Menu;
        public virtual Color InfoText { get; } = SystemColors.InfoText;
        public virtual Color Info { get; } = SystemColors.Info;
        public virtual Color InactiveCaptionText { get; } = SystemColors.InactiveCaptionText;
        public virtual Color InactiveCaption { get; } = SystemColors.InactiveCaption;
        public virtual Color InactiveBorder { get; } = SystemColors.InactiveBorder;
        public virtual Color HotTrack { get; } = SystemColors.HotTrack;
        public virtual Color HighlightText { get; } = SystemColors.HighlightText;
        public virtual Color Highlight { get; } = SystemColors.Highlight;
        public virtual Color WindowFrame { get; } = SystemColors.WindowFrame;
        public virtual Color GrayText { get; } = SystemColors.GrayText;
        public virtual Color GradientActiveCaption { get; } = SystemColors.GradientActiveCaption;
        public virtual Color Desktop { get; } = SystemColors.Desktop;
        public virtual Color ControlText { get; } = SystemColors.ControlText;
        public virtual Color ControlLightLight { get; } = SystemColors.ControlLightLight;
        public virtual Color ControlLight { get; } = SystemColors.ControlLight;
        public virtual Color ControlDarkDark { get; } = SystemColors.ControlDarkDark;
        public virtual Color ControlDark { get; } = SystemColors.ControlDark;
        public virtual Color Control { get; } = SystemColors.Control;
        public virtual Color ButtonShadow { get; } = SystemColors.ButtonShadow;
        public virtual Color ButtonHighlight { get; } = SystemColors.ButtonHighlight;
        public virtual Color ButtonFace { get; } = SystemColors.ButtonFace;
        public virtual Color AppWorkspace { get; } = SystemColors.AppWorkspace;
        public virtual Color ActiveCaptionText { get; } = SystemColors.ActiveCaptionText;
        public virtual Color ActiveCaption { get; } = SystemColors.ActiveCaption;
        public virtual Color GradientInactiveCaption { get; } = SystemColors.GradientActiveCaption;
        public virtual Color WindowText { get; } = SystemColors.WindowText;
    }
}
