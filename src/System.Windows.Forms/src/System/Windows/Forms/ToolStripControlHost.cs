// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Runtime.Versioning;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;
    
    /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost"]/*' />
    /// <devdoc>
    /// ToolStripItem that can host Controls.
    /// </devdoc>
    public class ToolStripControlHost : ToolStripItem {
 
        private Control control;
        private int suspendSyncSizeCount = 0;
        private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;
        private bool inSetVisibleCore = false;
        
        internal static readonly object EventGotFocus                        = new object();
        internal static readonly object EventLostFocus                       = new object();
        internal static readonly object EventKeyDown                         = new object();
        internal static readonly object EventKeyPress                        = new object();
        internal static readonly object EventKeyUp                           = new object();
        internal static readonly object EventEnter                           = new object();
        internal static readonly object EventLeave                           = new object();
        internal static readonly object EventValidated                       = new object();
        internal static readonly object EventValidating                      = new object();


        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.ToolStripControlHost"]/*' />
        /// <devdoc>
        /// Constructs a ToolStripControlHost
        /// </devdoc>
        
        public ToolStripControlHost(Control c) {
            if (c == null) {
                throw new ArgumentNullException(nameof(c), SR.ControlCannotBeNull);
            }
            control = c;
            SyncControlParent();
            c.Visible = true;
            SetBounds(c.Bounds);
            
            // now that we have a control set in, update the bounds.
            Rectangle bounds = this.Bounds;
            CommonProperties.UpdateSpecifiedBounds(c, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                c.ToolStripControlHost = this;
            }

            OnSubscribeControlEvents(c);
        }
        public ToolStripControlHost(Control c, string name) : this (c){
            this.Name = name;
        }
     
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.BackColor"]/*' />
        public override Color BackColor {
            get {
                return Control.BackColor;
            }
            set {
                Control.BackColor = value;
            }
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Image"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets the image that is displayed on a <see cref='System.Windows.Forms.Label'/>.
        /// </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageDescr)),
        DefaultValue(null)
        ]
        public override Image BackgroundImage {
            get {
                return Control.BackgroundImage;
            }
            set {
                Control.BackgroundImage = value;
            }
        }
 
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.BackgroundImageLayout"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ImageLayout.Tile),
        Localizable(true),
        SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))
        ]
        public override ImageLayout BackgroundImageLayout {
          get {
              return Control.BackgroundImageLayout;
          }
          set {
              Control.BackgroundImageLayout = value;
          }
        }
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.CanSelect"]/*' />
        /// <devdoc>
        /// Overriden to return value from Control.CanSelect.
        /// </devdoc>
        public override bool CanSelect {
            get { 
                if (control != null) {
                    return (DesignMode || this.Control.CanSelect); 
                }   
                return false;
            }
        }

        [
        SRCategory(nameof(SR.CatFocus)),
        DefaultValue(true),
        SRDescription(nameof(SR.ControlCausesValidationDescr))
        ]
        public bool CausesValidation {
            get { return Control.CausesValidation; }
            set { Control.CausesValidation = value; }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.ControlAlign"]/*' />
        [DefaultValue(ContentAlignment.MiddleCenter), Browsable(false)]
        public ContentAlignment ControlAlign {
            get { return controlAlign; }
            set { 
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }
                if (controlAlign != value) {
                    controlAlign = value;
                    OnBoundsChanged();
                }
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Control"]/*' />
        /// <devdoc>
        /// The control that this item is hosting.
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control Control {
            get {  
                return control;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.DefaultSize"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                if (Control != null) {
                    // When you create the control - it sets up its size as its default size.
                    // Since the property is protected we dont know for sure, but this is a pretty good guess.
                    return Control.Size;
                }
                return base.DefaultSize;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.DisplayStyle"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripItemDisplayStyle DisplayStyle {
            get { 
                return base.DisplayStyle; 
            }
            set {
               base.DisplayStyle = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DisplayStyleChanged {
            add { 
                Events.AddHandler(EventDisplayStyleChanged, value); 
            }
            remove {
                Events.RemoveHandler(EventDisplayStyleChanged, value);
            }
        }

        /// <devdoc> 
        //  For control hosts, this property has no effect
        /// as they get their own clicks.  Use ControlStyles.StandardClick
        /// instead. 
        /// </devdoc>
        [DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool DoubleClickEnabled {
            get {
                return base.DoubleClickEnabled;
            }
            set {
                base.DoubleClickEnabled = value;
            }
        }
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Font"]/*' />
        public override Font Font {
            get {
                return Control.Font;
            }
            set {
                Control.Font = value;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Enabled"]/*' />
        public override bool Enabled {
            get {
                return Control.Enabled;
            }
            set {
                Control.Enabled = value;
            }
        }


        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.GotFocus"]/*' />
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnEnterDescr))]
        public event EventHandler Enter {
            add {
                Events.AddHandler(EventEnter, value);
            }
            remove {
                Events.RemoveHandler(EventEnter, value);
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Focused"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public virtual bool Focused {
            get { 
                return Control.Focused;
           }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.ForeColor"]/*' />
        public override Color ForeColor {
            get {
                return Control.ForeColor;
            }
            set {
                Control.ForeColor = value;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.GotFocus"]/*' />
        [
        SRCategory(nameof(SR.CatFocus)), 
        SRDescription(nameof(SR.ToolStripItemOnGotFocusDescr)), 
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public event EventHandler GotFocus {
            add {
                Events.AddHandler(EventGotFocus, value);
            }
            remove {
                Events.RemoveHandler(EventGotFocus, value);
            }
        }

        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Image Image {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                return base.Image;
            }
            set {
                base.Image = value;
            }
        }
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ToolStripItemImageScaling ImageScaling {
            get {
                return base.ImageScaling;
            }
            set {
                base.ImageScaling = value;
            }
        }
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Color ImageTransparentColor {
            get {
                return base.ImageTransparentColor;
            }
            set {
                base.ImageTransparentColor = value;
            }
        }

        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ContentAlignment ImageAlign {
            get {
                return base.ImageAlign;
            }
            set {
                base.ImageAlign = value;

            }
        }

         [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnLeaveDescr))]
         public event EventHandler Leave {
             add {
                 Events.AddHandler(EventLeave, value);
             }
             remove {
                 Events.RemoveHandler(EventLeave, value);
             }
         }

         /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.LostFocus"]/*' />
         /// <devdoc>
         /// <para>Occurs when the control loses focus.</para>
         /// </devdoc>
         [
         SRCategory(nameof(SR.CatFocus)), 
         SRDescription(nameof(SR.ToolStripItemOnLostFocusDescr)), 
         Browsable(false), 
         EditorBrowsable(EditorBrowsableState.Advanced)
         ]
         public event EventHandler LostFocus {
             add {
                 Events.AddHandler(EventLostFocus, value);
             }
             remove {
                 Events.RemoveHandler(EventLostFocus, value);
             }
         }


         /// <include file='doc\WinBarControlHost.uex' path='docs/doc[@for="ToolStripControlHost.KeyDown"]/*' />
         /// <devdoc>
         /// <para>Occurs when a key is pressed down while the control has focus.</para>
         /// </devdoc>
         [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyDownDescr))]
         public event KeyEventHandler KeyDown {
             add {
                 Events.AddHandler(EventKeyDown, value);
             }
             remove {
                 Events.RemoveHandler(EventKeyDown, value);
             }
         }
      
         /// <include file='doc\WinBarControlHost.uex' path='docs/doc[@for="ToolStripControlHost.KeyPress"]/*' />
         /// <devdoc>
         /// <para> Occurs when a key is pressed while the control has focus.</para>
         /// </devdoc>
         [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyPressDescr))]
         public event KeyPressEventHandler KeyPress {
             add {
                 Events.AddHandler(EventKeyPress, value);
             }
             remove {
                 Events.RemoveHandler(EventKeyPress, value);
             }
         }
      
         /// <include file='doc\WinBarControlHost.uex' path='docs/doc[@for="ToolStripControlHost.KeyUp"]/*' />
         /// <devdoc>
         /// <para> Occurs when a key is released while the control has focus.</para>
         /// </devdoc>
         [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyUpDescr))]
         public event KeyEventHandler KeyUp {
             add {
                 Events.AddHandler(EventKeyUp, value);
             }
             remove {
                 Events.RemoveHandler(EventKeyUp, value);
             }
         }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.RightToLeft"]/*' />
        /// <devdoc>
        /// This is used for international applications where the language
        /// is written from RightToLeft. When this property is true,
        /// control placement and text will be from right to left.
        /// </devdoc>
        public override RightToLeft RightToLeft {
            get { 
                if (control != null) {
                    return control.RightToLeft;
                }
                return base.RightToLeft;
            }
            set {
                if (control != null) {
                    control.RightToLeft = value;
                }
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool RightToLeftAutoMirrorImage {
            get {
                return base.RightToLeftAutoMirrorImage;
            }
            set {
                base.RightToLeftAutoMirrorImage = value;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Selected"]/*' />
        public override bool Selected {
            get { 
                if (Control != null)
                {
                    return Control.Focused;
                }
                return false;
            }
        }

        public override Size Size {
            get {
                return base.Size;
            }
            set {
                Rectangle specifiedBounds = Rectangle.Empty;
                if (control != null) {
                    // we dont normally update the specified bounds, but if someone explicitly sets
                    // the size we should.
                    specifiedBounds = control.Bounds;
                    specifiedBounds.Size = value;
                    CommonProperties.UpdateSpecifiedBounds(control, specifiedBounds.X, specifiedBounds.Y, specifiedBounds.Width, specifiedBounds.Height);
                }
        

                base.Size = value;

                if (control != null) {
                    // checking again in case the control has adjusted the size.
                    Rectangle bounds = control.Bounds;
                    if (bounds != specifiedBounds) {
                        CommonProperties.UpdateSpecifiedBounds(control, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    }
                }
            }
        }

        /// <devdoc>
        /// Overriden to set the Site for the control hosted. This is set at DesignTime when the component is added to the Container. 
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override ISite Site {
            get {
                return base.Site;
            }
            set {
                base.Site = value;
                if (value != null)
                {
                    Control.Site = new StubSite(Control, this);
                }
                else
                {
                    Control.Site = null;
                }
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Text"]/*' />
        /// <devdoc>
        /// Overriden to modify hosted control's text.
        /// </devdoc>
        [DefaultValue("")]
        public override string Text {
            get {
                return Control.Text;
            }
            set {
                Control.Text = value;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.TextAlign"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new ContentAlignment TextAlign {
             get {
                 return base.TextAlign;
             }
             set {
                 base.TextAlign = value;
             }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(ToolStripTextDirection.Horizontal)]
        public override ToolStripTextDirection TextDirection {
             get {
                 return base.TextDirection;
             }
             set {
                 base.TextDirection = value;
             }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.TextImageRelation"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new TextImageRelation TextImageRelation {
           get {
               return base.TextImageRelation;
           }
           set {
               base.TextImageRelation = value;      
           }
        }

        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnValidatingDescr))]
        public event CancelEventHandler Validating {
            add {
                Events.AddHandler(EventValidating, value);
            }
            remove {
                Events.RemoveHandler(EventValidating, value);
            }
        }


        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnValidatedDescr))]
        public event EventHandler Validated {
            add {
                Events.AddHandler(EventValidated, value);
            }
            remove {
                Events.RemoveHandler(EventValidated, value);
            }
        }

                /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance() {
            return Control.AccessibilityObject;
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Dispose"]/*' />
        /// <devdoc>
        /// Cleans up and destroys the hosted control.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            // Call base first so other things stop trying to talk to the control.  This will
            // unparent the host item which will cause a SyncControlParent, so the control
            // will be correctly unparented before being disposed.
            base.Dispose (disposing);
           
            if (disposing && Control != null) {

                OnUnsubscribeControlEvents(Control);
                
                // we only call control.Dispose if we are NOT being disposed in the finalizer.
                Control.Dispose();
                control = null;
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.Focus"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Focus() 
        {
           Control.Focus();
        }

    
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.GetPreferredSize"]/*' />
        public override Size GetPreferredSize(Size constrainingSize) {
            if (control != null) {
                return Control.GetPreferredSize(constrainingSize - Padding.Size) + Padding.Size;
            }
            return base.GetPreferredSize(constrainingSize);
        }
        
        ///
        ///  Handle* wrappers:
        ///    We sync the event from the hosted control and call resurface it on ToolStripItem.
        ///

        private void HandleClick(object sender, System.EventArgs e) {
            OnClick(e);
        }   
        private void HandleBackColorChanged(object sender, System.EventArgs e) {
            OnBackColorChanged(e);
        }   
        private void HandleDoubleClick(object sender, System.EventArgs e) {
            OnDoubleClick(e);
        }   
        private void HandleDragDrop(object sender, DragEventArgs e) {
            OnDragDrop(e);
        }
        private void HandleDragEnter(object sender, DragEventArgs e) {
            OnDragEnter(e);
        }
        private void HandleDragLeave(object sender, EventArgs e) {
            OnDragLeave(e);
        }
        private void HandleDragOver(object sender, DragEventArgs e) {
            OnDragOver(e);
        }
        private void HandleEnter(object sender, System.EventArgs e) {
            OnEnter(e);
        }   
        private void HandleEnabledChanged(object sender, System.EventArgs e) {
            OnEnabledChanged(e);
        }
        private void HandleForeColorChanged(object sender, System.EventArgs e) {
            OnForeColorChanged(e);
        }   
        private void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) {
            OnGiveFeedback(e);
        }
        private void HandleGotFocus(object sender, EventArgs e) {
            OnGotFocus(e);
        }
        private void HandleLocationChanged(object sender, EventArgs e) {
            OnLocationChanged(e);            
        }
        private void HandleLostFocus(object sender, EventArgs e) {
            OnLostFocus(e);            
        }
        private void HandleKeyDown(object sender, KeyEventArgs e) {
            OnKeyDown(e);
        }
        private void HandleKeyPress(object sender, KeyPressEventArgs e) {
            OnKeyPress(e);
        }
        private void HandleKeyUp(object sender, KeyEventArgs e) {
            OnKeyUp(e);
        }
        private void HandleLeave(object sender, System.EventArgs e) {
            OnLeave(e);
        }   
        private void HandleMouseDown(object sender, MouseEventArgs e) {
            OnMouseDown(e);
            RaiseMouseEvent(ToolStripItem.EventMouseDown, e);
        }
        private void HandleMouseEnter(object sender, EventArgs e) {
            OnMouseEnter(e);   
            RaiseEvent(ToolStripItem.EventMouseEnter, e);
        }
        private void HandleMouseLeave(object sender, EventArgs e) {
            OnMouseLeave(e);   
            RaiseEvent(ToolStripItem.EventMouseLeave, e);
        }
        private void HandleMouseHover(object sender, EventArgs e) {
            OnMouseHover(e);   
            RaiseEvent(ToolStripItem.EventMouseHover, e);
        }
        private void HandleMouseMove(object sender, MouseEventArgs e) {
            OnMouseMove(e);
            RaiseMouseEvent(ToolStripItem.EventMouseMove, e);
        }
        private void HandleMouseUp(object sender, MouseEventArgs e) {
            OnMouseUp(e);
            RaiseMouseEvent(ToolStripItem.EventMouseUp, e);
        }
        private void HandlePaint(object sender, PaintEventArgs e) {
            OnPaint(e);
            RaisePaintEvent(ToolStripItem.EventPaint, e);
        }
        private void HandleQueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e) {
            QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Events[ToolStripItem.EventQueryAccessibilityHelp];
            if (handler != null) handler(this, e);
        }
        private void HandleQueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
            OnQueryContinueDrag(e);
        }
        private void HandleRightToLeftChanged(object sender, EventArgs e) {
            OnRightToLeftChanged(e);
        }
        private void HandleResize(object sender, EventArgs e) {
            if (suspendSyncSizeCount == 0) {
               OnHostedControlResize(e);
            }
        }
    
        private void HandleTextChanged(object sender, EventArgs e) {
            OnTextChanged(e);
        }
        private void HandleControlVisibleChanged(object sender, EventArgs e) {
            // check the STATE_VISIBLE flag rather than using Control.Visible.
            // if we check while it's unparented it will return visible false.
            // the easiest way to do this is to use ParticipatesInLayout.
            bool controlVisibleStateFlag = ((IArrangedElement)Control).ParticipatesInLayout;
            bool itemVisibleStateFlag = ((IArrangedElement)(this)).ParticipatesInLayout;
            
            if (itemVisibleStateFlag != controlVisibleStateFlag) {
                this.Visible = Control.Visible;
                // this should fire the OnVisibleChanged and raise events appropriately.
            };
        }

        private void HandleValidating(object sender, CancelEventArgs e) {
            OnValidating(e);
        }   
        
        private void HandleValidated(object sender, System.EventArgs e) {
            OnValidated(e);
        }   

        internal override void OnAccessibleDescriptionChanged(EventArgs e) {
            Control.AccessibleDescription = AccessibleDescription;
        }
        internal override void OnAccessibleNameChanged(EventArgs e) {
            Control.AccessibleName = AccessibleName;
        }
        internal override void OnAccessibleDefaultActionDescriptionChanged(EventArgs e) {
            Control.AccessibleDefaultActionDescription = AccessibleDefaultActionDescription;
        }
        internal override void OnAccessibleRoleChanged(EventArgs e) {
            Control.AccessibleRole = AccessibleRole;
        }

        
        protected virtual void OnEnter(EventArgs e) {
            RaiseEvent(EventEnter, e);
        }
        
        
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnGotFocus"]/*' />
        /// <devdoc>
        /// called when the control has lost focus
        /// </devdoc>
        protected virtual void OnGotFocus(EventArgs e) {
            RaiseEvent(EventGotFocus, e);            
        }

        protected virtual void OnLeave(EventArgs e) {
            RaiseEvent(EventLeave, e);
        }
        
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnLostFocus"]/*' />
        /// <devdoc>
        /// called when the control has lost focus
        /// </devdoc >
        protected virtual void OnLostFocus(EventArgs e) {         
            RaiseEvent(EventLostFocus, e);                              
        } 
        /// <include file='doc\WinBarControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnKeyDown"]/*' />
        protected virtual void OnKeyDown(KeyEventArgs e) {         
            RaiseKeyEvent(EventKeyDown, e);                              
        } 
        /// <include file='doc\WinBarControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnKeyPress"]/*' />
        protected virtual void OnKeyPress(KeyPressEventArgs e) {         
            RaiseKeyPressEvent(EventKeyPress, e);                              
        } 
        /// <include file='doc\WinBarControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnKeyUp"]/*' />
        protected virtual void OnKeyUp(KeyEventArgs e) {         
            RaiseKeyEvent(EventKeyUp, e);                              
        } 
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnBoundsChanged"]/*' />
        /// <devdoc>
        /// Called when the items bounds are changed.  Here, we update the Control's bounds.
        /// </devdoc>
        protected override void OnBoundsChanged() {
            if (control != null) {
                SuspendSizeSync();
                IArrangedElement element = control as IArrangedElement;
                if (element == null) {
                    Debug.Fail("why are we here? control should not be null");
                    return;
                }
                
                Size size = LayoutUtils.DeflateRect(this.Bounds, this.Padding).Size;
                Rectangle bounds = LayoutUtils.Align(size, this.Bounds, ControlAlign);
                
                // use BoundsSpecified.None so we dont deal w/specified bounds - this way we can tell what someone has set the size to.
                element.SetBounds(bounds, BoundsSpecified.None);

                // sometimes a control can ignore the size passed in, use the adjustment 
                // to re-align.
                if (bounds != control.Bounds) {                
                    bounds = LayoutUtils.Align(control.Size, this.Bounds, ControlAlign);
                    element.SetBounds(bounds,  BoundsSpecified.None);
                }
                ResumeSizeSync();
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnPaint"]/*' />
        /// <devdoc>
        /// Called when the control fires its Paint event.
        /// </devdoc>
        protected override void OnPaint(PaintEventArgs e) {
            // do nothing....
        }

        
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnLayout"]/*' />
        protected internal override void OnLayout(LayoutEventArgs e) {    
            // do nothing... called via the controls collection
        }
    
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnParentChanged"]/*' />
        /// <devdoc>
        /// Called when the item's parent has been changed.
        /// </devdoc>
        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent) {
            if (oldParent != null && Owner == null  && newParent == null && Control != null) {
                // if we've really been removed from the item collection,
                // politely remove ourselves from the control collection
                WindowsFormsUtils.ReadOnlyControlCollection  oldControlCollection 
                                = GetControlCollection(Control.ParentInternal as ToolStrip);
                if (oldControlCollection != null) {
                     oldControlCollection.RemoveInternal(Control);
                }
            }
            else {
               SyncControlParent();
            }
            
            base.OnParentChanged(oldParent, newParent);
        }




        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnSubscribeControlEvents"]/*' />
        /// <devdoc>
        /// The events from the hosted control are subscribed here.  
        /// Override to add/prevent syncing of control events.
        /// NOTE: if you override and hook up events here, you should unhook in OnUnsubscribeControlEvents.
        /// </devdoc>
        protected virtual void OnSubscribeControlEvents(Control control) {

            if (control != null) {
                // Please keep this alphabetized and in sync with Unsubscribe
                // 
                control.Click                       += new EventHandler(HandleClick);
                control.BackColorChanged            += new EventHandler(HandleBackColorChanged);
                control.DoubleClick                 += new EventHandler(HandleDoubleClick);
                control.DragDrop                    += new DragEventHandler(HandleDragDrop);
                control.DragEnter                   += new DragEventHandler(HandleDragEnter);
                control.DragLeave                   += new EventHandler(HandleDragLeave);
                control.DragOver                    += new DragEventHandler(HandleDragOver);
                control.Enter                       += new EventHandler(HandleEnter);
                control.EnabledChanged              += new EventHandler(HandleEnabledChanged);
                control.ForeColorChanged            += new EventHandler(HandleForeColorChanged);
                control.GiveFeedback                += new GiveFeedbackEventHandler(HandleGiveFeedback);
                control.GotFocus                    += new EventHandler(HandleGotFocus);
                control.Leave                       += new EventHandler(HandleLeave);
                control.LocationChanged             += new EventHandler(HandleLocationChanged);
                control.LostFocus                   += new EventHandler(HandleLostFocus);
                control.KeyDown                     += new KeyEventHandler(HandleKeyDown);
                control.KeyPress                    += new KeyPressEventHandler(HandleKeyPress);
                control.KeyUp                       += new KeyEventHandler(HandleKeyUp);
                control.MouseDown                   += new MouseEventHandler(HandleMouseDown);
                control.MouseEnter                  += new EventHandler(HandleMouseEnter);
                control.MouseHover                  += new EventHandler(HandleMouseHover);
                control.MouseLeave                  += new EventHandler(HandleMouseLeave);
                control.MouseMove                   += new MouseEventHandler(HandleMouseMove);
                control.MouseUp                     += new MouseEventHandler(HandleMouseUp);
                control.Paint                       += new PaintEventHandler(HandlePaint);
                control.QueryAccessibilityHelp      += new QueryAccessibilityHelpEventHandler(HandleQueryAccessibilityHelp);
                control.QueryContinueDrag           += new QueryContinueDragEventHandler(HandleQueryContinueDrag);
                control.Resize                      += new EventHandler(HandleResize);
                control.RightToLeftChanged          += new EventHandler(HandleRightToLeftChanged);
                control.TextChanged                 += new EventHandler(HandleTextChanged);
                control.VisibleChanged              += new EventHandler(HandleControlVisibleChanged);
                control.Validating                  += new CancelEventHandler(HandleValidating);
                control.Validated                   += new EventHandler(HandleValidated);
                
            }
    
        }
      

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnUnsubscribeControlEvents"]/*' />
        /// <devdoc>
        /// The events from the hosted control are unsubscribed here.  
        /// Override to unhook events subscribed in OnSubscribeControlEvents.
        /// </devdoc>
        protected virtual void OnUnsubscribeControlEvents(Control control) {
             if (control != null) {
                // Please keep this alphabetized and in sync with Subscribe
                // 
                control.Click                       -= new EventHandler(HandleClick);
                control.BackColorChanged            -= new EventHandler(HandleBackColorChanged);
                control.DoubleClick                 -= new EventHandler(HandleDoubleClick);
                control.DragDrop                    -= new DragEventHandler(HandleDragDrop);
                control.DragEnter                   -= new DragEventHandler(HandleDragEnter);
                control.DragLeave                   -= new EventHandler(HandleDragLeave);
                control.DragOver                    -= new DragEventHandler(HandleDragOver);
                control.Enter                       -= new EventHandler(HandleEnter);
                control.EnabledChanged              -= new EventHandler(HandleEnabledChanged);
                control.ForeColorChanged            -= new EventHandler(HandleForeColorChanged);
                control.GiveFeedback                -= new GiveFeedbackEventHandler(HandleGiveFeedback);
                control.GotFocus                    -= new EventHandler(HandleGotFocus);
                control.Leave                       -= new EventHandler(HandleLeave);
                control.LocationChanged             -= new EventHandler(HandleLocationChanged);
                control.LostFocus                   -= new EventHandler(HandleLostFocus);
                control.KeyDown                     -= new KeyEventHandler(HandleKeyDown);
                control.KeyPress                    -= new KeyPressEventHandler(HandleKeyPress);
                control.KeyUp                       -= new KeyEventHandler(HandleKeyUp);
                control.MouseDown                   -= new MouseEventHandler(HandleMouseDown);
                control.MouseEnter                  -= new EventHandler(HandleMouseEnter);
                control.MouseHover                  -= new EventHandler(HandleMouseHover);
                control.MouseLeave                  -= new EventHandler(HandleMouseLeave);
                control.MouseMove                   -= new MouseEventHandler(HandleMouseMove);
                control.MouseUp                     -= new MouseEventHandler(HandleMouseUp);
                control.Paint                       -= new PaintEventHandler(HandlePaint);
                control.QueryAccessibilityHelp      -= new QueryAccessibilityHelpEventHandler(HandleQueryAccessibilityHelp);
                control.QueryContinueDrag           -= new QueryContinueDragEventHandler(HandleQueryContinueDrag);
                control.Resize                      -= new EventHandler(HandleResize);
                control.RightToLeftChanged          -= new EventHandler(HandleRightToLeftChanged);
                control.TextChanged                 -= new EventHandler(HandleTextChanged);
                control.VisibleChanged              -= new EventHandler(HandleControlVisibleChanged);
                control.Validating                  -= new CancelEventHandler(HandleValidating);
                control.Validated                   -= new EventHandler(HandleValidated);
        
            }
    
        }

        protected virtual void OnValidating(CancelEventArgs e) {
            RaiseCancelEvent(EventValidating, e);
        }


        protected virtual void OnValidated(EventArgs e) {
            RaiseEvent(EventValidated, e);
        }

        private static WindowsFormsUtils.ReadOnlyControlCollection GetControlCollection(ToolStrip toolStrip) {
             WindowsFormsUtils.ReadOnlyControlCollection newControls =
                    toolStrip != null ? (WindowsFormsUtils.ReadOnlyControlCollection) toolStrip.Controls : null;
             return newControls;
        }

        // Ensures the hosted Control is parented to the ToolStrip hosting this ToolStripItem.
        private void SyncControlParent() {
            WindowsFormsUtils.ReadOnlyControlCollection  newControls = GetControlCollection(ParentInternal);
            if (newControls != null) {
                newControls.AddInternal(Control);
            }
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.OnHostedControlResize"]/*' />
        protected virtual void OnHostedControlResize(EventArgs e) {
            // support for syncing the wrapper when the control size has changed
            this.Size = Control.Size;
        }
    
        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.ProcessCmdKey"]/*' />
        // 


        [SuppressMessage("Microsoft.Security", "CA2114:MethodSecurityShouldBeASupersetOfType")]
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected internal override bool ProcessCmdKey(ref Message m, Keys keyData) {
            // Control will get this from being in the control collection.
            return false;
        }

        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'charCode' matches control.cs
        protected internal override bool ProcessMnemonic(char charCode) {
            if (control != null) {
                return control.ProcessMnemonic( charCode );
            }
            return base.ProcessMnemonic( charCode );
        }

        /// <include file='doc\ToolStripControlHost.uex' path='docs/doc[@for="ToolStripControlHost.ProcessDialogKey"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2114:MethodSecurityShouldBeASupersetOfType")]
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessDialogKey(Keys keyData) {
            // Control will get this from being in the control collection.
            return false;
        }

        protected override void SetVisibleCore(bool visible) {
            // This is needed, because if you try and set set visible to true before the parent is visible,
            // we will get called back into here, and set it back to false, since the parent is not visible.
            if (inSetVisibleCore) {
                return;
            }

            inSetVisibleCore = true;
            Control.SuspendLayout();
            try {
                Control.Visible = visible;
            }
            finally {
                Control.ResumeLayout(false);
                // this will go ahead and perform layout.
                base.SetVisibleCore(visible);
                inSetVisibleCore = false;
            }

            
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetBackColor() {
            Control.ResetBackColor();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetForeColor() {
            Control.ResetForeColor();
        }

        private void SuspendSizeSync() {
            this.suspendSyncSizeCount++;
        }
        
        private void ResumeSizeSync() {
            this.suspendSyncSizeCount--;
        }
        internal override bool ShouldSerializeBackColor() {
            if (control != null) {
                return control.ShouldSerializeBackColor();
            }
            return base.ShouldSerializeBackColor();   
        }
        internal override bool ShouldSerializeForeColor() {
            if (control != null) {
                return control.ShouldSerializeForeColor();
            }        
            return base.ShouldSerializeForeColor();
        }

        internal override bool ShouldSerializeFont() {
            if (control != null) {
                return control.ShouldSerializeFont();
            }
            return base.ShouldSerializeFont();
        }

        internal override bool ShouldSerializeRightToLeft() {
            if (control != null) {
                return control.ShouldSerializeRightToLeft();
            }
            return base.ShouldSerializeRightToLeft();   
        }

        internal override void OnKeyboardToolTipHook(ToolTip toolTip) {
            base.OnKeyboardToolTipHook(toolTip);

            KeyboardToolTipStateMachine.Instance.Hook(this.Control, toolTip);
        }

        internal override void OnKeyboardToolTipUnhook(ToolTip toolTip) {
            base.OnKeyboardToolTipUnhook(toolTip);

            KeyboardToolTipStateMachine.Instance.Unhook(this.Control, toolTip);
        }

        // Our implementation of ISite:
        // Since the Control which is wrapped by ToolStripControlHost is a runtime instance, there is no way of knowing 
        // whether the control is in runtime or designtime.
        // This implementation of ISite would be set to Control.Site when ToolStripControlHost.Site is set at DesignTime. (Refer to Site property on ToolStripControlHost)
        // This implementation just returns the DesigMode property to be ToolStripControlHost's DesignMode property. 
        // Everything else is pretty much default implementation.
        private class StubSite : ISite, IDictionaryService
        {
            private Hashtable _dictionary = null;
            IComponent comp = null;
            IComponent owner = null;
            
            public StubSite(Component control, Component host)
            {
                comp = control as IComponent;
                owner = host as IComponent;
            }
            // The component sited by this component site.
            /// <devdoc>
            ///    <para>When implemented by a class, gets the component associated with the <see cref='System.ComponentModel.ISite'/>.</para>
            /// </devdoc>
            IComponent ISite.Component {
                get
                {
                    return comp;
                }
            }
        
            // The container in which the component is sited.
            /// <devdoc>
            /// <para>When implemented by a class, gets the container associated with the <see cref='System.ComponentModel.ISite'/>.</para>
            /// </devdoc>
            IContainer ISite.Container {
                get {
                    return owner.Site.Container;
                }
            }
        
            // Indicates whether the component is in design mode.
            /// <devdoc>
            ///    <para>When implemented by a class, determines whether the component is in design mode.</para>
            /// </devdoc>
            bool ISite.DesignMode {
                get {
                    return owner.Site.DesignMode;
                }
            }

            // The name of the component.
            //
            /// <devdoc>
            ///    <para>When implemented by a class, gets or sets the name of
            ///       the component associated with the <see cref='System.ComponentModel.ISite'/>.</para>
            /// </devdoc>
            String ISite.Name {
                get {
                    return owner.Site.Name;
                }
                set {
                    owner.Site.Name = value;
                }
            }

            /// <summary>
            ///     Returns the requested service.
            /// </summary>
            object IServiceProvider.GetService(Type service) {
                if (service == null) {
                    throw new ArgumentNullException(nameof(service));
                }

                // We have to implement our own dictionary service. If we don't,
                // the properties of the underlying component will end up being
                // overwritten by our own properties when GetProperties is called
                if (service == typeof(IDictionaryService)) {
                    return this;
                }

                if (owner.Site != null) {
                    return owner.Site.GetService(service);
                }
                return null;
            }

            /// <devdoc>
            ///     Retrieves the key corresponding to the given value.
            /// </devdoc>
            object IDictionaryService.GetKey(object value) {
                if (_dictionary != null) {
                    foreach (DictionaryEntry de in _dictionary) {
                        object o = de.Value;
                        if (value != null && value.Equals(o)) {
                            return de.Key;
                        }
                    }
                }
                return null;
            }

            /// <devdoc>
            ///     Retrieves the value corresponding to the given key.
            /// </devdoc>
            object IDictionaryService.GetValue(object key) {
                if (_dictionary != null) {
                    return _dictionary[key];
                }
                return null;
            }

            /// <devdoc>
            ///     Stores the given key-value pair in an object's site.  This key-value
            ///     pair is stored on a per-object basis, and is a handy place to save
            ///     additional information about a component.
            /// </devdoc>
            void IDictionaryService.SetValue(object key, object value) {
                if (_dictionary == null) {
                    _dictionary = new Hashtable();
                }
                if (value == null) {
                    _dictionary.Remove(key);
                } else {
                    _dictionary[key] = value;
                }
            }
        }
    }
}
