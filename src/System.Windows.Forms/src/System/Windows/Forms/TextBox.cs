﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Windows.Forms;
    using System.ComponentModel.Design;    
    using System.Drawing;
    using Microsoft.Win32;
    using System.Reflection;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Collections.Specialized;
    using System.Drawing.Design;
    using System.Windows.Forms.VisualStyles;
    
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

        /// <devdoc>
        ///     This stores the value for the autocomplete mode which can be either
        ///     None, AutoSuggest, AutoAppend or AutoSuggestAppend.
        /// </devdoc>
        private AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;
        
        /// <devdoc>
        ///     This stores the value for the autoCompleteSource mode which can be one of the values
        ///     from AutoCompleteSource enum.
        /// </devdoc>
        private AutoCompleteSource autoCompleteSource = AutoCompleteSource.None;

        /// <devdoc>
        ///     This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
        /// </devdoc>
        private AutoCompleteStringCollection autoCompleteCustomSource;
        private bool fromHandleCreate = false;
        private StringSource stringSource = null;
        private string placeholderText;

        public TextBox(){
        }

        
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

                autoCompleteSource = value;
                SetAutoComplete(false);
            }
        }

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

     
        /// <devdoc>
        ///    <para>
        ///       Returns the parameters needed to create the handle. Inheriting classes
        ///       can override this to provide extra functionality. They should not,
        ///       however, forget to call base.getCreateParams() first to get the struct
        ///       filled up with the basic info.
        ///    </para>
        /// </devdoc>
        protected override CreateParams CreateParams {
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
                return (char)SendMessage(Interop.EditMessages.EM_GETPASSWORDCHAR, 0, 0);
            }
            set {
                passwordChar = value;
                if (!useSystemPasswordChar) {
                    if (IsHandleCreated) {
                        if (PasswordChar != value) {
                            // Set the password mode.
                            SendMessage(Interop.EditMessages.EM_SETPASSWORDCHAR, value, 0);

                            // Disable IME if setting the control to password mode.
                            VerifyImeRestrictedModeChanged();

                            ResetAutoComplete(false);
                            Invalidate();
                        }
                    }
                }
            }
        }

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
        
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))]
        public event EventHandler TextAlignChanged {
            add => Events.AddHandler(EVENT_TEXTALIGNCHANGED, value);

            remove => Events.RemoveHandler(EVENT_TEXTALIGNCHANGED, value);
        }

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

        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            // Force repainting of the entire window frame
            if (Application.RenderWithVisualStyles && this.IsHandleCreated && this.BorderStyle == BorderStyle.Fixed3D) {
                SafeNativeMethods.RedrawWindow(new HandleRef(this, this.Handle), null, NativeMethods.NullHandleRef, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
            }
        }

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            if (this.AutoCompleteMode != AutoCompleteMode.None) {
                //we always will recreate the handle when autocomplete mode is on
                RecreateHandle();
            }
        }

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

        /// <devdoc>
        ///    Overridden to update the newly created handle with the settings of the
        ///    PasswordChar properties.
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            base.SetSelectionOnHandle();

            if (passwordChar != 0) {
                if (!useSystemPasswordChar) {
                    SendMessage(Interop.EditMessages.EM_SETPASSWORDCHAR, passwordChar, 0);
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
        
        protected virtual void OnTextAlignChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_TEXTALIGNCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

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
            }
            else if (reset) {
                ResetAutoComplete(true);
            }
        }


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
                using (Graphics g = Graphics.FromHdc(m.WParam)) {
                    Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
                    using (Pen pen = new Pen(VisualStyleInformation.TextControlBorder)) {
                        g.DrawRectangle(pen, rect);
                    }
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(SystemPens.Window, rect);
                }
            }
        }
        
        /// <summary>
        ///  Gets or sets the text that is displayed when the control has no Text and is not on focus.
        /// </summary>
        [
        Localizable(true),
        DefaultValue(null),
        SRDescription(nameof(SR.TextBoxPlaceholderTextDescr))
        ]
        public string PlaceholderText
        {
            get
            {
                return placeholderText;
            }
            set
            {
                if (placeholderText != value)
                {
                    placeholderText = value;
                    Invalidate();
                }
            }
        }


        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// Draws the PlaceholderText in the client area of the TextBox using the default font and color.
        /// </summary>
        private void DrawPlaceholderText(Graphics graphics)
        {
            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top |
                                    TextFormatFlags.EndEllipsis;
            Rectangle rectangle = ClientRectangle;

            if (RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
                switch (TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags = flags | TextFormatFlags.HorizontalCenter;
                        rectangle.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags = flags | TextFormatFlags.Right;
                        rectangle.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags = flags | TextFormatFlags.Left;
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
                        flags = flags | TextFormatFlags.HorizontalCenter;
                        rectangle.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags = flags | TextFormatFlags.Left;
                        rectangle.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags = flags | TextFormatFlags.Right;
                        rectangle.Offset(0, 1);
                        break;
                }
            }

            TextRenderer.DrawText(graphics, PlaceholderText, Font, rectangle, SystemColors.GrayText, BackColor, flags);
        }

        /// <devdoc>
        ///    The edits window procedure.  Inheritng classes can override this
        ///    to add extra functionality, but should not forget to call
        ///    base.wndProc(m); to ensure the combo continues to function properly.
        /// </devdoc>
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                // Work around a very obscure Windows issue.
                case Interop.WindowMessages.WM_LBUTTONDOWN:
                    MouseButtons realState = MouseButtons;
                    bool wasValidationCancelled = ValidationCancelled;
                    Focus();
                    if (realState == MouseButtons && 
                       (!ValidationCancelled || wasValidationCancelled)) {
                           base.WndProc(ref m);
                    }                    
                    break;
                //for readability ... so that we know whats happening ...
                // case WM_LBUTTONUP is included here eventhough it just calls the base.
                case Interop.WindowMessages.WM_LBUTTONUP:  
                    base.WndProc(ref m);
                    break;
                case Interop.WindowMessages.WM_PRINT:
                    WmPrint(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }

            if ((m.Msg == Interop.WindowMessages.WM_PAINT || m.Msg == Interop.WindowMessages.WM_KILLFOCUS) &&
                 !this.GetStyle(ControlStyles.UserPaint) &&
                   string.IsNullOrEmpty(this.Text) &&
                   !this.Focused)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    DrawPlaceholderText(g);
                }
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            if (string.IsNullOrEmpty(Text))
            {
                AccessibleObject accessibleObject = base.CreateAccessibilityInstance();
                accessibleObject.Value = PlaceholderText;
                return accessibleObject;
            }
            else
            {
                return base.CreateAccessibilityInstance();
            }
        }

    }
}
