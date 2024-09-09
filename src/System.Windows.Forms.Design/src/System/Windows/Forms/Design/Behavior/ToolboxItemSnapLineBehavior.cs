// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  This class implements the behavior provided by DocumentDesigner
///  when the user is dragging a valid toolbox item. Here, we'll render a
///  default 'box' beneath the cursor that snaps to edges of other
///  components on the designer's surface.
/// </summary>
internal class ToolboxItemSnapLineBehavior : Behavior
{
    private readonly IServiceProvider _serviceProvider; // used for snaplines
    private readonly BehaviorService _behaviorService; // pointer to our big & bad service
    private bool _isPushed; // used to track if this is currently on the stack or not
    private Rectangle _lastRectangle; // cache the last mouse loc - so we can ignore when mouse doesn't move
    private Point _lastOffset; // cache the last snap so we know where to create our control if dropped
    private DragAssistanceManager _dragManager; // used to apply snaplines when dragging a new tool rect on the designer's surface
    private readonly bool _targetAllowsSnapLines; // indicates if the drop target allows snaplines (flowpanels don't for ex)
    private readonly StatusCommandUI _statusCommandUI; // used to update the StatusBar Information.
    private readonly bool _targetAllowsDragBox;    // indicates if the drop target allows the generic drag box to be drawn

    /// <summary>
    ///  Constructor that caches the designer (which invoked us) and a ptr
    ///  to the BehaviorService.
    /// </summary>
    public ToolboxItemSnapLineBehavior(IServiceProvider serviceProvider, BehaviorService behaviorService)
    {
        _serviceProvider = serviceProvider;
        _behaviorService = behaviorService;
        _isPushed = false;
        _lastRectangle = Rectangle.Empty;
        _lastOffset = Point.Empty;
        _statusCommandUI = new StatusCommandUI(serviceProvider);
        _targetAllowsDragBox = true;
        _targetAllowsSnapLines = true;
    }

    public ToolboxItemSnapLineBehavior(IServiceProvider serviceProvider, BehaviorService behaviorService, ControlDesigner controlDesigner)
        : this(serviceProvider, behaviorService)
    {
        // Check to see if the current designer participate with SnapLines
        if (controlDesigner is not null && !controlDesigner.ParticipatesWithSnapLines)
        {
            _targetAllowsSnapLines = false;
        }
    }

    public ToolboxItemSnapLineBehavior(IServiceProvider serviceProvider, BehaviorService behaviorService, ControlDesigner controlDesigner, bool allowDragBox)
        : this(serviceProvider, behaviorService, controlDesigner)
    {
        _targetAllowsDragBox = allowDragBox;
    }

    /// <summary>
    ///  OnDragDrop can be overridden so that a Behavior can specify its own
    ///  Drag/Drop rules.
    ///  CONSIDER: Should we have the BehaviorService fire push/pop events on Behaviors???
    /// </summary>
    public bool IsPushed
    {
        get
        {
            return _isPushed;
        }
        set
        {
            _isPushed = value;

            if (_isPushed)
            {
                _dragManager ??= new DragAssistanceManager(_serviceProvider);
            }
            else
            {
                // clean up all our temp objects
                if (!_lastRectangle.IsEmpty)
                {
                    _behaviorService.Invalidate(_lastRectangle);
                }

                _lastOffset = Point.Empty;
                _lastRectangle = Rectangle.Empty;

                // destroy the snapline engine (if we used it)
                if (_dragManager is not null)
                {
                    _dragManager.OnMouseUp();
                    _dragManager = null;
                }
            }
        }
    }

    /// <summary>
    ///  Called on a DragDrop - this generates our extra drag info
    ///  to pass along to the base class. Basically, we get the
    ///  last-rendered snaplines before the drop and attempt to
    ///  identify to which direction the mouse was snapped.
    /// </summary>
    private ToolboxSnapDragDropEventArgs CreateToolboxSnapArgs(DragEventArgs e, Point mouseLoc)
    {
        // we're trying to set these two vars here...
        ToolboxSnapDragDropEventArgs.SnapDirection snapDirections = ToolboxSnapDragDropEventArgs.SnapDirection.None;
        Point offset = Point.Empty;

        // as soon as these vars are true - we can stop looking at lines
        bool horizontalComponentIdentified = false;
        bool verticalComponentIdentified = false;

        if (_dragManager is not null)
        {
            DragAssistanceManager.Line[] lines = _dragManager.GetRecentLines();

            foreach (DragAssistanceManager.Line line in lines)
            {
                if (line.LineType == DragAssistanceManager.LineType.Standard)
                {
                    if (!horizontalComponentIdentified && line.X1 == line.X2)
                    {
                        // check for vertical equality
                        if (line.X1 == _lastRectangle.Left)
                        {
                            // we had a line on the left of the box - so we must have snapped left
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Left;
                            offset.X = _lastRectangle.Left - mouseLoc.X;
                        }
                        else
                        {// MUST BE RIGHT?  if (lines.x1 == lastRectangle.Right) {
                         // we had a line on the right of the box - so we must have snapped right
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Right;
                            offset.X = _lastRectangle.Right - mouseLoc.X;
                        }

                        horizontalComponentIdentified = true;
                    }
                    else if (!verticalComponentIdentified && line.Y1 == line.Y2)
                    {
                        // check for vertical equality
                        if (line.Y1 == _lastRectangle.Top)
                        {
                            // we had a line on the top of the box - so we must have snapped top
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Top;
                            offset.Y = _lastRectangle.Top - mouseLoc.Y;
                        }
                        else if (line.Y1 == _lastRectangle.Bottom)
                        {
                            // we had a line on the bottom of the box - so we must have snapped bottom
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Bottom;
                            offset.Y = _lastRectangle.Bottom - mouseLoc.Y;
                        }

                        verticalComponentIdentified = true;
                    }
                }
                else if (line.LineType is DragAssistanceManager.LineType.Margin
                    or DragAssistanceManager.LineType.Padding)
                {
                    if (!verticalComponentIdentified && line.X1 == line.X2)
                    {
                        // now, we're looking at a vertical margin line - is it above?
                        if (Math.Max(line.Y1, line.Y2) <= _lastRectangle.Top)
                        {
                            // aha - we had a margin line at the top of the box
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Top;
                            offset.Y = _lastRectangle.Top - mouseLoc.Y;
                        }
                        else
                        {
                            // aha - we had a margin line at the bottom of the box
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Bottom;
                            offset.Y = _lastRectangle.Bottom - mouseLoc.Y;
                        }

                        verticalComponentIdentified = true;
                    }
                    else if (!horizontalComponentIdentified && line.Y1 == line.Y2)
                    {
                        // now, we're looking at a horz margin line - is it left?
                        if (Math.Max(line.X1, line.X2) <= _lastRectangle.Left)
                        {
                            // aha - we had a margin line at the left of the box
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Left;
                            offset.X = _lastRectangle.Left - mouseLoc.X;
                        }
                        else
                        {
                            // aha - we had a margin line at the right of the box
                            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Right;
                            offset.X = _lastRectangle.Right - mouseLoc.X;
                        }

                        horizontalComponentIdentified = true;
                    }
                }

                if (horizontalComponentIdentified && verticalComponentIdentified)
                {
                    // we've found both components - stop looping
                    break;
                }
            }
        }

        // set default values is we haven't identified any 'snaps'
        if (!horizontalComponentIdentified)
        {
            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Left;
            offset.X = _lastRectangle.Left - mouseLoc.X;
        }

        if (!verticalComponentIdentified)
        {
            snapDirections |= ToolboxSnapDragDropEventArgs.SnapDirection.Top;
            offset.Y = _lastRectangle.Top - mouseLoc.Y;
        }

        // create our arg and pass it back
        return new ToolboxSnapDragDropEventArgs(snapDirections, offset, e);
    }

    /// <summary>
    ///  Used when dragging a new tool rect on the designer's surface -
    ///  this will return some generic snaplines Allowing the rect to
    ///  snap to existing control edges on the surface.
    /// </summary>
    private static SnapLine[] GenerateNewToolSnapLines(Rectangle r)
    {
        return [
            new(SnapLineType.Left, r.Left),
            new(SnapLineType.Right, r.Right),
            new(SnapLineType.Bottom, r.Bottom),
            new(SnapLineType.Top, r.Top),
            new(SnapLineType.Horizontal, r.Top - 4, SnapLine.MarginTop, SnapLinePriority.Always),
            new(SnapLineType.Horizontal, r.Bottom + 3, SnapLine.MarginBottom, SnapLinePriority.Always),
            new(SnapLineType.Vertical, r.Left - 4, SnapLine.MarginLeft, SnapLinePriority.Always),
            new(SnapLineType.Vertical, r.Right + 3, SnapLine.MarginRight, SnapLinePriority.Always)
        ];
    }

    /// <summary>
    ///  OnDragDrop can be overridden so that a Behavior can specify its own
    ///  Drag/Drop rules.
    /// </summary>
    public override void OnDragDrop(Glyph g, DragEventArgs e)
    {
        _behaviorService.PopBehavior(this);

        try
        {
            // offset the mouse loc to screen coords for calculations on drops
            Point screenOffset = _behaviorService.AdornerWindowToScreen();

            // build up our extra-special event args
            ToolboxSnapDragDropEventArgs se = CreateToolboxSnapArgs(e, new Point(e.X - screenOffset.X, e.Y - screenOffset.Y));

            base.OnDragDrop(g, se);
        }
        finally
        {
            // clear everything up
            IsPushed = false;
        }
    }

    // When we begin a drag we need to remove the glyphs that do not allow drops.
    // VSWhidbey #487816
    public void OnBeginDrag()
    {
        Adorner bodyAdorner = null;
        SelectionManager selMgr = (SelectionManager)_serviceProvider.GetService(typeof(SelectionManager));
        if (selMgr is not null)
        {
            bodyAdorner = selMgr.BodyGlyphAdorner;
        }

        List<ControlBodyGlyph> glyphsToRemove = [];
        foreach (ControlBodyGlyph body in bodyAdorner.Glyphs)
        {
            Control ctl = body.RelatedComponent as Control;
            if (ctl is not null)
            {
                if (!ctl.AllowDrop)
                {
                    glyphsToRemove.Add(body);
                }
            }
        }

        foreach (Glyph glyph in glyphsToRemove)
        {
            bodyAdorner.Glyphs.Remove(glyph);
        }
    }

    public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
    {
        bool altKeyPressed = Control.ModifierKeys == Keys.Alt;

        if (altKeyPressed && _dragManager is not null)
        {
            // erase any snaplines (if we had any)
            _dragManager.EraseSnapLines();
        }

        // call base
        bool retValue = base.OnMouseMove(g, button, mouseLoc);

        // identify where the new box should be...
        Rectangle newRectangle = new(mouseLoc.X - DesignerUtils.s_boxImageSize / 2, mouseLoc.Y - DesignerUtils.s_boxImageSize / 2,
                                          DesignerUtils.s_boxImageSize, DesignerUtils.s_boxImageSize);

        // don't do anything if the loc is the same
        if (newRectangle != _lastRectangle)
        {
            if (_dragManager is not null && _targetAllowsSnapLines && !altKeyPressed)
            {
                _lastOffset = _dragManager.OnMouseMove(newRectangle, GenerateNewToolSnapLines(newRectangle));
                newRectangle.Offset(_lastOffset.X, _lastOffset.Y);
            }

            // erase old
            if (!_lastRectangle.IsEmpty)
            {
                // build up the invalid region
                using Region invalidRegion = new(_lastRectangle);
                invalidRegion.Exclude(newRectangle);
                _behaviorService.Invalidate(invalidRegion);
            }

            if (_targetAllowsDragBox)
            {
                using Graphics graphics = _behaviorService.AdornerWindowGraphics;
                graphics.DrawImage(DesignerUtils.BoxImage, newRectangle.Location);
            }

            // offset the mouse loc to screen coords for calculations on drops
            IDesignerHost host = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
            if (host is not null)
            {
                Control baseControl = host.RootComponent as Control;
                if (baseControl is not null)
                {
                    Point adornerServiceOrigin = _behaviorService.MapAdornerWindowPoint(baseControl.Handle, new Point(0, 0));
                    Rectangle statusRect = new(newRectangle.X - adornerServiceOrigin.X, newRectangle.Y - adornerServiceOrigin.Y, 0, 0);
                    _statusCommandUI?.SetStatusInformation(statusRect);
                }
            }

            if (_dragManager is not null && _targetAllowsSnapLines && !altKeyPressed)
            {
                _dragManager.RenderSnapLinesInternal();
            }

            // store this off for the next time around
            _lastRectangle = newRectangle;
        }

        return retValue;
    }
}
