// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  MaskedTextBox control definition class.
    ///  Uses the services from the System.ComponentModel.MaskedTextBoxProvider class.
    ///  See spec at http://dotnetclient/whidbey/Specs/MaskEdit.doc
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(MaskInputRejected)),
    DefaultBindingProperty(nameof(Text)),
    DefaultProperty(nameof(Mask)),
    Designer("System.Windows.Forms.Design.MaskedTextBoxDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionMaskedTextBox))
    ]
    public class MaskedTextBox : TextBoxBase
    {
        // Consider: The MaskedTextBox control, when initialized with a non-null/empty mask, processes all
        // WM_CHAR messages and always sets the text using the SetWindowText Windows function in the furthest base
        // class.  This means that the underlying Edit control won't enable Undo operations and the context
        // menu behavior will be a bit different (for instance Copy option is enabled when PasswordChar is set).
        // To provide Undo functionality and make the context menu behave like the Edit control, we would have
        // to implement our own.  See http://msdn.microsoft.com/msdnmag/issues/1100/c/default.aspx for more info
        // about how to do this. See postponed

        private const bool forward = true;
        private const bool backward = false;
        private const string nullMask = "<>"; // any char/str is OK here.

        private static readonly object EVENT_MASKINPUTREJECTED = new object();
        private static readonly object EVENT_VALIDATIONCOMPLETED = new object();
        private static readonly object EVENT_TEXTALIGNCHANGED = new object();
        private static readonly object EVENT_ISOVERWRITEMODECHANGED = new object();
        private static readonly object EVENT_MASKCHANGED = new object();

        // The native edit control's default password char (per thread). See corresponding property for more info.
        private static char systemPwdChar;

        // Values to track changes in IME composition string (if any).  Having const variables is a bit more efficient
        // than having an enum (which creates a class).
        private const byte imeConvertionNone = 0;  // no convertion has been performed in the composition string.
        private const byte imeConvertionUpdate = 1;  // the char being composed has been updated but not coverted yet.
        private const byte imeConvertionCompleted = 2;  // the char being composed has been fully converted.

        /////////  Instance fields

        // Used for keeping selection when prompt is hidden on leave (text changes).
        private int lastSelLength;

        // Used for caret positioning.
        private int caretTestPos;

        // Bit mask - Determines when the Korean IME composition string is completed so converted character can be processed.
        private static readonly int IME_ENDING_COMPOSITION = BitVector32.CreateMask();

        // Bit mask - Determines when the Korean IME is completing a composition, used when forcing convertion.
        private static readonly int IME_COMPLETING = BitVector32.CreateMask(IME_ENDING_COMPOSITION);

        // Used for handling characters that have a modifier (Ctrl-A, Shift-Del...).
        private static readonly int HANDLE_KEY_PRESS = BitVector32.CreateMask(IME_COMPLETING);

        // Bit mask - Used to simulate a null mask.  Needed since a MaskedTextProvider object cannot be
        // initialized with a null mask but we need one even in this case as a backend for
        // default properties.  This is to support creating a MaskedTextBox with the default
        // constructor, specially at design time.
        private static readonly int IS_NULL_MASK = BitVector32.CreateMask(HANDLE_KEY_PRESS);

        // Bit mask - Used in conjuction with get_Text to return the text that is actually set in the native
        // control.  This is required to be able to measure text correctly (GetPreferredSize) and
        // to compare against during set_Text (to bail if the same and not to raise TextChanged event).
        private static readonly int QUERY_BASE_TEXT = BitVector32.CreateMask(IS_NULL_MASK);

        // If true, the input text is rejected whenever a character does not comply with the mask; a MaskInputRejected
        // event is fired for the failing character.
        // If false, characters in the input string are processed one by one accepting the ones that comply
        // with the mask and raising the MaskInputRejected event for the rejected ones.
        private static readonly int REJECT_INPUT_ON_FIRST_FAILURE = BitVector32.CreateMask(QUERY_BASE_TEXT);

        // Bit masks for boolean properties.
        private static readonly int HIDE_PROMPT_ON_LEAVE = BitVector32.CreateMask(REJECT_INPUT_ON_FIRST_FAILURE);
        private static readonly int BEEP_ON_ERROR = BitVector32.CreateMask(HIDE_PROMPT_ON_LEAVE);
        private static readonly int USE_SYSTEM_PASSWORD_CHAR = BitVector32.CreateMask(BEEP_ON_ERROR);
        private static readonly int INSERT_TOGGLED = BitVector32.CreateMask(USE_SYSTEM_PASSWORD_CHAR);
        private static readonly int CUTCOPYINCLUDEPROMPT = BitVector32.CreateMask(INSERT_TOGGLED);
        private static readonly int CUTCOPYINCLUDELITERALS = BitVector32.CreateMask(CUTCOPYINCLUDEPROMPT);

        /////////  Properties backend fields. See corresponding property comments for more info.

        private char passwordChar; // control's pwd char, it could be different from the one displayed if using system password.
        private Type validatingType;
        private IFormatProvider formatProvider;
        private MaskedTextProvider maskedTextProvider;
        private InsertKeyMode insertMode;
        private HorizontalAlignment textAlign;

        // Bit vector to represent bool variables.
        private BitVector32 flagState;

        /// <summary>
        ///  Constructs the MaskedTextBox with the specified MaskedTextProvider object.
        /// </summary>
        public MaskedTextBox()
        {
            MaskedTextProvider maskedTextProvider = new MaskedTextProvider(nullMask, CultureInfo.CurrentCulture);
            flagState[IS_NULL_MASK] = true;
            Initialize(maskedTextProvider);
        }

        /// <summary>
        ///  Constructs the MaskedTextBox with the specified MaskedTextProvider object.
        /// </summary>
        public MaskedTextBox(string mask)
        {
            if (mask == null)
            {
                throw new ArgumentNullException();
            }

            MaskedTextProvider maskedTextProvider = new MaskedTextProvider(mask, CultureInfo.CurrentCulture);
            flagState[IS_NULL_MASK] = false;
            Initialize(maskedTextProvider);
        }

        /// <summary>
        ///  Constructs the MaskedTextBox with the specified MaskedTextProvider object.
        /// </summary>
        public MaskedTextBox(MaskedTextProvider maskedTextProvider)
        {
            if (maskedTextProvider == null)
            {
                throw new ArgumentNullException();
            }

            flagState[IS_NULL_MASK] = false;
            Initialize(maskedTextProvider);
        }

        /// <summary>
        ///  Initializes the object with the specified MaskedTextProvider object and default
        ///  property values.
        /// </summary>
        private void Initialize(MaskedTextProvider maskedTextProvider)
        {
            Debug.Assert(maskedTextProvider != null, "Initializing from a null MaskProvider ref.");

            this.maskedTextProvider = maskedTextProvider;

            // set the initial display text.
            if (!flagState[IS_NULL_MASK])
            {
                SetWindowText();
            }

            // set default values.
            passwordChar = this.maskedTextProvider.PasswordChar;
            insertMode = InsertKeyMode.Default;

            flagState[HIDE_PROMPT_ON_LEAVE] = false;
            flagState[BEEP_ON_ERROR] = false;
            flagState[USE_SYSTEM_PASSWORD_CHAR] = false;
            flagState[REJECT_INPUT_ON_FIRST_FAILURE] = false;

            // CutCopyMaskFormat - set same defaults as TextMaskFormat (IncludePromptAndLiterals).
            // It is a lot easier to handle this flags individually since that's the way the MaskedTextProvider does it.
            flagState[CUTCOPYINCLUDEPROMPT] = this.maskedTextProvider.IncludePrompt;
            flagState[CUTCOPYINCLUDELITERALS] = this.maskedTextProvider.IncludeLiterals;

            // fields for internal use.
            flagState[HANDLE_KEY_PRESS] = true;
            caretTestPos = 0;
        }

        ///////////////////  Properties
        ///
        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxAllowPromptAsInputDescr)),
        DefaultValue(true)
        ]
        public bool AllowPromptAsInput
        {
            get
            {
                return maskedTextProvider.AllowPromptAsInput;
            }
            set
            {
                if (value != maskedTextProvider.AllowPromptAsInput)
                {
                    // Recreate masked text provider since this property is read-only.
                    MaskedTextProvider newProvider = new MaskedTextProvider(
                        maskedTextProvider.Mask,
                        maskedTextProvider.Culture,
                        value,
                        maskedTextProvider.PromptChar,
                        maskedTextProvider.PasswordChar,
                        maskedTextProvider.AsciiOnly);

                    SetMaskedTextProvider(newProvider);
                }
            }
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new event EventHandler AcceptsTabChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        ///  Specifies whether only ASCII characters are accepted as valid input.
        ///  This property has no particular effect if no mask has been set.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxAsciiOnlyDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(false)
        ]
        public bool AsciiOnly
        {
            get
            {
                return maskedTextProvider.AsciiOnly;
            }

            set
            {
                if (value != maskedTextProvider.AsciiOnly)
                {
                    // Recreate masked text provider since this property is read-only.
                    MaskedTextProvider newProvider = new MaskedTextProvider(
                        maskedTextProvider.Mask,
                        maskedTextProvider.Culture,
                        maskedTextProvider.AllowPromptAsInput,
                        maskedTextProvider.PromptChar,
                        maskedTextProvider.PasswordChar,
                        value);

                    SetMaskedTextProvider(newProvider);
                }
            }
        }

        /// <summary>
        ///  Specifies whether to play a beep when the input is not valid according to the mask.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxBeepOnErrorDescr)),
        DefaultValue(false)
        ]
        public bool BeepOnError
        {
            get
            {
                return flagState[BEEP_ON_ERROR];
            }
            set
            {
                flagState[BEEP_ON_ERROR] = value;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the user can undo the previous operation in a text box control.
        ///  Unsupported method/property.
        ///  WndProc ignores EM_CANUNDO.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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
                //
                HorizontalAlignment align = RtlTranslateHorizontal(textAlign);
                cp.ExStyle &= ~NativeMethods.WS_EX_RIGHT;   // WS_EX_RIGHT overrides the ES_XXXX alignment styles
                switch (align)
                {
                    case HorizontalAlignment.Left:
                        cp.Style |= NativeMethods.ES_LEFT;
                        break;
                    case HorizontalAlignment.Center:
                        cp.Style |= NativeMethods.ES_CENTER;
                        break;
                    case HorizontalAlignment.Right:
                        cp.Style |= NativeMethods.ES_RIGHT;
                        break;
                }

                return cp;
            }
        }

        /// <summary>
        ///  The culture that determines the value of the localizable mask language separators and placeholders.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxCultureDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        ]
        public CultureInfo Culture
        {
            get
            {
                return maskedTextProvider.Culture;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (!maskedTextProvider.Culture.Equals(value))
                {
                    // Recreate masked text provider since this property is read-only.
                    MaskedTextProvider newProvider = new MaskedTextProvider(
                        maskedTextProvider.Mask,
                        value,
                        maskedTextProvider.AllowPromptAsInput,
                        maskedTextProvider.PromptChar,
                        maskedTextProvider.PasswordChar,
                        maskedTextProvider.AsciiOnly);

                    SetMaskedTextProvider(newProvider);
                }
            }
        }

        /// <summary>
        ///  Specifies the formatting options for text cut/copited to the clipboard (Whether the mask returned from the Text
        ///  property includes Literals and/or prompt characters).
        ///  When prompt characters are excluded, theyare returned as spaces in the string returned.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxCutCopyMaskFormat)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(MaskFormat.IncludeLiterals)
        ]
        public MaskFormat CutCopyMaskFormat
        {
            get
            {
                if (flagState[CUTCOPYINCLUDEPROMPT])
                {
                    if (flagState[CUTCOPYINCLUDELITERALS])
                    {
                        return MaskFormat.IncludePromptAndLiterals;
                    }

                    return MaskFormat.IncludePrompt;
                }

                if (flagState[CUTCOPYINCLUDELITERALS])
                {
                    return MaskFormat.IncludeLiterals;
                }

                return MaskFormat.ExcludePromptAndLiterals;
            }

            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)MaskFormat.ExcludePromptAndLiterals, (int)MaskFormat.IncludePromptAndLiterals))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(MaskFormat));
                }

                if (value == MaskFormat.IncludePrompt)
                {
                    flagState[CUTCOPYINCLUDEPROMPT] = true;
                    flagState[CUTCOPYINCLUDELITERALS] = false;
                }
                else if (value == MaskFormat.IncludeLiterals)
                {
                    flagState[CUTCOPYINCLUDEPROMPT] = false;
                    flagState[CUTCOPYINCLUDELITERALS] = true;
                }
                else // value == MaskFormat.IncludePromptAndLiterals || value == MaskFormat.ExcludePromptAndLiterals
                {
                    bool include = value == MaskFormat.IncludePromptAndLiterals;
                    flagState[CUTCOPYINCLUDEPROMPT] = include;
                    flagState[CUTCOPYINCLUDELITERALS] = include;
                }
            }
        }

        /// <summary>
        ///  Specifies the IFormatProvider to be used when parsing the string to the ValidatingType.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public IFormatProvider FormatProvider
        {
            get
            {
                return formatProvider;
            }

            set
            {
                formatProvider = value;
            }
        }

        /// <summary>
        ///  Specifies whether the PromptCharacter is displayed when the control loses focus.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxHidePromptOnLeaveDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(false)
        ]
        public bool HidePromptOnLeave
        {
            get
            {
                return flagState[HIDE_PROMPT_ON_LEAVE];
            }
            set
            {
                if (flagState[HIDE_PROMPT_ON_LEAVE] != value)
                {
                    flagState[HIDE_PROMPT_ON_LEAVE] = value;

                    // If the control is not focused and there are available edit positions (mask not full) we need to
                    // update the displayed text.
                    if (!flagState[IS_NULL_MASK] && !Focused && !MaskFull && !DesignMode)
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
                return maskedTextProvider.IncludeLiterals;
            }
            set
            {
                maskedTextProvider.IncludeLiterals = value;
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
                return maskedTextProvider.IncludePrompt;
            }
            set
            {
                maskedTextProvider.IncludePrompt = value;
            }
        }

        /// <summary>
        ///  Specifies the text insertion mode of the text box.  This can be used to simulated the Access masked text
        ///  control behavior where insertion is set to TextInsertionMode.AlwaysOverwrite
        ///  This property has no particular effect if no mask has been set.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxInsertKeyModeDescr)),
        DefaultValue(InsertKeyMode.Default)
        ]
        public InsertKeyMode InsertKeyMode
        {
            get
            {
                return insertMode;
            }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)InsertKeyMode.Default, (int)InsertKeyMode.Overwrite))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(InsertKeyMode));
                }

                if (insertMode != value)
                {
                    bool isOverwrite = IsOverwriteMode;
                    insertMode = value;

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
        [
        Browsable(false)
        ]
        public bool IsOverwriteMode
        {
            get
            {
                if (flagState[IS_NULL_MASK])
                {
                    return false; // EditBox always inserts.
                }

                switch (insertMode)
                {
                    case InsertKeyMode.Overwrite:
                        return true;

                    case InsertKeyMode.Insert:
                        return false;

                    case InsertKeyMode.Default:

                        // Note that the insert key state should be per process and its initial state insert, this is the
                        // behavior of apps like WinWord, WordPad and VS; so we have to keep track of it and not query its
                        // system value.
                        //return Control.IsKeyLocked(Keys.Insert);
                        return flagState[INSERT_TOGGLED];

                    default:
                        Debug.Fail("Invalid InsertKeyMode.  This code path should have never been executed.");
                        return false;
                }
            }
        }

        /// <summary>
        ///  Event to notify when the insert mode has changed.  This is required for data binding.
        /// </summary>
        [
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.MaskedTextBoxIsOverwriteModeChangedDescr))
        ]
        public event EventHandler IsOverwriteModeChanged
        {
            add => Events.AddHandler(EVENT_ISOVERWRITEMODECHANGED, value);
            remove => Events.RemoveHandler(EVENT_ISOVERWRITEMODECHANGED, value);
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new string[] Lines
        {
            get
            {
                string[] lines;

                flagState[QUERY_BASE_TEXT] = true;
                try
                {
                    lines = base.Lines;
                }
                finally
                {
                    flagState[QUERY_BASE_TEXT] = false;
                }

                return lines;
            }

            set { }
        }

        /// <summary>
        ///  The mask applied to this control.  The setter resets the underlying MaskedTextProvider object and attempts
        ///  to add the existing input text (if any) using the new mask, failure is ignored.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxMaskDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(""),
        MergableProperty(false),
        Localizable(true),
        Editor("System.Windows.Forms.Design.MaskPropertyEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public string Mask
        {
            get
            {
                return flagState[IS_NULL_MASK] ? string.Empty : maskedTextProvider.Mask;
            }
            set
            {
                //
                // We dont' do anything if:
                // 1.  IsNullOrEmpty( value )->[Reset control] && this.flagState[IS_NULL_MASK]==>Already Reset.
                // 2. !IsNullOrEmpty( value )->[Set control] && !this.flagState[IS_NULL_MASK][control is set] && [value is the same]==>No need to update.
                //
                if (flagState[IS_NULL_MASK] == string.IsNullOrEmpty(value) && (flagState[IS_NULL_MASK] || value == maskedTextProvider.Mask))
                {
                    return;
                }

                string text = null;
                string newMask = value;

                // We need to update the this.flagState[IS_NULL_MASK]field before raising any events (when setting the maskedTextProvider) so
                // querying for properties from an event handler returns the right value (i.e: Text).

                if (string.IsNullOrEmpty(value)) // Resetting the control, the native edit control will be in charge.
                {
                    // Need to get the formatted & unformatted text before resetting the mask, they'll be used to determine whether we need to
                    // raise the TextChanged event.
                    string formattedText = TextOutput;
                    string unformattedText = maskedTextProvider.ToString(false, false);

                    flagState[IS_NULL_MASK] = true;

                    if (maskedTextProvider.IsPassword)
                    {
                        SetEditControlPasswordChar(maskedTextProvider.PasswordChar);
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

                    newMask = nullMask;
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

                    if (flagState[IS_NULL_MASK])
                    {
                        // If this.IsNullMask, we are setting the mask to a new value; in this case we need to get the text because
                        // the underlying MTP does not have it (used as a property backend only) and pass it to SetMaskedTextProvider
                        // method below to update the provider.

                        text = Text;
                    }
                }

                // Recreate masked text provider since this property is read-only.
                MaskedTextProvider newProvider = new MaskedTextProvider(
                    newMask,
                    maskedTextProvider.Culture,
                    maskedTextProvider.AllowPromptAsInput,
                    maskedTextProvider.PromptChar,
                    maskedTextProvider.PasswordChar,
                    maskedTextProvider.AsciiOnly);

                //text == null when setting to a different mask value or when resetting the mask to null.
                //text != null only when setting the mask from null to some value.
                SetMaskedTextProvider(newProvider, text);
            }
        }

        /// <summary>
        ///  Event to notify when the mask has changed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.MaskedTextBoxMaskChangedDescr))
        ]
        public event EventHandler MaskChanged
        {
            add => Events.AddHandler(EVENT_MASKCHANGED, value);
            remove => Events.RemoveHandler(EVENT_MASKCHANGED, value);
        }

        /// <summary>
        ///  Specifies whether the test string required input positions, as specified by the mask, have
        ///  all been assigned.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool MaskCompleted
        {
            get
            {
                return maskedTextProvider.MaskCompleted;
            }
        }

        /// <summary>
        ///  Specifies whether all inputs (required and optional) have been provided into the mask successfully.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool MaskFull
        {
            get
            {
                return maskedTextProvider.MaskFull;
            }
        }

        /// <summary>
        ///  Returns a copy of the control's internal MaskedTextProvider.  This is useful for user's to provide
        ///  cloning semantics for the control (we don't want to do it) w/o incurring in any perf penalty since
        ///  some of the properties require recreating the underlying provider when they are changed.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public MaskedTextProvider MaskedTextProvider
        {
            get
            {
                return flagState[IS_NULL_MASK] ? null : (MaskedTextProvider)maskedTextProvider.Clone();
            }
        }

        /// <summary>
        ///  Event to notify when an input has been rejected according to the mask.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxMaskInputRejectedDescr))
        ]
        public event MaskInputRejectedEventHandler MaskInputRejected
        {
            add => Events.AddHandler(EVENT_MASKINPUTREJECTED, value);
            remove => Events.RemoveHandler(EVENT_MASKINPUTREJECTED, value);
        }

        /// <summary>
        ///  Unsupported method/property.
        ///  WndProc ignores EM_LIMITTEXT & this is a virtual method.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override int MaxLength
        {
            get { return base.MaxLength; }
            set { }
        }

        /// <summary>
        ///  Unsupported method/property.
        ///  virtual method.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool Multiline
        {
            get { return false; }
            set { }
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new event EventHandler MultilineChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        ///  Specifies the character to be used in the formatted string in place of editable characters, if
        ///  set to any printable character, the text box becomes a password text box, to reset it use the null
        ///  character.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxPasswordCharDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue('\0') // This property is shadowed by MaskedTextBoxDesigner.
        ]
        public char PasswordChar
        {
            get
            {
                // The password char could be the one set in the control or the system password char,
                // in any case the maskedTextProvider has the correct one.
                return maskedTextProvider.PasswordChar;
            }
            set
            {
                if (!MaskedTextProvider.IsValidPasswordChar(value)) // null character accepted (resets value)
                {
                    // Same message as in SR.MaskedTextProviderInvalidCharError.
                    throw new ArgumentException(SR.MaskedTextBoxInvalidCharError);
                }

                if (passwordChar != value)
                {
                    if (value == maskedTextProvider.PromptChar)
                    {
                        // Prompt and password chars must be different.
                        throw new InvalidOperationException(SR.MaskedTextBoxPasswordAndPromptCharError);
                    }

                    passwordChar = value;

                    // UseSystemPasswordChar take precedence over PasswordChar...Let's check.
                    if (!UseSystemPasswordChar)
                    {
                        maskedTextProvider.PasswordChar = value;

                        if (flagState[IS_NULL_MASK])
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
        internal override bool PasswordProtect
        {
            get
            {
                if (maskedTextProvider != null) // could be queried during object construction.
                {
                    return maskedTextProvider.IsPassword;
                }
                return base.PasswordProtect;
            }
        }

        /// <summary>
        ///  Specifies the prompt character to be used in the formatted string for unsupplied characters.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.MaskedTextBoxPromptCharDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        Localizable(true),
        DefaultValue('_')
        ]
        public char PromptChar
        {
            get
            {
                return maskedTextProvider.PromptChar;
            }
            set
            {
                if (!MaskedTextProvider.IsValidInputChar(value))
                {
                    // This message is the same as the one in SR.MaskedTextProviderInvalidCharError.
                    throw new ArgumentException(SR.MaskedTextBoxInvalidCharError);
                }

                if (maskedTextProvider.PromptChar != value)
                {
                    // We need to check maskedTextProvider password char in case it is using the system password.
                    if (value == passwordChar || value == maskedTextProvider.PasswordChar)
                    {
                        // Prompt and password chars must be different.
                        throw new InvalidOperationException(SR.MaskedTextBoxPasswordAndPromptCharError);
                    }

                    // Recreate masked text provider to be consistent with AllowPromptAsInput - current text may have chars with same value as new prompt.
                    MaskedTextProvider newProvider = new MaskedTextProvider(
                        maskedTextProvider.Mask,
                        maskedTextProvider.Culture,
                        maskedTextProvider.AllowPromptAsInput,
                        value,
                        maskedTextProvider.PasswordChar,
                        maskedTextProvider.AsciiOnly);

                    SetMaskedTextProvider(newProvider);
                }
            }
        }

        /// <summary>
        ///  Overwrite base class' property.
        /// </summary>
        public new bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }

            set
            {
                if (ReadOnly != value)
                {
                    // if true, this disables IME in the base class.
                    base.ReadOnly = value;

                    if (!flagState[IS_NULL_MASK])
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxRejectInputOnFirstFailureDescr)),
        DefaultValue(false)
        ]
        public bool RejectInputOnFirstFailure
        {
            get
            {
                return flagState[REJECT_INPUT_ON_FIRST_FAILURE];
            }
            set
            {
                flagState[REJECT_INPUT_ON_FIRST_FAILURE] = value;
            }
        }

        /// <summary>
        ///  Designe time support for resetting the Culture property.
        /// </summary>
        /* No longer needed since Culture has been removed from the property browser - Left here for documentation.
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ResetCulture()
        {
            this.Culture = CultureInfo.CurrentCulture;
        }*/

        /// <summary>
        ///  Specifies whether to reset and skip the current position if editable, when the input character
        ///  has the same value as the prompt.  This property takes precedence over AllowPromptAsInput.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxResetOnPrompt)),
        DefaultValue(true)
        ]
        public bool ResetOnPrompt
        {
            get
            {
                return maskedTextProvider.ResetOnPrompt;
            }
            set
            {
                maskedTextProvider.ResetOnPrompt = value;
            }
        }

        /// <summary>
        ///  Specifies whether to reset and skip the current position if editable, when the input
        ///  is the space character.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxResetOnSpace)),
        DefaultValue(true)
        ]
        public bool ResetOnSpace
        {
            get
            {
                return maskedTextProvider.ResetOnSpace;
            }
            set
            {
                maskedTextProvider.ResetOnSpace = value;
            }
        }

        /// <summary>
        ///  Specifies whether to skip the current position if non-editable and the input character has
        ///  the same value as the literal at that position.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxSkipLiterals)),
        DefaultValue(true)
        ]
        public bool SkipLiterals
        {
            get
            {
                return maskedTextProvider.SkipLiterals;
            }
            set
            {
                maskedTextProvider.SkipLiterals = value;
            }
        }

        /// <summary>
        ///  The currently selected text (if any) in the control.
        /// </summary>
        public override string SelectedText
        {
            get
            {
                if (flagState[IS_NULL_MASK])
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

        internal override void SetSelectedTextInternal(string value, bool clearUndo)
        {
            if (flagState[IS_NULL_MASK])
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
            flagState[IME_COMPLETING] = true;
            ImeNotify(NativeMethods.CPS_COMPLETE);
        }

        /// <summary>
        ///  Notifies the IMM about changes to the status of the IME input context.
        /// </summary>
        private void ImeNotify(int action)
        {
            HandleRef handle = new HandleRef(this, Handle);
            IntPtr inputContext = UnsafeNativeMethods.ImmGetContext(handle);

            if (inputContext != IntPtr.Zero)
            {
                try
                {
                    UnsafeNativeMethods.ImmNotifyIME(new HandleRef(null, inputContext), NativeMethods.NI_COMPOSITIONSTR, action, 0);
                }
                finally
                {
                    UnsafeNativeMethods.ImmReleaseContext(handle, new HandleRef(null, inputContext));
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
                SendMessage(EditMessages.EM_SETPASSWORDCHAR, pwdChar, 0);
                Invalidate();
            }
        }

        /// <summary>
        ///  The value of the Edit control default password char.
        /// </summary>
        private char SystemPasswordChar
        {
            get
            {
                if (MaskedTextBox.systemPwdChar == '\0')
                {
                    // We need to temporarily create an edit control to get the default password character.
                    // We cannot use this control because we would have to reset the native control's password char to use
                    // the defult one so we can get it; this would change the text displayed in the box (even for a short time)
                    // opening a sec hole.

                    TextBox txtBox = new TextBox
                    {
                        UseSystemPasswordChar = true // this forces the creation of the control handle.
                    };

                    MaskedTextBox.systemPwdChar = txtBox.PasswordChar;

                    txtBox.Dispose();
                }

                return MaskedTextBox.systemPwdChar;
            }
        }

        /// <summary>
        ///  The Text setter validates the input char by char, raising the MaskInputRejected event for invalid chars.
        ///  The Text getter returns the formatted text according to the IncludeLiterals and IncludePrompt properties.
        /// </summary>
        [
        Editor("System.Windows.Forms.Design.MaskedTextBoxTextEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        SRCategory(nameof(SR.CatAppearance)),
        RefreshProperties(RefreshProperties.Repaint),
        Bindable(true),
        DefaultValue(""), // This property is shadowed by MaskedTextBoxDesigner.
        Localizable(true)
        ]
        public override string Text
        {
            get
            {
                if (flagState[IS_NULL_MASK] || flagState[QUERY_BASE_TEXT])
                {
                    return base.Text;
                }

                return TextOutput;
            }
            set
            {
                if (flagState[IS_NULL_MASK])
                {
                    base.Text = value;
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    // reset the input text.
                    Delete(Keys.Delete, 0, maskedTextProvider.Length);
                }
                else
                {
                    if (RejectInputOnFirstFailure)
                    {
                        string oldText = TextOutput;
                        if (maskedTextProvider.Set(value, out caretTestPos, out MaskedTextResultHint hint))
                        {
                            //if( hint == MaskedTextResultHint.Success || hint == MaskedTextResultHint.SideEffect )
                            if (TextOutput != oldText)
                            {
                                SetText();
                            }
                            SelectionStart = ++caretTestPos;
                        }
                        else
                        {
                            OnMaskInputRejected(new MaskInputRejectedEventArgs(caretTestPos, hint));
                        }
                    }
                    else
                    {
                        Replace(value, /*startPosition*/ 0, /*selectionLen*/ maskedTextProvider.Length);
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
                if (flagState[IS_NULL_MASK])
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
                Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");
                return maskedTextProvider.ToString();
            }
        }

        /// <summary>
        ///  Gets or sets how text is aligned in the control.
        ///  Note: This code is duplicated in TextBox for simplicity.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        SRDescription(nameof(SR.TextBoxTextAlignDescr))
        ]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                if (textAlign != value)
                {
                    //verify that 'value' is a valid enum type...
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                    }

                    textAlign = value;
                    RecreateHandle();
                    OnTextAlignChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Event to notify the text alignment has changed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))
        ]
        public event EventHandler TextAlignChanged
        {
            add => Events.AddHandler(EVENT_TEXTALIGNCHANGED, value);

            remove => Events.RemoveHandler(EVENT_TEXTALIGNCHANGED, value);
        }

        /// <summary>
        ///  Specifies the formatting options for text output (Whether the mask returned from the Text
        ///  property includes Literals and/or prompt characters).
        ///  When prompt characters are excluded, theyare returned as spaces in the string returned.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxTextMaskFormat)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(MaskFormat.IncludeLiterals)
        ]
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

                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)MaskFormat.ExcludePromptAndLiterals, (int)MaskFormat.IncludePromptAndLiterals))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(MaskFormat));
                }

                // Changing the TextMaskFormat will likely change the 'output' text (Text getter value).  Cache old value to
                // verify it against the new value and raise OnTextChange if needed.
                string oldText = flagState[IS_NULL_MASK] ? null : TextOutput;

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

                if (oldText != null && oldText != TextOutput)
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
            if (flagState[IS_NULL_MASK])
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
        [
        SRCategory(nameof(SR.CatFocus)),
        SRDescription(nameof(SR.MaskedTextBoxTypeValidationCompletedDescr))
        ]
        public event TypeValidationEventHandler TypeValidationCompleted
        {
            add => Events.AddHandler(EVENT_VALIDATIONCOMPLETED, value);
            remove => Events.RemoveHandler(EVENT_VALIDATIONCOMPLETED, value);
        }

        /// <summary>
        ///  Indicates if the text in the edit control should appear as the default password character.
        ///  This property has precedence over the PasswordChar property.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxUseSystemPasswordCharDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(false)
        ]
        public bool UseSystemPasswordChar
        {
            get
            {
                return flagState[USE_SYSTEM_PASSWORD_CHAR];
            }
            set
            {
                if (value != flagState[USE_SYSTEM_PASSWORD_CHAR])
                {
                    if (value)
                    {
                        if (SystemPasswordChar == PromptChar)
                        {
                            // Prompt and password chars must be different.
                            throw new InvalidOperationException(SR.MaskedTextBoxPasswordAndPromptCharError);
                        }

                        maskedTextProvider.PasswordChar = SystemPasswordChar;
                    }
                    else
                    {
                        // this.passwordChar could be '\0', in which case we are resetting the display to show the input char.
                        maskedTextProvider.PasswordChar = passwordChar;
                    }

                    flagState[USE_SYSTEM_PASSWORD_CHAR] = value;

                    if (flagState[IS_NULL_MASK])
                    {
                        SetEditControlPasswordChar(maskedTextProvider.PasswordChar);
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
        [
        Browsable(false),
        DefaultValue(null)
        ]
        public Type ValidatingType
        {
            get
            {
                return validatingType;
            }
            set
            {
                if (validatingType != value)
                {
                    validatingType = value;
                }
            }
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new void ClearUndo()
        {
        }

        /// <summary>
        ///  Creates a handle for this control. This method is called by the framework, this should
        ///  not be called directly. Inheriting classes should always call <c>base.CreateHandle</c> when overriding this method.
        ///  Overridden to be able to set the control text with the masked (passworded) value when recreating
        ///  handle, since the underlying native edit control is not aware of it.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        ]
        protected override void CreateHandle()
        {
            if (!flagState[IS_NULL_MASK] && RecreatingHandle)
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
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");
            Debug.Assert(keyCode == Keys.Delete || keyCode == Keys.Back, "Delete called with keyCode == " + keyCode.ToString());
            Debug.Assert(startPosition >= 0 && ((startPosition + selectionLen) <= maskedTextProvider.Length), "Invalid position range.");

            // On backspace, moving the start postion back by one has the same effect as delete.  If text is selected, there is no
            // need for moving the position back.

            caretTestPos = startPosition;

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
                    if ((startPosition + selectionLen) == maskedTextProvider.Length) // At end of string, delete does nothing.
                    {
                        return;
                    }
                }
            }

            int endPos = selectionLen > 0 ? startPosition + selectionLen - 1 : startPosition;

            string oldText = TextOutput;
            if (maskedTextProvider.RemoveAt(startPosition, endPos, out int tempPos, out MaskedTextResultHint hint))
            {
                //if( hint == MaskedTextResultHint.Success || hint == MaskedTextResultHint.SideEffect) // Text was changed.
                if (TextOutput != oldText)
                {
                    SetText();
                    caretTestPos = startPosition;
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
                        caretTestPos = startPosition;
                    }
                    else
                    {
                        if (hint == MaskedTextResultHint.NoEffect) // Case 2.
                        {
                            if (keyCode == Keys.Delete)
                            {
                                caretTestPos = maskedTextProvider.FindEditPositionFrom(startPosition, forward);
                            }
                            else
                            {
                                if (maskedTextProvider.FindAssignedEditPositionFrom(startPosition, forward) == MaskedTextProvider.InvalidIndex)
                                {
                                    // No assigned position at the right, nothing to shift then move to the next assigned position at the
                                    // left (if any).
                                    caretTestPos = maskedTextProvider.FindAssignedEditPositionFrom(startPosition, backward);
                                }
                                else
                                {
                                    // there are assigned positions at the right so move to an edit position at the left to get ready for
                                    // removing the character on it or just shifting the characters at the right
                                    caretTestPos = maskedTextProvider.FindEditPositionFrom(startPosition, backward);
                                }

                                if (caretTestPos != MaskedTextProvider.InvalidIndex)
                                {
                                    caretTestPos++; // backspace gets ready to remove one position past the edit position.
                                }
                            }

                            if (caretTestPos == MaskedTextProvider.InvalidIndex)
                            {
                                caretTestPos = startPosition;
                            }
                        }
                        else // (hint == MaskedTextProvider.OperationHint.SideEffect)
                        {
                            if (keyCode == Keys.Back)  // Case 3.
                            {
                                caretTestPos = startPosition;
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
            //this.SelectionLength = 0;
            //this.SelectionStart  = this.caretTestPos; // new caret position.
            base.SelectInternal(caretTestPos, 0, maskedTextProvider.Length);

            return;
        }

        /// <summary>
        ///  Returns the character nearest to the given point.
        /// </summary>
        public override char GetCharFromPosition(Point pt)
        {
            char ch;

            flagState[QUERY_BASE_TEXT] = true;
            try
            {
                ch = base.GetCharFromPosition(pt);
            }
            finally
            {
                flagState[QUERY_BASE_TEXT] = false;
            }
            return ch;
        }

        /// <summary>
        ///  Returns the index of the character nearest to the given point.
        /// </summary>
        public override int GetCharIndexFromPosition(Point pt)
        {
            int index;

            flagState[QUERY_BASE_TEXT] = true;
            try
            {
                index = base.GetCharIndexFromPosition(pt);
            }
            finally
            {
                flagState[QUERY_BASE_TEXT] = false;
            }
            return index;
        }

        /// <summary>
        ///  Returns the position of the last input character (or if available, the next edit position).
        ///  This is used by base.AppendText.
        /// </summary>
        internal override int GetEndPosition()
        {
            if (flagState[IS_NULL_MASK])
            {
                return base.GetEndPosition();
            }

            int pos = maskedTextProvider.FindEditPositionFrom(maskedTextProvider.LastAssignedPosition + 1, forward);

            if (pos == MaskedTextProvider.InvalidIndex)
            {
                pos = maskedTextProvider.LastAssignedPosition + 1;
            }

            return pos;
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new int GetFirstCharIndexOfCurrentLine()
        {
            return 0;
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
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
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

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

            return maskedTextProvider.ToString(/*ignorePwdChar */ false, includePrompt, /*includeLiterals*/ true, 0, maskedTextProvider.Length);
        }

        /// <summary>
        ///  Unsupported method/property.
        ///  virtual method.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
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

            flagState[QUERY_BASE_TEXT] = true;
            try
            {
                pos = base.GetPositionFromCharIndex(index);
            }
            finally
            {
                flagState[QUERY_BASE_TEXT] = false;
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

            flagState[QUERY_BASE_TEXT] = true;
            try
            {
                size = base.GetPreferredSizeCore(proposedConstraints);
            }
            finally
            {
                flagState[QUERY_BASE_TEXT] = false;
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
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            base.GetSelectionStartAndLength(out int selStart, out int selLength);

            if (selLength == 0)
            {
                return string.Empty;
            }

            bool includePrompt = (CutCopyMaskFormat & MaskFormat.IncludePrompt) != 0;
            bool includeLiterals = (CutCopyMaskFormat & MaskFormat.IncludeLiterals) != 0;

            return maskedTextProvider.ToString( /*ignorePasswordChar*/ true, includePrompt, includeLiterals, selStart, selLength);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            // Force repainting of the entire window frame
            if (Application.RenderWithVisualStyles && IsHandleCreated && BorderStyle == BorderStyle.Fixed3D)
            {
                SafeNativeMethods.RedrawWindow(new HandleRef(this, Handle), null, NativeMethods.NullHandleRef, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
            }
        }

        /// <summary>
        ///  Overridden to update the newly created handle with the settings of the PasswordChar properties
        ///  if no mask has been set.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            base.SetSelectionOnHandle();

            if (flagState[IS_NULL_MASK] && maskedTextProvider.IsPassword)
            {
                SetEditControlPasswordChar(maskedTextProvider.PasswordChar);
            }
        }

        /// <summary>
        ///  Raises the IsOverwriteModeChanged event.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void OnIsOverwriteModeChanged(EventArgs e)
        {
            if (Events[EVENT_ISOVERWRITEMODECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.KeyDown'/> event.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (flagState[IS_NULL_MASK])
            {
                // Operates as a regular text box base.
                return;
            }

            Keys keyCode = e.KeyCode;

            // Special-case Return & Esc since they generate invalid characters we should not process OnKeyPress.
            if (keyCode == Keys.Return || keyCode == Keys.Escape)
            {
                flagState[HANDLE_KEY_PRESS] = false;
            }

            // Insert is toggled when not modified with some other key (ctrl, shift...).  Note that shift-Insert is
            // same as paste.
            if (keyCode == Keys.Insert && e.Modifiers == Keys.None && insertMode == InsertKeyMode.Default)
            {
                flagState[INSERT_TOGGLED] = !flagState[INSERT_TOGGLED];
                OnIsOverwriteModeChanged(EventArgs.Empty);
                return;
            }

            if (e.Control && char.IsLetter((char)keyCode))
            {
                switch (keyCode)
                {
                    // Unsupported keys should not be handled to allow generatating the corresponding message
                    // which is handled in the WndProc.
                    //case Keys.Z:  // ctrl-z == Undo.
                    //case Keys.Y:  // ctrl-y == Redo.
                    //    e.Handled = true;
                    //    return;

                    // Note: Ctrl-Insert (Copy -Shortcut.CtrlIns) and Shft-Insert (Paste - Shortcut.ShiftIns) are
                    // handled by the base class and behavior depend on ShortcutsEnabled property.

                    // Special cases: usually cases where the native edit control would modify the mask.
                    case Keys.H:  // ctrl-h == Backspace == '\b'
                        keyCode = Keys.Back; // handle it below.
                        break;

                    default:
                        // Next OnKeyPress should not be handled to allow Ctrl-<x/c/v/a> to be processed in the
                        // base class so corresponding messages can be generated (WM_CUT/WM_COPY/WM_PASTE).
                        // Combined characters don't generate OnKeyDown by themselves but they generate OnKeyPress.
                        flagState[HANDLE_KEY_PRESS] = false;
                        return;
                }
            }

            if (keyCode == Keys.Delete || keyCode == Keys.Back) // Deletion keys.
            {
                if (!ReadOnly)
                {

                    base.GetSelectionStartAndLength(out int startPosition, out int selectionLen);

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
                                    selectionLen = maskedTextProvider.Length - startPosition;
                                }
                                else // ( keyCode == Keys.Back ) // delete to the beginning of the string.
                                {
                                    selectionLen = startPosition == maskedTextProvider.Length /*at end of text*/ ? startPosition : startPosition + 1;
                                    startPosition = 0;
                                }
                            }
                            goto default;

                        default:
                            if (!flagState[HANDLE_KEY_PRESS])
                            {
                                flagState[HANDLE_KEY_PRESS] = true;
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
        ///  Raises the <see cref='Control.KeyPress'/> event.
        /// </summary>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (flagState[IS_NULL_MASK])
            {
                // Operates as a regular text box base.
                return;
            }

            // This key may be a combined key involving a letter, like Ctrl-A; let the native control handle it.
            if (!flagState[HANDLE_KEY_PRESS])
            {
                flagState[HANDLE_KEY_PRESS] = true;

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

                base.GetSelectionStartAndLength(out int selectionStart, out int selectionLen);

                string oldText = TextOutput;
                if (PlaceChar(e.KeyChar, selectionStart, selectionLen, IsOverwriteMode, out MaskedTextResultHint hint))
                {
                    //if( hint == MaskedTextResultHint.Success || hint == MaskedTextResultHint.SideEffect )
                    if (TextOutput != oldText)
                    {
                        SetText(); // Now set the text in the display.
                    }

                    SelectionStart = ++caretTestPos; // caretTestPos is updated in PlaceChar.

                    if (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable)
                    {
                        // Korean IMEs complete composition when a character has been fully converted, so the composition string
                        // is only one-character long; once composed we block the IME if there ins't more room in the test string.

                        int editPos = maskedTextProvider.FindUnassignedEditPositionFrom(caretTestPos, forward);
                        if (editPos == MaskedTextProvider.InvalidIndex)
                        {
                            ImeComplete();  // Force completion of compostion.
                        }
                    }
                }
                else
                {
                    OnMaskInputRejected(new MaskInputRejectedEventArgs(caretTestPos, hint)); // caretTestPos is updated in PlaceChar.
                }

                if (selectionLen > 0)
                {
                    SelectionLength = 0;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.KeyUp'/> event.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            // KeyUp is the last message to be processed so it is the best place to reset these flags.

            if (flagState[IME_COMPLETING])
            {
                flagState[IME_COMPLETING] = false;
            }

            if (flagState[IME_ENDING_COMPOSITION])
            {
                flagState[IME_ENDING_COMPOSITION] = false;
            }
        }

        /// <summary>
        ///  Raises the MaskChanged event.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void OnMaskChanged(EventArgs e)
        {
            if (Events[EVENT_MASKCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the MaskInputRejected event.
        /// </summary>
        private void OnMaskInputRejected(MaskInputRejectedEventArgs e)
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            if (BeepOnError)
            {
                Media.SoundPlayer sp = new Media.SoundPlayer();
                sp.Play();
            }

            if (Events[EVENT_MASKINPUTREJECTED] is MaskInputRejectedEventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Unsupported method/property.
        ///  virtual method.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        protected override void OnMultilineChanged(EventArgs e)
        {
        }

        /// <summary>
        ///  Raises the TextAlignChanged event.
        /// </summary>
        protected virtual void OnTextAlignChanged(EventArgs e)
        {
            if (Events[EVENT_TEXTALIGNCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the TypeValidationCompleted event.
        /// </summary>
        private void OnTypeValidationCompleted(TypeValidationEventArgs e)
        {
            if (Events[EVENT_VALIDATIONCOMPLETED] is TypeValidationEventHandler eh)
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
            bool queryBaseText = flagState[QUERY_BASE_TEXT];
            flagState[QUERY_BASE_TEXT] = false;
            try
            {
                base.OnTextChanged(e);
            }
            finally
            {
                flagState[QUERY_BASE_TEXT] = queryBaseText;
            }
        }
        /// <summary>
        ///  Replaces the current selection in the text box specified by the startPosition and selectionLen parameters
        ///  with the contents of the supplied string.
        /// </summary>
        private void Replace(string text, int startPosition, int selectionLen)
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");
            Debug.Assert(text != null, "text is null.");

            // Clone the MaskedTextProvider so text properties are not modified until the paste operation is
            // completed.  This is needed in case one of these properties is retreived in a MaskedInputRejected
            // event handler (clipboard text is attempted to be set into the input text char by char).

            MaskedTextProvider clonedProvider = (MaskedTextProvider)maskedTextProvider.Clone();

            // Cache the current caret position so we restore it in case the text does not change.
            int currentCaretPos = caretTestPos;

            // First replace characters in the selection (if any and if any edit positions) until completed, or the test position falls
            // outside the selection range, or there's no more room in the test string for editable characters.
            // Then insert any remaining characters from the input.

            MaskedTextResultHint hint = MaskedTextResultHint.NoEffect;
            int endPos = startPosition + selectionLen - 1;

            if (RejectInputOnFirstFailure)
            {
                bool succeeded;

                succeeded = (startPosition > endPos) ?
                    clonedProvider.InsertAt(text, startPosition, out caretTestPos, out hint) :
                    clonedProvider.Replace(text, startPosition, endPos, out caretTestPos, out hint);

                if (!succeeded)
                {
                    OnMaskInputRejected(new MaskInputRejectedEventArgs(caretTestPos, hint));
                }
            }
            else
            {
                // temp hint used to preserve the 'primary' operation hint (no side effects).
                MaskedTextResultHint tempHint = hint;
                int testPos;

                foreach (char ch in text)
                {
                    if (!maskedTextProvider.VerifyEscapeChar(ch, startPosition))  // char won't be escaped, find and edit position for it.
                    {
                        // Observe that we look for a position w/o respecting the selection length, because the input text could be larger than
                        // the number of edit positions in the selection.
                        testPos = clonedProvider.FindEditPositionFrom(startPosition, forward);

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
                        startPosition = caretTestPos + 1;

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
                        if (!clonedProvider.RemoveAt(startPosition, endPos, out caretTestPos, out tempHint))
                        {
                            OnMaskInputRejected(new MaskInputRejectedEventArgs(caretTestPos, tempHint));
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
            maskedTextProvider = clonedProvider;

            // Update text if needed.
            if (updateText)
            {
                SetText();

                // Update caret position.
                caretTestPos = startPosition;
                base.SelectInternal(caretTestPos, 0, maskedTextProvider.Length);
            }
            else
            {
                caretTestPos = currentCaretPos;
            }

            return;
        }

        /// <summary>
        ///  Pastes specified text over the currently selected text (if any) shifting upper characters if
        ///  input is longer than selected text, and/or removing remaining characters from the selection if
        ///  input contains less characters.
        /// </summary>
        private void PasteInt(string text)
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            base.GetSelectionStartAndLength(out int selStart, out int selLength);

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
        private object PerformTypeValidation(CancelEventArgs e)
        {
            object parseRetVal = null;

            if (validatingType != null)
            {
                string message = null;

                if (!flagState[IS_NULL_MASK] && maskedTextProvider.MaskCompleted == false)
                {
                    message = SR.MaskedTextBoxIncompleteMsg;
                }
                else
                {
                    string textValue;

                    if (!flagState[IS_NULL_MASK]) // replace prompt with space.
                    {
                        textValue = maskedTextProvider.ToString(/*includePrompt*/ false, IncludeLiterals);
                    }
                    else
                    {
                        textValue = base.Text;
                    }

                    try
                    {
                        parseRetVal = Formatter.ParseObject(
                            textValue,              // data
                            validatingType,    // targetType
                            typeof(string),         // sourceType
                            null,                   // targetConverter
                            null,                   // sourceConverter
                            formatProvider,    // formatInfo
                            null,                   // nullValue
                            Formatter.GetDefaultDataSourceNullValue(validatingType));   // dataSourceNullValue
                    }
                    catch (Exception exception)
                    {
                        if (ClientUtils.IsSecurityOrCriticalException(exception))
                        {
                            throw;
                        }

                        if (exception.InnerException != null) // Outer exception is a generic TargetInvocationException.
                        {
                            exception = exception.InnerException;
                        }

                        message = exception.GetType().ToString() + ": " + exception.Message;
                    }
                }

                bool isValidInput = false;
                if (message == null)
                {
                    isValidInput = true;
                    message = SR.MaskedTextBoxTypeValidationSucceeded;
                }

                TypeValidationEventArgs tve = new TypeValidationEventArgs(validatingType, isValidInput, parseRetVal, message);
                OnTypeValidationCompleted(tve);

                if (e != null)
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
            return PlaceChar(maskedTextProvider, ch, startPosition, length, overwrite, out hint);
        }

        /// <summary>
        ///  Override version to be able to perform the operation on a cloned provider.
        /// </summary>
        private bool PlaceChar(MaskedTextProvider provider, char ch, int startPosition, int length, bool overwrite,
            out MaskedTextResultHint hint)
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            caretTestPos = startPosition;

            if (startPosition < maskedTextProvider.Length)
            {
                if (length > 0)  // Replacing selection with input char.
                {
                    int endPos = startPosition + length - 1;
                    return provider.Replace(ch, startPosition, endPos, out caretTestPos, out hint);
                }
                else
                {
                    if (overwrite)
                    {
                        // overwrite character at next edit position from startPosition (inclusive).
                        return provider.Replace(ch, startPosition, out caretTestPos, out hint);
                    }
                    else // insert.
                    {
                        return provider.InsertAt(ch, startPosition, out caretTestPos, out hint);
                    }
                }
            }

            hint = MaskedTextResultHint.UnavailableEditPosition;
            return false;
        }

        /// <summary>
        ///  <From Control.cs>:
        ///  Processes a command key. This method is called during message
        ///  pre-processing to handle command keys. Command keys are keys that always
        ///  take precedence over regular input keys. Examples of command keys
        ///  include accelerators and menu shortcuts. The method must return true to
        ///  indicate that it has processed the command key, or false to indicate
        ///  that the key is not a command key.
        ///
        ///  processCmdKey() first checks if the control has a context menu, and if
        ///  so calls the menu's processCmdKey() to check for menu shortcuts. If the
        ///  command key isn't a menu shortcut, and if the control has a parent, the
        ///  key is passed to the parent's processCmdKey() method. The net effect is
        ///  that command keys are "bubbled" up the control hierarchy.
        ///
        ///  When overriding processCmdKey(), a control should return true to
        ///  indicate that it has processed the key. For keys that aren't processed by
        ///  the control, the result of "base.processCmdKey()" should be returned.
        ///
        ///  Controls will seldom, if ever, need to override this method.
        ///  </From Control.cs>
        ///
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
                    base.SelectAll();
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

            if (flagState[IS_NULL_MASK])
            {
                return msgProcessed; // Operates as a regular text box base.
            }

            // If this WM_CHAR message is sent after WM_IME_CHAR, we ignore it since we already processed
            // the corresponding WM_IME_CHAR message.

            if (m.Msg == WindowMessages.WM_CHAR && base.ImeWmCharsToIgnore > 0)
            {
                return true;    // meaning, we handled the message so it is not passed to the default WndProc.
            }

            return msgProcessed;
        }

        /// <summary>
        ///  Designe time support for resetting Culture property..
        /// </summary>
        private void ResetCulture()
        {
            Culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        ///  Unsupported method/property.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
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
        private void SetMaskedTextProvider(MaskedTextProvider newProvider, string textOnInitializingMask)
        {
            Debug.Assert(newProvider != null, "Initializing from a null MaskProvider ref.");

            // Set R/W properties.
            newProvider.IncludePrompt = maskedTextProvider.IncludePrompt;
            newProvider.IncludeLiterals = maskedTextProvider.IncludeLiterals;
            newProvider.SkipLiterals = maskedTextProvider.SkipLiterals;
            newProvider.ResetOnPrompt = maskedTextProvider.ResetOnPrompt;
            newProvider.ResetOnSpace = maskedTextProvider.ResetOnSpace;

            // If mask not initialized and not initializing it, the new provider is just a property backend.
            // Change won't have any effect in text.
            if (flagState[IS_NULL_MASK] && textOnInitializingMask == null)
            {
                maskedTextProvider = newProvider;
                return;
            }

            int testPos = 0;
            bool raiseOnMaskInputRejected = false; // Raise if new provider rejects old text.
            MaskedTextResultHint hint = MaskedTextResultHint.NoEffect;
            MaskedTextProvider oldProvider = maskedTextProvider;

            // Attempt to add previous text.
            // If the mask is the same, we need to preserve the caret and character positions if the text is added successfully.
            bool preserveCharPos = oldProvider.Mask == newProvider.Mask;

            // Cache text output text before setting the new provider to determine whether we need to raise the TextChanged event.
            string oldText;

            // NOTE: Whenever changing the MTP, the text is lost if any character in the old text violates the new provider's mask.

            if (textOnInitializingMask != null) // Changing Mask (from null), which is the only RO property that requires passing text.
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
                    srcPos = oldProvider.FindAssignedEditPositionFrom(srcPos, forward);
                    Debug.Assert(srcPos != MaskedTextProvider.InvalidIndex, "InvalidIndex unexpected at this time.");

                    if (preserveCharPos)
                    {
                        dstPos = srcPos;
                    }
                    else
                    {
                        dstPos = newProvider.FindEditPositionFrom(dstPos, forward);

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
            maskedTextProvider = newProvider;

            if (flagState[IS_NULL_MASK])
            {
                flagState[IS_NULL_MASK] = false;
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

            if (textOnInitializingMask != null /*changing mask from null*/ || oldProvider.Mask != newProvider.Mask)
            {
                OnMaskChanged(e);
            }

            SetWindowText(GetFormattedDisplayString(), oldText != TextOutput, preserveCharPos);
        }

        /// <summary>
        ///  Sets the control's text to the formatted text obtained from the underlying MaskedTextProvider.
        ///  TextChanged is raised always, this assumes the display or the output text changed.
        ///  The caret position is lost (unless cached somewhere else like when lossing the focus).
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
            flagState[QUERY_BASE_TEXT] = true;

            try
            {
                if (preserveCaret)
                {
                    caretTestPos = SelectionStart;
                }

                WindowText = text;  // this calls Win32::SetWindowText directly, no OnTextChanged raised.

                if (raiseTextChangedEvent)
                {
                    OnTextChanged(EventArgs.Empty);
                }

                if (preserveCaret)
                {
                    SelectionStart = caretTestPos;
                }
            }
            finally
            {
                flagState[QUERY_BASE_TEXT] = false;
            }
        }

        /// <summary>
        ///  Designe time support for checking if Culture value in the designer should be serialized.
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
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new void Undo()
        {
        }

        /// <summary>
        ///  Forces type validation.  Returns the validated text value.
        /// </summary>
        public object ValidateText()
        {
            return PerformTypeValidation(null);
        }

        /// <summary>
        ///  Deletes all input characters in the current selection.
        /// </summary>
        private bool WmClear()
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            if (!ReadOnly)
            {
                base.GetSelectionStartAndLength(out int selStart, out int selLength);
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
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            if (maskedTextProvider.IsPassword) // cannot copy password to clipboard.
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
            catch (Exception ex)
            {
                // Note: Sometimes the above operation throws but it successfully sets the
                // data in the clipboard. This usually happens when the Application's Main
                // is not attributed with [STAThread].
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
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
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

#if DEBUG
            if (ReadOnly || maskedTextProvider.IsPassword)
            {
                // This should have been already handled by the ReadOnly, PasswordChar and ImeMode properties.
                Debug.Assert(ImeMode == ImeMode.Disable, "IME enabled when in RO or Pwd mode.");
            }
#endif
            // Non-Korean IMEs complete compositon when all characters in the string has been composed (when user hits enter);
            // Currently, we don't support checking the composition string characters because it would require similar logic
            // as the MaskedTextBox itself.

            if (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable)
            {
                byte imeConvertionType = imeConvertionNone;

                // Check if there's an update to the compositon string:
                if ((m.LParam.ToInt32() & NativeMethods.GCS_COMPSTR) != 0)
                {
                    // The character in the composition has been updated but not yet converted.
                    imeConvertionType = imeConvertionUpdate;
                }
                else if ((m.LParam.ToInt32() & NativeMethods.GCS_RESULTSTR) != 0)
                {
                    // The character(s) in the composition has been fully converted.
                    imeConvertionType = imeConvertionCompleted;
                }

                // Process any update in the composition string.
                if (imeConvertionType != imeConvertionNone)
                {
                    if (flagState[IME_ENDING_COMPOSITION])
                    {
                        // If IME is completing the convertion, we don't want to process further characters.
                        return flagState[IME_COMPLETING];
                    }
                }
            }

            return false; //message not handled.
        }

        /// <summary>
        ///  Processes the WM_IME_STARTCOMPOSITION message.
        ///  Returns true if the message is handled, false otherwise.
        /// </summary>
        private bool WmImeStartComposition()
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            // Position the composition window in a valid place.

            base.GetSelectionStartAndLength(out int startPosition, out int selectionLen);

            int startEditPos = maskedTextProvider.FindEditPositionFrom(startPosition, forward);

            if (startEditPos != MaskedTextProvider.InvalidIndex)
            {
                if (selectionLen > 0 && (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable))
                {
                    // Korean IME: We need to delete the selected text and reposition the caret so the IME processes one
                    // character only, otherwise it would overwrite the selection with the caret (composition string),
                    // deleting a portion of the mask.

                    int endEditPos = maskedTextProvider.FindEditPositionFrom(startPosition + selectionLen - 1, backward);

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
                    caretTestPos = startEditPos;
                    SelectionStart = caretTestPos;
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
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

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
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
                Debug.Fail(ex.ToString());
                return;
            }

            PasteInt(text);
        }

        private void WmPrint(ref Message m)
        {
            base.WndProc(ref m);
            if ((NativeMethods.PRF_NONCLIENT & unchecked((int)(long)m.LParam)) != 0 && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
            {
                using (Graphics g = Graphics.FromHdc(m.WParam))
                {
                    Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
                    using (Pen pen = new Pen(VisualStyleInformation.TextControlBorder))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(SystemPens.Window, rect);
                }
            }
        }

        /// <summary>
        ///  We need to override the WndProc method to have full control over what characters can be
        ///  displayed in the text box; particularly, we have special handling when IME is turned on.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            // Handle messages for special cases (unsupported operations or cases where mask doesn not matter).
            switch (m.Msg)
            {
                case WindowMessages.WM_PRINT:
                    WmPrint(ref m);
                    return;
                case WindowMessages.WM_CONTEXTMENU:
                case EditMessages.EM_CANUNDO:
                    base.ClearUndo(); // resets undo buffer.
                    base.WndProc(ref m);
                    return;

                case EditMessages.EM_SCROLLCARET:  // No scroll for single-line control.
                case EditMessages.EM_LIMITTEXT:    // Max/Min text is defined by the mask.
                case EditMessages.EM_UNDO:
                case WindowMessages.WM_UNDO:
                    return;

                default:
                    break;  // continue.
            }

            if (flagState[IS_NULL_MASK])
            {
                base.WndProc(ref m); // Operates as a regular text box base.
                return;
            }

            switch (m.Msg)
            {
                case WindowMessages.WM_IME_STARTCOMPOSITION:
                    if (WmImeStartComposition())
                    {
                        break;
                    }
                    goto default;

                case WindowMessages.WM_IME_ENDCOMPOSITION:
                    flagState[IME_ENDING_COMPOSITION] = true;
                    goto default;

                case WindowMessages.WM_IME_COMPOSITION:
                    if (WmImeComposition(ref m))
                    {
                        break;
                    }
                    goto default;

                case WindowMessages.WM_CUT:
                    if (!ReadOnly && WmCopy())
                    {
                        WmClear();
                    }
                    break;

                case WindowMessages.WM_COPY:
                    WmCopy();
                    break;

                case WindowMessages.WM_PASTE:
                    WmPaste();
                    break;

                case WindowMessages.WM_CLEAR:
                    WmClear();
                    break;

                case WindowMessages.WM_KILLFOCUS:
                    base.WndProc(ref m);
                    WmKillFocus();
                    break;

                case WindowMessages.WM_SETFOCUS:
                    WmSetFocus();
                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Processes the WM_KILLFOCUS message. Updates control's text replacing promp chars with space.
        /// </summary>
        private void WmKillFocus()
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            base.GetSelectionStartAndLength(out caretTestPos, out lastSelLength);

            if (HidePromptOnLeave && !MaskFull)
            {
                SetWindowText(); // Update text w/ no prompt.

                // We need to update selection info in case the control is queried for it while it doesn't have the focus.
                base.SelectInternal(caretTestPos, lastSelLength, maskedTextProvider.Length);
            }
        }

        /// <summary>
        ///  Processes the WM_SETFOCUS message. Updates control's text with formatted text according to
        ///  the include prompt property.
        /// </summary>
        private void WmSetFocus()
        {
            Debug.Assert(!flagState[IS_NULL_MASK], "This method must be called when a Mask is provided.");

            if (HidePromptOnLeave && !MaskFull) // Prompt will show up.
            {
                SetWindowText();
            }

            // Restore previous selection. Do this always (as opposed to within the condition above as in WmKillFocus)
            // because HidePromptOnLeave could have changed while the control did not have the focus.
            base.SelectInternal(caretTestPos, lastSelLength, maskedTextProvider.Length);
        }
    }
}

