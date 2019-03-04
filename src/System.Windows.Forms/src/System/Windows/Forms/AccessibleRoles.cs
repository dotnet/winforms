// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies values representing possible roles for an accessible object.
    /// </devdoc>
    /// <remarks>
    /// if adding to this enumeration please update Control and ToolStripItem
    /// AccessibleRole to ensure the new member is valid.
    /// </remarks>
    public enum AccessibleRole
    {
        /// <devdoc>
        /// A system provided role.
        /// </devdoc>
        Default = -1,

        /// <devdoc>
        /// No role.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// A title or caption bar for a window.
        /// </devdoc>
        TitleBar = 0x1,

        /// <devdoc>
        /// A menu bar, usually beneath the title bar of a window, from which menus
        /// can be selected by the user.
        /// </devdoc>
        MenuBar = 0x2,

        /// <devdoc>
        /// A vertical or horizontal scroll bar, which can be either part of the
        /// client area or used in a control.
        /// </devdoc>
        ScrollBar = 0x3,

        /// <devdoc>
        /// A special mouse pointer, which allows a user to manipulate user interface
        /// elements such as a window. For example, a user can click and drag a
        /// sizing grip in the lower-right corner of a window to resize it.
        /// </devdoc>
        Grip = 0x4,

        /// <devdoc>
        /// A system sound, which is associated with various system events.
        /// </devdoc>
        Sound = 0x5,

        /// <devdoc>
        /// A mouse pointer.
        /// </devdoc>
        Cursor = 0x6,

        /// <devdoc>
        /// A caret, which is a flashing line, block, or bitmap that marks the
        /// location of the insertion point in a window's client area.
        /// </devdoc>
        Caret = 0x7,

        /// <devdoc>
        /// An alert or condition that a user should be notified about. This role
        /// should be used only for objects that embody an alert but are not
        /// associated with another user interface element, such as a message box,
        /// graphic, text, or sound.
        /// </devdoc>
        Alert = 0x8,

        /// <devdoc>
        /// A window frame, which usually contains child objects such as a title
        /// bar, client, and other objects typically contained in a window.
        /// </devdoc>
        Window = 0x9,

        /// <devdoc>
        /// A window's user area.
        /// </devdoc>
        Client = 0xa,

        /// <devdoc>
        /// A menu, which presents a list of options from which the user can make
        /// a selection to perform an action. All menu types must have this role,
        /// including drop-down menus that are displayed by selection from a menu
        /// bar, and shortcut menus that are displayed when the right mouse
        /// button is clicked.
        /// </devdoc>
        MenuPopup = 0xb,

        /// <devdoc>
        /// A menu item, which is an entry in a menu that a user can choose to
        /// carry out a command, select an option, or display another menu.
        /// Functionally, a menu item can be equivalent to a push button, radio
        /// button, check box, or menu.
        /// </devdoc>
        MenuItem = 0xc,

        /// <devdoc>
        /// A tool tip, which is a small rectangular pop-up window that displays
        /// a brief description of a command bar button's purpose.
        /// </devdoc>
        ToolTip = 0xd,

        /// <devdoc>
        /// The main window for an application.
        /// </devdoc>
        Application = 0xe,

        /// <devdoc>
        /// A document window, which is always contained within an application
        /// window. This role applies only to multiple document interface (MDI)
        /// windows and refers to an object that contains the MDI title bar.
        /// </devdoc>
        Document = 0xf,

        /// <devdoc>
        /// One of the separate areas in a frame, a split document window, or a
        /// rectangular area of the status bar that can be used to display
        /// information. Users can navigate between panes and within the contents
        /// of the current pane, but cannot navigate between items in different
        /// panes. Thus, panes represent a level of grouping lower than frame
        /// windows or documents, but above individual controls. Typically the
        /// user navigates between panes by pressing TAB, F6, or CTRL+TAB, depending
        /// on the context.
        /// </devdoc>
        Pane = 0x10,

        /// <devdoc>
        /// A graphical image used to represent data.
        /// </devdoc>
        Chart = 0x11,

        /// <devdoc>
        /// A dialog box or message box.
        /// </devdoc>
        Dialog = 0x12,

        /// <devdoc>
        /// A window border. The entire border is represented by a single object,
        /// rather than by separate objects for each side.
        /// </devdoc>
        Border = 0x13,

        /// <devdoc>
        /// Objects grouped in a logical manner. There can be a parent-child
        /// relationship between the grouping object and the objects it contains.
        /// </devdoc>
        Grouping = 0x14,

        /// <devdoc>
        /// Visually divides a space into two regions, such as a separator menu
        /// item or a bar dividing split panes within a window.
        /// </devdoc>
        Separator = 0x15,

        /// <devdoc>
        /// A toolbar, which is a grouping of controls that provide easy access
        /// to frequently used features.
        /// </devdoc>
        ToolBar = 0x16,

        /// <devdoc>
        /// A status bar, which is an area typically at the bottom of an application
        /// window that displays information about the current operation, state of
        /// the application, or selected object. The status bar can have multiple
        /// fields that display different kinds of information, such as an
        /// explanation of the currently selected menu command in the status bar.
        /// </devdoc>
        StatusBar = 0x17,

        /// <devdoc>
        /// A table containing rows and columns of cells, and optionally, row
        /// headers and column headers.
        /// </devdoc>
        Table = 0x18,

        /// <devdoc>
        /// A column header, which provides a visual label for a column in a table.
        /// </devdoc>
        ColumnHeader = 0x19,

        /// <devdoc>
        /// A row header, which provides a visual label for a table row.
        /// </devdoc>
        RowHeader = 0x1a,

        /// <devdoc>
        /// A column of cells within a table.
        /// </devdoc>
        Column = 0x1b,

        /// <devdoc>
        /// A row of cells within a table.
        /// </devdoc>
        Row = 0x1c,

        /// <devdoc>
        /// A cell within a table.
        /// </devdoc>
        Cell = 0x1d,

        /// <devdoc>
        /// A link, which is a connection between a source document and a destination
        /// document. This object might look like text or a graphic, but it acts like
        /// a button.
        /// </devdoc>
        Link = 0x1e,

        /// <devdoc>
        /// A Help display in the form of a ToolTip or Help balloon, which contains
        /// buttons and labels that users can click to open custom Help topics.
        /// </devdoc>
        HelpBalloon = 0x1f,

        /// <devdoc>
        /// A cartoon-like graphic object, such as Microsoft Office Assistant, which
        /// is typically displayed to provide help to users of an application.
        /// </devdoc>
        Character = 0x20,

        /// <devdoc>
        /// A list box, which allows the user to select one or more items.
        /// </devdoc>
        List = 0x21,

        /// <devdoc>
        /// An item in a list box or the list portion of a combo box, drop-down
        /// list box, or drop-down combo box.
        /// </devdoc>
        ListItem = 0x22,

        /// <devdoc>
        /// An outline or tree structure, such as a tree view control, which
        /// displays a hierarchical list and usually allows the user to expand
        /// and collapse branches.
        /// </devdoc>
        Outline = 0x23,

        /// <devdoc>
        /// An item in an outline or tree structure.
        /// </devdoc>
        OutlineItem = 0x24,

        /// <devdoc>
        /// A property page that allows a user to view the attributes for a page,
        /// such as the page's title, whether it is a home page, or whether the
        /// page has been modified.
        /// Normally the only child of this control is a grouped object that contains
        /// the contents of the associated page.
        /// </devdoc>
        PageTab = 0x25,

        /// <devdoc>
        /// A property page, which is a dialog box that controls the appearance
        /// and the behavior of an object, such as a file or resource. A property
        /// page's appearance differs according to its purpose.
        /// </devdoc>
        PropertyPage = 0x26,

        /// <devdoc>
        /// An indicator, such as a pointer graphic, that points to the current item.
        /// </devdoc>
        Indicator = 0x27,

        /// <devdoc>
        /// A picture.
        /// </devdoc>
        Graphic = 0x28,

        /// <devdoc>
        /// Read-only text, such as in a label, for other controls or instructions
        /// in a dialog box. Static text cannot be modified or selected.
        /// </devdoc>
        StaticText = 0x29,

        /// <devdoc>
        /// Selectable text that can be editable or read-only.
        /// </devdoc>
        Text = 0x2a,

        /// <devdoc>
        /// A push button control, which is a small rectangular control that a user
        /// can turn on or off. A push button, also known as a command button, has
        /// a raised appearance in its default off state and a sunken appearance
        /// when it is turned on.
        /// </devdoc>
        PushButton = 0x2b,

        /// <devdoc>
        /// A check box control, which is an option that can be turned on or off
        /// independently of other options.
        /// </devdoc>
        CheckButton = 0x2c,

        /// <devdoc>
        /// An option button, also known as a radio button. All objects sharing the
        /// same parent that have this attribute are assumed to be part of a single
        /// mutually exclusive group. You can use grouped objects to divide option
        /// buttons into separate groups when necessary.
        /// </devdoc>
        RadioButton = 0x2d,

        /// <devdoc>
        /// A combo box, which is an edit control with an associated list box that
        /// provides a set of predefined choices.
        /// </devdoc>
        ComboBox = 0x2e,

        /// <devdoc>
        /// A drop-down list box. This control shows one item and allows the user
        /// to display and select another from a list of alternative choices.
        /// </devdoc>
        DropList = 0x2f,

        /// <devdoc>
        /// A progress bar, which indicates the progress of a lengthy operation by
        /// displaying a colored bar inside a horizontal rectangle. The length of
        /// the bar in relation to the length of the rectangle corresponds to
        /// the percentage of the operation that is complete. This control does
        /// not take user input.
        /// </devdoc>
        ProgressBar = 0x30,

        /// <devdoc>
        /// A dial or knob. This can also be a read-only object, like a speedometer.
        /// </devdoc>
        Dial = 0x31,

        /// <devdoc>
        /// A hot-key field that allows the user to enter a combination or sequence
        /// of keystrokes to be used as a hot key, which enables users to perform
        /// an action quickly. A hot-key control displays the keystrokes entered
        /// by the user and ensures that the user selects a valid key combination.
        /// </devdoc>
        HotkeyField = 0x32,

        /// <devdoc>
        /// A control, sometimes called a trackbar, that allows a user to adjust
        /// a setting in given increments between minimum and maximum values by
        /// moving a slider. The volume controls in the Windows operating system
        /// are slider controls.
        /// </devdoc>
        Slider = 0x33,

        /// <devdoc>
        /// A spin box, also known as an up-down control, which contains a pair
        /// of arrow buttons that a user click with a mouse to increment or
        /// decrement a value. A spin button control is most often used with a
        /// companion control, called a buddy window, where the current value is
        /// displayed.
        /// </devdoc>
        SpinButton = 0x34,

        /// <devdoc>
        /// A graphical image used to diagram data.
        /// </devdoc>
        Diagram = 0x35,

        /// <devdoc>
        /// An animation control, which contains content that is changing over
        /// time, such as a control that displays a series of bitmap frames, like
        /// a film strip. Animation controls are usually displayed when files
        /// are being copied, or when some other time-consuming task is being
        /// performed.
        /// </devdoc>
        Animation = 0x36,

        /// <devdoc>
        /// A mathematical equation.
        /// </devdoc>
        Equation = 0x37,

        /// <devdoc>
        /// A button that drops down a list of items.
        /// </devdoc>
        ButtonDropDown = 0x38,

        /// <devdoc>
        /// A button that drops down a menu.
        /// </devdoc>
        ButtonMenu = 0x39,

        /// <devdoc>
        /// A button that drops down a grid.
        /// </devdoc>
        ButtonDropDownGrid = 0x3a,

        /// <devdoc>
        /// A blank space between other objects.
        /// </devdoc>
        WhiteSpace = 0x3b,

        /// <devdoc>
        /// A container of page tab controls.
        /// </devdoc>
        PageTabList = 0x3c,

        /// <devdoc>
        /// A control that displays the time.
        /// </devdoc>
        Clock = 0x3d,

        /// <devdoc>
        /// A toolbar button that jas a drop-down list icon directly adjacent to
        /// the button.
        /// </devdoc>
        SplitButton = 0x3e,

        /// <devdoc>
        /// A control designed for entering Internet Protocol (IP) addresses.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase")]
        IpAddress = 0x3f,

        /// <devdoc>
        /// A control that navigates like an outline item.
        /// </devdoc>
        OutlineButton = 0x40,
    }
}
