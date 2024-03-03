// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class ProfessionalColorTable
{
    private Dictionary<KnownColors, Color>? _professionalRGB;
    private bool _usingSystemColors;
    private bool _useSystemColors;
    private string? _lastKnownColorScheme = string.Empty;

    private const string OliveColorScheme = "HomeStead";
    private const string NormalColorScheme = "NormalColor";
    private const string SilverColorScheme = "Metallic";
    private const string RoyaleColorScheme = "Royale";  // sometimes returns NormalColor, sometimes returns Royale.

    private const string LunaFileName = "luna.msstyles";
    private const string RoyaleFileName = "royale.msstyles";
    private const string AeroFileName = "aero.msstyles";

    private object? _colorFreshnessKey;

    public ProfessionalColorTable()
    {
    }

    private Dictionary<KnownColors, Color> ColorTable
    {
        get
        {
            if (UseSystemColors)
            {
                // someone has turned off theme support for the color table.
                if (!_usingSystemColors || _professionalRGB is null)
                {
                    _professionalRGB ??= new Dictionary<KnownColors, Color>((int)KnownColors.lastKnownColor);
                    InitSystemColors(ref _professionalRGB);
                }
            }
            else if (ToolStripManager.VisualStylesEnabled)
            {
                // themes are on and enabled in the manager
                if (_usingSystemColors || _professionalRGB is null)
                {
                    _professionalRGB ??= new Dictionary<KnownColors, Color>((int)KnownColors.lastKnownColor);
                    InitThemedColors(ref _professionalRGB);
                }
            }
            else
            {
                // themes are off.
                if (!_usingSystemColors || _professionalRGB is null)
                {
                    _professionalRGB ??= new Dictionary<KnownColors, Color>((int)KnownColors.lastKnownColor);
                    InitSystemColors(ref _professionalRGB);
                }
            }

            return _professionalRGB;
        }
    }

    /// <summary>
    ///  When this is specified, professional colors picks from SystemColors
    ///  rather than colors that match the current theme. If theming is not
    ///  turned on, we'll fall back to Application.SystemColors.
    /// </summary>
    public bool UseSystemColors
    {
        get => _useSystemColors;
        set
        {
            if (_useSystemColors == value)
            {
                return;
            }

            _useSystemColors = value;
            ResetRGBTable();
        }
    }

    private Color FromKnownColor(KnownColors color)
    {
        if (ProfessionalColors.ColorFreshnessKey != _colorFreshnessKey || ProfessionalColors.ColorScheme != _lastKnownColorScheme)
        {
            ResetRGBTable();
        }

        _colorFreshnessKey = ProfessionalColors.ColorFreshnessKey;
        _lastKnownColorScheme = ProfessionalColors.ColorScheme;

        return ColorTable[color];
    }

    private void ResetRGBTable()
    {
        _professionalRGB?.Clear();
        _professionalRGB = null;
    }

    [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightDescr))]
    public virtual Color ButtonSelectedHighlight => FromKnownColor(KnownColors.ButtonSelectedHighlight);

    [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedHighlightBorderDescr))]
    public virtual Color ButtonSelectedHighlightBorder => ButtonPressedBorder;

    [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightDescr))]
    public virtual Color ButtonPressedHighlight => FromKnownColor(KnownColors.ButtonPressedHighlight);

    [SRDescription(nameof(SR.ProfessionalColorsButtonPressedHighlightBorderDescr))]
    public virtual Color ButtonPressedHighlightBorder => Application.SystemColors.Highlight;

    [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightDescr))]
    public virtual Color ButtonCheckedHighlight => FromKnownColor(KnownColors.ButtonCheckedHighlight);

    [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedHighlightBorderDescr))]
    public virtual Color ButtonCheckedHighlightBorder => Application.SystemColors.Highlight;

    [SRDescription(nameof(SR.ProfessionalColorsButtonPressedBorderDescr))]
    public virtual Color ButtonPressedBorder => FromKnownColor(KnownColors.msocbvcrCBCtlBdrMouseOver);

    [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedBorderDescr))]
    public virtual Color ButtonSelectedBorder => FromKnownColor(KnownColors.msocbvcrCBCtlBdrMouseOver);

    [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientBeginDescr))]
    public virtual Color ButtonCheckedGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradSelectedBegin);

    [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientMiddleDescr))]
    public virtual Color ButtonCheckedGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradSelectedMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsButtonCheckedGradientEndDescr))]
    public virtual Color ButtonCheckedGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradSelectedEnd);

    [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientBeginDescr))]
    public virtual Color ButtonSelectedGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMouseOverBegin);

    [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientMiddleDescr))]
    public virtual Color ButtonSelectedGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradMouseOverMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsButtonSelectedGradientEndDescr))]
    public virtual Color ButtonSelectedGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMouseOverEnd);

    [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientBeginDescr))]
    public virtual Color ButtonPressedGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMouseDownBegin);

    [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientMiddleDescr))]
    public virtual Color ButtonPressedGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradMouseDownMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsButtonPressedGradientEndDescr))]
    public virtual Color ButtonPressedGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMouseDownEnd);
    [SRDescription(nameof(SR.ProfessionalColorsCheckBackgroundDescr))]
    public virtual Color CheckBackground => FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelected);

    [SRDescription(nameof(SR.ProfessionalColorsCheckSelectedBackgroundDescr))]
    public virtual Color CheckSelectedBackground => FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver);

    [SRDescription(nameof(SR.ProfessionalColorsCheckPressedBackgroundDescr))]
    public virtual Color CheckPressedBackground => FromKnownColor(KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver);

    [SRDescription(nameof(SR.ProfessionalColorsGripDarkDescr))]
    public virtual Color GripDark => FromKnownColor(KnownColors.msocbvcrCBDragHandle);

    [SRDescription(nameof(SR.ProfessionalColorsGripLightDescr))]
    public virtual Color GripLight => FromKnownColor(KnownColors.msocbvcrCBDragHandleShadow);

    [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientBeginDescr))]
    public virtual Color ImageMarginGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradVertBegin);

    [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientMiddleDescr))]
    public virtual Color ImageMarginGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradVertMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsImageMarginGradientEndDescr))]
    public virtual Color ImageMarginGradientEnd => (_usingSystemColors) ? Application.SystemColors.Control : FromKnownColor(KnownColors.msocbvcrCBGradVertEnd);

    [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientBeginDescr))]
    public virtual Color ImageMarginRevealedGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin);

    [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientMiddleDescr))]
    public virtual Color ImageMarginRevealedGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsImageMarginRevealedGradientEndDescr))]
    public virtual Color ImageMarginRevealedGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd);

    [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientBeginDescr))]
    public virtual Color MenuStripGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin);

    [SRDescription(nameof(SR.ProfessionalColorsMenuStripGradientEndDescr))]
    public virtual Color MenuStripGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedDescr))]
    public virtual Color MenuItemSelected => FromKnownColor(KnownColors.msocbvcrCBCtlBkgdMouseOver);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemBorderDescr))]
    public virtual Color MenuItemBorder => FromKnownColor(KnownColors.msocbvcrCBCtlBdrSelected);

    [SRDescription(nameof(SR.ProfessionalColorsMenuBorderDescr))]
    public virtual Color MenuBorder => FromKnownColor(KnownColors.msocbvcrCBMenuBdrOuter);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientBeginDescr))]
    public virtual Color MenuItemSelectedGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMouseOverBegin);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemSelectedGradientEndDescr))]
    public virtual Color MenuItemSelectedGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMouseOverEnd);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientBeginDescr))]
    public virtual Color MenuItemPressedGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMenuTitleBkgdBegin);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientMiddleDescr))]
    public virtual Color MenuItemPressedGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsMenuItemPressedGradientEndDescr))]
    public virtual Color MenuItemPressedGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMenuTitleBkgdEnd);

    [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientBeginDescr))]
    public virtual Color RaftingContainerGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin);

    [SRDescription(nameof(SR.ProfessionalColorsRaftingContainerGradientEndDescr))]
    public virtual Color RaftingContainerGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd);

    [SRDescription(nameof(SR.ProfessionalColorsSeparatorDarkDescr))]
    public virtual Color SeparatorDark => FromKnownColor(KnownColors.msocbvcrCBSplitterLine);

    [SRDescription(nameof(SR.ProfessionalColorsSeparatorLightDescr))]
    public virtual Color SeparatorLight => FromKnownColor(KnownColors.msocbvcrCBSplitterLineLight);

    [SRDescription(nameof(SR.ProfessionalColorsStatusStripBorderDescr))]
    // Note: the color is retained for backwards compatibility
    public virtual Color StatusStripBorder => Application.SystemColors.ButtonHighlight;

    [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientBeginDescr))]
    public virtual Color StatusStripGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin);

    [SRDescription(nameof(SR.ProfessionalColorsStatusStripGradientEndDescr))]
    public virtual Color StatusStripGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripBorderDescr))]
    public virtual Color ToolStripBorder => FromKnownColor(KnownColors.msocbvcrCBShadow);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripDropDownBackgroundDescr))]
    public virtual Color ToolStripDropDownBackground => FromKnownColor(KnownColors.msocbvcrCBMenuBkgd);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientBeginDescr))]
    public virtual Color ToolStripGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradVertBegin);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientMiddleDescr))]
    public virtual Color ToolStripGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradVertMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripGradientEndDescr))]
    public virtual Color ToolStripGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradVertEnd);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientBeginDescr))]
    public virtual Color ToolStripContentPanelGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripContentPanelGradientEndDescr))]
    public virtual Color ToolStripContentPanelGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientBeginDescr))]
    public virtual Color ToolStripPanelGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzBegin);

    [SRDescription(nameof(SR.ProfessionalColorsToolStripPanelGradientEndDescr))]
    public virtual Color ToolStripPanelGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradMainMenuHorzEnd);

    [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientBeginDescr))]
    public virtual Color OverflowButtonGradientBegin => FromKnownColor(KnownColors.msocbvcrCBGradOptionsBegin);

    [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientMiddleDescr))]
    public virtual Color OverflowButtonGradientMiddle => FromKnownColor(KnownColors.msocbvcrCBGradOptionsMiddle);

    [SRDescription(nameof(SR.ProfessionalColorsOverflowButtonGradientEndDescr))]
    public virtual Color OverflowButtonGradientEnd => FromKnownColor(KnownColors.msocbvcrCBGradOptionsEnd);

    internal Color ComboBoxButtonGradientBegin => MenuItemPressedGradientBegin;
    internal Color ComboBoxButtonGradientEnd => MenuItemPressedGradientEnd;
    internal Color ComboBoxButtonSelectedGradientBegin => MenuItemSelectedGradientBegin;
    internal Color ComboBoxButtonSelectedGradientEnd => MenuItemSelectedGradientEnd;
    internal Color ComboBoxButtonPressedGradientBegin => ButtonPressedGradientBegin;
    internal Color ComboBoxButtonPressedGradientEnd => ButtonPressedGradientEnd;
    internal Color ComboBoxButtonOnOverflow => ToolStripDropDownBackground;
    internal Color ComboBoxBorder => ButtonSelectedHighlightBorder;
    internal Color TextBoxBorder => ButtonSelectedHighlightBorder;

    private static Color GetAlphaBlendedColor(Graphics g, Color src, Color dest, int alpha)
    {
        int red = (src.R * alpha + (255 - alpha) * dest.R) / 255;
        int green = (src.G * alpha + (255 - alpha) * dest.G) / 255;
        int blue = (src.B * alpha + (255 - alpha) * dest.B) / 255;
        int newAlpha = (src.A * alpha + (255 - alpha) * dest.A) / 255;
        if (g is null)
        {
            return Color.FromArgb(newAlpha, red, green, blue);
        }
        else
        {
            return g.FindNearestColor(Color.FromArgb(newAlpha, red, green, blue));
        }
    }

    // this particular method gets us closer to office by increasing the resolution.

    private static Color GetAlphaBlendedColorHighRes(Graphics? graphics, Color src, Color dest, int alpha)
    {
        int sum;
        int nPart2;

        int nPart1 = alpha;
        if (nPart1 < 100)
        {
            nPart2 = 100 - nPart1;
            sum = 100;
        }
        else
        {
            nPart2 = 1000 - nPart1;
            sum = 1000;
        }

        // By adding on sum/2 before dividing by sum, we properly round the value,
        // rather than truncating it, while doing integer math.
        int r = (nPart1 * src.R + nPart2 * dest.R + sum / 2) / sum;
        int g = (nPart1 * src.G + nPart2 * dest.G + sum / 2) / sum;
        int b = (nPart1 * src.B + nPart2 * dest.B + sum / 2) / sum;

        if (graphics is null)
        {
            return Color.FromArgb(r, g, b);
        }

        return graphics.FindNearestColor(Color.FromArgb(r, g, b));
    }

    private static void InitCommonColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        // We need to calculate our own alpha blended color based on the Highlight and Window
        // colors on the system. Since terminalserver + alphablending doesnt work we can't just do a
        // FromARGB here. So we have a simple function which calculates the blending for us.
        if (!DisplayInformation.LowResolution)
        {
            using var screen = GdiCache.GetScreenDCGraphics();
            rgbTable[KnownColors.ButtonPressedHighlight] = GetAlphaBlendedColor(
                screen,
                Application.SystemColors.Window,
                GetAlphaBlendedColor(screen, Application.SystemColors.Highlight, Application.SystemColors.Window, 160),
                50);
            rgbTable[KnownColors.ButtonCheckedHighlight] = GetAlphaBlendedColor(
                screen,
                Application.SystemColors.Window,
                GetAlphaBlendedColor(screen, Application.SystemColors.Highlight, Application.SystemColors.Window, 80),
                20);
            rgbTable[KnownColors.ButtonSelectedHighlight] = rgbTable[KnownColors.ButtonCheckedHighlight];
        }
        else
        {
            rgbTable[KnownColors.ButtonPressedHighlight] = Application.SystemColors.Highlight;
            rgbTable[KnownColors.ButtonCheckedHighlight] = Application.SystemColors.ControlLight;
            rgbTable[KnownColors.ButtonSelectedHighlight] = Application.SystemColors.ControlLight;
        }
    }

    private void InitSystemColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        _usingSystemColors = true;

        InitCommonColors(ref rgbTable);

        // use locals so we aren't fetching again and again.
        Color buttonFace = Application.SystemColors.ButtonFace;
        Color buttonShadow = Application.SystemColors.ButtonShadow;
        Color highlight = Application.SystemColors.Highlight;
        Color window = Application.SystemColors.Window;
        Color empty = Color.Empty;
        Color controlText = Application.SystemColors.ControlText;
        Color buttonHighlight = Application.SystemColors.ButtonHighlight;
        Color grayText = Application.SystemColors.GrayText;
        Color highlightText = Application.SystemColors.HighlightText;
        Color windowText = Application.SystemColors.WindowText;

        // initialize to high contrast
        Color gradientBegin = buttonFace;
        Color gradientMiddle = buttonFace;
        Color gradientEnd = buttonFace;
        Color msocbvcrCBCtlBkgdMouseOver = highlight;
        Color msocbvcrCBCtlBkgdMouseDown = highlight;

        bool lowResolution = DisplayInformation.LowResolution;
        bool highContrast = DisplayInformation.HighContrast;

        if (lowResolution)
        {
            msocbvcrCBCtlBkgdMouseOver = window;
        }
        else if (!highContrast)
        {
            gradientBegin = GetAlphaBlendedColorHighRes(null, buttonFace, window, 23);
            gradientMiddle = GetAlphaBlendedColorHighRes(null, buttonFace, window, 50);
            gradientEnd = Application.SystemColors.ButtonFace;

            msocbvcrCBCtlBkgdMouseOver = GetAlphaBlendedColorHighRes(null, highlight, window, 30);
            msocbvcrCBCtlBkgdMouseDown = GetAlphaBlendedColorHighRes(null, highlight, window, 50);
        }

        if (lowResolution || highContrast)
        {
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBBkgd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Application.SystemColors.ControlLight;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBDragHandle] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMainMenuHorzEnd] = buttonFace;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsBegin] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradOptionsMiddle] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = buttonShadow;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBdrOuter] = controlText;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBMenuBkgd] = window;
            rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLine] = buttonShadow;
        }
        else
        {
            rgbTable[KnownColors.msocbvcrCBBkgd] = GetAlphaBlendedColorHighRes(null, window, buttonFace, 165);
            rgbTable[KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = GetAlphaBlendedColorHighRes(null, highlight, window, 50);
            rgbTable[KnownColors.msocbvcrCBDragHandle] = GetAlphaBlendedColorHighRes(null, buttonShadow, window, 75);
            rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzEnd] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 205);
            rgbTable[KnownColors.msocbvcrCBGradOptionsBegin] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 70);
            rgbTable[KnownColors.msocbvcrCBGradOptionsMiddle] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 90);
            rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 40);
            rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 70);
            rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 90);
            rgbTable[KnownColors.msocbvcrCBMenuBdrOuter] = GetAlphaBlendedColorHighRes(null, controlText, buttonShadow, 20);
            rgbTable[KnownColors.msocbvcrCBMenuBkgd] = GetAlphaBlendedColorHighRes(null, buttonFace, window, 143);
            rgbTable[KnownColors.msocbvcrCBSplitterLine] = GetAlphaBlendedColorHighRes(null, buttonShadow, window, 70);
        }

        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBCtlBkgdSelected] = (lowResolution) ? Application.SystemColors.ControlLight : highlight;

        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = buttonFace;
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBBdrOuterFloating] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseDown] = highlight;

        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseOver] = highlight;
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelected] = highlight;
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = highlight;
        rgbTable[KnownColors.msocbvcrCBCtlBkgd] = empty;
        rgbTable[KnownColors.msocbvcrCBCtlBkgdLight] = window;
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseDown] = highlight;
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = window;
        rgbTable[KnownColors.msocbvcrCBCtlText] = controlText;
        rgbTable[KnownColors.msocbvcrCBCtlTextDisabled] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBCtlTextLight] = grayText;
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseDown] = highlightText;
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = windowText;
        rgbTable[KnownColors.msocbvcrCBDockSeparatorLine] = empty;

        rgbTable[KnownColors.msocbvcrCBDragHandleShadow] = window;
        rgbTable[KnownColors.msocbvcrCBDropDownArrow] = empty;

        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzBegin] = buttonFace;

        rgbTable[KnownColors.msocbvcrCBGradMouseOverEnd] = msocbvcrCBCtlBkgdMouseOver;
        rgbTable[KnownColors.msocbvcrCBGradMouseOverBegin] = msocbvcrCBCtlBkgdMouseOver;
        rgbTable[KnownColors.msocbvcrCBGradMouseOverMiddle] = msocbvcrCBCtlBkgdMouseOver;

        rgbTable[KnownColors.msocbvcrCBGradOptionsEnd] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = empty;
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = empty;
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = empty;
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedBegin] = empty;
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedEnd] = empty;
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = empty;
        rgbTable[KnownColors.msocbvcrCBGradSelectedBegin] = empty;
        rgbTable[KnownColors.msocbvcrCBGradSelectedEnd] = empty;
        rgbTable[KnownColors.msocbvcrCBGradSelectedMiddle] = empty;

        rgbTable[KnownColors.msocbvcrCBGradVertBegin] = gradientBegin;
        rgbTable[KnownColors.msocbvcrCBGradVertMiddle] = gradientMiddle;
        rgbTable[KnownColors.msocbvcrCBGradVertEnd] = gradientEnd;

        rgbTable[KnownColors.msocbvcrCBGradMouseDownBegin] = msocbvcrCBCtlBkgdMouseDown;
        rgbTable[KnownColors.msocbvcrCBGradMouseDownMiddle] = msocbvcrCBCtlBkgdMouseDown;
        rgbTable[KnownColors.msocbvcrCBGradMouseDownEnd] = msocbvcrCBCtlBkgdMouseDown;

        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = gradientBegin;
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = gradientMiddle;

        rgbTable[KnownColors.msocbvcrCBIconDisabledDark] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBIconDisabledLight] = buttonFace;
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBLowColorIconDisabled] = empty;
        rgbTable[KnownColors.msocbvcrCBMainMenuBkgd] = buttonFace;

        rgbTable[KnownColors.msocbvcrCBMenuCtlText] = windowText;
        rgbTable[KnownColors.msocbvcrCBMenuCtlTextDisabled] = grayText;
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgd] = empty;
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBMenuShadow] = empty;
        rgbTable[KnownColors.msocbvcrCBMenuSplitArrow] = buttonShadow;
        rgbTable[KnownColors.msocbvcrCBOptionsButtonShadow] = empty;

        rgbTable[KnownColors.msocbvcrCBShadow] = rgbTable[KnownColors.msocbvcrCBBkgd];

        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBSplitterLineLight] = buttonHighlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandle] = empty;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTearOffHandleMouseOver] = empty;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleBkgd] = buttonShadow;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrCBTitleText] = buttonHighlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledFocuslessHighlightedText] = grayText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDisabledHighlightedText] = grayText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDlgGroupBoxText] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdr] = buttonShadow;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDark] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseDown] = highlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Application.SystemColors.MenuText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLight] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseDown] = highlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrLightMouseOver] = Application.SystemColors.MenuText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseDown] = highlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrMouseOver] = Application.SystemColors.MenuText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBdrSelected] = buttonShadow;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgd] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseDown] = highlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdMouseOver] = highlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabBkgdSelected] = window;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabText] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseDown] = highlightText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextMouseOver] = highlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDocTabTextSelected] = windowText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabBkgd] = buttonShadow;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabText] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = buttonShadow;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWActiveTabTextDisabled] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabBkgd] = buttonShadow;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = buttonHighlight;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWInactiveTabText] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseDown] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabBkgdMouseOver] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseDown] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrDWTabTextMouseOver] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = buttonFace;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedBkgd] = Application.SystemColors.InactiveCaption;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = controlText;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrFocuslessHighlightedText] = Application.SystemColors.InactiveCaptionText;
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
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipBkgd] = Application.SystemColors.Info;
        rgbTable[ProfessionalColorTable.KnownColors.msocbvcrWPInfoTipText] = Application.SystemColors.InfoText;
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

    private static void InitOliveLunaColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(81, 94, 51);
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(81, 94, 51);
        rgbTable[KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(116, 134, 94);
        rgbTable[KnownColors.msocbvcrCBBkgd] = Color.FromArgb(209, 222, 173);
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(209, 222, 173);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(255, 192, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(141, 141, 141);
        rgbTable[KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(96, 119, 66);
        rgbTable[KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(81, 94, 51);
        rgbTable[KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(217, 217, 167);
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(242, 241, 228);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(230, 230, 209);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(160, 177, 116);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(186, 201, 143);
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(237, 240, 214);
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(181, 196, 143);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(255, 223, 154);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(255, 177, 109);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(255, 255, 222);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(255, 203, 136);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(255, 225, 172);
        rgbTable[KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(186, 204, 150);
        rgbTable[KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(96, 119, 107);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(141, 160, 107);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(255, 255, 222);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(255, 193, 118);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(255, 225, 172);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(254, 140, 73);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(255, 221, 152);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(255, 184, 116);
        rgbTable[KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(255, 223, 154);
        rgbTable[KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(255, 166, 76);
        rgbTable[KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(255, 195, 116);
        rgbTable[KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(255, 255, 237);
        rgbTable[KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(181, 196, 143);
        rgbTable[KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(206, 220, 167);
        rgbTable[KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(131, 144, 113);
        rgbTable[KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(243, 244, 240);
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(218, 227, 187);
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(218, 227, 187);
        rgbTable[KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(159, 174, 122);
        rgbTable[KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(117, 141, 94);
        rgbTable[KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(244, 244, 238);
        rgbTable[KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(141, 141, 141);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(216, 227, 182);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(173, 181, 157);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(173, 181, 157);
        rgbTable[KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(134, 148, 108);
        rgbTable[KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBShadow] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(244, 247, 222);
        rgbTable[KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(197, 212, 159);
        rgbTable[KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(116, 134, 94);
        rgbTable[KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(220, 224, 208);
        rgbTable[KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(153, 84, 10);
        rgbTable[KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(96, 119, 107);
        rgbTable[KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(176, 194, 140);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(63, 93, 56);
        rgbTable[KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(218, 227, 187);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDocTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(218, 227, 187);
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(218, 227, 187);
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(183, 198, 145);
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(183, 198, 145);
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(191, 191, 223);
        rgbTable[KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(239, 235, 222);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(126, 125, 104);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(239, 235, 222);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 192, 111);
        rgbTable[KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(159, 171, 128);
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(159, 171, 128);
        rgbTable[KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(217, 227, 187);
        rgbTable[KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(230, 234, 208);
        rgbTable[KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(150, 145, 133);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(161, 176, 128);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(210, 223, 174);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(90, 107, 70);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(90, 107, 70);
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(0, 61, 178);
        rgbTable[KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(243, 242, 231);
        rgbTable[KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);
        rgbTable[KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(217, 217, 167);
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(217, 217, 167);
        rgbTable[KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(242, 241, 228);
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(242, 241, 228);
        rgbTable[KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(255, 255, 237);
        rgbTable[KnownColors.msocbvcrOABBkgd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(211, 211, 211);
        rgbTable[KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(151, 160, 123);
        rgbTable[KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(226, 231, 191);
        rgbTable[KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(171, 192, 138);
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(117, 141, 94);
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOGRulerText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(218, 227, 187);
        rgbTable[KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(151, 160, 123);
        rgbTable[KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(242, 240, 228);
        rgbTable[KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(96, 119, 66);
        rgbTable[KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(175, 192, 130);
        rgbTable[KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(234, 233, 225);
        rgbTable[KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(181, 196, 143);
        rgbTable[KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(253, 238, 201);
        rgbTable[KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(175, 186, 145);
        rgbTable[KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(115, 137, 84);
        rgbTable[KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(253, 247, 233);
        rgbTable[KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(151, 160, 123);
        rgbTable[KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(151, 160, 123);
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(187, 85, 3);
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(251, 200, 79);
        rgbTable[KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(200, 212, 172);
        rgbTable[KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(176, 191, 138);
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(234, 240, 207);
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(234, 240, 207);
        rgbTable[KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(247, 190, 87);
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 255, 220);
        rgbTable[KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(50, 69, 105);
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(248, 222, 128);
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(232, 127, 8);
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(238, 147, 17);
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(251, 230, 148);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(64, 81, 59);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(120, 142, 111);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(120, 142, 111);
        rgbTable[KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(242, 240, 228);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(96, 128, 88);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(206, 220, 167);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(107, 129, 107);
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(107, 129, 107);
        rgbTable[KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(107, 129, 107);
        rgbTable[KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(151, 160, 123);
        rgbTable[KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(193, 198, 176);
        rgbTable[KnownColors.msocbvcrSBBdr] = Color.FromArgb(211, 211, 211);
        rgbTable[KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(249, 249, 247);
        rgbTable[KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(237, 242, 212);
        rgbTable[KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(191, 206, 153);
        rgbTable[KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(242, 241, 228);
        rgbTable[KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(116, 134, 94);
        rgbTable[KnownColors.msocbvcrWPBkgd] = Color.FromArgb(243, 242, 231);
        rgbTable[KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(164, 185, 127);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(197, 212, 159);
        rgbTable[KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(222, 222, 222);
        rgbTable[KnownColors.msocbvcrWPCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPGroupline] = Color.FromArgb(188, 187, 177);
        rgbTable[KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 255, 204);
        rgbTable[KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(116, 134, 94);
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(216, 227, 182);
        rgbTable[KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(188, 205, 131);
        rgbTable[KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(217, 217, 167);
    }

    private static void InitSilverLunaColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(173, 174, 193);
        rgbTable[KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(122, 121, 153);
        rgbTable[KnownColors.msocbvcrCBBkgd] = Color.FromArgb(219, 218, 228);
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(219, 218, 228);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(255, 192, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(141, 141, 141);
        rgbTable[KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(110, 109, 143);
        rgbTable[KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(84, 84, 117);
        rgbTable[KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(224, 223, 227);
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(215, 215, 229);
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(243, 243, 247);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(215, 215, 226);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(118, 116, 151);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(184, 185, 202);
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(232, 233, 242);
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(172, 170, 194);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(255, 223, 154);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(255, 177, 109);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(255, 255, 222);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(255, 203, 136);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(255, 225, 172);
        rgbTable[KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(186, 185, 206);
        rgbTable[KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(118, 116, 146);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(156, 155, 180);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(255, 255, 222);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(255, 193, 118);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(255, 225, 172);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(254, 140, 73);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(255, 221, 152);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(255, 184, 116);
        rgbTable[KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(255, 223, 154);
        rgbTable[KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(255, 166, 76);
        rgbTable[KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(255, 195, 116);
        rgbTable[KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(249, 249, 255);
        rgbTable[KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(147, 145, 176);
        rgbTable[KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(225, 226, 236);
        rgbTable[KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(122, 121, 153);
        rgbTable[KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(247, 245, 249);
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(212, 212, 226);
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(212, 212, 226);
        rgbTable[KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(168, 167, 190);
        rgbTable[KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(198, 200, 215);
        rgbTable[KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(253, 250, 255);
        rgbTable[KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(141, 141, 141);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(214, 211, 231);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(185, 187, 200);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(185, 187, 200);
        rgbTable[KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(154, 140, 176);
        rgbTable[KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBShadow] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(110, 109, 143);
        rgbTable[KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(192, 192, 211);
        rgbTable[KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(122, 121, 153);
        rgbTable[KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(59, 59, 63);
        rgbTable[KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(7, 70, 213);
        rgbTable[KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(118, 116, 146);
        rgbTable[KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(186, 185, 206);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(75, 75, 111);
        rgbTable[KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(212, 212, 226);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDocTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(212, 212, 226);
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(212, 212, 226);
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(148, 148, 148);
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(148, 148, 148);
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(171, 169, 194);
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(171, 169, 194);
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(224, 223, 227);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(224, 223, 227);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(191, 191, 223);
        rgbTable[KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(239, 235, 222);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(126, 125, 104);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(223, 223, 234);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 192, 111);
        rgbTable[KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(162, 162, 181);
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(162, 162, 181);
        rgbTable[KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(212, 213, 229);
        rgbTable[KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(227, 227, 236);
        rgbTable[KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(150, 145, 133);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(169, 168, 191);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(208, 208, 223);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(92, 91, 121);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(92, 91, 121);
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(0, 61, 178);
        rgbTable[KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(238, 238, 244);
        rgbTable[KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);
        rgbTable[KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(215, 215, 229);
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(215, 215, 229);
        rgbTable[KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(243, 243, 247);
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(243, 243, 247);
        rgbTable[KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(249, 249, 255);
        rgbTable[KnownColors.msocbvcrOABBkgd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(211, 211, 211);
        rgbTable[KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(155, 154, 179);
        rgbTable[KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(223, 223, 234);
        rgbTable[KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(177, 176, 195);
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOGRulerText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(212, 212, 226);
        rgbTable[KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(155, 154, 179);
        rgbTable[KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(239, 239, 244);
        rgbTable[KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(110, 109, 143);
        rgbTable[KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(168, 167, 191);
        rgbTable[KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(234, 233, 225);
        rgbTable[KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(165, 164, 189);
        rgbTable[KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(253, 238, 201);
        rgbTable[KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(229, 229, 235);
        rgbTable[KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(112, 111, 145);
        rgbTable[KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(253, 247, 233);
        rgbTable[KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(155, 154, 179);
        rgbTable[KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(155, 154, 179);
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(187, 85, 3);
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(251, 200, 79);
        rgbTable[KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(204, 206, 219);
        rgbTable[KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(147, 145, 176);
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(225, 226, 236);
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(225, 226, 236);
        rgbTable[KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(247, 190, 87);
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 255, 220);
        rgbTable[KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(50, 69, 105);
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(248, 222, 128);
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(232, 127, 8);
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(238, 147, 17);
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(251, 230, 148);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(110, 109, 143);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(168, 167, 191);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(168, 167, 191);
        rgbTable[KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(224, 223, 227);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(243, 243, 247);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(124, 124, 148);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(215, 215, 229);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(142, 142, 170);
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(142, 142, 170);
        rgbTable[KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(142, 142, 170);
        rgbTable[KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(155, 154, 179);
        rgbTable[KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(195, 195, 210);
        rgbTable[KnownColors.msocbvcrSBBdr] = Color.FromArgb(236, 234, 218);
        rgbTable[KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(247, 247, 249);
        rgbTable[KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(239, 239, 247);
        rgbTable[KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(179, 178, 204);
        rgbTable[KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(243, 243, 247);
        rgbTable[KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(122, 121, 153);
        rgbTable[KnownColors.msocbvcrWPBkgd] = Color.FromArgb(238, 238, 244);
        rgbTable[KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(165, 172, 178);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(192, 192, 211);
        rgbTable[KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(222, 222, 222);
        rgbTable[KnownColors.msocbvcrWPCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPGroupline] = Color.FromArgb(161, 160, 187);
        rgbTable[KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 255, 204);
        rgbTable[KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(122, 121, 153);
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(184, 188, 234);
        rgbTable[KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(198, 198, 217);
        rgbTable[KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(215, 215, 229);
    }

    private static void InitRoyaleColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        rgbTable[KnownColors.msocbvcrCBBkgd] = Color.FromArgb(238, 237, 240); // msocbvcrCBBkgd
        rgbTable[KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(189, 188, 191); // msocbvcrCBDragHandle -> Needs to equal VSCOLOR_COMMANDBAR_DRAGHANDLE in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(193, 193, 196); // msocbvcrCBSplitterLine
        rgbTable[KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(167, 166, 170); // msocbvcrCBTitleBkgd
        rgbTable[KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255); // msocbvcrCBTitleText
        rgbTable[KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(142, 141, 145); // msocbvcrCBBdrOuterFloating
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(235, 233, 237); // msocbvcrCBBdrOuterDocked
        rgbTable[KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(238, 237, 240); // msocbvcrCBTearOffHandle
        rgbTable[KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(194, 207, 229); // msocbvcrCBTearOffHandleMouseOver
        rgbTable[KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(238, 237, 240); // msocbvcrCBCtlBkgd
        rgbTable[KnownColors.msocbvcrCBCtlText] = Color.FromArgb(000, 000, 000); // msocbvcrCBCtlText
        rgbTable[KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(176, 175, 179); // msocbvcrCBCtlTextDisabled
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(194, 207, 229); // msocbvcrCBCtlBkgdMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_HOVER in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(51, 94, 168);  // msocbvcrCBCtlBdrMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_BORDER in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(000, 000, 000); // msocbvcrCBCtlTextMouseOver
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(153, 175, 212); // msocbvcrCBCtlBkgdMouseDown -> Needs to equal VSCOLOR_COMMANDBAR_HOVEROVERSELECTED in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(51, 94, 168);   // msocbvcrCBCtlBdrMouseDown
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(255, 255, 255); // msocbvcrCBCtlTextMouseDown
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(226, 229, 238); // msocbvcrCBCtlBkgdSelected -> Needs to equal VSCOLOR_COMMANDBAR_SELECTED in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(51, 94, 168);  // msocbvcrCBCtlBdrSelected
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(51, 94, 168);  // msocbvcrCBCtlBkgdSelectedMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_HOVEROVERSELECTEDICON in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(51, 94, 168);   // msocbvcrCBCtlBdrSelectedMouseOver -> Needs to equal VSCOLOR_COMMANDBAR_HOVEROVERSELECTEDICON_BORDER in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255); // msocbvcrCBCtlBkgdLight
        rgbTable[KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(167, 166, 170); // msocbvcrCBCtlTextLight
        rgbTable[KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(235, 233, 237); // msocbvcrCBMainMenuBkgd
        rgbTable[KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(252, 252, 252); // msocbvcrCBMenuBkgd
        rgbTable[KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0); // msocbvcrCBMenuCtlText
        rgbTable[KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(193, 193, 196); // msocbvcrCBMenuCtlTextDisabled
        rgbTable[KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(134, 133, 136); // msocbvcrCBMenuBdrOuter
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(238, 237, 240); // msocbvcrCBMenuIconBkgd
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(228, 226, 230); // msocbvcrCBMenuIconBkgdDropped
        rgbTable[KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(167, 166, 170); // msocbvcrCBMenuSplitArrow
        rgbTable[KnownColors.msocbvcrWPBkgd] = Color.FromArgb(245, 244, 246); // msocbvcrWPBkgd
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(255, 51, 153);  // msocbvcrWPText
        rgbTable[KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(255, 51, 153);  // msocbvcrWPTitleBkgdActive
        rgbTable[KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(255, 51, 153);  // msocbvcrWPTitleBkgdInactive
        rgbTable[KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(255, 51, 153);  // msocbvcrWPTitleTextActive
        rgbTable[KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(255, 51, 153);  // msocbvcrWPTitleTextInactive
        rgbTable[KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(255, 51, 153);  // msocbvcrWPBdrOuterFloating
        rgbTable[KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(255, 51, 153);  // msocbvcrWPBdrOuterDocked
        rgbTable[KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlBdr
        rgbTable[KnownColors.msocbvcrWPCtlText] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlText
        rgbTable[KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlBkgd
        rgbTable[KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlBdrDisabled
        rgbTable[KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlTextDisabled
        rgbTable[KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlBkgdDisabled
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlBdrDefault
        rgbTable[KnownColors.msocbvcrWPGroupline] = Color.FromArgb(255, 51, 153);  // msocbvcrWPGroupline
        rgbTable[KnownColors.msocbvcrSBBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrSBBdr
        rgbTable[KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrOBBkgdBdr
        rgbTable[KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(255, 51, 153);  // msocbvcrOBBkgdBdrContrast
        rgbTable[KnownColors.msocbvcrOABBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOABBkgd
        rgbTable[KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGDHeaderBkgd
        rgbTable[KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrGDHeaderBdr
        rgbTable[KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrGDHeaderCellBdr
        rgbTable[KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(255, 51, 153);  // msocbvcrGDHeaderSeeThroughSelection
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGDHeaderCellBkgd
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 51, 153);  // msocbvcrGDHeaderCellBkgdSelected
        rgbTable[KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(255, 255, 255); // msocbvcrCBSplitterLineLight
        rgbTable[KnownColors.msocbvcrCBShadow] = Color.FromArgb(238, 237, 240); // msocbvcrCBShadow -> Needs to equal VSCOLOR_COMMANDBAR_SHADOW in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(245, 244, 246); // msocbvcrCBOptionsButtonShadow
        rgbTable[KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(193, 193, 196); // msocbvcrWPNavBarBkgnd
        rgbTable[KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(245, 244, 246);  // msocbvcrWPBdrInnerDocked
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(235, 233, 237); // msocbvcrCBLabelBkgnd
        rgbTable[KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(235, 233, 237); // msocbvcrCBIconDisabledLight
        rgbTable[KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(167, 166, 170); // msocbvcrCBIconDisabledDark
        rgbTable[KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(176, 175, 179); // msocbvcrCBLowColorIconDisabled
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(235, 233, 237); // msocbvcrCBGradMainMenuHorzBegin -> Needs to equal VSCOLOR_ENVIRONMENT_BACKGROUND and VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTBEGIN in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(251, 250, 251); // msocbvcrCBGradMainMenuHorzEnd -> Needs to equal VSCOLOR_ENVIRONMENT_BACKGROUND_GRADIENTEND in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(252, 252, 252); // msocbvcrCBGradVertBegin -> Needs to equal VSCOLOR_COMMANDBAR_GRADIENT_BEGIN in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(245, 244, 246); // msocbvcrCBGradVertMiddle -> Needs to equal VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(235, 233, 237); // msocbvcrCBGradVertEnd -> Needs to equal VSCOLOR_COMMANDBAR_GRADIENT_END in vscolors.cpp
        rgbTable[KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(242, 242, 242); // msocbvcrCBGradOptionsBegin
        rgbTable[KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(224, 224, 225); // msocbvcrCBGradOptionsMiddle
        rgbTable[KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(167, 166, 170); // msocbvcrCBGradOptionsEnd
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(252, 252, 252); // msocbvcrCBGradMenuTitleBkgdBegin
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(245, 244, 246); // msocbvcrCBGradMenuTitleBkgdEnd
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(247, 246, 248); // msocbvcrCBGradMenuIconBkgdDroppedBegin
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(241, 240, 242); // msocbvcrCBGradMenuIconBkgdDroppedMiddle
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(228, 226, 230); // msocbvcrCBGradMenuIconBkgdDroppedEnd
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(226, 229, 238); // msocbvcrCBGradOptionsSelectedBegin
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(226, 229, 238); // msocbvcrCBGradOptionsSelectedMiddle
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(226, 229, 238); // msocbvcrCBGradOptionsSelectedEnd
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(194, 207, 229); // msocbvcrCBGradOptionsMouseOverBegin
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(194, 207, 229); // msocbvcrCBGradOptionsMouseOverMiddle
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(194, 207, 229); // msocbvcrCBGradOptionsMouseOverEnd
        rgbTable[KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(226, 229, 238); // msocbvcrCBGradSelectedBegin
        rgbTable[KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(226, 229, 238); // msocbvcrCBGradSelectedMiddle
        rgbTable[KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(226, 229, 238); // msocbvcrCBGradSelectedEnd
        rgbTable[KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(194, 207, 229); // msocbvcrCBGradMouseOverBegin
        rgbTable[KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(194, 207, 229); // msocbvcrCBGradMouseOverMiddle
        rgbTable[KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(194, 207, 229); // msocbvcrCBGradMouseOverEnd
        rgbTable[KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(153, 175, 212); // msocbvcrCBGradMouseDownBegin
        rgbTable[KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(153, 175, 212); // msocbvcrCBGradMouseDownMiddle
        rgbTable[KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(153, 175, 212); // msocbvcrCBGradMouseDownEnd
        rgbTable[KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(235, 233, 237); // msocbvcrNetLookBkgnd
        rgbTable[KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(000, 000, 000); // msocbvcrCBMenuShadow
        rgbTable[KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(51, 94, 168);  // msocbvcrCBDockSeparatorLine
        rgbTable[KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(235, 233, 237); // msocbvcrCBDropDownArrow
        rgbTable[KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKGridlines
        rgbTable[KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKGroupText
        rgbTable[KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKGroupLine
        rgbTable[KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKGroupShaded
        rgbTable[KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKGroupNested
        rgbTable[KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKIconBar
        rgbTable[KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKFlagNone
        rgbTable[KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKFolderbarLight
        rgbTable[KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKFolderbarDark
        rgbTable[KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKFolderbarText
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBButtonLight
        rgbTable[KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBButtonDark
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBSelectedButtonLight
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBSelectedButtonDark
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBHoverButtonLight
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBHoverButtonDark
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBPressedButtonLight
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBPressedButtonDark
        rgbTable[KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBDarkOutline
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBSplitterLight
        rgbTable[KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBSplitterDark
        rgbTable[KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBActionDividerLine
        rgbTable[KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBLabelText
        rgbTable[KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKWBFoldersBackground
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKTodayIndicatorLight
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKTodayIndicatorDark
        rgbTable[KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKInfoBarBkgd
        rgbTable[KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKInfoBarText
        rgbTable[KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(255, 51, 153);  // msocbvcrOLKPreviewPaneLabelText
        rgbTable[KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);    // msocbvcrHyperlink
        rgbTable[KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);   // msocbvcrHyperlinkFollowed
        rgbTable[KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOGWorkspaceBkgd
        rgbTable[KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOGMDIParentWorkspaceBkgd
        rgbTable[KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerBkgd
        rgbTable[KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerActiveBkgd
        rgbTable[KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerInactiveBkgd
        rgbTable[KnownColors.msocbvcrOGRulerText] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerText
        rgbTable[KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerTabStopTicks
        rgbTable[KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerBdr
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerTabBoxBdr
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 51, 153);  // msocbvcrOGRulerTabBoxBdrHighlight
        rgbTable[KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrXLFormulaBarBkgd
        rgbTable[KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255); // msocbvcrCBDragHandleShadow -> Needs to equal VSCOLOR_COMMANDBAR_DRAGHANDLE_SHADOW in vscolors.cpp
        rgbTable[KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrOGTaskPaneGroupBoxHeaderBkgd
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(255, 51, 153);  // msocbvcrPPOutlineThumbnailsPaneTabBdr
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(255, 51, 153);  // msocbvcrPPOutlineThumbnailsPaneTabText
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(255, 51, 153);  // msocbvcrPPSlideBdrActiveSelected
        rgbTable[KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(255, 51, 153);  // msocbvcrPPSlideBdrInactiveSelected
        rgbTable[KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(255, 51, 153);  // msocbvcrPPSlideBdrMouseOver
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(255, 51, 153);  // msocbvcrPPSlideBdrActiveSelectedMouseOver
        rgbTable[KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(0, 0, 0);    // msocbvcrDlgGroupBoxText
        rgbTable[KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(237, 235, 239); // msocbvcrScrollbarBkgd
        rgbTable[KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(155, 154, 156); // msocbvcrListHeaderArrow
        rgbTable[KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(188, 202, 226); // msocbvcrDisabledHighlightedText
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(235, 233, 237); // msocbvcrFocuslessHighlightedBkgd
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(000, 000, 000); // msocbvcrFocuslessHighlightedText
        rgbTable[KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(167, 166, 170); // msocbvcrDisabledFocuslessHighlightedText
        rgbTable[KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(255, 51, 153);  // msocbvcrWPCtlTextMouseDown
        rgbTable[KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(255, 51, 153);  // msocbvcrWPTextDisabled
        rgbTable[KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrWPInfoTipBkgd
        rgbTable[KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(255, 51, 153);  // msocbvcrWPInfoTipText
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrDWActiveTabBkgd
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(255, 51, 153);  // msocbvcrDWActiveTabText
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(255, 51, 153);  // msocbvcrDWActiveTabTextDisabled
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrDWInactiveTabBkgd
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 51, 153);  // msocbvcrDWInactiveTabText
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 51, 153);  // msocbvcrDWTabBkgdMouseOver
        rgbTable[KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(255, 51, 153);  // msocbvcrDWTabTextMouseOver
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(255, 51, 153);  // msocbvcrDWTabBkgdMouseDown
        rgbTable[KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(255, 51, 153);  // msocbvcrDWTabTextMouseDown
        rgbTable[KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPLightBkgd
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPDarkBkgd
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupHeaderLightBkgd
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupHeaderDarkBkgd
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupHeaderText
        rgbTable[KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupContentLightBkgd
        rgbTable[KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupContentDarkBkgd
        rgbTable[KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupContentText
        rgbTable[KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupContentTextDisabled
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPGroupline
        rgbTable[KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(255, 51, 153);  // msocbvcrGSPHyperlink
        rgbTable[KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(212, 212, 226); // msocbvcrDocTabBkgd
        rgbTable[KnownColors.msocbvcrDocTabText] = Color.FromArgb(000, 000, 000); // msocbvcrDocTabText
        rgbTable[KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(118, 116, 146); // msocbvcrDocTabBdr
        rgbTable[KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255); // msocbvcrDocTabBdrLight
        rgbTable[KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(186, 185, 206); // msocbvcrDocTabBdrDark
        rgbTable[KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255); // msocbvcrDocTabBkgdSelected
        rgbTable[KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(000, 000, 000); // msocbvcrDocTabTextSelected
        rgbTable[KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(124, 124, 148); // msocbvcrDocTabBdrSelected
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(193, 210, 238); // msocbvcrDocTabBkgdMouseOver
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(49, 106, 197);  // msocbvcrDocTabTextMouseOver
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(49, 106, 197);  // msocbvcrDocTabBdrMouseOver
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(49, 106, 197);  // msocbvcrDocTabBdrLightMouseOver
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(49, 106, 197);  // msocbvcrDocTabBdrDarkMouseOver
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(154, 183, 228); // msocbvcrDocTabBkgdMouseDown
        rgbTable[KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(000, 000, 000); // msocbvcrDocTabTextMouseDown
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(75, 75, 111);   // msocbvcrDocTabBdrMouseDown
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(75, 75, 111);   // msocbvcrDocTabBdrLightMouseDown
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(75, 75, 111);   // msocbvcrDocTabBdrDarkMouseDown
        rgbTable[KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(246, 244, 236); // msocbvcrToastGradBegin
        rgbTable[KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(179, 178, 204); // msocbvcrToastGradEnd
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(236, 233, 216); // msocbvcrJotNavUIGradBegin
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(236, 233, 216); // msocbvcrJotNavUIGradMiddle
        rgbTable[KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255); // msocbvcrJotNavUIGradEnd
        rgbTable[KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(000, 000, 000); // msocbvcrJotNavUIText
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(172, 168, 153); // msocbvcrJotNavUIBdr
        rgbTable[KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(224, 223, 227); // msocbvcrPlacesBarBkgd
        rgbTable[KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(152, 181, 226); // msocbvcrPubPrintDocScratchPageBkgd
        rgbTable[KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(193, 210, 238); // msocbvcrPubWebDocScratchPageBkgd
    }

    private void InitThemedColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        string colorScheme = VisualStyleInformation.ColorScheme;
        string themeFileName = Path.GetFileName(VisualStyleInformation.ThemeFilename);
        bool initializedTable = false;

        // VS compares the filename of the theme to determine luna v. royale.
        if (string.Equals(LunaFileName, themeFileName, StringComparison.OrdinalIgnoreCase))
        {
            // once we know it's luna we've got to pick between
            // normal (blue) homestead (olive) and metallic (silver)
            if (colorScheme == NormalColorScheme)
            {
                InitBlueLunaColors(ref rgbTable);
                _usingSystemColors = false;
                initializedTable = true;
            }
            else if (colorScheme == OliveColorScheme)
            {
                InitOliveLunaColors(ref rgbTable);
                _usingSystemColors = false;
                initializedTable = true;
            }
            else if (colorScheme == SilverColorScheme)
            {
                InitSilverLunaColors(ref rgbTable);
                _usingSystemColors = false;
                initializedTable = true;
            }
        }
        else if (string.Equals(AeroFileName, themeFileName, StringComparison.OrdinalIgnoreCase))
        {
            // On Vista running Aero theme, Office looks like it's using SystemColors
            // With the exception of the MenuItemSelected Color for MenuStrip items that
            // are contained in DropDowns.  We're going to copy their behavior
            InitSystemColors(ref rgbTable);
            _usingSystemColors = true;
            initializedTable = true;

            // Exception to SystemColors, use the ButtonSelectedHighlight color otherwise
            // the background for DropDown MenuStrip items will have no contrast
            rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = rgbTable[KnownColors.ButtonSelectedHighlight];

            // CheckedBackground of ToolStripMenuItem
            rgbTable[KnownColors.msocbvcrCBCtlBkgdSelected] = rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver];
        }
        else if (string.Equals(RoyaleFileName, themeFileName, StringComparison.OrdinalIgnoreCase))
        {
            // once we know it's royale (TabletPC/MCE) we know about two color scheme names
            // which should do exactly the same thing
            if (colorScheme is NormalColorScheme or RoyaleColorScheme)
            {
                InitRoyaleColors(ref rgbTable);
                _usingSystemColors = false;
                initializedTable = true;
            }
        }

        if (!initializedTable)
        {
            // unknown color scheme - bailing
            InitSystemColors(ref rgbTable);
            _usingSystemColors = true;
        }

        InitCommonColors(ref rgbTable);
    }

    private static void InitBlueLunaColors(ref Dictionary<KnownColors, Color> rgbTable)
    {
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(196, 205, 218);
        rgbTable[KnownColors.msocbvcrCBBdrOuterDocked] = Color.FromArgb(196, 205, 218);
        rgbTable[KnownColors.msocbvcrCBBdrOuterFloating] = Color.FromArgb(42, 102, 201);
        rgbTable[KnownColors.msocbvcrCBBkgd] = Color.FromArgb(196, 219, 249);
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseDown] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrCBCtlBdrMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelected] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrCBCtlBdrSelectedMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrCBCtlBkgd] = Color.FromArgb(196, 219, 249);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelected] = Color.FromArgb(255, 192, 111);
        rgbTable[KnownColors.msocbvcrCBCtlBkgdSelectedMouseOver] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextDisabled] = Color.FromArgb(141, 141, 141);
        rgbTable[KnownColors.msocbvcrCBCtlTextLight] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBCtlTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBDockSeparatorLine] = Color.FromArgb(0, 53, 145);
        rgbTable[KnownColors.msocbvcrCBDragHandle] = Color.FromArgb(39, 65, 118);
        rgbTable[KnownColors.msocbvcrCBDragHandleShadow] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBDropDownArrow] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzBegin] = Color.FromArgb(158, 190, 245);
        rgbTable[KnownColors.msocbvcrCBGradMainMenuHorzEnd] = Color.FromArgb(196, 218, 250);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedBegin] = Color.FromArgb(203, 221, 246);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedEnd] = Color.FromArgb(114, 155, 215);
        rgbTable[KnownColors.msocbvcrCBGradMenuIconBkgdDroppedMiddle] = Color.FromArgb(161, 197, 249);
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdBegin] = Color.FromArgb(227, 239, 255);
        rgbTable[KnownColors.msocbvcrCBGradMenuTitleBkgdEnd] = Color.FromArgb(123, 164, 224);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownBegin] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownEnd] = Color.FromArgb(255, 223, 154);
        rgbTable[KnownColors.msocbvcrCBGradMouseDownMiddle] = Color.FromArgb(255, 177, 109);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverBegin] = Color.FromArgb(255, 255, 222);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverEnd] = Color.FromArgb(255, 203, 136);
        rgbTable[KnownColors.msocbvcrCBGradMouseOverMiddle] = Color.FromArgb(255, 225, 172);
        rgbTable[KnownColors.msocbvcrCBGradOptionsBegin] = Color.FromArgb(127, 177, 250);
        rgbTable[KnownColors.msocbvcrCBGradOptionsEnd] = Color.FromArgb(0, 53, 145);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMiddle] = Color.FromArgb(82, 127, 208);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverBegin] = Color.FromArgb(255, 255, 222);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverEnd] = Color.FromArgb(255, 193, 118);
        rgbTable[KnownColors.msocbvcrCBGradOptionsMouseOverMiddle] = Color.FromArgb(255, 225, 172);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedBegin] = Color.FromArgb(254, 140, 73);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedEnd] = Color.FromArgb(255, 221, 152);
        rgbTable[KnownColors.msocbvcrCBGradOptionsSelectedMiddle] = Color.FromArgb(255, 184, 116);
        rgbTable[KnownColors.msocbvcrCBGradSelectedBegin] = Color.FromArgb(255, 223, 154);
        rgbTable[KnownColors.msocbvcrCBGradSelectedEnd] = Color.FromArgb(255, 166, 76);
        rgbTable[KnownColors.msocbvcrCBGradSelectedMiddle] = Color.FromArgb(255, 195, 116);
        rgbTable[KnownColors.msocbvcrCBGradVertBegin] = Color.FromArgb(227, 239, 255);
        rgbTable[KnownColors.msocbvcrCBGradVertEnd] = Color.FromArgb(123, 164, 224);
        rgbTable[KnownColors.msocbvcrCBGradVertMiddle] = Color.FromArgb(203, 225, 252);
        rgbTable[KnownColors.msocbvcrCBIconDisabledDark] = Color.FromArgb(97, 122, 172);
        rgbTable[KnownColors.msocbvcrCBIconDisabledLight] = Color.FromArgb(233, 236, 242);
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(186, 211, 245);
        rgbTable[KnownColors.msocbvcrCBLabelBkgnd] = Color.FromArgb(186, 211, 245);
        rgbTable[KnownColors.msocbvcrCBLowColorIconDisabled] = Color.FromArgb(109, 150, 208);
        rgbTable[KnownColors.msocbvcrCBMainMenuBkgd] = Color.FromArgb(153, 204, 255);
        rgbTable[KnownColors.msocbvcrCBMenuBdrOuter] = Color.FromArgb(0, 45, 150);
        rgbTable[KnownColors.msocbvcrCBMenuBkgd] = Color.FromArgb(246, 246, 246);
        rgbTable[KnownColors.msocbvcrCBMenuCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrCBMenuCtlTextDisabled] = Color.FromArgb(141, 141, 141);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgd] = Color.FromArgb(203, 225, 252);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(172, 183, 201);
        rgbTable[KnownColors.msocbvcrCBMenuIconBkgdDropped] = Color.FromArgb(172, 183, 201);
        rgbTable[KnownColors.msocbvcrCBMenuShadow] = Color.FromArgb(95, 130, 234);
        rgbTable[KnownColors.msocbvcrCBMenuSplitArrow] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrCBOptionsButtonShadow] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrCBShadow] = Color.FromArgb(59, 97, 156);
        rgbTable[KnownColors.msocbvcrCBSplitterLine] = Color.FromArgb(106, 140, 203);
        rgbTable[KnownColors.msocbvcrCBSplitterLineLight] = Color.FromArgb(241, 249, 255);
        rgbTable[KnownColors.msocbvcrCBTearOffHandle] = Color.FromArgb(169, 199, 240);
        rgbTable[KnownColors.msocbvcrCBTearOffHandleMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrCBTitleBkgd] = Color.FromArgb(42, 102, 201);
        rgbTable[KnownColors.msocbvcrCBTitleText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDisabledFocuslessHighlightedText] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrDisabledHighlightedText] = Color.FromArgb(187, 206, 236);
        rgbTable[KnownColors.msocbvcrDlgGroupBoxText] = Color.FromArgb(0, 70, 213);
        rgbTable[KnownColors.msocbvcrDocTabBdr] = Color.FromArgb(0, 53, 154);
        rgbTable[KnownColors.msocbvcrDocTabBdrDark] = Color.FromArgb(117, 166, 241);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseDown] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrDarkMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrLight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseDown] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrLightMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseDown] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrMouseOver] = Color.FromArgb(0, 0, 128);
        rgbTable[KnownColors.msocbvcrDocTabBdrSelected] = Color.FromArgb(59, 97, 156);
        rgbTable[KnownColors.msocbvcrDocTabBkgd] = Color.FromArgb(186, 211, 245);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDocTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDocTabBkgdSelected] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDocTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDocTabTextSelected] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(186, 211, 245);
        rgbTable[KnownColors.msocbvcrDWActiveTabBkgd] = Color.FromArgb(186, 211, 245);
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(94, 94, 94);
        rgbTable[KnownColors.msocbvcrDWActiveTabTextDisabled] = Color.FromArgb(94, 94, 94);
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(129, 169, 226);
        rgbTable[KnownColors.msocbvcrDWInactiveTabBkgd] = Color.FromArgb(129, 169, 226);
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDWInactiveTabText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseDown] = Color.FromArgb(254, 128, 62);
        rgbTable[KnownColors.msocbvcrDWTabBkgdMouseOver] = Color.FromArgb(255, 238, 194);
        rgbTable[KnownColors.msocbvcrDWTabTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrDWTabTextMouseOver] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrFocuslessHighlightedText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrGDHeaderBdr] = Color.FromArgb(89, 89, 172);
        rgbTable[KnownColors.msocbvcrGDHeaderBkgd] = Color.FromArgb(239, 235, 222);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBdr] = Color.FromArgb(126, 125, 104);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgd] = Color.FromArgb(239, 235, 222);
        rgbTable[KnownColors.msocbvcrGDHeaderCellBkgdSelected] = Color.FromArgb(255, 192, 111);
        rgbTable[KnownColors.msocbvcrGDHeaderSeeThroughSelection] = Color.FromArgb(191, 191, 223);
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(74, 122, 201);
        rgbTable[KnownColors.msocbvcrGSPDarkBkgd] = Color.FromArgb(74, 122, 201);
        rgbTable[KnownColors.msocbvcrGSPGroupContentDarkBkgd] = Color.FromArgb(185, 208, 241);
        rgbTable[KnownColors.msocbvcrGSPGroupContentLightBkgd] = Color.FromArgb(221, 236, 254);
        rgbTable[KnownColors.msocbvcrGSPGroupContentText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrGSPGroupContentTextDisabled] = Color.FromArgb(150, 145, 133);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderDarkBkgd] = Color.FromArgb(101, 143, 224);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderLightBkgd] = Color.FromArgb(196, 219, 249);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(0, 45, 134);
        rgbTable[KnownColors.msocbvcrGSPGroupHeaderText] = Color.FromArgb(0, 45, 134);
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrGSPGroupline] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrGSPHyperlink] = Color.FromArgb(0, 61, 178);
        rgbTable[KnownColors.msocbvcrGSPLightBkgd] = Color.FromArgb(221, 236, 254);
        rgbTable[KnownColors.msocbvcrHyperlink] = Color.FromArgb(0, 61, 178);
        rgbTable[KnownColors.msocbvcrHyperlinkFollowed] = Color.FromArgb(170, 0, 170);
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(59, 97, 156);
        rgbTable[KnownColors.msocbvcrJotNavUIBdr] = Color.FromArgb(59, 97, 156);
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(158, 190, 245);
        rgbTable[KnownColors.msocbvcrJotNavUIGradBegin] = Color.FromArgb(158, 190, 245);
        rgbTable[KnownColors.msocbvcrJotNavUIGradEnd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(196, 218, 250);
        rgbTable[KnownColors.msocbvcrJotNavUIGradMiddle] = Color.FromArgb(196, 218, 250);
        rgbTable[KnownColors.msocbvcrJotNavUIText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrListHeaderArrow] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrNetLookBkgnd] = Color.FromArgb(227, 239, 255);
        rgbTable[KnownColors.msocbvcrOABBkgd] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOBBkgdBdr] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOBBkgdBdrContrast] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGMDIParentWorkspaceBkgd] = Color.FromArgb(144, 153, 174);
        rgbTable[KnownColors.msocbvcrOGRulerActiveBkgd] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGRulerBdr] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrOGRulerBkgd] = Color.FromArgb(216, 231, 252);
        rgbTable[KnownColors.msocbvcrOGRulerInactiveBkgd] = Color.FromArgb(158, 190, 245);
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdr] = Color.FromArgb(75, 120, 202);
        rgbTable[KnownColors.msocbvcrOGRulerTabBoxBdrHighlight] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOGRulerTabStopTicks] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrOGRulerText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrOGTaskPaneGroupBoxHeaderBkgd] = Color.FromArgb(186, 211, 245);
        rgbTable[KnownColors.msocbvcrOGWorkspaceBkgd] = Color.FromArgb(144, 153, 174);
        rgbTable[KnownColors.msocbvcrOLKFlagNone] = Color.FromArgb(242, 240, 228);
        rgbTable[KnownColors.msocbvcrOLKFolderbarDark] = Color.FromArgb(0, 53, 145);
        rgbTable[KnownColors.msocbvcrOLKFolderbarLight] = Color.FromArgb(89, 135, 214);
        rgbTable[KnownColors.msocbvcrOLKFolderbarText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKGridlines] = Color.FromArgb(234, 233, 225);
        rgbTable[KnownColors.msocbvcrOLKGroupLine] = Color.FromArgb(123, 164, 224);
        rgbTable[KnownColors.msocbvcrOLKGroupNested] = Color.FromArgb(253, 238, 201);
        rgbTable[KnownColors.msocbvcrOLKGroupShaded] = Color.FromArgb(190, 218, 251);
        rgbTable[KnownColors.msocbvcrOLKGroupText] = Color.FromArgb(55, 104, 185);
        rgbTable[KnownColors.msocbvcrOLKIconBar] = Color.FromArgb(253, 247, 233);
        rgbTable[KnownColors.msocbvcrOLKInfoBarBkgd] = Color.FromArgb(144, 153, 174);
        rgbTable[KnownColors.msocbvcrOLKInfoBarText] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKPreviewPaneLabelText] = Color.FromArgb(144, 153, 174);
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorDark] = Color.FromArgb(187, 85, 3);
        rgbTable[KnownColors.msocbvcrOLKTodayIndicatorLight] = Color.FromArgb(251, 200, 79);
        rgbTable[KnownColors.msocbvcrOLKWBActionDividerLine] = Color.FromArgb(215, 228, 251);
        rgbTable[KnownColors.msocbvcrOLKWBButtonDark] = Color.FromArgb(123, 164, 224);
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(203, 225, 252);
        rgbTable[KnownColors.msocbvcrOLKWBButtonLight] = Color.FromArgb(203, 225, 252);
        rgbTable[KnownColors.msocbvcrOLKWBDarkOutline] = Color.FromArgb(0, 45, 150);
        rgbTable[KnownColors.msocbvcrOLKWBFoldersBackground] = Color.FromArgb(255, 255, 255);
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonDark] = Color.FromArgb(247, 190, 87);
        rgbTable[KnownColors.msocbvcrOLKWBHoverButtonLight] = Color.FromArgb(255, 255, 220);
        rgbTable[KnownColors.msocbvcrOLKWBLabelText] = Color.FromArgb(50, 69, 105);
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonDark] = Color.FromArgb(248, 222, 128);
        rgbTable[KnownColors.msocbvcrOLKWBPressedButtonLight] = Color.FromArgb(232, 127, 8);
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonDark] = Color.FromArgb(238, 147, 17);
        rgbTable[KnownColors.msocbvcrOLKWBSelectedButtonLight] = Color.FromArgb(251, 230, 148);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterDark] = Color.FromArgb(0, 53, 145);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(89, 135, 214);
        rgbTable[KnownColors.msocbvcrOLKWBSplitterLight] = Color.FromArgb(89, 135, 214);
        rgbTable[KnownColors.msocbvcrPlacesBarBkgd] = Color.FromArgb(236, 233, 216);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabAreaBkgd] = Color.FromArgb(195, 218, 249);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabBdr] = Color.FromArgb(59, 97, 156);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabInactiveBkgd] = Color.FromArgb(158, 190, 245);
        rgbTable[KnownColors.msocbvcrPPOutlineThumbnailsPaneTabText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelected] = Color.FromArgb(61, 108, 192);
        rgbTable[KnownColors.msocbvcrPPSlideBdrActiveSelectedMouseOver] = Color.FromArgb(61, 108, 192);
        rgbTable[KnownColors.msocbvcrPPSlideBdrInactiveSelected] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrPPSlideBdrMouseOver] = Color.FromArgb(61, 108, 192);
        rgbTable[KnownColors.msocbvcrPubPrintDocScratchPageBkgd] = Color.FromArgb(144, 153, 174);
        rgbTable[KnownColors.msocbvcrPubWebDocScratchPageBkgd] = Color.FromArgb(189, 194, 207);
        rgbTable[KnownColors.msocbvcrSBBdr] = Color.FromArgb(211, 211, 211);
        rgbTable[KnownColors.msocbvcrScrollbarBkgd] = Color.FromArgb(251, 251, 248);
        rgbTable[KnownColors.msocbvcrToastGradBegin] = Color.FromArgb(220, 236, 254);
        rgbTable[KnownColors.msocbvcrToastGradEnd] = Color.FromArgb(167, 197, 238);
        rgbTable[KnownColors.msocbvcrWPBdrInnerDocked] = Color.FromArgb(185, 212, 249);
        rgbTable[KnownColors.msocbvcrWPBdrOuterDocked] = Color.FromArgb(196, 218, 250);
        rgbTable[KnownColors.msocbvcrWPBdrOuterFloating] = Color.FromArgb(42, 102, 201);
        rgbTable[KnownColors.msocbvcrWPBkgd] = Color.FromArgb(221, 236, 254);
        rgbTable[KnownColors.msocbvcrWPCtlBdr] = Color.FromArgb(127, 157, 185);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDefault] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlBdrDisabled] = Color.FromArgb(128, 128, 128);
        rgbTable[KnownColors.msocbvcrWPCtlBkgd] = Color.FromArgb(169, 199, 240);
        rgbTable[KnownColors.msocbvcrWPCtlBkgdDisabled] = Color.FromArgb(222, 222, 222);
        rgbTable[KnownColors.msocbvcrWPCtlText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPCtlTextDisabled] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrWPCtlTextMouseDown] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPGroupline] = Color.FromArgb(123, 164, 224);
        rgbTable[KnownColors.msocbvcrWPInfoTipBkgd] = Color.FromArgb(255, 255, 204);
        rgbTable[KnownColors.msocbvcrWPInfoTipText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPNavBarBkgnd] = Color.FromArgb(74, 122, 201);
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPText] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPTextDisabled] = Color.FromArgb(172, 168, 153);
        rgbTable[KnownColors.msocbvcrWPTitleBkgdActive] = Color.FromArgb(123, 164, 224);
        rgbTable[KnownColors.msocbvcrWPTitleBkgdInactive] = Color.FromArgb(148, 187, 239);
        rgbTable[KnownColors.msocbvcrWPTitleTextActive] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrWPTitleTextInactive] = Color.FromArgb(0, 0, 0);
        rgbTable[KnownColors.msocbvcrXLFormulaBarBkgd] = Color.FromArgb(158, 190, 245);
    }
}
