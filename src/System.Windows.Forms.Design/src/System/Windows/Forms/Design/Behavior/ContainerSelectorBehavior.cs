// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  This behavior is associated with the ContainerGlyph offered up by ParentControlDesigner.
///  This Behavior simply starts a new DragDrop behavior.
/// </summary>
internal sealed class ContainerSelectorBehavior : Behavior
{
    private Control _containerControl; // our related control
    private IServiceProvider _serviceProvider; // used for starting a drag/drop
    private BehaviorService _behaviorService; // ptr to where we start our drag/drop operation
    private bool _okToMove; // state identifying if we are allowed to move the container
    private Point _initialDragPoint; // cached "mouse down" point

    // For some controls, we want to change the original drag point to be the upper-left of the control
    // in order to make it easier to drop the control at a desired location. But not all controls want this behavior.
    // E.g. we want to do it for Panel and ToolStrip, but not for Label. Label has a ContainerSelectorBehavior via
    // the NoResizeSelectionBorder glyph.
    private readonly bool _setInitialDragPoint;

    /// <summary>
    ///  Constructor, here we cache off all of our member vars and sync location and size changes.
    /// </summary>
    internal ContainerSelectorBehavior(Control containerControl, IServiceProvider serviceProvider)
    {
        Init(containerControl, serviceProvider);
        _setInitialDragPoint = false;
    }

    /// <summary>
    ///  Constructor, here we cache off all of our member vars and sync location and size changes.
    /// </summary>
    internal ContainerSelectorBehavior(Control containerControl, IServiceProvider serviceProvider, bool setInitialDragPoint)
    {
        Init(containerControl, serviceProvider);
        _setInitialDragPoint = setInitialDragPoint;
    }

    private void Init(Control containerControl, IServiceProvider serviceProvider)
    {
        _behaviorService = (BehaviorService)serviceProvider.GetService(typeof(BehaviorService));
        if (_behaviorService is null)
        {
            Debug.Fail("Could not get the BehaviorService from ContainerSelectorBehavior!");
            return;
        }

        _containerControl = containerControl;
        _serviceProvider = serviceProvider;
        _initialDragPoint = Point.Empty;
        _okToMove = false;
    }

    public Control ContainerControl
    {
        get => _containerControl;
    }

    /// <summary>
    ///  This will be true when we detect a MouseDown on our glyph.
    ///  The Glyph can use this state to always return 'true' from HitTesting
    ///  indicating that it would like all messages (like MouseMove).
    /// </summary>
    public bool OkToMove
    {
        get => _okToMove;
        set => _okToMove = value;
    }

    public Point InitialDragPoint
    {
        get => _initialDragPoint;
        set => _initialDragPoint = value;
    }

    /// <summary>
    ///  If the user selects the ContainerGlyph - select our related component.
    /// </summary>
    public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
    {
        if (button == MouseButtons.Left)
        {
            // select our component
            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
            if (selSvc is not null && !_containerControl.Equals(selSvc.PrimarySelection as Control))
            {
                selSvc.SetSelectedComponents(new object[] { _containerControl }, SelectionTypes.Primary | SelectionTypes.Toggle);
                // Setting the selected component will create a new glyph, so this instance of the glyph won't receive
                // any more mouse messages. So we need to tell the new glyph what the initialDragPoint and okToMove are.
                if (g is not ContainerSelectorGlyph selOld)
                {
                    return false;
                }

                foreach (Adorner a in _behaviorService.Adorners)
                {
                    foreach (Glyph glyph in a.Glyphs)
                    {
                        if (glyph is not ContainerSelectorGlyph selNew)
                        {
                            continue;
                        }

                        // Don't care if we are looking at the same ContainerSelectorGlyph
                        if (selNew.Equals(selOld))
                        {
                            continue;
                        }

                        // Check if the containerControls are the same
                        if (!(selNew.RelatedBehavior is ContainerSelectorBehavior behNew) || !(selOld.RelatedBehavior is ContainerSelectorBehavior behOld))
                        {
                            continue;
                        }

                        // and the related components are the same, then we have found the new glyph that just got added
                        if (behOld.ContainerControl.Equals(behNew.ContainerControl))
                        {
                            behNew.OkToMove = true;
                            behNew.InitialDragPoint = DetermineInitialDragPoint(mouseLoc);
                            break;
                        }
                    }
                }
            }
            else
            {
                InitialDragPoint = DetermineInitialDragPoint(mouseLoc);
                // set 'okToMove' to true since the user actually clicked down on the glyph
                OkToMove = true;
            }
        }

        return false;
    }

    private Point DetermineInitialDragPoint(Point mouseLoc)
    {
        if (_setInitialDragPoint)
        {
            // Set the mouse location to be to control's location.
            Point controlOrigin = _behaviorService.ControlToAdornerWindow(_containerControl);
            controlOrigin = _behaviorService.AdornerWindowPointToScreen(controlOrigin);
            Cursor.Position = controlOrigin;
            return controlOrigin;
        }
        else
        {
            // This really amounts to doing nothing
            return mouseLoc;
        }
    }

    /// <summary>
    ///  We will compare the mouse loc to the initial point (set in OnMouseDown) and if we're far enough,
    ///  we'll create a <see cref="DropSourceBehavior"/> object and start out drag operation!
    /// </summary>
    public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
    {
        if (button == MouseButtons.Left && OkToMove)
        {
            if (InitialDragPoint == Point.Empty)
            {
                InitialDragPoint = DetermineInitialDragPoint(mouseLoc);
            }

            Size delta = new(Math.Abs(mouseLoc.X - InitialDragPoint.X), Math.Abs(mouseLoc.Y - InitialDragPoint.Y));
            if (delta.Width >= DesignerUtils.MinDragSize.Width / 2 || delta.Height >= DesignerUtils.MinDragSize.Height / 2)
            {
                // start our drag!
                Point screenLoc = _behaviorService.AdornerWindowToScreen();
                screenLoc.Offset(mouseLoc.X, mouseLoc.Y);
                StartDragOperation(screenLoc);
            }
        }

        return false;
    }

    /// <summary>
    ///  Simply clear the initial drag point, so we can start again on the next mouse down.
    /// </summary>
    public override bool OnMouseUp(Glyph g, MouseButtons button)
    {
        InitialDragPoint = Point.Empty;
        OkToMove = false;
        return false;
    }

    /// <summary>
    ///  Called when we've identified that we want to start a drag operation with our data container.
    /// </summary>
    private void StartDragOperation(Point initialMouseLocation)
    {
        // need to grab a hold of some services
        ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
        IDesignerHost host = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        if (selSvc is null || host is null)
        {
            Debug.Fail("Can't drag this Container! Either SelectionService is null or DesignerHost is null");
            return;
        }

        // must identify a required parent to avoid dragging mixes of children
        Control requiredParent = _containerControl.Parent;
        List<IComponent> dragControls = [];
        ICollection selComps = selSvc.GetSelectedComponents();
        // create our list of controls-to-drag
        foreach (IComponent comp in selComps)
        {
            if ((comp is Control ctrl) && (ctrl.Parent is not null))
            {
                if (!ctrl.Parent.Equals(requiredParent))
                {
                    continue; // mixed selection of different parents - don't add this
                }

                if (host.GetDesigner(ctrl) is ControlDesigner des && (des.SelectionRules & SelectionRules.Moveable) != 0)
                {
                    dragControls.Add(ctrl);
                }
            }
        }

        // if we have controls-to-drag, create our new behavior and start the drag/drop operation
        if (dragControls.Count > 0)
        {
            Point controlOrigin;
            if (_setInitialDragPoint)
            {
                // In this case we want the initialMouseLocation to be the control's origin.
                controlOrigin = _behaviorService.ControlToAdornerWindow(_containerControl);
                controlOrigin = _behaviorService.AdornerWindowPointToScreen(controlOrigin);
            }
            else
            {
                controlOrigin = initialMouseLocation;
            }

            DropSourceBehavior dsb = new(dragControls, _containerControl.Parent, controlOrigin);
            try
            {
                _behaviorService.DoDragDrop(dsb);
            }
            finally
            {
                OkToMove = false;
                InitialDragPoint = Point.Empty;
            }
        }
    }
}
