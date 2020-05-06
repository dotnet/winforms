// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

namespace System.Windows.Forms
{
    public static class ProfessionalColors
    {
        [ThreadStatic]
        private static ProfessionalColorTable? t_professionalColorTable;

        [ThreadStatic]
        private static string? t_colorScheme;

        [ThreadStatic]
        private static object? t_colorFreshnessKey;

        internal static ProfessionalColorTable ColorTable => t_professionalColorTable ??= new ProfessionalColorTable();

        static ProfessionalColors()
        {
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
            SetScheme();
        }

        internal static string? ColorScheme => t_colorScheme;

        // internal object used between professional color tables
        // to identify when a userpreferencechanged has occurred
        internal static object? ColorFreshnessKey => t_colorFreshnessKey;

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightDescr))]
        public static Color ButtonSelectedHighlight => ColorTable.ButtonSelectedHighlight;

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightBorderDescr))]
        public static Color ButtonSelectedHighlightBorder => ColorTable.ButtonSelectedHighlightBorder;

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightDescr))]
        public static Color ButtonPressedHighlight => ColorTable.ButtonPressedHighlight;

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightBorderDescr))]
        public static Color ButtonPressedHighlightBorder => ColorTable.ButtonPressedHighlightBorder;

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightDescr))]
        public static Color ButtonCheckedHighlight => ColorTable.ButtonCheckedHighlight;

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightBorderDescr))]
        public static Color ButtonCheckedHighlightBorder => ColorTable.ButtonCheckedHighlightBorder;

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedBorderDescr))]
        public static Color ButtonPressedBorder => ColorTable.ButtonPressedBorder;

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedBorderDescr))]
        public static Color ButtonSelectedBorder => ColorTable.ButtonSelectedBorder;

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientBeginDescr))]
        public static Color ButtonCheckedGradientBegin => ColorTable.ButtonCheckedGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientMiddleDescr))]
        public static Color ButtonCheckedGradientMiddle => ColorTable.ButtonCheckedGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientEndDescr))]
        public static Color ButtonCheckedGradientEnd => ColorTable.ButtonCheckedGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientBeginDescr))]
        public static Color ButtonSelectedGradientBegin => ColorTable.ButtonSelectedGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientMiddleDescr))]
        public static Color ButtonSelectedGradientMiddle => ColorTable.ButtonSelectedGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientEndDescr))]
        public static Color ButtonSelectedGradientEnd => ColorTable.ButtonSelectedGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientBeginDescr))]
        public static Color ButtonPressedGradientBegin => ColorTable.ButtonPressedGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientMiddleDescr))]
        public static Color ButtonPressedGradientMiddle => ColorTable.ButtonPressedGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientEndDescr))]
        public static Color ButtonPressedGradientEnd => ColorTable.ButtonPressedGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsCheckBackgroundDescr))]
        public static Color CheckBackground => ColorTable.CheckBackground;

        [SRDescription(nameof(SR.ProfessionalColorsCheckSelectedBackgroundDescr))]
        public static Color CheckSelectedBackground => ColorTable.CheckSelectedBackground;

        [SRDescription(nameof(SR.ProfessionalColorsCheckPressedBackgroundDescr))]
        public static Color CheckPressedBackground => ColorTable.CheckPressedBackground;

        [SRDescription(nameof(SR.ProfessionalColorsGripDarkDescr))]
        public static Color GripDark => ColorTable.GripDark;

        [SRDescription(nameof(SR.ProfessionalColorsGripLightDescr))]
        public static Color GripLight => ColorTable.GripLight;

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientBeginDescr))]
        public static Color ImageMarginGradientBegin => ColorTable.ImageMarginGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientMiddleDescr))]
        public static Color ImageMarginGradientMiddle => ColorTable.ImageMarginGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientEndDescr))]
        public static Color ImageMarginGradientEnd => ColorTable.ImageMarginGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientBeginDescr))]
        public static Color ImageMarginRevealedGradientBegin => ColorTable.ImageMarginRevealedGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientMiddleDescr))]
        public static Color ImageMarginRevealedGradientMiddle => ColorTable.ImageMarginRevealedGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientEndDescr))]
        public static Color ImageMarginRevealedGradientEnd => ColorTable.ImageMarginRevealedGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientBeginDescr))]
        public static Color MenuStripGradientBegin => ColorTable.MenuStripGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientEndDescr))]
        public static Color MenuStripGradientEnd => ColorTable.MenuStripGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsMenuBorderDescr))]
        public static Color MenuBorder => ColorTable.MenuBorder;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedDescr))]
        public static Color MenuItemSelected => ColorTable.MenuItemSelected;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemBorderDescr))]
        public static Color MenuItemBorder => ColorTable.MenuItemBorder;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientBeginDescr))]
        public static Color MenuItemSelectedGradientBegin => ColorTable.MenuItemSelectedGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientEndDescr))]
        public static Color MenuItemSelectedGradientEnd => ColorTable.MenuItemSelectedGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientBeginDescr))]
        public static Color MenuItemPressedGradientBegin => ColorTable.MenuItemPressedGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientMiddleDescr))]
        public static Color MenuItemPressedGradientMiddle => ColorTable.MenuItemPressedGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientEndDescr))]
        public static Color MenuItemPressedGradientEnd => ColorTable.MenuItemPressedGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientBeginDescr))]
        public static Color RaftingContainerGradientBegin => ColorTable.RaftingContainerGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientEndDescr))]
        public static Color RaftingContainerGradientEnd => ColorTable.RaftingContainerGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsSeparatorDarkDescr))]
        public static Color SeparatorDark => ColorTable.SeparatorDark;

        [SRDescription(nameof(SR.ProfessionalColorsSeparatorLightDescr))]
        public static Color SeparatorLight => ColorTable.SeparatorLight;

        [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientBeginDescr))]
        public static Color StatusStripGradientBegin => ColorTable.StatusStripGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientEndDescr))]
        public static Color StatusStripGradientEnd => ColorTable.StatusStripGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripBorderDescr))]
        public static Color ToolStripBorder => ColorTable.ToolStripBorder;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripDropDownBackgroundDescr))]
        public static Color ToolStripDropDownBackground => ColorTable.ToolStripDropDownBackground;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientBeginDescr))]
        public static Color ToolStripGradientBegin => ColorTable.ToolStripGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientMiddleDescr))]
        public static Color ToolStripGradientMiddle => ColorTable.ToolStripGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientEndDescr))]
        public static Color ToolStripGradientEnd => ColorTable.ToolStripGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientBeginDescr))]
        public static Color ToolStripContentPanelGradientBegin => ColorTable.ToolStripContentPanelGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientEndDescr))]
        public static Color ToolStripContentPanelGradientEnd => ColorTable.ToolStripContentPanelGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientBeginDescr))]
        public static Color ToolStripPanelGradientBegin => ColorTable.ToolStripPanelGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientEndDescr))]
        public static Color ToolStripPanelGradientEnd => ColorTable.ToolStripPanelGradientEnd;

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientBeginDescr))]
        public static Color OverflowButtonGradientBegin => ColorTable.OverflowButtonGradientBegin;

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientMiddleDescr))]
        public static Color OverflowButtonGradientMiddle => ColorTable.OverflowButtonGradientMiddle;

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientEndDescr))]
        public static Color OverflowButtonGradientEnd => ColorTable.OverflowButtonGradientEnd;

        private static void OnUserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e)
        {
            SetScheme();
            if (e.Category == UserPreferenceCategory.Color)
            {
                t_colorFreshnessKey = new object();
            }
        }

        private static void SetScheme()
        {
            if (VisualStyleRenderer.IsSupported)
            {
                t_colorScheme = VisualStyleInformation.ColorScheme;
            }
            else
            {
                t_colorScheme = null;
            }
        }
    }
}
