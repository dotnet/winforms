﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies events that are reported by accessible applications.</para>
    /// </devdoc>

    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum AccessibleEvents
    {
        /// <devdoc>>
        /// EVENT_SYSTEM_SOUND
        /// Sent when a sound is played. Currently nothing is generating this, we
        /// are going to be cleaning up the SOUNDSENTRY feature in the control panel
        /// and will use this at that time. Applications implementing WinEvents
        /// are perfectly welcome to use it. Clients of IAccessible* will simply
        /// turn around and get back a non-visual object that describes the sound.
        /// </devdoc>>
        SystemSound = 0x0001,

        /// <devdoc>>
        /// EVENT_SYSTEM_ALERT
        /// Sent when an alert needs to be given to the user. MessageBoxes generate
        /// alerts for example.
        /// </devdoc>>
        SystemAlert = 0x0002,

        /// <devdoc>>
        /// EVENT_SYSTEM_FOREGROUND
        /// Sent when the foreground (active) window changes, even if it is changing
        /// to another window in the same thread as the previous one.
        /// </devdoc>>
        SystemForeground = 0x0003,

        /// <devdoc>>
        /// EVENT_SYSTEM_MENUSTART
        /// EVENT_SYSTEM_MENUEND
        /// Sent when entering into and leaving from menu mode (system, app bar, and
        /// track popups).
        /// </devdoc>>
        SystemMenuStart = 0x0004,
        SystemMenuEnd = 0x0005,

        /// <devdoc>>
        /// EVENT_SYSTEM_MENUPOPUPSTART
        /// EVENT_SYSTEM_MENUPOPUPEND
        /// Sent when a menu popup comes up and just before it is taken down. Note
        /// that for a call to TrackPopupMenu(), a client will see EVENT_SYSTEM_MENUSTART
        /// followed almost immediately by EVENT_SYSTEM_MENUPOPUPSTART for the popup
        /// being shown.
        /// </devdoc>>
        SystemMenuPopupStart = 0x0006,
        SystemMenuPopupEnd = 0x0007,


        /// <devdoc>>
        /// EVENT_SYSTEM_CAPTURESTART
        /// EVENT_SYSTEM_CAPTUREEND
        /// Sent when a window takes the capture and releases the capture.
        /// </devdoc>>
        SystemCaptureStart = 0x0008,
        SystemCaptureEnd = 0x0009,

        /// </devdoc>>
        /// EVENT_SYSTEM_MOVESIZESTART
        /// EVENT_SYSTEM_MOVESIZEEND
        /// Sent when a window enters and leaves move-size dragging mode.
        /// <//summary>
        SystemMoveSizeStart = 0x000A,
        SystemMoveSizeEnd = 0x000B,

        /// <devdoc>>
        /// EVENT_SYSTEM_CONTEXTHELPSTART
        /// EVENT_SYSTEM_CONTEXTHELPEND
        /// Sent when a window enters and leaves context sensitive help mode.
        /// </devdoc>>
        SystemContextHelpStart = 0x000C,
        SystemContextHelpEnd = 0x000D,

        /// <devdoc>>
        /// EVENT_SYSTEM_DRAGDROPSTART
        /// EVENT_SYSTEM_DRAGDROPEND
        /// Sent when a window enters and leaves drag drop mode. Note that it is up
        /// to apps and OLE to generate this, since the system doesn't know. Like
        /// EVENT_SYSTEM_SOUND, it will be a while before this is prevalent.
        /// </devdoc>>
        SystemDragDropStart = 0x000E,
        SystemDragDropEnd = 0x000F,

        /// <devdoc>>
        /// EVENT_SYSTEM_DIALOGSTART
        /// EVENT_SYSTEM_DIALOGEND
        /// Sent when a dialog comes up and just before it goes away.
        /// </devdoc>>
        SystemDialogStart = 0x0010,
        SystemDialogEnd = 0x0011,

        /// <devdoc>>
        /// EVENT_SYSTEM_SCROLLINGSTART
        /// EVENT_SYSTEM_SCROLLINGEND
        /// Sent when beginning and ending the tracking of a scrollbar in a window,
        /// and also for scrollbar controls.
        /// </devdoc>>
        SystemScrollingStart = 0x0012,
        SystemScrollingEnd = 0x0013,

        /// <devdoc>>
        /// EVENT_SYSTEM_SWITCHSTART
        /// EVENT_SYSTEM_SWITCHEND
        /// Sent when beginning and ending alt-tab mode with the switch window.
        /// </devdoc>>
        SystemSwitchStart = 0x0014,
        SystemSwitchEnd = 0x0015,

        /// <devdoc>>
        /// EVENT_SYSTEM_MINIMIZESTART
        /// EVENT_SYSTEM_MINIMIZEEND
        /// Sent when a window minimizes and just before it restores.
        /// </devdoc>>
        SystemMinimizeStart = 0x0016,
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
        Create =  0x8000,  // hwnd + ID + idChild is created item
        Destroy = 0x8001,  // hwnd + ID + idChild is destroyed item
        Show =    0x8002,  // hwnd + ID + idChild is shown item
        Hide =    0x8003,  // hwnd + ID + idChild is hidden item
        Reorder = 0x8004,  // hwnd + ID + idChild is parent of zordering children

        /// <devdoc>
        /// Minimize the number of notifications!
        /// When you are hiding a parent object, obviously all child objects are no
        /// longer visible on screen. They still have the same "visible" status,
        /// but are not truly visible. Hence do not send HIDE notifications for the
        /// children also. One implies all. The same goes for SHOW.
        /// </devdoc>
        Focus =           0x8005,  // hwnd + ID + idChild is focused item
        Selection =       0x8006,  // hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        SelectionAdd =    0x8007,  // hwnd + ID + idChild is item added
        SelectionRemove = 0x8008,  // hwnd + ID + idChild is item removed
        SelectionWithin = 0x8009,  // hwnd + ID + idChild is parent of changed selected items

        /// <devdoc>
        /// There is only one "focused" child item in a parent. This is the place
        /// keystrokes are going at a given moment. Hence only send a notification
        /// about where the NEW focus is going. A NEW item getting the focus already
        /// implies that the OLD item is losing it.
        ///
        /// SELECTION however can be multiple. Hence the different SELECTION
        /// notifications. Here's when to use each:
        ///
        /// (1) Send a SELECTION notification in the simple single selection
        ///     case (like the focus) when the item with the selection is
        ///     merely moving to a different item within a container. hwnd + ID
        ///     is the container control, idChildItem is the new child with the
        ///     selection.
        ///
        /// (2) Send a SELECTIONADD notification when a new item has simply been added
        ///     to the selection within a container. This is appropriate when the
        ///     number of newly selected items is very small. hwnd + ID is the
        ///     container control, idChildItem is the new child added to the selection.
        ///
        /// (3) Send a SELECTIONREMOVE notification when a new item has simply been
        ///     removed from the selection within a container. This is appropriate
        ///     when the number of newly selected items is very small, just like
        ///     SELECTIONADD. hwnd + ID is the container control, idChildItem is the
        ///     new child removed from the selection.
        ///
        /// (4) Send a SELECTIONWITHIN notification when the selected items within a
        ///     control have changed substantially. Rather than propagate a large
        ///     number of changes to reflect removal for some items, addition of
        ///     others, just tell somebody who cares that a lot happened. It will
        ///     be faster an easier for somebody watching to just turn around and
        ///     query the container control what the new bunch of selected items
        ///     are.
        /// </devdoc>
        StateChange =         0x800A,  // hwnd + ID + idChild is item w/ state change
        LocationChange =      0x800B,  // hwnd + ID + idChild is moved/sized item
        NameChange =          0x800C,  // hwnd + ID + idChild is item w/ name change
        DescriptionChange =   0x800D,  // hwnd + ID + idChild is item w/ desc change
        ValueChange =         0x800E,  // hwnd + ID + idChild is item w/ value change
        ParentChange =        0x800F,  // hwnd + ID + idChild is item w/ new parent
        HelpChange =          0x8010,  // hwnd + ID + idChild is item w/ help change
        DefaultActionChange = 0x8011,  // hwnd + ID + idChild is item w/ def action change
        AcceleratorChange =   0x8012,  // hwnd + ID + idChild is item w/ keybd accel change
    }
}
