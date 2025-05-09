// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static partial class ButtonDarkModeRenderer
{
    /// <summary>Cache of colors for different button states</summary>
    internal static class DarkModeButtonColors
    {
        // Normal Button (non-default)
        /// <summary>Button background color for normal state (#2B2B2B)</summary>
        public static Color NormalBackgroundColor => Color.FromArgb(43, 43, 43);

        /// <summary>Button background color for hover state (#3B3B3B)</summary>
        public static Color HoverBackgroundColor => Color.FromArgb(59, 59, 59);

        /// <summary>Button background color for pressed state (#4B4B4B)</summary>
        public static Color PressedBackgroundColor => Color.FromArgb(75, 75, 75);

        /// <summary>Button background color for disabled state (#252525)</summary>
        public static Color DisabledBackgroundColor => Color.FromArgb(37, 37, 37);

        // Default Button
        /// <summary>Default button background color (#6B2FBF)</summary>
        public static Color DefaultBackgroundColor => Color.FromArgb(107, 47, 191);

        /// <summary>Default button hover background color (#7C3FD0)</summary>
        public static Color DefaultHoverBackgroundColor => Color.FromArgb(124, 63, 208);

        /// <summary>Default button pressed background color (#5B1FAF)</summary>
        public static Color DefaultPressedBackgroundColor => Color.FromArgb(91, 31, 175);

        /// <summary>Default button disabled background color (desaturated accent)</summary>
        public static Color DefaultDisabledBackgroundColor => Color.FromArgb(85, 40, 140);

        // Text Colors
        /// <summary>Normal text color (#E0E0E0)</summary>
        public static Color NormalTextColor => Color.FromArgb(224, 224, 224);

        /// <summary>Default button text color (#FFFFFF)</summary>
        public static Color DefaultTextColor => Color.White;

        /// <summary>Disabled text color (~40% opacity, #606060)</summary>
        public static Color DisabledTextColor => Color.FromArgb(96, 96, 96);

        // Border Colors
        /// <summary>Button single border color (#555555)</summary>
        public static Color SingleBorderColor => Color.FromArgb(85, 85, 85);

        /// <summary>Button top-left border color (#555555)</summary>
        public static Color TopLeftBorderColor => Color.FromArgb(85, 85, 85);

        /// <summary>Button bottom-right border color (#222222)</summary>
        public static Color BottomRightBorderColor => Color.FromArgb(34, 34, 34);

        // Focus Colors
        /// <summary>Focus indicator color (#AAAAAA)</summary>
        public static Color FocusIndicatorColor => Color.FromArgb(170, 170, 170);

        /// <summary>Default button focus indicator color (#FFFFFF)</summary>
        public static Color DefaultFocusIndicatorColor => Color.White;
    }
}
