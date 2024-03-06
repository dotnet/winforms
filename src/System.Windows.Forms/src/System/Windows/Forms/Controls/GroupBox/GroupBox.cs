// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Encapsulates a standard Windows group box.
/// </summary>
[DefaultEvent(nameof(Enter))]
[DefaultProperty(nameof(Text))]
[Designer($"System.Windows.Forms.Design.GroupBoxDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionGroupBox))]
public partial class GroupBox : Control
{
    private int _fontHeight = -1;
    private Font? _cachedFont;
    private FlatStyle _flatStyle = FlatStyle.Standard;

    /// <summary>
    ///  Initializes a new instance of the <see cref="GroupBox"/> class.
    /// </summary>
    public GroupBox() : base()
    {
        // This class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        SetStyle(ControlStyles.ContainerControl, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw, OwnerDraw);

        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the control will allow drag and
    ///  drop operations and events to be used.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    /// <summary>
    ///  Override to re-expose AutoSize.
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    /// <summary>
    ///  Allows the control to optionally shrink when AutoSize is true.
    /// </summary>
    [SRDescription(nameof(SR.ControlAutoSizeModeDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(true)]
    [DefaultValue(AutoSizeMode.GrowOnly)]
    [Localizable(true)]
    public AutoSizeMode AutoSizeMode
    {
        get => GetAutoSizeMode();
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (GetAutoSizeMode() == value)
            {
                return;
            }

            SetAutoSizeMode(value);
            if (ParentInternal is not null)
            {
                // DefaultLayout does not keep anchor information until it needs to.  When
                // AutoSize became a common property, we could no longer blindly call into
                // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                if (ParentInternal.LayoutEngine == DefaultLayout.Instance)
                {
                    ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                }

                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
            }
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            if (!OwnerDraw)
            {
                cp.ClassName = PInvoke.WC_BUTTON;
                cp.Style |= PInvoke.BS_GROUPBOX;
            }
            else
            {
                // If we swap back to a different flat style we need to reset these.
                cp.ClassName = null;
                cp.Style &= ~PInvoke.BS_GROUPBOX;
            }

            cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CONTROLPARENT;

            return cp;
        }
    }

    // Set the default Padding to 3 so that it is consistent with Everett
    protected override Padding DefaultPadding => new(3);

    protected override Size DefaultSize => new(200, 100);

    /// <summary>
    ///  Gets a rectangle that represents the dimensions of the <see cref="GroupBox"/>
    /// </summary>
    public override Rectangle DisplayRectangle
    {
        get
        {
            Size size = ClientSize;

            if (_fontHeight == -1)
            {
                _fontHeight = Font.Height;
                _cachedFont = Font;
            }
            else if (!ReferenceEquals(_cachedFont, Font))
            {
                // Must also cache font identity here because we need to provide an accurate DisplayRectangle
                // picture even before the OnFontChanged event bubbles through.
                _fontHeight = Font.Height;
                _cachedFont = Font;
            }

            // For efficiency, so that we don't need to read property store four times
            Padding padding = Padding;
            return new Rectangle(
                padding.Left,
                _fontHeight + padding.Top,
                Math.Max(size.Width - padding.Horizontal, 0),
                Math.Max(size.Height - _fontHeight - padding.Vertical, 0));
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public FlatStyle FlatStyle
    {
        get
        {
            return _flatStyle;
        }
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);

            if (_flatStyle == value)
            {
                return;
            }

            bool originalOwnerDraw = OwnerDraw;
            _flatStyle = value;

            // In CreateParams, we pick our class style based on OwnerDraw
            // if this has changed we need to recreate
            bool needRecreate = (OwnerDraw != originalOwnerDraw);

            SetStyle(ControlStyles.ContainerControl, true);

            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserMouse, OwnerDraw);

            if (needRecreate)
            {
                RecreateHandle();
            }
            else
            {
                Refresh();
            }
        }
    }

    private bool OwnerDraw => FlatStyle != FlatStyle.System;

    /// <summary>
    ///  Gets or sets a value indicating whether the user may press the TAB key to give the focus to the
    ///  <see cref="GroupBox"/>.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    [Localizable(true)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            // the GroupBox controls immediately draws when the WM_SETTEXT comes through, but
            // does so in the wrong font, so we suspend that behavior, and then
            // invalidate.
            bool suspendRedraw = Visible;
            try
            {
                if (suspendRedraw && IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)false);
                }

                base.Text = value;
            }
            finally
            {
                if (suspendRedraw && IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)true);
                }
            }

            Invalidate(true);
        }
    }

    /// <summary>
    ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))]
    public bool UseCompatibleTextRendering
    {
        get => UseCompatibleTextRenderingInternal;
        set => UseCompatibleTextRenderingInternal = value;
    }

    /// <summary>
    ///  Determines whether the control supports rendering text using GDI+ and GDI. This is provided for container
    ///  controls to iterate through its children to set UseCompatibleTextRendering to the same value if the child
    ///  control supports it.
    /// </summary>
    internal override bool SupportsUseCompatibleTextRendering => true;

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseClick
    {
        add => base.MouseClick += value;
        remove => base.MouseClick -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseDoubleClick
    {
        add => base.MouseDoubleClick += value;
        remove => base.MouseDoubleClick -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event KeyEventHandler? KeyUp
    {
        add => base.KeyUp += value;
        remove => base.KeyUp -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event KeyEventHandler? KeyDown
    {
        add => base.KeyDown += value;
        remove => base.KeyDown -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => base.KeyPress += value;
        remove => base.KeyPress -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseDown
    {
        add => base.MouseDown += value;
        remove => base.MouseDown -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseUp
    {
        add => base.MouseUp += value;
        remove => base.MouseUp -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseMove
    {
        add => base.MouseMove += value;
        remove => base.MouseMove -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? MouseEnter
    {
        add => base.MouseEnter += value;
        remove => base.MouseEnter -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? MouseLeave
    {
        add => base.MouseLeave += value;
        remove => base.MouseLeave -= value;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // BACKCOMPAT requirement:
        //
        // Why the Height/Width < 10 check? This is because Ux-theme doesn't seem to handle those cases similar to
        // what we do for the non-themed case, so if someone is using the GroupBox as a separator, their app will
        // look weird in .NET Framework 2.0. We render the old way in these cases.

        if (!Application.RenderWithVisualStyles || Width < 10 || Height < 10)
        {
            DrawGroupBox(e);
        }
        else
        {
            GroupBoxState gbState = Enabled ? GroupBoxState.Normal : GroupBoxState.Disabled;
            TextFormatFlags textFlags = TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak
                | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;

            if (!ShowKeyboardCues)
            {
                textFlags |= TextFormatFlags.HidePrefix;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                textFlags |= (TextFormatFlags.Right | TextFormatFlags.RightToLeft);
            }

            // We only pass in the text color if it is explicitly set, else we let the renderer use the color
            // specified by the theme. This is a temporary workaround till we find a good solution for the
            // "default theme color" issue.
            if (ShouldSerializeForeColor() || IsDarkModeEnabled || !Enabled)
            {
                Color textColor = Enabled ? ForeColor : TextRenderer.DisabledTextColor(BackColor);
                GroupBoxRenderer.DrawGroupBox(
                    e,
                    new Rectangle(0, 0, Width, Height),
                    Text,
                    Font,
                    textColor,
                    textFlags,
                    gbState);
            }
            else
            {
                GroupBoxRenderer.DrawGroupBox(
                    e,
                    new Rectangle(0, 0, Width, Height),
                    Text,
                    Font,
                    textFlags,
                    gbState);
            }
        }

        base.OnPaint(e); // raise paint event
    }

    private void DrawGroupBox(PaintEventArgs e)
    {
        // Offset from the left bound.
        const int TextOffset = 8;

        // Max text bounding box passed to drawing methods to support RTL.
        Rectangle textRectangle = ClientRectangle;
        textRectangle.X += TextOffset;
        textRectangle.Width -= 2 * TextOffset;

        Color backColor = DisabledColor;
        Size textSize;

        if (UseCompatibleTextRendering)
        {
            Graphics graphics = e.GraphicsInternal;
            using var textBrush = ForeColor.GetCachedSolidBrushScope();
            using StringFormat format = new StringFormat
            {
                HotkeyPrefix = ShowKeyboardCues ? HotkeyPrefix.Show : HotkeyPrefix.Hide
            };

            // Adjust string format for Rtl controls

            if (RightToLeft == RightToLeft.Yes)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            textSize = Size.Ceiling(graphics.MeasureString(Text, Font, textRectangle.Width, format));

            if (Enabled)
            {
                graphics.DrawString(Text, Font, textBrush, textRectangle, format);
            }
            else
            {
                ControlPaint.DrawStringDisabled(graphics, Text, Font, backColor, textRectangle, format);
            }
        }
        else
        {
            using DeviceContextHdcScope hdc = new(e);

            DRAW_TEXT_FORMAT flags = DRAW_TEXT_FORMAT.DT_WORDBREAK | DRAW_TEXT_FORMAT.DT_EDITCONTROL;

            if (!ShowKeyboardCues)
            {
                flags |= DRAW_TEXT_FORMAT.DT_HIDEPREFIX;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                flags |= DRAW_TEXT_FORMAT.DT_RTLREADING;
                flags |= DRAW_TEXT_FORMAT.DT_RIGHT;
            }

            using var hfont = GdiCache.GetHFONT(Font);
            textSize = hdc.HDC.MeasureText(Text, hfont, new Size(textRectangle.Width, int.MaxValue), (TextFormatFlags)flags);

            if (Enabled)
            {
                hdc.HDC.DrawText(Text, hfont, textRectangle, ForeColor, (TextFormatFlags)flags);
            }
            else
            {
                ControlPaint.DrawStringDisabled(
                    hdc,
                    Text,
                    Font,
                    backColor,
                    textRectangle,
                    (TextFormatFlags)flags);
            }
        }

        int textLeft = TextOffset;    // Left side of binding box (independent on RTL).

        if (RightToLeft == RightToLeft.Yes)
        {
            textLeft += textRectangle.Width - textSize.Width;
        }

        // Math.Min to assure we paint at least a small line.
        int textRight = Math.Min(textLeft + textSize.Width, Width - 6);

        int boxTop = FontHeight / 2;

        if (SystemInformation.HighContrast)
        {
            Color boxColor = Enabled ? ForeColor : Application.SystemColors.GrayText;

            ReadOnlySpan<int> lines =
            [
                0, boxTop, 0, Height,                       // Left
                0, Height - 1, Width, Height - 1,           // Bottom
                0, boxTop, textLeft, boxTop,                // Top-left
                textRight, boxTop, Width - 1, boxTop,       // Top-right
                Width - 1, boxTop, Width - 1, Height - 1    // Right
            ];

            if (boxColor.HasTransparency())
            {
                Graphics graphics = e.GraphicsInternal;
                using var boxPen = boxColor.GetCachedPenScope();
                graphics.DrawLines(boxPen, lines);
            }
            else
            {
                using DeviceContextHdcScope hdc = new(e);
                using CreatePenScope hpen = new(boxColor);
                hdc.DrawLines(hpen, lines);
            }
        }
        else
        {
            ReadOnlySpan<int> lightLines =
            [
                1, boxTop, 1, Height - 1,                       // Left
                0, Height - 1, Width, Height - 1,               // Bottom
                1, boxTop, textLeft, boxTop,                    // Top-left
                textRight, boxTop, Width - 1, boxTop,           // Top-right
                Width - 1, boxTop - 1, Width - 1, Height - 1    // Right
            ];

            ReadOnlySpan<int> darkLines =
            [
                0, boxTop, 0, Height - 2,                       // Left
                0, Height - 2, Width - 1, Height - 2,           // Bottom
                0, boxTop - 1, textLeft, boxTop - 1,            // Top-left
                textRight, boxTop - 1, Width - 2, boxTop - 1,   // Top-right
                Width - 2, boxTop - 1, Width - 2, Height - 2    // Right
            ];

            using DeviceContextHdcScope hdc = new(e);
            using CreatePenScope hpenLight = new(ControlPaint.Light(backColor, 1.0f));
            hdc.DrawLines(hpenLight, lightLines);
            using CreatePenScope hpenDark = new(ControlPaint.Dark(backColor, 0f));
            hdc.DrawLines(hpenDark, darkLines);
        }
    }

    internal override Size GetPreferredSizeCore(Size proposedSize)
    {
        // Translating 0,0 from ClientSize to actual Size tells us how much space is required for the borders.
        Size borderSize = SizeFromClientSize(Size.Empty);
        Size totalPadding = borderSize + new Size(0, _fontHeight) + Padding.Size;

        Size prefSize = LayoutEngine.GetPreferredSize(this, proposedSize - totalPadding);
        return prefSize + totalPadding;
    }

    protected override void OnFontChanged(EventArgs e)
    {
        _fontHeight = -1;
        _cachedFont = null;
        Invalidate();
        base.OnFontChanged(e);
    }

    /// <summary>
    ///  We use this to process mnemonics and send them on to the first child
    ///  control.
    /// </summary>
    protected internal override bool ProcessMnemonic(char charCode)
    {
        if (IsMnemonic(charCode, Text) && CanProcessMnemonic())
        {
            SelectNextControl(null, true, true, true, false);
            return true;
        }

        return false;
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        if (factor.Width != 1F && factor.Height != 1F)
        {
            // Make sure when we're scaling by non-unity to clear the font cache
            // as the font has likely changed, but we don't know it yet as OnFontChanged has yet to
            // be called on us by our parent.
            _fontHeight = -1;
            _cachedFont = null;
        }

        base.ScaleControl(factor, specified);
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString() => $"{base.ToString()}, Text: {Text}";

    /// <summary>
    ///  The Windows group box doesn't erase the background so we do it ourselves here.
    /// </summary>
    private void WmEraseBkgnd(ref Message m)
    {
        if (m.WParamInternal == 0u)
        {
            return;
        }

        PInvokeCore.GetClientRect(this, out RECT rect);
        Color backColor = BackColor;

        if (backColor.HasTransparency())
        {
            using Graphics graphics = Graphics.FromHdcInternal((HDC)m.WParamInternal);
            using var brush = backColor.GetCachedSolidBrushScope();
            graphics.FillRectangle(brush, rect);
        }
        else
        {
            using var hbrush = new CreateBrushScope(backColor);
            PInvoke.FillRect((HDC)m.WParamInternal, rect, hbrush);
        }

        m.ResultInternal = (LRESULT)1;
    }

    protected override void WndProc(ref Message m)
    {
        if (OwnerDraw)
        {
            base.WndProc(ref m);
            return;
        }

        switch (m.MsgInternal)
        {
            case PInvoke.WM_ERASEBKGND:
            case PInvoke.WM_PRINTCLIENT:
                WmEraseBkgnd(ref m);
                break;
            case PInvoke.WM_GETOBJECT:
                base.WndProc(ref m);

                // Force MSAA to always treat a group box as a custom window. This ensures its child controls
                // will always be exposed through MSAA. Reason: When FlatStyle=System, we map down to the Win32
                // "Button" window class to get OS group box rendering; but the OS does not expose the children
                // of buttons to MSAA (because it assumes buttons won't have children).
                if (m.LParamInternal == (int)OBJECT_IDENTIFIER.OBJID_QUERYCLASSNAMEIDX)
                {
                    m.ResultInternal = (LRESULT)0;
                }

                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new GroupBoxAccessibleObject(this);
}
