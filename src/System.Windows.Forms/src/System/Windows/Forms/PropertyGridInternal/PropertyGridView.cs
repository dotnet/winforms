// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView :
        Control,
        IWin32Window,
        IWindowsFormsEditorService,
        IServiceProvider
    {
        protected static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);

        public const int RENDERMODE_LEFTDOT = 2;
        public const int RENDERMODE_BOLD = 3;
        public const int RENDERMODE_TRIANGLE = 4;

        public static int inheritRenderMode = RENDERMODE_BOLD;

        public static TraceSwitch GridViewDebugPaint = new TraceSwitch("GridViewDebugPaint", "PropertyGridView: Debug property painting");
        private const int LEFTDOT_SIZE = 4;
        // constants
        private const int EDIT_INDENT = 0;
        private const int OUTLINE_INDENT = 10;
        private const int OUTLINE_SIZE = 9;
        private const int OUTLINE_SIZE_EXPLORER_TREE_STYLE = 16;
        private int outlineSize = OUTLINE_SIZE;
        private int outlineSizeExplorerTreeStyle = OUTLINE_SIZE_EXPLORER_TREE_STYLE;
        private const int PAINT_WIDTH = 20;
        private int paintWidth = PAINT_WIDTH;
        private const int PAINT_INDENT = 26;
        private int paintIndent = PAINT_INDENT;
        private const int ROWLABEL = 1;
        private const int ROWVALUE = 2;
        private const int MAX_LISTBOX_HEIGHT = 200;
        private int maxListBoxHeight = MAX_LISTBOX_HEIGHT;

        private const short ERROR_NONE = 0;
        private const short ERROR_THROWN = 1;
        private const short ERROR_MSGBOX_UP = 2;
        internal const short GDIPLUS_SPACE = 2;
        internal const int MaxRecurseExpand = 10;

        private const int DOTDOTDOT_ICONWIDTH = 7;
        private const int DOTDOTDOT_ICONHEIGHT = 8;
        private const int DOWNARROW_ICONWIDTH = 16;
        private const int DOWNARROW_ICONHEIGHT = 16;

        private const int OFFSET_2PIXELS = 2;
        private int offset_2Units = OFFSET_2PIXELS;

        protected static readonly Point InvalidPosition = new Point(int.MinValue, int.MinValue);

        // colors and fonts
        private Font _fontBold;
        private Color _grayTextColor;

        // for backwards compatibility of default colors
        private bool grayTextColorModified; // true if someone has set the grayTextColor property

        // property collections
        private GridEntryCollection topLevelGridEntries;     // top level props
        private GridEntryCollection allGridEntries;  // cache of viewable props

        // row information
        internal int totalProps = -1;        // # of viewable props
        private int visibleRows = -1;         // # of visible rows
        private int labelWidth = -1;
        public double labelRatio = 2; // ratio of whole row to label width

        private short requiredLabelPaintMargin = GDIPLUS_SPACE;

        // current selected row and tooltip.
        private int selectedRow = -1;
        private GridEntry selectedGridEntry;
        private int tipInfo = -1;

        // editors & controls
        private GridViewEdit edit;
        private DropDownButton btnDropDown;
        private DropDownButton btnDialog;
        private GridViewListBox listBox;
        private DropDownHolder dropDownHolder;
        private Rectangle lastClientRect = Rectangle.Empty;
        private Control currentEditor;
        private ScrollBar scrollBar;
        internal GridToolTip toolTip;
        private GridErrorDlg errorDlg;

        // flags
        private const short FlagNeedsRefresh = 0x0001;
        private const short FlagIsNewSelection = 0x0002;
        private const short FlagIsSplitterMove = 0x0004;
        private const short FlagIsSpecialKey = 0x0008;
        private const short FlagInPropertySet = 0x0010;
        private const short FlagDropDownClosing = 0x0020;
        private const short FlagDropDownCommit = 0x0040;
        private const short FlagNeedUpdateUIBasedOnFont = 0x0080;
        private const short FlagBtnLaunchedEditor = 0x0100;
        private const short FlagNoDefault = 0x0200;
        private const short FlagResizableDropDown = 0x0400;

        private short flags = FlagNeedsRefresh | FlagIsNewSelection | FlagNeedUpdateUIBasedOnFont;
        private short errorState = ERROR_NONE;

        private Point ptOurLocation = new Point(1, 1);

        private string originalTextValue;     // original text, in case of ESC
        private int cumulativeVerticalWheelDelta;
        private long rowSelectTime;
        private Point rowSelectPos = Point.Empty; // the position that we clicked on a row to test for double clicks
        private Point lastMouseDown = InvalidPosition;
        private int lastMouseMove;
        private GridEntry lastClickedEntry;

        private IServiceProvider serviceProvider;
        private IHelpService topHelpService;
        private IHelpService helpService;

        private readonly EventHandler ehValueClick;
        private readonly EventHandler ehLabelClick;
        private readonly EventHandler ehOutlineClick;
        private readonly EventHandler ehValueDblClick;
        private readonly EventHandler ehLabelDblClick;
        private readonly GridEntryRecreateChildrenEventHandler ehRecreateChildren;

        private int cachedRowHeight = -1;

        public PropertyGridView(IServiceProvider serviceProvider, PropertyGrid propertyGrid)
        : base()
        {
            if (DpiHelper.IsScalingRequired)
            {
                paintWidth = DpiHelper.LogicalToDeviceUnitsX(PAINT_WIDTH);
                paintIndent = DpiHelper.LogicalToDeviceUnitsX(PAINT_INDENT);
                outlineSizeExplorerTreeStyle = DpiHelper.LogicalToDeviceUnitsX(OUTLINE_SIZE_EXPLORER_TREE_STYLE);
                outlineSize = DpiHelper.LogicalToDeviceUnitsX(OUTLINE_SIZE);
                maxListBoxHeight = DpiHelper.LogicalToDeviceUnitsY(MAX_LISTBOX_HEIGHT);
            }

            ehValueClick = new EventHandler(OnGridEntryValueClick);
            ehLabelClick = new EventHandler(OnGridEntryLabelClick);
            ehOutlineClick = new EventHandler(OnGridEntryOutlineClick);
            ehValueDblClick = new EventHandler(OnGridEntryValueDoubleClick);
            ehLabelDblClick = new EventHandler(OnGridEntryLabelDoubleClick);
            ehRecreateChildren = new GridEntryRecreateChildrenEventHandler(OnRecreateChildren);

            OwnerGrid = propertyGrid;
            this.serviceProvider = serviceProvider;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.UserMouse, true);

            // properties
            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;
            _grayTextColor = SystemColors.GrayText;
            TabStop = true;

            Text = "PropertyGridView";

            CreateUI();
            LayoutWindow(true);
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanCopy
        {
            get
            {
                if (selectedGridEntry is null)
                {
                    return false;
                }

                if (!Edit.Focused)
                {
                    string val = selectedGridEntry.GetPropertyTextValue();

                    return val != null && val.Length > 0;
                }

                return true;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanCut
        {
            get
            {
                return CanCopy && selectedGridEntry != null && selectedGridEntry.IsTextEditable;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanPaste
        {
            get
            {
                return selectedGridEntry != null && selectedGridEntry.IsTextEditable; // return gridView.CanPaste;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanUndo
        {
            get
            {
                if (!Edit.Visible || !Edit.Focused)
                {
                    return false;
                }

                return User32.SendMessageW(Edit, (User32.WM)User32.EM.CANUNDO) != IntPtr.Zero;
            }
        }

        internal DropDownButton DropDownButton
        {
            get
            {
                if (btnDropDown is null)
                {
#if DEBUG
                    if (OwnerGrid.inGridViewCreate)
                    {
                        throw new Exception("PERF REGRESSION - Creating item in grid view create");
                    }
#endif

                    btnDropDown = new DropDownButton
                    {
                        UseComboBoxTheme = true
                    };
                    Bitmap bitmap = CreateResizedBitmap("Arrow", DOWNARROW_ICONWIDTH, DOWNARROW_ICONHEIGHT);
                    btnDropDown.Image = bitmap;
                    btnDropDown.BackColor = SystemColors.Control;
                    btnDropDown.ForeColor = SystemColors.ControlText;
                    btnDropDown.Click += new EventHandler(OnBtnClick);
                    btnDropDown.GotFocus += new EventHandler(OnDropDownButtonGotFocus);
                    btnDropDown.LostFocus += new EventHandler(OnChildLostFocus);
                    btnDropDown.TabIndex = 2;
                    CommonEditorSetup(btnDropDown);
                    btnDropDown.Size = DpiHelper.IsScalingRequirementMet ? new Size(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight) : new Size(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);
                }
                return btnDropDown;
            }
        }

        private Button DialogButton
        {
            get
            {
                if (btnDialog is null)
                {
#if DEBUG
                    if (OwnerGrid.inGridViewCreate)
                    {
                        throw new Exception("PERF REGRESSION - Creating item in grid view create");
                    }
#endif
                    btnDialog = new DropDownButton
                    {
                        BackColor = SystemColors.Control,
                        ForeColor = SystemColors.ControlText,
                        TabIndex = 3,
                        Image = CreateResizedBitmap("dotdotdot", DOTDOTDOT_ICONWIDTH, DOTDOTDOT_ICONHEIGHT)
                    };
                    btnDialog.Click += new EventHandler(OnBtnClick);
                    btnDialog.KeyDown += new KeyEventHandler(OnBtnKeyDown);
                    btnDialog.GotFocus += new EventHandler(OnDropDownButtonGotFocus);
                    btnDialog.LostFocus += new EventHandler(OnChildLostFocus);
                    btnDialog.Size = DpiHelper.IsScalingRequirementMet ? new Size(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight) : new Size(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);
                    CommonEditorSetup(btnDialog);
                }
                return btnDialog;
            }
        }

        private static Bitmap GetBitmapFromIcon(string iconName, int iconsWidth, int iconsHeight)
        {
            Size desiredSize = new Size(iconsWidth, iconsHeight);
            Icon icon = new Icon(new Icon(typeof(PropertyGrid), iconName), desiredSize);
            Bitmap b = icon.ToBitmap();
            icon.Dispose();

            if ((b.Size.Width != iconsWidth || b.Size.Height != iconsHeight))
            {
                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(b, desiredSize);
                if (scaledBitmap != null)
                {
                    b.Dispose();
                    b = scaledBitmap;
                }
            }
            return b;
        }

        private GridViewEdit Edit
        {
            get
            {
                if (edit is null)
                {
#if DEBUG
                    if (OwnerGrid.inGridViewCreate)
                    {
                        throw new Exception("PERF REGRESSION - Creating item in grid view create");
                    }
#endif

                    edit = new GridViewEdit(this)
                    {
                        BorderStyle = BorderStyle.None,
                        AutoSize = false,
                        TabStop = false,
                        AcceptsReturn = true,
                        BackColor = BackColor,
                        ForeColor = ForeColor
                    };
                    edit.KeyDown += new KeyEventHandler(OnEditKeyDown);
                    edit.KeyPress += new KeyPressEventHandler(OnEditKeyPress);
                    edit.GotFocus += new EventHandler(OnEditGotFocus);
                    edit.LostFocus += new EventHandler(OnEditLostFocus);
                    edit.MouseDown += new MouseEventHandler(OnEditMouseDown);
                    edit.TextChanged += new EventHandler(OnEditChange);
                    //edit.ImeModeChanged += new EventHandler(this.OnEditImeModeChanged);
                    edit.TabIndex = 1;
                    CommonEditorSetup(edit);
                }
                return edit;
            }
        }

        /// <summary>
        ///  Represents the Editor's control accessible object.
        /// </summary>
        internal AccessibleObject EditAccessibleObject
        {
            get
            {
                return Edit.AccessibilityObject;
            }
        }

        internal GridViewListBox DropDownListBox
        {
            get
            {
                if (listBox is null)
                {
#if DEBUG
                    if (OwnerGrid.inGridViewCreate)
                    {
                        throw new Exception("PERF REGRESSION - Creating item in grid view create");
                    }
#endif

                    listBox = new GridViewListBox(this)
                    {
                        DrawMode = DrawMode.OwnerDrawFixed
                    };
                    //listBox.Click += new EventHandler(this.OnListClick);
                    listBox.MouseUp += new MouseEventHandler(OnListMouseUp);
                    listBox.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                    listBox.SelectedIndexChanged += new EventHandler(OnListChange);
                    listBox.KeyDown += new KeyEventHandler(OnListKeyDown);
                    listBox.LostFocus += new EventHandler(OnChildLostFocus);
                    listBox.Visible = true;
                    listBox.ItemHeight = RowHeight;
                }
                return listBox;
            }
        }

        /// <summary>
        ///  Represents the DropDownListBox accessible object.
        /// </summary>
        internal AccessibleObject DropDownListBoxAccessibleObject
        {
            get
            {
                if (DropDownListBox.Visible)
                {
                    return DropDownListBox.AccessibilityObject;
                }

                return null;
            }
        }

        internal bool DrawValuesRightToLeft
        {
            get
            {
                if (edit != null && edit.IsHandleCreated)
                {
                    int exStyle = unchecked((int)((long)User32.GetWindowLong(edit, User32.GWL.EXSTYLE)));
                    return (exStyle & (int)User32.WS_EX.RTLREADING) != 0;
                }
                else
                {
                    return false;
                }
            }
        }

        internal DropDownHolder DropDownControlHolder => dropDownHolder;

        internal bool DropDownVisible
        {
            get
            {
                return dropDownHolder != null && dropDownHolder.Visible;
            }
        }

        public bool FocusInside
        {
            get
            {
                return (ContainsFocus || (dropDownHolder != null && dropDownHolder.ContainsFocus));
            }
        }

        internal Color GrayTextColor
        {
            get
            {
                // if changed from the default, then the set value is returned
                if (grayTextColorModified)
                {
                    return _grayTextColor;
                }

                if (ForeColor.ToArgb() == SystemColors.WindowText.ToArgb())
                {
                    return SystemColors.GrayText;
                }

                // compute the new color by halving the value of the old one.
                //
                int colorRGB = ForeColor.ToArgb();

                int alphaValue = (colorRGB >> 24) & 0xff;
                if (alphaValue != 0)
                {
                    alphaValue /= 2;
                    colorRGB &= 0xFFFFFF;
                    colorRGB |= (int)((alphaValue << 24) & 0xFF000000);
                }
                else
                {
                    colorRGB /= 2;
                }
                return Color.FromArgb(colorRGB);
            }
            set
            {
                _grayTextColor = value;
                grayTextColorModified = true;
            }
        }

        //   This dialog's width is defined by the summary message
        //   in the top pane. We don't restrict dialog width in any way.
        //   Use caution and check at all DPI scaling factors if adding a new message
        //   to be displayed in the top pane.
        private GridErrorDlg ErrorDialog
        {
            get
            {
                if (errorDlg is null)
                {
                    errorDlg = new GridErrorDlg(OwnerGrid);
                }
                return errorDlg;
            }
        }

        private bool HasEntries
        {
            get
            {
                return topLevelGridEntries != null && topLevelGridEntries.Count > 0;
            }
        }

        protected int InternalLabelWidth
        {
            get
            {
                if (GetFlag(FlagNeedUpdateUIBasedOnFont))
                {
                    UpdateUIBasedOnFont(true);
                }
                if (labelWidth == -1)
                {
                    SetConstants();
                }
                return labelWidth;
            }
        }

        internal int LabelPaintMargin
        {
            set
            {
                requiredLabelPaintMargin = (short)Math.Max(Math.Max(value, requiredLabelPaintMargin), GDIPLUS_SPACE);
            }
        }

        protected bool NeedsCommit
        {
            get
            {
                string text;

                if (edit is null || !Edit.Visible)
                {
                    return false;
                }

                text = Edit.Text;

                if (((text is null || text.Length == 0) && (originalTextValue is null || originalTextValue.Length == 0)) ||
                    (text != null && originalTextValue != null && text.Equals(originalTextValue)))
                {
                    return false;
                }
                return true;
            }
        }

        public PropertyGrid OwnerGrid { get; private set; }

        protected int RowHeight
        {
            get
            {
                if (cachedRowHeight == -1)
                {
                    cachedRowHeight = (int)Font.Height + 2;
                }
                return cachedRowHeight;
            }
        }

        /// <summary>
        ///  Returns a default location for showing the context menu.  This
        ///  location is the center of the active property label in the grid, and
        ///  is used useful to position the context menu when the menu is invoked
        ///  via the keyboard.
        /// </summary>
        public Point ContextMenuDefaultLocation
        {
            get
            {
                // get the rect for the currently selected prop name, find the middle
                Rectangle rect = GetRectangle(selectedRow, ROWLABEL);
                Point pt = PointToScreen(new Point(rect.X, rect.Y));
                return new Point(pt.X + (rect.Width / 2), pt.Y + (rect.Height / 2));
            }
        }

        private ScrollBar ScrollBar
        {
            get
            {
                if (scrollBar is null)
                {
#if DEBUG
                    if (OwnerGrid.inGridViewCreate)
                    {
                        throw new Exception("PERF REGRESSION - Creating item in grid view create");
                    }
#endif
                    scrollBar = new VScrollBar();
                    scrollBar.Scroll += new ScrollEventHandler(OnScroll);
                    Controls.Add(scrollBar);
                }
                return scrollBar;
            }
        }

        internal GridEntry SelectedGridEntry
        {
            get
            {
                return selectedGridEntry;
            }
            set
            {
                if (allGridEntries != null)
                {
                    foreach (GridEntry e in allGridEntries)
                    {
                        if (e == value)
                        {
                            SelectGridEntry(value, true);
                            return;
                        }
                    }
                }

                GridEntry gr = FindEquivalentGridEntry(new GridEntryCollection(null, new GridEntry[] { value }));

                if (gr != null)
                {
                    SelectGridEntry(gr, true);
                    return;
                }

                throw new ArgumentException(SR.PropertyGridInvalidGridEntry);
            }
        }

        /// <summary>
        ///  Returns or sets the IServiceProvider the PropertyGridView will use to obtain
        ///  services.  This may be null.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get
            {
                return serviceProvider;
            }
            set
            {
                if (value != serviceProvider)
                {
                    serviceProvider = value;

                    topHelpService = null;

                    if (helpService != null && helpService is IDisposable)
                    {
                        ((IDisposable)helpService).Dispose();
                    }

                    helpService = null;
                }
            }
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        private int TipColumn
        {
            get
            {
                return (tipInfo & unchecked((int)0xFFFF0000)) >> 16;
            }
            set
            {
                // clear the column
                tipInfo &= 0xFFFF;

                // set the row
                tipInfo |= ((value & 0xFFFF) << 16);
            }
        }

        private int TipRow
        {
            get
            {
                return tipInfo & 0xFFFF;
            }
            set
            {
                // clear the row
                tipInfo &= unchecked((int)0xFFFF0000);

                // set the row
                tipInfo |= (value & 0xFFFF);
            }
        }

        private GridToolTip ToolTip
        {
            get
            {
                if (toolTip is null)
                {
#if DEBUG
                    if (OwnerGrid.inGridViewCreate)
                    {
                        throw new Exception("PERF REGRESSION - Creating item in grid view create");
                    }
#endif
                    toolTip = new GridToolTip(new Control[] { this, Edit })
                    {
                        ToolTip = string.Empty,
                        Font = Font
                    };
                }
                return toolTip;
            }
        }

        /// <summary>
        ///  Gets the top level grid entries.
        /// </summary>
        internal GridEntryCollection TopLevelGridEntries
        {
            get
            {
                return topLevelGridEntries;
            }
        }

        internal GridEntryCollection AccessibilityGetGridEntries()
        {
            return GetAllGridEntries();
        }

        internal Rectangle AccessibilityGetGridEntryBounds(GridEntry gridEntry)
        {
            int row = GetRowFromGridEntry(gridEntry);
            if (row < 0)
            {
                return Rectangle.Empty;
            }

            Rectangle rect = GetRectangle(row, ROWVALUE | ROWLABEL);

            // Translate rect to screen coordinates
            var pt = new Point(rect.X, rect.Y);
            User32.ClientToScreen(new HandleRef(this, Handle), ref pt);

            Rectangle parent = gridEntry.OwnerGrid.GridViewAccessibleObject.Bounds;

            int propertyGridViewBottom = parent.Bottom - 1; // - 1 is PropertyGridView bottom border

            if (pt.Y > propertyGridViewBottom)
            {
                return Rectangle.Empty;
            }

            if (pt.Y + rect.Height > propertyGridViewBottom)
            {
                rect.Height = propertyGridViewBottom - pt.Y;
            }

            return new Rectangle(pt.X, pt.Y, rect.Width, rect.Height);
        }

        internal int AccessibilityGetGridEntryChildID(GridEntry gridEntry)
        {
            GridEntryCollection ipes = GetAllGridEntries();

            if (ipes is null)
            {
                return -1;
            }

            // Find the grid entry and return its ID
            //
            for (int index = 0; index < ipes.Count; ++index)
            {
                if (ipes[index].Equals(gridEntry))
                {
                    return index;
                }
            }

            return -1;
        }

        internal void AccessibilitySelect(GridEntry entry)
        {
            SelectGridEntry(entry, true);
            Focus();
        }

        private void AddGridEntryEvents(GridEntryCollection ipeArray, int startIndex, int count)
        {
            if (ipeArray is null)
            {
                return;
            }

            if (count == -1)
            {
                count = ipeArray.Count - startIndex;
            }

            for (int i = startIndex; i < (startIndex + count); i++)
            {
                if (ipeArray[i] != null)
                {
                    GridEntry ge = ipeArray.GetEntry(i);
                    ge.AddOnValueClick(ehValueClick);
                    ge.AddOnLabelClick(ehLabelClick);
                    ge.AddOnOutlineClick(ehOutlineClick);
                    ge.AddOnOutlineDoubleClick(ehOutlineClick);
                    ge.AddOnValueDoubleClick(ehValueDblClick);
                    ge.AddOnLabelDoubleClick(ehLabelDblClick);
                    ge.AddOnRecreateChildren(ehRecreateChildren);
                }
            }
        }

        protected virtual void AdjustOrigin(Graphics g, Point newOrigin, ref Rectangle r)
        {
            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Adjusting paint origin to (" + newOrigin.X.ToString(CultureInfo.InvariantCulture) + "," + newOrigin.Y.ToString(CultureInfo.InvariantCulture) + ")");

            g.ResetTransform();
            g.TranslateTransform(newOrigin.X, newOrigin.Y);
            r.Offset(-newOrigin.X, -newOrigin.Y);
        }

        private void CancelSplitterMove()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CancelSplitterMove");
            if (GetFlag(FlagIsSplitterMove))
            {
                SetFlag(FlagIsSplitterMove, false);
                Capture = false;

                if (selectedRow != -1)
                {
                    SelectRow(selectedRow);
                }
            }
        }

        internal GridPositionData CaptureGridPositionData()
        {
            return new GridPositionData(this);
        }

        private void ClearGridEntryEvents(GridEntryCollection ipeArray, int startIndex, int count)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ClearGridEntryEvents");
            if (ipeArray is null)
            {
                return;
            }

            if (count == -1)
            {
                count = ipeArray.Count - startIndex;
            }

            for (int i = startIndex; i < (startIndex + count); i++)
            {
                if (ipeArray[i] != null)
                {
                    GridEntry ge = ipeArray.GetEntry(i);
                    ge.RemoveOnValueClick(ehValueClick);
                    ge.RemoveOnLabelClick(ehLabelClick);
                    ge.RemoveOnOutlineClick(ehOutlineClick);
                    ge.RemoveOnOutlineDoubleClick(ehOutlineClick);
                    ge.RemoveOnValueDoubleClick(ehValueDblClick);
                    ge.RemoveOnLabelDoubleClick(ehLabelDblClick);
                    ge.RemoveOnRecreateChildren(ehRecreateChildren);
                }
            }
        }

        public void ClearProps()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ClearProps");

            if (!HasEntries)
            {
                return;
            }

            CommonEditorHide();
            topLevelGridEntries = null;
            ClearGridEntryEvents(allGridEntries, 0, -1);
            allGridEntries = null;
            selectedRow = -1;
            // Don't clear selectedGridEntry because then we can't save where we were on a Refresh()
            tipInfo = -1;
        }

        /// <summary>
        ///  Closes a previously opened drop down.  This should be called by the
        ///  drop down when the user does something that should close it.
        /// </summary>
        public void /* IWindowsFormsEditorService. */ CloseDropDown()
        {
            CloseDropDownInternal(true);
        }

        private void CloseDropDownInternal(bool resetFocus)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CloseDropDown");

            // the activation code in the DropDownHolder can cause this to recurse...

            if (GetFlag(FlagDropDownClosing))
            {
                return;
            }
            try
            {
                SetFlag(FlagDropDownClosing, true);
                if (dropDownHolder != null && dropDownHolder.Visible)
                {
                    if (dropDownHolder.Component == DropDownListBox && GetFlag(FlagDropDownCommit))
                    {
                        OnListClick(null, null);
                    }

                    Edit.Filter = false;

                    // disable the ddh so it wont' steal the focus back
                    //
                    dropDownHolder.SetComponent(null, false);
                    dropDownHolder.Visible = false;

                    // when we disable the dropdown holder, focus will be lost,
                    // so put it onto one of our children first.
                    if (resetFocus)
                    {
                        if (DialogButton.Visible)
                        {
                            DialogButton.Focus();
                        }
                        else if (DropDownButton.Visible)
                        {
                            DropDownButton.Focus();
                        }
                        else if (Edit.Visible)
                        {
                            Edit.Focus();
                        }
                        else
                        {
                            Focus();
                        }

                        if (selectedRow != -1)
                        {
                            SelectRow(selectedRow);
                        }
                    }

                    if (selectedRow != -1)
                    {
                        var gridEntry = GetGridEntryFromRow(selectedRow);
                        if (gridEntry != null)
                        {
                            gridEntry.AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                            gridEntry.AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                                UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                                UiaCore.ExpandCollapseState.Expanded,
                                UiaCore.ExpandCollapseState.Collapsed);
                        }
                    }
                }
            }
            finally
            {
                SetFlag(FlagDropDownClosing, false);
            }
        }

        private void CommonEditorHide()
        {
            CommonEditorHide(false);
        }

        private void CommonEditorHide(bool always)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommonEditorHide");

            if (!always && !HasEntries)
            {
                return;
            }

            CloseDropDown();

            bool gotfocus = false;

            if (Edit.Focused || DialogButton.Focused || DropDownButton.Focused)
            {
                if (IsHandleCreated && Visible && Enabled)
                {
                    gotfocus = IntPtr.Zero != User32.SetFocus(new HandleRef(this, Handle));
                }
            }

            try
            {
                // We do this because the Focus call above doesn't always stick, so
                // we make the Edit think that it doesn't have focus.  this prevents
                // ActiveControl code on the containercontrol from moving focus elsewhere
                // when the dropdown closes.
                Edit.DontFocus = true;
                if (Edit.Focused && !gotfocus)
                {
                    gotfocus = Focus();
                }
                Edit.Visible = false;

                Edit.SelectionStart = 0;
                Edit.SelectionLength = 0;

                if (DialogButton.Focused && !gotfocus)
                {
                    gotfocus = Focus();
                }
                DialogButton.Visible = false;

                if (DropDownButton.Focused && !gotfocus)
                {
                    gotfocus = Focus();
                }
                DropDownButton.Visible = false;
                currentEditor = null;
            }
            finally
            {
                Edit.DontFocus = false;
            }
        }

        protected virtual void CommonEditorSetup(Control ctl)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommonEditorSetup");
            ctl.Visible = false;
            Controls.Add(ctl);
        }

        protected virtual void CommonEditorUse(Control ctl, Rectangle rectTarget)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommonEditorUse");
            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Showing common editors");

            Debug.Assert(ctl != null, "Null control passed to CommonEditorUse");

            Rectangle rectCur = ctl.Bounds;

            // the client rect minus the border line
            Rectangle clientRect = ClientRectangle;

            clientRect.Inflate(-1, -1);

            try
            {
                rectTarget = Rectangle.Intersect(clientRect, rectTarget);
                //if (ctl is Button)
                //   Debug.WriteStackTrace();

                if (!rectTarget.IsEmpty)
                {
                    if (!rectTarget.Equals(rectCur))
                    {
                        ctl.SetBounds(rectTarget.X, rectTarget.Y,
                                      rectTarget.Width, rectTarget.Height);
                    }
                    ctl.Visible = true;
                }
            }
            catch
            {
                rectTarget = Rectangle.Empty;
            }

            if (rectTarget.IsEmpty)
            {
                ctl.Visible = false;
            }

            currentEditor = ctl;
        }

        private /*protected virtual*/ int CountPropsFromOutline(GridEntryCollection rgipes)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CountPropsFromOutLine");
            if (rgipes is null)
            {
                return 0;
            }

            int cProps = rgipes.Count;
            for (int i = 0; i < rgipes.Count; i++)
            {
                if (((GridEntry)rgipes[i]).InternalExpanded)
                {
                    cProps += CountPropsFromOutline(((GridEntry)rgipes[i]).Children);
                }
            }
            return cProps;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new PropertyGridViewAccessibleObject(this, OwnerGrid);
        }

        private Bitmap CreateResizedBitmap(string icon, int width, int height)
        {
            Bitmap bitmap = null;
            var scaledIconWidth = width;
            var scaledIconHeight = height;
            try
            {
                //scale for per-monitor DPI.
                if (DpiHelper.IsPerMonitorV2Awareness)
                {
                    scaledIconWidth = LogicalToDeviceUnits(width);
                    scaledIconHeight = LogicalToDeviceUnits(height);
                }
                else if (DpiHelper.IsScalingRequired)
                {
                    // only primary monitor scaling.
                    scaledIconWidth = DpiHelper.LogicalToDeviceUnitsX(width);
                    scaledIconHeight = DpiHelper.LogicalToDeviceUnitsY(height);
                }

                bitmap = GetBitmapFromIcon(icon, scaledIconWidth, scaledIconHeight);
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
                bitmap = new Bitmap(scaledIconWidth, scaledIconHeight);
            }
            return bitmap;
        }

        protected virtual void CreateUI()
        {
            UpdateUIBasedOnFont(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Dispose");
                if (scrollBar != null)
                {
                    scrollBar.Dispose();
                }

                if (listBox != null)
                {
                    listBox.Dispose();
                }

                if (dropDownHolder != null)
                {
                    dropDownHolder.Dispose();
                }

                scrollBar = null;
                listBox = null;
                dropDownHolder = null;

                OwnerGrid = null;
                topLevelGridEntries = null;
                allGridEntries = null;
                serviceProvider = null;

                topHelpService = null;

                if (helpService != null && helpService is IDisposable)
                {
                    ((IDisposable)helpService).Dispose();
                }

                helpService = null;

                if (edit != null)
                {
                    edit.Dispose();
                    edit = null;
                }

                if (_fontBold != null)
                {
                    _fontBold.Dispose();
                    _fontBold = null;
                }

                if (btnDropDown != null)
                {
                    btnDropDown.Dispose();
                    btnDropDown = null;
                }

                if (btnDialog != null)
                {
                    btnDialog.Dispose();
                    btnDialog = null;
                }

                if (toolTip != null)
                {
                    toolTip.Dispose();
                    toolTip = null;
                }
            }

            base.Dispose(disposing);
        }

        public void DoCopyCommand()
        {
            if (CanCopy)
            {
                if (Edit.Focused)
                {
                    Edit.Copy();
                }
                else if (selectedGridEntry != null)
                {
                    Clipboard.SetDataObject(selectedGridEntry.GetPropertyTextValue());
                }
            }
        }

        public void DoCutCommand()
        {
            if (CanCut)
            {
                DoCopyCommand();
                if (Edit.Visible)
                {
                    Edit.Cut();
                }
            }
        }

        public void DoPasteCommand()
        {
            if (CanPaste && Edit.Visible)
            {
                if (Edit.Focused)
                {
                    Edit.Paste();
                }
                else
                {
                    IDataObject dataObj = Clipboard.GetDataObject();
                    if (dataObj != null)
                    {
                        string data = (string)dataObj.GetData(typeof(string));
                        if (data != null)
                        {
                            Edit.Focus();
                            Edit.Text = data;
                            SetCommitError(ERROR_NONE, true);
                        }
                    }
                }
            }
        }

        public void DoUndoCommand()
        {
            if (CanUndo && Edit.Visible)
            {
                User32.SendMessageW(Edit, User32.WM.UNDO);
            }
        }

        internal void DumpPropsToConsole(GridEntry entry, string prefix)
        {
            Type propType = entry.PropertyType;

            if (entry.PropertyValue != null)
            {
                propType = entry.PropertyValue.GetType();
            }

            System.Console.WriteLine(prefix + entry.PropertyLabel + ", value type=" + (propType is null ? "(null)" : propType.FullName) + ", value=" + (entry.PropertyValue is null ? "(null)" : entry.PropertyValue.ToString()) +
                                     ", flags=" + entry.Flags.ToString(CultureInfo.InvariantCulture) +
                                     ", TypeConverter=" + (entry.TypeConverter is null ? "(null)" : entry.TypeConverter.GetType().FullName) + ", UITypeEditor=" + ((entry.UITypeEditor is null ? "(null)" : entry.UITypeEditor.GetType().FullName)));
            GridEntryCollection children = entry.Children;

            if (children != null)
            {
                foreach (GridEntry g in children)
                {
                    DumpPropsToConsole(g, prefix + "\t");
                }
            }
        }

        private int GetIPELabelIndent(GridEntry gridEntry)
        {
            return gridEntry.PropertyLabelIndent + 1;
        }

        private int GetIPELabelLength(Graphics g, GridEntry gridEntry)
        {
            SizeF sizeF = PropertyGrid.MeasureTextHelper.MeasureText(OwnerGrid, g, gridEntry.PropertyLabel, Font);
            Size size = Size.Ceiling(sizeF);
            return ptOurLocation.X + GetIPELabelIndent(gridEntry) + size.Width;
        }

        private bool IsIPELabelLong(Graphics g, GridEntry gridEntry)
        {
            if (gridEntry is null)
            {
                return false;
            }

            int length = GetIPELabelLength(g, gridEntry);
            return (length > ptOurLocation.X + InternalLabelWidth);
        }

        protected virtual void DrawLabel(Graphics g, int row, Rectangle rect, bool selected, bool fLongLabelRequest, ref Rectangle clipRect)
        {
            GridEntry gridEntry = GetGridEntryFromRow(row);

            if (gridEntry is null || rect.IsEmpty)
            {
                return;
            }

            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Drawing label for property " + gridEntry.PropertyLabel);

            Point newOrigin = new Point(rect.X, rect.Y);
            Rectangle cr = Rectangle.Intersect(rect, clipRect);

            if (cr.IsEmpty)
            {
                return;
            }

            AdjustOrigin(g, newOrigin, ref rect);
            cr.Offset(-newOrigin.X, -newOrigin.Y);

            try
            {
                try
                {
                    bool fLongLabel = false;
                    int labelEnd = 0;
                    int labelIndent = GetIPELabelIndent(gridEntry);

                    if (fLongLabelRequest)
                    {
                        labelEnd = GetIPELabelLength(g, gridEntry);
                        fLongLabel = IsIPELabelLong(g, gridEntry);
                    }

                    gridEntry.PaintLabel(g, rect, cr, selected, fLongLabel);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }
            finally
            {
                ResetOrigin(g);
            }
        }

        protected virtual void DrawValueEntry(Graphics g, int row, ref Rectangle clipRect)
        {
            GridEntry gridEntry = GetGridEntryFromRow(row);
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Drawing value for property " + gridEntry.PropertyLabel);

            Rectangle r = GetRectangle(row, ROWVALUE);
            Point newOrigin = new Point(r.X, r.Y);
            Rectangle cr = Rectangle.Intersect(clipRect, r);

            if (cr.IsEmpty)
            {
                return;
            }

            AdjustOrigin(g, newOrigin, ref r);
            cr.Offset(-newOrigin.X, -newOrigin.Y);

            try
            {
                try
                {
                    gridEntry.PaintValue(
                        null,
                        g,
                        r,
                        cr,
                        GridEntry.PaintValueFlags.FetchValue
                            | GridEntry.PaintValueFlags.PaintInPlace
                            | GridEntry.PaintValueFlags.CheckShouldSerialize);
                }
                catch
                {
                }
            }
            finally
            {
                ResetOrigin(g);
            }
        }

        private void F4Selection(bool popupModalDialog)
        {
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            // if we are in an errorState, just put the focus back on the Edit
            if (errorState != ERROR_NONE && Edit.Visible)
            {
                Edit.Focus();
                return;
            }

            if (DropDownButton.Visible)
            {
                PopupDialog(selectedRow);
            }
            else if (DialogButton.Visible)
            {
                if (popupModalDialog)
                {
                    PopupDialog(selectedRow);
                }
                else
                {
                    DialogButton.Focus();
                }
            }
            else if (Edit.Visible)
            {
                Edit.Focus();
                SelectEdit(false);
            }
            return;
        }

        public void DoubleClickRow(int row, bool toggleExpand, int type)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:DoubleClickRow");
            GridEntry gridEntry = GetGridEntryFromRow(row);
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Property " + gridEntry.PropertyLabel + " double clicked");

            if (!toggleExpand || type == ROWVALUE)
            {
                try
                {
                    bool action = gridEntry.DoubleClickPropertyValue();
                    if (action)
                    {
                        SelectRow(row);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    SetCommitError(ERROR_THROWN);
                    ShowInvalidMessage(gridEntry.PropertyLabel, null, ex);
                    return;
                }
            }

            SelectGridEntry(gridEntry, true);

            if (type == ROWLABEL && toggleExpand && gridEntry.Expandable)
            {
                SetExpand(gridEntry, !gridEntry.InternalExpanded);
                return;
            }

            if (gridEntry.IsValueEditable && gridEntry.Enumerable)
            {
                int index = GetCurrentValueIndex(gridEntry);

                if (index != -1)
                {
                    object[] values = gridEntry.GetPropertyValueList();

                    if (values is null || index >= (values.Length - 1))
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }

                    CommitValue(values[index]);
                    SelectRow(selectedRow);
                    Refresh();
                    return;
                }
            }

            if (Edit.Visible)
            {
                Edit.Focus();
                SelectEdit(false);
                return;
            }
        }

        public Font GetBaseFont()
        {
            return Font;
        }

        public Font GetBoldFont()
        {
            if (_fontBold is null)
            {
                _fontBold = new Font(Font, FontStyle.Bold);
            }
            return _fontBold;
        }

        /// <summary>
        ///  Gets the element from point.
        /// </summary>
        /// <param name="x">The point x coordinate.</param>
        /// <param name="y">The point y coordinate.</param>
        /// <returns>The found grid element.</returns>
        internal GridEntry GetElementFromPoint(int x, int y)
        {
            Point point = new Point(x, y);
            GridEntryCollection allGridEntries = GetAllGridEntries();
            GridEntry[] targetEntries = new GridEntry[allGridEntries.Count];
            try
            {
                GetGridEntriesFromOutline(allGridEntries, 0, allGridEntries.Count - 1, targetEntries);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }

            foreach (GridEntry gridEntry in targetEntries)
            {
                if (gridEntry.AccessibilityObject.Bounds.Contains(point))
                {
                    return gridEntry;
                }
            }

            return null;
        }

        private bool GetFlag(short flag)
        {
            return (flags & flag) != 0;
        }

        public virtual Color GetLineColor()
        {
            return OwnerGrid.LineColor;
        }

        public virtual Color GetSelectedItemWithFocusForeColor()
        {
            return OwnerGrid.SelectedItemWithFocusForeColor;
        }

        public virtual Color GetSelectedItemWithFocusBackColor()
        {
            return OwnerGrid.SelectedItemWithFocusBackColor;
        }

        public virtual IntPtr GetHostHandle()
        {
            return Handle;
        }

        public virtual int GetLabelWidth()
        {
            return InternalLabelWidth;
        }

        internal bool IsExplorerTreeSupported
        {
            get => OwnerGrid.CanShowVisualStyleGlyphs && VisualStyleRenderer.IsSupported;
        }

        public virtual int GetOutlineIconSize()
        {
            if (IsExplorerTreeSupported)
            {
                return outlineSizeExplorerTreeStyle;
            }
            else
            {
                return outlineSize;
            }
        }

        public virtual int GetGridEntryHeight()
        {
            return RowHeight;
        }

        // for qa automation
        internal int GetPropertyLocation(string propName, bool getXY, bool rowValue)
        {
            if (allGridEntries != null && allGridEntries.Count > 0)
            {
                for (int i = 0; i < allGridEntries.Count; i++)
                {
                    if (0 == string.Compare(propName, allGridEntries.GetEntry(i).PropertyLabel, true, CultureInfo.InvariantCulture))
                    {
                        if (getXY)
                        {
                            int row = GetRowFromGridEntry(allGridEntries.GetEntry(i));

                            if (row < 0 || row >= visibleRows)
                            {
                                return -1;
                            }
                            else
                            {
                                Rectangle r = GetRectangle(row, rowValue ? ROWVALUE : ROWLABEL);
                                return PARAM.ToInt(r.X, r.Y);
                            }
                        }
                        else
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public new object GetService(Type classService)
        {
            if (classService == typeof(IWindowsFormsEditorService))
            {
                return this;
            }
            if (ServiceProvider != null)
            {
                return serviceProvider.GetService(classService);
            }
            return null;
        }

        public virtual int GetSplitterWidth()
        {
            return 1;
        }

        public virtual int GetTotalWidth()
        {
            return GetLabelWidth() + GetSplitterWidth() + GetValueWidth();
        }

        public virtual int GetValuePaintIndent()
        {
            return paintIndent;
        }

        public virtual int GetValuePaintWidth()
        {
            return paintWidth;
        }

        public virtual int GetValueStringIndent()
        {
            return EDIT_INDENT;
        }

        public virtual int GetValueWidth()
        {
            return (int)(InternalLabelWidth * (labelRatio - 1));
        }

        /// <summary>
        ///  Displays the provided control in a drop down.  When possible, the
        ///  current dimensions of the control will be respected.  If this is not possible
        ///  for the current screen layout the control may be resized, so it should
        ///  be implemented using appropriate docking and anchoring so it will resize
        ///  nicely.  If the user performs an action that would cause the drop down
        ///  to prematurely disappear the control will be hidden.
        /// </summary>
        public void /* cpr IWindowsFormsEditorService. */ DropDownControl(Control ctl)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:DropDownControl");
            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "DropDownControl(ctl = " + ctl.GetType().Name + ")");
            if (dropDownHolder is null)
            {
                dropDownHolder = new DropDownHolder(this);
            }

            dropDownHolder.Visible = false;
            dropDownHolder.SetComponent(ctl, GetFlag(FlagResizableDropDown));
            Rectangle rect = GetRectangle(selectedRow, ROWVALUE);
            Size size = dropDownHolder.Size;
            Point loc = PointToScreen(new Point(0, 0));
            Rectangle rectScreen = Screen.FromControl(Edit).WorkingArea;
            size.Width = Math.Max(rect.Width + 1, size.Width);

            // Not needed... CYMAXDDLHEIGHT used to be 200, but why limit it???
            //size.Height = Math.Min(size.Height,CYMAXDDLHEIGHT);

            loc.X = Math.Min(rectScreen.X + rectScreen.Width - size.Width,
                             Math.Max(rectScreen.X, loc.X + rect.X + rect.Width - size.Width));
            loc.Y += rect.Y;
            if (rectScreen.Y + rectScreen.Height < (size.Height + loc.Y + Edit.Height))
            {
                loc.Y -= size.Height;
                dropDownHolder.ResizeUp = true;
            }
            else
            {
                loc.Y += rect.Height + 1;
                dropDownHolder.ResizeUp = false;
            }

            // control is a top=level window. standard way of setparent on the control is prohibited for top-level controls.
            // It is unknown why this control was created as a top-level control. Windows does not recommend this way of setting parent.
            // We are not touching this for this relase. We may revisit it in next release.
            User32.SetWindowLong(dropDownHolder, User32.GWL.HWNDPARENT, new HandleRef(this, Handle));
            dropDownHolder.SetBounds(loc.X, loc.Y, size.Width, size.Height);
            User32.ShowWindow(dropDownHolder, User32.SW.SHOWNA);
            Edit.Filter = true;
            dropDownHolder.Visible = true;
            dropDownHolder.FocusComponent();
            SelectEdit(false);

            var gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry != null)
            {
                gridEntry.AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                gridEntry.AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                    UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                    UiaCore.ExpandCollapseState.Collapsed,
                    UiaCore.ExpandCollapseState.Expanded);
            }

            try
            {
                DropDownButton.IgnoreMouse = true;
                dropDownHolder.DoModalLoop();
            }
            finally
            {
                DropDownButton.IgnoreMouse = false;
            }

            if (selectedRow != -1)
            {
                Focus();
                SelectRow(selectedRow);
            }
        }

        public virtual void DropDownDone()
        {
            CloseDropDown();
        }

        public virtual void DropDownUpdate()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:DropDownUpdate");
            if (dropDownHolder != null && dropDownHolder.GetUsed())
            {
                int row = selectedRow;
                GridEntry gridEntry = GetGridEntryFromRow(row);
                Edit.Text = gridEntry.GetPropertyTextValue();
            }
        }

        public bool EnsurePendingChangesCommitted()
        {
            CloseDropDown();
            return Commit();
        }

        private bool FilterEditWndProc(ref Message m)
        {
            // if it's the TAB key, we keep it since we'll give them focus with it.
            if (dropDownHolder != null && dropDownHolder.Visible && m.Msg == (int)User32.WM.KEYDOWN && (int)m.WParam != (int)Keys.Tab)
            {
                Control ctl = dropDownHolder.Component;
                if (ctl != null)
                {
                    m.Result = User32.SendMessageW(ctl, (User32.WM)m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        private bool FilterReadOnlyEditKeyPress(char keyChar)
        {
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry.Enumerable && gridEntry.IsValueEditable)
            {
                int index = GetCurrentValueIndex(gridEntry);

                object[] values = gridEntry.GetPropertyValueList();
                string letter = new string(new char[] { keyChar });
                for (int i = 0; i < values.Length; i++)
                {
                    object valueCur = values[(i + index + 1) % values.Length];
                    string text = gridEntry.GetPropertyTextValue(valueCur);
                    if (text != null && text.Length > 0 && string.Compare(text.Substring(0, 1), letter, true, CultureInfo.InvariantCulture) == 0)
                    {
                        CommitValue(valueCur);
                        if (Edit.Focused)
                        {
                            SelectEdit(false);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool WillFilterKeyPress(char charPressed)
        {
            if (!Edit.Visible)
            {
                return false;
            }

            Keys modifiers = ModifierKeys;
            if ((int)(modifiers & ~Keys.Shift) != 0)
            {
                return false;
            }

            // try to activate the Edit.
            // we don't activate for +,-, or * on expandable items because they have special meaning
            // for the tree.
            //

            if (selectedGridEntry != null)
            {
                switch (charPressed)
                {
                    case '+':
                    case '-':
                    case '*':
                        return !selectedGridEntry.Expandable;
                    case unchecked((char)(int)(long)Keys.Tab):
                        return false;
                }
            }

            return true;
        }

        public void FilterKeyPress(char keyChar)
        {
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:FilterKeyPress()");

            Edit.FilterKeyPress(keyChar);
        }

        private /*protected virtual*/ GridEntry FindEquivalentGridEntry(GridEntryCollection ipeHier)
        {
            if (ipeHier is null || ipeHier.Count == 0)
            {
                return null;
            }

            GridEntryCollection rgipes = GetAllGridEntries();

            if (rgipes is null || rgipes.Count == 0)
            {
                return null;
            }

            GridEntry targetEntry = null;
            int row = 0;
            int count = rgipes.Count;

            for (int i = 0; i < ipeHier.Count; i++)
            {
                if (ipeHier[i] is null)
                {
                    continue;
                }

                // if we've got one above, and it's expandable,
                // expand it
                if (targetEntry != null)
                {
                    // how many do we have?
                    int items = rgipes.Count;

                    // expand and get the new count
                    if (!targetEntry.InternalExpanded)
                    {
                        SetExpand(targetEntry, true);
                        rgipes = GetAllGridEntries();
                    }
                    count = targetEntry.VisibleChildCount;
                }

                int start = row;
                targetEntry = null;

                // now, we will only go as many as were expanded...
                for (; row < rgipes.Count && ((row - start) <= count); row++)
                {
                    if (ipeHier.GetEntry(i).NonParentEquals(rgipes[row]))
                    {
                        targetEntry = rgipes.GetEntry(row);
                        row++;
                        break;
                    }
                }

                // didn't find it...
                if (targetEntry is null)
                {
                    break;
                }
            }

            return targetEntry;
        }

        protected virtual Point FindPosition(int x, int y)
        {
            if (RowHeight == -1)
            {
                return InvalidPosition;
            }

            Size size = GetOurSize();

            if (x < 0 || x > size.Width + ptOurLocation.X)
            {
                return InvalidPosition;
            }

            Point pt = new Point(ROWLABEL, 0);
            if (x > InternalLabelWidth + ptOurLocation.X)
            {
                pt.X = ROWVALUE;
            }

            pt.Y = (y - ptOurLocation.Y) / (1 + RowHeight);
            return pt;
        }

        public virtual void Flush()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView::Flush()");
            if (Commit() && Edit.Focused)
            {
                Focus();
            }
        }

        private GridEntryCollection GetAllGridEntries()
        {
            return GetAllGridEntries(false);
        }

        private GridEntryCollection GetAllGridEntries(bool fUpdateCache)
        {
            if (visibleRows == -1 || totalProps == -1 || !HasEntries)
            {
                return null;
            }

            if (allGridEntries != null && !fUpdateCache)
            {
                return allGridEntries;
            }

            GridEntry[] rgipes = new GridEntry[totalProps];
            try
            {
                GetGridEntriesFromOutline(topLevelGridEntries, 0, 0, rgipes);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }
            allGridEntries = new GridEntryCollection(null, rgipes);
            AddGridEntryEvents(allGridEntries, 0, -1);
            return allGridEntries;
        }

        private int GetCurrentValueIndex(GridEntry gridEntry)
        {
            if (!gridEntry.Enumerable)
            {
                return -1;
            }

            try
            {
                object[] values = gridEntry.GetPropertyValueList();
                object value = gridEntry.PropertyValue;
                string textValue = gridEntry.TypeConverter.ConvertToString(gridEntry, value);

                if (values != null && values.Length > 0)
                {
                    string itemTextValue;
                    int stringMatch = -1;
                    int equalsMatch = -1;
                    for (int i = 0; i < values.Length; i++)
                    {
                        object curValue = values[i];

                        // check real values against string values.
                        itemTextValue = gridEntry.TypeConverter.ConvertToString(curValue);
                        if (value == curValue || 0 == string.Compare(textValue, itemTextValue, true, CultureInfo.InvariantCulture))
                        {
                            stringMatch = i;
                        }
                        // now try .equals if they are both non-null
                        if (value != null && curValue != null && curValue.Equals(value))
                        {
                            equalsMatch = i;
                        }

                        if (stringMatch == equalsMatch && stringMatch != -1)
                        {
                            return stringMatch;
                        }
                    }

                    if (stringMatch != -1)
                    {
                        return stringMatch;
                    }

                    if (equalsMatch != -1)
                    {
                        return equalsMatch;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
            }
            return -1;
        }

        public virtual int GetDefaultOutlineIndent()
        {
            return OUTLINE_INDENT;
        }

        private IHelpService GetHelpService()
        {
            if (helpService is null && ServiceProvider != null)
            {
                topHelpService = (IHelpService)ServiceProvider.GetService(typeof(IHelpService));
                if (topHelpService != null)
                {
                    IHelpService localHelpService = topHelpService.CreateLocalContext(HelpContextType.ToolWindowSelection);
                    if (localHelpService != null)
                    {
                        helpService = localHelpService;
                    }
                }
            }
            return helpService;
        }

        public virtual int GetScrollOffset()
        {
            if (scrollBar is null)
            {
                return 0;
            }
            int pos = ScrollBar.Value;
            return pos;
        }

        /// <summary>
        ///  returns an array of IPE specifying the current heirarchy of ipes from the given
        ///  gridEntry through its parents to the root.
        /// </summary>
        private GridEntryCollection GetGridEntryHierarchy(GridEntry gridEntry)
        {
            if (gridEntry is null)
            {
                return null;
            }

            int depth = gridEntry.PropertyDepth;
            if (depth > 0)
            {
                GridEntry[] entries = new GridEntry[depth + 1];

                while (gridEntry != null && depth >= 0)
                {
                    entries[depth] = gridEntry;
                    gridEntry = gridEntry.ParentGridEntry;
                    depth = gridEntry.PropertyDepth;
                }
                return new GridEntryCollection(null, entries);
            }
            return new GridEntryCollection(null, new GridEntry[] { gridEntry });
        }

        private /*protected virtual*/ GridEntry GetGridEntryFromRow(int row)
        {
            return GetGridEntryFromOffset(row + GetScrollOffset());
        }

        private /*protected virtual*/ GridEntry GetGridEntryFromOffset(int offset)
        {
            GridEntryCollection rgipesAll = GetAllGridEntries();
            if (rgipesAll != null)
            {
                if (offset >= 0 && offset < rgipesAll.Count)
                {
                    return rgipesAll.GetEntry(offset);
                }
            }
            return null;
        }

        private /*protected virtual*/ int GetGridEntriesFromOutline(GridEntryCollection rgipe, int cCur,
                                                 int cTarget, GridEntry[] rgipeTarget)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:GetGridEntriesFromOutline");
            if (rgipe is null || rgipe.Count == 0)
            {
                return cCur;
            }

            cCur--; // want to account for each entry as we find it.

            for (int cLocal = 0; cLocal < rgipe.Count; cLocal++)
            {
                cCur++;
                if (cCur >= cTarget + rgipeTarget.Length)
                {
                    break;
                }

                GridEntry ipeCur = rgipe.GetEntry(cLocal);
                if (cCur >= cTarget)
                {
                    rgipeTarget[cCur - cTarget] = ipeCur;
                }

                if (ipeCur.InternalExpanded)
                {
                    GridEntryCollection subGridEntry = ipeCur.Children;
                    if (subGridEntry != null && subGridEntry.Count > 0)
                    {
                        cCur = GetGridEntriesFromOutline(subGridEntry,
                                                  cCur + 1, cTarget, rgipeTarget);
                    }
                }
            }

            return cCur;
        }

        private Size GetOurSize()
        {
            Size size = ClientSize;
            if (size.Width == 0)
            {
                Size sizeWindow = Size;
                if (sizeWindow.Width > 10)
                {
                    Debug.Fail("We have a bad client width!");
                    size.Width = sizeWindow.Width;
                    size.Height = sizeWindow.Height;
                }
            }
            if (!GetScrollbarHidden())
            {
                Size sizeScroll = ScrollBar.Size;
                size.Width -= sizeScroll.Width;
            }
            size.Width -= 2;
            size.Height -= 2;
            return size;
        }

        public Rectangle GetRectangle(int row, int flRow)
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);
            Size size = GetOurSize();

            rect.X = ptOurLocation.X;

            bool fLabel = ((flRow & ROWLABEL) != 0);
            bool fValue = ((flRow & ROWVALUE) != 0);

            if (fLabel && fValue)
            {
                rect.X = 1;
                rect.Width = size.Width - 1;
            }
            else if (fLabel)
            {
                rect.X = 1;
                rect.Width = InternalLabelWidth - 1;
            }
            else if (fValue)
            {
                rect.X = ptOurLocation.X + InternalLabelWidth;
                rect.Width = size.Width - InternalLabelWidth;
            }

            rect.Y = (row) * (RowHeight + 1) + 1 + ptOurLocation.Y;
            rect.Height = RowHeight;

            return rect;
        }

        internal int GetRowFromGridEntry(GridEntry gridEntry)
        {
            GridEntryCollection rgipesAll = GetAllGridEntries();
            if (gridEntry is null || rgipesAll is null)
            {
                return -1;
            }

            int bestMatch = -1;

            for (int i = 0; i < rgipesAll.Count; i++)
            {
                // try for an exact match.  semantics of equals are a bit loose here...
                //
                if (gridEntry == rgipesAll[i])
                {
                    return i - GetScrollOffset();
                }
                else if (bestMatch == -1 && gridEntry.Equals(rgipesAll[i]))
                {
                    bestMatch = i - GetScrollOffset();
                }
            }

            if (bestMatch != -1)
            {
                return bestMatch;
            }

            return -1 - GetScrollOffset();
        }

        public virtual bool GetInPropertySet()
        {
            return GetFlag(FlagInPropertySet);
        }

        protected virtual bool GetScrollbarHidden()
        {
            if (scrollBar is null)
            {
                return true;
            }
            return !ScrollBar.Visible;
        }

        /// <summary>
        ///  Returns a string containing test info about a given GridEntry. Requires an offset into the top-level
        ///  entry collection (ie. nested entries are not accessible). Or specify -1 to get info for the current
        ///  selected entry (which can be any entry, top-level or nested).
        /// </summary>
        public virtual string GetTestingInfo(int entry)
        {
            GridEntry gridEntry = (entry < 0) ? GetGridEntryFromRow(selectedRow) : GetGridEntryFromOffset(entry);

            if (gridEntry is null)
            {
                return "";
            }
            else
            {
                return gridEntry.GetTestingInfo();
            }
        }

        public Color GetTextColor()
        {
            return ForeColor;
        }

        private void LayoutWindow(bool invalidate)
        {
            Rectangle rect = ClientRectangle;
            Size sizeWindow = new Size(rect.Width, rect.Height);

            if (scrollBar != null)
            {
                Rectangle boundsScroll = ScrollBar.Bounds;
                boundsScroll.X = sizeWindow.Width - boundsScroll.Width - 1;
                boundsScroll.Y = 1;
                boundsScroll.Height = sizeWindow.Height - 2;
                ScrollBar.Bounds = boundsScroll;
            }

            if (invalidate)
            {
                Invalidate();
            }
        }

        internal void InvalidateGridEntryValue(GridEntry ge)
        {
            int row = GetRowFromGridEntry(ge);
            if (row != -1)
            {
                InvalidateRows(row, row, ROWVALUE);
            }
        }

        private void InvalidateRow(int row)
        {
            InvalidateRows(row, row, ROWVALUE | ROWLABEL);
        }

        private void InvalidateRows(int startRow, int endRow)
        {
            InvalidateRows(startRow, endRow, ROWVALUE | ROWLABEL);
        }

        private void InvalidateRows(int startRow, int endRow, int type)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:InvalidateRows");

            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Invalidating rows " + startRow.ToString(CultureInfo.InvariantCulture) + " through " + endRow.ToString(CultureInfo.InvariantCulture));
            Rectangle rect;

            // invalidate from the start row down
            if (endRow == -1)
            {
                rect = GetRectangle(startRow, type);
                rect.Height = (Size.Height - rect.Y) - 1;
                Invalidate(rect);
            }
            else
            {
                for (int i = startRow; i <= endRow; i++)
                {
                    rect = GetRectangle(i, type);
                    Invalidate(rect);
                }
            }
        }

        /// <summary>
        ///  Overridden to handle TAB key.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Escape:
                case Keys.Tab:
                case Keys.F4:
                    return false;

                case Keys.Return:
                    if (Edit.Focused)
                    {
                        return false;
                    }
                    break;
            }
            return base.IsInputKey(keyData);
        }

        private bool IsMyChild(Control c)
        {
            if (c == this || c is null)
            {
                return false;
            }

            Control cParent = c.ParentInternal;

            while (cParent != null)
            {
                if (cParent == this)
                {
                    return true;
                }
                cParent = cParent.ParentInternal;
            }
            return false;
        }

        private bool IsScrollValueValid(int newValue)
        {
            /*Debug.WriteLine("se.newValue = " + se.newValue.ToString());
            Debug.WriteLine("ScrollBar.Value = " + ScrollBar.Value.ToString());
            Debug.WriteLine("visibleRows = " + visibleRows.ToString());
            Debug.WriteLine("totalProps = " + totalProps.ToString());
            Debug.WriteLine("ScrollBar.Max = " + ScrollBar.Maximum.ToString());
            Debug.WriteLine("ScrollBar.LargeChange = " + ScrollBar.LargeChange.ToString());*/

            // is this move valid?
            if (newValue == ScrollBar.Value ||
                newValue < 0 ||
                newValue > ScrollBar.Maximum ||
                (newValue + (ScrollBar.LargeChange - 1) >= totalProps))
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView: move not needed, returning");
                return false;
            }
            return true;
        }

        internal bool IsSiblingControl(Control c1, Control c2)
        {
            Control parent1 = c1.ParentInternal;
            Control parent2 = c2.ParentInternal;

            while (parent2 != null)
            {
                if (parent1 == parent2)
                {
                    return true;
                }
                parent2 = parent2.ParentInternal;
            }
            return false;
        }

        private void MoveSplitterTo(int xpos)
        {
            int widthPS = GetOurSize().Width;
            int startPS = ptOurLocation.X;
            int pos = Math.Max(Math.Min(xpos, widthPS - 10), GetOutlineIconSize() * 2);

            int oldLabelWidth = InternalLabelWidth;

            labelRatio = ((double)widthPS / (double)(pos - startPS));

            SetConstants();

            if (selectedRow != -1)
            {
                // do this to move any editor we have
                SelectRow(selectedRow);
            }

            Rectangle r = ClientRectangle;

            // if we're moving to the left, just invalidate the values
            if (oldLabelWidth > InternalLabelWidth)
            {
                int left = InternalLabelWidth - requiredLabelPaintMargin;
                Invalidate(new Rectangle(left, 0, Size.Width - left, Size.Height));
            }
            else
            {
                // to the right, just invalidate from where the splitter was
                // to the right
                r.X = oldLabelWidth - requiredLabelPaintMargin;
                r.Width -= r.X;
                Invalidate(r);
            }
        }

        private void OnBtnClick(object sender, EventArgs e)
        {
            if (GetFlag(FlagBtnLaunchedEditor))
            {
                return;
            }

            if (sender == DialogButton && !Commit())
            {
                return;
            }
            SetCommitError(ERROR_NONE);

            try
            {
                Commit();
                SetFlag(FlagBtnLaunchedEditor, true);
                PopupDialog(selectedRow);
            }
            finally
            {
                SetFlag(FlagBtnLaunchedEditor, false);
            }
        }

        private void OnBtnKeyDown(object sender, KeyEventArgs ke)
        {
            OnKeyDown(sender, ke);
        }

        private void OnChildLostFocus(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnChildLostFocus");
            InvokeLostFocus(this, e);
        }

        private void OnDropDownButtonGotFocus(object sender, EventArgs e)
        {
            if (sender is DropDownButton dropDownButton)
            {
                dropDownButton.AccessibilityObject.SetFocus();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnGotFocus");

            base.OnGotFocus(e);

            if (e != null && !GetInPropertySet())
            {
                if (!Commit())
                {
                    Edit.Focus();
                    return;
                }
            }

            if (selectedGridEntry != null && GetRowFromGridEntry(selectedGridEntry) != -1)
            {
                selectedGridEntry.Focus = true;
                SelectGridEntry(selectedGridEntry, false);
            }
            else
            {
                SelectRow(0);
            }

            if (selectedGridEntry != null && selectedGridEntry.GetValueOwner() != null)
            {
                UpdateHelpAttributes(null, selectedGridEntry);
            }

            // For empty GridView, draw a focus-indicator rectangle, just inside GridView borders
            if (totalProps <= 0)
            {
                int doubleOffset = 2 * offset_2Units;

                if ((Size.Width > doubleOffset) && (Size.Height > doubleOffset))
                {
                    using (Graphics g = CreateGraphicsInternal())
                    {
                        ControlPaint.DrawFocusRectangle(g, new Rectangle(offset_2Units, offset_2Units, Size.Width - doubleOffset, Size.Height - doubleOffset));
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnSysColorChange);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnSysColorChange);
            // We can leak this if we aren't disposed.
            //
            if (toolTip != null && !RecreatingHandle)
            {
                toolTip.Dispose();
                toolTip = null;
            }
            base.OnHandleDestroyed(e);
        }

        private void OnListChange(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnListChange");
            if (!DropDownListBox.InSetSelectedIndex())
            {
                GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
                Edit.Text = gridEntry.GetPropertyTextValue(DropDownListBox.SelectedItem);
                Edit.Focus();
                SelectEdit(false);
            }
            SetFlag(FlagDropDownCommit, true);
        }

        private void OnListMouseUp(object sender, MouseEventArgs me)
        {
            OnListClick(sender, me);
        }

        private void OnListClick(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnListClick");
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);

            if (DropDownListBox.Items.Count == 0)
            {
                CommonEditorHide();
                SetCommitError(ERROR_NONE);
                SelectRow(selectedRow);
                return;
            }
            else
            {
                object value = DropDownListBox.SelectedItem;

                // don't need the commit because we're committing anyway.
                //
                SetFlag(FlagDropDownCommit, false);
                if (value != null && !CommitText((string)value))
                {
                    SetCommitError(ERROR_NONE);
                    SelectRow(selectedRow);
                }
            }
        }

        private void OnListDrawItem(object sender, DrawItemEventArgs die)
        {
            int index = die.Index;

            if (index < 0 || selectedGridEntry is null)
            {
                return;
            }

            string text = (string)DropDownListBox.Items[die.Index];

            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Drawing list item, value='" + text + "'");
            die.DrawBackground();
            die.DrawFocusRectangle();

            Rectangle drawBounds = die.Bounds;
            drawBounds.Y += 1;
            drawBounds.X -= 1;

            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);

            try
            {
                gridEntry.PaintValue(
                    gridEntry.ConvertTextToValue(text),
                    die.GraphicsInternal,
                    drawBounds,
                    drawBounds,
                    die.State.HasFlag(DrawItemState.Selected) ? GridEntry.PaintValueFlags.DrawSelected : default);
            }
            catch (FormatException ex)
            {
                ShowFormatExceptionMessage(gridEntry.PropertyLabel, text, ex);
                if (DropDownListBox.IsHandleCreated)
                {
                    DropDownListBox.Visible = false;
                }
            }
        }

        private void OnListKeyDown(object sender, KeyEventArgs ke)
        {
            if (ke.KeyCode == Keys.Return)
            {
                OnListClick(null, null);
                if (selectedGridEntry != null)
                {
                    selectedGridEntry.OnValueReturnKey();
                }
            }

            OnKeyDown(sender, ke);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnLostFocus");
            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "PropertyGridView lost focus");

            if (e != null)
            {
                base.OnLostFocus(e);
            }

            if (FocusInside)
            {
                base.OnLostFocus(e);
                return;
            }

            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry != null)
            {
                Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "removing gridEntry focus");
                gridEntry.Focus = false;
                CommonEditorHide();
                InvalidateRow(selectedRow);
            }
            base.OnLostFocus(e);

            // For empty GridView, clear the focus indicator that was painted in OnGotFocus()
            if (totalProps <= 0)
            {
                Rectangle clearRect = new Rectangle(1, 1, Size.Width - 2, Size.Height - 2);
                Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, $"Filling empty gridview rect={clearRect}");

                Color color = BackColor;
                if (color.HasTransparency())
                {
                    using Graphics g = CreateGraphicsInternal();
                    using var brush = color.GetCachedSolidBrushScope();
                    g.FillRectangle(brush, clearRect);
                }
                else
                {
                    using var hdc = new User32.GetDcScope(Handle);
                    using var hbrush = new Gdi32.CreateBrushScope(color);
                    hdc.FillRectangle(hbrush, clearRect);
                }
            }
        }

        private void OnEditChange(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditChange");
            SetCommitError(ERROR_NONE, Edit.Focused);

            ToolTip.ToolTip = string.Empty;
            ToolTip.Visible = false;

            if (!Edit.InSetText())
            {
                GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
                if (gridEntry != null && (gridEntry.Flags & GridEntry.FLAG_IMMEDIATELY_EDITABLE) != 0)
                {
                    Commit();
                }
            }
        }

        private void OnEditGotFocus(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditGotFocus");

            if (!Edit.Visible)
            {
                Focus();
                return;
            }

            switch (errorState)
            {
                case ERROR_MSGBOX_UP:
                    return;
                case ERROR_THROWN:
                    if (Edit.Visible)
                    {
                        Edit.HookMouseDown = true;
                    }
                    break;
                default:
                    if (NeedsCommit)
                    {
                        SetCommitError(ERROR_NONE, true);
                    }
                    break;
            }

            if (selectedGridEntry != null && GetRowFromGridEntry(selectedGridEntry) != -1)
            {
                Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "adding gridEntry focus");
                selectedGridEntry.Focus = true;
                InvalidateRow(selectedRow);
                (Edit.AccessibilityObject as ControlAccessibleObject).NotifyClients(AccessibleEvents.Focus);

                Edit.AccessibilityObject.SetFocus();
            }
            else
            {
                SelectRow(0);
            }
        }

        private bool ProcessEnumUpAndDown(GridEntry gridEntry, Keys keyCode, bool closeDropDown = true)
        {
            object value = gridEntry.PropertyValue;
            object[] rgvalues = gridEntry.GetPropertyValueList();
            if (rgvalues != null)
            {
                for (int i = 0; i < rgvalues.Length; i++)
                {
                    object rgvalue = rgvalues[i];
                    if (value != null && rgvalue != null && value.GetType() != rgvalue.GetType() && gridEntry.TypeConverter.CanConvertTo(gridEntry, value.GetType()))
                    {
                        rgvalue = gridEntry.TypeConverter.ConvertTo(gridEntry, CultureInfo.CurrentCulture, rgvalue, value.GetType());
                    }

                    bool equal = (value == rgvalue) || (value != null && value.Equals(rgvalue));

                    if (!equal && value is string && rgvalue != null)
                    {
                        equal = 0 == string.Compare((string)value, rgvalue.ToString(), true, CultureInfo.CurrentCulture);
                    }
                    if (equal)
                    {
                        object valueNew = null;
                        if (keyCode == Keys.Up)
                        {
                            if (i == 0)
                            {
                                return true;
                            }

                            valueNew = rgvalues[i - 1];
                        }
                        else
                        {
                            if (i == rgvalues.Length - 1)
                            {
                                return true;
                            }

                            valueNew = rgvalues[i + 1];
                        }

                        CommitValue(gridEntry, valueNew, closeDropDown);
                        SelectEdit(false);
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnEditKeyDown(object sender, KeyEventArgs ke)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditKeyDown");
            bool fAlt = ke.Alt;
            if (!fAlt && (ke.KeyCode == Keys.Up || ke.KeyCode == Keys.Down))
            {
                GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
                if (!gridEntry.Enumerable || !gridEntry.IsValueEditable)
                {
                    return;
                }

                ke.Handled = true;
                bool processed = ProcessEnumUpAndDown(gridEntry, ke.KeyCode);
                if (processed)
                {
                    return;
                }
            }
            // Handle non-expand/collapse case of left & right as up & down
            else if ((ke.KeyCode == Keys.Left || ke.KeyCode == Keys.Right) &&
                     (ke.Modifiers & ~Keys.Shift) != 0)
            {
                return;
            }
            OnKeyDown(sender, ke);
        }

        private void OnEditKeyPress(object sender, KeyPressEventArgs ke)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditKeyPress");
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            if (!gridEntry.IsTextEditable)
            {
                ke.Handled = FilterReadOnlyEditKeyPress(ke.KeyChar);
            }
        }

        private void OnEditLostFocus(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditLostFocus");

            // believe it or not, this can actually happen.
            if (Edit.Focused || (errorState == ERROR_MSGBOX_UP) || (errorState == ERROR_THROWN) || GetInPropertySet())
            {
                return;
            }

            // check to see if the focus is on the drop down or one of it's children
            // if so, return;
            if (dropDownHolder != null && dropDownHolder.Visible)
            {
                bool found = false;
                for (IntPtr hwnd = User32.GetForegroundWindow();
                    hwnd != IntPtr.Zero; hwnd = User32.GetParent(hwnd))
                {
                    if (hwnd == dropDownHolder.Handle)
                    {
                        found = true;
                    }
                }
                if (found)
                {
                    return;
                }
            }

            if (FocusInside)
            {
                return;
            }

            // if the focus isn't goint to a child of the view
            if (!Commit())
            {
                Edit.Focus();
                return;
            }
            // change our focus state.
            InvokeLostFocus(this, EventArgs.Empty);
        }

        private void OnEditMouseDown(object sender, MouseEventArgs me)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditMouseDown");

            if (!FocusInside)
            {
                SelectGridEntry(selectedGridEntry, false);
            }

            if (me.Clicks % 2 == 0)
            {
                DoubleClickRow(selectedRow, false, ROWVALUE);
                Edit.SelectAll();
            }

            if (rowSelectTime == 0)
            {
                return;
            }

            // check if the click happened within the double click time since the row was selected.
            // this allows the edits to be selected with two clicks instead of 3 (select row, double click).
            //
            long timeStamp = DateTime.Now.Ticks;
            int delta = (int)((timeStamp - rowSelectTime) / 10000); // make it milleseconds

            if (delta < SystemInformation.DoubleClickTime)
            {
                Point screenPoint = Edit.PointToScreen(new Point(me.X, me.Y));

                if (Math.Abs(screenPoint.X - rowSelectPos.X) < SystemInformation.DoubleClickSize.Width &&
                    Math.Abs(screenPoint.Y - rowSelectPos.Y) < SystemInformation.DoubleClickSize.Height)
                {
                    DoubleClickRow(selectedRow, false, ROWVALUE);
                    User32.SendMessageW(Edit, User32.WM.LBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(me.X, me.Y));
                    Edit.SelectAll();
                }
                rowSelectPos = Point.Empty;

                rowSelectTime = 0;
            }
        }

        private bool OnF4(Control sender)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnF4");
            if (ModifierKeys != 0)
            {
                return false;
            }
            if (sender == this || sender == OwnerGrid)
            {
                F4Selection(true);
            }
            else
            {
                UnfocusSelection();
            }

            return true;
        }

        private bool OnEscape(Control sender)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEscape");
            if ((ModifierKeys & (Keys.Alt | Keys.Control)) != 0)
            {
                return false;
            }

            SetFlag(FlagDropDownCommit, false);

            if (sender == Edit && Edit.Focused)
            {
                // if we aren't in an error state, just quit
                if (errorState == ERROR_NONE)
                {
                    Edit.Text = originalTextValue;
                    Focus();
                    return true;
                }

                if (NeedsCommit)
                {
                    bool success = false;
                    Edit.Text = originalTextValue;
                    bool needReset = true;

                    if (selectedGridEntry != null)
                    {
                        string curTextValue = selectedGridEntry.GetPropertyTextValue();
                        needReset = originalTextValue != curTextValue && !(string.IsNullOrEmpty(originalTextValue) && string.IsNullOrEmpty(curTextValue));
                    }

                    if (needReset)
                    {
                        try
                        {
                            success = CommitText(originalTextValue);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        success = true;
                    }

                    // this would be an odd thing to happen, but...
                    if (!success)
                    {
                        Edit.Focus();
                        SelectEdit(false);
                        return true;
                    }
                }

                SetCommitError(ERROR_NONE);
                Focus();
                return true;
            }
            else if (sender != this)
            {
                CloseDropDown();
                Focus();
            }
            return false;
        }

        protected override void OnKeyDown(KeyEventArgs ke)
        {
            OnKeyDown(this, ke);
        }

        private void OnKeyDown(object sender, KeyEventArgs ke)
        {
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            ke.Handled = true;
            bool fControl = ke.Control;
            bool fShift = ke.Shift;
            bool fBoth = fControl && fShift;
            bool fAlt = ke.Alt;
            Keys keyCode = ke.KeyCode;
            bool fallingThorugh = false;

            // Microsoft, we have to do this here because if we are
            // hosted in a non-windows forms dialog, we never get a chance to
            // peek at the messages, we just get called,
            // so we have to do this here...
            //
            if (keyCode == Keys.Tab)
            {
                if (ProcessDialogKey(ke.KeyData))
                {
                    ke.Handled = true;
                    return;
                }
            }

            // Alt-Arrow support... sigh...
            if (keyCode == Keys.Down && fAlt && DropDownButton.Visible)
            {
                F4Selection(false);
                return;
            }

            if (keyCode == Keys.Up && fAlt && DropDownButton.Visible && (dropDownHolder != null) && dropDownHolder.Visible)
            {
                UnfocusSelection();
                return;
            }

            if (ToolTip.Visible)
            {
                ToolTip.ToolTip = string.Empty;
            }

            if (fBoth || sender == this || sender == OwnerGrid)
            {
                switch (keyCode)
                {
                    case Keys.Up:
                    case Keys.Down:
                        int pos = (keyCode == Keys.Up ? selectedRow - 1 : selectedRow + 1);
                        SelectGridEntry(GetGridEntryFromRow(pos), true);
                        SetFlag(FlagNoDefault, false);
                        return;
                    case Keys.Left:
                        if (fControl)
                        {
                            // move the splitter 3 pixels to the left
                            MoveSplitterTo(InternalLabelWidth - 3);
                            return;
                        }
                        if (gridEntry.InternalExpanded)
                        {
                            SetExpand(gridEntry, false);
                        }
                        else
                        {
                            // Handle non-expand/collapse case of left & right as up & down
                            SelectGridEntry(GetGridEntryFromRow(selectedRow - 1), true);
                        }
                        return;
                    case Keys.Right:
                        if (fControl)
                        {
                            // move the splitter 3 pixels to the right
                            MoveSplitterTo(InternalLabelWidth + 3);
                            return;
                        }
                        if (gridEntry.Expandable)
                        {
                            if (gridEntry.InternalExpanded)
                            {
                                GridEntryCollection rgipes2 = gridEntry.Children;
                                SelectGridEntry(rgipes2.GetEntry(0), true);
                            }
                            else
                            {
                                SetExpand(gridEntry, true);
                            }
                        }
                        else
                        {
                            // Handle non-expand/collapse case of left & right as up & down
                            SelectGridEntry(GetGridEntryFromRow(selectedRow + 1), true);
                        }
                        return;
                    case Keys.Return:
                        if (gridEntry.Expandable)
                        {
                            SetExpand(gridEntry, !gridEntry.InternalExpanded);
                        }
                        else
                        {
                            gridEntry.OnValueReturnKey();
                        }

                        return;
                    case Keys.Home:
                    case Keys.End:
                        GridEntryCollection rgipes = GetAllGridEntries();
                        int pos2 = (keyCode == Keys.Home ? 0 : rgipes.Count - 1);
                        SelectGridEntry(rgipes.GetEntry(pos2), true);
                        return;
                    case Keys.Add:
                    case Keys.Oemplus:
                    case Keys.OemMinus:
                    case Keys.Subtract:

                        if (!gridEntry.Expandable)
                        {
                            break;
                        }

                        SetFlag(FlagIsSpecialKey, true);
                        bool expand = (keyCode == Keys.Add || keyCode == Keys.Oemplus);
                        SetExpand(gridEntry, expand);
                        Invalidate();
                        ke.Handled = true;
                        return;

                    case Keys.D8:
                        if (fShift)
                        {
                            goto case Keys.Multiply;
                        }
                        break;
                    case Keys.Multiply:
                        SetFlag(FlagIsSpecialKey, true);
                        RecursivelyExpand(gridEntry, true, true, MaxRecurseExpand);
                        ke.Handled = false;
                        return;

                    case Keys.Prior:  //PAGE_UP:
                    case Keys.Next: //PAGE_DOWN

                        bool next = (keyCode == Keys.Next);
                        //int rowGoal = next ? visibleRows - 1 : 0;
                        int offset = next ? visibleRows - 1 : 1 - visibleRows;

                        int row = selectedRow;

                        if (fControl && !fShift)
                        {
                            return;
                        }
                        if (selectedRow != -1)
                        { // actual paging.
                            int start = GetScrollOffset();
                            SetScrollOffset(start + offset);
                            SetConstants();
                            if (GetScrollOffset() != (start + offset))
                            {
                                // we didn't make a full page
                                if (next)
                                {
                                    row = visibleRows - 1;
                                }
                                else
                                {
                                    row = 0;
                                }
                            }
                        }

                        SelectRow(row);
                        Refresh();
                        return;

                    // Copy/paste support...

                    case Keys.Insert:
                        if (fShift && !fControl && !fAlt)
                        {
                            fallingThorugh = true;
                            goto case Keys.V;
                        }
                        goto case Keys.C;
                    case Keys.C:
                        // copy text in current property
                        if (fControl && !fAlt && !fShift)
                        {
                            DoCopyCommand();
                            return;
                        }
                        break;
                    case Keys.Delete:
                        // cut text in current property
                        if (fShift && !fControl && !fAlt)
                        {
                            fallingThorugh = true;
                            goto case Keys.X;
                        }
                        break;
                    case Keys.X:
                        // cut text in current property
                        if (fallingThorugh || (fControl && !fAlt && !fShift))
                        {
                            Clipboard.SetDataObject(gridEntry.GetPropertyTextValue());
                            CommitText("");
                            return;
                        }
                        break;
                    case Keys.V:
                        // paste the text
                        if (fallingThorugh || (fControl && !fAlt && !fShift))
                        {
                            DoPasteCommand();
                        }
                        break;
                    case Keys.A:
                        if (fControl && !fAlt && !fShift && Edit.Visible)
                        {
                            Edit.Focus();
                            Edit.SelectAll();
                        }
                        break;
                }
            }

            if (gridEntry != null && ke.KeyData == (Keys.C | Keys.Alt | Keys.Shift | Keys.Control))
            {
                Clipboard.SetDataObject(gridEntry.GetTestingInfo());
                return;
            }

            /* Due to conflicts with other VS commands,
               we are removing this functionality.

               // Ctrl + Shift + 'X' selects the property that starts with 'X'
               if (fBoth) {
                   // now get the array to work with.
                   GridEntry[] rgipes = GetAllGridEntries();
                   int cLength = rgipes.Length;

                   // now get our char.
                   string strCh = (new string(new char[] {(char)ke.KeyCode})).ToLower(CultureInfo.InvariantCulture);

                   int cCur = -1;
                   if (gridEntry != null)
                       for (int i = 0; i < cLength; i++) {
                           if (rgipes[i] == gridEntry) {
                               cCur = i;
                               break;
                           }
                       }

                   cCur += 1; // this indicated where we start...
                   // find next label that starts with this letter.
                   for (int i = 0; i < cLength; i++) {
                       GridEntry ipeCur = rgipes[(i + cCur) % cLength];
                       if (ipeCur.PropertyLabel.ToLower(CultureInfo.InvariantCulture).StartsWith(strCh)) {
                           if (gridEntry != ipeCur) {
                               SelectGridEntry(ipeCur,true);
                               return;
                           }
                           break;
                       }
                   }
               }
            */

            if (selectedGridEntry != null && selectedGridEntry.Enumerable &&
                dropDownHolder != null && dropDownHolder.Visible &&
                (keyCode == Keys.Up || keyCode == Keys.Down))
            {
                ProcessEnumUpAndDown(selectedGridEntry, keyCode, false);
            }

            ke.Handled = false;
            return;
        }

        protected override void OnKeyPress(KeyPressEventArgs ke)
        {
            bool fControl = false; //ke.getControl();
            bool fShift = false; //ke.getShift();
            bool fBoth = fControl && fShift;
            if (!fBoth && WillFilterKeyPress(ke.KeyChar))
            {
                // find next property with letter typed.
                FilterKeyPress(ke.KeyChar);
            }

            SetFlag(FlagIsSpecialKey, false);
        }

        protected override void OnMouseDown(MouseEventArgs me)
        {
            // check for a splitter
            if (me.Button == MouseButtons.Left && SplitterInside(me.X, me.Y) && totalProps != 0)
            {
                if (!Commit())
                {
                    return;
                }

                if (me.Clicks == 2)
                {
                    MoveSplitterTo(Width / 2);
                    return;
                }

                UnfocusSelection();
                SetFlag(FlagIsSplitterMove, true);
                tipInfo = -1;
                Capture = true;
                return;
            }

            // are ew on a propentry?
            Point pos = FindPosition(me.X, me.Y);

            if (pos == InvalidPosition)
            {
                return;
            }

            // Notify that prop entry of the click...but normalize
            // it's coords first...we really  just need the x, y
            GridEntry gridEntry = GetGridEntryFromRow(pos.Y);

            if (gridEntry != null)
            {
                // get the origin of this pe
                Rectangle r = GetRectangle(pos.Y, ROWLABEL);

                lastMouseDown = new Point(me.X, me.Y);

                // offset the mouse points
                // notify the prop entry
                if (me.Button == MouseButtons.Left)
                {
                    gridEntry.OnMouseClick(me.X - r.X, me.Y - r.Y, me.Clicks, me.Button);
                }
                else
                {
                    SelectGridEntry(gridEntry, false);
                }
                lastMouseDown = InvalidPosition;
                gridEntry.Focus = true;
                SetFlag(FlagNoDefault, false);
            }
        }

        // this will make tool tip go away.
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!GetFlag(FlagIsSplitterMove))
            {
                Cursor = Cursors.Default; // Cursor = null;;
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs me)
        {
            int rowMoveCur;
            Point pt = Point.Empty;
            bool onLabel = false;

            if (me is null)
            {
                rowMoveCur = -1;
                pt = InvalidPosition;
            }
            else
            {
                pt = FindPosition(me.X, me.Y);
                if (pt == InvalidPosition || (pt.X != ROWLABEL && pt.X != ROWVALUE))
                {
                    rowMoveCur = -1;
                    ToolTip.ToolTip = string.Empty;
                }
                else
                {
                    rowMoveCur = pt.Y;
                    onLabel = pt.X == ROWLABEL;
                }
            }

            if (pt == InvalidPosition || me is null)
            {
                return;
            }

            if (GetFlag(FlagIsSplitterMove))
            {
                MoveSplitterTo(me.X);
            }

            if ((rowMoveCur != TipRow || pt.X != TipColumn) && !GetFlag(FlagIsSplitterMove))
            {
                GridEntry gridItem = GetGridEntryFromRow(rowMoveCur);
                string tip = string.Empty;
                tipInfo = -1;

                if (gridItem != null)
                {
                    Rectangle itemRect = GetRectangle(pt.Y, pt.X);
                    if (onLabel && gridItem.GetLabelToolTipLocation(me.X - itemRect.X, me.Y - itemRect.Y) != InvalidPoint)
                    {
                        tip = gridItem.LabelToolTipText;
                        TipRow = rowMoveCur;
                        TipColumn = pt.X;
                    }
                    else if (!onLabel && gridItem.ValueToolTipLocation != InvalidPoint && !Edit.Focused)
                    {
                        if (!NeedsCommit)
                        {
                            tip = gridItem.GetPropertyTextValue();
                        }
                        TipRow = rowMoveCur;
                        TipColumn = pt.X;
                    }
                }

                // Ensure that tooltips don't display when host application is not foreground app.
                // Assume that we don't want to display the tooltips
                IntPtr foregroundWindow = User32.GetForegroundWindow();
                if (User32.IsChild(foregroundWindow, new HandleRef(this, Handle)).IsTrue())
                {
                    // Don't show the tips if a dropdown is showing
                    if ((dropDownHolder is null || dropDownHolder.Component is null) || rowMoveCur == selectedRow)
                    {
                        ToolTip.ToolTip = tip;
                    }
                }
                else
                {
                    ToolTip.ToolTip = string.Empty;
                }
            }

            if (totalProps != 0 && (SplitterInside(me.X, me.Y) || GetFlag(FlagIsSplitterMove)))
            {
                Cursor = Cursors.VSplit;
            }
            else
            {
                Cursor = Cursors.Default; // Cursor = null;;
            }
            base.OnMouseMove(me);
        }

        protected override void OnMouseUp(MouseEventArgs me)
        {
            CancelSplitterMove();
        }

        protected override void OnMouseWheel(MouseEventArgs me)
        {
            OwnerGrid.OnGridViewMouseWheel(me);

            if (me is HandledMouseEventArgs e)
            {
                if (e.Handled)
                {
                    return;
                }
                e.Handled = true;
            }

            if ((ModifierKeys & (Keys.Shift | Keys.Alt)) != 0 || MouseButtons != MouseButtons.None)
            {
                return; // Do not scroll when Shift or Alt key is down, or when a mouse button is down.
            }

            int wheelScrollLines = SystemInformation.MouseWheelScrollLines;
            if (wheelScrollLines == 0)
            {
                return; // Do not scroll when the user system setting is 0 lines per notch
            }

            Debug.Assert(cumulativeVerticalWheelDelta > -NativeMethods.WHEEL_DELTA, "cumulativeVerticalWheelDelta is too small");
            Debug.Assert(cumulativeVerticalWheelDelta < NativeMethods.WHEEL_DELTA, "cumulativeVerticalWheelDelta is too big");

            // Should this only work if the Edit has focus?  anyway
            // we use the mouse wheel to change the values in the dropdown if it's
            // an enumerable value.
            //
            if (selectedGridEntry != null && selectedGridEntry.Enumerable && Edit.Focused && selectedGridEntry.IsValueEditable)
            {
                int index = GetCurrentValueIndex(selectedGridEntry);
                if (index != -1)
                {
                    int delta = me.Delta > 0 ? -1 : 1;
                    object[] values = selectedGridEntry.GetPropertyValueList();

                    if (delta > 0 && index >= (values.Length - 1))
                    {
                        index = 0;
                    }
                    else if (delta < 0 && index == 0)
                    {
                        index = values.Length - 1;
                    }
                    else
                    {
                        index += delta;
                    }

                    CommitValue(values[index]);
                    SelectGridEntry(selectedGridEntry, true);
                    Edit.Focus();
                    return;
                }
            }

            int initialOffset = GetScrollOffset();
            cumulativeVerticalWheelDelta += me.Delta;
            float partialNotches = (float)cumulativeVerticalWheelDelta / (float)NativeMethods.WHEEL_DELTA;
            int fullNotches = (int)partialNotches;

            if (wheelScrollLines == -1)
            {
                // Equivalent to large change scrolls
                if (fullNotches != 0)
                {
                    int originalOffset = initialOffset;
                    int large = fullNotches * scrollBar.LargeChange;
                    int newOffset = Math.Max(0, initialOffset - large);
                    newOffset = Math.Min(newOffset, totalProps - visibleRows + 1);

                    initialOffset -= fullNotches * scrollBar.LargeChange;
                    if (Math.Abs(initialOffset - originalOffset) >= Math.Abs(fullNotches * scrollBar.LargeChange))
                    {
                        cumulativeVerticalWheelDelta -= fullNotches * NativeMethods.WHEEL_DELTA;
                    }
                    else
                    {
                        cumulativeVerticalWheelDelta = 0;
                    }

                    if (!ScrollRows(newOffset))
                    {
                        cumulativeVerticalWheelDelta = 0;
                        return;
                    }
                }
            }
            else
            {
                // SystemInformation.MouseWheelScrollLines doesn't work under terminal server,
                // it default to the notches per scroll.
                int scrollBands = (int)((float)wheelScrollLines * partialNotches);

                if (scrollBands != 0)
                {
                    if (ToolTip.Visible)
                    {
                        ToolTip.ToolTip = string.Empty;
                    }

                    int newOffset = Math.Max(0, initialOffset - scrollBands);
                    newOffset = Math.Min(newOffset, totalProps - visibleRows + 1);

                    if (scrollBands > 0)
                    {
                        if (scrollBar.Value <= scrollBar.Minimum)
                        {
                            cumulativeVerticalWheelDelta = 0;
                        }
                        else
                        {
                            cumulativeVerticalWheelDelta -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
                        }
                    }
                    else
                    {
                        if (scrollBar.Value > (scrollBar.Maximum - visibleRows + 1))
                        {
                            cumulativeVerticalWheelDelta = 0;
                        }
                        else
                        {
                            cumulativeVerticalWheelDelta -= (int)((float)scrollBands * ((float)NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
                        }
                    }

                    if (!ScrollRows(newOffset))
                    {
                        cumulativeVerticalWheelDelta = 0;
                        return;
                    }
                }
                else
                {
                    cumulativeVerticalWheelDelta = 0;
                }
            }
        }

        protected override void OnMove(EventArgs e)
        {
            CloseDropDown();
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnPaint");
            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "On paint called.  Rect=" + pe.ClipRectangle.ToString());
            Graphics g = pe.Graphics;

            int yPos = 0;
            int startRow = 0;
            int endRow = visibleRows - 1;

            Rectangle clipRect = pe.ClipRectangle;

            // give ourselves a little breathing room to account for lines, etc., as well
            // as the entries themselves.
            clipRect.Inflate(0, 2);

            try
            {
                Size sizeWindow = Size;

                // figure out what rows we're painting
                Point posStart = FindPosition(clipRect.X, clipRect.Y);
                Point posEnd = FindPosition(clipRect.X, clipRect.Y + clipRect.Height);
                if (posStart != InvalidPosition)
                {
                    startRow = Math.Max(0, posStart.Y);
                }

                if (posEnd != InvalidPosition)
                {
                    endRow = posEnd.Y;
                }

                int cPropsVisible = Math.Min(totalProps - GetScrollOffset(), 1 + visibleRows);

#if DEBUG
                GridEntry debugIPEStart = GetGridEntryFromRow(startRow);
                GridEntry debugIPEEnd = GetGridEntryFromRow(endRow);
                string startName = debugIPEStart?.PropertyLabel;
                if (startName is null)
                {
                    startName = "(null)";
                }
                string endName = debugIPEEnd?.PropertyLabel;
                if (endName is null)
                {
                    endName = "(null)";
                }
#endif

                SetFlag(FlagNeedsRefresh, false);

                Size size = GetOurSize();
                Point loc = ptOurLocation;

                if (GetGridEntryFromRow(cPropsVisible - 1) is null)
                {
                    cPropsVisible--;
                }

                // if we actually have some properties, then start drawing the grid
                if (totalProps > 0)
                {
                    // draw splitter
                    cPropsVisible = Math.Min(cPropsVisible, endRow + 1);

                    Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Drawing splitter");
                    using var splitterPen = OwnerGrid.LineColor.GetCachedPenScope(GetSplitterWidth());
                    g.DrawLine(splitterPen, labelWidth, loc.Y, labelWidth, (cPropsVisible) * (RowHeight + 1) + loc.Y);

                    // draw lines.
                    Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Drawing lines");
                    using var linePen = g.FindNearestColor(OwnerGrid.LineColor).GetCachedPenScope();

                    int cHeightCurRow = 0;
                    int cLineEnd = loc.X + size.Width;
                    int cLineStart = loc.X;

                    // draw values.
                    int totalWidth = GetTotalWidth() + 1;

                    // draw labels. set clip rect.
                    for (int i = startRow; i < cPropsVisible; i++)
                    {
                        try
                        {
                            // draw the line
                            cHeightCurRow = (i) * (RowHeight + 1) + loc.Y;
                            g.DrawLine(linePen, cLineStart, cHeightCurRow, cLineEnd, cHeightCurRow);

                            // draw the value
                            DrawValueEntry(g, i, ref clipRect);

                            // draw the label
                            Rectangle rect = GetRectangle(i, ROWLABEL);
                            yPos = rect.Y + rect.Height;
                            DrawLabel(g, i, rect, (i == selectedRow), false, ref clipRect);
                            if (i == selectedRow)
                            {
                                Edit.Invalidate();
                            }
                        }
                        catch
                        {
                            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose,
                                $"Exception thrown during painting property {GetGridEntryFromRow(i).PropertyLabel}");
                        }
                    }

                    // draw the bottom line
                    cHeightCurRow = (cPropsVisible) * (RowHeight + 1) + loc.Y;
                    g.DrawLine(linePen, cLineStart, cHeightCurRow, cLineEnd, cHeightCurRow);
                }

                // fill anything left with window
                if (yPos < Size.Height)
                {
                    yPos++;
                    Rectangle clearRect = new Rectangle(1, yPos, Size.Width - 2, Size.Height - yPos - 1);
                    Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, $"Filling remaining area rect={clearRect}");

                    using var backBrush = BackColor.GetCachedSolidBrushScope();
                    g.FillRectangle(backBrush, clearRect);
                }

                // Draw outside border
                using var borderPen = OwnerGrid.ViewBorderColor.GetCachedPenScope();
                g.DrawRectangle(borderPen, 0, 0, sizeWindow.Width - 1, sizeWindow.Height - 1);

                _fontBold = null;
            }
            catch
            {
                Debug.Fail("Caught exception in OnPaint");
                // Do nothing.
            }
        }

        private void OnGridEntryLabelDoubleClick(object s, EventArgs e)
        {
            GridEntry gridEntry = (GridEntry)s;

            // if we've changed since the click (probably because we moved a row into view), bail
            //
            if (gridEntry != lastClickedEntry)
            {
                return;
            }
            int row = GetRowFromGridEntry(gridEntry);
            DoubleClickRow(row, gridEntry.Expandable, ROWLABEL);
        }

        private void OnGridEntryValueDoubleClick(object s, EventArgs e)
        {
            GridEntry gridEntry = (GridEntry)s;
            // if we've changed since the click (probably because we moved a row into view), bail
            //
            if (gridEntry != lastClickedEntry)
            {
                return;
            }
            int row = GetRowFromGridEntry(gridEntry);
            DoubleClickRow(row, gridEntry.Expandable, ROWVALUE);
        }

        private void OnGridEntryLabelClick(object s, EventArgs e)
        {
            lastClickedEntry = (GridEntry)s;
            SelectGridEntry(lastClickedEntry, true);
        }

        private void OnGridEntryOutlineClick(object s, EventArgs e)
        {
            GridEntry gridEntry = (GridEntry)s;
            Debug.Assert(gridEntry.Expandable, "non-expandable IPE firing outline click");

            Cursor oldCursor = Cursor;
            if (!ShouldSerializeCursor())
            {
                oldCursor = null;
            }
            Cursor = Cursors.WaitCursor;

            try
            {
                SetExpand(gridEntry, !gridEntry.InternalExpanded);
                SelectGridEntry(gridEntry, false);
            }
            finally
            {
                Cursor = oldCursor;
            }
        }

        private void OnGridEntryValueClick(object s, EventArgs e)
        {
            lastClickedEntry = (GridEntry)s;
            bool setSelectTime = s != selectedGridEntry;
            SelectGridEntry(lastClickedEntry, true);
            Edit.Focus();

            if (lastMouseDown != InvalidPosition)
            {
                // clear the row select time so we don't interpret this as a double click.
                //
                rowSelectTime = 0;

                Point editPoint = PointToScreen(lastMouseDown);
                editPoint = Edit.PointToClient(editPoint);
                User32.SendMessageW(Edit, User32.WM.LBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(editPoint.X, editPoint.Y));
                User32.SendMessageW(Edit, User32.WM.LBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(editPoint.X, editPoint.Y));
            }

            if (setSelectTime)
            {
                rowSelectTime = DateTime.Now.Ticks;
                rowSelectPos = PointToScreen(lastMouseDown);
            }
            else
            {
                rowSelectTime = 0;
                rowSelectPos = Point.Empty;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            cachedRowHeight = -1;

            if (Disposing || ParentInternal is null || ParentInternal.Disposing)
            {
                return;
            }

            _fontBold = null;    // fontBold is cached based on Font

            ToolTip.Font = Font;
            SetFlag(FlagNeedUpdateUIBasedOnFont, true);
            UpdateUIBasedOnFont(true);
            base.OnFontChanged(e);

            if (selectedGridEntry != null)
            {
                SelectGridEntry(selectedGridEntry, true);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Disposing || ParentInternal is null || ParentInternal.Disposing)
            {
                return;
            }

            if (Visible && ParentInternal != null)
            {
                SetConstants();
                if (selectedGridEntry != null)
                {
                    SelectGridEntry(selectedGridEntry, true);
                }
                if (toolTip != null)
                {
                    ToolTip.Font = Font;
                }
            }

            base.OnVisibleChanged(e);
        }

        // a GridEntry recreated its children
        protected virtual void OnRecreateChildren(object s, GridEntryRecreateChildrenEventArgs e)
        {
            GridEntry parent = (GridEntry)s;

            if (parent.Expanded)
            {
                GridEntry[] entries = new GridEntry[allGridEntries.Count];
                allGridEntries.CopyTo(entries, 0);

                // find the index of the gridEntry that fired the event in our main list.
                int parentIndex = -1;
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i] == parent)
                    {
                        parentIndex = i;
                        break;
                    }
                }

                Debug.Assert(parentIndex != -1, "parent GridEntry not found in allGridEntries");

                // clear our existing handlers
                ClearGridEntryEvents(allGridEntries, parentIndex + 1, e.OldChildCount);

                // resize the array if it's changed
                if (e.OldChildCount != e.NewChildCount)
                {
                    int newArraySize = entries.Length + (e.NewChildCount - e.OldChildCount);
                    GridEntry[] newEntries = new GridEntry[newArraySize];

                    // copy the existing entries up to the parent
                    Array.Copy(entries, 0, newEntries, 0, parentIndex + 1);

                    // copy the entries after the spot we'll be putting the new ones
                    Array.Copy(entries, parentIndex + e.OldChildCount + 1, newEntries, parentIndex + e.NewChildCount + 1, entries.Length - (parentIndex + e.OldChildCount + 1));

                    entries = newEntries;
                }

                // from that point, replace the children with tne new children.
                GridEntryCollection children = parent.Children;
                int childCount = children.Count;

                Debug.Assert(childCount == e.NewChildCount, "parent reports " + childCount + " new children, event reports " + e.NewChildCount);

                // replace the changed items
                for (int i = 0; i < childCount; i++)
                {
                    entries[parentIndex + i + 1] = children.GetEntry(i);
                }

                // reset the array, rehook the handlers.
                allGridEntries.Clear();
                allGridEntries.AddRange(entries);
                AddGridEntryEvents(allGridEntries, parentIndex + 1, childCount);
            }

            if (e.OldChildCount != e.NewChildCount)
            {
                totalProps = CountPropsFromOutline(topLevelGridEntries);
                SetConstants();
            }
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnResize");

            Rectangle newRect = ClientRectangle;
            int yDelta = lastClientRect == Rectangle.Empty ? 0 : newRect.Height - lastClientRect.Height;
            bool lastRow = (selectedRow + 1) == visibleRows;

            // if we are hiding or showing the scroll bar, update the selected row
            // or if we are changing widths
            //
            bool sbVisible = ScrollBar.Visible;

            if (!lastClientRect.IsEmpty && newRect.Width > lastClientRect.Width)
            {
                Rectangle rectInvalidate = new Rectangle(lastClientRect.Width - 1, 0, newRect.Width - lastClientRect.Width + 1, lastClientRect.Height);
                Invalidate(rectInvalidate);
            }

            if (!lastClientRect.IsEmpty && yDelta > 0)
            {
                Rectangle rectInvalidate = new Rectangle(0, lastClientRect.Height - 1, lastClientRect.Width, newRect.Height - lastClientRect.Height + 1);
                Invalidate(rectInvalidate);
            }

            int scroll = GetScrollOffset();
            SetScrollOffset(0);
            SetConstants();
            SetScrollOffset(scroll);

            if (DpiHelper.IsScalingRequirementMet)
            {
                SetFlag(FlagNeedUpdateUIBasedOnFont, true);
                UpdateUIBasedOnFont(true);
                base.OnFontChanged(e);
            }

            CommonEditorHide();

            LayoutWindow(false);

            bool selectionVisible = (selectedGridEntry != null && selectedRow >= 0 && selectedRow <= visibleRows);
            SelectGridEntry(selectedGridEntry, selectionVisible);
            lastClientRect = newRect;
        }

        private void OnScroll(object sender, ScrollEventArgs se)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnScroll(" + ScrollBar.Value.ToString(CultureInfo.InvariantCulture) + " -> " + se.NewValue.ToString(CultureInfo.InvariantCulture) + ")");

            if (!Commit() || !IsScrollValueValid(se.NewValue))
            {
                // cancel the move
                se.NewValue = ScrollBar.Value;
                return;
            }

            int oldRow = -1;
            GridEntry oldGridEntry = selectedGridEntry;
            if (selectedGridEntry != null)
            {
                oldRow = GetRowFromGridEntry(oldGridEntry);
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "OnScroll: SelectedGridEntry=" + oldGridEntry.PropertyLabel);
            }

            ScrollBar.Value = se.NewValue;
            if (oldGridEntry != null)
            {
                // we need to zero out the selected row so we don't try to commit again...since selectedRow is now bogus.
                selectedRow = -1;
                SelectGridEntry(oldGridEntry, (ScrollBar.Value == totalProps ? true : false));
                int newRow = GetRowFromGridEntry(oldGridEntry);
                if (oldRow != newRow)
                {
                    Invalidate();
                }
            }
            else
            {
                Invalidate();
            }
        }

        private void OnSysColorChange(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Color || e.Category == UserPreferenceCategory.Accessibility)
            {
                SetFlag(FlagNeedUpdateUIBasedOnFont, true);
            }
        }

        public virtual void PopupDialog(int row)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:PopupDialog");
            GridEntry gridEntry = GetGridEntryFromRow(row);
            if (gridEntry != null)
            {
                if (dropDownHolder != null && dropDownHolder.GetUsed())
                {
                    CloseDropDown();
                    return;
                }

                bool fBtnDropDown = gridEntry.NeedsDropDownButton;
                bool fEnum = gridEntry.Enumerable;
                bool fBtnDialog = gridEntry.NeedsCustomEditorButton;

                if (fEnum && !fBtnDropDown)
                {
                    DropDownListBox.Items.Clear();
                    object value = gridEntry.PropertyValue;
                    object[] rgItems = gridEntry.GetPropertyValueList();
                    int maxWidth = 0;

                    // The listbox draws with GDI, not GDI+.  So, we
                    // use a normal DC here.

                    using var hdc = new User32.GetDcScope(DropDownListBox.Handle);

                    var tm = new Gdi32.TEXTMETRICW();
                    int iSel = -1;

                    // This creates a copy of the given Font, and as such we need to delete it
                    Gdi32.HFONT hFont = (Gdi32.HFONT)Font.ToHfont();
                    using (var fontScope = new Gdi32.ObjectScope(hFont))
                    {
                        using var fontSelection = new Gdi32.SelectObjectScope(hdc, hFont);

                        iSel = GetCurrentValueIndex(gridEntry);
                        if (rgItems != null && rgItems.Length > 0)
                        {
                            string s;
                            Size textSize = new Size();

                            for (int i = 0; i < rgItems.Length; i++)
                            {
                                s = gridEntry.GetPropertyTextValue(rgItems[i]);
                                DropDownListBox.Items.Add(s);
                                Gdi32.GetTextExtentPoint32W(new HandleRef(DropDownListBox, hdc), s, s.Length, ref textSize);
                                maxWidth = Math.Max(textSize.Width, maxWidth);
                            }
                        }

                        Gdi32.GetTextMetricsW(hdc, ref tm);

                        // border + padding + scrollbar
                        maxWidth += 2 + tm.tmMaxCharWidth + SystemInformation.VerticalScrollBarWidth;
                    }

                    // Microsoft, 4/25/1998 - must check for -1 and not call the set...
                    if (iSel != -1)
                    {
                        DropDownListBox.SelectedIndex = iSel;
                    }

                    SetFlag(FlagDropDownCommit, false);
                    DropDownListBox.Height = Math.Max(tm.tmHeight + 2, Math.Min(maxListBoxHeight, DropDownListBox.PreferredHeight));
                    DropDownListBox.Width = Math.Max(maxWidth, GetRectangle(row, ROWVALUE).Width);
                    try
                    {
                        bool resizable = DropDownListBox.Items.Count > (DropDownListBox.Height / DropDownListBox.ItemHeight);
                        SetFlag(FlagResizableDropDown, resizable);
                        DropDownControl(DropDownListBox);
                    }
                    finally
                    {
                        SetFlag(FlagResizableDropDown, false);
                    }

                    Refresh();
                }
                else if (fBtnDialog || fBtnDropDown)
                {
                    try
                    {
                        SetFlag(FlagInPropertySet, true);
                        Edit.DisableMouseHook = true;

                        try
                        {
                            SetFlag(FlagResizableDropDown, gridEntry.UITypeEditor.IsDropDownResizable);
                            gridEntry.EditPropertyValue(this);
                        }
                        finally
                        {
                            SetFlag(FlagResizableDropDown, false);
                        }
                    }
                    finally
                    {
                        SetFlag(FlagInPropertySet, false);
                        Edit.DisableMouseHook = false;
                    }
                    Refresh();

                    // We can't do this because
                    // some dialogs are non-modal, and
                    // this will pull focus from them.
                    //
                    //if (fBtnDialog) {
                    //      this.Focus();
                    //}

                    if (FocusInside)
                    {
                        SelectGridEntry(gridEntry, false);
                    }
                }
            }
        }

        internal static void PositionTooltip(Control parent, GridToolTip ToolTip, Rectangle itemRect)
        {
            ToolTip.Visible = false;

            RECT rect = itemRect;

            User32.SendMessageW(ToolTip, (User32.WM)ComCtl32.TTM.ADJUSTRECT, (IntPtr)1, ref rect);

            // now offset it back to screen coords
            Point locPoint = parent.PointToScreen(new Point(rect.left, rect.top));

            ToolTip.Location = locPoint;   // set the position once so it updates it's size with it's real width.

            int overHang = (ToolTip.Location.X + ToolTip.Size.Width) - SystemInformation.VirtualScreen.Width;
            if (overHang > 0)
            {
                locPoint.X -= overHang;
                ToolTip.Location = locPoint;
            }

            // tell the control we've repositioned it.
            ToolTip.Visible = true;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ProcessDialogKey");
            if (HasEntries)
            {
                Keys keyCode = keyData & Keys.KeyCode;
                switch (keyCode)
                {
                    case Keys.F4:
                        if (FocusInside)
                        {
                            return OnF4(this);
                        }
                        break;

                    case Keys.Tab:

                        if (((keyData & Keys.Control) != 0) ||
                            ((keyData & Keys.Alt) != 0))
                        {
                            break;
                        }

                        bool forward = (keyData & Keys.Shift) == 0;

                        Control focusedControl = Control.FromHandle(User32.GetFocus());

                        if (focusedControl is null || !IsMyChild(focusedControl))
                        {
                            if (forward)
                            {
                                TabSelection();
                                focusedControl = Control.FromHandle(User32.GetFocus());
                                // make sure the value actually took the focus
                                if (IsMyChild(focusedControl))
                                {
                                    return true;
                                }
                                else
                                {
                                    return base.ProcessDialogKey(keyData);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            // one of our editors has focus

                            if (Edit.Focused)
                            {
                                if (forward)
                                {
                                    if (DropDownButton.Visible)
                                    {
                                        DropDownButton.Focus();
                                        return true;
                                    }
                                    else if (DialogButton.Visible)
                                    {
                                        DialogButton.Focus();
                                        return true;
                                    }
                                    // fall through
                                }
                                else
                                {
                                    SelectGridEntry(GetGridEntryFromRow(selectedRow), false);
                                    return true;
                                }
                            }
                            else if (DialogButton.Focused || DropDownButton.Focused)
                            {
                                if (!forward && Edit.Visible)
                                {
                                    Edit.Focus();
                                    return true;
                                }
                                // fall through
                            }
                        }
                        break;
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        return false;
                    case Keys.Return:
                        if (DialogButton.Focused || DropDownButton.Focused)
                        {
                            OnBtnClick((DialogButton.Focused ? DialogButton : DropDownButton), EventArgs.Empty);
                            return true;
                        }
                        else if (selectedGridEntry != null && selectedGridEntry.Expandable)
                        {
                            SetExpand(selectedGridEntry, !selectedGridEntry.InternalExpanded);
                            return true;
                        }
                        break;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        protected virtual void RecalculateProps()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:RecalculateProps");
            int props = CountPropsFromOutline(topLevelGridEntries);
            if (totalProps != props)
            {
                totalProps = props;
                ClearGridEntryEvents(allGridEntries, 0, -1);
                allGridEntries = null;
            }
        }

        internal /*public virtual*/ void RecursivelyExpand(GridEntry gridEntry, bool fInit, bool expand, int maxExpands)
        {
            if (gridEntry is null || (expand && --maxExpands < 0))
            {
                return;
            }

            SetExpand(gridEntry, expand);

            GridEntryCollection rgipes = gridEntry.Children;
            if (rgipes != null)
            {
                for (int i = 0; i < rgipes.Count; i++)
                {
                    RecursivelyExpand(rgipes.GetEntry(i), false, expand, maxExpands);
                }
            }

            if (fInit)
            {
                GridEntry ipeSelect = selectedGridEntry;
                Refresh();
                SelectGridEntry(ipeSelect, false);
                Invalidate();
            }
        }

        public override void Refresh()
        {
            Refresh(false, -1, -1);

            //resetting gridoutline rect to recalculate before repaint when viewsort property changed. This is necessary especially when user
            // changes sort and move to a secondary monitor with different DPI and change view sort back to original.
            if (topLevelGridEntries != null && DpiHelper.IsScalingRequirementMet)
            {
                var outlineRectIconSize = GetOutlineIconSize();
                foreach (GridEntry gridentry in topLevelGridEntries)
                {
                    if (gridentry.OutlineRect.Height != outlineRectIconSize || gridentry.OutlineRect.Width != outlineRectIconSize)
                    {
                        ResetOutline(gridentry);
                    }
                }
            }

            // make sure we got everything
            Invalidate();
        }

        public void Refresh(bool fullRefresh)
        {
            Refresh(fullRefresh, -1, -1);
        }

        GridPositionData positionData;

        private void Refresh(bool fullRefresh, int rowStart, int rowEnd)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Refresh");
            Debug.WriteLineIf(GridViewDebugPaint.TraceVerbose, "Refresh called for rows " + rowStart.ToString(CultureInfo.InvariantCulture) + " through " + rowEnd.ToString(CultureInfo.InvariantCulture));
            SetFlag(FlagNeedsRefresh, true);
            GridEntry gridEntry = null;

            // There are cases here where the grid could get be disposed.
            // so just bail.
            if (IsDisposed)
            {
                return;
            }

            bool pageInGridEntry = true;

            if (rowStart == -1)
            {
                rowStart = 0;
            }

            if (fullRefresh || OwnerGrid.HavePropEntriesChanged())
            {
                if (HasEntries && !GetInPropertySet() && !Commit())
                {
                    OnEscape(this);
                }

                int oldLength = totalProps;
                object oldObject = topLevelGridEntries is null || topLevelGridEntries.Count == 0 ? null : ((GridEntry)topLevelGridEntries[0]).GetValueOwner();

                // walk up to the main IPE and refresh it.
                if (fullRefresh)
                {
                    OwnerGrid.RefreshProperties(true);
                }

                if (oldLength > 0 && !GetFlag(FlagNoDefault))
                {
                    positionData = CaptureGridPositionData();
                    CommonEditorHide(true);
                }

                UpdateHelpAttributes(selectedGridEntry, null);
                selectedGridEntry = null;
                SetFlag(FlagIsNewSelection, true);
                topLevelGridEntries = OwnerGrid.GetPropEntries();

                ClearGridEntryEvents(allGridEntries, 0, -1);
                allGridEntries = null;
                RecalculateProps();

                int newLength = totalProps;
                if (newLength > 0)
                {
                    if (newLength < oldLength)
                    {
                        SetScrollbarLength();
                        SetScrollOffset(0);
                    }

                    SetConstants();

                    if (positionData != null)
                    {
                        gridEntry = positionData.Restore(this);

                        // Upon restoring the grid entry position, we don't
                        // want to page it in
                        //
                        object newObject = topLevelGridEntries is null || topLevelGridEntries.Count == 0 ? null : ((GridEntry)topLevelGridEntries[0]).GetValueOwner();
                        pageInGridEntry = (gridEntry is null) || oldLength != newLength || newObject != oldObject;
                    }

                    if (gridEntry is null)
                    {
                        gridEntry = OwnerGrid.GetDefaultGridEntry();
                        SetFlag(FlagNoDefault, gridEntry is null && totalProps > 0);
                    }

                    InvalidateRows(rowStart, rowEnd);
                    if (gridEntry is null)
                    {
                        selectedRow = 0;
                        selectedGridEntry = GetGridEntryFromRow(selectedRow);
                    }
                }
                else if (oldLength == 0)
                {
                    return;
                }
                else
                {
                    SetConstants();
                }

                // Release the old positionData which contains reference to previous selected objects.
                positionData = null;
                lastClickedEntry = null;
            }

            if (!HasEntries)
            {
                CommonEditorHide(selectedRow != -1);
                OwnerGrid.SetStatusBox(null, null);
                SetScrollOffset(0);
                selectedRow = -1;
                Invalidate();
                return;
            }
            // in case we added or removed properties

            OwnerGrid.ClearValueCaches();

            InvalidateRows(rowStart, rowEnd);

            if (gridEntry != null)
            {
                SelectGridEntry(gridEntry, pageInGridEntry);
            }
        }

        public virtual void Reset()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Reset");
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            gridEntry.ResetPropertyValue();
            SelectRow(selectedRow);
        }

        protected virtual void ResetOrigin(Graphics g)
        {
            g.ResetTransform();
        }

        internal void RestoreHierarchyState(ArrayList expandedItems)
        {
            if (expandedItems is null)
            {
                return;
            }

            foreach (GridEntryCollection gec in expandedItems)
            {
                FindEquivalentGridEntry(gec);
            }
        }

        public virtual DialogResult RunDialog(Form dialog)
        {
            return ShowDialog(dialog);
        }

        internal ArrayList SaveHierarchyState(GridEntryCollection entries)
        {
            return SaveHierarchyState(entries, null);
        }

        private ArrayList SaveHierarchyState(GridEntryCollection entries, ArrayList expandedItems)
        {
            if (entries is null)
            {
                return new ArrayList();
            }

            if (expandedItems is null)
            {
                expandedItems = new ArrayList();
            }

            for (int i = 0; i < entries.Count; i++)
            {
                if (((GridEntry)entries[i]).InternalExpanded)
                {
                    GridEntry entry = entries.GetEntry(i);
                    expandedItems.Add(GetGridEntryHierarchy(entry.Children.GetEntry(0)));
                    SaveHierarchyState(entry.Children, expandedItems);
                }
            }

            return expandedItems;
        }

        // Scroll to the new offset
        private bool ScrollRows(int newOffset)
        {
            GridEntry ipeCur = selectedGridEntry;

            if (!IsScrollValueValid(newOffset) || !Commit())
            {
                return false;
            }

            bool showEdit = Edit.Visible;
            bool showBtnDropDown = DropDownButton.Visible;
            bool showBtnEdit = DialogButton.Visible;

            Edit.Visible = false;
            DialogButton.Visible = false;
            DropDownButton.Visible = false;

            SetScrollOffset(newOffset);

            if (ipeCur != null)
            {
                int curRow = GetRowFromGridEntry(ipeCur);
                if (curRow >= 0 && curRow < visibleRows - 1)
                {
                    Edit.Visible = showEdit;
                    DialogButton.Visible = showBtnEdit;
                    DropDownButton.Visible = showBtnDropDown;
                    SelectGridEntry(ipeCur, true);
                }
                else
                {
                    CommonEditorHide();
                }
            }
            else
            {
                CommonEditorHide();
            }

            Invalidate();
            return true;
        }

        private void SelectEdit(bool caretAtEnd)
        {
            if (edit != null)
            {
                Edit.SelectAll();
            }
        }

        // select functions... selectGridEntry and selectRow will select a Row
        // and install the appropriate editors.
        //
        internal /*protected virtual*/ void SelectGridEntry(GridEntry gridEntry, bool fPageIn)
        {
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SelectGridEntry(" + gridEntry.PropertyLabel + ")");

            int row = GetRowFromGridEntry(gridEntry);
            if (row + GetScrollOffset() < 0)
            {
                // throw exception? return false?
                return;
            }

            int maxRows = (int)Math.Ceiling(((double)GetOurSize().Height) / (1 + RowHeight));

            // Determine whether or not we need to page-in this GridEntry
            //

            if (!fPageIn || (row >= 0 && row < (maxRows - 1)))
            {
                // No need to page-in: either fPageIn is false or the row is already in view
                //
                SelectRow(row);
            }
            else
            {
                // Page-in the selected GridEntry
                //

                selectedRow = -1; // clear the selected row since it's no longer a valid number

                int cOffset = GetScrollOffset();
                if (row < 0)
                {
                    SetScrollOffset(row + cOffset);
                    Invalidate();
                    SelectRow(0);
                }
                else
                {
                    // try to put it one row up from the bottom
                    int newOffset = row + cOffset - (maxRows - 2);

                    if (newOffset >= ScrollBar.Minimum && newOffset < ScrollBar.Maximum)
                    {
                        SetScrollOffset(newOffset);
                    }
                    Invalidate();
                    SelectGridEntry(gridEntry, false);
                }
            }
        }

        private void SelectRow(int row)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SelectRow(" + row.ToString(CultureInfo.InvariantCulture) + ")");

            if (!GetFlag(FlagIsNewSelection))
            {
                if (FocusInside)
                {
                    // If we're in an error state, we want to bail out of this.
                    if (errorState != ERROR_NONE || (row != selectedRow && !Commit()))
                    {
                        return;
                    }
                }
                else
                {
                    Focus();
                }
            }

            GridEntry gridEntry = GetGridEntryFromRow(row);

            // Update our reset command.
            //
            if (row != selectedRow)
            {
                UpdateResetCommand(gridEntry);
            }

            if (GetFlag(FlagIsNewSelection) && GetGridEntryFromRow(selectedRow) is null)
            {
                CommonEditorHide();
            }

            UpdateHelpAttributes(selectedGridEntry, gridEntry);

            // tell the old selection it's not focused any more
            if (selectedGridEntry != null)
            {
                selectedGridEntry.Focus = false;
            }

            // selection not visible.
            if (row < 0 || row >= visibleRows)
            {
                CommonEditorHide();
                selectedRow = row;
                selectedGridEntry = gridEntry;
                Refresh();
                return;
            }

            // leave current selection.
            if (gridEntry is null)
            {
                return;
            }

            bool newRow = false;
            int oldSel = selectedRow;
            if (selectedRow != row || !gridEntry.Equals(selectedGridEntry))
            {
                CommonEditorHide();
                newRow = true;
            }

            if (!newRow)
            {
                CloseDropDown();
            }

            Rectangle rect = GetRectangle(row, ROWVALUE);
            string s = gridEntry.GetPropertyTextValue();

            // what components are we using?
            bool fBtnDropDown = gridEntry.NeedsDropDownButton | gridEntry.Enumerable;
            bool fBtnDialog = gridEntry.NeedsCustomEditorButton;
            bool fEdit = gridEntry.IsTextEditable;
            bool fPaint = gridEntry.IsCustomPaint;

            rect.X += 1;
            rect.Width -= 1;

            // we want to allow builders on read-only properties
            if ((fBtnDialog || fBtnDropDown) && !gridEntry.ShouldRenderReadOnly && FocusInside)
            {
                Control btn = fBtnDropDown ? (Control)DropDownButton : (Control)DialogButton;
                Size sizeBtn = DpiHelper.IsScalingRequirementMet ? new Size(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight) :
                                                                               new Size(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);
                Rectangle rectTarget = new Rectangle(rect.X + rect.Width - sizeBtn.Width,
                                                      rect.Y,
                                                      sizeBtn.Width, rect.Height);
                CommonEditorUse(btn, rectTarget);
                sizeBtn = btn.Size;
                rect.Width -= (sizeBtn.Width);
                btn.Invalidate();
            }

            // if we're painting the value, size the rect between the button and the painted value
            if (fPaint)
            {
                rect.X += paintIndent + 1;
                rect.Width -= paintIndent + 1;
            }
            else
            {
                rect.X += EDIT_INDENT + 1; // +1 to compensate for where GDI+ draws it's string relative to the rect.
                rect.Width -= EDIT_INDENT + 1;
            }

            if ((GetFlag(FlagIsNewSelection) || !Edit.Focused) && (s != null && !s.Equals(Edit.Text)))
            {
                Edit.Text = s;
                originalTextValue = s;
                Edit.SelectionStart = 0;
                Edit.SelectionLength = 0;
            }
            Edit.AccessibleName = gridEntry.Label;

            switch (inheritRenderMode)
            {
                case RENDERMODE_BOLD:
                    if (gridEntry.ShouldSerializePropertyValue())
                    {
                        Edit.Font = GetBoldFont();
                    }
                    else
                    {
                        Edit.Font = Font;
                    }
                    break;
                case RENDERMODE_LEFTDOT:
                    if (gridEntry.ShouldSerializePropertyValue())
                    {
                        rect.X += (LEFTDOT_SIZE * 2);
                        rect.Width -= (LEFTDOT_SIZE * 2);
                    }
                    // nothing
                    break;
                case RENDERMODE_TRIANGLE:
                    // nothing
                    break;
            }

            if (GetFlag(FlagIsSplitterMove) || !gridEntry.HasValue || !FocusInside)
            {
                Edit.Visible = false;
            }
            else
            {
                rect.Offset(1, 1);
                rect.Height -= 1;
                rect.Width -= 1;
                CommonEditorUse(Edit, rect);
                bool drawReadOnly = gridEntry.ShouldRenderReadOnly;
                Edit.ForeColor = drawReadOnly ? GrayTextColor : ForeColor;
                Edit.BackColor = BackColor;
                Edit.ReadOnly = drawReadOnly || !gridEntry.IsTextEditable;
                Edit.UseSystemPasswordChar = gridEntry.ShouldRenderPassword;
            }

            GridEntry oldSelectedGridEntry = selectedGridEntry;
            selectedRow = row;
            selectedGridEntry = gridEntry;
            OwnerGrid.SetStatusBox(gridEntry.PropertyLabel, gridEntry.PropertyDescription);

            // tell the new focused item that it now has focus
            if (selectedGridEntry != null)
            {
                selectedGridEntry.Focus = FocusInside;
            }

            if (!GetFlag(FlagIsNewSelection))
            {
                Focus();
            }

            //

            InvalidateRow(oldSel);

            InvalidateRow(row);
            if (FocusInside)
            {
                SetFlag(FlagIsNewSelection, false);
            }

            try
            {
                if (selectedGridEntry != oldSelectedGridEntry)
                {
                    OwnerGrid.OnSelectedGridItemChanged(oldSelectedGridEntry, selectedGridEntry);
                }
            }
            catch
            {
            }
        }

        public virtual void SetConstants()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SetConstants");
            Size size = GetOurSize();

            visibleRows = (int)Math.Ceiling(((double)size.Height) / (1 + RowHeight));

            size = GetOurSize();

            if (size.Width >= 0)
            {
                labelRatio = Math.Max(Math.Min(labelRatio, 9), 1.1);
                labelWidth = ptOurLocation.X + (int)((double)size.Width / labelRatio);
            }

            int oldWidth = labelWidth;

            bool adjustWidth = SetScrollbarLength();
            GridEntryCollection rgipesAll = GetAllGridEntries();
            if (rgipesAll != null)
            {
                int scroll = GetScrollOffset();
                if ((scroll + visibleRows) >= rgipesAll.Count)
                {
                    visibleRows = rgipesAll.Count - scroll;
                }
            }

            if (adjustWidth && size.Width >= 0)
            {
                labelRatio = ((double)GetOurSize().Width / (double)(oldWidth - ptOurLocation.X));
                //labelWidth = loc.X + (int) ((double)size.Width / labelRatio);
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tsize       :" + size.ToString());
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tlocation   :" + ptOurLocation.ToString());
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tvisibleRows:" + (visibleRows).ToString(CultureInfo.InvariantCulture));
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tlabelWidth :" + (labelWidth).ToString(CultureInfo.InvariantCulture));
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tlabelRatio :" + (labelRatio).ToString(CultureInfo.InvariantCulture));
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\trowHeight  :" + (RowHeight).ToString(CultureInfo.InvariantCulture));
#if DEBUG
            if (rgipesAll is null)
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tIPE Count  :(null)");
            }
            else
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tIPE Count  :" + (rgipesAll.Count).ToString(CultureInfo.InvariantCulture));
            }
#endif
        }

        private void SetCommitError(short error)
        {
            SetCommitError(error, error == ERROR_THROWN);
        }

        private void SetCommitError(short error, bool capture)
        {
#if DEBUG
            if (CompModSwitches.DebugGridView.TraceVerbose)
            {
                string err = "UNKNOWN!";
                switch (error)
                {
                    case ERROR_NONE:
                        err = "ERROR_NONE";
                        break;
                    case ERROR_THROWN:
                        err = "ERROR_THROWN";
                        break;
                    case ERROR_MSGBOX_UP:
                        err = "ERROR_MSGBOX_UP";
                        break;
                }
                Debug.WriteLine("PropertyGridView:SetCommitError(error=" + err + ", capture=" + capture.ToString() + ")");
            }
#endif
            errorState = error;
            if (error != ERROR_NONE)
            {
                CancelSplitterMove();
            }

            Edit.HookMouseDown = capture;
        }

        internal /*public virtual*/ void SetExpand(GridEntry gridEntry, bool value)
        {
            if (gridEntry != null && gridEntry.Expandable)
            {
                int row = GetRowFromGridEntry(gridEntry);
                int countFromEnd = visibleRows - row;
                int curRow = selectedRow;

                // if the currently selected row is below us, we need to commit now
                // or the offsets will be wrong
                if (selectedRow != -1 && row < selectedRow && Edit.Visible)
                {
                    // this will cause the commit
                    Focus();
                }

                int offset = GetScrollOffset();
                int items = totalProps;

                gridEntry.InternalExpanded = value;

                var oldExpandedState = value ? UiaCore.ExpandCollapseState.Collapsed : UiaCore.ExpandCollapseState.Expanded;
                var newExpandedState = value ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                selectedGridEntry?.AccessibilityObject?.RaiseAutomationPropertyChangedEvent(
                    UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                    oldExpandedState,
                    newExpandedState);

                RecalculateProps();
                GridEntry ipeSelect = selectedGridEntry;
                if (!value)
                {
                    for (GridEntry ipeCur = ipeSelect; ipeCur != null; ipeCur = ipeCur.ParentGridEntry)
                    {
                        if (ipeCur.Equals(gridEntry))
                        {
                            ipeSelect = gridEntry;
                        }
                    }
                }
                row = GetRowFromGridEntry(gridEntry);

                SetConstants();

                int newItems = totalProps - items;

                if (value && newItems > 0 && newItems < visibleRows && (row + (newItems)) >= visibleRows && newItems < curRow)
                {
                    // scroll to show the newly opened items.
                    SetScrollOffset((totalProps - items) + offset);
                }

                Invalidate();

                SelectGridEntry(ipeSelect, false);

                int scroll = GetScrollOffset();
                SetScrollOffset(0);
                SetConstants();
                SetScrollOffset(scroll);
            }
        }

        private void SetFlag(short flag, bool value)
        {
            if (value)
            {
                flags = (short)((ushort)flags | (ushort)flag);
            }
            else
            {
                flags &= (short)~flag;
            }
        }

        public virtual void SetScrollOffset(int cOffset)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SetScrollOffset(" + cOffset.ToString(CultureInfo.InvariantCulture) + ")");
            int posNew = Math.Max(0, Math.Min(totalProps - visibleRows + 1, cOffset));
            int posOld = ScrollBar.Value;
            if (posNew != posOld && IsScrollValueValid(posNew) && visibleRows > 0)
            {
                ScrollBar.Value = posNew;
                Invalidate();
                selectedRow = GetRowFromGridEntry(selectedGridEntry);
            }
        }

        // C#r
        internal virtual bool _Commit()
        {
            return Commit();
        }

        private bool Commit()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Commit()");

            if (errorState == ERROR_MSGBOX_UP)
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Commit() returning false because an error has been thrown or we are in a property set");
                return false;
            }

            if (!NeedsCommit)
            {
                SetCommitError(ERROR_NONE);
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Commit() returning true because no change has been made");
                return true;
            }

            if (GetInPropertySet())
            {
                return false;
            }

            GridEntry ipeCur = GetGridEntryFromRow(selectedRow);
            if (ipeCur is null)
            {
                return true;
            }
            bool success = false;
            try
            {
                success = CommitText(Edit.Text);
            }
            finally
            {
                if (!success)
                {
                    Edit.Focus();
                    SelectEdit(false);
                }
                else
                {
                    SetCommitError(ERROR_NONE);
                }
            }
            return success;
        }

        private bool CommitValue(object value)
        {
            GridEntry ipeCur = selectedGridEntry;

            if (selectedGridEntry is null && selectedRow != -1)
            {
                ipeCur = GetGridEntryFromRow(selectedRow);
            }

            if (ipeCur is null)
            {
                Debug.Fail("Committing with no selected row!");
                return true;
            }

            return CommitValue(ipeCur, value);
        }

        internal bool CommitValue(GridEntry ipeCur, object value, bool closeDropDown = true)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommitValue(" + (value is null ? "null" : value.ToString()) + ")");

            int propCount = ipeCur.ChildCount;
            bool capture = Edit.HookMouseDown;
            object originalValue = null;

            try
            {
                originalValue = ipeCur.PropertyValue;
            }
            catch
            {
                // if the getter is failing, we still want to let
                // the set happen.
            }

            try
            {
                try
                {
                    SetFlag(FlagInPropertySet, true);

                    //if this propentry is enumerable, then once a value is selected from the editor,
                    //we'll want to close the drop down (like true/false).  Otherwise, if we're
                    //working with Anchor for ex., then we should be able to select different values
                    //from the editor, without having it close every time.
                    if (ipeCur != null &&
                        ipeCur.Enumerable &&
                        closeDropDown)
                    {
                        CloseDropDown();
                    }

                    try
                    {
                        Edit.DisableMouseHook = true;
                        ipeCur.PropertyValue = value;
                    }
                    finally
                    {
                        Edit.DisableMouseHook = false;
                        Edit.HookMouseDown = capture;
                    }
                }
                catch (Exception ex)
                {
                    SetCommitError(ERROR_THROWN);
                    ShowInvalidMessage(ipeCur.PropertyLabel, value, ex);
                    return false;
                }
            }
            finally
            {
                SetFlag(FlagInPropertySet, false);
            }

            SetCommitError(ERROR_NONE);

            string text = ipeCur.GetPropertyTextValue();
            if (!string.Equals(text, Edit.Text))
            {
                Edit.Text = text;
                Edit.SelectionStart = 0;
                Edit.SelectionLength = 0;
            }
            originalTextValue = text;

            // Update our reset command.
            //
            UpdateResetCommand(ipeCur);

            if (ipeCur.ChildCount != propCount)
            {
                ClearGridEntryEvents(allGridEntries, 0, -1);
                allGridEntries = null;
                SelectGridEntry(ipeCur, true);
            }

            // in case this guy got disposed...
            if (ipeCur.Disposed)
            {
                bool editfocused = (edit != null && edit.Focused);

                // reselect the row to find the replacement.
                //
                SelectGridEntry(ipeCur, true);
                ipeCur = selectedGridEntry;

                if (editfocused && edit != null)
                {
                    edit.Focus();
                }
            }

            OwnerGrid.OnPropertyValueSet(ipeCur, originalValue);

            return true;
        }

        private bool CommitText(string text)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommitValue(" + (text is null ? "null" : text.ToString()) + ")");

            object value = null;

            GridEntry ipeCur = selectedGridEntry;

            if (selectedGridEntry is null && selectedRow != -1)
            {
                ipeCur = GetGridEntryFromRow(selectedRow);
            }

            if (ipeCur is null)
            {
                Debug.Fail("Committing with no selected row!");
                return true;
            }

            try
            {
                value = ipeCur.ConvertTextToValue(text);
            }
            catch (Exception ex)
            {
                SetCommitError(ERROR_THROWN);
                ShowInvalidMessage(ipeCur.PropertyLabel, text, ex);
                return false;
            }

            SetCommitError(ERROR_NONE);

            return CommitValue(value);
        }

        internal void ReverseFocus()
        {
            if (selectedGridEntry is null)
            {
                Focus();
            }
            else
            {
                SelectGridEntry(selectedGridEntry, true);

                if (DialogButton.Visible)
                {
                    DialogButton.Focus();
                }
                else if (DropDownButton.Visible)
                {
                    DropDownButton.Focus();
                }
                else if (Edit.Visible)
                {
                    Edit.SelectAll();
                    Edit.Focus();
                }
            }
        }

        private bool SetScrollbarLength()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SetScrollBarLength");
            bool sbChange = false;
            if (totalProps != -1)
            {
                if (totalProps < visibleRows)
                {
                    SetScrollOffset(0);
                }
                else if (GetScrollOffset() > totalProps)
                {
                    SetScrollOffset((totalProps + 1) - visibleRows);
                }

                bool fHidden = !ScrollBar.Visible;
                if (visibleRows > 0)
                {
                    ScrollBar.LargeChange = visibleRows - 1;
                }
                ScrollBar.Maximum = Math.Max(0, totalProps - 1);
                if (fHidden != (totalProps < visibleRows))
                {
                    sbChange = true;
                    ScrollBar.Visible = fHidden;
                    Size size = GetOurSize();
                    if (labelWidth != -1 && size.Width > 0)
                    {
                        if (labelWidth > ptOurLocation.X + size.Width)
                        {
                            labelWidth = ptOurLocation.X + (int)((double)size.Width / labelRatio);
                        }
                        else
                        {
                            labelRatio = ((double)GetOurSize().Width / (double)(labelWidth - ptOurLocation.X));
                        }
                    }
                    Invalidate();
                }
            }
            return sbChange;
        }

        /// <summary>
        ///  Shows the given dialog, and returns its dialog result.  You should always
        ///  use this method rather than showing the dialog directly, as this will
        ///  properly position the dialog and provide it a dialog owner.
        /// </summary>
        public DialogResult /* IWindowsFormsEditorService. */ ShowDialog(Form dialog)
        {
            // try to shift down if sitting right on top of existing owner.
            if (dialog.StartPosition == FormStartPosition.CenterScreen)
            {
                Control topControl = this;
                if (topControl != null)
                {
                    while (topControl.ParentInternal != null)
                    {
                        topControl = topControl.ParentInternal;
                    }
                    if (topControl.Size.Equals(dialog.Size))
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        Point location = topControl.Location;
                        //
                        location.Offset(25, 25);
                        dialog.Location = location;
                    }
                }
            }

            IntPtr priorFocus = User32.GetFocus();

            IUIService service = (IUIService)GetService(typeof(IUIService));
            DialogResult result;
            if (service != null)
            {
                result = service.ShowDialog(dialog);
            }
            else
            {
                result = dialog.ShowDialog(this);
            }

            if (priorFocus != IntPtr.Zero)
            {
                User32.SetFocus(priorFocus);
            }

            return result;
        }

        private void ShowFormatExceptionMessage(string propName, object value, Exception ex)
        {
            if (value is null)
            {
                value = "(null)";
            }

            if (propName is null)
            {
                propName = "(unknown)";
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ShowFormatExceptionMessage(prop=" + propName + ")");

            // we have to uninstall our hook so the user can push the button!
            bool hooked = Edit.HookMouseDown;
            Edit.DisableMouseHook = true;
            SetCommitError(ERROR_MSGBOX_UP, false);

            // Before invoking the error dialog, flush all mouse messages in the message queue.
            // Otherwise the click that triggered the error will still be in the queue, and will get eaten by the dialog,
            // potentially causing an accidental button click. Problem occurs because we trap clicks using a system hook,
            // which usually discards the message by returning 1 to GetMessage(). But this won't occur until after the
            // error dialog gets closed, which is much too late.
            var mouseMsg = new User32.MSG();
            while (User32.PeekMessageW(ref mouseMsg,
                IntPtr.Zero,
                (User32.WM)User32.WM.MOUSEFIRST,
                (User32.WM)User32.WM.MOUSELAST,
                User32.PM.REMOVE).IsTrue())
            {
                // No-op.
            }

            // These things are just plain useless.
            //
            if (ex is Reflection.TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            // Try to find an exception message to display
            //
            string exMessage = ex.Message;

            bool revert = false;

            while (exMessage is null || exMessage.Length == 0)
            {
                ex = ex.InnerException;
                if (ex is null)
                {
                    break;
                }
                exMessage = ex.Message;
            }

            IUIService uiSvc = (IUIService)GetService(typeof(IUIService));

            ErrorDialog.Message = SR.PBRSFormatExceptionMessage;
            ErrorDialog.Text = SR.PBRSErrorTitle;
            ErrorDialog.Details = exMessage;

            if (uiSvc != null)
            {
                revert = (DialogResult.Cancel == uiSvc.ShowDialog(ErrorDialog));
            }
            else
            {
                revert = (DialogResult.Cancel == ShowDialog(ErrorDialog));
            }

            Edit.DisableMouseHook = false;

            if (hooked)
            {
                SelectGridEntry(selectedGridEntry, true);
            }
            SetCommitError(ERROR_THROWN, hooked);

            if (revert)
            {
                OnEscape(Edit);
            }
        }

        internal void ShowInvalidMessage(string propName, object value, Exception ex)
        {
            if (value is null)
            {
                value = "(null)";
            }

            if (propName is null)
            {
                propName = "(unknown)";
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ShowInvalidMessage(prop=" + propName + ")");

            // we have to uninstall our hook so the user can push the button!
            bool hooked = Edit.HookMouseDown;
            Edit.DisableMouseHook = true;
            SetCommitError(ERROR_MSGBOX_UP, false);

            // Before invoking the error dialog, flush all mouse messages in the message queue.
            // Otherwise the click that triggered the error will still be in the queue, and will get eaten by the dialog,
            // potentially causing an accidental button click. Problem occurs because we trap clicks using a system hook,
            // which usually discards the message by returning 1 to GetMessage(). But this won't occur until after the
            // error dialog gets closed, which is much too late.
            var mouseMsg = new User32.MSG();
            while (User32.PeekMessageW(ref mouseMsg, IntPtr.Zero, User32.WM.MOUSEFIRST, User32.WM.MOUSELAST, User32.PM.REMOVE).IsTrue())
            {
                // No-op.
            }

            // These things are just plain useless.
            //
            if (ex is Reflection.TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            // Try to find an exception message to display
            //
            string exMessage = ex.Message;

            bool revert = false;

            while (exMessage is null || exMessage.Length == 0)
            {
                ex = ex.InnerException;
                if (ex is null)
                {
                    break;
                }
                exMessage = ex.Message;
            }

            IUIService uiSvc = (IUIService)GetService(typeof(IUIService));

            ErrorDialog.Message = SR.PBRSErrorInvalidPropertyValue;
            ErrorDialog.Text = SR.PBRSErrorTitle;
            ErrorDialog.Details = exMessage;

            if (uiSvc != null)
            {
                revert = (DialogResult.Cancel == uiSvc.ShowDialog(ErrorDialog));
            }
            else
            {
                revert = (DialogResult.Cancel == ShowDialog(ErrorDialog));
            }

            Edit.DisableMouseHook = false;

            if (hooked)
            {
                SelectGridEntry(selectedGridEntry, true);
            }
            SetCommitError(ERROR_THROWN, hooked);

            if (revert)
            {
                OnEscape(Edit);
            }
        }

        private bool SplitterInside(int x, int y)
        {
            return (Math.Abs(x - InternalLabelWidth) < 4);
        }

        private void TabSelection()
        {
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            if (Edit.Visible)
            {
                Edit.Focus();
                SelectEdit(false);
            }
            else if (dropDownHolder != null && dropDownHolder.Visible)
            {
                dropDownHolder.FocusComponent();
                return;
            }
            else if (currentEditor != null)
            {
                currentEditor.Focus();
            }

            return;
        }

        internal void RemoveSelectedEntryHelpAttributes()
        {
            UpdateHelpAttributes(selectedGridEntry, null);
        }

        private void UpdateHelpAttributes(GridEntry oldEntry, GridEntry newEntry)
        {
            // Update the help context with the current property
            //
            IHelpService hsvc = GetHelpService();
            if (hsvc is null || oldEntry == newEntry)
            {
                return;
            }

            GridEntry temp = oldEntry;
            if (oldEntry != null && !oldEntry.Disposed)
            {
                while (temp != null)
                {
                    hsvc.RemoveContextAttribute("Keyword", temp.HelpKeyword);
                    temp = temp.ParentGridEntry;
                }
            }

            if (newEntry != null)
            {
                temp = newEntry;

                UpdateHelpAttributes(hsvc, temp, true);
            }
        }

        private void UpdateHelpAttributes(IHelpService helpSvc, GridEntry entry, bool addAsF1)
        {
            if (entry is null)
            {
                return;
            }

            UpdateHelpAttributes(helpSvc, entry.ParentGridEntry, false);
            string helpKeyword = entry.HelpKeyword;
            if (helpKeyword != null)
            {
                helpSvc.AddContextAttribute("Keyword", helpKeyword, addAsF1 ? HelpKeywordType.F1Keyword : HelpKeywordType.GeneralKeyword);
            }
        }

        private void UpdateUIBasedOnFont(bool layoutRequired)
        {
            if (IsHandleCreated && GetFlag(FlagNeedUpdateUIBasedOnFont))
            {
                try
                {
                    if (listBox != null)
                    {
                        DropDownListBox.ItemHeight = RowHeight + 2;
                    }

                    if (btnDropDown != null)
                    {
                        var isScalingRequirementMet = DpiHelper.IsScalingRequirementMet;
                        if (isScalingRequirementMet)
                        {
                            btnDropDown.Size = new Size(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight);
                        }
                        else
                        {
                            btnDropDown.Size = new Size(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);
                        }

                        if (btnDialog != null)
                        {
                            DialogButton.Size = DropDownButton.Size;
                            if (isScalingRequirementMet)
                            {
                                btnDialog.Image = CreateResizedBitmap("dotdotdot", DOTDOTDOT_ICONWIDTH, DOTDOTDOT_ICONHEIGHT);
                            }
                        }
                        if (isScalingRequirementMet)
                        {
                            btnDropDown.Image = CreateResizedBitmap("Arrow", DOWNARROW_ICONWIDTH, DOWNARROW_ICONHEIGHT);
                        }
                    }

                    if (layoutRequired)
                    {
                        LayoutWindow(true);
                    }
                }
                finally
                {
                    SetFlag(FlagNeedUpdateUIBasedOnFont, false);
                }
            }
        }

        private bool UnfocusSelection()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:UnfocusSelection()");
            GridEntry gridEntry = GetGridEntryFromRow(selectedRow);
            if (gridEntry is null)
            {
                return true;
            }

            bool commit = Commit();

            if (commit && FocusInside)
            {
                Focus();
            }
            return commit;
        }

        private void UpdateResetCommand(GridEntry gridEntry)
        {
            if (totalProps > 0)
            {
                IMenuCommandService mcs = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                if (mcs != null)
                {
                    MenuCommand reset = mcs.FindCommand(PropertyGridCommands.Reset);
                    if (reset != null)
                    {
                        reset.Enabled = gridEntry is null ? false : gridEntry.CanResetPropertyValue();
                    }
                }
            }
        }

        // a mini version of process dialog key
        // for responding to WM_GETDLGCODE
        internal bool WantsTab(bool forward)
        {
            if (forward)
            {
                if (Focused)
                {
                    // we want a tab if the grid has focus and
                    // we have a button or an Edit
                    if (DropDownButton.Visible ||
                        DialogButton.Visible ||
                        Edit.Visible)
                    {
                        return true;
                    }
                }
                else if (Edit.Focused && (DropDownButton.Visible || DialogButton.Visible))
                {
                    // if the Edit has focus, and we have a button, we want the tab as well
                    return true;
                }
                return OwnerGrid.WantsTab(forward);
            }
            else
            {
                if (Edit.Focused || DropDownButton.Focused || DialogButton.Focused)
                {
                    return true;
                }
                return OwnerGrid.WantsTab(forward);
            }
        }

        private unsafe bool WmNotify(ref Message m)
        {
            if (m.LParam != IntPtr.Zero)
            {
                User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;

                if (nmhdr->hwndFrom == ToolTip.Handle)
                {
                    switch ((ComCtl32.TTN)nmhdr->code)
                    {
                        case ComCtl32.TTN.POP:
                            break;
                        case ComCtl32.TTN.SHOW:
                            // we want to move the tooltip over where our text would be
                            Point mouseLoc = Cursor.Position;

                            // convert to window coords
                            mouseLoc = PointToClient(mouseLoc);

                            // figure out where we are and apply the offset
                            mouseLoc = FindPosition(mouseLoc.X, mouseLoc.Y);

                            if (mouseLoc == InvalidPosition)
                            {
                                break;
                            }

                            GridEntry curEntry = GetGridEntryFromRow(mouseLoc.Y);

                            if (curEntry is null)
                            {
                                break;
                            }

                            // get the proper rectangle
                            Rectangle itemRect = GetRectangle(mouseLoc.Y, mouseLoc.X);
                            Point tipPt = Point.Empty;

                            // and if we need a tooltip, move the tooltip control to that point.
                            if (mouseLoc.X == ROWLABEL)
                            {
                                tipPt = curEntry.GetLabelToolTipLocation(mouseLoc.X - itemRect.X, mouseLoc.Y - itemRect.Y);
                            }
                            else if (mouseLoc.X == ROWVALUE)
                            {
                                tipPt = curEntry.ValueToolTipLocation;
                            }
                            else
                            {
                                break;
                            }

                            if (tipPt != InvalidPoint)
                            {
                                itemRect.Offset(tipPt);
                                PositionTooltip(this, ToolTip, itemRect);
                                m.Result = (IntPtr)1;
                                return true;
                            }

                            break;
                    }
                }
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)User32.WM.SYSCOLORCHANGE:
                    Invalidate();
                    break;

                // Microsoft -- if we get focus in the error
                // state, make sure we push it back to the
                // Edit or bad bad things can happen with
                // our state...
                //
                case (int)User32.WM.SETFOCUS:
                    if (!GetInPropertySet() && Edit.Visible && (errorState != ERROR_NONE || !Commit()))
                    {
                        base.WndProc(ref m);
                        Edit.Focus();
                        return;
                    }
                    break;

                case (int)User32.WM.IME_STARTCOMPOSITION:
                    Edit.Focus();
                    Edit.Clear();
                    User32.PostMessageW(Edit, User32.WM.IME_STARTCOMPOSITION);
                    return;

                case (int)User32.WM.IME_COMPOSITION:
                    Edit.Focus();
                    User32.PostMessageW(Edit, User32.WM.IME_COMPOSITION, m.WParam, m.LParam);
                    return;

                case (int)User32.WM.GETDLGCODE:

                    int flags = (int)(User32.DLGC.WANTCHARS | User32.DLGC.WANTARROWS);

                    if (selectedGridEntry != null)
                    {
                        if ((ModifierKeys & Keys.Shift) == 0)
                        {
                            // if we're going backwards, we don't want the tab.
                            // otherwise, we only want it if we have an edit...
                            //
                            if (edit.Visible)
                            {
                                flags |= (int)User32.DLGC.WANTTAB;
                            }
                        }
                    }
                    m.Result = (IntPtr)(flags);
                    return;

                case (int)User32.WM.MOUSEMOVE:

                    // check if it's the same position, of so eat the message
                    if (unchecked((int)(long)m.LParam) == lastMouseMove)
                    {
                        return;
                    }
                    lastMouseMove = unchecked((int)(long)m.LParam);
                    break;

                case (int)User32.WM.NOTIFY:
                    if (WmNotify(ref m))
                    {
                        return;
                    }

                    break;
                case AutomationMessages.PGM_GETSELECTEDROW:
                    m.Result = (IntPtr)GetRowFromGridEntry(selectedGridEntry);
                    return;
                case AutomationMessages.PGM_GETVISIBLEROWCOUNT:
                    m.Result = (IntPtr)Math.Min(visibleRows, totalProps);
                    return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        ///  rescale constants for the DPI change
        /// </summary>
        /// <param name="deviceDpiOld"></param>
        /// <param name="deviceDpiNew"></param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            RescaleConstants();
        }

        /// <summary>
        ///  Rescale constants on this object
        /// </summary>
        private void RescaleConstants()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                cachedRowHeight = -1;
                paintWidth = LogicalToDeviceUnits(PAINT_WIDTH);
                paintIndent = LogicalToDeviceUnits(PAINT_INDENT);
                outlineSizeExplorerTreeStyle = LogicalToDeviceUnits(OUTLINE_SIZE_EXPLORER_TREE_STYLE);
                outlineSize = LogicalToDeviceUnits(OUTLINE_SIZE);
                maxListBoxHeight = LogicalToDeviceUnits(MAX_LISTBOX_HEIGHT);
                offset_2Units = LogicalToDeviceUnits(OFFSET_2PIXELS);
                if (topLevelGridEntries != null)
                {
                    foreach (GridEntry t in topLevelGridEntries)
                    {
                        ResetOutline(t);
                    }
                }
            }
        }

        /// <summary>
        ///  private method to recursively reset outlinerect for grid entries ( both visible and invisible)
        /// </summary>
        private void ResetOutline(GridEntry entry)
        {
            entry.OutlineRect = Rectangle.Empty;
            if (entry.ChildCount > 0)
            {
                foreach (GridEntry ent in entry.Children)
                {
                    ResetOutline(ent);
                }
            }
            return;
        }

        internal interface IMouseHookClient
        {
            // return true if the click is handled, false
            // to pass it on
            bool OnClickHooked();
        }

        internal class MouseHook
        {
            private readonly PropertyGridView gridView;
            private readonly Control control;
            private readonly IMouseHookClient client;

            internal uint _thisProcessID;
            private GCHandle _mouseHookRoot;
            private IntPtr _mouseHookHandle = IntPtr.Zero;
            private bool hookDisable;

            private bool processing;

            public MouseHook(Control control, IMouseHookClient client, PropertyGridView gridView)
            {
                this.control = control;
                this.gridView = gridView;
                this.client = client;
#if DEBUG
                callingStack = Environment.StackTrace;
#endif
            }

#if DEBUG
            readonly string callingStack;
            ~MouseHook()
            {
                Debug.Assert(_mouseHookHandle == IntPtr.Zero, "Finalizing an active mouse hook.  This will crash the process.  Calling stack: " + callingStack);
            }
#endif

            public bool DisableMouseHook
            {
                set
                {
                    hookDisable = value;
                    if (value)
                    {
                        UnhookMouse();
                    }
                }
            }

            public virtual bool HookMouseDown
            {
                get
                {
                    GC.KeepAlive(this);
                    return _mouseHookHandle != IntPtr.Zero;
                }
                set
                {
                    if (value && !hookDisable)
                    {
                        HookMouse();
                    }
                    else
                    {
                        UnhookMouse();
                    }
                }
            }

            public void Dispose()
            {
                UnhookMouse();
            }

            /// <summary>
            ///  Sets up the needed windows hooks to catch messages.
            /// </summary>
            private void HookMouse()
            {
                GC.KeepAlive(this);
                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (_mouseHookHandle != IntPtr.Zero)
                    {
                        return;
                    }

                    if (_thisProcessID == 0)
                    {
                        User32.GetWindowThreadProcessId(control, out _thisProcessID);
                    }

                    var hook = new User32.HOOKPROC(new MouseHookObject(this).Callback);
                    _mouseHookRoot = GCHandle.Alloc(hook);
                    _mouseHookHandle = User32.SetWindowsHookExW(
                        User32.WH.MOUSE,
                        hook,
                        IntPtr.Zero,
                        Kernel32.GetCurrentThreadId());
                    Debug.Assert(_mouseHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:HookMouse()");
                }
            }

            /// <summary>
            ///  HookProc used for catch mouse messages.
            /// </summary>
            private unsafe IntPtr MouseHookProc(User32.HC nCode, IntPtr wparam, IntPtr lparam)
            {
                GC.KeepAlive(this);
                if (nCode == User32.HC.ACTION)
                {
                    User32.MOUSEHOOKSTRUCT* mhs = (User32.MOUSEHOOKSTRUCT*)lparam;
                    if (mhs != null)
                    {
                        switch (unchecked((User32.WM)(long)wparam))
                        {
                            case User32.WM.LBUTTONDOWN:
                            case User32.WM.MBUTTONDOWN:
                            case User32.WM.RBUTTONDOWN:
                            case User32.WM.NCLBUTTONDOWN:
                            case User32.WM.NCMBUTTONDOWN:
                            case User32.WM.NCRBUTTONDOWN:
                            case User32.WM.MOUSEACTIVATE:
                                if (ProcessMouseDown(mhs->hWnd, mhs->pt.X, mhs->pt.Y))
                                {
                                    return (IntPtr)1;
                                }
                                break;
                        }
                    }
                }

                return User32.CallNextHookEx(new HandleRef(this, _mouseHookHandle), nCode, wparam, lparam);
            }

            /// <summary>
            ///  Removes the windowshook that was installed.
            /// </summary>
            private void UnhookMouse()
            {
                GC.KeepAlive(this);
                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (_mouseHookHandle != IntPtr.Zero)
                    {
                        User32.UnhookWindowsHookEx(new HandleRef(this, _mouseHookHandle));
                        _mouseHookRoot.Free();
                        _mouseHookHandle = IntPtr.Zero;
                        Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:UnhookMouse()");
                    }
                }
            }

            /*
           * Here is where we force validation on any clicks outside the
           */
            private bool ProcessMouseDown(IntPtr hWnd, int x, int y)
            {
                // If we put up the "invalid" message box, it appears this
                // method is getting called re-entrantly when it shouldn't be.
                // this prevents us from recursing.
                if (processing)
                {
                    return false;
                }

                IntPtr hWndAtPoint = hWnd;
                IntPtr handle = control.Handle;
                Control ctrlAtPoint = FromHandle(hWndAtPoint);

                // if it's us or one of our children, just process as normal
                if (hWndAtPoint != handle && !control.Contains(ctrlAtPoint))
                {
                    Debug.Assert(_thisProcessID != 0, "Didn't get our process id!");

                    // Make sure the window is in our process
                    User32.GetWindowThreadProcessId(hWndAtPoint, out uint pid);

                    // if this isn't our process, unhook the mouse.
                    if (pid != _thisProcessID)
                    {
                        HookMouseDown = false;
                        return false;
                    }

                    bool needCommit = false;

                    // if this a sibling control (e.g. the drop down or buttons), just forward the message and skip the commit
                    needCommit = ctrlAtPoint is null ? true : !gridView.IsSiblingControl(control, ctrlAtPoint);

                    try
                    {
                        processing = true;

                        if (needCommit)
                        {
                            if (client.OnClickHooked())
                            {
                                return true; // there was an error, so eat the mouse
                            }
                        }
                    }
                    finally
                    {
                        processing = false;
                    }

                    // cancel our hook at this point
                    HookMouseDown = false;
                    //gridView.UnfocusSelection();
                }
                return false;
            }

            /// <summary>
            ///  Forwards messageHook calls to ToolTip.messageHookProc
            /// </summary>
            private class MouseHookObject
            {
                internal WeakReference reference;

                public MouseHookObject(MouseHook parent)
                {
                    reference = new WeakReference(parent, false);
                }

                public virtual IntPtr Callback(User32.HC nCode, IntPtr wparam, IntPtr lparam)
                {
                    IntPtr ret = IntPtr.Zero;
                    try
                    {
                        MouseHook control = (MouseHook)reference.Target;
                        if (control != null)
                        {
                            ret = control.MouseHookProc(nCode, wparam, lparam);
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                    return ret;
                }
            }
        }

        /// <summary>
        ///  The accessible object class for a PropertyGridView. The child accessible objects
        ///  are accessible objects corresponding to the property grid entries.
        /// </summary>
        internal class PropertyGridViewAccessibleObject : ControlAccessibleObject
        {
            private readonly PropertyGridView _owningPropertyGridView;
            private readonly PropertyGrid _parentPropertyGrid;

            /// <summary>
            ///  Construct a PropertyGridViewAccessibleObject
            /// </summary>
            public PropertyGridViewAccessibleObject(PropertyGridView owner, PropertyGrid parentPropertyGrid) : base(owner)
            {
                _owningPropertyGridView = owner;
                _parentPropertyGrid = parentPropertyGrid;
            }

            /// <summary>
            ///  Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </summary>
            /// <param name="x">x coordinate of point to check</param>
            /// <param name="y">y coordinate of point to check</param>
            /// <returns>Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </returns>
            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
                => Owner.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (_parentPropertyGrid.IsHandleCreated &&
                    _parentPropertyGrid.AccessibilityObject is PropertyGridAccessibleObject propertyGridAccessibleObject)
                {
                    UiaCore.IRawElementProviderFragment navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                    if (navigationTarget != null)
                    {
                        return navigationTarget;
                    }
                }

                if (_owningPropertyGridView.OwnerGrid.SortedByCategories)
                {
                    switch (direction)
                    {
                        case UiaCore.NavigateDirection.FirstChild:
                            return GetFirstCategory();
                        case UiaCore.NavigateDirection.LastChild:
                            return GetLastCategory();
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case UiaCore.NavigateDirection.FirstChild:
                            return GetChild(0);
                        case UiaCore.NavigateDirection.LastChild:
                            int childCount = GetChildCount();
                            if (childCount > 0)
                            {
                                return GetChild(childCount - 1);
                            }

                            return null;
                    }
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owningPropertyGridView.OwnerGrid?.AccessibilityObject;
                }
            }

            /// <summary>
            ///  Gets the accessible object for the currently focused grid entry.
            /// </summary>
            /// <returns>The accessible object for the currently focused grid entry.</returns>
            internal override UiaCore.IRawElementProviderFragment GetFocus()
            {
                return GetFocused();
            }

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyID">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            internal override object GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TableControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.IsTablePatternAvailablePropertyId => true,
                    UiaCore.UIA.IsGridPatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.TablePatternId => true,
                    UiaCore.UIA.GridPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }

                    return string.Format(SR.PropertyGridDefaultAccessibleNameTemplate, _owningPropertyGridView?.OwnerGrid?.AccessibilityObject.Name);
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    else
                    {
                        return AccessibleRole.Table;
                    }
                }
            }

            public AccessibleObject Next(GridEntry current)
            {
                int row = ((PropertyGridView)Owner).GetRowFromGridEntry(current);
                GridEntry nextEntry = ((PropertyGridView)Owner).GetGridEntryFromRow(++row);
                if (nextEntry != null)
                {
                    return nextEntry.AccessibilityObject;
                }
                return null;
            }

            internal AccessibleObject GetCategory(int categoryIndex)
            {
                GridEntry[] targetEntries = new GridEntry[1];
                GridEntryCollection topLevelGridEntries = _owningPropertyGridView.TopLevelGridEntries;
                var topLevelGridEntriesCount = topLevelGridEntries.Count;
                if (topLevelGridEntriesCount > 0)
                {
                    GridItem targetEntry = topLevelGridEntries[categoryIndex];
                    if (targetEntry is CategoryGridEntry categoryGridEntry)
                    {
                        return categoryGridEntry.AccessibilityObject;
                    }
                }

                return null;
            }

            internal AccessibleObject GetFirstCategory()
            {
                return GetCategory(0);
            }

            internal AccessibleObject GetLastCategory()
            {
                GridEntryCollection topLevelGridEntries = _owningPropertyGridView.TopLevelGridEntries;
                var topLevelGridEntriesCount = topLevelGridEntries.Count;
                return GetCategory(topLevelGridEntries.Count - 1);
            }

            /// <summary>
            ///  Gets the previous grid entry accessibility object.
            /// </summary>
            /// <param name="currentGridEntry">The current grid entry.</param>
            /// <param name="gridEntryCollection">The grid entry collection.</param>
            /// <param name="currentGridEntryFound">Indicates whether the current grid entry is found.</param>
            /// <returns>The previous grid entry.</returns>
            internal AccessibleObject GetPreviousGridEntry(GridEntry currentGridEntry, GridEntryCollection gridEntryCollection, out bool currentGridEntryFound)
            {
                GridEntry previousGridEntry = null;
                currentGridEntryFound = false;

                foreach (GridEntry gridEntry in gridEntryCollection)
                {
                    if (currentGridEntry == gridEntry)
                    {
                        // Set to true to return the previous iterable element.
                        currentGridEntryFound = true;
                        if (previousGridEntry != null)
                        {
                            // In the current iteration return previous entry if the current entry == iterated grid entry.
                            return previousGridEntry.AccessibilityObject;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        previousGridEntry = gridEntry;
                        if (gridEntry.ChildCount > 0)
                        {
                            AccessibleObject foundChild = GetPreviousGridEntry(currentGridEntry, gridEntry.Children, out currentGridEntryFound);
                            if (foundChild != null)
                            {
                                // Return some down-level child if found.
                                return foundChild;
                            }
                            else if (currentGridEntryFound)
                            {
                                // If the passed current is found but there is no next near this current.
                                return null;
                            }
                        }
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the next grid entry.
            /// </summary>
            /// <param name="currentGridEntry">The current grid entry.</param>
            /// <param name="gridEntryCollection">The grid entry collection.</param>
            /// <param name="currentGridEntryFound">Indicates whether the current grid entry is found.</param>
            /// <returns>The next grid entry.</returns>
            internal AccessibleObject GetNextGridEntry(GridEntry currentGridEntry, GridEntryCollection gridEntryCollection, out bool currentGridEntryFound)
            {
                currentGridEntryFound = false;

                foreach (GridEntry gridEntry in gridEntryCollection)
                {
                    if (currentGridEntryFound)
                    {
                        // Return the next entry via IEnumerable.Next() if previous entry == passed current.
                        return gridEntry.AccessibilityObject;
                    }

                    if (currentGridEntry == gridEntry)
                    {
                        // Set to true to return the next iterable element. (see above)
                        currentGridEntryFound = true;
                    }
                    else if (gridEntry.ChildCount > 0)
                    {
                        AccessibleObject foundChild = GetNextGridEntry(currentGridEntry, gridEntry.Children, out currentGridEntryFound);
                        if (foundChild != null)
                        {
                            // Return some down-level child if found.
                            return foundChild;
                        }
                        else if (currentGridEntryFound)
                        {
                            // If the passed current is found but there is no next near this current.
                            return null;
                        }
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the first child property.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The first child property.</returns>
            internal AccessibleObject GetFirstChildProperty(CategoryGridEntry current)
            {
                if (current.ChildCount > 0)
                {
                    GridEntryCollection subGridEntry = current.Children;
                    if (subGridEntry != null && subGridEntry.Count > 0)
                    {
                        GridEntry[] targetEntries = new GridEntry[1];
                        try
                        {
                            _owningPropertyGridView.GetGridEntriesFromOutline(subGridEntry, 0, 0, targetEntries);
                        }
                        catch (Exception ex)
                        {
                            Debug.Fail(ex.ToString());
                        }

                        return targetEntries[0].AccessibilityObject;
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the last child property.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The last child property.</returns>
            internal AccessibleObject GetLastChildProperty(CategoryGridEntry current)
            {
                if (current.ChildCount > 0)
                {
                    GridEntryCollection subGridEntry = current.Children;
                    if (subGridEntry != null && subGridEntry.Count > 0)
                    {
                        GridEntry[] targetEntries = new GridEntry[1];
                        try
                        {
                            _owningPropertyGridView.GetGridEntriesFromOutline(subGridEntry, 0, subGridEntry.Count - 1, targetEntries);
                        }
                        catch (Exception ex)
                        {
                            Debug.Fail(ex.ToString());
                        }

                        return targetEntries[0].AccessibilityObject;
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the next category.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The next category.</returns>
            internal AccessibleObject GetNextCategory(CategoryGridEntry current)
            {
                int row = _owningPropertyGridView.GetRowFromGridEntry(current);

                GridEntry nextEntry;

                do
                {
                    nextEntry = _owningPropertyGridView.GetGridEntryFromRow(++row);
                    if (nextEntry is CategoryGridEntry)
                    {
                        return nextEntry.AccessibilityObject;
                    }
                }
                while (nextEntry != null);

                return null;
            }

            public AccessibleObject Previous(GridEntry current)
            {
                int row = ((PropertyGridView)Owner).GetRowFromGridEntry(current);
                GridEntry prevEntry = ((PropertyGridView)Owner).GetGridEntryFromRow(--row);
                if (prevEntry != null)
                {
                    return prevEntry.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  Gets the previous category.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The previous category.</returns>
            internal AccessibleObject GetPreviousCategory(CategoryGridEntry current)
            {
                int row = _owningPropertyGridView.GetRowFromGridEntry(current);

                GridEntry previousEntry;

                do
                {
                    previousEntry = _owningPropertyGridView.GetGridEntryFromRow(--row);
                    if (previousEntry is CategoryGridEntry)
                    {
                        return previousEntry.AccessibilityObject;
                    }
                }
                while (previousEntry != null);

                return null;
            }

            /// <summary>
            ///  Get the accessible child at the given index.
            ///  The accessible children of a PropertyGridView are accessible objects
            ///  corresponding to the property grid entries.
            /// </summary>
            public override AccessibleObject GetChild(int index)
            {
                GridEntryCollection properties = ((PropertyGridView)Owner).AccessibilityGetGridEntries();
                if (properties != null && index >= 0 && index < properties.Count)
                {
                    return properties.GetEntry(index).AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            ///  Get the number of accessible children.
            ///  The accessible children of a PropertyGridView are accessible objects
            ///  corresponding to the property grid entries.
            /// </summary>
            public override int GetChildCount()
            {
                GridEntryCollection properties = ((PropertyGridView)Owner).AccessibilityGetGridEntries();

                if (properties != null)
                {
                    return properties.Count;
                }
                else
                {
                    return 0;
                }
            }

            /// <summary>
            ///  Get the accessible object for the currently focused grid entry.
            /// </summary>
            public override AccessibleObject GetFocused()
            {
                GridEntry gridEntry = ((PropertyGridView)Owner).SelectedGridEntry;
                if (gridEntry != null && gridEntry.Focus)
                {
                    return gridEntry.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  Get the accessible object for the currently selected grid entry.
            /// </summary>
            public override AccessibleObject GetSelected()
            {
                GridEntry gridEntry = ((PropertyGridView)Owner).SelectedGridEntry;
                if (gridEntry != null)
                {
                    return gridEntry.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  Get the accessible child at the given screen location.
            ///  The accessible children of a PropertyGridView are accessible objects
            ///  corresponding to the property grid entries.
            /// </summary>
            public override AccessibleObject HitTest(int x, int y)
            {
                if (!Owner.IsHandleCreated)
                {
                    return null;
                }

                // Convert to client coordinates
                var pt = new Point(x, y);
                User32.ScreenToClient(new HandleRef(Owner, Owner.Handle), ref pt);

                // Find the grid entry at the given client coordinates
                //
                Point pos = ((PropertyGridView)Owner).FindPosition(pt.X, pt.Y);
                if (pos != PropertyGridView.InvalidPosition)
                {
                    GridEntry gridEntry = ((PropertyGridView)Owner).GetGridEntryFromRow(pos.Y);
                    if (gridEntry != null)
                    {
                        // Return the accessible object for this grid entry
                        //
                        return gridEntry.AccessibilityObject;
                    }
                }

                // No grid entry at this point
                //
                return null;
            }

            /// <summary>
            ///  Navigate to another object.
            /// </summary>
            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {
                if (GetChildCount() > 0)
                {
                    // We're only handling FirstChild and LastChild here
                    switch (navdir)
                    {
                        case AccessibleNavigation.FirstChild:
                            return GetChild(0);
                        case AccessibleNavigation.LastChild:
                            return GetChild(GetChildCount() - 1);
                    }
                }
                return null;    // Perform default behavior
            }

            internal override UiaCore.IRawElementProviderSimple GetItem(int row, int column)
            {
                return GetChild(row);
            }

            internal override int RowCount
            {
                get
                {
                    var topLevelGridEntries = _owningPropertyGridView.TopLevelGridEntries;
                    if (topLevelGridEntries is null)
                    {
                        return 0;
                    }

                    if (!_owningPropertyGridView.OwnerGrid.SortedByCategories)
                    {
                        return topLevelGridEntries.Count;
                    }

                    int categoriesCount = 0;
                    foreach (var topLevelGridEntry in topLevelGridEntries)
                    {
                        if (topLevelGridEntry is CategoryGridEntry)
                        {
                            categoriesCount++;
                        }
                    }

                    return categoriesCount;
                }
            }

            internal override int ColumnCount => 1; // There is one column: grid item represents both label and input.
        }

        internal class GridPositionData
        {
            readonly ArrayList expandedState;
            readonly GridEntryCollection selectedItemTree;
            readonly int itemRow;
            readonly int itemCount;

            public GridPositionData(PropertyGridView gridView)
            {
                selectedItemTree = gridView.GetGridEntryHierarchy(gridView.selectedGridEntry);
                expandedState = gridView.SaveHierarchyState(gridView.topLevelGridEntries);
                itemRow = gridView.selectedRow;
                itemCount = gridView.totalProps;
            }

            public GridEntry Restore(PropertyGridView gridView)
            {
                gridView.RestoreHierarchyState(expandedState);
                GridEntry entry = gridView.FindEquivalentGridEntry(selectedItemTree);

                if (entry != null)
                {
                    gridView.SelectGridEntry(entry, true);

                    int delta = gridView.selectedRow - itemRow;
                    if (delta != 0 && gridView.ScrollBar.Visible)
                    {
                        if (itemRow < gridView.visibleRows)
                        {
                            delta += gridView.GetScrollOffset();

                            if (delta < 0)
                            {
                                delta = 0;
                            }
                            else if (delta > gridView.ScrollBar.Maximum)
                            {
                                delta = gridView.ScrollBar.Maximum - 1;
                            }
                            gridView.SetScrollOffset(delta);
                        }
                    }
                }
                return entry;
            }
        }
    }
}
