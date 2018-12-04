// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Windows.Forms.Layout;
    using System.Windows.Forms.VisualStyles;

    /// <devdoc>
    ///     MaskedTextBox control definition class.  
    ///     Uses the services from the System.ComponentModel.MaskedTextBoxProvider class.
    ///     See spec at http://dotnetclient/whidbey/Specs/MaskEdit.doc
    /// </devdoc>
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

        private const bool   forward         = true;
        private const bool   backward        = false;
        private const string nullMask        = "<>"; // any char/str is OK here.

        private static readonly object EVENT_MASKINPUTREJECTED      = new object();
        private static readonly object EVENT_VALIDATIONCOMPLETED    = new object();
        private static readonly object EVENT_TEXTALIGNCHANGED       = new object();
        private static readonly object EVENT_ISOVERWRITEMODECHANGED = new object();
        private static readonly object EVENT_MASKCHANGED            = new object();

        // The native edit control's default password char (per thread). See corresponding property for more info.
        private static char systemPwdChar;

        // Values to track changes in IME composition string (if any).  Having const variables is a bit more efficient
        // than having an enum (which creates a class).
        private const byte imeConvertionNone      = 0;  // no convertion has been performed in the composition string.
        private const byte imeConvertionUpdate    = 1;  // the char being composed has been updated but not coverted yet.
        private const byte imeConvertionCompleted = 2;  // the char being composed has been fully converted.

        ///////// Instance fields

        // Used for keeping selection when prompt is hidden on leave (text changes).
        private int lastSelLength;

        // Used for caret positioning.
        private int caretTestPos;

        // Bit mask - Determines when the Korean IME composition string is completed so converted character can be processed.
        private static int IME_ENDING_COMPOSITION = BitVector32.CreateMask();

        // Bit mask - Determines when the Korean IME is completing a composition, used when forcing convertion.
        private static int IME_COMPLETING = BitVector32.CreateMask(IME_ENDING_COMPOSITION);
        
        // Used for handling characters that have a modifier (Ctrl-A, Shift-Del...).
        private static int HANDLE_KEY_PRESS = BitVector32.CreateMask(IME_COMPLETING);

        // Bit mask - Used to simulate a null mask.  Needed since a MaskedTextProvider object cannot be 
        // initialized with a null mask but we need one even in this case as a backend for 
        // default properties.  This is to support creating a MaskedTextBox with the default 
        // constructor, specially at design time.
        private static int IS_NULL_MASK = BitVector32.CreateMask(HANDLE_KEY_PRESS);

        // Bit mask - Used in conjuction with get_Text to return the text that is actually set in the native
        // control.  This is required to be able to measure text correctly (GetPreferredSize) and
        // to compare against during set_Text (to bail if the same and not to raise TextChanged event).
        private static int QUERY_BASE_TEXT = BitVector32.CreateMask(IS_NULL_MASK);

        // If true, the input text is rejected whenever a character does not comply with the mask; a MaskInputRejected
        // event is fired for the failing character.  
        // If false, characters in the input string are processed one by one accepting the ones that comply
        // with the mask and raising the MaskInputRejected event for the rejected ones.
        private static int REJECT_INPUT_ON_FIRST_FAILURE = BitVector32.CreateMask( QUERY_BASE_TEXT );

        // Bit masks for boolean properties.
        private static int HIDE_PROMPT_ON_LEAVE     = BitVector32.CreateMask(REJECT_INPUT_ON_FIRST_FAILURE);
        private static int BEEP_ON_ERROR            = BitVector32.CreateMask(HIDE_PROMPT_ON_LEAVE);
        private static int USE_SYSTEM_PASSWORD_CHAR = BitVector32.CreateMask(BEEP_ON_ERROR);
        private static int INSERT_TOGGLED           = BitVector32.CreateMask(USE_SYSTEM_PASSWORD_CHAR);
        private static int CUTCOPYINCLUDEPROMPT     = BitVector32.CreateMask(INSERT_TOGGLED);
        private static int CUTCOPYINCLUDELITERALS   = BitVector32.CreateMask(CUTCOPYINCLUDEPROMPT);

        ///////// Properties backend fields. See corresponding property comments for more info.
        
        private char                    passwordChar; // control's pwd char, it could be different from the one displayed if using system password.
        private Type                    validatingType;
        private IFormatProvider         formatProvider;
        private MaskedTextProvider      maskedTextProvider;
        private InsertKeyMode           insertMode;
        private HorizontalAlignment     textAlign;

        // Bit vector to represent bool variables.
        private BitVector32 flagState;

        /// <devdoc>
        ///     Constructs the MaskedTextBox with the specified MaskedTextProvider object.
        /// </devdoc>
        public MaskedTextBox()
        {
            MaskedTextProvider maskedTextProvider = new MaskedTextProvider(nullMask, CultureInfo.CurrentCulture);
            this.flagState[IS_NULL_MASK] = true;
            Initialize(maskedTextProvider);
        }

        /// <devdoc>
        ///     Constructs the MaskedTextBox with the specified MaskedTextProvider object.
        /// </devdoc>
        public MaskedTextBox(string mask)
        {
            if (mask == null)
            {
                throw new ArgumentNullException();
            }

            MaskedTextProvider maskedTextProvider = new MaskedTextProvider(mask, CultureInfo.CurrentCulture);
            this.flagState[IS_NULL_MASK] = false;
            Initialize(maskedTextProvider);
        }

        /// <devdoc>
        ///     Constructs the MaskedTextBox with the specified MaskedTextProvider object.
        /// </devdoc>
        public MaskedTextBox(MaskedTextProvider maskedTextProvider)
        {
            if (maskedTextProvider == null)
            {
                throw new ArgumentNullException();
            }

            this.flagState[IS_NULL_MASK] = false;
            Initialize(maskedTextProvider);
        }

        /// <devdoc>
        ///     Initializes the object with the specified MaskedTextProvider object and default
        ///     property values.
        /// </devdoc>
        private void Initialize(MaskedTextProvider maskedTextProvider)
        {
            Debug.Assert(maskedTextProvider != null, "Initializing from a null MaskProvider ref.");

            this.maskedTextProvider = maskedTextProvider;

            // set the initial display text.
            if (!this.flagState[IS_NULL_MASK])
            {
                SetWindowText();
            }

            // set default values.
            this.passwordChar = this.maskedTextProvider.PasswordChar;
            this.insertMode   = InsertKeyMode.Default;

            this.flagState[HIDE_PROMPT_ON_LEAVE         ] = false;
            this.flagState[BEEP_ON_ERROR                ] = false;
            this.flagState[USE_SYSTEM_PASSWORD_CHAR     ] = false;
            this.flagState[REJECT_INPUT_ON_FIRST_FAILURE] = false;

            // CutCopyMaskFormat - set same defaults as TextMaskFormat (IncludePromptAndLiterals).
            // It is a lot easier to handle this flags individually since that's the way the MaskedTextProvider does it.
            this.flagState[CUTCOPYINCLUDEPROMPT         ] = this.maskedTextProvider.IncludePrompt;
            this.flagState[CUTCOPYINCLUDELITERALS       ] = this.maskedTextProvider.IncludeLiterals;

            // fields for internal use.
            this.flagState[HANDLE_KEY_PRESS] = true;
            this.caretTestPos           = 0; 
        }


        /////////////////// Properties
        ///
   
        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool AcceptsTab 
        {
            get { return false; }
            set {}
        }

        /// <devdoc>
        ///     Specifies whether the prompt character should be treated as a valid input character or not.
        ///     The setter resets the underlying MaskedTextProvider object and attempts
        ///     to add the existing input text (if any) using the new mask, failure is ignored.
        ///     This property has no particular effect if no mask has been set.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxAllowPromptAsInputDescr)), 
        DefaultValue(true)
        ]
        public bool AllowPromptAsInput
        {
            get
            {
                return this.maskedTextProvider.AllowPromptAsInput;
            }
            set
            {
                if( value != this.maskedTextProvider.AllowPromptAsInput )
                {
                    // Recreate masked text provider since this property is read-only.
                    MaskedTextProvider newProvider = new MaskedTextProvider( 
                        this.maskedTextProvider.Mask, 
                        this.maskedTextProvider.Culture, 
                        value, 
                        this.maskedTextProvider.PromptChar,  
                        this.maskedTextProvider.PasswordChar, 
                        this.maskedTextProvider.AsciiOnly );

                    SetMaskedTextProvider( newProvider );
                }
            }
        }
     
        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
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

        /// <devdoc>
        ///     Specifies whether only ASCII characters are accepted as valid input.
        ///     This property has no particular effect if no mask has been set.
        /// </devdoc>
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
                return this.maskedTextProvider.AsciiOnly;
            }

            set
            {
                if( value != this.maskedTextProvider.AsciiOnly )
                {
                    // Recreate masked text provider since this property is read-only.
                    MaskedTextProvider newProvider = new MaskedTextProvider( 
                        this.maskedTextProvider.Mask, 
                        this.maskedTextProvider.Culture, 
                        this.maskedTextProvider.AllowPromptAsInput, 
                        this.maskedTextProvider.PromptChar,  
                        this.maskedTextProvider.PasswordChar, 
                        value );

                    SetMaskedTextProvider( newProvider );
                }
            }
        }

        /// <devdoc>
        ///     Specifies whether to play a beep when the input is not valid according to the mask.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxBeepOnErrorDescr)), 
        DefaultValue(false)
        ]
        public bool BeepOnError
        {
            get 
            {
                return this.flagState[BEEP_ON_ERROR];
            }
            set 
            {
                this.flagState[BEEP_ON_ERROR] = value;
            }
        }

        /// <devdoc>
        ///       Gets a value indicating whether the user can undo the previous operation in a text box control.
        ///       Unsupported method/property.
        ///       WndProc ignores EM_CANUNDO.
        /// </devdoc>
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

        /// <devdoc>
        ///     Returns the parameters needed to create the handle. Inheriting classes
        ///     can override this to provide extra functionality. They should not,
        ///     however, forget to call base.getCreateParams() first to get the struct
        ///     filled up with the basic info.
        /// </devdoc>
        protected override CreateParams CreateParams 
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
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

        /// <devdoc>
        ///     The culture that determines the value of the localizable mask language separators and placeholders.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.MaskedTextBoxCultureDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        ]
        public CultureInfo Culture
        {
            get
            {
                return this.maskedTextProvider.Culture;
            }

            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException();
                }
                
                if( !this.maskedTextProvider.Culture.Equals(value) )
                {
                    // Recreate masked text provider since this property is read-only.
                    MaskedTextProvider newProvider = new MaskedTextProvider( 
                        this.maskedTextProvider.Mask, 
                        value, 
                        this.maskedTextProvider.AllowPromptAsInput, 
                        this.maskedTextProvider.PromptChar,  
                        this.maskedTextProvider.PasswordChar, 
                        this.maskedTextProvider.AsciiOnly );

                    SetMaskedTextProvider( newProvider );
                }
            }
        }

        /// <devdoc>
        ///    Specifies the formatting options for text cut/copited to the clipboard (Whether the mask returned from the Text 
        ///    property includes Literals and/or prompt characters).  
        ///    When prompt characters are excluded, theyare returned as spaces in the string returned.
        /// </devdoc>
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
                if( this.flagState[CUTCOPYINCLUDEPROMPT] )
                {
                    if( this.flagState[CUTCOPYINCLUDELITERALS] )
                    {
                        return MaskFormat.IncludePromptAndLiterals;
                    }

                    return MaskFormat.IncludePrompt;
                }

                if( this.flagState[CUTCOPYINCLUDELITERALS] )
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

                if( value == MaskFormat.IncludePrompt )
                {
                    this.flagState[CUTCOPYINCLUDEPROMPT]   = true;
                    this.flagState[CUTCOPYINCLUDELITERALS] = false;
                }  
                else if( value == MaskFormat.IncludeLiterals )
                {
                    this.flagState[CUTCOPYINCLUDEPROMPT]   = false;
                    this.flagState[CUTCOPYINCLUDELITERALS] = true;
                }  
                else // value == MaskFormat.IncludePromptAndLiterals || value == MaskFormat.ExcludePromptAndLiterals
                {
                    bool include = value == MaskFormat.IncludePromptAndLiterals;
                    this.flagState[CUTCOPYINCLUDEPROMPT]   = include;
                    this.flagState[CUTCOPYINCLUDELITERALS] = include;
                }
            }
        }

        /// <devdoc>
        ///     Specifies the IFormatProvider to be used when parsing the string to the ValidatingType.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public IFormatProvider FormatProvider
        {
            get
            {
                return this.formatProvider;
            }

            set
            {
                this.formatProvider = value;
            }
        }

        /// <devdoc>
        ///     Specifies whether the PromptCharacter is displayed when the control loses focus.
        /// </devdoc>
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
                return this.flagState[HIDE_PROMPT_ON_LEAVE];
            }
            set 
            {
                if( this.flagState[HIDE_PROMPT_ON_LEAVE]  != value )
                {
                    this.flagState[HIDE_PROMPT_ON_LEAVE] = value;
                    
                    // If the control is not focused and there are available edit positions (mask not full) we need to 
                    // update the displayed text.
                    if( !this.flagState[IS_NULL_MASK]&& !this.Focused && !this.MaskFull && !this.DesignMode )
                    {
                        SetWindowText();
                    }
                }
            }
        }

        /// <devdoc>
        ///     Specifies whether to include mask literal characters when formatting the text.
        /// </devdoc>
        private bool IncludeLiterals
        {
            get
            {
                return this.maskedTextProvider.IncludeLiterals;
            }
            set
            {
                this.maskedTextProvider.IncludeLiterals = value;
            }
        }

        /// <devdoc>
        ///     Specifies whether to include the mask prompt character when formatting the text in places
        ///     where an edit char has not being assigned.
        /// </devdoc>
        private bool IncludePrompt
        {
            get
            {
                return this.maskedTextProvider.IncludePrompt;
            }
            set
            {
                this.maskedTextProvider.IncludePrompt = value;
            }
        }

        /// <devdoc>
        ///     Specifies the text insertion mode of the text box.  This can be used to simulated the Access masked text
        ///     control behavior where insertion is set to TextInsertionMode.AlwaysOverwrite
        ///     This property has no particular effect if no mask has been set.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxInsertKeyModeDescr)), 
        DefaultValue(InsertKeyMode.Default)
        ]
        public InsertKeyMode InsertKeyMode
        {
            get
            {
                return this.insertMode;
            }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)InsertKeyMode.Default, (int)InsertKeyMode.Overwrite))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(InsertKeyMode));
                }

                if (this.insertMode != value)
                {
                    bool isOverwrite = this.IsOverwriteMode;
                    this.insertMode  = value;

                    if (isOverwrite != this.IsOverwriteMode)
                    {
                        OnIsOverwriteModeChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <devdoc>
        ///     Overridden to handle unsupported RETURN key.
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) 
        {
            if ((keyData & Keys.KeyCode) == Keys.Return)
            {
                return false;
            }
            return base.IsInputKey(keyData);
        }

        /// <devdoc>
        ///     Specifies whether text insertion mode in 'on' or not.
        /// </devdoc>
        [
        Browsable(false)
        ]
        public bool IsOverwriteMode
        {
            get
            {
                if( this.flagState[IS_NULL_MASK])
                {
                    return false; // EditBox always inserts.
                }
                
                switch (this.insertMode)
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
                        return this.flagState[INSERT_TOGGLED];

                    default:
                        Debug.Fail("Invalid InsertKeyMode.  This code path should have never been executed.");
                        return false;
                }
            }
        }

        
        /// <devdoc>
        ///   Event to notify when the insert mode has changed.  This is required for data binding. 
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.MaskedTextBoxIsOverwriteModeChangedDescr))
        ]
        public event EventHandler IsOverwriteModeChanged
        {
            add
            {
                Events.AddHandler(EVENT_ISOVERWRITEMODECHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_ISOVERWRITEMODECHANGED, value);
            }
        }

        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
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
                
                this.flagState[QUERY_BASE_TEXT] = true;
                try
                {
                    lines = base.Lines;
                }
                finally
                {
                    this.flagState[QUERY_BASE_TEXT] = false;
                }

                return lines; 
            }

            set {}
        }

        /// <devdoc>
        ///     The mask applied to this control.  The setter resets the underlying MaskedTextProvider object and attempts
        ///     to add the existing input text (if any) using the new mask, failure is ignored.
        /// </devdoc>
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
                return this.flagState[IS_NULL_MASK]? string.Empty : this.maskedTextProvider.Mask;
            }
            set
            {
                //
                // We dont' do anything if:
                // 1.  IsNullOrEmpty( value )->[Reset control] && this.flagState[IS_NULL_MASK]==>Already Reset.
                // 2. !IsNullOrEmpty( value )->[Set control] && !this.flagState[IS_NULL_MASK][control is set] && [value is the same]==>No need to update.
                //
                if( this.flagState[IS_NULL_MASK] == string.IsNullOrEmpty( value ) && (this.flagState[IS_NULL_MASK] || value == this.maskedTextProvider.Mask) )
                {
                    return;
                }

                string text    = null;
                string newMask = value;
                
                // We need to update the this.flagState[IS_NULL_MASK]field before raising any events (when setting the maskedTextProvider) so 
                // querying for properties from an event handler returns the right value (i.e: Text).

                if( string.IsNullOrEmpty( value ) ) // Resetting the control, the native edit control will be in charge.
                {
                    // Need to get the formatted & unformatted text before resetting the mask, they'll be used to determine whether we need to
                    // raise the TextChanged event.
                    string formattedText   = TextOutput;
                    string unformattedText = this.maskedTextProvider.ToString(false, false);

                    this.flagState[IS_NULL_MASK] = true;

                    if( this.maskedTextProvider.IsPassword )
                    {
                        SetEditControlPasswordChar(this.maskedTextProvider.PasswordChar);
                    }

                    // Set the window text to the unformatted text before raising events. Also, TextChanged needs to be raised after MaskChanged so
                    // pass false to SetWindowText 'raiseTextChanged' param.
                    SetWindowText(unformattedText, false, false );

                    EventArgs e = EventArgs.Empty;

                    OnMaskChanged(e);

                    if( unformattedText != formattedText )
                    {
                        OnTextChanged(e);
                    }

                    newMask = nullMask;
                }
                else    // Setting control to a new value.
                {
                    foreach( char c in value )
                    {
                        if( !MaskedTextProvider.IsValidMaskChar( c ) )
                        {
                            // Same message as in SR.MaskedTextProviderMaskInvalidChar in System.txt
                            throw new ArgumentException( string.Format( SR.MaskedTextBoxMaskInvalidChar) );
                        }
                    }

                    if( this.flagState[IS_NULL_MASK] )
                    {
                        // If this.IsNullMask, we are setting the mask to a new value; in this case we need to get the text because
                        // the underlying MTP does not have it (used as a property backend only) and pass it to SetMaskedTextProvider
                        // method below to update the provider.

                        text = this.Text;
                    }
                }

                // Recreate masked text provider since this property is read-only.
                MaskedTextProvider newProvider = new MaskedTextProvider( 
                    newMask, 
                    this.maskedTextProvider.Culture, 
                    this.maskedTextProvider.AllowPromptAsInput, 
                    this.maskedTextProvider.PromptChar,  
                    this.maskedTextProvider.PasswordChar, 
                    this.maskedTextProvider.AsciiOnly );

                //text == null when setting to a different mask value or when resetting the mask to null.
                //text != null only when setting the mask from null to some value.
                SetMaskedTextProvider( newProvider, text );
            }
        }

        /// <devdoc>
        ///   Event to notify when the mask has changed.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.MaskedTextBoxMaskChangedDescr))
        ]
        public event EventHandler MaskChanged
        {
            add
            {
                Events.AddHandler(EVENT_MASKCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_MASKCHANGED, value);
            }
        }

        /// <devdoc>
        ///     Specifies whether the test string required input positions, as specified by the mask, have 
        ///     all been assigned.
        /// </devdoc>
        [
        Browsable(false)
        ]
        public bool MaskCompleted
        {
            get 
            { 
                return this.maskedTextProvider.MaskCompleted; 
            }
        }

        /// <devdoc>
        ///     Specifies whether all inputs (required and optional) have been provided into the mask successfully.
        /// </devdoc>
        [
        Browsable(false)
        ]
        public bool MaskFull
        {
            get
            {
                return this.maskedTextProvider.MaskFull;
            }
        }

        /// <devdoc>
        ///     Returns a copy of the control's internal MaskedTextProvider.  This is useful for user's to provide
        ///     cloning semantics for the control (we don't want to do it) w/o incurring in any perf penalty since 
        ///     some of the properties require recreating the underlying provider when they are changed.
        /// </devdoc>
        [
        Browsable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public MaskedTextProvider MaskedTextProvider
        {
            get
            {
                return this.flagState[IS_NULL_MASK] ? null : (MaskedTextProvider) this.maskedTextProvider.Clone();
            }
        }

        /// <devdoc>
        ///     Event to notify when an input has been rejected according to the mask.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxMaskInputRejectedDescr))
        ]
        public event MaskInputRejectedEventHandler MaskInputRejected
        {
            add
            {
                Events.AddHandler(EVENT_MASKINPUTREJECTED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_MASKINPUTREJECTED, value);
            }
        }

        /// <devdoc>
        ///     Unsupported method/property.
        ///     WndProc ignores EM_LIMITTEXT & this is a virtual method.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override int MaxLength
        {
            get{ return base.MaxLength; }
            set{}
        }

        /// <devdoc>
        ///     Unsupported method/property.
        ///     virtual method.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool Multiline
        {
            get { return false; }
            set {}
        }

        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
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

        /// <devdoc>
        ///     Specifies the character to be used in the formatted string in place of editable characters, if
        ///     set to any printable character, the text box becomes a password text box, to reset it use the null
        ///     character.
        /// </devdoc>
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
                return this.maskedTextProvider.PasswordChar;
            }
            set
            {
                if( !MaskedTextProvider.IsValidPasswordChar(value) ) // null character accepted (resets value)
                {
                    // Same message as in SR.MaskedTextProviderInvalidCharError.
                    throw new ArgumentException(SR.MaskedTextBoxInvalidCharError );
                }

                if( this.passwordChar != value )
                {
                    if( value == this.maskedTextProvider.PromptChar )
                    {
                        // Prompt and password chars must be different.
                        throw new InvalidOperationException( SR.MaskedTextBoxPasswordAndPromptCharError );
                    }

                    this.passwordChar = value;

                    // UseSystemPasswordChar take precedence over PasswordChar...Let's check.
                    if (!this.UseSystemPasswordChar)
                    {
                        this.maskedTextProvider.PasswordChar = value;

                        if( this.flagState[IS_NULL_MASK])
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

        /// <devdoc>
        ///     Determines if the control is in password protect mode.
        /// </devdoc>
        internal override bool PasswordProtect 
        {
            get 
            {
                if( this.maskedTextProvider != null ) // could be queried during object construction.
                {
                     return this.maskedTextProvider.IsPassword;
                }
                return base.PasswordProtect;
            }
        }

        /// <devdoc>
        ///     Specifies the prompt character to be used in the formatted string for unsupplied characters.
        /// </devdoc>
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
                return this.maskedTextProvider.PromptChar;
            }
            set
            {
                if( !MaskedTextProvider.IsValidInputChar(value) )
                {
                    // This message is the same as the one in SR.MaskedTextProviderInvalidCharError.
                    throw new ArgumentException(SR.MaskedTextBoxInvalidCharError );
                }

                if( this.maskedTextProvider.PromptChar != value )
                {
                    // We need to check maskedTextProvider password char in case it is using the system password.
                    if( value == this.passwordChar || value == this.maskedTextProvider.PasswordChar )
                    {
                        // Prompt and password chars must be different.
                        throw new InvalidOperationException( SR.MaskedTextBoxPasswordAndPromptCharError );
                    }
                
                    // Recreate masked text provider to be consistent with AllowPromptAsInput - current text may have chars with same value as new prompt.
                    MaskedTextProvider newProvider = new MaskedTextProvider( 
                        this.maskedTextProvider.Mask, 
                        this.maskedTextProvider.Culture, 
                        this.maskedTextProvider.AllowPromptAsInput, 
                        value,  
                        this.maskedTextProvider.PasswordChar, 
                        this.maskedTextProvider.AsciiOnly );

                    SetMaskedTextProvider( newProvider );
                }
            }
        }

        /// <devdoc>
        ///     Overwrite base class' property.
        /// </devdoc>
        public new bool ReadOnly 
        {
            get 
            { 
                return base.ReadOnly; 
            }

            set 
            {
                if (this.ReadOnly != value)
                {
                    // if true, this disables IME in the base class.
                    base.ReadOnly = value;

                    if (!this.flagState[IS_NULL_MASK])
                    {
                        // Prompt will be hidden.
                        SetWindowText();
                    }
                }
            }
        }

        /// <devdoc>
        ///     Specifies whether to include the mask prompt character when formatting the text in places
        ///     where an edit char has not being assigned.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxRejectInputOnFirstFailureDescr)), 
        DefaultValue(false)
        ]
        public bool RejectInputOnFirstFailure
        {
            get
            {
                return this.flagState[REJECT_INPUT_ON_FIRST_FAILURE];
            }
            set
            {
                this.flagState[REJECT_INPUT_ON_FIRST_FAILURE] = value;
            }
        }

        /// <devdoc>
        ///     Designe time support for resetting the Culture property.
        /// </devdoc>
        /* No longer needed since Culture has been removed from the property browser - Left here for documentation.
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ResetCulture()
        {
            this.Culture = CultureInfo.CurrentCulture;
        }*/

              
        /// <devdoc>
        ///     Specifies whether to reset and skip the current position if editable, when the input character
        ///     has the same value as the prompt.  This property takes precedence over AllowPromptAsInput.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxResetOnPrompt)), 
        DefaultValue(true)
        ]
        public bool ResetOnPrompt
        {
            get 
            {
                return this.maskedTextProvider.ResetOnPrompt;
            }
            set 
            {
                this.maskedTextProvider.ResetOnPrompt = value;
            }
        }

        /// <devdoc>
        ///     Specifies whether to reset and skip the current position if editable, when the input 
        ///     is the space character.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxResetOnSpace)), 
        DefaultValue(true)
        ]
        public bool ResetOnSpace
        {
            get 
            {
                return this.maskedTextProvider.ResetOnSpace;
            }
            set 
            {
                this.maskedTextProvider.ResetOnSpace = value;
            }
        }

        /// <devdoc>
        ///     Specifies whether to skip the current position if non-editable and the input character has 
        ///     the same value as the literal at that position.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.MaskedTextBoxSkipLiterals)), 
        DefaultValue(true)
        ]
        public bool SkipLiterals
        {
            get 
            {
                return this.maskedTextProvider.SkipLiterals;
            }
            set 
            {
                this.maskedTextProvider.SkipLiterals = value;
            }
        }

        /// <devdoc>
        ///       The currently selected text (if any) in the control.
        /// </devdoc>
        public override string SelectedText
        {
            get
            {
                if( this.flagState[IS_NULL_MASK])
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
            if (this.flagState[IS_NULL_MASK])
            {
                base.SetSelectedTextInternal(value, true); // Operates as a regular text box base.
                return;
            }

            PasteInt( value );
        }
       
        /// <devdoc>
        ///     Set the composition string as the result string.
        /// </devdoc>
        private void ImeComplete()
        {
            this.flagState[IME_COMPLETING] = true;
            ImeNotify(NativeMethods.CPS_COMPLETE);
        }
        
        /// <devdoc>
        ///     Notifies the IMM about changes to the status of the IME input context.
        /// </devdoc>
        private void ImeNotify(int action)
        {
            HandleRef handle    = new HandleRef(this, this.Handle);
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

        /// <devdoc>
        ///     Sets the underlying edit control's password char to the one obtained from this.PasswordChar.
        ///     This is used when the control is passworded and this.flagState[IS_NULL_MASK].
        /// </devdoc>
        private void SetEditControlPasswordChar( char pwdChar )
        {
            if (this.IsHandleCreated) 
            {
                // This message does not return a value.
                SendMessage(NativeMethods.EM_SETPASSWORDCHAR, pwdChar, 0);
                Invalidate();
            }
        }

        /// <devdoc>
        ///     The value of the Edit control default password char.
        /// </devdoc>
        private char SystemPasswordChar
        {
            get
            {
                if (MaskedTextBox.systemPwdChar == '\0')
                {
                    // This is the hard way to get the password char - left here for information.
                    // It is picked up from Comctl32.dll. If VisualStyles is enabled it will get the dot char. 
                    /*                
                    StringBuilder charVal = new StringBuilder(20);  // it could be 0x0000000000009999 format.
                    bool foundRsc         = false;
                    int IDS_PASSWORDCHAR  = 0x1076; // %ntsdx%\shell\comctrl32\v6\rcids.h
                                                    // defined in en.rc as: IDS_PASSWORDCHAR "9679" // 0x25cf - Black Circle

                    IntSecurity.UnmanagedCode.Assert();

                    try
                    {   
                        // The GetModuleHandle function returns a handle to a mapped module without incrementing its reference count. 
                        // @"C:\windows\winsxs\x86_Microsoft.Windows.Common-Controls_6595b64144ccf1df_6.0.10.0_x-ww_f7fb5805\comctl32.dll if VisulaStyles enabled.

                        IntPtr hModule = UnsafeNativeMethods.GetModuleHandle("comctl32.dll");
                        Debug.Assert(hModule != IntPtr.Zero, String.Format("Could not get a handle to comctl32.dll - Error: 0x{0:X8}", Marshal.GetLastWin32Error()));

                        foundRsc = UnsafeNativeMethods.LoadString(new HandleRef(null, hModule), IDS_PASSWORDCHAR, charVal, charVal.Capacity);
                    }
                    catch( Exception ex )
                    {
                        if( ClientUtils.IsSecurityOrCriticalException( ex ) )
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }

                    MaskedTextBox.systemPwdChar = foundRsc ? (char) int.Parse(charVal.ToString()) : MaskedTextProvider.DefaultPasswordChar;
                    */

                    // We need to temporarily create an edit control to get the default password character.  
                    // We cannot use this control because we would have to reset the native control's password char to use
                    // the defult one so we can get it; this would change the text displayed in the box (even for a short time)
                    // opening a sec hole.

                    TextBox txtBox = new TextBox();
                    txtBox.UseSystemPasswordChar = true; // this forces the creation of the control handle.

                    MaskedTextBox.systemPwdChar = txtBox.PasswordChar;

                    txtBox.Dispose();
                }

                return MaskedTextBox.systemPwdChar;
            }
        }

        /// <devdoc>
        ///     The Text setter validates the input char by char, raising the MaskInputRejected event for invalid chars.
        ///     The Text getter returns the formatted text according to the IncludeLiterals and IncludePrompt properties.
        /// </devdoc>
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
                if( this.flagState[IS_NULL_MASK] || this.flagState[QUERY_BASE_TEXT])
                {
                    return base.Text;
                }

                return TextOutput;
            }
            set
            {
                if (this.flagState[IS_NULL_MASK])
                {
                    base.Text = value;
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    // reset the input text.
                    Delete(Keys.Delete, 0, this.maskedTextProvider.Length);
                }
                else
                {
                    if( this.RejectInputOnFirstFailure )
                    {
                        MaskedTextResultHint hint;
                        string oldText = TextOutput;
                        if (this.maskedTextProvider.Set(value, out this.caretTestPos, out hint))
                        {
                            //if( hint == MaskedTextResultHint.Success || hint == MaskedTextResultHint.SideEffect )
                            if( TextOutput != oldText )
                            {
                                SetText();
                            }
                            this.SelectionStart = ++this.caretTestPos;
                        }
                        else
                        {
                            OnMaskInputRejected(new MaskInputRejectedEventArgs(this.caretTestPos, hint));
                        }
                    }
                    else
                    {
                        Replace(value, /*startPosition*/ 0, /*selectionLen*/ this.maskedTextProvider.Length);
                    }
                }
            }
        }

        /// <devdoc>
        ///     Returns the length of the displayed text.
        /// </devdoc>
        [Browsable( false )]
        public override int TextLength
        {
            get
            {
                if( this.flagState[IS_NULL_MASK] )
                {
                    return base.TextLength;
                }

                // In Win9x systems TextBoxBase.TextLength calls Text.Length directly and does not query the window for the actual text length.  
                // If TextMaskFormat is set to a anything different from IncludePromptAndLiterals or HidePromptOnLeave is true the return value 
                // may be incorrect because the Text property value and the display text may be different.  We need to handle this here.

                return GetFormattedDisplayString().Length;
            }
        }

        /// <devdoc>
        ///     The formatted text, it is what the Text getter returns when a mask has been applied to the control.
        ///     The text format follows the IncludeLiterals and IncludePrompt properties (See MaskedTextProvider.ToString()).
        /// </devdoc>
        private string TextOutput
        {
            get
            {
                Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
                return this.maskedTextProvider.ToString();
            }
        }

        /// <devdoc>
        ///     Gets or sets how text is aligned in the control.
        ///     Note: This code is duplicated in TextBox for simplicity.
        /// </devdoc>
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

        /// <devdoc>
        ///     Event to notify the text alignment has changed.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatPropertyChanged)), 
        SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))
        ]
        public event EventHandler TextAlignChanged 
        {
            add 
            {
                Events.AddHandler(EVENT_TEXTALIGNCHANGED, value);
            }

            remove 
            {
                Events.RemoveHandler(EVENT_TEXTALIGNCHANGED, value);
            }
        }

        /// <devdoc>
        ///    Specifies the formatting options for text output (Whether the mask returned from the Text 
        ///    property includes Literals and/or prompt characters).  
        ///    When prompt characters are excluded, theyare returned as spaces in the string returned.
        /// </devdoc>
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
                if( this.IncludePrompt )
                {
                    if( this.IncludeLiterals )
                    {
                        return MaskFormat.IncludePromptAndLiterals;
                    }

                    return MaskFormat.IncludePrompt;
                }

                if( this.IncludeLiterals )
                {
                    return MaskFormat.IncludeLiterals;
                }

                return MaskFormat.ExcludePromptAndLiterals;
            }
 
            set
            {
                if( this.TextMaskFormat == value )
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
                string oldText = this.flagState[IS_NULL_MASK] ? null : TextOutput;

                if( value == MaskFormat.IncludePrompt )
                {
                    this.IncludePrompt   = true;
                    this.IncludeLiterals = false;
                }  
                else if( value == MaskFormat.IncludeLiterals )
                {
                    this.IncludePrompt   = false;
                    this.IncludeLiterals = true;
                }  
                else // value == MaskFormat.IncludePromptAndLiterals || value == MaskFormat.ExcludePromptAndLiterals
                {
                    bool include = value == MaskFormat.IncludePromptAndLiterals;
                    this.IncludePrompt   = include;
                    this.IncludeLiterals = include;
                }

                if( oldText != null && oldText != TextOutput )
                {
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        /// <devdoc>
        ///    Provides some interesting information for the TextBox control in String form.
        ///    Returns the test string (no password, including literals and prompt).
        /// </devdoc>
        public override string ToString() 
        {
            if( this.flagState[IS_NULL_MASK] )
            {
                return base.ToString();
            }

            // base.ToString will call Text, we want to always display prompt and literals.
            bool includePrompt = this.IncludePrompt;
            bool includeLits   = this.IncludeLiterals;
            string str;
            try
            {
                this.IncludePrompt = this.IncludeLiterals = true;
                str = base.ToString();
            }
            finally
            {
                this.IncludePrompt = includePrompt;
                this.IncludeLiterals = includeLits;
            }

            return str;
        }

        /// <devdoc>
        ///     Event to notify when the validating object completes parsing the formatted text.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatFocus)), 
        SRDescription(nameof(SR.MaskedTextBoxTypeValidationCompletedDescr))
        ]
        public event TypeValidationEventHandler TypeValidationCompleted
        {
            add
            {
                Events.AddHandler(EVENT_VALIDATIONCOMPLETED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_VALIDATIONCOMPLETED, value);
            }
        }

        /// <devdoc>
        ///    Indicates if the text in the edit control should appear as the default password character. 
        ///    This property has precedence over the PasswordChar property.
        /// </devdoc>
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
                return this.flagState[USE_SYSTEM_PASSWORD_CHAR];
            }
            set
            {
                if (value != this.flagState[USE_SYSTEM_PASSWORD_CHAR])
                {
                    if (value)
                    {
                        if( this.SystemPasswordChar == this.PromptChar )
                        {
                            // Prompt and password chars must be different. 
                            throw new InvalidOperationException( SR.MaskedTextBoxPasswordAndPromptCharError );
                        }

                        this.maskedTextProvider.PasswordChar = this.SystemPasswordChar;
                    }
                    else
                    {
                        // this.passwordChar could be '\0', in which case we are resetting the display to show the input char.
                        this.maskedTextProvider.PasswordChar = this.passwordChar; 
                    }

                    this.flagState[USE_SYSTEM_PASSWORD_CHAR] = value;

                    if( this.flagState[IS_NULL_MASK])
                    {
                        SetEditControlPasswordChar(this.maskedTextProvider.PasswordChar);
                    }
                    else
                    {
                        SetWindowText();
                    }

                    VerifyImeRestrictedModeChanged();
                }
            }
        }

        /// <devdoc>
        ///     Type of the object to be used to parse the text when the user leaves the control. 
        ///     A ValidatingType object must implement a method with one fo the following signature:
        ///         public static Object Parse(string)
        ///         public static Object Parse(string, IFormatProvider)
        ///     See DateTime.Parse(...) for an example.
        /// </devdoc>
        [
        Browsable(false),
        DefaultValue(null)
        ]
        public Type ValidatingType
        {
            get 
            {
                return this.validatingType;
            }
            set 
            {
                if( this.validatingType != value )
                {
                    this.validatingType = value;
                }
            }
        }

        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool WordWrap
        {
            get { return false; }
            set {}
        }


        ////////////// Methods

        /// <devdoc>
        ///     Clears information about the most recent operation from the undo buffer of the control.
        ///     Unsupported property/method.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new void ClearUndo()
        {
        }

        /// <devdoc>
        ///     Creates a handle for this control. This method is called by the .NET Framework, this should
        ///     not be called. Inheriting classes should always call base.createHandle when overriding this method.
        ///     Overridden to be able to set the control text with the masked (passworded) value when recreating
        ///     handle, since the underlying native edit control is not aware of it.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)
        ]
        protected override void CreateHandle()
        {
            if (!this.flagState[IS_NULL_MASK] && RecreatingHandle)
            {
                // update cached text value in Control. Don't preserve caret, cannot query for selection start at this time.
                SetWindowText(GetFormattedDisplayString(), false, false);
            }
            
            base.CreateHandle();
        }

        /// 
        /// <devdoc>
        ///     Deletes characters from the control's text according to the key pressed (Delete/Backspace).
        ///     Returns true if something gets actually deleted, false otherwise.
        /// </devdoc>
        private void Delete(Keys keyCode, int startPosition, int selectionLen)
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
            Debug.Assert( keyCode == Keys.Delete || keyCode == Keys.Back, "Delete called with keyCode == " + keyCode.ToString() );
            Debug.Assert( startPosition >= 0 && ((startPosition + selectionLen) <= this.maskedTextProvider.Length), "Invalid position range." );

            // On backspace, moving the start postion back by one has the same effect as delete.  If text is selected, there is no
            // need for moving the position back.

            this.caretTestPos = startPosition;

            if( selectionLen == 0 )
            {
                if( keyCode == Keys.Back ) 
                {
                    if( startPosition == 0 ) // At beginning of string, backspace does nothing.
                    {
                        return;
                    }

                    startPosition--; // so it can be treated as delete.
                }
                else // (keyCode == Keys.Delete)
                {
                    if( (startPosition + selectionLen) == this.maskedTextProvider.Length ) // At end of string, delete does nothing.
                    {
                        return;
                    }
                }
            }

            int tempPos;
            int endPos = selectionLen > 0 ? startPosition + selectionLen - 1 : startPosition;
            MaskedTextResultHint hint;

            string oldText = TextOutput;
            if (this.maskedTextProvider.RemoveAt(startPosition, endPos, out tempPos, out hint))
            {
                //if( hint == MaskedTextResultHint.Success || hint == MaskedTextResultHint.SideEffect) // Text was changed.
                if( TextOutput != oldText )
                {
                    SetText();
                    this.caretTestPos = startPosition;
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

                    if( selectionLen > 0 )
                    {
                        this.caretTestPos = startPosition;
                    }
                    else
                    {
                        if( hint == MaskedTextResultHint.NoEffect ) // Case 2.
                        {
                            if( keyCode == Keys.Delete )
                            {
                                this.caretTestPos = this.maskedTextProvider.FindEditPositionFrom(startPosition, forward);
                            }
                            else
                            {
                                if( this.maskedTextProvider.FindAssignedEditPositionFrom( startPosition, forward ) == MaskedTextProvider.InvalidIndex )
                                {
                                    // No assigned position at the right, nothing to shift then move to the next assigned position at the
                                    // left (if any).
                                    this.caretTestPos = this.maskedTextProvider.FindAssignedEditPositionFrom(startPosition, backward);
                                }
                                else
                                {
                                    // there are assigned positions at the right so move to an edit position at the left to get ready for 
                                    // removing the character on it or just shifting the characters at the right
                                    this.caretTestPos = this.maskedTextProvider.FindEditPositionFrom(startPosition, backward);
                                }

                                if( this.caretTestPos != MaskedTextProvider.InvalidIndex )
                                {
                                    this.caretTestPos++; // backspace gets ready to remove one position past the edit position.
                                }
                            }

                            if( this.caretTestPos == MaskedTextProvider.InvalidIndex )
                            {
                                this.caretTestPos = startPosition;
                            }
                        }
                        else // (hint == MaskedTextProvider.OperationHint.SideEffect)
                        {
                            if( keyCode == Keys.Back )  // Case 3.
                            {
                                this.caretTestPos = startPosition;
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
            base.SelectInternal( this.caretTestPos, 0, this.maskedTextProvider.Length );

            return;
        }

        /// <devdoc>
        ///     Returns the character nearest to the given point.
        /// </devdoc>
        public override char GetCharFromPosition(Point pt) 
        {
            char ch;

            this.flagState[QUERY_BASE_TEXT] = true;
            try
            {
                ch = base.GetCharFromPosition(pt);
            }
            finally
            {
                this.flagState[QUERY_BASE_TEXT] = false;
            }
            return ch;
        }

        
        /// <devdoc>
        ///     Returns the index of the character nearest to the given point.
        /// </devdoc>
        public override int GetCharIndexFromPosition(Point pt) 
        {
            int index;

            this.flagState[QUERY_BASE_TEXT] = true;
            try
            {
                index = base.GetCharIndexFromPosition(pt);
            }
            finally
            {
                this.flagState[QUERY_BASE_TEXT] = false;
            }
            return index;
        }

        /// <devdoc>
        ///     Returns the position of the last input character (or if available, the next edit position). 
        ///     This is used by base.AppendText.
        /// </devdoc>
        internal override int GetEndPosition()
        {
            if( this.flagState[IS_NULL_MASK])
            {
                return base.GetEndPosition();
            }

            int pos = this.maskedTextProvider.FindEditPositionFrom( this.maskedTextProvider.LastAssignedPosition + 1, forward );

            if( pos == MaskedTextProvider.InvalidIndex )
            {
                pos = this.maskedTextProvider.LastAssignedPosition + 1;
            }

            return pos;
        }
        
        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new int GetFirstCharIndexOfCurrentLine()
        {
            return 0;
        }

        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new int GetFirstCharIndexFromLine(int lineNumber)
        {
            return 0;
        }

        /// <devdoc>
        ///     Gets the string in the text box following the formatting parameters includePrompt and includeLiterals and
        ///     honoring the PasswordChar property.
        /// </devdoc>
        private string GetFormattedDisplayString()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            bool includePrompt;

            if (this.ReadOnly) // Always hide prompt.
            {
                includePrompt = false;
            }
            else if (this.DesignMode) // Not RO and at design time, always show prompt.
            {
                includePrompt = true;
            }
            else // follow HidePromptOnLeave property.
            {
                includePrompt = !(this.HidePromptOnLeave && !this.Focused);
            }

            return this.maskedTextProvider.ToString(/*ignorePwdChar */ false, includePrompt, /*includeLiterals*/ true, 0, this.maskedTextProvider.Length);
        }

        /// <devdoc>
        ///     Unsupported method/property.
        ///     virtual method.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public override int GetLineFromCharIndex(int index)
        {
            return 0;
        }

        /// <devdoc>
        ///     Returns the location of the character at the given index.
        /// </devdoc>
        public override Point GetPositionFromCharIndex(int index) 
        {
            Point pos;

            this.flagState[QUERY_BASE_TEXT] = true;
            try
            {
                pos = base.GetPositionFromCharIndex(index);
            }
            finally
            {
                this.flagState[QUERY_BASE_TEXT] = false;
            }
            return pos;
        }

        /// <devdoc>
        ///     Need to override this method so when get_Text is called we return the text that is actually
        ///     painted in the control so measuring text works on the actual text and not the formatted one.
        /// </devdoc>
        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            Size size;

            this.flagState[QUERY_BASE_TEXT] = true;
            try
            {
                size = base.GetPreferredSizeCore( proposedConstraints );
            }
            finally
            {
                this.flagState[QUERY_BASE_TEXT] = false;
            }
            return size;
        }

        /// <devdoc>
        ///     The selected text in the control according to the CutCopyMaskFormat properties (IncludePrompt/IncludeLiterals).
        ///     This is used in Cut/Copy operations (SelectedText).
        ///     The prompt character is always replaced with a blank character.
        /// </devdoc>
        private string GetSelectedText()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            int selStart, selLength;
            base.GetSelectionStartAndLength( out selStart, out selLength );

            if( selLength == 0 )
            {
                return string.Empty;
            }

            bool includePrompt   = (CutCopyMaskFormat & MaskFormat.IncludePrompt  ) != 0;
            bool includeLiterals = (CutCopyMaskFormat & MaskFormat.IncludeLiterals) != 0; 

            return this.maskedTextProvider.ToString( /*ignorePasswordChar*/ true, includePrompt, includeLiterals, selStart, selLength );
        }


        /// <include file='doc\MaskedTextBox.uex' path='docs/doc[@for="MaskedTextBox.OnBackColorChanged"]/*' />
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            // Force repainting of the entire window frame
            if (Application.RenderWithVisualStyles && this.IsHandleCreated && this.BorderStyle == BorderStyle.Fixed3D)
            {
                SafeNativeMethods.RedrawWindow(new HandleRef(this, this.Handle), null, NativeMethods.NullHandleRef, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
            }
        }

        /// <devdoc>
        ///    Overridden to update the newly created handle with the settings of the PasswordChar properties 
        ///    if no mask has been set.
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e) 
        {
            base.OnHandleCreated(e);
            base.SetSelectionOnHandle();

            if( this.flagState[IS_NULL_MASK]&& this.maskedTextProvider.IsPassword )
            {
                SetEditControlPasswordChar(this.maskedTextProvider.PasswordChar);
            }
        }

        /// <devdoc>
        ///    Raises the IsOverwriteModeChanged event.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void OnIsOverwriteModeChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_ISOVERWRITEMODECHANGED] as EventHandler;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <devdoc>
        ///     Raises the <see cref='System.Windows.Forms.Control.KeyDown'/> event.
        /// </devdoc>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if( this.flagState[IS_NULL_MASK])
            {
                // Operates as a regular text box base.
                return;
            }

            Keys keyCode = e.KeyCode;

            // Special-case Return & Esc since they generate invalid characters we should not process OnKeyPress.
            if( keyCode == Keys.Return || keyCode == Keys.Escape )
            {
                this.flagState[HANDLE_KEY_PRESS] = false;
            }


            // Insert is toggled when not modified with some other key (ctrl, shift...).  Note that shift-Insert is 
            // same as paste.
            if (keyCode == Keys.Insert && e.Modifiers == Keys.None && this.insertMode == InsertKeyMode.Default)
            {
                this.flagState[INSERT_TOGGLED] = !this.flagState[INSERT_TOGGLED];
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
                        this.flagState[HANDLE_KEY_PRESS] = false;
                        return;
                }
            }

            if ( keyCode == Keys.Delete || keyCode == Keys.Back ) // Deletion keys.
            {
                if (!this.ReadOnly)
                {
                    int selectionLen;
                    int startPosition;

                    base.GetSelectionStartAndLength( out startPosition, out selectionLen );

                    switch (e.Modifiers)
                    {
                        case Keys.Shift:
                            if( keyCode == Keys.Delete )
                            {
                                keyCode = Keys.Back;
                            }
                            goto default;

                        case Keys.Control:
                            if( selectionLen == 0 ) // In other case, the selected text should be deleted.
                            {
                                if( keyCode == Keys.Delete ) // delete to the end of the string.
                                {
                                    selectionLen = this.maskedTextProvider.Length - startPosition;
                                }
                                else // ( keyCode == Keys.Back ) // delete to the beginning of the string.
                                {
                                    selectionLen = startPosition == this.maskedTextProvider.Length /*at end of text*/ ? startPosition : startPosition + 1;
                                    startPosition     = 0;    
                                }
                            }
                            goto default;

                        default:
                            if( !this.flagState[HANDLE_KEY_PRESS] )
                            {
                                this.flagState[HANDLE_KEY_PRESS] = true;
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


        /// <devdoc>
        ///     Raises the <see cref='System.Windows.Forms.Control.KeyPress'/> event.
        /// </devdoc>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if( this.flagState[IS_NULL_MASK])
            {
                // Operates as a regular text box base.
                return;
            }

            // This key may be a combined key involving a letter, like Ctrl-A; let the native control handle it.
            if( !this.flagState[HANDLE_KEY_PRESS] )
            {
                this.flagState[HANDLE_KEY_PRESS] = true;
                
                // When the combined key involves a letter, the final character is not a letter. There are some 
                // Ctrl combined keys that generate a letter and can be confusing; we do not mean to pass those 
                // characters to the underlying Edit control.  These combinations are: Ctrl-F<#> and Ctrl-Atl-<someKey> 
                if (!char.IsLetter(e.KeyChar))
                {
                    return;
                }
            }

            if( !this.ReadOnly)
            {
                // At this point the character needs to be processed ...

                MaskedTextResultHint hint;

                int selectionStart;
                int selectionLen;

                base.GetSelectionStartAndLength( out selectionStart, out selectionLen );

                string oldText = TextOutput;
                if (PlaceChar(e.KeyChar, selectionStart, selectionLen, this.IsOverwriteMode, out hint))
                {
                    //if( hint == MaskedTextResultHint.Success || hint == MaskedTextResultHint.SideEffect )
                    if( TextOutput != oldText )
                    {
                        SetText(); // Now set the text in the display.
                    }
                    
                    this.SelectionStart = ++this.caretTestPos; // caretTestPos is updated in PlaceChar.

                    if (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable)
                    {
                        // Korean IMEs complete composition when a character has been fully converted, so the composition string
                        // is only one-character long; once composed we block the IME if there ins't more room in the test string.

                        int editPos = this.maskedTextProvider.FindUnassignedEditPositionFrom(this.caretTestPos, forward);
                        if (editPos == MaskedTextProvider.InvalidIndex)
                        {
                            ImeComplete();  // Force completion of compostion.
                        }
                    }
                }
                else
                {
                    OnMaskInputRejected(new MaskInputRejectedEventArgs(this.caretTestPos, hint)); // caretTestPos is updated in PlaceChar.
                }

                if( selectionLen > 0 )
                {
                    this.SelectionLength = 0;
                }

                e.Handled = true;
            }
        }

        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyUp'/> event.</para>
        /// </devdoc>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            // KeyUp is the last message to be processed so it is the best place to reset these flags.

            if (this.flagState[IME_COMPLETING])
            {
                this.flagState[IME_COMPLETING] = false;
            }

            if( this.flagState[IME_ENDING_COMPOSITION] )
            {
                this.flagState[IME_ENDING_COMPOSITION] = false;
            }
        }

        /// <devdoc>
        ///    Raises the MaskChanged event.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void OnMaskChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_MASKCHANGED] as EventHandler;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <devdoc>
        ///     Raises the MaskInputRejected event.
        /// </devdoc>
        private void OnMaskInputRejected(MaskInputRejectedEventArgs e)
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            if (this.BeepOnError)
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
                sp.Play();
            }

            MaskInputRejectedEventHandler eh = Events[EVENT_MASKINPUTREJECTED] as MaskInputRejectedEventHandler;

            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <devdoc>
        ///     Unsupported method/property.
        ///     virtual method.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        protected override void OnMultilineChanged(EventArgs e)
        {
        }

        /// <devdoc>
        ///    Raises the TextAlignChanged event.
        /// </devdoc>
        protected virtual void OnTextAlignChanged(EventArgs e) 
        {
            EventHandler eh = Events[EVENT_TEXTALIGNCHANGED] as EventHandler;
            if (eh != null) 
            {
                eh(this, e);
            }
        }


        /// <devdoc>
        ///     Raises the TypeValidationCompleted event.
        /// </devdoc>
        private void OnTypeValidationCompleted(TypeValidationEventArgs e)
        {
            TypeValidationEventHandler eh = Events[EVENT_VALIDATIONCOMPLETED] as TypeValidationEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <devdoc>
        ///     Raises the  System.Windows.Forms.Control.Validating event.
        ///     Overridden here to be able to control the order validating events are
        ///     raised [TypeValidationCompleted - Validating - Validated - Leave - KillFocus]
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnValidating(CancelEventArgs e) 
        {
            // Note: It seems impractical to perform type validation here if the control is read only but we need
            // to be consistent with other TextBoxBase controls which don't check for RO; and we don't want 
            // to fix them to avoid introducing breaking changes.
            PerformTypeValidation(e);
            base.OnValidating(e);
        }

        /// <devdoc>
        ///    Raises the TextChanged event and related Input/Output text events when mask is null.
        ///    Overriden here to be able to control order of text changed events.
        /// </devdoc>
        protected override void OnTextChanged(EventArgs e) 
        {
            // A text changed event handler will most likely query for the Text value, we need to return the
            // formatted one.
            bool queryBaseText = this.flagState[QUERY_BASE_TEXT];
            this.flagState[QUERY_BASE_TEXT] = false;
            try
            {
                base.OnTextChanged(e);
            }
            finally
            {
                this.flagState[QUERY_BASE_TEXT] = queryBaseText;
            }
        }
        /// <devdoc>
        ///     Replaces the current selection in the text box specified by the startPosition and selectionLen parameters
        ///     with the contents of the supplied string.
        /// </devdoc>
        private void Replace(string text, int startPosition, int selectionLen)
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
            Debug.Assert(text != null, "text is null.");

            // Clone the MaskedTextProvider so text properties are not modified until the paste operation is
            // completed.  This is needed in case one of these properties is retreived in a MaskedInputRejected
            // event handler (clipboard text is attempted to be set into the input text char by char).

            MaskedTextProvider clonedProvider = (MaskedTextProvider) this.maskedTextProvider.Clone();

            // Cache the current caret position so we restore it in case the text does not change.
            int currentCaretPos = this.caretTestPos;

            // First replace characters in the selection (if any and if any edit positions) until completed, or the test position falls 
            // outside the selection range, or there's no more room in the test string for editable characters.
            // Then insert any remaining characters from the input.

            MaskedTextResultHint hint = MaskedTextResultHint.NoEffect;
            int endPos = startPosition + selectionLen - 1;

            if( this.RejectInputOnFirstFailure )
            {
                bool succeeded; 

                succeeded = (startPosition > endPos) ?
                    clonedProvider.InsertAt(text, startPosition, out this.caretTestPos, out hint ) :
                    clonedProvider.Replace(text, startPosition, endPos, out this.caretTestPos, out hint);

                if( !succeeded )
                {
                    OnMaskInputRejected(new MaskInputRejectedEventArgs(this.caretTestPos, hint));
                }
            }
            else
            {
                // temp hint used to preserve the 'primary' operation hint (no side effects).
                MaskedTextResultHint tempHint = hint;
                int testPos;
                
                foreach (char ch in text)
                {
                    if( !this.maskedTextProvider.VerifyEscapeChar( ch, startPosition ))  // char won't be escaped, find and edit position for it.
                    {
                        // Observe that we look for a position w/o respecting the selection length, because the input text could be larger than
                        // the number of edit positions in the selection.
                        testPos = clonedProvider.FindEditPositionFrom(startPosition, forward);

                        if( testPos == MaskedTextProvider.InvalidIndex )
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
                        startPosition = this.caretTestPos + 1;

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
                        if (!clonedProvider.RemoveAt(startPosition, endPos, out this.caretTestPos, out tempHint))
                        {
                            OnMaskInputRejected(new MaskInputRejectedEventArgs(this.caretTestPos, tempHint));
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
            this.maskedTextProvider = clonedProvider;
            
            // Update text if needed.
            if( updateText )
            {
                SetText();

                // Update caret position.
                this.caretTestPos = startPosition;
                base.SelectInternal( this.caretTestPos, 0, this.maskedTextProvider.Length );
            }
            else
            {
                this.caretTestPos = currentCaretPos;
            }

            return;
        }

        /// <devdoc>
        ///     Pastes specified text over the currently selected text (if any) shifting upper characters if
        ///     input is longer than selected text, and/or removing remaining characters from the selection if
        ///     input contains less characters.
        /// </devdoc>
        private void PasteInt( string text )
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
            
            int selStart, selLength;
            base.GetSelectionStartAndLength(out selStart, out selLength);

            if( string.IsNullOrEmpty(text) )
            {
                Delete( Keys.Delete, selStart, selLength );
            }
            else
            {
                Replace(text, selStart, selLength);
            }
        }

        /// <devdoc>
        ///     Performs validation of the input string using the provided ValidatingType object (if any).
        ///     Returns an object created from the formatted text.
        ///     If the CancelEventArgs param is not null, it is assumed the control is leaving focus and
        ///     the validation event chain is being executed (TypeValidationCompleted - Validating - Validated...);
        ///     the value of the CancelEventArgs.Cancel property is the same as the TypeValidationEventArgs.Cancel
        ///     on output (Cancel provides proper handling of focus shifting at the Control class level).
        ///     Note: The text being validated does not include prompt chars.
        /// </devdoc>
        private object PerformTypeValidation(CancelEventArgs e)
        {
            object parseRetVal = null;

            if (this.validatingType != null)
            {
                string message = null;
                
                if (!this.flagState[IS_NULL_MASK]&& this.maskedTextProvider.MaskCompleted == false)
                {
                    message = SR.MaskedTextBoxIncompleteMsg;
                }
                else
                {
                    string textValue;

                    if( !this.flagState[IS_NULL_MASK]) // replace prompt with space.
                    {
                        textValue = this.maskedTextProvider.ToString(/*includePrompt*/ false, this.IncludeLiterals);
                    }
                    else
                    {
                        textValue = base.Text;
                    }

                    try
                    {
                        parseRetVal = Formatter.ParseObject(
                            textValue,              // data
                            this.validatingType,    // targetType
                            typeof(string),         // sourceType
                            null,                   // targetConverter
                            null,                   // sourceConverter
                            this.formatProvider,    // formatInfo
                            null,                   // nullValue
                            Formatter.GetDefaultDataSourceNullValue(this.validatingType));   // dataSourceNullValue
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
                
                TypeValidationEventArgs tve = new TypeValidationEventArgs(this.validatingType, isValidInput, parseRetVal, message);
                OnTypeValidationCompleted(tve);

                if( e != null ) 
                {
                    e.Cancel = tve.Cancel;
                }
            }

            return parseRetVal;
        }

        /// <devdoc>
        ///     Insert or replaces the specified character into the control's text and updates the caret position.  
        ///     If overwrite is true, it replaces the character at the selection start position.
        /// </devdoc>
        private bool PlaceChar(char ch, int startPosition, int length, bool overwrite,
            out MaskedTextResultHint hint)
        {
            return PlaceChar(this.maskedTextProvider, ch, startPosition, length, overwrite, out hint );
        }

        /// <devdoc>
        ///     Override version to be able to perform the operation on a cloned provider.
        /// </devdoc>
        private bool PlaceChar(MaskedTextProvider provider, char ch, int startPosition, int length, bool overwrite, 
            out MaskedTextResultHint hint)
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            this.caretTestPos = startPosition;

            if (startPosition < this.maskedTextProvider.Length)
            {
                if (length > 0)  // Replacing selection with input char.
                {
                    int endPos = startPosition + length - 1;
                    return provider.Replace(ch, startPosition, endPos, out this.caretTestPos, out hint);
                }
                else
                {
                    if (overwrite)
                    {
                        // overwrite character at next edit position from startPosition (inclusive).
                        return provider.Replace(ch, startPosition, out this.caretTestPos, out hint);
                    }
                    else // insert.
                    {
                        return provider.InsertAt(ch, startPosition, out this.caretTestPos, out hint);
                    }
                }
            }

            hint = MaskedTextResultHint.UnavailableEditPosition;
            return false;
        }

        /// <devdoc>
        ///     <From Control.cs>:
        ///     Processes a command key. This method is called during message
        ///     pre-processing to handle command keys. Command keys are keys that always
        ///     take precedence over regular input keys. Examples of command keys
        ///     include accelerators and menu shortcuts. The method must return true to
        ///     indicate that it has processed the command key, or false to indicate
        ///     that the key is not a command key.
        /// 
        ///     processCmdKey() first checks if the control has a context menu, and if
        ///     so calls the menu's processCmdKey() to check for menu shortcuts. If the
        ///     command key isn't a menu shortcut, and if the control has a parent, the
        ///     key is passed to the parent's processCmdKey() method. The net effect is
        ///     that command keys are "bubbled" up the control hierarchy.
        /// 
        ///     When overriding processCmdKey(), a control should return true to
        ///     indicate that it has processed the key. For keys that aren't processed by
        ///     the control, the result of "base.processCmdKey()" should be returned.
        /// 
        ///     Controls will seldom, if ever, need to override this method.
        ///     </From Control.cs>
        /// 
        ///     Implements the handling of Ctrl+A (select all). Note: Code copied from TextBox.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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

        /// <devdoc>
        ///     We need to override this method so we can handle input language changes properly.  Control
        ///     doesn't handle the WM_CHAR messages generated after WM_IME_CHAR messages, it passes them
        ///     to DefWndProc (the characters would be displayed in the text box always).
        ///     
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected internal override bool ProcessKeyMessage(ref Message m)
        {
            // call base's method so the WM_CHAR and other messages are processed; this gives Control the 
            // chance to flush all pending WM_CHAR processing after WM_IME_CHAR messages are generated.
            
            bool msgProcessed = base.ProcessKeyMessage(ref m);

            if (this.flagState[IS_NULL_MASK])
            {
                return msgProcessed; // Operates as a regular text box base.
            }

            // If this WM_CHAR message is sent after WM_IME_CHAR, we ignore it since we already processed 
            // the corresponding WM_IME_CHAR message.  

            if( m.Msg == NativeMethods.WM_CHAR && base.ImeWmCharsToIgnore > 0 ) {
                return true;    // meaning, we handled the message so it is not passed to the default WndProc.
            }

            return msgProcessed;
        }

        
        /// <devdoc>
        ///     Designe time support for resetting Culture property..
        /// </devdoc>
        private void ResetCulture()
        {
            this.Culture = CultureInfo.CurrentCulture;
        }

        /// <devdoc>
        ///     Unsupported method/property.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new void ScrollToCaret() 
        {
        }

        /// <devdoc>
        ///     Sets the underlying MaskedTextProvider object.  Used when the control is initialized
        ///     and one of its properties, backed up by the MaskedTextProvider, changes; this requires
        ///     recreating the provider because it is immutable.
        /// </devdoc>
        private void SetMaskedTextProvider( MaskedTextProvider newProvider )
        {
            SetMaskedTextProvider( newProvider, null);
        }

        /// <devdoc>
        ///     Overload to allow for passing the text when the mask is being changed from null,
        ///     in this case the maskedTextProvider holds backend info only (not the text).
        /// </devdoc>
        private void SetMaskedTextProvider( MaskedTextProvider newProvider, string textOnInitializingMask )
        {
            Debug.Assert( newProvider != null, "Initializing from a null MaskProvider ref." );
   
            // Set R/W properties.
            newProvider.IncludePrompt    = this.maskedTextProvider.IncludePrompt;
            newProvider.IncludeLiterals  = this.maskedTextProvider.IncludeLiterals;
            newProvider.SkipLiterals     = this.maskedTextProvider.SkipLiterals;
            newProvider.ResetOnPrompt    = this.maskedTextProvider.ResetOnPrompt;
            newProvider.ResetOnSpace     = this.maskedTextProvider.ResetOnSpace;

            // If mask not initialized and not initializing it, the new provider is just a property backend.
            // Change won't have any effect in text.
            if( this.flagState[IS_NULL_MASK] && textOnInitializingMask == null)
            {
                this.maskedTextProvider = newProvider;
                return;
            }

            int testPos = 0;
            bool raiseOnMaskInputRejected = false; // Raise if new provider rejects old text.
            MaskedTextResultHint hint = MaskedTextResultHint.NoEffect;
            MaskedTextProvider oldProvider = this.maskedTextProvider;
            
            // Attempt to add previous text.
            // If the mask is the same, we need to preserve the caret and character positions if the text is added successfully.
            bool preserveCharPos = oldProvider.Mask == newProvider.Mask;

            // Cache text output text before setting the new provider to determine whether we need to raise the TextChanged event.
            string oldText;

            // NOTE: Whenever changing the MTP, the text is lost if any character in the old text violates the new provider's mask.

            if( textOnInitializingMask != null ) // Changing Mask (from null), which is the only RO property that requires passing text.
            {
                oldText  = textOnInitializingMask;
                raiseOnMaskInputRejected = !newProvider.Set( textOnInitializingMask, out testPos, out hint );
            }
            else
            {
                oldText  = TextOutput;

                // We need to attempt to set the input characters one by one in the edit positions so they are not
                // escaped. 
                int assignedCount = oldProvider.AssignedEditPositionCount;
                int srcPos = 0;
                int dstPos = 0;

                while( assignedCount > 0 )
                {
                    srcPos = oldProvider.FindAssignedEditPositionFrom( srcPos, forward );
                    Debug.Assert( srcPos != MaskedTextProvider.InvalidIndex, "InvalidIndex unexpected at this time." );

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

                    if( !newProvider.Replace( oldProvider[srcPos], dstPos, out testPos, out hint ))
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
            this.maskedTextProvider = newProvider;

            if( this.flagState[IS_NULL_MASK] )
            {
                this.flagState[IS_NULL_MASK] = false;
            }

            // Raising events need to be done only after the new provider has been set so the MTB is in a state where properties 
            // can be queried from event handlers safely.
            if( raiseOnMaskInputRejected )
            {
                OnMaskInputRejected(new MaskInputRejectedEventArgs(testPos, hint));
            }

            if( newProvider.IsPassword )
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

        /// <devdoc>
        ///     Sets the control's text to the formatted text obtained from the underlying MaskedTextProvider.
        ///     TextChanged is raised always, this assumes the display or the output text changed.
        ///     The caret position is lost (unless cached somewhere else like when lossing the focus).
        ///     This is the common way of changing the text in the control.
        /// </devdoc>
        private void SetText()
        {
            SetWindowText(GetFormattedDisplayString(), true, false);
        }

        /// <devdoc>
        ///     Sets the control's text to the formatted text obtained from the underlying MaskedTextProvider.
        ///     TextChanged is not raised. [PasswordChar]
        ///     The caret position is preserved.
        /// </devdoc>
        private void SetWindowText()
        {
            SetWindowText(GetFormattedDisplayString(), false, true);
        }

        /// <devdoc>
        ///     Sets the text directly in the underlying edit control to the value specified.
        ///     The 'raiseTextChangedEvent' param determines whether TextChanged event is raised or not.
        ///     The 'preserveCaret' param determines whether an attempt to preserve the caret position should be made or not
        ///     after the call to SetWindowText (WindowText) is performed.
        /// </devdoc>
        private void SetWindowText(string text, bool raiseTextChangedEvent, bool preserveCaret)
        {
            this.flagState[QUERY_BASE_TEXT] = true;

            try
            {
                if( preserveCaret )
                {
                    this.caretTestPos = this.SelectionStart;
                }

                WindowText = text;  // this calls Win32::SetWindowText directly, no OnTextChanged raised.

                if( raiseTextChangedEvent )
                {
                    OnTextChanged(EventArgs.Empty);
                }

                if( preserveCaret )
                {
                    this.SelectionStart = this.caretTestPos;
                }
            }
            finally
            {
                this.flagState[QUERY_BASE_TEXT] = false;
            }        
        }

        /// <devdoc>
        ///     Designe time support for checking if Culture value in the designer should be serialized.
        /// </devdoc>
        private bool ShouldSerializeCulture()
        {
            return !CultureInfo.CurrentCulture.Equals(this.Culture);
        }

        /// <devdoc>
        ///       Undoes the last edit operation in the text box.
        ///       Unsupported property/method.
        ///       WndProc ignores EM_UNDO.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new void Undo()
        {
        }

        /// <devdoc>
        ///       Forces type validation.  Returns the validated text value.
        /// </devdoc>
        public object ValidateText()
        {
            return PerformTypeValidation(null);
        }

        /// <devdoc>
        ///     Deletes all input characters in the current selection.
        /// </devdoc>
        private bool WmClear()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
            
            if( !this.ReadOnly )
            {
                int selStart, selLength;
                base.GetSelectionStartAndLength( out selStart, out selLength );
                Delete(Keys.Delete, selStart, selLength);
                return true;
            }
            
            return false;
        }

        /// <devdoc>
        ///     Copies current selection text to the clipboard, formatted according to the IncludeLiterals properties but
        ///     ignoring the prompt character.
        ///     Returns true if the operation succeeded, false otherwise.
        /// </devdoc>
        private bool WmCopy()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            if (this.maskedTextProvider.IsPassword) // cannot copy password to clipboard.
            {
                return false;
            }

            string text = GetSelectedText();

            try
            {
                // 




                IntSecurity.ClipboardWrite.Assert();

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

        /// <devdoc>
        ///     Processes the WM_IME_COMPOSITION message when using Korean IME.
        ///     Korean IME uses the control's caret as the composition string (it processes only one character at a time), 
        ///     we need to have special message handling for it.
        ///     Returns true if the message is handled, false otherwise.
        /// </devdoc>
        private bool WmImeComposition(ref Message m)
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

#if DEBUG
            if (this.ReadOnly || this.maskedTextProvider.IsPassword)
            {
                // This should have been already handled by the ReadOnly, PasswordChar and ImeMode properties.
                Debug.Assert(this.ImeMode == ImeMode.Disable, "IME enabled when in RO or Pwd mode.");
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
                    if (this.flagState[IME_ENDING_COMPOSITION])
                    {
                        // If IME is completing the convertion, we don't want to process further characters.
                        return this.flagState[IME_COMPLETING];
                    }
                }
            }

            return false; //message not handled.
        }

        /// <devdoc>
        ///     Processes the WM_IME_STARTCOMPOSITION message.
        ///     Returns true if the message is handled, false otherwise.
        /// </devdoc>
        private bool WmImeStartComposition()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
            
            // Position the composition window in a valid place.
            
            int startPosition, selectionLen;
            base.GetSelectionStartAndLength( out startPosition, out selectionLen );

            int startEditPos = this.maskedTextProvider.FindEditPositionFrom( startPosition, forward );
            
            if( startEditPos != MaskedTextProvider.InvalidIndex )
            {
                if (selectionLen > 0  && (ImeModeConversion.InputLanguageTable == ImeModeConversion.KoreanTable))
                {
                    // Korean IME: We need to delete the selected text and reposition the caret so the IME processes one
                    // character only, otherwise it would overwrite the selection with the caret (composition string), 
                    // deleting a portion of the mask.

                    int endEditPos = this.maskedTextProvider.FindEditPositionFrom(startPosition + selectionLen - 1, backward);

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
                if( startPosition != startEditPos )
                {
                    this.caretTestPos   = startEditPos;
                    this.SelectionStart = this.caretTestPos;
                }

                this.SelectionLength = 0;
            }
            else
            {
                ImeComplete();
                OnMaskInputRejected(new MaskInputRejectedEventArgs(startPosition, MaskedTextResultHint.UnavailableEditPosition));
                return true;
            }

            return false;
        }


        /// <devdoc>
        ///     Processes the WM_PASTE message. Copies the text from the clipboard, if is valid,
        ///     formatted according to the mask applied to this control.
        ///     Returns true if the operation succeeded, false otherwise.
        /// </devdoc>
        private void WmPaste()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            if( this.ReadOnly )
            {
                return;
            }

            // Get the text from the clipboard.

            string text;

            try
            {
                IntSecurity.ClipboardRead.Assert();
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

            PasteInt( text );
        }

        private void WmPrint(ref Message m) {
            base.WndProc(ref m);
            if ((NativeMethods.PRF_NONCLIENT & unchecked( (int) (long)m.LParam)) != 0 && Application.RenderWithVisualStyles && this.BorderStyle == BorderStyle.Fixed3D) {
                IntSecurity.UnmanagedCode.Assert();
                try {
                    using (Graphics g = Graphics.FromHdc(m.WParam)) {
                        Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
                        using (Pen pen = new Pen(VisualStyleInformation.TextControlBorder)) {
                            g.DrawRectangle(pen, rect);
                        }
                        rect.Inflate(-1, -1);
                        g.DrawRectangle(SystemPens.Window, rect);
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        /// <devdoc>
        ///     We need to override the WndProc method to have full control over what characters can be
        ///     displayed in the text box; particularly, we have special handling when IME is turned on.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            // Handle messages for special cases (unsupported operations or cases where mask doesn not matter).
            switch (m.Msg)
            {
                case NativeMethods.WM_PRINT:
                    WmPrint(ref m);
                    return;
                case NativeMethods.WM_CONTEXTMENU:
                case NativeMethods.EM_CANUNDO:
                    base.ClearUndo(); // resets undo buffer.
                    base.WndProc(ref m);
                    return;

                case NativeMethods.EM_SCROLLCARET:  // No scroll for single-line control.
                case NativeMethods.EM_LIMITTEXT:    // Max/Min text is defined by the mask.
                case NativeMethods.EM_UNDO:
                case NativeMethods.WM_UNDO:
                    return;

                default:
                    break;  // continue.
            }

            if( this.flagState[IS_NULL_MASK])
            {
                base.WndProc(ref m); // Operates as a regular text box base.
                return;
            }

            switch (m.Msg)
            {
                case NativeMethods.WM_IME_STARTCOMPOSITION:
                    if( WmImeStartComposition() )
                    {
                        break;
                    }
                    goto default;

                case NativeMethods.WM_IME_ENDCOMPOSITION:
                    this.flagState[IME_ENDING_COMPOSITION] = true;
                    goto default;

                case NativeMethods.WM_IME_COMPOSITION:
                    if( WmImeComposition( ref m ) )
                    {
                        break;
                    }
                    goto default;

                case NativeMethods.WM_CUT:
                    if (!this.ReadOnly && WmCopy())
                    {
                        WmClear();
                    }
                    break;

                case NativeMethods.WM_COPY:
                    WmCopy();
                    break;

                case NativeMethods.WM_PASTE:
                    WmPaste();
                    break;

                case NativeMethods.WM_CLEAR:
                    WmClear();
                    break;

                case NativeMethods.WM_KILLFOCUS:
                    base.WndProc(ref m);
                    WmKillFocus();
                    break;

                case NativeMethods.WM_SETFOCUS:
                    WmSetFocus();
                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <devdoc>
        ///     Processes the WM_KILLFOCUS message. Updates control's text replacing promp chars with space.
        /// </devdoc>
        private void WmKillFocus()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );

            base.GetSelectionStartAndLength( out this.caretTestPos, out this.lastSelLength );

            if (this.HidePromptOnLeave && !this.MaskFull)
            {
                SetWindowText(); // Update text w/ no prompt.

                // We need to update selection info in case the control is queried for it while it doesn't have the focus.
                base.SelectInternal( this.caretTestPos, this.lastSelLength, this.maskedTextProvider.Length );
            }
        }

        /// <devdoc>
        ///     Processes the WM_SETFOCUS message. Updates control's text with formatted text according to 
        ///     the include prompt property.
        /// </devdoc>
        private void WmSetFocus()
        {
            Debug.Assert( !this.flagState[IS_NULL_MASK], "This method must be called when a Mask is provided." );
 
            if (this.HidePromptOnLeave && !this.MaskFull) // Prompt will show up.
            {
                SetWindowText();
            }
        
            // Restore previous selection. Do this always (as opposed to within the condition above as in WmKillFocus)
            // because HidePromptOnLeave could have changed while the control did not have the focus.
            base.SelectInternal( this.caretTestPos, this.lastSelLength, this.maskedTextProvider.Length );
        }
    }
}

