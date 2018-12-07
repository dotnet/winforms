// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Text;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Layout;
    using System.ComponentModel.Design;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Design;
    using Microsoft.Win32;
    using System.Reflection;
    using System.Globalization;

    /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Implements the basic functionality required by text
    ///       controls.
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(TextChanged)),
    DefaultBindingProperty(nameof(Text)),
    Designer("System.Windows.Forms.Design.TextBoxBaseDesigner, " + AssemblyRef.SystemDesign)
    ]
    public abstract class TextBoxBase : Control {

        // The boolean properties for this control are contained in the textBoxFlags bit
        // vector.  We can store up to 32 boolean values in this one vector.  Here we
        // create the bitmasks for each bit in the vector.
        //
        private static readonly int autoSize         = BitVector32.CreateMask();
        private static readonly int hideSelection    = BitVector32.CreateMask(autoSize);
        private static readonly int multiline        = BitVector32.CreateMask(hideSelection);
        private static readonly int modified         = BitVector32.CreateMask(multiline);
        private static readonly int readOnly         = BitVector32.CreateMask(modified);
        private static readonly int acceptsTab       = BitVector32.CreateMask(readOnly);
        private static readonly int wordWrap         = BitVector32.CreateMask(acceptsTab);
        private static readonly int creatingHandle   = BitVector32.CreateMask(wordWrap);
        private static readonly int codeUpdateText   = BitVector32.CreateMask(creatingHandle);
        private static readonly int shortcutsEnabled   = BitVector32.CreateMask(codeUpdateText);
        private static readonly int scrollToCaretOnHandleCreated = BitVector32.CreateMask(shortcutsEnabled);
        private static readonly int setSelectionOnHandleCreated = BitVector32.CreateMask(scrollToCaretOnHandleCreated);

        private static readonly object EVENT_ACCEPTSTABCHANGED      = new object();
        private static readonly object EVENT_BORDERSTYLECHANGED     = new object();
        private static readonly object EVENT_HIDESELECTIONCHANGED   = new object();
        private static readonly object EVENT_MODIFIEDCHANGED        = new object();
        private static readonly object EVENT_MULTILINECHANGED       = new object();
        private static readonly object EVENT_READONLYCHANGED        = new object();

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.borderStyle"]/*' />
        /// <devdoc>
        ///     The current border for this edit control.
        /// </devdoc>
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.maxLength"]/*' />
        /// <devdoc>
        ///     Controls the maximum length of text in the edit control.
        /// </devdoc>
        private int maxLength = 32767; // Win9X default, used for consistency

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.requestedHeight"]/*' />
        /// <devdoc>
        ///     Used by the autoSizing code to help figure out the desired height of
        ///     the edit box.
        /// </devdoc>
        private int requestedHeight;
        bool integralHeightAdjust = false;

        //these indices are used to cache the values of the selection, by doing this
        //if the handle isn't created yet, we don't force a creation.
        private int selectionStart = 0;
        private int selectionLength = 0;

        /// <devdoc>
        ///     Controls firing of click event (Left click).
        ///     This is used by TextBox, RichTextBox and MaskedTextBox, code was moved down from TextBox/RichTextBox
        ///     but cannot make it as default behavior to avoid introducing breaking changes.
        /// </devdoc>
        private  bool doubleClickFired = false;

        private static int[] shortcutsToDisable;

        // We store all boolean properties in here.
        //
        private BitVector32 textBoxFlags = new BitVector32();

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.TextBoxBase"]/*' />
        /// <devdoc>
        ///     Creates a new TextBox control.  Uses the parent's current font and color
        ///     set.
        /// </devdoc>
        internal TextBoxBase() : base() {
        
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true); 
            
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

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.AcceptsTab"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       a value indicating whether pressing the TAB key
        ///       in a multiline text box control types
        ///       a TAB character in the control instead of moving the focus to the next control
        ///       in the tab order.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxAcceptsTabDescr))
        ]
        public bool AcceptsTab {
            get {
                return textBoxFlags[acceptsTab];
            }
            set {
                if (textBoxFlags[acceptsTab] != value) {
                    textBoxFlags[acceptsTab] = value;
                    OnAcceptsTabChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.AcceptsTabChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnAcceptsTabChangedDescr))]
        public event EventHandler AcceptsTabChanged {
            add {
                Events.AddHandler(EVENT_ACCEPTSTABCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_ACCEPTSTABCHANGED, value);
            }
        }


        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ShortcutsEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the following shortcuts should be enabled or not:
        ///       Ctrl-Z, Ctrl-C, Ctrl-X, Ctrl-V, Ctrl-A, Ctrl-L, Ctrl-R, Ctrl-E, Ctrl-I, Ctrl-Y,
        ///       Ctrl-BackSpace, Ctrl-Del, Shift-Del, Shift-Ins.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TextBoxShortcutsEnabledDescr))
        ]
        public virtual bool ShortcutsEnabled {
            get {
                return textBoxFlags[shortcutsEnabled];
            }
            set {
                if (shortcutsToDisable == null) {
                    shortcutsToDisable = new int[] {(int)Shortcut.CtrlZ, (int)Shortcut.CtrlC, (int)Shortcut.CtrlX,
                    (int)Shortcut.CtrlV, (int)Shortcut.CtrlA, (int)Shortcut.CtrlL, (int)Shortcut.CtrlR,
                    (int)Shortcut.CtrlE, (int)Shortcut.CtrlY, (int)Keys.Control + (int)Keys.Back,
                    (int)Shortcut.CtrlDel, (int)Shortcut.ShiftDel, (int)Shortcut.ShiftIns, (int)Shortcut.CtrlJ};
                }
                textBoxFlags[shortcutsEnabled] = value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for=TextBoxBase.ProcessCmdKey]/*'/ />
        /// <devdoc>
        ///     Implements the <see cref='System.Windows.Forms.TextBoxBase.ShortcutsEnabled'/> property.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            // First call parent's ProcessCmdKey, since we don't to eat up
            // the shortcut key we are not supported in TextBox.
            bool returnedValue = base.ProcessCmdKey(ref msg, keyData);

            if (this.ShortcutsEnabled == false) {
                foreach (int shortcutValue in shortcutsToDisable) {
                    if ((int)keyData == shortcutValue ||
                        (int)keyData == (shortcutValue | (int)Keys.Shift)) {
                        return true;
                    }
                }
            }
            //
            // There are a few keys that change the alignment of the text, but that
            // are not ignored by the native control when the ReadOnly property is set.
            // We need to workaround that.
            if (textBoxFlags[readOnly]) {
                int k = (int)keyData;
                if (k == (int)Shortcut.CtrlL        // align left
                    || k == (int)Shortcut.CtrlR     // align right
                    || k == (int)Shortcut.CtrlE     // align centre
                    || k == (int)Shortcut.CtrlJ) {  // align justified
                    return true;
                }
            }

            return returnedValue;
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.AutoSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the size
        ///       of the control automatically adjusts when the font assigned to the control
        ///       is changed.
        ///
        ///       Note: this works differently than other Controls' AutoSize, so we're hiding
        ///       it to avoid confusion.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxAutoSizeDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]

        public override bool AutoSize {
            get {
                return textBoxFlags[autoSize];
            }
            set {
                // Note that we intentionally do not call base.  TextBoxes size themselves by
                // overriding SetBoundsCore (old RTM code).  We let CommonProperties.GetAutoSize
                // continue to return false to keep our LayoutEngines from messing with TextBoxes.
                // This is done for backwards compatibility since the new AutoSize behavior differs.
                if (textBoxFlags[autoSize] != value) {
                    textBoxFlags[autoSize] = value;

                    // AutoSize's effects are ignored for a multi-line textbox
                    //
                    if (!Multiline) {
                        SetStyle(ControlStyles.FixedHeight, value);
                        AdjustHeight(false);
                    }

                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BackColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the background color of the control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DispId(NativeMethods.ActiveX.DISPID_BACKCOLOR),
        SRDescription(nameof(SR.ControlBackColorDescr))
        ]
        public override Color BackColor {
            get {
                if (ShouldSerializeBackColor()) {
                    return base.BackColor;
                }
                else if (this.ReadOnly) {
                    return SystemColors.Control;
                } else {
                    return SystemColors.Window;
                }
            }
            set {
                base.BackColor = value;
            }
        }


        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BackgroundImage"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }



        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.AutoSizeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged {
            add {
                base.AutoSizeChanged += value;
            }
            remove {
                base.AutoSizeChanged -= value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BackgroundImageLayout"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BorderStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the border type
        ///       of the text box control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.TextBoxBorderDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                if (borderStyle != value) {
                    //verify that 'value' is a valid enum type...

                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    borderStyle = value;
                    UpdateStyles();
                    RecreateHandle();            

                    // PreferredSize depends on BorderStyle : thru CreateParams.ExStyle in User32!AdjustRectEx.
                    // So when the BorderStyle changes let the parent of this control know about it.
                    using(LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.BorderStyle)) {
                        OnBorderStyleChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.BorderStyleChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnBorderStyleChangedDescr))]
        public event EventHandler BorderStyleChanged {
            add {
                Events.AddHandler(EVENT_BORDERSTYLECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_BORDERSTYLECHANGED, value);
            }
        }

        internal virtual bool CanRaiseTextChangedEvent {
            get {
                return true;
            }
        }

        /// <devdoc>
        ///     Specifies whether the ImeMode can be enabled - See also ImeModeBase.
        /// </devdoc>
        protected override bool CanEnableIme {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CanEnableIme(), this = " + this );
                Debug.Indent();

                bool canEnable = !( this.ReadOnly || this.PasswordProtect ) && base.CanEnableIme;

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + canEnable );
                Debug.Unindent();

                return canEnable;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.CanUndo"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value
        ///       indicating whether the user can undo the previous operation in a text box control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxCanUndoDescr))
        ]
        public bool CanUndo {
            get {
                if (IsHandleCreated) {
                    bool b;
                    b = unchecked( (int) (long)SendMessage(NativeMethods.EM_CANUNDO, 0, 0)) != 0;

                    return b;
                }
                return false;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.CreateParams"]/*' />
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
                cp.ClassName = "EDIT";
                cp.Style |= NativeMethods.ES_AUTOHSCROLL | NativeMethods.ES_AUTOVSCROLL;
                if (!textBoxFlags[hideSelection]) cp.Style |= NativeMethods.ES_NOHIDESEL;
                if (textBoxFlags[readOnly]) cp.Style |= NativeMethods.ES_READONLY;
                cp.ExStyle &= (~NativeMethods.WS_EX_CLIENTEDGE);
                cp.Style &= (~NativeMethods.WS_BORDER);

                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }
                if (textBoxFlags[multiline]) {
                    cp.Style |= NativeMethods.ES_MULTILINE;
                    if (textBoxFlags[wordWrap]) cp.Style &= ~NativeMethods.ES_AUTOHSCROLL;
                }

                return cp;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.DoubleBuffered"]/*' />
        /// <devdoc>
        ///     This property is overridden and hidden from statement completion
        ///     on controls that are based on Win32 Native Controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered {
            get {
                return base.DoubleBuffered;
            }
            set {
                base.DoubleBuffered = value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Click"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler Click {
            add {
                base.Click += value;
            }
            remove {
                base.Click -= value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.MouseClick"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event MouseEventHandler MouseClick {
            add {
                base.MouseClick += value;
            }
            remove {
                base.MouseClick -= value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.DefaultCursor"]/*' />
        protected override Cursor DefaultCursor {
            get {
                return Cursors.IBeam;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(100, PreferredHeight);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ForeColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the foreground color of the control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DispId(NativeMethods.ActiveX.DISPID_FORECOLOR),
        SRDescription(nameof(SR.ControlForeColorDescr))
        ]
        public override Color ForeColor {
            get {
                if (ShouldSerializeForeColor()) {
                    return base.ForeColor;
                }
                else {
                    return SystemColors.WindowText;
                }
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.HideSelection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the selected
        ///       text in the text box control remains highlighted when the control loses focus.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TextBoxHideSelectionDescr))
        ]
        public bool HideSelection {
            get {
                return textBoxFlags[hideSelection];
            }

            set {
                if (textBoxFlags[hideSelection] != value) {
                    textBoxFlags[hideSelection] = value;
                    RecreateHandle();
                    OnHideSelectionChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.HideSelectionChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnHideSelectionChangedDescr))]
        public event EventHandler HideSelectionChanged {
            add {
                Events.AddHandler(EVENT_HIDESELECTIONCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_HIDESELECTIONCHANGED, value);
            }
        }

        /// <devdoc>
        ///     Internal version of ImeMode property.  The ImeMode of TextBoxBase controls depend on its IME restricted
        ///     mode which is determined by the CanEnableIme property which checks whether the control is in Password or
        ///     ReadOnly mode.
        /// </devdoc>
        protected override ImeMode ImeModeBase {
            get {
                if( DesignMode ) {
                    return base.ImeModeBase;
                }

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_ImeModeInternal(), this = " + this );
                Debug.Indent();

                ImeMode imeMode = CanEnableIme ? base.ImeModeBase : ImeMode.Disable;

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + imeMode );
                Debug.Unindent();

                return imeMode;
            }
            set {
                base.ImeModeBase = value;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Lines"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets the lines of text in an text box control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        MergableProperty(false),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxLinesDescr)),
        Editor("System.Windows.Forms.Design.StringArrayEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public string[] Lines {
            get {
                string text = Text;
                ArrayList list = new ArrayList();

                int lineStart = 0;
                while (lineStart < text.Length) {
                    int lineEnd = lineStart;
                    for (; lineEnd < text.Length; lineEnd++) {
                        char c = text[lineEnd];
                        if (c == '\r' || c == '\n')
                            break;
                    }

                    string line = text.Substring(lineStart, lineEnd - lineStart);
                    list.Add(line);

                    // Treat "\r", "\r\n", and "\n" as new lines
                    if (lineEnd < text.Length && text[lineEnd] == '\r')
                        lineEnd++;
                    if (lineEnd < text.Length && text[lineEnd] == '\n')
                        lineEnd++;

                    lineStart = lineEnd;
                }

                // Corner case -- last character in Text is a new line; need to add blank line to list
                if (text.Length > 0 && (text[text.Length - 1] == '\r' || text[text.Length - 1] == '\n'))
                    list.Add("");

                return(string[]) list.ToArray(typeof(string));
            }
            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // We append a new line to the Text property.
                                                                                                            // Don't localize the new line character.
            ]
            set {
                //unparse this string list...
                if (value != null && value.Length > 0) {

                    // Using a StringBuilder instead of a String
                    // speeds things up approx 150 times
                    StringBuilder text = new StringBuilder(value[0]);
                    for (int i=1; i < value.Length; ++i) {
                        text.Append("\r\n");
                        text.Append(value[i]);
                    }
                    Text = text.ToString();
                }
                else {
                    Text = "";
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.MaxLength"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the maximum number of
        ///       characters the user can type into the text box control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(32767),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxMaxLengthDescr))
        ]
        public virtual int MaxLength {
            get {
                return maxLength;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(MaxLength), string.Format(SR.InvalidLowBoundArgumentEx, "MaxLength", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }

                if (maxLength != value) {
                    maxLength = value;
                    UpdateMaxLength();
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Modified"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value that indicates that the text box control has been modified by the user since
        ///       the control was created or its contents were last set.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxModifiedDescr))
        ]
        public bool Modified {
            get {
                if (IsHandleCreated) {
                    bool curState = (0 != unchecked( (int) (long)SendMessage(NativeMethods.EM_GETMODIFY, 0, 0)));
                    if (textBoxFlags[modified] != curState) {
                        // Raise ModifiedChanged event.  See WmReflectCommand for more info.
                        textBoxFlags[modified] = curState;
                        OnModifiedChanged(EventArgs.Empty);
                    }
                    return curState;
                }
                else{
                    return textBoxFlags[modified];
                }

            }

            set {
                if (Modified != value) {
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.EM_SETMODIFY, value ? 1 : 0, 0);
                        // Must maintain this state always in order for the
                        // test in the Get method to work properly.
                    }

                    textBoxFlags[modified] = value;
                    OnModifiedChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ModifiedChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnModifiedChangedDescr))]
        public event EventHandler ModifiedChanged {
            add {
                Events.AddHandler(EVENT_MODIFIEDCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MODIFIEDCHANGED, value);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Multiline"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether this
        ///       is a multiline text box control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxMultilineDescr)),
        RefreshProperties(RefreshProperties.All)
        ]
        public virtual bool Multiline {
            get {
                return textBoxFlags[multiline];
            }
            set {
                if (textBoxFlags[multiline] != value) {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Multiline)) {
                        textBoxFlags[multiline] = value;

                        if (value) {
                            // Multi-line textboxes do not have fixed height
                            //
                            SetStyle(ControlStyles.FixedHeight, false);
                        }
                        else {
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

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.MultilineChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnMultilineChangedDescr))]
        public event EventHandler MultilineChanged {
            add {
                Events.AddHandler(EVENT_MULTILINECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MULTILINECHANGED, value);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PaddingChanged"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnPaddingChangedDescr))
        ]
        public new event EventHandler PaddingChanged {
            add {
                base.PaddingChanged += value;
            }
            remove {
                base.PaddingChanged -= value;
            }
        }

        /// <devdoc>
        ///     Determines if the control is in password protect mode.  This is overridden in TextBox and
        ///     MaskedTextBox and is false by default so RichTextBox that doesn't support Password doesn't
        ///     have to care about this.
        /// </devdoc>
        virtual internal bool PasswordProtect
        {
            get {
                return false;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.PreferredHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns the preferred
        ///       height for a single-line text box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxPreferredHeightDescr))
        ]
        public int PreferredHeight {
            get {
                // COMPAT we must return the same busted height we did in Everett, even
                // if it doesnt take multiline and word wrap into account.  For better accuracy and/or wrapping use 
                // GetPreferredSize instead.
                int height = FontHeight;
                if (borderStyle != BorderStyle.None) {
                    height += SystemInformation.GetBorderSizeForDpi(deviceDpi).Height * 4 + 3;
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

            if (BorderStyle != BorderStyle.None) {
                bordersAndPadding += new Size(0, 3);
            }

            if (BorderStyle == BorderStyle.FixedSingle) {
                // Bump these by 2px to match BorderStyle.Fixed3D - they'll be omitted from the SizeFromClientSize call.
                bordersAndPadding.Width +=2;
                bordersAndPadding.Height +=2;
            }
            // Reduce constraints by border/padding size
            proposedConstraints -= bordersAndPadding;

            // Fit the text to the remaining space
            // Fix for Dev10 

            TextFormatFlags format = TextFormatFlags.NoPrefix;
            if(!Multiline) {
                format |= TextFormatFlags.SingleLine;
            } else if(WordWrap) {
                format |= TextFormatFlags.WordBreak;
            }
            Size textSize = TextRenderer.MeasureText(this.Text, this.Font, proposedConstraints, format);

            // We use this old computation as a lower bound to ensure backwards compatibility.
            textSize.Height = Math.Max(textSize.Height, FontHeight);
            Size preferredSize = textSize + bordersAndPadding;
            return preferredSize;
        }

        /// <devdoc>
        ///     Get the currently selected text start position and length.  Use this method internally
        ///     to avoid calling SelectionStart + SelectionLength each of which does essentially the
        ///     same (save one message round trip).
        /// </devdoc>
        internal void GetSelectionStartAndLength( out int start, out int length ){
            int end = 0;

            if( !IsHandleCreated ) {
                // It is possible that the cached values are no longer valid if the Text has been changed
                // while the control does not have a handle. We need to return valid values.  We also need
                // to keep the old cached values in case the Text is changed again making the cached values
                // valid again.
                AdjustSelectionStartAndEnd( this.selectionStart, this.selectionLength, out start, out end, -1 );
                length = end - start;
            }
            else {
                start = 0;
                UnsafeNativeMethods.SendMessage( new HandleRef( this, Handle ), NativeMethods.EM_GETSEL, ref start, ref end );

                //Here, we return the max of either 0 or the # returned by
                //the windows call.  This eliminates a problem on nt4 where
                // a huge negative # is being returned.
                //
                start = Math.Max( 0, start );
                // ditto for end
                end = Math.Max( 0, end );

                length = end - start;
            }

#if DEBUG
            {
                string t = WindowText;
                int len;

                end = start + length - 1;

                if (t == null) {
                    len = 0;
                }
                else {
                    len = t.Length;
                }

                Debug.Assert(end <= len, "SelectionEnd is outside the set of valid caret positions for the current WindowText (end ="
                                + end + ", WindowText.Length =" + len +")");
            }
#endif
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ReadOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether text in the text box is read-only.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.TextBoxReadOnlyDescr))
        ]
        public bool ReadOnly {
            get {
                return textBoxFlags[readOnly];
            }
            set {
                if (textBoxFlags[readOnly] != value) {
                    textBoxFlags[readOnly] = value;
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.EM_SETREADONLY, value? -1: 0, 0);
                    }

                    OnReadOnlyChanged(EventArgs.Empty);

                    VerifyImeRestrictedModeChanged();
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ReadOnlyChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnReadOnlyChangedDescr))]
        public event EventHandler ReadOnlyChanged {
            add {
                Events.AddHandler(EVENT_READONLYCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_READONLYCHANGED, value);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.SelectedText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The currently selected text in the control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectedTextDescr))
        ]
        public virtual string SelectedText {
            get {
                int selStart, selLength;
                GetSelectionStartAndLength( out selStart, out selLength );
                return Text.Substring(selStart, selLength);
            }
            set {
                SetSelectedTextInternal(value, true);
            }
        }

        /// <devdoc>
        ///     Replaces the selected text with the one passed in.
        /// </devdoc>
        internal virtual void SetSelectedTextInternal(string text, bool clearUndo){
            if (!IsHandleCreated) {
                CreateHandle();
            }

            if( text == null ){
                text = "";
            }

            // The EM_LIMITTEXT message limits only the text the user can enter. It does not affect any text
            // already in the edit control when the message is sent, nor does it affect the length of the text
            // copied to the edit control by the WM_SETTEXT message.
            SendMessage(NativeMethods.EM_LIMITTEXT, 0, 0);

            if( clearUndo ){
                SendMessage(NativeMethods.EM_REPLACESEL, 0, text);
                // For consistency with Text, we clear the modified flag
                SendMessage(NativeMethods.EM_SETMODIFY, 0, 0);
                ClearUndo();
            }
            else{
                SendMessage(NativeMethods.EM_REPLACESEL, /*undoable*/ -1, text);
            }

            // Re-enable user input.
            SendMessage(NativeMethods.EM_LIMITTEXT, maxLength, 0);
        }


        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.SelectionLength"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the number of characters selected in the text
        ///       box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectionLengthDescr))
        ]
        public virtual int SelectionLength {
            get {
                int start, length;

                GetSelectionStartAndLength( out start, out length );

                return length;
            }

            set {
                if (value < 0){
                    throw new ArgumentOutOfRangeException(nameof(SelectionLength), string.Format(SR.InvalidArgument, "SelectionLength", value.ToString(CultureInfo.CurrentCulture)));
                }

                int selStart, selLength;
                GetSelectionStartAndLength( out selStart, out selLength );

                if (value != selLength) {
                    Select(selStart, value);
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.SelectionStart"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the starting
        ///       point of text selected in the text
        ///       box.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectionStartDescr))
        ]
        public int SelectionStart {
            get {
                int selStart, selLength;

                GetSelectionStartAndLength( out selStart, out selLength );

                return selStart;
            }
            set {
                if (value < 0){
                    throw new ArgumentOutOfRangeException(nameof(SelectionStart), string.Format(SR.InvalidArgument, "SelectionStart", value.ToString(CultureInfo.CurrentCulture)));
                }
                Select(value, SelectionLength);
            }
        }

        // Call SetSelectionOnHandle inside CreateHandle()
        internal virtual bool SetSelectionInCreateHandle {
            get {
                return true;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the current text in the text box.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                if (value != base.Text) {
                    base.Text = value;
                    if (IsHandleCreated) {
                        // clear the modified flag
                        SendMessage(NativeMethods.EM_SETMODIFY, 0, 0);
                    }
                }
            }
        }

        [Browsable(false)]
        public virtual int TextLength
            // Note: Currently Winforms does not fully support surrogates.  If
            // the text contains surrogate characters this property may return incorrect values.

            => IsHandleCreated ? SafeNativeMethods.GetWindowTextLength(new HandleRef(this, Handle)) : Text.Length;

        // Since setting the WindowText while the handle is created
        // generates a WM_COMMAND message, we must trap that case
        // and prevent the event from getting fired, or we get
        // double "TextChanged" events.
        //
        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.WindowText"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        internal override string WindowText {
            get {
                return base.WindowText;
            }

            set {
                if (value == null) value = "";
                if (!WindowText.Equals(value)) {
                    textBoxFlags[codeUpdateText] = true;
                    try {
                        base.WindowText = value;
                    }
                    finally {
                        textBoxFlags[codeUpdateText] = false;
                    }
                }
            }
        }

        /// <devdoc>
        ///     In certain circumstances we might have to force
        ///     text into the window whether or not the text is the same.
        ///     Make this a method on TextBoxBase rather than RichTextBox (which is the only
        ///     control that needs this at this point), since we need to set codeUpdateText.
        /// </devdoc>
        internal void ForceWindowText(string value) {
            if (value == null) {
                value = "";
            }
            
            textBoxFlags[codeUpdateText] = true;
            try {
                if (IsHandleCreated) {
                    UnsafeNativeMethods.SetWindowText(new HandleRef(this, Handle), value);
                }
                else {
                    if (value.Length == 0) {
                        Text = null;
                    }
                    else {
                        Text = value;
                    }
                }
            }
            finally {
                textBoxFlags[codeUpdateText] = false;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.WordWrap"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether a
        ///       multiline text box control automatically wraps words to the beginning of the next
        ///       line when necessary.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(true),
        SRDescription(nameof(SR.TextBoxWordWrapDescr))
        ]
        public bool WordWrap {
            get {
                return textBoxFlags[wordWrap];
            }
            set {
                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.WordWrap)) {
                    if (textBoxFlags[wordWrap] != value) {
                        textBoxFlags[wordWrap] = value;
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.AdjustHeight"]/*' />
        /// <devdoc>
        ///     Adjusts the height of a single-line edit control to match the height of
        ///     the control's font.
        /// </devdoc>
        /// <internalonly/>
        private void AdjustHeight(bool returnIfAnchored) {

            // If we're anchored to two opposite sides of the form, don't adjust the size because
            // we'll lose our anchored size by resetting to the requested width.
            //
            if (returnIfAnchored && (this.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom)) {
                return;
            }

            int saveHeight = requestedHeight;
            try {
                if (textBoxFlags[autoSize] && !textBoxFlags[multiline]) {
                    Height = PreferredHeight;
                }
                else {

                    int curHeight = Height;

                    // Changing the font of a multi-line textbox can sometimes cause a painting problem
                    // The only workaround I can find is to size the textbox big enough for the font, and
                    // then restore its correct size.
                    //
                    if (textBoxFlags[multiline]) {
                        Height = Math.Max(saveHeight, PreferredHeight + 2); // 2 = fudge factor
                    }

                    integralHeightAdjust = true;
                    try {
                        Height = saveHeight;
                    }
                    finally {
                        integralHeightAdjust = false;
                    }
                }
            }
            finally {
                requestedHeight = saveHeight;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.AppendText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Append text to the current text of text box.
        ///    </para>
        /// </devdoc>
        public void AppendText( string text ) {
            if (text.Length > 0) {
                int selStart, selLength;

                GetSelectionStartAndLength( out selStart, out selLength );

                try {
                    // This enables you to use SelectionColor to AppendText in color.
                    int endOfText = GetEndPosition();

                    SelectInternal(endOfText, endOfText, endOfText);
                    SelectedText = text;
                }
                finally {
                    // If AppendText is called when the control is docked and the form is minimized,
                    // all the text will scroll to the top and the control will look empty when the
                    // form is restored. We work around this by selecting back whatever was originally
                    // selected when AppendText was called.
                    if (this.Width == 0 || this.Height == 0) {
                        this.Select(selStart, selLength);
                    }
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Clear"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Clears all text from the text box control.
        ///    </para>
        /// </devdoc>
        public void Clear() {
            Text = null;
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ClearUndo"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Clears information about the most recent operation
        ///       from the undo buffer of the text box.
        ///    </para>
        /// </devdoc>
        public void ClearUndo() {
            if (IsHandleCreated) {
                SendMessage(NativeMethods.EM_EMPTYUNDOBUFFER, 0, 0);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Copy"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Copies the current selection in the text box to the Clipboard.
        ///    </para>
        /// </devdoc>
        [UIPermission(SecurityAction.Demand, Clipboard=UIPermissionClipboard.OwnClipboard)]
        public void Copy() {
            SendMessage(NativeMethods.WM_COPY, 0, 0);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.CreateHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void CreateHandle() {
            // This "creatingHandle" stuff is to avoid property change events
            // when we set the Text property.
            textBoxFlags[creatingHandle] = true;
            try {
                base.CreateHandle();

                if (SetSelectionInCreateHandle) {
                    // send EM_SETSEL message
                    SetSelectionOnHandle();
                }
                
            }
            finally {
                textBoxFlags[creatingHandle] = false;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Cut"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Moves the current selection in the text box to the Clipboard.
        ///    </para>
        /// </devdoc>
        public void Cut() {
            SendMessage(NativeMethods.WM_CUT, 0, 0);
        }

        /// <devdoc>
        ///     Returns the text end position (one past the last input character).  This property is virtual to allow MaskedTextBox
        ///     to set the last input char position as opposed to the last char position which may be a mask character.
        /// </devdoc>
        internal virtual int GetEndPosition(){
            // +1 because RichTextBox has this funny EOF pseudo-character after all the text.
            return IsHandleCreated ? TextLength + 1 : TextLength;
        }

                /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.IsInputKey"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Overridden to handle TAB key.
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            if ((keyData & Keys.Alt) != Keys.Alt) {
                switch (keyData & Keys.KeyCode) {
                    case Keys.Tab:
                        // Single-line RichEd's want tab characters (see WM_GETDLGCODE),
                        // so we don't ask it
                        return Multiline && textBoxFlags[acceptsTab] && ((keyData & Keys.Control) == 0);
                    case Keys.Escape:
                        if (Multiline)
                            return false;
                        break;
                    case Keys.Back:
                        if (!this.ReadOnly)
                            return true;
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

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnHandleCreated"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Overridden to update the newly created handle with the settings of the
        ///    MaxLength and PasswordChar properties.
        /// </devdoc>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            // it's likely here that the create params could have changed
            // the border size/etc.
            CommonProperties.xClearPreferredSizeCache(this);
            AdjustHeight(true);

            UpdateMaxLength();
            if (textBoxFlags[modified]){
                SendMessage(NativeMethods.EM_SETMODIFY, 1, 0);
            }
            if (textBoxFlags[scrollToCaretOnHandleCreated]) {
                ScrollToCaret();
                textBoxFlags[scrollToCaretOnHandleCreated] = false;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnHandleDestroyed"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleDestroyed(EventArgs e) {
            textBoxFlags[modified] = Modified;
            textBoxFlags[setSelectionOnHandleCreated] = true;            
            // Update text selection cached values to be restored when recreating the handle.
            GetSelectionStartAndLength( out this.selectionStart, out this.selectionLength );
            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Paste"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Replaces the current selection in the text box with the contents of the Clipboard.
        ///    </para>
        /// </devdoc>
        [UIPermission(SecurityAction.Demand, Clipboard=UIPermissionClipboard.OwnClipboard)]
        public void Paste() {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ClipboardRead Demanded");
            IntSecurity.ClipboardRead.Demand();

            SendMessage(NativeMethods.WM_PASTE, 0, 0);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ProcessDialogKey"]/*' />
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "TextBoxBase.ProcessDialogKey [" + keyData.ToString() + "]");
            Keys keyCode = (Keys)keyData & Keys.KeyCode;

            if (keyCode == Keys.Tab && this.AcceptsTab && (keyData & Keys.Control) != 0) {
                // When this control accepts Tabs, Ctrl-Tab is treated exactly like Tab.
                keyData &= ~Keys.Control;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnPaint"]/*' />
        /// <devdoc>
        ///     TextBox / RichTextBox Onpaint.
        /// </devdoc>
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }


        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnAcceptsTabChanged"]/*' />
        protected virtual void OnAcceptsTabChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_ACCEPTSTABCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnBorderStyleChanged"]/*' />
        protected virtual void OnBorderStyleChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_BORDERSTYLECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnFontChanged"]/*' />
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            AdjustHeight(false);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnHideSelectionChanged"]/*' />
        protected virtual void OnHideSelectionChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_HIDESELECTIONCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnModifiedChanged"]/*' />
        protected virtual void OnModifiedChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_MODIFIEDCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

         /// <devdoc>
        ///     Raises the MouseUp event.
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs mevent) {
            Point pt = PointToScreen(mevent.Location);

            if (mevent.Button == MouseButtons.Left) {
                if (!ValidationCancelled && UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {
                    if (!this.doubleClickFired) {
                        OnClick(mevent);
                        OnMouseClick(mevent);
                    }
                    else {
                        this.doubleClickFired = false;
                        OnDoubleClick(mevent);
                        OnMouseDoubleClick(mevent);
                    }
                }
                this.doubleClickFired = false;
            }
            base.OnMouseUp(mevent);
        }

         /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnMultilineChanged"]/*' />
        protected virtual void OnMultilineChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_MULTILINECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }


        protected override void OnPaddingChanged(EventArgs e) {
            base.OnPaddingChanged(e);
            AdjustHeight(false);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.OnReadOnlyChanged"]/*' />
        protected virtual void OnReadOnlyChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_READONLYCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        protected override void OnTextChanged(EventArgs e) {
            // since AutoSize existed in Everett, (and is the default) we can't 
            // relayout the parent when the "preferredsize" of the control changes.
            // this means a multiline = true textbox wont natrually grow in height when 
            // the text changes.
            CommonProperties.xClearPreferredSizeCache(this);
            base.OnTextChanged(e);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.GetCharFromPosition"]/*' />
        /// <devdoc>
        ///     Returns the character nearest to the given point.
        /// </devdoc>
        public virtual char GetCharFromPosition(Point pt) {
            string t = this.Text;
            int index = GetCharIndexFromPosition(pt);
            return (index < 0 || index >= t.Length) ? (char)0 : t[index];
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.GetCharIndexFromPosition"]/*' />
        /// <devdoc>
        ///     Returns the index of the character nearest to the given point.
        /// </devdoc>
        public virtual int GetCharIndexFromPosition(Point pt) {
            int longPoint = NativeMethods.Util.MAKELONG(pt.X, pt.Y);
            int index = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.EM_CHARFROMPOS, 0, longPoint);
            index = NativeMethods.Util.LOWORD(index);

            if (index < 0) {
                index = 0;
            }
            else {
                string t = this.Text;
                // EM_CHARFROMPOS will return an invalid number if the last character in the RichEdit
                // is a newline.
                //
                if (index >= t.Length) {
                    index = Math.Max(t.Length - 1, 0);
                }
            }
            return index;
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.GetLineFromCharIndex"]/*' />
        /// <devdoc>
        ///     Returns the number of the line containing a specified character position
        ///     in a textbox. Note that this returns the physical line number
        ///     and not the conceptual line number. For example, if the first conceptual
        ///     line (line number 0) word-wraps and extends to the second line, and if
        ///     you pass the index of a overflowed character, GetLineFromCharIndex would
        ///     return 1 and not 0.
        /// </devdoc>
        public virtual int GetLineFromCharIndex(int index) {
            return unchecked( (int) (long)SendMessage(NativeMethods.EM_LINEFROMCHAR, index, 0));
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.GetPositionFromCharIndex"]/*' />
        /// <devdoc>
        ///     Returns the location of the character at the given index.
        /// </devdoc>
        public virtual Point GetPositionFromCharIndex(int index) {
            if (index < 0 || index >= Text.Length)
                return Point.Empty;

            int i = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.EM_POSFROMCHAR, index, 0);
            return new Point(NativeMethods.Util.SignedLOWORD(i), NativeMethods.Util.SignedHIWORD(i));
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.GetFirstCharIndexFromLine"]/*' />
        /// <devdoc>
        ///     Returns the index of the first character of a given line. Returns -1 of lineNumber is invalid.
        /// </devdoc>
        public int GetFirstCharIndexFromLine(int lineNumber) {
            if (lineNumber < 0) {
                throw new ArgumentOutOfRangeException(nameof(lineNumber), string.Format(SR.InvalidArgument, "lineNumber", lineNumber.ToString(CultureInfo.CurrentCulture)));
            }
            return unchecked( (int) (long)SendMessage(NativeMethods.EM_LINEINDEX, lineNumber, 0));
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.GetFirstCharIndexofCurrentLine"]/*' />
        /// <devdoc>
        ///     Returns the index of the first character of the line where the caret is.
        /// </devdoc>
        public int GetFirstCharIndexOfCurrentLine() {
            return unchecked( (int) (long)SendMessage(NativeMethods.EM_LINEINDEX, -1, 0));
        }



        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ScrollToCaret"]/*' />
        /// <devdoc>
        ///     Ensures that the caret is visible in the TextBox window, by scrolling the
        ///     TextBox control surface if necessary.
        /// </devdoc>
        public void ScrollToCaret() {
            if (IsHandleCreated) {
                if (String.IsNullOrEmpty(this.WindowText)) {
                    // If there is no text, then there is no place to go.
                    return;
                }

                bool scrolled = false;
                object editOle = null;
                IntPtr editOlePtr = IntPtr.Zero;
                try {
                    if (UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichTextBoxConstants.EM_GETOLEINTERFACE, 0, out editOle) != 0) {

                        editOlePtr = Marshal.GetIUnknownForObject(editOle);

                        if (editOlePtr != IntPtr.Zero) {
                            IntPtr iTextDocument = IntPtr.Zero;
                            Guid iiTextDocumentGuid = typeof(UnsafeNativeMethods.ITextDocument).GUID;

                            try {
                                Marshal.QueryInterface(editOlePtr, ref iiTextDocumentGuid, out iTextDocument);

                                UnsafeNativeMethods.ITextDocument textDocument = Marshal.GetObjectForIUnknown(iTextDocument) as UnsafeNativeMethods.ITextDocument;

                                if (textDocument != null) {
                                    int selStart, selLength;

                                    // When the user calls RichTextBox::ScrollToCaret we want the RichTextBox to show as
                                    // much text as possible.
                                    // Here is how we do that:
                                    // 1. We scroll the RichTextBox all the way to the bottom so the last line of text is the last visible line.
                                    // 2. We get the first visible line.
                                    // 3. If the first visible line is smaller than the start of the selection, then we are done:
                                    //      The selection fits inside the RichTextBox display rectangle.
                                    // 4. Otherwise, scroll the selection to the top of the RichTextBox.
                                    GetSelectionStartAndLength( out selStart, out selLength );
                                    int selStartLine = GetLineFromCharIndex(selStart);

                                    // 1. Scroll the RichTextBox all the way to the bottom
                                    UnsafeNativeMethods.ITextRange textRange = textDocument.Range(this.WindowText.Length - 1, this.WindowText.Length - 1);
                                    textRange.ScrollIntoView(0);   // 0 ==> tomEnd

                                    // 2. Get the first visible line.
                                    int firstVisibleLine = unchecked( (int) (long)SendMessage(NativeMethods.EM_GETFIRSTVISIBLELINE, 0, 0));

                                    // 3. If the first visible line is smaller than the start of the selection, we are done;
                                    if (firstVisibleLine <= selStartLine) {
                                        // we are done
                                    } else {
                                        // 4. Scroll the selection to the top of the RichTextBox
                                        textRange = textDocument.Range(selStart, selStart + selLength);
                                        textRange.ScrollIntoView(32);   // 32 ==> tomStart
                                    }

                                    scrolled = true;
                                }
                            } finally {
                                if (iTextDocument != IntPtr.Zero) {
                                    Marshal.Release(iTextDocument);
                                }
                            }
                        }
                    }
                } finally {
                    if (editOlePtr != IntPtr.Zero) {
                        Marshal.Release(editOlePtr);
                    }
                }

                if (!scrolled) {
                    SendMessage(NativeMethods.EM_SCROLLCARET, 0, 0);
                }
            }
            else {
                textBoxFlags[scrollToCaretOnHandleCreated] = true;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.DeselectAll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the SelectionLength to 0.
        ///    </para>
        /// </devdoc>
        public void DeselectAll() {
            this.SelectionLength = 0;
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Select"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selects a range of text in the text box.
        ///    </para>
        /// </devdoc>
        public void Select(int start, int length) {
            if (start < 0){
                throw new ArgumentOutOfRangeException(nameof(start), string.Format(SR.InvalidArgument, "start", start.ToString(CultureInfo.CurrentCulture)));
            }

            int textLen = TextLength;

            if (start > textLen) {
                //We shouldn't allow positive length if you're starting at the end, but
                //should allow negative length.
                long longLength = Math.Min(0, (long)length + start - textLen);
                if (longLength < int.MinValue) {
                    length = int.MinValue;
                }
                else {
                    length = (int)longLength;
                }
                start = textLen;
            }

            SelectInternal(start, length, textLen);
        }

        /// <devdoc>
        ///     Performs the actual select without doing arg checking.
        ///
        ///     Send in -1 for the textLen parameter if you don't have the text
        ///     length cached when calling this method. It will be computed.
        ///     But if you do have it cached, please pass it in. This will avoid
        ///     the expensive call to the TextLength property.
        /// </devdoc>
        internal virtual void SelectInternal(int start, int length, int textLen) {
            //if our handle is created - send message...
            if (IsHandleCreated) {
                int s, e;
                AdjustSelectionStartAndEnd(start, length, out s, out e, textLen);

                SendMessage(NativeMethods.EM_SETSEL, s, e);
                // 

            }
            else {
                //otherwise, wait until handle is created to send this message.
                //Store the indices until then...
                this.selectionStart = start;
                this.selectionLength = length;
                textBoxFlags[setSelectionOnHandleCreated] = true;
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.SelectAll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selects all text in the text box.
        ///    </para>
        /// </devdoc>
        public void SelectAll() {
            int textLen = TextLength;
            SelectInternal(0, textLen, textLen);
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.SetBoundsCore"]/*' />
        /// <devdoc>
        ///    Overrides Control.setBoundsCore to enforce autoSize.
        /// </devdoc>
        /// <internalonly/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {

            if (!integralHeightAdjust && height != Height)
                requestedHeight = height;

            if (textBoxFlags[autoSize] && !textBoxFlags[multiline])
                height = PreferredHeight;

            base.SetBoundsCore(x, y, width, height, specified);
        }

        private static void Swap(ref int n1, ref int n2) {
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
        internal void AdjustSelectionStartAndEnd(int selStart, int selLength, out int start, out int end, int textLen) {
            start = selStart;
            end = 0;

            if (start <= -1) {
                start = -1;
            }
            else {
                int textLength;

                if (textLen >= 0) {
                    textLength = textLen;
                }
                else {
                    textLength = this.TextLength;
                }

                if (start > textLength) {
                    start = textLength;
                }

                checked {
                    try {
                        end = start + selLength;
                    } 
                    catch (OverflowException) {
                        //Since we overflowed, cap at the max/min value: we'll correct the value below
                        end = start > 0 ? int.MaxValue : int.MinValue;
                    }
                }

                // Make sure end is in range
                if (end < 0) {
                    end = 0;
                }
                else if (end > textLength) {
                    end = textLength;
                }
            }
        }

        // Called by CreateHandle or OnHandleCreated
        internal void SetSelectionOnHandle() {
            Debug.Assert(IsHandleCreated, "Don't call this method until the handle is created.");
            if (textBoxFlags[setSelectionOnHandleCreated]) {
                textBoxFlags[setSelectionOnHandleCreated] = false;
                int start, end;
                AdjustSelectionStartAndEnd(this.selectionStart, this.selectionLength, out start, out end, -1);
                SendMessage(NativeMethods.EM_SETSEL, start, end);
            }
        }
 
        /// <devdoc>
        ///     Converts byte offsset to unicode offsets.
        ///     When procssing WM_GETSEL/WM_SETSEL, EDIT control works with byte offsets instead of character positions
        ///     as opposed to RICHEDIT which does it always as character positions.  
        ///     This method is used when handling the WM_GETSEL message.
        /// </devdoc>
        static void ToUnicodeOffsets(string str, ref int start, ref int end) {
            Encoding e = Encoding.Default;

            // Acutally, we may get away with this call if we can get the bytes from Win9x.  Dont know if it is possible.
            // This can be expensive since start/end could be small, but str.Length can be quite big.
            byte[] bytes = e.GetBytes(str);

            bool swap = start > end;
            if (swap) {
                Swap(ref start, ref end);
            }

            // Make sure start and end are within the string
            //
            if (start < 0){
                start = 0;
            }
            if (start > bytes.Length){
                start = bytes.Length;
            }
            if (end > bytes.Length){
                end = bytes.Length;
            }
            
            // IMPORTANT: Avoid off-by-1 errors! 
            // The end value passed in is the character immediately after the last character selected.

            int newStart = start == 0 ? 0 : e.GetCharCount(bytes, 0, start);
            end = newStart + e.GetCharCount(bytes, start, end - start);
            start = newStart;

            if (swap){
                Swap(ref start, ref end);
            }
        }

        /// <devdoc>
        ///     Converts unicode offsset to byte offsets.
        ///     When procssing WM_GETSEL/WM_SETSEL, EDIT control works with byte offsets instead of character positions
        ///     as opposed to RICHEDIT which does it always as character positions.  
        ///     This method is used when handling the WM_SETSEL message.
        /// </devdoc>
        static internal void ToDbcsOffsets(string str, ref int start, ref int end) {
            Encoding e = Encoding.Default;

            bool swap = start > end;
            if (swap) {
                Swap(ref start, ref end);
            }

            // Make sure start and end are within the string
            //
            if (start < 0) {
                start = 0;
            }
            if (start > str.Length) {
                start = str.Length;
            }
            if (end < start) {
                end = start;
            }
            if (end > str.Length) {
                end = str.Length;
            }

            // IMPORTANT: Avoid off-by-1 errors!
            // The end value passed in is the character immediately after the last character selected.

            int newStart =  start == 0 ? 0 : e.GetByteCount(str.Substring(0, start));
            end = newStart + e.GetByteCount(str.Substring(start, end - start));
            start = newStart;

            if (swap) {
                Swap(ref start, ref end);
            }
        }



        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.ToString"]/*' />
        /// <devdoc>
        ///    Provides some interesting information for the TextBox control in
        ///    String form.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();

            string txt = Text;
            if (txt.Length > 40) txt = txt.Substring(0, 40) + "...";
            return s + ", Text: " + txt.ToString();
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.Undo"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Undoes the last edit operation in the text box.
        ///    </para>
        /// </devdoc>
        public void Undo() {
            SendMessage(NativeMethods.EM_UNDO, 0, 0);
        }

        internal virtual void UpdateMaxLength() {
            if (IsHandleCreated) {
                SendMessage(NativeMethods.EM_LIMITTEXT, maxLength, 0);
            }
        }


        internal override IntPtr InitializeDCForWmCtlColor (IntPtr dc, int msg)
        {
            if ((msg == NativeMethods.WM_CTLCOLORSTATIC) && !ShouldSerializeBackColor()) {
                // Let the Win32 Edit control handle background colors itself.
                // This is necessary because a disabled edit control will display a different
                // BackColor than when enabled.
                return IntPtr.Zero;
            }
            else {
                return base.InitializeDCForWmCtlColor(dc, msg);
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.WmReflectCommand"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectCommand(ref Message m) {
            if (!textBoxFlags[codeUpdateText] && !textBoxFlags[creatingHandle]) {
                if (NativeMethods.Util.HIWORD(m.WParam) == NativeMethods.EN_CHANGE && CanRaiseTextChangedEvent) {
                    OnTextChanged(EventArgs.Empty);
                }
                else if (NativeMethods.Util.HIWORD(m.WParam) == NativeMethods.EN_UPDATE) {
                    // Force update to the Modified property, which will trigger
                    // ModifiedChanged event handlers
                    bool force = this.Modified;
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.WmSetFont"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void WmSetFont(ref Message m) {
            base.WndProc(ref m);
            if (!textBoxFlags[multiline]) {
                SendMessage(NativeMethods.EM_SETMARGINS, NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN, 0);
            }
        }

        void WmGetDlgCode(ref Message m) {
            base.WndProc(ref m);
            if (AcceptsTab) {
                Debug.WriteLineIf(Control.ControlKeyboardRouting.TraceVerbose, "TextBox wants tabs");
                m.Result = (IntPtr)(unchecked( (int) (long)m.Result) | NativeMethods.DLGC_WANTTAB);
            }
            else {
                Debug.WriteLineIf(Control.ControlKeyboardRouting.TraceVerbose, "TextBox doesn't want tabs");
                m.Result = (IntPtr)(unchecked( (int) (long)m.Result) & ~(NativeMethods.DLGC_WANTTAB | NativeMethods.DLGC_WANTALLKEYS));
            }
        }

        /// <devdoc>
        ///     Handles the WM_CONTEXTMENU message based on this
        ///     table:
        ///     ShortcutsEnabled    #1      #2      #3
        ///     Yes                 strip   context system
        ///     No                  strip   context N/A
        /// </devdoc>
        /// <internalonly/>
        private void WmTextBoxContextMenu(ref Message m) {

            if (ContextMenu != null || ContextMenuStrip != null) {
                int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                Point client;
                bool keyboardActivated = false;
                // lparam will be exactly -1 when the user invokes the context menu
                // with the keyboard.
                //
                if (unchecked((int)(long)m.LParam) == -1) {
                    keyboardActivated = true;
                    client = new Point(Width/2, Height/2);
                }
                else {
                    client = PointToClientInternal(new Point(x, y));
                }

                // 

                // VisualStudio7 # 156, only show the context menu when clicked in the client area
                if (ClientRectangle.Contains( client )) {
                    if (ContextMenu != null) {
                        ContextMenu.Show(this, client);
                    }
                    else if (ContextMenuStrip != null) {
                        ContextMenuStrip.ShowInternal(this, client, keyboardActivated);
                    }
                    else {
                      Debug.Fail("contextmenu and contextmenustrip are both null... hmm how did we get here?");
                      DefWndProc( ref m );
                    }
                }
            }
        }

        /// <include file='doc\TextBoxBase.uex' path='docs/doc[@for="TextBoxBase.WndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    The control's window procedure.  Inheriting classes can override this
        ///    to add extra functionality, but should not forget to call
        ///    base.wndProc(m); to ensure the control continues to function properly.
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_LBUTTONDBLCLK:
                    this.doubleClickFired = true;
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_COMMAND:
                    WmReflectCommand(ref m);
                    break;
                case NativeMethods.WM_GETDLGCODE:
                    WmGetDlgCode(ref m);
                    break;
                case NativeMethods.WM_SETFONT:
                    WmSetFont(ref m);
                    break;
                case NativeMethods.WM_CONTEXTMENU:
                    if (ShortcutsEnabled) {
                        //calling base will find ContextMenus in this order:
                        // 1) ContextMenu 2) ContextMenuStrip 3) SystemMenu
                        base.WndProc(ref m);
                    }
                    else {
                        // we'll handle this message so we can hide the
                        // SystemMenu if Context and Strip menus are null
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
