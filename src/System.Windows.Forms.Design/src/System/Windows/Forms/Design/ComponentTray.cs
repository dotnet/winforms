// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     <para>
    ///         Provides the component tray UI for the form designer.
    ///     </para>
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [ProvideProperty("Location", typeof(IComponent))]
    [ProvideProperty("TrayLocation", typeof(IComponent))]
    public class ComponentTray : ScrollableControl, IExtenderProvider, ISelectionUIHandler, IOleDragClient
    {
        // Empty class for build time dependancy

        /// <summary>
        ///     Creates a new component tray.  The component tray
        ///     will monitor component additions and removals and create
        ///     appropriate UI objects in its space.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
        public ComponentTray(IDesigner mainDesigner, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public bool AutoArrange
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Gets the number of compnents contained within this tray.
        ///     </para>
        /// </summary>
        public int ComponentCount => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Determines whether the tray will show large icon view or not.
        /// </summary>
        public bool ShowLargeIcons
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        bool IExtenderProvider.CanExtend(object extendee)
        {
            throw new NotImplementedException();
        }

        IComponent IOleDragClient.Component => throw new NotImplementedException();

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

        bool ISelectionUIHandler.BeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.DragMoved(object[] components, Rectangle offset)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.EndDrag(object[] components, bool cancel)
        {
            throw new NotImplementedException();
        }

        Rectangle ISelectionUIHandler.GetComponentBounds(object component)
        {
            throw new NotImplementedException();
        }

        SelectionRules ISelectionUIHandler.GetComponentRules(object component)
        {
            throw new NotImplementedException();
        }

        Rectangle ISelectionUIHandler.GetSelectionClipRect(object component)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.OnSelectionDoubleClick(IComponent component)
        {
            throw new NotImplementedException();
        }

        bool ISelectionUIHandler.QueryBeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.ShowContextMenu(IComponent component)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.OleDragEnter(DragEventArgs de)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.OleDragDrop(DragEventArgs de)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.OleDragOver(DragEventArgs de)
        {
            throw new NotImplementedException();
        }

        void ISelectionUIHandler.OleDragLeave()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     <para>Adds a component to the tray.</para>
        /// </summary>
        public virtual void AddComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [CLSCompliant(false)]
        protected virtual bool CanCreateComponentFromTool(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method determines if a UI representation for the given component should be provided.
        ///     If it returns true, then the component will get a glyph in the tray area.  If it returns
        ///     false, then the component will not actually be added to the tray.  The default
        ///     implementation looks for DesignTimeVisibleAttribute.Yes on the component's class.
        /// </summary>
        protected virtual bool CanDisplayComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [CLSCompliant(false)]
        public void CreateComponentFromTool(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Displays the given exception to the user.
        /// </summary>
        protected void DisplayError(Exception e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        //

        /// <summary>
        ///     <para>
        ///         Disposes of the resources (other than memory) used by the component tray object.
        ///     </para>
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Similar to GetNextControl on Control, this method returns the next
        ///     component in the tray, given a starting component.  It will return
        ///     null if the end (or beginning, if forward is false) of the list
        ///     is encountered.
        /// </summary>
        public IComponent GetNextComponent(IComponent component, bool forward)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Accessor method for the location extender property.  We offer this extender
        ///     to all non-visual components.
        /// </summary>
        [Category("Layout")]
        [Localizable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription("ControlLocationDescr")]
        [DesignOnly(true)]
        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
        public Point GetLocation(IComponent receiver)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Accessor method for the location extender property.  We offer this extender
        ///     to all non-visual components.
        /// </summary>
        [Category("Layout")]
        [Localizable(false)]
        [Browsable(false)]
        [SRDescription("ControlLocationDescr")]
        [DesignOnly(true)]
        public Point GetTrayLocation(IComponent receiver)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Gets the requsted service type.
        ///     </para>
        /// </summary>
        protected override object GetService(Type serviceType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns true if the given componenent is being shown on the tray.
        /// </summary>
        public bool IsTrayComponent(IComponent comp)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onGiveFeedback to send this event to any registered event listeners.
        /// </summary>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs gfevent)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called in response to a drag drop for OLE drag and drop.  Here we
        ///     drop a toolbox component on our parent control.
        /// </summary>
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

        /// <internalonly />
        /// <summary>
        ///     Forces the layout of any docked or anchored child controls.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This is called when we lose capture.  Here we get rid of any
        ///     rubber band we were drawing.  You should put any cleanup
        ///     code in here.
        /// </summary>
        protected virtual void OnLostCapture()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onMouseDown to send this event to any registered event listeners.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onMouseMove to send this event to any registered event listeners.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onMouseUp to send this event to any registered event listeners.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Sets the cursor.  You may override this to set your own
        ///     cursor.
        /// </summary>
        protected virtual void OnSetCursor()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Removes a component from the tray.
        /// </summary>
        public virtual void RemoveComponent(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Accessor method for the location extender property.  We offer this extender
        ///     to all non-visual components.
        /// </summary>
        public void SetLocation(IComponent receiver, Point location)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Accessor method for the location extender property.  We offer this extender
        ///     to all non-visual components.
        /// </summary>
        public void SetTrayLocation(IComponent receiver, Point location)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     We override our base class's WndProc to monitor certain messages.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
