// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies values representing possible states for an accessible object
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum AccessibleStates {

        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No state.
        ///    </para>
        /// </devdoc>
        None = 0,
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Unavailable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An unavailable object.
        ///    </para>
        /// </devdoc>
        Unavailable =     ( 0x1 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Selected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A selected object.
        ///    </para>
        /// </devdoc>
        Selected =        ( 0x2 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Focused"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An object with the keyboard focus.
        ///    </para>
        /// </devdoc>
        Focused =         ( 0x4 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Pressed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A pressed object.
        ///    </para>
        /// </devdoc>
        Pressed =         ( 0x8 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Checked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An object with a
        ///       selected check box.
        ///    </para>
        /// </devdoc>
        Checked =    ( 0x10 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Mixed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A three-state check box or toolbar button
        ///       whose state is indeterminate. The check box is neither checked nor unchecked and
        ///       it is in the
        ///       third or mixed state.
        ///    </para>
        /// </devdoc>
        Mixed =      ( 0x20 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Indeterminate"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A three-state check box or toolbar button
        ///       whose state is indeterminate. The check box is neither checked nor unchecked and it
        ///       is in the third
        ///       or mixed state.
        ///    </para>
        /// </devdoc>
        Indeterminate =      ( Mixed ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.ReadOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A read-only object.
        ///    </para>
        /// </devdoc>
        ReadOnly =   ( 0x40 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.HotTracked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object is hot-tracked by
        ///       the mouse, meaning its appearance is highlighted to indicate the mouse
        ///       pointer is located over it.
        ///    </para>
        /// </devdoc>
        HotTracked = ( 0x80 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Default"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The
        ///       default button or menu item.
        ///    </para>
        /// </devdoc>
        Default =    ( 0x100 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Expanded"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Children of the object that are items in an outline or tree
        ///       structure are displayed.
        ///    </para>
        /// </devdoc>
        Expanded =   ( 0x200 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Collapsed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Children of the object that are items in an outline or tree structure are
        ///       hidden.
        ///    </para>
        /// </devdoc>
        Collapsed =  ( 0x400 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Busy"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A
        ///       control
        ///       that cannot accept input in its current condition.
        ///    </para>
        /// </devdoc>
        Busy =       ( 0x800 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Floating"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object is not fixed to the boundary of its parent object, and
        ///       does not move automatically along with the parent.
        ///    </para>
        /// </devdoc>
        Floating =   ( 0x1000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Marqueed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An object with scrolling
        ///       or moving text or graphics.
        ///    </para>
        /// </devdoc>
        Marqueed =   ( 0x2000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Animated"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object has a rapidly or constantly changing
        ///       appearance. Graphics that are occasionally animated, but not always, should be
        ///       defined as <see langword='AccessibleRole.Graphic '/>|
        ///    <see langword='AccessibleStates.Animated '/>. This state should
        ///       not be used to indicate that
        ///       the object's location is changing.
        ///    </para>
        /// </devdoc>
        Animated =   ( 0x4000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Invisible"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An object that is currently invisible.
        ///    </para>
        /// </devdoc>
        Invisible =  ( 0x8000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Offscreen"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No on-screen representation. A
        ///       sound or alert object would have this state, or a
        ///       hidden window that is never made visible.
        ///    </para>
        /// </devdoc>
        Offscreen =  ( 0x10000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Sizeable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A sizable object.
        ///    </para>
        /// </devdoc>
        Sizeable =   ( 0x20000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Moveable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A movable object.
        ///    </para>
        /// </devdoc>
        Moveable =   ( 0x40000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.SelfVoicing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object or child can use text-to-speech (TTS) to describe itself. A
        ///       speech-based accessibility aid should not announce information when an object
        ///       with this state has the focus because the object will automatically announce
        ///       information about itself.
        ///    </para>
        /// </devdoc>
        SelfVoicing =        ( 0x80000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Focusable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object is on the active window
        ///       and can receive keyboard focus.
        ///    </para>
        /// </devdoc>
        Focusable =  ( 0x100000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Selectable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An object that can accept selection.
        ///    </para>
        /// </devdoc>
        Selectable = ( 0x200000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Linked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A linked object that has not
        ///       been previously selected.
        ///    </para>
        /// </devdoc>
        Linked =     ( 0x400000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Traversed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A linked object that has previously been selected.
        ///    </para>
        /// </devdoc>
        Traversed =  ( 0x800000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.MultiSelectable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An object
        ///       that accepts multiple selected items.
        ///    </para>
        /// </devdoc>
        MultiSelectable =    ( 0x1000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.ExtSelectable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Alters the selection so that all objects
        ///       between the selection anchor, which is the object with the
        ///       keyboard focus, and this object take on the anchor object's selection
        ///       state. If the anchor object is not selected, the objects are removed from
        ///       the selection. If the anchor object is selected, the selection is extended to
        ///       include this object and all the objects in between. You can set the selection
        ///       state by combining this flag with <see langword='AccessibleSelection.AddSelection '/>or <see langword='AccessibleSelection.RemoveSelection'/>. This flag does
        ///       not change the focus or the selection anchor unless it is combined with
        ///    <see langword='AccessibleSelection.TakeFocus'/>. The behavior of 
        ///    <see langword='AccessibleStates.ExtendSelection'/> | 
        ///    <see langword='AccessibleSelection.TakeFocus '/>is equivalent to adding an item 
        ///       to a selection manually by holding down the SHIFT key and clicking an unselected
        ///       object. This flag may not be combined with
        ///    <see langword='AccessibleSelection.TakeSelection'/>
        ///    .
        /// </para>
        /// </devdoc>
        ExtSelectable =      ( 0x2000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.AlertLow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Low-priority information that may not be important to the user.
        ///    </para>
        /// </devdoc>
        AlertLow =  ( 0x4000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.AlertMedium"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Important information that does not need to be conveyed to the user
        ///       immediately. For example, when a battery level indicator is starting to reach a
        ///       low level, it could generate a medium-level alert. Blind access utilities could
        ///       then generate a sound to let the user know that important information is
        ///       available, without actually interrupting the user's work. The user could then
        ///       query the alert information at his or her leisure.
        ///    </para>
        /// </devdoc>
        AlertMedium =       ( 0x8000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.AlertHigh"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Important information that should be conveyed to the user immediately. For
        ///       example, a battery level indicator reaching a critical low level would
        ///       transition to this state, in which case a blind access utility would announce
        ///       this information immediately to the user, and a screen magnification program
        ///       would scroll the screen so that the battery indicator is in view. This state is
        ///       also appropriate for any prompt or operation that must be completed before the
        ///       user can continue.
        ///    </para>
        /// </devdoc>
        AlertHigh = ( 0x10000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Protected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A password-protected edit control.
        ///    </para>
        /// </devdoc>
        Protected =  ( 0x20000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.HasPopup"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Object displays a pop-up menu or window when invoked.
        ///    </para>
        /// </devdoc>
        HasPopup =  ( 0x40000000 ),
        
        
        /// <include file='doc\AccessibleStates.uex' path='docs/doc[@for="AccessibleStates.Valid"]/*' />
        /// <devdoc>
        /// </devdoc>
        [Obsolete("This enumeration value has been deprecated. There is no replacement. http://go.microsoft.com/fwlink/?linkid=14202")]
        Valid =      ( 0x3fffffff ),
        
    }
}
