// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.Globalization;

    /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter"]/*' />
    /// <devdoc>
    ///     Provides user resizing of docked elements at run time. To use a Splitter you can
    ///     dock any control to an edge of a container, and then dock the splitter to the same
    ///     edge. The splitter will then resize the control that is previous in the docking
    ///     order.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(SplitterMoved)),
    DefaultProperty(nameof(Dock)),
    SRDescription(nameof(SR.DescriptionSplitter)),
    Designer("System.Windows.Forms.Design.SplitterDesigner, " + AssemblyRef.SystemDesign)
    ]
    public class Splitter : Control  {
        private const int DRAW_START = 1;
        private const int DRAW_MOVE = 2;
        private const int DRAW_END = 3;

        private const int defaultWidth = 3;

        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;
        private int minSize = 25;
        private int minExtra = 25;
        private Point anchor = Point.Empty;
        private Control splitTarget;
        private int splitSize = -1;
        private int splitterThickness = 3;
        private int initTargetSize;
        private int lastDrawSplit = -1;       
        private int maxSize;
        private static readonly object EVENT_MOVING = new object();
        private static readonly object EVENT_MOVED = new object();

        // Cannot expose IMessageFilter.PreFilterMessage through this unsealed class
        private SplitterMessageFilter splitterMessageFilter = null;

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Splitter"]/*' />
        /// <devdoc>
        ///     Creates a new Splitter.
        /// </devdoc>
        public Splitter()
        : base() {
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            minSize = 25;
            minExtra = 25;
            
            Dock = DockStyle.Left;
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Anchor"]/*' />
        /// <devdoc>
        ///     The current value of the anchor property. The anchor property
        ///     determines which edges of the control are anchored to the container's
        ///     edges.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DefaultValue(AnchorStyles.None)]
        public override AnchorStyles Anchor {
            get {
                return AnchorStyles.None;
            }
            set {
                // do nothing!
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.AllowDrop"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop {
            get {
                return base.AllowDrop;
            }
            set {
                base.AllowDrop = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(defaultWidth, defaultWidth);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.DefaultCursor"]/*' />
        protected override Cursor DefaultCursor {
            get {
                switch (Dock) {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        return Cursors.HSplit;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        return Cursors.VSplit;
                }
                return base.DefaultCursor;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.ForeColor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.ForeColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                base.ForeColorChanged += value;
            }
            remove {
                base.ForeColorChanged -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.BackgroundImageChanged"]/*' />
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

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.BackgroundImageLayoutChanged"]/*' />
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

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Font"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.FontChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged {
            add {
                base.FontChanged += value;
            }
            remove {
                base.FontChanged -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.BorderStyle"]/*' />
        /// <devdoc>
        ///     Indicates what type of border the Splitter control has.  This value
        ///     comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </devdoc>
        [
        DefaultValue(BorderStyle.None),
        SRCategory(nameof(SR.CatAppearance)),
        System.Runtime.InteropServices.DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.SplitterBorderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }
            
                if (borderStyle != value) {
                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.CreateParams"]/*' />
        /// <devdoc>
        ///     Returns the parameters needed to create the handle.  Inheriting classes
        ///     can override this to provide extra functionality.  They should not,
        ///     however, forget to call base.getCreateParams() first to get the struct
        ///     filled up with the basic info.
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
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
                return cp;
            }
        }
        
        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.DefaultImeMode"]/*' />
        protected override ImeMode DefaultImeMode {
            get {
                return ImeMode.Disable;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Dock"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [
        Localizable(true),
        DefaultValue(DockStyle.Left)
        ]
        public override DockStyle Dock {
            get { return base.Dock;}

            set {
            
                if (!(value == DockStyle.Top || value == DockStyle.Bottom || value == DockStyle.Left || value == DockStyle.Right)) {
                    throw new ArgumentException(SR.SplitterInvalidDockEnum);
                }
                
                int requestedSize = splitterThickness;
                
                base.Dock = value;
                switch (Dock) {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        if (splitterThickness != -1) {
                            Height = requestedSize;
                        }
                        break;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        if (splitterThickness != -1) {
                            Width = requestedSize;
                        }
                        break;
                }
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Horizontal"]/*' />
        /// <devdoc>
        ///     Determines if the splitter is horizontal.
        /// </devdoc>
        /// <internalonly/>
        private bool Horizontal {
            get {
                DockStyle dock = Dock;
                return dock == DockStyle.Left || dock == DockStyle.Right;
            }
        }
        
        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.ImeMode"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode {
            get {
                return base.ImeMode;
            }
            set {
                base.ImeMode = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.ImeModeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged {
            add {
                base.ImeModeChanged += value;
            }
            remove {
                base.ImeModeChanged -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.MinExtra"]/*' />
        /// <devdoc>
        ///     The minExtra is this minimum size (in pixels) of the remaining
        ///     area of the container. This area is center of the container that
        ///     is not occupied by edge docked controls, this is the are that
        ///     would be used for any fill docked control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(25),
        SRDescription(nameof(SR.SplitterMinExtraDescr))
        ]
        public int MinExtra {
            get {
                return minExtra;
            }
            set {
                if (value < 0) value = 0;
                minExtra = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.MinSize"]/*' />
        /// <devdoc>
        ///     The minSize is the minimum size (in pixels) of the target of the
        ///     splitter. The target of a splitter is always the control adjacent
        ///     to the splitter, just prior in the dock order.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(25),
        SRDescription(nameof(SR.SplitterMinSizeDescr))
        ]
        public int MinSize {
            get {
                return minSize;
            }
            set {
                if (value < 0) value = 0;
                minSize = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitPosition"]/*' />
        /// <devdoc>
        ///     The position of the splitter. If the splitter is not bound
        ///     to a control, SplitPosition will be -1.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.SplitterSplitPositionDescr))
        ]
        public int SplitPosition {
            get {
                if (splitSize == -1) splitSize = CalcSplitSize();
                return splitSize;
            }
            set {
                // calculate maxSize and other bounding conditions
                SplitData spd = CalcSplitBounds();

                // this is not an else-if to handle the maxSize < minSize case...
                // ie. we give minSize priority over maxSize...
                if (value > maxSize) value = maxSize;
                if (value < minSize) value = minSize;

                // if (value == splitSize) return;  -- do we need this check?

                splitSize = value;
                DrawSplitBar(DRAW_END);

                if (spd.target == null) {
                    splitSize = -1;
                    return;
                }

                Rectangle bounds = spd.target.Bounds;
                switch (Dock) {
                    case DockStyle.Top:
                        bounds.Height = value;
                        break;
                    case DockStyle.Bottom:
                        bounds.Y += bounds.Height - splitSize;
                        bounds.Height = value;
                        break;
                    case DockStyle.Left:
                        bounds.Width = value;
                        break;
                    case DockStyle.Right:
                        bounds.X += bounds.Width - splitSize;
                        bounds.Width = value;
                        break;
                }
                spd.target.Bounds = bounds;
                Application.DoEvents();
                OnSplitterMoved(new SplitterEventArgs(Left, Top, (Left + bounds.Width / 2), (Top + bounds.Height / 2)));
                
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.TabStop"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.TabStopChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Text"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
        Bindable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]                
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }
        
        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }
        
        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Enter"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Enter {
            add {
                base.Enter += value;
            }
            remove {
                base.Enter -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.KeyUp"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyUp {
            add {
                base.KeyUp += value;
            }
            remove {
                base.KeyUp -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.KeyDown"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown {
            add {
                base.KeyDown += value;
            }
            remove {
                base.KeyDown -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.KeyPress"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler KeyPress {
            add {
                base.KeyPress += value;
            }
            remove {
                base.KeyPress -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.Leave"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Leave {
            add {
                base.Leave += value;
            }
            remove {
                base.Leave -= value;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitterMoving"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.SplitterSplitterMovingDescr))]
        public event SplitterEventHandler SplitterMoving {
            add {
                Events.AddHandler(EVENT_MOVING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOVING, value);
            }
        }


        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitterMoved"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.SplitterSplitterMovedDescr))]
        public event SplitterEventHandler SplitterMoved {
            add {
                Events.AddHandler(EVENT_MOVED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOVED, value);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.DrawSplitBar"]/*' />
        /// <devdoc>
        ///     Draws the splitter bar at the current location. Will automatically
        ///     cleanup anyplace the splitter was drawn previously.
        /// </devdoc>
        /// <internalonly/>
        private void DrawSplitBar(int mode) {
            if (mode != DRAW_START && lastDrawSplit != -1) {
                DrawSplitHelper(lastDrawSplit);
                lastDrawSplit = -1;
            }
            // Bail if drawing with no old point...
            //
            else if (mode != DRAW_START && lastDrawSplit == -1) {
                return;
            }

            if (mode != DRAW_END) {
                DrawSplitHelper(splitSize);
                lastDrawSplit = splitSize;
            }
            else {
                if (lastDrawSplit != -1) {
                    DrawSplitHelper(lastDrawSplit);
                }
                lastDrawSplit = -1;
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.CalcSplitLine"]/*' />
        /// <devdoc>
        ///     Calculates the bounding rect of the split line. minWeight refers
        ///     to the minimum height or width of the splitline.
        /// </devdoc>
        private Rectangle CalcSplitLine(int splitSize, int minWeight) {
            Rectangle r = Bounds;
            Rectangle bounds = splitTarget.Bounds;
            switch (Dock) {
                case DockStyle.Top:
                    if (r.Height < minWeight) r.Height = minWeight;
                    r.Y = bounds.Y + splitSize;
                    break;
                case DockStyle.Bottom:
                    if (r.Height < minWeight) r.Height = minWeight;
                    r.Y = bounds.Y + bounds.Height - splitSize - r.Height;
                    break;
                case DockStyle.Left:
                    if (r.Width < minWeight) r.Width = minWeight;
                    r.X = bounds.X + splitSize;
                    break;
                case DockStyle.Right:
                    if (r.Width < minWeight) r.Width = minWeight;
                    r.X = bounds.X + bounds.Width - splitSize - r.Width;
                    break;
            }
            return r;
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.CalcSplitSize"]/*' />
        /// <devdoc>
        ///     Calculates the current size of the splitter-target.
        /// </devdoc>
        /// <internalonly/>
        private int CalcSplitSize() {
            Control target = FindTarget();
            if (target == null) return -1;
            Rectangle r = target.Bounds;
            switch (Dock) {
                case DockStyle.Top:
                case DockStyle.Bottom:
                    return r.Height;
                case DockStyle.Left:
                case DockStyle.Right:
                    return r.Width;
                default:
                    return -1; // belts & braces
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.CalcSplitBounds"]/*' />
        /// <devdoc>
        ///     Calculates the bounding criteria for the splitter.
        /// </devdoc>
        /// <internalonly/>
        private SplitData CalcSplitBounds() {
            SplitData spd = new SplitData();
            Control target = FindTarget();
            spd.target = target;
            if (target != null) {
                switch (target.Dock) {
                    case DockStyle.Left:
                    case DockStyle.Right:
                        initTargetSize = target.Bounds.Width;
                        break;
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        initTargetSize = target.Bounds.Height;
                        break;
                }
                Control parent = ParentInternal;
                Control.ControlCollection children = parent.Controls;
                int count = children.Count;
                int dockWidth = 0, dockHeight = 0;
                for (int i = 0; i < count; i++) {
                    Control ctl = children[i];
                    if (ctl != target) {
                        switch (((Control)ctl).Dock) {
                            case DockStyle.Left:
                            case DockStyle.Right:
                                dockWidth += ctl.Width;
                                break;
                            case DockStyle.Top:
                            case DockStyle.Bottom:
                                dockHeight += ctl.Height;
                                break;
                        }
                    }
                }
                Size clientSize = parent.ClientSize;
                if (Horizontal) {
                    maxSize = clientSize.Width - dockWidth - minExtra;
                }
                else {
                    maxSize = clientSize.Height - dockHeight - minExtra;
                }
                spd.dockWidth = dockWidth;
                spd.dockHeight = dockHeight;
            }
            return spd;
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.DrawSplitHelper"]/*' />
        /// <devdoc>
        ///     Draws the splitter line at the requested location. Should only be called
        ///     by drawSpltBar.
        /// </devdoc>
        /// <internalonly/>
        private void DrawSplitHelper(int splitSize) {
            if (splitTarget == null) {
                return;
            }

            Rectangle r = CalcSplitLine(splitSize, 3);
            IntPtr parentHandle = ParentInternal.Handle;
            IntPtr dc = UnsafeNativeMethods.GetDCEx(new HandleRef(ParentInternal, parentHandle), NativeMethods.NullHandleRef, NativeMethods.DCX_CACHE | NativeMethods.DCX_LOCKWINDOWUPDATE);
            IntPtr halftone = ControlPaint.CreateHalftoneHBRUSH();
            IntPtr saveBrush = SafeNativeMethods.SelectObject(new HandleRef(ParentInternal, dc), new HandleRef(null, halftone));
            SafeNativeMethods.PatBlt(new HandleRef(ParentInternal, dc), r.X, r.Y, r.Width, r.Height, NativeMethods.PATINVERT);
            SafeNativeMethods.SelectObject(new HandleRef(ParentInternal, dc), new HandleRef(null, saveBrush));
            SafeNativeMethods.DeleteObject(new HandleRef(null, halftone));
            UnsafeNativeMethods.ReleaseDC(new HandleRef(ParentInternal, parentHandle), new HandleRef(null, dc));
        }


        /// <devdoc>
        ///     Raises a splitter event
        /// </devdoc>
        /// <internalonly/>

        /* No one seems to be calling this, so it is okay to comment it out
        private void RaiseSplitterEvent(object key, SplitterEventArgs spevent) {
            SplitterEventHandler handler = (SplitterEventHandler)Events[key];
            if (handler != null) handler(this, spevent);
        }
        */

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.FindTarget"]/*' />
        /// <devdoc>
        ///     Finds the target of the splitter. The target of the splitter is the
        ///     control that is "outside" or the splitter. For example, if the splitter
        ///     is docked left, the target is the control that is just to the left
        ///     of the splitter.
        /// </devdoc>
        /// <internalonly/>
        private Control FindTarget() {
            Control parent = ParentInternal;
            if (parent == null) return null;
            Control.ControlCollection children = parent.Controls;
            int count = children.Count;
            DockStyle dock = Dock;
            for (int i = 0; i < count; i++) {
                Control target = children[i];
                if (target != this) {
                    switch (dock) {
                        case DockStyle.Top:
                            if (target.Bottom == Top) return(Control)target;
                            break;
                        case DockStyle.Bottom:
                            if (target.Top == Bottom) return(Control)target;
                            break;
                        case DockStyle.Left:
                            if (target.Right == Left) return(Control)target;
                            break;
                        case DockStyle.Right:
                            if (target.Left == Right) return(Control)target;
                            break;
                    }
                }
            }
            return null;
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.GetSplitSize"]/*' />
        /// <devdoc>
        ///     Calculates the split size based on the mouse position (x, y).
        /// </devdoc>
        /// <internalonly/>
        private int GetSplitSize(int x, int y) {
            int delta;
            if (Horizontal) {
                delta = x - anchor.X;
            }
            else {
                delta = y - anchor.Y;
            }
            int size = 0;
            switch (Dock) {
                case DockStyle.Top:
                    size = splitTarget.Height + delta;
                    break;
                case DockStyle.Bottom:
                    size = splitTarget.Height - delta;
                    break;
                case DockStyle.Left:
                    size = splitTarget.Width + delta;
                    break;
                case DockStyle.Right:
                    size = splitTarget.Width - delta;
                    break;
            }
            return Math.Max(Math.Min(size, maxSize), minSize);
        }
        
        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.OnKeyDown"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (splitTarget != null && e.KeyCode == Keys.Escape) {
                SplitEnd(false);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.OnMouseDown"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1) {
                SplitBegin(e.X, e.Y);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.OnMouseMove"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (splitTarget != null) {
                int x = e.X + Left;
                int y = e.Y + Top;
                Rectangle r = CalcSplitLine(GetSplitSize(e.X, e.Y), 0);
                int xSplit = r.X;
                int ySplit = r.Y;
                OnSplitterMoving(new SplitterEventArgs(x, y, xSplit, ySplit));
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.OnMouseUp"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            if (splitTarget != null) {
                int x = e.X + Left;
                int y = e.Y + Top;
                Rectangle r = CalcSplitLine(GetSplitSize(e.X, e.Y), 0);
                int xSplit = r.X;
                int ySplit = r.Y;
                SplitEnd(true);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.OnSplitterMoving"]/*' />
        /// <devdoc>
        ///     Inherriting classes should override this method to respond to the
        ///     splitterMoving event. This event occurs while the splitter is
        ///     being moved by the user.
        /// </devdoc>
        protected virtual void OnSplitterMoving(SplitterEventArgs sevent) {
            SplitterEventHandler handler = (SplitterEventHandler)Events[EVENT_MOVING];
            if (handler != null) handler(this,sevent);
            if (splitTarget != null) {
                SplitMove(sevent.SplitX, sevent.SplitY);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.OnSplitterMoved"]/*' />
        /// <devdoc>
        ///     Inherriting classes should override this method to respond to the
        ///     splitterMoved event. This event occurs when the user finishes
        ///     moving the splitter.
        /// </devdoc>
        protected virtual void OnSplitterMoved(SplitterEventArgs sevent) {
            SplitterEventHandler handler = (SplitterEventHandler)Events[EVENT_MOVED];
            if (handler != null) handler(this,sevent);
            if (splitTarget != null) {
                SplitMove(sevent.SplitX, sevent.SplitY);
            }
        }

        

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SetBoundsCore"]/*' />
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            if (Horizontal) {
                if (width < 1) {
                    width = 3;
                }
                splitterThickness = width;
            }
            else {
                if (height < 1) {
                    height = 3;
                }
                splitterThickness = height;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitBegin"]/*' />
        /// <devdoc>
        ///     Begins the splitter moving.
        /// </devdoc>
        /// <internalonly/>
        private void SplitBegin(int x, int y) {
            SplitData spd = CalcSplitBounds();
            if (spd.target != null && (minSize < maxSize)) {
                anchor = new Point(x, y);
                splitTarget = spd.target;
                splitSize = GetSplitSize(x, y);

                // 



                IntSecurity.UnmanagedCode.Assert();
                try {
                    if (splitterMessageFilter != null)
                    {
                        splitterMessageFilter = new SplitterMessageFilter(this);
                    }
                    Application.AddMessageFilter(splitterMessageFilter);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
                CaptureInternal = true;
                DrawSplitBar(DRAW_START);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitEnd"]/*' />
        /// <devdoc>
        ///     Finishes the split movement.
        /// </devdoc>
        /// <internalonly/>
        private void SplitEnd(bool accept) {
            DrawSplitBar(DRAW_END);
            splitTarget = null;
            CaptureInternal = false;
            if (splitterMessageFilter != null)
            {
                Application.RemoveMessageFilter(splitterMessageFilter);
                splitterMessageFilter = null;
            }

            if (accept) {
                ApplySplitPosition();
            }
            else if (splitSize != initTargetSize) {
                SplitPosition = initTargetSize;
            }
            anchor = Point.Empty;
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.ApplySplitPosition"]/*' />
        /// <devdoc>
        ///     Sets the split position to be the current split size. This is called
        ///     by splitEdit
        /// </devdoc>
        /// <internalonly/>
        private void ApplySplitPosition() {
            SplitPosition = splitSize;
        }
        
        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitMove"]/*' />
        /// <devdoc>
        ///     Moves the splitter line to the splitSize for the mouse position
        ///     (x, y).
        /// </devdoc>
        /// <internalonly/>
        private void SplitMove(int x, int y) {
            int size = GetSplitSize(x-Left+anchor.X, y-Top+anchor.Y);
            if (splitSize != size) {
                splitSize = size;
                DrawSplitBar(DRAW_MOVE);
            }
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", MinExtra: " + MinExtra.ToString(CultureInfo.CurrentCulture) + ", MinSize: " + MinSize.ToString(CultureInfo.CurrentCulture);
        }

        /// <include file='doc\Splitter.uex' path='docs/doc[@for="Splitter.SplitData"]/*' />
        /// <devdoc>
        ///     Return value holder...
        /// </devdoc>
        private class SplitData {
            public int dockWidth = -1;
            public int dockHeight = -1;
            internal Control target;
        }


        private class SplitterMessageFilter : IMessageFilter 
        {
            private Splitter owner = null;

            public SplitterMessageFilter(Splitter splitter)
            {
                this.owner = splitter;
            }
            
            /// <include file='doc\SplitterMessageFilter.uex' path='docs/doc[@for="SplitterMessageFilter.PreFilterMessage"]/*' />
            /// <devdoc>
            /// </devdoc>
            /// <internalonly/>
            [
                System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode),
            ]
            public bool PreFilterMessage(ref Message m) {
                if (m.Msg >= NativeMethods.WM_KEYFIRST && m.Msg <= NativeMethods.WM_KEYLAST) {
                    if (m.Msg == NativeMethods.WM_KEYDOWN && unchecked((int)(long)m.WParam) == (int)Keys.Escape) {
                        owner.SplitEnd(false);
                    }
                    return true;
                }
                return false;
            }
        }
    }
}

