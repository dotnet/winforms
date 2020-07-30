// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents
    ///  a common dialog box that displays a list of fonts that are currently installed
    ///  on
    ///  the system.
    /// </summary>
    [DefaultEvent(nameof(Apply))]
    [DefaultProperty(nameof(Font))]
    [SRDescription(nameof(SR.DescriptionFontDialog))]
    public class FontDialog : CommonDialog
    {
        protected static readonly object EventApply = new object();

        private const int defaultMinSize = 0;
        private const int defaultMaxSize = 0;

        private Comdlg32.CF options;
        private Font font;
        private Color color;
        private int minSize = defaultMinSize;
        private int maxSize = defaultMaxSize;
        private bool showColor;
        private bool usingDefaultIndirectColor;

        /// <summary>
        ///  Initializes a new instance of the <see cref='FontDialog'/>
        ///  class.
        /// </summary>
        public FontDialog()
        {
            Reset();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box allows graphics device interface
        ///  (GDI) font simulations.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FnDallowSimulationsDescr))]
        public bool AllowSimulations
        {
            get => !GetOption(Comdlg32.CF.NOSIMULATIONS);
            set => SetOption(Comdlg32.CF.NOSIMULATIONS, !value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box allows vector font selections.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FnDallowVectorFontsDescr))]
        public bool AllowVectorFonts
        {
            get => !GetOption(Comdlg32.CF.NOVECTORFONTS);
            set => SetOption(Comdlg32.CF.NOVECTORFONTS, !value);
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
            get => !GetOption(Comdlg32.CF.NOVERTFONTS);
            set => SetOption(Comdlg32.CF.NOVERTFONTS, !value);
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating whether the user can change the character set specified
        ///  in the Script combo box to display a character set other than the one
        ///  currently displayed.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FnDallowScriptChangeDescr))]
        public bool AllowScriptChange
        {
            get => !GetOption(Comdlg32.CF.SELECTSCRIPT);
            set => SetOption(Comdlg32.CF.SELECTSCRIPT, !value);
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
                // Convert to RGB and back to resolve indirect colors like SystemColors.ControlText
                // to real color values like Color.Lime
                if (usingDefaultIndirectColor)
                {
                    return ColorTranslator.FromWin32(ColorTranslator.ToWin32(color));
                }
                return color;
            }
            set
            {
                if (!value.IsEmpty)
                {
                    color = value;
                    usingDefaultIndirectColor = false;
                }
                else
                {
                    color = SystemColors.ControlText;
                    usingDefaultIndirectColor = true;
                }
            }
        }

        /// <summary>
        ///  Gets or sets
        ///  a value indicating whether the dialog box allows only the selection of fixed-pitch fonts.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FnDfixedPitchOnlyDescr))]
        public bool FixedPitchOnly
        {
            get => GetOption(Comdlg32.CF.FIXEDPITCHONLY);
            set => SetOption(Comdlg32.CF.FIXEDPITCHONLY, value);
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
                Font result = font;
                if (result is null)
                {
                    result = Control.DefaultFont;
                }

                float actualSize = result.SizeInPoints;
                if (minSize != defaultMinSize && actualSize < MinSize)
                {
                    result = new Font(result.FontFamily, MinSize, result.Style, GraphicsUnit.Point);
                }

                if (maxSize != defaultMaxSize && actualSize > MaxSize)
                {
                    result = new Font(result.FontFamily, MaxSize, result.Style, GraphicsUnit.Point);
                }

                return result;
            }
            set
            {
                font = value;
            }
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
            get => GetOption(Comdlg32.CF.FORCEFONTEXIST);
            set
            {
                SetOption(Comdlg32.CF.FORCEFONTEXIST, value);
            }
        }

        /// <summary>
        ///  Gets or sets the maximum
        ///  point size a user can select.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue(defaultMaxSize)]
        [SRDescription(nameof(SR.FnDmaxSizeDescr))]
        public int MaxSize
        {
            get
            {
                return maxSize;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                maxSize = value;

                if (maxSize > 0 && maxSize < minSize)
                {
                    minSize = maxSize;
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating the minimum point size a user can select.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue(defaultMinSize)]
        [SRDescription(nameof(SR.FnDminSizeDescr))]
        public int MinSize
        {
            get
            {
                return minSize;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                minSize = value;

                if (maxSize > 0 && maxSize < minSize)
                {
                    maxSize = minSize;
                }
            }
        }

        /// <summary>
        ///  Gets the value passed to CHOOSEFONT.Flags.
        /// </summary>
        protected int Options => (int)options;

        /// <summary>
        ///  Gets or sets a
        ///  value indicating whether the dialog box allows selection of fonts for all non-OEM and Symbol character
        ///  sets, as well as the ----n National Standards Institute (ANSI) character set.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FnDscriptsOnlyDescr))]
        public bool ScriptsOnly
        {
            get => GetOption(Comdlg32.CF.SCRIPTSONLY);
            set => SetOption(Comdlg32.CF.SCRIPTSONLY, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box contains an Apply button.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FnDshowApplyDescr))]
        public bool ShowApply
        {
            get => GetOption(Comdlg32.CF.APPLY);
            set => SetOption(Comdlg32.CF.APPLY, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays the color choice.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FnDshowColorDescr))]
        public bool ShowColor
        {
            get
            {
                return showColor;
            }
            set
            {
                showColor = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box contains controls that allow the
        ///  user to specify strikethrough, underline, and text color options.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FnDshowEffectsDescr))]
        public bool ShowEffects
        {
            get => GetOption(Comdlg32.CF.EFFECTS);
            set => SetOption(Comdlg32.CF.EFFECTS, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the dialog box displays a Help button.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FnDshowHelpDescr))]
        public bool ShowHelp
        {
            get => GetOption(Comdlg32.CF.SHOWHELP);
            set => SetOption(Comdlg32.CF.SHOWHELP, value);
        }

        /// <summary>
        ///  Occurs when the user clicks the Apply button in the font
        ///  dialog box.
        /// </summary>
        [SRDescription(nameof(SR.FnDapplyDescr))]
        public event EventHandler Apply
        {
            add => Events.AddHandler(EventApply, value);
            remove => Events.RemoveHandler(EventApply, value);
        }

        /// <summary>
        ///  Returns the state of the given option flag.
        /// </summary>
        internal bool GetOption(Comdlg32.CF option)
        {
            return (options & option) != 0;
        }

        /// <summary>
        ///  Specifies the common dialog box hook procedure that is overridden to add
        ///  specific functionality to a common dialog box.
        /// </summary>
        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            switch ((User32.WM)msg)
            {
                case User32.WM.COMMAND:
                    if ((int)wparam == 0x402)
                    {
                        var logFont = new User32.LOGFONTW();
                        User32.SendMessageW(hWnd, User32.WM.CHOOSEFONT_GETLOGFONT, IntPtr.Zero, ref logFont);
                        UpdateFont(ref logFont);
                        int index = (int)User32.SendDlgItemMessageW(hWnd, User32.DialogItemID.cmb4, (User32.WM)User32.CB.GETCURSEL);
                        if (index != User32.CB_ERR)
                        {
                            UpdateColor((int)User32.SendDlgItemMessageW(hWnd, User32.DialogItemID.cmb4, (User32.WM)User32.CB.GETITEMDATA, (IntPtr)index));
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
                    }
                    break;
                case User32.WM.INITDIALOG:
                    if (!showColor)
                    {
                        IntPtr hWndCtl = User32.GetDlgItem(hWnd, User32.DialogItemID.cmb4);
                        User32.ShowWindow(hWndCtl, User32.SW.HIDE);
                        hWndCtl = User32.GetDlgItem(hWnd, User32.DialogItemID.stc4);
                        User32.ShowWindow(hWndCtl, User32.SW.HIDE);
                    }
                    break;
            }

            return base.HookProc(hWnd, msg, wparam, lparam);
        }

        /// <summary>
        ///  Raises the <see cref='Apply'/> event.
        /// </summary>
        protected virtual void OnApply(EventArgs e)
        {
            ((EventHandler)Events[EventApply])?.Invoke(this, e);
        }

        /// <summary>
        ///  Resets all dialog box options to their default values.
        /// </summary>
        public override void Reset()
        {
            options = Comdlg32.CF.SCREENFONTS | Comdlg32.CF.EFFECTS;
            font = null;
            color = SystemColors.ControlText;
            usingDefaultIndirectColor = true;
            showColor = false;
            minSize = defaultMinSize;
            maxSize = defaultMaxSize;
            SetOption(Comdlg32.CF.TTONLY, true);
        }

        private void ResetFont()
        {
            font = null;
        }

        /// <summary>
        ///  The actual implementation of running the dialog. Inheriting classes
        ///  should override this if they want to add more functionality, and call
        ///  base.runDialog() if necessary
        /// </summary>
        protected unsafe override bool RunDialog(IntPtr hWndOwner)
        {
            var hookProcPtr = new User32.WNDPROCINT(HookProc);
            using var dc = User32.GetDcScope.ScreenDC;
            using Graphics graphics = Graphics.FromHdcInternal(dc);
            User32.LOGFONTW logFont = User32.LOGFONTW.FromFont(Font, graphics);

            var cf = new Comdlg32.CHOOSEFONTW
            {
                lStructSize = (uint)Marshal.SizeOf<Comdlg32.CHOOSEFONTW>(),
                hwndOwner = hWndOwner,
                hDC = IntPtr.Zero,
                lpLogFont = &logFont,
                Flags = (Comdlg32.CF)Options | Comdlg32.CF.INITTOLOGFONTSTRUCT | Comdlg32.CF.ENABLEHOOK,
                lpfnHook = hookProcPtr,
                hInstance = Kernel32.GetModuleHandleW(null),
                nSizeMin = minSize,
                nSizeMax = maxSize == 0 ? int.MaxValue : maxSize,
                rgbColors = ShowColor || ShowEffects
                    ? ColorTranslator.ToWin32(color)
                    : ColorTranslator.ToWin32(SystemColors.ControlText)
            };

            if (minSize > 0 || maxSize > 0)
            {
                cf.Flags |= Comdlg32.CF.LIMITSIZE;
            }

            // if ShowColor=true then try to draw the sample text in color,
            // if ShowEffects=false then we will draw the sample text in standard control text color regardless.
            // (limitation of windows control)

            Debug.Assert(cf.nSizeMin <= cf.nSizeMax, "min and max font sizes are the wrong way around");
            if (Comdlg32.ChooseFontW(ref cf).IsFalse())
            {
                return false;
            }

            if (logFont.FaceName.Length > 0)
            {
                UpdateFont(ref logFont);
                UpdateColor(cf.rgbColors);
            }

            return true;
        }

        /// <summary>
        ///  Sets the given option to the given boolean value.
        /// </summary>
        internal void SetOption(Comdlg32.CF option, bool value)
        {
            if (value)
            {
                options |= option;
            }
            else
            {
                options &= ~option;
            }
        }

        /// <summary>
        ///  Indicates whether the <see cref='Font'/> property should be
        ///  persisted.
        /// </summary>
        private bool ShouldSerializeFont()
        {
            return !Font.Equals(Control.DefaultFont);
        }

        /// <summary>
        ///  Retrieves a string that includes the name of the current font selected in
        ///  the dialog box.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ",  Font: " + Font.ToString();
        }

        private void UpdateColor(int rgb)
        {
            if (ColorTranslator.ToWin32(color) != rgb)
            {
                color = ColorTranslator.FromOle(rgb);
                usingDefaultIndirectColor = false;
            }
        }

        private void UpdateFont(ref User32.LOGFONTW lf)
        {
            using var dc = User32.GetDcScope.ScreenDC;
            using Font fontInWorldUnits = Font.FromLogFont(lf, dc);

            // The dialog claims its working in points (a device-independent unit),
            // but actually gives us something in world units (device-dependent).
            font = ControlPaint.FontInPoints(fontInWorldUnits);
        }
    }
}
