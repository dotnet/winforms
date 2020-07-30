// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This Glyph represents the UI appended to a control when DesignerActions are available. Each image that represents these states are demand created.  This is done because it is entirely possible that a DesignerActionGlyph will only ever be in one of these states during its lifetime... kind of sad really.
    /// </summary>
    internal sealed class DesignerActionGlyph : Glyph
    {
        internal const int CONTROLOVERLAP_X = 5; // number of pixels the anchor should be offset to the left of the control's upper-right
        internal const int CONTROLOVERLAP_Y = 2; // number of pixels the anchor overlaps the control in the y-direction

        private Rectangle _bounds; // the bounds of our glyph
        private readonly Adorner _adorner; // A ptr back to our adorner - so when we decide to change state, we can invalidate
        private bool _mouseOver; // on mouse over, we shade our image differently, this is used to track that state
        private Rectangle _alternativeBounds = Rectangle.Empty; // if !empty, this represents the bounds of the tray control this gyph is related to
        private readonly Control _alternativeParent; // if this is valid - then the glyph will invalidate itself here instead of on the adorner
        private bool _insidePaint;
        private DockStyle _dockStyle;
        private Bitmap _glyphImageClosed;
        private Bitmap _glyphImageOpened;

        /// <summary>
        ///  Constructor that passes empty alternative bounds and parents. Typically this is done for control on the designer's surface since component tray glyphs will have these alternative values.
        /// </summary>
        public DesignerActionGlyph(DesignerActionBehavior behavior, Adorner adorner) : this(behavior, adorner, Rectangle.Empty, null)
        { }
        public DesignerActionGlyph(DesignerActionBehavior behavior, Rectangle alternativeBounds, Control alternativeParent) : this(behavior, null, alternativeBounds, alternativeParent)
        { }

        /// <summary>
        ///  Constructor that sets the dropdownbox size, creates a our hottrack brush and invalidates the glyph (to configure location).
        /// </summary>
        private DesignerActionGlyph(DesignerActionBehavior behavior, Adorner adorner, Rectangle alternativeBounds, Control alternativeParent) : base(behavior)
        {
            _adorner = adorner;
            _alternativeBounds = alternativeBounds;
            _alternativeParent = alternativeParent;
            Invalidate();
        }

        /// <summary>
        ///  Returns the bounds of our glyph.  This is used by the related Behavior to determine where to show the contextmenu (list of actions).
        /// </summary>
        public override Rectangle Bounds
        {
            get => _bounds;
        }

        public DockStyle DockEdge
        {
            get => _dockStyle;
            set
            {
                if (_dockStyle != value)
                {
                    _dockStyle = value;
                }
            }
        }

        public bool IsInComponentTray
        {
            get => (_adorner is null); // adorner and alternative bounds are exclusive
        }

        /// <summary>
        ///  Standard hit test logic that returns true if the point is contained within our bounds. This is also used to manage out mouse over state.
        /// </summary>
        public override Cursor GetHitTest(Point p)
        {
            if (_bounds.Contains(p))
            {
                MouseOver = true;
                return Cursors.Default;
            }
            MouseOver = false;
            return null;
        }

        /// <summary>
        ///  Returns an image representing the
        /// </summary>
        private Image GlyphImageClosed
        {
            get
            {
                if (_glyphImageClosed is null)
                {
                    _glyphImageClosed = new Icon(typeof(DesignerActionGlyph), "Close_left").ToBitmap();

                    if (DpiHelper.IsScalingRequired)
                    {
                        DpiHelper.ScaleBitmapLogicalToDevice(ref _glyphImageClosed);
                    }
                }
                return _glyphImageClosed;
            }
        }

        private Image GlyphImageOpened
        {
            get
            {
                if (_glyphImageOpened is null)
                {
                    _glyphImageOpened = new Icon(typeof(DesignerActionGlyph), "Open_left").ToBitmap();

                    if (DpiHelper.IsScalingRequired)
                    {
                        DpiHelper.ScaleBitmapLogicalToDevice(ref _glyphImageOpened);
                    }
                }
                return _glyphImageOpened;
            }
        }

        internal void InvalidateOwnerLocation()
        {
            if (_alternativeParent != null)
            { // alternative parent and adoner are exclusive...
                _alternativeParent.Invalidate(_bounds);
            }
            else
            {
                _adorner.Invalidate(_bounds);
            }
        }

        /// <summary>
        ///  Called when the state for this DesignerActionGlyph changes.  Or when the related component's size or location change.  Here, we re-calculate the Glyph's bounds and change our image.
        /// </summary>
        internal void Invalidate()
        {
            IComponent relatedComponent = ((DesignerActionBehavior)Behavior).RelatedComponent;
            Point topRight = Point.Empty;
            //handle the case that our comp is a control
            if (relatedComponent is Control relatedControl && !(relatedComponent is ToolStripDropDown) && _adorner != null)
            {
                topRight = _adorner.BehaviorService.ControlToAdornerWindow(relatedControl);
                topRight.X += relatedControl.Width;
            }
            // ISSUE: we can't have this special cased here - we should find a more generic approach to solving this problem special logic here for our comp being a toolstrip item
            else
            {
                // update alternative bounds if possible...
                if (_alternativeParent is ComponentTray compTray)
                {
                    ComponentTray.TrayControl trayControl = compTray.GetTrayControlFromComponent(relatedComponent);
                    if (trayControl != null)
                    {
                        _alternativeBounds = trayControl.Bounds;
                    }
                }
                Rectangle newRect = DesignerUtils.GetBoundsForNoResizeSelectionType(_alternativeBounds, SelectionBorderGlyphType.Top);
                topRight.X = newRect.Right;
                topRight.Y = newRect.Top;
            }
            topRight.X -= (GlyphImageOpened.Width + CONTROLOVERLAP_X);
            topRight.Y -= (GlyphImageOpened.Height - CONTROLOVERLAP_Y);
            _bounds = (new Rectangle(topRight.X, topRight.Y, GlyphImageOpened.Width, GlyphImageOpened.Height));
        }

        /// <summary>
        ///  Used to manage the mouse-pointer-is-over-glyph state.  If this is true,  then we will shade our BoxImage in the Paint logic.
        /// </summary>
        private bool MouseOver
        {
            get => _mouseOver;
            set
            {
                if (_mouseOver != value)
                {
                    _mouseOver = value;

                    InvalidateOwnerLocation();
                }
            }
        }

        /// <summary>
        ///  Responds to a paint event.  This Glyph will paint its current image and, if  MouseHover is true, we'll paint over the image with the 'hoverBrush'.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            Image image;
            if (Behavior is DesignerActionBehavior)
            {
                if (_insidePaint)
                {
                    return;
                }
                IComponent panelComponent = ((DesignerActionUI)((DesignerActionBehavior)Behavior).ParentUI).LastPanelComponent;
                IComponent relatedComponent = ((DesignerActionBehavior)Behavior).RelatedComponent;
                if (panelComponent != null && panelComponent == relatedComponent)
                {
                    image = GlyphImageOpened;
                }
                else
                {
                    image = GlyphImageClosed;
                }
                try
                {
                    _insidePaint = true;
                    pe.Graphics.DrawImage(image, _bounds.Left, _bounds.Top);
                    if (MouseOver || (panelComponent != null && panelComponent == relatedComponent))
                    {
                        pe.Graphics.FillRectangle(DesignerUtils.HoverBrush, Rectangle.Inflate(_bounds, -1, -1));
                    }
                }
                finally
                {
                    _insidePaint = false;
                }
            }
        }

        /// <summary>
        ///  Called by the ComponentTray when a tray control changes location.
        /// </summary>
        internal void UpdateAlternativeBounds(Rectangle newBounds)
        {
            _alternativeBounds = newBounds;
            Invalidate();
        }
    }
}
