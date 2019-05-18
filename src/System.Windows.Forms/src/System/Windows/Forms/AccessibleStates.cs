// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies values representing possible states for an accessible object
    /// </devdoc>
    [Flags]
    public enum AccessibleStates
    {
        /// <summary>
        /// No state.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// An unavailable object.
        /// </devdoc>
        Unavailable =     0x1,

        /// <summary>
        /// A selected object.
        /// </devdoc>
        Selected =        0x2,

        /// <summary>
        /// An object with the keyboard focus.
        /// </devdoc>
        Focused =         0x4,

        /// <summary>
        /// A pressed object.
        /// </devdoc>
        Pressed =         0x8,

        /// <summary>
        /// An object with a selected check box.
        /// </devdoc>
        Checked =    0x10,

        /// <summary>
        /// A three-state check box or toolbar button whose state is indeterminate.
        /// The check box is neither checked nor unchecked and it is in the third
        /// or mixed state.
        /// </devdoc>
        Mixed =      0x20,

        /// <summary>
        /// A three-state check box or toolbar button whose state is indeterminate.
        /// The check box is neither checked nor unchecked and it is in the third
        /// or mixed state.
        /// </devdoc>
        Indeterminate =      Mixed,

        /// <summary>
        /// A read-only object.
        /// </devdoc>
        ReadOnly =   0x40,

        /// <summary>
        /// The object is hot-tracked by the mouse, meaning its appearance is
        /// highlighted to indicate the mouse pointer is located over it.
        /// </devdoc>
        HotTracked = 0x80,

        /// <summary>
        /// The default button or menu item.
        /// </devdoc>
        Default =    0x100,

        /// <summary>
        /// Children of the object that are items in an outline or tree structure
        /// are displayed.
        /// </devdoc>
        Expanded =   0x200,

        /// <summary>
        /// Children of the object that are items in an outline or tree structure
        /// are hidden.
        /// </devdoc>
        Collapsed =  0x400,

        /// <summary>
        /// A control that cannot accept input in its current condition.
        /// </devdoc>
        Busy =       0x800,

        /// <summary>
        /// The object is not fixed to the boundary of its parent object, and
        /// does not move automatically along with the parent.
        /// </devdoc>
        Floating =   0x1000,

        /// <summary>
        /// An object with scrolling or moving text or graphics.
        /// </devdoc>
        Marqueed =   0x2000,

        /// <summary>
        /// The object has a rapidly or constantly changing appearance. Graphics
        /// that are occasionally animated, but not always, should be defined as
        /// <see cref='AccessibleRole.Graphic'/>|<see cref='AccessibleStates.Animated'/>.
        /// This state should not be used to indicate that the object's location
        /// is changing.
        /// </devdoc>
        Animated =   0x4000,

        /// <summary>
        /// An object that is currently invisible.
        /// </devdoc>
        Invisible =  0x8000,

        /// <summary>
        /// No on-screen representation. A sound or alert object would have this
        /// state, or a hidden window that is never made visible.
        /// </devdoc>
        Offscreen =  0x10000,

        /// <summary>
        /// A sizable object.
        /// </devdoc>
        Sizeable =   0x20000,

        /// <summary>
        /// A movable object.
        /// </devdoc>
        Moveable =   0x40000,

        /// <summary>
        /// The object or child can use text-to-speech (TTS) to describe itself. A
        /// speech-based accessibility aid should not announce information when an
        /// object with this state has the focus because the object will
        /// automatically announce information about itself.
        /// </devdoc>
        SelfVoicing =        0x80000,

        /// <summary>
        /// The object is on the active window and can receive keyboard focus.
        /// </devdoc>
        Focusable =  0x100000,

        /// <summary>
        /// An object that can accept selection.
        /// </devdoc>
        Selectable = 0x200000,

        /// <summary>
        /// A linked object that has not been previously selected.
        /// </devdoc>
        Linked =     0x400000,

        /// <summary>
        /// A linked object that has previously been selected.
        /// </devdoc>
        Traversed =  0x800000,

        /// <summary>
        /// An object that accepts multiple selected items.
        /// </devdoc>
        MultiSelectable =    0x1000000,

        /// <summary>
        /// Alters the selection so that all objects between the selection anchor,
        /// which is the object with the keyboard focus, and this object take on
        /// the anchor object's selection state. If the anchor object is not
        /// selected, the objects are removed from the selection. If the anchor
        /// object is selected, the selection is extended to include this object
        /// and all the objects in between. You can set the selection state by
        /// combining this flag with <see cref='AccessibleSelection.AddSelection '/>
        /// or <see cref='AccessibleSelection.RemoveSelection'/>. This flag does
        /// not change the focus or the selection anchor unless it is combined with
        /// <see cref='AccessibleSelection.TakeFocus'/>. The behavior of
        /// <see cref='AccessibleStates.ExtendSelection'/>|<see cref='AccessibleSelection.TakeFocus '/>
        /// is equivalent to adding an item to a selection manually by holding down
        /// the SHIFT key and clicking an unselected object.
        /// This flag may not be combined with <see cref='AccessibleSelection.TakeSelection'/>.
        /// </devdoc>
        ExtSelectable =      0x2000000,

        /// <summary>
        /// Low-priority information that may not be important to the user.
        /// </devdoc>
        AlertLow =  0x4000000,

        /// <summary>
        /// Important information that does not need to be conveyed to the user
        /// immediately. For example, when a battery level indicator is starting
        /// to reach a low level, it could generate a medium-level alert. Blind
        /// access utilities could then generate a sound to let the user know that
        /// important information is available, without actually interrupting the
        /// user's work. The user could then query the alert information at their
        /// leisure.
        /// </devdoc>
        AlertMedium =       0x8000000,

        /// <summary>
        /// Important information that should be conveyed to the user immediately.
        /// For example, a battery level indicator reaching a critical low level
        /// would transition to this state, in which case a blind access utility
        /// would announce this information immediately to the user, and a screen
        /// magnification program would scroll the screen so that the battery
        /// indicator is in view. This state is also appropriate for any prompt
        /// or operation that must be completed before the user can continue.
        ///    </para>
        /// </devdoc>
        AlertHigh = 0x10000000,

        /// <summary>
        /// A password-protected edit control.
        /// </devdoc>
        Protected =  0x20000000,

        /// <summary>
        /// Object displays a pop-up menu or window when invoked.
        /// </devdoc>
        HasPopup =  0x40000000,

        [Obsolete("This enumeration value has been deprecated. There is no replacement. http://go.microsoft.com/fwlink/?linkid=14202")]
        Valid =      0x3fffffff,
    }
}
