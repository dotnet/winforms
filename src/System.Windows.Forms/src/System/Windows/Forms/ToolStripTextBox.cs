// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public partial class ToolStripTextBox : ToolStripControlHost
    {
        internal static readonly object s_eventTextBoxTextAlignChanged = new object();
        internal static readonly object s_eventAcceptsTabChanged = new object();
        internal static readonly object s_eventBorderStyleChanged = new object();
        internal static readonly object s_eventHideSelectionChanged = new object();
        internal static readonly object s_eventReadOnlyChanged = new object();
        internal static readonly object s_eventMultilineChanged = new object();
        internal static readonly object s_eventModifiedChanged = new object();

        private static readonly Padding s_defaultMargin = new Padding(1, 0, 1, 0);
        private static readonly Padding s_defaultDropDownMargin = new Padding(1);
        private Padding _scaledDefaultMargin = s_defaultMargin;
        private Padding _scaledDefaultDropDownMargin = s_defaultDropDownMargin;

        public ToolStripTextBox() : base(CreateControlInstance())
        {
            ToolStripTextBoxControl textBox = Control as ToolStripTextBoxControl;
            textBox.Owner = this;

            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(s_defaultMargin);
                _scaledDefaultDropDownMargin = DpiHelper.LogicalToDeviceUnits(s_defaultDropDownMargin);
            }
        }

        public ToolStripTextBox(string name) : this()
        {
            Name = name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripTextBox(Control c) : base(c)
        {
            throw new NotSupportedException(SR.ToolStripMustSupplyItsOwnTextBox);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected internal override Padding DefaultMargin
        {
            get
            {
                if (IsOnDropDown)
                {
                    return _scaledDefaultDropDownMargin;
                }
                else
                {
                    return _scaledDefaultMargin;
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 22);
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextBox TextBox
        {
            get
            {
                return Control as TextBox;
            }
        }

        private static Control CreateControlInstance()
        {
            TextBox textBox = new ToolStripTextBoxControl
            {
                BorderStyle = BorderStyle.Fixed3D,
                AutoSize = true
            };
            return textBox;
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            // dont call TextBox.GPS because it will grow and shrink as the text changes.
            Rectangle bounds = CommonProperties.GetSpecifiedBounds(TextBox);
            return new Size(bounds.Width, TextBox.PreferredHeight);
        }
        private void HandleAcceptsTabChanged(object sender, EventArgs e)
        {
            OnAcceptsTabChanged(e);
        }
        private void HandleBorderStyleChanged(object sender, EventArgs e)
        {
            OnBorderStyleChanged(e);
        }
        private void HandleHideSelectionChanged(object sender, EventArgs e)
        {
            OnHideSelectionChanged(e);
        }
        private void HandleModifiedChanged(object sender, EventArgs e)
        {
            OnModifiedChanged(e);
        }
        private void HandleMultilineChanged(object sender, EventArgs e)
        {
            OnMultilineChanged(e);
        }
        private void HandleReadOnlyChanged(object sender, EventArgs e)
        {
            OnReadOnlyChanged(e);
        }
        private void HandleTextBoxTextAlignChanged(object sender, EventArgs e)
        {
            RaiseEvent(s_eventTextBoxTextAlignChanged, e);
        }
        protected virtual void OnAcceptsTabChanged(EventArgs e)
        {
            RaiseEvent(s_eventAcceptsTabChanged, e);
        }
        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            RaiseEvent(s_eventBorderStyleChanged, e);
        }
        protected virtual void OnHideSelectionChanged(EventArgs e)
        {
            RaiseEvent(s_eventHideSelectionChanged, e);
        }
        protected virtual void OnModifiedChanged(EventArgs e)
        {
            RaiseEvent(s_eventModifiedChanged, e);
        }
        protected virtual void OnMultilineChanged(EventArgs e)
        {
            RaiseEvent(s_eventMultilineChanged, e);
        }
        protected virtual void OnReadOnlyChanged(EventArgs e)
        {
            RaiseEvent(s_eventReadOnlyChanged, e);
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            if (control is TextBox textBox)
            {
                // Please keep this alphabetized and in sync with Unsubscribe
                //
                textBox.AcceptsTabChanged += new EventHandler(HandleAcceptsTabChanged);
                textBox.BorderStyleChanged += new EventHandler(HandleBorderStyleChanged);
                textBox.HideSelectionChanged += new EventHandler(HandleHideSelectionChanged);
                textBox.ModifiedChanged += new EventHandler(HandleModifiedChanged);
                textBox.MultilineChanged += new EventHandler(HandleMultilineChanged);
                textBox.ReadOnlyChanged += new EventHandler(HandleReadOnlyChanged);
                textBox.TextAlignChanged += new EventHandler(HandleTextBoxTextAlignChanged);
            }

            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            if (control is TextBox textBox)
            {
                // Please keep this alphabetized and in sync with Subscribe
                //
                textBox.AcceptsTabChanged -= new EventHandler(HandleAcceptsTabChanged);
                textBox.BorderStyleChanged -= new EventHandler(HandleBorderStyleChanged);
                textBox.HideSelectionChanged -= new EventHandler(HandleHideSelectionChanged);
                textBox.ModifiedChanged -= new EventHandler(HandleModifiedChanged);
                textBox.MultilineChanged -= new EventHandler(HandleMultilineChanged);
                textBox.ReadOnlyChanged -= new EventHandler(HandleReadOnlyChanged);
                textBox.TextAlignChanged -= new EventHandler(HandleTextBoxTextAlignChanged);
            }
            base.OnUnsubscribeControlEvents(control);
        }

        internal override bool ShouldSerializeFont()
        {
            return Font != ToolStripManager.DefaultFont;
        }

        #region WrappedProperties
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxAcceptsTabDescr))]
        public bool AcceptsTab
        {
            get { return TextBox.AcceptsTab; }
            set { TextBox.AcceptsTab = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxAcceptsReturnDescr))]
        public bool AcceptsReturn
        {
            get { return TextBox.AcceptsReturn; }
            set { TextBox.AcceptsReturn = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr))]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return TextBox.AutoCompleteCustomSource; }
            set { TextBox.AutoCompleteCustomSource = value; }
        }

        [DefaultValue(AutoCompleteMode.None)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return TextBox.AutoCompleteMode; }
            set { TextBox.AutoCompleteMode = value; }
        }

        [DefaultValue(AutoCompleteSource.None)]
        [SRDescription(nameof(SR.TextBoxAutoCompleteSourceDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return TextBox.AutoCompleteSource; }
            set { TextBox.AutoCompleteSource = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.Fixed3D)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.TextBoxBorderDescr))]
        public BorderStyle BorderStyle
        {
            get => TextBox.BorderStyle;
            set => TextBox.BorderStyle = value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxCanUndoDescr))]
        public bool CanUndo
        {
            get { return TextBox.CanUndo; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(CharacterCasing.Normal)]
        [SRDescription(nameof(SR.TextBoxCharacterCasingDescr))]
        public CharacterCasing CharacterCasing
        {
            get { return TextBox.CharacterCasing; }
            set { TextBox.CharacterCasing = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxHideSelectionDescr))]
        public bool HideSelection
        {
            get { return TextBox.HideSelection; }
            set { TextBox.HideSelection = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxLinesDescr))]
        [Editor("System.Windows.Forms.Design.StringArrayEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        public string[] Lines
        {
            get { return TextBox.Lines; }
            set { TextBox.Lines = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(32767)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxMaxLengthDescr))]
        public int MaxLength
        {
            get { return TextBox.MaxLength; }
            set { TextBox.MaxLength = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxModifiedDescr))]
        public bool Modified
        {
            get { return TextBox.Modified; }
            set { TextBox.Modified = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [Localizable(true)]
        [SRDescription(nameof(SR.TextBoxMultilineDescr))]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Multiline
        {
            get { return TextBox.Multiline; }
            set { TextBox.Multiline = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TextBoxReadOnlyDescr))]
        public bool ReadOnly
        {
            get { return TextBox.ReadOnly; }
            set { TextBox.ReadOnly = value; }
        }
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectedTextDescr))]
        public string SelectedText
        {
            get { return TextBox.SelectedText; }
            set { TextBox.SelectedText = value; }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectionLengthDescr))]
        public int SelectionLength
        {
            get { return TextBox.SelectionLength; }
            set { TextBox.SelectionLength = value; }
        }
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TextBoxSelectionStartDescr))]
        public int SelectionStart
        {
            get { return TextBox.SelectionStart; }
            set { TextBox.SelectionStart = value; }
        }
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxShortcutsEnabledDescr))]
        public bool ShortcutsEnabled
        {
            get { return TextBox.ShortcutsEnabled; }
            set { TextBox.ShortcutsEnabled = value; }
        }
        [Browsable(false)]
        public int TextLength
        {
            get { return TextBox.TextLength; }
        }

        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(HorizontalAlignment.Left)]
        [SRDescription(nameof(SR.TextBoxTextAlignDescr))]
        public HorizontalAlignment TextBoxTextAlign
        {
            get { return TextBox.TextAlign; }
            set { TextBox.TextAlign = value; }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [Localizable(true)]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.TextBoxWordWrapDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool WordWrap
        {
            get { return TextBox.WordWrap; }
            set { TextBox.WordWrap = value; }
        }

        #endregion WrappedProperties

        #region WrappedEvents
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnAcceptsTabChangedDescr))]
        public event EventHandler AcceptsTabChanged
        {
            add => Events.AddHandler(s_eventAcceptsTabChanged, value);
            remove => Events.RemoveHandler(s_eventAcceptsTabChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnBorderStyleChangedDescr))]
        public event EventHandler BorderStyleChanged
        {
            add => Events.AddHandler(s_eventBorderStyleChanged, value);
            remove => Events.RemoveHandler(s_eventBorderStyleChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnHideSelectionChangedDescr))]
        public event EventHandler HideSelectionChanged
        {
            add => Events.AddHandler(s_eventHideSelectionChanged, value);
            remove => Events.RemoveHandler(s_eventHideSelectionChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnModifiedChangedDescr))]
        public event EventHandler ModifiedChanged
        {
            add => Events.AddHandler(s_eventModifiedChanged, value);
            remove => Events.RemoveHandler(s_eventModifiedChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnMultilineChangedDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler MultilineChanged
        {
            add => Events.AddHandler(s_eventMultilineChanged, value);
            remove => Events.RemoveHandler(s_eventMultilineChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.TextBoxBaseOnReadOnlyChangedDescr))]
        public event EventHandler ReadOnlyChanged
        {
            add => Events.AddHandler(s_eventReadOnlyChanged, value);
            remove => Events.RemoveHandler(s_eventReadOnlyChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ToolStripTextBoxTextBoxTextAlignChangedDescr))]
        public event EventHandler TextBoxTextAlignChanged
        {
            add => Events.AddHandler(s_eventTextBoxTextAlignChanged, value);
            remove => Events.RemoveHandler(s_eventTextBoxTextAlignChanged, value);
        }
        #endregion WrappedEvents

        #region WrappedMethods
        public void AppendText(string text) { TextBox.AppendText(text); }
        public void Clear() { TextBox.Clear(); }
        public void ClearUndo() { TextBox.ClearUndo(); }
        public void Copy() { TextBox.Copy(); }
        public void Cut() { TextBox.Copy(); }
        public void DeselectAll() { TextBox.DeselectAll(); }
        public char GetCharFromPosition(Point pt) { return TextBox.GetCharFromPosition(pt); }
        public int GetCharIndexFromPosition(Point pt) { return TextBox.GetCharIndexFromPosition(pt); }
        public int GetFirstCharIndexFromLine(int lineNumber) { return TextBox.GetFirstCharIndexFromLine(lineNumber); }
        public int GetFirstCharIndexOfCurrentLine() { return TextBox.GetFirstCharIndexOfCurrentLine(); }
        public int GetLineFromCharIndex(int index) { return TextBox.GetLineFromCharIndex(index); }
        public Point GetPositionFromCharIndex(int index) { return TextBox.GetPositionFromCharIndex(index); }
        public void Paste() { TextBox.Paste(); }
        public void ScrollToCaret() { TextBox.ScrollToCaret(); }
        public void Select(int start, int length) { TextBox.Select(start, length); }
        public void SelectAll() { TextBox.SelectAll(); }
        public void Undo() { TextBox.Undo(); }
        #endregion
    }
}
