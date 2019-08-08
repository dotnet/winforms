// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The DragAssistanceManager, for lack of a better name, is responsible for integrating SnapLines into the DragBehavior.  At the beginning of a DragBehavior this class is instantiated and at every mouse move this class is called and given the opportunity to adjust the position of the drag.  The DragAssistanceManager needs to work as fast as possible - so not to interupt a drag operation.  Because of this, this class has many global variables that are re-used, in hopes to limit the # of allocations per mouse move / drag operation.  Also, for loops are used extensively (instead of foreach calls) to eliminate the creation of an enumerator.
    /// </summary>
    internal sealed class DragAssistanceManager
    {
        private readonly BehaviorService _behaviorService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Graphics _graphics; //graphics to the adornerwindow
        private readonly IntPtr _rootComponentHandle; //used for mapping window points of nested controls
        private Point _dragOffset; //the offset from the new drag pos compared to the last
        private Rectangle _cachedDragRect; //used to store drag rect between erasing & waiting to render
        private readonly Pen _edgePen = SystemPens.Highlight;
        private readonly bool _disposeEdgePen = false;
        private readonly Pen _baselinePen = new Pen(Color.Fuchsia);
        // These are global lists of all the existing vertical and hoirizontal snaplineson the designer's surface excluding the targetControl.  All SnapLine coords in theselists have been properly adjusted for the AdornerWindow coords.
        private readonly ArrayList _verticalSnapLines = new ArrayList();
        private readonly ArrayList _horizontalSnapLines = new ArrayList();
        // These are SnapLines that represent our target control.
        private readonly ArrayList _targetVerticalSnapLines = new ArrayList();
        private readonly ArrayList _targetHorizontalSnapLines = new ArrayList();
        // This is a list of all the different type of SnapLines our target controlhas.  When compiling our global SnapLine lists, if we see a SnapLineTypethat doesn't exist on our target - we can safely ignore it
        private readonly ArrayList _targetSnapLineTypes = new ArrayList();
        // These are created in our init() method (so we don't have to recreate them for every mousemove). These arrays represent the closest distance to any snap point on our target control.  Once these are calculated - we can: 1) remove anything > than snapDistance and 2) determine the smallest distanceoverall
        private int[] _verticalDistances;
        private int[] _horizontalDistances;
        //T hese are cleared and populated on every mouse move.  These lists contain all the new vertical and horizontal lines we need to draw.  At the end of each mouse move - these lines are stored off in the vertLines and horzLines arrays.  This way - we can keep track of old snap lines and can avoid erasing and redrawing the same line.  HA.
        private readonly ArrayList _tempVertLines = new ArrayList();
        private readonly ArrayList _tempHorzLines = new ArrayList();
        private Line[] _vertLines = Array.Empty<Line>();
        private Line[] _horzLines = Array.Empty<Line>();
        // When we draw snap lines - we only draw lines from the targetControl to the control we're snapping to.  To do this, we'll keep a hashtable... format: snapLineToBounds[SnapLine]=ControlBounds.
        private readonly Hashtable _snapLineToBounds = new Hashtable();
        // We remember the last set of (vert & horz) lines we draw so that we can push them to the beh. svc.  From there, if we receive a test hook message requesting these - we got 'em
        private Line[] _recentLines;
        private readonly Image _backgroundImage; //instead of calling .invalidate on the windows below us, we'll just draw over w/the background image
        private const int SnapDistance = 8; //default snapping distance (pixels)
        private int _snapPointX, _snapPointY; //defines the snap adjustment that needs to be made during the mousemove/drag operation
        private const int INVALID_VALUE = 0x1111; //used to represent 'un-set' distances
        private readonly bool _resizing; // Are we resizing?
        private readonly bool _ctrlDrag; // Are we in a ctrl-drag?

        /// <summary>
        ///  Internal constructor called that only takes a service provider.  Here it is assumed that all painting will be done to the AdornerWindow and that there are no target controsl to exclude from snapping.
        /// </summary>
        internal DragAssistanceManager(IServiceProvider serviceProvider) : this(serviceProvider, null, null, null, false, false)
        {
        }

        /// <summary>
        ///  Internal constructor that takes the service provider and the list of dragCompoents.
        /// </summary>
        internal DragAssistanceManager(IServiceProvider serviceProvider, ArrayList dragComponents) : this(serviceProvider, null, dragComponents, null, false, false)
        {
        }

        /// <summary>
        ///  Internal constructor that takes the service provider, the list of dragCompoents, and a boolean
        ///  indicating that we are resizing.
        /// </summary>
        internal DragAssistanceManager(IServiceProvider serviceProvider, ArrayList dragComponents, bool resizing) : this(serviceProvider, null, dragComponents, null, resizing, false)
        {
        }

        /// <summary>
        ///  Internal constructor called by DragBehavior.
        /// </summary>
        internal DragAssistanceManager(IServiceProvider serviceProvider, Graphics graphics, ArrayList dragComponents, Image backgroundImage, bool ctrlDrag) : this(serviceProvider, graphics, dragComponents, backgroundImage, false, ctrlDrag)
        {
        }

        /// <summary>
        ///  Internal constructor called by DragBehavior.
        /// </summary>
        internal DragAssistanceManager(IServiceProvider serviceProvider, Graphics graphics, ArrayList dragComponents, Image backgroundImage, bool resizing, bool ctrlDrag)
        {
            _serviceProvider = serviceProvider;
            _behaviorService = serviceProvider.GetService(typeof(BehaviorService)) as BehaviorService;
            if (!(serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host) || _behaviorService == null)
            {
                Debug.Fail("Cannot get DesignerHost or BehaviorService");
                return;
            }

            if (graphics == null)
            {
                _graphics = _behaviorService.AdornerWindowGraphics;
            }
            else
            {
                _graphics = graphics;
            }

            if (serviceProvider.GetService(typeof(IUIService)) is IUIService uiService)
            {
                //Can't use 'as' here since Color is a value type
                if (uiService.Styles["VsColorSnaplines"] is Color)
                {
                    _edgePen = new Pen((Color)uiService.Styles["VsColorSnaplines"]);
                    _disposeEdgePen = true;
                }

                if (uiService.Styles["VsColorSnaplinesTextBaseline"] is Color)
                {
                    _baselinePen.Dispose();
                    _baselinePen = new Pen((Color)uiService.Styles["VsColorSnaplinesTextBaseline"]);
                }
            }
            _backgroundImage = backgroundImage;
            _rootComponentHandle = host.RootComponent is Control ? ((Control)host.RootComponent).Handle : IntPtr.Zero;
            _resizing = resizing;
            _ctrlDrag = ctrlDrag;
            Initialize(dragComponents, host);
        }

        /// <summary>
        ///  Adjusts then adds each snap line the designer has to offer to either our global horizontal and vertical lists or our target lists. Note that we also keep track of our target snapline types - 'cause we can safely ignore all other types.  If valid target is false- then we don't yet know what we're snapping against - so we'll exclude the check below to skip unwanted snap line types.
        /// </summary>
        private void AddSnapLines(ControlDesigner controlDesigner, ArrayList horizontalList, ArrayList verticalList, bool isTarget, bool validTarget)
        {
            IList snapLines = controlDesigner.SnapLines;
            //Used for padding snaplines
            Rectangle controlRect = controlDesigner.Control.ClientRectangle;
            //Used for all others
            Rectangle controlBounds = controlDesigner.Control.Bounds;
            // Now map the location
            controlBounds.Location = controlRect.Location = _behaviorService.ControlToAdornerWindow(controlDesigner.Control);
            // Remember the offset -- we need those later
            int xOffset = controlBounds.Left;
            int yOffset = controlBounds.Top;

            // THIS IS ONLY NEEDED FOR PADDING SNAPLINES
            // We need to adjust the bounds to the client area. This is so that we don't include borders + titlebar in the snaplines. In order to add padding, we need to get the offset from the usable client area of our control and the actual origin of our control.  In other words: how big is the non-client area here? Ex: we want to add padding on a form to the insides of the borders and below the titlebar.
            Point offset = controlDesigner.GetOffsetToClientArea();
            controlRect.X += offset.X; //offset for non-client area
            controlRect.Y += offset.Y; //offset for non-client area

            //Adjust each snapline to local coords and add it to our global list
            foreach (SnapLine snapLine in snapLines)
            {
                if (isTarget)
                {
                    //we will remove padding snaplines from targets - it doesn't make sense to snap to the target's padding lines
                    if (snapLine.Filter != null && snapLine.Filter.StartsWith(SnapLine.Padding))
                    {
                        continue;
                    }

                    if (validTarget && !_targetSnapLineTypes.Contains(snapLine.SnapLineType))
                    {
                        _targetSnapLineTypes.Add(snapLine.SnapLineType);
                    }
                }
                else
                {
                    if (validTarget && !_targetSnapLineTypes.Contains(snapLine.SnapLineType))
                    {
                        continue;
                    }
                    // store off the bounds in our hashtable, so if we draw snaplines we know the length of the line we need to remember different bounds based on what type of snapline this is.
                    if ((snapLine.Filter != null) && snapLine.Filter.StartsWith(SnapLine.Padding))
                    {
                        _snapLineToBounds.Add(snapLine, controlRect);
                    }
                    else
                    {
                        _snapLineToBounds.Add(snapLine, controlBounds);
                    }
                }

                if (snapLine.IsHorizontal)
                {
                    snapLine.AdjustOffset(yOffset);
                    horizontalList.Add(snapLine);
                }
                else
                {
                    snapLine.AdjustOffset(xOffset);
                    verticalList.Add(snapLine);
                }
            }
        }

        /// <summary>
        ///  Build up a distance array of all same-type-alignment pts to the closest point on our targetControl.  Also, keep track of the smallest distance overall.
        /// </summary>
        private int BuildDistanceArray(ArrayList snapLines, ArrayList targetSnapLines, int[] distances, Rectangle dragBounds)
        {
            int smallestDistance = INVALID_VALUE;
            int highestPriority = 0;

            for (int i = 0; i < snapLines.Count; i++)
            {
                SnapLine snapLine = (SnapLine)snapLines[i];
                if (IsMarginOrPaddingSnapLine(snapLine))
                {
                    // validate margin and padding snaplines (to make sure it intersects with the dragbounds) if not, skip this guy
                    if (!ValidateMarginOrPaddingLine(snapLine, dragBounds))
                    {
                        distances[i] = INVALID_VALUE;
                        continue;
                    }
                }

                int smallestDelta = INVALID_VALUE; //some large #
                for (int j = 0; j < targetSnapLines.Count; j++)
                {
                    SnapLine targetSnapLine = (SnapLine)targetSnapLines[j];

                    if (SnapLine.ShouldSnap(snapLine, targetSnapLine))
                    {
                        int delta = targetSnapLine.Offset - snapLine.Offset;
                        if (Math.Abs(delta) < Math.Abs(smallestDelta))
                        {
                            smallestDelta = delta;
                        }
                    }
                }

                distances[i] = smallestDelta;
                int pri = (int)((SnapLine)snapLines[i]).Priority;
                //save off this delta for the overall smallest delta! Need to check the priority here as well if the distance is the same. E.g. smallestDistance so far is 1, for a Low snapline. We now find another distance of -1, for a Medium snapline. The old check if (Math.Abs(smallestDelta) < Math.Abs(smallestDistance)) would not set smallestDistance to -1, since the ABSOLUTE values are the same. Since the return value is used to phycially move the control, we would move the control in the direction of the Low snapline, but draw the Medium snapline in the opposite direction.
                if ((Math.Abs(smallestDelta) < Math.Abs(smallestDistance)) ||
                    ((Math.Abs(smallestDelta) == Math.Abs(smallestDistance)) && (pri > highestPriority)))
                {
                    smallestDistance = smallestDelta;
                    if (pri != (int)SnapLinePriority.Always)
                    {
                        highestPriority = pri;
                    }
                }
            }
            return smallestDistance;
        }

        /// <summary>
        ///  Here, we erase all of our old horizontal and vertical snaplines UNLESS they are also contained in our tempHorzLines or tempVertLines arrays - if they are - then erasing them would be redundant (since we know we want to draw them on this mousemove)
        /// </summary>
        private Line[] EraseOldSnapLines(Line[] lines, ArrayList tempLines)
        {
            if (lines != null)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    Rectangle invalidRect = Rectangle.Empty;
                    bool foundMatch = false;
                    Line line = lines[i];
                    if (tempLines != null)
                    {
                        for (int j = 0; j < tempLines.Count; j++)
                        {
                            if (line.LineType != ((Line)tempLines[j]).LineType)
                            {
                                // If the lines are not the same type, then we should forcefully try to remove it. Say you have a Panel with a Button in it. By default Panel.Padding = 0, and Button.Margin = 3. As you move the button to the left, you will first get the combined LEFT margin+padding snap line. If you keep moving the button, you will now snap to the Left edge, and you will get the Blue snapline. You now move the button back to the right, and you will immediately snap to the LEFT Padding snapline. But what's gonna happen. Both the old (Left) snapline, and the LEFT Padding snapline (remember these are the panels) have the same coordinates, since Panel.Padding is 0. Thus Line.GetDiffs will return a non-null diffs. BUT e.g the first line will result in an invalidRect of (x1,y1,0,0), this we end up invalidating only a small portion of the existing Blue (left) Snapline. That's actually not okay since VERTICAL (e.g. LEFT) padding snaplines actually end up getting drawn HORIZONTALLY - thus we didn't really invalidate correctly.
                                continue;
                            }
                            Line[] diffs = Line.GetDiffs(line, (Line)tempLines[j]);
                            if (diffs != null)
                            {
                                for (int k = 0; k < diffs.Length; k++)
                                {
                                    invalidRect = new Rectangle(diffs[k].x1, diffs[k].y1, diffs[k].x2 - diffs[k].x1, diffs[k].y2 - diffs[k].y1);

                                    invalidRect.Inflate(1, 1);
                                    if (_backgroundImage != null)
                                    {
                                        _graphics.DrawImage(_backgroundImage, invalidRect, invalidRect, GraphicsUnit.Pixel);
                                    }
                                    else
                                    {
                                        _behaviorService.Invalidate(invalidRect);
                                    }
                                }
                                foundMatch = true;
                                break;
                            }
                        }
                    }

                    if (!foundMatch)
                    {
                        invalidRect = new Rectangle(line.x1, line.y1, line.x2 - line.x1, line.y2 - line.y1);
                        invalidRect.Inflate(1, 1);
                        if (_backgroundImage != null)
                        {
                            _graphics.DrawImage(_backgroundImage, invalidRect, invalidRect, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            _behaviorService.Invalidate(invalidRect);
                        }
                    }
                }
            }

            if (tempLines != null)
            {
                // Now, store off all the new lines (from the temp structures), so next time around (next mousemove message) we know which lines to erase and which ones to keep
                lines = new Line[tempLines.Count];
                tempLines.CopyTo(lines);
            }
            else
            {
                lines = Array.Empty<Line>();
            }
            return lines;
        }

        internal void EraseSnapLines()
        {
            EraseOldSnapLines(_vertLines, null);
            EraseOldSnapLines(_horzLines, null);
        }

        /// <summary>
        ///  This internal method returns a snap line[] representing the last SnapLines that were rendered before this algorithm was stopped (usually by an OnMouseUp). This is used for storing additional toolbox drag/drop info and testing hooks.
        /// </summary>
        internal Line[] GetRecentLines()
        {
            if (_recentLines != null)
            {
                return _recentLines;
            }
            return Array.Empty<Line>();
        }

        private void IdentifyAndStoreValidLines(ArrayList snapLines, int[] distances, Rectangle dragBounds, int smallestDistance)
        {
            int highestPriority = 1; //low
            //identify top pri
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] == smallestDistance)
                {
                    int pri = (int)((SnapLine)snapLines[i]).Priority;
                    if ((pri > highestPriority) && (pri != (int)SnapLinePriority.Always))
                    { // Always is a special category
                        highestPriority = pri;
                    }
                }
            }

            //store all snapLines equal to the smallest distance (of the highest priority)
            for (int i = 0; i < distances.Length; i++)
            {
                if ((distances[i] == smallestDistance) &&
                  (((int)((SnapLine)snapLines[i]).Priority == highestPriority) ||
                    ((int)((SnapLine)snapLines[i]).Priority == (int)SnapLinePriority.Always)))
                { //always render SnapLines with Priority.Always which has the same distance.
                    StoreSnapLine((SnapLine)snapLines[i], dragBounds);
                }
            }
        }

        // Returns true of this child component (off the root control) should add its snaplines to the collection
        private bool AddChildCompSnaplines(IComponent comp, ArrayList dragComponents, Rectangle clipBounds, Control targetControl)
        {
            if (!(comp is Control control) || //has to be a control to get snaplines
               (dragComponents != null && dragComponents.Contains(comp) && !_ctrlDrag) || //cannot be something that we are dragging, unless we are in a ctrlDrag
               IsChildOfParent(control, targetControl) ||//cannot be a child of the control we will drag
               !clipBounds.IntersectsWith(control.Bounds) || //has to be partially visible on the rootcomp's surface
               control.Parent == null || // control must have a parent.
               !control.Visible)
            { //control itself has to be visible -- we do mean visible, not ShadowedVisible
                return false;
            }

            Control c = control;
            if (!c.Equals(targetControl))
            {
                if (_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    if (host.GetDesigner(c) is ControlDesigner controlDesigner)
                    {
                        return controlDesigner.ControlSupportsSnaplines;
                    }
                }
            }
            return true;
        }

        // Returns true if we should add snaplines for this control
        private bool AddControlSnaplinesWhenResizing(ControlDesigner designer, Control control, Control targetControl)
        {
            // do not add snaplines if we are resizing the control is a container control with AutoSize set to true and the control is the parent of the targetControl
            if (_resizing &&
                (designer is ParentControlDesigner) &&
                (control.AutoSize == true) &&
                (targetControl != null) &&
                (targetControl.Parent != null) &&
                (targetControl.Parent.Equals(control)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Initializes our class - we cache all snap lines for every control we can find. This is done for perf. reasons.
        /// </summary>
        private void Initialize(ArrayList dragComponents, IDesignerHost host)
        {
            // our targetControl will always be the 0th component in our dragComponents array list (a.k.a. the primary selected component).
            Control targetControl = null;
            if (dragComponents != null && dragComponents.Count > 0)
            {
                targetControl = dragComponents[0] as Control;
            }
            Control rootControl = host.RootComponent as Control;
            // the clipping bounds will be used to ignore all controls that are completely outside of our rootcomponent's bounds -this way we won't end up snapping to controls that are not visible on the form's surface
            Rectangle clipBounds = new Rectangle(0, 0, rootControl.ClientRectangle.Width, rootControl.ClientRectangle.Height);
            clipBounds.Inflate(-1, -1);
            //determine the screen offset from our rootComponent to the AdornerWindow (since all drag notification coords will be in adorner window coords)
            if (targetControl != null)
            {
                _dragOffset = _behaviorService.ControlToAdornerWindow(targetControl);
            }
            else
            {
                _dragOffset = _behaviorService.MapAdornerWindowPoint(rootControl.Handle, Point.Empty);
                if (rootControl.Parent != null && rootControl.Parent.IsMirrored)
                {
                    _dragOffset.Offset(-rootControl.Width, 0);
                }
            }

            if (targetControl != null)
            {
                bool disposeDesigner = false;
                //get all the target snapline information we need to create one then
                if (!(host.GetDesigner(targetControl) is ControlDesigner designer))
                {
                    designer = TypeDescriptor.CreateDesigner(targetControl, typeof(IDesigner)) as ControlDesigner;
                    if (designer != null)
                    {
                        //Make sure the control is not forced visible
                        designer.ForceVisible = false;
                        designer.Initialize(targetControl);
                        disposeDesigner = true;
                    }
                }
                AddSnapLines(designer, _targetHorizontalSnapLines, _targetVerticalSnapLines, true, targetControl != null);
                if (disposeDesigner)
                {
                    designer.Dispose();
                }
            }

            //get SnapLines for all our children (nested too) off our root control
            foreach (IComponent comp in host.Container.Components)
            {
                if (!AddChildCompSnaplines(comp, dragComponents, clipBounds, targetControl))
                {
                    continue;
                }

                if (host.GetDesigner(comp) is ControlDesigner designer)
                {
                    if (AddControlSnaplinesWhenResizing(designer, comp as Control, targetControl))
                    {
                        AddSnapLines(designer, _horizontalSnapLines, _verticalSnapLines, false, targetControl != null);
                    }

                    // Does the designer have internal control designers for which we need to add snaplines (like SplitPanelContainer, ToolStripContainer)
                    int numInternalDesigners = designer.NumberOfInternalControlDesigners();
                    for (int i = 0; i < numInternalDesigners; i++)
                    {
                        ControlDesigner internalDesigner = designer.InternalControlDesigner(i);
                        if (internalDesigner != null &&
                            AddChildCompSnaplines(internalDesigner.Component, dragComponents, clipBounds, targetControl) &&
                            AddControlSnaplinesWhenResizing(internalDesigner, internalDesigner.Component as Control, targetControl))
                        {
                            AddSnapLines(internalDesigner, _horizontalSnapLines, _verticalSnapLines, false, targetControl != null);
                        }
                    }
                }
            }

            // Now that we know how many snaplines everyone has, we can create temp arrays now.  Intentionally avoiding this on every mousemove.
            _verticalDistances = new int[_verticalSnapLines.Count];
            _horizontalDistances = new int[_horizontalSnapLines.Count];
        }

        /// <summary>
        ///  Helper function that determines if the child control is related to the parent.
        /// </summary>
        private static bool IsChildOfParent(Control child, Control parent)
        {
            if (child == null || parent == null)
            {
                return false;
            }
            Control currentParent = child.Parent;
            while (currentParent != null)
            {
                if (currentParent.Equals(parent))
                {
                    return true;
                }
                currentParent = currentParent.Parent;
            }
            return false;
        }

        /// <summary>
        ///  Helper function that identifies margin or padding snaplines
        /// </summary>
        private static bool IsMarginOrPaddingSnapLine(SnapLine snapLine)
        {
            return snapLine.Filter != null && (snapLine.Filter.StartsWith(SnapLine.Margin) || snapLine.Filter.StartsWith(SnapLine.Padding));
        }

        /// <summary>
        ///  Returns the offset in which the targetControl's rect needs to be re-positioned (given the direction by 'directionOffset') in order to align with the nearest possible snapline.  This is called by commandSet during keyboard movements to auto-snap the control around the designer.
        /// </summary>
        internal Point OffsetToNearestSnapLocation(Control targetControl, IList targetSnaplines, Point directionOffset)
        {
            _targetHorizontalSnapLines.Clear();
            _targetVerticalSnapLines.Clear();
            //manually add our snaplines as targets
            foreach (SnapLine snapline in targetSnaplines)
            {
                if (snapline.IsHorizontal)
                {
                    _targetHorizontalSnapLines.Add(snapline);
                }
                else
                {
                    _targetVerticalSnapLines.Add(snapline);
                }
            }
            return OffsetToNearestSnapLocation(targetControl, directionOffset);
        }

        /// <summary>
        ///  Returns the offset in which the targetControl's rect needs to be re-positioned (given the direction by 'directionOffset') in order to align with the nearest possible snapline.  This is called by commandSet during keyboard movements to auto-snap the control around the designer.
        /// </summary>
        internal Point OffsetToNearestSnapLocation(Control targetControl, Point directionOffset)
        {
            Point offset = Point.Empty;
            Rectangle currentBounds = new Rectangle(_behaviorService.ControlToAdornerWindow(targetControl), targetControl.Size);
            if (directionOffset.X != 0)
            {//movement somewhere in the x dir
                //first, build up our distance array
                BuildDistanceArray(_verticalSnapLines, _targetVerticalSnapLines, _verticalDistances, currentBounds);
                //now start with the smallest distance and find the first snapline we would intercept given our horizontal direction
                int minRange = directionOffset.X < 0 ? 0 : currentBounds.X;
                int maxRange = directionOffset.X < 0 ? currentBounds.Right : int.MaxValue;
                offset.X = FindSmallestValidDistance(_verticalSnapLines, _verticalDistances, minRange, maxRange, directionOffset.X);
                if (offset.X != 0)
                {
                    //store off the line structs for actual rendering
                    IdentifyAndStoreValidLines(_verticalSnapLines, _verticalDistances, currentBounds, offset.X);
                    if (directionOffset.X < 0)
                    {
                        offset.X *= -1;
                    }
                }
            }
            if (directionOffset.Y != 0)
            {//movement somewhere in the y dir
                //first, build up our distance array
                BuildDistanceArray(_horizontalSnapLines, _targetHorizontalSnapLines, _horizontalDistances, currentBounds);
                //now start with the smallest distance and find the first snapline we would intercept given our horizontal direction
                int minRange = directionOffset.Y < 0 ? 0 : currentBounds.Y;
                int maxRange = directionOffset.Y < 0 ? currentBounds.Bottom : int.MaxValue;
                offset.Y = FindSmallestValidDistance(_horizontalSnapLines, _horizontalDistances, minRange, maxRange, directionOffset.Y);
                if (offset.Y != 0)
                {
                    //store off the line structs for actual rendering
                    IdentifyAndStoreValidLines(_horizontalSnapLines, _horizontalDistances, currentBounds, offset.Y);
                    if (directionOffset.Y < 0)
                    {
                        offset.Y *= -1;
                    }
                }
            }

            if (!offset.IsEmpty)
            {
                //setup the cached info for drawing
                _cachedDragRect = currentBounds;
                _cachedDragRect.Offset(offset.X, offset.Y);
                if (offset.X != 0)
                {
                    _vertLines = new Line[_tempVertLines.Count];
                    _tempVertLines.CopyTo(_vertLines);
                }
                if (offset.Y != 0)
                {
                    _horzLines = new Line[_tempHorzLines.Count];
                    _tempHorzLines.CopyTo(_horzLines);
                }
            }

            return offset;
        }

        private static int FindSmallestValidDistance(ArrayList snapLines, int[] distances, int min, int max, int direction)
        {
            int distanceValue = 0;
            int snapLineIndex = 0;
            // loop while we still have valid distance to check and try to find the smallest valid distance
            while (true)
            {
                // get the next smallest snapline index
                snapLineIndex = SmallestDistanceIndex(distances, direction, out distanceValue);

                if (snapLineIndex == INVALID_VALUE)
                {
                    // ran out of valid distances
                    break;
                }
                if (IsWithinValidRange(((SnapLine)snapLines[snapLineIndex]).Offset, min, max))
                {
                    // found it - make sure we restore the original value for rendering the snap line in the future
                    distances[snapLineIndex] = distanceValue;
                    return distanceValue;
                }
            }
            return 0;
        }

        private static bool IsWithinValidRange(int offset, int min, int max) => offset > min && offset < max;

        private static int SmallestDistanceIndex(int[] distances, int direction, out int distanceValue)
        {
            distanceValue = INVALID_VALUE;
            int smallestIndex = INVALID_VALUE;
            //check for valid array
            if (distances.Length == 0)
            {
                return smallestIndex;
            }

            //find the next smallest
            for (int i = 0; i < distances.Length; i++)
            {
                // If a distance is 0 or if it is to our left and we're heading right or if it is to our right and we're heading left then we can null this value out
                if (distances[i] == 0 ||
                  distances[i] > 0 && direction > 0 ||
                  distances[i] < 0 && direction < 0)
                {
                    distances[i] = INVALID_VALUE;
                }

                if (Math.Abs(distances[i]) < distanceValue)
                {
                    distanceValue = Math.Abs(distances[i]);
                    smallestIndex = i;
                }
            }

            if (smallestIndex < distances.Length)
            {
                //return and clear the smallest one we found
                distances[smallestIndex] = INVALID_VALUE;
            }
            return smallestIndex;
        }

        /// <summary>
        ///  Actually draws the snaplines based on type, location, and specified pen
        /// </summary>
        private void RenderSnapLines(Line[] lines, Rectangle dragRect)
        {
            Pen currentPen;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].LineType == LineType.Margin || lines[i].LineType == LineType.Padding)
                {
                    currentPen = _edgePen;
                    if (lines[i].x1 == lines[i].x2)
                    {//vertical margin
                        int coord = Math.Max(dragRect.Top, lines[i].OriginalBounds.Top);
                        coord += (Math.Min(dragRect.Bottom, lines[i].OriginalBounds.Bottom) - coord) / 2;
                        lines[i].y1 = lines[i].y2 = coord;
                        if (lines[i].LineType == LineType.Margin)
                        {
                            lines[i].x1 = Math.Min(dragRect.Right, lines[i].OriginalBounds.Right);
                            lines[i].x2 = Math.Max(dragRect.Left, lines[i].OriginalBounds.Left);
                        }
                        else if (lines[i].PaddingLineType == PaddingLineType.PaddingLeft)
                        {
                            lines[i].x1 = lines[i].OriginalBounds.Left;
                            lines[i].x2 = dragRect.Left;
                        }
                        else
                        {
                            Debug.Assert(lines[i].PaddingLineType == PaddingLineType.PaddingRight);
                            lines[i].x1 = dragRect.Right;
                            lines[i].x2 = lines[i].OriginalBounds.Right;
                        }
                        lines[i].x2--; //off by 1 adjust
                    }
                    else
                    {//horizontal margin
                        int coord = Math.Max(dragRect.Left, lines[i].OriginalBounds.Left);
                        coord += (Math.Min(dragRect.Right, lines[i].OriginalBounds.Right) - coord) / 2;
                        lines[i].x1 = lines[i].x2 = coord;
                        if (lines[i].LineType == LineType.Margin)
                        {
                            lines[i].y1 = Math.Min(dragRect.Bottom, lines[i].OriginalBounds.Bottom);
                            lines[i].y2 = Math.Max(dragRect.Top, lines[i].OriginalBounds.Top);
                        }
                        else if (lines[i].PaddingLineType == PaddingLineType.PaddingTop)
                        {
                            lines[i].y1 = lines[i].OriginalBounds.Top;
                            lines[i].y2 = dragRect.Top;
                        }
                        else
                        {
                            Debug.Assert(lines[i].PaddingLineType == PaddingLineType.PaddingBottom);
                            lines[i].y1 = dragRect.Bottom;
                            lines[i].y2 = lines[i].OriginalBounds.Bottom;
                        }
                        lines[i].y2--; //off by 1 adjust
                    }
                }
                else if (lines[i].LineType == LineType.Baseline)
                {
                    currentPen = _baselinePen;
                    lines[i].x2 -= 1; //off by 1 adjust
                }
                else
                {
                    //default to edgePen
                    currentPen = _edgePen;
                    if (lines[i].x1 == lines[i].x2)
                    {
                        lines[i].y2--; //off by 1 adjustment
                    }
                    else
                    {
                        lines[i].x2--; //off by 1 adjustment
                    }
                }
                _graphics.DrawLine(currentPen, lines[i].x1, lines[i].y1, lines[i].x2, lines[i].y2);
            }
        }

        /// <summary>
        ///  Performance improvement: Given an snapline we will render, check if it overlaps with an existing snapline. If so, combine the two.
        /// </summary>
        private static void CombineSnaplines(Line snapLine, ArrayList currentLines)
        {
            bool merged = false;
            for (int i = 0; i < currentLines.Count; i++)
            {
                Line curLine = (Line)currentLines[i];
                Line mergedLine = Line.Overlap(snapLine, curLine);
                if (mergedLine != null)
                {
                    currentLines[i] = mergedLine;
                    merged = true;
                }
            }
            if (!merged)
            {
                currentLines.Add(snapLine);
            }
        }

        /// <summary>
        ///  Here, we store all the SnapLines we will render.  This way we can erase them when they are no longer needed.
        /// </summary>
        private void StoreSnapLine(SnapLine snapLine, Rectangle dragBounds)
        {
            Rectangle bounds = (Rectangle)_snapLineToBounds[snapLine];
            // In order for CombineSnapelines to work correctly, we have to determine the type first
            LineType type = LineType.Standard;
            if (IsMarginOrPaddingSnapLine(snapLine))
            {
                type = snapLine.Filter.StartsWith(SnapLine.Margin) ? LineType.Margin : LineType.Padding;
            }
            //propagate the baseline through to the linetype
            else if (snapLine.SnapLineType == SnapLineType.Baseline)
            {
                type = LineType.Baseline;
            }

            Line line;
            if (snapLine.IsVertical)
            {
                line = new Line(snapLine.Offset, Math.Min(dragBounds.Top + (_snapPointY != INVALID_VALUE ? _snapPointY : 0), bounds.Top),
                             snapLine.Offset, Math.Max(dragBounds.Bottom + (_snapPointY != INVALID_VALUE ? _snapPointY : 0), bounds.Bottom))
                {
                    LineType = type
                };
                // Performance improvement: Check if the newly added line overlaps existing lines and if so, combine them.
                CombineSnaplines(line, _tempVertLines);
            }
            else
            {
                line = new Line(Math.Min(dragBounds.Left + (_snapPointX != INVALID_VALUE ? _snapPointX : 0), bounds.Left), snapLine.Offset,
                             Math.Max(dragBounds.Right + (_snapPointX != INVALID_VALUE ? _snapPointX : 0), bounds.Right), snapLine.Offset)
                {
                    LineType = type
                };
                // Performance improvement: Check if the newly added line overlaps existing lines and if so, combine them.
                CombineSnaplines(line, _tempHorzLines);
            }

            if (IsMarginOrPaddingSnapLine(snapLine))
            {
                line.OriginalBounds = bounds;
                // need to know which padding line (left, right) we are storing. The original check in RenderSnapLines was wrong. It assume that the dragRect was completely within the OriginalBounds which is not necessarily true
                if (line.LineType == LineType.Padding)
                {
                    switch (snapLine.Filter)
                    {
                        case SnapLine.PaddingRight:
                            line.PaddingLineType = PaddingLineType.PaddingRight;
                            break;
                        case SnapLine.PaddingLeft:
                            line.PaddingLineType = PaddingLineType.PaddingLeft;
                            break;
                        case SnapLine.PaddingTop:
                            line.PaddingLineType = PaddingLineType.PaddingTop;
                            break;
                        case SnapLine.PaddingBottom:
                            line.PaddingLineType = PaddingLineType.PaddingBottom;
                            break;
                        default:
                            Debug.Fail("Unknown snapline filter type");
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///  This function validates a Margin or Padding SnapLine.  A valid Margin SnapLine is one that will be drawn only if the target control being dragged somehow intersects (vertically or horizontally) the coords of the given snapLine. This is done so we don't start drawing margin lines when controls are large distances apart (too much mess);
        /// </summary>
        private bool ValidateMarginOrPaddingLine(SnapLine snapLine, Rectangle dragBounds)
        {
            Rectangle bounds = (Rectangle)_snapLineToBounds[snapLine];
            if (snapLine.IsVertical)
            {
                if (bounds.Top < dragBounds.Top)
                {
                    if (bounds.Top + bounds.Height < dragBounds.Top)
                    {
                        return false;
                    }
                }
                else if (dragBounds.Top + dragBounds.Height < bounds.Top)
                {
                    return false;
                }
            }
            else
            {
                if (bounds.Left < dragBounds.Left)
                {
                    if (bounds.Left + bounds.Width < dragBounds.Left)
                    {
                        return false;
                    }
                }
                else if (dragBounds.Left + dragBounds.Width < bounds.Left)
                {
                    return false;
                }
            }
            //valid overlapping margin line
            return true;
        }

        internal Point OnMouseMove(Rectangle dragBounds, SnapLine[] snapLines)
        {
            bool didSnap = false;
            return OnMouseMove(dragBounds, snapLines, ref didSnap, true);
        }

        /// <summary>
        ///  Called by the DragBehavior on every mouse move.  We first offset all of our drag-control's snap lines by the amount of the mouse move then follow our 2-pass heuristic to determine which SnapLines to render.
        /// </summary>
        internal Point OnMouseMove(Rectangle dragBounds, SnapLine[] snapLines, ref bool didSnap, bool shouldSnapHorizontally)
        {
            if (snapLines == null || snapLines.Length == 0)
            {
                return Point.Empty;
            }
            _targetHorizontalSnapLines.Clear();
            _targetVerticalSnapLines.Clear();
            //manually add our snaplines as targets
            foreach (SnapLine snapline in snapLines)
            {
                if (snapline.IsHorizontal)
                {
                    _targetHorizontalSnapLines.Add(snapline);
                }
                else
                {
                    _targetVerticalSnapLines.Add(snapline);
                }
            }
            return OnMouseMove(dragBounds, false, ref didSnap, shouldSnapHorizontally);
        }

        /// <summary>
        ///  Called by the DragBehavior on every mouse move.  We first offset all of our drag-control's snap lines by the amount of the mouse move then follow our 2-pass heuristic to determine which SnapLines to render.
        /// </summary>
        internal Point OnMouseMove(Rectangle dragBounds)
        {
            bool didSnap = false;
            return OnMouseMove(dragBounds, true, ref didSnap, true);
        }

        /// <summary>
        ///  Called by the resizebehavior. It needs to know whether we really snapped or not. The snapPoint could be (0,0) even though we snapped.
        /// </summary>
        internal Point OnMouseMove(Control targetControl, SnapLine[] snapLines, ref bool didSnap, bool shouldSnapHorizontally)
        {
            Rectangle dragBounds = new Rectangle(_behaviorService.ControlToAdornerWindow(targetControl), targetControl.Size);
            didSnap = false;
            return OnMouseMove(dragBounds, snapLines, ref didSnap, shouldSnapHorizontally);
        }

        /// <summary>
        ///  Called by the DragBehavior on every mouse move.  We first offset all of our drag-control's snap lines by the amount of the mouse move then follow our 2-pass heuristic to determine which SnapLines to render.
        /// </summary>
        private Point OnMouseMove(Rectangle dragBounds, bool offsetSnapLines, ref bool didSnap, bool shouldSnapHorizontally)
        {
            _tempVertLines.Clear();
            _tempHorzLines.Clear();
            _dragOffset = new Point(dragBounds.X - _dragOffset.X, dragBounds.Y - _dragOffset.Y);
            if (offsetSnapLines)
            {
                //offset our targetSnapLines by the amount we have dragged it
                for (int i = 0; i < _targetHorizontalSnapLines.Count; i++)
                {
                    ((SnapLine)_targetHorizontalSnapLines[i]).AdjustOffset(_dragOffset.Y);
                }
                for (int i = 0; i < _targetVerticalSnapLines.Count; i++)
                {
                    ((SnapLine)_targetVerticalSnapLines[i]).AdjustOffset(_dragOffset.X);
                }
            }

            //First pass - build up a distance array of all same-type-alignment pts to theclosest point on our targetControl.  Also, keep track of the smallestdistance overall
            int smallestDistanceVert = BuildDistanceArray(_verticalSnapLines, _targetVerticalSnapLines, _verticalDistances, dragBounds);
            int smallestDistanceHorz = INVALID_VALUE;
            if (shouldSnapHorizontally)
            {
                smallestDistanceHorz = BuildDistanceArray(_horizontalSnapLines, _targetHorizontalSnapLines, _horizontalDistances, dragBounds);
            }
            //Second Pass!  We only need to do a second pass if the smallest delta is <= SnapDistance. If this is the case - then we draw snap lines for every line equal to the smallest distance available in the distance array
            _snapPointX = (Math.Abs(smallestDistanceVert) <= SnapDistance) ? -smallestDistanceVert : INVALID_VALUE;
            _snapPointY = (Math.Abs(smallestDistanceHorz) <= SnapDistance) ? -smallestDistanceHorz : INVALID_VALUE;
            // certain behaviors (like resize) might want to know whether we really snapped or not. They can't check the returned snapPoint for (0,0) since that is a valid snapPoint.
            didSnap = false;
            if (_snapPointX != INVALID_VALUE)
            {
                IdentifyAndStoreValidLines(_verticalSnapLines, _verticalDistances, dragBounds, smallestDistanceVert);
                didSnap = true;
            }

            if (_snapPointY != INVALID_VALUE)
            {
                IdentifyAndStoreValidLines(_horizontalSnapLines, _horizontalDistances, dragBounds, smallestDistanceHorz);
                didSnap = true;
            }

            Point snapPoint = new Point(_snapPointX != INVALID_VALUE ? _snapPointX : 0, _snapPointY != INVALID_VALUE ? _snapPointY : 0);
            Rectangle tempDragRect = new Rectangle(dragBounds.Left + snapPoint.X, dragBounds.Top + snapPoint.Y, dragBounds.Width, dragBounds.Height);
            //out with the old...
            _vertLines = EraseOldSnapLines(_vertLines, _tempVertLines);
            _horzLines = EraseOldSnapLines(_horzLines, _tempHorzLines);
            //store this drag rect - we'll use it when we are (eventually) called back on to actually render our lines

            //NOTE NOTE NOTE: If OnMouseMove is called during a resize operation, then cachedDragRect is not guaranteed to work. That is why I introduced RenderSnapLinesInternal(dragRect)
            _cachedDragRect = tempDragRect;
            //reset the dragoffset to this last location
            _dragOffset = dragBounds.Location;
            //this 'snapPoint' will be the amount we want the dragBehavior to shift the dragging control by ('cause we snapped somewhere)
            return snapPoint;
        }

        //NOTE NOTE NOTE: If OnMouseMove is called during a resize operation, then cachedDragRect is not guaranteed to work. That is why I introduced RenderSnapLinesInternal(dragRect)
        /// <summary>
        ///  Called by the ResizeBehavior after it has finished drawing
        /// </summary>
        internal void RenderSnapLinesInternal(Rectangle dragRect)
        {
            _cachedDragRect = dragRect;
            RenderSnapLinesInternal();
        }

        /// <summary>
        ///  Called by the DropSourceBehavior after it finished drawing its' draging images so that we can draw our lines on top of everything.
        /// </summary>
        internal void RenderSnapLinesInternal()
        {
            RenderSnapLines(_vertLines, _cachedDragRect);
            RenderSnapLines(_horzLines, _cachedDragRect);
            _recentLines = new Line[_vertLines.Length + _horzLines.Length];
            _vertLines.CopyTo(_recentLines, 0);
            _horzLines.CopyTo(_recentLines, _vertLines.Length);
        }

        /// <summary>
        ///  Clean up all of our references.
        /// </summary>
        internal void OnMouseUp()
        {
            // Here, we store off our recent snapline info to the behavior service - this is used for testing purposes
            if (_behaviorService != null)
            {
                Line[] recent = GetRecentLines();
                string[] lines = new string[recent.Length];
                for (int i = 0; i < recent.Length; i++)
                {
                    lines[i] = recent[i].ToString();
                }
                _behaviorService.RecentSnapLines = lines;
            }
            EraseSnapLines();
            _graphics.Dispose();
            if (_disposeEdgePen && _edgePen != null)
            {
                _edgePen.Dispose();
            }

            if (_baselinePen != null)
            {
                _baselinePen.Dispose();
            }

            if (_backgroundImage != null)
            {
                _backgroundImage.Dispose();
            }
        }

        /// <summary>
        ///  Our 'line' class - used to manage two points and calculate the difference between any two lines.
        /// </summary>
        internal class Line
        {
            public int x1, y1, x2, y2;
            private LineType _lineType;
            private PaddingLineType _paddingLineType;
            private Rectangle _originalBounds;

            public LineType LineType
            {
                get => _lineType;
                set => _lineType = value;
            }

            public Rectangle OriginalBounds
            {
                get => _originalBounds;
                set => _originalBounds = value;
            }

            public PaddingLineType PaddingLineType
            {
                get => _paddingLineType;
                set => _paddingLineType = value;
            }

            public Line(int x1, int y1, int x2, int y2)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
                _lineType = LineType.Standard;
            }

            private Line(int x1, int y1, int x2, int y2, LineType type)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
                _lineType = type;
            }

            public static Line[] GetDiffs(Line l1, Line l2)
            {
                //x's align
                if (l1.x1 == l1.x2 && l1.x1 == l2.x1)
                {
                    return new Line[2] {new Line(l1.x1, Math.Min(l1.y1, l2.y1), l1.x1, Math.Max(l1.y1, l2.y1)),
                                        new Line(l1.x1, Math.Min(l1.y2, l2.y2), l1.x1, Math.Max(l1.y2, l2.y2))};
                }

                //y's align
                if (l1.y1 == l1.y2 && l1.y1 == l2.y1)
                {
                    return new Line[2] {new Line(Math.Min(l1.x1, l2.x1), l1.y1, Math.Max(l1.x1, l2.x1), l1.y1),
                                        new Line(Math.Min(l1.x2, l2.x2), l1.y1, Math.Max(l1.x2, l2.x2), l1.y1)};
                }
                return null;
            }

            public static Line Overlap(Line l1, Line l2)
            {
                // Need to be the same type
                if (l1.LineType != l2.LineType)
                {
                    return null;
                }

                // only makes sense to do this for Standard and Baseline
                if ((l1.LineType != LineType.Standard) && (l1.LineType != LineType.Baseline))
                {
                    return null;
                }

                // 2 overlapping vertical lines
                if ((l1.x1 == l1.x2) && (l2.x1 == l2.x2) && (l1.x1 == l2.x1))
                {
                    return new Line(l1.x1, Math.Min(l1.y1, l2.y1), l1.x2, Math.Max(l1.y2, l2.y2), l1.LineType);
                }

                // 2 overlapping horizontal lines
                if ((l1.y1 == l1.y2) && (l2.y1 == l2.y2) && (l1.y1 == l2.y2))
                {
                    return new Line(Math.Min(l1.x1, l2.x1), l1.y1, Math.Max(l1.x2, l2.x2), l1.y2, l1.LineType);
                }
                return null;
            }

            public override string ToString()
            {
                return "Line, type = " + _lineType + ", dims =(" + x1 + ", " + y1 + ")->(" + x2 + ", " + y2 + ")";
            }
        }

        /// <summary>
        ///  Describes different types of lines (used for margins, etc..)
        /// </summary>
        internal enum LineType
        {
            Standard, Margin, Padding, Baseline
        }

        /// <summary>
        ///  Describes what kind of padding line we have
        /// </summary>
        internal enum PaddingLineType
        {
            None, PaddingRight, PaddingLeft, PaddingTop, PaddingBottom
        }
    }
}
