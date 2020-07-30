// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A SplitContainer is a ContainerControl with 2 panels separated with a splitter
    ///  in the middle. This is a composite control. The user can drag and drop this control from Toolbox.
    ///  Controls can be added to the right panel and the left panel. The Orientation can be either Horizontal or Vertical.
    ///  The Controls inside the Panels would be redrawn with the new Orientation.
    ///  With this control the user need be aware of docking, z-order of the controls. The controls get parented when thry are
    ///  dropped on the SpitContainer.
    /// </summary>
    [DefaultEvent(nameof(SplitterMoved))]
    [Docking(DockingBehavior.AutoDock)]
    [Designer("System.Windows.Forms.Design.SplitContainerDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionSplitContainer))]
    public class SplitContainer : ContainerControl, ISupportInitialize
    {
        // Constants used during split container movement
        private const int DrawStart = 1;
        private const int DrawMove = 2;
        private const int DrawEnd = 3;

        private int _borderSize;

        // SplitContainer private Cached copies of public properties
        private Orientation _orientation = Orientation.Vertical;
        private BorderStyle _borderStyle = BorderStyle.None;
        private FixedPanel _fixedPanel = FixedPanel.None;

        private int _panel1MinSize = 25;    // Panel1 Minimum Size
        private int _newPanel1MinSize = 25; // New panel1 Minimum Size used for ISupportInitialize
        private int _panel2MinSize = 25;    // Panel2 Minimum Size
        private int _newPanel2MinSize = 25; // New panel2 Minimum Size used for ISupportInitialize
        private bool _tabStop = true;
        private int _panelSize;

        // Splitter properties
        private Rectangle _splitterRect;
        private int _splitterInc = 1;
        private int _splitterDistance = 50; // Default splitter distance
        private int _splitterWidth = 4;
        private int _newSplitterWidth = 4;  // New splitter width used for ISupportInitialize
        private int _splitDistance = 50;

        // Properties used using drawing a moving splitter
        private int _lastDrawSplit = 1;
        private int _initialSplitterDistance;
        private Rectangle _initialSplitterRectangle;
        private Point _anchor = Point.Empty;
        private bool _splitBegin;
        private bool _splitMove;
        private bool _splitBreak;

        // Split Cursor
        Cursor _overrideCursor;

        // Needed For Tabbing
        Control _nextActiveControl;
        private bool _callBaseVersion;
        private bool _splitterFocused;

        // Required to keep track of Splitter movements
        private bool _splitterClick;
        private bool _splitterDrag;

        // FixedPanel.None requires us to keep the Width/Height Ratio Depending on SplitContainer.Orientation
        private double _ratioWidth;
        private double _ratioHeight;
        private bool _resizeCalled;
        private bool _splitContainerScaling;
        private bool _setSplitterDistance;

        // Events
        private static readonly object s_eventMoving = new object();
        private static readonly object s_eventMoved = new object();

        // IMessageFilter implementation
        private SplitContainerMessageFilter _splitContainerMessageFilter;

        // This would avoid re-entrant code into SelectNextControl.
        private bool _selectNextControl;

        // Initialization flag for ISupportInitialize
        private bool _initializing;

        public SplitContainer()
        {
            // either the left or top panel - LTR
            // either the right or top panel - RTL
            Panel1 = new SplitterPanel(this);
            // either the right or bottom panel - LTR
            // either the left or bottom panel - RTL
            Panel2 = new SplitterPanel(this);
            _splitterRect = new Rectangle();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            ((TypedControlCollection)Controls).AddInternal(Panel1);
            ((TypedControlCollection)Controls).AddInternal(Panel2);
            UpdateSplitter();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                         //
        //PROPERTIES START IN ALPHABETICAL ORDER                                                   //
        //                                                                                         //
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  This property is overridden to allow the AutoScroll to be set on all the panels when
        ///  The autoScroll on SplitContainer is shown.
        ///  Here we dont set the base value ... but set autoscroll for panels.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FormAutoScrollDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll
        {
            get
            {
                //Always return false ... as Splitcontainer doesnt support AutoScroll
                return false;
            }
            set => base.AutoScroll = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(typeof(Point), "0, 0")]
        public override Point AutoScrollOffset
        {
            get => base.AutoScrollOffset;
            set => base.AutoScrollOffset = value;
        }

        /// <summary>
        ///  Override AutoScrollMinSize to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Size AutoScrollMinSize
        {
            get => base.AutoScrollMinSize;
            set => base.AutoScrollMinSize = value;
        }

        /// <summary>
        ///  Override AutoScrollMargin to make it hidden from the user in the designer
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new Size AutoScrollMargin
        {
            get => base.AutoScrollMargin;
            set => base.AutoScrollMargin = value;
        }

        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormAutoScrollPositionDescr))]
        public new Point AutoScrollPosition
        {
            get => base.AutoScrollPosition;
            set => base.AutoScrollPosition = value;
        }

        /// <summary>
        ///  Hide AutoSize, as it can mean more than one thing and might confuse users
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        /// <summary>
        ///  The binding manager for the container control.
        /// </summary>
        [Browsable(false)]
        [SRDescription(nameof(SR.ContainerControlBindingContextDescr))]
        public override BindingContext BindingContext
        {
            get
            {
                return BindingContextInternal;
            }
            set
            {
                BindingContextInternal = value;
            }
        }

        /// <summary>
        ///  Indicates what type of border the Splitter control has.  This value
        ///  comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </summary>
        [DefaultValue(BorderStyle.None)]
        [SRCategory(nameof(SR.CatAppearance))]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.SplitterBorderStyleDescr))]
        public BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (_borderStyle != value)
                {
                    _borderStyle = value;
                    Invalidate();
                    SetInnerMostBorder(this);
                    if (ParentInternal != null)
                    {
                        if (ParentInternal is SplitterPanel)
                        {
                            SplitContainer sc = (SplitContainer)((SplitterPanel)ParentInternal).Owner;
                            sc.SetInnerMostBorder(sc);
                        }
                    }
                }

                switch (BorderStyle)
                {
                    case BorderStyle.None:
                        _borderSize = 0;
                        break;
                    case BorderStyle.FixedSingle:
                        _borderSize = 1;
                        break;
                    case BorderStyle.Fixed3D:
                        _borderSize = 4;
                        break;
                }
            }
        }

        /// <summary>
        ///  Controls Collection...
        ///  This is overriden so that the Controls.Add ( ) is not Code Gened...
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls
        {
            get => base.Controls;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event ControlEventHandler ControlAdded
        {
            add => base.ControlAdded += value;
            remove => base.ControlAdded -= value;
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event ControlEventHandler ControlRemoved
        {
            add => base.ControlRemoved += value;
            remove => base.ControlRemoved -= value;
        }
        /// <summary>
        ///  The dock property. The dock property controls to which edge
        ///  of the container this control is docked to. For example, when docked to
        ///  the top of the container, the control will be displayed flush at the
        ///  top of the container, extending the length of the container.
        /// </summary>
        public new DockStyle Dock
        {
            get => base.Dock;
            set
            {
                base.Dock = value;
                if (ParentInternal != null)
                {
                    if (ParentInternal is SplitterPanel)
                    {
                        SplitContainer sc = (SplitContainer)((SplitterPanel)ParentInternal).Owner;
                        sc.SetInnerMostBorder(sc);
                    }
                }
                ResizeSplitContainer();
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(150, 100);
            }
        }

        /// <summary>
        ///  Indicates what type of border the Splitter control has.  This value
        ///  comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </summary>
        [DefaultValue(FixedPanel.None)]
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.SplitContainerFixedPanelDescr))]
        public FixedPanel FixedPanel
        {
            get
            {
                return _fixedPanel;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FixedPanel.None, (int)FixedPanel.Panel2))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FixedPanel));
                }

                if (_fixedPanel != value)
                {
                    _fixedPanel = value;
                    // UpdatePanelSize !!
                    switch (_fixedPanel)
                    {
                        case FixedPanel.Panel2:
                            if (Orientation == Orientation.Vertical)
                            {
                                _panelSize = Width - SplitterDistanceInternal - SplitterWidthInternal;
                            }
                            else
                            {
                                _panelSize = Height - SplitterDistanceInternal - SplitterWidthInternal;
                            }
                            break;
                        default:
                            _panelSize = SplitterDistanceInternal;
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///  This property determines whether the the splitter can move.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(false)]
        [Localizable(true)]
        [SRDescription(nameof(SR.SplitContainerIsSplitterFixedDescr))]

        public bool IsSplitterFixed { get; set; }

        //Private property used to check whether the splitter can be moved by the user.
        private bool IsSplitterMovable
        {
            get
            {
                if (Orientation == Orientation.Vertical)
                {
                    return (Width >= Panel1MinSize + SplitterWidthInternal + Panel2MinSize);
                }
                else
                {
                    return (Height >= Panel1MinSize + SplitterWidthInternal + Panel2MinSize);
                }
            }
        }

        // Refer to IsContainerControl property on Control for more details.
        internal override bool IsContainerControl
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  This Property sets or gets if the splitter is vertical or horizontal.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(Orientation.Vertical)]
        [Localizable(true)]
        [SRDescription(nameof(SR.SplitContainerOrientationDescr))]
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)Orientation.Horizontal, (int)Orientation.Vertical))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Orientation));
                }
                if (_orientation != value)
                {
                    _orientation = value;
                    //update the splitterDistance to validate it w.r.t the new Orientation.
                    _splitDistance = 0;
                    SplitterDistance = SplitterDistanceInternal;
                    UpdateSplitter();
                }
            }
        }

        private Cursor OverrideCursor
        {
            get
            {
                return _overrideCursor;
            }
            set
            {
                if (_overrideCursor != value)
                {
                    _overrideCursor = value;

                    if (IsHandleCreated)
                    {
                        // We want to instantly change the cursor if the mouse is within our bounds.
                        var r = new RECT();
                        User32.GetCursorPos(out Point p);
                        User32.GetWindowRect(this, ref r);
                        if ((r.left <= p.X && p.X < r.right && r.top <= p.Y && p.Y < r.bottom) || User32.GetCapture() == Handle)
                        {
                            User32.SendMessageW(this, User32.WM.SETCURSOR, Handle, (IntPtr)User32.HT.CLIENT);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates if either panel is collapsed
        /// </summary>
        private bool CollapsedMode
        {
            get
            {
                return Panel1Collapsed || Panel2Collapsed;
            }
        }

        /// <summary>
        ///  The Left or Top panel in the SplitContainer.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.SplitContainerPanel1Descr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SplitterPanel Panel1 { get; }

        /// <summary>
        ///  Collapses or restores the given panel
        /// </summary>
        private void CollapsePanel(SplitterPanel p, bool collapsing)
        {
            p.Collapsed = collapsing;
            if (collapsing)
            {
                p.Visible = false;
            }
            else
            {
                // restore panel
                p.Visible = true;
            }
            UpdateSplitter();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  Collapses or restores panel1
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.SplitContainerPanel1CollapsedDescr))]
        public bool Panel1Collapsed
        {
            get
            {
                return Panel1.Collapsed;
            }
            set
            {
                if (value != Panel1.Collapsed)
                {
                    if (value && Panel2.Collapsed)
                    {
                        CollapsePanel(Panel2, false);
                    }
                    CollapsePanel(Panel1, value);
                }
            }
        }

        /// <summary>
        ///  Collapses or restores panel2
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.SplitContainerPanel2CollapsedDescr))]
        public bool Panel2Collapsed
        {
            get
            {
                return Panel2.Collapsed;
            }
            set
            {
                if (value != Panel2.Collapsed)
                {
                    if (value && Panel1.Collapsed)
                    {
                        CollapsePanel(Panel1, false);
                    }
                    CollapsePanel(Panel2, value);
                }
            }
        }

        /// <summary>
        ///  This property determines the minimum distance of pixels of the splitter from the left or the top edge of Panel1.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(25)]
        [Localizable(true)]
        [SRDescription(nameof(SR.SplitContainerPanel1MinSizeDescr))]
        [RefreshProperties(RefreshProperties.All)]
        public int Panel1MinSize
        {
            get
            {
                return _panel1MinSize;
            }
            set
            {
                _newPanel1MinSize = value;
                if (value != Panel1MinSize && !_initializing)
                {
                    ApplyPanel1MinSize(value);
                }
            }
        }

        /// <summary>
        ///  This is the Right or Bottom panel in the SplitContainer.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.SplitContainerPanel2Descr))]
        [Localizable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SplitterPanel Panel2 { get; }

        /// <summary>
        ///  This property determines the minimum distance of pixels of the splitter from the right or the bottom edge of Panel2
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(25)]
        [Localizable(true)]
        [SRDescription(nameof(SR.SplitContainerPanel2MinSizeDescr))]
        [RefreshProperties(RefreshProperties.All)]
        public int Panel2MinSize
        {
            get
            {
                return _panel2MinSize;
            }
            set
            {
                _newPanel2MinSize = value;
                if (value != Panel2MinSize && !_initializing)
                {
                    ApplyPanel2MinSize(value);
                }
            }
        }

        /// <summary>
        ///  This property determines pixel distance of the splitter from the left or top edge.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true),
        SettingsBindable(true)]
        [SRDescription(nameof(SR.SplitContainerSplitterDistanceDescr))]
        [DefaultValue(50)]
        public int SplitterDistance
        {
            get
            {
                return _splitDistance;
            }
            set
            {
                if (value != SplitterDistance)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(SplitterDistance), string.Format(SR.InvalidLowBoundArgument, "SplitterDistance", value, 0));
                    }

                    try
                    {
                        _setSplitterDistance = true;

                        if (Orientation == Orientation.Vertical)
                        {
                            if (value < Panel1MinSize)
                            {
                                value = Panel1MinSize;
                            }
                            if (value + SplitterWidthInternal > Width - Panel2MinSize)
                            {
                                value = Width - Panel2MinSize - SplitterWidthInternal;
                            }
                            if (value < 0)
                            {
                                throw new InvalidOperationException(SR.SplitterDistanceNotAllowed);
                            }
                            _splitDistance = value;
                            _splitterDistance = value;
                            Panel1.WidthInternal = SplitterDistance;
                        }
                        else
                        {
                            if (value < Panel1MinSize)
                            {
                                value = Panel1MinSize;
                            }

                            if (value + SplitterWidthInternal > Height - Panel2MinSize)
                            {
                                value = Height - Panel2MinSize - SplitterWidthInternal;
                            }
                            if (value < 0)
                            {
                                throw new InvalidOperationException(SR.SplitterDistanceNotAllowed);
                            }
                            _splitDistance = value;
                            _splitterDistance = value;
                            Panel1.HeightInternal = SplitterDistance;
                        }

                        switch (_fixedPanel)
                        {
                            case FixedPanel.Panel1:
                                _panelSize = SplitterDistance;
                                break;
                            case FixedPanel.Panel2:
                                if (Orientation == Orientation.Vertical)
                                {
                                    _panelSize = Width - SplitterDistance - SplitterWidthInternal;
                                }
                                else
                                {
                                    _panelSize = Height - SplitterDistance - SplitterWidthInternal;
                                }
                                break;
                        }
                        UpdateSplitter();
                    }
                    finally
                    {
                        _setSplitterDistance = false;
                    }
                    OnSplitterMoved(new SplitterEventArgs(SplitterRectangle.X + SplitterRectangle.Width / 2, SplitterRectangle.Y + SplitterRectangle.Height / 2, SplitterRectangle.X, SplitterRectangle.Y));
                }
            }
        }

        private int SplitterDistanceInternal
        {
            get
            {
                return _splitterDistance;
            }
            set
            {
                SplitterDistance = value;
            }
        }

        /// <summary>
        ///  This determines the number of pixels the splitter moves in increments.This is defaulted to 1.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(1)]
        [Localizable(true)]
        [SRDescription(nameof(SR.SplitContainerSplitterIncrementDescr))]
        public int SplitterIncrement
        {
            get
            {
                return _splitterInc;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SplitterIncrement), value, 1));
                }

                _splitterInc = value;
            }
        }

        /// <summary>
        ///  This property determines the rectangle bounds of the splitter.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.SplitContainerSplitterRectangleDescr))]
        [Browsable(false)]
        public Rectangle SplitterRectangle
        {
            get
            {
                Rectangle r = _splitterRect;
                r.X = _splitterRect.X - Left;
                r.Y = _splitterRect.Y - Top;
                return r;
            }
        }

        /// <summary>
        ///  This property determines the thickness of the splitter.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.SplitContainerSplitterWidthDescr))]
        [Localizable(true)]
        [DefaultValue(4)]
        public int SplitterWidth
        {
            get
            {
                return _splitterWidth;
            }
            set
            {
                _newSplitterWidth = value;
                if (value != SplitterWidth && !_initializing)
                {
                    ApplySplitterWidth(value);
                }
            }
        }

        /// <summary>
        ///  We need to have a internal Property for the SplitterWidth which returns zero if we are in collapased mode.
        ///  This property is used to Layout SplitContainer.
        /// </summary>
        private int SplitterWidthInternal
        {
            get
            {
                // if CollapsedMode then splitterwidth == 0;
                return (CollapsedMode) ? 0 : _splitterWidth;
            }
        }

        /// <summary>
        ///  Indicates whether the user can give the focus to this control using the TAB
        ///  key. This property is read-only.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [DispId((int)Ole32.DispatchID.TABSTOP)]
        [SRDescription(nameof(SR.ControlTabStopDescr))]
        public new bool TabStop
        {
            get => _tabStop;
            set
            {
                if (TabStop != value)
                {
                    _tabStop = value;
                    OnTabStopChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //END PROPERTIES                                                              //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //Start PUBLIC FUNCTIONS                                                      //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  ISupportInitialize support. Disables splitter panel min size and splitter width
        ///  validation during initialization.
        /// </summary>
        public void BeginInit()
        {
            _initializing = true;
        }

        /// <summary>
        ///  ISupportInitialize support. Enables splitter panel min size and splitter width
        ///  validation after initialization.
        /// </summary>
        public void EndInit()
        {
            _initializing = false;

            // validate and apply new value
            if (_newPanel1MinSize != _panel1MinSize)
            {
                ApplyPanel1MinSize(_newPanel1MinSize);
            }
            if (_newPanel2MinSize != _panel2MinSize)
            {
                ApplyPanel2MinSize(_newPanel2MinSize);
            }
            if (_newSplitterWidth != _splitterWidth)
            {
                ApplySplitterWidth(_newSplitterWidth);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //End PUBLIC FUNCTIONS                                                        //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //Start EVENT HANDLERS                                                        //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.SplitterSplitterMovingDescr))]
        public event SplitterCancelEventHandler SplitterMoving
        {
            add => Events.AddHandler(s_eventMoving, value);
            remove => Events.RemoveHandler(s_eventMoving, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.SplitterSplitterMovedDescr))]
        public event SplitterEventHandler SplitterMoved
        {
            add => Events.AddHandler(s_eventMoved, value);
            remove => Events.RemoveHandler(s_eventMoved, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //End EVENT HANDLERS                                                          //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                            //
        //start EVENT Delegates                                                       //
        //                                                                            //
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Overides the Control.OnGotFocus to Invalidate...
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        /// <summary>
        ///  Overrides the Control.OnKeydown for implementing splitter movements.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Debug.Assert(Enabled, "SplitContainer.OnKeyDown should not be called if the button is disabled");
            base.OnKeyDown(e);
            //If the Panel1MinSize + Panel2MinSize < SplitContainer.Size then carry on the splitter move...
            if (IsSplitterMovable && !IsSplitterFixed)
            {
                if (e.KeyData == Keys.Escape && _splitBegin)
                {
                    _splitBegin = false;
                    _splitBreak = true;
                    return;
                }
                //valid Keys that move the splitter...
                if (_splitterFocused &&
                    (e.KeyData == Keys.Right || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Up))
                {
                    if (_splitBegin)
                    {
                        _splitMove = true;
                    }

                    //left OR up
                    if (_splitterFocused && (e.KeyData == Keys.Left || e.KeyData == Keys.Up))
                    {
                        _splitterDistance -= SplitterIncrement;
                        _splitterDistance = (_splitterDistance < Panel1MinSize) ? _splitterDistance + SplitterIncrement : Math.Max(_splitterDistance, _borderSize);
                    }
                    //right OR down
                    if (_splitterFocused && (e.KeyData == Keys.Right || e.KeyData == Keys.Down))
                    {
                        _splitterDistance += SplitterIncrement;
                        if (Orientation == Orientation.Vertical)
                        {
                            _splitterDistance = (_splitterDistance + SplitterWidth > Width - Panel2MinSize - _borderSize) ? _splitterDistance - SplitterIncrement : _splitterDistance;
                        }
                        else
                        {
                            _splitterDistance = (_splitterDistance + SplitterWidth > Height - Panel2MinSize - _borderSize) ? _splitterDistance - SplitterIncrement : _splitterDistance;
                        }
                    }

                    if (!_splitBegin)
                    {
                        _splitBegin = true;
                    }
                    //draw Helper start
                    if (_splitBegin && !_splitMove)
                    {
                        _initialSplitterDistance = SplitterDistanceInternal;
                        DrawSplitBar(DrawStart);
                    }
                    else
                    { //draw helper move
                        DrawSplitBar(DrawMove);
                        //Moving by mouse .....gives the origin of the splitter..
                        //
                        Rectangle r = CalcSplitLine(_splitterDistance, 0);
                        int xSplit = r.X;
                        int ySplit = r.Y;
                        SplitterCancelEventArgs se = new SplitterCancelEventArgs(Left + SplitterRectangle.X + SplitterRectangle.Width / 2, Top + SplitterRectangle.Y + SplitterRectangle.Height / 2, xSplit, ySplit);
                        OnSplitterMoving(se);
                        if (se.Cancel)
                        {
                            SplitEnd(false);
                        }
                    }
                } //End Valid Keys....
            } //End SplitterFixed Check...
        }

        /// <summary>
        ///  Overrides the Control.OnKeydown for implementing splitter movements.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (_splitBegin && IsSplitterMovable)
            {
                if (_splitterFocused &&
                    (e.KeyData == Keys.Right || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Up))
                {
                    DrawSplitBar(DrawEnd);
                    ApplySplitterDistance();
                    _splitBegin = false;
                    _splitMove = false;
                }
            }
            if (_splitBreak)
            {
                _splitBreak = false;
                SplitEnd(false);
            }

            // Problem after KeyUp- focus rect and reversible lines leave a trace behind on the splitter.
            using Graphics g = CreateGraphicsInternal();
            if (BackgroundImage is null)
            {
                using var brush = BackColor.GetCachedSolidBrushScope();
                g.FillRectangle(brush, SplitterRectangle);
            }

            DrawFocus(g, SplitterRectangle);
        }

        /// <summary>
        ///  Overrides the Control.OnLayout.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            SetInnerMostBorder(this);

            if (IsSplitterMovable && !_setSplitterDistance)
            {
                ResizeSplitContainer();
            }
            base.OnLayout(e);
        }

        /// <summary>
        ///  Overrides the Control.OnLostFocus to Invalidate.
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!IsSplitterFixed && IsSplitterMovable)
            {
                //change cursor if default and user hasnt changed the cursor.
                if (Cursor == DefaultCursor && SplitterRectangle.Contains(e.Location))
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        OverrideCursor = Cursors.VSplit;
                    }
                    else
                    {
                        OverrideCursor = Cursors.HSplit;
                    }
                }
                else
                {
                    OverrideCursor = null;
                    ;
                }

                if (_splitterClick)
                {
                    int x = e.X;
                    int y = e.Y;
                    _splitterDrag = true;
                    SplitMove(x, y);
                    if (Orientation == Orientation.Vertical)
                    {
                        x = Math.Max(Math.Min(x, Width - Panel2MinSize), Panel1MinSize);
                        y = Math.Max(y, 0);
                    }
                    else
                    {
                        y = Math.Max(Math.Min(y, Height - Panel2MinSize), Panel1MinSize);
                        x = Math.Max(x, 0);
                    }
                    Rectangle r = CalcSplitLine(GetSplitterDistance(e.X, e.Y), 0);
                    int xSplit = r.X;
                    int ySplit = r.Y;
                    SplitterCancelEventArgs se = new SplitterCancelEventArgs(x, y, xSplit, ySplit);
                    OnSplitterMoving(se);
                    if (se.Cancel)
                    {
                        SplitEnd(false);
                    }
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='OnMouseLeave'/> event.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!Enabled)
            {
                return;
            }
            OverrideCursor = null;
        }

        /// <summary>
        ///  Raises the <see cref='OnMouseDown'/> event.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            //If the Panel1MinSize + Panel2MinSize < SplitContainer.Size then carry on the splitter move...
            if (IsSplitterMovable && SplitterRectangle.Contains(e.Location))
            {
                if (!Enabled)
                {
                    return;
                }
                if (e.Button == MouseButtons.Left && e.Clicks == 1 && !IsSplitterFixed)
                {
                    // Focus the current splitter OnMouseDown.
                    _splitterFocused = true;
                    IContainerControl c = ParentInternal.GetContainerControl();
                    if (c != null)
                    {
                        if (!(c is ContainerControl cc))
                        {
                            c.ActiveControl = this;
                        }
                        else
                        {
                            cc.SetActiveControl(this);
                        }
                    }
                    SetActiveControl(null);
                    _nextActiveControl = Panel2;

                    SplitBegin(e.X, e.Y);
                    _splitterClick = true;
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='OnMouseUp'/> event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!Enabled)
            {
                return;
            }
            if (!IsSplitterFixed && IsSplitterMovable && _splitterClick)
            {
                Capture = false;

                if (_splitterDrag)
                {
                    CalcSplitLine(GetSplitterDistance(e.X, e.Y), 0);
                    SplitEnd(true);
                }
                else
                {
                    SplitEnd(false);
                }
                _splitterClick = false;
                _splitterDrag = false;
            }
        }

        /// <summary>
        ///  Overrides the Control.OnMove() to synchronize the
        ///  splitterRect with the position of the SplitContainer.
        /// </summary>
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            SetSplitterRect(Orientation == Orientation.Vertical);
        }

        /// <summary>
        ///  Overrides the Control.OnPaint() to focus the Splitter.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Focused)
            {
                DrawFocus(e.Graphics, SplitterRectangle);
            }
        }

        /// <summary>
        ///  Inherriting classes should override this method to respond to the
        ///  splitterMoving event. This event occurs while the splitter is
        ///  being moved by the user.
        /// </summary>
        public void OnSplitterMoving(SplitterCancelEventArgs e)
        {
            ((SplitterCancelEventHandler)Events[s_eventMoving])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inherriting classes should override this method to respond to the
        ///  splitterMoved event. This event occurs when the user finishes
        ///  moving the splitter.
        /// </summary>
        public void OnSplitterMoved(SplitterEventArgs e)
        {
            ((SplitterEventHandler)Events[s_eventMoved])?.Invoke(this, e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                            //
        ///END DELEGATES                                                                              //
        //                                                                                            //
        ////////////////////////////////////////////////////////////////////////////////////////////////
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            // pass the RightToLeft value to the Parent.
            Panel1.RightToLeft = RightToLeft;
            Panel2.RightToLeft = RightToLeft;
            UpdateSplitter();
        }

        /// <summary>
        ///  Validate and set the minimum size for Panel1.
        /// </summary>
        private void ApplyPanel1MinSize(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgument, nameof(Panel1MinSize), value));
            }

            if (Orientation == Orientation.Vertical)
            {
                if (DesignMode && Width != DefaultSize.Width && value + Panel2MinSize + SplitterWidth > Width)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(Panel1MinSize), value));
                }
            }
            else if (Orientation == Orientation.Horizontal)
            {
                if (DesignMode && Height != DefaultSize.Height && value + Panel2MinSize + SplitterWidth > Height)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(Panel1MinSize), value));
                }
            }

            _panel1MinSize = value;
            if (value > SplitterDistanceInternal)
            {
                SplitterDistanceInternal = value;  //Set the Splitter Distance to the end of Panel1
            }
        }

        /// <summary>
        ///  Validate and set the minimum size for Panel2.
        /// </summary>
        private void ApplyPanel2MinSize(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgument, nameof(Panel2MinSize), value, 0));
            }
            if (Orientation == Orientation.Vertical)
            {
                if (DesignMode && Width != DefaultSize.Width && value + Panel1MinSize + SplitterWidth > Width)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(Panel2MinSize), value.ToString()));
                }
            }
            else if (Orientation == Orientation.Horizontal)
            {
                if (DesignMode && Height != DefaultSize.Height && value + Panel1MinSize + SplitterWidth > Height)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(Panel2MinSize), value));
                }
            }
            _panel2MinSize = value;
            if (value > Panel2.Width)
            {
                SplitterDistanceInternal = Panel2.Width + SplitterWidthInternal;  //Set the Splitter Distance to the start of Panel2
            }
        }

        /// <summary>
        ///  Validate and set the splitter width.
        /// </summary>
        private void ApplySplitterWidth(int value)
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SplitterWidth), value, 1));
            }
            if (Orientation == Orientation.Vertical)
            {
                if (DesignMode && value + Panel1MinSize + Panel2MinSize > Width)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SplitterWidth), value));
                }
            }
            else if (Orientation == Orientation.Horizontal)
            {
                if (DesignMode && value + Panel1MinSize + Panel2MinSize > Height)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SplitterWidth), value));
                }
            }
            _splitterWidth = value;
            UpdateSplitter();
        }

        /// <summary>
        ///  Sets the split position to be the current split size. This is called
        ///  by splitEdit
        /// </summary>
        private void ApplySplitterDistance()
        {
            using (new Layout.LayoutTransaction(this, this, "SplitterDistance", false))
            {
                SplitterDistanceInternal = _splitterDistance;
            }

            // We need to invalidate when we have transparent backgournd.
            if (BackColor == Color.Transparent)
            {
                // the panel1 retains the focus rect... so Invalidate the rect ...
                Invalidate();
            }

            if (Orientation == Orientation.Vertical)
            {
                if (RightToLeft == RightToLeft.No)
                {
                    _splitterRect.X = Location.X + SplitterDistanceInternal;
                }
                else
                {
                    _splitterRect.X = Right - SplitterDistanceInternal - SplitterWidthInternal;
                }
            }
            else
            {
                _splitterRect.Y = Location.Y + SplitterDistanceInternal;
            }
        }

        /// <summary>
        ///  Calculates the bounding rect of the split line. minWeight refers
        ///  to the minimum height or width of the splitline.
        /// </summary>
        private Rectangle CalcSplitLine(int splitSize, int minWeight)
        {
            Rectangle r = new Rectangle();
            switch (Orientation)
            {
                case Orientation.Vertical:
                    r.Width = SplitterWidthInternal;
                    r.Height = Height;
                    if (r.Width < minWeight)
                    {
                        r.Width = minWeight;
                    }

                    if (RightToLeft == RightToLeft.No)
                    {
                        r.X = Panel1.Location.X + splitSize;
                    }
                    else
                    {
                        r.X = Width - splitSize - SplitterWidthInternal;
                    }

                    break;

                case Orientation.Horizontal:
                    r.Width = Width;
                    r.Height = SplitterWidthInternal;
                    if (r.Width < minWeight)
                    {
                        r.Width = minWeight;
                    }
                    r.Y = Panel1.Location.Y + splitSize;
                    break;
            }
            return r;
        }

        /// <summary>
        ///  Draws the splitter bar at the current location. Will automatically
        ///  cleanup anyplace the splitter was drawn previously.
        /// </summary>
        private void DrawSplitBar(int mode)
        {
            if (mode != DrawStart && _lastDrawSplit != -1)
            {
                DrawSplitHelper(_lastDrawSplit);
                _lastDrawSplit = -1;
            }
            // Bail if drawing with no old point...
            //
            else if (mode != DrawStart && _lastDrawSplit == -1)
            {
                return;
            }

            if (mode != DrawEnd)
            {
                if (_splitMove || _splitBegin)
                { // Splitter is moved by keys and not by mouse
                    DrawSplitHelper(_splitterDistance);
                    _lastDrawSplit = _splitterDistance;
                }
                else
                {
                    DrawSplitHelper(_splitterDistance);
                    _lastDrawSplit = _splitterDistance;
                }
            }
            else
            {
                if (_lastDrawSplit != -1)
                {
                    DrawSplitHelper(_lastDrawSplit);
                }
                _lastDrawSplit = -1;
            }
        }

        /// <summary>
        ///  Draws the focus rectangle if the control has focus.
        /// </summary>
        private void DrawFocus(Graphics g, Rectangle r)
        {
            r.Inflate(-1, -1);
            ControlPaint.DrawFocusRectangle(g, r, ForeColor, BackColor);
        }

        /// <summary>
        ///  Draws the splitter line at the requested location. Should only be called
        ///  by drawSpltBar.
        /// </summary>
        private void DrawSplitHelper(int splitSize)
        {
            Rectangle r = CalcSplitLine(splitSize, 3);
            using var dc = new User32.GetDcScope(Handle, IntPtr.Zero, User32.DCX.CACHE | User32.DCX.LOCKWINDOWUPDATE);
            Gdi32.HBRUSH halftone = ControlPaint.CreateHalftoneHBRUSH();
            using var objectScope = new Gdi32.ObjectScope(halftone);
            using var selectBrush = new Gdi32.SelectObjectScope(dc, halftone);
            Gdi32.PatBlt(dc, r.X, r.Y, r.Width, r.Height, Gdi32.ROP.PATINVERT);

            GC.KeepAlive(this);
        }

        /// <summary>
        ///  Calculates the split size based on the mouse position (x, y).
        /// </summary>
        private int GetSplitterDistance(int x, int y)
        {
            int delta;
            if (Orientation == Orientation.Vertical)
            {
                delta = x - _anchor.X;
            }
            else
            {
                delta = y - _anchor.Y;
            }

            // Negative delta - moving to the left
            // Positive delta - moving to the right

            int size = 0;
            switch (Orientation)
            {
                case Orientation.Vertical:
                    if (RightToLeft == RightToLeft.No)
                    {
                        size = Math.Max(Panel1.Width + delta, _borderSize);
                    }
                    else
                    {
                        // In RTL negative delta actually means increasing the size....
                        size = Math.Max(Panel1.Width - delta, _borderSize);
                    }
                    break;
                case Orientation.Horizontal:
                    size = Math.Max(Panel1.Height + delta, _borderSize);
                    break;
            }
            if (Orientation == Orientation.Vertical)
            {
                return Math.Max(Math.Min(size, Width - Panel2MinSize), Panel1MinSize);
            }
            else
            {
                return Math.Max(Math.Min(size, Height - Panel2MinSize), Panel1MinSize);
            }
        }

        /// <summary>
        ///  Process an arrowKey press by selecting the next control in the group
        ///  that the activeControl belongs to.
        /// </summary>
        private bool ProcessArrowKey(bool forward)
        {
            Control group = this;
            if (ActiveControl != null)
            {
                group = ActiveControl.ParentInternal;
            }
            return group.SelectNextControl(ActiveControl, forward, false, false, true);
        }

        /// <summary>
        ///  Re paint SplitterRect for SplitContainer
        /// </summary>
        private void RepaintSplitterRect()
        {
            if (IsHandleCreated)
            {
                Graphics g = CreateGraphicsInternal();
                if (BackgroundImage != null)
                {
                    using TextureBrush textureBrush = new TextureBrush(BackgroundImage, WrapMode.Tile);
                    g.FillRectangle(textureBrush, ClientRectangle);
                }
                else
                {
                    using var solidBrush = BackColor.GetCachedSolidBrushScope();
                    g.FillRectangle(solidBrush, _splitterRect);
                }
                g.Dispose();
            }
        }

        private void SetSplitterRect(bool vertical)
        {
            if (vertical)
            {
                _splitterRect.X = ((RightToLeft == RightToLeft.Yes) ? Width - _splitterDistance - SplitterWidthInternal : Location.X + _splitterDistance);
                _splitterRect.Y = Location.Y;
                _splitterRect.Width = SplitterWidthInternal;
                _splitterRect.Height = Height;
            }
            else
            {
                _splitterRect.X = Location.X;
                _splitterRect.Y = Location.Y + SplitterDistanceInternal;
                _splitterRect.Width = Width;
                _splitterRect.Height = SplitterWidthInternal;
            }
        }

        /// <summary>
        ///  Reize SplitContainer
        /// </summary>
        private void ResizeSplitContainer()
        {
            if (_splitContainerScaling)
            {
                return;
            }

            Panel1.SuspendLayout();
            Panel2.SuspendLayout();

            if (Width == 0)
            {         // Set the correct Width iif the WIDTH has changed to ZERO.
                Panel1.Size = new Size(0, Panel1.Height);
                Panel2.Size = new Size(0, Panel2.Height);
            }
            else if (Height == 0)
            {   // Set the correct Height iif the HEIGHT has changed to ZERO.
                Panel1.Size = new Size(Panel1.Width, 0);
                Panel2.Size = new Size(Panel2.Width, 0);
            }
            else
            {
                if (Orientation == Orientation.Vertical)
                {
                    // If no panel is collapsed then do the default ...
                    if (!CollapsedMode)
                    {
                        if (FixedPanel == FixedPanel.Panel1)
                        {
                            Panel1.Size = new Size(_panelSize, Height);
                            Panel2.Size = new Size(Math.Max(Width - _panelSize - SplitterWidthInternal, Panel2MinSize), Height);
                        }
                        if (FixedPanel == FixedPanel.Panel2)
                        {
                            Panel2.Size = new Size(_panelSize, Height);
                            _splitterDistance = Math.Max(Width - _panelSize - SplitterWidthInternal, Panel1MinSize);
                            Panel1.WidthInternal = _splitterDistance;
                            Panel1.HeightInternal = Height;
                        }
                        if (FixedPanel == FixedPanel.None)
                        {
                            if (_ratioWidth != 0.0)
                            {
                                _splitterDistance = Math.Max((int)(Math.Floor(Width / _ratioWidth)), Panel1MinSize);
                            }
                            Panel1.WidthInternal = _splitterDistance; //Default splitter distance from left or top.
                            Panel1.HeightInternal = Height;
                            Panel2.Size = new Size(Math.Max(Width - _splitterDistance - SplitterWidthInternal, Panel2MinSize), Height);
                        }
                        if (RightToLeft == RightToLeft.No)
                        {
                            Panel2.Location = new Point(Panel1.WidthInternal + SplitterWidthInternal, 0);
                        }
                        else
                        {
                            Panel1.Location = new Point(Width - Panel1.WidthInternal, 0);
                        }
                        RepaintSplitterRect();
                        SetSplitterRect(true);
                    }
                    else
                    {
                        if (Panel1Collapsed)
                        {
                            Panel2.Size = Size;
                            Panel2.Location = new Point(0, 0);
                        }
                        else if (Panel2Collapsed)
                        {
                            Panel1.Size = Size;
                            Panel1.Location = new Point(0, 0);
                        }
                    }
                }
                else if (Orientation == Orientation.Horizontal)
                {
                    // If no panel is collapsed then do the default ...
                    if (!CollapsedMode)
                    {
                        if (FixedPanel == FixedPanel.Panel1)
                        {
                            //Default splitter distance from left or top.
                            Panel1.Size = new Size(Width, _panelSize);
                            int panel2Start = _panelSize + SplitterWidthInternal;
                            Panel2.Size = new Size(Width, Math.Max(Height - panel2Start, Panel2MinSize));
                            Panel2.Location = new Point(0, panel2Start);
                        }
                        if (FixedPanel == FixedPanel.Panel2)
                        {
                            Panel2.Size = new Size(Width, _panelSize);
                            _splitterDistance = Math.Max(Height - Panel2.Height - SplitterWidthInternal, Panel1MinSize);
                            Panel1.HeightInternal = _splitterDistance;
                            Panel1.WidthInternal = Width;
                            int panel2Start = _splitterDistance + SplitterWidthInternal;
                            Panel2.Location = new Point(0, panel2Start);
                        }
                        if (FixedPanel == FixedPanel.None)
                        {
                            //NO PANEL FIXED !!
                            if (_ratioHeight != 0.0)
                            {
                                _splitterDistance = Math.Max((int)(Math.Floor(Height / _ratioHeight)), Panel1MinSize);
                            }
                            Panel1.HeightInternal = _splitterDistance; //Default splitter distance from left or top.
                            Panel1.WidthInternal = Width;
                            int panel2Start = _splitterDistance + SplitterWidthInternal;
                            Panel2.Size = new Size(Width, Math.Max(Height - panel2Start, Panel2MinSize));
                            Panel2.Location = new Point(0, panel2Start);
                        }
                        RepaintSplitterRect();
                        SetSplitterRect(false);
                    }
                    else
                    {
                        if (Panel1Collapsed)
                        {
                            Panel2.Size = Size;
                            Panel2.Location = new Point(0, 0);
                        }
                        else if (Panel2Collapsed)
                        {
                            Panel1.Size = Size;
                            Panel1.Location = new Point(0, 0);
                        }
                    }
                }
                try
                {
                    _resizeCalled = true;
                    ApplySplitterDistance();
                }
                finally
                {
                    _resizeCalled = false;
                }
            }
            Panel1.ResumeLayout();
            Panel2.ResumeLayout();
        }

        /// <summary>
        ///  Scales an individual control's location, size, padding and margin.
        ///  If the control is top level, this will not scale the control's location.
        ///  This does not scale children or the size of auto sized controls.  You can
        ///  omit scaling in any direction by changing BoundsSpecified.
        ///
        ///  After the control is scaled the RequiredScaling property is set to
        ///  BoundsSpecified.None.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            try
            {
                _splitContainerScaling = true;
                base.ScaleControl(factor, specified);

                float scale;
                if (_orientation == Orientation.Vertical)
                {
                    scale = factor.Width;
                }
                else
                {
                    scale = factor.Height;
                }
                SplitterWidth = (int)Math.Round((float)SplitterWidth * scale);
            }
            finally
            {
                _splitContainerScaling = false;
            }
        }

        protected override void Select(bool directed, bool forward)
        {
            // avoid re-entrant code.
            // SelectNextControl can call back on us.. and we might end up infinitely recursing.
            if (_selectNextControl)
            {
                return;
            }
            // continue selection iff panels have controls or tabstop is true.
            if ((Panel1.Controls.Count > 0 || Panel2.Controls.Count > 0) || TabStop)
            {
                SelectNextControlInContainer(this, forward, true, true, false);
            }
            else
            { //If this SplitContainer cannot be selected let the parent select the next in line
                try
                {
                    Control parent = ParentInternal;
                    _selectNextControl = true;
                    while (parent != null)
                    {
                        if (parent.SelectNextControl(this, forward, true, true, parent.ParentInternal is null))
                        {
                            break;
                        }
                        parent = parent.ParentInternal;
                    }
                }
                finally
                {
                    _selectNextControl = false;
                }
            }
        }

        /// <summary>
        ///  Selects the next control following ctl.
        /// </summary>
        private bool SelectNextControlInContainer(Control ctl, bool forward, bool tabStopOnly,
                                      bool nested, bool wrap)
        {
#pragma warning disable SA1408 // Conditional expressions should declare precedence
            if (!Contains(ctl) || !nested && ctl.ParentInternal != this)
#pragma warning restore SA1408 // Conditional expressions should declare precedence
            {
                ctl = null;
            }

            Control start = ctl;
            SplitterPanel firstPanel = null;
            do
            {
                ctl = GetNextControl(ctl, forward);

                if (ctl is SplitterPanel panel && panel.Visible)
                {
                    //We have crossed over to the second Panel...
                    if (firstPanel != null)
                    {
                        break;
                    }
                    firstPanel = panel;
                }
                if (!forward && firstPanel != null && ctl.ParentInternal != firstPanel)
                {
                    //goback to start correct re-ordering ....
                    ctl = firstPanel;
                    break;
                }
                if (ctl is null)
                {
                    break;
                }
                else
                {
                    if (ctl.CanSelect && ctl.TabStop)
                    {
                        if (ctl is SplitContainer)
                        {
                            ((SplitContainer)ctl).Select(forward, forward);
                        }
                        else
                        {
                            SelectNextActiveControl(ctl, forward, tabStopOnly, nested, wrap);
                        }
                        return true;
                    }
                }
            } while (ctl != null);
            if (ctl != null && TabStop)
            {
                //we are on Splitter.....Focus it
                _splitterFocused = true;
                IContainerControl c = ParentInternal.GetContainerControl();
                if (c != null)
                {
                    if (!(c is ContainerControl cc))
                    {
                        c.ActiveControl = this;
                    }
                    else
                    {
                        cc.SetActiveControl(this);
                    }
                }
                SetActiveControl(null);
                _nextActiveControl = ctl;
                return true;
            }
            else
            {
                // If the splitter cannot be selected select the next control in the splitter
                bool selected = SelectNextControlInPanel(ctl, forward, tabStopOnly, nested, wrap);
                if (!selected)
                {
                    Control parent = ParentInternal;
                    if (parent != null)
                    {
                        try
                        {
                            _selectNextControl = true;
                            parent.SelectNextControl(this, forward, true, true, true);
                        }
                        finally
                        {
                            _selectNextControl = false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  Selects the next control following ctl.
        /// </summary>
        private bool SelectNextControlInPanel(Control ctl, bool forward, bool tabStopOnly,
                                      bool nested, bool wrap)
        {
#pragma warning disable SA1408 // Conditional expressions should declare precedence
            if (!Contains(ctl) || !nested && ctl.ParentInternal != this)
#pragma warning restore SA1408 // Conditional expressions should declare precedence
            {
                ctl = null;
            }

            Control start = ctl;
            do
            {
                ctl = GetNextControl(ctl, forward);
                if (ctl is null || (ctl is SplitterPanel && ctl.Visible))
                {
                    break;
                }
                else
                {
                    if (ctl.CanSelect && (!tabStopOnly || ctl.TabStop))
                    {
                        if (ctl is SplitContainer)
                        {
                            ((SplitContainer)ctl).Select(forward, forward);
                        }
                        else
                        {
                            SelectNextActiveControl(ctl, forward, tabStopOnly, nested, wrap);
                        }
                        return true;
                    }
                }
            } while (ctl != null);

            //If CTL is null .. we r out of the Current SplitContainer...
            if (ctl is null || (ctl is SplitterPanel && !ctl.Visible))
            {
                _callBaseVersion = true;
            }
            //IF the CTL == typeof(SpliterPanel) find the NEXT Control... so that we know
            // we can focus the NEXT control within this SPLITCONTAINER....
            else
            {
                ctl = GetNextControl(ctl, forward);
                if (forward)
                {
                    _nextActiveControl = Panel2;
                }
                else
                {
                    if (ctl is null || !(ctl.ParentInternal.Visible))
                    {
                        _callBaseVersion = true;
                    }
                    else
                    {
                        _nextActiveControl = Panel2;
                    }
                }
            }
            return false;
        }

        // This will select the correct active control in the containerControl (if the passed in control is a containerControl)
        private static void SelectNextActiveControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            if (ctl is ContainerControl container)
            {
                bool correctParentActiveControl = true;
                if (container.ParentInternal != null)
                {
                    IContainerControl c = container.ParentInternal.GetContainerControl();
                    if (c != null)
                    {
                        c.ActiveControl = container;
                        correctParentActiveControl = (c.ActiveControl == container);
                    }
                }
                if (correctParentActiveControl)
                {
                    ctl.SelectNextControl(null, forward, tabStopOnly, nested, wrap);
                }
            }
            else
            {
                ctl.Select();
            }
        }

        /// <summary>
        ///  Selects the innermost PANEL.
        /// </summary>
        private void SetInnerMostBorder(SplitContainer sc)
        {
            foreach (Control ctl in sc.Controls)
            {
                bool foundChildSplitContainer = false;
                if (ctl is SplitterPanel)
                {
                    foreach (Control c in ctl.Controls)
                    {
                        if (c is SplitContainer c1 && c1.Dock == DockStyle.Fill)
                        {
                            // We need to Overlay borders
                            // if the Children have matching BorderStyles ...
                            if (c1.BorderStyle != BorderStyle)
                            {
                                break;
                            }
                           ((SplitterPanel)ctl).BorderStyle = BorderStyle.None;
                            SetInnerMostBorder(c1);
                            foundChildSplitContainer = true;
                        }
                    }
                    if (!foundChildSplitContainer)
                    {
                        ((SplitterPanel)ctl).BorderStyle = BorderStyle;
                    }
                }
            }
        }

        /// <summary>
        ///  This protected override allows us to check is an unvalid value is set for Width and Height.
        ///  The SplitContainer would not throw on invalid Size (i.e Width and Height) settings, but would correct the error like Form
        ///  Say, the Panel1MinSize == 150 , Panel2MinSize == 50 and SplitterWidth == 4 and the user tries
        ///  to set SplitContainer.Width = 50 ... then this function would try to correct the value to 204.. instead of throwing.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // If we are changing Height, check if its greater than minimun else ... make it equal to the minimum
            if ((specified & BoundsSpecified.Height) != BoundsSpecified.None && Orientation == Orientation.Horizontal)
            {
                if (height < Panel1MinSize + SplitterWidthInternal + Panel2MinSize)
                {
                    height = Panel1MinSize + SplitterWidthInternal + Panel2MinSize;
                }
            }

            // If we are changing Width, check if its greater than minimun else ... make it equal to the minimum
            if ((specified & BoundsSpecified.Width) != BoundsSpecified.None && Orientation == Orientation.Vertical)
            {
                if (width < Panel1MinSize + SplitterWidthInternal + Panel2MinSize)
                {
                    width = Panel1MinSize + SplitterWidthInternal + Panel2MinSize;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);

            SetSplitterRect(Orientation == Orientation.Vertical);
        }

        /// <summary>
        ///  Begins the splitter moving.
        /// </summary>
        private void SplitBegin(int x, int y)
        {
            _anchor = new Point(x, y);
            _splitterDistance = GetSplitterDistance(x, y);
            _initialSplitterDistance = _splitterDistance;
            _initialSplitterRectangle = SplitterRectangle;

            if (_splitContainerMessageFilter is null)
            {
                _splitContainerMessageFilter = new SplitContainerMessageFilter(this);
            }
            Application.AddMessageFilter(_splitContainerMessageFilter);

            Capture = true;
            DrawSplitBar(DrawStart);
        }

        /// <summary>
        ///  The split movement.
        /// </summary>
        private void SplitMove(int x, int y)
        {
            int size = GetSplitterDistance(x, y);
            int delta = size - _initialSplitterDistance;
            int mod = delta % SplitterIncrement;
            if (_splitterDistance != size)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (size + SplitterWidthInternal <= Width - Panel2MinSize - _borderSize)
                    {
                        _splitterDistance = size - mod;
                    }
                }
                else
                {
                    if (size + SplitterWidthInternal <= Height - Panel2MinSize - _borderSize)
                    {
                        _splitterDistance = size - mod;
                    }
                }
            }
            DrawSplitBar(DrawMove);
        }

        /// <summary>
        ///  Finishes the split movement.
        /// </summary>
        private void SplitEnd(bool accept)
        {
            DrawSplitBar(DrawEnd);
            if (_splitContainerMessageFilter != null)
            {
                Application.RemoveMessageFilter(_splitContainerMessageFilter);
                _splitContainerMessageFilter = null;
            }

            if (accept)
            {
                ApplySplitterDistance();
            }
            else if (_splitterDistance != _initialSplitterDistance)
            {
                _splitterClick = false;
                _splitterDistance = SplitterDistanceInternal = _initialSplitterDistance;
            }
            _anchor = Point.Empty;
        }

        /// <summary>
        ///  Update Splitter
        /// </summary>
        private void UpdateSplitter()
        {
            if (_splitContainerScaling)
            {
                return;
            }
            Panel1.SuspendLayout();
            Panel2.SuspendLayout();
            if (Orientation == Orientation.Vertical)
            {
                bool isRTL = RightToLeft == RightToLeft.Yes;

                //NO PANEL FIXED !!
                if (!CollapsedMode)
                {
                    Panel1.HeightInternal = Height;
                    Panel1.WidthInternal = _splitterDistance; //Default splitter distance from left or top.
                    Panel2.Size = new Size(Width - _splitterDistance - SplitterWidthInternal, Height);

                    if (!isRTL)
                    {
                        Panel1.Location = new Point(0, 0);
                        Panel2.Location = new Point(_splitterDistance + SplitterWidthInternal, 0);
                    }
                    else
                    {
                        Panel1.Location = new Point(Width - _splitterDistance, 0);
                        Panel2.Location = new Point(0, 0);
                    }

                    RepaintSplitterRect();
                    SetSplitterRect(true /*Vertical*/);
                    if (!_resizeCalled)
                    {
                        _ratioWidth = ((double)(Width) / (double)(Panel1.Width) > 0) ? (double)(Width) / (double)(Panel1.Width) : _ratioWidth;
                    }
                }
                else
                {
                    if (Panel1Collapsed)
                    {
                        Panel2.Size = Size;
                        Panel2.Location = new Point(0, 0);
                    }
                    else if (Panel2Collapsed)
                    {
                        Panel1.Size = Size;
                        Panel1.Location = new Point(0, 0);
                    }
                    // Update Ratio when the splitContainer is in CollapsedMode.
                    if (!_resizeCalled)
                    {
                        _ratioWidth = ((double)(Width) / (double)(_splitterDistance) > 0) ? (double)(Width) / (double)(_splitterDistance) : _ratioWidth;
                    }
                }
            }
            else
            {
                //NO PANEL FIXED !!
                if (!CollapsedMode)
                {
                    Panel1.Location = new Point(0, 0);
                    Panel1.WidthInternal = Width;

                    Panel1.HeightInternal = SplitterDistanceInternal; //Default splitter distance from left or top.
                    int panel2Start = _splitterDistance + SplitterWidthInternal;
                    Panel2.Size = new Size(Width, Height - panel2Start);
                    Panel2.Location = new Point(0, panel2Start);

                    RepaintSplitterRect();
                    SetSplitterRect(false/*Horizontal*/);

                    if (!_resizeCalled)
                    {
                        _ratioHeight = ((double)(Height) / (double)(Panel1.Height) > 0) ? (double)(Height) / (double)(Panel1.Height) : _ratioHeight;
                    }
                }
                else
                {
                    if (Panel1Collapsed)
                    {
                        Panel2.Size = Size;
                        Panel2.Location = new Point(0, 0);
                    }
                    else if (Panel2Collapsed)
                    {
                        Panel1.Size = Size;
                        Panel1.Location = new Point(0, 0);
                    }

                    // Update Ratio when the splitContainer is in CollapsedMode.
                    if (!_resizeCalled)
                    {
                        _ratioHeight = ((double)(Height) / (double)(_splitterDistance) > 0) ? (double)(Height) / (double)(_splitterDistance) : _ratioHeight;
                    }
                }
            }
            Panel1.ResumeLayout();
            Panel2.ResumeLayout();
        }

        /// <summary>
        ///  Handles the WM_SETCURSOR message
        /// </summary>
        private void WmSetCursor(ref Message m)
        {
            // Accessing through the Handle property has side effects that break this
            // logic. You must use InternalHandle.
            if (m.WParam == InternalHandle && ((int)m.LParam & 0x0000FFFF) == (int)User32.HT.CLIENT)
            {
                Cursor.Current = OverrideCursor ?? Cursor;
            }
            else
            {
                DefWndProc(ref m);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                               //
        // END PRIVATE FUNCTIONS ...                                                                     //
        //                                                                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                               //
        // Start PROTECTED OVERRIDE FUNCTIONS                                                            //
        //                                                                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        internal override Rectangle GetToolNativeScreenRectangle()
        {
            // Return splitter rectangle instead of the whole container rectangle to be consistent with the mouse ToolTip
            Rectangle containerRectangle = base.GetToolNativeScreenRectangle();
            Rectangle splitterRectangle = SplitterRectangle;
            return new Rectangle(containerRectangle.X + splitterRectangle.X, containerRectangle.Y + splitterRectangle.Y, splitterRectangle.Width, splitterRectangle.Height);
        }

        internal override void AfterControlRemoved(Control control, Control oldParent)
        {
            base.AfterControlRemoved(control, oldParent);
            if (control is SplitContainer && control.Dock == DockStyle.Fill)
            {
                SetInnerMostBorder(this);
            }
        }

        /// <summary>
        ///  Processes a dialog key. Overrides Control.processDialogKey(). This
        ///  method implements handling of the TAB, LEFT, RIGHT, UP, and DOWN
        ///  keys in dialogs.
        ///  The method performs no processing on keys that include the ALT or
        ///  CONTROL modifiers. For the TAB key, the method selects the next control
        ///  on the form. For the arrow keys,
        ///  !!!
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
#if DEBUG
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "ContainerControl.ProcessDialogKey [" + keyData.ToString() + "]");
#endif
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
            {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                switch (keyCode)
                {
                    case Keys.Tab:
                        if (ProcessTabKey((keyData & Keys.Shift) == Keys.None))
                        {
                            return true;
                        }

                        break;
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        if (!_splitterFocused)
                        {
                            if (ProcessArrowKey(keyCode == Keys.Right ||
                                            keyCode == Keys.Down))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }

                        break;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        ///  /// <summary>
        ///  This will process the TabKey for the SplitContainer. The Focus needs to Shift from controls to the Left of the Splitter
        ///  to the splitter and then to the controls on the right of the splitter. This override implements this Logic.
        /// </summary>
        protected override bool ProcessTabKey(bool forward)
        {
            //Dont Focus the Splitter if TabStop == False or if the Splitter is Fixed !!
            if (!TabStop || IsSplitterFixed)
            {
                return base.ProcessTabKey(forward);
            }

            if (_nextActiveControl != null)
            {
                SetActiveControl(_nextActiveControl);
                _nextActiveControl = null;
            }

            if (SelectNextControlInPanel(ActiveControl, forward, true, true, true))
            {
                _nextActiveControl = null;
                _splitterFocused = false;
                return true;
            }
            else
            {
                if (_callBaseVersion)
                {
                    _callBaseVersion = false;
                    return base.ProcessTabKey(forward);
                }
                else
                {
                    //We are om Splitter ......
                    _splitterFocused = true;
                    IContainerControl c = ParentInternal.GetContainerControl();
                    if (c != null)
                    {
                        if (!(c is ContainerControl cc))
                        {
                            c.ActiveControl = this;
                        }
                        else
                        {
                            cc.SetActiveControl(this);
                        }
                    }
                    SetActiveControl(null);
                    return true;
                }
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (_splitContainerMessageFilter != null)
            {
                Application.RemoveMessageFilter(_splitContainerMessageFilter);
                _splitContainerMessageFilter = null;
            }
        }

        protected override void WndProc(ref Message msg)
        {
            switch ((User32.WM)msg.Msg)
            {
                case User32.WM.SETCURSOR:
                    WmSetCursor(ref msg);
                    break;
                case User32.WM.SETFOCUS:
                    _splitterFocused = true;
                    base.WndProc(ref msg);
                    break;
                case User32.WM.KILLFOCUS:
                    _splitterFocused = false;
                    base.WndProc(ref msg);
                    break;

                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override ControlCollection CreateControlsInstance()
        {
            return new SplitContainerTypedControlCollection(this, typeof(SplitterPanel), /*isReadOnly*/true);
        }

        private class SplitContainerMessageFilter : IMessageFilter
        {
            private readonly SplitContainer _owner;

            public SplitContainerMessageFilter(SplitContainer splitContainer)
            {
                _owner = splitContainer;
            }

            /// <summary>
            /// </summary>
            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                if (m.Msg >= (int)User32.WM.KEYFIRST && m.Msg <=(int) User32.WM.KEYLAST)
                {
                    if ((m.Msg == (int)User32.WM.KEYDOWN && (int)m.WParam == (int)Keys.Escape)
                        || (m.Msg == (int)User32.WM.SYSKEYDOWN))
                    {
                        //Notify that splitMOVE was reverted ..
                        //this is used in ONKEYUP!!
                        _owner._splitBegin = false;
                        _owner.SplitEnd(false);
                        _owner._splitterClick = false;
                        _owner._splitterDrag = false;
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        ///  This control collection only allows a specific type of control
        ///  into the controls collection.  It optionally supports readonlyness.
        /// </summary>
        internal class SplitContainerTypedControlCollection : TypedControlCollection
        {
            private readonly SplitContainer _owner;

            public SplitContainerTypedControlCollection(Control c, Type type, bool isReadOnly) : base(c, type, isReadOnly)
            {
                _owner = c as SplitContainer;
            }

            public override void Remove(Control value)
            {
                if (value is SplitterPanel)
                {
                    if (!_owner.DesignMode)
                    {
                        if (IsReadOnly)
                        {
                            throw new NotSupportedException(SR.ReadonlyControlsCollection);
                        }
                    }
                }
                base.Remove(value);
            }

            internal override void SetChildIndexInternal(Control child, int newIndex)
            {
                if (child is SplitterPanel)
                {
                    if (!_owner.DesignMode)
                    {
                        if (IsReadOnly)
                        {
                            throw new NotSupportedException(SR.ReadonlyControlsCollection);
                        }
                    }
                    else
                    {
                        // just no-op it at DT.
                        return;
                    }
                }
                base.SetChildIndexInternal(child, newIndex);
            }
        }
    }
}
