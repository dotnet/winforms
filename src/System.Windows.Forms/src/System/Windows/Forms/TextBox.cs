// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a Windows text box control.
    /// </summary>
    [
      ClassInterface(ClassInterfaceType.AutoDispatch),
      ComVisible(true),
      Designer("System.Windows.Forms.Design.TextBoxDesigner, " + AssemblyRef.SystemDesign),
      SRDescription(nameof(SR.DescriptionTextBox))
    ]
    public class TextBox : TextBoxBase
    {
        private static readonly object EVENT_TEXTALIGNCHANGED = new object();

        /// <summary>
        ///  Controls whether or not the edit box consumes/respects ENTER key
        ///  presses.  While this is typically desired by multiline edits, this
        ///  can interfere with normal key processing in a dialog.
        /// </summary>
        private bool acceptsReturn = false;

        /// <summary>
        ///  Indicates what the current special password character is.  This is
        ///  displayed instead of any other text the user might enter.
        /// </summary>
        private char passwordChar = (char)0;

        private bool useSystemPasswordChar;

        /// <summary>
        ///  Controls whether or not the case of characters entered into the edit
        ///  box is forced to a specific case.
        /// </summary>
        private CharacterCasing characterCasing = System.Windows.Forms.CharacterCasing.Normal;

        /// <summary>
        ///  Controls which scrollbars appear by default.
        /// </summary>
        private ScrollBars scrollBars = System.Windows.Forms.ScrollBars.None;

        /// <summary>
        ///  Controls text alignment in the edit box.
        /// </summary>
        private HorizontalAlignment textAlign = HorizontalAlignment.Left;

        /// <summary>
        ///  True if the selection has been set by the user.  If the selection has
        ///  never been set and we get focus, we focus all the text in the control
        ///  so we mimic the Windows dialog manager.
        /// </summary>
        private bool selectionSet = false;

        /// <summary>
        ///  This stores the value for the autocomplete mode which can be either
        ///  None, AutoSuggest, AutoAppend or AutoSuggestAppend.
        /// </summary>
        private AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;

        /// <summary>
        ///  This stores the value for the autoCompleteSource mode which can be one of the values
        ///  from AutoCompleteSource enum.
        /// </summary>
        private AutoCompleteSource autoCompleteSource = AutoCompleteSource.None;

        /// <summary>
        ///  This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
        /// </summary>
        private AutoCompleteStringCollection autoCompleteCustomSource;
        private bool fromHandleCreate = false;
        private StringSource stringSource = null;
        private string placeholderText = string.Empty;

        public TextBox()
        {
        }

        /// <summary>
        ///  Gets or sets a value indicating whether pressing ENTER
        ///  in a multiline <see cref='TextBox'/>
        ///  control creates a new line of text in the control or activates the default button
        ///  for the form.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxAcceptsReturnDescr))
        ]
        public bool AcceptsReturn
        {
            get
            {
                return acceptsReturn;
            }

            set
            {
                acceptsReturn = value;
            }
        }

        /// <summary>
        ///  This is the AutoCompleteMode which can be either
        ///  None, AutoSuggest, AutoAppend or AutoSuggestAppend.
        ///  This property in conjunction with AutoCompleteSource enables the AutoComplete feature for TextBox.
        /// </summary>
        [
        DefaultValue(AutoCompleteMode.None),
        SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return autoCompleteMode;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoCompleteMode.None, (int)AutoCompleteMode.SuggestAppend))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoCompleteMode));
                }
                bool resetAutoComplete = false;
                if (autoCompleteMode != AutoCompleteMode.None && value == AutoCompleteMode.None)
                {
                    resetAutoComplete = true;
                }
                autoCompleteMode = value;
                SetAutoComplete(resetAutoComplete);
            }
        }

        /// <summary>
        ///  This is the AutoCompleteSource which can be one of the
        ///  values from AutoCompleteSource enumeration.
        ///  This property in conjunction with AutoCompleteMode enables the AutoComplete feature for TextBox.
        /// </summary>
        [
        DefaultValue(AutoCompleteSource.None),
        SRDescription(nameof(SR.TextBoxAutoCompleteSourceDescr)),
        TypeConverter(typeof(TextBoxAutoCompleteSourceConverter)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource
        {
            get
            {
                return autoCompleteSource;
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
                        autoCompleteSource = value;
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
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get
            {
                if (autoCompleteCustomSource == null)
                {
                    autoCompleteCustomSource = new AutoCompleteStringCollection();
                    autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                }
                return autoCompleteCustomSource;
            }
            set
            {
                if (autoCompleteCustomSource != value)
                {
                    if (autoCompleteCustomSource != null)
                    {
                        autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                    }

                    autoCompleteCustomSource = value;

                    if (value != null)
                    {
                        autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                    }
                    SetAutoComplete(false);
                }

            }
        }

        /// <summary>
        ///  Gets or sets whether the TextBox control
        ///  modifies the case of characters as they are typed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(CharacterCasing.Normal),
        SRDescription(nameof(SR.TextBoxCharacterCasingDescr))
        ]
        public CharacterCasing CharacterCasing
        {
            get
            {
                return characterCasing;
            }
            set
            {
                if (characterCasing != value)
                {
                    //verify that 'value' is a valid enum type...
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)CharacterCasing.Normal, (int)CharacterCasing.Lower))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CharacterCasing));
                    }

                    characterCasing = value;
                    RecreateHandle();
                }
            }
        }

        public override bool Multiline
        {
            get
            {
                return base.Multiline;
            }
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
        internal override bool PasswordProtect
        {
            get
            {
                return PasswordChar != '\0';
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
                switch (characterCasing)
                {
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

                if (Multiline)
                {
                    // Don't show horizontal scroll bars which won't do anything
                    if ((scrollBars & ScrollBars.Horizontal) == ScrollBars.Horizontal
                        && textAlign == HorizontalAlignment.Left
                        && !WordWrap)
                    {
                        cp.Style |= NativeMethods.WS_HSCROLL;
                    }
                    if ((scrollBars & ScrollBars.Vertical) == ScrollBars.Vertical)
                    {
                        cp.Style |= NativeMethods.WS_VSCROLL;
                    }
                }

                if (useSystemPasswordChar)
                {
                    cp.Style |= NativeMethods.ES_PASSWORD;
                }

                return cp;
            }
        }

        /// <summary>
        ///  Gets or sets the character used to mask characters in a single-line text box
        ///  control used to enter passwords.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue((char)0),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxPasswordCharDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public char PasswordChar
        {
            get
            {
                if (!IsHandleCreated)
                {
                    CreateHandle();
                }
                return (char)SendMessage(EditMessages.EM_GETPASSWORDCHAR, 0, 0);
            }
            set
            {
                passwordChar = value;
                if (!useSystemPasswordChar)
                {
                    if (IsHandleCreated)
                    {
                        if (PasswordChar != value)
                        {
                            // Set the password mode.
                            SendMessage(EditMessages.EM_SETPASSWORDCHAR, value, 0);

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
        ///  appear in a multiline <see cref='TextBox'/>
        ///  control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(ScrollBars.None),
        SRDescription(nameof(SR.TextBoxScrollBarsDescr))
        ]
        public ScrollBars ScrollBars
        {
            get
            {
                return scrollBars;
            }
            set
            {
                if (scrollBars != value)
                {
                    //valid values are 0x0 to 0x3
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ScrollBars.None, (int)ScrollBars.Both))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ScrollBars));
                    }

                    scrollBars = value;
                    RecreateHandle();
                }
            }
        }

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
        ///  Gets or sets
        ///  the current text in the text box.
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                selectionSet = false;
            }
        }

        /// <summary>
        ///  Gets or sets how text is
        ///  aligned in a <see cref='TextBox'/>
        ///  control.
        ///  Note: This code is duplicated in MaskedTextBox for simplicity.
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
        ///  Indicates if the text in the edit control should appear as
        ///  the default password character. This property has precedence
        ///  over the PasswordChar property.  Whenever the UseSystemPasswordChar
        ///  is set to true, the default system password character is used,
        ///  any character set into PasswordChar is ignored.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxUseSystemPasswordCharDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public bool UseSystemPasswordChar
        {
            get
            {
                return useSystemPasswordChar;
            }
            set
            {
                if (value != useSystemPasswordChar)
                {
                    useSystemPasswordChar = value;

                    // RecreateHandle will update IME restricted mode.
                    RecreateHandle();

                    if (value)
                    {
                        ResetAutoComplete(false);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))]
        public event EventHandler TextAlignChanged
        {
            add => Events.AddHandler(EVENT_TEXTALIGNCHANGED, value);

            remove => Events.RemoveHandler(EVENT_TEXTALIGNCHANGED, value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Reset this just in case, because the SHAutoComplete stuff
                // will subclass this guys wndproc (and nativewindow can't know about it).
                // so this will undo it, but on a dispose we'll be Destroying the window anyay.
                //
                ResetAutoComplete(true);
                if (autoCompleteCustomSource != null)
                {
                    autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                }
                if (stringSource != null)
                {
                    stringSource.ReleaseAutoComplete();
                    stringSource = null;
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
                        return acceptsReturn;
                }
            }
            return base.IsInputKey(keyData);
        }

        private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e)
        {
            if (AutoCompleteSource == AutoCompleteSource.CustomSource)
            {
                SetAutoComplete(true);
            }
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

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (AutoCompleteMode != AutoCompleteMode.None)
            {
                //we always will recreate the handle when autocomplete mode is on
                RecreateHandle();
            }
        }

        /// <summary>
        ///  Overrideen to focus the text on first focus.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (!selectionSet)
            {
                // We get one shot at selecting when we first get focus.  If we don't
                // do it, we still want to act like the selection was set.
                selectionSet = true;

                // If the user didn't provide a selection, force one in.
                if (SelectionLength == 0 && Control.MouseButtons == MouseButtons.None)
                {
                    SelectAll();
                }
            }
        }

        /// <summary>
        ///  Overridden to update the newly created handle with the settings of the
        ///  PasswordChar properties.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            base.SetSelectionOnHandle();

            if (passwordChar != 0)
            {
                if (!useSystemPasswordChar)
                {
                    SendMessage(EditMessages.EM_SETPASSWORDCHAR, passwordChar, 0);
                }
            }

            VerifyImeRestrictedModeChanged();

            if (AutoCompleteMode != AutoCompleteMode.None)
            {
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

        protected virtual void OnTextAlignChanged(EventArgs e)
        {
            if (Events[EVENT_TEXTALIGNCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Process a command key.
        ///  Native "EDIT" control does not support "Select All" shorcut represented by Ctrl-A keys, when in multiline mode,
        ///  Winforms TextBox supports this in .NET.
        /// </summary>
        /// <param name="m">The current windows message.</param>
        /// <param name="keyData">The bitmask containing one or more keys.</param>
        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            bool returnValue = base.ProcessCmdKey(ref m, keyData);
            if (!returnValue && Multiline && ShortcutsEnabled && (keyData == (Keys.Control | Keys.A)))
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
        public void Paste(string text)
        {
            base.SetSelectedTextInternal(text, false);
        }

        /// <summary>
        ///  Performs the actual select without doing arg checking.
        /// </summary>
        internal override void SelectInternal(int start, int length, int textLen)
        {
            // If user set selection into text box, mark it so we don't
            // clobber it when we get focus.
            selectionSet = true;
            base.SelectInternal(start, length, textLen);
        }

        private string[] GetStringsForAutoComplete()
        {
            string[] strings = new string[AutoCompleteCustomSource.Count];
            for (int i = 0; i < AutoCompleteCustomSource.Count; i++)
            {
                strings[i] = AutoCompleteCustomSource[i];
            }
            return strings;
        }

        /// <summary>
        ///  Sets the AutoComplete mode in TextBox.
        /// </summary>
        internal void SetAutoComplete(bool reset)
        {
            //Autocomplete Not Enabled for Password enabled and MultiLine Textboxes.
            if (Multiline || passwordChar != 0 || useSystemPasswordChar || AutoCompleteSource == AutoCompleteSource.None)
            {
                return;
            }

            if (AutoCompleteMode != AutoCompleteMode.None)
            {
                if (!fromHandleCreate)
                {
                    //RecreateHandle to avoid Leak.
                    // notice the use of member variable to avoid re-entrancy
                    AutoCompleteMode backUpMode = AutoCompleteMode;
                    autoCompleteMode = AutoCompleteMode.None;
                    RecreateHandle();
                    autoCompleteMode = backUpMode;
                }

                if (AutoCompleteSource == AutoCompleteSource.CustomSource)
                {
                    if (IsHandleCreated && AutoCompleteCustomSource != null)
                    {
                        if (AutoCompleteCustomSource.Count == 0)
                        {
                            ResetAutoComplete(true);
                        }
                        else
                        {
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
                else
                {
                    if (IsHandleCreated)
                    {
                        int mode = 0;
                        if (AutoCompleteMode == AutoCompleteMode.Suggest)
                        {
                            mode |= NativeMethods.AUTOSUGGEST | NativeMethods.AUTOAPPEND_OFF;
                        }
                        if (AutoCompleteMode == AutoCompleteMode.Append)
                        {
                            mode |= NativeMethods.AUTOAPPEND | NativeMethods.AUTOSUGGEST_OFF;
                        }
                        if (AutoCompleteMode == AutoCompleteMode.SuggestAppend)
                        {
                            mode |= NativeMethods.AUTOSUGGEST;
                            mode |= NativeMethods.AUTOAPPEND;
                        }
                        int ret = SafeNativeMethods.SHAutoComplete(new HandleRef(this, Handle), (int)AutoCompleteSource | mode);
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
                int mode = (int)AutoCompleteSource.AllSystemSources | NativeMethods.AUTOSUGGEST_OFF | NativeMethods.AUTOAPPEND_OFF;
                SafeNativeMethods.SHAutoComplete(new HandleRef(this, Handle), mode);
            }
        }

        private void ResetAutoCompleteCustomSource()
        {
            AutoCompleteCustomSource = null;
        }

        private void WmPrint(ref Message m)
        {
            base.WndProc(ref m);
            if ((NativeMethods.PRF_NONCLIENT & (int)m.LParam) != 0 && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
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
        ///  Gets or sets the text that is displayed when the control has no text and does not have the focus.
        /// </summary>
        /// <value>The text that is displayed when the control has no text and does not have the focus.</value>
        [
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.TextBoxPlaceholderTextDescr))
        ]
        public virtual string PlaceholderText
        {
            get
            {
                return placeholderText;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (placeholderText != value)
                {
                    placeholderText = value;
                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        ///  Draws the <see cref="PlaceholderText"/> in the client area of the <see cref="TextBox"/> using the default font and color.
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

            TextRenderer.DrawText(graphics, PlaceholderText, Font, rectangle, SystemColors.GrayText, BackColor, flags);
        }

        /// <summary>
        ///  The edits window procedure.  Inheritng classes can override this
        ///  to add extra functionality, but should not forget to call
        ///  base.wndProc(m); to ensure the combo continues to function properly.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // Work around a very obscure Windows issue.
                case WindowMessages.WM_LBUTTONDOWN:
                    MouseButtons realState = MouseButtons;
                    bool wasValidationCancelled = ValidationCancelled;
                    Focus();
                    if (realState == MouseButtons &&
                       (!ValidationCancelled || wasValidationCancelled))
                    {
                        base.WndProc(ref m);
                    }
                    break;
                //for readability ... so that we know whats happening ...
                // case WM_LBUTTONUP is included here eventhough it just calls the base.
                case WindowMessages.WM_LBUTTONUP:
                    base.WndProc(ref m);
                    break;
                case WindowMessages.WM_PRINT:
                    WmPrint(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }

            if (ShouldRenderPlaceHolderText(m))
            {
                using (Graphics g = CreateGraphics())
                {
                    DrawPlaceholderText(g);
                }
            }
        }

        private bool ShouldRenderPlaceHolderText(in Message m) =>
                    !string.IsNullOrEmpty(PlaceholderText) &&
                    (m.Msg == WindowMessages.WM_PAINT || m.Msg == WindowMessages.WM_KILLFOCUS) &&
                    !GetStyle(ControlStyles.UserPaint) &&
                    !Focused &&
                    TextLength == 0;

        internal TestAccessor GetTestAccessor() => new TestAccessor(this);

        internal readonly struct TestAccessor
        {
            private readonly TextBox _textBox;

            public TestAccessor(TextBox textBox)
            {
                _textBox = textBox;
            }

            public bool ShouldRenderPlaceHolderText(in Message m) => _textBox.ShouldRenderPlaceHolderText(m);
        }

    }
}
