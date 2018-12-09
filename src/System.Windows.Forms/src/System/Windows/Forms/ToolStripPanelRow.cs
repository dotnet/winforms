// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define DEBUG_PAINT


namespace System.Windows.Forms {
    using System.Drawing;
    using System.Windows.Forms.Layout;
    using System.Collections.Specialized;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow"]/*' />
    [ToolboxItem(false)]
    public class ToolStripPanelRow : Component, IArrangedElement {
        private Rectangle bounds = Rectangle.Empty;
        private ToolStripPanel parent = null;
        private BitVector32 state = new BitVector32();
        private PropertyStore propertyStore = new PropertyStore();  // Contains all properties that are not always set.
        private int suspendCount = 0;
        private ToolStripPanelRowManager rowManager = null;


        private const int MINALLOWEDWIDTH = 50;
        private int minAllowedWidth = MINALLOWEDWIDTH;

        private static readonly int stateVisible = BitVector32.CreateMask();
        private static readonly int stateDisposing = BitVector32.CreateMask(stateVisible);
        private static readonly int stateLocked = BitVector32.CreateMask(stateDisposing);
        private static readonly int stateInitialized = BitVector32.CreateMask(stateLocked);
        private static readonly int stateCachedBoundsMode = BitVector32.CreateMask(stateInitialized);
        private static readonly int stateInLayout = BitVector32.CreateMask(stateCachedBoundsMode);
        
        
        
              

        private static readonly int PropControlsCollection = PropertyStore.CreateKey();
        
#if DEBUG
        internal static TraceSwitch ToolStripPanelRowCreationDebug = new TraceSwitch("ToolStripPanelRowCreationDebug", "Debug code for rafting row creation");
#else
        internal static TraceSwitch ToolStripPanelRowCreationDebug ;
#endif

#if DEBUG
        private static int rowCreationCount = 0;
        private int thisRowID;
#endif


        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.ToolStripPanelRow"]/*' />
        public ToolStripPanelRow(ToolStripPanel parent) : this(parent, true){
        }


        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        internal ToolStripPanelRow(ToolStripPanel parent, bool visible) {
#if DEBUG
            thisRowID = ++rowCreationCount;
#endif            
            if (DpiHelper.IsScalingRequirementMet) {
                minAllowedWidth = DpiHelper.LogicalToDeviceUnitsX(MINALLOWEDWIDTH);
            }

            this.parent = parent;
            this.state[stateVisible] = visible;
            this.state[stateDisposing | stateLocked| stateInitialized] = false;

            Debug.WriteLineIf(ToolStripPanelRowCreationDebug.TraceVerbose, "Created new ToolStripPanelRow");

            using (LayoutTransaction lt = new LayoutTransaction(parent, this, null)) {
                this.Margin = DefaultMargin;
                CommonProperties.SetAutoSize(this, true);
            }
            
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Bounds"]/*' />
        public Rectangle Bounds {
            get {
                return bounds;
            }
        }
        
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SRDescription(nameof(SR.ControlControlsDescr)), SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public Control[] Controls {
            get {
                Control[] controls = new Control[ControlsInternal.Count];
                ControlsInternal.CopyTo(controls,0);
                return controls;
            }
        }

            
        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Controls"]/*' />
        /// <devdoc>
        /// Collection of child controls.
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SRDescription(nameof(SR.ControlControlsDescr))]
        internal ToolStripPanelRowControlCollection ControlsInternal {
            get {
                ToolStripPanelRowControlCollection controlsCollection = (ToolStripPanelRowControlCollection)Properties.GetObject(PropControlsCollection);

                if (controlsCollection == null) {
                    controlsCollection = CreateControlsInstance();
                    Properties.SetObject(PropControlsCollection, controlsCollection);
                }

                return controlsCollection;
            }
        }

        internal ArrangedElementCollection Cells {
            get { 
                return ControlsInternal.Cells;
            }
        }

        internal bool CachedBoundsMode {
            get {
                return state[stateCachedBoundsMode];
            }
            set {
                state[stateCachedBoundsMode] = value;
            }
        }



        private ToolStripPanelRowManager RowManager {
            get {
                if (rowManager == null) {
                    rowManager = (Orientation == Orientation.Horizontal) ? new HorizontalRowManager(this) as ToolStripPanelRowManager 
                                                                         : new VerticalRowManager(this) as ToolStripPanelRowManager;
                    Initialized = true;
                }

                return rowManager;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.DefaultMargin"]/*' />
        protected virtual Padding DefaultMargin {
            get { 
                ToolStripPanelCell cell = RowManager.GetNextVisibleCell(0, /*forward*/true);
                if (cell != null && cell.DraggedControl != null) {
                    if (cell.DraggedControl.Stretch) {
                        Padding padding = ToolStripPanel.RowMargin;
                        // clear out the padding.
                        if (Orientation == Orientation.Horizontal) {
                            padding.Left = 0;
                            padding.Right = 0;
                        }
                        else {
                            padding.Top = 0;
                            padding.Bottom = 0;
                        }
                        return padding;
                    }
                }
                return ToolStripPanel.RowMargin; 

            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.DefaultPadding"]/*' />
        protected virtual Padding DefaultPadding {
            get { return Padding.Empty; }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.DisplayRectangle"]/*' />
        public Rectangle DisplayRectangle {
            get {
                return RowManager.DisplayRectangle;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.LayoutEngine"]/*' />
        public LayoutEngine LayoutEngine {
            get {
                return FlowLayout.Instance;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Locked"]/*' />
        internal bool Locked {
            get {
                return state[stateLocked];
            }           
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Initialized "]/*' />
        private bool Initialized {
            get {
                return state[stateInitialized];
            }
            set {
                state[stateInitialized] = value;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Margin"]/*' />
        public Padding Margin {
            get { return CommonProperties.GetMargin(this); }
            set { if (Margin != value ) CommonProperties.SetMargin(this, value); }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Padding"]/*' />
        public virtual Padding Padding {
            get { return CommonProperties.GetPadding(this, DefaultPadding); }
            set {
                if (Padding != value) CommonProperties.SetPadding(this, value);
            }
        }

        internal Control ParentInternal {
            get {
                return parent;
            }         
        }

        /// <devdoc>
        ///     Retrieves our internal property storage object. If you have a property
        ///     whose value is not always set, you should store it in here to save
        ///     space.
        /// </devdoc>
        internal PropertyStore Properties {
            get {
                return propertyStore;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.ToolStripPanel"]/*' />
        public ToolStripPanel ToolStripPanel {
            get {
                return parent;
            }
        }

        internal bool Visible {
            get {
                return state[stateVisible];
                }       
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Orientation"]/*' />
        public Orientation Orientation {
            get {
                return ToolStripPanel.Orientation;
            }
        }

#if DEBUG
        internal void Debug_PrintRowID() {
            Debug.Write(thisRowID.ToString(CultureInfo.CurrentCulture));
        }
#endif

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.CanMove"]/*' />
        /// <devdoc>
        /// returns true if there is enough space to "raft" the control
        /// ow returns false
        /// </devdoc>
        public bool CanMove(ToolStrip toolStripToDrag) {
            return !ToolStripPanel.Locked && !Locked && RowManager.CanMove(toolStripToDrag);
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.CreateControlsInstance"]/*' />
        private ToolStripPanelRowControlCollection CreateControlsInstance() {
            return new ToolStripPanelRowControlCollection(this);
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            try {
                if (disposing) {
                    
                    Debug.WriteLineIf(ToolStripPanelRowCreationDebug.TraceVerbose, "Disposed ToolStripPanelRow");
                    state[stateDisposing] = true;
                    this.ControlsInternal.Clear();
                }
            }
            finally {
                state[stateDisposing] = false;
                base.Dispose(disposing);
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.OnControlAdded"]/*' />
        protected internal virtual void OnControlAdded(Control control, int index) {

        
            // if previously added - remove.
            ISupportToolStripPanel controlToBeDragged = control as ISupportToolStripPanel;
            
            if (controlToBeDragged != null) {
                controlToBeDragged.ToolStripPanelRow = this;
            }
            RowManager.OnControlAdded(control, index);
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.OnOrientationChanged"]/*' />
        protected internal virtual void OnOrientationChanged() {
            this.rowManager = null;
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.OnBoundsChanged"]/*' />
        protected void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds) {
            ((IArrangedElement)this).PerformLayout((IArrangedElement)this, PropertyNames.Size);

            RowManager.OnBoundsChanged(oldBounds,newBounds);
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.OnControlRemoved"]/*' />
        protected internal virtual void OnControlRemoved(Control control, int index) {
            if (!state[stateDisposing]) {
                this.SuspendLayout();
                RowManager.OnControlRemoved(control, index);

                // if previously added - remove.
                ISupportToolStripPanel controlToBeDragged = control as ISupportToolStripPanel;

				if (controlToBeDragged != null && controlToBeDragged.ToolStripPanelRow  == this) {
                    controlToBeDragged.ToolStripPanelRow = null;
                }

                this.ResumeLayout(true);
                if (this.ControlsInternal.Count <= 0) {
                    ToolStripPanel.RowsInternal.Remove(this);
                    Dispose();
                }
            }
        }

        internal Size GetMinimumSize(ToolStrip toolStrip) {
            if (toolStrip.MinimumSize == Size.Empty) {
                return new Size(minAllowedWidth,minAllowedWidth);
            }
            else {
                return toolStrip.MinimumSize;
            }
        }

        private void ApplyCachedBounds() {
            for (int i = 0; i < this.Cells.Count; i++) {
                IArrangedElement element = Cells[i] as IArrangedElement;
                if (element.ParticipatesInLayout) {
                    ToolStripPanelCell cell = element as ToolStripPanelCell;
                    element.SetBounds(cell.CachedBounds, BoundsSpecified.None);
//                    Debug.Assert( cell.Control == null || cell.CachedBounds.Location == cell.Control.Bounds.Location, "CachedBounds out of sync with bounds!");
                }           
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.OnLayout"]/*' />
        protected virtual void OnLayout(LayoutEventArgs e) {
            if (Initialized && !state[stateInLayout]) {
             state[stateInLayout] = true;
             try {
                    this.Margin = DefaultMargin;
                    CachedBoundsMode = true;
                    try {
                        // dont layout in the constructor that's just tacky.
                        bool parentNeedsLayout = LayoutEngine.Layout(this, e);
                    }
                    finally {
                        CachedBoundsMode = false;
                    }

                    ToolStripPanelCell cell = RowManager.GetNextVisibleCell(this.Cells.Count -1, /*forward*/false);
                    if (cell == null) {
                        ApplyCachedBounds();
                    }
                    else if (Orientation == Orientation.Horizontal) {
                        OnLayoutHorizontalPostFix();
                    }
                    else {
                        OnLayoutVerticalPostFix();
                    }
               
                }
                 finally {
                    state[stateInLayout] = false;
                }
            }
        }

     
        
        private void OnLayoutHorizontalPostFix() {

            ToolStripPanelCell cell = RowManager.GetNextVisibleCell(this.Cells.Count -1, /*forward*/false);
            if (cell == null) {
                ApplyCachedBounds();
                return;
            }
            // figure out how much space we actually need to free.
            int spaceToFree = cell.CachedBounds.Right - RowManager.DisplayRectangle.Right;
            
            if (spaceToFree <= 0) {
                // we're all good. Just apply the cached bounds.
                ApplyCachedBounds();
                return;
            }
            // STEP 1 remove empty space in the row.

            // since layout sisuspended, we'll need to watch changes to the margin 
            // as a result of calling FreeSpaceFromRow.
            int[] margins = new int[Cells.Count];
            for (int i = 0; i < Cells.Count; i++) {
                ToolStripPanelCell c = Cells[i] as ToolStripPanelCell;
                margins[i] = c.Margin.Left;
            }

            spaceToFree -= RowManager.FreeSpaceFromRow(spaceToFree);

            // now apply those changes to the cached bounds.
            for (int i = 0; i < Cells.Count; i++) {
                ToolStripPanelCell c = Cells[i] as ToolStripPanelCell;
                Rectangle cachedBounds = c.CachedBounds;
                cachedBounds.X -= Math.Max(0, margins[i] - c.Margin.Left);
                c.CachedBounds = cachedBounds;
            }
            
            if (spaceToFree <= 0) {
                ApplyCachedBounds();
                return;
            }

            
            
            // STEP 2 change the size of the remaing ToolStrips from Right to Left.
            int[] cellOffsets = null;                        
            for (int i = Cells.Count-1; i >= 0; i--) {
                ToolStripPanelCell currentCell =Cells[i] as ToolStripPanelCell;
                if (currentCell.Visible) {
                    Size minSize = GetMinimumSize(currentCell.Control as ToolStrip);
                    Rectangle cachedBounds = currentCell.CachedBounds;

                    // found some space to free.
                    if (cachedBounds.Width > minSize.Width) {
                        spaceToFree -= (cachedBounds.Width - minSize.Width);
                        // make sure we dont take more space than we need - if spaceToFree is less than 0, add back in.
                        cachedBounds.Width =  (spaceToFree < 0) ? minSize.Width + -spaceToFree : minSize.Width;

                        // we're not reperforming a layout, so we need to adjust the next cell
                        for (int j = i+1; j < Cells.Count; j++) {
                            if (cellOffsets == null) {
                                cellOffsets = new int[Cells.Count];
                            }
                            cellOffsets[j] += Math.Max(0,currentCell.CachedBounds.Width-cachedBounds.Width);
                        }
                        currentCell.CachedBounds = cachedBounds;
                    }
                }
                if (spaceToFree <= 0) {
                    break;
                }
            }

            // fixup for items before it shrinking.
            if (cellOffsets != null) {
                for (int i = 0; i < Cells.Count; i++) {
                   ToolStripPanelCell c = Cells[i] as ToolStripPanelCell;
                   Rectangle cachedBounds = c.CachedBounds;
                   cachedBounds.X -= cellOffsets[i];
                   c.CachedBounds = cachedBounds;
               }
            }

            ApplyCachedBounds();
          
        }

        
        private void OnLayoutVerticalPostFix() {
            
          ToolStripPanelCell cell = RowManager.GetNextVisibleCell(this.Cells.Count -1, /*forward*/false);
          // figure out how much space we actually need to free.
          int spaceToFree = cell.CachedBounds.Bottom - RowManager.DisplayRectangle.Bottom;
          
          if (spaceToFree <= 0) {
              // we're all good. Just apply the cached bounds.
              ApplyCachedBounds();
              return;
          }
           // STEP 1 remove empty space in the row.

           // since layout sisuspended, we'll need to watch changes to the margin 
           // as a result of calling FreeSpaceFromRow.
           int[] margins = new int[Cells.Count];
           for (int i = 0; i < Cells.Count; i++) {
               ToolStripPanelCell c = Cells[i] as ToolStripPanelCell;
               margins[i] = c.Margin.Top;
           }

           spaceToFree -= RowManager.FreeSpaceFromRow(spaceToFree);

           // now apply those changes to the cached bounds.
           for (int i = 0; i < Cells.Count; i++) {
               ToolStripPanelCell c = Cells[i] as ToolStripPanelCell;
               Rectangle cachedBounds = c.CachedBounds;
               cachedBounds.X = Math.Max(0, cachedBounds.X - margins[i] - c.Margin.Top);
               c.CachedBounds = cachedBounds;
           }
           
           if (spaceToFree <= 0) {
               ApplyCachedBounds();
               return;
           }

           
           
           // STEP 2 change the size of the remaing ToolStrips from Bottom to Top.
           int[] cellOffsets = null;                        
           for (int i = Cells.Count-1; i >= 0; i--) {
               ToolStripPanelCell currentCell =Cells[i] as ToolStripPanelCell;
               if (currentCell.Visible) {
                   Size minSize = GetMinimumSize(currentCell.Control as ToolStrip);
                   Rectangle cachedBounds = currentCell.CachedBounds;

                   // found some space to free.
                   if (cachedBounds.Height > minSize.Height) {
                       spaceToFree -= (cachedBounds.Height - minSize.Height);
                       // make sure we dont take more space than we need - if spaceToFree is less than 0, add back in.
                       cachedBounds.Height =  (spaceToFree < 0) ? minSize.Height + -spaceToFree : minSize.Height;

                       // we're not reperforming a layout, so we need to adjust the next cell
                       for (int j = i+1; j < Cells.Count; j++) {
                           if (cellOffsets == null) {
                               cellOffsets = new int[Cells.Count];
                           }
                           cellOffsets[j] += Math.Max(0,currentCell.CachedBounds.Height-cachedBounds.Height);
                       }
                       currentCell.CachedBounds = cachedBounds;
                   }
               }
               if (spaceToFree <= 0) {
                   break;
               }
           }

           // fixup for items before it shrinking.
           if (cellOffsets != null) {
               for (int i = 0; i < Cells.Count; i++) {
                  ToolStripPanelCell c = Cells[i] as ToolStripPanelCell;
                  Rectangle cachedBounds = c.CachedBounds;
                  cachedBounds.Y -= cellOffsets[i];
                  c.CachedBounds = cachedBounds;
              }
           }

           ApplyCachedBounds();
         
       }

#if DEBUG_PAINT
        internal void PaintColumns(PaintEventArgs e) {
            Graphics g = e.Graphics;
            
            using (Pen pen = new Pen(Color.Green)) {
                g.DrawRectangle(pen, this.DisplayRectangle);
            }

            foreach (ToolStripPanelCell c in this.Cells) {
                Rectangle inner = c.Bounds;
                Rectangle b = LayoutUtils.InflateRect(inner, c.Margin);
                if ((b.Width > 0) && (b.Height > 0)) {
                    using(Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(b, ProfessionalColors.ButtonSelectedGradientBegin, ProfessionalColors.ButtonSelectedGradientEnd, System.Drawing.Drawing2D.LinearGradientMode.Horizontal)) {
                        g.FillRectangle(brush, b);
                        g.DrawRectangle(SystemPens.ControlDarkDark, b.X, b.Y, b.Width -1, b.Height -1);
                        g.DrawRectangle(Pens.HotPink, inner.X, inner.Y, inner.Width -1, inner.Height -1);
                        if (c.Control != null) {
                            g.DrawString("BAD\r\n" + c.Control.Name, ToolStripPanel.Font, SystemBrushes.ControlText, inner); 
                        }
                        else {
                            g.DrawString("BAD\r\n" + "NULL control", ToolStripPanel.Font, SystemBrushes.ControlText, inner); 

                        }
                    }
                }
            }
        }
#endif

        private void SetBounds(Rectangle bounds) {
            if (bounds != this.bounds) {
                Rectangle oldBounds = this.bounds;

                this.bounds = bounds;
                OnBoundsChanged(oldBounds, bounds);
            }
        }

        private void SuspendLayout() {
            suspendCount++;
        }

        private void ResumeLayout(bool performLayout) {
            suspendCount--;
            if (performLayout) {
                ((IArrangedElement)this).PerformLayout(this, null);
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.Children"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        ArrangedElementCollection IArrangedElement.Children {
            get {
                return Cells;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.Container"]/*' />
        /// <devdoc>
        /// Should not be exposed as this returns an unexposed type.
        /// </devdoc>
        /// <internalonly/>
        IArrangedElement IArrangedElement.Container {
            get {
                return this.ToolStripPanel;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.DisplayRectangle"]/*' />

        Rectangle IArrangedElement.DisplayRectangle {
            get {
                Rectangle displayRectangle = this.Bounds;

                return displayRectangle;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.ParticipatesInLayout"]/*' />

        bool IArrangedElement.ParticipatesInLayout {
            get {
                return Visible;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.Properties"]/*' />
        /// <internalonly/>
        PropertyStore IArrangedElement.Properties {
            get {
                return this.Properties;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.GetPreferredSize"]/*' />
        /// <internalonly/>
        Size IArrangedElement.GetPreferredSize(Size constrainingSize) {
            Size preferredSize = LayoutEngine.GetPreferredSize(this, constrainingSize - Padding.Size) + Padding.Size;

            if (Orientation == Orientation.Horizontal && ParentInternal != null) {
                preferredSize.Width = DisplayRectangle.Width;
            }
            else {
                preferredSize.Height = DisplayRectangle.Height;
            }

            return preferredSize;
        }

        // Sets the bounds for an element.
        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.SetBounds"]/*' />
        /// <internalonly/>
        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified) {
            // in this case the parent is telling us to refresh our bounds - dont 
            // call PerformLayout
            SetBounds(bounds);
        }

       
        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.IArrangedElement.PerformLayout"]/*' />

        void IArrangedElement.PerformLayout(IArrangedElement container, string propertyName) {
            if (suspendCount <= 0) {
                OnLayout(new LayoutEventArgs(container, propertyName));
            }
        }

            #region MouseStuff

#if DEBUG
      internal static readonly TraceSwitch ToolStripPanelMouseDebug = new TraceSwitch("ToolStripPanelMouse", "Debug WinBar WM_MOUSEACTIVATE code");
#else
        internal static readonly TraceSwitch ToolStripPanelMouseDebug;
#endif

     
        internal Rectangle DragBounds {
            get {
                return RowManager.DragBounds;
            }
        }

        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRow.MoveControl"]/*' />
        internal void MoveControl(ToolStrip movingControl, Point startClientLocation, Point endClientLocation) {
            RowManager.MoveControl(movingControl, startClientLocation, endClientLocation);
        }

        // 


        internal void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag) {
            RowManager.JoinRow(toolStripToDrag, locationToDrag);
        }

        internal void LeaveRow(ToolStrip toolStripToDrag) {
            RowManager.LeaveRow(toolStripToDrag);
            if (ControlsInternal.Count == 0) {
                ToolStripPanel.RowsInternal.Remove(this);
                Dispose();
            }
        }

        [Conditional("DEBUG")]
        private void PrintPlacements(int index) {
          /*  Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "Results:\r\n\t-------");
            Debug.Indent();
            Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "ToolStripPanelRow: " + this.Bounds.ToString());

            float sumColWidths = 0F;
            int sumWidths = 0;

            for (int i = 0; i < this.Controls.Count - 1; i++) {
                string indicator = (i == index) ? "*" : " ";

                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, String.Format("{0} {1} Column Width {2} Control Size {3}", indicator, this.Controls[i].Name, TableLayoutSettings.ColumnStyles[i].Width, this.Controls[i].Bounds));
                sumColWidths += TableLayoutSettings.ColumnStyles[i].Width;
                sumWidths += this.Controls[i].Width;
            }

            Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "Total Column Width " + sumColWidths.ToString() + " Total control widths " + sumWidths.ToString());
            Debug.Unindent();
            */
        }
            #endregion


        private abstract class ToolStripPanelRowManager {
            private FlowLayoutSettings flowLayoutSettings = null;

            private ToolStripPanelRow owner = null;

            public ToolStripPanelRowManager(ToolStripPanelRow owner) {
                this.owner = owner;
            }

            public virtual bool CanMove(ToolStrip toolStripToDrag) {
                ISupportToolStripPanel raftingControl = toolStripToDrag as ISupportToolStripPanel;
                if (raftingControl != null) {
                   if (raftingControl.Stretch) {
                        Debug.WriteLineIf(ToolStripPanelRow.ToolStripPanelRowCreationDebug.TraceVerbose, "TSP RM CanMove returns false - the item moving is stretched.");
                        return false;
                   }
                }
                foreach (Control c in Row.ControlsInternal) {
                     raftingControl = c as ISupportToolStripPanel;
                     if (raftingControl != null) {
                        if (raftingControl.Stretch) {
                             Debug.WriteLineIf(ToolStripPanelRow.ToolStripPanelRowCreationDebug.TraceVerbose, "TSP RM CanMove returns false - the row already contains a stretched item.");
                             return false;
                        }
                     }
                }
                return true;
            }

            public virtual Rectangle DragBounds {
                get { return Rectangle.Empty; }
            }

            public virtual Rectangle DisplayRectangle {
                get { return Rectangle.Empty; }
            }

            public ToolStripPanel ToolStripPanel {
                get { return owner.ToolStripPanel; }
            }

            public ToolStripPanelRow Row {
                get { return owner; }
            }

            public FlowLayoutSettings FlowLayoutSettings {
                get {
                    if (flowLayoutSettings == null) {
                        flowLayoutSettings = new FlowLayoutSettings(owner);
                    }

                    return flowLayoutSettings;
                }
            }

            protected internal virtual int FreeSpaceFromRow(int spaceToFree) {
                return 0;
            }

            protected virtual int Grow(int index, int growBy) {
                int freedSpace = 0;
                if (index >= 0 && index < Row.ControlsInternal.Count - 1) {
                    ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[index];
                    if (cell.Visible) {
                        freedSpace = cell.Grow(growBy);
                    }
                }
                return freedSpace;                
            }

            public ToolStripPanelCell GetNextVisibleCell(int index, bool forward) {
                if (forward) {
                    for (int i = index; i < Row.Cells.Count; i++) {
                        ToolStripPanelCell cell = Row.Cells[i] as ToolStripPanelCell;
                        if ((cell.Visible || (owner.parent.Visible && cell.ControlInDesignMode)) && cell.ToolStripPanelRow == this.owner) {
                            return cell;
                        }
                    }
                }
                else {
                    for (int i = index; i >=0; i--) {
                        ToolStripPanelCell cell = Row.Cells[i] as ToolStripPanelCell;
                        if ((cell.Visible || (owner.parent.Visible && cell.ControlInDesignMode)) && cell.ToolStripPanelRow == this.owner) {
                            return cell;
                        }
                    }

                }
                return null;
                
            }
          
            /// <devdoc>
            /// grows all controls after the index to be their preferred size.
            /// reports back how much space was used.
            /// </devdoc>
            protected virtual int GrowControlsAfter(int index, int growBy) {
               if (growBy < 0) {
                   Debug.Fail("why was a negative number given to growControlsAfter?");
                   return 0;
               }

               int spaceToFree = growBy;

               for (int i = index + 1; i < Row.ControlsInternal.Count; i++) {
                   // grow the n+1 item first if it was previously shrunk.
                   int freedSpace = Grow(i, spaceToFree);

                   if (freedSpace >= 0) {
                       spaceToFree -= freedSpace;
                       if (spaceToFree <= 0) {
                           return growBy;
                       }
                   }
               }

               return growBy - spaceToFree;
            }

            /// <devdoc>
            /// grows all controls before the index to be their preferred size.
            /// reports back how much space was used.
            /// </devdoc>
            protected virtual int GrowControlsBefore(int index, int growBy) {
               if (growBy < 0) {
                   Debug.Fail("why was a negative number given to growControlsAfter?");
                   return 0;
               }

               int spaceToFree = growBy;

               // grow the n-1 item first if it was previously shrunk.
               for (int i = index - 1; i >= 0; i--) {
                   spaceToFree -= Grow(i, spaceToFree);
                   if (spaceToFree <= 0) {
                       return growBy; // we've already gotten all the free space.
                   }
               }

               return growBy - spaceToFree;
            }

            

            public virtual void MoveControl(ToolStrip movingControl, Point startClientLocation, Point endClientLocation) {
           //     ToolStripPanel.Join(movingControl, endScreenLocation);
            }
            public virtual void LeaveRow(ToolStrip toolStripToDrag) {
            }

            public virtual void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag) {
            }

            protected internal virtual void OnControlAdded(Control c, int index) {
            }

            protected internal virtual void OnControlRemoved(Control c, int index) {
            }

            protected internal virtual void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds) {
            }
        }

        private class HorizontalRowManager : ToolStripPanelRowManager {
            private const int DRAG_BOUNDS_INFLATE = 4;

         
            public HorizontalRowManager(ToolStripPanelRow owner): base (owner) {
                owner.SuspendLayout();
                FlowLayoutSettings.WrapContents = false;
                FlowLayoutSettings.FlowDirection = FlowDirection.LeftToRight;
                owner.ResumeLayout(false);
            }
           
            public override Rectangle DisplayRectangle {
                get {
                    Rectangle displayRect = ((IArrangedElement)Row).DisplayRectangle;

                    if (ToolStripPanel != null) {
                        Rectangle raftingDisplayRectangle = ToolStripPanel.DisplayRectangle;
                  
                        if ((!ToolStripPanel.Visible || LayoutUtils.IsZeroWidthOrHeight(raftingDisplayRectangle)) && (ToolStripPanel.ParentInternal != null)){
                      
                            // if were layed out before we're visible we have the wrong display rectangle, so we need to calculate it.
                            displayRect.Width = ToolStripPanel.ParentInternal.DisplayRectangle.Width - (ToolStripPanel.Margin.Horizontal + ToolStripPanel.Padding.Horizontal) - Row.Margin.Horizontal;
                        }
                        else {
                            displayRect.Width = raftingDisplayRectangle.Width - Row.Margin.Horizontal;
                  
                        }
                    }

                    return displayRect;
                }
            }

            public override Rectangle DragBounds {
                get {
                    Rectangle dragBounds = Row.Bounds;
                    int index = ToolStripPanel.RowsInternal.IndexOf(Row);

                    if (index > 0) {
                        Rectangle previousRowBounds = ToolStripPanel.RowsInternal[index - 1].Bounds;
                        int y = previousRowBounds.Y + previousRowBounds.Height - (previousRowBounds.Height >> 2);

                        dragBounds.Height += dragBounds.Y - y;
                        dragBounds.Y = y;
                    }

                    if (index < ToolStripPanel.RowsInternal.Count - 1) {
                        Rectangle nextRowBounds = ToolStripPanel.RowsInternal[index + 1].Bounds;

                        dragBounds.Height += (nextRowBounds.Height >> 2) + Row.Margin.Bottom + ToolStripPanel.RowsInternal[index + 1].Margin.Top;
                    }

                    dragBounds.Width += Row.Margin.Horizontal + ToolStripPanel.Padding.Horizontal +5;
                    dragBounds.X -= Row.Margin.Left + ToolStripPanel.Padding.Left +4;
                    return dragBounds;
                }
            }
            
            /// <devdoc>
            ///  returns true if there is enough space to "raft" the control
            ///  ow returns false
            /// </devdoc>
            public override bool CanMove(ToolStrip toolStripToDrag) {

                if (base.CanMove(toolStripToDrag)) {
                    Size totalSize = Size.Empty;

                    for (int i = 0; i < Row.ControlsInternal.Count; i++ ){
                        totalSize += Row.GetMinimumSize(Row.ControlsInternal[i] as ToolStrip); 
                    }

                    totalSize += Row.GetMinimumSize(toolStripToDrag as ToolStrip); 
                    return totalSize.Width < DisplayRectangle.Width;
                }
                Debug.WriteLineIf(ToolStripPanelRow.ToolStripPanelRowCreationDebug.TraceVerbose, "HorizontalRM.CanMove returns false - not enough room");              
                return false;
            }
            
            protected internal override int FreeSpaceFromRow(int spaceToFree) {
               int requiredSpace = spaceToFree;
               // take a look at the last guy.  if his right edge exceeds
               // the new bounds, then we should go ahead and push him into view.
             
               if (spaceToFree > 0){
                   // we should shrink the last guy and then move him.
                   ToolStripPanelCell lastCellOnRow = GetNextVisibleCell(Row.Cells.Count-1,  /*forward*/false);
                   if (lastCellOnRow == null) {
                        return 0;
                   }
                   Padding cellMargin = lastCellOnRow.Margin;
    
                   // only check margin.left as we are only concerned with getting right edge of
                   // the toolstrip into view. (space after the fact doesnt count).
                    if (cellMargin.Left >= spaceToFree) {
                       cellMargin.Left -= spaceToFree;
                       cellMargin.Right = 0;
                       spaceToFree = 0;
    
                   }
                   else {
                       spaceToFree -= lastCellOnRow.Margin.Left;  
                       cellMargin.Left = 0;
                       cellMargin.Right = 0;
                   }
                   lastCellOnRow.Margin = cellMargin;
                   
                   // start moving the toolstrips before this guy.
                   spaceToFree -= MoveLeft(Row.Cells.Count -1, spaceToFree);
    
                   if (spaceToFree > 0) {
                       spaceToFree -= lastCellOnRow.Shrink(spaceToFree);
                   }
                }
                return requiredSpace - Math.Max(0,spaceToFree);
           }
 
            public override void MoveControl(ToolStrip movingControl, Point clientStartLocation, Point clientEndLocation) {
                if (Row.Locked) {
                    return;
                }

                if (DragBounds.Contains(clientEndLocation)) {
                    int index = Row.ControlsInternal.IndexOf(movingControl);
                    int deltaX = clientEndLocation.X - clientStartLocation.X;

                    if (deltaX < 0) {
                        // moving to the left
                        MoveLeft(index, deltaX * -1);
                    }
                    else {
                        MoveRight(index, deltaX);
                    }
                }
                else  {
                    base.MoveControl(movingControl, clientStartLocation, clientEndLocation);
                }
            }

            private int MoveLeft(int index, int spaceToFree) {

                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveLeft: " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                int freedSpace = 0;
                          
                Row.SuspendLayout();
                try {
                    if (spaceToFree == 0 || index < 0) {
                        Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveLeft Early EXIT - 0 ");           
                        return 0;
                    }

                    
                    // remove all margins starting from the index.
                    for (int i = index; i >= 0; i--) {
                        ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[i];
                        if (!cell.Visible && !cell.ControlInDesignMode) {
                            continue;
                        }
                        int requiredSpace = spaceToFree - freedSpace;

                        Padding cellMargin = cell.Margin;
                        
                        if (cellMargin.Horizontal >= requiredSpace) {
                            freedSpace += requiredSpace;
                             
                            cellMargin.Left -= requiredSpace;
                            cellMargin.Right = 0;
                            cell.Margin = cellMargin;
                            
                        }
                        else {
                            freedSpace += cell.Margin.Horizontal;
                            cellMargin.Left = 0;
                            cellMargin.Right = 0;
                            cell.Margin = cellMargin;
                        }

                        if (freedSpace >= spaceToFree) {
                            // add the space we freed to the next guy.
                            if (index +1 < Row.Cells.Count) {
                                cell = GetNextVisibleCell(index+1, /*forward*/true);
                                if (cell != null) {
                                    cellMargin = cell.Margin;
                                    cellMargin.Left += spaceToFree;
                                    cell.Margin = cellMargin;
                                }
                            }
                            
                            Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveLeft Recovered (Margin only): " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                            return spaceToFree;
                        }
                    }
                }
                finally {
                    Row.ResumeLayout(true);
                }
                
                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveLeft Recovered Partial (Shrink): " + freedSpace.ToString(CultureInfo.InvariantCulture));
                return freedSpace;
            }

            private int MoveRight(int index, int spaceToFree) {

                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveRight: " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                int freedSpace = 0;     
                Row.SuspendLayout();
                try {
                
                    if (spaceToFree == 0 || index < 0 || index >= Row.ControlsInternal.Count) {
                        Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveRight Early EXIT - 0 ");           
                        return 0;
                    }

                    
                    ToolStripPanelCell cell;
                    Padding cellMargin;

                    // remove all margins after this point in the index.
                    for (int i = index+1; i < Row.Cells.Count; i++) {
                        cell = (ToolStripPanelCell)Row.Cells[i];
                        if (!cell.Visible && !cell.ControlInDesignMode) {
                            continue;
                        }
                        int requiredSpace = spaceToFree - freedSpace;

                        cellMargin = cell.Margin;
                        
                        if (cellMargin.Horizontal >= requiredSpace) {
                            freedSpace += requiredSpace;
                             
                            cellMargin.Left -= requiredSpace;
                            cellMargin.Right = 0;
                            cell.Margin = cellMargin;
                            
                        }
                        else {
                            freedSpace += cell.Margin.Horizontal;
                            cellMargin.Left = 0;
                            cellMargin.Right = 0;
                            cell.Margin = cellMargin;
                        }

                        break;
                    }

                    // add in the space at the end of the row.
                    if (Row.Cells.Count > 0 && (spaceToFree > freedSpace)) {
                        ToolStripPanelCell lastCell = GetNextVisibleCell(Row.Cells.Count -1, /*forward*/false);
                        if (lastCell != null) {
                            freedSpace += DisplayRectangle.Right - lastCell.Bounds.Right;
                        }
                        else {
                            freedSpace += DisplayRectangle.Width;
                        }
                        
                    }


                    // set the margin of the control that's moving.
                    if (spaceToFree <= freedSpace) {
                        // add the space we freed to the first guy.
                        cell = GetNextVisibleCell(index, /*forward*/true);
                        if (cell == null) {
                            cell = Row.Cells[index] as ToolStripPanelCell;
                        }
                        Debug.Assert(cell != null, "Dont expect cell to be null here, what's going on?");

                        if (cell != null) {
                            cellMargin = cell.Margin;
                            cellMargin.Left += spaceToFree;
                            cell.Margin = cellMargin;
                        }
                        Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveRight Recovered (Margin only): " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                        return spaceToFree;
                    }

                    // Now start shrinking.
                    for (int i = index+1; i < Row.Cells.Count; i++) {
                        cell = (ToolStripPanelCell)Row.Cells[i];
                        if (!cell.Visible && !cell.ControlInDesignMode) {
                            continue;
                        }
                        int requiredSpace = spaceToFree - freedSpace;
                        freedSpace += cell.Shrink(requiredSpace);
                        
                         if (spaceToFree >= freedSpace) {
                            Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveRight Recovered (Shrink): " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                            Row.ResumeLayout(true);
                            return spaceToFree;
                         }

                    }

                    if (Row.Cells.Count == 1) {
                        cell = GetNextVisibleCell(index,/*forward*/true);
                        if (cell != null) {
                            cellMargin = cell.Margin;
                            cellMargin.Left += freedSpace;
                            cell.Margin = cellMargin;
                        }
                    }

                }
                finally {
                    Row.ResumeLayout(true);
                }

                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveRight Recovered Partial (Shrink): " + freedSpace.ToString(CultureInfo.InvariantCulture));

                return freedSpace;
            }

            
            public override void LeaveRow(ToolStrip toolStripToDrag) {
                // this code is here to properly add space to the next control when the
                // toolStripToDrag has been removed from the row.
                Row.SuspendLayout();
                int index = Row.ControlsInternal.IndexOf(toolStripToDrag);
                if (index >= 0) {
                    if (index < Row.ControlsInternal.Count -1 /*not the last one in the row*/) {
                        ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[index];
                        if (cell.Visible) {
                            int spaceOccupiedByCell = cell.Margin.Horizontal + cell.Bounds.Width;

                            // add the space occupied by the cell to the next one.
                            ToolStripPanelCell nextCell = GetNextVisibleCell(index+1, /*forward*/true);
                            if (nextCell != null) {
                                Padding nextCellMargin = nextCell.Margin;
                                nextCellMargin.Left += spaceOccupiedByCell;
                                nextCell.Margin = nextCellMargin;
                            }
                        }
                    }
                    // remove the control from the row.
                    ((IList)Row.Cells).RemoveAt(index);
                }
                Row.ResumeLayout(true);
            }
            
            protected internal override void OnControlAdded(Control control, int index) {
            }

            protected internal override void OnControlRemoved(Control control, int index) {
            }

            public override void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag) {

                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "Horizontal JoinRow called " );
                int index;
                

                if (!Row.ControlsInternal.Contains(toolStripToDrag)) {
                    Row.SuspendLayout();
           
                    try {                 
                         if (Row.ControlsInternal.Count > 0) {
                            
                            // walk through the columns and determine which column you want to insert into.
                            for (index = 0; index < Row.Cells.Count; index++) {
                                ToolStripPanelCell cell = Row.Cells[index] as ToolStripPanelCell;
                                if (!cell.Visible && !cell.ControlInDesignMode) {
                                    continue;
                                }
                                
                                //  [:   ]  [: x  ]
                                if (Row.Cells[index].Bounds.Contains(locationToDrag)) {
                                    break;
                                }

                                // take into account the following scenarios
                                //  [:   ]  x [:   ]
                                // x [:  ]    [:   ]
                                if (Row.Cells[index].Bounds.X >= locationToDrag.X) {
                                    break;
                                }

                            }

                            Control controlToPushAside = Row.ControlsInternal[index];
                            // Plop the new control in the midst of the row in question.
                            if (index < Row.ControlsInternal.Count) {
                                Row.ControlsInternal.Insert(index, toolStripToDrag);
                            }
                            else {
                                Row.ControlsInternal.Add(toolStripToDrag);
                            }

                            // since layout is suspended the control may not be set to its preferred size yet
                            int controlToDragWidth = (toolStripToDrag.AutoSize) ? toolStripToDrag.PreferredSize.Width : toolStripToDrag.Width;
                            
                    
                            //
                            // now make it look like it belongs in the row.
                            //
                            // PUSH the controls after it to the right
                            
                            int requiredSpace = controlToDragWidth;
                            if (index == 0) {
                                // make sure we account for the left side
                                requiredSpace += locationToDrag.X;
                            }
                            int freedSpace = 0;
                            
                            if (index < Row.ControlsInternal.Count -1) {
                                ToolStripPanelCell nextCell = (ToolStripPanelCell)Row.Cells[index+1];
                                Padding nextCellMargin =  nextCell.Margin;
                    
                                // if we've already got the empty space
                                // (available to us via the margin) use that.
                                if (nextCellMargin.Left > requiredSpace) {
                                    nextCellMargin.Left -= requiredSpace;
                                    nextCell.Margin = nextCellMargin;
                                    freedSpace = requiredSpace;
                                }
                                else {
                                    // otherwise we've got to 
                                    // push all controls after this point to the right 
                                    // this dumps the extra stuff into the margin of index+1
                                    freedSpace = MoveRight(index+1, requiredSpace - freedSpace);
                    
                                    // refetch the margin for "index+1" and remove the freed space 
                                    // from it - we want to actually put this to use on the control
                                    // before this one - we're making room for the control at 
                                    // position "index"
                                    if (freedSpace > 0) {
                                        nextCellMargin =  nextCell.Margin;
                                        nextCellMargin.Left = Math.Max(0, nextCellMargin.Left - freedSpace);
                                        nextCell.Margin = nextCellMargin; 
                                    }
                               
                                }
                                
                    
                            }
                            else {
                                // we're adding to the end.
                                ToolStripPanelCell nextCell = GetNextVisibleCell(Row.Cells.Count-2,  /*forward*/false);
                                ToolStripPanelCell lastCell = GetNextVisibleCell(Row.Cells.Count-1,  /*forward*/false);
                           
                                // count the stuff at the end of the row as freed space
                                if (nextCell != null && lastCell != null) {
                                    Padding lastCellMargin = lastCell.Margin;
                                    lastCellMargin.Left = Math.Max(0,locationToDrag.X - nextCell.Bounds.Right);
                                    lastCell.Margin = lastCellMargin;
                                    freedSpace=requiredSpace;
                                }                            
                            }
                            
                            // If we still need more space, then...
                            // PUSH the controls before it to the left
                            if (freedSpace < requiredSpace && index > 0) {
                                freedSpace = MoveLeft(index - 1, requiredSpace - freedSpace);
                            }
                    
                            if (index == 0) {
                                // if the index is zero and there were controls in the row
                                // we need to take care of pushing over the new cell.
                                if (freedSpace - controlToDragWidth > 0) {
                                    ToolStripPanelCell newCell = Row.Cells[index] as ToolStripPanelCell;
                                    Padding newCellMargin =  newCell.Margin;
                                    newCellMargin.Left = freedSpace - controlToDragWidth;
                                    newCell.Margin = newCellMargin;
                                }
                            }
                                
                                
                            
                           
                        }
                        else {
                            
                            // we're adding to the beginning.
                            Row.ControlsInternal.Add(toolStripToDrag);
  
#if DEBUG
                            ISupportToolStripPanel ctg = toolStripToDrag as ISupportToolStripPanel;
                            ToolStripPanelRow newPanelRow = ctg.ToolStripPanelRow;
                            Debug.Assert(newPanelRow == Row, "we should now be in the new panel row.");
#endif
                            if (Row.Cells.Count >0 || toolStripToDrag.IsInDesignMode) {                            
                                // we're adding to the beginning.
                                ToolStripPanelCell cell = GetNextVisibleCell(Row.Cells.Count-1, /*forward*/false);
                                if (cell == null && toolStripToDrag.IsInDesignMode) {
                                    cell = (ToolStripPanelCell)Row.Cells[Row.Cells.Count-1];
                                }

                                if (cell != null) {
                                    Padding cellMargin = cell.Margin;
                                    cellMargin.Left = Math.Max(0,locationToDrag.X-Row.Margin.Left);
                                    cell.Margin = cellMargin;
                                }
                            }

                            
                          
                        }
                    }
                    finally {
                        Row.ResumeLayout(true);
                    }
                }                    
            }

            protected internal override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds) {
               base.OnBoundsChanged(oldBounds, newBounds);
            }

            
            
        }
   
        private class VerticalRowManager : ToolStripPanelRowManager {
            
            private const int DRAG_BOUNDS_INFLATE = 4;
          
            public VerticalRowManager(ToolStripPanelRow owner): base (owner) {
                owner.SuspendLayout();
                FlowLayoutSettings.WrapContents = false;
                FlowLayoutSettings.FlowDirection = FlowDirection.TopDown;
                owner.ResumeLayout(false);
            }
            

            public override Rectangle DisplayRectangle {
                get {
                    Rectangle displayRect = ((IArrangedElement)Row).DisplayRectangle;

                    if (ToolStripPanel != null) {
                        Rectangle raftingDisplayRectangle = ToolStripPanel.DisplayRectangle;
                        
                        if ((!ToolStripPanel.Visible || LayoutUtils.IsZeroWidthOrHeight(raftingDisplayRectangle)) && (ToolStripPanel.ParentInternal != null)){
                            // if were layed out before we're visible we have the wrong display rectangle, so we need to calculate it.
                            displayRect.Height = ToolStripPanel.ParentInternal.DisplayRectangle.Height - (ToolStripPanel.Margin.Vertical + ToolStripPanel.Padding.Vertical) - Row.Margin.Vertical;
                        }
                        else {
                            displayRect.Height = raftingDisplayRectangle.Height - Row.Margin.Vertical;
                        }
                    }

                    return displayRect;
                }
            }
            public override Rectangle DragBounds {
                get {
                    Rectangle dragBounds = Row.Bounds;
                    int index = ToolStripPanel.RowsInternal.IndexOf(Row);

                    /// 
                    if (index > 0) {
                        Rectangle previousRowBounds = ToolStripPanel.RowsInternal[index - 1].Bounds;
                        int x = previousRowBounds.X + previousRowBounds.Width - (previousRowBounds.Width >> 2);

                        dragBounds.Width += dragBounds.X - x;
                        dragBounds.X = x;
                    }

                    if (index < ToolStripPanel.RowsInternal.Count - 1) {
                        Rectangle nextRowBounds = ToolStripPanel.RowsInternal[index + 1].Bounds;

                        dragBounds.Width += (nextRowBounds.Width >> 2) + Row.Margin.Right + ToolStripPanel.RowsInternal[index + 1].Margin.Left;
                    }

                    dragBounds.Height += Row.Margin.Vertical + ToolStripPanel.Padding.Vertical +5;
                    dragBounds.Y -= Row.Margin.Top+ ToolStripPanel.Padding.Top +4;


                    return dragBounds;
                }
            }

            /// <devdoc>
            ///  returns true if there is enough space to "raft" the control
            ///  ow returns false
            /// </devdoc>
            public override bool CanMove(ToolStrip toolStripToDrag) {

                 if (base.CanMove(toolStripToDrag)) {
                    Size totalSize = Size.Empty;

                    for (int i = 0; i < Row.ControlsInternal.Count; i++ ){
                        totalSize += Row.GetMinimumSize(Row.ControlsInternal[i] as ToolStrip); 
                    }

                    totalSize += Row.GetMinimumSize(toolStripToDrag);
                    return totalSize.Height < DisplayRectangle.Height;
                }
             
                Debug.WriteLineIf(ToolStripPanelRow.ToolStripPanelRowCreationDebug.TraceVerbose, "VerticalRM.CanMove returns false - not enough room");              
                return false;
            }
            protected internal override int FreeSpaceFromRow(int spaceToFree) {
                  int requiredSpace = spaceToFree;
                  // take a look at the last guy.  if his right edge exceeds
                  // the new bounds, then we should go ahead and push him into view.
                
                  if (spaceToFree > 0){
                      // we should shrink the last guy and then move him.
                      ToolStripPanelCell lastCellOnRow = GetNextVisibleCell(Row.Cells.Count-1,  /*forward*/false);
                      if (lastCellOnRow == null) {
                         return 0;
                      }
                      Padding cellMargin = lastCellOnRow.Margin;
            
                      // only check margin.left as we are only concerned with getting right edge of
                      // the toolstrip into view. (space after the fact doesnt count).
                       if (cellMargin.Top >= spaceToFree) {
                          cellMargin.Top -= spaceToFree;
                          cellMargin.Bottom = 0;
                          spaceToFree = 0;
            
                      }
                      else {
                          spaceToFree -= lastCellOnRow.Margin.Top;  
                          cellMargin.Top = 0;
                          cellMargin.Bottom = 0;
                      }
                      lastCellOnRow.Margin = cellMargin;
                      
                      // start moving the toolstrips before this guy.
                      spaceToFree -= MoveUp(Row.Cells.Count -1, spaceToFree);
            
                      if (spaceToFree > 0) {
                          spaceToFree -= lastCellOnRow.Shrink(spaceToFree);
                      }
                   }
                   return requiredSpace - Math.Max(0,spaceToFree);
            }

            
           
            public override void MoveControl(ToolStrip movingControl, Point clientStartLocation, Point clientEndLocation) {

                if (Row.Locked) {
                    return;
                }
                if (DragBounds.Contains(clientEndLocation)) {
                   int index = Row.ControlsInternal.IndexOf(movingControl);
                   int deltaY = clientEndLocation.Y - clientStartLocation.Y;

                   if (deltaY < 0) {
                       // moving to the left
                       MoveUp(index, deltaY * -1);
                   }
                   else {
                       MoveDown(index, deltaY);
                   }
               }
               else {
                    base.MoveControl(movingControl, clientStartLocation, clientEndLocation);
               }
            }



            private int MoveUp(int index, int spaceToFree) {
                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveUp: " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                int freedSpace = 0;
                          
                Row.SuspendLayout();
                try {
                    if (spaceToFree == 0 || index < 0) {
                        Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveUp Early EXIT - 0 ");           
                        return 0;
                    }

                    
                    // remove all margins starting from the index.
                    for (int i = index; i >= 0; i--) {
                        ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[i];
                        if (!cell.Visible && !cell.ControlInDesignMode) {
                            continue;
                        }
                        int requiredSpace = spaceToFree - freedSpace;

                        Padding cellMargin = cell.Margin;
                        
                        if (cellMargin.Vertical >= requiredSpace) {
                            freedSpace += requiredSpace;
                             
                            cellMargin.Top -= requiredSpace;
                            cellMargin.Bottom = 0;
                            cell.Margin = cellMargin;
                            
                        }
                        else {
                            freedSpace += cell.Margin.Vertical;
                            cellMargin.Top = 0;
                            cellMargin.Bottom = 0;
                            cell.Margin = cellMargin;
                        }

                        if (freedSpace >= spaceToFree) {
                            // add the space we freed to the next guy.
                            if (index +1 < Row.Cells.Count) {
                                cell = GetNextVisibleCell(index+1, /*forward*/true);
                                if (cell != null) {
                                    cellMargin = cell.Margin;
                                    cellMargin.Top += spaceToFree;
                                    cell.Margin = cellMargin;
                                }
                            }
                            
                            Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveUp Recovered (Margin only): " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                            return spaceToFree;
                        }
                    }
                }
                finally {
                    Row.ResumeLayout(true);
                }
                
                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveLeft Recovered Partial (Shrink): " + freedSpace.ToString(CultureInfo.InvariantCulture));

                return freedSpace;
            }

            private int MoveDown(int index, int spaceToFree) {
            
                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveDown: " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                int freedSpace = 0;     
                Row.SuspendLayout();
                try {
                
                    if (spaceToFree == 0 || index < 0 || index >= Row.ControlsInternal.Count) {
                        Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveDown Early EXIT - 0 ");           
                        return 0;
                    }

                    
                    ToolStripPanelCell cell;
                    Padding cellMargin;

                    // remove all margins after this point in the index.
                    for (int i = index+1; i < Row.Cells.Count; i++) {
                        cell = (ToolStripPanelCell)Row.Cells[i];
                        if (!cell.Visible && !cell.ControlInDesignMode) {
                            continue;
                        }

                        int requiredSpace = spaceToFree - freedSpace;

                        cellMargin = cell.Margin;
                        
                        if (cellMargin.Vertical >= requiredSpace) {
                            freedSpace += requiredSpace;
                             
                            cellMargin.Top -= requiredSpace;
                            cellMargin.Bottom = 0;
                            cell.Margin = cellMargin;
                            
                        }
                        else {
                            freedSpace += cell.Margin.Vertical;
                            cellMargin.Top = 0;
                            cellMargin.Bottom = 0;
                            cell.Margin = cellMargin;
                        }

                        break;
                    }

                    // add in the space at the end of the row.
                    if (Row.Cells.Count > 0 && (spaceToFree > freedSpace)) {
                        ToolStripPanelCell lastCell = GetNextVisibleCell(Row.Cells.Count -1, /*forward*/false);
                        if (lastCell != null) {
                            freedSpace += DisplayRectangle.Bottom - lastCell.Bounds.Bottom;
                        }
                        else { 
                            freedSpace += DisplayRectangle.Height;
                        }
                    }

                    // set the margin of the control that's moving.
                    if (spaceToFree <= freedSpace) {
                        // add the space we freed to the first guy.
                        cell = (ToolStripPanelCell)Row.Cells[index];
                        cellMargin = cell.Margin;
                        cellMargin.Top += spaceToFree;
                        cell.Margin = cellMargin;
                        
                        Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveDown Recovered (Margin only): " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                        return spaceToFree;
                    }

                    // Now start shrinking.
                    for (int i = index+1; i < Row.Cells.Count; i++) {
                        cell = (ToolStripPanelCell)Row.Cells[i];
                        if (!cell.Visible && !cell.ControlInDesignMode) {
                            continue;
                        }                       
                        int requiredSpace = spaceToFree - freedSpace;
                        freedSpace += cell.Shrink(requiredSpace);
                        
                         if (spaceToFree >= freedSpace) {
                            Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveDown Recovered (Shrink): " + spaceToFree.ToString(CultureInfo.InvariantCulture));
                            Row.ResumeLayout(true);
                            return spaceToFree;
                         }

                    }

                    if (Row.Cells.Count == 1) {
                        cell = GetNextVisibleCell(index,/*forward*/true);
                        if (cell != null) {
                            cellMargin = cell.Margin;
                            cellMargin.Top += freedSpace;
                            cell.Margin = cellMargin;
                        }
                    }

                }
                finally {
                    Row.ResumeLayout(true);
                }

                int recoveredSpace = spaceToFree - freedSpace;
                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "MoveDown Recovered Partial (Shrink): " + recoveredSpace.ToString(CultureInfo.InvariantCulture));
                    
                return recoveredSpace;
            }

            protected internal override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds) {
    
                base.OnBoundsChanged(oldBounds, newBounds);
                
                // if our bounds have changed - we should shove the toolbars up so they're in view.
                if (Row.Cells.Count > 0) {
    
                    // take a look at the last guy.  if his right edge exceeds
                    // the new bounds, then we should go ahead and push him into view.
                    ToolStripPanelCell lastCell = GetNextVisibleCell(Row.Cells.Count -1, /*forward=*/false);
                    int spaceToFree = (lastCell != null)?  lastCell.Bounds.Bottom - newBounds.Height : 0;
                  
                    if (spaceToFree > 0){
                        // we should shrink the last guy and then move him.
                        ToolStripPanelCell lastCellOnRow = GetNextVisibleCell(Row.Cells.Count-1,  /*forward*/false);
                        
                        Padding cellMargin = lastCellOnRow.Margin;
    
                        // only check margin.left as we are only concerned with getting bottom edge of
                        // the toolstrip into view. (space after the fact doesnt count).
                         if (cellMargin.Top >= spaceToFree) {
                             
                            cellMargin.Top -= spaceToFree;
                            cellMargin.Bottom = 0;
                            lastCellOnRow.Margin = cellMargin;
                            spaceToFree = 0;
    
                        }
                        else {
                            spaceToFree -= lastCellOnRow.Margin.Top;  
                            cellMargin.Top = 0;
                            cellMargin.Bottom = 0;
                            lastCellOnRow.Margin = cellMargin;
                        }
                        spaceToFree -= lastCellOnRow.Shrink(spaceToFree);
                        // start moving the toolstrips before this guy.
                        MoveUp(Row.Cells.Count -1, spaceToFree);
                     }
                   
                }
                
            }
    
         
    

            protected internal override void OnControlRemoved(Control c, int index) {
            
            }

            protected internal override void OnControlAdded(Control control, int index) {
             
            }

      
            public override void JoinRow(ToolStrip toolStripToDrag, Point locationToDrag) {
            
                Debug.WriteLineIf(ToolStripPanelMouseDebug.TraceVerbose, "Vertical JoinRow called " );
                int index;
            
                if (!Row.ControlsInternal.Contains(toolStripToDrag)) {
                    Row.SuspendLayout();
                    try {
                        if (Row.ControlsInternal.Count > 0) {
            
                            
                            // walk through the columns and determine which column you want to insert into.
                            for (index = 0; index < Row.Cells.Count; index++) {
                                ToolStripPanelCell cell = Row.Cells[index] as ToolStripPanelCell;
                                if (!cell.Visible && !cell.ControlInDesignMode) {
                                    continue;
                                }
                                //  [:   ]  [: x  ]
                                if (cell.Bounds.Contains(locationToDrag)) {
                                    break;
                                }
            
                                // take into account the following scenarios
                                //  [:   ]  x [:   ]
                                // x [:  ]    [:   ]                
                                if (cell.Bounds.Y >= locationToDrag.Y) {
                                    break;
                                }
            
                            }
            
                            Control controlToPushAside = Row.ControlsInternal[index];
                            // Plop the new control in the midst of the row in question.
                            if (index < Row.ControlsInternal.Count) {
                                Row.ControlsInternal.Insert(index, toolStripToDrag);
                            }
                            else {
                                Row.ControlsInternal.Add(toolStripToDrag);
                            }
                            
                            // since layout is suspended the control may not be set to its preferred size yet
                            int controlToDragWidth = (toolStripToDrag.AutoSize) ? toolStripToDrag.PreferredSize.Height : toolStripToDrag.Height;
        
                            //
                            // now make it look like it belongs in the row.
                            //
                            // PUSH the controls after it to the right
                            
                            int requiredSpace = controlToDragWidth;
                            
                            if (index == 0) {
                                // make sure we account for the left side
                                requiredSpace += locationToDrag.Y;
                            }
                            int freedSpace = 0;
                            
                            if (index < Row.ControlsInternal.Count -1) {
                                
                                ToolStripPanelCell nextCell = GetNextVisibleCell(index+1,  /*forward*/true);
                                if (nextCell != null) {
                                    Padding nextCellMargin =  nextCell.Margin;
                
                                    // if we've already got the empty space
                                    // (available to us via the margin) use that.
                                    if (nextCellMargin.Top > requiredSpace) {
                                        nextCellMargin.Top -= requiredSpace;
                                        nextCell.Margin = nextCellMargin;                                         
                                        freedSpace = requiredSpace;
                                    }
                                    else {
                                        // otherwise we've got to 
                                        // push all controls after this point to the right 
                                        // this dumps the extra stuff into the margin of index+1
                                        freedSpace = MoveDown(index+1, requiredSpace - freedSpace);
                
                                        // refetch the margin for "index+1" and remove the freed space 
                                        // from it - we want to actually put this to use on the control
                                        // before this one - we're making room for the control at 
                                        // position "index"
                                        if (freedSpace > 0) {
                                            nextCellMargin =  nextCell.Margin;
                                            nextCellMargin.Top -= freedSpace;
                                            nextCell.Margin = nextCellMargin; 
                                        }
                                   
                                    }
                                }
                                
                    
                            }
                            else {
                                 // we're adding to the end.
                                ToolStripPanelCell nextCell = GetNextVisibleCell(Row.Cells.Count-2,  /*forward*/false);
                                ToolStripPanelCell lastCell = GetNextVisibleCell(Row.Cells.Count-1,  /*forward*/false);
                           
                                // count the stuff at the end of the row as freed space
                                if (nextCell != null && lastCell != null) {
                                    Padding lastCellMargin =  lastCell.Margin;
                                    lastCellMargin.Top = Math.Max(0,locationToDrag.Y - nextCell.Bounds.Bottom);
                                    lastCell.Margin = lastCellMargin;
                                    freedSpace=requiredSpace;
                                }     
                                           
                            }
                            
                            // If we still need more space, then...
                            // PUSH the controls before it to the left
                            if (freedSpace < requiredSpace && index > 0) {
                                freedSpace = MoveUp(index - 1, requiredSpace - freedSpace);
                            }

                            
                            if (index == 0) {
                                // if the index is zero and there were controls in the row
                                // we need to take care of pushing over the new cell.
                                if (freedSpace - controlToDragWidth > 0) {
                                    ToolStripPanelCell newCell = Row.Cells[index] as ToolStripPanelCell;
                                    Padding newCellMargin =  newCell.Margin;
                                    newCellMargin.Top = freedSpace - controlToDragWidth;
                                    newCell.Margin = newCellMargin;
                                }
                            }
                           
                        }
                        else {
                            
                            // we're adding to the beginning.
                            Row.ControlsInternal.Add(toolStripToDrag);

#if DEBUG
                            ISupportToolStripPanel ctg = toolStripToDrag as ISupportToolStripPanel;
                            ToolStripPanelRow newPanelRow = ctg.ToolStripPanelRow;
                            Debug.Assert(newPanelRow == Row, "we should now be in the new panel row.");
#endif
                            if (Row.Cells.Count >0) {                            
                                ToolStripPanelCell cell = GetNextVisibleCell(Row.Cells.Count-1, /*forward*/false);
                                if (cell != null) {
                                    Padding cellMargin = cell.Margin;
                                    cellMargin.Top = Math.Max(0,locationToDrag.Y-Row.Margin.Top);
                                    cell.Margin = cellMargin;
                                }
                            }
                        }
                    }
                    finally {
                        Row.ResumeLayout(true);
                    }
                }
            }
        
            public override void LeaveRow(ToolStrip toolStripToDrag) {
                // this code is here to properly add space to the next control when the
                // toolStripToDrag has been removed from the row.
                Row.SuspendLayout();
                int index = Row.ControlsInternal.IndexOf(toolStripToDrag);
                if (index >= 0) {
                    if (index < Row.ControlsInternal.Count -1 /*not the last one in the row*/) {
                
                        ToolStripPanelCell cell = (ToolStripPanelCell)Row.Cells[index];
                        if (cell.Visible) {
                            int spaceOccupiedByCell = cell.Margin.Vertical + cell.Bounds.Height;
                
                            // add the space occupied by the cell to the next one.
                            ToolStripPanelCell nextCell = GetNextVisibleCell(index+1, /*forward*/true);
                            if (nextCell != null) {
                                Padding nextCellMargin = nextCell.Margin;
                                nextCellMargin.Top += spaceOccupiedByCell;
                                nextCell.Margin = nextCellMargin;
                            }
                        }
                    }
                    // remove the control from the row.
                    ((IList)Row.Cells).RemoveAt(index);
                }
                Row.ResumeLayout(true);
            }
        }
      
        

        /// <devdoc>
        /// ToolStripPanelRowControlCollection
        ///
        /// this class represents the collection of controls on a particular row.
        /// when you add and remove controls from this collection - you also add and remove
        /// controls to and from the ToolStripPanel.Control's collection (which happens 
        /// to be externally readonly.)
        /// 
        /// This class is used to represent the IArrangedElement.Children for the ToolStripPanelRow -
        /// which means that this collection represents the IArrangedElements to layout for 
        /// a particular ToolStripPanelRow.
        ///
        /// We need to keep copies of the controls in both the ToolStripPanelRowControlCollection and
        /// the ToolStripPanel.Control collection  as the ToolStripPanel.Control collection
        /// is responsible for parenting and unparenting the controls (ToolStripPanelRows do NOT derive from 
        /// Control and thus are NOT hwnd backed).
        /// </devdoc>
        /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection"]/*' />
        internal class ToolStripPanelRowControlCollection : ArrangedElementCollection, IList, IEnumerable {
            private ToolStripPanelRow owner;
            private ArrangedElementCollection cellCollection;
            
            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.ToolStripPanelRowControlCollection"]/*' />
            public ToolStripPanelRowControlCollection(ToolStripPanelRow owner) {
                this.owner = owner;
            }


            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.ToolStripPanelRowControlCollection1"]/*' />
            public ToolStripPanelRowControlCollection(ToolStripPanelRow owner, Control[] value) {
                this.owner = owner;
                AddRange(value);
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.this"]/*' />
            public new virtual Control this[int index] {
                get {
                    return GetControl(index);
                }
            }

            public ArrangedElementCollection Cells {
                get { 
                    if (cellCollection == null) {
                        cellCollection = new ArrangedElementCollection(InnerList);
                    }
                    return cellCollection;
                }
            }
    

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.ToolStripPanel"]/*' />
            public ToolStripPanel ToolStripPanel {
                get {
                    return owner.ToolStripPanel;
                }
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.Add"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int Add(Control value) {
                ISupportToolStripPanel control = value as ISupportToolStripPanel;
                
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                if (control == null) {
                    throw new NotSupportedException(string.Format(SR.TypedControlCollectionShouldBeOfType, typeof(ToolStrip).Name));                 
                }

                int index = InnerList.Add(control.ToolStripPanelCell);

                OnAdd(control, index);
                return index;
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.AddRange"]/*' />
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void AddRange(Control[] value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                ToolStripPanel currentOwner = ToolStripPanel;

                if (currentOwner != null) {
                    currentOwner.SuspendLayout();
                }

                try {
                    for (int i = 0; i < value.Length; i++) {
                        this.Add(value[i]);
                    }
                }
                finally {
                    if (currentOwner != null) {
                        currentOwner.ResumeLayout();
                    }
                }
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.Contains"]/*' />
            public bool Contains(Control value) {
                for (int i = 0; i < Count; i++) {
                    if (GetControl(i) == value) {
                        return true;
                    }
                }
                return false;
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.Clear"]/*' />
            public virtual void Clear() {
                if (owner != null) {
                    ToolStripPanel.SuspendLayout();
                }

                try {
                    while (Count != 0) {
                        RemoveAt(Count - 1);
                    }
                }
                finally {
                    if (owner != null) {
                        ToolStripPanel.ResumeLayout();
                    }
                }
            }

            public override IEnumerator GetEnumerator() { return new ToolStripPanelCellToControlEnumerator(InnerList); }

            private Control GetControl(int index) {
                Control control = null;
                ToolStripPanelCell cell = null;

                if (index < Count && index >= 0) {
                    cell  = (ToolStripPanelCell)(InnerList[index]);
                    control = (cell != null) ? cell.Control : null;
                }
                return control;

            }
            private int IndexOfControl(Control c) {
                for (int i = 0; i < Count; i++) {
                    ToolStripPanelCell cell  = (ToolStripPanelCell)(InnerList[i]);
                    if (cell.Control == c) {
                        return i;
                    }
                }
                return -1;
                
            }
           
           
                

            void IList.Clear() { Clear(); }

            bool IList.IsFixedSize { get { return InnerList.IsFixedSize; } }

            bool IList.Contains(object value) { return InnerList.Contains(value); }

            bool IList.IsReadOnly { get { return InnerList.IsReadOnly; } }

            void IList.RemoveAt(int index) { RemoveAt(index); }

            void IList.Remove(object value) { Remove(value as Control); }

            int IList.Add(object value) { return Add(value as Control); }

            int IList.IndexOf(object value) { return IndexOf(value as Control); }

            void IList.Insert(int index, object value) { Insert(index, value as Control); }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.IndexOf"]/*' />
            public int IndexOf(Control value) {
                for (int i = 0; i < Count; i++) {
                    if (GetControl(i) == value) {
                        return i;
                    }
                }
                return -1;
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.Insert"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Insert(int index, Control value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                ISupportToolStripPanel control = value as ISupportToolStripPanel;
                if (control == null) {
                    throw new NotSupportedException(string.Format(SR.TypedControlCollectionShouldBeOfType, typeof(ToolStrip).Name));
                }

                InnerList.Insert(index, control.ToolStripPanelCell);
                OnAdd(control, index);
            }

            /// <devdoc>
            ///  Do proper cleanup of ownership, etc.
            /// </devdoc>
            private void OnAfterRemove(Control control, int index) {
                if (owner != null) {
                    // unfortunately we dont know the index of the control in the ToolStripPanel's 
                    // control collection, as all rows share this collection.
                    // To unparent this control we need to use Remove instead  of RemoveAt.
                    using (LayoutTransaction t = new LayoutTransaction(ToolStripPanel, control, PropertyNames.Parent)) {
                        owner.ToolStripPanel.Controls.Remove(control);
                        owner.OnControlRemoved(control, index);
                    }
                }
            }

            private void OnAdd(ISupportToolStripPanel controlToBeDragged, int index) {
                if (owner != null) {
                    LayoutTransaction layoutTransaction = null;
                    if (ToolStripPanel != null && ToolStripPanel.ParentInternal != null) {
                        layoutTransaction = new LayoutTransaction(ToolStripPanel, ToolStripPanel.ParentInternal, PropertyNames.Parent);

                    }
                    try {

                        if (controlToBeDragged != null) {
                            
                            controlToBeDragged.ToolStripPanelRow = owner;

                            Control control = controlToBeDragged as Control;

                            if (control != null) {
                                control.ParentInternal = owner.ToolStripPanel;
                                owner.OnControlAdded(control, index);
                            }
                        }                      
                         
                    } 
                    finally {
                        if (layoutTransaction != null) {
                            layoutTransaction.Dispose();
                        }
                    }

                }
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.Remove"]/*' />
            
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Remove(Control value) {
                int index = IndexOfControl(value);
                RemoveAt(index);
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.RemoveAt"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void RemoveAt(int index) {
                if (index >= 0 && index < Count) {
                    Control control = GetControl(index);
                    ToolStripPanelCell cell = InnerList[index] as ToolStripPanelCell;
                    InnerList.RemoveAt(index);
                    OnAfterRemove(control, index);
                }
            }

            /// <include file='doc\ToolStripPanelRow.uex' path='docs/doc[@for="ToolStripPanelRowControlCollection.CopyTo"]/*' />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void CopyTo(Control[] array, int index) {
                if (array == null) {
                    throw new ArgumentNullException(nameof(array));
                }

                if (index < 0) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index >= array.Length || InnerList.Count > array.Length - index) {
                    throw new ArgumentException(SR.ToolStripPanelRowControlCollectionIncorrectIndexLength);
                }

                for (int i = 0; i < InnerList.Count; i++) {
                    array[index++] = GetControl(i);                  
                }                
            }
            
            /// We want to pretend like we're only holding controls... so everywhere we've returned controls.
            /// but the problem is if you do a foreach, you'll get the cells not the controls.  So we've got
            /// to sort of write a wrapper class around the ArrayList enumerator.
            private class ToolStripPanelCellToControlEnumerator : IEnumerator, ICloneable {                            
                private IEnumerator arrayListEnumerator; 

                internal ToolStripPanelCellToControlEnumerator(ArrayList list) {
                    arrayListEnumerator = ((IEnumerable)list).GetEnumerator();
                }
           
                public virtual Object Current {
                    get {
                        ToolStripPanelCell cell = arrayListEnumerator.Current as ToolStripPanelCell;
                        Debug.Assert(cell != null, "Expected ToolStripPanel cells only!!!" + arrayListEnumerator.Current.GetType().ToString());
                        return (cell == null) ? null : cell.Control;
                    }
                }
            
            
                public Object Clone() {
                    return MemberwiseClone();
                }
            
                public virtual bool MoveNext() {
                    return arrayListEnumerator.MoveNext();
                }
            
                public virtual void Reset() {
                    arrayListEnumerator.Reset();
                }
            }
        }
    }
}

