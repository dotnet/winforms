// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Represents a Windows text box control.
/// </summary>
[Designer($"System.Windows.Forms.Design.TextBoxDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionTextBox))]
public partial class TextBox : TextBoxBase
{
    private static readonly object s_textAlignChangedEvent = new();

    /// <summary>
    ///  Controls whether or not the edit box consumes/respects ENTER key
    ///  presses.  While this is typically desired by multiline edits, this
    ///  can interfere with normal key processing in a dialog.
    /// </summary>
    private bool _acceptsReturn;

    /// <summary>
    ///  Indicates what the current special password character is.  This is
    ///  displayed instead of any other text the user might enter.
    /// </summary>
    private char _passwordChar;

    private bool _useSystemPasswordChar;

    /// <summary>
    ///  Controls whether or not the case of characters entered into the edit
    ///  box is forced to a specific case.
    /// </summary>
    private CharacterCasing _characterCasing = CharacterCasing.Normal;

    /// <summary>
    ///  Controls which scrollbars appear by default.
    /// </summary>
    private ScrollBars _scrollBars = ScrollBars.None;

    /// <summary>
    ///  Controls text alignment in the edit box.
    /// </summary>
    private HorizontalAlignment _textAlign = HorizontalAlignment.Left;

    /// <summary>
    ///  True if the selection has been set by the user.  If the selection has
    ///  never been set and we get focus, we focus all the text in the control
    ///  so we mimic the Windows dialog manager.
    /// </summary>
    private bool _selectionSet;

    /// <summary>
    ///  This stores the value for the autocomplete mode which can be either
    ///  None, AutoSuggest, AutoAppend or AutoSuggestAppend.
    /// </summary>
    private AutoCompleteMode _autoCompleteMode = AutoCompleteMode.None;

    /// <summary>
    ///  This stores the value for the autoCompleteSource mode which can be one of the values
    ///  from AutoCompleteSource enum.
    /// </summary>
    private AutoCompleteSource _autoCompleteSource = AutoCompleteSource.None;

    /// <summary>
    ///  This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
    /// </summary>
    private AutoCompleteStringCollection? _autoCompleteCustomSource;
    private bool _fromHandleCreate;
    private StringSource? _stringSource;
    private string _placeholderText = string.Empty;

    public TextBox()
    {
    }

    /// <summary>
    ///  Gets or sets a value indicating whether pressing ENTER in a multiline <see cref="TextBox"/>
    ///  control creates a new line of text in the control or activates the default button
    ///  for the form.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TextBoxAcceptsReturnDescr))]
    public bool AcceptsReturn
    {
        get
        {
            return _acceptsReturn;
        }
        set
        {
            _acceptsReturn = value;
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new TextBoxAccessibleObject(this);

    /// <summary>
    ///  This is the AutoCompleteMode which can be either
    ///  None, AutoSuggest, AutoAppend or AutoSuggestAppend.
    ///  This property in conjunction with AutoCompleteSource enables the AutoComplete feature for TextBox.
    /// </summary>
    [DefaultValue(AutoCompleteMode.None)]
    [SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteMode AutoCompleteMode
    {
        get
        {
            return _autoCompleteMode;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            bool resetAutoComplete = false;
            if (_autoCompleteMode != AutoCompleteMode.None && value == AutoCompleteMode.None)
            {
                resetAutoComplete = true;
            }

            _autoCompleteMode = value;
            SetAutoComplete(resetAutoComplete);
        }
    }

    /// <summary>
    ///  This is the AutoCompleteSource which can be one of the values from AutoCompleteSource enumeration.
    ///  This property in conjunction with AutoCompleteMode enables the AutoComplete feature for TextBox.
    /// </summary>
    [DefaultValue(AutoCompleteSource.None)]
    [SRDescription(nameof(SR.TextBoxAutoCompleteSourceDescr))]
    [TypeConverter(typeof(TextBoxAutoCompleteSourceConverter))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteSource AutoCompleteSource
    {
        get
        {
            return _autoCompleteSource;
        }
        set
        {
            switch (value)
            {
                case AutoCompleteSource.None:
                case AutoCompleteSource.AllSystemSources:
                case AutoCompleteSource.AllUrl:
                case AutoCompleteSource.CustomSource:
                case AutoCompleteSource.FileSystem:
                case AutoCompleteSource.FileSystemDirectories:
                case AutoCompleteSource.HistoryList:
                case AutoCompleteSource.RecentlyUsedList:
                    _autoCompleteSource = value;
                    SetAutoComplete(false);
                    break;
                case AutoCompleteSource.ListItems:
                    throw new NotSupportedException(SR.TextBoxAutoCompleteSourceNoItems);
                default:
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoCompleteSource));
            }
        }
    }

    /// <summary>
    ///  This is the AutoCompleteCustomSource which is custom StringCollection used when the
    ///  AutoCompleteSource is CustomSource.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [AllowNull]
    public AutoCompleteStringCollection AutoCompleteCustomSource
    {
        get
        {
            if (_autoCompleteCustomSource is null)
            {
                _autoCompleteCustomSource = [];
                _autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
            }

            return _autoCompleteCustomSource;
        }
        set
        {
            if (_autoCompleteCustomSource != value)
            {
                if (_autoCompleteCustomSource is not null)
                {
                    _autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                }

                _autoCompleteCustomSource = value;

                if (_autoCompleteCustomSource is not null)
                {
                    _autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                }

                SetAutoComplete(false);
            }
        }
    }

    /// <summary>
    ///  Gets or sets whether the TextBox control
    ///  modifies the case of characters as they are typed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(CharacterCasing.Normal)]
    [SRDescription(nameof(SR.TextBoxCharacterCasingDescr))]
    public CharacterCasing CharacterCasing
    {
        get
        {
            return _characterCasing;
        }
        set
        {
            if (_characterCasing != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _characterCasing = value;
                RecreateHandle();
            }
        }
    }

    public override bool Multiline
    {
        get => base.Multiline;
        set
        {
            if (Multiline != value)
            {
                base.Multiline = value;
                if (value && AutoCompleteMode != AutoCompleteMode.None)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  Determines if the control is in password protect mode.
    /// </summary>
    private protected override bool PasswordProtect
        => PasswordChar != '\0';

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
            switch (_characterCasing)
            {
                case CharacterCasing.Lower:
                    cp.Style |= PInvoke.ES_LOWERCASE;
                    break;
                case CharacterCasing.Upper:
                    cp.Style |= PInvoke.ES_UPPERCASE;
                    break;
            }

            // Translate for Rtl if necessary
            HorizontalAlignment align = RtlTranslateHorizontal(_textAlign);

            // WS_EX_RIGHT overrides the ES_XXXX alignment styles
            cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_RIGHT;

            switch (align)
            {
                case HorizontalAlignment.Left:
                    cp.Style |= PInvoke.ES_LEFT;
                    break;
                case HorizontalAlignment.Center:
                    cp.Style |= PInvoke.ES_CENTER;
                    break;
                case HorizontalAlignment.Right:
                    cp.Style |= PInvoke.ES_RIGHT;
                    break;
            }

            if (Multiline)
            {
                // Don't show horizontal scroll bars which won't do anything
                if ((_scrollBars & ScrollBars.Horizontal) == ScrollBars.Horizontal
                    && _textAlign == HorizontalAlignment.Left
                    && !WordWrap)
                {
                    cp.Style |= (int)WINDOW_STYLE.WS_HSCROLL;
                }

                if ((_scrollBars & ScrollBars.Vertical) == ScrollBars.Vertical)
                {
                    cp.Style |= (int)WINDOW_STYLE.WS_VSCROLL;
                }
            }

            if (_useSystemPasswordChar)
            {
                cp.Style |= PInvoke.ES_PASSWORD;
            }

            return cp;
        }
    }

    /// <summary>
    ///  Gets or sets the character used to mask characters in a single-line text box
    ///  control used to enter passwords.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue((char)0)]
    [Localizable(true)]
    [SRDescription(nameof(SR.TextBoxPasswordCharDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public char PasswordChar
    {
        get
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            return (char)PInvoke.SendMessage(this, PInvoke.EM_GETPASSWORDCHAR);
        }
        set
        {
            _passwordChar = value;
            if (!_useSystemPasswordChar)
            {
                if (IsHandleCreated)
                {
                    if (PasswordChar != value)
                    {
                        // Set the password mode.
                        PInvoke.SendMessage(this, PInvoke.EM_SETPASSWORDCHAR, (WPARAM)value);

                        // Disable IME if setting the control to password mode.
                        VerifyImeRestrictedModeChanged();

                        ResetAutoComplete(false);
                        Invalidate();
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets which scroll bars should
    ///  appear in a multiline <see cref="TextBox"/>
    ///  control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue(ScrollBars.None)]
    [SRDescription(nameof(SR.TextBoxScrollBarsDescr))]
    public ScrollBars ScrollBars
    {
        get
        {
            return _scrollBars;
        }
        set
        {
            if (_scrollBars != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _scrollBars = value;
                RecreateHandle();
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        Size scrollBarPadding = Size.Empty;

        if (Multiline && !WordWrap && (ScrollBars & ScrollBars.Horizontal) != 0)
        {
            scrollBarPadding.Height += SystemInformation.GetHorizontalScrollBarHeightForDpi(_deviceDpi);
        }

        if (Multiline && (ScrollBars & ScrollBars.Vertical) != 0)
        {
            scrollBarPadding.Width += SystemInformation.GetVerticalScrollBarWidthForDpi(_deviceDpi);
        }

        // Subtract the scroll bar padding before measuring
        proposedConstraints -= scrollBarPadding;

        Size prefSize = base.GetPreferredSizeCore(proposedConstraints);

        return prefSize + scrollBarPadding;
    }

    /// <summary>
    ///  Gets or sets the current text in the text box.
    /// </summary>
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            _selectionSet = false;
        }
    }

    /// <summary>
    ///  Gets or sets how text is aligned in a <see cref="TextBox"/> control.
    ///  Note: This code is duplicated in MaskedTextBox for simplicity.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(HorizontalAlignment.Left)]
    [SRDescription(nameof(SR.TextBoxTextAlignDescr))]
    public HorizontalAlignment TextAlign
    {
        get
        {
            return _textAlign;
        }
        set
        {
            if (_textAlign != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _textAlign = value;
                RecreateHandle();
                OnTextAlignChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Indicates if the text in the edit control should appear as
    ///  the default password character. This property has precedence
    ///  over the PasswordChar property.  Whenever the UseSystemPasswordChar
    ///  is set to true, the default system password character is used,
    ///  any character set into PasswordChar is ignored.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TextBoxUseSystemPasswordCharDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public bool UseSystemPasswordChar
    {
        get
        {
            return _useSystemPasswordChar;
        }
        set
        {
            if (value != _useSystemPasswordChar)
            {
                _useSystemPasswordChar = value;

                // RecreateHandle will update IME restricted mode.
                RecreateHandle();

                if (value)
                {
                    ResetAutoComplete(false);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))]
    public event EventHandler? TextAlignChanged
    {
        add => Events.AddHandler(s_textAlignChangedEvent, value);
        remove => Events.RemoveHandler(s_textAlignChangedEvent, value);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Reset this just in case, because the SHAutoComplete stuff
            // will subclass this guys wndproc (and nativewindow can't know about it).
            // so this will undo it, but on a dispose we'll be Destroying the window anyway.

            ResetAutoComplete(true);
            if (_autoCompleteCustomSource is not null)
            {
                _autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
            }

            if (_stringSource is not null)
            {
                _stringSource.ReleaseAutoComplete();
                _stringSource = null;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Overridden to handle RETURN key.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if (Multiline && (keyData & Keys.Alt) == 0)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Return:
                    return _acceptsReturn;
            }
        }

        return base.IsInputKey(keyData);
    }

    private void OnAutoCompleteCustomSourceChanged(object? sender, CollectionChangeEventArgs e)
    {
        if (AutoCompleteSource == AutoCompleteSource.CustomSource)
        {
            SetAutoComplete(true);
        }
    }

    protected override unsafe void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);

        // Force repainting of the entire window frame.
        if (Application.RenderWithVisualStyles && IsHandleCreated && BorderStyle == BorderStyle.Fixed3D)
        {
            PInvoke.RedrawWindow(this, lprcUpdate: null, HRGN.Null, REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_FRAME);
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        if (AutoCompleteMode != AutoCompleteMode.None)
        {
            // Always recreate the handle when autocomplete mode is on
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Overridden to focus the text on first focus.
    /// </summary>
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        if (!_selectionSet)
        {
            // We get one shot at selecting when we first get focus.  If we don't
            // do it, we still want to act like the selection was set.
            _selectionSet = true;

            // If the user didn't provide a selection, force one in.
            if (SelectionLength == 0 && MouseButtons == MouseButtons.None)
            {
                SelectAll();
            }
        }

        if (IsAccessibilityObjectCreated)
        {
            AccessibilityObject.SetFocus();
        }
    }

    /// <summary>
    ///  Overridden to update the newly created handle with the settings of the
    ///  PasswordChar properties.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!IsHandleCreated)
        {
            return;
        }

        SetSelectionOnHandle();

        if (_passwordChar != 0)
        {
            if (!_useSystemPasswordChar)
            {
                PInvoke.SendMessage(this, PInvoke.EM_SETPASSWORDCHAR, (WPARAM)_passwordChar);
            }
        }

        VerifyImeRestrictedModeChanged();

        if (AutoCompleteMode != AutoCompleteMode.None)
        {
            try
            {
                _fromHandleCreate = true;
                SetAutoComplete(false);
            }
            finally
            {
                _fromHandleCreate = false;
            }
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (_stringSource is not null)
        {
            _stringSource.ReleaseAutoComplete();
            _stringSource = null;
        }

        base.OnHandleDestroyed(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (IsHandleCreated && IsAccessibilityObjectCreated && ContainsNavigationKeyCode(e.KeyCode))
        {
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (IsHandleCreated && IsAccessibilityObjectCreated)
        {
            // As there is no corresponding windows notification
            // about text selection changed for TextBox assuming
            // that any mouse down on textbox leads to change of
            // the caret position and thereby change the selection.
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
        }
    }

    protected virtual void OnTextAlignChanged(EventArgs e)
    {
        if (Events[s_textAlignChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Process a command key.
    ///  Native "EDIT" control does not support "Select All" shortcut represented by Ctrl-A keys, when in multiline mode,
    ///  WinForms TextBox supports this in .NET.
    /// </summary>
    /// <param name="m">The current windows message.</param>
    /// <param name="keyData">The bitmask containing one or more keys.</param>
    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        bool returnValue = base.ProcessCmdKey(ref m, keyData);
        if (!returnValue && ShortcutsEnabled && (keyData == (Keys.Control | Keys.A)))
        {
            SelectAll();
            return true;
        }

        return returnValue;
    }

    /// <summary>
    ///  Replaces the portion of the text specified by startPos and length with the one passed in,
    ///  without resetting the undo buffer (if any).
    ///  This method is provided as an alternative to SelectedText which clears the undo buffer.
    ///  Observe that this method does not honor the MaxLength property as the parameter-less base's
    ///  Paste does
    /// </summary>
    public void Paste(string? text)
    {
        base.SetSelectedTextInternal(text, false);
    }

    /// <summary>
    ///  Performs the actual select without doing arg checking.
    /// </summary>
    private protected override void SelectInternal(int start, int length, int textLen)
    {
        // If user set selection into text box, mark it so we don't
        // clobber it when we get focus.
        _selectionSet = true;
        base.SelectInternal(start, length, textLen);
    }

    /// <summary>
    ///  Sets the AutoComplete mode in TextBox.
    /// </summary>
    private unsafe void SetAutoComplete(bool reset)
    {
        // Autocomplete Not Enabled for Password enabled and MultiLine Textboxes.
        if (Multiline || _passwordChar != 0 || _useSystemPasswordChar || AutoCompleteSource == AutoCompleteSource.None)
        {
            return;
        }

        if (AutoCompleteMode != AutoCompleteMode.None)
        {
            if (!_fromHandleCreate)
            {
                // RecreateHandle to avoid Leak.
                // notice the use of member variable to avoid re-entrancy
                AutoCompleteMode backUpMode = AutoCompleteMode;
                _autoCompleteMode = AutoCompleteMode.None;
                RecreateHandle();
                _autoCompleteMode = backUpMode;
            }

            if (AutoCompleteSource == AutoCompleteSource.CustomSource)
            {
                if (IsHandleCreated && AutoCompleteCustomSource is not null)
                {
                    if (AutoCompleteCustomSource.Count == 0)
                    {
                        ResetAutoComplete(true);
                    }
                    else
                    {
                        if (_stringSource is null)
                        {
                            _stringSource = new StringSource(AutoCompleteCustomSource.ToArray());
                            if (!_stringSource.Bind(this, (AUTOCOMPLETEOPTIONS)AutoCompleteMode))
                            {
                                throw new ArgumentException(SR.AutoCompleteFailure);
                            }
                        }
                        else
                        {
                            _stringSource.RefreshList(AutoCompleteCustomSource.ToArray());
                        }
                    }
                }
            }
            else
            {
                if (IsHandleCreated)
                {
                    SHELL_AUTOCOMPLETE_FLAGS mode = SHELL_AUTOCOMPLETE_FLAGS.SHACF_DEFAULT;
                    if (AutoCompleteMode == AutoCompleteMode.Suggest)
                    {
                        mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_ON | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF;
                    }

                    if (AutoCompleteMode == AutoCompleteMode.Append)
                    {
                        mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_ON | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF;
                    }

                    if (AutoCompleteMode == AutoCompleteMode.SuggestAppend)
                    {
                        mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_ON;
                        mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_ON;
                    }

                    PInvoke.SHAutoComplete(this, (SHELL_AUTOCOMPLETE_FLAGS)AutoCompleteSource | mode);
                }
            }
        }
        else if (reset)
        {
            ResetAutoComplete(true);
        }
    }

    /// <summary>
    ///  Resets the AutoComplete mode in TextBox.
    /// </summary>
    private void ResetAutoComplete(bool force)
    {
        if ((AutoCompleteMode != AutoCompleteMode.None || force) && IsHandleCreated)
        {
            PInvoke.SHAutoComplete(this, (SHELL_AUTOCOMPLETE_FLAGS)AutoCompleteSource.AllSystemSources | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF);
        }
    }

    private void ResetAutoCompleteCustomSource()
    {
        AutoCompleteCustomSource = null;
    }

    private void WmPrint(ref Message m)
    {
        base.WndProc(ref m);
        if (((nint)m.LParamInternal & PInvoke.PRF_NONCLIENT) != 0 && Application.RenderWithVisualStyles
            && BorderStyle == BorderStyle.Fixed3D)
        {
            using Graphics g = Graphics.FromHdc((HDC)m.WParamInternal);
            Rectangle rect = new(0, 0, Size.Width - 1, Size.Height - 1);
            using var pen = VisualStyleInformation.TextControlBorder.GetCachedPenScope();
            g.DrawRectangle(pen, rect);
            rect.Inflate(-1, -1);
            g.DrawRectangle(SystemPens.Window, rect);
        }
    }

    /// <summary>
    ///  Gets or sets the text that is displayed when the control has no text and does not have the focus.
    /// </summary>
    /// <value>The text that is displayed when the control has no text and does not have the focus.</value>
    [Localizable(true)]
    [DefaultValue("")]
    [SRDescription(nameof(SR.TextBoxPlaceholderTextDescr))]
    [AllowNull]
    public virtual string PlaceholderText
    {
        get
        {
            return _placeholderText;
        }
        set
        {
            value ??= string.Empty;

            if (_placeholderText != value)
            {
                _placeholderText = value;
                if (IsHandleCreated)
                {
                    Invalidate();
                }
            }
        }
    }

    /// <summary>
    ///  Draws the <see cref="PlaceholderText"/> in the client area of the <see cref="TextBox"/> using the default font and color.
    /// </summary>
    private void DrawPlaceholderText(HDC hdc)
    {
        TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top | TextFormatFlags.EndEllipsis;
        Rectangle rectangle = ClientRectangle;

        if (RightToLeft == RightToLeft.Yes)
        {
            flags |= TextFormatFlags.RightToLeft;
            switch (TextAlign)
            {
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    rectangle.Offset(0, 1);
                    break;
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Right;
                    rectangle.Offset(1, 1);
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Left;
                    rectangle.Offset(0, 1);
                    break;
            }
        }
        else
        {
            flags &= ~TextFormatFlags.RightToLeft;
            switch (TextAlign)
            {
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    rectangle.Offset(0, 1);
                    break;
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Left;
                    rectangle.Offset(1, 1);
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    rectangle.Offset(0, 1);
                    break;
            }
        }

        TextRenderer.DrawTextInternal(hdc, PlaceholderText, Font, rectangle, Application.ApplicationColors.GrayText, TextRenderer.DefaultQuality, flags);
    }

    protected override unsafe void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            // Work around a very obscure Windows issue.
            case PInvoke.WM_LBUTTONDOWN:
                MouseButtons realState = MouseButtons;
                bool wasValidationCancelled = ValidationCancelled;

                Focus();

                if (realState == MouseButtons &&
                   (!ValidationCancelled || wasValidationCancelled))
                {
                    base.WndProc(ref m);
                }

                break;

            case PInvoke.WM_PAINT:
                WmPaintInternal(ref m);
                break;

            case PInvoke.WM_PRINT:
                WmPrint(ref m);
                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }

    /// <summary>
    ///  Handles the WM_PAINT message to render the placeholder text in the TextBox control.
    /// </summary>
    /// <param name="m">The message to handle.</param>
    private unsafe void WmPaintInternal(ref Message m)
    {
        // The native control tracks its own state, which can lead to two issues:
        //
        // 1. The native control invalidates itself and overwrites the placeholder text.
        // 2. The placeholder text is drawn multiple times without being cleared first.
        //
        // To avoid these issues, we need the following operations.
        //
        // NOTE: There is still observable flicker with this implementation. A second WM_PAINT is triggered
        // after our BeginPaint/EndPaint calls. Something seems to be invalidating the control again.
        // Explicitly calling ValidateRect should, in theory, prevent this second call, but it doesn't seem to work.

        // Invalidate the whole control to ensure the native control doesn't make any assumptions about what to paint.
        if (ShouldRenderPlaceHolderText())
        {
            PInvoke.InvalidateRect(this, lpRect: null, bErase: true);
        }

        // Let the native implementation draw the background and animate the frame.
        base.WndProc(ref m);

        if (ShouldRenderPlaceHolderText())
        {
            // Invalidate again because the native WM_PAINT has already validated everything by calling BeginPaint itself.
            PInvoke.InvalidateRect(this, lpRect: null, bErase: true);

            // Use BeginPaint instead of GetDC to prevent flicker and support print-to-image scenarios.
            using BeginPaintScope paintScope = new(HWND);
            DrawPlaceholderText(paintScope);

            // Validate the rectangle to prevent further invalidation and flicker.
            PInvoke.ValidateRect(this, lpRect: null);
        }
    }

    private bool ShouldRenderPlaceHolderText() =>
        !string.IsNullOrEmpty(PlaceholderText) &&
        !GetStyle(ControlStyles.UserPaint) &&
        !Focused &&
        TextLength == 0;
}
