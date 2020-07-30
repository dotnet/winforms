// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms.Layout;

    ///  this class is a container for toolstrips on a rafting row.
    ///  you can set layout styles on this container all day long and not
    ///  affect the underlying toolstrip's properties.... so if its
    ///  removed from a rafting container its still got its defaults
    ///  set up for it.
    internal class ToolStripPanelCell : ArrangedElement
    {
        private ToolStrip _wrappedToolStrip;
        private ToolStripPanelRow _parent;
        private Size _maxSize = LayoutUtils.MaxSize;
        private bool _currentlySizing;
        private bool _currentlyDragging;
        private bool _restoreOnVisibleChanged;

        private Rectangle _cachedBounds = Rectangle.Empty;
#if DEBUG
        private readonly string _cellID;

        [ThreadStatic]
        private static int t_cellCount;
#endif

        public ToolStripPanelCell(Control control) : this(null, control)
        {
        }
        public ToolStripPanelCell(ToolStripPanelRow parent, Control control)
        {
#if DEBUG

            // Ensure 1:1 Cell/ToolStripPanel mapping
            _cellID = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", control.Name, ++t_cellCount);
            Debug.Assert(t_cellCount <= ToolStripManager.ToolStrips.Count, "who is allocating an extra toolstrippanel cell?");
#endif

            ToolStripPanelRow = parent;
            _wrappedToolStrip = control as ToolStrip;
            if (control is null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            else if (_wrappedToolStrip is null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.TypedControlCollectionShouldBeOfType, typeof(ToolStrip).Name)), control.GetType().Name);
            }
            CommonProperties.SetAutoSize(this, true);
            _wrappedToolStrip.LocationChanging += new ToolStripLocationCancelEventHandler(OnToolStripLocationChanging);
            _wrappedToolStrip.VisibleChanged += new EventHandler(OnToolStripVisibleChanged);
        }

        public Rectangle CachedBounds
        {
            get { return _cachedBounds; }
            set
            {
                _cachedBounds = value;
                Debug.Assert(_cachedBounds.X >= 0 && _cachedBounds.Y >= 0, "cached bounds are outside of the client area, investigate");
            }
        }

        public Control Control
        {
            get { return _wrappedToolStrip; }
        }

        // ToolStripPanelCell is considered to be Visible if the wrapped control is InDesignMode.
        // This property is accessed from the ToolStripPanelRow in cases where we are moving the toolStrip around
        // during a drag operation.
        public bool ControlInDesignMode
        {
            get { return (_wrappedToolStrip != null && _wrappedToolStrip.IsInDesignMode); }
        }

        public IArrangedElement InnerElement
        {
            get { return _wrappedToolStrip as IArrangedElement; }
        }

        public ISupportToolStripPanel DraggedControl
        {
            get { return _wrappedToolStrip as ISupportToolStripPanel; }
        }

        public ToolStripPanelRow ToolStripPanelRow
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    if (_parent != null)
                    {
                        ((IList)_parent.Cells).Remove(this);
                    }
                    _parent = value;
                    Margin = Padding.Empty;
                }
            }
        }

        public override bool Visible
        {
            get
            {
                if (Control != null && Control.ParentInternal == ToolStripPanelRow.ToolStripPanel)
                {
                    return InnerElement.ParticipatesInLayout;
                }
                return false;
            }
            set
            {
                Control.Visible = value;
            }
        }

        public Size MaximumSize
        {
            get { return _maxSize; }
        }

        public override LayoutEngine LayoutEngine
        {
            get { return DefaultLayout.Instance; }
        }

        protected override IArrangedElement GetContainer()
        {
            return _parent;
        }

        public int Grow(int growBy)
        {
            if (ToolStripPanelRow.Orientation == Orientation.Vertical)
            {
                return GrowVertical(growBy);
            }
            else
            {
                return GrowHorizontal(growBy);
            }
        }

        private int GrowVertical(int growBy)
        {
            // Grow         ---]
            // Pref [      ]
            // Max  [      ]
            if (MaximumSize.Height >= Control.PreferredSize.Height)
            {
                // nothing to grow.
                return 0;
            }

            // Grow         ---]
            // Pref [        ]
            // Max  [      ]
            if (MaximumSize.Height + growBy >= Control.PreferredSize.Height)
            {
                int freed = Control.PreferredSize.Height - MaximumSize.Height;
                _maxSize = LayoutUtils.MaxSize;
                return freed;
            }

            // Grow         ---]
            // Pref [            ]
            // Max  [      ]
            if (MaximumSize.Height + growBy < Control.PreferredSize.Height)
            {
                _maxSize.Height += growBy;
                return growBy;
            }
            return 0;
        }

        private int GrowHorizontal(int growBy)
        {
            // Grow         ---]
            // Pref [      ]
            // Max  [      ]
            if (MaximumSize.Width >= Control.PreferredSize.Width)
            {
                // nothing to grow.
                return 0;
            }

            // Grow         ---]
            // Pref [        ]
            // Max  [      ]
            if (MaximumSize.Width + growBy >= Control.PreferredSize.Width)
            {
                int freed = Control.PreferredSize.Width - MaximumSize.Width;
                _maxSize = LayoutUtils.MaxSize;
                return freed;
            }

            // Grow         ---]
            // Pref [            ]
            // Max  [      ]
            if (MaximumSize.Width + growBy < Control.PreferredSize.Width)
            {
                _maxSize.Width += growBy;
                return growBy;
            }
            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_wrappedToolStrip != null)
                    {
#if DEBUG
                        t_cellCount--;
#endif
                        _wrappedToolStrip.LocationChanging -= new ToolStripLocationCancelEventHandler(OnToolStripLocationChanging);
                        _wrappedToolStrip.VisibleChanged -= new EventHandler(OnToolStripVisibleChanged);
                    }
                    _wrappedToolStrip = null;
                    if (_parent != null)
                    {
                        ((IList)_parent.Cells).Remove(this);
                    }
                    _parent = null;
                }
#if DEBUG
                else
                {
                    t_cellCount--;
                }
#endif

            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override ArrangedElementCollection GetChildren()
        {
            return ArrangedElementCollection.Empty;
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            ISupportToolStripPanel draggedControl = DraggedControl;
            Size preferredSize = Size.Empty;

            if (draggedControl.Stretch)
            {
                if (ToolStripPanelRow.Orientation == Orientation.Horizontal)
                {
                    constrainingSize.Width = ToolStripPanelRow.Bounds.Width;
                    preferredSize = _wrappedToolStrip.GetPreferredSize(constrainingSize);
                    preferredSize.Width = constrainingSize.Width;
                }
                else
                {
                    constrainingSize.Height = ToolStripPanelRow.Bounds.Height;
                    preferredSize = _wrappedToolStrip.GetPreferredSize(constrainingSize);
                    preferredSize.Height = constrainingSize.Height;
                }
            }
            else
            {
                preferredSize = (!_wrappedToolStrip.AutoSize) ? _wrappedToolStrip.Size : _wrappedToolStrip.GetPreferredSize(constrainingSize);
            }

            // return LayoutUtils.IntersectSizes(constrainingSize, preferredSize);
            return preferredSize;
        }

        protected override void SetBoundsCore(Rectangle bounds, BoundsSpecified specified)
        {
            _currentlySizing = true;
            CachedBounds = bounds;
            try
            {
                if (DraggedControl.IsCurrentlyDragging)
                {
                    if (ToolStripPanelRow.Cells[ToolStripPanelRow.Cells.Count - 1] == this)
                    {
                        Rectangle displayRectangle = ToolStripPanelRow.DisplayRectangle;
                        if (ToolStripPanelRow.Orientation == Orientation.Horizontal)
                        {
                            int spaceToFree = bounds.Right - displayRectangle.Right;
                            if (spaceToFree > 0 && bounds.Width > spaceToFree)
                            {
                                bounds.Width -= spaceToFree;
                            }
                        }
                        else
                        {
                            int spaceToFree = bounds.Bottom - displayRectangle.Bottom;
                            if (spaceToFree > 0 && bounds.Height > spaceToFree)
                            {
                                bounds.Height -= spaceToFree;
                            }
                        }
                    }
                    Debug.WriteLineIf(ToolStripPanelRow.ToolStripPanelMouseDebug.TraceVerbose, "[CELL] DRAGGING calling SetBounds " + bounds.ToString());
                    base.SetBoundsCore(bounds, specified);
                    InnerElement.SetBounds(bounds, specified);
                }
                else
                {
                    if (!ToolStripPanelRow.CachedBoundsMode)
                    {
                        Debug.WriteLineIf(ToolStripPanelRow.ToolStripPanelMouseDebug.TraceVerbose, "[CELL] NOT DRAGGING calling SetBounds " + bounds.ToString());
                        base.SetBoundsCore(bounds, specified);
                        InnerElement.SetBounds(bounds, specified);
                    }
                }
            }
            finally
            {
                _currentlySizing = false;
            }
        }

        public int Shrink(int shrinkBy)
        {
            if (ToolStripPanelRow.Orientation == Orientation.Vertical)
            {
                return ShrinkVertical(shrinkBy);
            }
            else
            {
                return ShrinkHorizontal(shrinkBy);
            }
        }

        private int ShrinkHorizontal(int shrinkBy)
        {
            return 0;
        }

        private int ShrinkVertical(int shrinkBy)
        {
            return 0;
        }

        /// <summary>
        ///  New EventHandler for The LocationChanging so that ToolStripPanelCell Listens to the Location Property on the ToolStrips's being changed.
        ///  The ToolStrip needs to Raft (Join) to the approriate Location Depending on the new Location w.r.t to the oldLocation ...
        ///  Hence the need for this event listener.
        /// </summary>
        private void OnToolStripLocationChanging(object sender, ToolStripLocationCancelEventArgs e)
        {
            if (ToolStripPanelRow is null)
            {
                return;
            }
            if (!_currentlySizing && !_currentlyDragging)
            {
                try
                {
                    _currentlyDragging = true;
                    Point newloc = e.NewLocation;
                    // detect if we havent yet performed a layout - force one so we can
                    // properly join to the row.
                    if (ToolStripPanelRow != null && ToolStripPanelRow.Bounds == Rectangle.Empty)
                    {
                        ToolStripPanelRow.ToolStripPanel.PerformUpdate(true);
                    }
                    if (_wrappedToolStrip != null)
                    {
                        ToolStripPanelRow.ToolStripPanel.Join(_wrappedToolStrip, newloc);
                    }
                }
                finally
                {
                    _currentlyDragging = false;
                    e.Cancel = true;
                }
            }
        }

        private void OnToolStripVisibleChanged(object sender, EventArgs e)
        {
            if (_wrappedToolStrip != null
                && !_wrappedToolStrip.IsInDesignMode

                && !_wrappedToolStrip.IsCurrentlyDragging
                && !_wrappedToolStrip.IsDisposed      // ensure we have a live-runtime only toolstrip.
                && !_wrappedToolStrip.Disposing)
            {
                // Rejoin the row when visibility is toggled.
                // we dont want to do this logic at DT, as the DropSourceBehavior
                // will set the toolstrip visible = false.
                if (!Control.Visible)
                {
                    // if we are becoming visible = false, remember if we were in a toolstrippanelrow at the time.
                    _restoreOnVisibleChanged = (ToolStripPanelRow != null && ((IList)ToolStripPanelRow.Cells).Contains(this));
                }
                else if (_restoreOnVisibleChanged)
                {
                    try
                    {
                        // if we are becoming visible = true, and we ARE in a toolstrippanelrow, rejoin.
                        if (ToolStripPanelRow != null && ((IList)ToolStripPanelRow.Cells).Contains(this))
                        {
                            ToolStripPanelRow.ToolStripPanel.Join(_wrappedToolStrip, _wrappedToolStrip.Location);
                        }
                    }
                    finally
                    {
                        _restoreOnVisibleChanged = false;
                    }
                }
            }
        }
    }
}
