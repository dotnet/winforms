// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The selection UI service is used to provide a standard user interface for selection across designers. Using this service is optional, but is recommended to provide a standard UI component selection.
    /// </summary>
    internal interface ISelectionUIService
    {
        /// <summary>
        ///  Determines if the selection UI is shown or not.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        ///  Adds an event handler to the ContainerSelectorActive event. This event is fired whenever the user interacts with the container selector in a manor that would indicate that the selector should continued to be displayed. Since the container selector normally will vanish after a timeout, designers should listen to this event and reset the timeout when this event occurs.
        /// </summary>
        event ContainerSelectorActiveEventHandler ContainerSelectorActive;

        /// <summary>
        ///  Assigns a selection UI handler to a given component.  The handler will be called when the UI service needs information about the component.  A single selection UI handler can be assigned to multiple components. When multiple components are dragged, only a single handler may control the drag.  Because of this, only components that are assigned the same handler as the primary selection are included in drag operations. A selection UI handler is automatically unassigned when the component is removed from the container or disposed.
        /// </summary>
        void AssignSelectionUIHandler(object component, ISelectionUIHandler handler);

        void ClearSelectionUIHandler(object component, ISelectionUIHandler handler);

        /// <summary>
        ///  This can be called by an outside party to begin a drag of the currently selected set of components.  At least one designer must have added a UI handler or else this method will always return false.
        /// </summary>
        bool BeginDrag(SelectionRules rules, int initialX, int initialY);

        /// <summary>
        ///  This can be used to determine if the user is in the middle of a drag operation.
        /// </summary>
        bool Dragging { get; }

        /// <summary>
        ///  Called by an outside party to update drag information.  This can only be called after a successful call to beginDrag.
        /// </summary>
        void DragMoved(Rectangle offset);

        /// <summary>
        ///  Called by an outside party to finish a drag operation.  This can only be called after a successful call to beginDrag.
        /// </summary>
        void EndDrag(bool cancel);

        /// <summary>
        ///  Filters the set of selected components.  The selection service will retrieve all components that are currently selected.  This method allows you to filter this set down to components that match your criteria.  The selectionRules parameter must contain one or more flags from the SelectionRules class.  These flags allow you to constrain the set of selected objects to visible, movable, sizeable or all objects.
        /// </summary>
        object[] FilterSelection(object[] components, SelectionRules selectionRules);

        /// <summary>
        ///  Retrieves the width and height of a selection border grab handle. Designers may need this to properly position their user interfaces.
        /// </summary>
        Size GetAdornmentDimensions(AdornmentType adornmentType);

        /// <summary>
        ///  Tests to determine if the given screen coordinate is over an adornment for the specified component. This will only return true if the adornment, and selection UI, is visible.
        /// </summary>
        bool GetAdornmentHitTest(object component, Point pt);

        /// <summary>
        ///  Gets a value indicating whether the specified component is the currently selected container.
        /// </summary>
        bool GetContainerSelected(object component);

        /// <summary>
        ///  Retrieves a set of flags that define rules for the selection.  Selection rules indicate if the given component can be moved or sized, for example.
        /// </summary>
        SelectionRules GetSelectionRules(object component);

        /// <summary>
        ///  Allows you to configure the style of the selection frame that a component uses.  This is useful if your component supports different modes of operation (such as an in-place editing mode and a static design mode).  Where possible, you should leave the selection style as is and use the design-time hit testing feature of the IDesigner interface to provide features at design time.  The value of style must be one of the  SelectionStyle enum values. The selection style is only valid for the duration that the component is selected.
        /// </summary>
        SelectionStyles GetSelectionStyle(object component);

        /// <summary>
        ///  Changes the container selection status of the specified component.
        /// </summary>
        void SetContainerSelected(object component, bool selected);

        /// <summary>
        ///  Allows you to configure the style of the selection frame that a component uses.  This is useful if your component supports different modes of operation (such as an in-place editing mode and a static design mode).  Where possible, you should leave the selection style as is and use the design-time hit testing feature of the IDesigner interface to provide features at design time.  The value of style must be one of the  SelectionStyle enum values. The selection style is only valid for the duration that the component is selected.
        /// </summary>
        void SetSelectionStyle(object component, SelectionStyles style);

        /// <summary>
        ///  This should be called when a component has been moved, sized or re-parented, but the change was not the result of a property change.  All property changes are monitored by the selection UI service, so this is automatic most of the time.  There are times, however, when a component may be moved without a property change notification occurring.  Scrolling an auto scroll Win32 form is an example of this. This method simply re-queries all currently selected components for their bounds and udpates the selection handles for any that have changed.
        /// </summary>
        void SyncSelection();

        /// <summary>
        ///  This should be called when a component's property changed, that the designer thinks should result in a selection UI change. This method simply re-queries all currently selected components for their bounds and udpates the selection handles for any that have changed.
        /// </summary>
        void SyncComponent(object component);
    }
}
