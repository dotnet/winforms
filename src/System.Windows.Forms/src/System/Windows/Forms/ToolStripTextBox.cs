// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Drawing;
    using System.Drawing.Design;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms.Layout;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Design; 
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Win32;
    
    /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox"]/*' />
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class ToolStripTextBox : ToolStripControlHost {

        internal static readonly object EventTextBoxTextAlignChanged                              = new object();
        internal static readonly object EventAcceptsTabChanged                                    = new object();
        internal static readonly object EventBorderStyleChanged                                   = new object();
        internal static readonly object EventHideSelectionChanged                                 = new object();
        internal static readonly object EventReadOnlyChanged                                      = new object();
        internal static readonly object EventMultilineChanged                                     = new object();
        internal static readonly object EventModifiedChanged                                      = new object();
        
        private static readonly Padding defaultMargin = new Padding(1, 0, 1, 0);
        private static readonly Padding defaultDropDownMargin = new Padding(1);
        private Padding scaledDefaultMargin = defaultMargin;
        private Padding scaledDefaultDropDownMargin = defaultDropDownMargin;

        /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ToolStripTextBox"]/*' />
        public ToolStripTextBox() : base(CreateControlInstance()) {
            ToolStripTextBoxControl textBox = Control as ToolStripTextBoxControl;
            textBox.Owner = this;

            if (DpiHelper.IsScalingRequirementMet) {
                scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(defaultMargin);
                scaledDefaultDropDownMargin = DpiHelper.LogicalToDeviceUnits(defaultDropDownMargin);
            }
        }

        public ToolStripTextBox(string name) : this() {
            this.Name = name;
        }
        
        /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ToolStripTextBox1"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripTextBox(Control c): base(c) {
            throw new NotSupportedException(SR.ToolStripMustSupplyItsOwnTextBox);
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }
        
        /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox.DefaultMargin"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected internal override Padding DefaultMargin {
            get {
                if (IsOnDropDown) {
                    return scaledDefaultDropDownMargin;
                }
                else {
                    return scaledDefaultMargin;
                }
            }
        }

        /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return  new Size(100,22);
            }
        }
        /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox.TextBox"]/*' />
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextBox TextBox {
            get{
                return Control as TextBox;
            }
        }

        private static Control CreateControlInstance() {
            TextBox textBox = new ToolStripTextBoxControl();  
            textBox.BorderStyle = BorderStyle.Fixed3D;
            textBox.AutoSize = true;
            return textBox;
        }

     
        /// <include file='doc\ToolStripTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetPreferredSize"]/*' />
        public override Size GetPreferredSize(Size constrainingSize) {
            // dont call TextBox.GPS because it will grow and shrink as the text changes.
            Rectangle bounds = CommonProperties.GetSpecifiedBounds(TextBox);
            return new Size(bounds.Width, TextBox.PreferredHeight);               
        }
        private void HandleAcceptsTabChanged(object sender, EventArgs e) {
            OnAcceptsTabChanged(e);
        }
        private void HandleBorderStyleChanged(object sender, EventArgs e) {
            OnBorderStyleChanged(e);
        }
        private void HandleHideSelectionChanged(object sender, EventArgs e) {
            OnHideSelectionChanged(e);
        }
        private void HandleModifiedChanged(object sender, EventArgs e) {
            OnModifiedChanged(e);
        }      
        private void HandleMultilineChanged(object sender, EventArgs e) {
            OnMultilineChanged(e);
        }      
        private void HandleReadOnlyChanged(object sender, EventArgs e) {
            OnReadOnlyChanged(e);
        }               
        private void HandleTextBoxTextAlignChanged(object sender, EventArgs e) {
            RaiseEvent(EventTextBoxTextAlignChanged, e);
        }               
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnAcceptsTabChanged"]/*' />
        protected virtual void OnAcceptsTabChanged(EventArgs e) {
            RaiseEvent(EventAcceptsTabChanged, e);            
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnBorderStyleChanged"]/*' />
        protected virtual void OnBorderStyleChanged(EventArgs e) {
            RaiseEvent(EventBorderStyleChanged, e);            
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnHideSelectionChanged"]/*' />
        protected virtual void OnHideSelectionChanged(EventArgs e) {
            RaiseEvent(EventHideSelectionChanged, e);            
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnModifiedChanged"]/*' />
        protected virtual void OnModifiedChanged(EventArgs e) {
            RaiseEvent(EventModifiedChanged, e);            
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnMultilineChanged"]/*' />
        protected virtual void OnMultilineChanged(EventArgs e) {
            RaiseEvent(EventMultilineChanged, e);            
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnReadOnlyChanged"]/*' />
        protected virtual void OnReadOnlyChanged(EventArgs e) {
            RaiseEvent(EventReadOnlyChanged, e);            
        }


        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnSubscribeControlEvents"]/*' />
        protected override void OnSubscribeControlEvents(Control control) {
            TextBox textBox = control as TextBox;
            if (textBox != null) {
                // Please keep this alphabetized and in sync with Unsubscribe
                // 
                textBox.AcceptsTabChanged             += new EventHandler(HandleAcceptsTabChanged);
                textBox.BorderStyleChanged            += new EventHandler(HandleBorderStyleChanged);
                textBox.HideSelectionChanged          += new EventHandler(HandleHideSelectionChanged);
                textBox.ModifiedChanged               += new EventHandler(HandleModifiedChanged);
                textBox.MultilineChanged              += new EventHandler(HandleMultilineChanged);
                textBox.ReadOnlyChanged               += new EventHandler(HandleReadOnlyChanged);
                textBox.TextAlignChanged              += new EventHandler(HandleTextBoxTextAlignChanged);
             
            }      
    
            base.OnSubscribeControlEvents(control);
        }
      
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.OnUnsubscribeControlEvents"]/*' />
        protected override void OnUnsubscribeControlEvents(Control control) {

              TextBox textBox = control as TextBox;
              if (textBox != null) {
                  // Please keep this alphabetized and in sync with Subscribe
                  // 
                  textBox.AcceptsTabChanged             -= new EventHandler(HandleAcceptsTabChanged);
                  textBox.BorderStyleChanged            -= new EventHandler(HandleBorderStyleChanged);
                  textBox.HideSelectionChanged          -= new EventHandler(HandleHideSelectionChanged);
                  textBox.ModifiedChanged               -= new EventHandler(HandleModifiedChanged);
                  textBox.MultilineChanged              -= new EventHandler(HandleMultilineChanged);
                  textBox.ReadOnlyChanged               -= new EventHandler(HandleReadOnlyChanged);
                  textBox.TextAlignChanged              -= new EventHandler(HandleTextBoxTextAlignChanged);
              }      
              base.OnUnsubscribeControlEvents(control);

        }
            
        internal override bool ShouldSerializeFont() {
            return Font != ToolStripManager.DefaultFont;
        }


    
#region WrappedProperties   
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AcceptsTab"]/*' />
        [
         SRCategory(nameof(SR.CatBehavior)),
         DefaultValue(false),
         SRDescription(nameof(SR.TextBoxAcceptsTabDescr))
         ]
        public bool AcceptsTab { 
            get { return TextBox.AcceptsTab; } 
            set { TextBox.AcceptsTab = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AcceptsReturn"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxAcceptsReturnDescr))
        ]
        public bool AcceptsReturn { 
            get { return TextBox.AcceptsReturn; } 
            set { TextBox.AcceptsReturn = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AutoCompleteCustomSource"]/*' />
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxAutoCompleteCustomSourceDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public System.Windows.Forms.AutoCompleteStringCollection AutoCompleteCustomSource { 
            get { return TextBox.AutoCompleteCustomSource; } 
            set { TextBox.AutoCompleteCustomSource = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AutoCompleteMode"]/*' />
        [
        DefaultValue(AutoCompleteMode.None),
        SRDescription(nameof(SR.TextBoxAutoCompleteModeDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteMode AutoCompleteMode {
            get { return TextBox.AutoCompleteMode; } 
            set { TextBox.AutoCompleteMode = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AutoCompleteSource"]/*' />
        [
        DefaultValue(AutoCompleteSource.None),
        SRDescription(nameof(SR.TextBoxAutoCompleteSourceDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource { 
            get { return TextBox.AutoCompleteSource; } 
            set { TextBox.AutoCompleteSource = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.BorderStyle"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.TextBoxBorderDescr))
        ]
        public BorderStyle BorderStyle { 
            get { return TextBox.BorderStyle; } 
            set { TextBox.BorderStyle = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.CanUndo"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxCanUndoDescr))
        ]
        public bool CanUndo { 
            get { return TextBox.CanUndo; } 
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.CharacterCasing"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(CharacterCasing.Normal),
        SRDescription(nameof(SR.TextBoxCharacterCasingDescr))
        ]
        public CharacterCasing CharacterCasing { 
            get { return TextBox.CharacterCasing; } 
            set { TextBox.CharacterCasing = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.HideSelection"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TextBoxHideSelectionDescr))
        ]
        public bool HideSelection {
            get { return TextBox.HideSelection; } 
            set { TextBox.HideSelection = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Lines"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxLinesDescr)),
        Editor("System.Windows.Forms.Design.StringArrayEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public string[] Lines { 
            get { return TextBox.Lines; } 
            set { TextBox.Lines = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.MaxLength"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(32767),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxMaxLengthDescr))
        ]
        public int MaxLength {
            get { return TextBox.MaxLength; }
            set { TextBox.MaxLength = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Modified"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxModifiedDescr))
        ]
        public bool Modified { 
            get { return TextBox.Modified; }
            set { TextBox.Modified = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Multiline"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Localizable(true),
        SRDescription(nameof(SR.TextBoxMultilineDescr)),
        RefreshProperties(RefreshProperties.All),
        Browsable(false),EditorBrowsable(EditorBrowsableState.Never)
        ]
        public bool Multiline { 
            get { return TextBox.Multiline; }
            set { TextBox.Multiline = value; }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ReadOnly"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TextBoxReadOnlyDescr))
        ]
        public bool ReadOnly {
            get { return TextBox.ReadOnly; }
            set { TextBox.ReadOnly = value; }
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.SelectedText"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectedTextDescr))
        ]
        public string SelectedText { 
            get { return TextBox.SelectedText; }
            set { TextBox.SelectedText = value; }
        }
       
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.SelectionLength"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectionLengthDescr))
        ]
        public int SelectionLength {
            get { return TextBox.SelectionLength; }
            set { TextBox.SelectionLength = value; }
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.SelectionStart"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectionStartDescr))
        ]
        public int SelectionStart {
            get { return TextBox.SelectionStart; }
            set { TextBox.SelectionStart = value; }
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ShortcutsEnabled"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TextBoxShortcutsEnabledDescr))
        ]
        public bool ShortcutsEnabled { 
            get { return TextBox.ShortcutsEnabled; }
            set { TextBox.ShortcutsEnabled = value; }
        }
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.TextLength"]/*' />
        [Browsable(false)]
        public int TextLength { 
            get { return TextBox.TextLength; }
        }
        
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        SRDescription(nameof(SR.TextBoxTextAlignDescr))
        ]
        public HorizontalAlignment TextBoxTextAlign { 
            get { return TextBox.TextAlign; } 
            set { TextBox.TextAlign = value; }
        }
        
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.WordWrap"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(true),
        SRDescription(nameof(SR.TextBoxWordWrapDescr)),
        Browsable(false),EditorBrowsable(EditorBrowsableState.Never)
        ]
        public bool WordWrap { 
            get { return TextBox.WordWrap; } 
            set { TextBox.WordWrap = value; }
        }
 
 
#endregion WrappedProperties      

    

#region WrappedEvents
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AcceptsTabChanged"]/*' />
       [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnAcceptsTabChangedDescr))]
       public event EventHandler AcceptsTabChanged {
            add {
                Events.AddHandler(EventAcceptsTabChanged, value);
            }
            remove {
                Events.RemoveHandler(EventAcceptsTabChanged, value);
            }
        }

       
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.BorderStyleChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnBorderStyleChangedDescr))]
        public event EventHandler BorderStyleChanged {
            add {
                Events.AddHandler(EventBorderStyleChanged, value);
            }
            remove {
                Events.RemoveHandler(EventBorderStyleChanged, value);
            }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.HideSelectionChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnHideSelectionChangedDescr))]
        public event EventHandler HideSelectionChanged {
            add {
                Events.AddHandler(EventHideSelectionChanged, value);
            }
            remove {
                Events.RemoveHandler(EventHideSelectionChanged, value);
            }
        }
        
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ModifiedChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnModifiedChangedDescr))]
        public event EventHandler ModifiedChanged {
            add {
                Events.AddHandler(EventModifiedChanged, value);
            }
            remove {
                Events.RemoveHandler(EventModifiedChanged, value);
            }
        }

        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.MultilineChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnMultilineChangedDescr)),Browsable(false),EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler MultilineChanged {
           add {
               Events.AddHandler(EventMultilineChanged, value);
           }
           remove {
               Events.RemoveHandler(EventMultilineChanged, value);
           }
        }
        
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ReadOnlyChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.TextBoxBaseOnReadOnlyChangedDescr))]
        public event EventHandler ReadOnlyChanged {
           add {
               Events.AddHandler(EventReadOnlyChanged, value);
           }
           remove {
               Events.RemoveHandler(EventReadOnlyChanged, value);
           }
        }

        
        /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.TextAlignChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripTextBoxTextBoxTextAlignChangedDescr))]
        public event EventHandler TextBoxTextAlignChanged {
            add {
                Events.AddHandler(EventTextBoxTextAlignChanged, value);
            }
            remove {
                Events.RemoveHandler(EventTextBoxTextAlignChanged, value);
            }
        }
#endregion WrappedEvents

#region WrappedMethods
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.AppendText"]/*' />
       public void AppendText(string text) { TextBox.AppendText(text); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Clear"]/*' />
       public void Clear(){ TextBox.Clear(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ClearUndo"]/*' />
       public void ClearUndo() {TextBox.ClearUndo(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Copy"]/*' />
       public void Copy() {TextBox.Copy(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Cut"]/*' />
       public void Cut() {TextBox.Copy(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.DeselectAll"]/*' />
       public void DeselectAll() { TextBox.DeselectAll(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetCharFromPosition"]/*' />
       public char GetCharFromPosition(System.Drawing.Point pt) { return TextBox.GetCharFromPosition(pt); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetCharIndexFromPosition"]/*' />
       public int GetCharIndexFromPosition(System.Drawing.Point pt) { return TextBox.GetCharIndexFromPosition(pt); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetFirstCharIndexFromLine"]/*' />
       public int GetFirstCharIndexFromLine(int lineNumber) { return TextBox.GetFirstCharIndexFromLine(lineNumber); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetFirstCharIndexOfCurrentLine"]/*' />
       public int GetFirstCharIndexOfCurrentLine() { return TextBox.GetFirstCharIndexOfCurrentLine(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetLineFromCharIndex"]/*' />
       public int GetLineFromCharIndex(int index) { return TextBox.GetLineFromCharIndex(index); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.GetPositionFromCharIndex"]/*' />
       public System.Drawing.Point GetPositionFromCharIndex(int index) { return TextBox.GetPositionFromCharIndex(index); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Paste"]/*' />
       public void Paste() {  TextBox.Paste(); } 
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.ScrollToCaret"]/*' />
       public void ScrollToCaret() {  TextBox.ScrollToCaret(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Select"]/*' />
       public void Select(int start, int length) {  TextBox.Select(start, length); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.SelectAll"]/*' />
       public void SelectAll() { TextBox.SelectAll(); }
       /// <include file='doc\WinBarTextBox.uex' path='docs/doc[@for="ToolStripTextBox.Undo"]/*' />
       public void Undo() {  TextBox.Undo(); }
#endregion
        private class ToolStripTextBoxControl : TextBox {
                private bool mouseIsOver = false;
                private ToolStripTextBox ownerItem;
                private bool isFontSet = true;
                private bool alreadyHooked;
                
               [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily")]  // FXCop doesnt understand that setting Font changes the value of isFontSet
                public ToolStripTextBoxControl() {
                    // required to make the text box height match the combo.
                    this.Font = ToolStripManager.DefaultFont;      
                    isFontSet = false;
                }

              
                // returns the distance from the client rect to the upper left hand corner of the control
                private NativeMethods.RECT AbsoluteClientRECT {
                    get {
                        NativeMethods.RECT rect = new NativeMethods.RECT();
                        CreateParams cp = CreateParams;

                        AdjustWindowRectEx(ref rect, cp.Style, HasMenu, cp.ExStyle);

                        // the coordinates we get back are negative, we need to translate this back to positive.
                        int offsetX = -rect.left; // one to get back to 0,0, another to translate
                        int offsetY = -rect.top;

                        // fetch the client rect, then apply the offset.
                        UnsafeNativeMethods.GetClientRect(new HandleRef(this, this.Handle), ref rect);

                        rect.left   += offsetX;
                        rect.right  += offsetX;
                        rect.top    += offsetY;
                        rect.bottom += offsetY;
                        
                        return rect;                        
                    }
                }
                private Rectangle AbsoluteClientRectangle {
                    get {
                        NativeMethods.RECT rect = AbsoluteClientRECT;
                        return Rectangle.FromLTRB(rect.top, rect.top, rect.right, rect.bottom);
                    }
                }

                
                private ProfessionalColorTable ColorTable {
                     get {
                         if (Owner != null) {
                             ToolStripProfessionalRenderer renderer = Owner.Renderer as ToolStripProfessionalRenderer;
                             if (renderer != null) {
                                 return renderer.ColorTable;
                             }
                         }
                         return ProfessionalColors.ColorTable;
                     }
                }
                
                private bool IsPopupTextBox {
                   get { 
                        return ((BorderStyle == BorderStyle.Fixed3D) && 
                                 (Owner != null && (Owner.Renderer is ToolStripProfessionalRenderer)));
                    }
                }

                internal bool MouseIsOver {
                    get { return mouseIsOver; }
                    set {
                        if (mouseIsOver != value) {
                            mouseIsOver = value;
                            if (!Focused) {
                                InvalidateNonClient();
                            }
                        }
                    }
                }

                public override Font Font {
                    get { return base.Font; }
                    set { 
                        base.Font = value;
                        isFontSet = ShouldSerializeFont();    
                    }
                }

                public ToolStripTextBox Owner {
                   get { return ownerItem; }
                   set { ownerItem = value; }
                }

                private void InvalidateNonClient() {
                    if (!IsPopupTextBox) {
                        return;
                    }
                    NativeMethods.RECT absoluteClientRectangle = AbsoluteClientRECT;
                    HandleRef hNonClientRegion = NativeMethods.NullHandleRef;
                    HandleRef hClientRegion = NativeMethods.NullHandleRef;
                    HandleRef hTotalRegion = NativeMethods.NullHandleRef;
                    
                    try {
                        // get the total client area, then exclude the client by using XOR
                        hTotalRegion = new HandleRef(this, SafeNativeMethods.CreateRectRgn(0, 0, this.Width, this.Height));
                        hClientRegion = new HandleRef(this, SafeNativeMethods.CreateRectRgn(absoluteClientRectangle.left, absoluteClientRectangle.top, absoluteClientRectangle.right, absoluteClientRectangle.bottom));
                        hNonClientRegion = new HandleRef(this, SafeNativeMethods.CreateRectRgn(0,0,0,0));
                        
                        SafeNativeMethods.CombineRgn(hNonClientRegion, hTotalRegion, hClientRegion, NativeMethods.RGN_XOR);

                        // Call RedrawWindow with the region.
                        NativeMethods.RECT ignored = new NativeMethods.RECT();
                        SafeNativeMethods.RedrawWindow(new HandleRef(this, Handle),
                                                       ref ignored , hNonClientRegion,
                                                       NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_ERASE |
                                                       NativeMethods.RDW_UPDATENOW  | NativeMethods.RDW_ERASENOW   | 
                                                       NativeMethods.RDW_FRAME);
                    }
                    finally {
                        // clean up our regions.
                        try {
                            if (hNonClientRegion.Handle != IntPtr.Zero) {
                                SafeNativeMethods.DeleteObject(hNonClientRegion);
                            }
                        }
                        finally {
                            try {
                                if (hClientRegion.Handle != IntPtr.Zero) {
                                    SafeNativeMethods.DeleteObject(hClientRegion);
                                }
                            }
                            finally {
                                if (hTotalRegion.Handle != IntPtr.Zero) {
                                    SafeNativeMethods.DeleteObject(hTotalRegion);
                                }
                            }
                        }
                     
                    }                  
          
                }

                protected override void OnGotFocus(EventArgs e) {
                    base.OnGotFocus(e);
                    InvalidateNonClient();
                }

                protected override void OnLostFocus(EventArgs e) {
                    base.OnLostFocus(e);
                    InvalidateNonClient();
                
                }
           
                protected override void OnMouseEnter(EventArgs e) {
                    base.OnMouseEnter(e);
                    MouseIsOver = true;

                }

                protected override void OnMouseLeave(EventArgs e) {
                    base.OnMouseLeave(e);
                    MouseIsOver = false;
                }
               
                
                private void HookStaticEvents(bool hook) {
                   if (hook) {
                       if (!alreadyHooked) {
                          try {
                             SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                          }
                          finally{
                             alreadyHooked = true;
                          } 
                      }                  
                   }
                   else if (alreadyHooked) {
                        try {
                            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                        }
                        finally {
                            alreadyHooked = false;
                        }
                   }

                }

                private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) {
                    if (e.Category == UserPreferenceCategory.Window) {
                        if (!isFontSet) {
                            this.Font = ToolStripManager.DefaultFont;
                        }
                    }
                }

                protected override void OnVisibleChanged(EventArgs e) {
                    base.OnVisibleChanged(e);
                    if (!Disposing && !IsDisposed) {
                        HookStaticEvents(Visible);
                    }
                }

                protected override void Dispose(bool disposing) {
                    if(disposing) {
                        HookStaticEvents(false);
                    }
                    base.Dispose(disposing);
                }

                private void WmNCPaint(ref Message m) {

                    if (!IsPopupTextBox) {
                        base.WndProc(ref m);
                        return;
                    }

                    // Paint over the edges of the text box.

                    // Using GetWindowDC instead of GetDCEx as GetDCEx seems to return a null handle and a last error of 
                    // the operation succeeded.  We're not going to use the clipping rect anyways - so it's not
                    // that bigga deal.
                    HandleRef hdc = new HandleRef(this, UnsafeNativeMethods.GetWindowDC(new HandleRef(this,m.HWnd)));
                    if (hdc.Handle == IntPtr.Zero) {
                        throw new Win32Exception();
                    }
                    try {
                        // Don't set the clipping region based on the WParam - windows seems to take out the two pixels intended for the non-client border.
                        
                        Color outerBorderColor = (MouseIsOver || Focused) ? ColorTable.TextBoxBorder : this.BackColor;
                        Color innerBorderColor = this.BackColor;

                        if (!Enabled) {
                            outerBorderColor = SystemColors.ControlDark;
                            innerBorderColor = SystemColors.Control;
                        }
                        using (Graphics g = Graphics.FromHdcInternal(hdc.Handle)) {

                            Rectangle clientRect = AbsoluteClientRectangle;

                            // could have set up a clip and fill-rectangled, thought this would be faster.
                            using (Brush b = new SolidBrush(innerBorderColor)) {
                                g.FillRectangle(b, 0, 0, this.Width, clientRect.Top); // top border
                                g.FillRectangle(b, 0, 0, clientRect.Left, this.Height); // left border
                                g.FillRectangle(b, 0, clientRect.Bottom, this.Width, this.Height - clientRect.Height); // bottom border
                                g.FillRectangle(b, clientRect.Right, 0, this.Width - clientRect.Right, this.Height); // right border
                            }
   
                            // paint the outside rect.
                            using (Pen p = new Pen(outerBorderColor)) {
                                g.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                            }
                         
                        }
                    }
                    finally {
                        UnsafeNativeMethods.ReleaseDC(new HandleRef(this, this.Handle),hdc);
                    }
                    // we've handled WM_NCPAINT.
                    m.Result = IntPtr.Zero; 

                }
                protected override void WndProc(ref Message m) {
                    if (m.Msg == NativeMethods.WM_NCPAINT) {
                        WmNCPaint(ref m);
                        return;
                    }
                    else {
                        base.WndProc(ref m);
                    }
                }
        }

        
    }

}
