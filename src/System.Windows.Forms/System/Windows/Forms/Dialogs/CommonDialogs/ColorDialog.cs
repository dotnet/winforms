// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms;

/// <summary>
///  Represents a common dialog box that displays available colors along with controls
///  that allow the user to define custom colors.
/// </summary>
[DefaultProperty(nameof(Color))]
[SRDescription(nameof(SR.DescriptionColorDialog))]
public class ColorDialog : CommonDialog
{
    private int _options;
    private readonly COLORREF[] _customColors = new COLORREF[16];

    private Color _color;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ColorDialog"/> class.
    /// </summary>
    public ColorDialog() => Reset();

    /// <summary>
    ///  Gets or sets a value indicating whether the user can use the dialog box
    ///  to define custom colors.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.CDallowFullOpenDescr))]
    public virtual bool AllowFullOpen
    {
        get => !GetOption((int)CHOOSECOLOR_FLAGS.CC_PREVENTFULLOPEN);
        set => SetOption((int)CHOOSECOLOR_FLAGS.CC_PREVENTFULLOPEN, !value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box displays all available colors in
    ///  the set of basic colors.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.CDanyColorDescr))]
    public virtual bool AnyColor
    {
        get => GetOption((int)CHOOSECOLOR_FLAGS.CC_ANYCOLOR);
        set => SetOption((int)CHOOSECOLOR_FLAGS.CC_ANYCOLOR, value);
    }

    /// <summary>
    ///  Gets or sets the color selected by the user.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.CDcolorDescr))]
    public Color Color
    {
        get => _color;
        set => _color = !value.IsEmpty ? value : Color.Black;
    }

    /// <summary>
    ///  Gets or sets the set of custom colors shown in the dialog box.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.CDcustomColorsDescr))]
    [AllowNull]
    public int[] CustomColors
    {
        get
        {
            int[] result = new int[_customColors.Length];
            MemoryMarshal.Cast<COLORREF, int>(_customColors).CopyTo(result);
            return result;
        }
        set
        {
            Span<COLORREF> customColors = _customColors;
            customColors.Fill(0x00FFFFFF);

            if (value is not null && value.Length > 0)
            {
                MemoryMarshal.Cast<int, COLORREF>(value)[..Math.Min(customColors.Length, value.Length)].CopyTo(customColors);
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the controls used to create custom
    ///  colors are visible when the dialog box is opened.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.CDfullOpenDescr))]
    public virtual bool FullOpen
    {
        get => GetOption((int)CHOOSECOLOR_FLAGS.CC_FULLOPEN);
        set => SetOption((int)CHOOSECOLOR_FLAGS.CC_FULLOPEN, value);
    }

    /// <summary>
    ///  Our HINSTANCE from Windows.
    /// </summary>
    protected virtual nint Instance => PInvoke.GetModuleHandle((PCWSTR)null);

    /// <summary>
    ///  Returns our CHOOSECOLOR options.
    /// </summary>
    protected virtual int Options => _options;

    /// <summary>
    ///  Gets or sets a value indicating whether a Help button appears in the color dialog box.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.CDshowHelpDescr))]
    public virtual bool ShowHelp
    {
        get => GetOption((int)CHOOSECOLOR_FLAGS.CC_SHOWHELP);
        set => SetOption((int)CHOOSECOLOR_FLAGS.CC_SHOWHELP, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the dialog box will restrict users to selecting solid colors only.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.CDsolidColorOnlyDescr))]
    public virtual bool SolidColorOnly
    {
        get => GetOption((int)CHOOSECOLOR_FLAGS.CC_SOLIDCOLOR);
        set => SetOption((int)CHOOSECOLOR_FLAGS.CC_SOLIDCOLOR, value);
    }

    /// <summary>
    ///  Lets us control the CHOOSECOLOR options.
    /// </summary>
    private bool GetOption(int option) => (_options & option) != 0;

    /// <summary>
    ///  Resets all options to their default values, the last selected color to black, and the custom
    ///  colors to their default values.
    /// </summary>
    public override void Reset()
    {
        _options = 0;
        _color = Color.Black;
        CustomColors = null;
    }

    private void ResetColor() => Color = Color.Black;

    protected override unsafe bool RunDialog(IntPtr hwndOwner)
    {
        CHOOSECOLOR_FLAGS flags = (CHOOSECOLOR_FLAGS)Options | CHOOSECOLOR_FLAGS.CC_RGBINIT | CHOOSECOLOR_FLAGS.CC_ENABLEHOOK;

        // Our docs say AllowFullOpen takes precedence over FullOpen; ChooseColor implements the opposite
        if (!AllowFullOpen)
        {
            flags &= ~CHOOSECOLOR_FLAGS.CC_FULLOPEN;
        }

        CHOOSECOLORW cc = new()
        {
            lStructSize = (uint)sizeof(CHOOSECOLORW),
            hwndOwner = (HWND)hwndOwner,
            hInstance = (HWND)Instance,
            rgbResult = (COLORREF)_color,
            Flags = flags,
            lpfnHook = HookProcFunctionPointer
        };

        fixed (COLORREF* customColors = _customColors)
        {
            cc.lpCustColors = customColors;

            if (!PInvoke.ChooseColor(&cc))
            {
                return false;
            }

            _color = cc.rgbResult;
            return true;
        }
    }

    /// <summary>
    ///  Allows us to manipulate the CHOOSECOLOR options
    /// </summary>
    private void SetOption(int option, bool value)
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
    ///  Indicates whether the <see cref="Color"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeColor() => !Color.Equals(Color.Black);

    public override string ToString() => $"{base.ToString()},  Color: {Color}";
}
