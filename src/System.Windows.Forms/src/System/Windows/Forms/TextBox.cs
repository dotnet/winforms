// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.ComponentModel.Design;    
    using System.Drawing;
    using Microsoft.Win32;
    using System.Reflection;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Collections.Specialized;
    using System.Drawing.Design;
    using System.Security;
    using System.Windows.Forms.VisualStyles;
    
    /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a Windows text box control.
    ///    </para>
    /// </devdoc>


    [
      ClassInterface(ClassInterfaceType.AutoDispatch),
      ComVisible(true),
      Designer("System.Windows.Forms.Design.TextBoxDesigner, " + AssemblyRef.SystemDesign),
      SRDescription(nameof(SR.DescriptionTextBox))
    ]
    public class TextBox : TextBoxBase {
    
        private static readonly object EVENT_TEXTALIGNCHANGED = new object();
    
        /// <devdoc>
        ///     Controls whether or not the edit box consumes/respects ENTER key
        ///     presses.  While this is typically desired by multiline edits, this
        ///     can interfere with normal key processing in a dialog.
        /// </devdoc>
        private bool acceptsReturn = false;

        /// <devdoc>
        ///     Indicates what the current special password character is.  This is 
        ///     displayed instead of any other text the user might enter.
        /// </devdoc>
        private char passwordChar = (char)0;

        private bool useSystemPasswordChar;

        /// <devdoc>
        ///     Controls whether or not the case of characters entered into the edit
        ///     box is forced to a specific case.
        /// </devdoc>
        private CharacterCasing characterCasing = System.Windows.Forms.CharacterCasing.Normal;

        /// <devdoc>
        ///     Controls which scrollbars appear by default.
        /// </devdoc>
        private ScrollBars scrollBars = System.Windows.Forms.ScrollBars.None;

        /// <devdoc>
        ///     Controls text alignment in the edit box.
        /// </devdoc>
        private HorizontalAlignment textAlign = HorizontalAlignment.Left;
        
        /// <devdoc>
        ///     True if the selection has been set by the user.  If the selection has
        ///     never been set and we get focus, we focus all the text in the control
        ///     so we mimic the Windows dialog manager.
        /// </devdoc>
        private bool selectionSet = false;

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.autoCompleteMode"]/*' />
        /// <devdoc>
        ///     This stores the value for the autocomplete mode which can be either
        ///     None, AutoSuggest, AutoAppend or AutoSuggestAppend.
        /// </devdoc>
        private AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;
        
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.autoCompleteSource"]/*' />
        /// <devdoc>
        ///     This stores the value for the autoCompleteSource mode which can be one of the values
        ///     from AutoCompleteSource enum.
        /// </devdoc>
        private AutoCompleteSource autoCompleteSource = AutoCompleteSource.None;

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.autoCompleteCustomSource"]/*' />
        /// <devdoc>
        ///     This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
        /// </devdoc>
        private AutoCompleteStringCollection autoCompleteCustomSource;
        private bool fromHandleCreate = false;
        private StringSource stringSource = null;

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.TextBox"]/*' />
        public TextBox(){
        }

        
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.AcceptsReturn"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether pressing ENTER
        ///       in a multiline <see cref='System.Windows.Forms.TextBox'/>
        ///       control creates a new line of text in the control or activates the default button
        ///       for the form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxAcceptsReturnDescr))
        ]
        public bool AcceptsReturn {
            get {
                return acceptsReturn;
            }

            set {
                acceptsReturn = value;
            }
        }


        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.AutoCompleteMode"]/*' />
        /// <devdoc>
        ///     This is the AutoCompleteMode which can be either
        ///     None, AutoSuggest, AutoAppend or AutoSuggestAppend. 
        ///     This property in conjunction with AutoCompleteSource enables the AutoComplete feature for TextBox.
        /// </devdoc>
        [
        DefaultValue(AutoCompleteMode.None),
        SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteMode AutoCompleteMode {
            get {
                return autoCompleteMode;
            }
            set {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoCompleteMode.None, (int)AutoCompleteMode.SuggestAppend)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoCompleteMode));
                }
                bool resetAutoComplete = false;
                if (autoCompleteMode != AutoCompleteMode.None && value == AutoCompleteMode.None) {
                    resetAutoComplete = true;
                }
                autoCompleteMode = value;
                SetAutoComplete(resetAutoComplete);
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.AutoCompleteSource"]/*' />
        /// <devdoc>
        ///     This is the AutoCompleteSource which can be one of the 
        ///     values from AutoCompleteSource enumeration. 
        ///     This property in conjunction with AutoCompleteMode enables the AutoComplete feature for TextBox.
        /// </devdoc>
        [
        DefaultValue(AutoCompleteSource.None),
        SRDescription(nameof(SR.TextBoxAutoCompleteSourceDescr)),
        TypeConverterAttribute(typeof(TextBoxAutoCompleteSourceConverter)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource {
            get {
                return autoCompleteSource;
            }
            set {
                // FxCop: Avoid usage of Enum.IsDefined - this looks like an enum that could grow
                if (!ClientUtils.IsEnumValid_NotSequential(value, 
                                             (int)value,
                                             (int)AutoCompleteSource.None, 
                                             (int)AutoCompleteSource.AllSystemSources,
                                             (int)AutoCompleteSource.AllUrl,
                                             (int)AutoCompleteSource.CustomSource,
                                             (int)AutoCompleteSource.FileSystem,
                                             (int)AutoCompleteSource.FileSystemDirectories,
                                             (int)AutoCompleteSource.HistoryList,
                                             (int)AutoCompleteSource.ListItems,
                                             (int)AutoCompleteSource.RecentlyUsedList)){   
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoCompleteSource));
                }
                if (value == AutoCompleteSource.ListItems) {
                    throw new NotSupportedException(SR.TextBoxAutoCompleteSourceNoItems);
                }

                if (value != AutoCompleteSource.None && value != AutoCompleteSource.CustomSource)
                {
                    FileIOPermission fiop = new FileIOPermission(PermissionState.Unrestricted);
                    fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
                    fiop.Demand();
                }

                autoCompleteSource = value;
                SetAutoComplete(false);
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.AutoCompleteCustomSource"]/*' />
        /// <devdoc>
        ///     This is the AutoCompleteCustomSource which is custom StringCollection used when the 
        ///     AutoCompleteSource is CustomSource. 
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteStringCollection AutoCompleteCustomSource {
            get {
                if (autoCompleteCustomSource == null) {
                    autoCompleteCustomSource = new AutoCompleteStringCollection();
                    autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                }
                return autoCompleteCustomSource;
            }
            set {
                if (autoCompleteCustomSource != value) {
                    if (autoCompleteCustomSource != null) {
                        autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                    }
                    
                    autoCompleteCustomSource = value;
                    
                    if (value != null) {
                        autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                    }
                    SetAutoComplete(false);
                }
                
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.CharacterCasing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets whether the TextBox control
        ///       modifies the case of characters as they are typed.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(CharacterCasing.Normal),
        SRDescription(nameof(SR.TextBoxCharacterCasingDescr))
        ]
        public CharacterCasing CharacterCasing {
            get {
                return characterCasing;
            }
            set {
                if (characterCasing != value) {
                    //verify that 'value' is a valid enum type...
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)CharacterCasing.Normal, (int)CharacterCasing.Lower)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CharacterCasing));
                    }

                    characterCasing = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.Multiline"]/*' />
        public override bool Multiline {
            get {
                return base.Multiline;
            }
            set {
                
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

        /// <devdoc>
        ///     Determines if the control is in password protect mode.
        /// </devdoc>
        internal override bool PasswordProtect {
            get {
                return this.PasswordChar != '\0';
            }
        }

     
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Returns the parameters needed to create the handle. Inheriting classes
        ///       can override this to provide extra functionality. They should not,
        ///       however, forget to call base.getCreateParams() first to get the struct
        ///       filled up with the basic info.
        ///    </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                switch (characterCasing) {
                    case CharacterCasing.Lower:
                        cp.Style |= NativeMethods.ES_LOWERCASE;
                        break;
                    case CharacterCasing.Upper:
                        cp.Style |= NativeMethods.ES_UPPERCASE;
                        break;
                }

                // Translate for Rtl if necessary
                //
                HorizontalAlignment align = RtlTranslateHorizontal(textAlign);
                cp.ExStyle &= ~NativeMethods.WS_EX_RIGHT;   // WS_EX_RIGHT overrides the ES_XXXX alignment styles
                switch (align) {
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
                
                if (Multiline) {
                    // Don't show horizontal scroll bars which won't do anything
                    if ((scrollBars & ScrollBars.Horizontal) == ScrollBars.Horizontal
                        && textAlign == HorizontalAlignment.Left
                        && !WordWrap) {
                        cp.Style |= NativeMethods.WS_HSCROLL;
                    }
                    if ((scrollBars & ScrollBars.Vertical) == ScrollBars.Vertical) {
                        cp.Style |= NativeMethods.WS_VSCROLL;
                    }
                }

                if (useSystemPasswordChar) {
                    cp.Style |= NativeMethods.ES_PASSWORD;
                }
                
                return cp;
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.PasswordChar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the character used to mask characters in a single-line text box
        ///       control used to enter passwords.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue((char)0),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxPasswordCharDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public char PasswordChar {
            get {
                if (!IsHandleCreated) {
                    CreateHandle();
                }
                return (char)SendMessage(NativeMethods.EM_GETPASSWORDCHAR, 0, 0);
            }
            set {
                passwordChar = value;
                if (!useSystemPasswordChar) {
                    if (IsHandleCreated) {
                        if (PasswordChar != value) {
                            // Set the password mode.
                            SendMessage(NativeMethods.EM_SETPASSWORDCHAR, value, 0);

                            // Disable IME if setting the control to password mode.
                            VerifyImeRestrictedModeChanged();

                            ResetAutoComplete(false);
                            Invalidate();
                        }
                    }
                }
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.ScrollBars"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets which scroll bars should
        ///       appear in a multiline <see cref='System.Windows.Forms.TextBox'/>
        ///       control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(ScrollBars.None),
        SRDescription(nameof(SR.TextBoxScrollBarsDescr))
        ]
        public ScrollBars ScrollBars {
            get {
                return scrollBars;
            }
            set {
                if (scrollBars != value) {
                    //valid values are 0x0 to 0x3
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ScrollBars.None, (int)ScrollBars.Both)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ScrollBars));
                    }

                    scrollBars = value;
                    RecreateHandle();
                }
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints) {
            Size scrollBarPadding = Size.Empty;

            if(Multiline && !WordWrap && (ScrollBars & ScrollBars.Horizontal) != 0) {
                scrollBarPadding.Height += SystemInformation.GetHorizontalScrollBarHeightForDpi(deviceDpi);
            }
            if(Multiline && (ScrollBars & ScrollBars.Vertical) != 0) {
                scrollBarPadding.Width += SystemInformation.GetVerticalScrollBarWidthForDpi(deviceDpi);
            }

            // Subtract the scroll bar padding before measuring
            proposedConstraints -= scrollBarPadding;
            
            Size prefSize = base.GetPreferredSizeCore(proposedConstraints);

            return prefSize + scrollBarPadding;
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the current text in the text box.
        ///    </para>
        /// </devdoc>
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
                selectionSet = false;
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.TextAlign"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets how text is
        ///       aligned in a <see cref='System.Windows.Forms.TextBox'/>
        ///       control.
        ///       Note: This code is duplicated in MaskedTextBox for simplicity.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        SRDescription(nameof(SR.TextBoxTextAlignDescr))
        ]
        public HorizontalAlignment TextAlign {
            get {
                return textAlign;
            }
            set {
                if (textAlign != value) {
                    //verify that 'value' is a valid enum type...

                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                    }

                    textAlign = value;
                    RecreateHandle();
                    OnTextAlignChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.IsPasswordMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///    Indicates if the text in the edit control should appear as
        ///    the default password character. This property has precedence
        ///    over the PasswordChar property.  Whenever the UseSystemPasswordChar
        ///    is set to true, the default system password character is used,
        ///    any character set into PasswordChar is ignored.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxUseSystemPasswordCharDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public bool UseSystemPasswordChar {
            get {
                return useSystemPasswordChar;
            }
            set {
                if (value != useSystemPasswordChar) {
                    useSystemPasswordChar = value;

                    // RecreateHandle will update IME restricted mode.
                    RecreateHandle();

                    if (value) {
                        ResetAutoComplete(false);
                    }
                }
            }
        }
        
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.TextAlignChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))]
        public event EventHandler TextAlignChanged {
            add {
                Events.AddHandler(EVENT_TEXTALIGNCHANGED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_TEXTALIGNCHANGED, value);
            }
        }

        /// <include file='doc\TabControl.uex' path='docs/doc[@for="TextBox.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                // Reset this just in case, because the SHAutoComplete stuff
                // will subclass this guys wndproc (and nativewindow can't know about it).
                // so this will undo it, but on a dispose we'll be Destroying the window anyay.
                //
                ResetAutoComplete(true);
                if (autoCompleteCustomSource != null) {
                    autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                }
                if (stringSource != null)
                {
                    stringSource.ReleaseAutoComplete();
                    stringSource = null;
                }
            }
            base.Dispose(disposing);
        }
       
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.IsInputKey"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Overridden to handle RETURN key.
        ///    </para>
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            if (Multiline && (keyData & Keys.Alt) == 0) {
                switch (keyData & Keys.KeyCode) {
                    case Keys.Return:
                        return acceptsReturn;
                }
            }
            return base.IsInputKey(keyData);
        }


        private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e) {
            if (AutoCompleteSource == AutoCompleteSource.CustomSource)
            {
                SetAutoComplete(true);
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.OnBackColorChanged"]/*' />
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            // Force repainting of the entire window frame
            if (Application.RenderWithVisualStyles && this.IsHandleCreated && this.BorderStyle == BorderStyle.Fixed3D) {
                SafeNativeMethods.RedrawWindow(new HandleRef(this, this.Handle), null, NativeMethods.NullHandleRef, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.OnFontChanged"]/*' />
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            if (this.AutoCompleteMode != AutoCompleteMode.None) {
                //we always will recreate the handle when autocomplete mode is on
                RecreateHandle();
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.OnGotFocus"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Overrideen to focus the text on first focus.
        /// </devdoc>
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            if (!selectionSet) {
                // We get one shot at selecting when we first get focus.  If we don't
                // do it, we still want to act like the selection was set.
                selectionSet = true;

                // If the user didn't provide a selection, force one in.
                if (SelectionLength == 0 && Control.MouseButtons == MouseButtons.None) {
                    SelectAll();
                }
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.OnHandleCreated"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Overridden to update the newly created handle with the settings of the
        ///    PasswordChar properties.
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            base.SetSelectionOnHandle();

            if (passwordChar != 0) {
                if (!useSystemPasswordChar) {
                    SendMessage(NativeMethods.EM_SETPASSWORDCHAR, passwordChar, 0);
                }
            }

            VerifyImeRestrictedModeChanged();

            if (AutoCompleteMode != AutoCompleteMode.None) {
                try
                {
                    fromHandleCreate = true;
                    SetAutoComplete(false);
                }
                finally
                {
                    fromHandleCreate = false;
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (stringSource != null)
            {
                stringSource.ReleaseAutoComplete();
                stringSource = null;
            }
            base.OnHandleDestroyed(e);
        }
        
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.OnTextAlignChanged"]/*' />
        protected virtual void OnTextAlignChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_TEXTALIGNCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.ProcessCmdKey"]/*' />
        /// <devdoc>
        /// Process a command key.
        /// Native "EDIT" control does not support "Select All" shorcut represented by Ctrl-A keys, when in multiline mode,
        /// and historically Winforms TextBox did not support it either.
        /// We are adding support for this shortcut for application targeting 4.6.1 and newer and for applications targeting 4.0 and newer 
        /// versions of the .NET Framework if they opt into this feature by adding the following config switch to the 'runtime' section of the app.config file:
        ///   <runtime>
        ///       <AppContextSwitchOverrides value = "Switch.System.Windows.Forms.DoNotSupportSelectAllShortcutInMultilineTextBox=false" />
        ///   </ runtime>
        /// To opt out of this feature, when targeting 4.6.1 and newer, please set the above mentioned switch to true. 
        /// <para>
        ///  m - the current windows message
        /// keyData - bitmask containing one or more keys
        /// </para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref Message m, Keys keyData) {
            bool returnValue = base.ProcessCmdKey(ref m, keyData);
            if (!returnValue && this.Multiline && !LocalAppContextSwitches.DoNotSupportSelectAllShortcutInMultilineTextBox 
                && this.ShortcutsEnabled && (keyData == (Keys.Control | Keys.A))) {
                SelectAll();
                return true;
            }

            return returnValue;
        }

        /// <devdoc>
        ///     Replaces the portion of the text specified by startPos and length with the one passed in,
        ///     without resetting the undo buffer (if any).
        ///     This method is provided as an alternative to SelectedText which clears the undo buffer.
        ///     Observe that this method does not honor the MaxLength property as the parameter-less base's
        ///     Paste does
        /// </devdoc>
        public void Paste(string text){
            base.SetSelectedTextInternal(text, false);
        }
     
        /// <devdoc>
        ///     Performs the actual select without doing arg checking.
        /// </devdoc>        
        internal override void SelectInternal(int start, int length, int textLen) {
            // If user set selection into text box, mark it so we don't
            // clobber it when we get focus.
            selectionSet = true;
            base.SelectInternal( start, length, textLen );
        }

        private string[] GetStringsForAutoComplete()
        {
            string[] strings = new string[AutoCompleteCustomSource.Count];
            for (int i = 0; i < AutoCompleteCustomSource.Count; i++) {
                strings[i] = AutoCompleteCustomSource[i];
            }
            return strings;
        }



        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.SetAutoComplete"]/*' />
        /// <devdoc>
        ///     Sets the AutoComplete mode in TextBox.
        /// </devdoc>
        internal void SetAutoComplete(bool reset)
        {
            //Autocomplete Not Enabled for Password enabled and MultiLine Textboxes.
            if (Multiline || passwordChar != 0 || useSystemPasswordChar || AutoCompleteSource == AutoCompleteSource.None) {
                return;
            }

            if (AutoCompleteMode != AutoCompleteMode.None) {
                if (!fromHandleCreate)
                {
                    //RecreateHandle to avoid Leak.
                    // notice the use of member variable to avoid re-entrancy
                    AutoCompleteMode backUpMode = this.AutoCompleteMode;
                    autoCompleteMode = AutoCompleteMode.None;
                    RecreateHandle();
                    autoCompleteMode = backUpMode;
                }
                
                if (AutoCompleteSource == AutoCompleteSource.CustomSource) {
                    if (IsHandleCreated && AutoCompleteCustomSource != null) {
                        if (AutoCompleteCustomSource.Count == 0) {
                            ResetAutoComplete(true);
                        }
                        else {
                            if (stringSource == null)
                            {
                                stringSource = new StringSource(GetStringsForAutoComplete());
                                if (!stringSource.Bind(new HandleRef(this, Handle), (int)AutoCompleteMode))
                                {
                                   throw new ArgumentException(SR.AutoCompleteFailure);
                                }
                            }
                            else
                            {
                                stringSource.RefreshList(GetStringsForAutoComplete());
                            }
                        }
                    }
        
                }
                else {
                    try {
                        if (IsHandleCreated) {
                            int mode = 0;
                            if (AutoCompleteMode == AutoCompleteMode.Suggest) {
                                mode |=  NativeMethods.AUTOSUGGEST | NativeMethods.AUTOAPPEND_OFF;
                            }
                            if (AutoCompleteMode == AutoCompleteMode.Append) {
                                mode |=  NativeMethods.AUTOAPPEND | NativeMethods.AUTOSUGGEST_OFF;
                            }
                            if (AutoCompleteMode == AutoCompleteMode.SuggestAppend) {
                                mode |=  NativeMethods.AUTOSUGGEST;
                                mode |=  NativeMethods.AUTOAPPEND;
                            }
                            int ret = SafeNativeMethods.SHAutoComplete(new HandleRef(this, Handle) , (int)AutoCompleteSource | mode);
                        }
                    }
                    catch (SecurityException) {
                        // If we don't have full trust, degrade gracefully. Allow the control to
                        // function without auto-complete. Allow the app to continue running.
                    }
                }
            }
            else if (reset) {
                ResetAutoComplete(true);
            }
        }


        // <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.ResetAutoComplete"]/*' />
        /// <devdoc>
        ///     Resets the AutoComplete mode in TextBox.
        /// </devdoc>
        private void ResetAutoComplete(bool force) {
            if ((AutoCompleteMode != AutoCompleteMode.None || force) && IsHandleCreated) {
                int mode = (int)AutoCompleteSource.AllSystemSources | NativeMethods.AUTOSUGGEST_OFF | NativeMethods.AUTOAPPEND_OFF;
                SafeNativeMethods.SHAutoComplete(new HandleRef(this, Handle) , mode);
            }
        }

        private void ResetAutoCompleteCustomSource() {
            AutoCompleteCustomSource = null;
        }

        private void WmPrint(ref Message m) {
            base.WndProc(ref m);
            if ((NativeMethods.PRF_NONCLIENT & (int)m.LParam) != 0 && Application.RenderWithVisualStyles && this.BorderStyle == BorderStyle.Fixed3D) {
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
	
	//-------------------------------------------------------------------------------------------------
        
        /// <include file='doc\TextBox.uex' path='docs/doc[@for="TextBox.WndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    The edits window procedure.  Inheritng classes can override this
        ///    to add extra functionality, but should not forget to call
        ///    base.wndProc(m); to ensure the combo continues to function properly.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                // Work around a very obscure Windows issue.
                case NativeMethods.WM_LBUTTONDOWN:
                    MouseButtons realState = MouseButtons;
                    bool wasValidationCancelled = ValidationCancelled;
                    FocusInternal();
                    if (realState == MouseButtons && 
                       (!ValidationCancelled || wasValidationCancelled)) {
                           base.WndProc(ref m);
                    }                    
                    break;
                //for readability ... so that we know whats happening ...
                // case WM_LBUTTONUP is included here eventhough it just calls the base.
                case NativeMethods.WM_LBUTTONUP:  
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_PRINT:
                    WmPrint(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
            
        }
        
    }
}
