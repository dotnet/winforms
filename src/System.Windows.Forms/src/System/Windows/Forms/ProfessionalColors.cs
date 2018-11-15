// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using Microsoft.Win32;
    using System.Drawing;
    using System.Collections;
    using System.Diagnostics;
    
    public sealed class ProfessionalColors {
        [ThreadStatic]
        private static ProfessionalColorTable professionalColorTable = null;

        [ThreadStatic]
        private static string colorScheme = null;

        [ThreadStatic]
        private static object colorFreshnessKey = null;

            
        internal static ProfessionalColorTable ColorTable {
            get {
                if (professionalColorTable == null) {
                    professionalColorTable = new ProfessionalColorTable();
                }
                return professionalColorTable;
            }
        }

        static ProfessionalColors() {
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
            SetScheme();
        }

        private ProfessionalColors() {
        }
        
        internal static string ColorScheme {
            get { return colorScheme; }
        }

        // internal object used between professional color tables
        // to identify when a userpreferencechanged has occurred
        internal static object ColorFreshnessKey {
            get { return colorFreshnessKey; }
        }

#region Colors

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightDescr))]
        public static Color ButtonSelectedHighlight {
            get { return ColorTable.ButtonSelectedHighlight; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightBorderDescr))]
        public static Color ButtonSelectedHighlightBorder {
            get { return ColorTable.ButtonSelectedHighlightBorder; }
        }


        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightDescr))]
        public static Color ButtonPressedHighlight {
             get { return ColorTable.ButtonPressedHighlight; }
        }


        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightBorderDescr))]
        public static Color ButtonPressedHighlightBorder {
           get { return ColorTable.ButtonPressedHighlightBorder; }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightDescr))]
        public static Color ButtonCheckedHighlight {
            get { return ColorTable.ButtonCheckedHighlight; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightBorderDescr))]
        public static Color ButtonCheckedHighlightBorder {
            get { return ColorTable.ButtonCheckedHighlightBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedBorderDescr))]
        public static Color ButtonPressedBorder {
            get { return ColorTable.ButtonPressedBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedBorderDescr))]
        public static Color ButtonSelectedBorder {
            get { return ColorTable.ButtonSelectedBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientBeginDescr))]
        public static Color ButtonCheckedGradientBegin {
            get { return ColorTable.ButtonCheckedGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientMiddleDescr))]
        public static Color ButtonCheckedGradientMiddle {
            get { return ColorTable.ButtonCheckedGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientEndDescr))]
        public static Color ButtonCheckedGradientEnd {
            get { return ColorTable.ButtonCheckedGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientBeginDescr))]
        public static Color ButtonSelectedGradientBegin {
            get { return ColorTable.ButtonSelectedGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientMiddleDescr))]
        public static Color ButtonSelectedGradientMiddle {
            get { return ColorTable.ButtonSelectedGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientEndDescr))]
        public static Color ButtonSelectedGradientEnd {
            get { return ColorTable.ButtonSelectedGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientBeginDescr))]
        public static Color ButtonPressedGradientBegin {
            get { return ColorTable.ButtonPressedGradientBegin; }
        }
     
        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientMiddleDescr))]
        public static Color ButtonPressedGradientMiddle {
            get { return ColorTable.ButtonPressedGradientMiddle; }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientEndDescr))]
        public static Color ButtonPressedGradientEnd {
            get { return ColorTable.ButtonPressedGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsCheckBackgroundDescr))]
        public static Color CheckBackground {
            get { return ColorTable.CheckBackground; }
        }
            
        [SRDescription(nameof(SR.ProfessionalColorsCheckSelectedBackgroundDescr))]
        public static Color CheckSelectedBackground {
            get { return ColorTable.CheckSelectedBackground; }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsCheckPressedBackgroundDescr))]
        public static Color CheckPressedBackground {
            get { return ColorTable.CheckPressedBackground; }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsGripDarkDescr))]
        public static Color GripDark {
            get { return ColorTable.GripDark; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsGripLightDescr))]
        public static Color GripLight {
            get { return ColorTable.GripLight; }
        }

          
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
 

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientBeginDescr))]
        public static Color ImageMarginGradientBegin {
            get { return ColorTable.ImageMarginGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientMiddleDescr))]
        public static Color ImageMarginGradientMiddle {
            get { return ColorTable.ImageMarginGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientEndDescr))]
        public static Color ImageMarginGradientEnd {
            get { return ColorTable.ImageMarginGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientBeginDescr))]
        public static Color ImageMarginRevealedGradientBegin {
            get { return ColorTable.ImageMarginRevealedGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientMiddleDescr))]
        public static Color ImageMarginRevealedGradientMiddle {
            get { return ColorTable.ImageMarginRevealedGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientEndDescr))]
        public static Color ImageMarginRevealedGradientEnd {
            get { return ColorTable.ImageMarginRevealedGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientBeginDescr))]
        public static Color MenuStripGradientBegin {
            get { return ColorTable.MenuStripGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientEndDescr))]
        public static Color MenuStripGradientEnd{
            get { return ColorTable.MenuStripGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuBorderDescr))]
        public static Color MenuBorder  {
            get { return ColorTable.MenuBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedDescr))]
        public static Color MenuItemSelected {
            get { return ColorTable.MenuItemSelected; }  
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemBorderDescr))]
        public static Color MenuItemBorder {
            get { return ColorTable.MenuItemBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientBeginDescr))]
        public static Color MenuItemSelectedGradientBegin {
            get { return ColorTable.MenuItemSelectedGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientEndDescr))]
        public static Color MenuItemSelectedGradientEnd {
            get { return ColorTable.MenuItemSelectedGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientBeginDescr))]
        public static Color MenuItemPressedGradientBegin {
            get { return ColorTable.MenuItemPressedGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientMiddleDescr))]
        public static Color MenuItemPressedGradientMiddle {
            get { return ColorTable.MenuItemPressedGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientEndDescr))]
        public static Color MenuItemPressedGradientEnd {
            get { return ColorTable.MenuItemPressedGradientEnd; }
        }

   
        [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientBeginDescr))]
        public static Color RaftingContainerGradientBegin {
            get { return ColorTable.RaftingContainerGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientEndDescr))]
        public static Color RaftingContainerGradientEnd {
            get { return ColorTable.RaftingContainerGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsSeparatorDarkDescr))]
        public static Color SeparatorDark {
            get { return ColorTable.SeparatorDark; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsSeparatorLightDescr))]
        public static Color SeparatorLight {
            get { return ColorTable.SeparatorLight; }
        }
        [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientBeginDescr))]
        public static Color StatusStripGradientBegin {
            get { return ColorTable.StatusStripGradientBegin; }
        }
   
        [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientEndDescr))]
        public static Color StatusStripGradientEnd {
            get { return ColorTable.StatusStripGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripBorderDescr))]
        public static Color ToolStripBorder {
            get { return ColorTable.ToolStripBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripDropDownBackgroundDescr))]
        public static Color ToolStripDropDownBackground {
            get { return ColorTable.ToolStripDropDownBackground; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientBeginDescr))]
        public static Color ToolStripGradientBegin {
            get { return ColorTable.ToolStripGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientMiddleDescr))]
        public static Color ToolStripGradientMiddle {
            get { return ColorTable.ToolStripGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientEndDescr))]
        public static Color ToolStripGradientEnd {
            get { return ColorTable.ToolStripGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientBeginDescr))]
        public static Color ToolStripContentPanelGradientBegin {
            get { return ColorTable.ToolStripContentPanelGradientBegin; }
        }
   
        [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientEndDescr))]
        public static Color ToolStripContentPanelGradientEnd {
            get { return ColorTable.ToolStripContentPanelGradientEnd; }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientBeginDescr))]
        public static Color ToolStripPanelGradientBegin {
            get { return ColorTable.ToolStripPanelGradientBegin; }
        }
  
        [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientEndDescr))]
        public static Color ToolStripPanelGradientEnd {
            get { return ColorTable.ToolStripPanelGradientEnd; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientBeginDescr))]
        public static Color OverflowButtonGradientBegin {
            get { return ColorTable.OverflowButtonGradientBegin; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientMiddleDescr))]
        public static Color OverflowButtonGradientMiddle {
            get { return ColorTable.OverflowButtonGradientMiddle; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientEndDescr))]
        public static Color OverflowButtonGradientEnd {
            get { return ColorTable.OverflowButtonGradientEnd; }
        }
#endregion Colors

      /*  public static Color ControlLight {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBkgdLight); }
        } */
            
 
        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) {
            SetScheme();
            if (e.Category == UserPreferenceCategory.Color) {
                colorFreshnessKey = new object();
            }
        }

        private static void SetScheme() {
            if (VisualStyleRenderer.IsSupported) {
                colorScheme = VisualStyleInformation.ColorScheme;
            }
            else {
                colorScheme = null;
            }
        }

    }

    
}

