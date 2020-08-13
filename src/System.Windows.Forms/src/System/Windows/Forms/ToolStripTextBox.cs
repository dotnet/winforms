// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class ToolStripTextBox : ToolStripControlHost
    {
        internal static readonly object s_eventTextBoxTextAlignChanged = new object();
        internal static readonly object s_eventAcceptsTabChanged = new object();
        internal static readonly object s_eventBorderStyleChanged = new object();
        internal static readonly object s_eventHideSelectionChanged = new object();
        internal static readonly object s_eventReadOnlyChanged = new object();
        internal static readonly object s_eventMultilineChanged = new object();
        internal static readonly object s_eventModifiedChanged = new object();

        private static readonly Padding s_defaultMargin = new Padding(1, 0, 1, 0);
        private static readonly Padding s_defaultDropDownMargin = new Padding(1);
        private Padding _scaledDefaultMargin = s_defaultMargin;
        private Padding _scaledDefaultDropDownMargin = s_defaultDropDownMargin;

        public ToolStripTextBox() : base(CreateControlInstance())
        {
            ToolStripTextBoxControl textBox = Control as ToolStripTextBoxControl;
            textBox.Owner = this;

            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(s_defaultMargin);
                _scaledDefaultDropDownMargin = DpiHelper.LogicalToDeviceUnits(s_defaultDropDownMargin);
            }
        }

        public ToolStripTextBox(string name) : this()
        {
            Name = name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripTextBox(Control c) : base(c)
        {
            throw new NotSupportedException(SR.ToolStripMustSupplyItsOwnTextBox);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected internal override Padding DefaultMargin
        {
            get
            {
                if (IsOnDropDown)
                {
                    return _scaledDefaultDropDownMargin;
                }
                else
                {
                    return _scaledDefaultMargin;
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 22);
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextBox TextBox
        {
            get
            {
                return Control as TextBox;
            }
        }

        private static Control CreateControlInstance()
        {
            TextBox textBox = new ToolStripTextBoxControl
            {
                BorderStyle = BorderStyle.Fixed3D,
                AutoSize = true
            };
            return textBox;
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            // dont call TextBox.GPS because it will grow and shrink as the text changes.
            Rectangle bounds = CommonProperties.GetSpecifiedBounds(TextBox);
            return new Size(bounds.Width, TextBox.PreferredHeight);
        }
        private void HandleAcceptsTabChanged(object sender, EventArgs e)
        {
            OnAcceptsTabChanged(e);
        }
        private void HandleBorderStyleChanged(object sender, EventArgs e)
        {
            OnBorderStyleChanged(e);
        }
        private void HandleHideSelectionChanged(object sender, EventArgs e)
        {
            OnHideSelectionChanged(e);
        }
        private void HandleModifiedChanged(object sender, EventArgs e)
        {
            OnModifiedChanged(e);
        }
        private void HandleMultilineChanged(object sender, EventArgs e)
        {
            OnMultilineChanged(e);
        }
        private void HandleReadOnlyChanged(object sender, EventArgs e)
        {
            OnReadOnlyChanged(e);
        }
        private void HandleTextBoxTextAlignChanged(object sender, EventArgs e)
        {
            RaiseEvent(s_eventTextBoxTextAlignChanged, e);
        }
        protected virtual void OnAcceptsTabChanged(EventArgs e)
        {
            RaiseEvent(s_eventAcceptsTabChanged, e);
        }
        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            RaiseEvent(s_eventBorderStyleChanged, e);
        }
        protected virtual void OnHideSelectionChanged(EventArgs e)
        {
            RaiseEvent(s_eventHideSelectionChanged, e);
        }
        protected virtual void OnModifiedChanged(EventArgs e)
        {
            RaiseEvent(s_eventModifiedChanged, e);
        }
        protected virtual void OnMultilineChanged(EventArgs e)
        {
            RaiseEvent(s_eventMultilineChanged, e);
        }
        protected virtual void OnReadOnlyChanged(EventArgs e)
        {
            RaiseEvent(s_eventReadOnlyChanged, e);
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            if (control is TextBox textBox)
            {
                // Please keep this alphabetized and in sync with Unsubscribe
                //
                textBox.AcceptsTabChanged += new EventHandler(HandleAcceptsTabChanged);
                textBox.BorderStyleChanged += new EventHandler(HandleBorderStyleChanged);
                textBox.HideSelectionChanged += new EventHandler(HandleHideSelectionChanged);
                textBox.ModifiedChanged += new EventHandler(HandleModifiedChanged);
                textBox.MultilineChanged += new EventHandler(HandleMultilineChanged);
                textBox.ReadOnlyChanged += new EventHandler(HandleReadOnlyChanged);
                textBox.TextAlignChanged += new EventHandler(HandleTextBoxTextAlignChanged);
            }

            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            if (control is TextBox textBox)
            {
                // Please keep this alphabetized and in sync with Subscribe
                //
                textBox.AcceptsTabChanged -= new EventHandler(HandleAcceptsTabChanged);
                textBox.BorderStyleChanged -= new EventHandler(HandleBorderStyleChanged);
                textBox.HideSelectionChanged -= new EventHandler(HandleHideSelectionChanged);
                textBox.ModifiedChanged -= new EventHandler(HandleModifiedChanged);
                textBox.MultilineChanged -= new EventHandler(HandleMultilineChanged);
                textBox.ReadOnlyChanged -= new EventHandler(HandleReadOnlyChanged);
                textBox.TextAlignChanged -= new EventHandler(HandleTextBoxTextAlignChanged);
            }
            base.OnUnsubscribeControlEvents(control);
        }

        internal override bool ShouldSerializeFont()
        {
            return Font != ToolStripManager.DefaultFont;
        }

        #region WrappedProperties
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxAcceptsTabDescr))]
        public bool AcceptsTab
        {
            get { return TextBox.AcceptsTab; }
            set { TextBox.AcceptsTab = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxAcceptsReturnDescr))]
        public bool AcceptsReturn
        {
            get { return TextBox.AcceptsReturn; }
            set { TextBox.AcceptsReturn = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr))]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return TextBox.AutoCompleteCustomSource; }
            set { TextBox.AutoCompleteCustomSource = value; }
        }

        [DefaultValue(AutoCompleteMode.None)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return TextBox.AutoCompleteMode; }
            set { TextBox.AutoCompleteMode = value; }
        }

        [DefaultValue(AutoCompleteSource.None)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteSourceDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return TextBox.AutoCompleteSource; }
            set { TextBox.AutoCompleteSource = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.Fixed3D)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.TextBoxBorderDescr))]
        public BorderStyle BorderStyle
        {
            get => TextBox.BorderStyle;
            set => TextBox.BorderStyle = value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxCanUndoDescr))]
        public bool CanUndo
        {
            get { return TextBox.CanUndo; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(CharacterCasing.Normal)]
        [SRDescription(nameof(SR.TextBoxCharacterCasingDescr))]
        public CharacterCasing CharacterCasing
        {
            get { return TextBox.CharacterCasing; }
            set { TextBox.CharacterCasing = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxHideSelectionDescr))]
        public bool HideSelection
        {
            get { return TextBox.HideSelection; }
            set { TextBox.HideSelection = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxLinesDescr))]
        [Editor("System.Windows.Forms.Design.StringArrayEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public string[] Lines
        {
            get { return TextBox.Lines; }
            set { TextBox.Lines = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(32767)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxMaxLengthDescr))]
        public int MaxLength
        {
            get { return TextBox.MaxLength; }
            set { TextBox.MaxLength = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxModifiedDescr))]
        public bool Modified
        {
            get { return TextBox.Modified; }
            set { TextBox.Modified = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxMultilineDescr))]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Multiline
        {
            get { return TextBox.Multiline; }
            set { TextBox.Multiline = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxReadOnlyDescr))]
        public bool ReadOnly
        {
            get { return TextBox.ReadOnly; }
            set { TextBox.ReadOnly = value; }
        }
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectedTextDescr))]
        public string SelectedText
        {
            get { return TextBox.SelectedText; }
            set { TextBox.SelectedText = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectionLengthDescr))]
        public int SelectionLength
        {
            get { return TextBox.SelectionLength; }
            set { TextBox.SelectionLength = value; }
        }
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectionStartDescr))]
        public int SelectionStart
        {
            get { return TextBox.SelectionStart; }
            set { TextBox.SelectionStart = value; }
        }
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxShortcutsEnabledDescr))]
        public bool ShortcutsEnabled
        {
            get { return TextBox.ShortcutsEnabled; }
            set { TextBox.ShortcutsEnabled = value; }
        }
        [Browsable(false)]
        public int TextLength
        {
            get { return TextBox.TextLength; }
        }

        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(HorizontalAlignment.Left)]
        [SRDescription(nameof(SR.TextBoxTextAlignDescr))]
        public HorizontalAlignment TextBoxTextAlign
        {
            get { return TextBox.TextAlign; }
            set { TextBox.TextAlign = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [Localizable(true)]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxWordWrapDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool WordWrap
        {
            get { return TextBox.WordWrap; }
            set { TextBox.WordWrap = value; }
        }

        #endregion WrappedProperties

        #region WrappedEvents
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnAcceptsTabChangedDescr))]
        public event EventHandler AcceptsTabChanged
        {
            add => Events.AddHandler(s_eventAcceptsTabChanged, value);
            remove => Events.RemoveHandler(s_eventAcceptsTabChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnBorderStyleChangedDescr))]
        public event EventHandler BorderStyleChanged
        {
            add => Events.AddHandler(s_eventBorderStyleChanged, value);
            remove => Events.RemoveHandler(s_eventBorderStyleChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnHideSelectionChangedDescr))]
        public event EventHandler HideSelectionChanged
        {
            add => Events.AddHandler(s_eventHideSelectionChanged, value);
            remove => Events.RemoveHandler(s_eventHideSelectionChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnModifiedChangedDescr))]
        public event EventHandler ModifiedChanged
        {
            add => Events.AddHandler(s_eventModifiedChanged, value);
            remove => Events.RemoveHandler(s_eventModifiedChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnMultilineChangedDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler MultilineChanged
        {
            add => Events.AddHandler(s_eventMultilineChanged, value);
            remove => Events.RemoveHandler(s_eventMultilineChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnReadOnlyChangedDescr))]
        public event EventHandler ReadOnlyChanged
        {
            add => Events.AddHandler(s_eventReadOnlyChanged, value);
            remove => Events.RemoveHandler(s_eventReadOnlyChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ToolStripTextBoxTextBoxTextAlignChangedDescr))]
        public event EventHandler TextBoxTextAlignChanged
        {
            add => Events.AddHandler(s_eventTextBoxTextAlignChanged, value);
            remove => Events.RemoveHandler(s_eventTextBoxTextAlignChanged, value);
        }
        #endregion WrappedEvents

        #region WrappedMethods
        public void AppendText(string text) { TextBox.AppendText(text); }
        public void Clear() { TextBox.Clear(); }
        public void ClearUndo() { TextBox.ClearUndo(); }
        public void Copy() { TextBox.Copy(); }
        public void Cut() { TextBox.Copy(); }
        public void DeselectAll() { TextBox.DeselectAll(); }
        public char GetCharFromPosition(Point pt) { return TextBox.GetCharFromPosition(pt); }
        public int GetCharIndexFromPosition(Point pt) { return TextBox.GetCharIndexFromPosition(pt); }
        public int GetFirstCharIndexFromLine(int lineNumber) { return TextBox.GetFirstCharIndexFromLine(lineNumber); }
        public int GetFirstCharIndexOfCurrentLine() { return TextBox.GetFirstCharIndexOfCurrentLine(); }
        public int GetLineFromCharIndex(int index) { return TextBox.GetLineFromCharIndex(index); }
        public Point GetPositionFromCharIndex(int index) { return TextBox.GetPositionFromCharIndex(index); }
        public void Paste() { TextBox.Paste(); }
        public void ScrollToCaret() { TextBox.ScrollToCaret(); }
        public void Select(int start, int length) { TextBox.Select(start, length); }
        public void SelectAll() { TextBox.SelectAll(); }
        public void Undo() { TextBox.Undo(); }
        #endregion
        private class ToolStripTextBoxControl : TextBox
        {
            private bool _mouseIsOver;
            private bool _isFontSet = true;
            private bool _alreadyHooked;

            public ToolStripTextBoxControl()
            {
                // required to make the text box height match the combo.
                Font = ToolStripManager.DefaultFont;
                _isFontSet = false;
            }

            // returns the distance from the client rect to the upper left hand corner of the control
            private RECT AbsoluteClientRECT
            {
                get
                {
                    RECT rect = new RECT();
                    CreateParams cp = CreateParams;

                    AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);

                    // the coordinates we get back are negative, we need to translate this back to positive.
                    int offsetX = -rect.left; // one to get back to 0,0, another to translate
                    int offsetY = -rect.top;

                    // fetch the client rect, then apply the offset.
                    User32.GetClientRect(new HandleRef(this, Handle), ref rect);

                    rect.left += offsetX;
                    rect.right += offsetX;
                    rect.top += offsetY;
                    rect.bottom += offsetY;

                    return rect;
                }
            }
            private Rectangle AbsoluteClientRectangle
            {
                get
                {
                    RECT rect = AbsoluteClientRECT;
                    return Rectangle.FromLTRB(rect.top, rect.top, rect.right, rect.bottom);
                }
            }

            private ProfessionalColorTable ColorTable
            {
                get
                {
                    if (Owner != null)
                    {
                        if (Owner.Renderer is ToolStripProfessionalRenderer renderer)
                        {
                            return renderer.ColorTable;
                        }
                    }
                    return ProfessionalColors.ColorTable;
                }
            }

            private bool IsPopupTextBox
            {
                get
                {
                    return ((BorderStyle == BorderStyle.Fixed3D) &&
                             (Owner != null && (Owner.Renderer is ToolStripProfessionalRenderer)));
                }
            }

            internal bool MouseIsOver
            {
                get { return _mouseIsOver; }
                set
                {
                    if (_mouseIsOver != value)
                    {
                        _mouseIsOver = value;
                        if (!Focused)
                        {
                            InvalidateNonClient();
                        }
                    }
                }
            }

            public override Font Font
            {
                get => base.Font;
                set
                {
                    base.Font = value;
                    _isFontSet = ShouldSerializeFont();
                }
            }

            public ToolStripTextBox Owner { get; set; }

            internal override bool SupportsUiaProviders => true;

            private unsafe void InvalidateNonClient()
            {
                if (!IsPopupTextBox)
                {
                    return;
                }

                RECT absoluteClientRectangle = AbsoluteClientRECT;

                // Get the total client area, then exclude the client by using XOR
                using var hTotalRegion = new Gdi32.RegionScope(0, 0, Width, Height);
                using var hClientRegion = new Gdi32.RegionScope(
                    absoluteClientRectangle.left,
                    absoluteClientRectangle.top,
                    absoluteClientRectangle.right,
                    absoluteClientRectangle.bottom);
                using var hNonClientRegion = new Gdi32.RegionScope(0, 0, 0, 0);

                Gdi32.CombineRgn(hNonClientRegion, hTotalRegion, hClientRegion, Gdi32.RGN.XOR);

                // Call RedrawWindow with the region.
                User32.RedrawWindow(
                    new HandleRef(this, Handle),
                    null,
                    hNonClientRegion,
                    User32.RDW.INVALIDATE | User32.RDW.ERASE | User32.RDW.UPDATENOW
                        | User32.RDW.ERASENOW | User32.RDW.FRAME);
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                InvalidateNonClient();
            }

            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                InvalidateNonClient();
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                MouseIsOver = true;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                MouseIsOver = false;
            }

            private void HookStaticEvents(bool hook)
            {
                if (hook)
                {
                    if (!_alreadyHooked)
                    {
                        try
                        {
                            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                        }
                        finally
                        {
                            _alreadyHooked = true;
                        }
                    }
                }
                else if (_alreadyHooked)
                {
                    try
                    {
                        SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                    }
                    finally
                    {
                        _alreadyHooked = false;
                    }
                }
            }

            private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
            {
                if (e.Category == UserPreferenceCategory.Window)
                {
                    if (!_isFontSet)
                    {
                        Font = ToolStripManager.DefaultFont;
                    }
                }
            }

            protected override void OnVisibleChanged(EventArgs e)
            {
                base.OnVisibleChanged(e);
                if (!Disposing && !IsDisposed)
                {
                    HookStaticEvents(Visible);
                }
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new ToolStripTextBoxControlAccessibleObject(this, Owner);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    HookStaticEvents(false);
                }
                base.Dispose(disposing);
            }

            private void WmNCPaint(ref Message m)
            {
                if (!IsPopupTextBox)
                {
                    base.WndProc(ref m);
                    return;
                }

                // Paint over the edges of the text box.

                // Note that GetWindowDC just calls GetDCEx with DCX_WINDOW | DCX_USESTYLE.

                using var hdc = new User32.GetDcScope(m.HWnd, IntPtr.Zero, User32.DCX.WINDOW | User32.DCX.USESTYLE);
                if (hdc.IsNull)
                {
                    throw new Win32Exception();
                }

                // Don't set the clipping region based on the WParam - windows seems to take out the two pixels intended for the non-client border.

                Color outerBorderColor = (MouseIsOver || Focused) ? ColorTable.TextBoxBorder : BackColor;
                Color innerBorderColor = BackColor;

                if (!Enabled)
                {
                    outerBorderColor = SystemColors.ControlDark;
                    innerBorderColor = SystemColors.Control;
                }

                using Graphics g = hdc.CreateGraphics();
                Rectangle clientRect = AbsoluteClientRectangle;

                // Could have set up a clip and fill-rectangled, thought this would be faster.
                using var brush = innerBorderColor.GetCachedSolidBrushScope();
                g.FillRectangle(brush, 0, 0, Width, clientRect.Top);                                // top border
                g.FillRectangle(brush, 0, 0, clientRect.Left, Height);                              // left border
                g.FillRectangle(brush, 0, clientRect.Bottom, Width, Height - clientRect.Height);    // bottom border
                g.FillRectangle(brush, clientRect.Right, 0, Width - clientRect.Right, Height);      // right border

                // Paint the outside rect.
                using var pen = outerBorderColor.GetCachedPenScope();
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

                // We've handled WM_NCPAINT.
                m.Result = IntPtr.Zero;
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.WM.NCPAINT)
                {
                    WmNCPaint(ref m);
                    return;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }

        private class ToolStripTextBoxControlAccessibleObject : ToolStripHostedControlAccessibleObject
        {
            public ToolStripTextBoxControlAccessibleObject(Control toolStripHostedControl, ToolStripControlHost toolStripControlHost) : base(toolStripHostedControl, toolStripControlHost)
            {
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                       return UiaCore.UIA.EditControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                       return Name;
            }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }
        }
    }
}
