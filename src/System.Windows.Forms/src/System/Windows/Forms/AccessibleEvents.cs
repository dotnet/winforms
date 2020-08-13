// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies events that are reported by accessible applications.
    /// </summary>
    public enum AccessibleEvents
    {
        /// <summary>
        /// <c>EVENT_SYSTEM_SOUND</c>
        ///  Sent when a sound is played. Currently nothing is generating this, we
        ///  are going to be cleaning up the SOUNDSENTRY feature in the control panel
        ///  and will use this at that time. Applications implementing WinEvents
        ///  are perfectly welcome to use it. Clients of IAccessible* will simply
        ///  turn around and get back a non-visual object that describes the sound.
        /// </summary>
        SystemSound = 0x0001,

        /// <summary>
        /// <c>EVENT_SYSTEM_ALERT</c>
        ///  Sent when an alert needs to be given to the user. MessageBoxes generate
        ///  alerts for example.
        /// </summary>
        SystemAlert = 0x0002,

        /// <summary>
        /// <c>EVENT_SYSTEM_FOREGROUND</c>
        ///  Sent when the foreground (active) window changes, even if it is changing
        ///  to another window in the same thread as the previous one.
        /// </summary>
        SystemForeground = 0x0003,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUSTART</c>
        ///  Sent when entering into menu mode (system, app bar, and track popups).
        /// </summary>
        /// <seealso cref="SystemMenuEnd"/>.
        SystemMenuStart = 0x0004,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUEND</c>
        ///  Sent when leaving from menu mode (system, app bar, and track popups).
        /// </summary>
        /// <seealso cref="SystemMenuStart"/>.
        SystemMenuEnd = 0x0005,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUPOPUPSTART</c>
        ///  Sent when a menu popup comes up.
        /// </summary>
        /// <remarks>
        ///  Note that for a call to TrackPopupMenu(), a client will see <c>EVENT_SYSTEM_MENUSTART</c>
        ///  followed almost immediately by <c>EVENT_SYSTEM_MENUPOPUPSTART</c> for the popup being shown.
        /// </remarks>
        /// <seealso cref="SystemMenuPopupEnd"/>.
        SystemMenuPopupStart = 0x0006,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUPOPUPEND</c>
        ///  Sent when a menu popup just before it is taken down.
        /// </summary>
        /// <remarks>
        ///  Note that for a call to TrackPopupMenu(), a client will see <c>EVENT_SYSTEM_MENUSTART</c>
        ///  followed almost immediately by <c>EVENT_SYSTEM_MENUPOPUPSTART</c> for the popup being shown.
        /// </remarks>
        /// <seealso cref="SystemMenuPopupStart"/>.
        SystemMenuPopupEnd = 0x0007,

        /// <summary>
        /// <c>EVENT_SYSTEM_CAPTURESTART</c>
        ///  Sent when a window takes the capture.
        /// </summary>
        /// <seealso cref="SystemCaptureEnd"/>.
        SystemCaptureStart = 0x0008,

        /// <summary>
        /// <c>EVENT_SYSTEM_CAPTUREEND</c>
        ///  Sent when a window releases the capture.
        /// </summary>
        /// <seealso cref="SystemCaptureStart"/>.
        SystemCaptureEnd = 0x0009,

        /// <summary>
        /// <c>EVENT_SYSTEM_MOVESIZESTART</c>
        ///  Sent when a window enters move-size dragging mode.
        /// </summary>
        /// <seealso cref="SystemMoveSizeEnd"/>.
        SystemMoveSizeStart = 0x000A,

        /// <summary>
        /// <c>EVENT_SYSTEM_MOVESIZEEND</c>
        ///  Sent when a window leaves move-size dragging mode.
        /// </summary>
        /// <seealso cref="SystemMoveSizeStart"/>.
        SystemMoveSizeEnd = 0x000B,

        /// <summary>
        /// <c>EVENT_SYSTEM_CONTEXTHELPSTART</c>
        ///  Sent when a window enters context sensitive help mode.
        /// </summary>
        /// <seealso cref="SystemContextHelpEnd"/>.
        SystemContextHelpStart = 0x000C,

        /// <summary>
        ///  <c>EVENT_SYSTEM_CONTEXTHELPEND</c>
        ///  Sent when a window leaves context sensitive help mode.
        /// </summary>
        /// <seealso cref="SystemContextHelpStart"/>.
        SystemContextHelpEnd = 0x000D,

        /// <summary>
        ///  <c>EVENT_SYSTEM_DRAGDROPSTART</c>
        ///  Sent when a window enters drag drop mode.
        /// </summary>
        /// <seealso cref="SystemDragDropEnd"/>.
        //  Note that it is up to apps and OLE to generate this, since the system doesn't know.
        //  Like <c>EVENT_SYSTEM_SOUND</c>, it will be a while before this is prevalent.
        SystemDragDropStart = 0x000E,

        /// <summary>
        ///  <c>EVENT_SYSTEM_DRAGDROPEND</c>
        ///  Sent when a window leaves drag drop mode.
        /// </summary>
        /// <seealso cref="SystemDragDropStart"/>.
        //  Note that it is up to apps and OLE to generate this, since the system doesn't know.
        //  Like <c>EVENT_SYSTEM_SOUND</c>, it will be a while before this is prevalent.
        SystemDragDropEnd = 0x000F,

        /// <summary>
        ///  <c>EVENT_SYSTEM_DIALOGSTART</c>
        ///  Sent when a dialog comes up.
        /// </summary>
        /// <seealso cref="SystemDialogEnd"/>.
        SystemDialogStart = 0x0010,

        /// <summary>
        ///  <c>EVENT_SYSTEM_DIALOGEND</c>
        ///  Sent when a dialog just before it goes away.
        /// </summary>
        /// <seealso cref="SystemDialogStart"/>.
        SystemDialogEnd = 0x0011,

        /// <summary>
        ///  <c>EVENT_SYSTEM_SCROLLINGSTART</c>
        ///  Sent when beginning the tracking of a scrollbar in a window, and also for scrollbar controls.
        /// </summary>
        /// <seealso cref="SystemScrollingEnd"/>.
        SystemScrollingStart = 0x0012,

        /// <summary>
        ///  <c>EVENT_SYSTEM_SCROLLINGEND</c>
        ///  Sent when ending the tracking of a scrollbar in a window, and also for scrollbar controls.
        /// </summary>
        /// <seealso cref="SystemScrollingStart"/>.
        SystemScrollingEnd = 0x0013,

        /// <summary>
        ///  <c>EVENT_SYSTEM_SWITCHSTART</c>
        ///  Sent when beginning alt-tab mode with the switch window.
        /// </summary>
        /// <seealso cref="SystemSwitchEnd"/>.
        SystemSwitchStart = 0x0014,

        /// <summary>
        ///  <c>EVENT_SYSTEM_SWITCHEND</c>
        ///  Sent when ending alt-tab mode with the switch window.
        /// </summary>
        /// <seealso cref="SystemSwitchStart"/>.
        SystemSwitchEnd = 0x0015,

        /// <summary>
        ///  <c>EVENT_SYSTEM_MINIMIZESTART</c>
        ///  Sent when a window minimizes and just before it restores.
        /// </summary>
        /// <seealso cref="SystemMinimizeEnd"/>.
        SystemMinimizeStart = 0x0016,

        /// <summary>
        ///  <c>EVENT_SYSTEM_MINIMIZEEND</c>
        ///  Sent when a window minimizes and just before it restores.
        /// </summary>
        /// <seealso cref="SystemMinimizeStart"/>.
        SystemMinimizeEnd = 0x0017,

        // Object events
        //
        // The system AND apps generate these. The system generates these for
        // real windows. Apps generate these for objects within their window which
        // act like a separate control, e.g. an item in a list view.
        //
        // For all events, if you want detailed accessibility information, callers
        // should
        //      * Call AccessibleObjectFromWindow() with the hwnd, idObject parameters
        //          of the event, and IID_IAccessible as the REFIID, to get back an
        //          IAccessible* to talk to
        //      * Initialize and fill in a VARIANT as VT_I4 with lVal the idChild
        //          parameter of the event.
        //      * If idChild isn't zero, call get_accChild() in the container to see
        //          if the child is an object in its own right. If so, you will get
        //          back an IDispatch* object for the child. You should release the
        //          parent, and call QueryInterface() on the child object to get its
        //          IAccessible*. Then you talk directly to the child. Otherwise,
        //          if get_accChild() returns you nothing, you should continue to
        //          use the child VARIANT. You will ask the container for the properties
        //          of the child identified by the VARIANT. In other words, the
        //          child in this case is accessible but not a full object.
        //          Like a button on a titlebar which is 'small' and has no children.

        /// <summary>
        ///  <c>EVENT_OBJECT_CREATE</c>
        ///  Sent when an object has been created.
        ///  The system sends this event for the following user interface elements:
        ///  caret, header control, list-view control, tab control, toolbar control, tree view control,
        ///  and window object. Server applications send this event for their accessible objects.
        /// </summary>
        Create = 0x8000,                // hwnd + ID + idChild is created item

        /// <summary>
        ///  <c>EVENT_OBJECT_DESTROY</c>
        ///  Sent when an object has been destroyed.
        ///  The system sends this event for the following user interface elements:
        ///  caret, header control, list-view control, tab control, toolbar control, tree view control,
        ///  and window object. Server applications send this event for their accessible objects.
        /// </summary>
        Destroy = 0x8001,               // hwnd + ID + idChild is destroyed item

        /// <summary>
        /// <c>EVENT_OBJECT_SHOW</c>
        ///  Sent when a hidden object is shown.
        ///  The system sends this event for the following user interface elements:
        ///  caret, cursor, and window object. Server applications send this event for their accessible objects.
        ///  Clients assume that when this event is sent by a parent object, all child objects are already displayed.
        ///  Therefore, server applications do not send this event for the child objects.
        /// </summary>
        Show = 0x8002,                  // hwnd + ID + idChild is shown item

        /// <summary>
        /// <c>EVENT_OBJECT_HIDE</c>
        ///  Sent when an object is hidden.
        ///  The system sends this event for the following user interface elements: caret and cursor.
        ///  Server applications send this event for their accessible objects.
        ///  When this event is generated for a parent object, all child objects are already hidden.
        ///  Server applications do not send this event for the child objects.
        /// </summary>
        Hide = 0x8003,                  // hwnd + ID + idChild is hidden item

        /// <summary>
        /// <c>EVENT_OBJECT_REORDER</c>
        ///  Sent when a container object has added, removed, or reordered its children.
        ///  The system sends this event for the following user interface elements:
        ///  header control, list-view control, toolbar control, and window object.
        ///  Server applications send this event as appropriate for their accessible objects.
        /// </summary>
        Reorder = 0x8004,               // hwnd + ID + idChild is parent of zordering children

        /// <summary>
        ///  Minimize the number of notifications!
        ///  When you are hiding a parent object, obviously all child objects are no
        ///  longer visible on screen. They still have the same "visible" status,
        ///  but are not truly visible. Hence do not send HIDE notifications for the
        ///  children also. One implies all. The same goes for SHOW.
        /// </summary>
        Focus = 0x8005,                 // hwnd + ID + idChild is focused item

        Selection = 0x8006,             // hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        SelectionAdd = 0x8007,          // hwnd + ID + idChild is item added
        SelectionRemove = 0x8008,       // hwnd + ID + idChild is item removed
        SelectionWithin = 0x8009,       // hwnd + ID + idChild is parent of changed selected items

        /// <summary>
        ///  There is only one "focused" child item in a parent. This is the place
        ///  keystrokes are going at a given moment. Hence only send a notification
        ///  about where the NEW focus is going. A NEW item getting the focus already
        ///  implies that the OLD item is losing it.
        ///
        ///  SELECTION however can be multiple. Hence the different SELECTION
        ///  notifications. Here's when to use each:
        ///
        ///  (1) Send a SELECTION notification in the simple single selection
        ///  case (like the focus) when the item with the selection is
        ///  merely moving to a different item within a container. hwnd + ID
        ///  is the container control, idChildItem is the new child with the
        ///  selection.
        ///
        ///  (2) Send a SELECTIONADD notification when a new item has simply been added
        ///  to the selection within a container. This is appropriate when the
        ///  number of newly selected items is very small. hwnd + ID is the
        ///  container control, idChildItem is the new child added to the selection.
        ///
        ///  (3) Send a SELECTIONREMOVE notification when a new item has simply been
        ///  removed from the selection within a container. This is appropriate
        ///  when the number of newly selected items is very small, just like
        ///  SELECTIONADD. hwnd + ID is the container control, idChildItem is the
        ///  new child removed from the selection.
        ///
        ///  (4) Send a SELECTIONWITHIN notification when the selected items within a
        ///  control have changed substantially. Rather than propagate a large
        ///  number of changes to reflect removal for some items, addition of
        ///  others, just tell somebody who cares that a lot happened. It will
        ///  be faster an easier for somebody watching to just turn around and
        ///  query the container control what the new bunch of selected items
        ///  are.
        /// </summary>
        StateChange = 0x800A,           // hwnd + ID + idChild is item w/ state change
        LocationChange = 0x800B,        // hwnd + ID + idChild is moved/sized item
        NameChange = 0x800C,            // hwnd + ID + idChild is item w/ name change
        DescriptionChange = 0x800D,     // hwnd + ID + idChild is item w/ desc change
        ValueChange = 0x800E,           // hwnd + ID + idChild is item w/ value change
        ParentChange = 0x800F,          // hwnd + ID + idChild is item w/ new parent
        HelpChange = 0x8010,            // hwnd + ID + idChild is item w/ help change
        DefaultActionChange = 0x8011,   // hwnd + ID + idChild is item w/ def action change
        AcceleratorChange = 0x8012,     // hwnd + ID + idChild is item w/ keybd accel change
    }
}
