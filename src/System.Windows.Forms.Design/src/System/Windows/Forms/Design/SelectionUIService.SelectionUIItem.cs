// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

internal sealed partial class SelectionUIService
{
    /// <summary>
    ///  This class represents a single selected object.
    /// </summary>
    private class SelectionUIItem
    {
        // Flags describing how a given selection point may be sized
        public const int SIZE_X = 0x0001;
        public const int SIZE_Y = 0x0002;
        public const int SIZE_MASK = 0x0003;
        // Flags describing how a given selection point may be moved
        public const int MOVE_X = 0x0004;
        public const int MOVE_Y = 0x0008;
        public const int MOVE_MASK = 0x000C;
        // Flags describing where a given selection point is located on an object
        public const int POS_LEFT = 0x0010;
        public const int POS_TOP = 0x0020;
        public const int POS_RIGHT = 0x0040;
        public const int POS_BOTTOM = 0x0080;
        public const int POS_MASK = 0x00F0;
        // This is returned if the given selection point is not within the selection
        public const int NOHIT = 0x0100;
        // This is returned if the given selection point on the "container selector"
        public const int CONTAINER_SELECTOR = 0x0200;
        public const int GRABHANDLE_WIDTH = 7;
        public const int GRABHANDLE_HEIGHT = 7;
        // tables we use to determine how things can move and size
        internal static readonly int[] s_activeSizeArray =
        [
            SIZE_X | SIZE_Y | POS_LEFT | POS_TOP,      SIZE_Y | POS_TOP,      SIZE_X | SIZE_Y | POS_TOP | POS_RIGHT,
            SIZE_X | POS_LEFT,                                                SIZE_X | POS_RIGHT,
            SIZE_X | SIZE_Y | POS_LEFT | POS_BOTTOM,   SIZE_Y | POS_BOTTOM,   SIZE_X | SIZE_Y | POS_RIGHT | POS_BOTTOM
        ];

        internal static readonly Cursor[] s_activeCursorArrays =
        [
            Cursors.SizeNWSE,   Cursors.SizeNS,   Cursors.SizeNESW,
            Cursors.SizeWE,                      Cursors.SizeWE,
            Cursors.SizeNESW,   Cursors.SizeNS,   Cursors.SizeNWSE
        ];

        internal static readonly int[] s_inactiveSizeArray = [0, 0, 0, 0, 0, 0, 0, 0];
        internal static readonly Cursor[] s_inactiveCursorArray =
        [
            Cursors.Arrow,   Cursors.Arrow,   Cursors.Arrow,
            Cursors.Arrow,                   Cursors.Arrow,
            Cursors.Arrow,   Cursors.Arrow,   Cursors.Arrow
        ];

        internal int[] _sizes; // array of sizing rules for this selection
        internal Cursor[] _cursors; // array of cursors for each grab location
        internal SelectionUIService _selUIsvc;
        internal Rectangle _innerRect = Rectangle.Empty; // inner part of selection (== control bounds)
        internal Rectangle _outerRect = Rectangle.Empty; // outer part of selection (inner + border size)
        internal Region? _region; // region object that defines the shape
        internal object _component; // the component we're rendering
        private readonly Control? _control;
        private SelectionStyles _selectionStyle; // how do we draw this thing?
        private SelectionRules _selectionRules;
        private readonly ISelectionUIHandler? _handler; // the components selection UI handler (can be null)

        ///  Its ok to call virtual method as this is a private class.
        public SelectionUIItem(SelectionUIService selUIsvc, object component)
        {
            _selUIsvc = selUIsvc;
            _component = component;
            _selectionStyle = SelectionStyles.Selected;
            // By default, a component isn't visible. We must establish what it can do through it's UI handler.
            _handler = selUIsvc.GetHandler(component);
            _sizes = s_inactiveSizeArray;
            _cursors = s_inactiveCursorArray;
            if (component is IComponent comp)
            {
                if (selUIsvc._host.GetDesigner(comp) is ControlDesigner cd)
                {
                    _control = cd.Control;
                }
            }

            UpdateRules();
            UpdateGrabSettings();
            UpdateSize();
        }

        /// <summary>
        ///  Retrieves the style of the selection frame for this selection.
        /// </summary>
        public virtual SelectionStyles Style
        {
            get => _selectionStyle;
            set
            {
                if (value != _selectionStyle)
                {
                    _selectionStyle = value;
                    if (_region is not null)
                    {
                        _region.Dispose();
                        _region = null;
                    }
                }
            }
        }

        /// <summary>
        ///  paints the selection
        /// </summary>
        public virtual void DoPaint(Graphics graphics)
        {
            // If we're not visible, then there's nothing to do...
            //
            if ((GetRules() & SelectionRules.Visible) == SelectionRules.None)
            {
                return;
            }

            bool fActive = false;
            if (_selUIsvc._selSvc is not null)
            {
                fActive = _component == _selUIsvc._selSvc.PrimarySelection;
                // Office rules:  If this is a multi-select, reverse the colors for active / inactive.
                fActive = (fActive == (_selUIsvc._selSvc.SelectionCount <= 1));
            }

            Rectangle rect = new(_outerRect.X, _outerRect.Y, GRABHANDLE_WIDTH, GRABHANDLE_HEIGHT);
            Rectangle inner = _innerRect;
            Rectangle outer = _outerRect;
            Region oldClip = graphics.Clip;
            Color borderColor = SystemColors.Control;
            if (_control is not null && _control.Parent is not null)
            {
                Control parent = _control.Parent;
                borderColor = parent.BackColor;
            }

            Brush brush = new SolidBrush(borderColor);
            graphics.ExcludeClip(inner);
            graphics.FillRectangle(brush, outer);
            brush.Dispose();
            graphics.Clip = oldClip;
            ControlPaint.DrawSelectionFrame(graphics, false, outer, inner, borderColor);
            // if it's not locked & it is sizeable...
            if (((GetRules() & SelectionRules.Locked) == SelectionRules.None) && (GetRules() & SelectionRules.AllSizeable) != SelectionRules.None)
            {
                // upper left
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, (_sizes[0] != 0));
                // upper right
                rect.X = inner.X + inner.Width;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[2] != 0);
                // lower right
                rect.Y = inner.Y + inner.Height;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[7] != 0);
                // lower left
                rect.X = outer.X;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[5] != 0);
                // lower middle
                rect.X += (outer.Width - GRABHANDLE_WIDTH) / 2;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[6] != 0);
                // upper middle
                rect.Y = outer.Y;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[1] != 0);
                // left middle
                rect.X = outer.X;
                rect.Y = inner.Y + (inner.Height - GRABHANDLE_HEIGHT) / 2;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[3] != 0);
                // right middle
                rect.X = inner.X + inner.Width;
                ControlPaint.DrawGrabHandle(graphics, rect, fActive, _sizes[4] != 0);
            }
            else
            {
                ControlPaint.DrawLockedFrame(graphics, outer, fActive);
            }
        }

        /// <summary>
        ///  Retrieves an appropriate cursor at the given point. If there is no appropriate cursor here
        ///  (ie, the point lies outside the selection rectangle), then this will return null.
        /// </summary>
        public virtual Cursor? GetCursorAtPoint(Point point)
        {
            Cursor? cursor = null;
            if (PointWithinSelection(point))
            {
                int nOffset = -1;
                if ((GetRules() & SelectionRules.AllSizeable) != SelectionRules.None)
                {
                    nOffset = GetHandleIndexOfPoint(point);
                }

                if (nOffset == -1)
                {
                    if ((GetRules() & SelectionRules.Moveable) == SelectionRules.None)
                    {
                        cursor = Cursors.Default;
                    }
                    else
                    {
                        cursor = Cursors.SizeAll;
                    }
                }
                else
                {
                    cursor = _cursors[nOffset];
                }
            }

            return cursor;
        }

        /// <summary>
        ///  returns the hit test code of the given point. This may be one of:
        /// </summary>
        public virtual int GetHitTest(Point pt)
        {
            // Is it within our rectangles?
            if (!PointWithinSelection(pt))
            {
                return NOHIT;
            }

            // Which index in the array is this?
            int nOffset = GetHandleIndexOfPoint(pt);
            // If no index, the user has picked on the hatch
            if (nOffset == -1 || _sizes[nOffset] == 0)
            {
                return ((GetRules() & SelectionRules.Moveable) == SelectionRules.None ? 0 : MOVE_X | MOVE_Y);
            }

            return _sizes[nOffset];
        }

        /// <summary>
        ///  gets the array offset of the handle at the given point
        /// </summary>
        private int GetHandleIndexOfPoint(Point pt)
        {
            if (pt.X >= _outerRect.X && pt.X <= _innerRect.X)
            {
                // Something on the left side.
                if (pt.Y >= _outerRect.Y && pt.Y <= _innerRect.Y)
                {
                    return 0; // top left
                }

                if (pt.Y >= _innerRect.Y + _innerRect.Height && pt.Y <= _outerRect.Y + _outerRect.Height)
                {
                    return 5; // bottom left
                }

                if (pt.Y >= _outerRect.Y + (_outerRect.Height - GRABHANDLE_HEIGHT) / 2
                    && pt.Y <= _outerRect.Y + (_outerRect.Height + GRABHANDLE_HEIGHT) / 2)
                {
                    return 3; // middle left
                }

                return -1; // unknown hit
            }

            if (pt.Y >= _outerRect.Y && pt.Y <= _innerRect.Y)
            {
                // something on the top
                Debug.Assert(!(pt.X >= _outerRect.X && pt.X <= _innerRect.X), "Should be handled by left top check");
                if (pt.X >= _innerRect.X + _innerRect.Width && pt.X <= _outerRect.X + _outerRect.Width)
                {
                    return 2; // top right
                }

                if (pt.X >= _outerRect.X + (_outerRect.Width - GRABHANDLE_WIDTH) / 2
                    && pt.X <= _outerRect.X + (_outerRect.Width + GRABHANDLE_WIDTH) / 2)
                {
                    return 1; // top middle
                }

                return -1; // unknown hit
            }

            if (pt.X >= _innerRect.X + _innerRect.Width && pt.X <= _outerRect.X + _outerRect.Width)
            {
                // something on the right side
                Debug.Assert(!(pt.Y >= _outerRect.Y && pt.Y <= _innerRect.Y), "Should be handled by top right check");
                if (pt.Y >= _innerRect.Y + _innerRect.Height && pt.Y <= _outerRect.Y + _outerRect.Height)
                {
                    return 7; // bottom right
                }

                if (pt.Y >= _outerRect.Y + (_outerRect.Height - GRABHANDLE_HEIGHT) / 2
                    && pt.Y <= _outerRect.Y + (_outerRect.Height + GRABHANDLE_HEIGHT) / 2)
                {
                    return 4; // middle right
                }

                return -1; // unknown hit
            }

            if (pt.Y >= _innerRect.Y + _innerRect.Height && pt.Y <= _outerRect.Y + _outerRect.Height)
            {
                // something on the bottom
                Debug.Assert(!(pt.X >= _outerRect.X && pt.X <= _innerRect.X), "Should be handled by left bottom check");

                Debug.Assert(!(pt.X >= _innerRect.X + _innerRect.Width && pt.X <= _outerRect.X + _outerRect.Width), "Should be handled by right bottom check");

                if (pt.X >= _outerRect.X + (_outerRect.Width - GRABHANDLE_WIDTH) / 2 && pt.X <= _outerRect.X + (_outerRect.Width + GRABHANDLE_WIDTH) / 2)
                {
                    return 6; // bottom middle
                }

                return -1; // unknown hit
            }

            return -1; // unknown hit
        }

        /// <summary>
        ///  returns a region handle that defines this selection. This is used to piece together a paint region
        ///  for the surface that we draw our selection handles on.
        /// </summary>
        public virtual Region GetRegion()
        {
            if (_region is null)
            {
                if ((GetRules() & SelectionRules.Visible) != SelectionRules.None && !_outerRect.IsEmpty)
                {
                    _region = new(_outerRect);
                    _region.Exclude(_innerRect);
                }
                else
                {
                    _region = new(Rectangle.Empty);
                }

                if (_handler is not null)
                {
                    Rectangle handlerClip = _handler.GetSelectionClipRect(_component);
                    if (!handlerClip.IsEmpty)
                    {
                        _region.Intersect(_selUIsvc.RectangleToClient(handlerClip));
                    }
                }
            }

            return _region;
        }

        /// <summary>
        ///  Retrieves the rules associated with this selection.
        /// </summary>
        public SelectionRules GetRules() => _selectionRules;

        public void Dispose()
        {
            if (_region is not null)
            {
                _region.Dispose();
                _region = null;
            }
        }

        /// <summary>
        ///  Invalidates the region for this selection glyph.
        /// </summary>
        public void Invalidate()
        {
            if (!_outerRect.IsEmpty && !_selUIsvc.Disposing)
            {
                _selUIsvc.Invalidate(_outerRect);
            }
        }

        /// <summary>
        ///  Part of our hit testing logic; determines if the point is somewhere within our selection.
        /// </summary>
        protected bool PointWithinSelection(Point pt)
        {
            // This is only supported for visible selections
            if ((GetRules() & SelectionRules.Visible) == SelectionRules.None || _outerRect.IsEmpty || _innerRect.IsEmpty)
            {
                return false;
            }

            if (pt.X < _outerRect.X || pt.X > _outerRect.X + _outerRect.Width)
            {
                return false;
            }

            if (pt.Y < _outerRect.Y || pt.Y > _outerRect.Y + _outerRect.Height)
            {
                return false;
            }

            if (pt.X > _innerRect.X
                && pt.X < _innerRect.X + _innerRect.Width
                && pt.Y > _innerRect.Y
                && pt.Y < _innerRect.Y + _innerRect.Height)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Updates the available grab handle settings based on the current rules.
        /// </summary>
        private void UpdateGrabSettings()
        {
            SelectionRules rules = GetRules();
            if ((rules & SelectionRules.AllSizeable) == SelectionRules.None)
            {
                _sizes = s_inactiveSizeArray;
                _cursors = s_inactiveCursorArray;
            }
            else
            {
                _sizes = new int[8];
                _cursors = new Cursor[8];
                Array.Copy(s_activeCursorArrays, _cursors, _cursors.Length);
                Array.Copy(s_activeSizeArray, _sizes, _sizes.Length);
                if ((rules & SelectionRules.TopSizeable) != SelectionRules.TopSizeable)
                {
                    _sizes[0] = 0;
                    _sizes[1] = 0;
                    _sizes[2] = 0;
                    _cursors[0] = Cursors.Arrow;
                    _cursors[1] = Cursors.Arrow;
                    _cursors[2] = Cursors.Arrow;
                }

                if ((rules & SelectionRules.LeftSizeable) != SelectionRules.LeftSizeable)
                {
                    _sizes[0] = 0;
                    _sizes[3] = 0;
                    _sizes[5] = 0;
                    _cursors[0] = Cursors.Arrow;
                    _cursors[3] = Cursors.Arrow;
                    _cursors[5] = Cursors.Arrow;
                }

                if ((rules & SelectionRules.BottomSizeable) != SelectionRules.BottomSizeable)
                {
                    _sizes[5] = 0;
                    _sizes[6] = 0;
                    _sizes[7] = 0;
                    _cursors[5] = Cursors.Arrow;
                    _cursors[6] = Cursors.Arrow;
                    _cursors[7] = Cursors.Arrow;
                }

                if ((rules & SelectionRules.RightSizeable) != SelectionRules.RightSizeable)
                {
                    _sizes[2] = 0;
                    _sizes[4] = 0;
                    _sizes[7] = 0;
                    _cursors[2] = Cursors.Arrow;
                    _cursors[4] = Cursors.Arrow;
                    _cursors[7] = Cursors.Arrow;
                }
            }
        }

        /// <summary>
        ///  Updates our cached selection rules based on current handler values.
        /// </summary>
        public void UpdateRules()
        {
            if (_handler is null)
            {
                _selectionRules = SelectionRules.None;
            }
            else
            {
                SelectionRules oldRules = _selectionRules;
                _selectionRules = _handler.GetComponentRules(_component);
                if (_selectionRules != oldRules)
                {
                    UpdateGrabSettings();
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  rebuilds the inner and outer rectangles based on the current selItem.component dimensions.
        ///  We could calculate this every time, but that would be expensive for functions like getHitTest
        ///  that are called a lot (like on every mouse move)
        /// </summary>
        public virtual bool UpdateSize()
        {
            bool sizeChanged = false;
            // Short circuit common cases
            if (_handler is null)
            {
                return false;
            }

            if ((GetRules() & SelectionRules.Visible) == SelectionRules.None)
            {
                return false;
            }

            _innerRect = _handler.GetComponentBounds(_component);
            if (!_innerRect.IsEmpty)
            {
                _innerRect = _selUIsvc.RectangleToClient(_innerRect);
                Rectangle rcOuterNew = new(_innerRect.X - GRABHANDLE_WIDTH, _innerRect.Y - GRABHANDLE_HEIGHT, _innerRect.Width + 2 * GRABHANDLE_WIDTH, _innerRect.Height + 2 * GRABHANDLE_HEIGHT);
                if (_outerRect.IsEmpty || !_outerRect.Equals(rcOuterNew))
                {
                    if (!_outerRect.IsEmpty)
                    {
                        Invalidate();
                    }

                    _outerRect = rcOuterNew;
                    Invalidate();
                    if (_region is not null)
                    {
                        _region.Dispose();
                        _region = null;
                    }

                    sizeChanged = true;
                }
            }
            else
            {
                Rectangle rcNew = Rectangle.Empty;
                sizeChanged = _outerRect.IsEmpty || !_outerRect.Equals(rcNew);
                _innerRect = _outerRect = rcNew;
            }

            return sizeChanged;
        }
    }
}
