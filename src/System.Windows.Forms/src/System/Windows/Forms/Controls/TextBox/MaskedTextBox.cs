// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.Globalization;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.Ime;

namespace System.Windows.Forms;

/// <summary>
///  MaskedTextBox control definition class.
///  Uses the services from the System.ComponentModel.MaskedTextBoxProvider class.
///  Search Microsoft SPO for "MaskEdit.doc" to see spec
/// </summary>
[DefaultEvent(nameof(MaskInputRejected))]
[DefaultBindingProperty(nameof(Text))]
[DefaultProperty(nameof(Mask))]
[Designer($"System.Windows.Forms.Design.MaskedTextBoxDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionMaskedTextBox))]
public partial class MaskedTextBox : TextBoxBase
{
    // Consider: The MaskedTextBox control, when initialized with a non-null/empty mask, processes all
    // WM_CHAR messages and always sets the text using the SetWindowText Windows function in the furthest base
    // class.  This means that the underlying Edit control won't enable Undo operations and the context
    // menu behavior will be a bit different (for instance Copy option is enabled when PasswordChar is set).
    // To provide Undo functionality and make the context menu behave like the Edit control, we would have
    // to implement our own.  For more info about how to do this, see:
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2000/november/c-q-a-filetype-icon-detector-app-custom-context-menus-unreferenced-variables-and-string-conversions

    private const bool Forward = true;
    private const bool Backward = false;
    private const string NullMask = "<>"; // any char/str is OK here.

    private static readonly object s_maskInputRejectedEvent = new();
    private static readonly object s_validationCompletedEvent = new();
    private static readonly object s_textAlignChangedEvent = new();
    private static readonly object s_isOverwriteModeChangedEvent = new();
    private static readonly object s_maskChangedEvent = new();

    // The native edit control's default password char (per thread). See corresponding property for more info.
    private static char s_systemPwdChar;

    // Values to track changes in IME composition string (if any).  Having const variables is a bit more efficient
    // than having an enum (which creates a class).
    private const byte ImeConversionNone = 0;       // no conversion has been performed in the composition string.
    private const byte ImeConversionUpdate = 1;     // the char being composed has been updated but not converted yet.
    private const byte ImeConversionCompleted = 2;  // the char being composed has been fully converted.

    /////////  Instance fields

    // Used for keeping selection when prompt is hidden on leave (text changes).
    private int _lastSelLength;

    // Used for caret positioning.
    private int _caretTestPos;

    // Bit mask - Determines when the Korean IME composition string is completed so converted character can be processed.
    private static readonly int s_imeEndingComposition = BitVector32.CreateMask();

    // Bit mask - Determines when the Korean IME is completing a composition, used when forcing conversion.
    private static readonly int s_imeCompleting = BitVector32.CreateMask(s_imeEndingComposition);

    // Used for handling characters that have a modifier (Ctrl-A, Shift-Del...).
    private static readonly int s_handleKeyPress = BitVector32.CreateMask(s_imeCompleting);

    // Bit mask - Used to simulate a null mask.  Needed since a MaskedTextProvider object cannot be
    // initialized with a null mask but we need one even in this case as a backend for
    // default properties.  This is to support creating a MaskedTextBox with the default
    // constructor, specially at design time.
    private static readonly int s_isNullMask = BitVector32.CreateMask(s_handleKeyPress);

    // Bit mask - Used in conjuction with get_Text to return the text that is actually set in the native
    // control.  This is required to be able to measure text correctly (GetPreferredSize) and
    // to compare against during set_Text (to bail if the same and not to raise TextChanged event).
    private static readonly int s_queryBaseText = BitVector32.CreateMask(s_isNullMask);

    // If true, the input text is rejected whenever a character does not comply with the mask; a MaskInputRejected
    // event is fired for the failing character.
    // If false, characters in the input string are processed one by one accepting the ones that comply
    // with the mask and raising the MaskInputRejected event for the rejected ones.
    private static readonly int s_rejectInputOnFirstFailure = BitVector32.CreateMask(s_queryBaseText);

    // Bit masks for boolean properties.
    private static readonly int s_hidePromptOnLeave = BitVector32.CreateMask(s_rejectInputOnFirstFailure);
    private static readonly int s_beepOnError = BitVector32.CreateMask(s_hidePromptOnLeave);
    private static readonly int s_useSystemPasswordChar = BitVector32.CreateMask(s_beepOnError);
    private static readonly int s_insertToggled = BitVector32.CreateMask(s_useSystemPasswordChar);
    private static readonly int s_cutCopyIncludePrompt = BitVector32.CreateMask(s_insertToggled);
    private static readonly int s_cutCopyIncludeLiterals = BitVector32.CreateMask(s_cutCopyIncludePrompt);

    /////////  Properties backend fields. See corresponding property comments for more info.

    private char _passwordChar; // control's pwd char, it could be different from the one displayed if using system password.
    private Type? _validatingType;
    private IFormatProvider? _formatProvider;
    private MaskedTextProvider _maskedTextProvider;
    private InsertKeyMode _insertMode;
    private HorizontalAlignment _textAlign;

    // Bit vector to represent bool variables.
    private BitVector32 _flagState;

    /// <summary>
    ///  Constructs the MaskedTextBox with the specified MaskedTextProvider object.
    /// </summary>
    public MaskedTextBox()
    {
        MaskedTextProvider maskedTextProvider = new(NullMask, CultureInfo.CurrentCulture);
        _flagState[s_isNullMask] = true;
        Initialize(maskedTextProvider);
    }

    /// <summary>
    ///  Constructs the MaskedTextBox with the specified MaskedTextProvider object.
    /// </summary>
    public MaskedTextBox(string mask)
    {
        ArgumentNullException.ThrowIfNull(mask);

        MaskedTextProvider maskedTextProvider = new(mask, CultureInfo.CurrentCulture);
        _flagState[s_isNullMask] = false;
        Initialize(maskedTextProvider);
    }

    /// <summary>
    ///  Constructs the MaskedTextBox with the specified MaskedTextProvider object.
    /// </summary>
    public MaskedTextBox(MaskedTextProvider maskedTextProvider)
    {
        ArgumentNullException.ThrowIfNull(maskedTextProvider);

        _flagState[s_isNullMask] = false;
        Initialize(maskedTextProvider);
    }

    /// <summary>
    ///  Initializes the object with the specified MaskedTextProvider object and default
    ///  property values.
    /// </summary>
    [MemberNotNull(nameof(_maskedTextProvider))]
    private void Initialize(MaskedTextProvider maskedTextProvider)
    {
        Debug.Assert(maskedTextProvider is not null, "Initializing from a null MaskProvider ref.");

        _maskedTextProvider = maskedTextProvider;

        // set the initial display text.
        if (!_flagState[s_isNullMask])
        {
            SetWindowText();
        }

        // set default values.
        _passwordChar = _maskedTextProvider.PasswordChar;
        _insertMode = InsertKeyMode.Default;

        _flagState[s_hidePromptOnLeave] = false;
        _flagState[s_beepOnError] = false;
        _flagState[s_useSystemPasswordChar] = false;
        _flagState[s_rejectInputOnFirstFailure] = false;

        // CutCopyMaskFormat - set same defaults as TextMaskFormat (IncludePromptAndLiterals).
        // It is a lot easier to handle this flags individually since that's the way the MaskedTextProvider does it.
        _flagState[s_cutCopyIncludePrompt] = _maskedTextProvider.IncludePrompt;
        _flagState[s_cutCopyIncludeLiterals] = _maskedTextProvider.IncludeLiterals;

        // fields for internal use.
        _flagState[s_handleKeyPress] = true;
        _caretTestPos = 0;
    }

    ///////////////////  Properties
    ///
    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool AcceptsTab
    {
        get { return false; }
        set { }
    }

    /// <summary>
    ///  Specifies whether the prompt character should be treated as a valid input character or not.
    ///  The setter resets the underlying MaskedTextProvider object and attempts
    ///  to add the existing input text (if any) using the new mask, failure is ignored.
    ///  This property has no particular effect if no mask has been set.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxAllowPromptAsInputDescr))]
    [DefaultValue(true)]
    public bool AllowPromptAsInput
    {
        get
        {
            return _maskedTextProvider.AllowPromptAsInput;
        }
        set
        {
            if (value != _maskedTextProvider.AllowPromptAsInput)
            {
                // Recreate masked text provider since this property is read-only.
                MaskedTextProvider newProvider = new(
                    _maskedTextProvider.Mask,
                    _maskedTextProvider.Culture,
                    value,
                    _maskedTextProvider.PromptChar,
                    _maskedTextProvider.PasswordChar,
                    _maskedTextProvider.AsciiOnly);

                SetMaskedTextProvider(newProvider);
            }
        }
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? AcceptsTabChanged
    {
        add { }
        remove { }
    }

    /// <summary>
    ///  Specifies whether only ASCII characters are accepted as valid input.
    ///  This property has no particular effect if no mask has been set.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxAsciiOnlyDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(false)]
    public bool AsciiOnly
    {
        get
        {
            return _maskedTextProvider.AsciiOnly;
        }

        set
        {
            if (value != _maskedTextProvider.AsciiOnly)
            {
                // Recreate masked text provider since this property is read-only.
                MaskedTextProvider newProvider = new(
                    _maskedTextProvider.Mask,
                    _maskedTextProvider.Culture,
                    _maskedTextProvider.AllowPromptAsInput,
                    _maskedTextProvider.PromptChar,
                    _maskedTextProvider.PasswordChar,
                    value);

                SetMaskedTextProvider(newProvider);
            }
        }
    }

    /// <summary>
    ///  Specifies whether to play a beep when the input is not valid according to the mask.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxBeepOnErrorDescr))]
    [DefaultValue(false)]
    public bool BeepOnError
    {
        get
        {
            return _flagState[s_beepOnError];
        }
        set
        {
            _flagState[s_beepOnError] = value;
        }
    }

    /// <summary>
    ///  Gets a value indicating whether the user can undo the previous operation in a text box control.
    ///  Unsupported method/property.
    ///  WndProc ignores EM_CANUNDO.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool CanUndo
    {
        get
        {
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

            // Translate for Rtl if necessary
            HorizontalAlignment align = RtlTranslateHorizontal(_textAlign);
            cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_RIGHT;   // WS_EX_RIGHT overrides the ES_XXXX alignment styles
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

            return cp;
        }
    }

    /// <summary>
    ///  The culture that determines the value of the localizable mask language separators and placeholders.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxCultureDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public CultureInfo Culture
    {
        get
        {
            return _maskedTextProvider.Culture;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!_maskedTextProvider.Culture.Equals(value))
            {
                // Recreate masked text provider since this property is read-only.
                MaskedTextProvider newProvider = new(
                    _maskedTextProvider.Mask,
                    value,
                    _maskedTextProvider.AllowPromptAsInput,
                    _maskedTextProvider.PromptChar,
                    _maskedTextProvider.PasswordChar,
                    _maskedTextProvider.AsciiOnly);

                SetMaskedTextProvider(newProvider);
            }
        }
    }

    /// <summary>
    ///  Specifies the formatting options for text cut/copied to the clipboard (Whether the mask returned from the Text
    ///  property includes Literals and/or prompt characters).
    ///  When prompt characters are excluded, they are returned as spaces in the string returned.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxCutCopyMaskFormat))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(MaskFormat.IncludeLiterals)]
    public MaskFormat CutCopyMaskFormat
    {
        get
        {
            if (_flagState[s_cutCopyIncludePrompt])
            {
                if (_flagState[s_cutCopyIncludeLiterals])
                {
                    return MaskFormat.IncludePromptAndLiterals;
                }

                return MaskFormat.IncludePrompt;
            }

            if (_flagState[s_cutCopyIncludeLiterals])
            {
                return MaskFormat.IncludeLiterals;
            }

            return MaskFormat.ExcludePromptAndLiterals;
        }

        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);

            if (value == MaskFormat.IncludePrompt)
            {
                _flagState[s_cutCopyIncludePrompt] = true;
                _flagState[s_cutCopyIncludeLiterals] = false;
            }
            else if (value == MaskFormat.IncludeLiterals)
            {
                _flagState[s_cutCopyIncludePrompt] = false;
                _flagState[s_cutCopyIncludeLiterals] = true;
            }
            else // value == MaskFormat.IncludePromptAndLiterals || value == MaskFormat.ExcludePromptAndLiterals
            {
                bool include = value == MaskFormat.IncludePromptAndLiterals;
                _flagState[s_cutCopyIncludePrompt] = include;
                _flagState[s_cutCopyIncludeLiterals] = include;
            }
        }
    }

    /// <summary>
    ///  Specifies the IFormatProvider to be used when parsing the string to the ValidatingType.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IFormatProvider? FormatProvider
    {
        get
        {
            return _formatProvider;
        }

        set
        {
            _formatProvider = value;
        }
    }

    /// <summary>
    ///  Specifies whether the PromptCharacter is displayed when the control loses focus.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxHidePromptOnLeaveDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(false)]
    public bool HidePromptOnLeave
    {
        get
        {
            return _flagState[s_hidePromptOnLeave];
        }
        set
        {
            if (_flagState[s_hidePromptOnLeave] != value)
            {
                _flagState[s_hidePromptOnLeave] = value;

                // If the control is not focused and there are available edit positions (mask not full) we need to
                // update the displayed text.
                if (!_flagState[s_isNullMask] && !Focused && !MaskFull && !DesignMode)
                {
                    SetWindowText();
                }
            }
        }
    }

    /// <summary>
    ///  Specifies whether to include mask literal characters when formatting the text.
    /// </summary>
    private bool IncludeLiterals
    {
        get
        {
            return _maskedTextProvider.IncludeLiterals;
        }
        set
        {
            _maskedTextProvider.IncludeLiterals = value;
        }
    }

    /// <summary>
    ///  Specifies whether to include the mask prompt character when formatting the text in places
    ///  where an edit char has not being assigned.
    /// </summary>
    private bool IncludePrompt
    {
        get
        {
            return _maskedTextProvider.IncludePrompt;
        }
        set
        {
            _maskedTextProvider.IncludePrompt = value;
        }
    }

    /// <summary>
    ///  Specifies the text insertion mode of the text box.  This can be used to simulated the Access masked text
    ///  control behavior where insertion is set to TextInsertionMode.AlwaysOverwrite
    ///  This property has no particular effect if no mask has been set.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxInsertKeyModeDescr))]
    [DefaultValue(InsertKeyMode.Default)]
    public InsertKeyMode InsertKeyMode
    {
        get
        {
            return _insertMode;
        }
        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            if (_insertMode != value)
            {
                bool isOverwrite = IsOverwriteMode;
                _insertMode = value;

                if (isOverwrite != IsOverwriteMode)
                {
                    OnIsOverwriteModeChanged(EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///  Overridden to handle unsupported RETURN key.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.KeyCode) == Keys.Return)
        {
            return false;
        }

        return base.IsInputKey(keyData);
    }

    /// <summary>
    ///  Specifies whether text insertion mode in 'on' or not.
    /// </summary>
    [Browsable(false)]
    public bool IsOverwriteMode
    {
        get
        {
            if (_flagState[s_isNullMask])
            {
                return false; // EditBox always inserts.
            }

            switch (_insertMode)
            {
                case InsertKeyMode.Overwrite:
                    return true;

                case InsertKeyMode.Insert:
                    return false;

                case InsertKeyMode.Default:

                    // Note that the insert key state should be per process and its initial state insert, this is the
                    // behavior of apps like WinWord, WordPad and VS; so we have to keep track of it and not query its
                    // system value.
                    // return Control.IsKeyLocked(Keys.Insert);
                    return _flagState[s_insertToggled];

                default:
                    Debug.Fail("Invalid InsertKeyMode.  This code path should have never been executed.");
                    return false;
            }
        }
    }

    /// <summary>
    ///  Event to notify when the insert mode has changed.  This is required for data binding.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.MaskedTextBoxIsOverwriteModeChangedDescr))]
    public event EventHandler? IsOverwriteModeChanged
    {
        add => Events.AddHandler(s_isOverwriteModeChangedEvent, value);
        remove => Events.RemoveHandler(s_isOverwriteModeChangedEvent, value);
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public new string[] Lines
    {
        get
        {
            string[] lines;

            _flagState[s_queryBaseText] = true;
            try
            {
                lines = base.Lines;
            }
            finally
            {
                _flagState[s_queryBaseText] = false;
            }

            return lines;
        }
        set { }
    }

    /// <summary>
    ///  The mask applied to this control.  The setter resets the underlying MaskedTextProvider object and attempts
    ///  to add the existing input text (if any) using the new mask, failure is ignored.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxMaskDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue("")]
    [MergableProperty(false)]
    [Localizable(true)]
    [AllowNull]
    [Editor($"System.Windows.Forms.Design.MaskPropertyEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    public string Mask
    {
        get
        {
            return _flagState[s_isNullMask] ? string.Empty : _maskedTextProvider.Mask;
        }
        set
        {
            //
            // We don't do anything if:
            // 1.  IsNullOrEmpty( value )->[Reset control] && _flagState[IS_NULL_MASK]==>Already Reset.
            // 2. !IsNullOrEmpty( value )->[Set control] && !_flagState[IS_NULL_MASK][control is set] && [value is the same]==>No need to update.
            //
            if (_flagState[s_isNullMask] == string.IsNullOrEmpty(value) && (_flagState[s_isNullMask] || value == _maskedTextProvider.Mask))
            {
                return;
            }

            string? text = null;
            string? newMask = value;

            // We need to update the _flagState[IS_NULL_MASK]field before raising any events (when setting the maskedTextProvider) so
            // querying for properties from an event handler returns the right value (i.e: Text).

            if (string.IsNullOrEmpty(value)) // Resetting the control, the native edit control will be in charge.
            {
                // Need to get the formatted & unformatted text before resetting the mask, they'll be used to determine whether we need to
                // raise the TextChanged event.
                string formattedText = TextOutput;
                string unformattedText = _maskedTextProvider.ToString(false, false);

                _flagState[s_isNullMask] = true;

                if (_maskedTextProvider.IsPassword)
                {
                    SetEditControlPasswordChar(_maskedTextProvider.PasswordChar);
                }

                // Set the window text to the unformatted text before raising events. Also, TextChanged needs to be raised after MaskChanged so
                // pass false to SetWindowText 'raiseTextChanged' param.
                SetWindowText(unformattedText, false, false);

                EventArgs e = EventArgs.Empty;

                OnMaskChanged(e);

                if (unformattedText != formattedText)
                {
                    OnTextChanged(e);
                }

                newMask = NullMask;
            }
            else    // Setting control to a new value.
            {
                foreach (char c in value)
                {
                    if (!MaskedTextProvider.IsValidMaskChar(c))
                    {
                        // Same message as in SR.MaskedTextProviderMaskInvalidChar in System.txt
                        throw new ArgumentException(SR.MaskedTextBoxMaskInvalidChar);
                    }
                }

                if (_flagState[s_isNullMask])
                {
                    // If this.IsNullMask, we are setting the mask to a new value; in this case we need to get the text because
                    // the underlying MTP does not have it (used as a property backend only) and pass it to SetMaskedTextProvider
                    // method below to update the provider.

                    text = Text;
                }
            }

            // Recreate masked text provider since this property is read-only.
            MaskedTextProvider newProvider = new(
                newMask!,
                _maskedTextProvider.Culture,
                _maskedTextProvider.AllowPromptAsInput,
                _maskedTextProvider.PromptChar,
                _maskedTextProvider.PasswordChar,
                _maskedTextProvider.AsciiOnly);

            // text is null when setting to a different mask value or when resetting the mask to null.
            // text is not null only when setting the mask from null to some value.
            SetMaskedTextProvider(newProvider, text);
        }
    }

    /// <summary>
    ///  Event to notify when the mask has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.MaskedTextBoxMaskChangedDescr))]
    public event EventHandler? MaskChanged
    {
        add => Events.AddHandler(s_maskChangedEvent, value);
        remove => Events.RemoveHandler(s_maskChangedEvent, value);
    }

    /// <summary>
    ///  Specifies whether the test string required input positions, as specified by the mask, have
    ///  all been assigned.
    /// </summary>
    [Browsable(false)]
    public bool MaskCompleted
    {
        get
        {
            return _maskedTextProvider.MaskCompleted;
        }
    }

    /// <summary>
    ///  Specifies whether all inputs (required and optional) have been provided into the mask successfully.
    /// </summary>
    [Browsable(false)]
    public bool MaskFull
    {
        get
        {
            return _maskedTextProvider.MaskFull;
        }
    }

    /// <summary>
    ///  Returns a copy of the control's internal MaskedTextProvider.  This is useful for user's to provide
    ///  cloning semantics for the control (we don't want to do it) w/o incurring in any perf penalty since
    ///  some of the properties require recreating the underlying provider when they are changed.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MaskedTextProvider? MaskedTextProvider
    {
        get
        {
            return _flagState[s_isNullMask] ? null : (MaskedTextProvider)_maskedTextProvider.Clone();
        }
    }

    /// <summary>
    ///  Event to notify when an input has been rejected according to the mask.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxMaskInputRejectedDescr))]
    public event MaskInputRejectedEventHandler? MaskInputRejected
    {
        add => Events.AddHandler(s_maskInputRejectedEvent, value);
        remove => Events.RemoveHandler(s_maskInputRejectedEvent, value);
    }

    /// <summary>
    ///  Unsupported method/property.
    ///  WndProc ignores EM_LIMITTEXT &amp; this is a virtual method.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override int MaxLength
    {
        get => base.MaxLength;
        set { }
    }

    /// <summary>
    ///  Unsupported method/property.
    ///  virtual method.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool Multiline
    {
        get { return false; }
        set { }
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? MultilineChanged
    {
        add { }
        remove { }
    }

    /// <summary>
    ///  Specifies the character to be used in the formatted string in place of editable characters, if
    ///  set to any printable character, the text box becomes a password text box, to reset it use the null
    ///  character.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxPasswordCharDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue('\0')]
    public char PasswordChar
    {
        get
        {
            // The password char could be the one set in the control or the system password char,
            // in any case the maskedTextProvider has the correct one.
            return _maskedTextProvider.PasswordChar;
        }
        set
        {
            if (!MaskedTextProvider.IsValidPasswordChar(value)) // null character accepted (resets value)
            {
                // Same message as in SR.MaskedTextProviderInvalidCharError.
                throw new ArgumentException(SR.MaskedTextBoxInvalidCharError);
            }

            if (_passwordChar != value)
            {
                if (value == _maskedTextProvider.PromptChar)
                {
                    // Prompt and password chars must be different.
                    throw new InvalidOperationException(SR.MaskedTextBoxPasswordAndPromptCharError);
                }

                _passwordChar = value;

                // UseSystemPasswordChar take precedence over PasswordChar...Let's check.
                if (!UseSystemPasswordChar)
                {
                    _maskedTextProvider.PasswordChar = value;

                    if (_flagState[s_isNullMask])
                    {
                        SetEditControlPasswordChar(value);
                    }
                    else
                    {
                        SetWindowText();
                    }

                    VerifyImeRestrictedModeChanged();
                }
            }
        }
    }

    /// <summary>
    ///  Determines if the control is in password protect mode.
    /// </summary>
    private protected override bool PasswordProtect
        => _maskedTextProvider.IsPassword;

    /// <summary>
    ///  Specifies the prompt character to be used in the formatted string for unsupplied characters.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.MaskedTextBoxPromptCharDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Localizable(true)]
    [DefaultValue('_')]
    public char PromptChar
    {
        get
        {
            return _maskedTextProvider.PromptChar;
        }
        set
        {
            if (!MaskedTextProvider.IsValidInputChar(value))
            {
                // This message is the same as the one in SR.MaskedTextProviderInvalidCharError.
                throw new ArgumentException(SR.MaskedTextBoxInvalidCharError);
            }

            if (_maskedTextProvider.PromptChar != value)
            {
                // We need to check maskedTextProvider password char in case it is using the system password.
                if (value == _passwordChar || value == _maskedTextProvider.PasswordChar)
                {
                    // Prompt and password chars must be different.
                    throw new InvalidOperationException(SR.MaskedTextBoxPasswordAndPromptCharError);
                }

                // Recreate masked text provider to be consistent with AllowPromptAsInput - current text may have chars with same value as new prompt.
                MaskedTextProvider newProvider = new(
                    _maskedTextProvider.Mask,
                    _maskedTextProvider.Culture,
                    _maskedTextProvider.AllowPromptAsInput,
                    value,
                    _maskedTextProvider.PasswordChar,
                    _maskedTextProvider.AsciiOnly);

                SetMaskedTextProvider(newProvider);
            }
        }
    }

    /// <summary>
    ///  Overwrite base class' property.
    /// </summary>
    public new bool ReadOnly
    {
        get => base.ReadOnly;

        set
        {
            if (ReadOnly != value)
            {
                // if true, this disables IME in the base class.
                base.ReadOnly = value;

                if (!_flagState[s_isNullMask])
                {
                    // Prompt will be hidden.
                    SetWindowText();
                }
            }
        }
    }

    /// <summary>
    ///  Specifies whether to include the mask prompt character when formatting the text in places
    ///  where an edit char has not being assigned.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxRejectInputOnFirstFailureDescr))]
    [DefaultValue(false)]
    public bool RejectInputOnFirstFailure
    {
        get
        {
            return _flagState[s_rejectInputOnFirstFailure];
        }
        set
        {
            _flagState[s_rejectInputOnFirstFailure] = value;
        }
    }

    /// <summary>
    ///  Specifies whether to reset and skip the current position if editable, when the input character
    ///  has the same value as the prompt.  This property takes precedence over AllowPromptAsInput.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxResetOnPrompt))]
    [DefaultValue(true)]
    public bool ResetOnPrompt
    {
        get
        {
            return _maskedTextProvider.ResetOnPrompt;
        }
        set
        {
            _maskedTextProvider.ResetOnPrompt = value;
        }
    }

    /// <summary>
    ///  Specifies whether to reset and skip the current position if editable, when the input
    ///  is the space character.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxResetOnSpace))]
    [DefaultValue(true)]
    public bool ResetOnSpace
    {
        get
        {
            return _maskedTextProvider.ResetOnSpace;
        }
        set
        {
            _maskedTextProvider.ResetOnSpace = value;
        }
    }

    /// <summary>
    ///  Specifies whether to skip the current position if non-editable and the input character has
    ///  the same value as the literal at that position.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxSkipLiterals))]
    [DefaultValue(true)]
    public bool SkipLiterals
    {
        get
        {
            return _maskedTextProvider.SkipLiterals;
        }
        set
        {
            _maskedTextProvider.SkipLiterals = value;
        }
    }

    /// <summary>
    ///  The currently selected text (if any) in the control.
    /// </summary>
    [AllowNull]
    public override string SelectedText
    {
        get
        {
            if (_flagState[s_isNullMask])
            {
                return base.SelectedText;
            }

            return GetSelectedText();
        }
        set
        {
            SetSelectedTextInternal(value, true);
        }
    }

    internal override void SetSelectedTextInternal(string? value, bool clearUndo)
    {
        if (_flagState[s_isNullMask])
        {
            base.SetSelectedTextInternal(value, true); // Operates as a regular text box base.
            return;
        }

        PasteInt(value);
    }

    /// <summary>
    ///  Set the composition string as the result string.
    /// </summary>
    private void ImeComplete()
    {
        _flagState[s_imeCompleting] = true;
        ImeNotify(NOTIFY_IME_INDEX.CPS_COMPLETE);
    }

    /// <summary>
    ///  Notifies the IMM about changes to the status of the IME input context.
    /// </summary>
    private void ImeNotify(NOTIFY_IME_INDEX action)
    {
        HIMC inputContext = PInvoke.ImmGetContext(this);

        if (inputContext != IntPtr.Zero)
        {
            try
            {
                PInvoke.ImmNotifyIME(inputContext, NOTIFY_IME_ACTION.NI_COMPOSITIONSTR, action, 0);
            }
            finally
            {
                PInvoke.ImmReleaseContext(this, inputContext);
            }
        }
        else
        {
            Debug.Fail("Could not get IME input context.");
        }
    }

    /// <summary>
    ///  Sets the underlying edit control's password char to the one obtained from this.PasswordChar.
    ///  This is used when the control is passworded and this.flagState[IS_NULL_MASK].
    /// </summary>
    private void SetEditControlPasswordChar(char pwdChar)
    {
        if (IsHandleCreated)
        {
            // This message does not return a value.
            PInvoke.SendMessage(this, PInvoke.EM_SETPASSWORDCHAR, (WPARAM)pwdChar);
            Invalidate();
        }
    }

    /// <summary>
    ///  The value of the Edit control default password char.
    /// </summary>
    private static char SystemPasswordChar
    {
        get
        {
            if (s_systemPwdChar == '\0')
            {
                // We need to temporarily create an edit control to get the default password character.
                // We cannot use this control because we would have to reset the native control's password char to use
                // the default one so we can get it; this would change the text displayed in the box (even for a short time)
                // opening a sec hole.

                using TextBox txtBox = new();
                txtBox.UseSystemPasswordChar = true; // this forces the creation of the control handle.

                s_systemPwdChar = txtBox.PasswordChar;
            }

            return s_systemPwdChar;
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  The Text setter validates the input char by char, raising the MaskInputRejected event for invalid chars.
    ///  The Text getter returns the formatted text according to the IncludeLiterals and IncludePrompt properties.
    /// </summary>
    [Editor($"System.Windows.Forms.Design.MaskedTextBoxTextEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRCategory(nameof(SR.CatAppearance))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Bindable(true)]
    [DefaultValue("")]
    [AllowNull]
    [Localizable(true)]
    public override string Text
    {
        get
        {
            if (_flagState[s_isNullMask] || _flagState[s_queryBaseText])
            {
                return base.Text;
            }

            return TextOutput;
        }
        set
        {
            if (_flagState[s_isNullMask])
            {
                base.Text = value;
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                // reset the input text.
                Delete(Keys.Delete, 0, _maskedTextProvider.Length);
            }
            else
            {
                if (RejectInputOnFirstFailure)
                {
                    string oldText = TextOutput;
                    if (_maskedTextProvider.Set(value, out _caretTestPos, out MaskedTextResultHint hint))
                    {
                        if (TextOutput != oldText)
                        {
                            SetText();
                        }

                        SelectionStart = ++_caretTestPos;
                    }
                    else
                    {
                        OnMaskInputRejected(new MaskInputRejectedEventArgs(_caretTestPos, hint));
                    }
                }
                else
                {
                    Replace(value, /*startPosition*/ 0, /*selectionLen*/ _maskedTextProvider.Length);
                }
            }
        }
    }

    /// <summary>
    ///  Returns the length of the displayed text.
    /// </summary>
    [Browsable(false)]
    public override int TextLength
    {
        get
        {
            if (_flagState[s_isNullMask])
            {
                return base.TextLength;
            }

            // On older platforms TextBoxBase.TextLength calls Text.Length directly and
            // does not query the window for the actual text length.
            // If TextMaskFormat is set to a anything different from IncludePromptAndLiterals
            // or HidePromptOnLeave is true the return value may be incorrect because the
            // Text property value and the display text may be different.
            // We need to handle this here.
            return GetFormattedDisplayString().Length;
        }
    }

    /// <summary>
    ///  The formatted text, it is what the Text getter returns when a mask has been applied to the control.
    ///  The text format follows the IncludeLiterals and IncludePrompt properties (See MaskedTextProvider.ToString()).
    /// </summary>
    private string TextOutput
    {
        get
        {
            Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");
            return _maskedTextProvider.ToString();
        }
    }

    /// <summary>
    ///  Gets or sets how text is aligned in the control.
    ///  Note: This code is duplicated in TextBox for simplicity.
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
                // verify that 'value' is a valid enum type...
                // valid values are 0x0 to 0x2
                SourceGenerated.EnumValidator.Validate(value);

                _textAlign = value;
                RecreateHandle();
                OnTextAlignChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Event to notify the text alignment has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))]
    public event EventHandler? TextAlignChanged
    {
        add => Events.AddHandler(s_textAlignChangedEvent, value);

        remove => Events.RemoveHandler(s_textAlignChangedEvent, value);
    }

    /// <summary>
    ///  Specifies the formatting options for text output (Whether the mask returned from the Text
    ///  property includes Literals and/or prompt characters).
    ///  When prompt characters are excluded, they're returned as spaces in the string returned.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxTextMaskFormat))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(MaskFormat.IncludeLiterals)]
    public MaskFormat TextMaskFormat
    {
        get
        {
            if (IncludePrompt)
            {
                if (IncludeLiterals)
                {
                    return MaskFormat.IncludePromptAndLiterals;
                }

                return MaskFormat.IncludePrompt;
            }

            if (IncludeLiterals)
            {
                return MaskFormat.IncludeLiterals;
            }

            return MaskFormat.ExcludePromptAndLiterals;
        }

        set
        {
            if (TextMaskFormat == value)
            {
                return;
            }

            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);

            // Changing the TextMaskFormat will likely change the 'output' text (Text getter value).  Cache old value to
            // verify it against the new value and raise OnTextChange if needed.
            string? oldText = _flagState[s_isNullMask] ? null : TextOutput;

            if (value == MaskFormat.IncludePrompt)
            {
                IncludePrompt = true;
                IncludeLiterals = false;
            }
            else if (value == MaskFormat.IncludeLiterals)
            {
                IncludePrompt = false;
                IncludeLiterals = true;
            }
            else // value == MaskFormat.IncludePromptAndLiterals || value == MaskFormat.ExcludePromptAndLiterals
            {
                bool include = value == MaskFormat.IncludePromptAndLiterals;
                IncludePrompt = include;
                IncludeLiterals = include;
            }

            if (oldText is not null && oldText != TextOutput)
            {
                OnTextChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Provides some interesting information for the TextBox control in String form.
    ///  Returns the test string (no password, including literals and prompt).
    /// </summary>
    public override string ToString()
    {
        if (_flagState[s_isNullMask])
        {
            return base.ToString();
        }

        // base.ToString will call Text, we want to always display prompt and literals.
        bool includePrompt = IncludePrompt;
        bool includeLits = IncludeLiterals;
        string str;
        try
        {
            IncludePrompt = IncludeLiterals = true;
            str = base.ToString();
        }
        finally
        {
            IncludePrompt = includePrompt;
            IncludeLiterals = includeLits;
        }

        return str;
    }

    /// <summary>
    ///  Event to notify when the validating object completes parsing the formatted text.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.MaskedTextBoxTypeValidationCompletedDescr))]
    public event TypeValidationEventHandler? TypeValidationCompleted
    {
        add => Events.AddHandler(s_validationCompletedEvent, value);
        remove => Events.RemoveHandler(s_validationCompletedEvent, value);
    }

    /// <summary>
    ///  Indicates if the text in the edit control should appear as the default password character.
    ///  This property has precedence over the PasswordChar property.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MaskedTextBoxUseSystemPasswordCharDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(false)]
    public bool UseSystemPasswordChar
    {
        get
        {
            return _flagState[s_useSystemPasswordChar];
        }
        set
        {
            if (value != _flagState[s_useSystemPasswordChar])
            {
                if (value)
                {
                    if (SystemPasswordChar == PromptChar)
                    {
                        // Prompt and password chars must be different.
                        throw new InvalidOperationException(SR.MaskedTextBoxPasswordAndPromptCharError);
                    }

                    _maskedTextProvider.PasswordChar = SystemPasswordChar;
                }
                else
                {
                    // _passwordChar could be '\0', in which case we are resetting the display to show the input char.
                    _maskedTextProvider.PasswordChar = _passwordChar;
                }

                _flagState[s_useSystemPasswordChar] = value;

                if (_flagState[s_isNullMask])
                {
                    SetEditControlPasswordChar(_maskedTextProvider.PasswordChar);
                }
                else
                {
                    SetWindowText();
                }

                VerifyImeRestrictedModeChanged();
            }
        }
    }

    /// <summary>
    ///  Type of the object to be used to parse the text when the user leaves the control.
    ///  A ValidatingType object must implement a method with one fo the following signature:
    ///  public static Object Parse(string)
    ///  public static Object Parse(string, IFormatProvider)
    ///  See DateTime.Parse(...) for an example.
    /// </summary>
    [Browsable(false)]
    [DefaultValue(null)]
    public Type? ValidatingType
    {
        get
        {
            return _validatingType;
        }
        set
        {
            if (_validatingType != value)
            {
                _validatingType = value;
            }
        }
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool WordWrap
    {
        get { return false; }
        set { }
    }

    //////////////  Methods

    /// <summary>
    ///  Clears information about the most recent operation from the undo buffer of the control.
    ///  Unsupported property/method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void ClearUndo()
    {
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new MaskedTextBoxAccessibleObject(this);

    /// <summary>
    ///  Creates a handle for this control. This method is called by the framework, this should
    ///  not be called directly. Inheriting classes should always call <c>base.CreateHandle</c> when overriding this method.
    ///  Overridden to be able to set the control text with the masked (passworded) value when recreating
    ///  handle, since the underlying native edit control is not aware of it.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void CreateHandle()
    {
        if (!_flagState[s_isNullMask] && RecreatingHandle)
        {
            // update cached text value in Control. Don't preserve caret, cannot query for selection start at this time.
            SetWindowText(GetFormattedDisplayString(), false, false);
        }

        base.CreateHandle();
    }

    /// <summary>
    ///  Deletes characters from the control's text according to the key pressed (Delete/Backspace).
    ///  Returns true if something gets actually deleted, false otherwise.
    /// </summary>
    private void Delete(Keys keyCode, int startPosition, int selectionLen)
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");
        Debug.Assert(keyCode is Keys.Delete or Keys.Back, $"Delete called with keyCode == {keyCode}");
        Debug.Assert(startPosition >= 0 && ((startPosition + selectionLen) <= _maskedTextProvider.Length), "Invalid position range.");

        // On backspace, moving the start postion back by one has the same effect as delete.  If text is selected, there is no
        // need for moving the position back.

        _caretTestPos = startPosition;

        if (selectionLen == 0)
        {
            if (keyCode == Keys.Back)
            {
                if (startPosition == 0) // At beginning of string, backspace does nothing.
                {
                    return;
                }

                startPosition--; // so it can be treated as delete.
            }
            else // (keyCode == Keys.Delete)
            {
                if ((startPosition + selectionLen) == _maskedTextProvider.Length) // At end of string, delete does nothing.
                {
                    return;
                }
            }
        }

        int endPos = selectionLen > 0 ? startPosition + selectionLen - 1 : startPosition;

        string oldText = TextOutput;
        if (_maskedTextProvider.RemoveAt(startPosition, endPos, out int tempPos, out MaskedTextResultHint hint))
        {
            if (TextOutput != oldText)
            {
                SetText();
                _caretTestPos = startPosition;
            }
            else
            {
                // If succeeded but nothing removed, the caret should move as follows:
                // 1. If selectionLen > 0, or on back and hint == SideEffect: move to selectionStart.
                // 2. If hint == NoEffect, On Delete move to next edit position, if any or not already in one.
                //    On back move to the next edit postion at the left if no more assigned position at the right,
                //    in such case find an assigned position and move one past or one position left if no assigned pos found
                //    (taken care by 'startPosition--' above).
                // 3. If hint == SideEffect, on Back move like arrow key, (startPosition is already moved, startPosition-- above).

                if (selectionLen > 0)
                {
                    _caretTestPos = startPosition;
                }
                else
                {
                    if (hint == MaskedTextResultHint.NoEffect) // Case 2.
                    {
                        if (keyCode == Keys.Delete)
                        {
                            _caretTestPos = _maskedTextProvider.FindEditPositionFrom(startPosition, Forward);
                        }
                        else
                        {
                            if (_maskedTextProvider.FindAssignedEditPositionFrom(startPosition, Forward) == MaskedTextProvider.InvalidIndex)
                            {
                                // No assigned position at the right, nothing to shift then move to the next assigned position at the
                                // left (if any).
                                _caretTestPos = _maskedTextProvider.FindAssignedEditPositionFrom(startPosition, Backward);
                            }
                            else
                            {
                                // there are assigned positions at the right so move to an edit position at the left to get ready for
                                // removing the character on it or just shifting the characters at the right
                                _caretTestPos = _maskedTextProvider.FindEditPositionFrom(startPosition, Backward);
                            }

                            if (_caretTestPos != MaskedTextProvider.InvalidIndex)
                            {
                                _caretTestPos++; // backspace gets ready to remove one position past the edit position.
                            }
                        }

                        if (_caretTestPos == MaskedTextProvider.InvalidIndex)
                        {
                            _caretTestPos = startPosition;
                        }
                    }
                    else // (hint == MaskedTextProvider.OperationHint.SideEffect)
                    {
                        if (keyCode == Keys.Back)  // Case 3.
                        {
                            _caretTestPos = startPosition;
                        }
                    }
                }
            }
        }
        else
        {
            OnMaskInputRejected(new MaskInputRejectedEventArgs(tempPos, hint));
        }

        // Reposition caret.  Call base.SelectInternal for perf reasons.
        // this.SelectionLength = 0;
        // this.SelectionStart  = _caretTestPos; // new caret position.
        base.SelectInternal(_caretTestPos, 0, _maskedTextProvider.Length);

        return;
    }

    /// <summary>
    ///  Returns the character nearest to the given point.
    /// </summary>
    public override char GetCharFromPosition(Point pt)
    {
        char ch;

        _flagState[s_queryBaseText] = true;
        try
        {
            ch = base.GetCharFromPosition(pt);
        }
        finally
        {
            _flagState[s_queryBaseText] = false;
        }

        return ch;
    }

    /// <summary>
    ///  Returns the index of the character nearest to the given point.
    /// </summary>
    public override int GetCharIndexFromPosition(Point pt)
    {
        int index;

        _flagState[s_queryBaseText] = true;
        try
        {
            index = base.GetCharIndexFromPosition(pt);
        }
        finally
        {
            _flagState[s_queryBaseText] = false;
        }

        return index;
    }

    /// <summary>
    ///  Returns the position of the last input character (or if available, the next edit position).
    ///  This is used by base.AppendText.
    /// </summary>
    internal override int GetEndPosition()
    {
        if (_flagState[s_isNullMask])
        {
            return base.GetEndPosition();
        }

        int pos = _maskedTextProvider.FindEditPositionFrom(_maskedTextProvider.LastAssignedPosition + 1, Forward);

        if (pos == MaskedTextProvider.InvalidIndex)
        {
            pos = _maskedTextProvider.LastAssignedPosition + 1;
        }

        return pos;
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int GetFirstCharIndexOfCurrentLine()
    {
        return 0;
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int GetFirstCharIndexFromLine(int lineNumber)
    {
        return 0;
    }

    /// <summary>
    ///  Gets the string in the text box following the formatting parameters includePrompt and includeLiterals and
    ///  honoring the PasswordChar property.
    /// </summary>
    private string GetFormattedDisplayString()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        bool includePrompt;

        if (ReadOnly) // Always hide prompt.
        {
            includePrompt = false;
        }
        else if (DesignMode) // Not RO and at design time, always show prompt.
        {
            includePrompt = true;
        }
        else // follow HidePromptOnLeave property.
        {
            includePrompt = !(HidePromptOnLeave && !Focused);
        }

        return _maskedTextProvider.ToString(/*ignorePwdChar */ false, includePrompt, /*includeLiterals*/ true, 0, _maskedTextProvider.Length);
    }

    /// <summary>
    ///  Unsupported method/property.
    ///  virtual method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetLineFromCharIndex(int index)
    {
        return 0;
    }

    /// <summary>
    ///  Returns the location of the character at the given index.
    /// </summary>
    public override Point GetPositionFromCharIndex(int index)
    {
        Point pos;

        _flagState[s_queryBaseText] = true;
        try
        {
            pos = base.GetPositionFromCharIndex(index);
        }
        finally
        {
            _flagState[s_queryBaseText] = false;
        }

        return pos;
    }

    /// <summary>
    ///  Need to override this method so when get_Text is called we return the text that is actually
    ///  painted in the control so measuring text works on the actual text and not the formatted one.
    /// </summary>
    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        Size size;

        _flagState[s_queryBaseText] = true;
        try
        {
            size = base.GetPreferredSizeCore(proposedConstraints);
        }
        finally
        {
            _flagState[s_queryBaseText] = false;
        }

        return size;
    }

    /// <summary>
    ///  The selected text in the control according to the CutCopyMaskFormat properties (IncludePrompt/IncludeLiterals).
    ///  This is used in Cut/Copy operations (SelectedText).
    ///  The prompt character is always replaced with a blank character.
    /// </summary>
    private string GetSelectedText()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        GetSelectionStartAndLength(out int selStart, out int selLength);

        if (selLength == 0)
        {
            return string.Empty;
        }

        bool includePrompt = (CutCopyMaskFormat & MaskFormat.IncludePrompt) != 0;
        bool includeLiterals = (CutCopyMaskFormat & MaskFormat.IncludeLiterals) != 0;

        return _maskedTextProvider.ToString(ignorePasswordChar: true, includePrompt, includeLiterals, selStart, selLength);
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

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        if (IsAccessibilityObjectCreated)
        {
            AccessibilityObject.SetFocus();
        }
    }

    /// <summary>
    ///  Overridden to update the newly created handle with the settings of the PasswordChar properties
    ///  if no mask has been set.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        SetSelectionOnHandle();

        if (_flagState[s_isNullMask] && _maskedTextProvider.IsPassword)
        {
            SetEditControlPasswordChar(_maskedTextProvider.PasswordChar);
        }
    }

    /// <summary>
    ///  Raises the IsOverwriteModeChanged event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnIsOverwriteModeChanged(EventArgs e)
    {
        if (Events[s_isOverwriteModeChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.KeyDown"/> event.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (_flagState[s_isNullMask])
        {
            // Operates as a regular text box base.
            return;
        }

        Keys keyCode = e.KeyCode;

        // Special-case Return & Esc since they generate invalid characters we should not process OnKeyPress.
        if (keyCode is Keys.Return or Keys.Escape)
        {
            _flagState[s_handleKeyPress] = false;
        }

        // Insert is toggled when not modified with some other key (ctrl, shift...).  Note that shift-Insert is
        // same as paste.
        if (keyCode == Keys.Insert && e.Modifiers == Keys.None && _insertMode == InsertKeyMode.Default)
        {
            _flagState[s_insertToggled] = !_flagState[s_insertToggled];
            OnIsOverwriteModeChanged(EventArgs.Empty);
            return;
        }

        if (e.Control && char.IsLetter((char)keyCode))
        {
            switch (keyCode)
            {
                // Unsupported keys should not be handled to allow generating the corresponding message
                // which is handled in the WndProc.
                // case Keys.Z:  // ctrl-z == Undo.
                // case Keys.Y:  // ctrl-y == Redo.
                //    e.Handled = true;
                //    return;

                // Note: Ctrl-Insert (Copy -Shortcut.CtrlIns) and Shift-Insert (Paste - Shortcut.ShiftIns) are
                // handled by the base class and behavior depend on ShortcutsEnabled property.

                // Special cases: usually cases where the native edit control would modify the mask.
                case Keys.H:  // ctrl-h == Backspace == '\b'
                    keyCode = Keys.Back; // handle it below.
                    break;

                default:
                    // Next OnKeyPress should not be handled to allow Ctrl-<x/c/v/a> to be processed in the
                    // base class so corresponding messages can be generated (WM_CUT/WM_COPY/WM_PASTE).
                    // Combined characters don't generate OnKeyDown by themselves but they generate OnKeyPress.
                    _flagState[s_handleKeyPress] = false;
                    return;
            }
        }

        if (keyCode is Keys.Delete or Keys.Back) // Deletion keys.
        {
            if (!ReadOnly)
            {
                GetSelectionStartAndLength(out int startPosition, out int selectionLen);

                switch (e.Modifiers)
                {
                    case Keys.Shift:
                        if (keyCode == Keys.Delete)
                        {
                            keyCode = Keys.Back;
                        }

                        goto default;

                    case Keys.Control:
                        if (selectionLen == 0) // In other case, the selected text should be deleted.
                        {
                            if (keyCode == Keys.Delete) // delete to the end of the string.
                            {
                                selectionLen = _maskedTextProvider.Length - startPosition;
                            }
                            else // ( keyCode == Keys.Back ) // delete to the beginning of the string.
                            {
                                selectionLen = startPosition == _maskedTextProvider.Length /*at end of text*/ ? startPosition : startPosition + 1;
                                startPosition = 0;
                            }
                        }

                        goto default;

                    default:
                        if (!_flagState[s_handleKeyPress])
                        {
                            _flagState[s_handleKeyPress] = true;
                        }

                        break;
                }

                //
                // Handle special case when using Korean IME and ending a composition.
                //
                /*  This code is no longer needed after fixing

*/

                Delete(keyCode, startPosition, selectionLen);
                e.SuppressKeyPress = true;
            }
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.KeyPress"/> event.
    /// </summary>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);

        if (_flagState[s_isNullMask])
        {
            // Operates as a regular text box base.
            return;
        }

        // This key may be a combined key involving a letter, like Ctrl-A; let the native control handle it.
        if (!_flagState[s_handleKeyPress])
        {
            _flagState[s_handleKeyPress] = true;

            // When the combined key involves a letter, the final character is not a letter. There are some
            // Ctrl combined keys that generate a letter and can be confusing; we do not mean to pass those
            // characters to the underlying Edit control.  These combinations are: Ctrl-F<#> and Ctrl-Atl-<someKey>
            if (!char.IsLetter(e.KeyChar))
            {
                return;
            }
        }

        if (!ReadOnly)
        {
            // At this point the character needs to be processed ...

            GetSelectionStartAndLength(out int selectionStart, out int selectionLen);

            string oldText = TextOutput;
            if (PlaceChar(e.KeyChar, selectionStart, selectionLen, IsOverwriteMode, out MaskedTextResultHint hint))
            {
                if (TextOutput != oldText)
                {
                    SetText(); // Now set the text in the display.
                }

                SelectionStart = ++_caretTestPos; // caretTestPos is updated in PlaceChar.

                if (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable)
                {
                    // Korean IMEs complete composition when a character has been fully converted, so the composition string
                    // is only one-character long; once composed we block the IME if there isn't more room in the test string.

                    int editPos = _maskedTextProvider.FindUnassignedEditPositionFrom(_caretTestPos, Forward);
                    if (editPos == MaskedTextProvider.InvalidIndex)
                    {
                        ImeComplete();  // Force completion of composition.
                    }
                }
            }
            else
            {
                OnMaskInputRejected(new MaskInputRejectedEventArgs(_caretTestPos, hint)); // caretTestPos is updated in PlaceChar.
            }

            if (selectionLen > 0)
            {
                SelectionLength = 0;
            }

            e.Handled = true;
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.KeyUp"/> event.
    /// </summary>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        // KeyUp is the last message to be processed so it is the best place to reset these flags.

        if (_flagState[s_imeCompleting])
        {
            _flagState[s_imeCompleting] = false;
        }

        if (_flagState[s_imeEndingComposition])
        {
            _flagState[s_imeEndingComposition] = false;
        }

        if (IsHandleCreated && IsAccessibilityObjectCreated && ContainsNavigationKeyCode(e.KeyCode))
        {
            AccessibilityObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
        }
    }

    /// <summary>
    ///  Raises the MaskChanged event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMaskChanged(EventArgs e)
    {
        if (Events[s_maskChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the MaskInputRejected event.
    /// </summary>
    private void OnMaskInputRejected(MaskInputRejectedEventArgs e)
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        if (BeepOnError)
        {
            Media.SoundPlayer sp = new();
            sp.Play();
        }

        if (Events[s_maskInputRejectedEvent] is MaskInputRejectedEventHandler eh)
        {
            eh(this, e);
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

    /// <summary>
    ///  Unsupported method/property.
    ///  virtual method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnMultilineChanged(EventArgs e)
    {
    }

    /// <summary>
    ///  Raises the TextAlignChanged event.
    /// </summary>
    protected virtual void OnTextAlignChanged(EventArgs e)
    {
        if (Events[s_textAlignChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the TypeValidationCompleted event.
    /// </summary>
    private void OnTypeValidationCompleted(TypeValidationEventArgs e)
    {
        if (Events[s_validationCompletedEvent] is TypeValidationEventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the  System.Windows.Forms.Control.Validating event.
    ///  Overridden here to be able to control the order validating events are
    ///  raised [TypeValidationCompleted - Validating - Validated - Leave - KillFocus]
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnValidating(CancelEventArgs e)
    {
        // Note: It seems impractical to perform type validation here if the control is read only but we need
        // to be consistent with other TextBoxBase controls which don't check for RO; and we don't want
        // to fix them to avoid introducing breaking changes.
        PerformTypeValidation(e);
        base.OnValidating(e);
    }

    /// <summary>
    ///  Raises the TextChanged event and related Input/Output text events when mask is null.
    ///  Overriden here to be able to control order of text changed events.
    /// </summary>
    protected override void OnTextChanged(EventArgs e)
    {
        // A text changed event handler will most likely query for the Text value, we need to return the
        // formatted one.
        bool queryBaseText = _flagState[s_queryBaseText];
        _flagState[s_queryBaseText] = false;
        try
        {
            base.OnTextChanged(e);
        }
        finally
        {
            _flagState[s_queryBaseText] = queryBaseText;
        }
    }

    /// <summary>
    ///  Replaces the current selection in the text box specified by the startPosition and selectionLen parameters
    ///  with the contents of the supplied string.
    /// </summary>
    private void Replace(string text, int startPosition, int selectionLen)
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");
        Debug.Assert(text is not null, "text is null.");

        // Clone the MaskedTextProvider so text properties are not modified until the paste operation is
        // completed.  This is needed in case one of these properties is retrieved in a MaskedInputRejected
        // event handler (clipboard text is attempted to be set into the input text char by char).

        MaskedTextProvider clonedProvider = (MaskedTextProvider)_maskedTextProvider.Clone();

        // Cache the current caret position so we restore it in case the text does not change.
        int currentCaretPos = _caretTestPos;

        // First replace characters in the selection (if any and if any edit positions) until completed, or the test position falls
        // outside the selection range, or there's no more room in the test string for editable characters.
        // Then insert any remaining characters from the input.

        MaskedTextResultHint hint = MaskedTextResultHint.NoEffect;
        int endPos = startPosition + selectionLen - 1;

        if (RejectInputOnFirstFailure)
        {
            bool succeeded;

            succeeded = (startPosition > endPos) ?
                clonedProvider.InsertAt(text, startPosition, out _caretTestPos, out hint) :
                clonedProvider.Replace(text, startPosition, endPos, out _caretTestPos, out hint);

            if (!succeeded)
            {
                OnMaskInputRejected(new MaskInputRejectedEventArgs(_caretTestPos, hint));
            }
        }
        else
        {
            // temp hint used to preserve the 'primary' operation hint (no side effects).
            MaskedTextResultHint tempHint = hint;
            int testPos;

            foreach (char ch in text)
            {
                if (!_maskedTextProvider.VerifyEscapeChar(ch, startPosition))  // char won't be escaped, find and edit position for it.
                {
                    // Observe that we look for a position w/o respecting the selection length, because the input text could be larger than
                    // the number of edit positions in the selection.
                    testPos = clonedProvider.FindEditPositionFrom(startPosition, Forward);

                    if (testPos == MaskedTextProvider.InvalidIndex)
                    {
                        // this will continue to execute (fail) until the end of the text so we fire the event for each remaining char.
                        OnMaskInputRejected(new MaskInputRejectedEventArgs(startPosition, MaskedTextResultHint.UnavailableEditPosition));
                        continue;
                    }

                    startPosition = testPos;
                }

                int length = endPos >= startPosition ? 1 : 0;

                // if length > 0 we are (re)placing the input char in the current startPosition, otherwise we are inserting the input.
                bool replace = length > 0;

                if (PlaceChar(clonedProvider, ch, startPosition, length, replace, out tempHint))
                {
                    // caretTestPos is updated in PlaceChar call.
                    startPosition = _caretTestPos + 1;

                    // place char will insert or replace a single character so the hint must be success, and that will be the final operation
                    // result hint.
                    if (tempHint == MaskedTextResultHint.Success && hint != tempHint)
                    {
                        hint = tempHint;
                    }
                }
                else
                {
                    OnMaskInputRejected(new MaskInputRejectedEventArgs(startPosition, tempHint));
                }
            }

            if (selectionLen > 0)
            {
                // At this point we have processed all characters from the input text (if any) but still need to
                // remove remaining characters from the selected text (if editable and valid chars).

                if (startPosition <= endPos)
                {
                    if (!clonedProvider.RemoveAt(startPosition, endPos, out _caretTestPos, out tempHint))
                    {
                        OnMaskInputRejected(new MaskInputRejectedEventArgs(_caretTestPos, tempHint));
                    }

                    // If 'replace' is not actually performed (maybe the input is empty which means 'remove', hint will be whatever
                    // the 'remove' operation result hint is.
                    if (hint == MaskedTextResultHint.NoEffect && hint != tempHint)
                    {
                        hint = tempHint;
                    }
                }
            }
        }

        bool updateText = TextOutput != clonedProvider.ToString();

        // Always set the mtp, the formatted text could be the same but the assigned positions may be different.
        _maskedTextProvider = clonedProvider;

        // Update text if needed.
        if (updateText)
        {
            SetText();

            // Update caret position.
            _caretTestPos = startPosition;
            base.SelectInternal(_caretTestPos, 0, _maskedTextProvider.Length);
        }
        else
        {
            _caretTestPos = currentCaretPos;
        }

        return;
    }

    /// <summary>
    ///  Pastes specified text over the currently selected text (if any) shifting upper characters if
    ///  input is longer than selected text, and/or removing remaining characters from the selection if
    ///  input contains less characters.
    /// </summary>
    private void PasteInt(string? text)
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        GetSelectionStartAndLength(out int selStart, out int selLength);

        if (string.IsNullOrEmpty(text))
        {
            Delete(Keys.Delete, selStart, selLength);
        }
        else
        {
            Replace(text, selStart, selLength);
        }
    }

    /// <summary>
    ///  Performs validation of the input string using the provided ValidatingType object (if any).
    ///  Returns an object created from the formatted text.
    ///  If the CancelEventArgs param is not null, it is assumed the control is leaving focus and
    ///  the validation event chain is being executed (TypeValidationCompleted - Validating - Validated...);
    ///  the value of the CancelEventArgs.Cancel property is the same as the TypeValidationEventArgs.Cancel
    ///  on output (Cancel provides proper handling of focus shifting at the Control class level).
    ///  Note: The text being validated does not include prompt chars.
    /// </summary>
    private object? PerformTypeValidation(CancelEventArgs? e)
    {
        object? parseRetVal = null;

        if (_validatingType is not null)
        {
            string? message = null;

            if (!_flagState[s_isNullMask] && !_maskedTextProvider.MaskCompleted)
            {
                message = SR.MaskedTextBoxIncompleteMsg;
            }
            else
            {
                string textValue;

                if (!_flagState[s_isNullMask]) // replace prompt with space.
                {
                    textValue = _maskedTextProvider.ToString(/*includePrompt*/ false, IncludeLiterals);
                }
                else
                {
                    textValue = base.Text;
                }

                try
                {
                    parseRetVal = Formatter.ParseObject(
                        textValue,
                        _validatingType,
                        typeof(string),
                        targetConverter: null,
                        sourceConverter: null,
                        _formatProvider,
                        formattedNullValue: null,
                        Formatter.GetDefaultDataSourceNullValue(_validatingType));
                }
                catch (Exception exception) when (!exception.IsCriticalException())
                {
                    if (exception.InnerException is not null)
                    {
                        // Outer exception is a generic TargetInvocationException.
                        exception = exception.InnerException;
                    }

                    message = $"{exception.GetType()}: {exception.Message}";
                }
            }

            bool isValidInput = false;
            if (message is null)
            {
                isValidInput = true;
                message = SR.MaskedTextBoxTypeValidationSucceeded;
            }

            TypeValidationEventArgs tve = new(_validatingType, isValidInput, parseRetVal, message);
            OnTypeValidationCompleted(tve);

            if (e is not null)
            {
                e.Cancel = tve.Cancel;
            }
        }

        return parseRetVal;
    }

    /// <summary>
    ///  Insert or replaces the specified character into the control's text and updates the caret position.
    ///  If overwrite is true, it replaces the character at the selection start position.
    /// </summary>
    private bool PlaceChar(char ch, int startPosition, int length, bool overwrite,
        out MaskedTextResultHint hint)
    {
        return PlaceChar(_maskedTextProvider, ch, startPosition, length, overwrite, out hint);
    }

    /// <summary>
    ///  Override version to be able to perform the operation on a cloned provider.
    /// </summary>
    private bool PlaceChar(MaskedTextProvider provider, char ch, int startPosition, int length, bool overwrite,
        out MaskedTextResultHint hint)
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        _caretTestPos = startPosition;

        if (startPosition < _maskedTextProvider.Length)
        {
            if (length > 0)  // Replacing selection with input char.
            {
                int endPos = startPosition + length - 1;
                return provider.Replace(ch, startPosition, endPos, out _caretTestPos, out hint);
            }
            else
            {
                if (overwrite)
                {
                    // overwrite character at next edit position from startPosition (inclusive).
                    return provider.Replace(ch, startPosition, out _caretTestPos, out hint);
                }
                else // insert.
                {
                    return provider.InsertAt(ch, startPosition, out _caretTestPos, out hint);
                }
            }
        }

        hint = MaskedTextResultHint.UnavailableEditPosition;
        return false;
    }

    /// <summary>
    ///  Implements the handling of Ctrl+A (select all). Note: Code copied from TextBox.
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        //
        // The base class should be called first because it implements ShortcutsEnabled,
        // which takes precedence over Ctrl+A
        //
        bool msgProcessed = base.ProcessCmdKey(ref msg, keyData);

        if (!msgProcessed)
        {
            if ((int)keyData == (int)Shortcut.CtrlA)
            {
                SelectAll();
                msgProcessed = true; // This prevents generating a WM_CHAR for 'A'.
            }
        }

        return msgProcessed;
    }

    /// <summary>
    ///  We need to override this method so we can handle input language changes properly.  Control
    ///  doesn't handle the WM_CHAR messages generated after WM_IME_CHAR messages, it passes them
    ///  to DefWndProc (the characters would be displayed in the text box always).
    /// </summary>
    protected internal override bool ProcessKeyMessage(ref Message m)
    {
        // call base's method so the WM_CHAR and other messages are processed; this gives Control the
        // chance to flush all pending WM_CHAR processing after WM_IME_CHAR messages are generated.

        bool msgProcessed = base.ProcessKeyMessage(ref m);

        if (_flagState[s_isNullMask])
        {
            return msgProcessed; // Operates as a regular text box base.
        }

        // If this WM_CHAR message is sent after WM_IME_CHAR, we ignore it since we already processed
        // the corresponding WM_IME_CHAR message.
        if (m.Msg == PInvoke.WM_CHAR && ImeWmCharsToIgnore > 0)
        {
            return true;    // meaning, we handled the message so it is not passed to the default WndProc.
        }

        return msgProcessed;
    }

    /// <summary>
    ///  Design time support for resetting Culture property..
    /// </summary>
    private void ResetCulture()
    {
        Culture = CultureInfo.CurrentCulture;
    }

    /// <summary>
    ///  Unsupported method/property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void ScrollToCaret()
    {
    }

    /// <summary>
    ///  Sets the underlying MaskedTextProvider object.  Used when the control is initialized
    ///  and one of its properties, backed up by the MaskedTextProvider, changes; this requires
    ///  recreating the provider because it is immutable.
    /// </summary>
    private void SetMaskedTextProvider(MaskedTextProvider newProvider)
    {
        SetMaskedTextProvider(newProvider, null);
    }

    /// <summary>
    ///  Overload to allow for passing the text when the mask is being changed from null,
    ///  in this case the maskedTextProvider holds backend info only (not the text).
    /// </summary>
    private void SetMaskedTextProvider(MaskedTextProvider newProvider, string? textOnInitializingMask)
    {
        Debug.Assert(newProvider is not null, "Initializing from a null MaskProvider ref.");

        // Set R/W properties.
        newProvider.IncludePrompt = _maskedTextProvider.IncludePrompt;
        newProvider.IncludeLiterals = _maskedTextProvider.IncludeLiterals;
        newProvider.SkipLiterals = _maskedTextProvider.SkipLiterals;
        newProvider.ResetOnPrompt = _maskedTextProvider.ResetOnPrompt;
        newProvider.ResetOnSpace = _maskedTextProvider.ResetOnSpace;

        // If mask not initialized and not initializing it, the new provider is just a property backend.
        // Change won't have any effect in text.
        if (_flagState[s_isNullMask] && textOnInitializingMask is null)
        {
            _maskedTextProvider = newProvider;
            return;
        }

        int testPos = 0;
        bool raiseOnMaskInputRejected = false; // Raise if new provider rejects old text.
        MaskedTextResultHint hint = MaskedTextResultHint.NoEffect;
        MaskedTextProvider oldProvider = _maskedTextProvider;

        // Attempt to add previous text.
        // If the mask is the same, we need to preserve the caret and character positions if the text is added successfully.
        bool preserveCharPos = oldProvider.Mask == newProvider.Mask;

        // Cache text output text before setting the new provider to determine whether we need to raise the TextChanged event.
        string oldText;

        // NOTE: Whenever changing the MTP, the text is lost if any character in the old text violates the new provider's mask.

        if (textOnInitializingMask is not null) // Changing Mask (from null), which is the only RO property that requires passing text.
        {
            oldText = textOnInitializingMask;
            raiseOnMaskInputRejected = !newProvider.Set(textOnInitializingMask, out testPos, out hint);
        }
        else
        {
            oldText = TextOutput;

            // We need to attempt to set the input characters one by one in the edit positions so they are not
            // escaped.
            int assignedCount = oldProvider.AssignedEditPositionCount;
            int srcPos = 0;
            int dstPos = 0;

            while (assignedCount > 0)
            {
                srcPos = oldProvider.FindAssignedEditPositionFrom(srcPos, Forward);
                Debug.Assert(srcPos != MaskedTextProvider.InvalidIndex, "InvalidIndex unexpected at this time.");

                if (preserveCharPos)
                {
                    dstPos = srcPos;
                }
                else
                {
                    dstPos = newProvider.FindEditPositionFrom(dstPos, Forward);

                    if (dstPos == MaskedTextProvider.InvalidIndex)
                    {
                        newProvider.Clear();

                        testPos = newProvider.Length;
                        hint = MaskedTextResultHint.UnavailableEditPosition;
                        break;
                    }
                }

                if (!newProvider.Replace(oldProvider[srcPos], dstPos, out testPos, out hint))
                {
                    preserveCharPos = false;
                    newProvider.Clear();
                    break;
                }

                srcPos++;
                dstPos++;
                assignedCount--;
            }

            raiseOnMaskInputRejected = !MaskedTextProvider.GetOperationResultFromHint(hint);
        }

        // Set provider.
        _maskedTextProvider = newProvider;

        if (_flagState[s_isNullMask])
        {
            _flagState[s_isNullMask] = false;
        }

        // Raising events need to be done only after the new provider has been set so the MTB is in a state where properties
        // can be queried from event handlers safely.
        if (raiseOnMaskInputRejected)
        {
            OnMaskInputRejected(new MaskInputRejectedEventArgs(testPos, hint));
        }

        if (newProvider.IsPassword)
        {
            // Reset native edit control so the MaskedTextBox will take control over the characters that
            // need to be replaced with the password char (the input text characters).
            // MTB takes over.
            SetEditControlPasswordChar('\0');
        }

        EventArgs e = EventArgs.Empty;

        if (textOnInitializingMask is not null /*changing mask from null*/ || oldProvider.Mask != newProvider.Mask)
        {
            OnMaskChanged(e);
        }

        SetWindowText(GetFormattedDisplayString(), oldText != TextOutput, preserveCharPos);
    }

    /// <summary>
    ///  Sets the control's text to the formatted text obtained from the underlying MaskedTextProvider.
    ///  TextChanged is raised always, this assumes the display or the output text changed.
    ///  The caret position is lost (unless cached somewhere else like when losing the focus).
    ///  This is the common way of changing the text in the control.
    /// </summary>
    private void SetText()
    {
        SetWindowText(GetFormattedDisplayString(), true, false);
    }

    /// <summary>
    ///  Sets the control's text to the formatted text obtained from the underlying MaskedTextProvider.
    ///  TextChanged is not raised. [PasswordChar]
    ///  The caret position is preserved.
    /// </summary>
    private void SetWindowText()
    {
        SetWindowText(GetFormattedDisplayString(), false, true);
    }

    /// <summary>
    ///  Sets the text directly in the underlying edit control to the value specified.
    ///  The 'raiseTextChangedEvent' param determines whether TextChanged event is raised or not.
    ///  The 'preserveCaret' param determines whether an attempt to preserve the caret position should be made or not
    ///  after the call to SetWindowText (WindowText) is performed.
    /// </summary>
    private void SetWindowText(string text, bool raiseTextChangedEvent, bool preserveCaret)
    {
        _flagState[s_queryBaseText] = true;

        try
        {
            if (preserveCaret)
            {
                _caretTestPos = SelectionStart;
            }

            WindowText = text;  // this calls Win32::SetWindowText directly, no OnTextChanged raised.

            if (raiseTextChangedEvent)
            {
                OnTextChanged(EventArgs.Empty);
            }

            if (preserveCaret)
            {
                SelectionStart = _caretTestPos;
            }
        }
        finally
        {
            _flagState[s_queryBaseText] = false;
        }
    }

    /// <summary>
    ///  Design time support for checking if Culture value in the designer should be serialized.
    /// </summary>
    private bool ShouldSerializeCulture()
    {
        return !CultureInfo.CurrentCulture.Equals(Culture);
    }

    /// <summary>
    ///  Undoes the last edit operation in the text box.
    ///  Unsupported property/method.
    ///  WndProc ignores EM_UNDO.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void Undo()
    {
    }

    /// <summary>
    ///  Forces type validation.  Returns the validated text value.
    /// </summary>
    public object? ValidateText()
    {
        return PerformTypeValidation(null);
    }

    /// <summary>
    ///  Deletes all input characters in the current selection.
    /// </summary>
    private bool WmClear()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        if (!ReadOnly)
        {
            GetSelectionStartAndLength(out int selStart, out int selLength);
            Delete(Keys.Delete, selStart, selLength);
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Copies current selection text to the clipboard, formatted according to the IncludeLiterals properties but
    ///  ignoring the prompt character.
    ///  Returns true if the operation succeeded, false otherwise.
    /// </summary>
    private bool WmCopy()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        if (_maskedTextProvider.IsPassword) // cannot copy password to clipboard.
        {
            return false;
        }

        string text = GetSelectedText();

        try
        {
            if (text.Length == 0)
            {
                Clipboard.Clear();
            }
            else
            {
                Clipboard.SetText(text);
            }
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Note: Sometimes the above operation throws but it successfully sets the
            // data in the clipboard. This usually happens when the Application's Main
            // is not attributed with [STAThread].
        }

        return true;
    }

    /// <summary>
    ///  Processes the WM_IME_COMPOSITION message when using Korean IME.
    ///  Korean IME uses the control's caret as the composition string (it processes only one character at a time),
    ///  we need to have special message handling for it.
    ///  Returns true if the message is handled, false otherwise.
    /// </summary>
    private bool WmImeComposition(ref Message m)
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

#if DEBUG
        if (ReadOnly || _maskedTextProvider.IsPassword)
        {
            // This should have been already handled by the ReadOnly, PasswordChar and ImeMode properties.
            Debug.Assert(ImeMode == ImeMode.Disable, "IME enabled when in RO or Pwd mode.");
        }
#endif
        // Non-Korean IMEs complete composition when all characters in the string has been composed (when user hits enter);
        // Currently, we don't support checking the composition string characters because it would require similar logic
        // as the MaskedTextBox itself.

        if (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable)
        {
            byte imeConversionType = ImeConversionNone;

            // Check if there's an update to the composition string:
            if ((m.LParamInternal & (int)IME_COMPOSITION_STRING.GCS_COMPSTR) != 0)
            {
                // The character in the composition has been updated but not yet converted.
                imeConversionType = ImeConversionUpdate;
            }
            else if ((m.LParamInternal & (int)IME_COMPOSITION_STRING.GCS_RESULTSTR) != 0)
            {
                // The character(s) in the composition has been fully converted.
                imeConversionType = ImeConversionCompleted;
            }

            // Process any update in the composition string.
            if (imeConversionType != ImeConversionNone)
            {
                if (_flagState[s_imeEndingComposition])
                {
                    // If IME is completing the conversion, we don't want to process further characters.
                    return _flagState[s_imeCompleting];
                }
            }
        }

        return false; // message not handled.
    }

    /// <summary>
    ///  Processes the WM_IME_STARTCOMPOSITION message.
    ///  Returns true if the message is handled, false otherwise.
    /// </summary>
    private bool WmImeStartComposition()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        // Position the composition window in a valid place.

        GetSelectionStartAndLength(out int startPosition, out int selectionLen);

        int startEditPos = _maskedTextProvider.FindEditPositionFrom(startPosition, Forward);

        if (startEditPos != MaskedTextProvider.InvalidIndex)
        {
            if (selectionLen > 0 && (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable))
            {
                // Korean IME: We need to delete the selected text and reposition the caret so the IME processes one
                // character only, otherwise it would overwrite the selection with the caret (composition string),
                // deleting a portion of the mask.

                int endEditPos = _maskedTextProvider.FindEditPositionFrom(startPosition + selectionLen - 1, Backward);

                if (endEditPos >= startEditPos)
                {
                    selectionLen = endEditPos - startEditPos + 1;
                    Delete(Keys.Delete, startEditPos, selectionLen);
                }
                else
                {
                    ImeComplete();
                    OnMaskInputRejected(new MaskInputRejectedEventArgs(startPosition, MaskedTextResultHint.UnavailableEditPosition));
                    return true;
                }
            }

            // update caret position.
            if (startPosition != startEditPos)
            {
                _caretTestPos = startEditPos;
                SelectionStart = _caretTestPos;
            }

            SelectionLength = 0;
        }
        else
        {
            ImeComplete();
            OnMaskInputRejected(new MaskInputRejectedEventArgs(startPosition, MaskedTextResultHint.UnavailableEditPosition));
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Processes the WM_PASTE message. Copies the text from the clipboard, if is valid,
    ///  formatted according to the mask applied to this control.
    ///  Returns true if the operation succeeded, false otherwise.
    /// </summary>
    private void WmPaste()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        if (ReadOnly)
        {
            return;
        }

        // Get the text from the clipboard.

        string text;

        try
        {
            text = Clipboard.GetText();
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            Debug.Fail(ex.ToString());
            return;
        }

        PasteInt(text);
    }

    private void WmPrint(ref Message m)
    {
        base.WndProc(ref m);
        if (((nint)m.LParamInternal & PInvoke.PRF_NONCLIENT) != 0
            && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
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
    ///  We need to override the WndProc method to have full control over what characters can be
    ///  displayed in the text box; particularly, we have special handling when IME is turned on.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        // Handle messages for special cases (unsupported operations or cases where mask doesn not matter).
        switch (m.MsgInternal)
        {
            case PInvoke.WM_PRINT:
                WmPrint(ref m);
                return;
            case PInvoke.WM_CONTEXTMENU:
            case (int)PInvoke.EM_CANUNDO:
                base.ClearUndo(); // resets undo buffer.
                base.WndProc(ref m);
                return;

            case (int)PInvoke.EM_SCROLLCARET:  // No scroll for single-line control.
            case (int)PInvoke.EM_LIMITTEXT:    // Max/Min text is defined by the mask.
            case (int)PInvoke.EM_UNDO:
            case PInvoke.WM_UNDO:
                return;

            default:
                break;  // continue.
        }

        if (_flagState[s_isNullMask])
        {
            base.WndProc(ref m); // Operates as a regular text box base.
            return;
        }

        switch (m.MsgInternal)
        {
            case PInvoke.WM_IME_STARTCOMPOSITION:
                if (WmImeStartComposition())
                {
                    break;
                }

                goto default;

            case PInvoke.WM_IME_ENDCOMPOSITION:
                _flagState[s_imeEndingComposition] = true;
                goto default;

            case PInvoke.WM_IME_COMPOSITION:
                if (WmImeComposition(ref m))
                {
                    break;
                }

                goto default;

            case PInvoke.WM_CUT:
                if (!ReadOnly && WmCopy())
                {
                    WmClear();
                }

                break;

            case PInvoke.WM_COPY:
                WmCopy();
                break;

            case PInvoke.WM_PASTE:
                WmPaste();
                break;

            case PInvoke.WM_CLEAR:
                WmClear();
                break;

            case PInvoke.WM_KILLFOCUS:
                base.WndProc(ref m);
                WmKillFocus();
                break;

            case PInvoke.WM_SETFOCUS:
                WmSetFocus();
                base.WndProc(ref m);
                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }

    /// <summary>
    ///  Processes the WM_KILLFOCUS message. Updates control's text replacing prompt chars with space.
    /// </summary>
    private void WmKillFocus()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        GetSelectionStartAndLength(out _caretTestPos, out _lastSelLength);

        if (HidePromptOnLeave && !MaskFull)
        {
            SetWindowText(); // Update text w/ no prompt.

            // We need to update selection info in case the control is queried for it while it doesn't have the focus.
            base.SelectInternal(_caretTestPos, _lastSelLength, _maskedTextProvider.Length);
        }
    }

    /// <summary>
    ///  Processes the WM_SETFOCUS message. Updates control's text with formatted text according to
    ///  the include prompt property.
    /// </summary>
    private void WmSetFocus()
    {
        Debug.Assert(!_flagState[s_isNullMask], "This method must be called when a Mask is provided.");

        if (HidePromptOnLeave && !MaskFull) // Prompt will show up.
        {
            SetWindowText();
        }

        // Restore previous selection. Do this always (as opposed to within the condition above as in WmKillFocus)
        // because HidePromptOnLeave could have changed while the control did not have the focus.
        base.SelectInternal(_caretTestPos, _lastSelLength, _maskedTextProvider.Length);
    }
}
