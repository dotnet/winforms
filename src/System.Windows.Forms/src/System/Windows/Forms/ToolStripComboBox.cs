// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing.Design;
    using System.Collections.Specialized;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms.Design; 
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Internal;     
    using System.Globalization;
    
    /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox"]/*' />
    /// <devdoc/>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    [DefaultProperty(nameof(Items))]
    public class ToolStripComboBox : ToolStripControlHost {


        internal static readonly object EventDropDown                                    = new object();
        internal static readonly object EventDropDownClosed                              = new object();
        internal static readonly object EventDropDownStyleChanged                        = new object();
        internal static readonly object EventSelectedIndexChanged                        = new object();
        internal static readonly object EventSelectionChangeCommitted                    = new object();
        internal static readonly object EventTextUpdate                                   = new object();

        private static readonly Padding dropDownPadding = new Padding(2);
        private static readonly Padding padding = new Padding(1, 0, 1, 0);

        private Padding scaledDropDownPadding = dropDownPadding;
        private Padding scaledPadding = padding;
    
        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.ToolStripComboBox"]/*' />
        public ToolStripComboBox() : base(CreateControlInstance()) {
            ToolStripComboBoxControl combo = Control as ToolStripComboBoxControl;
            combo.Owner = this;

            if (DpiHelper.IsScalingRequirementMet) {
                scaledPadding = DpiHelper.LogicalToDeviceUnits(padding);
                scaledDropDownPadding = DpiHelper.LogicalToDeviceUnits(dropDownPadding);
            }
        }


        public ToolStripComboBox(string name) : this() {
            this.Name = name;
        }
      
        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.ToolStripComboBox1"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripComboBox(Control c) : base(c) {
            throw new NotSupportedException(SR.ToolStripMustSupplyItsOwnComboBox);
        }

        private static Control CreateControlInstance() {
            ComboBox comboBox = new ToolStripComboBoxControl();  
            comboBox.FlatStyle = FlatStyle.Popup;
            comboBox.Font = ToolStripManager.DefaultFont;
            return comboBox;
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.AutoCompleteCustomSource"]/*' />
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxAutoCompleteCustomSourceDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public System.Windows.Forms.AutoCompleteStringCollection AutoCompleteCustomSource { 
            get { return ComboBox.AutoCompleteCustomSource; }
            set { ComboBox.AutoCompleteCustomSource = value;}
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.AutoCompleteMode"]/*' />
        [
        DefaultValue(AutoCompleteMode.None),
        SRDescription(nameof(SR.ComboBoxAutoCompleteModeDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteMode AutoCompleteMode { 
            get { return ComboBox.AutoCompleteMode; }
            set { ComboBox.AutoCompleteMode = value;}
        }
        
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.AutoCompleteSource"]/*' />
        [
        DefaultValue(AutoCompleteSource.None),
        SRDescription(nameof(SR.ComboBoxAutoCompleteSourceDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource { 
            get { return ComboBox.AutoCompleteSource; }
            set { ComboBox.AutoCompleteSource = value;}
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

        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.ComboBox"]/*' />
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboBox ComboBox {
            get{
                return Control as ComboBox;
            }
        }

        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return new Size(100,22);
            }
        }

        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DefaultMargin"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected internal override Padding DefaultMargin {
            get {
                if (IsOnDropDown) {
                    return scaledDropDownPadding;
                }
                else {
                    return scaledPadding;
                }
            }
        }
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler DoubleClick {
            add {
                base.DoubleClick+=value;
            }
            remove {
                base.DoubleClick -= value;
            }
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DropDown"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxOnDropDownDescr))]
        public event EventHandler DropDown {
            add {
                Events.AddHandler(EventDropDown, value);
            }
            remove {
                Events.RemoveHandler(EventDropDown, value);
            }
        }

         /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DropDown"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxOnDropDownClosedDescr))]
        public event EventHandler DropDownClosed {
            add {
                Events.AddHandler(EventDropDownClosed, value);
            }
            remove {
                Events.RemoveHandler(EventDropDownClosed, value);
            }
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DropDownStyleChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxDropDownStyleChangedDescr))]
        public event EventHandler DropDownStyleChanged {
            add {
                Events.AddHandler(EventDropDownStyleChanged, value);
            }
            remove {
                Events.RemoveHandler(EventDropDownStyleChanged, value);
            }
        }


        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DropDownHeight"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ComboBoxDropDownHeightDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(106)
        ]
        public int DropDownHeight { 
            get { return ComboBox.DropDownHeight; }
            set { ComboBox.DropDownHeight = value;}

        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DropDownStyle"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ComboBoxStyle.DropDown),
        SRDescription(nameof(SR.ComboBoxStyleDescr)),
        RefreshPropertiesAttribute(RefreshProperties.Repaint)
        ]
        public ComboBoxStyle DropDownStyle {
            get { return ComboBox.DropDownStyle; }
            set { ComboBox.DropDownStyle = value;}
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DropDownWidth"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ComboBoxDropDownWidthDescr))
        ]
        public int DropDownWidth { 
            get { return ComboBox.DropDownWidth; }
            set { ComboBox.DropDownWidth = value;}
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.DroppedDown"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxDroppedDownDescr))
        ]
        public bool DroppedDown { 
            get { return ComboBox.DroppedDown; }
            set { ComboBox.DroppedDown = value;}
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.FlatStyle"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(FlatStyle.Popup),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxFlatStyleDescr))
        ]
        public FlatStyle FlatStyle { 
            get { return ComboBox.FlatStyle; }
            set { ComboBox.FlatStyle = value;}
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.IntegralHeight"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxIntegralHeightDescr))
        ]
        public bool IntegralHeight {
            get { return ComboBox.IntegralHeight; }
            set { ComboBox.IntegralHeight = value;}
        }
        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.Items"]/*' />
        /// <devdoc>
        /// Collection of the items contained in this ComboBox.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxItemsDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
        ]
        public ComboBox.ObjectCollection Items {
            get{
                return ComboBox.Items;
            }
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.MaxDropDownItems"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(8),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxMaxDropDownItemsDescr))
        ]
        public int MaxDropDownItems { 
            get { return ComboBox.MaxDropDownItems; }
            set { ComboBox.MaxDropDownItems = value;}
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.MaxLength"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxMaxLengthDescr))
        ]
        public int MaxLength {
           get {  return ComboBox.MaxLength; }
           set {  ComboBox.MaxLength = value; }
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectedIndex"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectedIndexDescr))
        ]
        public int SelectedIndex { 
            get { return ComboBox.SelectedIndex; }
            set { ComboBox.SelectedIndex = value;}
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectedIndexChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged {
            add {
                Events.AddHandler(EventSelectedIndexChanged, value);
            }
            remove {
                Events.RemoveHandler(EventSelectedIndexChanged, value);
            }
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectedItem"]/*' />
        [
        Browsable(false),
        Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectedItemDescr))
        ]
        public object SelectedItem { 
            get { return ComboBox.SelectedItem; }
            set { ComboBox.SelectedItem = value;}
        }  

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectedText"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectedTextDescr))
        ]
        public string SelectedText {
            get { return ComboBox.SelectedText; }
            set { ComboBox.SelectedText = value;}
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectionLength"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectionLengthDescr))
        ]
        public int SelectionLength { 
            get { return ComboBox.SelectionLength; }
            set { ComboBox.SelectionLength = value;}
        }

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectionStart"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectionStartDescr))
        ]
        public int SelectionStart { 
            get { return ComboBox.SelectionStart; }
            set { ComboBox.SelectionStart = value;}
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.Sorted"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ComboBoxSortedDescr))
        ]
        public bool Sorted { 
            get { return ComboBox.Sorted; }
            set {ComboBox.Sorted = value;}
        }


        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.TextUpdate"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxOnTextUpdateDescr))]
        public event EventHandler TextUpdate {
            add {
                Events.AddHandler(EventTextUpdate, value);
            }
            remove {
                Events.RemoveHandler(EventTextUpdate, value);
            }
        }

#region WrappedMethods        

        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.BeginUpdate"]/*' />
        public void BeginUpdate() { ComboBox.BeginUpdate(); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.EndUpdate"]/*' />
        public void EndUpdate() { ComboBox.EndUpdate(); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.FindString"]/*' />
        public int FindString(string s) { return ComboBox.FindString(s); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.FindString1"]/*' />
        public int FindString(string s, int startIndex) { return ComboBox.FindString(s, startIndex); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.FindStringExact"]/*' />
        public int FindStringExact(string s) { return ComboBox.FindStringExact(s); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.FindStringExact1"]/*' />
        public int FindStringExact(string s, int startIndex) { return ComboBox.FindStringExact(s, startIndex); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.GetItemHeight"]/*' />
        public int GetItemHeight(int index) { return ComboBox.GetItemHeight(index); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.Select"]/*' />
        public void Select(int start, int length) { ComboBox.Select(start, length); }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.SelectAll"]/*' />
        public void SelectAll() { ComboBox.SelectAll(); }

#endregion WrappedMethods

        /// <include file='doc\ToolStripComboBox.uex' path='docs/doc[@for="ToolStripComboBox.GetPreferredSize"]/*' />
        public override Size GetPreferredSize(Size constrainingSize) {

            // 
            Size preferredSize = base.GetPreferredSize(constrainingSize);
            preferredSize.Width = Math.Max(preferredSize.Width, 75);

            return preferredSize;
        }
        private void HandleDropDown(object sender, System.EventArgs e) {
            OnDropDown(e);
        }
        private void HandleDropDownClosed(object sender, System.EventArgs e) {
            OnDropDownClosed(e);
        }

        private void HandleDropDownStyleChanged(object sender, System.EventArgs e) {
            OnDropDownStyleChanged(e);
        }
        private void HandleSelectedIndexChanged(object sender, System.EventArgs e) {
            OnSelectedIndexChanged(e);
        }
        private void HandleSelectionChangeCommitted(object sender, System.EventArgs e) {
            OnSelectionChangeCommitted(e);
        }
        private void HandleTextUpdate(object sender, System.EventArgs e) {
            OnTextUpdate(e);
        }
     
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnDropDown"]/*' />
        protected virtual void OnDropDown(EventArgs e) {
            if (ParentInternal != null) {
                Application.ThreadContext.FromCurrent().RemoveMessageFilter(ParentInternal.RestoreFocusFilter); 
                ToolStripManager.ModalMenuFilter.SuspendMenuMode();
            }            
            RaiseEvent(EventDropDown, e);            
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnDropDown"]/*' />
        protected virtual void OnDropDownClosed(EventArgs e) {
            if (ParentInternal != null) {
                // PERF, 

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(ParentInternal.RestoreFocusFilter);  
                ToolStripManager.ModalMenuFilter.ResumeMenuMode();
            }
            RaiseEvent(EventDropDownClosed, e);            
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnDropDownStyleChanged"]/*' />
        protected virtual void OnDropDownStyleChanged(EventArgs e) {
            RaiseEvent(EventDropDownStyleChanged, e);            
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnSelectedIndexChanged"]/*' />
        protected virtual void OnSelectedIndexChanged(EventArgs e) {
            RaiseEvent(EventSelectedIndexChanged, e);            
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnSelectionChangeCommitted"]/*' />
        protected virtual void OnSelectionChangeCommitted(EventArgs e) {
            RaiseEvent(EventSelectionChangeCommitted, e);            
        }
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnTextUpdate"]/*' />
        protected virtual void OnTextUpdate(EventArgs e) {
            RaiseEvent(EventTextUpdate, e);            
        }
       
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnSubscribeControlEvents"]/*' />
        protected override void OnSubscribeControlEvents(Control control) {
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null) {
                // Please keep this alphabetized and in sync with Unsubscribe
                // 
                comboBox.DropDown                   += new EventHandler(HandleDropDown);
                comboBox.DropDownClosed             += new EventHandler(HandleDropDownClosed);
                comboBox.DropDownStyleChanged       += new EventHandler(HandleDropDownStyleChanged);
                comboBox.SelectedIndexChanged       += new EventHandler(HandleSelectedIndexChanged);
                comboBox.SelectionChangeCommitted   += new EventHandler(HandleSelectionChangeCommitted);
                comboBox.TextUpdate                 += new EventHandler(HandleTextUpdate);
            }      
    
            base.OnSubscribeControlEvents(control);
        }
      
        /// <include file='doc\WinBarComboBox.uex' path='docs/doc[@for="ToolStripComboBox.OnUnsubscribeControlEvents"]/*' />
        protected override void OnUnsubscribeControlEvents(Control control) {
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null) {
               // Please keep this alphabetized and in sync with Unsubscribe
               // 
               comboBox.DropDown                   -= new EventHandler(HandleDropDown);
               comboBox.DropDownClosed             -= new EventHandler(HandleDropDownClosed);
               comboBox.DropDownStyleChanged       -= new EventHandler(HandleDropDownStyleChanged);
               comboBox.SelectedIndexChanged       -= new EventHandler(HandleSelectedIndexChanged);
               comboBox.SelectionChangeCommitted   -= new EventHandler(HandleSelectionChangeCommitted);
               comboBox.TextUpdate                 -= new EventHandler(HandleTextUpdate);
            }    
            base.OnUnsubscribeControlEvents(control);
        }

        private bool ShouldSerializeDropDownWidth() {
            return ComboBox.ShouldSerializeDropDownWidth();
        }

        internal override bool ShouldSerializeFont() {
            return !object.Equals(this.Font, ToolStripManager.DefaultFont);
        }

        public override string ToString() {
            return base.ToString() + ", Items.Count: " + Items.Count.ToString(CultureInfo.CurrentCulture);
        }


        internal class ToolStripComboBoxControl : ComboBox {
               private ToolStripComboBox owner = null;

            public ToolStripComboBoxControl() {
                   this.FlatStyle = FlatStyle.Popup;
                   SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
               }
               public ToolStripComboBox Owner {
                  get { return owner; }
                  set { owner = value; }
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

               internal override FlatComboAdapter CreateFlatComboAdapterInstance() {
                   return new ToolStripComboBoxFlatComboAdapter(this);
               }

               internal class ToolStripComboBoxFlatComboAdapter : FlatComboAdapter {

                    public ToolStripComboBoxFlatComboAdapter(ComboBox comboBox) : base(comboBox, /*smallButton=*/true) {
                    }

                    private static bool UseBaseAdapter(ComboBox comboBox) {
                        ToolStripComboBoxControl toolStripComboBox = comboBox as ToolStripComboBoxControl;
                        if (toolStripComboBox == null || !(toolStripComboBox.Owner.Renderer is ToolStripProfessionalRenderer)) {
                            Debug.Assert(toolStripComboBox != null, "Why are we here and not a toolstrip combo?");
                            return true;
                        }
                        return false;
                    }
                    private static ProfessionalColorTable GetColorTable(ToolStripComboBoxControl toolStripComboBoxControl) {
                        if (toolStripComboBoxControl != null) {
                            return toolStripComboBoxControl.ColorTable;
                        }
                        return ProfessionalColors.ColorTable;                    
                   }
                    protected override Color GetOuterBorderColor(ComboBox comboBox) {
                        if (UseBaseAdapter(comboBox)) {
                            return base.GetOuterBorderColor(comboBox);
                        }                            
                        return (comboBox.Enabled) ? SystemColors.Window : GetColorTable(comboBox as ToolStripComboBoxControl).ComboBoxBorder;
                    }

                    protected override Color GetPopupOuterBorderColor(ComboBox comboBox, bool focused) {
                        if (UseBaseAdapter(comboBox)) {
                            return base.GetPopupOuterBorderColor(comboBox, focused);
                        }
                        if (!comboBox.Enabled) {
                            return SystemColors.ControlDark;
                        }
                        return (focused) ? GetColorTable(comboBox as ToolStripComboBoxControl).ComboBoxBorder : SystemColors.Window;
                    }

                    protected override void DrawFlatComboDropDown(ComboBox comboBox, Graphics g, Rectangle dropDownRect) {
                        if (UseBaseAdapter(comboBox)) {
                             base.DrawFlatComboDropDown(comboBox, g, dropDownRect);
                             return;
                        }

                                          
                        if (!comboBox.Enabled || !ToolStripManager.VisualStylesEnabled) {
                            g.FillRectangle(SystemBrushes.Control, dropDownRect);
                        }
                        else {
                           
                            ToolStripComboBoxControl toolStripComboBox = comboBox as ToolStripComboBoxControl;
                            ProfessionalColorTable colorTable = GetColorTable(toolStripComboBox);

                            if (!comboBox.DroppedDown) {
                                bool focused = comboBox.ContainsFocus || comboBox.MouseIsOver;
                                if (focused) {
                                    using (Brush b = new LinearGradientBrush(dropDownRect, colorTable.ComboBoxButtonSelectedGradientBegin, colorTable.ComboBoxButtonSelectedGradientEnd, LinearGradientMode.Vertical)) {
                                        g.FillRectangle(b, dropDownRect);
                                    }
                                }
                                else if (toolStripComboBox.Owner.IsOnOverflow) {
                                    using (Brush b = new SolidBrush(colorTable.ComboBoxButtonOnOverflow)) {
                                        g.FillRectangle(b, dropDownRect);
                                    }
                                }
                                else {
                                    using (Brush b = new LinearGradientBrush(dropDownRect, colorTable.ComboBoxButtonGradientBegin, colorTable.ComboBoxButtonGradientEnd, LinearGradientMode.Vertical)) {
                                        g.FillRectangle(b, dropDownRect);
                                    }
                                }
                            }
                            else {                              
                                using (Brush b = new LinearGradientBrush(dropDownRect, colorTable.ComboBoxButtonPressedGradientBegin, colorTable.ComboBoxButtonPressedGradientEnd, LinearGradientMode.Vertical)) {
                                    g.FillRectangle(b, dropDownRect);
                                }                            
                            }
                        }
                        
                        Brush brush;
                        if (comboBox.Enabled) {
                            if (AccessibilityImprovements.Level2 && SystemInformation.HighContrast && (comboBox.ContainsFocus || comboBox.MouseIsOver) && ToolStripManager.VisualStylesEnabled) {
                                brush = SystemBrushes.HighlightText;
                            }
                            else {
                                brush = SystemBrushes.ControlText;
                            }
                        }
                        else {
                            brush = SystemBrushes.GrayText;
                        }
                        Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
                        
                        // if the width is odd - favor pushing it over one pixel right.
                        middle.X += (dropDownRect.Width % 2);
                        g.FillPolygon(brush, new Point[] {
                            new Point(middle.X - FlatComboAdapter.Offset2Pixels, middle.Y - 1), 
                            new Point(middle.X + FlatComboAdapter.Offset2Pixels + 1, middle.Y - 1), 
                            new Point(middle.X, middle.Y + FlatComboAdapter.Offset2Pixels)
                        });

                    }
               }

               protected override bool IsInputKey(Keys keyData) {
                   if ((keyData & Keys.Alt) == Keys.Alt) {
                       if ((keyData & Keys.Down) == Keys.Down || (keyData & Keys.Up) == Keys.Up) {
                           return true;
                       }
                   }
                   return base.IsInputKey(keyData);
               }
        
              
               protected override void OnDropDownClosed(EventArgs e) {
                   base.OnDropDownClosed(e);
                   Invalidate();
                   Update(); 
               }
               
        }
      
    }

}
