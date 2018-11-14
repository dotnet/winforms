// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Remoting;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Drawing;
    using Microsoft.Win32;

    // ----------------------------------------------------------------------------------------------------------------------------
    // WARNING: if adding to this enumeration please update Control & ToolStripItem AccessibleRole to ensure the new member is valid.
    // -----------------------------------------------------------------------------------------------------------------------------
    
    /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies values representing possible roles for an accessible object.
    ///    </para>
    /// </devdoc>
    public enum AccessibleRole {

        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Default"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A system provided role.
        ///    </para>
        /// </devdoc>
        Default = -1,
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No role.
        ///    </para>
        /// </devdoc>
        None = 0,
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.TitleBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A title or caption bar
        ///       for a window.
        ///    </para>
        /// </devdoc>
        TitleBar =    ( 0x1 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.MenuBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A menu bar, usually beneath the
        ///       title bar of a window, from which menus can be selected
        ///       by the user.
        ///    </para>
        /// </devdoc>
        MenuBar =     ( 0x2 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ScrollBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A vertical or horizontal scroll bar, which
        ///       can be either part of the client area or used
        ///       in a control.
        ///    </para>
        /// </devdoc>
        ScrollBar =   ( 0x3 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Grip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A special mouse pointer, which
        ///       allows a user to manipulate user interface elements such as a window. For
        ///       example, a user can click and drag a sizing grip in the lower-right corner of a
        ///       window to
        ///       resize it.
        ///    </para>
        /// </devdoc>
        Grip =        ( 0x4 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Sound"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A system sound, which is associated with
        ///       various
        ///       system events.
        ///    </para>
        /// </devdoc>
        Sound =       ( 0x5 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Cursor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A mouse pointer.
        ///    </para>
        /// </devdoc>
        Cursor =      ( 0x6 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Caret"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A caret, which is a flashing line, block, or bitmap
        ///       that marks the location of the insertion point in a
        ///       window's client area.
        ///    </para>
        /// </devdoc>
        Caret =       ( 0x7 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Alert"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An alert or condition that a
        ///       user should be notified about. This role should be used only for objects that
        ///       embody an alert but are not associated with another user interface element, such
        ///       as a message box, graphic, text,
        ///       or sound.
        ///    </para>
        /// </devdoc>
        Alert =       ( 0x8 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Window"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A window frame, which usually contains child
        ///       objects such as a title bar, client, and other objects typically contained in
        ///       a window.
        ///    </para>
        /// </devdoc>
        Window =      ( 0x9 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Client"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A
        ///       window's user area.
        ///    </para>
        /// </devdoc>
        Client =      ( 0xa ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.MenuPopup"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A menu, which presents a list of
        ///       options from which the user can make a selection to perform an action. All menu
        ///       types must have this role, including drop-down menus that are displayed by
        ///       selection from a menu bar, and shortcut menus that are displayed when the right
        ///       mouse button is
        ///       clicked.
        ///    </para>
        /// </devdoc>
        MenuPopup =   ( 0xb ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.MenuItem"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A menu item, which is an entry in
        ///       a menu that a user can choose to carry out a command, select
        ///       an option, or display another menu.
        ///       Functionally, a menu item can be equivalent to a push button, radio button, check
        ///       box, or menu.
        ///    </para>
        /// </devdoc>
        MenuItem =    ( 0xc ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ToolTip"]/*' />
        /// <devdoc>
        ///    A tool tip, which is a small rectangular pop-up
        ///    window that displays a brief description of a command bar button's
        ///    purpose.
        /// </devdoc>
        ToolTip =     ( 0xd ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Application"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The main window for
        ///       an application.
        ///    </para>
        /// </devdoc>
        Application = ( 0xe ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Document"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A
        ///       document window, which is always
        ///       contained within an application window. This role applies only to
        ///       multiple document interface (MDI) windows and refers to an object that contains
        ///       the MDI title bar.
        ///    </para>
        /// </devdoc>
        Document =    ( 0xf ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Pane"]/*' />
        /// <devdoc>
        ///    <para>
        ///       One of the separate areas in a frame, a split document
        ///       window, or a rectangular area of the status
        ///       bar that can be used to display
        ///       information. Users can navigate between panes and within the contents of the current
        ///       pane, but cannot navigate between items in different panes. Thus, panes
        ///       represent a level of grouping lower than frame windows or documents, but above
        ///       individual controls. Typically the user navigates between panes by pressing TAB,
        ///       F6, or CTRL+TAB, depending on the
        ///       context.
        ///    </para>
        /// </devdoc>
        Pane =        ( 0x10 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Chart"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A graphical image used to
        ///       represent data.
        ///    </para>
        /// </devdoc>
        Chart =       ( 0x11 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Dialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A dialog box
        ///       or message box.
        ///    </para>
        /// </devdoc>
        Dialog =      ( 0x12 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Border"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A window border. The entire border
        ///       is represented by a single object, rather than by separate objects for
        ///       each side.
        ///    </para>
        /// </devdoc>
        Border =      ( 0x13 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Grouping"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Objects grouped in a logical
        ///       manner. There can be a parent-child relationship between the grouping object and the
        ///       objects
        ///       it contains.
        ///    </para>
        /// </devdoc>
        Grouping =    ( 0x14 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Separator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Visually divides a space into two regions, such as a separator menu item or a
        ///       bar dividing split panes within a window.
        ///    </para>
        /// </devdoc>
        Separator =   ( 0x15 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ToolBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A toolbar, which is a grouping of controls that provide
        ///       easy access to frequently
        ///       used features.
        ///    </para>
        /// </devdoc>
        ToolBar =     ( 0x16 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.StatusBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A status bar, which is an area typically at the bottom
        ///       of an application window
        ///       that displays information about the current operation, state of the application,
        ///       or selected object. The status bar can have multiple fields that display different
        ///       kinds of information, such as an explanation of the currently selected menu
        ///       command in the status bar.
        ///    </para>
        /// </devdoc>
        StatusBar =   ( 0x17 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Table"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A table containing rows and columns of cells, and
        ///       optionally, row headers and column headers.
        ///    </para>
        /// </devdoc>
        Table =       ( 0x18 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ColumnHeader"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A column header, which provides a visual label for a column
        ///       in a table.
        ///    </para>
        /// </devdoc>
        ColumnHeader =        ( 0x19 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.RowHeader"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A row header, which provides a visual
        ///       label for
        ///       a table row.
        ///    </para>
        /// </devdoc>
        RowHeader =   ( 0x1a ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Column"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A column of cells within a table.
        ///    </para>
        /// </devdoc>
        Column =      ( 0x1b ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Row"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A row of cells within
        ///       a table.
        ///    </para>
        /// </devdoc>
        Row = ( 0x1c ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Cell"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A cell within
        ///       a table.
        ///    </para>
        /// </devdoc>
        Cell =        ( 0x1d ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Link"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A link, which is a connection between a source
        ///       document and a destination document. This
        ///       object might look like text or a graphic, but it acts like
        ///       a button.
        ///    </para>
        /// </devdoc>
        Link =        ( 0x1e ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.HelpBalloon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Help display in the form of a ToolTip or Help
        ///       balloon, which
        ///       contains
        ///       buttons and labels that users can click to open
        ///       custom Help topics.
        ///    </para>
        /// </devdoc>
        HelpBalloon = ( 0x1f ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Character"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A cartoon-like graphic object,
        ///       such as Microsoft Office Assistant, which is typically displayed to provide help
        ///       to users of
        ///       an application.
        ///    </para>
        /// </devdoc>
        Character =   ( 0x20 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.List"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A list box, which allows the user
        ///       to select one or
        ///       more items.
        ///    </para>
        /// </devdoc>
        List =        ( 0x21 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ListItem"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An item in a list box or the list
        ///       portion of a combo box, drop-down list box, or
        ///       drop-down combo box.
        ///    </para>
        /// </devdoc>
        ListItem =    ( 0x22 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Outline"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An outline or tree structure, such
        ///       as a tree view control, which displays a hierarchical list and usually allows
        ///       the
        ///       user to expand and collapse branches.
        ///    </para>
        /// </devdoc>
        Outline =     ( 0x23 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.OutlineItem"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An item
        ///       in an outline or tree structure.
        ///    </para>
        /// </devdoc>
        OutlineItem = ( 0x24 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.PageTab"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A property page that allows a user
        ///       to view the attributes for a page, such as the page's title, whether it is a
        ///       home page, or
        ///       whether the page has been modified.
        ///       Normally the only child of this control is a grouped object that contains
        ///       the contents of the associated page.
        ///    </para>
        /// </devdoc>
        PageTab =     ( 0x25 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.PropertyPage"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A property page, which is a dialog box that
        ///       controls the appearance and the behavior of an object, such as a file or
        ///       resource. A property page's appearance differs according
        ///       to its purpose.
        ///    </para>
        /// </devdoc>
        PropertyPage =        ( 0x26 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Indicator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An indicator, such as a pointer
        ///       graphic, that points to the
        ///       current item.
        ///    </para>
        /// </devdoc>
        Indicator =   ( 0x27 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Graphic"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A picture.
        ///    </para>
        /// </devdoc>
        Graphic =     ( 0x28 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.StaticText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Read-only text,
        ///       such as in a label, for other
        ///       controls or instructions in a dialog box. Static text cannot be modified
        ///       or selected.
        ///    </para>
        /// </devdoc>
        StaticText =  ( 0x29 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selectable text
        ///       that can be editable or read-only.
        ///    </para>
        /// </devdoc>
        Text =        ( 0x2a ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.PushButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A push button control, which is a
        ///       small rectangular control that a user can turn on or off. A push button, also known
        ///       as a command button, has a raised appearance in its default off state and
        ///       a sunken appearance when it is turned
        ///       on.
        ///    </para>
        /// </devdoc>
        PushButton =  ( 0x2b ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.CheckButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A check box control, which is an option
        ///       that can be turned on or off independently
        ///       of other options.
        ///    </para>
        /// </devdoc>
        CheckButton = ( 0x2c ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.RadioButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An option button, also known as a
        ///       radio button. All objects sharing the same parent that have this attribute are
        ///       assumed to be part of a single mutually exclusive group. You can use
        ///       grouped objects to divide option buttons into separate groups when
        ///       necessary.
        ///    </para>
        /// </devdoc>
        RadioButton = ( 0x2d ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ComboBox"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A
        ///       combo box, which is an edit
        ///       control with an associated list box that provides a set of predefined
        ///       choices.
        ///    </para>
        /// </devdoc>
        ComboBox =    ( 0x2e ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.DropList"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A drop-down list box. This control shows one
        ///       item and allows the user to display and select another
        ///       from a list of alternative choices.
        ///    </para>
        /// </devdoc>
        DropList =    ( 0x2f ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ProgressBar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A progress bar, which indicates the progress of a
        ///       lengthy operation by displaying a colored bar inside a horizontal rectangle. The
        ///       length of the bar in relation
        ///       to the length of the rectangle corresponds
        ///       to the percentage of the operation that is complete. This control does
        ///       not take user
        ///       input.
        ///    </para>
        /// </devdoc>
        ProgressBar = ( 0x30 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Dial"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A dial or knob. This can also be a
        ///       read-only object, like a speedometer.
        ///    </para>
        /// </devdoc>
        Dial =        ( 0x31 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.HotkeyField"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A hot-key field that allows the user to enter a combination or sequence of
        ///       keystrokes to be used as a hot key, which enables users to perform an action
        ///       quickly. A hot-key control displays the keystrokes entered by the user and
        ///       ensures that the user selects a valid key combination.
        ///    </para>
        /// </devdoc>
        HotkeyField = ( 0x32 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Slider"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A control, sometimes called a trackbar,
        ///       that allows a user to adjust a setting in given increments between
        ///       minimum and maximum values by moving a slider. The volume controls in the
        ///       Windows operating system are slider
        ///       controls.
        ///    </para>
        /// </devdoc>
        Slider =      ( 0x33 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.SpinButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A spin box, also
        ///       known as an up-down control, which contains a pair of arrow buttons that a
        ///       user click with a mouse to increment or decrement a value. A spin button control is
        ///       most often used with a companion control, called a buddy window, where the current
        ///       value is
        ///       displayed.
        ///    </para>
        /// </devdoc>
        SpinButton =  ( 0x34 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Diagram"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A graphical image used to diagram data.
        ///    </para>
        /// </devdoc>
        Diagram =     ( 0x35 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Animation"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An animation control, which
        ///       contains content that is changing over time, such as a control that displays a
        ///       series of bitmap frames, like a film strip. Animation controls are usually
        ///       displayed when files are being copied, or when some other time-consuming task is
        ///       being
        ///       performed.
        ///    </para>
        /// </devdoc>
        Animation =   ( 0x36 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Equation"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A mathematical equation.
        ///    </para>
        /// </devdoc>
        Equation =    ( 0x37 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ButtonDropDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A button that
        ///       drops down a list of items.
        ///    </para>
        /// </devdoc>
        ButtonDropDown =      ( 0x38 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ButtonMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A button that drops down a menu.
        ///    </para>
        /// </devdoc>
        ButtonMenu =  ( 0x39 ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.ButtonDropDownGrid"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A button that drops down a grid.
        ///    </para>
        /// </devdoc>
        ButtonDropDownGrid =  ( 0x3a ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.WhiteSpace"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A blank space between other objects.
        ///    </para>
        /// </devdoc>
        WhiteSpace =  ( 0x3b ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.PageTabList"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A container of page tab controls.
        ///    </para>
        /// </devdoc>
        PageTabList = ( 0x3c ),
        
        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.Clock"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A
        ///       control that displays the time.
        ///    </para>
        /// </devdoc>
        Clock =       ( 0x3d ),

        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.SplitButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A toolbar button that jas a drop-down list icon directly adjacent to the button.
        ///    </para>
        /// </devdoc>
        SplitButton = ( 0x3e ),

        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.IpAddress"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A control designed for entering Internet Protocol (IP) addresses.
        ///    </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase")]
        IpAddress = ( 0x3f ),

        /// <include file='doc\AccessibleRoles.uex' path='docs/doc[@for="AccessibleRole.OutlineButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A control that navigates like an outline item.
        ///    </para>
        /// </devdoc>
        OutlineButton = ( 0x40 ),


    }
}
