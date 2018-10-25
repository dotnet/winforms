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

        [SRDescription(SR.ProfessionalColorsButtonSelectedHighlightDescr)]
        public static Color ButtonSelectedHighlight {
            get { return ColorTable.ButtonSelectedHighlight; }
        }

        [SRDescription(SR.ProfessionalColorsButtonSelectedHighlightBorderDescr)]
        public static Color ButtonSelectedHighlightBorder {
            get { return ColorTable.ButtonSelectedHighlightBorder; }
        }


        [SRDescription(SR.ProfessionalColorsButtonPressedHighlightDescr)]
        public static Color ButtonPressedHighlight {
             get { return ColorTable.ButtonPressedHighlight; }
        }


        [SRDescription(SR.ProfessionalColorsButtonPressedHighlightBorderDescr)]
        public static Color ButtonPressedHighlightBorder {
           get { return ColorTable.ButtonPressedHighlightBorder; }
        }
        
        [SRDescription(SR.ProfessionalColorsButtonCheckedHighlightDescr)]
        public static Color ButtonCheckedHighlight {
            get { return ColorTable.ButtonCheckedHighlight; }
        }

        [SRDescription(SR.ProfessionalColorsButtonCheckedHighlightBorderDescr)]
        public static Color ButtonCheckedHighlightBorder {
            get { return ColorTable.ButtonCheckedHighlightBorder; }
        }

        [SRDescription(SR.ProfessionalColorsButtonPressedBorderDescr)]
        public static Color ButtonPressedBorder {
            get { return ColorTable.ButtonPressedBorder; }
        }

        [SRDescription(SR.ProfessionalColorsButtonSelectedBorderDescr)]
        public static Color ButtonSelectedBorder {
            get { return ColorTable.ButtonSelectedBorder; }
        }

        [SRDescription(SR.ProfessionalColorsButtonCheckedGradientBeginDescr)]
        public static Color ButtonCheckedGradientBegin {
            get { return ColorTable.ButtonCheckedGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsButtonCheckedGradientMiddleDescr)]
        public static Color ButtonCheckedGradientMiddle {
            get { return ColorTable.ButtonCheckedGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsButtonCheckedGradientEndDescr)]
        public static Color ButtonCheckedGradientEnd {
            get { return ColorTable.ButtonCheckedGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsButtonSelectedGradientBeginDescr)]
        public static Color ButtonSelectedGradientBegin {
            get { return ColorTable.ButtonSelectedGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsButtonSelectedGradientMiddleDescr)]
        public static Color ButtonSelectedGradientMiddle {
            get { return ColorTable.ButtonSelectedGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsButtonSelectedGradientEndDescr)]
        public static Color ButtonSelectedGradientEnd {
            get { return ColorTable.ButtonSelectedGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsButtonPressedGradientBeginDescr)]
        public static Color ButtonPressedGradientBegin {
            get { return ColorTable.ButtonPressedGradientBegin; }
        }
     
        [SRDescription(SR.ProfessionalColorsButtonPressedGradientMiddleDescr)]
        public static Color ButtonPressedGradientMiddle {
            get { return ColorTable.ButtonPressedGradientMiddle; }
        }
        
        [SRDescription(SR.ProfessionalColorsButtonPressedGradientEndDescr)]
        public static Color ButtonPressedGradientEnd {
            get { return ColorTable.ButtonPressedGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsCheckBackgroundDescr)]
        public static Color CheckBackground {
            get { return ColorTable.CheckBackground; }
        }
            
        [SRDescription(SR.ProfessionalColorsCheckSelectedBackgroundDescr)]
        public static Color CheckSelectedBackground {
            get { return ColorTable.CheckSelectedBackground; }
        }
        
        [SRDescription(SR.ProfessionalColorsCheckPressedBackgroundDescr)]
        public static Color CheckPressedBackground {
            get { return ColorTable.CheckPressedBackground; }
        }
        
        [SRDescription(SR.ProfessionalColorsGripDarkDescr)]
        public static Color GripDark {
            get { return ColorTable.GripDark; }
        }

        [SRDescription(SR.ProfessionalColorsGripLightDescr)]
        public static Color GripLight {
            get { return ColorTable.GripLight; }
        }

          
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
 

        [SRDescription(SR.ProfessionalColorsImageMarginGradientBeginDescr)]
        public static Color ImageMarginGradientBegin {
            get { return ColorTable.ImageMarginGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsImageMarginGradientMiddleDescr)]
        public static Color ImageMarginGradientMiddle {
            get { return ColorTable.ImageMarginGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsImageMarginGradientEndDescr)]
        public static Color ImageMarginGradientEnd {
            get { return ColorTable.ImageMarginGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsImageMarginRevealedGradientBeginDescr)]
        public static Color ImageMarginRevealedGradientBegin {
            get { return ColorTable.ImageMarginRevealedGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsImageMarginRevealedGradientMiddleDescr)]
        public static Color ImageMarginRevealedGradientMiddle {
            get { return ColorTable.ImageMarginRevealedGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsImageMarginRevealedGradientEndDescr)]
        public static Color ImageMarginRevealedGradientEnd {
            get { return ColorTable.ImageMarginRevealedGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsMenuStripGradientBeginDescr)]
        public static Color MenuStripGradientBegin {
            get { return ColorTable.MenuStripGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsMenuStripGradientEndDescr)]
        public static Color MenuStripGradientEnd{
            get { return ColorTable.MenuStripGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsMenuBorderDescr)]
        public static Color MenuBorder  {
            get { return ColorTable.MenuBorder; }
        }

        [SRDescription(SR.ProfessionalColorsMenuItemSelectedDescr)]
        public static Color MenuItemSelected {
            get { return ColorTable.MenuItemSelected; }  
        }

        [SRDescription(SR.ProfessionalColorsMenuItemBorderDescr)]
        public static Color MenuItemBorder {
            get { return ColorTable.MenuItemBorder; }
        }

        [SRDescription(SR.ProfessionalColorsMenuItemSelectedGradientBeginDescr)]
        public static Color MenuItemSelectedGradientBegin {
            get { return ColorTable.MenuItemSelectedGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsMenuItemSelectedGradientEndDescr)]
        public static Color MenuItemSelectedGradientEnd {
            get { return ColorTable.MenuItemSelectedGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsMenuItemPressedGradientBeginDescr)]
        public static Color MenuItemPressedGradientBegin {
            get { return ColorTable.MenuItemPressedGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsMenuItemPressedGradientMiddleDescr)]
        public static Color MenuItemPressedGradientMiddle {
            get { return ColorTable.MenuItemPressedGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsMenuItemPressedGradientEndDescr)]
        public static Color MenuItemPressedGradientEnd {
            get { return ColorTable.MenuItemPressedGradientEnd; }
        }

   
        [SRDescription(SR.ProfessionalColorsRaftingContainerGradientBeginDescr)]
        public static Color RaftingContainerGradientBegin {
            get { return ColorTable.RaftingContainerGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsRaftingContainerGradientEndDescr)]
        public static Color RaftingContainerGradientEnd {
            get { return ColorTable.RaftingContainerGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsSeparatorDarkDescr)]
        public static Color SeparatorDark {
            get { return ColorTable.SeparatorDark; }
        }

        [SRDescription(SR.ProfessionalColorsSeparatorLightDescr)]
        public static Color SeparatorLight {
            get { return ColorTable.SeparatorLight; }
        }
        [SRDescription(SR.ProfessionalColorsStatusStripGradientBeginDescr)]
        public static Color StatusStripGradientBegin {
            get { return ColorTable.StatusStripGradientBegin; }
        }
   
        [SRDescription(SR.ProfessionalColorsStatusStripGradientEndDescr)]
        public static Color StatusStripGradientEnd {
            get { return ColorTable.StatusStripGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsToolStripBorderDescr)]
        public static Color ToolStripBorder {
            get { return ColorTable.ToolStripBorder; }
        }

        [SRDescription(SR.ProfessionalColorsToolStripDropDownBackgroundDescr)]
        public static Color ToolStripDropDownBackground {
            get { return ColorTable.ToolStripDropDownBackground; }
        }

        [SRDescription(SR.ProfessionalColorsToolStripGradientBeginDescr)]
        public static Color ToolStripGradientBegin {
            get { return ColorTable.ToolStripGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsToolStripGradientMiddleDescr)]
        public static Color ToolStripGradientMiddle {
            get { return ColorTable.ToolStripGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsToolStripGradientEndDescr)]
        public static Color ToolStripGradientEnd {
            get { return ColorTable.ToolStripGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsToolStripContentPanelGradientBeginDescr)]
        public static Color ToolStripContentPanelGradientBegin {
            get { return ColorTable.ToolStripContentPanelGradientBegin; }
        }
   
        [SRDescription(SR.ProfessionalColorsToolStripContentPanelGradientEndDescr)]
        public static Color ToolStripContentPanelGradientEnd {
            get { return ColorTable.ToolStripContentPanelGradientEnd; }
        }
        
        [SRDescription(SR.ProfessionalColorsToolStripPanelGradientBeginDescr)]
        public static Color ToolStripPanelGradientBegin {
            get { return ColorTable.ToolStripPanelGradientBegin; }
        }
  
        [SRDescription(SR.ProfessionalColorsToolStripPanelGradientEndDescr)]
        public static Color ToolStripPanelGradientEnd {
            get { return ColorTable.ToolStripPanelGradientEnd; }
        }

        [SRDescription(SR.ProfessionalColorsOverflowButtonGradientBeginDescr)]
        public static Color OverflowButtonGradientBegin {
            get { return ColorTable.OverflowButtonGradientBegin; }
        }

        [SRDescription(SR.ProfessionalColorsOverflowButtonGradientMiddleDescr)]
        public static Color OverflowButtonGradientMiddle {
            get { return ColorTable.OverflowButtonGradientMiddle; }
        }

        [SRDescription(SR.ProfessionalColorsOverflowButtonGradientEndDescr)]
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

