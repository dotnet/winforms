// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

public unsafe partial class Control
{
    [Experimental("WFO9001")]
    public class ControlSystemColors
    {
        private readonly Color[] _mappedSystemColors;

        private static readonly Color[] s_systemColors =
        [
            Drawing.SystemColors.ActiveBorder,
            Drawing.SystemColors.ActiveCaption,
            Drawing.SystemColors.ActiveCaptionText,
            Drawing.SystemColors.AppWorkspace,
            Drawing.SystemColors.Control,
            Drawing.SystemColors.ControlDark,
            Drawing.SystemColors.ControlDarkDark,
            Drawing.SystemColors.ControlLight,
            Drawing.SystemColors.ControlLightLight,
            Drawing.SystemColors.ControlText,
            Drawing.SystemColors.Desktop,
            Drawing.SystemColors.GrayText,
            Drawing.SystemColors.Highlight,
            Drawing.SystemColors.HighlightText,
            Drawing.SystemColors.HotTrack,
            Drawing.SystemColors.InactiveBorder,
            Drawing.SystemColors.InactiveCaption,
            Drawing.SystemColors.InactiveCaptionText,
            Drawing.SystemColors.Info,
            Drawing.SystemColors.InfoText,
            Drawing.SystemColors.Menu,
            Drawing.SystemColors.MenuText,
            Drawing.SystemColors.ScrollBar,
            Drawing.SystemColors.Window,
            Drawing.SystemColors.WindowFrame,
            Drawing.SystemColors.WindowText,
            Drawing.SystemColors.ButtonFace,
            Drawing.SystemColors.ButtonHighlight,
            Drawing.SystemColors.ButtonShadow,
            Drawing.SystemColors.GradientActiveCaption,
            Drawing.SystemColors.GradientInactiveCaption,
            Drawing.SystemColors.MenuBar,
            Drawing.SystemColors.MenuHighlight
        ];

        private static readonly Color[] s_darkSystemColors =
        [
            unchecked(Color.FromArgb((int)0xFF2D2D2D)), // ActiveBorder - Dark gray
            unchecked(Color.FromArgb((int)0xFF0078D4)), // ActiveCaption - Highlighted Text Background
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // ActiveCaptionText - White
            unchecked(Color.FromArgb((int)0xFF252526)), // AppWorkspace - Panel Background
            unchecked(Color.FromArgb((int)0xFF202020)), // Control - Normal Panel/Windows Background
            unchecked(Color.FromArgb((int)0xFF4A4A4A)), // ControlDark - A lighter gray for dark mode
            unchecked(Color.FromArgb((int)0xFF5A5A5A)), // ControlDarkDark - An even lighter gray for dark mode
            unchecked(Color.FromArgb((int)0xFF2E2E2E)), // ControlLight - Unfocused Textbox Background
            unchecked(Color.FromArgb((int)0xFF1F1F1F)), // ControlLightLight - Focused Textbox Background
            unchecked(Color.FromArgb((int)0xFFE0E0E0)), // ControlText - Control Forecolor and Text Color
            unchecked(Color.FromArgb((int)0xFF000000)), // Desktop - Black
            unchecked(Color.FromArgb((int)0xFF969696)), // GrayText - Prompt Text Focused TextBox
            unchecked(Color.FromArgb((int)0xFF2B2B2B)), // Highlight - Highlighted Panel in DarkMode
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // HighlightText - White
            unchecked(Color.FromArgb((int)0xFF4CC2FF)), // HotTrack - Background of the ToggleSwitch
            unchecked(Color.FromArgb((int)0xFF2D2D2D)), // InactiveBorder - Dark gray
            unchecked(Color.FromArgb((int)0xFF2B2B2B)), // InactiveCaption - Highlighted Panel in DarkMode
            unchecked(Color.FromArgb((int)0xFF121212)), // InactiveCaptionText - Middle Dark Panel
            unchecked(Color.FromArgb((int)0xFF99EBFF)), // Info - Link Label
            unchecked(Color.FromArgb((int)0xFFCFCFCF)), // InfoText - Prompt Text Color
            unchecked(Color.FromArgb((int)0xFF2E2E2E)), // Menu - Normal Menu Background
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // MenuText - White
            unchecked(Color.FromArgb((int)0xFF9A9A9A)), // ScrollBar - Scrollbars and Scrollbar Arrows
            unchecked(Color.FromArgb((int)0xFF202020)), // Window - Window Background
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // WindowFrame - White
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // WindowText - White
            unchecked(Color.FromArgb((int)0xFF202020)), // ButtonFace - Same as Window Background
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // ButtonHighlight - White
            unchecked(Color.FromArgb((int)0xFF9A9A9A)), // ButtonShadow - Same as Scrollbar Elements
            unchecked(Color.FromArgb((int)0xFF0078D4)), // GradientActiveCaption - Same as Highlighted Text Background
            unchecked(Color.FromArgb((int)0xFF2B2B2B)), // GradientInactiveCaption - Same as Highlighted Panel in DarkMode
            unchecked(Color.FromArgb((int)0xFF2E2E2E)), // MenuBar - Same as Normal Menu Background
            unchecked(Color.FromArgb((int)0xFF3D3D3D))  // MenuHighlight - Same as Highlighted Menu Background
        ];

        internal ControlSystemColors(bool isDarkMode = false)
        {
            var sourceColors = isDarkMode ? s_darkSystemColors : s_systemColors;
            _mappedSystemColors = new Color[sourceColors.Length];

            var sourceSpan = sourceColors.AsSpan();
            var targetSpan = _mappedSystemColors.AsSpan();

            for (int i = 0; i < sourceSpan.Length; i++)
            {
                targetSpan[i] = sourceSpan[i];
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            ActiveBorder = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ActiveBorder)];
            ActiveCaption = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ActiveCaption)];
            ActiveCaptionText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ActiveCaptionText)];
            AppWorkspace = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.AppWorkspace)];
            Control = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.Control)];
            ControlDark = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ControlDark)];
            ControlDarkDark = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ControlDarkDark)];
            ControlLight = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ControlLight)];
            ControlLightLight = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ControlLightLight)];
            ControlText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ControlText)];
            Desktop = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.Desktop)];
            GrayText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.GrayText)];
            Highlight = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.Highlight)];
            HighlightText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.HighlightText)];
            HotTrack = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.HotTrack)];
            InactiveBorder = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.InactiveBorder)];
            InactiveCaption = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.InactiveCaption)];
            InactiveCaptionText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.InactiveCaptionText)];
            Info = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.Info)];
            InfoText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.InfoText)];
            Menu = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.Menu)];
            MenuText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.MenuText)];
            ScrollBar = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ScrollBar)];
            Window = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.Window)];
            WindowFrame = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.WindowFrame)];
            WindowText = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.WindowText)];
            ButtonFace = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ButtonFace)];
            ButtonHighlight = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ButtonHighlight)];
            ButtonShadow = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.ButtonShadow)];
            GradientActiveCaption = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.GradientActiveCaption)];
            GradientInactiveCaption = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.GradientInactiveCaption)];
            MenuBar = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.MenuBar)];
            MenuHighlight = _mappedSystemColors[GetSystemColorLookupIndex(KnownColor.MenuHighlight)];
        }

        /// <summary>
        ///  Returns the default system colors. This reflects the actual current system colors.
        /// </summary>
        internal static ControlSystemColors Default { get; } = new();

        /// <summary>
        ///  Returns the default system colors for dark mode. This is a hard-coded set of colors intended to use for the WinForms dark mode.
        /// </summary>
        internal static ControlSystemColors DefaultDarkMode { get; } = new(true);

        /// <summary>
        ///  Gets the current system colors depending on <see cref="Application.IsDarkModeEnabled"/>.
        /// </summary>
        public static ControlSystemColors Current =>
            Application.IsDarkModeEnabled ? DefaultDarkMode : Default;

        /// <summary>
        ///  Gets the active border color.
        /// </summary>
        public Color ActiveBorder { get; }

        /// <summary>
        ///  Gets the window color.
        /// </summary>
        public Color Window { get; }

        /// <summary>
        ///  Gets the scroll bar color.
        /// </summary>
        public Color ScrollBar { get; }

        /// <summary>
        ///  Gets the menu text color.
        /// </summary>
        public Color MenuText { get; }

        /// <summary>
        ///  Gets the menu highlight color.
        /// </summary>
        public Color MenuHighlight { get; }

        /// <summary>
        ///  Gets the menu bar color.
        /// </summary>
        public Color MenuBar { get; }

        /// <summary>
        ///  Gets the menu color.
        /// </summary>
        public Color Menu { get; }

        /// <summary>
        ///  Gets the info text color.
        /// </summary>
        public Color InfoText { get; }

        /// <summary>
        ///  Gets the info color.
        /// </summary>
        public Color Info { get; }

        /// <summary>
        ///  Gets the inactive caption text color.
        /// </summary>
        public Color InactiveCaptionText { get; }

        /// <summary>
        ///  Gets the inactive caption color.
        /// </summary>
        public Color InactiveCaption { get; }

        /// <summary>
        ///  Gets the inactive border color.
        /// </summary>
        public Color InactiveBorder { get; }

        /// <summary>
        ///  Gets the hot track color.
        /// </summary>
        public Color HotTrack { get; }

        /// <summary>
        ///  Gets the highlight text color.
        /// </summary>
        public Color HighlightText { get; }

        /// <summary>
        ///  Gets the highlight color.
        /// </summary>
        public Color Highlight { get; }

        /// <summary>
        ///  Gets the window frame color.
        /// </summary>
        public Color WindowFrame { get; }

        /// <summary>
        ///  Gets the window frame color.
        /// </summary>
        public Color WindowText { get; }

        /// <summary>
        ///  Gets the gray text color.
        /// </summary>
        public Color GrayText { get; }

        /// <summary>
        ///  Gets the gradient active caption color.
        /// </summary>
        public Color GradientActiveCaption { get; }

        /// <summary>
        ///  Gets the desktop color.
        /// </summary>
        public Color Desktop { get; }

        /// <summary>
        ///  Gets the control text color.
        /// </summary>
        public Color ControlText { get; }

        /// <summary>
        ///  Gets the control light light color.
        /// </summary>
        public Color ControlLightLight { get; }

        /// <summary>
        ///  Gets the control light color.
        /// </summary>
        public Color ControlLight { get; }

        /// <summary>
        ///  Gets the control dark dark color.
        /// </summary>
        public Color ControlDarkDark { get; }

        /// <summary>
        ///  Gets the control dark color.
        /// </summary>
        public Color ControlDark { get; }

        /// <summary>
        ///  Gets the control color.
        /// </summary>
        public Color Control { get; }

        /// <summary>
        ///  Gets the button shadow color.
        /// </summary>
        public Color ButtonShadow { get; }

        /// <summary>
        ///  Gets the button highlight color.
        /// </summary>
        public Color ButtonHighlight { get; }

        /// <summary>
        ///  Gets the button face color.
        /// </summary>
        public Color ButtonFace { get; }

        /// <summary>
        ///  Gets the application workspace color.
        /// </summary>
        public Color AppWorkspace { get; }

        /// <summary>
        ///  Gets the active caption text color.
        /// </summary>
        public Color ActiveCaptionText { get; }

        /// <summary>
        ///  Gets the active caption color.
        /// </summary>
        public Color ActiveCaption { get; }

        /// <summary>
        ///  Gets the gradient inactive caption color.
        /// </summary>
        public Color GradientInactiveCaption { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetSystemColorLookupIndex(KnownColor color)
        {
            int index;
            if (color >= KnownColor.YellowGreen)
            {
                index = (int)color - (int)KnownColor.YellowGreen + (int)KnownColor.WindowText;
            }
            else
            {
                index = (int)color;
            }

            index--;
            return index;
        }

        /// <summary>
        ///  Get the system color index for the specified known color.
        /// </summary>
        /// <param name="knownColor">The known color for the given system color.</param>
        /// <returns>The 0-based index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the known color does not reflect a system color.</exception>
        public static Color GetAdaptedDarkModeColorFromKnownColor(KnownColor knownColor, bool darkMode)
        {
            int index = GetSystemColorLookupIndex(knownColor);

            if (index < 0 || index >= s_systemColors.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(knownColor),
                    knownColor,
                    $"{knownColor} is not a System Color.");
            }

            return darkMode ? s_darkSystemColors[index] : s_systemColors[index];
        }
    }
}
