// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Interop;
using System.Runtime.CompilerServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms;

/// <summary>
///  Represents a common dialog box that displays a list of fonts that are currently installed on the system.
/// </summary>
[DefaultEvent(nameof(Apply))]
[DefaultProperty(nameof(Font))]
[SRDescription(nameof(SR.DescriptionFontDialog))]
public class FontDialog : CommonDialog
{
    protected static readonly object EventApply = new();

    private const int DefaultMinSize = 0;
    private const int DefaultMaxSize = 0;

    private CHOOSEFONT_FLAGS _options;
    private Font? _font;
    private Color _color;
    private int _minSize = DefaultMinSize;
    private int _maxSize = DefaultMaxSize;
    private bool _usingDefaultIndirectColor;

    /// <summary>
    ///  Initializes a new instance of the <see cref="FontDialog"/> class.
    /// </summary>
    public FontDialog() => Reset();

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box allows graphics device interface
    ///  (GDI) font simulations.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FnDallowSimulationsDescr))]
    public bool AllowSimulations
    {
        get => !GetOption(CHOOSEFONT_FLAGS.CF_NOSIMULATIONS);
        set => SetOption(CHOOSEFONT_FLAGS.CF_NOSIMULATIONS, !value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box allows vector font selections.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FnDallowVectorFontsDescr))]
    public bool AllowVectorFonts
    {
        get => !GetOption(CHOOSEFONT_FLAGS.CF_NOVECTORFONTS);
        set => SetOption(CHOOSEFONT_FLAGS.CF_NOVECTORFONTS, !value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether
    ///  the dialog box displays both vertical and horizontal fonts or only
    ///  horizontal fonts.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FnDallowVerticalFontsDescr))]
    public bool AllowVerticalFonts
    {
        get => !GetOption(CHOOSEFONT_FLAGS.CF_NOVERTFONTS);
        set => SetOption(CHOOSEFONT_FLAGS.CF_NOVERTFONTS, !value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the user can change the character set specified in the Script combo
    ///  box to display a character set other than the one currently displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FnDallowScriptChangeDescr))]
    public bool AllowScriptChange
    {
        get => !GetOption(CHOOSEFONT_FLAGS.CF_SELECTSCRIPT);
        set => SetOption(CHOOSEFONT_FLAGS.CF_SELECTSCRIPT, !value);
    }

    /// <summary>
    ///  Gets or sets a value indicating the selected font color.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.FnDcolorDescr))]
    [DefaultValue(typeof(Color), "Black")]
    public Color Color
    {
        get
        {
            // Convert to RGB and back to resolve indirect colors like Application.SystemColors.ControlText
            // to real color values like Color.Lime
            return !_usingDefaultIndirectColor ? _color : ColorTranslator.FromWin32(ColorTranslator.ToWin32(_color));
        }
        set
        {
            if (!value.IsEmpty)
            {
                _color = value;
                _usingDefaultIndirectColor = false;
            }
            else
            {
                _color = Application.SystemColors.ControlText;
                _usingDefaultIndirectColor = true;
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box allows only the selection of fixed-pitch fonts.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FnDfixedPitchOnlyDescr))]
    public bool FixedPitchOnly
    {
        get => GetOption(CHOOSEFONT_FLAGS.CF_FIXEDPITCHONLY);
        set => SetOption(CHOOSEFONT_FLAGS.CF_FIXEDPITCHONLY, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating the selected font.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.FnDfontDescr))]
    public Font Font
    {
        get
        {
            Font? result = _font ?? Control.DefaultFont;

            float actualSize = result.SizeInPoints;
            if (_minSize != DefaultMinSize && actualSize < MinSize)
            {
                result = new Font(result.FontFamily, MinSize, result.Style, GraphicsUnit.Point);
            }

            if (_maxSize != DefaultMaxSize && actualSize > MaxSize)
            {
                result = new Font(result.FontFamily, MaxSize, result.Style, GraphicsUnit.Point);
            }

            return result;
        }
        set => _font = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box specifies an error condition if the
    ///  user attempts to select a font or style that does not exist.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FnDfontMustExistDescr))]
    public bool FontMustExist
    {
        get => GetOption(CHOOSEFONT_FLAGS.CF_FORCEFONTEXIST);
        set => SetOption(CHOOSEFONT_FLAGS.CF_FORCEFONTEXIST, value);
    }

    /// <summary>
    ///  Gets or sets the maximum point size a user can select.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(DefaultMaxSize)]
    [SRDescription(nameof(SR.FnDmaxSizeDescr))]
    public int MaxSize
    {
        get => _maxSize;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            _maxSize = value;

            if (_maxSize > 0 && _maxSize < _minSize)
            {
                _minSize = _maxSize;
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating the minimum point size a user can select.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(DefaultMinSize)]
    [SRDescription(nameof(SR.FnDminSizeDescr))]
    public int MinSize
    {
        get => _minSize;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            _minSize = value;

            if (_maxSize > 0 && _maxSize < _minSize)
            {
                _maxSize = _minSize;
            }
        }
    }

    /// <summary>
    ///  Gets the value passed to CHOOSEFONT.Flags.
    /// </summary>
    protected int Options => (int)_options;

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box allows selection of fonts for all non-OEM and Symbol
    ///  character sets, as well as the ANSI character set.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FnDscriptsOnlyDescr))]
    public bool ScriptsOnly
    {
        get => GetOption(CHOOSEFONT_FLAGS.CF_SCRIPTSONLY);
        set => SetOption(CHOOSEFONT_FLAGS.CF_SCRIPTSONLY, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box contains an Apply button.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FnDshowApplyDescr))]
    public bool ShowApply
    {
        get => GetOption(CHOOSEFONT_FLAGS.CF_APPLY);
        set => SetOption(CHOOSEFONT_FLAGS.CF_APPLY, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box displays the color choice.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FnDshowColorDescr))]
    public bool ShowColor { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box contains controls that allow the
    ///  user to specify strikethrough, underline, and text color options.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FnDshowEffectsDescr))]
    public bool ShowEffects
    {
        get => GetOption(CHOOSEFONT_FLAGS.CF_EFFECTS);
        set => SetOption(CHOOSEFONT_FLAGS.CF_EFFECTS, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box displays a Help button.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.FnDshowHelpDescr))]
    public bool ShowHelp
    {
        get => GetOption(CHOOSEFONT_FLAGS.CF_SHOWHELP);
        set => SetOption(CHOOSEFONT_FLAGS.CF_SHOWHELP, value);
    }

    /// <summary>
    ///  Occurs when the user clicks the Apply button in the font dialog box.
    /// </summary>
    [SRDescription(nameof(SR.FnDapplyDescr))]
    public event EventHandler? Apply
    {
        add => Events.AddHandler(EventApply, value);
        remove => Events.RemoveHandler(EventApply, value);
    }

    /// <summary>
    ///  Returns the state of the given option flag.
    /// </summary>
    internal bool GetOption(CHOOSEFONT_FLAGS option) => (_options & option) != 0;

    /// <summary>
    ///  Specifies the common dialog box hook procedure that is overridden to add
    ///  specific functionality to a common dialog box.
    /// </summary>
    protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        switch ((uint)msg)
        {
            case PInvoke.WM_COMMAND:
                if (wparam != 0x402)
                {
                    break;
                }

                LOGFONT logFont = default;
                PInvoke.SendMessage((HWND)hWnd, PInvoke.WM_CHOOSEFONT_GETLOGFONT, (WPARAM)0, ref logFont);
                UpdateFont(ref logFont);
                int index = (int)PInvoke.SendDlgItemMessage((HWND)hWnd, (int)PInvoke.cmb4, PInvoke.CB_GETCURSEL, 0, 0);
                if (index != PInvoke.CB_ERR)
                {
                    UpdateColor((COLORREF)(int)PInvoke.SendDlgItemMessage(
                        (HWND)hWnd,
                        (int)PInvoke.cmb4,
                        PInvoke.CB_GETITEMDATA,
                        (WPARAM)index,
                        0));
                }

                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    OnApply(EventArgs.Empty);
                }
                else
                {
                    try
                    {
                        OnApply(EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        Application.OnThreadException(e);
                    }
                }

                break;
            case PInvoke.WM_INITDIALOG:
                if (!ShowColor)
                {
                    HWND hWndCtl = PInvoke.GetDlgItem((HWND)hWnd, (int)PInvoke.cmb4);
                    PInvoke.ShowWindow(hWndCtl, SHOW_WINDOW_CMD.SW_HIDE);
                    hWndCtl = PInvoke.GetDlgItem((HWND)hWnd, (int)PInvoke.stc4);
                    PInvoke.ShowWindow(hWndCtl, SHOW_WINDOW_CMD.SW_HIDE);
                }

                break;
        }

        return base.HookProc(hWnd, msg, wparam, lparam);
    }

    /// <summary>
    ///  Raises the <see cref="Apply"/> event.
    /// </summary>
    protected virtual void OnApply(EventArgs e) => (Events[EventApply] as EventHandler)?.Invoke(this, e);

    /// <summary>
    ///  Resets all dialog box options to their default values.
    /// </summary>
    public override void Reset()
    {
        _options = CHOOSEFONT_FLAGS.CF_SCREENFONTS | CHOOSEFONT_FLAGS.CF_EFFECTS;
        _font = null;
        _color = Application.SystemColors.ControlText;
        _usingDefaultIndirectColor = true;
        ShowColor = false;
        _minSize = DefaultMinSize;
        _maxSize = DefaultMaxSize;
        SetOption(CHOOSEFONT_FLAGS.CF_TTONLY, true);
    }

    private void ResetFont() => _font = null;

    protected override unsafe bool RunDialog(IntPtr hWndOwner)
    {
        using var dc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdcInternal(dc);
        LOGFONTW logFont = Font.ToLogicalFont(graphics);

        CHOOSEFONTW cf = new()
        {
            lStructSize = (uint)sizeof(CHOOSEFONTW),
            hwndOwner = (HWND)hWndOwner,
            lpLogFont = &logFont,
            Flags = (CHOOSEFONT_FLAGS)Options | CHOOSEFONT_FLAGS.CF_INITTOLOGFONTSTRUCT | CHOOSEFONT_FLAGS.CF_ENABLEHOOK,
            lpfnHook = HookProcFunctionPointer,
            hInstance = PInvoke.GetModuleHandle((PCWSTR)null),
            nSizeMin = _minSize,
            nSizeMax = _maxSize == 0 ? int.MaxValue : _maxSize,
            rgbColors = ShowColor || ShowEffects ? _color : Application.SystemColors.ControlText
        };

        if (_minSize > 0 || _maxSize > 0)
        {
            cf.Flags |= CHOOSEFONT_FLAGS.CF_LIMITSIZE;
        }

        // if ShowColor=true then try to draw the sample text in color,
        // if ShowEffects=false then we will draw the sample text in standard control text color regardless.
        // (limitation of windows control)

        Debug.Assert(cf.nSizeMin <= cf.nSizeMax, "min and max font sizes are the wrong way around");

        // The native font dialog does not currently support Per Monitor V2 mode. We are setting DpiAwareness
        // to SystemAware as a workaround. This action has no effect when the application is not running in
        // Per Monitor V2 mode.
        //
        // https://microsoft.visualstudio.com/OS/_workitems/edit/42835582
        using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
        {
            if (!PInvoke.ChooseFont(&cf))
            {
                return false;
            }
        }

        if (!logFont.FaceName.IsEmpty)
        {
            UpdateFont(ref Unsafe.As<LOGFONTW, LOGFONT>(ref logFont));
            UpdateColor(cf.rgbColors);
        }

        return true;
    }

    /// <summary>
    ///  Sets the given option to the given boolean value.
    /// </summary>
    internal void SetOption(CHOOSEFONT_FLAGS option, bool value)
    {
        if (value)
        {
            _options |= option;
        }
        else
        {
            _options &= ~option;
        }
    }

    /// <summary>
    ///  Indicates whether the <see cref="Font"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeFont() => !Font.Equals(Control.DefaultFont);

    public override string ToString() => $"{base.ToString()},  Font: {Font}";

    private void UpdateColor(Color color)
    {
        if (_color != color)
        {
            _color = color;
            _usingDefaultIndirectColor = false;
        }
    }

    private void UpdateFont(ref LOGFONT lf)
    {
        using var dc = GetDcScope.ScreenDC;
        using Font fontInWorldUnits = Font.FromLogFont(in lf, dc);

        // The dialog claims its working in points (a device-independent unit),
        // but actually gives us something in world units (device-dependent).
        _font = ControlPaint.FontInPoints(fontInWorldUnits);
    }
}
