// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Layout;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements the basic functionality required by text
    ///  controls.
    /// </summary>
    [DefaultEvent(nameof(TextChanged))]
    [DefaultBindingProperty(nameof(Text))]
    [Designer("System.Windows.Forms.Design.TextBoxBaseDesigner, " + AssemblyRef.SystemDesign)]
    public abstract partial class TextBoxBase : Control
    {
        // The boolean properties for this control are contained in the textBoxFlags bit
        // vector.  We can store up to 32 boolean values in this one vector.  Here we
        // create the bitmasks for each bit in the vector.
        //
        private static readonly int autoSize = BitVector32.CreateMask();
        private static readonly int hideSelection = BitVector32.CreateMask(autoSize);
        private static readonly int multiline = BitVector32.CreateMask(hideSelection);
        private static readonly int modified = BitVector32.CreateMask(multiline);
        private static readonly int readOnly = BitVector32.CreateMask(modified);
        private static readonly int acceptsTab = BitVector32.CreateMask(readOnly);
        private static readonly int wordWrap = BitVector32.CreateMask(acceptsTab);
        private static readonly int creatingHandle = BitVector32.CreateMask(wordWrap);
        private static readonly int codeUpdateText = BitVector32.CreateMask(creatingHandle);
        private static readonly int shortcutsEnabled = BitVector32.CreateMask(codeUpdateText);
        private static readonly int scrollToCaretOnHandleCreated = BitVector32.CreateMask(shortcutsEnabled);
        private static readonly int setSelectionOnHandleCreated = BitVector32.CreateMask(scrollToCaretOnHandleCreated);

        private static readonly object EVENT_ACCEPTSTABCHANGED = new object();
        private static readonly object EVENT_BORDERSTYLECHANGED = new object();
        private static readonly object EVENT_HIDESELECTIONCHANGED = new object();
        private static readonly object EVENT_MODIFIEDCHANGED = new object();
        private static readonly object EVENT_MULTILINECHANGED = new object();
        private static readonly object EVENT_READONLYCHANGED = new object();

        /// <summary>
        ///  The current border for this edit control.
        /// </summary>
        private BorderStyle borderStyle = BorderStyle.Fixed3D;

        /// <summary>
        ///  Controls the maximum length of text in the edit control.
        ///  Matches the Windows limit.
        /// </summary>
        private int maxLength = 32767;

        /// <summary>
        ///  Used by the autoSizing code to help figure out the desired height of
        ///  the edit box.
        /// </summary>
        private int requestedHeight;
        bool integralHeightAdjust;

        //these indices are used to cache the values of the selection, by doing this
        //if the handle isn't created yet, we don't force a creation.
        private int selectionStart;
        private int selectionLength;

        /// <summary>
        ///  Controls firing of click event (Left click).
        ///  This is used by TextBox, RichTextBox and MaskedTextBox, code was moved down from TextBox/RichTextBox
        ///  but cannot make it as default behavior to avoid introducing breaking changes.
        /// </summary>
        private bool doubleClickFired;

        private static int[] shortcutsToDisable;

        // We store all boolean properties in here.
        //
        private BitVector32 textBoxFlags;

        /// <summary>
        ///  Creates a new TextBox control.  Uses the parent's current font and color
        ///  set.
        /// </summary>
        internal TextBoxBase() : base()
        {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

            textBoxFlags[autoSize | hideSelection | wordWrap | shortcutsEnabled] = true;
            SetStyle(ControlStyles.FixedHeight, textBoxFlags[autoSize]);
            SetStyle(ControlStyles.StandardClick
                    | ControlStyles.StandardDoubleClick
                    | ControlStyles.UseTextForAccessibility
                    | ControlStyles.UserPaint, false);

            // cache requestedHeight. Note: Control calls DefaultSize (overridable) in the constructor
            // to set the control's cached height that is returned when calling Height, so we just
            // need to get the cached height here.
            requestedHeight = Height;
        }

        /// <summary>
        ///  Gets or sets
        ///  a value indicating whether pressing the TAB key
        ///  in a multiline text box control types
        ///  a TAB character in the control instead of moving the focus to the next control
        ///  in the tab order.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxAcceptsTabDescr))]
        public bool AcceptsTab
        {
            get
            {
                return textBoxFlags[acceptsTab];
            }
            set
            {
                if (textBoxFlags[acceptsTab] != value)
                {
                    textBoxFlags[acceptsTab] = value;
                    OnAcceptsTabChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnAcceptsTabChangedDescr))]
        public event EventHandler AcceptsTabChanged
        {
            add => Events.AddHandler(EVENT_ACCEPTSTABCHANGED, value);
            remove => Events.RemoveHandler(EVENT_ACCEPTSTABCHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the following shortcuts should be enabled or not:
        ///  Ctrl-Z, Ctrl-C, Ctrl-X, Ctrl-V, Ctrl-A, Ctrl-L, Ctrl-R, Ctrl-E, Ctrl-I, Ctrl-Y,
        ///  Ctrl-BackSpace, Ctrl-Del, Shift-Del, Shift-Ins.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxShortcutsEnabledDescr))]
        public virtual bool ShortcutsEnabled
        {
            get
            {
                return textBoxFlags[shortcutsEnabled];
            }
            set
            {
                if (shortcutsToDisable is null)
                {
                    shortcutsToDisable = new int[] {(int)Shortcut.CtrlZ, (int)Shortcut.CtrlC, (int)Shortcut.CtrlX,
                    (int)Shortcut.CtrlV, (int)Shortcut.CtrlA, (int)Shortcut.CtrlL, (int)Shortcut.CtrlR,
                    (int)Shortcut.CtrlE, (int)Shortcut.CtrlY, (int)Keys.Control + (int)Keys.Back,
                    (int)Shortcut.CtrlDel, (int)Shortcut.ShiftDel, (int)Shortcut.ShiftIns, (int)Shortcut.CtrlJ};
                }
                textBoxFlags[shortcutsEnabled] = value;
            }
        }

        /// <summary>
        ///  Implements the <see cref='ShortcutsEnabled'/> property.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // First call parent's ProcessCmdKey, since we don't to eat up
            // the shortcut key we are not supported in TextBox.
            bool returnedValue = base.ProcessCmdKey(ref msg, keyData);

            if (ShortcutsEnabled == false)
            {
                foreach (int shortcutValue in shortcutsToDisable)
                {
                    if ((int)keyData == shortcutValue ||
                        (int)keyData == (shortcutValue | (int)Keys.Shift))
                    {
                        return true;
                    }
                }
            }
            //
            // There are a few keys that change the alignment of the text, but that
            // are not ignored by the native control when the ReadOnly property is set.
            // We need to workaround that.
            if (textBoxFlags[readOnly])
            {
                int k = (int)keyData;
                if (k == (int)Shortcut.CtrlL        // align left
                    || k == (int)Shortcut.CtrlR     // align right
                    || k == (int)Shortcut.CtrlE     // align centre
                    || k == (int)Shortcut.CtrlJ)
                {  // align justified
                    return true;
                }
            }

            if (!ReadOnly && (keyData == (Keys.Control | Keys.Back) || keyData == (Keys.Control | Keys.Shift | Keys.Back)))
            {
                if (SelectionLength != 0)
                {
                    SetSelectedTextInternal(string.Empty, clearUndo: false);
                }
                else if (SelectionStart != 0)
                {
                    int boundaryStart = ClientUtils.GetWordBoundaryStart(Text, SelectionStart);
                    int length = SelectionStart - boundaryStart;
                    BeginUpdateInternal();
                    SelectionStart = boundaryStart;
                    SelectionLength = length;
                    EndUpdateInternal();
                    SetSelectedTextInternal(string.Empty, clearUndo: false);
                }
                return true;
            }

            return returnedValue;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the size
        ///  of the control automatically adjusts when the font assigned to the control
        ///  is changed.
        ///
        ///  Note: this works differently than other Controls' AutoSize, so we're hiding
        ///  it to avoid confusion.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxAutoSizeDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoSize
        {
            get
            {
                return textBoxFlags[autoSize];
            }
            set
            {
                // Note that we intentionally do not call base.  TextBoxes size themselves by
                // overriding SetBoundsCore (old RTM code).  We let CommonProperties.GetAutoSize
                // continue to return false to keep our LayoutEngines from messing with TextBoxes.
                // This is done for backwards compatibility since the new AutoSize behavior differs.
                if (textBoxFlags[autoSize] != value)
                {
                    textBoxFlags[autoSize] = value;

                    // AutoSize's effects are ignored for a multi-line textbox
                    //
                    if (!Multiline)
                    {
                        SetStyle(ControlStyles.FixedHeight, value);
                        AdjustHeight(false);
                    }

                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Gets or sets the background color of the control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DispId((int)Ole32.DispatchID.BACKCOLOR)]
        [SRDescription(nameof(SR.ControlBackColorDescr))]
        public override Color BackColor
        {
            get
            {
                if (ShouldSerializeBackColor())
                {
                    return base.BackColor;
                }
                else if (ReadOnly)
                {
                    return SystemColors.Control;
                }
                else
                {
                    return SystemColors.Window;
                }
            }
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the border type
        ///  of the text box control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.Fixed3D)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.TextBoxBorderDescr))]
        public BorderStyle BorderStyle
        {
            get => borderStyle;
            set
            {
                if (borderStyle != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    borderStyle = value;
                    UpdateStyles();
                    RecreateHandle();

                    // PreferredSize depends on BorderStyle : thru CreateParams.ExStyle in User32!AdjustRectEx.
                    // So when the BorderStyle changes let the parent of this control know about it.
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.BorderStyle))
                    {
                        OnBorderStyleChanged(EventArgs.Empty);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnBorderStyleChangedDescr))]
        public event EventHandler BorderStyleChanged
        {
            add => Events.AddHandler(EVENT_BORDERSTYLECHANGED, value);
            remove => Events.RemoveHandler(EVENT_BORDERSTYLECHANGED, value);
        }

        internal virtual bool CanRaiseTextChangedEvent
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  Specifies whether the ImeMode can be enabled - See also ImeModeBase.
        /// </summary>
        protected override bool CanEnableIme
        {
            get
            {
                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CanEnableIme(), this = " + this);
                Debug.Indent();

                bool canEnable = !(ReadOnly || PasswordProtect) && base.CanEnableIme;

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + canEnable);
                Debug.Unindent();

                return canEnable;
            }
        }

        /// <summary>
        ///  Gets a value
        ///  indicating whether the user can undo the previous operation in a text box control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxCanUndoDescr))]
        public bool CanUndo
        {
            get
            {
                if (IsHandleCreated)
                {
                    bool b;
                    b = unchecked((int)(long)SendMessageW(this, (WM)EM.CANUNDO)) != 0;

                    return b;
                }
                return false;
            }
        }

        /// <summary>
        ///  Returns the parameters needed to create the handle. Inheriting classes
        ///  can override this to provide extra functionality. They should not,
        ///  however, forget to call base.getCreateParams() first to get the struct
        ///  filled up with the basic info.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = ComCtl32.WindowClasses.WC_EDIT;
                cp.Style |= (int)(ES.AUTOHSCROLL | ES.AUTOVSCROLL);
                if (!textBoxFlags[hideSelection])
                {
                    cp.Style |= (int)ES.NOHIDESEL;
                }

                if (textBoxFlags[readOnly])
                {
                    cp.Style |= (int)ES.READONLY;
                }

                cp.Style &= ~(int)WS.BORDER;
                cp.ExStyle &= ~(int)WS_EX.CLIENTEDGE;

                switch (borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= (int)WS_EX.CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= (int)WS.BORDER;
                        break;
                }
                if (textBoxFlags[multiline])
                {
                    cp.Style |= (int)ES.MULTILINE;
                    if (textBoxFlags[wordWrap])
                    {
                        cp.Style &= ~(int)ES.AUTOHSCROLL;
                    }
                }

                return cp;
            }
        }

        /// <summary>
        ///  This property is overridden and hidden from statement completion
        ///  on controls that are based on Win32 Native Controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler Click
        {
            add => base.Click += value;
            remove => base.Click -= value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event MouseEventHandler MouseClick
        {
            add => base.MouseClick += value;
            remove => base.MouseClick -= value;
        }

        protected override Cursor DefaultCursor
        {
            get
            {
                return Cursors.IBeam;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, PreferredHeight);
            }
        }

        /// <summary>
        ///  Gets or sets the foreground color of the control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DispId((int)Ole32.DispatchID.FORECOLOR)]
        [SRDescription(nameof(SR.ControlForeColorDescr))]
        public override Color ForeColor
        {
            get
            {
                if (ShouldSerializeForeColor())
                {
                    return base.ForeColor;
                }
                else
                {
                    return SystemColors.WindowText;
                }
            }
            set => base.ForeColor = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the selected
        ///  text in the text box control remains highlighted when the control loses focus.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxHideSelectionDescr))]
        public bool HideSelection
        {
            get
            {
                return textBoxFlags[hideSelection];
            }

            set
            {
                if (textBoxFlags[hideSelection] != value)
                {
                    textBoxFlags[hideSelection] = value;
                    RecreateHandle();
                    OnHideSelectionChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnHideSelectionChangedDescr))]
        public event EventHandler HideSelectionChanged
        {
            add => Events.AddHandler(EVENT_HIDESELECTIONCHANGED, value);
            remove => Events.RemoveHandler(EVENT_HIDESELECTIONCHANGED, value);
        }

        /// <summary>
        ///  Internal version of ImeMode property.  The ImeMode of TextBoxBase controls depend on its IME restricted
        ///  mode which is determined by the CanEnableIme property which checks whether the control is in Password or
        ///  ReadOnly mode.
        /// </summary>
        protected override ImeMode ImeModeBase
        {
            get
            {
                if (DesignMode)
                {
                    return base.ImeModeBase;
                }

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_ImeModeInternal(), this = " + this);
                Debug.Indent();

                ImeMode imeMode = CanEnableIme ? base.ImeModeBase : ImeMode.Disable;

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + imeMode);
                Debug.Unindent();

                return imeMode;
            }
            set => base.ImeModeBase = value;
        }

        /// <summary>
        ///  Gets or
        ///  sets the lines of text in an text box control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [MergableProperty(false)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxLinesDescr))]
        [Editor("System.Windows.Forms.Design.StringArrayEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public string[] Lines
        {
            get
            {
                string text = Text;
                ArrayList list = new ArrayList();

                int lineStart = 0;
                while (lineStart < text.Length)
                {
                    int lineEnd = lineStart;
                    for (; lineEnd < text.Length; lineEnd++)
                    {
                        char c = text[lineEnd];
                        if (c == '\r' || c == '\n')
                        {
                            break;
                        }
                    }

                    string line = text.Substring(lineStart, lineEnd - lineStart);
                    list.Add(line);

                    // Treat "\r", "\r\n", and "\n" as new lines
                    if (lineEnd < text.Length && text[lineEnd] == '\r')
                    {
                        lineEnd++;
                    }

                    if (lineEnd < text.Length && text[lineEnd] == '\n')
                    {
                        lineEnd++;
                    }

                    lineStart = lineEnd;
                }

                // Corner case -- last character in Text is a new line; need to add blank line to list
                if (text.Length > 0 && (text[text.Length - 1] == '\r' || text[text.Length - 1] == '\n'))
                {
                    list.Add("");
                }

                return (string[])list.ToArray(typeof(string));
            }
            set
            {
                //unparse this string list...
                if (value != null && value.Length > 0)
                {
                    // Using a StringBuilder instead of a String
                    // speeds things up approx 150 times
                    StringBuilder text = new StringBuilder(value[0]);
                    for (int i = 1; i < value.Length; ++i)
                    {
                        text.Append("\r\n");
                        text.Append(value[i]);
                    }
                    Text = text.ToString();
                }
                else
                {
                    Text = string.Empty;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the maximum number of
        ///  characters the user can type into the text box control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(32767)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxMaxLengthDescr))]
        public virtual int MaxLength
        {
            get
            {
                return maxLength;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxLength), value, 0));
                }

                if (maxLength != value)
                {
                    maxLength = value;
                    UpdateMaxLength();
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value that indicates that the text box control has been modified by the user since
        ///  the control was created or its contents were last set.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxModifiedDescr))]
        public bool Modified
        {
            get
            {
                if (IsHandleCreated)
                {
                    bool curState = (0 != unchecked((int)(long)SendMessageW(this, (WM)EM.GETMODIFY)));
                    if (textBoxFlags[modified] != curState)
                    {
                        // Raise ModifiedChanged event.  See WmReflectCommand for more info.
                        textBoxFlags[modified] = curState;
                        OnModifiedChanged(EventArgs.Empty);
                    }
                    return curState;
                }
                else
                {
                    return textBoxFlags[modified];
                }
            }

            set
            {
                if (Modified != value)
                {
                    if (IsHandleCreated)
                    {
                        SendMessageW(this, (WM)EM.SETMODIFY, PARAM.FromBool(value));
                        // Must maintain this state always in order for the
                        // test in the Get method to work properly.
                    }

                    textBoxFlags[modified] = value;
                    OnModifiedChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnModifiedChangedDescr))]
        public event EventHandler ModifiedChanged
        {
            add => Events.AddHandler(EVENT_MODIFIEDCHANGED, value);
            remove => Events.RemoveHandler(EVENT_MODIFIEDCHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether this
        ///  is a multiline text box control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxMultilineDescr))]
        [RefreshProperties(RefreshProperties.All)]
        public virtual bool Multiline
        {
            get
            {
                return textBoxFlags[multiline];
            }
            set
            {
                if (textBoxFlags[multiline] != value)
                {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Multiline))
                    {
                        textBoxFlags[multiline] = value;

                        if (value)
                        {
                            // Multi-line textboxes do not have fixed height
                            //
                            SetStyle(ControlStyles.FixedHeight, false);
                        }
                        else
                        {
                            // Single-line textboxes may have fixed height, depending on AutoSize
                            SetStyle(ControlStyles.FixedHeight, AutoSize);
                        }

                        RecreateHandle();
                        AdjustHeight(false);
                        OnMultilineChanged(EventArgs.Empty);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnMultilineChangedDescr))]
        public event EventHandler MultilineChanged
        {
            add => Events.AddHandler(EVENT_MULTILINECHANGED, value);
            remove => Events.RemoveHandler(EVENT_MULTILINECHANGED, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.ControlOnPaddingChangedDescr))]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  Determines if the control is in password protect mode.  This is overridden in TextBox and
        ///  MaskedTextBox and is false by default so RichTextBox that doesn't support Password doesn't
        ///  have to care about this.
        /// </summary>
        private protected virtual bool PasswordProtect => false;

        /// <summary>
        ///  Returns the preferred
        ///  height for a single-line text box.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxPreferredHeightDescr))]
        public int PreferredHeight
        {
            get
            {
                // COMPAT we must return the same busted height we did in Everett, even
                // if it doesnt take multiline and word wrap into account.  For better accuracy and/or wrapping use
                // GetPreferredSize instead.
                int height = FontHeight;
                if (borderStyle != BorderStyle.None)
                {
                    height += SystemInformation.GetBorderSizeForDpi(_deviceDpi).Height * 4 + 3;
                }
                return height;
            }
        }

        //  GetPreferredSizeCore
        //  This method can return a different value than PreferredHeight!  It properly handles
        //  border style + multiline and wordwrap.

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            // 3px vertical space is required between the text and the border to keep the last
            // line from being clipped.
            // This 3 pixel size was added in everett and we do this to maintain compat.
            // old everett behavior was FontHeight + [SystemInformation.BorderSize.Height * 4 + 3]
            // however the [ ] was only added if borderstyle was not none.
            Size bordersAndPadding = SizeFromClientSize(Size.Empty) + Padding.Size;

            if (BorderStyle != BorderStyle.None)
            {
                bordersAndPadding += new Size(0, 3);
            }

            if (BorderStyle == BorderStyle.FixedSingle)
            {
                // Bump these by 2px to match BorderStyle.Fixed3D - they'll be omitted from the SizeFromClientSize call.
                bordersAndPadding.Width += 2;
                bordersAndPadding.Height += 2;
            }
            // Reduce constraints by border/padding size
            proposedConstraints -= bordersAndPadding;

            // Fit the text to the remaining space.
            // Fixed for .NET Framework 4.0
            TextFormatFlags format = TextFormatFlags.NoPrefix;
            if (!Multiline)
            {
                format |= TextFormatFlags.SingleLine;
            }
            else if (WordWrap)
            {
                format |= TextFormatFlags.WordBreak;
            }
            Size textSize = TextRenderer.MeasureText(Text, Font, proposedConstraints, format);

            // We use this old computation as a lower bound to ensure backwards compatibility.
            textSize.Height = Math.Max(textSize.Height, FontHeight);
            Size preferredSize = textSize + bordersAndPadding;
            return preferredSize;
        }

        /// <summary>
        ///  Get the currently selected text start position and length.  Use this method internally
        ///  to avoid calling SelectionStart + SelectionLength each of which does essentially the
        ///  same (save one message round trip).
        /// </summary>
        internal unsafe void GetSelectionStartAndLength(out int start, out int length)
        {
            int end = 0;

            if (!IsHandleCreated)
            {
                // It is possible that the cached values are no longer valid if the Text has been changed
                // while the control does not have a handle. We need to return valid values.  We also need
                // to keep the old cached values in case the Text is changed again making the cached values
                // valid again.
                AdjustSelectionStartAndEnd(selectionStart, selectionLength, out start, out end, -1);
                length = end - start;
            }
            else
            {
                start = 0;
                int startResult = 0;
                User32.SendMessageW(this, (WM)EM.GETSEL, (IntPtr)(&startResult), ref end);
                start = startResult;

                //Here, we return the max of either 0 or the # returned by
                //the windows call.  This eliminates a problem on nt4 where
                // a huge negative # is being returned.
                //
                start = Math.Max(0, start);
                // ditto for end
                end = Math.Max(0, end);

                length = end - start;
            }

#if DEBUG
            {
                string t = WindowText;
                int len;

                end = start + length - 1;

                if (t is null)
                {
                    len = 0;
                }
                else
                {
                    len = t.Length;
                }

                Debug.Assert(end <= len, "SelectionEnd is outside the set of valid caret positions for the current WindowText (end ="
                                + end + ", WindowText.Length =" + len + ")");
            }
#endif
        }

        /// <summary>
        ///  Gets or sets a value indicating whether text in the text box is read-only.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.TextBoxReadOnlyDescr))]
        public bool ReadOnly
        {
            get
            {
                return textBoxFlags[readOnly];
            }
            set
            {
                if (textBoxFlags[readOnly] != value)
                {
                    textBoxFlags[readOnly] = value;
                    if (IsHandleCreated)
                    {
                        SendMessageW(this, (WM)EM.SETREADONLY, PARAM.FromBool(value));
                    }

                    OnReadOnlyChanged(EventArgs.Empty);

                    VerifyImeRestrictedModeChanged();
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnReadOnlyChangedDescr))]
        public event EventHandler ReadOnlyChanged
        {
            add => Events.AddHandler(EVENT_READONLYCHANGED, value);
            remove => Events.RemoveHandler(EVENT_READONLYCHANGED, value);
        }

        /// <summary>
        ///  The currently selected text in the control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectedTextDescr))]
        public virtual string SelectedText
        {
            get
            {
                GetSelectionStartAndLength(out int selStart, out int selLength);
                return Text.Substring(selStart, selLength);
            }
            set
            {
                SetSelectedTextInternal(value, true);
            }
        }

        /// <summary>
        ///  Replaces the selected text with the one passed in.
        /// </summary>
        internal virtual void SetSelectedTextInternal(string text, bool clearUndo)
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            if (text is null)
            {
                text = string.Empty;
            }

            // The EM_LIMITTEXT message limits only the text the user can enter. It does not affect any text
            // already in the edit control when the message is sent, nor does it affect the length of the text
            // copied to the edit control by the WM_SETTEXT message.
            SendMessageW(this, (WM)EM.LIMITTEXT);

            if (clearUndo)
            {
                SendMessageW(this, (WM)EM.REPLACESEL, IntPtr.Zero, text);
                // For consistency with Text, we clear the modified flag
                SendMessageW(this, (WM)EM.SETMODIFY);
                ClearUndo();
            }
            else
            {
                SendMessageW(this, (WM)EM.REPLACESEL, /*undoable*/ (IntPtr)(-1), text);
            }

            // Re-enable user input.
            SendMessageW(this, (WM)EM.LIMITTEXT, (IntPtr)maxLength);
        }

        /// <summary>
        ///  Gets or sets the number of characters selected in the text
        ///  box.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectionLengthDescr))]
        public virtual int SelectionLength
        {
            get
            {
                GetSelectionStartAndLength(out int start, out int length);

                return length;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SelectionLength), value));
                }

                GetSelectionStartAndLength(out int selStart, out int selLength);

                if (value != selLength)
                {
                    Select(selStart, value);
                }
            }
        }

        /// <summary>
        ///  Gets or sets the starting
        ///  point of text selected in the text
        ///  box.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectionStartDescr))]
        public int SelectionStart
        {
            get
            {
                GetSelectionStartAndLength(out int selStart, out int selLength);

                return selStart;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SelectionStart), value));
                }

                Select(value, SelectionLength);
            }
        }

        /// <summary>
        ///  Gets or sets
        ///  the current text in the text box.
        /// </summary>
        [Localizable(true)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public override string Text
        {
            get => base.Text;
            set
            {
                if (value != base.Text)
                {
                    base.Text = value;
                    if (IsHandleCreated)
                    {
                        // clear the modified flag
                        SendMessageW(this, (WM)EM.SETMODIFY);
                    }
                }
            }
        }

        [Browsable(false)]
        public virtual int TextLength
            // Note: Currently Winforms does not fully support surrogates.  If
            // the text contains surrogate characters this property may return incorrect values.

            => IsHandleCreated ? GetWindowTextLengthW(new HandleRef(this, Handle)) : Text.Length;

        // Since setting the WindowText while the handle is created
        // generates a WM_COMMAND message, we must trap that case
        // and prevent the event from getting fired, or we get
        // double "TextChanged" events.
        //
        internal override string WindowText
        {
            get => base.WindowText;

            set
            {
                if (value is null)
                {
                    value = string.Empty;
                }

                if (!WindowText.Equals(value))
                {
                    textBoxFlags[codeUpdateText] = true;
                    try
                    {
                        base.WindowText = value;
                    }
                    finally
                    {
                        textBoxFlags[codeUpdateText] = false;
                    }
                }
            }
        }

        /// <summary>
        ///  In certain circumstances we might have to force
        ///  text into the window whether or not the text is the same.
        ///  Make this a method on TextBoxBase rather than RichTextBox (which is the only
        ///  control that needs this at this point), since we need to set codeUpdateText.
        /// </summary>
        internal void ForceWindowText(string value)
        {
            if (value is null)
            {
                value = string.Empty;
            }

            textBoxFlags[codeUpdateText] = true;
            try
            {
                if (IsHandleCreated)
                {
                    SetWindowTextW(new HandleRef(this, Handle), value);
                }
                else
                {
                    if (value.Length == 0)
                    {
                        Text = null;
                    }
                    else
                    {
                        Text = value;
                    }
                }
            }
            finally
            {
                textBoxFlags[codeUpdateText] = false;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether a
        ///  multiline text box control automatically wraps words to the beginning of the next
        ///  line when necessary.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Localizable(true)]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxWordWrapDescr))]
        public bool WordWrap
        {
            get
            {
                return textBoxFlags[wordWrap];
            }
            set
            {
                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.WordWrap))
                {
                    if (textBoxFlags[wordWrap] != value)
                    {
                        textBoxFlags[wordWrap] = value;
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        ///  Adjusts the height of a single-line edit control to match the height of
        ///  the control's font.
        /// </summary>
        private void AdjustHeight(bool returnIfAnchored)
        {
            // If we're anchored to two opposite sides of the form, don't adjust the size because
            // we'll lose our anchored size by resetting to the requested width.
            //
            if (returnIfAnchored && (Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom))
            {
                return;
            }

            int saveHeight = requestedHeight;
            try
            {
                if (textBoxFlags[autoSize] && !textBoxFlags[multiline])
                {
                    Height = PreferredHeight;
                }
                else
                {
                    int curHeight = Height;

                    // Changing the font of a multi-line textbox can sometimes cause a painting problem
                    // The only workaround I can find is to size the textbox big enough for the font, and
                    // then restore its correct size.
                    //
                    if (textBoxFlags[multiline])
                    {
                        Height = Math.Max(saveHeight, PreferredHeight + 2); // 2 = fudge factor
                    }

                    integralHeightAdjust = true;
                    try
                    {
                        Height = saveHeight;
                    }
                    finally
                    {
                        integralHeightAdjust = false;
                    }
                }
            }
            finally
            {
                requestedHeight = saveHeight;
            }
        }

        /// <summary>
        ///  Append text to the current text of text box.
        /// </summary>
        public void AppendText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            GetSelectionStartAndLength(out int selStart, out int selLength);

            try
            {
                // This enables you to use SelectionColor to AppendText in color.
                int endOfText = GetEndPosition();

                SelectInternal(endOfText, endOfText, endOfText);
                SelectedText = text;
            }
            finally
            {
                // If AppendText is called when the control is docked and the form is minimized,
                // all the text will scroll to the top and the control will look empty when the
                // form is restored. We work around this by selecting back whatever was originally
                // selected when AppendText was called.
                if (Width == 0 || Height == 0)
                {
                    Select(selStart, selLength);
                }
            }
        }

        /// <summary>
        ///  Clears all text from the text box control.
        /// </summary>
        public void Clear()
        {
            Text = null;
        }

        /// <summary>
        ///  Clears information about the most recent operation
        ///  from the undo buffer of the text box.
        /// </summary>
        public void ClearUndo()
        {
            if (IsHandleCreated)
            {
                SendMessageW(this, (WM)EM.EMPTYUNDOBUFFER);
            }
        }

        /// <summary>
        ///  Copies the current selection in the text box to the Clipboard.
        /// </summary>
        public void Copy() => SendMessageW(this, WM.COPY);

        protected override AccessibleObject CreateAccessibilityInstance() => new TextBoxBaseAccessibleObject(this);

        protected override void CreateHandle()
        {
            // This "creatingHandle" stuff is to avoid property change events
            // when we set the Text property.
            textBoxFlags[creatingHandle] = true;
            try
            {
                base.CreateHandle();

                // send EM_SETSEL message
                SetSelectionOnHandle();
            }
            finally
            {
                textBoxFlags[creatingHandle] = false;
            }
        }

        /// <summary>
        ///  Moves the current selection in the text box to the Clipboard.
        /// </summary>
        public void Cut() => SendMessageW(this, WM.CUT);

        /// <summary>
        ///  Returns the text end position (one past the last input character).  This property is virtual to allow MaskedTextBox
        ///  to set the last input char position as opposed to the last char position which may be a mask character.
        /// </summary>
        internal virtual int GetEndPosition()
        {
            // +1 because RichTextBox has this funny EOF pseudo-character after all the text.
            return IsHandleCreated ? TextLength + 1 : TextLength;
        }

        /// <summary>
        ///  Overridden to handle TAB key.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) != Keys.Alt)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Tab:
                        // Single-line RichEd's want tab characters (see WM_GETDLGCODE),
                        // so we don't ask it
                        return Multiline && textBoxFlags[acceptsTab] && ((keyData & Keys.Control) == 0);
                    case Keys.Escape:
                        if (Multiline)
                        {
                            return false;
                        }

                        break;
                    case Keys.Back:
                        if (!ReadOnly)
                        {
                            return true;
                        }

                        break;
                    case Keys.PageUp:
                    case Keys.PageDown:
                    case Keys.Home:
                    case Keys.End:
                        return true;
                        // else fall through to base
                }
            }
            return base.IsInputKey(keyData);
        }

        /// <summary>
        ///  Overridden to update the newly created handle with the settings of the
        ///  MaxLength and PasswordChar properties.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!IsHandleCreated)
            {
                return;
            }

            // it's likely here that the create params could have changed
            // the border size/etc.
            CommonProperties.xClearPreferredSizeCache(this);
            AdjustHeight(true);

            UpdateMaxLength();
            if (textBoxFlags[modified])
            {
                SendMessageW(this, (WM)EM.SETMODIFY, PARAM.FromBool(true));
            }
            if (textBoxFlags[scrollToCaretOnHandleCreated])
            {
                ScrollToCaret();
                textBoxFlags[scrollToCaretOnHandleCreated] = false;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            textBoxFlags[modified] = Modified;
            textBoxFlags[setSelectionOnHandleCreated] = true;
            // Update text selection cached values to be restored when recreating the handle.
            GetSelectionStartAndLength(out selectionStart, out selectionLength);
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///  Replaces the current selection in the text box with the contents of the Clipboard.
        /// </summary>
        public void Paste() => SendMessageW(this, WM.PASTE);

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "TextBoxBase.ProcessDialogKey [" + keyData.ToString() + "]");
            Keys keyCode = (Keys)keyData & Keys.KeyCode;

            if (keyCode == Keys.Tab && AcceptsTab && (keyData & Keys.Control) != 0)
            {
                // When this control accepts Tabs, Ctrl-Tab is treated exactly like Tab.
                keyData &= ~Keys.Control;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  TextBox / RichTextBox Onpaint.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        protected virtual void OnAcceptsTabChanged(EventArgs e)
        {
            if (Events[EVENT_ACCEPTSTABCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            if (Events[EVENT_BORDERSTYLECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustHeight(false);
        }

        protected virtual void OnHideSelectionChanged(EventArgs e)
        {
            if (Events[EVENT_HIDESELECTIONCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        protected virtual void OnModifiedChanged(EventArgs e)
        {
            if (Events[EVENT_MODIFIEDCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the MouseUp event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent != null)
            {
                Point pt = PointToScreen(mevent.Location);

                if (mevent.Button == MouseButtons.Left)
                {
                    if (!ValidationCancelled && WindowFromPoint(pt) == Handle)
                    {
                        if (!doubleClickFired)
                        {
                            OnClick(mevent);
                            OnMouseClick(mevent);
                        }
                        else
                        {
                            doubleClickFired = false;
                            OnDoubleClick(mevent);
                            OnMouseDoubleClick(mevent);
                        }
                    }

                    doubleClickFired = false;
                }
            }

            base.OnMouseUp(mevent);
        }

        protected virtual void OnMultilineChanged(EventArgs e)
        {
            if (Events[EVENT_MULTILINECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            AdjustHeight(false);
        }

        protected virtual void OnReadOnlyChanged(EventArgs e)
        {
            if (Events[EVENT_READONLYCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            // since AutoSize existed in Everett, (and is the default) we can't
            // relayout the parent when the "preferredsize" of the control changes.
            // this means a multiline = true textbox wont natrually grow in height when
            // the text changes.
            CommonProperties.xClearPreferredSizeCache(this);
            base.OnTextChanged(e);
        }

        /// <summary>
        ///  Returns the character nearest to the given point.
        /// </summary>
        public virtual char GetCharFromPosition(Point pt)
        {
            string t = Text;
            int index = GetCharIndexFromPosition(pt);
            return (index < 0 || index >= t.Length) ? (char)0 : t[index];
        }

        /// <summary>
        ///  Returns the index of the character nearest to the given point.
        /// </summary>
        public virtual int GetCharIndexFromPosition(Point pt)
        {
            int index = (int)User32.SendMessageW(this, (WM)EM.CHARFROMPOS, IntPtr.Zero, PARAM.FromLowHigh(pt.X, pt.Y));
            index = PARAM.LOWORD(index);

            if (index < 0)
            {
                index = 0;
            }
            else
            {
                string t = Text;
                // EM_CHARFROMPOS will return an invalid number if the last character in the RichEdit
                // is a newline.
                //
                if (index >= t.Length)
                {
                    index = Math.Max(t.Length - 1, 0);
                }
            }
            return index;
        }

        /// <summary>
        ///  Returns the number of the line containing a specified character position
        ///  in a textbox. Note that this returns the physical line number
        ///  and not the conceptual line number. For example, if the first conceptual
        ///  line (line number 0) word-wraps and extends to the second line, and if
        ///  you pass the index of a overflowed character, GetLineFromCharIndex would
        ///  return 1 and not 0.
        /// </summary>
        public virtual int GetLineFromCharIndex(int index)
        {
            return (int)(long)SendMessageW(this, (WM)EM.LINEFROMCHAR, (IntPtr)index);
        }

        /// <summary>
        ///  Returns the location of the character at the given index.
        /// </summary>
        public virtual Point GetPositionFromCharIndex(int index)
        {
            if (index < 0 || index >= Text.Length)
            {
                return Point.Empty;
            }

            int i = (int)(long)SendMessageW(this, (WM)EM.POSFROMCHAR, (IntPtr)index);
            return new Point(PARAM.SignedLOWORD(i), PARAM.SignedHIWORD(i));
        }

        /// <summary>
        ///  Returns the index of the first character of a given line. Returns -1 of lineNumber is invalid.
        /// </summary>
        public int GetFirstCharIndexFromLine(int lineNumber)
        {
            if (lineNumber < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber), lineNumber, string.Format(SR.InvalidArgument, nameof(lineNumber), lineNumber));
            }
            return unchecked((int)(long)SendMessageW(this, (WM)EM.LINEINDEX, (IntPtr)lineNumber));
        }

        /// <summary>
        ///  Returns the index of the first character of the line where the caret is.
        /// </summary>
        public int GetFirstCharIndexOfCurrentLine()
        {
            return unchecked((int)(long)SendMessageW(this, (WM)EM.LINEINDEX, (IntPtr)(-1)));
        }

        /// <summary>
        ///  Ensures that the caret is visible in the TextBox window, by scrolling the
        ///  TextBox control surface if necessary.
        /// </summary>
        public unsafe void ScrollToCaret()
        {
            if (!IsHandleCreated)
            {
                textBoxFlags[scrollToCaretOnHandleCreated] = true;
                return;
            }

            if (string.IsNullOrEmpty(WindowText))
            {
                // If there is no text, then there is no place to go.
                return;
            }

            bool scrolled = false;
            IntPtr editOlePtr = IntPtr.Zero;
            try
            {
                if (SendMessageW(this, (WM)Richedit.EM.GETOLEINTERFACE, IntPtr.Zero, ref editOlePtr) != IntPtr.Zero)
                {
                    IntPtr iTextDocument = IntPtr.Zero;
                    Guid iiTextDocumentGuid = typeof(Richedit.ITextDocument).GUID;

                    try
                    {
                        Marshal.QueryInterface(editOlePtr, ref iiTextDocumentGuid, out iTextDocument);

                        if (Marshal.GetObjectForIUnknown(iTextDocument) is Richedit.ITextDocument textDocument)
                        {
                            // When the user calls RichTextBox::ScrollToCaret we want the RichTextBox to show as
                            // much text as possible.
                            // Here is how we do that:
                            // 1. We scroll the RichTextBox all the way to the bottom so the last line of text is the last visible line.
                            // 2. We get the first visible line.
                            // 3. If the first visible line is smaller than the start of the selection, then we are done:
                            //      The selection fits inside the RichTextBox display rectangle.
                            // 4. Otherwise, scroll the selection to the top of the RichTextBox.
                            GetSelectionStartAndLength(out int selStart, out int selLength);
                            int selStartLine = GetLineFromCharIndex(selStart);

                            // 1. Scroll the RichTextBox all the way to the bottom
                            Richedit.ITextRange textRange = textDocument.Range(WindowText.Length - 1, WindowText.Length - 1);
                            textRange.ScrollIntoView(0);   // 0 ==> tomEnd

                            // 2. Get the first visible line.
                            int firstVisibleLine = unchecked((int)(long)SendMessageW(this, (WM)EM.GETFIRSTVISIBLELINE));

                            // 3. If the first visible line is smaller than the start of the selection, we are done;
                            if (firstVisibleLine <= selStartLine)
                            {
                                // we are done
                            }
                            else
                            {
                                // 4. Scroll the selection to the top of the RichTextBox
                                textRange = textDocument.Range(selStart, selStart + selLength);
                                textRange.ScrollIntoView(32);   // 32 ==> tomStart
                            }

                            scrolled = true;
                        }
                    }
                    finally
                    {
                        if (iTextDocument != IntPtr.Zero)
                        {
                            Marshal.Release(iTextDocument);
                        }
                    }
                }
            }
            finally
            {
                if (editOlePtr != IntPtr.Zero)
                {
                    Marshal.Release(editOlePtr);
                }
            }

            if (!scrolled)
            {
                SendMessageW(this, (WM)EM.SCROLLCARET);
            }
        }

        /// <summary>
        ///  Sets the SelectionLength to 0.
        /// </summary>
        public void DeselectAll()
        {
            SelectionLength = 0;
        }

        /// <summary>
        ///  Selects a range of text in the text box.
        /// </summary>
        public void Select(int start, int length)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, string.Format(SR.InvalidArgument, nameof(start), start));
            }

            int textLen = TextLength;

            if (start > textLen)
            {
                //We shouldn't allow positive length if you're starting at the end, but
                //should allow negative length.
                long longLength = Math.Min(0, (long)length + start - textLen);
                if (longLength < int.MinValue)
                {
                    length = int.MinValue;
                }
                else
                {
                    length = (int)longLength;
                }

                start = textLen;
            }

            SelectInternal(start, length, textLen);
        }

        /// <summary>
        ///  Performs the actual select without doing arg checking.
        ///
        ///  Send in -1 for the textLen parameter if you don't have the text
        ///  length cached when calling this method. It will be computed.
        ///  But if you do have it cached, please pass it in. This will avoid
        ///  the expensive call to the TextLength property.
        /// </summary>
        private protected virtual void SelectInternal(int start, int length, int textLen)
        {
            //if our handle is created - send message...
            if (IsHandleCreated)
            {
                AdjustSelectionStartAndEnd(start, length, out int s, out int e, textLen);

                SendMessageW(this, (WM)EM.SETSEL, (IntPtr)s, (IntPtr)e);

                AccessibilityObject?.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
            }
            else
            {
                //otherwise, wait until handle is created to send this message.
                //Store the indices until then...
                selectionStart = start;
                selectionLength = length;
                textBoxFlags[setSelectionOnHandleCreated] = true;
            }
        }

        /// <summary>
        ///  Selects all text in the text box.
        /// </summary>
        public void SelectAll()
        {
            int textLen = TextLength;
            SelectInternal(0, textLen, textLen);
        }

        /// <summary>
        ///  Overrides Control.setBoundsCore to enforce autoSize.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!integralHeightAdjust && height != Height)
            {
                requestedHeight = height;
            }

            if (textBoxFlags[autoSize] && !textBoxFlags[multiline])
            {
                height = PreferredHeight;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        private static void Swap(ref int n1, ref int n2)
        {
            int temp = n2;
            n2 = n1;
            n1 = temp;
        }

        //
        // Send in -1 if you don't have the text length cached
        // when calling this method. It will be computed. If not,
        // please pass in the text length as the last parameter.
        // This will avoid the expensive call to the TextLength
        // property.
        internal void AdjustSelectionStartAndEnd(int selStart, int selLength, out int start, out int end, int textLen)
        {
            start = selStart;
            end = 0;

            if (start <= -1)
            {
                start = -1;
            }
            else
            {
                int textLength;

                if (textLen >= 0)
                {
                    textLength = textLen;
                }
                else
                {
                    textLength = TextLength;
                }

                if (start > textLength)
                {
                    start = textLength;
                }

                checked
                {
                    try
                    {
                        end = start + selLength;
                    }
                    catch (OverflowException)
                    {
                        //Since we overflowed, cap at the max/min value: we'll correct the value below
                        end = start > 0 ? int.MaxValue : int.MinValue;
                    }
                }

                // Make sure end is in range
                if (end < 0)
                {
                    end = 0;
                }
                else if (end > textLength)
                {
                    end = textLength;
                }
            }
        }

        // Called by CreateHandle or OnHandleCreated
        internal void SetSelectionOnHandle()
        {
            Debug.Assert(IsHandleCreated, "Don't call this method until the handle is created.");
            if (textBoxFlags[setSelectionOnHandleCreated])
            {
                textBoxFlags[setSelectionOnHandleCreated] = false;
                AdjustSelectionStartAndEnd(selectionStart, selectionLength, out int start, out int end, -1);
                SendMessageW(this, (WM)EM.SETSEL, (IntPtr)start, (IntPtr)end);
            }
        }

        /// <summary>
        ///  Converts byte offsset to unicode offsets.
        ///  When procssing WM_GETSEL/WM_SETSEL, EDIT control works with byte offsets instead of character positions
        ///  as opposed to RICHEDIT which does it always as character positions.
        ///  This method is used when handling the WM_GETSEL message.
        /// </summary>
        static void ToUnicodeOffsets(string str, ref int start, ref int end)
        {
            Encoding e = Encoding.Default;

            byte[] bytes = e.GetBytes(str);

            bool swap = start > end;
            if (swap)
            {
                Swap(ref start, ref end);
            }

            // Make sure start and end are within the string
            //
            if (start < 0)
            {
                start = 0;
            }
            if (start > bytes.Length)
            {
                start = bytes.Length;
            }
            if (end > bytes.Length)
            {
                end = bytes.Length;
            }

            // IMPORTANT: Avoid off-by-1 errors!
            // The end value passed in is the character immediately after the last character selected.

            int newStart = start == 0 ? 0 : e.GetCharCount(bytes, 0, start);
            end = newStart + e.GetCharCount(bytes, start, end - start);
            start = newStart;

            if (swap)
            {
                Swap(ref start, ref end);
            }
        }

        /// <summary>
        ///  Converts unicode offsset to byte offsets.
        ///  When procssing WM_GETSEL/WM_SETSEL, EDIT control works with byte offsets instead of character positions
        ///  as opposed to RICHEDIT which does it always as character positions.
        ///  This method is used when handling the WM_SETSEL message.
        /// </summary>
        static internal void ToDbcsOffsets(string str, ref int start, ref int end)
        {
            Encoding e = Encoding.Default;

            bool swap = start > end;
            if (swap)
            {
                Swap(ref start, ref end);
            }

            // Make sure start and end are within the string
            //
            if (start < 0)
            {
                start = 0;
            }
            if (start > str.Length)
            {
                start = str.Length;
            }
            if (end < start)
            {
                end = start;
            }
            if (end > str.Length)
            {
                end = str.Length;
            }

            // IMPORTANT: Avoid off-by-1 errors!
            // The end value passed in is the character immediately after the last character selected.

            int newStart = start == 0 ? 0 : e.GetByteCount(str.Substring(0, start));
            end = newStart + e.GetByteCount(str.Substring(start, end - start));
            start = newStart;

            if (swap)
            {
                Swap(ref start, ref end);
            }
        }

        /// <summary>
        ///  Provides some interesting information for the TextBox control in
        ///  String form.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();

            string txt = Text;
            if (txt.Length > 40)
            {
                txt = txt.Substring(0, 40) + "...";
            }

            return s + ", Text: " + txt.ToString();
        }

        /// <summary>
        ///  Undoes the last edit operation in the text box.
        /// </summary>
        public void Undo() => SendMessageW(this, (WM)EM.UNDO);

        internal virtual void UpdateMaxLength()
        {
            if (IsHandleCreated)
            {
                SendMessageW(this, (WM)EM.LIMITTEXT, (IntPtr)maxLength);
            }
        }

        internal override Gdi32.HBRUSH InitializeDCForWmCtlColor(Gdi32.HDC dc, User32.WM msg)
        {
            if (msg == WM.CTLCOLORSTATIC && !ShouldSerializeBackColor())
            {
                // Let the Win32 Edit control handle background colors itself.
                // This is necessary because a disabled edit control will display a different
                // BackColor than when enabled.
                return default;
            }
            else
            {
                return base.InitializeDCForWmCtlColor(dc, msg);
            }
        }

        private void WmReflectCommand(ref Message m)
        {
            if (!textBoxFlags[codeUpdateText] && !textBoxFlags[creatingHandle])
            {
                EN wParamAsEN = (EN)PARAM.HIWORD(m.WParam);
                if (wParamAsEN == EN.CHANGE && CanRaiseTextChangedEvent)
                {
                    OnTextChanged(EventArgs.Empty);
                }
                else if (wParamAsEN == EN.UPDATE)
                {
                    // Force update to the Modified property, which will trigger
                    // ModifiedChanged event handlers
                    bool force = Modified;
                }
            }
        }

        void WmSetFont(ref Message m)
        {
            base.WndProc(ref m);
            if (!textBoxFlags[multiline])
            {
                SendMessageW(this, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
            }
        }

        void WmGetDlgCode(ref Message m)
        {
            base.WndProc(ref m);
            if (AcceptsTab)
            {
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "TextBox wants tabs");
                m.Result = (IntPtr)(unchecked((int)(long)m.Result) | (int)DLGC.WANTTAB);
            }
            else
            {
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "TextBox doesn't want tabs");
                m.Result = (IntPtr)(unchecked((int)(long)m.Result) & ~(int)(DLGC.WANTTAB | DLGC.WANTALLKEYS));
            }
        }

        /// <summary>
        ///  Handles the WM_CONTEXTMENU message. Show the ContextMenuStrip if present.
        /// </summary>
        private void WmTextBoxContextMenu(ref Message m)
        {
            if (ContextMenuStrip is null)
            {
                return;
            }

            int x = PARAM.SignedLOWORD(m.LParam);
            int y = PARAM.SignedHIWORD(m.LParam);
            Point client;
            bool keyboardActivated = false;

            // Lparam will be exactly -1 when the user invokes the context menu
            // with the keyboard.
            if (unchecked((int)(long)m.LParam) == -1)
            {
                keyboardActivated = true;
                client = new Point(Width / 2, Height / 2);
            }
            else
            {
                client = PointToClient(new Point(x, y));
            }

            // Only show the context menu when clicked in the client area (VisualStudio7 #156)
            if (ClientRectangle.Contains(client))
            {
                ContextMenuStrip.ShowInternal(this, client, keyboardActivated);
            }
        }

        /// <summary>
        ///  The control's window procedure.  Inheriting classes can override this
        ///  to add extra functionality, but should not forget to call
        ///  base.wndProc(m); to ensure the control continues to function properly.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch ((WM)m.Msg)
            {
                case WM.LBUTTONDBLCLK:
                    doubleClickFired = true;
                    base.WndProc(ref m);
                    break;
                case WM.REFLECT_COMMAND:
                    WmReflectCommand(ref m);
                    break;
                case WM.GETDLGCODE:
                    WmGetDlgCode(ref m);
                    break;
                case WM.SETFONT:
                    WmSetFont(ref m);
                    break;
                case WM.CONTEXTMENU:
                    if (ShortcutsEnabled)
                    {
                        // Calling base will find ContextMenus in this order:
                        // 1) ContextMenuStrip 2) SystemMenu
                        base.WndProc(ref m);
                    }
                    else
                    {
                        // We'll handle this message so we can hide the
                        // SystemMenu if ContextMenuStrip menus are null
                        WmTextBoxContextMenu(ref m);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
