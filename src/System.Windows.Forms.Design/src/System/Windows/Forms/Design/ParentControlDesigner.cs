// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The ParentControlDesigner class builds on the ControlDesigner.  It adds the ability
    ///  to manipulate child components, and provides a selection UI handler for all
    ///  components it contains.
    /// </summary>
    public class ParentControlDesigner : ControlDesigner, IOleDragClient
    {
        private Control pendingRemoveControl; // we've gotten an OnComponentRemoving, and are waiting for OnComponentRemove

        private OleDragDropHandler oleDragDropHandler; // handler for ole drag drop operations
        private IComponentChangeService componentChangeSvc;
        private StatusCommandUI statusCommandUI; // UI for setting the StatusBar Information..

        private int suspendChanging = 0;

        /// <summary>
        ///  This is called after the user selects a toolbox item (that has a ParentControlDesigner
        ///  associated with it) and draws a reversible rectangle on a designer's surface.  If
        ///  this property returns true, it is indicating that the Controls that were lasso'd on the
        ///  designer's surface will be re-parented to this designer's control.
        /// </summary>
        protected virtual bool AllowControlLasso => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  This is called to check whether a generic dragbox should be drawn when dragging a toolbox item
        ///  over the designer's surface.
        /// </summary>
        protected virtual bool AllowGenericDragBox => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  This is called to check whether the z-order of dragged controls should be maintained when dropped on a
        ///  ParentControlDesigner. By default it will, but e.g. FlowLayoutPanelDesigner wants to do its own z-ordering.
        ///  If this returns true, then the DropSourceBehavior will attempt to set the index of the controls being
        ///  dropped to preserve the original order (in the dragSource). If it returns false, the index will not
        ///  be set.
        ///  If this is set to false, then the DropSourceBehavior will not treat a drag as a local drag even
        ///  if the dragSource and the dragTarget are the same. This will allow a ParentControlDesigner to hook
        ///  OnChildControlAdded to set the right child index, since in this case, the control(s) being dragged
        ///  will be removed from the dragSource and then added to the dragTarget.
        /// </summary>
        protected internal virtual bool AllowSetChildIndexOnDrop =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  Determines the default location for a control added to this designer.
        ///  it is usualy (0,0), but may be modified if the container has special borders, etc.
        /// </summary>
        protected virtual Point DefaultControlLocation => new Point(0, 0);

        /// <summary>
        ///  Accessor method for the DrawGrid property.  This property determines
        ///  if the grid should be drawn on a control.
        /// </summary>
        protected virtual bool DrawGrid
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Determines whether drag rects can be drawn on this designer.
        /// </summary>
        protected override bool EnableDragRect => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  Gets/Sets the GridSize property for a form or user control.
        /// </summary>
        protected Size GridSize
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This property is used by deriving classes to determine if the designer is
        ///  in a state where it has a valid MouseDragTool.
        /// </summary>
        [CLSCompliant(false)]
        protected ToolboxItem MouseDragTool => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  Returns a list of SnapLine objects representing interesting
        ///  alignment points for this control.  These SnapLines are used
        ///  to assist in the positioning of the control on a parent's
        ///  surface.
        /// </summary>
        public override IList SnapLines => throw new NotImplementedException(SR.NotImplementedByDesign);

        internal Size ParentGridSize
        {
            get => GridSize;
        }

        internal OleDragDropHandler GetOleDragHandler()
        {
            if (oleDragDropHandler == null)
            {
                oleDragDropHandler = new OleDragDropHandler(null, (IServiceProvider)GetService(typeof(IDesignerHost)), this);
            }
            return oleDragDropHandler;
        }

        internal void AddControl(Control newChild, IDictionary defaultValues)
        {
            Point location = Point.Empty;
            Size size = Size.Empty;
            Size offset = new Size(0, 0);
            bool hasLocation = (defaultValues != null && defaultValues.Contains("Location"));
            bool hasSize = (defaultValues != null && defaultValues.Contains("Size"));

            if (hasLocation)
            {
                location = (Point)defaultValues["Location"];
            }

            if (hasSize)
            {
                size = (Size)defaultValues["Size"];
            }

            if (defaultValues != null && defaultValues.Contains("Offset"))
            {
                offset = (Size)defaultValues["Offset"];
            }

            // If this component doesn't have a control designer, or if this control is top level, then ignore it.  We have the reverse logic in OnComponentAdded in the document designer so that we will add those guys to the tray. Also, if the child-control has already been parented, we assume it's also been located and return immediately. Otherwise, proceed with the parenting and locating.
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null && newChild != null && !Control.Contains(newChild) && (host.GetDesigner(newChild) as ControlDesigner) != null && !(newChild is Form && ((Form)newChild).TopLevel))
            {
                Rectangle bounds = new Rectangle();
                // If we were provided with a location, convert it to parent control coordinates. Otherwise, get the control's size and put the location in the middle of it
                if (hasLocation)
                {
                    location = Control.PointToClient(location);
                    bounds.X = location.X;
                    bounds.Y = location.Y;
                }
                else
                {
                    // is the currently selected control this container?
                    ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
                    object primarySelection = selSvc.PrimarySelection;
                    Control selectedControl = null;
                    if (primarySelection != null)
                    {
                        selectedControl = ((IOleDragClient)this).GetControlForComponent(primarySelection);
                    }

                    // If the resulting control that came back isn't sited, it's not part of the design surface and should not be used as a marker.
                    if (selectedControl != null && selectedControl.Site == null)
                    {
                        selectedControl = null;
                    }

                    // if the currently selected container is this parent control, default to 0,0
                    if (primarySelection == Component || selectedControl == null)
                    {
                        bounds.X = DefaultControlLocation.X;
                        bounds.Y = DefaultControlLocation.Y;
                    }
                    else
                    {
                        // otherwise offset from selected control.
                        bounds.X = selectedControl.Location.X + GridSize.Width;
                        bounds.Y = selectedControl.Location.Y + GridSize.Height;
                    }

                }
                // If we were not given a size, ask the control for its default.  We also update the location here so the control is in the middle of the user's point, rather than at the edge.
                if (hasSize)
                {
                    bounds.Width = size.Width;
                    bounds.Height = size.Height;
                }
                else
                {
                    bounds.Size = GetDefaultSize(newChild);
                }

                // If we were given neither, center the control
                if (!hasSize && !hasLocation)
                {
                    // get the adjusted location, then inflate the rect so we can find a nice spot for this control to live.
                    Rectangle tempBounds = GetAdjustedSnapLocation(Rectangle.Empty, bounds);
                    // compute the stacking location
                    tempBounds = GetControlStackLocation(tempBounds);
                    bounds = tempBounds;
                }
                else
                {
                    // Finally, convert the bounds to the appropriate grid snaps
                    bounds = GetAdjustedSnapLocation(Rectangle.Empty, bounds);
                }

                // Adjust for the offset, if any
                bounds.X += offset.Width;
                bounds.Y += offset.Height;
                // check to see if we have additional information for bounds from the behaviorservice dragdrop logic
                if (defaultValues != null && defaultValues.Contains("ToolboxSnapDragDropEventArgs"))
                {
                    ToolboxSnapDragDropEventArgs e = defaultValues["ToolboxSnapDragDropEventArgs"] as ToolboxSnapDragDropEventArgs;
                    Debug.Assert(e != null, "Why can't we get a ToolboxSnapDragDropEventArgs object out of our default values?");
                    Rectangle snappedBounds = DesignerUtils.GetBoundsFromToolboxSnapDragDropInfo(e, bounds, Control.IsMirrored);
                    //Make sure the snapped bounds intersects with the bounds of the root control before we go adjusting the drag offset.  A race condition exists where the user can drag a tbx item so fast that the adorner window will never receive the proper drag/mouse move messages and never properly adjust the snap drag info.  This cause the control to be added @ 0,0 w.r.t. the adorner window.
                    if (host.RootComponent is Control rootControl && snappedBounds.IntersectsWith(rootControl.ClientRectangle))
                    {
                        bounds = snappedBounds;
                    }
                }
                // Parent the control to the designer and set it to the front.
                PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(Control)["Controls"];
                if (componentChangeSvc != null)
                {
                    componentChangeSvc.OnComponentChanging(Control, controlsProp);
                }
                AddChildControl(newChild);
                // Now see if the control has size and location properties.  Update these values if it does.
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(newChild);
                if (props != null)
                {
                    PropertyDescriptor prop = props["Size"];
                    if (prop != null)
                    {
                        prop.SetValue(newChild, new Size(bounds.Width, bounds.Height));
                    }

                    // ControlDesigner shadows the Location property. If the control is parented and the parent is a scrollable control, then it expects the Location to be in displayrectangle coordinates. At this point bounds are in clientrectangle coordinates, so we need to check if we need to adjust the coordinates. The reason this worked in Everett was that the AddChildControl was done AFTER this. The AddChildControl was moved above a while back. Not sure what will break if AddChildControl is moved down below, so let's just fix up things here.
                    Point pt = new Point(bounds.X, bounds.Y);
                    if (newChild.Parent is ScrollableControl p)
                    {
                        Point ptScroll = p.AutoScrollPosition;
                        pt.Offset(-ptScroll.X, -ptScroll.Y); //always want to add the control below/right of the AutoScrollPosition
                    }

                    prop = props["Location"];
                    if (prop != null)
                    {
                        prop.SetValue(newChild, pt);
                    }
                }

                if (componentChangeSvc != null)
                {
                    componentChangeSvc.OnComponentChanged(Control, controlsProp, Control.Controls, Control.Controls);
                }
                newChild.Update();
            }
        }

        internal virtual void AddChildControl(Control newChild)
        {
            if (newChild.Left == 0 && newChild.Top == 0 && newChild.Width >= Control.Width && newChild.Height >= Control.Height)
            {
                // bump the control down one gridsize just so it's selectable...
                Point loc = newChild.Location;
                loc.Offset(GridSize.Width, GridSize.Height);
                newChild.Location = loc;
            }
            Control.Controls.Add(newChild);
            Control.Controls.SetChildIndex(newChild, 0);
        }

        private Rectangle GetControlStackLocation(Rectangle centeredLocation)
        {
            Control parent = Control;
            int parentHeight = parent.ClientSize.Height;
            int parentWidth = parent.ClientSize.Width;
            if (centeredLocation.Bottom >= parentHeight || centeredLocation.Right >= parentWidth)
            {
                centeredLocation.X = DefaultControlLocation.X;
                centeredLocation.Y = DefaultControlLocation.Y;
            }
            return centeredLocation;
        }

        private Size GetDefaultSize(IComponent component)
        {
            //Check to see if the control is AutoSized. VSWhidbey #416721
            PropertyDescriptor prop = TypeDescriptor.GetProperties(component)["AutoSize"];
            Size size;
            if (prop != null &&
                !(prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden) ||
                  prop.Attributes.Contains(BrowsableAttribute.No)))
            {
                bool autoSize = (bool)prop.GetValue(component);
                if (autoSize)
                {
                    prop = TypeDescriptor.GetProperties(component)["PreferredSize"];
                    if (prop != null)
                    {
                        size = (Size)prop.GetValue(component);
                        if (size != Size.Empty)
                        {
                            return size;
                        }
                    }
                }
            }

            // attempt to get the size property of our component
            prop = TypeDescriptor.GetProperties(component)["Size"];
            if (prop != null)
            {
                // first, let's see if we can get a valid size...
                size = (Size)prop.GetValue(component);
                // ...if not, we'll see if there's a default size attribute...
                if (size.Width <= 0 || size.Height <= 0)
                {
                    DefaultValueAttribute sizeAttr = (DefaultValueAttribute)prop.Attributes[typeof(DefaultValueAttribute)];
                    if (sizeAttr != null)
                    {
                        return ((Size)sizeAttr.Value);
                    }
                }
                else
                {
                    return size;
                }
            }
            // Couldn't get the size or a def size attrib, returning 75,23...
            return (new Size(75, 23));
        }

        private Rectangle GetAdjustedSnapLocation(Rectangle originalRect, Rectangle dragRect)
        {
            Rectangle adjustedRect = GetUpdatedRect(originalRect, dragRect, true);
            //now, preserve the width and height that was originally passed in
            adjustedRect.Width = dragRect.Width;
            adjustedRect.Height = dragRect.Height;
            // we need to keep in mind that if we adjust to the snap, that we could have possibly moved the control's position outside of the display rect. ex: groupbox's display rect.x = 3, but we might snap to 0. so we need to check with the control's designer to make sure this doesn't happen
            Point minimumLocation = DefaultControlLocation;
            if (adjustedRect.X < minimumLocation.X)
            {
                adjustedRect.X = minimumLocation.X;
            }
            if (adjustedRect.Y < minimumLocation.Y)
            {
                adjustedRect.Y = minimumLocation.Y;
            }
            // here's our rect that has been snapped to grid
            return adjustedRect;
        }

        bool IOleDragClient.CanModifyComponents => throw new NotImplementedException();

        bool IOleDragClient.AddComponent(IComponent component, string name, bool firstAdd)
        {
            throw new NotImplementedException();
        }

        bool IOleDragClient.IsDropOk(IComponent component)
        {
            throw new NotImplementedException();
        }

        Control IOleDragClient.GetDesignerControl()
        {
            throw new NotImplementedException();
        }

        Control IOleDragClient.GetControlForComponent(object component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  This is called when the component is added to the parent container.
        ///  Theoretically it performs the same function as IsDropOK does, but
        ///  unfortunately IsDropOK is not robust enough and does not allow for specific error messages.
        ///  This method is a chance to display the same error as is displayed at runtime.
        /// </summary>
        protected internal virtual bool CanAddComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This property is used by deriving classes to determine if it returns the control being designed or some other
        ///  Container ...
        ///  while adding a component to it.
        ///  e.g: When SplitContainer is selected and a component is being added ... the SplitContainer designer would return a
        ///  SelectedPanel as the ParentControl for all the items being added rather than itself.
        /// </summary>
        protected virtual Control GetParentForComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        // We need to allocation new ArrayList and pass it to the caller..
        // So its ok to Suppress this.
        protected void AddPaddingSnapLines(ref ArrayList snapLines)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Disposes this component.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [CLSCompliant(false)]
        protected static void InvokeCreateTool(ParentControlDesigner toInvoke, ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Determines if the this designer can parent to the specified desinger --
        ///  generally this means if the control for this designer can parent the
        ///  given ControlDesigner's control.
        /// </summary>
        public virtual bool CanParent(ControlDesigner controlDesigner)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Determines if the this designer can parent to the specified desinger --
        ///  generally this means if the control for this designer can parent the
        ///  given ControlDesigner's control.
        /// </summary>
        public virtual bool CanParent(Control control)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Creates the given tool in the center of the currently selected
        ///  control.  The default size for the tool is used.
        /// </summary>
        [CLSCompliant(false)]
        protected void CreateTool(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Creates the given tool in the currently selected control at the
        ///  given position.  The default size for the tool is used.
        /// </summary>
        [CLSCompliant(false)]
        protected void CreateTool(ToolboxItem tool, Point location)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Creates the given tool in the currently selected control.  The
        ///  tool is created with the provided shape.
        /// </summary>
        [CLSCompliant(false)]
        protected void CreateTool(ToolboxItem tool, Rectangle bounds)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This is the worker method of all CreateTool methods.  It is the only one
        ///  that can be overridden.
        /// </summary>
        [CLSCompliant(false)]
        protected virtual IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height,
            bool hasLocation, bool hasSize)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Returns the control that represents the UI for the given component.
        /// </summary>
        protected Control GetControl(object component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Returns a 'BodyGlyph' representing the bounds of this control.
        ///  The BodyGlyph is responsible for hit testing the related CtrlDes
        ///  and forwarding messages directly to the designer.
        /// </summary>
        protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Adds our ContainerSelectorGlyph to the selection glyphs.
        /// </summary>
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        internal Point GetSnappedPoint(Point pt)
        {
            Rectangle r = GetUpdatedRect(Rectangle.Empty, new Rectangle(pt.X, pt.Y, 0, 0), false);
            return new Point(r.X, r.Y);
        }

        /// <summary>
        ///  Updates the given rectangle, adjusting it for grid snaps as
        ///  needed.
        /// </summary>
        protected Rectangle GetUpdatedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Initializes the designer with the given component.  The designer can
        ///  get the component's site and request services from it in this call.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            if (Control is ScrollableControl)
            {
                ((ScrollableControl)Control).Scroll += new ScrollEventHandler(OnScroll);
            }
            EnableDragDrop(true);
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null)
            {
                componentChangeSvc = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
            }
            // update the Status Command
            statusCommandUI = new StatusCommandUI(component.Site);
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component == pendingRemoveControl)
            {
                pendingRemoveControl = null;
                componentChangeSvc.OnComponentChanged(Control, TypeDescriptor.GetProperties(Control)["Controls"], null, null);
            }
        }

        private void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
            if (e.Component is Control comp && comp.Parent != null && comp.Parent == Control)
            {
                pendingRemoveControl = (Control)comp;
                //We suspend Component Changing Events for bulk operations to avoid unnecessary serialization\deserialization for undo
                if (suspendChanging == 0)
                {
                    componentChangeSvc.OnComponentChanging(Control, TypeDescriptor.GetProperties(Control)["Controls"]);
                }
            }
        }

        internal void SuspendChangingEvents()
        {
            suspendChanging++;
            Debug.Assert(suspendChanging > 0, "Unbalanced SuspendChangingEvents\\ResumeChangingEvents");
        }

        internal void ForceComponentChanging()
        {
            componentChangeSvc.OnComponentChanging(Control, TypeDescriptor.GetProperties(Control)["Controls"]);
        }

        internal void ResumeChangingEvents()
        {
            suspendChanging--;
            Debug.Assert(suspendChanging >= 0, "Unbalanced SuspendChangingEvents\\ResumeChangingEvents");
        }

        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called in order to cleanup a drag and drop operation.  Here we
        ///  cleanup any operations that were performed at the beginning of a drag.
        /// </summary>
        protected override void OnDragComplete(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called in response to a drag drop for OLE drag and drop.  Here we
        ///  drop a toolbox component on our parent control.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        protected override void OnDragDrop(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called in response to a drag enter for OLE drag and drop.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called when a drag-drop operation leaves the control designer view
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called when a drag drop object is dragged over the control designer view
        /// </summary>
        protected override void OnDragOver(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected override void OnMouseDragBegin(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called at the end of a drag operation.  This either commits or rolls back the
        ///  drag.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        protected override void OnMouseDragEnd(bool cancel)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called for each movement of the mouse.  This will check to see if a drag operation
        ///  is in progress.  If so, it will pass the updated drag dimensions on to the selection
        ///  UI service.
        /// </summary>
        protected override void OnMouseDragMove(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called after our component has finished painting.  Here we draw our grid surface
        /// </summary>
        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When the control is scrolled, we want to invalidate areas previously covered by glyphs.
        /// </summary>
        private void OnScroll(object sender, ScrollEventArgs se)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called each time the cursor needs to be set.  The ParentControlDesigner behavior here
        ///  will set the cursor to one of three things:
        ///  1.  If the toolbox service has a tool selected, it will allow the toolbox service to
        ///  set the cursor.
        ///  2.  The arrow will be set.  Parent controls allow dragging within their interior.
        /// </summary>
        protected override void OnSetCursor()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties
        ///  the component it is designing will expose through the
        ///  TypeDescriptor object.  This method is called
        ///  immediately before its corresponding "Post" method.
        ///  If you are overriding this method you should call
        ///  the base implementation before you perform your own
        ///  filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
