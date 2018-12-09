// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     The ParentControlDesigner class builds on the ControlDesigner.  It adds the ability
    ///     to manipulate child components, and provides a selection UI handler for all
    ///     components it contains.
    /// </summary>
    public class ParentControlDesigner : ControlDesigner, IOleDragClient
    {
        /// <summary>
        ///     This is called after the user selects a toolbox item (that has a ParentControlDesigner
        ///     associated with it) and draws a reversible rectangle on a designer's surface.  If
        ///     this property returns true, it is indicating that the Controls that were lasso'd on the
        ///     designer's surface will be re-parented to this designer's control.
        /// </summary>
        protected virtual bool AllowControlLasso => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     This is called to check whether a generic dragbox should be drawn when dragging a toolbox item
        ///     over the designer's surface.
        /// </summary>
        protected virtual bool AllowGenericDragBox => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     This is called to check whether the z-order of dragged controls should be maintained when dropped on a
        ///     ParentControlDesigner. By default it will, but e.g. FlowLayoutPanelDesigner wants to do its own z-ordering.
        ///     If this returns true, then the DropSourceBehavior will attempt to set the index of the controls being
        ///     dropped to preserve the original order (in the dragSource). If it returns false, the index will not
        ///     be set.
        ///     If this is set to false, then the DropSourceBehavior will not treat a drag as a local drag even
        ///     if the dragSource and the dragTarget are the same. This will allow a ParentControlDesigner to hook
        ///     OnChildControlAdded to set the right child index, since in this case, the control(s) being dragged
        ///     will be removed from the dragSource and then added to the dragTarget.
        /// </summary>
        protected internal virtual bool AllowSetChildIndexOnDrop =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Determines the default location for a control added to this designer.
        ///     it is usualy (0,0), but may be modified if the container has special borders, etc.
        /// </summary>
        protected virtual Point DefaultControlLocation => new Point(0, 0);

        /// <summary>
        ///     Accessor method for the DrawGrid property.  This property determines
        ///     if the grid should be drawn on a control.
        /// </summary>
        protected virtual bool DrawGrid
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Determines whether drag rects can be drawn on this designer.
        /// </summary>
        protected override bool EnableDragRect => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Gets/Sets the GridSize property for a form or user control.
        /// </summary>
        protected Size GridSize
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property is used by deriving classes to determine if the designer is
        ///     in a state where it has a valid MouseDragTool.
        /// </summary>
        [CLSCompliant(false)]
        protected ToolboxItem MouseDragTool => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Returns a list of SnapLine objects representing interesting
        ///     alignment points for this control.  These SnapLines are used
        ///     to assist in the positioning of the control on a parent's
        ///     surface.
        /// </summary>
        public override IList SnapLines => throw new NotImplementedException(SR.NotImplementedByDesign);

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
        ///     This is called when the component is added to the parent container.
        ///     Theoretically it performs the same function as IsDropOK does, but
        ///     unfortunately IsDropOK is not robust enough and does not allow for specific error messages.
        ///     This method is a chance to display the same error as is displayed at runtime.
        /// </summary>
        protected internal virtual bool CanAddComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This property is used by deriving classes to determine if it returns the control being designed or some other
        ///     Container ...
        ///     while adding a component to it.
        ///     e.g: When SplitContainer is selected and a component is being added ... the SplitContainer designer would return a
        ///     SelectedPanel as the ParentControl for all the items being added rather than itself.
        /// </summary>
        protected virtual Control GetParentForComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
        // We need to allocation new ArrayList and pass it to the caller..
        // So its ok to Suppress this.
        protected void AddPaddingSnapLines(ref ArrayList snapLines)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Disposes this component.
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
        ///     Determines if the this designer can parent to the specified desinger --
        ///     generally this means if the control for this designer can parent the
        ///     given ControlDesigner's control.
        /// </summary>
        public virtual bool CanParent(ControlDesigner controlDesigner)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Determines if the this designer can parent to the specified desinger --
        ///     generally this means if the control for this designer can parent the
        ///     given ControlDesigner's control.
        /// </summary>
        public virtual bool CanParent(Control control)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Creates the given tool in the center of the currently selected
        ///     control.  The default size for the tool is used.
        /// </summary>
        [CLSCompliant(false)]
        protected void CreateTool(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Creates the given tool in the currently selected control at the
        ///     given position.  The default size for the tool is used.
        /// </summary>
        [CLSCompliant(false)]
        protected void CreateTool(ToolboxItem tool, Point location)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Creates the given tool in the currently selected control.  The
        ///     tool is created with the provided shape.
        /// </summary>
        [CLSCompliant(false)]
        protected void CreateTool(ToolboxItem tool, Rectangle bounds)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This is the worker method of all CreateTool methods.  It is the only one
        ///     that can be overridden.
        /// </summary>
        [CLSCompliant(false)]
        protected virtual IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height,
            bool hasLocation, bool hasSize)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the control that represents the UI for the given component.
        /// </summary>
        protected Control GetControl(object component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns a 'BodyGlyph' representing the bounds of this control.
        ///     The BodyGlyph is responsible for hit testing the related CtrlDes
        ///     and forwarding messages directly to the designer.
        /// </summary>
        protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Adds our ContainerSelectorGlyph to the selection glyphs.
        /// </summary>
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Updates the given rectangle, adjusting it for grid snaps as
        ///     needed.
        /// </summary>
        protected Rectangle GetUpdatedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Initializes the designer with the given component.  The designer can
        ///     get the component's site and request services from it in this call.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called in order to cleanup a drag and drop operation.  Here we
        ///     cleanup any operations that were performed at the beginning of a drag.
        /// </summary>
        protected override void OnDragComplete(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called in response to a drag drop for OLE drag and drop.  Here we
        ///     drop a toolbox component on our parent control.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        protected override void OnDragDrop(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called in response to a drag enter for OLE drag and drop.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when a drag-drop operation leaves the control designer view
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when a drag drop object is dragged over the control designer view
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
        ///     Called at the end of a drag operation.  This either commits or rolls back the
        ///     drag.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        protected override void OnMouseDragEnd(bool cancel)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called for each movement of the mouse.  This will check to see if a drag operation
        ///     is in progress.  If so, it will pass the updated drag dimensions on to the selection
        ///     UI service.
        /// </summary>
        protected override void OnMouseDragMove(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called after our component has finished painting.  Here we draw our grid surface
        /// </summary>
        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     When the control is scrolled, we want to invalidate areas previously covered by glyphs.
        ///     VSWhidbey# 183588.
        /// </summary>
        private void OnScroll(object sender, ScrollEventArgs se)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called each time the cursor needs to be set.  The ParentControlDesigner behavior here
        ///     will set the cursor to one of three things:
        ///     1.  If the toolbox service has a tool selected, it will allow the toolbox service to
        ///     set the cursor.
        ///     2.  The arrow will be set.  Parent controls allow dragging within their interior.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnSetCursor()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Allows a designer to filter the set of properties
        ///     the component it is designing will expose through the
        ///     TypeDescriptor object.  This method is called
        ///     immediately before its corresponding "Post" method.
        ///     If you are overriding this method you should call
        ///     the base implementation before you perform your own
        ///     filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
