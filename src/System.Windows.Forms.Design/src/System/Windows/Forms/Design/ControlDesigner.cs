// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     <para>
    ///         Provides a designer that can design components
    ///         that extend Control.
    ///     </para>
    /// </summary>
    public class ControlDesigner : ComponentDesigner
    {
        protected AccessibleObject accessibilityObj = null;

        protected BehaviorService BehaviorService => throw new NotImplementedException(SR.NotImplementedByDesign);

        internal bool ForceVisible
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Retrieves a list of associated components.  These are components that should be incluced in a cut or copy
        ///         operation on this component.
        ///     </para>
        /// </summary>
        public override ICollection AssociatedComponents =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        public virtual AccessibleObject AccessibilityObject =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Retrieves the control we're designing.
        /// </summary>
        public virtual Control Control => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Determines whether drag rects can be drawn on this designer.
        /// </summary>
        protected virtual bool EnableDragRect => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Returns the parent component for this control designer.
        ///     The default implementation just checks to see if the
        ///     component being designed is a control, and if it is it
        ///     returns its parent.  This property can return null if there
        ///     is no parent component.
        /// </summary>
        protected override IComponent ParentComponent => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Determines whether or not the ControlDesigner will allow SnapLine alignment during a
        ///     drag operation when the primary drag control is over this designer, or when a control
        ///     is being dragged from the toolbox, or when a control is being drawn through click-drag.
        /// </summary>
        public virtual bool ParticipatesWithSnapLines => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        /// </summary>
        public bool AutoResizeHandles
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Retrieves a set of rules concerning the movement capabilities of a component.
        ///     This should be one or more flags from the SelectionRules class.  If no designer
        ///     provides rules for a component, the component will not get any UI services.
        /// </summary>
        public virtual SelectionRules SelectionRules => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Returns a list of SnapLine objects representing interesting
        ///     alignment points for this control.  These SnapLines are used
        ///     to assist in the positioning of the control on a parent's
        ///     surface.
        /// </summary>
        public virtual IList SnapLines => throw new NotImplementedException(SR.NotImplementedByDesign);

        protected override InheritanceAttribute InheritanceAttribute =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Returns the number of internal control designers in the ControlDesigner. An internal control
        ///     is a control that is not in the IDesignerHost.Container.Components collection.
        ///     SplitterPanel is an example of one such control. We use this to get SnapLines for the internal
        ///     control designers.
        /// </summary>
        public virtual int NumberOfInternalControlDesigners()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the internal control designer with the specified index in the ControlDesigner. An internal control
        ///     is a control that is not in the IDesignerHost.Container.Components collection.
        ///     SplitterPanel is an example of one such control.
        ///     internalControlIndex is zero-based.
        /// </summary>
        public virtual ControlDesigner InternalControlDesigner(int internalControlIndex)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Default processing for messages.  This method causes the message to
        ///     get processed by windows, skipping the control.  This is useful if
        ///     you want to block this message from getting to the control, but
        ///     you do not want to block it from getting to Windows itself because
        ///     it causes other messages to be generated.
        /// </summary>
        protected void BaseWndProc(ref Message m)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Determines if the this designer can be parented to the specified desinger --
        ///     generally this means if the control for this designer can be parented into the
        ///     given ParentControlDesigner's designer.
        /// </summary>
        public virtual bool CanBeParentedTo(IDesigner parentDesigner)
        {
            ParentControlDesigner p = parentDesigner as ParentControlDesigner;
            return p != null && !Control.Contains(p.Control);
        }

        /// <summary>
        ///     Default processing for messages.  This method causes the message to
        ///     get processed by the control, rather than the designer.
        /// </summary>
        protected void DefWndProc(ref Message m)
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

        /// <summary>
        ///     Disposes of this object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Enables design time functionality for a child control.  The child control is a child
        ///     of this control designer's control.  The child does not directly participate in
        ///     persistence, but it will if it is exposed as a property of the main control.  Consider
        ///     a control like the SplitContainer:  it has two panels, Panel1 and Panel2.  These panels
        ///     are exposed through read only Panel1 and Panel2 properties on the SplitContainer class.
        ///     SplitContainer's designer calls EnableDesignTime for each panel, which allows other
        ///     components to be dropped on them.  But, in order for the contents of Panel1 and Panel2
        ///     to be saved, SplitContainer itself needed to expose the panels as public properties.
        ///     The child paramter is the control to enable.  The name paramter is the name of this
        ///     control as exposed to the end user.  Names need to be unique within a control designer,
        ///     but do not have to be unique to other control designer's children.
        ///     This method returns true if the child control could be enabled for design time, or
        ///     false if the hosting infrastructure does not support it.  To support this feature, the
        ///     hosting infrastructure must expose the INestedContainer class as a service off of the site.
        /// </summary>
        protected bool EnableDesignMode(Control child, string name)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Enables or disables drag/drop support.  This
        ///     hooks drag event handlers to the control.
        /// </summary>
        protected void EnableDragDrop(bool value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns a 'BodyGlyph' representing the bounds of this control.
        ///     The BodyGlyph is responsible for hit testing the related CtrlDes
        ///     and forwarding messages directly to the designer.
        /// </summary>
        protected virtual ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns a collection of Glyph objects representing the selection
        ///     borders and grab handles for a standard control.  Note that
        ///     based on 'selectionType' the Glyphs returned will either: represent
        ///     a fully resizeable selection border with grab handles, a locked
        ///     selection border, or a single 'hidden' selection Glyph.
        /// </summary>
        public virtual GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Allows your component to support a design time user interface.  A TabStrip
        ///     control, for example, has a design time user interface that allows the user
        ///     to click the tabs to change tabs.  To implement this, TabStrip returns
        ///     true whenever the given point is within its tabs.
        /// </summary>
        protected virtual bool GetHitTest(Point point)
        {
            return false;
        }

        /// <summary>
        ///     Hooks the children of the given control.  We need to do this for
        ///     child controls that are not in design mode, which is the case
        ///     for composite controls.
        /// </summary>
        protected void HookChildControls(Control firstChild)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called by the host when we're first initialized.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     ControlDesigner overrides this method to handle after-drop cases.
        /// </summary>
        public override void InitializeExistingComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     ControlDesigner overrides this method.  It will look at the default property for the control and,
        ///     if it is of type string, it will set this property's value to the name of the component.  It only
        ///     does this if the designer has been configured with this option in the options service.  This method
        ///     also connects the control to its parent and positions it.  If you override this method, you should
        ///     always call base.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when the designer is intialized.  This allows the designer to provide some
        ///     meaningful default values in the component.  The default implementation of this
        ///     sets the components's default property to it's name, if that property is a string.
        /// </summary>
        [Obsolete(
            "This method has been deprecated. Use InitializeNewComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public override void OnSetComponentDefaults()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when the context menu should be displayed
        /// </summary>
        protected virtual void OnContextMenu(int x, int y)
        {
            ShowContextMenu(x, y);
        }

        /// <summary>
        ///     This is called immediately after the control handle has been created.
        /// </summary>
        protected virtual void OnCreateHandle()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when a drag-drop operation enters the control designer view
        /// </summary>
        protected virtual void OnDragEnter(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called to cleanup a D&D operation
        /// </summary>
        protected virtual void OnDragComplete(DragEventArgs de)
        {
            // default implementation - does nothing.
        }

        /// <summary>
        ///     Called when a drag drop object is dropped onto the control designer view
        /// </summary>
        protected virtual void OnDragDrop(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when a drag-drop operation leaves the control designer view
        /// </summary>
        protected virtual void OnDragLeave(EventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when a drag drop object is dragged over the control designer view
        /// </summary>
        protected virtual void OnDragOver(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Event handler for our GiveFeedback event, which is called when a drag operation
        ///     is in progress.  The host will call us with
        ///     this when an OLE drag event happens.
        /// </summary>
        protected virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called in response to the left mouse button being pressed on a
        ///     component. It ensures that the component is selected.
        /// </summary>
        protected virtual void OnMouseDragBegin(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called at the end of a drag operation.  This either commits or rolls back the
        ///     drag.
        /// </summary>
        protected virtual void OnMouseDragEnd(bool cancel)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called for each movement of the mouse.  This will check to see if a drag operation
        ///     is in progress.  If so, it will pass the updated drag dimensions on to the selection
        ///     UI service.
        /// </summary>
        protected virtual void OnMouseDragMove(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when the mouse first enters the control. This is forwarded to the parent
        ///     designer to enable the container selector.
        /// </summary>
        protected virtual void OnMouseEnter()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called after the mouse hovers over the control. This is forwarded to the parent
        ///     designer to enabled the container selector.
        /// </summary>
        protected virtual void OnMouseHover()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when the mouse first enters the control. This is forwarded to the parent
        ///     designer to enable the container selector.
        /// </summary>
        protected virtual void OnMouseLeave()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called when the control we're designing has finished painting.  This method
        ///     gives the designer a chance to paint any additional adornments on top of the
        ///     control.
        /// </summary>
        protected virtual void OnPaintAdornments(PaintEventArgs pe)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Called each time the cursor needs to be set.  The ControlDesigner behavior here
        ///     will set the cursor to one of three things:
        ///     1.  If the toolbox service has a tool selected, it will allow the toolbox service to
        ///     set the cursor.
        ///     2.  If the selection UI service shows a locked selection, or if there is no location
        ///     property on the control, then the default arrow will be set.
        ///     3.  Otherwise, the four headed arrow will be set to indicate that the component can
        ///     be clicked and moved.
        ///     4.  If the user is currently dragging a component, the crosshair cursor will be used
        ///     instead of the four headed arrow.
        /// </summary>
        protected virtual void OnSetCursor()
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

        /// <summary>
        ///     Hooks the children of the given control.  We need to do this for
        ///     child controls that are not in design mode, which is the case
        ///     for composite controls.
        /// </summary>
        protected void UnhookChildControls(Control firstChild)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This method should be called by the extending designer for each message
        ///     the control would normally receive.  This allows the designer to pre-process
        ///     messages before allowing them to be routed to the control.
        /// </summary>
        protected virtual void WndProc(ref Message m)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        [ComVisible(true)]
        public class ControlDesignerAccessibleObject : AccessibleObject
        {
            public ControlDesignerAccessibleObject(ControlDesigner designer, Control control)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public override Rectangle Bounds => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override string Description => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override string DefaultAction => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override string Name => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override AccessibleObject Parent => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override AccessibleRole Role => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override AccessibleStates State => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override string Value => throw new NotImplementedException(SR.NotImplementedByDesign);

            public override AccessibleObject GetChild(int index)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public override int GetChildCount()
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public override AccessibleObject GetFocused()
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public override AccessibleObject GetSelected()
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
        }

        /// <summary>
        ///     This TransparentBehavior is associated with the BodyGlyph for
        ///     this ControlDesigner.  When the BehaviorService hittests a glyph
        ///     w/a TransparentBehavior, all messages will be passed through
        ///     the BehaviorService directly to the ControlDesigner.
        ///     During a Drag operation, when the BehaviorService hittests
        /// </summary>
        internal class TransparentBehavior : Behavior.Behavior
        {
            /// <summary>
            ///     Constructor that accepts the related ControlDesigner.
            /// </summary>
            internal TransparentBehavior(ControlDesigner designer)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///     This property performs a hit test on the ControlDesigner
            ///     to determine if the BodyGlyph should return '-1' for
            ///     hit testing (letting all messages pass directly to the
            ///     the control).
            /// </summary>
            internal bool IsTransparent(Point p)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///     Forwards DragDrop notification from the BehaviorService to
            ///     the related ControlDesigner.
            /// </summary>
            public override void OnDragDrop(Glyph g, DragEventArgs e)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///     Forwards DragDrop notification from the BehaviorService to
            ///     the related ControlDesigner.
            /// </summary>
            public override void OnDragEnter(Glyph g, DragEventArgs e)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///     Forwards DragDrop notification from the BehaviorService to
            ///     the related ControlDesigner.
            /// </summary>
            public override void OnDragLeave(Glyph g, EventArgs e)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///     Forwards DragDrop notification from the BehaviorService to
            ///     the related ControlDesigner.
            /// </summary>
            public override void OnDragOver(Glyph g, DragEventArgs e)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }

            /// <summary>
            ///     Forwards DragDrop notification from the BehaviorService to
            ///     the related ControlDesigner.
            /// </summary>
            public override void OnGiveFeedback(Glyph g, GiveFeedbackEventArgs e)
            {
                throw new NotImplementedException(SR.NotImplementedByDesign);
            }
        }
    }
}
