// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a Windows text box control.
    /// </summary>
    [Designer("System.Windows.Forms.Design.TextBoxDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionTextBox))]
    public class TextBox : TextBoxBase
    {
        private static readonly object EVENT_TEXTALIGNCHANGED = new object();

        /// <summary>
        ///  Controls whether or not the edit box consumes/respects ENTER key
        ///  presses.  While this is typically desired by multiline edits, this
        ///  can interfere with normal key processing in a dialog.
        /// </summary>
        private bool acceptsReturn;

        /// <summary>
        ///  Indicates what the current special password character is.  This is
        ///  displayed instead of any other text the user might enter.
        /// </summary>
        private char passwordChar;

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
        private bool selectionSet;

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
        private bool fromHandleCreate;
        private StringSource stringSource;
        private string placeholderText = string.Empty;

        public TextBox()
        {
        }

        /// <summary>
        ///  Gets or sets a value indicating whether pressing ENTER in a multiline <see cref='TextBox'/>
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
        [DefaultValue(AutoCompleteMode.None)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr))]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get
            {
                if (autoCompleteCustomSource is null)
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
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(CharacterCasing.Normal)]
        [SRDescription(nameof(SR.TextBoxCharacterCasingDescr))]
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
                switch (characterCasing)
                {
                    case CharacterCasing.Lower:
                        cp.Style |= (int)ES.LOWERCASE;
                        break;
                    case CharacterCasing.Upper:
                        cp.Style |= (int)ES.UPPERCASE;
                        break;
                }

                // Translate for Rtl if necessary
                HorizontalAlignment align = RtlTranslateHorizontal(textAlign);

                // WS_EX_RIGHT overrides the ES_XXXX alignment styles
                cp.ExStyle &= ~(int)WS_EX.RIGHT;

                switch (align)
                {
                    case HorizontalAlignment.Left:
                        cp.Style |= (int)ES.LEFT;
                        break;
                    case HorizontalAlignment.Center:
                        cp.Style |= (int)ES.CENTER;
                        break;
                    case HorizontalAlignment.Right:
                        cp.Style |= (int)ES.RIGHT;
                        break;
                }

                if (Multiline)
                {
                    // Don't show horizontal scroll bars which won't do anything
                    if ((scrollBars & ScrollBars.Horizontal) == ScrollBars.Horizontal
                        && textAlign == HorizontalAlignment.Left
                        && !WordWrap)
                    {
                        cp.Style |= (int)WS.HSCROLL;
                    }
                    if ((scrollBars & ScrollBars.Vertical) == ScrollBars.Vertical)
                    {
                        cp.Style |= (int)WS.VSCROLL;
                    }
                }

                if (useSystemPasswordChar)
                {
                    cp.Style |= (int)ES.PASSWORD;
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

                return (char)SendMessageW(this, (WM)EM.GETPASSWORDCHAR);
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
                            SendMessageW(this, (WM)EM.SETPASSWORDCHAR, (IntPtr)value);

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
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [DefaultValue(ScrollBars.None)]
        [SRDescription(nameof(SR.TextBoxScrollBarsDescr))]
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
        ///  Gets or sets the current text in the text box.
        /// </summary>
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                selectionSet = false;
            }
        }

        /// <summary>
        ///  Gets or sets how text is aligned in a <see cref='TextBox'/> control.
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
                return textAlign;
            }
            set
            {
                if (textAlign != value)
                {
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
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxUseSystemPasswordCharDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
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

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.RadioButtonOnTextAlignChangedDescr))]
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

        protected unsafe override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            // Force repainting of the entire window frame
            if (Application.RenderWithVisualStyles && IsHandleCreated && BorderStyle == BorderStyle.Fixed3D)
            {
                RedrawWindow(
                    new HandleRef(this, Handle),
                    null,
                    IntPtr.Zero,
                    RDW.INVALIDATE | RDW.FRAME);
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
            if (!IsHandleCreated)
            {
                return;
            }

            base.SetSelectionOnHandle();

            if (passwordChar != 0)
            {
                if (!useSystemPasswordChar)
                {
                    SendMessageW(this, (WM)EM.SETPASSWORDCHAR, (IntPtr)passwordChar);
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

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (IsHandleCreated && ContainsNavigationKeyCode(e.KeyCode))
            {
                AccessibilityObject?.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
            }
        }

        private bool ContainsNavigationKeyCode(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                case Keys.Left:
                case Keys.Right:
                    return true;
                default:
                    return false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsHandleCreated)
            {
                // As there is no corresponding windows notification
                // about text selection changed for TextBox assuming
                // that any mouse down on textbox leads to change of
                // the caret position and thereby change the selection.
                AccessibilityObject?.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
            }
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
        private protected override void SelectInternal(int start, int length, int textLen)
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
        private void SetAutoComplete(bool reset)
        {
            // Autocomplete Not Enabled for Password enabled and MultiLine Textboxes.
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
                            if (stringSource is null)
                            {
                                stringSource = new StringSource(GetStringsForAutoComplete());
                                if (!stringSource.Bind(new HandleRef(this, Handle), (Shell32.AUTOCOMPLETEOPTIONS)AutoCompleteMode))
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
                        Shlwapi.SHACF mode = Shlwapi.SHACF.DEFAULT;
                        if (AutoCompleteMode == AutoCompleteMode.Suggest)
                        {
                            mode |= Shlwapi.SHACF.AUTOSUGGEST_FORCE_ON | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF;
                        }
                        if (AutoCompleteMode == AutoCompleteMode.Append)
                        {
                            mode |= Shlwapi.SHACF.AUTOAPPEND_FORCE_ON | Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF;
                        }
                        if (AutoCompleteMode == AutoCompleteMode.SuggestAppend)
                        {
                            mode |= Shlwapi.SHACF.AUTOSUGGEST_FORCE_ON;
                            mode |= Shlwapi.SHACF.AUTOAPPEND_FORCE_ON;
                        }

                        Shlwapi.SHAutoComplete(this, (Shlwapi.SHACF)AutoCompleteSource | mode);
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
                Shlwapi.SHAutoComplete(this, (Shlwapi.SHACF)AutoCompleteSource.AllSystemSources | Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF);
            }
        }

        private void ResetAutoCompleteCustomSource()
        {
            AutoCompleteCustomSource = null;
        }

        private void WmPrint(ref Message m)
        {
            base.WndProc(ref m);
            if (((PRF)m.LParam & PRF.NONCLIENT) != 0 && Application.RenderWithVisualStyles
                && BorderStyle == BorderStyle.Fixed3D)
            {
                using Graphics g = Graphics.FromHdc(m.WParam);
                Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
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
        public virtual string PlaceholderText
        {
            get
            {
                return placeholderText;
            }
            set
            {
                if (value is null)
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
            switch ((User32.WM)m.Msg)
            {
                // Work around a very obscure Windows issue.
                case User32.WM.LBUTTONDOWN:
                    MouseButtons realState = MouseButtons;
                    bool wasValidationCancelled = ValidationCancelled;
                    Focus();
                    if (realState == MouseButtons &&
                       (!ValidationCancelled || wasValidationCancelled))
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case User32.WM.PRINT:
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
            (m.Msg == (int)User32.WM.PAINT || m.Msg == (int)User32.WM.KILLFOCUS) &&
            !GetStyle(ControlStyles.UserPaint) &&
            !Focused &&
            TextLength == 0;
    }
}
