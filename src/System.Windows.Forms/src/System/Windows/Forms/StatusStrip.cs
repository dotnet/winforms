// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections.Specialized;    
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Layout;
    using System.Security.Permissions;
    using System.Security;
    
    /// <include file='doc\StatusStrip.uex' path='docs/doc[@for="StatusStrip"]/*' />
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     SRDescription(nameof(SR.DescriptionStatusStrip))
    ]
    public class StatusStrip : ToolStrip  {

        private const AnchorStyles AllAnchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
        private const AnchorStyles HorizontalAnchor = AnchorStyles.Left | AnchorStyles.Right;
        private const AnchorStyles VerticalAnchor = AnchorStyles.Top | AnchorStyles.Bottom;

        private BitVector32                    state                               = new BitVector32();

        private static readonly int stateSizingGrip                       = BitVector32.CreateMask();
        private static readonly int stateCalledSpringTableLayout          = BitVector32.CreateMask(stateSizingGrip);
        

        private const int gripWidth = 12;
        private RightToLeftLayoutGrip rtlLayoutGrip;
        private Orientation lastOrientation = Orientation.Horizontal;
        
        /// <include file='doc\StatusStrip.uex' path='docs/doc[@for="StatusStrip.StatusStrip"]/*' />
        public StatusStrip() {
            SuspendLayout();
            this.CanOverflow = false;
            this.LayoutStyle = ToolStripLayoutStyle.Table;
            this.RenderMode = ToolStripRenderMode.System;
            this.GripStyle = ToolStripGripStyle.Hidden;

            SetStyle(ControlStyles.ResizeRedraw, true);
            this.Stretch = true;
            state[stateSizingGrip] = true;
            ResumeLayout(true);
            
        }

        [
        DefaultValue(false),
        SRDescription(nameof(SR.ToolStripCanOverflowDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false)
        ]
        public new bool CanOverflow { 
            get { 
                return base.CanOverflow;         
            }
            set { 
                base.CanOverflow = value;
            }
        }


        protected override bool DefaultShowItemToolTips {
            get {
                return false;
            }
        }


        protected override Size DefaultSize {
            get {
                return new Size(200, 22);
            }
        }

        
        protected override Padding DefaultPadding {
            get {
                if (Orientation == Orientation.Horizontal) {
                    if (RightToLeft == RightToLeft.No) {
                        return new Padding(1, 0, 14, 0);
                    }
                    else {
                        return new Padding(14, 0, 1, 0);                    
                    }
                }
                else {
                    // vertical
                    // the difference in symmetry here is that the grip does not actually rotate, it remains the same height it
                    // was before, so the DisplayRectangle needs to shrink up by its height.
                    return new Padding(1, 3, 1, DefaultSize.Height);
                }
               
            }
        }

        protected override DockStyle DefaultDock {
           get {
               return DockStyle.Bottom;
           }
        }
   

       /// <include file='doc\StatusStrip.uex' path='docs/doc[@for="StatusStrip.Dock"]/*' />
       [DefaultValue(DockStyle.Bottom)]
       public override DockStyle Dock {
           get {
               return base.Dock;
           }
           set {
               base.Dock = value;
           }
       }

        [DefaultValue(ToolStripGripStyle.Hidden)]
        public new ToolStripGripStyle GripStyle {
            get {
            	return base.GripStyle;
            }
            set {
            	base.GripStyle = value;
            }
        }

        [DefaultValue(ToolStripLayoutStyle.Table)]
        public new ToolStripLayoutStyle LayoutStyle {
            get { return base.LayoutStyle; }
            set { base.LayoutStyle = value; }
        }

        // we do some custom stuff with padding to accomodate size grip.
        // changing this is not supported at DT
        [Browsable(false)]
        public new Padding Padding {
            get { 
                return base.Padding;
            }
            set {
                base.Padding = value;
            }
        }

        [Browsable(false)]
        public new event EventHandler PaddingChanged {
            add {
                base.PaddingChanged += value;
            }
            remove {
                base.PaddingChanged -= value;
            }
        }
     
        private Control RTLGrip {
             get{
                 if (rtlLayoutGrip == null) {
                     rtlLayoutGrip = new RightToLeftLayoutGrip();
                 }
                 return rtlLayoutGrip;
             }
        }

    
       [DefaultValue(false)]
       [SRDescription(nameof(SR.ToolStripShowItemToolTipsDescr))]
       [SRCategory(nameof(SR.CatBehavior))]
       public new bool ShowItemToolTips { 
           get {
                return base.ShowItemToolTips;
           }
           set {
                base.ShowItemToolTips = value;
            }
       }

       // return whether we should paint the sizing grip.
       private bool ShowSizingGrip {
            get{
                if (SizingGrip && IsHandleCreated) {
                    if (DesignMode) {
                        return true;  // we dont care about the state of VS.
                    }
                    else {
                        HandleRef rootHwnd = WindowsFormsUtils.GetRootHWnd(this);
                        if (rootHwnd.Handle != IntPtr.Zero) {
                            return !UnsafeNativeMethods.IsZoomed(rootHwnd); 
                        }
                    }
                }
                return false;
            }
       }
       [
       SRCategory(nameof(SR.CatAppearance)),
       DefaultValue(true),
       SRDescription(nameof(SR.StatusStripSizingGripDescr))
       ]
       public bool SizingGrip {
          get {
              return state[stateSizingGrip];
          }
          set {
              if (value != state[stateSizingGrip]) {
                  state[stateSizingGrip] = value;
                  EnsureRightToLeftGrip();
                  Invalidate(true);
              }
          }
       }

       [Browsable(false)]
       public Rectangle SizeGripBounds {
            get { 
                if (SizingGrip) {
                    Size statusStripSize = this.Size;
                    // we cant necessarily make this the height of the status strip, as
                    // the orientation could change.
                    int gripHeight = Math.Min(DefaultSize.Height, statusStripSize.Height);
                    
                    if (RightToLeft == RightToLeft.Yes) {
                        return new Rectangle(0, statusStripSize.Height - gripHeight, gripWidth, gripHeight);
                    }
                    else {
                        return new Rectangle(statusStripSize.Width - gripWidth, statusStripSize.Height - gripHeight, gripWidth, gripHeight);
                    }
                }
                return Rectangle.Empty;
            }
       }

       
       [DefaultValue(true)]
       [SRCategory(nameof(SR.CatLayout))]
       [SRDescription(nameof(SR.ToolStripStretchDescr))]
       public new bool Stretch {
           get {
              return base.Stretch;
           }
           set {
              base.Stretch = value;
           }
       }

     
       private TableLayoutSettings TableLayoutSettings {
           get { return this.LayoutSettings as TableLayoutSettings; }
       }
 
       /// <include file='doc\StatusStrip.uex' path='docs/doc[@for="StatusStrip.CreateAccessibilityInstance"]/*' />
       protected override AccessibleObject CreateAccessibilityInstance() {
           return new StatusStripAccessibleObject(this);
       }
       
       protected internal override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) {
            return new ToolStripStatusLabel(text,image,onClick);
       }

       protected override void Dispose( bool disposing ) {
            if (disposing) {
                if (rtlLayoutGrip != null) {
                    rtlLayoutGrip.Dispose();
                    rtlLayoutGrip = null;
                }
            }
            base.Dispose(disposing);
       }

       // in RTL, we parent a transparent control over the grip to support mirroring.
       private void EnsureRightToLeftGrip() {
          if (SizingGrip && RightToLeft == RightToLeft.Yes) {
              RTLGrip.Bounds = SizeGripBounds;
              if (!this.Controls.Contains(RTLGrip)) {
                   WindowsFormsUtils.ReadOnlyControlCollection  controlCollection = this.Controls as WindowsFormsUtils.ReadOnlyControlCollection;
   
                   if (controlCollection != null) {
                       controlCollection.AddInternal(RTLGrip);
                   }
              }                
          }
          else if (rtlLayoutGrip != null) {
             if (this.Controls.Contains(rtlLayoutGrip)) {
                 WindowsFormsUtils.ReadOnlyControlCollection  controlCollection = this.Controls as WindowsFormsUtils.ReadOnlyControlCollection;
                 
                  if (controlCollection != null) {
                      controlCollection.RemoveInternal(rtlLayoutGrip);
                  }
                  rtlLayoutGrip.Dispose();
                  rtlLayoutGrip = null;
             }
               
          }
   
       }

       internal override Size GetPreferredSizeCore(Size proposedSize) {
            if (LayoutStyle == ToolStripLayoutStyle.Table) {
                
                if (proposedSize.Width == 1) {
                    proposedSize.Width = Int32.MaxValue;
                }
                if (proposedSize.Height == 1) {
                    proposedSize.Height = Int32.MaxValue;
                }
                if (Orientation == Orientation.Horizontal) {
                    return GetPreferredSizeHorizontal(this, proposedSize) + Padding.Size;
                }
                else {
                    return GetPreferredSizeVertical(this, proposedSize) + Padding.Size;
                }
            }
            return base.GetPreferredSizeCore(proposedSize);
       }

       protected override void OnPaintBackground(PaintEventArgs e) {
          base.OnPaintBackground(e);

          if (ShowSizingGrip) {
            Renderer.DrawStatusStripSizingGrip(new ToolStripRenderEventArgs(e.Graphics, this));
          }
       }

  
       protected override void OnLayout(LayoutEventArgs levent) {
           state[stateCalledSpringTableLayout] = false;
           bool inDisplayedItemCollecton = false;
           ToolStripItem item = levent.AffectedComponent as ToolStripItem;
           int itemCount = DisplayedItems.Count;
           if (item != null) {            
             inDisplayedItemCollecton = DisplayedItems.Contains(item);
           }
           
           
           if (this.LayoutStyle == ToolStripLayoutStyle.Table) {
               OnSpringTableLayoutCore();
           }
           base.OnLayout(levent);

           if (itemCount != DisplayedItems.Count || (item != null && (inDisplayedItemCollecton != DisplayedItems.Contains(item)))) {
               // calling OnLayout has changed the displayed items collection 
               // the SpringTableLayoutCore requires the count of displayed items to
               // be accurate.
               // - so we need to perform layout again.   
               if (this.LayoutStyle == ToolStripLayoutStyle.Table) {
                   OnSpringTableLayoutCore();
                   base.OnLayout(levent);
               }

           }

           EnsureRightToLeftGrip();
           
       }
  
        internal override bool SupportsUiaProviders {
            get {
                return AccessibilityImprovements.Level3;
            }
        }

    
       protected override void SetDisplayedItems() {
           if (state[stateCalledSpringTableLayout]) {
               bool rightToLeft = ((Orientation == Orientation.Horizontal) && (RightToLeft == RightToLeft.Yes));
                
               // shove all items that dont fit one pixel outside the displayed region
               Rectangle displayRect = DisplayRectangle;
               Point noMansLand = displayRect.Location;
               noMansLand.X += ClientSize.Width + 1;
               noMansLand.Y += ClientSize.Height + 1;
               bool overflow = false;
               Rectangle lastItemBounds = Rectangle.Empty;
               
               ToolStripItem lastItem = null;
               for (int i = 0; i < Items.Count; i++) {
                   ToolStripItem item = Items[i];
                   
                   // using spring layout we can get into a situation where there's extra items which arent 
                   // visible.
                   if (overflow || ((IArrangedElement)item).ParticipatesInLayout) {

                        if (overflow || (SizingGrip && item.Bounds.IntersectsWith(SizeGripBounds))) {
                            // if the item collides with the size grip, set the location to nomansland.
                            SetItemLocation(item, noMansLand); 
                            item.SetPlacement(ToolStripItemPlacement.None);
                        }
                   }
                   else if (lastItem != null && (lastItemBounds.IntersectsWith(item.Bounds))) {
                      // if it overlaps the previous element, set the location to nomansland.
                      SetItemLocation(item, noMansLand);
                      item.SetPlacement(ToolStripItemPlacement.None);
                   }
                   else if (item.Bounds.Width == 1) {
                        ToolStripStatusLabel panel = item as ToolStripStatusLabel;
                        if (panel != null && panel.Spring) {
                            // once we get down to one pixel, there can always be a one pixel 
                            // distribution problem with the TLP - there's usually a spare one around.
                            // so set this off to nomansland as well.
                            SetItemLocation(item, noMansLand);
                            item.SetPlacement(ToolStripItemPlacement.None);
                        }
                   }
                   

                   if (item.Bounds.Location != noMansLand){
                       // set the next item to inspect for collisions
                       lastItem = item;
                       lastItemBounds = lastItem.Bounds; 
                   }
                   else {
                       // we cant fit an item, everything else after it should not be displayed
                       if (((IArrangedElement)item).ParticipatesInLayout) {
                           overflow = true;
                       }
                   }
			  }
           }
           base.SetDisplayedItems();
       }
  
       internal override void ResetRenderMode() {
          RenderMode = ToolStripRenderMode.System;
       }
       internal override bool ShouldSerializeRenderMode() {
           // We should NEVER serialize custom.
           return (RenderMode != ToolStripRenderMode.System && RenderMode != ToolStripRenderMode.Custom);
       }


       
      /// <devdoc>
      /// Override this function if you want to do custom table layouts for the 
      /// StatusStrip.  The default layoutstyle is tablelayout, and we need to play
      /// with the row/column styles 
      /// </devdoc>
      protected virtual void OnSpringTableLayoutCore() {
          if (this.LayoutStyle == ToolStripLayoutStyle.Table) {
            state[stateCalledSpringTableLayout]= true;
             
              this.SuspendLayout();

              if (lastOrientation != Orientation) {
                 TableLayoutSettings settings = this.TableLayoutSettings;
                 settings.RowCount = 0;
                 settings.ColumnCount = 0;
                 settings.ColumnStyles.Clear();
                 settings.RowStyles.Clear();
              }
              lastOrientation = Orientation;

              if (Orientation == Orientation.Horizontal) {
                  
                  // 
                  // Horizontal layout
                  //
                  TableLayoutSettings.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
   
                  int originalColumnCount = this.TableLayoutSettings.ColumnStyles.Count;
   
                  // iterate through the elements which are going to be displayed.
                  for (int i = 0; i < this.DisplayedItems.Count; i++) {
                      if (i >= originalColumnCount) {
                          // add if it's necessary.
                          this.TableLayoutSettings.ColumnStyles.Add(new ColumnStyle());
                      }
   
                      // determine if we "spring" or "autosize" the column
                      ToolStripStatusLabel panel = DisplayedItems[i] as ToolStripStatusLabel;
                      bool spring = (panel != null && panel.Spring);
                      DisplayedItems[i].Anchor = (spring) ? AllAnchor : VerticalAnchor;
                     
                      // spring is achieved by using 100% as the column style
                      ColumnStyle colStyle = this.TableLayoutSettings.ColumnStyles[i];
                      colStyle.Width = 100; // this width is ignored in AutoSize.
                      colStyle.SizeType = (spring) ? SizeType.Percent : SizeType.AutoSize;
                  }
  
                  if (TableLayoutSettings.RowStyles.Count > 1 || TableLayoutSettings.RowStyles.Count == 0) {
                      TableLayoutSettings.RowStyles.Clear();
                      TableLayoutSettings.RowStyles.Add(new RowStyle());
                  }
                  TableLayoutSettings.RowCount = 1;
  
                  TableLayoutSettings.RowStyles[0].SizeType = SizeType.Absolute;
                  TableLayoutSettings.RowStyles[0].Height = Math.Max(0,this.DisplayRectangle.Height);
                  TableLayoutSettings.ColumnCount =  DisplayedItems.Count+1; // add an extra cell so it fills the remaining space

                  // dont remove the extra column styles, just set them back to autosize.
                  for (int i = DisplayedItems.Count; i < TableLayoutSettings.ColumnStyles.Count; i++) {
                        this.TableLayoutSettings.ColumnStyles[i].SizeType = SizeType.AutoSize;
                  }
              }
              else {
                  // 
                  // Vertical layout
                  //
                
                  TableLayoutSettings.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
   
                  int originalRowCount = this.TableLayoutSettings.RowStyles.Count;
   
                  // iterate through the elements which are going to be displayed.
                  for (int i = 0; i < this.DisplayedItems.Count; i++) {
                      if (i >= originalRowCount) {
                          // add if it's necessary.
                          this.TableLayoutSettings.RowStyles.Add(new RowStyle());
                      }
   
                      // determine if we "spring" or "autosize" the row
                      ToolStripStatusLabel panel = DisplayedItems[i] as ToolStripStatusLabel;
                      bool spring = (panel != null && panel.Spring);
                      DisplayedItems[i].Anchor = (spring) ? AllAnchor : HorizontalAnchor;
   
                      // spring is achieved by using 100% as the row style
                      RowStyle rowStyle = this.TableLayoutSettings.RowStyles[i];
                      rowStyle.Height = 100; // this width is ignored in AutoSize.
                      rowStyle.SizeType = (spring) ? SizeType.Percent : SizeType.AutoSize;
                  }
                  TableLayoutSettings.ColumnCount = 1;
                  
                  if (TableLayoutSettings.ColumnStyles.Count > 1 || TableLayoutSettings.ColumnStyles.Count == 0) {
                      TableLayoutSettings.ColumnStyles.Clear();
                      TableLayoutSettings.ColumnStyles.Add(new ColumnStyle());
                  }
  
                  TableLayoutSettings.ColumnCount = 1;
                  TableLayoutSettings.ColumnStyles[0].SizeType = SizeType.Absolute;
                  TableLayoutSettings.ColumnStyles[0].Width = Math.Max(0,this.DisplayRectangle.Width);
               
                  TableLayoutSettings.RowCount =  DisplayedItems.Count+1; // add an extra cell so it fills the remaining space

                  // dont remove the extra column styles, just set them back to autosize.
                  for (int i = DisplayedItems.Count; i < TableLayoutSettings.RowStyles.Count; i++) {
                        this.TableLayoutSettings.RowStyles[i].SizeType = SizeType.AutoSize;
                  }
  
              }
   
              this.ResumeLayout(false);
          }
      }

      [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
      protected override void  WndProc(ref Message m)            {                
          if ((m.Msg == NativeMethods.WM_NCHITTEST) && SizingGrip) {
            // if we're within the grip bounds tell windows
            // that we're the bottom right of the window.  
            Rectangle sizeGripBounds = SizeGripBounds;
            int x = NativeMethods.Util.LOWORD(m.LParam);
            int y = NativeMethods.Util.HIWORD(m.LParam);

                       
            if (sizeGripBounds.Contains(PointToClient(new Point(x, y)))) {
                HandleRef rootHwnd = WindowsFormsUtils.GetRootHWnd(this);

                // if the main window isnt maximized - we should paint a resize grip.
                // double check that we're at the bottom right hand corner of the window.
                if (rootHwnd.Handle != IntPtr.Zero && !UnsafeNativeMethods.IsZoomed(rootHwnd)) {
                    // get the client area of the topmost window.  If we're next to the edge then 
                    // the sizing grip is valid.
                    NativeMethods.RECT rootHwndClientArea = new NativeMethods.RECT();
                    UnsafeNativeMethods.GetClientRect(rootHwnd, ref rootHwndClientArea);
                    
                    // map the size grip FROM statusStrip coords TO the toplevel window coords.
                    NativeMethods.POINT gripLocation;
                    if (RightToLeft == RightToLeft.Yes) {
                        gripLocation = new NativeMethods.POINT(SizeGripBounds.Left, SizeGripBounds.Bottom);
                    }
                    else {
                        gripLocation = new NativeMethods.POINT(SizeGripBounds.Right, SizeGripBounds.Bottom);
                    }
                    UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, this.Handle), rootHwnd, gripLocation, 1);

                    int deltaBottomEdge = Math.Abs(rootHwndClientArea.bottom - gripLocation.y);
                    int deltaRightEdge = Math.Abs(rootHwndClientArea.right - gripLocation.x);
                    
                    if (RightToLeft != RightToLeft.Yes) {
                        if ((deltaRightEdge + deltaBottomEdge) < 2) {
                            m.Result = (IntPtr)NativeMethods.HTBOTTOMRIGHT;                   
                            return;                      
                        }
                    }
                    
                    
                }
               
            }                
          }
          base.WndProc(ref m);            
       }

       // special transparent mirrored window which says it's the bottom left of the form.
       private class RightToLeftLayoutGrip : Control {
            public RightToLeftLayoutGrip() {
                SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                this.BackColor = Color.Transparent;
            }            
            protected override CreateParams CreateParams {
               [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
                get {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL;
                    return cp;            
                }
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            protected override void WndProc(ref Message m) {
                 if (m.Msg == NativeMethods.WM_NCHITTEST) {
                    int x = NativeMethods.Util.LOWORD(m.LParam);
                    int y = NativeMethods.Util.HIWORD(m.LParam);

                    if (ClientRectangle.Contains(PointToClient(new Point(x, y)))) {
                        m.Result = (IntPtr)NativeMethods.HTBOTTOMLEFT;
                        return;
                    }

                 }
                 base.WndProc(ref m);
            }
       }
  

       [System.Runtime.InteropServices.ComVisible(true)]
       internal class StatusStripAccessibleObject : ToolStripAccessibleObject {
            public StatusStripAccessibleObject(StatusStrip owner) : base(owner) {
            }

            public override AccessibleRole Role {
                get {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    return AccessibleRole.StatusBar;
                }
            }

            internal override object GetPropertyValue(int propertyID) {
                if (AccessibilityImprovements.Level3 && propertyID == NativeMethods.UIA_ControlTypePropertyId) {
                    return NativeMethods.UIA_StatusBarControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                StatusStrip statusStrip = Owner as StatusStrip;
                if (statusStrip == null || statusStrip.Items.Count == 0) {
                    return null;
                }

                switch (direction) {
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        AccessibleObject firstChild = null;
                        for (int i = 0; i < GetChildCount(); i++) {
                            firstChild = GetChild(i);
                            if (firstChild != null && !(firstChild is ControlAccessibleObject)) {
                                return firstChild;
                            }
                        }
                        return null;

                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        AccessibleObject lastChild = null;
                        for (int i = GetChildCount() - 1; i >= 0; i--) {
                            lastChild = GetChild(i);
                            if (lastChild != null && !(lastChild is ControlAccessibleObject)) {
                                return lastChild;
                            }
                        }
                        return null;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y) {
                return HitTest((int)x, (int)y);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus() {
                return GetFocused();
            }
        }

    }
}
