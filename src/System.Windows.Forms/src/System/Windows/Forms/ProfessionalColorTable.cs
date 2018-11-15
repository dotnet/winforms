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
    using System.Collections.Generic;
    using System.Diagnostics;
    
    public class ProfessionalColorTable {

        private Dictionary<KnownColors,Color> professionalRGB = null;
        private bool usingSystemColors  = false;
        private bool useSystemColors    = false;
        private string lastKnownColorScheme = String.Empty;


        private const string oliveColorScheme      =  "HomeStead";
        private const string normalColorScheme     =  "NormalColor";
        private const string silverColorScheme     =  "Metallic";
        private const string royaleColorScheme     =  "Royale";  // sometimes returns NormalColor, sometimes returns Royale.

        private const string lunaFileName =  "luna.msstyles";
        private const string royaleFileName = "royale.msstyles";
        private const string aeroFileName = "aero.msstyles";
         

        private object colorFreshnessKey = null;
        
        public ProfessionalColorTable() {
        }

        private Dictionary<KnownColors,Color> ColorTable {
            get {
           		if (UseSystemColors) {
                    // someone has turned off theme support for the color table.
                    if (!usingSystemColors || professionalRGB == null) {
                        if (professionalRGB == null) {
                           professionalRGB= new Dictionary<KnownColors,Color>((int)KnownColors.lastKnownColor);
                        }
                        InitSystemColors(ref professionalRGB);
                    }
                }
				else if (ToolStripManager.VisualStylesEnabled) {
                    // themes are on and enabled in the manager
                    if (usingSystemColors || professionalRGB == null) {
                        if (professionalRGB == null) {
                           professionalRGB= new Dictionary<KnownColors,Color>((int)KnownColors.lastKnownColor);
                        }
                        InitThemedColors(ref professionalRGB);
                    }
                }
                else {
                    // themes are off.
                    if (!usingSystemColors || professionalRGB == null) {
                        if (professionalRGB == null) {
                           professionalRGB= new Dictionary<KnownColors,Color>((int)KnownColors.lastKnownColor);
                        }
                        InitSystemColors(ref professionalRGB);
                    }
                }
                return professionalRGB;
            }
        }


        /// <devdoc> when this is specified, professional colors picks from SystemColors rather than colors
        ///          that match the current theme.  If theming is not turned on, we'll fall back to SystemColors.
        /// </devdoc>
        public bool UseSystemColors {
            get {
                return useSystemColors;
            }
            set{
                if (useSystemColors != value) {
                    useSystemColors = value;
                    ResetRGBTable();
                }
            }
        }
      
        internal Color FromKnownColor(ProfessionalColorTable.KnownColors color) {
            if (ProfessionalColors.ColorFreshnessKey != colorFreshnessKey  || ProfessionalColors.ColorScheme  != lastKnownColorScheme) {
                ResetRGBTable();               
            }
            colorFreshnessKey = ProfessionalColors.ColorFreshnessKey;
            lastKnownColorScheme = ProfessionalColors.ColorScheme;

            return (Color)ColorTable[color];
        }



        private void ResetRGBTable() {
            if (professionalRGB != null) {
               professionalRGB.Clear();
            }
            professionalRGB = null;           
        }

       
#region Colors

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightDescr))]
        public virtual Color ButtonSelectedHighlight {
            get { return FromKnownColor(KnownColors.ButtonSelectedHighlight); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightBorderDescr))]
        public virtual Color ButtonSelectedHighlightBorder {
            get { return ButtonPressedBorder; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightDescr))]
        public virtual Color ButtonPressedHighlight {
            get { return FromKnownColor(KnownColors.ButtonPressedHighlight); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightBorderDescr))]
        public virtual Color ButtonPressedHighlightBorder {
           get { return SystemColors.Highlight; }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightDescr))]
        public virtual Color ButtonCheckedHighlight {
            get { return FromKnownColor(KnownColors.ButtonCheckedHighlight); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightBorderDescr))]
        public virtual Color ButtonCheckedHighlightBorder {
            get { return SystemColors.Highlight; }
        }

       [SRDescription(nameof(SR.ProfessionalColorsButtonPressedBorderDescr))]     
        public virtual Color ButtonPressedBorder {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBdrMouseOver); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedBorderDescr))]
        public virtual Color ButtonSelectedBorder {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBdrMouseOver); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientBeginDescr))]
        public virtual Color ButtonCheckedGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradSelectedBegin); }
        }  
        
        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientMiddleDescr))]
        public virtual Color ButtonCheckedGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradSelectedMiddle); }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientEndDescr))]
        public virtual Color ButtonCheckedGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradSelectedEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientBeginDescr))]
        public virtual Color ButtonSelectedGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseOverBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientMiddleDescr))]
        public virtual Color ButtonSelectedGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseOverMiddle); }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientEndDescr))]
        public virtual Color ButtonSelectedGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseOverEnd); }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientBeginDescr))]
        public virtual Color ButtonPressedGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseDownBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientMiddleDescr))]
        public virtual Color ButtonPressedGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseDownMiddle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientEndDescr))]
        public virtual Color ButtonPressedGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseDownEnd); }
        }
        [SRDescription(nameof(SR.ProfessionalColorsCheckBackgroundDescr))]
        public virtual Color CheckBackground {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelected); }
        }
        
        [SRDescription(nameof(SR.ProfessionalColorsCheckSelectedBackgroundDescr))]        
        public virtual Color CheckSelectedBackground {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsCheckPressedBackgroundDescr))]
        public virtual Color CheckPressedBackground {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsGripDarkDescr))]
        public virtual Color GripDark {
            get { return FromKnownColor(KnownColors.msocbvcrCBDragHandle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsGripLightDescr))]
        public virtual Color GripLight {
            get { return FromKnownColor(KnownColors.msocbvcrCBDragHandleShadow); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientBeginDescr))]
        public virtual Color ImageMarginGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradVertBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientMiddleDescr))]
        public virtual Color ImageMarginGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradVertMiddle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientEndDescr))]
        public virtual Color ImageMarginGradientEnd {
            get { return (usingSystemColors) ? SystemColors.Control : FromKnownColor(KnownColors.msocbvcrCBGradVertEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientBeginDescr))]       
        public virtual Color ImageMarginRevealedGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientMiddleDescr))]
        public virtual Color ImageMarginRevealedGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientEndDescr))]
        public virtual Color ImageMarginRevealedGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientBeginDescr))]
        public virtual Color MenuStripGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientEndDescr))]
        public virtual Color MenuStripGradientEnd{
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedDescr))]
        public virtual Color MenuItemSelected {
           get { return FromKnownColor(KnownColors.msocbvcrCBCtlBkgdMouseOver); }  
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemBorderDescr))]
        public virtual Color MenuItemBorder {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBdrSelected); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuBorderDescr))]
        public virtual Color MenuBorder  {
            get { return FromKnownColor(KnownColors.msocbvcrCBMenuBdrOuter); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientBeginDescr))]
        public virtual Color MenuItemSelectedGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseOverBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientEndDescr))]
        public virtual Color MenuItemSelectedGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMouseOverEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientBeginDescr))]
        public virtual Color MenuItemPressedGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMenuTitleBkgdBegin); }
        }


        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientMiddleDescr))]
        public virtual Color MenuItemPressedGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientEndDescr))]
        public virtual Color MenuItemPressedGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMenuTitleBkgdEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientBeginDescr))]
        public virtual Color RaftingContainerGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientEndDescr))]
        public virtual Color RaftingContainerGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsSeparatorDarkDescr))]
        public virtual Color SeparatorDark {
            get { return FromKnownColor(KnownColors.msocbvcrCBSplitterLine); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsSeparatorLightDescr))]
        public virtual Color SeparatorLight {
            get { return FromKnownColor(KnownColors.msocbvcrCBSplitterLineLight); }
        }
      
        [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientBeginDescr))]
        public virtual Color StatusStripGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); }
        }
   
        [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientEndDescr))]
        public virtual Color StatusStripGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripBorderDescr))]
        public virtual Color ToolStripBorder {
            get { return FromKnownColor(KnownColors.msocbvcrCBShadow); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripDropDownBackgroundDescr))]
        public virtual Color ToolStripDropDownBackground {
            get { return FromKnownColor(KnownColors.msocbvcrCBMenuBkgd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientBeginDescr))]
        public virtual Color ToolStripGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradVertBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientMiddleDescr))]
        public virtual Color ToolStripGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradVertMiddle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientEndDescr))]
        public virtual Color ToolStripGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradVertEnd); }
        }
   
        [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientBeginDescr))]
        public virtual Color ToolStripContentPanelGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientEndDescr))]
        public virtual Color ToolStripContentPanelGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientBeginDescr))]
        public virtual Color ToolStripPanelGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientEndDescr))]
        public virtual Color ToolStripPanelGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientBeginDescr))]
        public virtual Color OverflowButtonGradientBegin {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradOptionsBegin); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientMiddleDescr))]  
        public virtual Color OverflowButtonGradientMiddle {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradOptionsMiddle); }
        }

        [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientEndDescr))]
        public virtual Color OverflowButtonGradientEnd {
            get { return FromKnownColor(KnownColors.msocbvcrCBGradOptionsEnd); }
        }
#endregion Colors
      

#region NotDirectlyExposed
         // 



         internal Color ComboBoxButtonGradientBegin {
              get { return MenuItemPressedGradientBegin; }
          }
          internal Color ComboBoxButtonGradientEnd {
              get { return MenuItemPressedGradientEnd; }
          }
          internal Color ComboBoxButtonSelectedGradientBegin {
              get { return MenuItemSelectedGradientBegin; }
          }
          internal Color ComboBoxButtonSelectedGradientEnd {
              get { return MenuItemSelectedGradientEnd;}
          }
          internal Color ComboBoxButtonPressedGradientBegin {
              get { return ButtonPressedGradientBegin; }
          }
          internal Color ComboBoxButtonPressedGradientEnd {
              get { return ButtonPressedGradientEnd; }
          }
          internal Color ComboBoxButtonOnOverflow {
              get { return ToolStripDropDownBackground; }
          }
          internal Color ComboBoxBorder {
              get { return ButtonSelectedHighlightBorder; }
          }
          internal Color TextBoxBorder {
               get { return ButtonSelectedHighlightBorder; }
          }
#endregion


      /*  public virtual Color ControlLight {
            get { return FromKnownColor(KnownColors.msocbvcrCBCtlBkgdLight); }
        } */
            
        private static  Color GetAlphaBlendedColor(Graphics g, Color src, Color dest, int alpha) {
            int red = (src.R * alpha + (255 - alpha) * dest.R) / 255;
            int green = (src.G * alpha + (255 - alpha) * dest.G) / 255;
            int blue = (src.B * alpha + (255 - alpha) * dest.B) / 255;
            int newAlpha = (src.A * alpha + (255 - alpha) * dest.A) / 255;
            if (g == null) {
                return Color.FromArgb(newAlpha, red, green, blue);
            }
            else {
                return g.GetNearestColor(Color.FromArgb(newAlpha, red, green, blue));
            }
        }


        // this particular method gets us closer to office by increasing the resolution...  

        private static Color GetAlphaBlendedColorHighRes(Graphics graphics, Color src, Color dest, int alpha) {

            int sum;
            int nPart2;
            int r, g, b;

            int nPart1 = alpha;

            if (nPart1 < 100) {
                nPart2 = 100 - nPart1;
                sum = 100;
            }
            else {
                nPart2 = 1000 - nPart1;
                sum = 1000;
            }

            // By adding on sum/2 before dividing by sum, we properly round the value,
            // rather than truncating it, while doing integer math.

            r = (nPart1 * src.R + nPart2 * dest.R + sum / 2) / sum;
            g = (nPart1 * src.G + nPart2 * dest.G + sum / 2) / sum;
            b = (nPart1 * src.B + nPart2 * dest.B + sum / 2) / sum;

            if (graphics == null) {
                return Color.FromArgb(r, g, b);
            }
            else {
                return graphics.GetNearestColor(Color.FromArgb(r, g, b));
            }

        }

        private void InitCommonColors(ref Dictionary<KnownColors,Color> rgbTable) {
            /// we need to calculate our own alpha blended color based on the Higlight and Window
            /// colors on the system.  Since terminalserver + alphablending doesnt work we cant just do a 
            /// FromARGB here.  So we have a simple function which calculates the blending for us.

            if (!DisplayInformation.LowResolution) {
                using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics()) {
                    rgbTable[ProfessionalColorTable.KnownColors.ButtonPressedHighlight] = GetAlphaBlendedColor(g, SystemColors.Window, GetAlphaBlendedColor(g, SystemColors.Highlight, SystemColors.Window, 160), 50);
                    rgbTable[ProfessionalColorTable.KnownColors.ButtonCheckedHighlight] = GetAlphaBlendedColor(g, SystemColors.Window, GetAlphaBlendedColor(g, SystemColors.Highlight, SystemColors.Window, 80), 20);
                    rgbTable[ProfessionalColorTable.KnownColors.ButtonSelectedHighlight] = rgbTable[ProfessionalColorTable.KnownColors.ButtonCheckedHighlight];
                }
            }
            else {
                rgbTable[ProfessionalColorTable.KnownColors.ButtonPressedHighlight] = SystemColors.Highlight;
                rgbTable[ProfessionalColorTable.KnownColors.ButtonCheckedHighlight] = SystemColors.ControlLight;
                rgbTable[ProfessionalColorTable.KnownColors.ButtonSelectedHighlight] = SystemColors.ControlLight;
            }

        }
        internal void InitSystemColors(ref Dictionary<KnownColors,Color> rgbTable) {
            usingSystemColors = true;

            InitCommonColors(ref rgbTable);


            // use locals so we arent fetching again and again.
            Color buttonFace = SystemColors.ButtonFace;
            Color buttonShadow = SystemColors.ButtonShadow;
            Color highlight = SystemColors.Highlight;
            Color window   = SystemColors.Window;
            Color empty = Color.Empty;
            Color controlText = SystemColors.ControlText;
            Color buttonHighlight = SystemColors.ButtonHighlight;
            Color grayText =  SystemColors.GrayText;
            Color highlightText =  SystemColors.HighlightText;
            Color windowText = SystemColors.WindowText;

            // initialize to high contrast
            Color gradientBegin  = buttonFace;
            Color gradientMiddle = buttonFace;
            Color gradientEnd    = buttonFace;
            Color msocbvcrCBCtlBkgdMouseOver = highlight;
            Color msocbvcrCBCtlBkgdMouseDown = highlight;

            bool lowResolution = DisplayInformation.LowResolution;
            bool highContrast = DisplayInformation.HighContrast;
            
            if (lowResolution) {
                msocbvcrCBCtlBkgdMouseOver = window;
            }
            else if (!highContrast) {
                gradientBegin   = GetAlphaBlendedColorHighRes(null, buttonFace, window, 23);
                gradientMiddle  = GetAlphaBlendedColorHighRes(null, buttonFace, window, 50 );
                gradientEnd     = SystemColors.ButtonFace;

                
                msocbvcrCBCtlBkgdMouseOver  = GetAlphaBlendedColorHighRes(null, highlight, window, 30);
                msocbvcrCBCtlBkgdMouseDown = GetAlphaBlendedColorHighRes(null, highlight, window, 50);
  
	           
            }
            
 

            if (lowResolution || highContrast) {
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd]                     =  buttonFace;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] =  SystemColors.ControlLight;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle]               =  controlText;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd]      =  buttonFace;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin]                 =  buttonShadow;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle]                =  buttonShadow;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin]     =  buttonShadow;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle]    =  buttonShadow;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd]       =  buttonShadow;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter]                     =  controlText;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd]                         =  window;
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine]                     =  buttonShadow; 
            }
            else {
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd]                     =  GetAlphaBlendedColorHighRes(null, window, buttonFace, 165);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] =  GetAlphaBlendedColorHighRes(null, highlight, window, 50);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle]               =  GetAlphaBlendedColorHighRes(null, buttonShadow, window, 75);            
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd]      =  GetAlphaBlendedColorHighRes(null, buttonFace, window, 205);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin]         =  GetAlphaBlendedColorHighRes(null, buttonFace, window, 70);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle]        =  GetAlphaBlendedColorHighRes(null, buttonFace, window, 90);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin]     = GetAlphaBlendedColorHighRes(null, buttonFace, window, 40);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle]    = GetAlphaBlendedColorHighRes(null, buttonFace, window, 70);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd]       = GetAlphaBlendedColorHighRes(null, buttonFace, window, 90);
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter]                     = GetAlphaBlendedColorHighRes(null, controlText, buttonShadow, 20); 
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd]                         = GetAlphaBlendedColorHighRes(null, buttonFace, window, 143); 
                rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine]                     = GetAlphaBlendedColorHighRes(null, buttonShadow, window, 70);

            }
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelected]          = (lowResolution) ? SystemColors.ControlLight : highlight;


		
			rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked]           = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked]           = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterFloating]         = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseDown]          = highlight;

            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver]          = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelected]           = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelectedMouseOver]  = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgd]                  = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdLight]             = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseDown]         = highlight;
//            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver]         = (lowResolution) ? SystemColors.ControlLight : highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver]         = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlText]                  = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextDisabled]          = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextLight]             = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseDown]         = highlightText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver]         = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDockSeparatorLine]        = empty;

            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandleShadow]         = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDropDownArrow]            = empty;
            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin]    = buttonFace;           

            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverEnd]         = msocbvcrCBCtlBkgdMouseOver;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverBegin]       = msocbvcrCBCtlBkgdMouseOver;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle]      = msocbvcrCBCtlBkgdMouseOver;

            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsEnd]           = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverBegin]    = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverEnd]      = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverMiddle]   = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedBegin]     = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedEnd]       = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedMiddle]    = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedBegin]            = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedEnd]              = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedMiddle]           = empty;



            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertBegin]                = gradientBegin;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertMiddle]               = gradientMiddle;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertEnd]                  = gradientEnd;

          
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownBegin]   = msocbvcrCBCtlBkgdMouseDown;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle]  = msocbvcrCBCtlBkgdMouseDown;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownEnd]     = msocbvcrCBCtlBkgdMouseDown;


            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin]   = gradientBegin;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd]     = gradientMiddle;


            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledDark]             = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledLight]            = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd]                   = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLowColorIconDisabled]         = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMainMenuBkgd]                 = buttonFace;
            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlText]                  = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlTextDisabled]          = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgd]                 = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped]          = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuShadow]                   = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuSplitArrow]               = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBOptionsButtonShadow]          = empty;


            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBShadow] = rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd];
            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLineLight]        = buttonHighlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandle]            = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver]   = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleBkgd]                = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleText]                = buttonHighlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledHighlightedText]          = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDlgGroupBoxText]                  = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdr]                        = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDark]                    = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown]           = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver]           = SystemColors.MenuText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLight]                   = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown]          = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver]          = SystemColors.MenuText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseDown]  = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver]  = SystemColors.MenuText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrSelected]   = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgd]          = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdSelected]  = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabText]          = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseDown] = highlightText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextSelected]  = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd]     = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd]     = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText]     = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText]     = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd]  = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd]  = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText]  = buttonHighlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText]  = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseDown] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseOver] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = SystemColors.InactiveCaption;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = SystemColors.InactiveCaptionText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBdr] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgdSelected] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderSeeThroughSelection] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentDarkBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentLightBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentTextDisabled] = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPHyperlink] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPLightBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlink] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlinkFollowed] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradEnd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrListHeaderArrow] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrNetLookBkgnd] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOABBkgd] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdrContrast] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerActiveBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBdr] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerInactiveBkgd] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = buttonHighlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabStopTicks] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGWorkspaceBkgd] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFlagNone] = buttonHighlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarDark] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarLight] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarText] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGridlines] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupLine] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupNested] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupShaded] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupText] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKIconBar] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarText] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKPreviewPaneLabelText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorDark] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorLight] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBActionDividerLine] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonDark] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = buttonHighlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBDarkOutline] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBFoldersBackground] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonDark] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonLight] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBLabelText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonDark] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonLight] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonDark] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonLight] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterDark] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPlacesBarBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelected] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrInactiveSelected] = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrMouseOver] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubWebDocScratchPageBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrSBBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrScrollbarBkgd] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradBegin] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradEnd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrInnerDocked] = empty;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterDocked] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterFloating] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdr] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDisabled] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgdDisabled] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlText] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextDisabled] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextMouseDown] = highlightText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPGroupline] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipBkgd] = SystemColors.Info;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipText] = SystemColors.InfoText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPNavBarBkgnd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = windowText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTextDisabled] = grayText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdActive] = highlight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdInactive] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextActive] = highlightText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextInactive] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrXLFormulaBarBkgd] = buttonFace;


        }

        internal void InitOliveLunaColors(ref Dictionary<KnownColors,Color> rgbTable) {
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(81, 94, 51);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(81, 94, 51);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(116, 134, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd] = Color.FromArgb(209, 222, 173);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(209, 222, 173);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(255, 192, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(141, 141, 141);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(96, 119, 66);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(81, 94, 51);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(217, 217, 167);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(242, 241, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(230, 230, 209);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(160, 177, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(186, 201, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(237, 240, 214);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(181, 196, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(255, 223, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(255, 177, 109);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(255, 255, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(255, 203, 136);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(255, 225, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(186, 204, 150);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(96, 119, 107);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(141, 160, 107);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(255, 255, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(255, 193, 118);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(255, 225, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(254, 140, 73);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(255, 221, 152);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(255, 184, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(255, 223, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(255, 166, 76);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(255, 195, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(255, 255, 237);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(181, 196, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(206, 220, 167);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(131, 144, 113);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(243, 244, 240);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(218, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(218, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(159, 174, 122);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(117, 141, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(244, 244, 238);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(141, 141, 141);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(216, 227, 182);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(173, 181, 157);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(173, 181, 157);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(134, 148, 108);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBShadow] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(244, 247, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(197, 212, 159);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(116, 134, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(220, 224, 208);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(153, 84, 10);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(96, 119, 107);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(176, 194, 140);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(63, 93, 56);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(218, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(218, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(218, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(183, 198, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(183, 198, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(191, 191, 223);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(239, 235, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(126, 125, 104);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(239, 235, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 192, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(159, 171, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(159, 171, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(217, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(230, 234, 208);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(150, 145, 133);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(161, 176, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(210, 223, 174);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(90, 107, 70);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(90, 107, 70);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(0, 61, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(243, 242, 231);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(217, 217, 167);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(217, 217, 167);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(242, 241, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(242, 241, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(255, 255, 237);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOABBkgd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(211, 211, 211);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(151, 160, 123);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(226, 231, 191);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(171, 192, 138);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(117, 141, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(218, 227, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(151, 160, 123);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(242, 240, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(96, 119, 66);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(175, 192, 130);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(234, 233, 225);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(181, 196, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(253, 238, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(175, 186, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(115, 137, 84);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(253, 247, 233);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(151, 160, 123);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(151, 160, 123);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(187, 85, 3);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(251, 200, 79);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(200, 212, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(176, 191, 138);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(234, 240, 207);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(234, 240, 207);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(247, 190, 87);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 255, 220);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(50, 69, 105);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(248, 222, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(232, 127, 8);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(238, 147, 17);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(251, 230, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(64, 81, 59);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(120, 142, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(120, 142, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(242, 240, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(96, 128, 88);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(206, 220, 167);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(107, 129, 107);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(107, 129, 107);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(107, 129, 107);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(151, 160, 123);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(193, 198, 176);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrSBBdr] = Color.FromArgb(211, 211, 211);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(249, 249, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(237, 242, 212);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(191, 206, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(242, 241, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(116, 134, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBkgd] = Color.FromArgb(243, 242, 231);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(164, 185, 127);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(197, 212, 159);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(222, 222, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPGroupline] = Color.FromArgb(188, 187, 177);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 255, 204);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(116, 134, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(216, 227, 182);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(188, 205, 131);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(217, 217, 167);
        }

        internal void InitSilverLunaColors(ref Dictionary<KnownColors,Color> rgbTable) {
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(173, 174, 193);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(122, 121, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd] = Color.FromArgb(219, 218, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(219, 218, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(255, 192, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(141, 141, 141);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(110, 109, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(84, 84, 117);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(224, 223, 227);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(215, 215, 229);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(243, 243, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(215, 215, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(118, 116, 151);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(184, 185, 202);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(232, 233, 242);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(172, 170, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(255, 223, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(255, 177, 109);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(255, 255, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(255, 203, 136);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(255, 225, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(186, 185, 206);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(118, 116, 146);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(156, 155, 180);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(255, 255, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(255, 193, 118);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(255, 225, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(254, 140, 73);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(255, 221, 152);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(255, 184, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(255, 223, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(255, 166, 76);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(255, 195, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(249, 249, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(147, 145, 176);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(225, 226, 236);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(122, 121, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(247, 245, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(212, 212, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(212, 212, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(168, 167, 190);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(198, 200, 215);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(253, 250, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(141, 141, 141);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(214, 211, 231);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(185, 187, 200);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(185, 187, 200);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(154, 140, 176);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBShadow] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(110, 109, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(192, 192, 211);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(122, 121, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(59, 59, 63);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(7, 70, 213);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(118, 116, 146);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(186, 185, 206);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(75, 75, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(212, 212, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(212, 212, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(212, 212, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(148, 148, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(148, 148, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(171, 169, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(171, 169, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(224, 223, 227);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(224, 223, 227);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(191, 191, 223);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(239, 235, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(126, 125, 104);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(223, 223, 234);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 192, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(162, 162, 181);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(162, 162, 181);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(212, 213, 229);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(227, 227, 236);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(150, 145, 133);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(169, 168, 191);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(208, 208, 223);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(92, 91, 121);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(92, 91, 121);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(0, 61, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(238, 238, 244);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(215, 215, 229);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(215, 215, 229);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(243, 243, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(243, 243, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(249, 249, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOABBkgd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(211, 211, 211);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(155, 154, 179);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(223, 223, 234);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(177, 176, 195);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(212, 212, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(155, 154, 179);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(239, 239, 244);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(110, 109, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(168, 167, 191);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(234, 233, 225);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(165, 164, 189);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(253, 238, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(229, 229, 235);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(112, 111, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(253, 247, 233);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(155, 154, 179);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(155, 154, 179);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(187, 85, 3);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(251, 200, 79);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(204, 206, 219);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(147, 145, 176);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(225, 226, 236);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(225, 226, 236);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(247, 190, 87);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 255, 220);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(50, 69, 105);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(248, 222, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(232, 127, 8);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(238, 147, 17);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(251, 230, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(110, 109, 143);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(168, 167, 191);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(168, 167, 191);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(224, 223, 227);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(243, 243, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(124, 124, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(215, 215, 229);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(142, 142, 170);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(142, 142, 170);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(142, 142, 170);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(155, 154, 179);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(195, 195, 210);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrSBBdr] = Color.FromArgb(236, 234, 218);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(247, 247, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(239, 239, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(179, 178, 204);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(243, 243, 247);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(122, 121, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBkgd] = Color.FromArgb(238, 238, 244);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(165, 172, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(192, 192, 211);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(222, 222, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPGroupline] = Color.FromArgb(161, 160, 187);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 255, 204);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(122, 121, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(184, 188, 234);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(198, 198, 217);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(215, 215, 229);
        }

        private void InitRoyaleColors(ref Dictionary<KnownColors,Color> rgbTable) {
            
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd] =     Color.FromArgb(238,237,240); // msocbvcrCBBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle ] =     Color.FromArgb(189,188,191); // msocbvcrCBDragHandle -> Needs to equal VSCOLOR_COMMANDBAR_DRAGHANDLE in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine] =     Color.FromArgb(193,193,196); // msocbvcrCBSplitterLine
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleBkgd] =     Color.FromArgb(167,166,170); // msocbvcrCBTitleBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleText] =     Color.FromArgb(255,255,255); // msocbvcrCBTitleText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterFloating] =     Color.FromArgb(142,141,145); // msocbvcrCBBdrOuterFloating
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked] =     Color.FromArgb(235,233,237); // msocbvcrCBBdrOuterDocked
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandle] =     Color.FromArgb(238,237,240); // msocbvcrCBTearOffHandle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver] =     Color.FromArgb(194,207,229); // msocbvcrCBTearOffHandleMouseOver 
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgd] =     Color.FromArgb(238,237,240); // msocbvcrCBCtlBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlText] =     Color.FromArgb(000,000,000); // msocbvcrCBCtlText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextDisabled] =     Color.FromArgb(176,175,179); // msocbvcrCBCtlTextDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] =     Color.FromArgb(194,207,229); // msocbvcrCBCtlBkgdMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_HOVER in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver] =     Color.FromArgb(51,94,168);  // msocbvcrCBCtlBdrMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_BORDER in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] =     Color.FromArgb(000,000,000); // msocbvcrCBCtlTextMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseDown] =     Color.FromArgb(153,175,212); // msocbvcrCBCtlBkgdMouseDown -> Needs to equal VSCOLOR_COMMANDBAR_HOVEROVERSELECTED in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseDown] =     Color.FromArgb(51,94,168);   // msocbvcrCBCtlBdrMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseDown] =     Color.FromArgb(255,255,255); // msocbvcrCBCtlTextMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelected] =     Color.FromArgb(226,229,238); // msocbvcrCBCtlBkgdSelected -> Needs to equal VSCOLOR_COMMANDBAR_SELECTED in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelected] =     Color.FromArgb(51,94,168);  // msocbvcrCBCtlBdrSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] =     Color.FromArgb(51,94,168);  // msocbvcrCBCtlBkgdSelectedMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_HOVEROVERSELECTEDICON in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] =     Color.FromArgb(51,94,168);   // msocbvcrCBCtlBdrSelectedMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_HOVEROVERSELECTEDICON_BORDER in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdLight] =     Color.FromArgb(255,255,255); // msocbvcrCBCtlBkgdLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextLight] =     Color.FromArgb(167,166,170); // msocbvcrCBCtlTextLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMainMenuBkgd] =     Color.FromArgb(235,233,237); // msocbvcrCBMainMenuBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd] =     Color.FromArgb(252,252,252); // msocbvcrCBMenuBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlText] =     Color.FromArgb(0,0,0); // msocbvcrCBMenuCtlText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlTextDisabled] =     Color.FromArgb(193,193,196); // msocbvcrCBMenuCtlTextDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter] =     Color.FromArgb(134,133,136); // msocbvcrCBMenuBdrOuter
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgd] =     Color.FromArgb(238,237,240); // msocbvcrCBMenuIconBkgd 
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] =     Color.FromArgb(228,226,230); // msocbvcrCBMenuIconBkgdDropped
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuSplitArrow] =     Color.FromArgb(167,166,170); // msocbvcrCBMenuSplitArrow
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBkgd] =     Color.FromArgb(245,244,246); // msocbvcrWPBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] =     Color.FromArgb(255,51,153);  // msocbvcrWPText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdActive] =     Color.FromArgb(255,51,153);  // msocbvcrWPTitleBkgdActive
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdInactive] =     Color.FromArgb(255,51,153);  // msocbvcrWPTitleBkgdInactive
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextActive] =     Color.FromArgb(255,51,153);  // msocbvcrWPTitleTextActive
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextInactive] =     Color.FromArgb(255,51,153);  // msocbvcrWPTitleTextInactive
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterFloating] =     Color.FromArgb(255,51,153);  // msocbvcrWPBdrOuterFloating
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterDocked] =     Color.FromArgb(255,51,153);  // msocbvcrWPBdrOuterDocked
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdr] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlText] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDisabled] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlBdrDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextDisabled] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlTextDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgdDisabled] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlBkgdDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlBdrDefault
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPGroupline] =     Color.FromArgb(255,51,153);  // msocbvcrWPGroupline
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrSBBdr] =     Color.FromArgb(255,51,153);  // msocbvcrSBBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdr] =     Color.FromArgb(255,51,153);  // msocbvcrOBBkgdBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdrContrast] =     Color.FromArgb(255,51,153);  // msocbvcrOBBkgdBdrContrast
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOABBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOABBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGDHeaderBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBdr] =     Color.FromArgb(255,51,153);  // msocbvcrGDHeaderBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBdr] =     Color.FromArgb(255,51,153);  // msocbvcrGDHeaderCellBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderSeeThroughSelection] =     Color.FromArgb(255,51,153);  // msocbvcrGDHeaderSeeThroughSelection
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGDHeaderCellBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgdSelected] =     Color.FromArgb(255,51,153);  // msocbvcrGDHeaderCellBkgdSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLineLight] =     Color.FromArgb(255,255,255); // msocbvcrCBSplitterLineLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBShadow] =     Color.FromArgb(238,237,240); // msocbvcrCBShadow -> Needs to equal VSCOLOR_COMMANDBAR_SHADOW in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBOptionsButtonShadow] =     Color.FromArgb(245,244,246); // msocbvcrCBOptionsButtonShadow
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPNavBarBkgnd] =     Color.FromArgb(193,193,196); // msocbvcrWPNavBarBkgnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrInnerDocked] =     Color.FromArgb(245,244,246);  // msocbvcrWPBdrInnerDocked
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] =     Color.FromArgb(235,233,237); // msocbvcrCBLabelBkgnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledLight] =     Color.FromArgb(235,233,237); // msocbvcrCBIconDisabledLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledDark] =     Color.FromArgb(167,166,170); // msocbvcrCBIconDisabledDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLowColorIconDisabled] =     Color.FromArgb(176,175,179); // msocbvcrCBLowColorIconDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin] =     Color.FromArgb(235,233,237); // msocbvcrCBGradMainMenuHorzBegin -> Needs to equal VSCOLOR_ENVIRONMENT_BACKGROUND and VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTBEGIN in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd] =     Color.FromArgb(251,250,251); // msocbvcrCBGradMainMenuHorzEnd -> Needs to equal VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTEND in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertBegin] =     Color.FromArgb(252,252,252); // msocbvcrCBGradVertBegin -> Needs to equal VSCOLOR_COMMANDBAR_GRADIENT_BEGIN in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertMiddle] =     Color.FromArgb(245,244,246); // msocbvcrCBGradVertMiddle -> Needs to equal VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertEnd] =     Color.FromArgb(235,233,237); // msocbvcrCBGradVertEnd -> Needs to equal VSCOLOR_COMMANDBAR_GRADIENT_END in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin] =     Color.FromArgb(242,242,242); // msocbvcrCBGradOptionsBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle] =     Color.FromArgb(224,224,225); // msocbvcrCBGradOptionsMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsEnd] =     Color.FromArgb(167,166,170); // msocbvcrCBGradOptionsEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] =     Color.FromArgb(252,252,252); // msocbvcrCBGradMenuTitleBkgdBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] =     Color.FromArgb(245,244,246); // msocbvcrCBGradMenuTitleBkgdEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] =     Color.FromArgb(247,246,248); // msocbvcrCBGradMenuIconBkgdDroppedBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] =     Color.FromArgb(241,240,242); // msocbvcrCBGradMenuIconBkgdDroppedMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] =     Color.FromArgb(228,226,230); // msocbvcrCBGradMenuIconBkgdDroppedEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedBegin] =     Color.FromArgb(226,229,238); // msocbvcrCBGradOptionsSelectedBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedMiddle] =     Color.FromArgb(226,229,238); // msocbvcrCBGradOptionsSelectedMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedEnd] =     Color.FromArgb(226,229,238); // msocbvcrCBGradOptionsSelectedEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverBegin] =     Color.FromArgb(194,207,229); // msocbvcrCBGradOptionsMouseOverBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] =     Color.FromArgb(194,207,229); // msocbvcrCBGradOptionsMouseOverMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverEnd] =     Color.FromArgb(194,207,229); // msocbvcrCBGradOptionsMouseOverEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedBegin] =     Color.FromArgb(226,229,238); // msocbvcrCBGradSelectedBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedMiddle] =     Color.FromArgb(226,229,238); // msocbvcrCBGradSelectedMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedEnd] =     Color.FromArgb(226,229,238); // msocbvcrCBGradSelectedEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverBegin] =     Color.FromArgb(194,207,229); // msocbvcrCBGradMouseOverBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle] =     Color.FromArgb(194,207,229); // msocbvcrCBGradMouseOverMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverEnd] =     Color.FromArgb(194,207,229); // msocbvcrCBGradMouseOverEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownBegin] =     Color.FromArgb(153,175,212); // msocbvcrCBGradMouseDownBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle] =     Color.FromArgb(153,175,212); // msocbvcrCBGradMouseDownMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownEnd] =     Color.FromArgb(153,175,212); // msocbvcrCBGradMouseDownEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrNetLookBkgnd] =     Color.FromArgb(235,233,237); // msocbvcrNetLookBkgnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuShadow] =     Color.FromArgb(000,000,000); // msocbvcrCBMenuShadow
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDockSeparatorLine] =     Color.FromArgb(51,94,168);  // msocbvcrCBDockSeparatorLine
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDropDownArrow] =     Color.FromArgb(235,233,237); // msocbvcrCBDropDownArrow
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGridlines] =     Color.FromArgb(255,51,153);  // msocbvcrOLKGridlines
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupText] =     Color.FromArgb(255,51,153);  // msocbvcrOLKGroupText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupLine] =     Color.FromArgb(255,51,153);  // msocbvcrOLKGroupLine
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupShaded] =     Color.FromArgb(255,51,153);  // msocbvcrOLKGroupShaded
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupNested] =     Color.FromArgb(255,51,153);  // msocbvcrOLKGroupNested
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKIconBar] =     Color.FromArgb(255,51,153);  // msocbvcrOLKIconBar
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFlagNone] =     Color.FromArgb(255,51,153);  // msocbvcrOLKFlagNone
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKFolderbarLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKFolderbarDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarText] =     Color.FromArgb(255,51,153);  // msocbvcrOLKFolderbarText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBButtonLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBButtonDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBSelectedButtonLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBSelectedButtonDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBHoverButtonLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBHoverButtonDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBPressedButtonLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBPressedButtonDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBDarkOutline] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBDarkOutline
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBSplitterLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBSplitterDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBActionDividerLine] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBActionDividerLine
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBLabelText] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBLabelText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBFoldersBackground] =     Color.FromArgb(255,51,153);  // msocbvcrOLKWBFoldersBackground
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorLight] =     Color.FromArgb(255,51,153);  // msocbvcrOLKTodayIndicatorLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorDark] =     Color.FromArgb(255,51,153);  // msocbvcrOLKTodayIndicatorDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOLKInfoBarBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarText] =     Color.FromArgb(255,51,153);  // msocbvcrOLKInfoBarText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKPreviewPaneLabelText] =     Color.FromArgb(255,51,153);  // msocbvcrOLKPreviewPaneLabelText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlink] =     Color.FromArgb(0,61,178);    // msocbvcrHyperlink
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlinkFollowed] =     Color.FromArgb(170,0,170);   // msocbvcrHyperlinkFollowed
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGWorkspaceBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOGWorkspaceBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOGMDIParentWorkspaceBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerActiveBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerActiveBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerInactiveBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerInactiveBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerText] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabStopTicks] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerTabStopTicks
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBdr] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdr] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerTabBoxBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] =     Color.FromArgb(255,51,153);  // msocbvcrOGRulerTabBoxBdrHighlight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrXLFormulaBarBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrXLFormulaBarBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandleShadow] =     Color.FromArgb(255,255,255); // msocbvcrCBDragHandleShadow -> Needs to equal VSCOLOR_COMMANDBAR_DRAGHANDLE_SHADOW in vscolors.cpp
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrOGTaskPaneGroupBoxHeaderBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] =     Color.FromArgb(255,51,153);  // msocbvcrPPOutlineThumbnailsPaneTabBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] =     Color.FromArgb(255,51,153);  // msocbvcrPPOutlineThumbnailsPaneTabText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelected] =     Color.FromArgb(255,51,153);  // msocbvcrPPSlideBdrActiveSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrInactiveSelected] =     Color.FromArgb(255,51,153);  // msocbvcrPPSlideBdrInactiveSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrMouseOver] =     Color.FromArgb(255,51,153);  // msocbvcrPPSlideBdrMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] =     Color.FromArgb(255,51,153);  // msocbvcrPPSlideBdrActiveSelectedMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDlgGroupBoxText] =     Color.FromArgb(0,0,0);    // msocbvcrDlgGroupBoxText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrScrollbarBkgd] =     Color.FromArgb(237,235,239); // msocbvcrScrollbarBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrListHeaderArrow] =     Color.FromArgb(155,154,156); // msocbvcrListHeaderArrow
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledHighlightedText] =     Color.FromArgb(188,202,226); // msocbvcrDisabledHighlightedText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] =     Color.FromArgb(235,233,237); // msocbvcrFocuslessHighlightedBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] =     Color.FromArgb(000,000,000); // msocbvcrFocuslessHighlightedText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] =     Color.FromArgb(167,166,170); // msocbvcrDisabledFocuslessHighlightedText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextMouseDown] =     Color.FromArgb(255,51,153);  // msocbvcrWPCtlTextMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTextDisabled] =     Color.FromArgb(255,51,153);  // msocbvcrWPTextDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrWPInfoTipBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipText] =     Color.FromArgb(255,51,153);  // msocbvcrWPInfoTipText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrDWActiveTabBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] =     Color.FromArgb(255,51,153);  // msocbvcrDWActiveTabText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] =     Color.FromArgb(255,51,153);  // msocbvcrDWActiveTabTextDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrDWInactiveTabBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] =     Color.FromArgb(255,51,153);  // msocbvcrDWInactiveTabText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] =     Color.FromArgb(255,51,153);  // msocbvcrDWTabBkgdMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseOver] =     Color.FromArgb(255,51,153);  // msocbvcrDWTabTextMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] =     Color.FromArgb(255,51,153);  // msocbvcrDWTabBkgdMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseDown] =     Color.FromArgb(255,51,153);  // msocbvcrDWTabTextMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPLightBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGSPLightBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGSPDarkBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderLightBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupHeaderLightBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupHeaderDarkBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupHeaderText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentLightBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupContentLightBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentDarkBkgd] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupContentDarkBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentText] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupContentText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentTextDisabled] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupContentTextDisabled
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] =     Color.FromArgb(255,51,153);  // msocbvcrGSPGroupline
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPHyperlink] =     Color.FromArgb(255,51,153);  // msocbvcrGSPHyperlink
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgd] =     Color.FromArgb(212,212,226); // msocbvcrDocTabBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabText] =     Color.FromArgb(000,000,000); // msocbvcrDocTabText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdr] =     Color.FromArgb(118,116,146); // msocbvcrDocTabBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLight] =     Color.FromArgb(255,255,255); // msocbvcrDocTabBdrLight
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDark] =     Color.FromArgb(186,185,206); // msocbvcrDocTabBdrDark
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdSelected] =     Color.FromArgb(255,255,255); // msocbvcrDocTabBkgdSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextSelected] =     Color.FromArgb(000,000,000); // msocbvcrDocTabTextSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrSelected] =     Color.FromArgb(124,124,148); // msocbvcrDocTabBdrSelected
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] =     Color.FromArgb(193,210,238); // msocbvcrDocTabBkgdMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] =     Color.FromArgb(49,106,197);  // msocbvcrDocTabTextMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] =     Color.FromArgb(49,106,197);  // msocbvcrDocTabBdrMouseOver 
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] =     Color.FromArgb(49,106,197);  // msocbvcrDocTabBdrLightMouseOver 
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] =     Color.FromArgb(49,106,197);  // msocbvcrDocTabBdrDarkMouseOver
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] =     Color.FromArgb(154,183,228); // msocbvcrDocTabBkgdMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseDown] =     Color.FromArgb(000,000,000); // msocbvcrDocTabTextMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseDown] =     Color.FromArgb(75,75,111);   // msocbvcrDocTabBdrMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown] =     Color.FromArgb(75,75,111);   // msocbvcrDocTabBdrLightMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown] =     Color.FromArgb(75,75,111);   // msocbvcrDocTabBdrDarkMouseDown
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradBegin] =     Color.FromArgb(246,244,236); // msocbvcrToastGradBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradEnd] =     Color.FromArgb(179,178,204); // msocbvcrToastGradEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] =     Color.FromArgb(236,233,216); // msocbvcrJotNavUIGradBegin
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] =     Color.FromArgb(236,233,216); // msocbvcrJotNavUIGradMiddle
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradEnd] =     Color.FromArgb(255,255,255); // msocbvcrJotNavUIGradEnd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIText] =     Color.FromArgb(000,000,000); // msocbvcrJotNavUIText
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] =     Color.FromArgb(172,168,153); // msocbvcrJotNavUIBdr
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPlacesBarBkgd] =     Color.FromArgb(224,223,227); // msocbvcrPlacesBarBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubPrintDocScratchPageBkgd] =     Color.FromArgb(152,181,226); // msocbvcrPubPrintDocScratchPageBkgd
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubWebDocScratchPageBkgd] =     Color.FromArgb(193,210,238); // msocbvcrPubWebDocScratchPageBkgd

        }


 
        
        internal void InitThemedColors(ref Dictionary<KnownColors,Color> rgbTable) {

            string colorScheme = VisualStyleInformation.ColorScheme;
            string themeFileName = System.IO.Path.GetFileName(VisualStyleInformation.ThemeFilename);
            bool initializedTable = false;

            // VS compares the filename of the theme to determine luna v. royale.
            if (string.Equals(lunaFileName, themeFileName, StringComparison.OrdinalIgnoreCase)) {
                // once we know it's luna we've got to pick between 
                // normal (blue) homestead (olive) and metallic (silver)
                if (colorScheme == normalColorScheme) {
                    InitBlueLunaColors(ref rgbTable);
                    usingSystemColors = false;
                    initializedTable = true;
                }
                else if (colorScheme == oliveColorScheme) {
                    InitOliveLunaColors(ref rgbTable);
                    usingSystemColors = false;
                    initializedTable = true;
                }
                else if (colorScheme == silverColorScheme) {
                    InitSilverLunaColors(ref rgbTable);
                    usingSystemColors = false;
                    initializedTable = true;
                }

            }
            else if (string.Equals(aeroFileName, themeFileName, StringComparison.OrdinalIgnoreCase)) {
                // On Vista running Aero theme, Office looks like it's using SystemColors
                // With the exception of the MenuItemSelected Color for MenuStrip items that
                // are contained in DropDowns.  We're going to copy their behavior
                InitSystemColors(ref rgbTable);
                usingSystemColors = true;
                initializedTable = true;
                
                // Exception to SystemColors, use the ButtonSelectedHighlight color otherwise
                // the background for DropDown MenuStrip items will have no contrast
                rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = rgbTable[KnownColors.ButtonSelectedHighlight];

                // CheckedBackground of ToolStripMenuItem
                rgbTable[KnownColors.msocbvcrCBCtlBkgdSelected] = rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver];
            }
            else if (string.Equals(royaleFileName, themeFileName,StringComparison.OrdinalIgnoreCase)) {

                // once we know it's royale (TabletPC/MCE) we know about two color scheme names 
                // which should do exactly the same thing                
                if (colorScheme == normalColorScheme || colorScheme == royaleColorScheme ) {
                    InitRoyaleColors(ref rgbTable);
                    usingSystemColors = false;
                    initializedTable = true;
                }

            }
            
           
            if (!initializedTable) {
                // unknown color scheme - bailing 

                InitSystemColors(ref rgbTable);
                usingSystemColors = true;
            }

            InitCommonColors(ref rgbTable);
            
        }

        internal void InitBlueLunaColors(ref Dictionary<KnownColors,Color> rgbTable) {
       
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(196, 205, 218);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(196, 205, 218);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(42, 102, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd] = Color.FromArgb(196, 219, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(196, 219, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(255, 192, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(141, 141, 141);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(0, 53, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(39, 65, 118);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(158, 190, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(196, 218, 250);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(203, 221, 246);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(114, 155, 215);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(161, 197, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(227, 239, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(123, 164, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(255, 223, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(255, 177, 109);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(255, 255, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(255, 203, 136);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(255, 225, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(127, 177, 250);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(0, 53, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(82, 127, 208);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(255, 255, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(255, 193, 118);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(255, 225, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(254, 140, 73);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(255, 221, 152);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(255, 184, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(255, 223, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(255, 166, 76);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(255, 195, 116);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(227, 239, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(123, 164, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(203, 225, 252);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(97, 122, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(233, 236, 242);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(186, 211, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(186, 211, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(109, 150, 208);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(153, 204, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(0, 45, 150);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(246, 246, 246);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(141, 141, 141);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(203, 225, 252);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(172, 183, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(172, 183, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(95, 130, 234);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBShadow] = Color.FromArgb(59, 97, 156);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(106, 140, 203);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(241, 249, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(169, 199, 240);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(42, 102, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(187, 206, 236);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(0, 70, 213);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(0, 53, 154);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(117, 166, 241);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(0, 0, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(59, 97, 156);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(186, 211, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(186, 211, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(186, 211, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(94, 94, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(94, 94, 94);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(129, 169, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(129, 169, 226);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(89, 89, 172);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(239, 235, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(126, 125, 104);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(239, 235, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 192, 111);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(191, 191, 223);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(74, 122, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(74, 122, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(185, 208, 241);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(221, 236, 254);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(150, 145, 133);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(101, 143, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(196, 219, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(0, 45, 134);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(0, 45, 134);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(0, 61, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(221, 236, 254);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(59, 97, 156);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(59, 97, 156);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(158, 190, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(158, 190, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(196, 218, 250);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(196, 218, 250);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(227, 239, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOABBkgd] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(144, 153, 174);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(216, 231, 252);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(158, 190, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(75, 120, 202);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGRulerText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(186, 211, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(144, 153, 174);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(242, 240, 228);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(0, 53, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(89, 135, 214);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(234, 233, 225);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(123, 164, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(253, 238, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(190, 218, 251);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(55, 104, 185);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(253, 247, 233);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(144, 153, 174);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(144, 153, 174);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(187, 85, 3);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(251, 200, 79);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(215, 228, 251);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(123, 164, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(203, 225, 252);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(203, 225, 252);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(0, 45, 150);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 255, 255);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(247, 190, 87);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 255, 220);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(50, 69, 105);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(248, 222, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(232, 127, 8);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(238, 147, 17);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(251, 230, 148);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(0, 53, 145);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(89, 135, 214);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(89, 135, 214);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(236, 233, 216);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(195, 218, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(59, 97, 156);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(158, 190, 245);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(61, 108, 192);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(61, 108, 192);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(61, 108, 192);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(144, 153, 174);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(189, 194, 207);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrSBBdr] = Color.FromArgb(211, 211, 211);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(251, 251, 248);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(220, 236, 254);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(167, 197, 238);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(185, 212, 249);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(196, 218, 250);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(42, 102, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPBkgd] = Color.FromArgb(221, 236, 254);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(127, 157, 185);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(128, 128, 128);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(169, 199, 240);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(222, 222, 222);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPGroupline] = Color.FromArgb(123, 164, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 255, 204);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(74, 122, 201);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(172, 168, 153);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(123, 164, 224);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(148, 187, 239);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(0, 0, 0);
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(158, 190, 245);
        }

        

        internal enum KnownColors {
            msocbvcrCBBdrOuterDocked,
            msocbvcrCBBdrOuterFloating,
            msocbvcrCBBkgd,
            msocbvcrCBCtlBdrMouseDown,
            msocbvcrCBCtlBdrMouseOver,
            msocbvcrCBCtlBdrSelected,
            msocbvcrCBCtlBdrSelectedMouseOver,
            msocbvcrCBCtlBkgd,
            msocbvcrCBCtlBkgdLight,
            msocbvcrCBCtlBkgdMouseDown,
            msocbvcrCBCtlBkgdMouseOver,
            msocbvcrCBCtlBkgdSelected,
            msocbvcrCBCtlBkgdSelectedMouseOver,
            msocbvcrCBCtlText,
            msocbvcrCBCtlTextDisabled,
            msocbvcrCBCtlTextLight,
            msocbvcrCBCtlTextMouseDown,
            msocbvcrCBCtlTextMouseOver,
            msocbvcrCBDockSeparatorLine,
            msocbvcrCBDragHandle,
            msocbvcrCBDragHandleShadow,
            msocbvcrCBDropDownArrow,
            msocbvcrCBGradMainMenuHorzBegin,
            msocbvcrCBGradMainMenuHorzEnd,
            msocbvcrCBGradMenuIconBkgdDroppedBegin,
            msocbvcrCBGradMenuIconBkgdDroppedEnd,
            msocbvcrCBGradMenuIconBkgdDroppedMiddle,
            msocbvcrCBGradMenuTitleBkgdBegin,
            msocbvcrCBGradMenuTitleBkgdEnd,
            msocbvcrCBGradMouseDownBegin,
            msocbvcrCBGradMouseDownEnd,
            msocbvcrCBGradMouseDownMiddle,
            msocbvcrCBGradMouseOverBegin,
            msocbvcrCBGradMouseOverEnd,
            msocbvcrCBGradMouseOverMiddle,
            msocbvcrCBGradOptionsBegin,
            msocbvcrCBGradOptionsEnd,
            msocbvcrCBGradOptionsMiddle,
            msocbvcrCBGradOptionsMouseOverBegin,
            msocbvcrCBGradOptionsMouseOverEnd,
            msocbvcrCBGradOptionsMouseOverMiddle,
            msocbvcrCBGradOptionsSelectedBegin,
            msocbvcrCBGradOptionsSelectedEnd,
            msocbvcrCBGradOptionsSelectedMiddle,
            msocbvcrCBGradSelectedBegin,
            msocbvcrCBGradSelectedEnd,
            msocbvcrCBGradSelectedMiddle,
            msocbvcrCBGradVertBegin,
            msocbvcrCBGradVertEnd,
            msocbvcrCBGradVertMiddle,
            msocbvcrCBIconDisabledDark,
            msocbvcrCBIconDisabledLight,
            msocbvcrCBLabelBkgnd,
            msocbvcrCBLowColorIconDisabled,
            msocbvcrCBMainMenuBkgd,
            msocbvcrCBMenuBdrOuter,
            msocbvcrCBMenuBkgd,
            msocbvcrCBMenuCtlText,
            msocbvcrCBMenuCtlTextDisabled,
            msocbvcrCBMenuIconBkgd,
            msocbvcrCBMenuIconBkgdDropped,
            msocbvcrCBMenuShadow,
            msocbvcrCBMenuSplitArrow,
            msocbvcrCBOptionsButtonShadow,
            msocbvcrCBShadow,
            msocbvcrCBSplitterLine,
            msocbvcrCBSplitterLineLight,
            msocbvcrCBTearOffHandle,
            msocbvcrCBTearOffHandleMouseOver,
            msocbvcrCBTitleBkgd,
            msocbvcrCBTitleText,
            msocbvcrDisabledFocuslessHighlightedText,
            msocbvcrDisabledHighlightedText,
            msocbvcrDlgGroupBoxText,
            msocbvcrDocTabBdr,
            msocbvcrDocTabBdrDark,
            msocbvcrDocTabBdrDarkMouseDown,
            msocbvcrDocTabBdrDarkMouseOver,
            msocbvcrDocTabBdrLight,
            msocbvcrDocTabBdrLightMouseDown,
            msocbvcrDocTabBdrLightMouseOver,
            msocbvcrDocTabBdrMouseDown,
            msocbvcrDocTabBdrMouseOver,
            msocbvcrDocTabBdrSelected,
            msocbvcrDocTabBkgd,
            msocbvcrDocTabBkgdMouseDown,
            msocbvcrDocTabBkgdMouseOver,
            msocbvcrDocTabBkgdSelected,
            msocbvcrDocTabText,
            msocbvcrDocTabTextMouseDown,
            msocbvcrDocTabTextMouseOver,
            msocbvcrDocTabTextSelected,
            msocbvcrDWActiveTabBkgd,
            msocbvcrDWActiveTabText,
            msocbvcrDWActiveTabTextDisabled,
            msocbvcrDWInactiveTabBkgd,
            msocbvcrDWInactiveTabText,
            msocbvcrDWTabBkgdMouseDown,
            msocbvcrDWTabBkgdMouseOver,
            msocbvcrDWTabTextMouseDown,
            msocbvcrDWTabTextMouseOver,
            msocbvcrFocuslessHighlightedBkgd,
            msocbvcrFocuslessHighlightedText,
            msocbvcrGDHeaderBdr,
            msocbvcrGDHeaderBkgd,
            msocbvcrGDHeaderCellBdr,
            msocbvcrGDHeaderCellBkgd,
            msocbvcrGDHeaderCellBkgdSelected,
            msocbvcrGDHeaderSeeThroughSelection,
            msocbvcrGSPDarkBkgd,
            msocbvcrGSPGroupContentDarkBkgd,
            msocbvcrGSPGroupContentLightBkgd,
            msocbvcrGSPGroupContentText,
            msocbvcrGSPGroupContentTextDisabled,
            msocbvcrGSPGroupHeaderDarkBkgd,
            msocbvcrGSPGroupHeaderLightBkgd,
            msocbvcrGSPGroupHeaderText,
            msocbvcrGSPGroupline,
            msocbvcrGSPHyperlink,
            msocbvcrGSPLightBkgd,
            msocbvcrHyperlink,
            msocbvcrHyperlinkFollowed,
            msocbvcrJotNavUIBdr,
            msocbvcrJotNavUIGradBegin,
            msocbvcrJotNavUIGradEnd,
            msocbvcrJotNavUIGradMiddle,
            msocbvcrJotNavUIText,
            msocbvcrListHeaderArrow,
            msocbvcrNetLookBkgnd,
            msocbvcrOABBkgd,
            msocbvcrOBBkgdBdr,
            msocbvcrOBBkgdBdrContrast,
            msocbvcrOGMDIParentWorkspaceBkgd,
            msocbvcrOGRulerActiveBkgd,
            msocbvcrOGRulerBdr,
            msocbvcrOGRulerBkgd,
            msocbvcrOGRulerInactiveBkgd,
            msocbvcrOGRulerTabBoxBdr,
            msocbvcrOGRulerTabBoxBdrHighlight,
            msocbvcrOGRulerTabStopTicks,
            msocbvcrOGRulerText,
            msocbvcrOGTaskPaneGroupBoxHeaderBkgd,
            msocbvcrOGWorkspaceBkgd,
            msocbvcrOLKFlagNone,
            msocbvcrOLKFolderbarDark,
            msocbvcrOLKFolderbarLight,
            msocbvcrOLKFolderbarText,
            msocbvcrOLKGridlines,
            msocbvcrOLKGroupLine,
            msocbvcrOLKGroupNested,
            msocbvcrOLKGroupShaded,
            msocbvcrOLKGroupText,
            msocbvcrOLKIconBar,
            msocbvcrOLKInfoBarBkgd,
            msocbvcrOLKInfoBarText,
            msocbvcrOLKPreviewPaneLabelText,
            msocbvcrOLKTodayIndicatorDark,
            msocbvcrOLKTodayIndicatorLight,
            msocbvcrOLKWBActionDividerLine,
            msocbvcrOLKWBButtonDark,
            msocbvcrOLKWBButtonLight,
            msocbvcrOLKWBDarkOutline,
            msocbvcrOLKWBFoldersBackground,
            msocbvcrOLKWBHoverButtonDark,
            msocbvcrOLKWBHoverButtonLight,
            msocbvcrOLKWBLabelText,
            msocbvcrOLKWBPressedButtonDark,
            msocbvcrOLKWBPressedButtonLight,
            msocbvcrOLKWBSelectedButtonDark,
            msocbvcrOLKWBSelectedButtonLight,
            msocbvcrOLKWBSplitterDark,
            msocbvcrOLKWBSplitterLight,
            msocbvcrPlacesBarBkgd,
            msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd,
            msocbvcrPPOutlineThumbnailsPaneTabBdr,
            msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd,
            msocbvcrPPOutlineThumbnailsPaneTabText,
            msocbvcrPPSlideBdrActiveSelected,
            msocbvcrPPSlideBdrActiveSelectedMouseOver,
            msocbvcrPPSlideBdrInactiveSelected,
            msocbvcrPPSlideBdrMouseOver,
            msocbvcrPubPrintDocScratchPageBkgd,
            msocbvcrPubWebDocScratchPageBkgd,
            msocbvcrSBBdr,
            msocbvcrScrollbarBkgd,
            msocbvcrToastGradBegin,
            msocbvcrToastGradEnd,
            msocbvcrWPBdrInnerDocked,
            msocbvcrWPBdrOuterDocked,
            msocbvcrWPBdrOuterFloating,
            msocbvcrWPBkgd,
            msocbvcrWPCtlBdr,
            msocbvcrWPCtlBdrDefault,
            msocbvcrWPCtlBdrDisabled,
            msocbvcrWPCtlBkgd,
            msocbvcrWPCtlBkgdDisabled,
            msocbvcrWPCtlText,
            msocbvcrWPCtlTextDisabled,
            msocbvcrWPCtlTextMouseDown,
            msocbvcrWPGroupline,
            msocbvcrWPInfoTipBkgd,
            msocbvcrWPInfoTipText,
            msocbvcrWPNavBarBkgnd,
            msocbvcrWPText,
            msocbvcrWPTextDisabled,
            msocbvcrWPTitleBkgdActive,
            msocbvcrWPTitleBkgdInactive,
            msocbvcrWPTitleTextActive,
            msocbvcrWPTitleTextInactive,
            msocbvcrXLFormulaBarBkgd,
            ButtonSelectedHighlight, // not actually from MSO tables
            ButtonPressedHighlight,// not actually from MSO tables
            ButtonCheckedHighlight,// not actually from MSO tables
            lastKnownColor = ButtonCheckedHighlight
        }

    }

    
}

