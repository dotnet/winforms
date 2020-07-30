// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    [Designer("System.Windows.Forms.Design.ToolStripPanelDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxBitmap(typeof(ToolStripPanel), "ToolStripPanel_standalone")]
    public class ToolStripPanel : ContainerControl, IArrangedElement
    {
        private Orientation _orientation = Orientation.Horizontal;
        private static readonly Padding s_rowMargin = new Padding(3, 0, 0, 0);
        private Padding _scaledRowMargin = s_rowMargin;
        private ToolStripRendererSwitcher _rendererSwitcher;
        private BitVector32 _state;
        private readonly ToolStripContainer owner;

#if DEBUG
        internal static TraceSwitch s_toolStripPanelDebug = new TraceSwitch("ToolStripPanelDebug", "Debug code for rafting mouse movement");
        internal static TraceSwitch s_toolStripPanelFeedbackDebug = new TraceSwitch("ToolStripPanelFeedbackDebug", "Debug code for rafting feedback");
        internal static TraceSwitch s_toolStripPanelMissingRowDebug = new TraceSwitch("ToolStripPanelMissingRowDebug", "Debug code for rafting feedback");
#else
        internal static TraceSwitch s_toolStripPanelDebug;
        internal static TraceSwitch s_toolStripPanelFeedbackDebug;
        internal static TraceSwitch s_toolStripPanelMissingRowDebug;
#endif

        private static readonly int s_propToolStripPanelRowCollection = PropertyStore.CreateKey();

        private static readonly int s_stateLocked = BitVector32.CreateMask();
        private static readonly int s_stateBeginInit = BitVector32.CreateMask(s_stateLocked);
        private static readonly int s_stateChangingZOrder = BitVector32.CreateMask(s_stateBeginInit);
        private static readonly int s_stateInJoin = BitVector32.CreateMask(s_stateChangingZOrder);
        private static readonly int s_stateEndInit = BitVector32.CreateMask(s_stateInJoin);
        private static readonly int s_stateLayoutSuspended = BitVector32.CreateMask(s_stateEndInit);
        private static readonly int s_stateRightToLeftChanged = BitVector32.CreateMask(s_stateLayoutSuspended);

        internal static readonly Padding s_dragMargin = new Padding(10);

        private static readonly object s_eventRendererChanged = new object();

        public ToolStripPanel()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledRowMargin = DpiHelper.LogicalToDeviceUnits(s_rowMargin);
            }

            SuspendLayout();
            AutoScaleMode = AutoScaleMode.None;
            InitFlowLayout();
            AutoSize = true;
            MinimumSize = Size.Empty; // consider 1,1
            _state[s_stateLocked | s_stateBeginInit | s_stateChangingZOrder] = false;
            TabStop = false;

            ToolStripManager.ToolStripPanels.Add(this);
            // not setting ControlStyles.AllPaintingInWmPaint as we don't do any painting in OnPaint... all
            // is done in OnPaintBackground... so its better to show the rafting container in WM_ERASEBACKGROUND.
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | /*ControlStyles.AllPaintingInWmPaint |*/ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            ResumeLayout(true);
        }

        internal ToolStripPanel(ToolStripContainer owner)
            : this()
        {
            this.owner = owner;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AllowDrop
        {
            get => base.AllowDrop;
            set => base.AllowDrop = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoScroll
        {
            get => base.AutoScroll;
            set => base.AutoScroll = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size AutoScrollMargin
        {
            get => base.AutoScrollMargin;
            set => base.AutoScrollMargin = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size AutoScrollMinSize
        {
            get => base.AutoScrollMinSize;
            set => base.AutoScrollMinSize = value;
        }

        [    DefaultValue(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        ///  Override base AutoSizeChanged to we can change visibility/browsability attributes
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        protected override Padding DefaultPadding
        {
            get { return Padding.Empty; }
        }
        protected override Padding DefaultMargin
        {
            get { return Padding.Empty; }
        }

        public Padding RowMargin
        {
            get { return _scaledRowMargin; }
            set
            {
                _scaledRowMargin = value;
                LayoutTransaction.DoLayout(this, this, "RowMargin");
            }
        }

        public override DockStyle Dock
        {
            get => base.Dock;
            set
            {
                base.Dock = value;

                if (value == DockStyle.Left || value == DockStyle.Right)
                {
                    Orientation = Orientation.Vertical;
                }
                else
                {
                    Orientation = Orientation.Horizontal;
                }
            }
        }

        internal Rectangle DragBounds
        {
            get
            {
                return LayoutUtils.InflateRect(ClientRectangle, s_dragMargin);
            }
        }

        internal bool IsInDesignMode
        {
            get
            {
                return DesignMode;
            }
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                return FlowLayout.Instance;
            }
        }

        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool Locked
        {
            get
            {
                return _state[s_stateLocked];
            }
            set
            {
                _state[s_stateLocked] = value;
            }
        }

        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    _scaledRowMargin = LayoutUtils.FlipPadding(_scaledRowMargin);
                    InitFlowLayout();
                    foreach (ToolStripPanelRow row in RowsInternal)
                    {
                        row.OnOrientationChanged();
                    }
                }
            }
        }

        private ToolStripRendererSwitcher RendererSwitcher
        {
            get
            {
                if (_rendererSwitcher is null)
                {
                    _rendererSwitcher = new ToolStripRendererSwitcher(this);
                    HandleRendererChanged(this, EventArgs.Empty);
                    _rendererSwitcher.RendererChanged += new EventHandler(HandleRendererChanged);
                }
                return _rendererSwitcher;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripRenderer Renderer
        {
            get
            {
                return RendererSwitcher.Renderer;
            }
            set
            {
                RendererSwitcher.Renderer = value;
            }
        }

        [SRDescription(nameof(SR.ToolStripRenderModeDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public ToolStripRenderMode RenderMode
        {
            get
            {
                return RendererSwitcher.RenderMode;
            }
            set
            {
                RendererSwitcher.RenderMode = value;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripRendererChanged))]
        public event EventHandler RendererChanged
        {
            add => Events.AddHandler(s_eventRendererChanged, value);
            remove => Events.RemoveHandler(s_eventRendererChanged, value);
        }

        /// <summary>
        ///  Collection of child controls.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRDescription(nameof(SR.ToolStripPanelRowsDescr))]
        internal ToolStripPanelRowCollection RowsInternal
        {
            get
            {
                ToolStripPanelRowCollection rowCollection = (ToolStripPanelRowCollection)Properties.GetObject(s_propToolStripPanelRowCollection);

                if (rowCollection is null)
                {
                    rowCollection = CreateToolStripPanelRowCollection();
                    Properties.SetObject(s_propToolStripPanelRowCollection, rowCollection);
                }
                return rowCollection;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ToolStripPanelRowsDescr))]
        public ToolStripPanelRow[] Rows
        {
            get
            {
                ToolStripPanelRow[] rows = new ToolStripPanelRow[RowsInternal.Count];
                RowsInternal.CopyTo(rows, 0);
                return rows;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get => base.TabIndex;
            set => base.TabIndex = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabIndexChanged
        {
            add => base.TabIndexChanged += value;
            remove => base.TabIndexChanged -= value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool TabStop
        {
            get => base.TabStop;
            set
            {
                SetStyle(ControlStyles.Selectable, value);

                base.TabStop = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        #region ISupportInitialize

        public void BeginInit()
        {
            _state[s_stateBeginInit] = true;
        }

        public void EndInit()
        {
            _state[s_stateBeginInit] = false;
            _state[s_stateEndInit] = true;
            try
            {
                if (!_state[s_stateInJoin])
                {
                    JoinControls();
                }
            }
            finally
            {
                _state[s_stateEndInit] = false;
            }
        }

        #endregion ISupportInitialize

        private ToolStripPanelRowCollection CreateToolStripPanelRowCollection()
        {
            return new ToolStripPanelRowCollection(this);
        }

        protected override ControlCollection CreateControlsInstance()
        {
            return new ToolStripPanelControlCollection(this);
        }

        /// <summary>
        ///  Disposes of the resources (other than memory) used by
        ///  the <see cref='ContainerControl'/>
        ///  .
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ToolStripManager.ToolStripPanels.Remove(this);
            }
            base.Dispose(disposing);
        }

        private void InitFlowLayout()
        {
            if (Orientation == Orientation.Horizontal)
            {
                FlowLayout.SetFlowDirection(this, FlowDirection.TopDown);
            }
            else
            {
                FlowLayout.SetFlowDirection(this, FlowDirection.LeftToRight);
            }
            FlowLayout.SetWrapContents(this, false);
        }
        private Point GetStartLocation(ToolStrip toolStripToDrag)
        {
            if (toolStripToDrag.IsCurrentlyDragging
                && Orientation == Orientation.Horizontal
                && toolStripToDrag.RightToLeft == RightToLeft.Yes)
            {
                // the grip is on the right side, not left.
                return new Point(toolStripToDrag.Right, toolStripToDrag.Top);
            }
            return toolStripToDrag.Location;
        }

        private void HandleRendererChanged(object sender, EventArgs e)
        {
            OnRendererChanged(e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            ToolStripPanelRenderEventArgs rea = new ToolStripPanelRenderEventArgs(e.Graphics, this);
            Renderer.DrawToolStripPanelBackground(rea);

            if (!rea.Handled)
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (!_state[s_stateBeginInit] && !_state[s_stateInJoin])
            {
                if (!_state[s_stateLayoutSuspended])
                {
                    Join(e.Control as ToolStrip, e.Control.Location);
                }
                else
                {
                    BeginInit();
                }
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (e.Control is ISupportToolStripPanel controlToBeDragged)
            {
                controlToBeDragged.ToolStripPanelRow?.ControlsInternal.Remove(e.Control);
            }
            base.OnControlRemoved(e);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            if (e.AffectedComponent != ParentInternal && e.AffectedComponent as Control != null)
            {
                if (e.AffectedComponent is ISupportToolStripPanel draggedControl && RowsInternal.Contains(draggedControl.ToolStripPanelRow))
                {
                    // there's a problem in the base onlayout... if toolstrip needs more space it talks to us
                    // not the row that needs layout.
                    LayoutTransaction.DoLayout(draggedControl.ToolStripPanelRow, e.AffectedComponent as IArrangedElement, e.AffectedProperty);
                }
            }
            base.OnLayout(e);
        }

        internal override void OnLayoutSuspended()
        {
            base.OnLayoutSuspended();
            _state[s_stateLayoutSuspended] = true;
        }

        internal override void OnLayoutResuming(bool resumeLayout)
        {
            base.OnLayoutResuming(resumeLayout);
            _state[s_stateLayoutSuspended] = false;
            if (_state[s_stateBeginInit])
            {
                EndInit();
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (!_state[s_stateBeginInit])
            {
                if (Controls.Count > 0)
                {
                    // rejoin the controls on the other side of the toolstrippanel.
                    SuspendLayout();
                    Control[] controls = new Control[Controls.Count];
                    Point[] controlLocations = new Point[Controls.Count];
                    int j = 0;
                    foreach (ToolStripPanelRow row in RowsInternal)
                    {
                        foreach (Control control in row.ControlsInternal)
                        {
                            controls[j] = control;
                            controlLocations[j] = new Point(row.Bounds.Width - control.Right, control.Top);
                            j++;
                        }
                    }

                    Controls.Clear();

                    for (int i = 0; i < controls.Length; i++)
                    {
                        Join(controls[i] as ToolStrip, controlLocations[i]);
                    }

                    ResumeLayout(true);
                }
            }
            else
            {
                _state[s_stateRightToLeftChanged] = true;
            }
        }

        protected virtual void OnRendererChanged(EventArgs e)
        {
            Renderer.InitializePanel(this);

            Invalidate();

            ((EventHandler)Events[s_eventRendererChanged])?.Invoke(this, e);
        }

        /// <summary>
        ///  We want to Set ToolStripPanel at DesignTime when the ToolStripPanel is added to the Form,
        /// </summary>
        protected override void OnParentChanged(EventArgs e)
        {
            PerformUpdate();
            base.OnParentChanged(e);
        }

        protected override void OnDockChanged(EventArgs e)
        {
            PerformUpdate();
            base.OnDockChanged(e);
        }

        internal void PerformUpdate()
        {
            PerformUpdate(false);
        }
        internal void PerformUpdate(bool forceLayout)
        {
            if (!_state[s_stateBeginInit] && !_state[s_stateInJoin])
            {
                JoinControls(forceLayout);
            }
        }
        private void ResetRenderMode()
        {
            RendererSwitcher.ResetRenderMode();
        }

        private bool ShouldSerializeRenderMode()
        {
            return RendererSwitcher.ShouldSerializeRenderMode();
        }

        private bool ShouldSerializeDock()
        {
            return (owner is null && (Dock != DockStyle.None));
        }

        private void JoinControls()
        {
            JoinControls(false);
        }
        private void JoinControls(bool forceLayout)
        {
            // undone: config - shift to other container
            ToolStripPanelControlCollection controls = Controls as ToolStripPanelControlCollection;
            if (controls.Count > 0)
            {
                controls.Sort();

                // since Join is going to mess with our order - make a copy. (ugh).
                Control[] controlArray = new Control[controls.Count];
                controls.CopyTo(controlArray, 0);
                for (int i = 0; i < controlArray.Length; i++)
                {
                    int numRows = RowsInternal.Count;

                    if (controlArray[i] is ISupportToolStripPanel draggedControl && draggedControl.ToolStripPanelRow != null && !draggedControl.IsCurrentlyDragging)
                    {
                        ToolStripPanelRow row = draggedControl.ToolStripPanelRow;
                        if (row.Bounds.Contains(controlArray[i].Location))
                        {
                            // this toolstrip does not require join.
                            continue;
                        }
                    }
                    if (controlArray[i].AutoSize)
                    {
                        controlArray[i].Size = controlArray[i].PreferredSize;
                    }
                    Point controlLocation = controlArray[i].Location;

                    // right to left has changed while layout was deferred...
                    if (_state[s_stateRightToLeftChanged])
                    {
                        controlLocation = new Point(Width - controlArray[i].Right, controlLocation.Y);
                    }
                    Join(controlArray[i] as ToolStrip, controlArray[i].Location);
                    if (numRows < RowsInternal.Count || forceLayout)
                    {
                        // OK this is weird but here we're in the midst of a suspend layout.
                        // the only way we can deterministically place these guys is to force a layout
                        // each time we've added a new row.  Otherwise we wont find the correct
                        // row to add the control to (PointToRow will fail as Row.Bounds isn't set yet)
                        OnLayout(new LayoutEventArgs(this, PropertyNames.Rows));
                    }
                }
            }
            _state[s_stateRightToLeftChanged] = false;
        }

        #region Feedback

        [ThreadStatic]
        private static FeedbackRectangle feedbackRect;

        private void GiveToolStripPanelFeedback(ToolStrip toolStripToDrag, Point screenLocation)
        {
            if (Orientation == Orientation.Horizontal && RightToLeft == RightToLeft.Yes)
            {
                // paint the feedback in the correct location when RTL.Yes
                screenLocation.Offset(-toolStripToDrag.Width, 0);
            }
            if (CurrentFeedbackRect is null)
            {
                Debug.WriteLineIf(s_toolStripPanelFeedbackDebug.TraceVerbose, "FEEDBACK: creating NEW feedback at " + screenLocation.ToString());

                CurrentFeedbackRect = new FeedbackRectangle(toolStripToDrag.ClientRectangle);
            }

            if (!CurrentFeedbackRect.Visible)
            {
                Debug.WriteLineIf(s_toolStripPanelFeedbackDebug.TraceVerbose, "FEEDBACK: Showing NEW feedback at " + screenLocation.ToString());
                toolStripToDrag.SuspendCaputureMode();
                try
                {
                    CurrentFeedbackRect.Show(screenLocation);
                    toolStripToDrag.Capture = true;
                }
                finally
                {
                    toolStripToDrag.ResumeCaputureMode();
                }
            }
            else
            {
                Debug.WriteLineIf(s_toolStripPanelFeedbackDebug.TraceVerbose, "FEEDBACK: Moving feedback to " + screenLocation.ToString());
                CurrentFeedbackRect.Move(screenLocation);
            }
        }

        internal static void ClearDragFeedback()
        {
#if DEBUG
            if (s_toolStripPanelFeedbackDebug.TraceVerbose)
            {
                Debug.WriteLine("FEEDBACK:  clearing old feedback at "/*+ new StackTrace().ToString()*/);
            }
#endif
            FeedbackRectangle oldFeedback = feedbackRect;
            feedbackRect = null;
            if (oldFeedback != null)
            {
                oldFeedback.Dispose();
            }
        }

        private static FeedbackRectangle CurrentFeedbackRect
        {
            get
            {
                return feedbackRect;
            }
            set
            {
                feedbackRect = value;
            }
        }

        // The FeedbackRectangle happens to encapsulate a toolstripdropdown
        // with a special region. The feedback rectangle exposes the minimum
        // API so the underlying implementation can be replaced if necessary.
        private class FeedbackRectangle : IDisposable
        {
            private FeedbackDropDown _dropDown;

            public FeedbackRectangle(Rectangle bounds)
            {
                _dropDown = new FeedbackDropDown(bounds);
            }

            public bool Visible
            {
                get
                {
                    if (_dropDown != null && !_dropDown.IsDisposed)
                    {
                        return _dropDown.Visible;
                    }
                    return false;
                }
                set
                {
                    if (_dropDown != null && !_dropDown.IsDisposed)
                    {
                        _dropDown.Visible = value;
                    }
                }
            }

            public void Show(Point newLocation)
            {
                _dropDown.Show(newLocation);
            }
            public void Move(Point newLocation)
            {
                _dropDown.MoveTo(newLocation);
            }

            protected void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_dropDown != null)
                    {
                        Visible = false;
                        _dropDown.Dispose();
                        _dropDown = null;
                    }
                }
            }

            public void Dispose()
            {
                Dispose(true);
            }

            ~FeedbackRectangle()
            {
                Dispose(false);
            }

            private class FeedbackDropDown : ToolStripDropDown
            {
                private const int MaxPaintsToService = 20;
                private int _numPaintsServiced; // member variable to protect against re-entrancy

                public FeedbackDropDown(Rectangle bounds) : base()
                {
                    SetStyle(ControlStyles.AllPaintingInWmPaint, false);
                    SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    SetStyle(ControlStyles.CacheText, true);
                    AutoClose = false;
                    AutoSize = false;
                    DropShadowEnabled = false;
                    Bounds = bounds;

                    // caching as this is unlikely to change during the lifetime
                    // of the dropdown.

                    Rectangle regionRect = bounds;    //create a region the size of the client area
                    regionRect.Inflate(-1, -1);        //squish down by one pixel

                    Region rgn = new Region(bounds);  // create region
                    rgn.Exclude(regionRect);          // exclude the center part

                    // set it into the toolstripdropdown’s region
                    Region = rgn;
                }

                // ForceSynchronousPaint - peeks through the message queue, looking for WM_PAINTs
                // calls UpdateWindow on the hwnd to force the paint to happen now.
                //
                // When we're changing the location of the feedback dropdown, we need to
                // force WM_PAINTS to happen, as things that don't respond to WM_ERASEBKGND
                // have bits of the dropdown region drawn all over them.
                private void ForceSynchronousPaint()
                {
                    if (!IsDisposed)
                    {
                        if (_numPaintsServiced == 0)
                        {
                            // protect against re-entrancy.
                            try
                            {
                                var msg = new User32.MSG();
                                while (User32.PeekMessageW(ref msg, IntPtr.Zero, User32.WM.PAINT, User32.WM.PAINT, User32.PM.REMOVE).IsTrue())
                                {
                                    User32.UpdateWindow(msg.hwnd);

                                    // Infinite loop protection
                                    if (_numPaintsServiced++ > MaxPaintsToService)
                                    {
                                        Debug.Fail("somehow we've gotten ourself in a situation where we're pumping an unreasonable number of paint messages, investigate.");
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                _numPaintsServiced = 0;
                            }
                        }
                    }
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                }
                protected override void OnPaintBackground(PaintEventArgs e)
                {
                    // respond to everything in WM_ERASEBKGND
                    Renderer.DrawToolStripBackground(new ToolStripRenderEventArgs(e.Graphics, this));
                    Renderer.DrawToolStripBorder(new ToolStripRenderEventArgs(e.Graphics, this));
                }
                protected override void OnOpening(CancelEventArgs e)
                {
                    base.OnOpening(e);
                    e.Cancel = false;
                }

                public void MoveTo(Point newLocation)
                {
                    Location = newLocation;
                    // if we don't force a paint here, we'll only send WM_ERASEBKGNDs right away
                    // and leave rectangles all over controls that don't respond to that window message.
                    ForceSynchronousPaint();
                }

                protected override void WndProc(ref Message m)
                {
                    if (m.Msg == (int)User32.WM.NCHITTEST)
                    {
                        m.Result = (IntPtr)User32.HT.TRANSPARENT;
                    }

                    base.WndProc(ref m);
                }
            }
        }

        #endregion Feedback

        #region JoinAndMove

        public void Join(ToolStrip toolStripToDrag)
        {
            Join(toolStripToDrag, Point.Empty);
        }

        public void Join(ToolStrip toolStripToDrag, int row)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(row), string.Format(SR.IndexOutOfRange, row.ToString(CultureInfo.CurrentCulture)));
            }

            Point location = Point.Empty;
            Rectangle dragRect = Rectangle.Empty;
            if (row >= RowsInternal.Count)
            {
                dragRect = DragBounds;
            }
            else
            {
                dragRect = RowsInternal[row].DragBounds;
            }

            if (Orientation == Orientation.Horizontal)
            {
                location = new Point(0, dragRect.Bottom - 1);
            }
            else
            {
                location = new Point(dragRect.Right - 1, 0);
            }

            Join(toolStripToDrag, location);
        }

        public void Join(ToolStrip toolStripToDrag, int x, int y)
        {
            Join(toolStripToDrag, new Point(x, y));
        }

        public void Join(ToolStrip toolStripToDrag, Point location)
        {
            if (toolStripToDrag is null)
            {
                throw new ArgumentNullException(nameof(toolStripToDrag));
            }
            if (!_state[s_stateBeginInit] && !_state[s_stateInJoin])
            {
                try
                {
                    _state[s_stateInJoin] = true;
                    toolStripToDrag.ParentInternal = this;
                    MoveInsideContainer(toolStripToDrag, location);
                }
                finally
                {
                    _state[s_stateInJoin] = false;
                }
            }
            else
            {
                Controls.Add(toolStripToDrag);
                toolStripToDrag.Location = location;
            }
        }

        internal void MoveControl(ToolStrip toolStripToDrag, Point screenLocation)
        {
            if (!(toolStripToDrag is ISupportToolStripPanel draggedControl))
            {
                Debug.Fail("Move called on immovable object.");
                return;
            }

            Point clientLocation = PointToClient(screenLocation);
            if (!DragBounds.Contains(clientLocation))
            {
                Debug.WriteLineIf(s_toolStripPanelDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "RC.MoveControl - Point {0} is not in current rafting container drag bounds {1}, calling MoveOutsideContainer", clientLocation, DragBounds));
                MoveOutsideContainer(toolStripToDrag, screenLocation);
                return;
            }
            else
            {
                Join(toolStripToDrag as ToolStrip, clientLocation);
            }
        }

        private void MoveInsideContainer(ToolStrip toolStripToDrag, Point clientLocation)
        {
            ISupportToolStripPanel draggedControl = toolStripToDrag as ISupportToolStripPanel;
            // if the point is not in this rafting container forward on to the appropriate container.

            if (draggedControl.IsCurrentlyDragging && !DragBounds.Contains(clientLocation))
            {
                return;
            }

            // we know we're moving inside the container.

            bool changedRow = false;

            ClearDragFeedback();

            // In design mode we get bogus values for client location.
            if (toolStripToDrag.Site != null && toolStripToDrag.Site.DesignMode && IsHandleCreated)
            {
                if (clientLocation.X < 0 || clientLocation.Y < 0)
                {
                    Point currentCursorLoc = PointToClient(WindowsFormsUtils.LastCursorPoint);
                    if (ClientRectangle.Contains(currentCursorLoc))
                    {
                        clientLocation = currentCursorLoc;
                    }
                }
            }

            //
            // Point INSIDE this rafting container
            //

            ToolStripPanelRow currentToolStripPanelRow = draggedControl.ToolStripPanelRow;

#if DEBUG
            bool debugModeOnly_ChangedContainers = currentToolStripPanelRow != null ?
                                (currentToolStripPanelRow.ToolStripPanel != this) : true;
#endif

            bool pointInCurrentRow = false;
            if (currentToolStripPanelRow != null && currentToolStripPanelRow.Visible && currentToolStripPanelRow.ToolStripPanel == this)
            {
                if (toolStripToDrag.IsCurrentlyDragging)
                {
                    // Dragging with mouse, use DragBounds to check
                    pointInCurrentRow = currentToolStripPanelRow.DragBounds.Contains(clientLocation);
                }
                else
                {
                    // Location is set directly, use normal Bounds to check
                    pointInCurrentRow = currentToolStripPanelRow.Bounds.Contains(clientLocation);
                }
            }

            if (pointInCurrentRow)
            {
                // Point INSIDE same rafting row
                Debug.WriteLineIf(s_toolStripPanelDebug.TraceVerbose, "RC.MoveControl - Point  " + clientLocation + "is in the same row as the control" + draggedControl.ToolStripPanelRow.DragBounds);
                draggedControl.ToolStripPanelRow.MoveControl(toolStripToDrag, GetStartLocation(toolStripToDrag), clientLocation);
            }
            else
            {
                // Point OUTSIDE current rafting row.

                Debug.WriteLineIf(s_toolStripPanelDebug.TraceVerbose, "RC.MoveControl - Point " + clientLocation + " is outside the current rafting row.");

                ToolStripPanelRow row = PointToRow(clientLocation);
                if (row is null)
                {
                    Debug.WriteLineIf(s_toolStripPanelDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "\tThere is no row corresponding to this point, creating a new one."));

                    // there's no row at this point so lets create one
                    int index = RowsInternal.Count;

                    if (Orientation == Orientation.Horizontal)
                    {
                        // if it's above the first row, insert at the front.
                        index = (clientLocation.Y <= Padding.Left) ? 0 : index;
                    }
                    else
                    { // Orientation.Vertical
                        // if it's before the first row, insert at the front.
                        index = (clientLocation.X <= Padding.Left) ? 0 : index;
                    }

                    ToolStripPanelRow previousRow = null;
                    if (RowsInternal.Count > 0)
                    {
                        if (index == 0)
                        {
                            previousRow = RowsInternal[0];
                        }
                        else if (index > 0)
                        {
                            previousRow = RowsInternal[index - 1];
                        }
                    }
                    if (previousRow != null /* there was a previous row */
                        && previousRow.ControlsInternal.Count == 1 /*toolStripToDrag*/
                        && previousRow.ControlsInternal.Contains(toolStripToDrag))
                    {
                        // if the previous row already contains this control
                        // it's futile to create a new row, we're just going to wind
                        // up disposing this one and causing great amounts of flicker.
                        row = previousRow;

                        Debug.WriteLineIf(ToolStripPanelRow.s_toolStripPanelRowCreationDebug.TraceVerbose, "Reusing previous row");
                        // Move the ToolStrip to the new Location in the existing row.
                        if (toolStripToDrag.IsInDesignMode)
                        {
                            Point endLocation = (Orientation == Orientation.Horizontal) ? new Point(clientLocation.X, row.Bounds.Y) : new Point(row.Bounds.X, clientLocation.Y);
                            draggedControl.ToolStripPanelRow.MoveControl(toolStripToDrag, GetStartLocation(toolStripToDrag), endLocation);
                        }
                    }
                    else
                    {
                        // Create a new row and insert it.
                        //
                        Debug.WriteLineIf(ToolStripPanelRow.s_toolStripPanelRowCreationDebug.TraceVerbose, "Inserting a new row at " + index.ToString(CultureInfo.InvariantCulture));
                        row = new ToolStripPanelRow(this);
                        RowsInternal.Insert(index, row);
                    }
                }
                else if (!row.CanMove(toolStripToDrag))
                {
                    Debug.WriteLineIf(ToolStripPanelRow.s_toolStripPanelRowCreationDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "\tThere was a row, but we cant add the control to it, creating/inserting new row."));

                    // we have a row at that point, but its too full or doesnt want
                    // anyone to join it.
                    int index = RowsInternal.IndexOf(row);

                    if (currentToolStripPanelRow != null && currentToolStripPanelRow.ControlsInternal.Count == 1)
                    {
                        if (index > 0 && index - 1 == RowsInternal.IndexOf(currentToolStripPanelRow))
                        {
                            Debug.WriteLineIf(ToolStripPanelRow.s_toolStripPanelRowCreationDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "\tAttempts to leave the current row failed as there's no space in the next row.  Since there's only one control, just keep the row."));
                            return;
                        }
                    }
                    row = new ToolStripPanelRow(this);
                    RowsInternal.Insert(index, row);
                    clientLocation.Y = row.Bounds.Y;
                }

                changedRow = (currentToolStripPanelRow != row);
                if (!changedRow)
                {
                    if (currentToolStripPanelRow != null && currentToolStripPanelRow.ControlsInternal.Count > 1)
                    {
                        // force a leave/re-enter to occur.
                        currentToolStripPanelRow.LeaveRow(toolStripToDrag);
                        currentToolStripPanelRow = null;
                        changedRow = true;
                    }
                }
                if (changedRow)
                {
                    Debug.WriteLineIf(s_toolStripPanelDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "\tCalling JoinRow."));
                    if (currentToolStripPanelRow != null)
                    {
                        currentToolStripPanelRow.LeaveRow(toolStripToDrag);
                    }
                    row.JoinRow(toolStripToDrag, clientLocation);
                }

                if (changedRow && draggedControl.IsCurrentlyDragging)
                {
                    // force the layout of the new row.
                    for (int i = 0; i < RowsInternal.Count; i++)
                    {
                        LayoutTransaction.DoLayout(RowsInternal[i], this, PropertyNames.Rows);
                    }

                    if (RowsInternal.IndexOf(row) > 0)
                    {
                        // When joining a new row, move the cursor to to the location of
                        // the grip, otherwise budging the mouse can pull it down into the next row.
                        Point cursorLoc = toolStripToDrag.PointToScreen(toolStripToDrag.GripRectangle.Location);
                        if (Orientation == Orientation.Vertical)
                        {
                            cursorLoc.X += toolStripToDrag.GripRectangle.Width / 2;
                            cursorLoc.Y = Cursor.Position.Y;
                        }
                        else
                        {
                            cursorLoc.Y += toolStripToDrag.GripRectangle.Height / 2;
                            cursorLoc.X = Cursor.Position.X;
                        }
                        Cursor.Position = cursorLoc;
                    }
                }
            }

#if DEBUG
            Debug_VerifyOneToOneCellRowControlMatchup();
            //            Debug_VerifyCountRows();
            if (draggedControl.IsCurrentlyDragging && changedRow && !debugModeOnly_ChangedContainers)
            {
                // if we have changed containers, we're in a SuspendLayout.
                Debug_VerifyNoOverlaps();
            }
#endif
        }

        private void MoveOutsideContainer(ToolStrip toolStripToDrag, Point screenLocation)
        {
            // look for another rafting container.
            ToolStripPanel panel = ToolStripManager.ToolStripPanelFromPoint(toolStripToDrag, screenLocation);
            if (panel != null)
            {
                using (new LayoutTransaction(panel, panel, null))
                {
                    panel.MoveControl(toolStripToDrag, screenLocation);
                }
                toolStripToDrag.PerformLayout();
#if DEBUG
                ISupportToolStripPanel draggedControl = toolStripToDrag as ISupportToolStripPanel;
                if (draggedControl.IsCurrentlyDragging)
                {
                    Debug_VerifyNoOverlaps();
                }
#endif
            }
            else
            {
                GiveToolStripPanelFeedback(toolStripToDrag, screenLocation);
            }
        }

        /// <summary>
        ///  Given a point within the ToolStripPanel client area -
        ///  it returns the row.  If no such row exists, returns null
        /// </summary>
        public ToolStripPanelRow PointToRow(Point clientLocation)
        {
            // PERF: since we're using the PropertyStore for this.RowsInternal, its actually
            // faster to use foreach.
            foreach (ToolStripPanelRow row in RowsInternal)
            {
                Rectangle bounds = LayoutUtils.InflateRect(row.Bounds, row.Margin);

                // at this point we may not be sized correctly.  Guess.
                if (ParentInternal != null)
                {
                    if (Orientation == Orientation.Horizontal && (bounds.Width == 0))
                    {
                        bounds.Width = ParentInternal.DisplayRectangle.Width;
                    }
                    else if (Orientation == Orientation.Vertical && (bounds.Height == 0))
                    {
                        bounds.Height = ParentInternal.DisplayRectangle.Height;
                    }
                }

                if (bounds.Contains(clientLocation))
                {
                    return row;
                }
            }
            return null;
        }

        #endregion JoinAndMove

        [Conditional("DEBUG")]
        private void Debug_VerifyOneToOneCellRowControlMatchup()
        {
            for (int i = 0; i < RowsInternal.Count; i++)
            {
                ToolStripPanelRow row = RowsInternal[i];
                foreach (ToolStripPanelCell cell in row.Cells)
                {
                    if (cell.Control != null)
                    {
                        ToolStripPanelRow currentlyAssignedRow = ((ISupportToolStripPanel)cell.Control).ToolStripPanelRow;
                        if (currentlyAssignedRow != row)
                        {
                            int goodRowIndex = (currentlyAssignedRow != null) ? RowsInternal.IndexOf(currentlyAssignedRow) : -1;
                            if (goodRowIndex == -1)
                            {
                                Debug.Fail(string.Format(CultureInfo.CurrentCulture, "ToolStripPanelRow has not been assigned!  Should be set to {0}.", i));
                            }
                            else
                            {
                                Debug.Fail(string.Format(CultureInfo.CurrentCulture, "Detected orphan cell! {0} is in row {1}. It shouldn't have a cell in {2}! \r\n\r\nTurn on DEBUG_PAINT in ToolStripPanel and ToolStripPanelRow to investigate.", cell.Control.Name, goodRowIndex, i));
                            }
                        }
                    }
                    else
                    {
                        Debug.Fail("why do we have a cell with a null control in this row?");
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        private void Debug_PrintRows()
        {
            for (int i = 0; i < RowsInternal.Count; i++)
            {
                Debug.Write("Row " + i.ToString(CultureInfo.CurrentCulture) + ": ");
                for (int j = 0; j < RowsInternal[i].ControlsInternal.Count; j++)
                {
                    Debug.Write(string.Format(CultureInfo.CurrentCulture, "[{0} {1}] ", RowsInternal[i].ControlsInternal[j].Name, ((ToolStripPanelCell)RowsInternal[i].Cells[j]).Margin));
                }
                Debug.Write("\r\n");
            }
        }

        [Conditional("DEBUG")]
        private void Debug_VerifyCountRows()
        {
            Debug.Assert(RowsInternal.Count <= Controls.Count, "How did the number of rows get larger than the number of controls?");
        }

        [Conditional("DEBUG")]
        private void Debug_VerifyNoOverlaps()
        {
            foreach (Control c1 in Controls)
            {
                foreach (Control c2 in Controls)
                {
                    if (c1 == c2)
                    {
                        continue;
                    }
                    Rectangle intersection = c1.Bounds;
                    intersection.Intersect(c2.Bounds);

                    if (!LayoutUtils.IsZeroWidthOrHeight(intersection))
                    {
                        ISupportToolStripPanel draggedToolStrip1 = c1 as ISupportToolStripPanel;
                        ISupportToolStripPanel draggedToolStrip2 = c2 as ISupportToolStripPanel;

                        string fail = string.Format(CultureInfo.CurrentCulture,
                            "OVERLAP detection:\r\n{0}: {1} row {2} row bounds {3}",
                            c1.Name ?? "",
                            c1.Bounds,
                            !RowsInternal.Contains(draggedToolStrip1.ToolStripPanelRow) ? "unknown" : RowsInternal.IndexOf(draggedToolStrip1.ToolStripPanelRow).ToString(CultureInfo.CurrentCulture),
                            draggedToolStrip1.ToolStripPanelRow.Bounds);

                        fail += string.Format(CultureInfo.CurrentCulture,
                            "\r\n{0}: {1} row {2} row bounds {3}",
                            c2.Name ?? "",
                            c2.Bounds,
                            !RowsInternal.Contains(draggedToolStrip2.ToolStripPanelRow) ? "unknown" : RowsInternal.IndexOf(draggedToolStrip2.ToolStripPanelRow).ToString(CultureInfo.CurrentCulture),
                            draggedToolStrip2.ToolStripPanelRow.Bounds);
                        Debug.Fail(fail);
                    }
                }
            }
        }

        ArrangedElementCollection IArrangedElement.Children
        {
            get
            {
                return RowsInternal;
            }
        }

        [ListBindable(false)]
        public class ToolStripPanelRowCollection : ArrangedElementCollection, IList
        {
            private readonly ToolStripPanel _owner;

            public ToolStripPanelRowCollection(ToolStripPanel owner)
            {
                _owner = owner;
            }

            public ToolStripPanelRowCollection(ToolStripPanel owner, ToolStripPanelRow[] value)
            {
                _owner = owner;
                AddRange(value);
            }

            /// <summary>
            ///
            /// </summary>
            public new virtual ToolStripPanelRow this[int index]
            {
                get
                {
                    return (ToolStripPanelRow)(InnerList[index]);
                }
            }

            public int Add(ToolStripPanelRow value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                int retVal = InnerList.Add(value);
                OnAdd(value, retVal);
                return retVal;
            }

            public void AddRange(ToolStripPanelRow[] value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                ToolStripPanel currentOwner = _owner;
                if (currentOwner != null)
                {
                    currentOwner.SuspendLayout();
                }
                try
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        Add(value[i]);
                    }
                }
                finally
                {
                    if (currentOwner != null)
                    {
                        currentOwner.ResumeLayout();
                    }
                }
            }
            public void AddRange(ToolStripPanelRowCollection value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                ToolStripPanel currentOwner = _owner;
                if (currentOwner != null)
                {
                    currentOwner.SuspendLayout();
                }
                try
                {
                    int currentCount = value.Count;
                    for (int i = 0; i < currentCount; i++)
                    {
                        Add(value[i]);
                    }
                }
                finally
                {
                    if (currentOwner != null)
                    {
                        currentOwner.ResumeLayout();
                    }
                }
            }

            public bool Contains(ToolStripPanelRow value)
            {
                return InnerList.Contains(value);
            }

            public virtual void Clear()
            {
                if (_owner != null)
                {
                    _owner.SuspendLayout();
                }
                try
                {
                    while (Count != 0)
                    {
                        RemoveAt(Count - 1);
                    }
                }
                finally
                {
                    if (_owner != null)
                    {
                        _owner.ResumeLayout();
                    }
                }
            }

            void IList.Clear() { Clear(); }
            bool IList.IsFixedSize { get { return InnerList.IsFixedSize; } }
            bool IList.Contains(object value) { return InnerList.Contains(value); }
            bool IList.IsReadOnly { get { return InnerList.IsReadOnly; } }
            void IList.RemoveAt(int index) { RemoveAt(index); }
            void IList.Remove(object value) { Remove(value as ToolStripPanelRow); }
            int IList.Add(object value) { return Add(value as ToolStripPanelRow); }
            int IList.IndexOf(object value) { return IndexOf(value as ToolStripPanelRow); }
            void IList.Insert(int index, object value) { Insert(index, value as ToolStripPanelRow); }

            object IList.this[int index]
            {
                get { return InnerList[index]; }
                set { throw new NotSupportedException(SR.ToolStripCollectionMustInsertAndRemove); /* InnerList[index] = value; */ }
            }

            public int IndexOf(ToolStripPanelRow value)
            {
                return InnerList.IndexOf(value);
            }

            public void Insert(int index, ToolStripPanelRow value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                InnerList.Insert(index, value);
                OnAdd(value, index);
            }

            private void OnAdd(ToolStripPanelRow value, int index)
            {
                if (_owner != null)
                {
                    LayoutTransaction.DoLayout(_owner, value, PropertyNames.Parent);
                }
            }

            /// <summary>
            ///  Do proper cleanup of ownership, etc.
            /// </summary>
            private void OnAfterRemove(ToolStripPanelRow row)
            {
#if DEBUG
                if (s_toolStripPanelMissingRowDebug.TraceVerbose)
                {
                    if (row != null)
                    {
                        Debug.Write("Removing row: ");
                        row.Debug_PrintRowID();
                        Debug.WriteLine(new StackTrace().ToString());
                    }
                }
#endif

            }

            public void Remove(ToolStripPanelRow value)
            {
                InnerList.Remove(value);
                OnAfterRemove(value);
            }

            public void RemoveAt(int index)
            {
                ToolStripPanelRow item = null;
                if (index < Count && index >= 0)
                {
                    item = (ToolStripPanelRow)(InnerList[index]);
                }
                InnerList.RemoveAt(index);
                OnAfterRemove(item);
            }

            public void CopyTo(ToolStripPanelRow[] array, int index)
            {
                InnerList.CopyTo(array, index);
            }
        }

        internal class ToolStripPanelControlCollection : TypedControlCollection
        {
            private readonly ToolStripPanel _owner;

            public ToolStripPanelControlCollection(ToolStripPanel owner)
                : base(owner, typeof(ToolStrip))
            {
                _owner = owner;
            }

            internal override void AddInternal(Control value)
            {
                if (value != null)
                {
                    using (new LayoutTransaction(value, value, PropertyNames.Parent))
                    {
                        base.AddInternal(value);
                    }
                }
                else
                {
                    base.AddInternal(value);
                }
            }

            internal void Sort()
            {
                if (_owner.Orientation == Orientation.Horizontal)
                {
                    InnerList.Sort(new YXComparer());
                }
                else
                {
                    InnerList.Sort(new XYComparer());
                }
            }

            // sort by X, then Y
            public class XYComparer : IComparer
            {
                public XYComparer() { }
                public int Compare(object first, object second)
                {
                    Control one = first as Control;
                    Control two = second as Control;

                    if (one.Bounds.X < two.Bounds.X)
                    {
                        return -1;
                    }

                    if (one.Bounds.X == two.Bounds.X)
                    {
                        if (one.Bounds.Y < two.Bounds.Y)
                        {
                            return -1;
                        }
                        return 1;
                    }
                    return 1;
                }
            }

            // sort by Y, then X
            public class YXComparer : IComparer
            {
                public YXComparer() { }
                public int Compare(object first, object second)
                {
                    Control one = first as Control;
                    Control two = second as Control;

                    if (one.Bounds.Y < two.Bounds.Y)
                    {
                        return -1;
                    }

                    if (one.Bounds.Y == two.Bounds.Y)
                    {
                        if (one.Bounds.X < two.Bounds.X)
                        {
                            return -1;
                        }
                        return 1;
                    }
                    return 1;
                }
            }
        }
    }
}
