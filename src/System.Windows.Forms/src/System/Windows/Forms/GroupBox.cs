// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Encapsulates a standard Windows group box.
    /// </summary>
    [DefaultEvent(nameof(Enter))]
    [DefaultProperty(nameof(Text))]
    [Designer("System.Windows.Forms.Design.GroupBoxDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionGroupBox))]
    public partial class GroupBox : Control
    {
        private int _fontHeight = -1;
        private Font _cachedFont;
        private FlatStyle _flatStyle = FlatStyle.Standard;

        /// <summary>
        ///  Initializes a new instance of the <see cref='GroupBox'/> class.
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
        new public event EventHandler AutoSizeChanged
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }

                if (GetAutoSizeMode() != value)
                {
                    SetAutoSizeMode(value);
                    if (ParentInternal != null)
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
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!OwnerDraw)
                {
                    cp.ClassName = ComCtl32.WindowClasses.WC_BUTTON;
                    cp.Style |= (int)User32.BS.GROUPBOX;
                }
                else
                {
                    // if we swap back to a different flat style
                    // we need to reset these guys.
                    cp.ClassName = null;
                    cp.Style &= ~(int)User32.BS.GROUPBOX;
                }

                cp.ExStyle |= (int)User32.WS_EX.CONTROLPARENT;

                return cp;
            }
        }

        /// <summary>
        ///  Set the default Padding to 3 so that it is consistent with Everett
        /// </summary>
        protected override Padding DefaultPadding => new Padding(3);

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize => new Size(200, 100);

        /// <summary>
        ///  Gets a rectangle that represents the dimensions of the <see cref='GroupBox'/>
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
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlatStyle.Flat, (int)FlatStyle.System))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlatStyle));
                }

                if (_flatStyle != value)
                {
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
        }

        private bool OwnerDraw => FlatStyle != FlatStyle.System;

        /// <summary>
        ///  Gets or sets a value indicating whether the user may press the TAB key to give the focus to the
        ///  <see cref='GroupBox'/>.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        new public bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        new public event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        [Localizable(true)]
        public override string Text
        {
            get => base.Text;
            set
            {
                // the GroupBox controls immediately draws when teh WM_SETTEXT comes through, but
                // does so in the wrong font, so we suspend that behavior, and then
                // invalidate.
                bool suspendRedraw = Visible;
                try
                {
                    if (suspendRedraw && IsHandleCreated)
                    {
                        User32.SendMessageW(this, User32.WM.SETREDRAW, PARAM.FromBool(false));
                    }
                    base.Text = value;
                }
                finally
                {
                    if (suspendRedraw && IsHandleCreated)
                    {
                        User32.SendMessageW(this, User32.WM.SETREDRAW, PARAM.FromBool(true));
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
            get => UseCompatibleTextRenderingInt;
            set => UseCompatibleTextRenderingInt = value;
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
        public new event EventHandler Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyEventHandler KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyEventHandler KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyPressEventHandler KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDown
        {
            add => base.MouseDown += value;
            remove => base.MouseDown -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseUp
        {
            add => base.MouseUp += value;
            remove => base.MouseUp -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseMove
        {
            add => base.MouseMove += value;
            remove => base.MouseMove -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseEnter
        {
            add => base.MouseEnter += value;
            remove => base.MouseEnter -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseLeave
        {
            add => base.MouseLeave += value;
            remove => base.MouseLeave -= value;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // BACKCOMPAT requirement:
            //
            // Why the Height/Width < 10 check? This is because uxtheme doesn't seem to handle those cases similar to
            // what we do for the non-themed case, so if someone is using the groupbox as a separator, their app will
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
                if (ShouldSerializeForeColor() || Enabled == false)
                {
                    Color textcolor = Enabled ? ForeColor : TextRenderer.DisabledTextColor(BackColor);
                    GroupBoxRenderer.DrawGroupBox(
                        e,
                        new Rectangle(0, 0, Width, Height),
                        Text,
                        Font,
                        textcolor,
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
                using var hdc = new DeviceContextHdcScope(e);

                User32.DT flags = User32.DT.WORDBREAK | User32.DT.EDITCONTROL;

                if (!ShowKeyboardCues)
                {
                    flags |= User32.DT.HIDEPREFIX;
                }

                if (RightToLeft == RightToLeft.Yes)
                {
                    flags |= User32.DT.RTLREADING;
                    flags |= User32.DT.RIGHT;
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
                Color boxColor = Enabled ? ForeColor : SystemColors.GrayText;

                ReadOnlySpan<int> lines = stackalloc int[]
                {
                    0, boxTop, 0, Height,                       // Left
                    0, Height - 1, Width, Height - 1,           // Bottom
                    0, boxTop, textLeft, boxTop,                // Top-left
                    textRight, boxTop, Width - 1, boxTop,       // Top-right
                    Width - 1, boxTop, Width - 1, Height - 1    // Right
                };

                if (boxColor.HasTransparency())
                {
                    Graphics graphics = e.GraphicsInternal;
                    using var boxPen = boxColor.GetCachedPenScope();
                    graphics.DrawLines(boxPen, lines);
                }
                else
                {
                    using var hdc = new DeviceContextHdcScope(e);
                    using var hpen = new Gdi32.CreatePenScope(boxColor);
                    hdc.DrawLines(hpen, lines);
                }
            }
            else
            {
                ReadOnlySpan<int> lightLines = stackalloc int[]
                {
                    1, boxTop, 1, Height - 1,                       // Left
                    0, Height - 1, Width, Height - 1,               // Bottom
                    1, boxTop, textLeft, boxTop,                    // Top-left
                    textRight, boxTop, Width - 1, boxTop,           // Top-right
                    Width - 1, boxTop - 1, Width - 1, Height - 1    // Right
                };

                ReadOnlySpan<int> darkLines = stackalloc int[]
                {
                    0, boxTop, 0, Height - 2,                       // Left
                    0, Height - 2, Width - 1, Height - 2,           // Bottom
                    0, boxTop - 1, textLeft, boxTop - 1,            // Top-left
                    textRight, boxTop - 1, Width - 2, boxTop - 1,   // Top-right
                    Width - 2, boxTop - 1, Width - 2, Height - 2    // Right
                };

                using var hdc = new DeviceContextHdcScope(e);
                using var hpenLight = new Gdi32.CreatePenScope(ControlPaint.Light(backColor, 1.0f));
                hdc.DrawLines(hpenLight, lightLines);
                using var hpenDark = new Gdi32.CreatePenScope(ControlPaint.Dark(backColor, 0f));
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
                // as the font has likely changed, but we dont know it yet as OnFontChanged has yet to
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
            if (m.WParam == IntPtr.Zero)
            {
                return;
            }

            RECT rect = new RECT();
            User32.GetClientRect(this, ref rect);

            Color backColor = BackColor;

            if (backColor.HasTransparency())
            {
                using Graphics graphics = Graphics.FromHdcInternal(m.WParam);
                using var brush = backColor.GetCachedSolidBrushScope();
                graphics.FillRectangle(brush, rect);
            }
            else
            {
                var hdc = (Gdi32.HDC)m.WParam;
                using var hbrush = new Gdi32.CreateBrushScope(backColor);
                User32.FillRect(hdc, ref rect, hbrush);
            }

            m.Result = (IntPtr)1;
        }

        protected override void WndProc(ref Message m)
        {
            if (OwnerDraw)
            {
                base.WndProc(ref m);
                return;
            }

            switch ((User32.WM)m.Msg)
            {
                case User32.WM.ERASEBKGND:
                case User32.WM.PRINTCLIENT:
                    WmEraseBkgnd(ref m);
                    break;
                case User32.WM.GETOBJECT:
                    base.WndProc(ref m);

                    // Force MSAA to always treat a group box as a custom window. This ensures its child controls
                    // will always be exposed through MSAA. Reason: When FlatStyle=System, we map down to the Win32
                    // "Button" window class to get OS group box rendering; but the OS does not expose the children
                    // of buttons to MSAA (beacuse it assumes buttons won't have children).
                    if (unchecked((int)(long)m.LParam) == User32.OBJID.QUERYCLASSNAMEIDX)
                    {
                        m.Result = IntPtr.Zero;
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance() => new GroupBoxAccessibleObject(this);
    }
}
