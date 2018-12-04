// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Drawing.Imaging;
    using System.ComponentModel;
    using System.Windows.Forms.Design; 
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Diagnostics;
    using System.Windows.Forms.Layout;
    using System.Runtime.Versioning;
    
    /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton"]/*' />
    /// <devdoc/>
    [
    ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip),
    DefaultEvent(nameof(ButtonClick))
    ]
    public class ToolStripSplitButton : ToolStripDropDownItem {
        
        private ToolStripItem                    defaultItem                =  null;
        private ToolStripSplitButtonButton       splitButtonButton          =  null;      
        private Rectangle                     dropDownButtonBounds       =  Rectangle.Empty;
        private ToolStripSplitButtonButtonLayout splitButtonButtonLayout    =  null;
        private int                           dropDownButtonWidth        =  0;
        private int                           splitterWidth              =  1;
        private Rectangle                     splitterBounds             =  Rectangle.Empty;
        private byte                          openMouseId                =  0;
        private long lastClickTime = 0;

        private const int DEFAULT_DROPDOWN_WIDTH = 11;

        private static readonly object EventDefaultItemChanged                         = new object();
        private static readonly object EventButtonClick                                = new object();
        private static readonly object EventButtonDoubleClick                          = new object();
        private static readonly object EventDropDownOpened                             = new object();
        private static readonly object EventDropDownClosed                             = new object();

        private static bool isScalingInitialized = false;
        private static int scaledDropDownButtonWidth = DEFAULT_DROPDOWN_WIDTH;
       
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ToolStripSplitButton"]/*' />
        /// <devdoc>
        /// Summary of ToolStripSplitButton.
        /// </devdoc>
        public ToolStripSplitButton() {
            Initialize(); // all additional work should be done in Initialize
        }
        public ToolStripSplitButton(string text):base(text,null,(EventHandler)null) {
            Initialize(); 
        }
        public ToolStripSplitButton(Image image):base(null,image,(EventHandler)null) {
            Initialize(); 
        }
        public ToolStripSplitButton(string text, Image image):base(text,image,(EventHandler)null) {
            Initialize(); 
        }
        public ToolStripSplitButton(string text, Image image, EventHandler onClick):base(text,image,onClick) {
            Initialize(); 
        }
        public ToolStripSplitButton(string text, Image image, EventHandler onClick, string name) :base(text,image,onClick,name){
            Initialize(); 
        }
        public ToolStripSplitButton(string text, Image image, params ToolStripItem[] dropDownItems):base(text,image,dropDownItems) {
            Initialize(); 
        }

        [DefaultValue(true)]
        public new bool AutoToolTip {
            get { 
                return base.AutoToolTip;
            }
            set {
                base.AutoToolTip = value;
            }
        }


        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ButtonBounds"]/*' />
        /// <devdoc>
        /// Summary of ToolStripSplitButton.
        /// </devdoc>
        [Browsable(false)]
        public Rectangle ButtonBounds {
            get {
                //Rectangle bounds = SplitButtonButton.Bounds;
                //bounds.Offset(this.Bounds.Location);
                return SplitButtonButton.Bounds;
            }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ButtonPressed"]/*' />
        /// <devdoc>
        /// Summary of ButtonPressed.
        /// </devdoc>
        [Browsable(false)]
        public bool ButtonPressed {
            get {
                return SplitButtonButton.Pressed;

            }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ButtonSelected"]/*' />
        /// <devdoc>
        /// Summary of ButtonPressed.
        /// </devdoc>
        [Browsable(false)]
        public bool ButtonSelected {
            get {
                return SplitButtonButton.Selected || DropDownButtonPressed;
            }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ButtonClick"]/*' />
        /// <devdoc>
        /// <para>Occurs when the button portion of a split button is clicked.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAction)),
        SRDescription(nameof(SR.ToolStripSplitButtonOnButtonClickDescr))
        ]
        public event EventHandler ButtonClick {
            add { 
                Events.AddHandler(EventButtonClick, value); 
            }
            remove {
                Events.RemoveHandler(EventButtonClick, value);
            }
        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ButtonDoubleClick"]/*' />
        /// <devdoc>
        /// <para>Occurs when the utton portion of a split button  is double clicked.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAction)),
        SRDescription(nameof(SR.ToolStripSplitButtonOnButtonDoubleClickDescr))
        ]
        public event EventHandler ButtonDoubleClick {
            add {
                Events.AddHandler(EventButtonDoubleClick, value);
            }
            remove {
                Events.RemoveHandler(EventButtonDoubleClick, value);
            }
        }


        protected override bool DefaultAutoToolTip {
            get { 
               return true; 
            }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DefaultItem"]/*' />
        /// <devdoc>
        /// Summary of DefaultItem.
        /// </devdoc>
        [DefaultValue(null), Browsable(false)]
        public ToolStripItem DefaultItem {
            get { 
                return defaultItem; 
            }
            set { 
                if (defaultItem != value) {
                    OnDefaultItemChanged(new EventArgs()); 
                    defaultItem = value; 
                }
            }
        }
     

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DefaultItemChanged"]/*' />
        /// <devdoc>
        /// <para>Occurs when the default item has changed</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAction)),
        SRDescription(nameof(SR.ToolStripSplitButtonOnDefaultItemChangedDescr))
        ]
        public event EventHandler DefaultItemChanged {
            add { 
                Events.AddHandler(EventDefaultItemChanged, value); 
            }
            remove {
                Events.RemoveHandler(EventDefaultItemChanged, value);
            }
        }
	
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DismissWhenClicked"]/*' />
        /// <devdoc>
        /// specifies the default behavior of these items on ToolStripDropDowns when clicked.
        /// </devdoc>
        internal protected override bool DismissWhenClicked {
            get {
                return DropDown.Visible != true;
            }

        }
        
        internal override Rectangle DropDownButtonArea {
               get { return this.DropDownButtonBounds; }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DropDownButtonBounds"]/*' />
        /// <devdoc>
        /// The bounds of the DropDown in ToolStrip coordinates.
        /// </devdoc>
        [Browsable(false)]
        public Rectangle DropDownButtonBounds {
            get {
                 return dropDownButtonBounds; 
            }

        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DropDownButtonPressed"]/*' />
        /// <devdoc>
        /// Summary of DropDownButtonBounds.
        /// </devdoc>
        [Browsable(false)]
        public bool DropDownButtonPressed {
            get {
                // 
                return DropDown.Visible; 
            }
        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DropDownButtonSelected"]/*' />
        /// <devdoc>
        /// Summary of DropDownButtonSelected.
        /// </devdoc>
        [Browsable(false)]
        public bool DropDownButtonSelected{
            get {
                return this.Selected; 
            }
        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.DropDownButtonWidth"]/*' />
        /// <devdoc>
        /// Summary of DropDownButtonWidth.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.ToolStripSplitButtonDropDownButtonWidthDescr))
        ]
        public int DropDownButtonWidth {
            get{ 
                return dropDownButtonWidth;
            }
            set {
                if (value < 0) {
                    // throw if less than 0.
                    throw new ArgumentOutOfRangeException(nameof(DropDownButtonWidth), string.Format(SR.InvalidLowBoundArgumentEx, "DropDownButtonWidth", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));                
                }
            
                if (dropDownButtonWidth != value) {
                    dropDownButtonWidth = value;
                    InvalidateSplitButtonLayout();
                    InvalidateItemLayout(PropertyNames.DropDownButtonWidth, true);
                }
             }
        }

        /// <devdoc>
        /// This is here for serialization purposes.
        /// </devdoc>
        private int DefaultDropDownButtonWidth {
            get {
                // lets start off with a size roughly equivalent to a combobox dropdown
                if (!isScalingInitialized) {
                    if (DpiHelper.IsScalingRequired) {
                        scaledDropDownButtonWidth = DpiHelper.LogicalToDeviceUnitsX(DEFAULT_DROPDOWN_WIDTH);
                    }                   
                    isScalingInitialized = true;
                }

                return scaledDropDownButtonWidth;
            }
        }

     
                
        /// <summary>
        /// Just used as a convenience to help manage layout
        /// </summary>
        private ToolStripSplitButtonButton SplitButtonButton {
            get { 
                if (splitButtonButton == null) { 
                    splitButtonButton = new ToolStripSplitButtonButton(this);
                }
                splitButtonButton.Image = this.Image;
                splitButtonButton.Text = this.Text;
                splitButtonButton.BackColor = this.BackColor;
                splitButtonButton.ForeColor = this.ForeColor;
                splitButtonButton.Font = this.Font;
                splitButtonButton.ImageAlign = this.ImageAlign;
                splitButtonButton.TextAlign = this.TextAlign;
                splitButtonButton.TextImageRelation = this.TextImageRelation;
                return splitButtonButton;
            }
        }
        /// <devdoc>
        /// Summary of SplitButtonButtonLayout.
        /// </devdoc>	
        internal ToolStripItemInternalLayout SplitButtonButtonLayout {
            get { 
                // For preferred size caching reasons, we need to keep our two 
                // internal layouts (button, dropdown button) in sync. 
             
                if (InternalLayout != null /*if layout is invalid - calls CreateInternalLayout - which resets splitButtonButtonLayout to null*/
                    && splitButtonButtonLayout == null) {
                    splitButtonButtonLayout = new ToolStripSplitButtonButtonLayout(this);
                }
                return splitButtonButtonLayout;
            }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.SplitterWidth"]/*' />
        /// <devdoc>
        /// the width of the separator between the default and drop down button
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripSplitButtonSplitterWidthDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        internal int SplitterWidth {
            get {
                return splitterWidth;
            }
            set {
                if (value < 0) {
                    splitterWidth = 0;
                }
                else {
                    splitterWidth = value;
                }
                InvalidateSplitButtonLayout();
            }
        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.SplitterBounds"]/*' />
        /// <devdoc>
        /// the boundaries of the separator between the default and drop down button, exposed for custom
        /// painting purposes.
        /// </devdoc>
        [Browsable(false)]
        public Rectangle SplitterBounds {
            get {
                return splitterBounds;
            }
        }
        /// <devdoc>
        /// Summary of CalculateLayout.
        /// </devdoc>	
        private void CalculateLayout() {

            // Figure out where the DropDown image goes.
            Rectangle dropDownButtonBounds = new Rectangle(Point.Empty, this.Size);
            Rectangle splitButtonButtonBounds = Rectangle.Empty;
			
           
            dropDownButtonBounds = new Rectangle(Point.Empty, new Size(Math.Min(this.Width, DropDownButtonWidth), this.Height));
   			
            // Figure out the height and width of the selected item.
            int splitButtonButtonWidth = Math.Max(0, this.Width - dropDownButtonBounds.Width);
            int splitButtonButtonHeight = Math.Max(0, this.Height);

            splitButtonButtonBounds = new Rectangle(Point.Empty, new Size(splitButtonButtonWidth, splitButtonButtonHeight));

            // grow the selected item by one since we're overlapping the borders.
            splitButtonButtonBounds.Width -= splitterWidth; 

            if (this.RightToLeft == RightToLeft.No) {
                // the dropdown button goes on the right
                dropDownButtonBounds.Offset(splitButtonButtonBounds.Right+splitterWidth, 0);
                splitterBounds = new Rectangle(splitButtonButtonBounds.Right, splitButtonButtonBounds.Top, splitterWidth, splitButtonButtonBounds.Height);
            }
            else {
                // the split button goes on the right.
                splitButtonButtonBounds.Offset(DropDownButtonWidth+splitterWidth, 0);
                splitterBounds = new Rectangle(dropDownButtonBounds.Right, dropDownButtonBounds.Top, splitterWidth, dropDownButtonBounds.Height);
      
            }
            
            this.SplitButtonButton.SetBounds(splitButtonButtonBounds);
            this.SetDropDownButtonBounds(dropDownButtonBounds);

        }

        protected override AccessibleObject CreateAccessibilityInstance() {
            if (AccessibilityImprovements.Level1) {
                return new ToolStripSplitButtonExAccessibleObject(this);
            }
            else {
                return new ToolStripSplitButtonAccessibleObject(this);
            }
       }

        protected override ToolStripDropDown CreateDefaultDropDown() {
             // AutoGenerate a Winbar DropDown - set the property so we hook events
              return new ToolStripDropDownMenu(this, /*isAutoGenerated=*/true);
         }


        internal override ToolStripItemInternalLayout CreateInternalLayout() {
            // whenever the master layout is invalidated - invalidate the splitbuttonbutton layout.
            this.splitButtonButtonLayout = null;
            return new ToolStripItemInternalLayout(this);
            
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.GetPreferredSize"]/*' />
        public override Size GetPreferredSize(Size constrainingSize) {
            Size preferredSize = SplitButtonButtonLayout.GetPreferredSize(constrainingSize);
            preferredSize.Width += DropDownButtonWidth + SplitterWidth + Padding.Horizontal;
            return preferredSize;
        }

        /// <devdoc>
        /// Summary of InvalidateSplitButtonLayout.
        /// </devdoc>	
        private void InvalidateSplitButtonLayout() {
            this.splitButtonButtonLayout = null;	
            CalculateLayout();
        }

        private void Initialize() {           
            dropDownButtonWidth = DefaultDropDownButtonWidth;
            SupportsSpaceKey = true;
        }

        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessDialogKey(Keys keyData) {
            if (Enabled && (keyData == Keys.Enter || (SupportsSpaceKey && keyData == Keys.Space))) {
               PerformButtonClick();
               return true;
            }
            
            return base.ProcessDialogKey(keyData);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'charCode' matches control.cs
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
           // checking IsMnemonic is not necessary - toolstrip does this for us          
           PerformButtonClick();
           return true;
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnButtonClick"]/*' />
        /// <devdoc>
        /// called when the button portion of a split button is clicked
        /// if there is a default item, this will route the click to the default item
        /// </devdoc>
        protected virtual void OnButtonClick(System.EventArgs e) {
 
            if (DefaultItem != null) {
                DefaultItem.FireEvent(ToolStripItemEventType.Click);
            }

            EventHandler handler = (EventHandler)Events[EventButtonClick];
            if (handler != null) handler(this, e);
 
        }
        
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnButtonDoubleClick"]/*' />
        /// <devdoc>
        /// called when the button portion of a split button is double clicked
        /// if there is a default item, this will route the double click to the default item
        /// </devdoc>
        public virtual void OnButtonDoubleClick(System.EventArgs e) {
            if (DefaultItem != null) {
                DefaultItem.FireEvent(ToolStripItemEventType.DoubleClick);
            }
           
            EventHandler handler = (EventHandler)Events[EventButtonDoubleClick];
            if (handler != null) handler(this,e);
        }


        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnDefaultItemChanged"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnDefaultItemChanged(EventArgs e) {
            InvalidateSplitButtonLayout();
            if (CanRaiseEvents) {
                EventHandler eh = Events[EventDefaultItemChanged] as EventHandler;
                if (eh != null) {
                    eh(this, e);
                }
            }

        }        

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnMouseDown"]/*' />
        /// <devdoc>
        /// Summary of OnMouseDown.
        /// </devdoc>
        protected override void OnMouseDown(MouseEventArgs e) {
		    
            if (DropDownButtonBounds.Contains(e.Location)) {
                if (e.Button == MouseButtons.Left) {
                   
                    if (!DropDown.Visible) {
                        Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                        openMouseId = (ParentInternal == null) ? (byte)0: ParentInternal.GetMouseId();
                        this.ShowDropDown(/*mousePress = */true);
                    }
                }
            }
            else {
                SplitButtonButton.Push(true);
            }

        }
        
            
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnMouseUp"]/*' />
        /// <devdoc>
        /// Summary of OnMouseUp.
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs e) {
            if (!Enabled) {
                return;
            }
           

            SplitButtonButton.Push(false);
 
            if (DropDownButtonBounds.Contains(e.Location)) {
                if (e.Button == MouseButtons.Left) {
                    if (DropDown.Visible) {
                        Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                        byte closeMouseId = (ParentInternal == null) ? (byte)0: ParentInternal.GetMouseId();
                        if (closeMouseId != openMouseId) {
                            openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
                            ToolStripManager.ModalMenuFilter.CloseActiveDropDown(DropDown, ToolStripDropDownCloseReason.AppClicked);
                            Select();
                       }
                    }
                }
            }
            Point clickPoint = new Point(e.X, e.Y);
            if ((e.Button == MouseButtons.Left) && this.SplitButtonButton.Bounds.Contains(clickPoint)) {
                bool shouldFireDoubleClick = false;
                if (DoubleClickEnabled) {
                    long newTime = DateTime.Now.Ticks;
                    long deltaTicks = newTime - lastClickTime;
                    lastClickTime = newTime;
                    // use >= for cases where the succession of click events is so fast it's not picked up by
                    // DateTime resolution.
                    Debug.Assert(deltaTicks >= 0, "why are deltaticks less than zero? thats some mighty fast clicking");
                    // if we've seen a mouse up less than the double click time ago, we should fire.
                    if (deltaTicks >= 0 && deltaTicks < DoubleClickTicks) {
                        shouldFireDoubleClick = true;
                    }
                }
                if (shouldFireDoubleClick) {
                    OnButtonDoubleClick(new System.EventArgs());
                    // If we actually fired DoubleClick - reset the lastClickTime.
                    lastClickTime = 0;
                } 
                else {
                    OnButtonClick(new System.EventArgs());
                }           
            }

        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnMouseLeave"]/*' />
        protected override void OnMouseLeave(EventArgs e) {
            openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
            SplitButtonButton.Push(false);
            base.OnMouseLeave(e);
        }
      	
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnRightToLeftChanged"]/*' />
        /// <devdoc>
        /// Summary of OnRightToLeftChanged.
        /// </devdoc>
        protected override void OnRightToLeftChanged(EventArgs e) {
            base.OnRightToLeftChanged(e);
            InvalidateSplitButtonLayout();			
        }
        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.OnPaint"]/*' />
        /// <devdoc>
        /// Summary of OnPaint.
        /// </devdoc>
        /// <param name=e></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {

             ToolStripRenderer renderer = this.Renderer;
             if (renderer != null) {
                 InvalidateSplitButtonLayout();
                 Graphics g = e.Graphics;

                 renderer.DrawSplitButton(new ToolStripItemRenderEventArgs(g, this));

                 if ((DisplayStyle & ToolStripItemDisplayStyle.Image) != ToolStripItemDisplayStyle.None)  {   
                     renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, SplitButtonButtonLayout.ImageRectangle));             
                 }

                 if ((DisplayStyle & ToolStripItemDisplayStyle.Text) != ToolStripItemDisplayStyle.None) {                    
                      renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, SplitButtonButton.Text, SplitButtonButtonLayout.TextRectangle, this.ForeColor, this.Font, SplitButtonButtonLayout.TextFormat));
                 }
             }
        }  

        public void PerformButtonClick() {
            if (Enabled && Available) {
                PerformClick();
                OnButtonClick(EventArgs.Empty);
            }
        }

        /// <include file='doc\ToolStripComboButton.uex' path='docs/doc[@for="ToolStripSplitButton.ResetDropDownButtonWidth"]/*' />
        /// <devdoc>
        /// Resets the RightToLeft to be the default.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetDropDownButtonWidth() {
            DropDownButtonWidth = DefaultDropDownButtonWidth;
        }

        /// <devdoc>
        /// Summary of SetDropDownBounds.
        /// </devdoc>
        private void SetDropDownButtonBounds(Rectangle rect) {
            dropDownButtonBounds = rect; 
        }
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Size'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeDropDownButtonWidth() {
            return  (DropDownButtonWidth != DefaultDropDownButtonWidth);
        }
            
        /// <devdoc>
        ///  This class represents the item to the left of the dropdown [ A |v]  (e.g the "A")  
        ///  It exists so that we can use our existing methods for text and image layout
        ///  and have a place to stick certain state information like pushed and selected 
        ///  Note since this is NOT an actual item hosted on the Winbar - it wont get things
        ///  like MouseOver, wont be laid out by the ToolStrip, etc etc.  This is purely internal
        ///  convenience.
        /// </devdoc>
        private class ToolStripSplitButtonButton : ToolStripButton {

            private ToolStripSplitButton owner = null;

            public ToolStripSplitButtonButton(ToolStripSplitButton owner) {
                   this.owner = owner;
            }

            public override bool Enabled {
                get {
                    return owner.Enabled;
                }
                set {
                    // do nothing
                }
            }
            

            public override ToolStripItemDisplayStyle DisplayStyle {
                get {
                    return owner.DisplayStyle;
                }
                set {
                    // do nothing
                }
            }

            public override Padding Padding {
                get {
                    return this.owner.Padding;
                }
                set {
                    // do nothing
                }
            }

          
            public override ToolStripTextDirection TextDirection {
                get {
                    return owner.TextDirection;
                }
            }
            
            
            public override Image Image {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    if ((owner.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image) {
                        return owner.Image;
                    }
                    else {
                        return null;
                    }
                }
                set {
                    // do nothing
                }
            }

            public override bool Selected {
                get {
                    
                    if (owner != null) {
                        return owner.Selected;
                    }
                    return base.Selected;
                }
            }

            public override string Text {
                get {
                    if ((owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) {
                        return owner.Text;
                    }
                    else {
                        return null;
                    }
                }
                set {
                    // do nothing
                }
            }

        }

        /// <devdoc>
        ///  This class performs internal layout for the "split button button" portion of a split button.
        ///  Its main job is to make sure the inner button has the same parent as the split button, so
        ///  that layout can be performed using the correct graphics context.
        /// </devdoc>
        private class ToolStripSplitButtonButtonLayout : ToolStripItemInternalLayout {

            ToolStripSplitButton owner;

            public ToolStripSplitButtonButtonLayout(ToolStripSplitButton owner) : base(owner.SplitButtonButton) {
                this.owner = owner;
            }

            protected override ToolStripItem Owner {
                get { return owner; }
            }

            protected override ToolStrip ParentInternal {
                get {
                    return owner.ParentInternal;
                }
            }
            public override Rectangle ImageRectangle {
                get {
                    Rectangle imageRect = base.ImageRectangle;
                    // translate to ToolStripItem coordinates
                    imageRect.Offset(owner.SplitButtonButton.Bounds.Location);
                    return imageRect;     
                }
            }
            
            public override Rectangle TextRectangle {
                get {
                    Rectangle textRect = base.TextRectangle;
                    // translate to ToolStripItem coordinates
                    textRect.Offset(owner.SplitButtonButton.Bounds.Location);
                    return textRect;     
                }
            }
        }

        /// <include file='doc\ToolStripDropDownItem.uex' path='docs/doc[@for="ToolStripDropDownItemAccessibleObject"]/*' />        
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public class ToolStripSplitButtonAccessibleObject : ToolStripItem.ToolStripItemAccessibleObject {
            private ToolStripSplitButton owner;

            public ToolStripSplitButtonAccessibleObject(ToolStripSplitButton item) : base(item) {
                owner = item;
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                owner.PerformButtonClick();
            }
        }

        /// <include file='doc\ToolStripDropDownItem.uex' path='docs/doc[@for="ToolStripSplitButtonExAccessibleObject"]/*' /> 
        internal class ToolStripSplitButtonExAccessibleObject: ToolStripSplitButtonAccessibleObject {

            private ToolStripSplitButton ownerItem;

            public ToolStripSplitButtonExAccessibleObject(ToolStripSplitButton item)
                : base(item) {
                ownerItem = item;
            }

            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId) {
                    return NativeMethods.UIA_ButtonControlTypeId;
                }
                else {
                    return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsIAccessibleExSupported() {
                if (ownerItem != null) {
                    return true;
                }
                else {
                    return base.IsIAccessibleExSupported();
                }
            }

            internal override bool IsPatternSupported(int patternId) {
                if (patternId == NativeMethods.UIA_ExpandCollapsePatternId && ownerItem.HasDropDownItems) {
                    return true;
                }
                else {
                    return base.IsPatternSupported(patternId);
                }
            }

            internal override void Expand() {
                DoDefaultAction();
            }

            internal override void Collapse() {
                if (ownerItem != null && ownerItem.DropDown != null && ownerItem.DropDown.Visible) {
                    ownerItem.DropDown.Close();
                }
            }

            internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState {
                get {
                    return ownerItem.DropDown.Visible ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                switch (direction) {
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        return ownerItem.DropDownItems.Count > 0 ? ownerItem.DropDown.Items[0].AccessibilityObject : null;
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        return ownerItem.DropDownItems.Count > 0 ? ownerItem.DropDown.Items[ownerItem.DropDown.Items.Count - 1].AccessibilityObject : null;
                }
                return base.FragmentNavigate(direction);
            }
        }
    }
}
    


