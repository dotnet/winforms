// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using System.Windows.Forms.Layout;
    using System.Diagnostics;
    
    /// <summary>
    /// 

    internal class ToolStripGrip : ToolStripButton {

        private Cursor oldCursor;
        private int gripThickness = 0;
        Point startLocation = Point.Empty;
        private bool movingToolStrip = false;
        private Point lastEndLocation = ToolStrip.InvalidMouseEnter;
        private static Size DragSize = LayoutUtils.MaxSize;

        private static readonly Padding defaultPadding = new Padding(2);
        private static readonly int gripThicknessDefault = 3;
        private static readonly int gripThicknessVisualStylesEnabled = 5;
        private Padding scaledDefaultPadding = defaultPadding;
        private int scaledGripThickness = gripThicknessDefault;
        private int scaledGripThicknessVisualStylesEnabled = gripThicknessVisualStylesEnabled;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        internal ToolStripGrip()	{
            if (DpiHelper.IsScalingRequirementMet) {
                scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(defaultPadding);
                scaledGripThickness = DpiHelper.LogicalToDeviceUnitsX(gripThicknessDefault);
                scaledGripThicknessVisualStylesEnabled = DpiHelper.LogicalToDeviceUnitsX(gripThicknessVisualStylesEnabled);
            }

            // if we're using Visual Styles we've got to be a bit thicker.
            gripThickness = ToolStripManager.VisualStylesEnabled ? scaledGripThicknessVisualStylesEnabled : scaledGripThickness;     
            SupportsItemClick = false;
        }

	    /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected internal override Padding DefaultMargin {
            get {
                return scaledDefaultPadding;
            }
        }

        public override bool CanSelect {  
            get  {
                return false; 
            } 
        }

        internal int GripThickness {
            get {
                return gripThickness;
            }
        }

        internal bool MovingToolStrip {
            get {
                return ((ToolStripPanelRow != null) && movingToolStrip);  
            }
            set {
                if ((movingToolStrip != value) && ParentInternal != null) {
                    if (value) {
                        // dont let grips move the toolstrip
                        if (ParentInternal.ToolStripPanelRow == null) {
                            return;
                        }
                    }
                    movingToolStrip = value;
                    lastEndLocation = ToolStrip.InvalidMouseEnter;
                    if (movingToolStrip) {
                       ((ISupportToolStripPanel)this.ParentInternal).BeginDrag();
                    }
                    else {
                       ((ISupportToolStripPanel)this.ParentInternal).EndDrag();
                    }
                }
            }
        }

        private ToolStripPanelRow ToolStripPanelRow {
            get {
                return (ParentInternal == null) ? null : ((ISupportToolStripPanel)ParentInternal).ToolStripPanelRow;
            }
        }

        
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new ToolStripGripAccessibleObject(this);
        }

        public override Size GetPreferredSize(Size constrainingSize) {
            Size preferredSize = Size.Empty;
            if (this.ParentInternal != null) {
                if (this.ParentInternal.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow) {
                    preferredSize = new Size(this.ParentInternal.Width, gripThickness);
                }
                else {
                    preferredSize = new Size(gripThickness, this.ParentInternal.Height);
                    
                }
                    
            }
            // Constrain ourselves
            if (preferredSize.Width > constrainingSize.Width) {
                preferredSize.Width = constrainingSize.Width;
            }

            if (preferredSize.Height > constrainingSize.Height) {
                preferredSize.Height = constrainingSize.Height;
            }
            
            return preferredSize;
        }

        private bool LeftMouseButtonIsDown() {
          return (Control.MouseButtons == MouseButtons.Left) && (Control.ModifierKeys == Keys.None);
        }
         
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            // all the grip painting should be on the ToolStrip itself. 
            if (ParentInternal != null) {
                ParentInternal.OnPaintGrip(e);
            }
        }
            
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mea"></param>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs mea) {            
            startLocation =  TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
            base.OnMouseDown(mea);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs mea) {  
            bool leftMouseButtonDown = LeftMouseButtonIsDown();
            if (!MovingToolStrip && leftMouseButtonDown) {

                // determine if we've moved far enough such that the toolstrip
                // can be considered as moving.
                Point currentLocation =  TranslatePoint(mea.Location, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
                int deltaX = currentLocation.X - startLocation.X;
                deltaX = (deltaX < 0) ? deltaX *-1 : deltaX;

                if (DragSize == LayoutUtils.MaxSize) {
                    DragSize = SystemInformation.DragSize;
                }
               
                if (deltaX >= DragSize.Width) {
                    MovingToolStrip = true;
                }
                else {
                     int deltaY = currentLocation.Y - startLocation.Y;
                     deltaY = (deltaY < 0) ? deltaY *-1 : deltaY;

                    if (deltaY >= DragSize.Height) {
                        MovingToolStrip = true;
                    }
                }
                
            }
            if (MovingToolStrip) {
                if (leftMouseButtonDown) {
                    Point endLocation = TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
                    // protect against calling when the mouse hasnt really moved.  moving the toolstrip/creating the feedback rect
                    // can cause extra mousemove events, we want to make sure we're not doing all this work
                    // for nothing.
                    if (endLocation != lastEndLocation) {
                        ToolStripPanelRow.ToolStripPanel.MoveControl(ParentInternal, /*startLocation,*/endLocation );
                        lastEndLocation = endLocation;
                    }                        
                    startLocation = endLocation;
                }
                else {
                    // sometimes we dont get mouseup in DT.   Release now.
                    MovingToolStrip = false;
                }
            }
    
            base.OnMouseMove(mea);
        }

        protected override void OnMouseEnter(System.EventArgs e) { 

            // only switch the cursor if we've got a rafting row.
            if ((ParentInternal != null) && (ToolStripPanelRow != null) && (!ParentInternal.IsInDesignMode)) {
                oldCursor = ParentInternal.Cursor;
                SetCursor(ParentInternal, Cursors.SizeAll);
            }
            else {
                oldCursor = null;
            }
            base.OnMouseEnter(e);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(System.EventArgs e) { 
            if (oldCursor != null && !ParentInternal.IsInDesignMode) {
                SetCursor(ParentInternal,oldCursor);
            }
            if (!MovingToolStrip && LeftMouseButtonIsDown()) {
                MovingToolStrip = true;
            }
            base.OnMouseLeave(e);
        }

       
        
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs mea) {    
            if (MovingToolStrip) {
                Point endLocation = TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
                ToolStripPanelRow.ToolStripPanel.MoveControl(ParentInternal, /*startLocation,*/endLocation );
            }

            if (!ParentInternal.IsInDesignMode) {
                SetCursor(ParentInternal, oldCursor);
            }
            ToolStripPanel.ClearDragFeedback();
            MovingToolStrip = false;
            base.OnMouseUp(mea);
        }

        private static void SetCursor(Control control, Cursor cursor) {
            IntSecurity.ModifyCursor.Assert();
            control.Cursor = cursor;
            // CodeAccessPermission.RevertAssert automatically called 
        }

        internal class ToolStripGripAccessibleObject : ToolStripButtonAccessibleObject {
            private string stockName;

            public ToolStripGripAccessibleObject(ToolStripGrip owner) : base(owner){
            }

            public override string Name {
                get {
                    string name = Owner.AccessibleName;
                    if (name != null) {
                        return name;
                    }
                    if (string.IsNullOrEmpty(stockName)) {
                        stockName = SR.ToolStripGripAccessibleName;
                    }
                    return stockName;
                }
                set {
                    base.Name  = value;
                }
            }

            
            public override AccessibleRole Role {
               get {
                   AccessibleRole role = Owner.AccessibleRole;
                   if (role != AccessibleRole.Default) {
                       return role;
                   }
                   return AccessibleRole.Grip;
               }
            }

            internal override object GetPropertyValue(int propertyID) {
                if (AccessibilityImprovements.Level3) {
                    switch (propertyID) {
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return false;
                        case NativeMethods.UIA_ControlTypePropertyId:
                            return NativeMethods.UIA_ThumbControlTypeId;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
   

