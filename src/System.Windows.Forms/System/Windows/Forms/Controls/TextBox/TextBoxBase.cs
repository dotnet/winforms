// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Rendering.Animation;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Implements the basic functionality required by text
///  controls.
/// </summary>
[DefaultEvent(nameof(TextChanged))]
[DefaultBindingProperty(nameof(Text))]
[Designer($"System.Windows.Forms.Design.TextBoxBaseDesigner, {Assemblies.SystemDesign}")]
public abstract partial class TextBoxBase : Control
{
    // The boolean properties for this control are contained in the textBoxFlags bit
    // vector. We can store up to 32 boolean values in this one vector. Here we
    // create the bitmasks for each bit in the vector.

    private static readonly int s_autoSize = BitVector32.CreateMask();
    private static readonly int s_hideSelection = BitVector32.CreateMask(s_autoSize);
    private static readonly int s_multiline = BitVector32.CreateMask(s_hideSelection);
    private static readonly int s_modified = BitVector32.CreateMask(s_multiline);
    private static readonly int s_readOnly = BitVector32.CreateMask(s_modified);
    private static readonly int s_acceptsTab = BitVector32.CreateMask(s_readOnly);
    private static readonly int s_wordWrap = BitVector32.CreateMask(s_acceptsTab);
    private static readonly int s_creatingHandle = BitVector32.CreateMask(s_wordWrap);
    private static readonly int s_codeUpdateText = BitVector32.CreateMask(s_creatingHandle);
    private static readonly int s_shortcutsEnabled = BitVector32.CreateMask(s_codeUpdateText);
    private static readonly int s_scrollToCaretOnHandleCreated = BitVector32.CreateMask(s_shortcutsEnabled);
    private static readonly int s_setSelectionOnHandleCreated = BitVector32.CreateMask(s_scrollToCaretOnHandleCreated);

    private static readonly object s_acceptsTabChangedEvent = new();
    private static readonly object s_borderStyleChangedEvent = new();
    private static readonly object s_hideSelectionChangedEvent = new();
    private static readonly object s_modifiedChangedEvent = new();
    private static readonly object s_multilineChangedEvent = new();
    private static readonly object s_readOnlyChangedEvent = new();

    /// <summary>
    ///  The current border for this edit control.
    /// </summary>
    private BorderStyle _borderStyle = BorderStyle.Fixed3D;
    private AnimatedFocusIndicatorRenderer? _focusIndicatorRenderer;

    private const OBJECT_IDENTIFIER HorizontalScrollBarObjectId = (OBJECT_IDENTIFIER)(-6);
    private const OBJECT_IDENTIFIER VerticalScrollBarObjectId = (OBJECT_IDENTIFIER)(-5);
    private const uint StateSystemInvisible = 0x00008000;

    /// <summary>
    ///  One-shot latch that gates the single NC-calc round trip used to carve the modern Visual
    ///  Styles padding band. Set by <see cref="InitializeClientArea"/> and reset on handle recreation.
    /// </summary>
    private bool _triggerNewClientSizeRequest;

    /// <summary>
    ///  Controls the maximum length of text in the edit control.
    ///  Matches the Windows limit.
    /// </summary>
    private int _maxLength = 32767;

    /// <summary>
    ///  Used by the autoSizing code to help figure out the desired height of
    ///  the edit box.
    /// </summary>
    private int _requestedHeight;
    private bool _integralHeightAdjust;

    // these indices are used to cache the values of the selection, by doing this
    // if the handle isn't created yet, we don't force a creation.
    private int _selectionStart;
    private int _selectionLength;

    /// <summary>
    ///  Controls firing of click event (Left click).
    ///  This is used by TextBox, RichTextBox and MaskedTextBox, code was moved down from TextBox/RichTextBox
    ///  but cannot make it as default behavior to avoid introducing breaking changes.
    /// </summary>
    private bool _doubleClickFired;

    private static int[]? s_shortcutsToDisable;

    // We store all boolean properties in here.
    private BitVector32 _textBoxFlags;

    /// <summary>
    ///  Creates a new TextBox control. Uses the parent's current font and color set.
    /// </summary>
    internal TextBoxBase() : base()
    {
        // this class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        _textBoxFlags[s_autoSize | s_hideSelection | s_wordWrap | s_shortcutsEnabled] = true;
        SetStyle(ControlStyles.FixedHeight, _textBoxFlags[s_autoSize]);
        SetStyle(ControlStyles.StandardClick
                | ControlStyles.StandardDoubleClick
                | ControlStyles.UseTextForAccessibility
                | ControlStyles.UserPaint, false);

        // cache requestedHeight. Note: Control calls DefaultSize (overridable) in the constructor
        // to set the control's cached height that is returned when calling Height, so we just
        // need to get the cached height here.
        _requestedHeight = Height;
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
        get => _textBoxFlags[s_acceptsTab];
        set
        {
            if (_textBoxFlags[s_acceptsTab] != value)
            {
                _textBoxFlags[s_acceptsTab] = value;
                OnAcceptsTabChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.TextBoxBaseOnAcceptsTabChangedDescr))]
    public event EventHandler? AcceptsTabChanged
    {
        add => Events.AddHandler(s_acceptsTabChangedEvent, value);
        remove => Events.RemoveHandler(s_acceptsTabChangedEvent, value);
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
        get => _textBoxFlags[s_shortcutsEnabled];

        set
        {
            s_shortcutsToDisable ??=
                [
                    (int)Shortcut.CtrlZ, (int)Shortcut.CtrlC, (int)Shortcut.CtrlX,
                    (int)Shortcut.CtrlV, (int)Shortcut.CtrlA, (int)Shortcut.CtrlL, (int)Shortcut.CtrlR,
                    (int)Shortcut.CtrlE, (int)Shortcut.CtrlY, (int)Keys.Control + (int)Keys.Back,
                    (int)Shortcut.CtrlDel, (int)Shortcut.ShiftDel, (int)Shortcut.ShiftIns, (int)Shortcut.CtrlJ
                ];

            _textBoxFlags[s_shortcutsEnabled] = value;
        }
    }

    /// <summary>
    ///  Implements the <see cref="ShortcutsEnabled"/> property.
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // First call parent's ProcessCmdKey, since we don't to eat up
        // the shortcut key we are not supported in TextBox.
        bool returnedValue = base.ProcessCmdKey(ref msg, keyData);

        if (!ShortcutsEnabled && s_shortcutsToDisable is not null)
        {
            foreach (int shortcutValue in s_shortcutsToDisable)
            {
                if ((int)keyData == shortcutValue ||
                    (int)keyData == (shortcutValue | (int)Keys.Shift))
                {
                    return true;
                }
            }
        }

        // There are a few keys that change the alignment of the text, but that
        // are not ignored by the native control when the ReadOnly property is set.
        // We need to workaround that.
        if (_textBoxFlags[s_readOnly])
        {
            int k = (int)keyData;

            if (k is ((int)Shortcut.CtrlL)        // align left
                or ((int)Shortcut.CtrlR)          // align right
                or ((int)Shortcut.CtrlE)          // align center
                or ((int)Shortcut.CtrlJ))         // align justified
            {
                return true;
            }
        }

        if (ReadOnly
            || (keyData != (Keys.Control | Keys.Back)
                && keyData != (Keys.Control | Keys.Shift | Keys.Back)))
        {
            return returnedValue;
        }

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
        get => _textBoxFlags[s_autoSize];

        set
        {
            // Note that we intentionally do not call base. TextBoxes size themselves by
            // overriding SetBoundsCore (old RTM code). We let CommonProperties.GetAutoSize
            // continue to return false to keep our LayoutEngines from messing with TextBoxes.
            // This is done for backwards compatibility since the new AutoSize behavior differs.
            if (_textBoxFlags[s_autoSize] != value)
            {
                _textBoxFlags[s_autoSize] = value;

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
    [DispId(PInvokeCore.DISPID_BACKCOLOR)]
    [SRDescription(nameof(SR.ControlBackColorDescr))]
    public override Color BackColor
    {
        get
        {
            if (ShouldSerializeBackColor())
            {
                return base.BackColor;
            }
            else
            {
                return ReadOnly
                    // If we're ReadOnly and in DarkMode, we are using a different background color.
                    ? Application.IsDarkModeEnabled
                        && DarkModeRequestState is true
                            ? SystemColors.ControlDarkDark
                            : SystemColors.Control
                    : SystemColors.Window;
            }
        }

        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
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
    public new event EventHandler? BackgroundImageLayoutChanged
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
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.TextBoxBorderDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            if (_borderStyle != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _borderStyle = value;
                _focusIndicatorRenderer?.Synchronize(Focused, invalidate: false);
                CommonProperties.xClearPreferredSizeCache(this);

                if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
                {
                    AdjustHeight(false);
                }

                UpdateStyles();

                if (EffectiveVisualStylesMode >= VisualStylesMode.Net11 && IsHandleCreated)
                {
                    RecalculateVisualStylesClientArea();
                }
                else
                {
                    RecreateHandle();
                }

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
    public event EventHandler? BorderStyleChanged
    {
        add => Events.AddHandler(s_borderStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_borderStyleChangedEvent, value);
    }

    internal virtual bool CanRaiseTextChangedEvent
        => true;

    protected override bool CanEnableIme
        => !(ReadOnly || PasswordProtect) && base.CanEnableIme;

    /// <summary>
    ///  Gets a value indicating whether the user can undo the previous operation in a text box control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TextBoxCanUndoDescr))]
    public bool CanUndo
        => IsHandleCreated
            && (int)PInvokeCore.SendMessage(this, PInvokeCore.EM_CANUNDO) != 0;

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
            SetStyle(ControlStyles.ApplyThemingImplicitly, true);

            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_EDIT;
            cp.Style |= PInvoke.ES_AUTOHSCROLL | PInvoke.ES_AUTOVSCROLL;

            if (!_textBoxFlags[s_hideSelection])
            {
                cp.Style |= PInvoke.ES_NOHIDESEL;
            }

            if (_textBoxFlags[s_readOnly])
            {
                cp.Style |= PInvoke.ES_READONLY;
            }

            cp.Style &= ~(int)WINDOW_STYLE.WS_BORDER;
            cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;

            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                    break;

                case BorderStyle.FixedSingle:
                    cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                    break;
            }

            if (_textBoxFlags[s_multiline])
            {
                cp.Style |= PInvoke.ES_MULTILINE;

                if (_textBoxFlags[s_wordWrap])
                {
                    cp.Style &= ~PInvoke.ES_AUTOHSCROLL;
                }
            }

            if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
            {
                // Under a modern VisualStylesMode we custom-draw the frame in the non-client area (see OnNcPaint),
                // so the natively drawn border must be suppressed to avoid a double frame. Only the effective
                // Win32 styles are stripped here; the public BorderStyle property remains authoritative for what
                // the custom non-client paint draws.
                cp.Style &= ~(int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
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
    public new event EventHandler? Click
    {
        add => base.Click += value;
        remove => base.Click -= value;
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event MouseEventHandler? MouseClick
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
        => new Size(100, PreferredHeight);

    /// <summary>
    ///  Gets or sets the foreground color of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DispId(PInvokeCore.DISPID_FORECOLOR)]
    [SRDescription(nameof(SR.ControlForeColorDescr))]
    public override Color ForeColor
    {
        get => ShouldSerializeForeColor()
            ? base.ForeColor
            : SystemColors.WindowText;

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
        get => _textBoxFlags[s_hideSelection];

        set
        {
            if (_textBoxFlags[s_hideSelection] != value)
            {
                _textBoxFlags[s_hideSelection] = value;
                RecreateHandle();
                OnHideSelectionChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.TextBoxBaseOnHideSelectionChangedDescr))]
    public event EventHandler? HideSelectionChanged
    {
        add => Events.AddHandler(s_hideSelectionChangedEvent, value);
        remove => Events.RemoveHandler(s_hideSelectionChangedEvent, value);
    }

    /// <summary>
    ///  Internal version of ImeMode property. The ImeMode of TextBoxBase controls depend on its IME restricted
    ///  mode which is determined by the CanEnableIme property which checks whether the control is in Password or
    ///  ReadOnly mode.
    /// </summary>
    protected override ImeMode ImeModeBase
    {
        get => (DesignMode || CanEnableIme)
            ? base.ImeModeBase
            : ImeMode.Disable;

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
    [AllowNull]
    [SRDescription(nameof(SR.TextBoxLinesDescr))]
    [Editor($"System.Windows.Forms.Design.StringArrayEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    public string[] Lines
    {
        get
        {
            string text = Text;
            List<string> list = [];

            int lineStart = 0;
            while (lineStart < text.Length)
            {
                int lineEnd = lineStart;

                for (; lineEnd < text.Length; lineEnd++)
                {
                    char c = text[lineEnd];

                    if (c is '\r' or '\n')
                    {
                        break;
                    }
                }

                string line = text[lineStart..lineEnd];
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
            if (text.Length > 0 && (text[^1] == '\r' || text[^1] == '\n'))
            {
                list.Add(string.Empty);
            }

            return [.. list];
        }

        set =>
            // unparse this string list...
            Text = value is not null && value.Length > 0
                ? string.Join(Environment.NewLine, value)
                : string.Empty;
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
        get => _maxLength;

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (_maxLength != value)
            {
                _maxLength = value;
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
            if (!IsHandleCreated)
            {
                return _textBoxFlags[s_modified];
            }
            else
            {
                bool curState = (int)PInvokeCore.SendMessage(
                    this,
                    PInvokeCore.EM_GETMODIFY) != 0;

                if (_textBoxFlags[s_modified] != curState)
                {
                    // Raise ModifiedChanged event.
                    // See WmReflectCommand for more info.
                    _textBoxFlags[s_modified] = curState;
                    OnModifiedChanged(EventArgs.Empty);
                }

                return curState;
            }
        }

        set
        {
            if (Modified != value)
            {
                if (IsHandleCreated)
                {
                    PInvokeCore.SendMessage(
                        this,
                        PInvokeCore.EM_SETMODIFY,
                        (WPARAM)(BOOL)value);
                }

                // Must maintain this state always in order for the
                // test in the Get method to work properly.
                _textBoxFlags[s_modified] = value;
                OnModifiedChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.TextBoxBaseOnModifiedChangedDescr))]
    public event EventHandler? ModifiedChanged
    {
        add => Events.AddHandler(s_modifiedChangedEvent, value);
        remove => Events.RemoveHandler(s_modifiedChangedEvent, value);
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
        get => _textBoxFlags[s_multiline];
        set
        {
            if (_textBoxFlags[s_multiline] == value)
            {
                return;
            }

            using (LayoutTransaction.CreateTransactionIf(
                condition: AutoSize,
                controlToLayout: ParentInternal,
                elementCausingLayout: this,
                property: PropertyNames.Multiline))
            {
                _textBoxFlags[s_multiline] = value;

                if (value)
                {
                    // Multi-line textboxes do not have fixed height
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

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.TextBoxBaseOnMultilineChangedDescr))]
    public event EventHandler? MultilineChanged
    {
        add => Events.AddHandler(s_multilineChangedEvent, value);
        remove => Events.RemoveHandler(s_multilineChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets the distance, in pixels, between the text displayed in a text box control and the top, bottom and side edges of the control.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Note: While shadowing this property may seem redundant at first glance, it is necessary for binary compatibility.
    ///   Prior to .NET 11, this property was marked with <see cref="BrowsableAttribute">Browsable(false)</see> and
    ///   <see cref="EditorBrowsableAttribute">EditorBrowsable(EditorBrowsableState.Never)</see>,
    ///   and was never serialized. Effectively, <see cref="Control.Padding"/> did not work before .NET 11. While this changed in .NET 11,
    ///   removing the property shadowing would constitute a binary breaking change. Therefore, the shadowing is retained
    ///   with updated attributes to ensure <c>Padding</c> now functions as expected.
    ///  </para>
    ///  <para>
    ///   As long as the default value for the <c>Padding</c> property is not modified, <see cref="TextBoxBase"/>-derived
    ///   controls (<see cref="TextBox"/>, <see cref="RichTextBox"/>) will behave as they always have. However, if you
    ///   opt into modern styling by calling <see cref="Application.EnableVisualStyles"/> and set
    ///   <see cref="VisualStylesMode"/> to a value other than <see cref="VisualStylesMode.Classic"/> or
    ///   <see cref="VisualStylesMode.Disabled"/>, the control will automatically apply styling that conforms to the
    ///   Windows 10+ Fluent Design Language. This provides both stylistic improvements (such as Dark Mode support) and
    ///   functional enhancements (including increased touch targets for easier mouse and touch interaction), thereby
    ///   meeting the accessibility requirements of modern high-resolution displays.
    ///  </para>
    /// </remarks>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    private new bool ShouldSerializePadding()
        => Padding != DefaultPadding;

    private void ResetPadding()
        => Padding = DefaultPadding;

    // Not new API and not a breaking change: TextBoxBase.PaddingChanged is already in
    // PublicAPI.Shipped.txt. It is re-declared here with 'new' purely to apply the designer-hiding
    // attributes below (Browsable(false) / EditorBrowsable(Never)) on top of Control.PaddingChanged.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnPaddingChangedDescr))]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  Determines if the control is in password protect mode. This is overridden in TextBox and
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
    public int PreferredHeight =>
        EffectiveVisualStylesMode switch
        {
            VisualStylesMode.Disabled => PreferredHeightClassic,
            VisualStylesMode.Classic => PreferredHeightClassic,
            >= VisualStylesMode.Net11 => PreferredHeightCore,

            // We should never be here.
            _ => throw new InvalidEnumArgumentException(
                argumentName: nameof(VisualStylesMode),
                invalidValue: (int)VisualStylesMode,
                enumClass: typeof(VisualStylesMode))
        };

    /// <summary>
    ///  Returns the preferred height for modern Visual Styles, taking the carved padding band
    ///  (including the live scrollbar allowance and the user <see cref="Padding"/>) into account.
    /// </summary>
    private protected virtual int PreferredHeightCore
    {
        get
        {
            Padding visualStylesPadding = GetVisualStylesPadding(
                includeScrollbars: true);
            int preferredHeight = FontHeight + visualStylesPadding.Vertical;

            if (AutoSize && !Multiline && BorderStyle == BorderStyle.Fixed3D)
            {
                preferredHeight = ModernControlVisualStyles.GetPreferredFieldHeight(
                    FontHeight,
                    visualStylesPadding,
                    DeviceDpiInternal);
            }

            return preferredHeight;
        }
    }

    /// <summary>
    ///  Returns the classic (Everett-compatible) preferred height for a single-line text box.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   COMPAT: we must return the same busted height we did in Everett, even if it does not take
    ///   multiline and word wrap into account. For better accuracy and/or wrapping use
    ///   <see cref="Control.GetPreferredSize(Size)"/> instead.
    ///  </para>
    /// </remarks>
    private int PreferredHeightClassic
    {
        get
        {
            int height = FontHeight;

            if (_borderStyle != BorderStyle.None)
            {
                height += SystemInformation.GetBorderSizeForDpi(DeviceDpiInternal).Height
                    * 4 + 3;
            }

            return height;
        }
    }

    /// <summary>
    ///  Defines the visible part of the padding band carved for modern Visual Styles chrome.
    /// </summary>
    /// <param name="includeScrollbars">
    ///  <see langword="true"/> to add the live scrollbar allowance (see <see cref="GetScrollBarPadding"/>)
    ///  on top of the border padding; otherwise <see langword="false"/>.
    /// </param>
    /// <returns>The visible padding dimensions.</returns>
    /// <remarks>
    ///  <para>
    ///   The visible part of the padding is the area by which we extend the real-estate of the control
    ///  with its back color. Border/corner reservation, the DPI-scaled internal chrome inset, the
    ///  user-provided <see cref="Padding"/>, and scrollbar reservation are each added once.
    ///  </para>
    /// </remarks>
    private protected Padding GetVisualStylesPadding(bool includeScrollbars)
    {
        SystemVisualSettings settings = Application.SystemVisualSettings;
        Padding padding = ModernControlVisualStyles.GetFieldPadding(
            BorderStyle,
            Padding,
            settings.FocusBorderMetrics,
            settings.TextScaleFactor,
            DeviceDpiInternal);

        if (includeScrollbars)
        {
            padding += GetScrollBarPadding();
        }

        return padding;
    }

    private int ScaleVisualStylesMetric(int logicalValue)
        => ScaleHelper.ScaleToDpi(logicalValue, DeviceDpiInternal);

    private Size GetVisualStylesFocusBorderMetrics()
    {
        SystemVisualSettings settings = Application.SystemVisualSettings;
        return GetVisualStylesFocusBorderMetrics(
            settings.FocusBorderMetrics,
            settings.TextScaleFactor,
            DeviceDpiInternal);
    }

    private int GetVisualStylesFocusBandHeight()
    {
        SystemVisualSettings settings = Application.SystemVisualSettings;
        return GetVisualStylesFocusBandHeight(
            settings.FocusBorderMetrics,
            settings.TextScaleFactor,
            DeviceDpiInternal);
    }

    internal static Size GetVisualStylesFocusBorderMetrics(
        Size focusBorderMetrics,
        float textScaleFactor,
        int deviceDpi)
        => ModernControlVisualStyles.GetFocusBorderMetrics(
            focusBorderMetrics,
            textScaleFactor,
            deviceDpi);

    internal static int GetVisualStylesFocusBandHeight(
        Size focusBorderMetrics,
        float textScaleFactor,
        int deviceDpi)
        => ModernControlVisualStyles.GetFocusBandHeight(
            focusBorderMetrics,
            textScaleFactor,
            deviceDpi);

    internal static bool CanRenderVisualStylesRoundedChrome(
        Rectangle bounds,
        int cornerSize,
        int borderThickness)
    {
        int minimumDimension = cornerSize + borderThickness;
        return bounds.Width >= minimumDimension && bounds.Height >= minimumDimension;
    }

    internal static Color GetVisualStylesFocusColor(bool highContrast)
        => highContrast
            ? SystemColors.Highlight
            : Application.SystemVisualSettings.AccentColor;

    /// <summary>
    ///  Returns the additional padding required to clear the live scrollbars,
    ///  if any are currently shown.
    /// </summary>
    private protected virtual Padding GetScrollBarPadding()
    {
        Padding padding = Padding.Empty;

        // Are the scrollbars visible?
        WINDOW_STYLE style = (WINDOW_STYLE)PInvokeCore.GetWindowLong(
            this,
            WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        bool hasHScroll = (style & WINDOW_STYLE.WS_HSCROLL) != 0;
        bool hasVScroll = (style & WINDOW_STYLE.WS_VSCROLL) != 0;

        if (hasHScroll)
        {
            padding.Bottom += SystemInformation.GetHorizontalScrollBarHeightForDpi(DeviceDpiInternal);
        }

        if (hasVScroll)
        {
            padding.Right += SystemInformation.GetVerticalScrollBarWidthForDpi(DeviceDpiInternal);
        }

        return padding;
    }

    /// <summary>
    ///  Computes the preferred size of the control, honoring border style, multiline, and word wrap.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This can return a different value than <see cref="PreferredHeight"/>. <see cref="PreferredHeight"/>
    ///   is a single-line, border-only height kept for backward compatibility, whereas this method measures
    ///   the actual text against the available width and additionally accounts for multiline and word wrap,
    ///   and - for modern Visual Styles - the carved adorner band and the user <see cref="Padding"/>.
    ///  </para>
    /// </remarks>
    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        // 3px vertical space is required between the text and the border to keep the last
        // line from being clipped.

        // This 3 pixel size was added in Everett, and we do this to maintain compat.
        // Old Everett behavior was FontHeight + [SystemInformation.BorderSize.Height * 4 + 3],
        // however the [ ] was only added if BorderStyle was not None.
        Padding padding = default;

        // Fit the text to the remaining space.
        // Originally fixed for .NET Framework 4.0
        TextFormatFlags format = TextFormatFlags.NoPrefix;

        if (!Multiline)
        {
            format |= TextFormatFlags.SingleLine;
        }
        else if (WordWrap)
        {
            format |= TextFormatFlags.WordBreak;
        }

        if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
        {
            // For modern Visual Styles we take the carved adorner band (including scrollbars
            // and the user Padding) into account when measuring.
            padding = GetVisualStylesPadding(includeScrollbars: true);
            proposedConstraints -= padding.Size;
        }
        else
        {
            // Keep the pre-.NET 11 layout contract for classic or disabled visual styles, including
            // the user Padding contribution that historically participated in preferred-size calculations.
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

            padding.Left = 0;
            padding.Top = 0;
            padding.Right = bordersAndPadding.Width;
            padding.Bottom = bordersAndPadding.Height;

            // Reduce constraints by the classic border and padding size.
            proposedConstraints -= bordersAndPadding;
        }

        Size textSize = TextRenderer.MeasureText(Text, Font, proposedConstraints, format);

        // We use this old computation as a lower bound to ensure backwards compatibility.
        textSize.Height = Math.Max(textSize.Height, FontHeight);

        Size preferredSize = textSize + new Size(padding.Horizontal, padding.Vertical);

        return preferredSize;
    }

    /// <summary>
    ///  Get the currently selected text start position and length. Use this method internally
    ///  to avoid calling SelectionStart + SelectionLength each of which does essentially the
    ///  same (save one message round trip).
    /// </summary>
    internal unsafe void GetSelectionStartAndLength(out int start, out int length)
    {
        int end = 0;

        if (!IsHandleCreated)
        {
            // It is possible that the cached values are no longer valid if the Text has been changed
            // while the control does not have a handle. We need to return valid values. We also need
            // to keep the old cached values in case the Text is changed again making the cached values
            // valid again.
            AdjustSelectionStartAndEnd(_selectionStart, _selectionLength, out start, out end, -1);
            length = end - start;
        }
        else
        {
            start = 0;
            int startResult = 0;
            PInvokeCore.SendMessage(this, PInvokeCore.EM_GETSEL, (WPARAM)(&startResult), ref end);
            start = startResult;

            // Here, we return the max of either 0 or the # returned by
            // the windows call. This eliminates a problem on nt4 where
            // a huge negative # is being returned.
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

            len = t is null
                ? 0
                : t.Length;

            Debug.Assert(end <= len,
                $"SelectionEnd is outside the set of valid caret positions for the current WindowText (end ={end}, WindowText.Length ={len})");
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
        get => _textBoxFlags[s_readOnly];

        set
        {
            if (_textBoxFlags[s_readOnly] == value)
            {
                return;
            }

            _textBoxFlags[s_readOnly] = value;

            if (IsHandleCreated)
            {
                PInvokeCore.SendMessage(this, PInvokeCore.EM_SETREADONLY, (WPARAM)(BOOL)value);
                EnsureReadonlyBackgroundColor(value);
            }

            OnReadOnlyChanged(EventArgs.Empty);
            VerifyImeRestrictedModeChanged();
        }
    }

    private void EnsureReadonlyBackgroundColor(bool value)
    {
        // If we have no specifically defined back color, we set the back color in case we're in dark mode.
        if (Application.IsDarkModeEnabled
            && DarkModeRequestState is true
            && !ShouldSerializeBackColor())
        {
            base.BackColor = value
                ? SystemColors.ControlLight
                : SystemColors.Window;

            Invalidate();
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.TextBoxBaseOnReadOnlyChangedDescr))]
    public event EventHandler? ReadOnlyChanged
    {
        add => Events.AddHandler(s_readOnlyChangedEvent, value);
        remove => Events.RemoveHandler(s_readOnlyChangedEvent, value);
    }

    /// <summary>
    ///  The currently selected text in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [AllowNull]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TextBoxSelectedTextDescr))]
    public virtual string SelectedText
    {
        get
        {
            GetSelectionStartAndLength(out int selStart, out int selLength);

            return Text.Substring(selStart, selLength);
        }

        set => SetSelectedTextInternal(value, true);
    }

    /// <summary>
    ///  Replaces the selected text with the one passed in.
    /// </summary>
    internal virtual void SetSelectedTextInternal(string? text, bool clearUndo)
    {
        if (!IsHandleCreated)
        {
            CreateHandle();
        }

        text ??= string.Empty;

        // The EM_LIMITTEXT message limits only the text the user can enter. It does not affect any text
        // already in the edit control when the message is sent, nor does it affect the length of the text
        // copied to the edit control by the WM_SETTEXT message.
        PInvokeCore.SendMessage(this, PInvokeCore.EM_LIMITTEXT);

        if (clearUndo)
        {
            PInvokeCore.SendMessage(this, PInvokeCore.EM_REPLACESEL, 0, text);

            // For consistency with Text, we clear the modified flag
            PInvokeCore.SendMessage(this, PInvokeCore.EM_SETMODIFY);
            ClearUndo();
        }
        else
        {
            PInvokeCore.SendMessage(this, PInvokeCore.EM_REPLACESEL, (WPARAM)(-1), text);
        }

        // Re-enable user input.
        PInvokeCore.SendMessage(this, PInvokeCore.EM_LIMITTEXT, (WPARAM)_maxLength);
    }

    /// <summary>
    ///  Gets or sets the number of characters selected in the text box.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TextBoxSelectionLengthDescr))]
    public virtual int SelectionLength
    {
        get
        {
            GetSelectionStartAndLength(out int _, out int length);
            return length;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            GetSelectionStartAndLength(out int start, out int length);

            if (value != length)
            {
                Select(start, value);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the starting point of text selected in the text box.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TextBoxSelectionStartDescr))]
    public int SelectionStart
    {
        get
        {
            GetSelectionStartAndLength(out int start, out _);
            return start;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            Select(value, SelectionLength);
        }
    }

    /// <summary>
    ///  Gets or sets the current text in the text box.
    /// </summary>
    [Localizable(true)]
    [AllowNull]
    [Editor($"System.ComponentModel.Design.MultilineStringEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
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
                    PInvokeCore.SendMessage(this, PInvokeCore.EM_SETMODIFY);
                }
            }
        }
    }

    [Browsable(false)]
    public virtual int TextLength
        // Note: Currently WinForms does not fully support surrogates. If
        // the text contains surrogate characters this property may return incorrect values.

        => IsHandleCreated ? PInvokeCore.GetWindowTextLength(this) : Text.Length;

    /// <summary>
    ///  Gets or sets the underlying native window text (the edit control's caption). This is overridden
    ///  so that setting the text from code raises a "code update" flag for the duration of the assignment:
    ///  updating the text on a created handle produces a <c>WM_COMMAND</c> / <c>EN_CHANGE</c> notification,
    ///  and the flag lets us swallow it so we do not raise a duplicate <see cref="Control.TextChanged"/> event.
    /// </summary>
    internal override string WindowText
    {
        get => base.WindowText;

        set
        {
            value ??= string.Empty;

            // Since setting the WindowText while the handle is created generates a WM_COMMAND message, we must trap
            // that case and prevent the event from getting fired, or we get double "TextChanged" events.

            if (!WindowText.Equals(value))
            {
                _textBoxFlags[s_codeUpdateText] = true;

                try
                {
                    base.WindowText = value;
                }
                finally
                {
                    _textBoxFlags[s_codeUpdateText] = false;
                }
            }
        }
    }

    /// <summary>
    ///  In certain circumstances we might have to force text into the window whether or not the text is the same.
    ///  Make this a method on <see cref="TextBoxBase"/> rather than <see cref="RichTextBox"/> (which is the only
    ///  control that needs this at this point), since we need to set <see cref="s_codeUpdateText"/>.
    /// </summary>
    internal void ForceWindowText(string? value)
    {
        value ??= string.Empty;

        _textBoxFlags[s_codeUpdateText] = true;

        try
        {
            if (IsHandleCreated)
            {
                PInvoke.SetWindowText(this, value);
            }
            else
            {
                Text = value.Length == 0 ? null : value;
            }
        }
        finally
        {
            _textBoxFlags[s_codeUpdateText] = false;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether a multiline text box control automatically wraps words to the
    ///  beginning of the next line when necessary.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TextBoxWordWrapDescr))]
    public bool WordWrap
    {
        get => _textBoxFlags[s_wordWrap];

        set
        {
            using (LayoutTransaction.CreateTransactionIf(
                condition: AutoSize,
                controlToLayout: ParentInternal,
                elementCausingLayout: this,
                property: PropertyNames.WordWrap))
            {
                if (_textBoxFlags[s_wordWrap] != value)
                {
                    _textBoxFlags[s_wordWrap] = value;
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
        if (returnIfAnchored
            && (Anchor & (AnchorStyles.Top | AnchorStyles.Bottom))
                == (AnchorStyles.Top | AnchorStyles.Bottom))
        {
            return;
        }

        int saveHeight = _requestedHeight;

        try
        {
            if (_textBoxFlags[s_autoSize] && !_textBoxFlags[s_multiline])
            {
                Height = PreferredHeight;
            }
            else
            {
                int curHeight = Height;

                // Changing the font of a multi-line textbox can sometimes cause a painting problem
                // The only workaround I can find is to size the textbox big enough for the font, and
                // then restore its correct size.
                if (_textBoxFlags[s_multiline])
                {
                    Height = Math.Max(saveHeight, PreferredHeight + 2); // 2 = fudge factor
                }

                _integralHeightAdjust = true;

                try
                {
                    Height = saveHeight;
                }
                finally
                {
                    _integralHeightAdjust = false;
                }
            }
        }
        finally
        {
            _requestedHeight = saveHeight;
        }
    }

    /// <summary>
    ///  Append text to the current text of text box.
    /// </summary>
    public void AppendText(string? text)
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
            PInvokeCore.SendMessage(this, PInvokeCore.EM_EMPTYUNDOBUFFER);
        }
    }

    protected bool ContainsNavigationKeyCode(Keys keyCode) => keyCode switch
    {
        Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown
        or Keys.Home or Keys.End or Keys.Left or Keys.Right => true,
        _ => false,
    };

    /// <summary>
    ///  Copies the current selection in the text box to the Clipboard.
    /// </summary>
    public void Copy()
        => PInvokeCore.SendMessage(this, PInvokeCore.WM_COPY);

    protected override AccessibleObject CreateAccessibilityInstance()
        => new TextBoxBaseAccessibleObject(this);

    protected override void CreateHandle()
    {
        // Should the handle be (re)created at this point, we need to reset the latch so the
        // modern Visual Styles client area is re-carved against the new window.
        _triggerNewClientSizeRequest = false;

        // This "creatingHandle" stuff is to avoid property change events
        // when we set the Text property.
        _textBoxFlags[s_creatingHandle] = true;

        try
        {
            base.CreateHandle();

            // send EM_SETSEL message
            SetSelectionOnHandle();
        }
        finally
        {
            _textBoxFlags[s_creatingHandle] = false;
        }
    }

    /// <summary>
    ///  Moves the current selection in the text box to the Clipboard.
    /// </summary>
    public void Cut()
        => PInvokeCore.SendMessage(this, PInvokeCore.WM_CUT);

    /// <summary>
    ///  Returns the text end position (one past the last input character). This property is virtual to allow MaskedTextBox
    ///  to set the last input char position as opposed to the last char position which may be a mask character.
    /// </summary>
    internal virtual int GetEndPosition() =>
        // +1 because RichTextBox has this funny EOF pseudo-character after all the text.
        IsHandleCreated
            ? TextLength + 1
        : TextLength;

    /// <summary>
    ///  Overridden to handle TAB key.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.Alt) == Keys.Alt)
        {
            return base.IsInputKey(keyData);
        }

        switch (keyData & Keys.KeyCode)
        {
            case Keys.Tab:
                // Single-line RichEd's want tab characters (see WM_GETDLGCODE),
                // so we don't ask it
                return Multiline && _textBoxFlags[s_acceptsTab] && ((keyData & Keys.Control) == 0);

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

        if (_textBoxFlags[s_modified])
        {
            PInvokeCore.SendMessage(this, PInvokeCore.EM_SETMODIFY, (WPARAM)(BOOL)true);
        }

        EnsureReadonlyBackgroundColor(true);

        if (_textBoxFlags[s_scrollToCaretOnHandleCreated])
        {
            ScrollToCaret();
            _textBoxFlags[s_scrollToCaretOnHandleCreated] = false;
        }

        RecalculateVisualStylesClientArea();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _focusIndicatorRenderer?.Dispose();
        _focusIndicatorRenderer = null;
        _textBoxFlags[s_modified] = Modified;
        _textBoxFlags[s_setSelectionOnHandleCreated] = true;
        // Update text selection cached values to be restored when recreating the handle.
        GetSelectionStartAndLength(out _selectionStart, out _selectionLength);
        base.OnHandleDestroyed(e);
    }

    /// <inheritdoc/>
    protected override void OnVisualStylesModeChanged(EventArgs e)
    {
        // UpdateStyles may synchronously provoke WM_NCCALCSIZE, so it must observe a cleared latch.
        _triggerNewClientSizeRequest = false;
        base.OnVisualStylesModeChanged(e);
        AdjustHeight(false);
        _focusIndicatorRenderer?.Synchronize(Focused, invalidate: false);

        RecalculateVisualStylesClientArea();
    }

    /// <inheritdoc/>
    protected override void OnSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs e)
    {
        base.OnSystemVisualSettingsChanged(e);

        if ((e.Changed & (SystemVisualSettingsCategories.TextScale | SystemVisualSettingsCategories.FocusMetrics)) == 0
            || EffectiveVisualStylesMode < VisualStylesMode.Net11)
        {
            return;
        }

        CommonProperties.xClearPreferredSizeCache(this);
        AdjustHeight(false);
        RecalculateVisualStylesClientArea();

        if (ParentInternal is { } parent)
        {
            LayoutTransaction.DoLayout(parent, this, PropertyNames.SystemVisualSettings);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    ///  <para>
    ///   <see cref="PreferredHeightCore"/> and the modern non-client padding table are shared by
    ///   <see cref="VisualStylesMode.Net11"/> and <see cref="VisualStylesMode.Latest"/>. Consequently, switching
    ///   between those modes only repaints. Crossing the classic or disabled boundary changes both the preferred
    ///   height selection and the non-client padding model, so it requires a metrics update.
    ///  </para>
    /// </remarks>
    protected override VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
        VisualStylesMode oldMode,
        VisualStylesMode newMode)
    {
        bool oldUsesModernMetrics = oldMode >= VisualStylesMode.Net11;
        bool newUsesModernMetrics = newMode >= VisualStylesMode.Net11;

        return oldUsesModernMetrics != newUsesModernMetrics
            ? VisualStylesModeChangeImpact.Metrics
            : VisualStylesModeChangeImpact.Repaint;
    }

    /// <summary>
    ///  Replaces the current selection in the text box with the contents of the Clipboard.
    /// </summary>
    public void Paste()
        => PInvokeCore.SendMessage(this, PInvokeCore.WM_PASTE);

    protected override bool ProcessDialogKey(Keys keyData)
    {
        Keys keyCode = keyData & Keys.KeyCode;

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
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    protected virtual void OnAcceptsTabChanged(EventArgs e)
    {
        if (Events[s_acceptsTabChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected virtual void OnBorderStyleChanged(EventArgs e)
    {
        if (Events[s_borderStyleChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected override unsafe void OnGotFocus(EventArgs e)
    {
        if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
        {
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                FocusIndicatorRenderer.SetFocused(
                    focused: true,
                    animate: SystemInformation.UIEffectsEnabled && !SystemInformation.HighContrast);
            }
            else
            {
                InvalidateVisualStylesFrame();
            }
        }

        base.OnGotFocus(e);
    }

    protected override unsafe void OnLostFocus(EventArgs e)
    {
        if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
        {
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                FocusIndicatorRenderer.SetFocused(
                    focused: false,
                    animate: SystemInformation.UIEffectsEnabled && !SystemInformation.HighContrast);
            }
            else
            {
                InvalidateVisualStylesFrame();
            }
        }

        base.OnLostFocus(e);
    }

    protected override unsafe void OnSizeChanged(EventArgs e)
    {
        if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
        {
            // Invalidate the non-client area to ensure the border chrome is drawn correctly after a resize.
            PInvoke.RedrawWindow(
                hWnd: this,
                lprcUpdate: null,
                hrgnUpdate: HRGN.Null,
                flags: REDRAW_WINDOW_FLAGS.RDW_FRAME | REDRAW_WINDOW_FLAGS.RDW_INVALIDATE);
        }

        base.OnSizeChanged(e);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        CommonProperties.xClearPreferredSizeCache(this);
        LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.Font);
        AdjustHeight(false);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);

        CommonProperties.xClearPreferredSizeCache(this);
        LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.Bounds);
        AdjustHeight(false);

        if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
        {
            RecalculateVisualStylesClientArea();
        }
    }

    protected virtual void OnHideSelectionChanged(EventArgs e)
    {
        if (Events[s_hideSelectionChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected virtual void OnModifiedChanged(EventArgs e)
    {
        if (Events[s_modifiedChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the MouseUp event.
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        if (mevent is not null && mevent.Button == MouseButtons.Left)
        {
            if (!ValidationCancelled && PInvoke.WindowFromPoint(PointToScreen(mevent.Location)) == HWND)
            {
                if (!_doubleClickFired)
                {
                    OnClick(mevent);
                    OnMouseClick(mevent);
                }
                else
                {
                    _doubleClickFired = false;
                    OnDoubleClick(mevent);
                    OnMouseDoubleClick(mevent);
                }
            }

            _doubleClickFired = false;
        }

        // Because the code has been like that since long time, we assume that mevent is not null.
        base.OnMouseUp(mevent!);
    }

    protected virtual void OnMultilineChanged(EventArgs e)
    {
        if (Events[s_multilineChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        CommonProperties.xClearPreferredSizeCache(this);
        LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.Padding);
        AdjustHeight(false);

        // The carved modern Visual Styles padding band includes the user Padding, so a runtime change
        // has to re-provoke the non-client calculation for it to be reflected in the client rectangle.
        if (EffectiveVisualStylesMode >= VisualStylesMode.Net11)
        {
            RecalculateVisualStylesClientArea();
        }
    }

    protected virtual void OnReadOnlyChanged(EventArgs e)
    {
        if (Events[s_readOnlyChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        // since AutoSize existed in Everett, (and is the default) we can't
        // relayout the parent when the "PreferredSize" of the control changes.
        // this means a multiline = true textbox won't naturally grow in height when
        // the text changes.
        CommonProperties.xClearPreferredSizeCache(this);
        base.OnTextChanged(e);

        if (PInvoke.UiaClientsAreListening())
        {
            RaiseAccessibilityTextChangedEvent();
        }
    }

    private protected virtual void RaiseAccessibilityTextChangedEvent()
    {
        if (IsAccessibilityObjectCreated)
        {
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextChangedEventId);
            using var textVariant = PasswordProtect ? (VARIANT)string.Empty : (VARIANT)Text;
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_ValueValuePropertyId, textVariant, textVariant);
        }
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
        int index = (int)PInvokeCore.SendMessage(this, PInvokeCore.EM_CHARFROMPOS, 0, PARAM.FromPoint(pt));
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
        => (int)PInvokeCore.SendMessage(this, PInvokeCore.EM_LINEFROMCHAR, (WPARAM)index);

    /// <summary>
    ///  Returns the location of the character at the given index.
    /// </summary>
    public virtual Point GetPositionFromCharIndex(int index)
    {
        if (index < 0 || index >= Text.Length)
        {
            return Point.Empty;
        }

        int i = (int)PInvokeCore.SendMessage(
            hWnd: this,
            Msg: PInvokeCore.EM_POSFROMCHAR,
            wParam: (WPARAM)index);

        return new Point(PARAM.SignedLOWORD(i), PARAM.SignedHIWORD(i));
    }

    /// <summary>
    ///  Returns the index of the first character of a given line. Returns -1 of lineNumber is invalid.
    /// </summary>
    public int GetFirstCharIndexFromLine(int lineNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(lineNumber);

        return (int)PInvokeCore.SendMessage(
            hWnd: this,
            Msg: PInvokeCore.EM_LINEINDEX,
            wParam: (WPARAM)lineNumber);
    }

    /// <summary>
    ///  Returns the index of the first character of the line where the caret is.
    /// </summary>
    public int GetFirstCharIndexOfCurrentLine()
        => (int)PInvokeCore.SendMessage(
            hWnd: this,
            Msg: PInvokeCore.EM_LINEINDEX,
            wParam: (WPARAM)(-1));

    /// <summary>
    ///  Ensures that the caret is visible in the TextBox window, by scrolling the
    ///  TextBox control surface if necessary.
    /// </summary>
    public void ScrollToCaret()
    {
        if (!IsHandleCreated)
        {
            _textBoxFlags[s_scrollToCaretOnHandleCreated] = true;
            return;
        }

        if (string.IsNullOrEmpty(WindowText))
        {
            // If there is no text, then there is no place to go.
            return;
        }

        ScrollToCaretCore();
    }

    /// <summary>
    ///  Performs the control-specific work of scrolling the caret into view. The base implementation
    ///  asks the native edit control to scroll the caret into view; <see cref="RichTextBox"/> overrides
    ///  this to additionally show as much of the surrounding text as possible.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Invoked by <see cref="ScrollToCaret"/> only once the handle exists and the control has text, so
    ///   overrides do not need to re-check those conditions.
    ///  </para>
    /// </remarks>
    private protected virtual void ScrollToCaretCore()
        => PInvokeCore.SendMessage(this, PInvokeCore.EM_SCROLLCARET);

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
        ArgumentOutOfRangeException.ThrowIfNegative(start);

        int textLen = TextLength;

        if (start > textLen)
        {
            // We shouldn't allow positive length if you're starting at the end, but
            // should allow negative length.
            long longLength = Math.Min(0, (long)length + start - textLen);

            length = longLength < int.MinValue
                ? int.MinValue
                : (int)longLength;

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
    private protected virtual void SelectInternal(int selectionStart, int selectionLength, int textLength)
    {
        // if our handle is created - send message...
        if (!IsHandleCreated)
        {
            // otherwise, wait until handle is created to send this message.
            // Store the indices until then...
            _selectionStart = selectionStart;
            _selectionLength = selectionLength;
            _textBoxFlags[s_setSelectionOnHandleCreated] = true;
        }
        else
        {
            AdjustSelectionStartAndEnd(
                selectionStart,
                selectionLength,
                out int start,
                out int end,
                textLength);

            PInvokeCore.SendMessage(this, PInvokeCore.EM_SETSEL, (WPARAM)start, (LPARAM)end);

            if (IsAccessibilityObjectCreated)
            {
                AccessibilityObject.RaiseAutomationEvent(end == 0
                    ? UIA_EVENT_ID.UIA_AutomationFocusChangedEventId
                    : UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
            }
        }
    }

    /// <summary>
    ///  Selects all text in the text box.
    /// </summary>
    public void SelectAll()
    {
        int textLength = TextLength;
        SelectInternal(0, textLength, textLength);
    }

    /// <summary>
    ///  Overrides Control.setBoundsCore to enforce autoSize.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (!_integralHeightAdjust && height != Height)
        {
            _requestedHeight = height;
        }

        if (_textBoxFlags[s_autoSize] && !_textBoxFlags[s_multiline])
        {
            height = PreferredHeight;
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    private static void Swap(ref int n1, ref int n2)
        => (n1, n2) = (n2, n1);

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
            int textLength = textLen >= 0
                ? textLen
                : TextLength;

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
                    // Since we overflowed, cap at the max/min value: we'll correct the value below
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

        if (_textBoxFlags[s_setSelectionOnHandleCreated])
        {
            _textBoxFlags[s_setSelectionOnHandleCreated] = false;
            AdjustSelectionStartAndEnd(_selectionStart, _selectionLength, out int start, out int end, -1);
            PInvokeCore.SendMessage(this, PInvokeCore.EM_SETSEL, (WPARAM)start, (LPARAM)end);
        }
    }

    /// <summary>
    ///  Converts byte offset to unicode offsets.
    ///  When processing WM_GETSEL/WM_SETSEL, EDIT control works with byte offsets instead of character positions
    ///  as opposed to RICHEDIT which does it always as character positions.
    ///  This method is used when handling the WM_GETSEL message.
    /// </summary>
    private static void ToUnicodeOffsets(string str, ref int start, ref int end)
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
    ///  Converts unicode offset to byte offsets.
    ///  When processing WM_GETSEL/WM_SETSEL, EDIT control works with byte offsets instead of character positions
    ///  as opposed to RICHEDIT which does it always as character positions.
    ///  This method is used when handling the WM_SETSEL message.
    /// </summary>
    internal static void ToDbcsOffsets(string str, ref int start, ref int end)
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

        int newStart = start == 0 ? 0 : e.GetByteCount(str.AsSpan(0, start));
        end = newStart + e.GetByteCount(str.AsSpan(start, end - start));
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
            txt = $"{txt.AsSpan(0, 40)}...";
        }

        return $"{s}, Text: {txt}";
    }

    /// <summary>
    ///  Undoes the last edit operation in the text box.
    /// </summary>
    public void Undo()
        => PInvokeCore.SendMessage(this, PInvokeCore.EM_UNDO);

    internal virtual void UpdateMaxLength()
    {
        if (IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvokeCore.EM_LIMITTEXT, (WPARAM)_maxLength);
        }
    }

    internal override HBRUSH InitializeDCForWmCtlColor(HDC dc, MessageId msg)
    {
        InitializeClientArea(dc, (HWND)Handle);

        if (msg == PInvokeCore.WM_CTLCOLORSTATIC && !ShouldSerializeBackColor())
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

    /// <summary>
    ///  Provokes the single non-client calc round trip that carves the modern Visual Styles padding
    ///  band from the client area.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This runs only for <see cref="VisualStylesMode"/> values of <see cref="VisualStylesMode.Net11"/>
    ///   and above, and only once per handle. The latch (<see cref="_triggerNewClientSizeRequest"/>) is
    ///   reset on handle recreation in <see cref="CreateHandle"/>.
    ///  </para>
    /// </remarks>
    private protected virtual unsafe void InitializeClientArea(HDC hDC, HWND hwnd)
    {
        if (EffectiveVisualStylesMode < VisualStylesMode.Net11
            || _triggerNewClientSizeRequest)
        {
            return;
        }

        RecalculateVisualStylesClientArea();
    }

    /// <summary>
    ///  Sets the latch and provokes the <c>WM_NCCALCSIZE</c> round trip that carves the modern Visual
    ///  Styles padding band from the client area. Safe to call repeatedly; it re-carves against the
    ///  current <see cref="Padding"/> and scrollbar state.
    /// </summary>
    private protected void RecalculateVisualStylesClientArea()
    {
        if (!IsHandleCreated || EffectiveVisualStylesMode < VisualStylesMode.Net11)
        {
            return;
        }

        _triggerNewClientSizeRequest = true;

        // Call SetWindowPos with the current bounds and the SWP_FRAMECHANGED flag. We do not change the
        // window position/size, but this provokes the WM_NCCALCSIZE message we need to carve the client area.
        PInvoke.SetWindowPos(
            hWnd: this,
            hWndInsertAfter: HWND.HWND_TOP,
            X: 0,
            Y: 0,
            cx: 0,
            cy: 0,
            uFlags: SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED
                | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                | SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                | SET_WINDOW_POS_FLAGS.SWP_NOSIZE
                | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);

        Invalidate(true);
    }

    /// <summary>
    ///  When <see langword="true"/>, the native window reserves its own non-client metrics (border and
    ///  scrollbars) during <c>WM_NCCALCSIZE</c>, so the modern Visual Styles padding band is carved from
    ///  the client rectangle the default handler produces rather than from the raw proposed window
    ///  rectangle. The default is <see langword="false"/>: the managed carve is authoritative and the
    ///  default handler is not invoked (a plain <c>EDIT</c> control keeps its scrollbars inside the
    ///  managed <see cref="GetScrollBarPadding"/> allowance).
    /// </summary>
    private protected virtual bool ReservesNativeNonClientArea => false;

    /// <summary>
    ///  Handles <c>WM_NCCALCSIZE</c> by carving the modern Visual Styles padding band from the client
    ///  rectangle. The carve is floored so the client rectangle can never invert.
    /// </summary>
    private unsafe void WmNcCalcSize(ref Message m)
    {
        // Make sure we actually kicked this off.
        if (_triggerNewClientSizeRequest)
        {
            NCCALCSIZE_PARAMS* ncCalcSizeParams = (NCCALCSIZE_PARAMS*)(void*)m.LParamInternal;

            if (ncCalcSizeParams is not null)
            {
                // Controls whose native window reserves its own non-client metrics (RichEdit reserves its
                // border and scrollbars) must run the default handler first, so we carve the modern padding
                // band from the already-adjusted client rectangle rather than the raw proposed window
                // rectangle. Their managed scrollbar allowance is omitted from this carve because the
                // native handler already reserved it.
                if (ReservesNativeNonClientArea)
                {
                    base.WndProc(ref m);
                }

                Padding padding = GetVisualStylesPadding(includeScrollbars: !ReservesNativeNonClientArea);

                ref RECT clientRect = ref ncCalcSizeParams->rgrc._0;

                // Never-invert clamp: a large Padding plus the live scrollbar allowance can drive the
                // carved client rect to zero or inverted. A 0-1px client area is acceptable and intended
                // (shipping multiline TextBox already collapses this way); we only prevent underflow past
                // zero. This is deliberately NOT a MinimumSize and does not vary by VisualStylesMode.
                int newTop = clientRect.top + padding.Top;
                int newBottom = clientRect.bottom - padding.Bottom;
                int newLeft = clientRect.left + padding.Left;
                int newRight = clientRect.right - padding.Right;

                clientRect.top = newTop;
                clientRect.bottom = Math.Max(newTop, newBottom);
                clientRect.left = newLeft;
                clientRect.right = Math.Max(newLeft, newRight);

                m.ResultInternal = (LRESULT)0;

                return;
            }
        }

        base.WndProc(ref m);
    }

    /// <summary>
    ///  Handles <c>WM_NCPAINT</c> for modern Visual Styles by painting the custom chrome into the
    ///  window DC. The <c>wParam</c> clip region is intentionally ignored for our own paint and the
    ///  custom frame is repainted to avoid the offscreen-restore "dirty corners" artifact.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The default (themed) non-client paint for a scrollbar-bearing edit control fills the client
    ///   rectangle, which erases the control's content on every non-client repaint - for example when
    ///   the mouse hovers over the frame. To prevent that, the default handler is invoked with an update
    ///   region that spans the whole window but excludes the live client rectangle, so the border and
    ///   scrollbars still paint while the client area is left untouched.
    ///  </para>
    ///  <para>
    ///   The custom chrome blit also excludes native scrollbars that are currently visible. This keeps
    ///   the native scrollbar pixels painted by the default handler from being covered by the frame.
    ///  </para>
    /// </remarks>
    private void WmNcPaint(ref Message m)
    {
        if (EffectiveVisualStylesMode < VisualStylesMode.Net11)
        {
            base.WndProc(ref m);
            return;
        }

        HWND hwnd = (HWND)m.HWnd;

        // A non-client update region (in screen coordinates, as WM_NCPAINT requires) that excludes the
        // client rectangle. Handing this to the default handler keeps it from overpainting the client.
        using RegionScope nonClientRegion = CreateNonClientClipRegion();
        WPARAM originalWParam = m.WParamInternal;

        HDC hdc = PInvokeCore.GetWindowDC(hwnd);

        // Intentional: Graphics.FromHdc does NOT own the DC, so we release the DC ourselves in the
        // finally. Do not "tidy" the DC into a single using - that would be a regression.
        using Graphics graphics = Graphics.FromHdc(hdc);

        try
        {
            if (!nonClientRegion.IsNull)
            {
                m.WParamInternal = (WPARAM)(nuint)(nint)nonClientRegion.Region;
            }

            base.WndProc(ref m);

            // Restore the original wParam so nothing downstream observes our temporary clip region.
            m.WParamInternal = originalWParam;

            OnNcPaint(graphics, hdc);
        }
        finally
        {
            int result = PInvokeCore.ReleaseDC(hwnd, hdc);
            Debug.Assert(result != 0);
        }
    }

    private void WmPrint(ref Message m)
    {
        base.WndProc(ref m);

        if (EffectiveVisualStylesMode < VisualStylesMode.Net11
            || ((nint)m.LParamInternal & PInvoke.PRF_NONCLIENT) == 0)
        {
            return;
        }

        HDC hdc = (HDC)m.WParamInternal;

        if (hdc.IsNull)
        {
            return;
        }

        using Graphics graphics = Graphics.FromHdc(hdc);
        OnNcPaint(graphics, hdc);
    }

    /// <summary>
    ///  Builds a non-client update region, in screen coordinates, that spans the whole window but
    ///  excludes the live client rectangle. This is handed to the default <c>WM_NCPAINT</c> handler so
    ///  it cannot overpaint (erase) the client area of scrollbar-bearing edit controls.
    /// </summary>
    /// <returns>
    ///  The non-client region scope, or a null scope when the window rectangle cannot be retrieved.
    /// </returns>
    private RegionScope CreateNonClientClipRegion()
    {
        if (!PInvokeCore.GetWindowRect(this, out RECT windowRect))
        {
            return new RegionScope(HRGN.Null);
        }

        PInvokeCore.GetClientRect(this, out RECT clientRect);

        Point clientTopLeft = default;
        PInvoke.ClientToScreen(this, ref clientTopLeft);

        RegionScope nonClientRegion = new(windowRect.left, windowRect.top, windowRect.right, windowRect.bottom);

        using RegionScope clientRegion = new(
            clientTopLeft.X,
            clientTopLeft.Y,
            clientTopLeft.X + clientRect.Width,
            clientTopLeft.Y + clientRect.Height);

        if (PInvokeCore.CombineRgn(nonClientRegion.Region, nonClientRegion.Region, clientRegion.Region, RGN_COMBINE_MODE.RGN_DIFF)
            == GDI_REGION_TYPE.RGN_ERROR)
        {
            nonClientRegion.Dispose();
            return new RegionScope(HRGN.Null);
        }

        return nonClientRegion;
    }

    /// <summary>
    ///  Paints the modern Visual Styles non-client chrome (border, rounded <see cref="BorderStyle.Fixed3D"/>
    ///  lozenge, focus indicator) into a shared offscreen buffer and blits it to the supplied window DC.
    /// </summary>
    private protected virtual void OnNcPaint(Graphics graphics, HDC windowHdc)
    {
        int cornerRadius = ScaleVisualStylesMetric(ModernControlVisualStyles.FieldCornerRadius);
        Size focusBorderMetrics = GetVisualStylesFocusBorderMetrics();
        int borderThickness = Math.Max(focusBorderMetrics.Width, focusBorderMetrics.Height);
        int focusBandHeight = GetVisualStylesFocusBandHeight();

        Color adornerColor = ForeColor;

        Color clientBackColor = BackColor;
        Color parentBackColor = Parent?.BackColor ?? BackColor;

        using var clientBackgroundBrush = clientBackColor.GetCachedSolidBrushScope();
        using var adornerBrush = adornerColor.GetCachedSolidBrushScope();
        using var adornerPen = adornerColor.GetCachedPenScope(borderThickness);

        Rectangle bounds = new(
            x: 0,
            y: 0,
            width: Bounds.Width,
            height: Bounds.Height);

        // Repaint the outermost client pixel with the chrome background so no residual native edge can
        // remain between the managed frame and edit surface. The rest of the live client stays protected.
        Rectangle nativeClientBounds = Rectangle.Intersect(bounds, GetNativeClientRectangle());
        Rectangle protectedClientBounds = GetProtectedClientBounds(nativeClientBounds);
        Rectangle[] scrollBarBounds = GetVisibleScrollBarRectangles(bounds);

        Rectangle deflatedBounds = bounds;

        // Making sure we never color outside the lines.
        deflatedBounds.Width -= 1;
        deflatedBounds.Height -= 1;

        // Keep the target clip excluded from the GDI+ drawing as well as from the explicit blits below.
        using Region region = new(bounds);
        graphics.Clip = region;
        graphics.ExcludeClip(protectedClientBounds);

        // WinForms paints NC serially (one HWND at a time on the UI thread), so a single shared buffer
        // suffices for any number of controls. The buffer is reused when the size fits; steady-state
        // allocation is zero.
        BufferedGraphicsContext context = BufferedGraphicsManager.Current;
        using BufferedGraphics buffer = context.Allocate(graphics, bounds);

        // Intentional: the buffer owns this Graphics - do NOT dispose buffer.Graphics separately.
        Graphics offscreenGraphics = buffer.Graphics;
        Rectangle bufferBounds = bounds;

        // We need anti-aliasing for the rounded chrome.
        offscreenGraphics.SmoothingMode = SmoothingMode.AntiAlias;

        // AddRoundedRectangle receives the bounding size of each corner arc, so one corner size plus
        // the border thickness is the minimum height that avoids overlapping curves.
        bool canRenderRoundedChrome = CanRenderVisualStylesRoundedChrome(
            deflatedBounds,
            cornerRadius,
            borderThickness);

        if (BorderStyle == BorderStyle.Fixed3D && canRenderRoundedChrome)
        {
            ParentBackgroundRenderer.Paint(this, offscreenGraphics, bufferBounds, parentBackColor);
        }
        else
        {
            using var parentBackgroundBrush = parentBackColor.GetCachedSolidBrushScope();
            offscreenGraphics.FillRectangle(parentBackgroundBrush, bounds);
        }

        switch (BorderStyle)
        {
            case BorderStyle.None:

                // Just fill a rectangle.
                offscreenGraphics.FillRectangle(clientBackgroundBrush, deflatedBounds);
                break;

            case BorderStyle.FixedSingle:

                offscreenGraphics.FillRectangle(clientBackgroundBrush, deflatedBounds);
                offscreenGraphics.DrawRectangle(adornerPen, deflatedBounds);
                break;

            case BorderStyle.Fixed3D:

                if (canRenderRoundedChrome)
                {
                    using GraphicsPath roundedBodyPath = new();
                    roundedBodyPath.AddRoundedRectangle(deflatedBounds, new Size(cornerRadius, cornerRadius));
                    offscreenGraphics.FillPath(clientBackgroundBrush, roundedBodyPath);
                    offscreenGraphics.DrawPath(adornerPen, roundedBodyPath);

                    // The rounded chrome is clipped with a non-antialiased region; blend the resulting
                    // corner artifacts into the parent by tracing the parent color just outside the border.
                    ParentBackgroundRenderer.PaintRoundedBorderRegionMitigation(
                        offscreenGraphics,
                        deflatedBounds,
                        new Size(cornerRadius, cornerRadius),
                        borderThickness,
                        parentBackColor);
                }
                else
                {
                    // Chrome degradation fallback - flat render in place of the broken lozenge.
                    offscreenGraphics.FillRectangle(clientBackgroundBrush, deflatedBounds);
                    offscreenGraphics.DrawRectangle(adornerPen, deflatedBounds);
                }

                break;
        }

        if (BorderStyle == BorderStyle.Fixed3D && canRenderRoundedChrome)
        {
            Color focusColor = GetVisualStylesFocusColor(Application.SystemVisualSettings.HighContrastEnabled);
            FocusIndicatorRenderer.DrawRoundedFocusIndicator(
                offscreenGraphics,
                deflatedBounds,
                cornerRadius,
                borderThickness,
                focusBandHeight,
                adornerColor,
                focusColor);
        }
        else if (Focused)
        {
            Color focusColor = GetVisualStylesFocusColor(Application.SystemVisualSettings.HighContrastEnabled);
            using var focusPen = focusColor.GetCachedPenScope(borderThickness);
            int focusLineCount = Math.Min(
                Math.Max(2, focusBorderMetrics.Height),
                Math.Max(1, deflatedBounds.Height));
            for (int i = 0; i < focusLineCount; i++)
            {
                offscreenGraphics.DrawLine(
                    focusPen,
                    deflatedBounds.Left,
                    deflatedBounds.Bottom - i,
                    deflatedBounds.Right,
                    deflatedBounds.Bottom - i);
            }
        }

        Rectangle[] nonClientBands = GetNonClientPaintBands(
            bufferBounds,
            protectedClientBounds,
            scrollBarBounds);
        IntPtr bufferHdc = offscreenGraphics.GetHdc();

        try
        {
            foreach (Rectangle band in nonClientBands)
            {
                if (band.Width > 0 && band.Height > 0)
                {
                    PInvokeCore.BitBlt(
                        hdc: windowHdc,
                        x: band.X,
                        y: band.Y,
                        cx: band.Width,
                        cy: band.Height,
                        hdcSrc: (HDC)bufferHdc,
                        x1: band.X,
                        y1: band.Y,
                        rop: ROP_CODE.SRCCOPY);
                }
            }
        }
        finally
        {
            offscreenGraphics.ReleaseHdcInternal(bufferHdc);
        }
    }

    private static Rectangle[] GetNonClientPaintBands(Rectangle bounds, Rectangle clientBounds)
        => GetNonClientPaintBands(bounds, clientBounds, []);

    private static Rectangle GetProtectedClientBounds(Rectangle clientBounds)
        => clientBounds.Width > 2 && clientBounds.Height > 2
            ? Rectangle.Inflate(clientBounds, -1, -1)
            : clientBounds;

    private static Rectangle[] GetNonClientPaintBands(
        Rectangle bounds,
        Rectangle clientBounds,
        Rectangle[] additionalProtectedBounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return [Rectangle.Empty, Rectangle.Empty, Rectangle.Empty, Rectangle.Empty];
        }

        Rectangle protectedBounds = Rectangle.Intersect(bounds, clientBounds);

        Rectangle[] initialBands = protectedBounds.Width <= 0 || protectedBounds.Height <= 0
            ? [bounds, Rectangle.Empty, Rectangle.Empty, Rectangle.Empty]
            : [
                Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, protectedBounds.Top),
                Rectangle.FromLTRB(bounds.Left, protectedBounds.Bottom, bounds.Right, bounds.Bottom),
                Rectangle.FromLTRB(bounds.Left, protectedBounds.Top, protectedBounds.Left, protectedBounds.Bottom),
                Rectangle.FromLTRB(protectedBounds.Right, protectedBounds.Top, bounds.Right, protectedBounds.Bottom)
              ];

        if (additionalProtectedBounds.Length == 0)
        {
            return initialBands;
        }

        List<Rectangle> paintBands = [.. initialBands];

        foreach (Rectangle additionalProtectedBoundsItem in additionalProtectedBounds)
        {
            Rectangle clippedProtectedBounds = Rectangle.Intersect(bounds, additionalProtectedBoundsItem);

            if (clippedProtectedBounds.Width <= 0 || clippedProtectedBounds.Height <= 0)
            {
                continue;
            }

            for (int i = paintBands.Count - 1; i >= 0; i--)
            {
                Rectangle band = paintBands[i];
                Rectangle intersection = Rectangle.Intersect(band, clippedProtectedBounds);

                if (intersection.Width <= 0 || intersection.Height <= 0)
                {
                    continue;
                }

                paintBands.RemoveAt(i);
                AddPaintBand(Rectangle.FromLTRB(band.Left, band.Top, band.Right, intersection.Top));
                AddPaintBand(Rectangle.FromLTRB(band.Left, intersection.Bottom, band.Right, band.Bottom));
                AddPaintBand(Rectangle.FromLTRB(band.Left, intersection.Top, intersection.Left, intersection.Bottom));
                AddPaintBand(Rectangle.FromLTRB(intersection.Right, intersection.Top, band.Right, intersection.Bottom));
            }
        }

        return [.. paintBands];

        void AddPaintBand(Rectangle band)
        {
            if (band.Width > 0 && band.Height > 0)
            {
                paintBands.Add(band);
            }
        }
    }

    private unsafe Rectangle[] GetVisibleScrollBarRectangles(Rectangle bounds)
    {
        if (!IsHandleCreated
            || !PInvokeCore.GetWindowRect(this, out RECT windowRect))
        {
            return [];
        }

        List<Rectangle> scrollBarBounds = [];
        AddVisibleScrollBar(HorizontalScrollBarObjectId);
        AddVisibleScrollBar(VerticalScrollBarObjectId);

        return [.. scrollBarBounds];

        void AddVisibleScrollBar(OBJECT_IDENTIFIER objectId)
        {
            SCROLLBARINFO scrollBarInfo = new()
            {
                cbSize = (uint)sizeof(SCROLLBARINFO)
            };

            if (!PInvoke.GetScrollBarInfo((HWND)Handle, objectId, ref scrollBarInfo)
                || (scrollBarInfo.rgstate[0] & StateSystemInvisible) != 0)
            {
                return;
            }

            Rectangle scrollBarRectangle = Rectangle.FromLTRB(
                scrollBarInfo.rcScrollBar.left - windowRect.left,
                scrollBarInfo.rcScrollBar.top - windowRect.top,
                scrollBarInfo.rcScrollBar.right - windowRect.left,
                scrollBarInfo.rcScrollBar.bottom - windowRect.top);
            scrollBarRectangle.Intersect(bounds);

            if (scrollBarRectangle.Width > 0 && scrollBarRectangle.Height > 0)
            {
                scrollBarBounds.Add(scrollBarRectangle);
            }
        }
    }

    private Rectangle GetNativeClientRectangle()
    {
        if (!IsHandleCreated
            || !PInvokeCore.GetWindowRect(this, out RECT windowRect))
        {
            return Rectangle.Empty;
        }

        PInvokeCore.GetClientRect(this, out RECT clientRect);
        Point clientTopLeft = default;
        PInvoke.ClientToScreen(this, ref clientTopLeft);

        return new Rectangle(
            clientTopLeft.X - windowRect.left,
            clientTopLeft.Y - windowRect.top,
            clientRect.Width,
            clientRect.Height);
    }

    private AnimatedFocusIndicatorRenderer FocusIndicatorRenderer
        => _focusIndicatorRenderer ??= new(this, InvalidateVisualStylesFrame);

    private unsafe void InvalidateVisualStylesFrame()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        PInvoke.RedrawWindow(
            hWnd: this,
            lprcUpdate: null,
            hrgnUpdate: HRGN.Null,
            flags: REDRAW_WINDOW_FLAGS.RDW_FRAME | REDRAW_WINDOW_FLAGS.RDW_INVALIDATE);
    }

    private void WmReflectCommand(ref Message m)
    {
        if (_textBoxFlags[s_codeUpdateText] || _textBoxFlags[s_creatingHandle])
        {
            return;
        }

        uint hiword = m.WParamInternal.HIWORD;

        if (hiword == PInvoke.EN_CHANGE && CanRaiseTextChangedEvent)
        {
            OnTextChanged(EventArgs.Empty);
        }
        else if (hiword == PInvoke.EN_UPDATE)
        {
            // Force update to the Modified property, which will trigger ModifiedChanged event handlers
            _ = Modified;
        }
    }

    private void WmSetFont(ref Message m)
    {
        base.WndProc(ref m);

        if (!_textBoxFlags[s_multiline])
        {
            PInvokeCore.SendMessage(this, PInvokeCore.EM_SETMARGINS, (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN));
        }
    }

    private void WmGetDlgCode(ref Message m)
    {
        base.WndProc(ref m);

        m.ResultInternal = AcceptsTab
            ? (LRESULT)(nint)(m.ResultInternal | (int)PInvoke.DLGC_WANTTAB)
            : (LRESULT)(nint)(m.ResultInternal & ~(int)(PInvoke.DLGC_WANTTAB
                | PInvoke.DLGC_WANTALLKEYS));
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

        Point client;
        bool keyboardActivated = false;

        // LParam will be -1 when the user invokes the context menu with the keyboard.
        if (m.LParamInternal == -1)
        {
            keyboardActivated = true;
            client = new Point(Width / 2, Height / 2);
        }
        else
        {
            client = PointToClient(PARAM.ToPoint(m.LParamInternal));
        }

        // Only show the context menu when clicked in the client area.
        if (ClientRectangle.Contains(client))
        {
            ContextMenuStrip.ShowInternal(this, client, keyboardActivated);
        }
    }

    /// <summary>
    ///  The control's window procedure. Inheriting classes can override this
    ///  to add extra functionality, but should not forget to call
    ///  base.wndProc(m); to ensure the control continues to function properly.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_NCCALCSIZE:
                WmNcCalcSize(ref m);
                break;

            case PInvokeCore.WM_NCPAINT:
                WmNcPaint(ref m);
                break;
            case PInvokeCore.WM_PRINT:
                WmPrint(ref m);
                break;

            case PInvokeCore.WM_LBUTTONDBLCLK:
                _doubleClickFired = true;
                base.WndProc(ref m);
                break;

            case MessageId.WM_REFLECT_COMMAND:
                WmReflectCommand(ref m);
                break;

            case PInvokeCore.WM_GETDLGCODE:
                WmGetDlgCode(ref m);
                break;

            case PInvokeCore.WM_SETFONT:
                WmSetFont(ref m);
                break;

            case PInvokeCore.WM_CONTEXTMENU:
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

            case PInvokeCore.WM_DESTROY:
                if (TryGetAccessibilityObject(out AccessibleObject? @object) && @object is TextBoxBaseAccessibleObject accessibleObject &&
                    !RecreatingHandle)
                {
                    accessibleObject.ClearObjects();
                }

                base.WndProc(ref m);

                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }
}
