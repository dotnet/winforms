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
            unchecked(Color.FromArgb((int)0xFF464646)), // FFD4D0C8 - FF464646: ActiveBorder - Dark gray
            unchecked(Color.FromArgb((int)0xFF3C5F78)), // FF0054E3 - FF3C5F78: ActiveCaption - Highlighted Text Background
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // FFFFFFFF - FF000000: ActiveCaptionText - White
            unchecked(Color.FromArgb((int)0xFF3C3C3C)), // FF808080 - FF3C3C3C: AppWorkspace - Panel Background
            unchecked(Color.FromArgb((int)0xFF202020)), // FFECE9D8 - FF373737: Control - Normal Panel/Windows Background
            unchecked(Color.FromArgb((int)0xFF4A4A4A)), // FFA0A0A0 - FF464646: ControlDark - A lighter gray for dark mode
            unchecked(Color.FromArgb((int)0xFF5A5A5A)), // FF696969 - FF5A5A5A: ControlDarkDark - An even lighter gray for dark mode
            unchecked(Color.FromArgb((int)0xFF2E2E2E)), // FF716F64 - FF2E2E2E: ControlLight - Unfocused Textbox Background
            unchecked(Color.FromArgb((int)0xFF1F1F1F)), // FFFFFFFF - FF1F1F1F: ControlLightLight - Focused Textbox Background
            unchecked(Color.FromArgb((int)0xFFFFFFFF)), // FF000000 - FFFFFFFF: ControlText - Control Forecolor and Text Color
            unchecked(Color.FromArgb((int)0xFF101010)), // FF004E98 - FF101010: Desktop - Black
            unchecked(Color.FromArgb((int)0xFF969696)), // FFACA899 - FF969696: GrayText - Prompt Text Focused TextBox
            unchecked(Color.FromArgb((int)0xFF2864B4)), // FF316AC5 - FF2864B4: Highlight - Highlighted Panel in ColorMode
            unchecked(Color.FromArgb((int)0xFFF0F0F0)), // FFFFFFFF - FF000000: HighlightText - White
            unchecked(Color.FromArgb((int)0xFF2D5FAF)), // FF0066CC - FF2D5FAF: HotTrack - Background of the ToggleSwitch
            unchecked(Color.FromArgb((int)0xFF3C3F41)), // FFD4D0C8 - FF3C3F41: InactiveBorder - Dark gray
            unchecked(Color.FromArgb((int)0xFF374B5A)), // FF7A96DF - FF374B5A: InactiveCaption - Highlighted Panel in ColorMode
            unchecked(Color.FromArgb((int)0xFFBEBEBE)), // FFD8E4F8 - FFBEBEBE: InactiveCaptionText - Middle Dark Panel
            unchecked(Color.FromArgb((int)0xFF50503C)), // FFFFFFE1 - FF50503C: Info - Link Label
            unchecked(Color.FromArgb((int)0xFFBEBEBE)), // FF000000 - FFBEBEBE: InfoText - Prompt Text Color
            unchecked(Color.FromArgb((int)0xFF373737)), // FFFFFFFF - FF373737: Menu - Normal Menu Background
            unchecked(Color.FromArgb((int)0xFFF0F0F0)), // FF000000 - FFF0F0F0: MenuText - White
            unchecked(Color.FromArgb((int)0xFF505050)), // FFD4D0C8 - FF505050: ScrollBar - Scrollbars and Scrollbar Arrows
            unchecked(Color.FromArgb((int)0xFF323232)), // FFFFFFFF - FF323232: Window - Window Background
            unchecked(Color.FromArgb((int)0xFF282828)), // FF000000 - FF282828: WindowFrame - White/Accent color
            unchecked(Color.FromArgb((int)0xFFF0F0F0)), // FF000000 - FFF0F0F0: WindowText - White

            unchecked(Color.FromArgb((int)0xFF202020)), // FFF0F0F0 - FF373737: ButtonFace - Same as Window Background
            unchecked(Color.FromArgb((int)0xFF101010)), // FFFFFFFF - FF101010: ButtonHighlight - White
            unchecked(Color.FromArgb((int)0xFF464646)), // FFA0A0A0 - FF464646: ButtonShadow - Same as Scrollbar Elements
            unchecked(Color.FromArgb((int)0XFF416482)), // FFB9D1EA - FF416482: GradientActiveCaption - Same as Highlighted Text Background
            unchecked(Color.FromArgb((int)0xFF557396)), // FFD7E4F2 - FF557396: GradientInactiveCaption - Same as Highlighted Panel in SystemColorMode
            unchecked(Color.FromArgb((int)0xFF373737)), // FFF0F0F0 - FF373737: MenuBar - Same as Normal Menu Background
            unchecked(Color.FromArgb((int)0xFF2A80D2))  // FF3399FF - FF2A80D2: MenuHighlight - Same as Highlighted Menu Background
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
